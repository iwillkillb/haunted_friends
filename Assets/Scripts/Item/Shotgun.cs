using UnityEngine;
using System.Collections;

public class Shotgun : Item {

	public int bulletAmount = 5, angleVarient = 10;
	bool angleVarientPlus = true;

	public GameObject objLineRenderer;
	GameObject[] arrayObjLineRenderer;

	protected override void Awake () {
		base.Awake ();

		if (objLineRenderer == null)
			return;
		
		arrayObjLineRenderer = new GameObject[bulletAmount];
		for (int a = 0; a < arrayObjLineRenderer.Length; a++) {
			arrayObjLineRenderer [a] = Instantiate (objLineRenderer, transform) as GameObject;
			arrayObjLineRenderer [a].transform.position = shotPoint.position;

			if(arrayObjLineRenderer [a].GetComponent<LineRenderer>() != null)
				arrayObjLineRenderer [a].GetComponent<LineRenderer>().sortingLayerName = "Effect";
		}
	}



	public override void Shot (LayerMask attackLayerMask, LayerMask blockLayerMask) {	//Penetrate and many shot
		ammo--;

		RaycastHit2D[][] aimRay = new RaycastHit2D [bulletAmount][];
		float weaponEZ = transform.parent.eulerAngles.z - 90;
		if (weaponEZ < 0)
			weaponEZ += 360;
		Vector3 rotV;	//What Vector will rotate?
		BaseCharacter colBaseChar;
		BaseCharacter[] killedChars = new BaseCharacter[bulletAmount];	//Check score.

		for (int a = 0; a < aimRay.Length; a++) {
			if(angleVarientPlus)
				weaponEZ += Random.Range (0, angleVarient);
			else
				weaponEZ -= Random.Range (0, angleVarient);
			angleVarientPlus = !angleVarientPlus;
			
			if (weaponEZ < 0)
				weaponEZ += 360;
			
			rotV = Quaternion.Euler (0, 0, weaponEZ) * Vector3.right;
			aimRay [a] = Physics2D.RaycastAll (shotPoint.position, rotV, bulletMaxDistance, attackLayerMask);
			lineR = arrayObjLineRenderer [a].GetComponent<LineRenderer> ();	//New Line renderer.

			for (int b = 0; b < aimRay [a].Length; b++) {

				ObjectPooling.instance.MakeFind ("Effect Hit", aimRay[a][b].point, new Quaternion (0, 0, 0, 0));	//Damaged point effect

				if (aimRay [a] [b].collider != null && ((1 << aimRay [a] [b].collider.gameObject.layer) & blockLayerMask) != 0)
					break;

				colBaseChar = aimRay [a] [b].collider.GetComponentInParent<BaseCharacter> ();
				killedChars [a] = colBaseChar;
				if (colBaseChar == myBaseHumanoid.GetComponentInParent<BaseCharacter> ())	//Don't shot myself!
					continue;

				if (aimRay [a] [b].rigidbody != null) {	//Give physics force
					aimRay [a] [b].rigidbody.AddForceAtPosition (rotV * weaponForce, aimRay[a][b].point);
				}

				if (colBaseChar != null && colBaseChar != GetComponent<BaseCharacter> ()) {	//Give damage to enemy, Don't shoot myself.

					float giveDamage = damage;
					switch (aimRay [a] [b].collider.name) {	//What is damaged body part?
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
				}
			}

			//If you killed this...
			if (killedChars [a] != null && killedChars [a].hp <= 0 && myBaseHumanoid.youAlreadyKilledThisChar != killedChars [a]) {
				myBaseHumanoid.score++;
				myBaseHumanoid.youAlreadyKilledThisChar = killedChars [a];
			}

			//Line Renderer
			if (aimRay [a].Length == 0) {	//Can't find target -> Draw line without goal point.
				LineEffectRenderer (true, shotPoint.position, transform.position + rotV * bulletMaxDistance);
			} else {
				RaycastHit2D lineRay = Physics2D.Raycast (shotPoint.position, rotV, bulletMaxDistance, blockLayerMask);
				if(lineRay.collider == null)
					LineEffectRenderer (true, shotPoint.position, transform.position + rotV * bulletMaxDistance);//There is enemy but no wall -> Penetrate shot
				else
					LineEffectRenderer (true, shotPoint.position, lineRay.point);	//There is wall -> Blocked shot
			}
		}

		transform.GetComponentInChildren<ParticleSystem> ().Play ();	//Gunfire effect
		audioSource.PlayOneShot (soundUse);								//Sound effect													//Sound effect
	}





















	/*
	public override void Shot (LayerMask attackLayerMask, LayerMask blockLayerMask) {	//Used by Character.  No Penetration
		ammo--;

		float weaponEZ = transform.parent.eulerAngles.z - 90;
		if (weaponEZ < 0)
			weaponEZ += 360;

		Vector3 rotV = Quaternion.Euler (0, 0, weaponEZ) * Vector3.right;	//What Vector will rotate?
		//RaycastHit2D aimRay = Physics2D.Raycast (shotPoint.position, rotV, bulletMaxDistance, attackLayerMask);
		RaycastHit2D[] aimRay = new RaycastHit2D [bulletAmount];

		transform.GetComponentInChildren<ParticleSystem> ().Play ();	//Gunfire effect
		SoundOn (0);													//Sound effect


		for (int a = 0; a < aimRay.Length; a++) {

			if(angleVarientPlus)
				weaponEZ += Random.Range (0, angleVarient);
			else
				weaponEZ -= Random.Range (0, angleVarient);
			angleVarientPlus = !angleVarientPlus;

			if (weaponEZ < 0)
				weaponEZ += 360;

			rotV = Quaternion.Euler (0, 0, weaponEZ) * Vector3.right;
			aimRay [a] = Physics2D.Raycast (shotPoint.position, rotV, bulletMaxDistance, attackLayerMask);
			lineR = arrayObjLineRenderer [a].GetComponent<LineRenderer> ();	//New Line renderer.
			
			if (aimRay[a].collider != null) {

				//Line Renderer
				RaycastHit2D lineRay = Physics2D.Raycast (shotPoint.position, rotV, bulletMaxDistance, blockLayerMask);
				if (lineRay.collider == null)
					LineEffectRenderer (true);										//There is enemy but no wall -> Penetrate shot
				else if (lineRay.collider.GetComponentInParent<BaseCharacter> () != myBaseHumanoid.GetComponentInParent<BaseCharacter> ())
					LineEffectRenderer (true, shotPoint.position, lineRay.point);	//There is wall -> Blocked shot
				else
					LineEffectRenderer (true);

				ObjectPooling.instance.MakeFind ("Effect Hit", aimRay[a].point, new Quaternion (0, 0, 0, 0));	//Damaged point effect

				BaseCharacter colBaseChar = aimRay[a].collider.GetComponentInParent<BaseCharacter> ();

				if (colBaseChar == myBaseHumanoid.GetComponent<BaseCharacter> ())	//Don't shot myself!
				return;

				if (aimRay[a].rigidbody != null)		//Give physics force
					aimRay[a].rigidbody.AddForceAtPosition (rotV * weaponForce, aimRay[a].point);

				if (colBaseChar != null && colBaseChar != GetComponent<BaseCharacter> ()) {	//Give damage to enemy, Don't shoot myself.

					float giveDamage = damage;

					switch (aimRay[a].collider.name) {	//What is damaged body part?
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
	}*/
}
