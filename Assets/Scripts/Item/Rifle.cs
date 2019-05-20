using UnityEngine;
using System.Collections;

public class Rifle : Item {

	public override void Shot (LayerMask attackLayerMask, LayerMask blockLayerMask) {	//Penetrate shot
		ammo--;

		float weaponEZ = transform.parent.eulerAngles.z - 90;
		if (weaponEZ < 0)
			weaponEZ += 360;

		Vector3 rotV = Quaternion.Euler (0, 0, weaponEZ) * Vector3.right;	//What Vector will rotate?
		RaycastHit2D[] aimRay = Physics2D.RaycastAll (shotPoint.position, rotV, bulletMaxDistance, attackLayerMask);
		BaseCharacter colBaseChar;

		//Line Renderer
		if (aimRay.Length == 0) {	//Can't find target -> Draw line without goal point.
			LineEffectRenderer (true);
		} else {
			RaycastHit2D lineRay = Physics2D.Raycast (shotPoint.position, rotV, bulletMaxDistance, blockLayerMask);
			if(lineRay.collider == null)
				LineEffectRenderer (true);										//There is enemy but no wall -> Penetrate shot
			else if(lineRay.collider.GetComponentInParent<BaseCharacter> () != myBaseHumanoid.GetComponentInParent<BaseCharacter> ())
				LineEffectRenderer (true, shotPoint.position, lineRay.point);	//There is wall -> Blocked shot
			else
				LineEffectRenderer (true);
		}

		transform.GetComponentInChildren<ParticleSystem> ().Play ();	//Gunfire effect
		audioSource.PlayOneShot (soundUse);								//Sound effect

		for (int a = 0; a < aimRay.Length; a++) {
			
			ObjectPooling.instance.MakeFind ("Effect Hit", aimRay[a].point, new Quaternion (0, 0, 0, 0));	//Damaged point effect

			if (aimRay [a].collider != null && ((1 << aimRay [a].collider.gameObject.layer) & blockLayerMask) != 0)
				return;
			
			colBaseChar = aimRay [a].collider.GetComponentInParent<BaseCharacter> ();
			if (colBaseChar == myBaseHumanoid.GetComponentInParent<BaseCharacter> ())	//Don't shot myself!
				continue;

			if (aimRay [a].rigidbody != null)	//Give physics force
				aimRay [a].rigidbody.AddForceAtPosition (rotV * weaponForce, aimRay [a].point);
			
			if (colBaseChar != null && colBaseChar != GetComponent<BaseCharacter> ()) {	//Give damage to enemy, Don't shoot myself.

				float giveDamage = damage;

				switch (aimRay [a].collider.name) {	//What is damaged body part?
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
		}
	}

}
