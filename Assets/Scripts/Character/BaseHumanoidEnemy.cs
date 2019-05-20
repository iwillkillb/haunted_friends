using UnityEngine;
using System.Collections;

public class BaseHumanoidEnemy : BaseHumanoid {

	public Transform target;
	public float distanceMinDetectChaseObject = 2, distanceMaxDetectChaseObject = 100;

	//Computer's Input for auto motion.
	public float inputHorizontal = 0;	//-1 ~ 1, fuzzy
	public int inputVertical = 0, inputWeaponChange = 0, inputItem = 1;	//-1(Left), 0(Non), 1(Right), crispy
	public bool inputAttack = false;

	float targetDegree = 0;

	GroundCheck wc;

	protected override void Awake () {
		base.Awake ();

		wc = transform.FindChild ("Wall Check").GetComponent<GroundCheck> ();

		if (target == null && GameObject.FindObjectOfType<BasePlayer> () != null)
			target = GameObject.FindObjectOfType<BasePlayer> ().transform;
	}



	protected override void Start () {
		base.Start ();
	}



	protected override void OnEnable () {	//This function is Used by outer function.
		base.OnEnable ();

	}



	protected override void FixedUpdate () {
		base.FixedUpdate ();
	}



	// Update is called once per frame
	protected override void Update () {
		base.Update ();

		if (weaponKind >= Item.WeaponKind.Pistol)
			ChaseAngle ();

		if (weaponKind < Item.WeaponKind.Throwable) {
			if (inputItem <= 0)
				inputItem = 1;
		} else {
			if (inputItem >= 0)
				inputItem = 0;
			else {
				inputItem = 0;
				WeaponDrop (Vector3.right);
			}
		}

		if (inputItem > 0 && !getItem)
			getItem = true;
		else if (inputItem <= 0 && getItem) 
			getItem = false;





		if (target.gameObject.activeInHierarchy == false) {	//Player must lives.
			SetInputHorizontal (0);
			return;
		}





		if ((distanceMaxDetectChaseObject > Vector3.Distance (transform.position, target.position)) &&
			(distanceMinDetectChaseObject < Vector3.Distance (transform.position, target.position))) {	//Move
			SetInputHorizontal (1);
		} else
			SetInputHorizontal (0);





		SetInputVertical ();



		if (gc.grounded && inputVertical < 0) 
			Sit (true);
		else if (!gc.grounded || (gc.grounded && inputVertical >= 0)) 
			Sit (false);



		SetInputAttack ();

		if (weaponKind >= Item.WeaponKind.Throwable) {	//If you have anything weapon...

			if (inputAttack)
				UseItem ();

			if (weaponKind >= Item.WeaponKind.Pistol && selectedWeaponItem.ammoRemain > 0 && selectedWeaponItem.ammo <= 0 &&
				anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash != animHash_Pistol_Reload &&
				anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash != animHash_Rifle_Reload &&
				anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash != animHash_Shotgun_Reload) {
				Reload ();
			}
		}
	}





	protected override void ChaseAngle () {	//This codes find Mouse cursor. Player uses this, but Other use new codes.
		Vector3 mPosition, oPosition;
		float dx, dy;

		mPosition = target.position;
		oPosition = aimPivot.position;

		dx = mPosition.x - oPosition.x;
		dy = mPosition.y - oPosition.y;

		mouseDegree = Mathf.Atan2 (dy, dx) * Mathf.Rad2Deg;	//Angle from chracter's arm to mouse cursor.

		if (targetDegree < mouseDegree)
			targetDegree += Time.deltaTime * 10;
		if (targetDegree > mouseDegree)
			targetDegree -= Time.deltaTime * 10;

		Aim ();
	}


	//============================This character private function.==================================


	void SetInputHorizontal (int order) {
		if (order > 0) {			//Go to target
			if (transform.position.x < target.position.x - 0.5f) {
				if (inputHorizontal <= 0)
					inputHorizontal = 0.1f;
				else
					inputHorizontal *= 1.1f;
			} else if (transform.position.x > target.position.x + 0.5f) {
				if (inputHorizontal >= 0)
					inputHorizontal = -0.1f;
				else
					inputHorizontal *= 1.1f;
			}
		} else if (order < 0) {		//Back from target
			if (transform.position.x < target.position.x - 0.5f) {
				if (inputHorizontal >= 0)
					inputHorizontal = -0.1f;
				else
					inputHorizontal *= 1.1f;
			} else if (transform.position.x > target.position.x + 0.5f) {
				if (inputHorizontal <= 0)
					inputHorizontal = 0.1f;
				else
					inputHorizontal *= 1.1f;
			}
		} else if (order == 0) {	//Stop
			if (Mathf.Abs (inputHorizontal) > 0.1)
				inputHorizontal *= 0.9f;
			else
				inputHorizontal = 0;
		}

		//Clamping
		if (inputHorizontal > 1)
			inputHorizontal = 1;
		else if (inputHorizontal < -1)
			inputHorizontal = -1;
		
		Move (inputHorizontal);
	}

	void SetInputVertical () {
		if (!gc.grounded)
			return;

		if(Mathf.Sign(wc.direction.x) != Mathf.Sign(transform.localScale.x))
			wc.direction.x *= -1;

		if (wc.grounded)
			inputVertical = 1;
		else
			inputVertical = 0;

		//Clamping
		if (inputVertical > 1)
			inputVertical = 1;
		else if (inputVertical < -1)
			inputVertical = -1;
		
		if (inputVertical > 0)
			Jump ();
	}

	void SetInputAttack () {
		if (weaponKind < Item.WeaponKind.Throwable)
			return;
		else if (weaponKind < Item.WeaponKind.Pistol) {	//Sword
			if (Vector3.Distance (transform.position, target.position) > distanceMinDetectChaseObject) {
				inputAttack = false;
			} else {
				if (target.GetComponentInParent<BaseHumanoid> () != null && 
					((facingRight && transform.position.x < target.position.x) ||
						(!facingRight && transform.position.x > target.position.x)))
					inputAttack = true;
				else
					inputAttack = false;
			}
		} else {										//Gun			
			Vector3 rotV = Quaternion.Euler (0, 0, targetDegree) * Vector3.right;	//What Vector will rotate?
			RaycastHit2D aimRay = Physics2D.Raycast(shotPoint.position, rotV, 100f);

			if (aimRay.collider != null &&
				aimRay.collider.GetComponentInParent<BaseHumanoid> () != null && 
				aimRay.collider.GetComponentInParent<BaseHumanoid> ().tag == "Player") {
				inputAttack = true;
			} else {
				inputAttack = false;
			}
		}
	}
}
