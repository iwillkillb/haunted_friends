using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
	// This is super class of every weapon's class!

	public enum WeaponKind {Object, Expendable, Throwable, Knife, Sword, Pistol, Rifle, Shotgun};
	public WeaponKind weaponKind = WeaponKind.Object;
	// Object : Just decoration
	// Throwable : Character can throw this.
	// etc... : Melee weapon or Guns...

	[Header("Restoration")]
	public float plusHp;	//Restoration for Character.
	public float plusMp;

	[Header("Throwable only")]
	public float delayAfterThrow = 3f;
	public GameObject effectObjAfterThrow; // Something(Boom Effect etc...) is spawned after throwed.
	[System.NonSerialized]public int itemUserId = -1; // Who throw me?

	[Header("Weapons")]
	public float delay = 0.5f;
	public float damage = 100f;
	public float damageVarient = 0;
	public float weaponForce = 1000f;	//Physics Addforce
	public AudioClip soundUse; // This is used by gun's sound or character's melee attack animation.
	public AudioClip soundMelee; // This is used when character damaged by melee weapon.

	[Header("Guns only")]
	public float bulletMaxDistance;
	public int ammo, ammoOrigin, ammoRemain;
	public AudioClip soundReload;

	protected SpriteRenderer sr;
	protected TrailRenderer trailR;
	protected LineRenderer lineR;
	protected BaseHumanoid myBaseHumanoid;

	[System.NonSerialized] public Transform shotPoint;	//Where is the point spawned Bullet?
	[System.NonSerialized] public AudioSource audioSource;
	[System.NonSerialized] public bool active, isEquiped = false;	//Is this lied? or catched by Character?, , For Active
	[System.NonSerialized] public int spriteSortingLayerIdBackup = 0, spriteSortingOrderBackup = 0;

	protected virtual void Awake () {
		if(transform.FindChild ("Shot Point") != null)			//Gun -> Have "Shot Point"
			shotPoint = transform.FindChild ("Shot Point");
		else
			shotPoint = transform;

		sr = GetComponent<SpriteRenderer> ();
		trailR = GetComponentInChildren<TrailRenderer> ();
		lineR = GetComponentInChildren<LineRenderer> ();
		audioSource = GetComponent<AudioSource> ();

		spriteSortingLayerIdBackup = sr.sortingLayerID;
		spriteSortingOrderBackup = sr.sortingOrder;

		if (trailR != null)
			trailR.sortingLayerName = "Effect";
		if(lineR != null)
			lineR.sortingLayerName = "Effect";
	}



	protected virtual void OnEnable () {
		Dropped (Vector3.zero);
	}



	protected virtual void Update () {
		if (isEquiped && transform.localPosition != Vector3.zero)
			transform.localPosition = Vector3.zero;

		if (!isEquiped) {
			if (!active)
				Active = true;
			if (GetComponent<Collider2D> () == null && GetComponent<Collider2D> ().isTrigger == true)
				GetComponent<Collider2D> ().isTrigger = false;
			if (GetComponent<Rigidbody2D> () == null)
				gameObject.AddComponent<Rigidbody2D> ();
		}
	}


	void OnCollisionEnter2D(Collision2D col) {
		if (col.transform.tag == "Player" && !isEquiped) {
			myBaseHumanoid = col.transform.GetComponent<BaseHumanoid> ();	// Player have survived and input getting item.
			
			if (myBaseHumanoid == null || myBaseHumanoid.hp <= 0 || !myBaseHumanoid.getItem)
				return;
			
			Equip (myBaseHumanoid.toolF);
		}
	}











	//Because SetActive(false) Objects is excepted Transform[], I made custom Activating function.
	public virtual bool Active {
		get {
			return active;
		}
		set {
			active = value;
			sr.enabled = active;
			GetComponent<Collider2D> ().enabled = active;
		}
	}



	void Equip (Transform parent) {	//<---> "BaseHumanoid.cs".WeaponDrop().
		transform.parent = parent;	// -> Give me to player!
		isEquiped = true;
		transform.localPosition = Vector3.zero;	//Position in Character's Hand.
		transform.localScale = new Vector3 (
			Mathf.Abs (transform.localScale.x), 
			Mathf.Abs (transform.localScale.y), 
			Mathf.Abs (transform.localScale.z)
		);

		GetComponent<Collider2D> ().isTrigger = true;
		
		if(GetComponent<Rigidbody2D> () != null)	//Delete Rigidbody2D Component.
			Destroy (GetComponent<Rigidbody2D> ());

		if (weaponKind >= WeaponKind.Pistol) {	//Am I Gun?
			transform.localEulerAngles = new Vector3 (0, 0, -90);	//Because Every gun sprite is Horizontal...
			myBaseHumanoid.shotPoint = shotPoint;					//Setting Character's Shot Point 
		} else
			transform.localEulerAngles = Vector3.zero;				//Not gun -> Sprite angle is all zero.

		//Sprite sorting layer Setting.
		if (sr != null && parent.GetComponent<SpriteRenderer> () != null) {
			spriteSortingLayerIdBackup = sr.sortingLayerID;
			spriteSortingOrderBackup = sr.sortingOrder;
			sr.sortingLayerID = parent.GetComponent<SpriteRenderer> ().sortingLayerID;
			sr.sortingOrder = parent.GetComponent<SpriteRenderer> ().sortingOrder;
		}

		myBaseHumanoid.WeaponChange (parent.childCount - 1);	//Most recent weapon's index is Last of weapon list.

		// Get user's id.
		if (myBaseHumanoid.gameObject.GetComponent<PhotonView> () != null)
			itemUserId = myBaseHumanoid.gameObject.GetComponent<PhotonView> ().viewID;
	}



	public void Dropped (Vector3 start) {
		transform.parent = null;
		isEquiped = false;
		Active = true;
		GetComponent<Collider2D> ().isTrigger = false;	//Abandoned weapon -> Physics object.

		start.x *= transform.localScale.x;				//Throw weapon.
		transform.localPosition += start;

		sr.sortingLayerID = spriteSortingLayerIdBackup;
		sr.sortingOrder = spriteSortingOrderBackup;

		if(GetComponent<Rigidbody2D>() == null)	//Add Rigidbody2D
			gameObject.AddComponent<Rigidbody2D> ();
		Rigidbody2D dropItemRigi = GetComponent<Rigidbody2D> ();
		dropItemRigi.AddTorque (start.x * -50);
		dropItemRigi.AddForce (new Vector2 (start.x, start.y) * 500);

		// delete user's id.
		itemUserId = -1;
	}



	public void Use () {
		switch (weaponKind) {
		case WeaponKind.Expendable:
			myBaseHumanoid.hp += plusHp;	//Restoration
			myBaseHumanoid.mp += plusMp;

			if (myBaseHumanoid.GetComponent<PlayerNetwork> () != null) {
				myBaseHumanoid.GetComponent<PhotonView> ().RPC ("ChangeHp", PhotonTargets.Others, myBaseHumanoid.hp);
				myBaseHumanoid.GetComponent<PhotonView> ().RPC ("ChangeMp", PhotonTargets.Others, myBaseHumanoid.mp);
			}

			gameObject.SetActive (false);
			myBaseHumanoid.WeaponDrop (Vector3.right);	//Item use -> That item is abandoned.
			break;

		case WeaponKind.Throwable:
			if(!IsInvoking())
				Invoke ("AfterThrow", delayAfterThrow);
			myBaseHumanoid.WeaponDrop (Vector3.right);	//Item use -> That item is abandoned.
			break;
		}
	}
	protected virtual void AfterThrow () {
		//Spawn Boom effect. -> Use object pool.
		if (effectObjAfterThrow != null)
			ObjectPooling.instance.MakeEffectBoom (transform.position, transform.rotation, itemUserId);
		/*
		if (effectObjAfterThrow != null)
			ObjectPooling.instance.MakeFind (effectObjAfterThrow.name, transform.position, transform.rotation);
		*/
		CancelInvoke ();
		gameObject.SetActive (false);
	}






	public virtual void Shot (LayerMask attackLayerMask, LayerMask blockLayerMask) {	//Used by Character.
		ammo--;

		float weaponEZ = transform.parent.eulerAngles.z - 90;
		if (weaponEZ < 0)
			weaponEZ += 360;

		Vector3 rotV = Quaternion.Euler (0, 0, weaponEZ) * Vector3.right;	//What Vector will rotate?
		RaycastHit2D aimRay = Physics2D.Raycast (shotPoint.position, rotV, bulletMaxDistance, attackLayerMask);

		transform.GetComponentInChildren<ParticleSystem> ().Play ();	//Gunfire effect
		audioSource.PlayOneShot (soundUse);								//Sound effect

		if (aimRay.collider != null) {

			//Line Renderer
			RaycastHit2D lineRay = Physics2D.Raycast (shotPoint.position, rotV, bulletMaxDistance, blockLayerMask);
			if(lineRay.collider == null)
				LineEffectRenderer (true);										//There is enemy but no wall -> Penetrate shot
			else if(lineRay.collider.GetComponentInParent<BaseCharacter> () != myBaseHumanoid.GetComponentInParent<BaseCharacter> ())
				LineEffectRenderer (true, shotPoint.position, lineRay.point);	//There is wall -> Blocked shot
			else
				LineEffectRenderer (true);

			ObjectPooling.instance.MakeFind ("Effect Hit", aimRay.point, new Quaternion (0, 0, 0, 0));	//Damaged point effect

			BaseCharacter colBaseChar = aimRay.collider.GetComponentInParent<BaseCharacter> ();

			if (colBaseChar == myBaseHumanoid.GetComponent<BaseCharacter> ())	//Don't shot myself!
				return;
			
			if (aimRay.rigidbody != null)		//Give physics force
				aimRay.rigidbody.AddForceAtPosition (rotV * weaponForce, aimRay.point);

			if (colBaseChar != null && colBaseChar != GetComponent<BaseCharacter> ()) {	//Give damage to enemy, Don't shoot myself.
				
				float giveDamage = damage;

				switch (aimRay.collider.name) {	//What is damaged body part?
				case "Head":	//HEADSHOT!!
					giveDamage *= 3;
					break;

				case "ArmFL":
				case "ArmBL":
					giveDamage *= 0.66f;
					break;
				case "HandF":
				case "HandB":
					giveDamage *= 0.33f;
					break;

				case "LegFL":
				case "LegBL":
					giveDamage *= 0.66f;
					break;
				case "FootF":
				case "FootB":
					giveDamage *= 0.33f;
					break;
				}
				giveDamage += Random.Range (0, damageVarient);
				colBaseChar.hp -= giveDamage;

				if (colBaseChar.hp <= 0 && myBaseHumanoid.youAlreadyKilledThisChar != colBaseChar) {	//If you killed this...
					myBaseHumanoid.score++;
					myBaseHumanoid.youAlreadyKilledThisChar = colBaseChar;
				}
			}
		} else {
			LineEffectRenderer (true);	//Can't find target -> Draw line without goal point.
		}
	}

	public void Reload () {
		int ammoReloading;	//Don't waste ammo!

		if (ammoRemain >= ammoOrigin) {	//Full magazine
			ammoReloading = ammoOrigin - ammo;
		} else {						//No full
			if (ammoOrigin - ammo > ammoRemain) {
				ammoReloading = ammoRemain;
			} else {
				ammoReloading = ammoOrigin - ammo;
			}
		}
		ammoRemain -= ammoReloading;
		ammo += ammoReloading;
	}



	// For only melee weapon.
	public void TrailEffectRenderer (bool active) {
		if (trailR == null)
			return;
		
		if(!active)
			trailR.Clear ();
		trailR.enabled = active;
	}

	// For only guns.
	public void LineEffectRenderer (bool active) {
		ObjectPooling.instance.MakeEffectGunLine
			(shotPoint.position, shotPoint.position + transform.TransformDirection(Vector3.right * bulletMaxDistance));
		/*
		if (lineR == null)
			return;

		lineR.enabled = active;
		if (active) {
			if (!lineR.useWorldSpace)
				lineR.useWorldSpace = true;
			lineR.SetPosition (0, shotPoint.position);
			lineR.SetPosition (1, shotPoint.position + transform.TransformDirection(Vector3.right * bulletMaxDistance));
			Invoke ("DeleteLineEffect", 0.05f);
		}*/
	}
	public void LineEffectRenderer (bool active, Vector3 start, Vector3 end) {
		ObjectPooling.instance.MakeEffectGunLine (start, end);
		/*
		if (lineR == null)
			return;

		lineR.enabled = active;
		if (active) {
			if (!lineR.useWorldSpace)
				lineR.useWorldSpace = true;
			lineR.SetPosition (0, start);
			lineR.SetPosition (1, end);
			Invoke ("DeleteLineEffect", 0.05f);
		}*/
	}
	/*
	public void DeleteLineEffect () {	//It is Used by LineEffectRenderer's Invoke.
		foreach (LineRenderer lr in GetComponentsInChildren<LineRenderer>()) {
			lr.enabled = false;
		}
	}
	*/
}
