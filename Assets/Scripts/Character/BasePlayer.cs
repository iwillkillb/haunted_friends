using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasePlayer : BaseHumanoid {

	ParticleSystem effect;
	PhotonView pv;

	[Header("Others...")]
	public AudioClip soundJump;
	protected int jumpCount = 0;	//Can you Double Jump?
	protected float gravityBackup, massBackup;



	protected override void Awake () {
		base.Awake ();

		effect = transform.FindChild ("Effect").GetComponent<ParticleSystem> ();
		pv = GetComponent<PhotonView> ();

		gravityBackup = rigi.gravityScale;
		massBackup = rigi.mass;
	}

	protected override void Start () {
		base.Start ();
	}

	protected override void OnEnable () {
		base.OnEnable ();

		rigi.gravityScale = gravityBackup;
	}

	protected override void FixedUpdate () {
		base.FixedUpdate ();
	}

	protected override void Update () {
		base.Update ();

		// If you died, Don't take input.
		if (isDied)
			return;

		// You can't control other user object.
		if (!pv.isMine)
			return;

		//Weapon select(Left/Right key)
		if (Input.GetButtonDown ("Weapon Select"))
			WeaponSelect (Input.GetAxisRaw ("Weapon Select"));

		//Weapon select(Number key), Alpha number key's start is 1 -> First weapon(index 0) match to Key 1.
		if (Input.GetKeyDown (KeyCode.Alpha0))
			WeaponSelectAtNumber (9);
		else if (Input.GetKeyDown (KeyCode.Alpha1))
			WeaponSelectAtNumber (0);
		else if (Input.GetKeyDown (KeyCode.Alpha2))
			WeaponSelectAtNumber (1);
		else if (Input.GetKeyDown (KeyCode.Alpha3))
			WeaponSelectAtNumber (2);
		else if (Input.GetKeyDown (KeyCode.Alpha4))
			WeaponSelectAtNumber (3);
		else if (Input.GetKeyDown (KeyCode.Alpha5))
			WeaponSelectAtNumber (4);
		else if (Input.GetKeyDown (KeyCode.Alpha6))
			WeaponSelectAtNumber (5);
		else if (Input.GetKeyDown (KeyCode.Alpha7))
			WeaponSelectAtNumber (6);
		else if (Input.GetKeyDown (KeyCode.Alpha8))
			WeaponSelectAtNumber (7);
		else if (Input.GetKeyDown (KeyCode.Alpha9))
			WeaponSelectAtNumber (8);
		


		Move (Input.GetAxis ("Horizontal"));

		Jump ();

		//Sit
		if (gc.grounded && Input.GetAxisRaw ("Vertical") < 0) 
			Sit (true);
		else if (!gc.grounded || (gc.grounded && Input.GetAxisRaw ("Vertical") >= 0)) 
			Sit (false);

		//Getting item
		if (Input.GetAxisRaw("Item") > 0 && !getItem) 
			getItem = true;
		if (Input.GetAxisRaw("Item") <= 0 && getItem) 
			getItem = false;

		//Drop item
		if (Input.GetAxisRaw ("Item") < 0)
			WeaponDrop (Vector3.right);

		//If you have Gun, You can 360 degree aim.
		if (weaponKind >= Item.WeaponKind.Pistol)
			ChaseAngle ();

		//You need delay time to attack action.
		if (Input.GetAxisRaw("Attack") > 0)
			UseItem ();

		//Reload
		if (Input.GetButtonDown ("Reload") && weaponKind >= Item.WeaponKind.Pistol &&
			selectedWeaponItem.ammoRemain > 0 && selectedWeaponItem.ammo < selectedWeaponItem.ammoOrigin &&
			anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash != animHash_Pistol_Reload &&
			anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash != animHash_Rifle_Reload &&
			anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash != animHash_Shotgun_Reload)
			Reload ();

	}



	protected override void Die () {
		base.Die ();

		// Display die message.
		if (pv.isMine)
			GameManager.instance.PrintLogMsg ("[" + PhotonNetwork.player.NickName + "] is down.", "#ff0000");
	}



	protected void WeaponSelectAtNumber (int num) {
		if (num < -1 || num >= toolF.childCount)
			num = -1;

		WeaponChange (num);
	}



	protected override void Jump () {
		if (gc.grounded && jumpCount != 0)
			jumpCount = 0;	//Grounded, Not jump -> 0
		if (jumpCount == 0 && !gc.grounded)	//End of First Input -> You can Second Input
			jumpCount = 1;	//Grounded -> jump -> 1
		if (Input.GetButtonDown ("Vertical") && Input.GetAxisRaw ("Vertical") > 0) {	//Double jump
			if (jumpCount < 1) {
				if (gc.grounded && !sit) {	//First Input, Key:Up
					rigi.velocity = new Vector2 (rigi.velocity.x, jumpPower);
				}
			}
		}
	}



	public void EffectSet (int order) {
		if (effect == null)
			return;

		switch (order) {
		case 0:
			effect.Play ();
			break;
		case 1:
			effect.Pause ();
			break;
		case 2:
			effect.Stop ();
			break;

		case 3:
			effect.loop = true;
			break;
		case 4:
			effect.loop = false;
			break;
		}

	}



	protected override void ChaseAngle () {	//This codes find Mouse cursor. Player uses this, but Other use new codes.
		Vector3 mPosition, oPosition, target;
		float dx, dy;

		mPosition = Input.mousePosition;
		oPosition = aimPivot.position;
		mPosition.z = oPosition.z - CameraFollow.instance.transform.position.z;

		target = CameraFollow.instance.followCamera.ScreenToWorldPoint (mPosition);

		dx = target.x - oPosition.x;
		dy = target.y - oPosition.y;
		mouseDegree = Mathf.Atan2 (dy, dx) * Mathf.Rad2Deg;	//Angle from chracter's arm to mouse cursor.

		Aim ();
	}
}
