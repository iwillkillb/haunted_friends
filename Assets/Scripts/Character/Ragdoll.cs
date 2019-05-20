using UnityEngine;
using System.Collections;

public class Ragdoll : MonoBehaviour {

	Transform[] parentBodyTranses, dollBodyTranses;
	SpriteRenderer parentSprite, dollSprite;

	HingeJoint2D hinge;
	JointAngleLimits2D setLimit;
	float limitMin, limitMax;

	public string[] dontChangeSpriteObject;
	bool thisPartNoChangeSprite;	//Don't copy/paste in this body part.

	bool jointRight = true, needJointFlip = false;

	public void RagdollPose (Transform myParent) {
		if (myParent == null)
			return;

		transform.parent = null;
		transform.position = myParent.position;
		transform.rotation = myParent.rotation;
		transform.localScale = myParent.localScale;
		GetComponent<Rigidbody2D> ().velocity = myParent.GetComponent<Rigidbody2D> ().velocity;

		parentBodyTranses = myParent.GetComponentsInChildren<Transform> ();	//Parent : Who character spawned ragdoll?
		dollBodyTranses = transform.GetComponentsInChildren<Transform> ();

		needJointFlip = false;
		if ((transform.localScale.x < 0 && jointRight) || (transform.localScale.x > 0 && !jointRight)) {
			jointRight = !jointRight;
			needJointFlip = true;
		}

		foreach (Transform dollBodyT in dollBodyTranses) {
			//Hinge Joint's Limit angles is setted as World Space.
			//Then, You have Change limit angles when ragdoll's scaleX is fliped.
			hinge = dollBodyT.GetComponent<HingeJoint2D> ();
			if (hinge != null && needJointFlip) {
				limitMax = hinge.limits.max;
				limitMin = hinge.limits.min;
				setLimit.max = -limitMin;
				setLimit.min = -limitMax;
				hinge.limits = setLimit;
			}

			foreach (Transform parentBodyT in parentBodyTranses) {	//Success parent's body to doll's body.
				if (dollBodyT.name == parentBodyT.name) {			//Compare Doll's part and Parent's part.

					thisPartNoChangeSprite = false;	//In ragdoll obj's component, There are sprites no succeeding parent's sprite.
					foreach (string dontChangeSprite in dontChangeSpriteObject) {
						if (dollBodyT.name == dontChangeSprite) {
							thisPartNoChangeSprite = true;
							break;
						}
					}



					dollBodyT.localPosition = parentBodyT.localPosition;	//Position success
					dollBodyT.localRotation = parentBodyT.localRotation;	//Rotation success
					dollBodyT.localScale = parentBodyT.localScale;			//Scale success



					dollSprite = dollBodyT.GetComponent<SpriteRenderer> ();
					parentSprite = parentBodyT.GetComponent<SpriteRenderer> ();

					dollSprite.sortingOrder = parentSprite.sortingOrder;	//Sprite layer success
					if (dollSprite != null && parentSprite != null) {
						if (!thisPartNoChangeSprite) {
							dollSprite.sprite = parentSprite.sprite;		//Sprite success
							dollSprite.color = parentSprite.color;			//Sprite color success
						}
					}

					break;
				}
			}
		}

	}
}
