using UnityEngine;
using System.Collections;

public class BaseHumanoid : BaseCharacter {	//Both Player and NPC used this code.

	protected bool backStep = false;	//When you equip gun, If your anim sight and character's eye sight is opposite, this is true.
	protected bool sit = false;		//True -> Sit / False -> Stand
	public float sitSpeedMul = 0.2f;	//When you move with sitting action, this value multiple to speed value.

	[System.NonSerialized]public bool getItem = false;	//If this is false, This character can't gets Item. this is such as anim's trigger.

	//Hands, These need to equip weapon... (Their "Transform", "Sprite Renderer" Components is used by "Item.cs". Don't Delete them.)
	[System.NonSerialized]public Transform pelvis, head, toolF, toolB;	//Weapon's parent object.

	//Weapon select
	[System.NonSerialized]public Item.WeaponKind weaponKind = Item.WeaponKind.Object;
	[System.NonSerialized]public int selectedWeaponIndex = -1;	//Array index of weapon's parent. If this is -1, You equip nothing.
	[System.NonSerialized]public Item selectedWeaponItem;		//Currently equiped weapon's Item component.

	//Hit, Meele attack
	protected int meeleAttackYDirection = 1;	//This use backup value, "selectedWeaponItem.weaponForce".
	protected bool youHitAlready = false;		//Weapon Trigger activates only one time by one motion.
	bool giveMeeleAttackDamage = false;			//When you meele attack, If this is true then you can give damage when enemy contact your knife of sword collider.

	[Header("Shot, Remote attack")]
	public LayerMask attackLayerMask;	//You can attack objects in these layers.
	public LayerMask BlockLayerMask;
	[System.NonSerialized]public Transform shotPoint;	//Where is bullet spawn point?
	protected float mouseDegree;	//mouse's Euler angle Z.
	protected Transform aimPivot;	//The pivot's position and rotation angle to mouse cursor.

	//Animator Hash
	protected readonly static int animHash_Knife_Slash = Animator.StringToHash ("Upper.Knife Slash");
	protected readonly static int animHash_Sword_Slash = Animator.StringToHash ("Upper.Sword Slash");
	protected readonly static int animHash_Pistol_Shot = Animator.StringToHash ("Upper.Pistol Shot");
	protected readonly static int animHash_Pistol_Reload = Animator.StringToHash ("Upper.Pistol Reload");
	protected readonly static int animHash_Rifle_Shot = Animator.StringToHash ("Upper.Rifle Shot");
	protected readonly static int animHash_Rifle_Reload = Animator.StringToHash ("Upper.Rifle Reload");
	protected readonly static int animHash_Shotgun_Shot = Animator.StringToHash ("Upper.Shotgun Shot");
	protected readonly static int animHash_Shotgun_Reload = Animator.StringToHash ("Upper.Shotgun Reload");



	protected override void Awake () {
		base.Awake ();

		pelvis = transform.FindChild ("Pelvis");
		head = pelvis.transform.FindChild ("Chest").FindChild ("Head");
		toolF = pelvis.FindChild ("Chest").FindChild ("ArmFU").
			FindChild ("ArmFL").FindChild ("HandF").FindChild ("ToolF");
		toolB = pelvis.FindChild ("Chest").FindChild ("ArmBU").
			FindChild ("ArmBL").FindChild ("HandB").FindChild ("ToolB");
		aimPivot = transform.FindChild ("Aim Pivot");

	}



	protected override void Start () {
		base.Start ();
	}



	protected override void OnEnable () {	//This function is Used by outer function.
		base.OnEnable ();

		selectedWeaponIndex = -1;
		WeaponKindAnimParameterChange (Item.WeaponKind.Object);

	}



	protected override void Update () {
		base.Update ();
	}



	protected override void FixedUpdate () {
		base.FixedUpdate ();

		if (gameObject.activeInHierarchy) {	//Bug fix "Animation has not been initialized"
			if (anim.GetBool ("grounded") != gc.grounded)
				anim.SetBool ("grounded", gc.grounded);
		}

		if (delayAttack < delayAttackOrigin)
			delayAttack += Time.deltaTime;

		if (youHitAlready && !giveMeeleAttackDamage)
			youHitAlready = false;

		if (sit && mp < mpOrigin)
			mp += Time.deltaTime * 2;

	}




	protected void Move (float input) {	//Character's walking or running. input : Horizontal, Float in -1 ~ 1

		if (weaponKind >= Item.WeaponKind.Pistol) {	//Gun -> You can backstep.
			if ((input > 0 && !facingRight) || (input < 0 && facingRight)) {	//There is no Flip().
				if (!backStep)
					backStep = true;
			} else if ((input > 0 && facingRight) || (input < 0 && !facingRight) || input == 0) {	//Same direction
				if (backStep)
					backStep = false;
			}
		} else {																		//Not gun -> You can't backstep.
			if ((input > 0 && !facingRight) || (input < 0 && facingRight))	//Input direction == Character's direction
				Flip ();
			if (backStep)
				backStep = false;
		}

		if (anim.GetBool ("backStep") == !backStep)	//Setting Animation parameters.
			anim.SetBool ("backStep", backStep);
		anim.SetFloat ("move", Mathf.Abs(input));

		if (sit)
			input *= sitSpeedMul;
		
		rigi.velocity = new Vector2 (input * speed, rigi.velocity.y);	//Physics moving.
		transform.eulerAngles = new Vector3(0, 0, input * -20);
	}





	protected void Sit (bool s) {
		if (s != sit)
			sit = s;
		else
			return;

		if (anim.GetBool ("sit") != sit)
			anim.SetBool ("sit", sit);
	}




	protected override void Die () {
		base.Die ();

		if (selectedWeaponItem != null) {	//Weapon Effect OFF
			selectedWeaponItem.TrailEffectRenderer (false);
			// selectedWeaponItem.DeleteLineEffect ();
		}

		//weapons drop!
		int dropRepeat = toolF.childCount;
		for (int a = 0; a < dropRepeat; a++) {
			WeaponChange (a);
			WeaponDrop (Vector3.right);
		}

		MakeToRagdoll ();

		//gameObject.SetActive (false);
	}
	protected void MakeToRagdoll () {
		anim.Stop ();

		Collider2D[] cols = GetComponentsInChildren<Collider2D> ();
		foreach (Collider2D col in cols) {
			// Make rigidbody
			col.gameObject.AddComponent<Rigidbody2D> ();

			// Make hingejoint
			HingeJoint2D colJoint = col.gameObject.AddComponent<HingeJoint2D> ();
			colJoint.connectedBody = col.transform.parent.GetComponent<Rigidbody2D> ();
			colJoint.useLimits = true;
			// Set angle limit.
			JointAngleLimits2D jointAngleLimit = new JointAngleLimits2D ();
			switch (col.name) {
			case "Head":
			case "Chest":
			case "Tail":
				jointAngleLimit.min = -30;
				jointAngleLimit.max = 30;
				break;

			case "LegFU":
			case "LegBU":
				jointAngleLimit.min = -140;
				jointAngleLimit.max = 40;
				break;
			case "LegFL":
			case "LegBL":
				jointAngleLimit.min = -5;
				jointAngleLimit.max = 160;
				break;
			case "FootF":
			case "FootB":
				jointAngleLimit.min = -20;
				jointAngleLimit.max = 20;
				break;

			case "ArmFU":
			case "ArmBU":
				jointAngleLimit.min = -225;
				jointAngleLimit.max = 135;
				break;
			case "ArmFL":
			case "ArmBL":
				jointAngleLimit.min = -180;
				jointAngleLimit.max = 5;
				break;
			case "HandF":
			case "HandB":
				jointAngleLimit.min = -10;
				jointAngleLimit.max = 10;
				break;
			}
			if (!facingRight) {	// Left direction -> Flip joint angle.
				float tempMax, tempMin;
				tempMin = jointAngleLimit.min;
				tempMax = jointAngleLimit.max;
				jointAngleLimit.min = -tempMax;
				jointAngleLimit.max = -tempMin;
			}
			colJoint.limits = jointAngleLimit;
		}
	}





	public void WeaponDrop (Vector3 start) {	//This is used by Die (), Player's input, and Item.cs...
		if (selectedWeaponItem == null)
			return;

		selectedWeaponItem.Dropped (start);		//Activate drop function in current weapon.
		WeaponChange (-1);						//Drop weapon -> No weapon
		selectedWeaponItem = null;
	}




	protected void WeaponSelect (float input) {
		//selectedWeaponIndex : [-1] ~ [toolF.childCount - 1] Cycle.
		//If this is -1, You don't select any weapon.
		//Take input weapon change button. selectedWeaponIndex is cycle value.

		int setIndex = selectedWeaponIndex;

		if (input < 0) {		//Left key
			if (setIndex > -1)
				setIndex--;
			else
				setIndex = toolF.childCount - 1;

		} else if (input > 0) {	//Right key
			if (setIndex < toolF.childCount - 1)
				setIndex++;
			else
				setIndex = -1;
		}

		WeaponChange (setIndex);
	}







	public void WeaponChange (int index) {	//This is used in "Here"(Player change their weapon) and "Item.cs"(Getting weapon item)...
		selectedWeaponIndex = index;

		if (selectedWeaponItem != null) {	//Delete weapon's effect.
			selectedWeaponItem.TrailEffectRenderer(false);
			// selectedWeaponItem.DeleteLineEffect ();
		}

		Item[] weapons = toolF.GetComponentsInChildren<Item> ();

		if (index < 0) {	//No select any weapon -> All weapon OFF.
			foreach (Item weapon in weapons) {
				if (weapon.Active)
					weapon.Active = false;
			}
			selectedWeaponItem = null;
			WeaponKindAnimParameterChange (Item.WeaponKind.Object);		//Animation : No weapon
		} else {			//You select a weapon -> That weapon ON only.
			foreach (Item weapon in weapons) {
				if (index >= weapons.Length)		//Index clamping.
					index = weapons.Length - 1;
				else if (index < 0)
					index = 0;
				
				if (weapons [index] == weapon) {	//If I find selected weapon in toolF's children array...

					selectedWeaponItem = weapon;									//Weapon Select!!
					selectedWeaponItem.Active = true;							//That weapon ON!
					WeaponKindAnimParameterChange (selectedWeaponItem.weaponKind);	//Animation setting.
					shotPoint = selectedWeaponItem.shotPoint;						//Bullet start point setting.
					delayAttackOrigin = selectedWeaponItem.delay;					//Attack delay setting.
					delayAttack = 0;												//Attack delay reset.
				} else {
					weapon.Active = false;										//Other weapons OFF...
				}
			}
		}
	}
	void WeaponKindAnimParameterChange (Item.WeaponKind newKind) {	//Change weaponKind value and animation.
		weaponKind = newKind;
		if (gameObject.activeInHierarchy) {	//Bug fix "Animation has not been initialized"
			anim.SetInteger ("weaponKind", (int)newKind - 2);
		}
	}















	protected void UseItem () {
		if (selectedWeaponItem == null)
			return;

		if (delayAttack >= delayAttackOrigin) {
			delayAttack = 0;
			if (weaponKind >= Item.WeaponKind.Pistol) {			//Remote attack : You can't shot when reloading or zero ammo.
				if (selectedWeaponItem.ammo > 0 && (
				        anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash != animHash_Pistol_Reload &&
				        anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash != animHash_Rifle_Reload &&
				        anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash != animHash_Shotgun_Reload
				)) {
					selectedWeaponItem.Shot (attackLayerMask, BlockLayerMask);
					if (selectedWeaponItem.GetComponent<RocketLauncher> () == null) {	//Rocket Launcher -> No Shot Motion.
						switch ((int)weaponKind) {
						case (int)Item.WeaponKind.Pistol:
							anim.CrossFade ("Pistol Shot", 0.1f, -1, 0);
							break;

						case (int)Item.WeaponKind.Rifle:
							anim.CrossFade ("Rifle Shot", 0.1f, -1, 0);
							break;

						case (int)Item.WeaponKind.Shotgun:
							anim.CrossFade ("Shotgun Shot", 0.1f, -1, 0);
							break;
						}
					}
				}

			} else if (weaponKind >= Item.WeaponKind.Knife) {	//Meele attack : This just play animation.
				MeeleAttackAnimation ();						//Damage calculation is in OnTriggerEnter.
			} else if (weaponKind >= Item.WeaponKind.Throwable) {	//Other itmes...
				anim.CrossFade ("Knife Slash", 0.1f);
			} else {
				anim.CrossFade ("Lift up", 0.1f);
			}
		}
	}

	void MeeleAttackAnimation () {
		switch ((int)weaponKind) {

		case (int)Item.WeaponKind.Knife:
			if (anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash != animHash_Knife_Slash) {
				meeleAttackYDirection = 0;
				anim.CrossFade ("Knife Slash", 0.1f);
			}
			break;

		case (int)Item.WeaponKind.Sword:
			if (anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash != animHash_Sword_Slash) {
				meeleAttackYDirection = -1;
				anim.CrossFade ("Sword Slash", 0.1f);
			}
			break;
		}
	}



	//When meele attack, Your weapon contacts enemy's collider... (If col's layer is in Attack layer mask.)
	protected void OnTriggerEnter2D (Collider2D col) {
		if (giveMeeleAttackDamage && ((1 << col.gameObject.layer) & attackLayerMask) != 0) {

			if (weaponKind < Item.WeaponKind.Knife || weaponKind >= Item.WeaponKind.Pistol)
				return;	//You need meele weapon.

			if (anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash != animHash_Knife_Slash &&
			    anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash != animHash_Sword_Slash)
				return;	//Can't give damage when your motion is attack.

			//Give physics force
			if (col.attachedRigidbody != null && selectedWeaponItem != null) {
				Vector3 giveForce = Quaternion.Euler (0, 0, selectedWeaponItem.transform.rotation.z) * Vector3.right;
				col.attachedRigidbody.AddRelativeForce (giveForce * selectedWeaponItem.weaponForce);
				col.attachedRigidbody.AddTorque (selectedWeaponItem.weaponForce * Mathf.Sign(transform.localScale.x));
			}

			BaseCharacter colBaseChar = col.GetComponentInParent<BaseCharacter> ();
			//Give damage to enemy
			if (colBaseChar != null) {
				float damage = selectedWeaponItem.damage;
				switch (col.name) {	//What is damaged body part?
				case "ArmFU":
				case "ArmBU":
					damage *= 0.3f;
					break;
				case "ArmFL":
				case "ArmBL":
					damage *= 0.2f;
					break;
				case "HandF":
				case "HandB":
					damage *= 0.1f;
					break;

				case "LegFU":
				case "LegBU":
					damage *= 0.3f;
					break;
				case "LegFL":
				case "LegBL":
					damage *= 0.2f;
					break;
				case "FootF":
				case "FootB":
					damage *= 0.1f;
					break;
				}
				damage += Random.Range (0, selectedWeaponItem.damageVarient);
				colBaseChar.hp -= selectedWeaponItem.damage + damage;

				if (colBaseChar.hp <= 0 && youAlreadyKilledThisChar != colBaseChar) {	//If you killed this...
					score++;
					youAlreadyKilledThisChar = colBaseChar;
				}

				if (!youHitAlready) {
					youHitAlready = true;
					selectedWeaponItem.audioSource.PlayOneShot (selectedWeaponItem.soundMelee);
				}
			}
			

		}
	}




	protected virtual void ChaseAngle () {	//This function is defined by child's code.
		
	}
	protected void Aim () {	//When you have gun...
		float pelvisEZ = pelvis.eulerAngles.z;				//Take pelvis's world angle.
		if (pelvisEZ > 180)									//-180 ~ 180
			pelvisEZ -= 360;
		float limitAimAngle = 95 + Mathf.Abs (pelvisEZ);	//95 is standard in facing Right and zero pelvis angle.
		if (limitAimAngle > 180)							//-180 ~ 180
			limitAimAngle -= 360;
		
		//Not Equal character's sight and cursor part -> FLIP
		if (facingRight && Mathf.Abs (mouseDegree) > limitAimAngle)
			Flip ();
		else if (!facingRight && Mathf.Abs (mouseDegree) < 180 - limitAimAngle)
			Flip ();

		if (facingRight) {	// mouseDegree : -90 ~ 90, anim.SetFloat : 0 ~ 1
			anim.SetFloat ("cursorAngle", (mouseDegree - pelvisEZ + 90) / 180);
		} else {
			if (mouseDegree > 0) {	// mouseDegree : 90 ~ 180
				anim.SetFloat ("cursorAngle", 90 / (mouseDegree - pelvisEZ));
			} else {				// mouseDegree : -90 ~ -180
				anim.SetFloat ("cursorAngle", -(mouseDegree - pelvisEZ + 90) / 180);
			}
		}
	}







	protected void Reload () {
		switch ((int)weaponKind) {
		case (int)Item.WeaponKind.Pistol:
			anim.CrossFade ("Pistol Reload", 0.1f);
			break;

		case (int)Item.WeaponKind.Rifle:
			anim.CrossFade ("Rifle Reload", 0.1f);
			break;

		case (int)Item.WeaponKind.Shotgun:
			anim.CrossFade ("Shotgun Reload", 0.1f);
			break;
		}
	}



	//-------------------------------Those functions is activated in Animation clip.-----------------------------



	public void UseItemInAnim () {
		if (selectedWeaponItem != null)
			selectedWeaponItem.Use ();
	}

	public void ReloadInAnim () {	//This is actually reloading function. this is activated in Animation playing by Reload().
		if (selectedWeaponItem != null) {
			selectedWeaponItem.Reload ();
			selectedWeaponItem.audioSource.PlayOneShot (selectedWeaponItem.soundReload);
		}
	}

	public void SetGiveMeeleAttackDamageInAnim (int ac) {
		bool setting;
		if (ac == 0)
			setting = false;
		else
			setting = true;

		if (selectedWeaponItem != null) {
			giveMeeleAttackDamage = setting;
			selectedWeaponItem.TrailEffectRenderer (setting);
		}

		if (setting && selectedWeaponItem != null)
			selectedWeaponItem.audioSource.PlayOneShot (selectedWeaponItem.soundUse);
	}
}
