using UnityEngine;
using System.Collections;

public class Shoebill : BasePlayer {

	//Jump
	bool fly = false, fall = false;
	public float mpFlyReduce = 10, flySpeedMul = 2;



	// Update is called once per frame
	protected override void Update () {
		base.Update ();

		Buzz ();

		if (sit || fly || fall) {
			if (rigi.mass != massBackup * 10)
				rigi.mass = massBackup * 10;
		} else {
			if (rigi.mass != massBackup)
				rigi.mass = massBackup;
		}
	}

	protected override void Jump () {	//Jump and Fly
		if (gc.grounded && jumpCount != 0)
			jumpCount = 0;	//Grounded, Not jump -> 0
		if (jumpCount == 0 && !gc.grounded)	//End of First Input -> You can Second Input
			jumpCount = 1;	//Grounded -> jump -> 1
		if (Input.GetButtonDown ("Vertical") && Input.GetAxisRaw ("Vertical") > 0) {	//Jump with Flying
			if (jumpCount < 2) {
				if (gc.grounded && !sit) {	//First Input, Key:Up
					rigi.velocity = new Vector2 (rigi.velocity.x, jumpPower);
				}
				if (jumpCount == 1 && mp >= Time.deltaTime * mpFlyReduce) {	//Fly on
					if(!fly)
						fly = true;
					if(rigi.gravityScale != -gravityBackup)
						rigi.gravityScale = -gravityBackup;
					if (soundJump != null)
						audioSource.PlayOneShot (soundJump);
					EffectSet (0);
				}
			}
		}

		if (fly) {										//Flying...
			mp -= Time.deltaTime * mpFlyReduce;
		}

		if (jumpCount > 0 && Input.GetButtonDown ("Vertical") && Input.GetAxisRaw ("Vertical") < 0) {		//Falling
			if(!fall)
				fall = true;
			if(rigi.gravityScale != gravityBackup * 10)
				rigi.gravityScale = gravityBackup * 10;
		}

		if (jumpCount == 0 || Input.GetButtonUp ("Vertical") || mp < Time.deltaTime * mpFlyReduce) {		//Fly off
			if(fly)
				fly = false;
			if(fall)
				fall = false;
			if(rigi.gravityScale != gravityBackup)
				rigi.gravityScale = gravityBackup;
			EffectSet (2);
		}
	}




	void Buzz () {
		int parameter = 0;

		if (gc.grounded) {
			if (backStep)
				parameter = 1;
			else
				parameter = 0;
		} else {
			if (rigi.velocity.y >= 0.1f || fly)
				parameter = 1;
			else if (rigi.velocity.y < -0.1f)
				parameter = 2;
		}

		if(anim.GetInteger ("wingNum") != parameter)
			anim.SetInteger ("wingNum", parameter);
	}
}
