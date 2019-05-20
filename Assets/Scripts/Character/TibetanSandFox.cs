using UnityEngine;
using System.Collections;

public class TibetanSandFox : BasePlayer {

	public float needMpOtherJump = 10;
	bool fall = false;



	protected override void Update () {
		base.Update ();

		if (sit || fall) {
			if (rigi.mass != massBackup * 10)
				rigi.mass = massBackup * 10;
		} else {
			if (rigi.mass != massBackup)
				rigi.mass = massBackup;
		}
	}

	protected override void Jump () {	//Double jump
		if (gc.grounded && jumpCount != 0)
			jumpCount = 0;	//Grounded, Not jump -> 0
		if (jumpCount == 0 && !gc.grounded)	//End of First Input -> You can Second Input
			jumpCount = 1;	//Grounded -> jump -> 1
		if (Input.GetButtonDown ("Vertical") && Input.GetAxisRaw ("Vertical") > 0) {	//Double jump
			if (jumpCount < 2) {
				if (gc.grounded && !sit) {	//First Input, Key:Up
					rigi.velocity = new Vector2 (rigi.velocity.x, jumpPower);
				}
				if (jumpCount == 1 && mp >= needMpOtherJump) {	//Second Input, KeyDown:Up, Need SP
					mp -= needMpOtherJump;
					rigi.velocity = new Vector2 (rigi.velocity.x, jumpPower);
					jumpCount = 2;
					EffectSet (0);
					if(soundJump != null)
						audioSource.PlayOneShot (soundJump);
				}
			}
		}

		if (jumpCount > 0 && Input.GetButtonDown ("Vertical") && Input.GetAxisRaw ("Vertical") < 0) {		//Falling
			if(!fall)
				fall = true;
			if(rigi.gravityScale != gravityBackup * 10)
				rigi.gravityScale = gravityBackup * 10;
		}

		if (jumpCount == 0 || Input.GetButtonUp ("Vertical")) {		//Fall off
			if(fall)
				fall = false;
			if(rigi.gravityScale != gravityBackup)
				rigi.gravityScale = gravityBackup;
			EffectSet (2);
		}
	}

}
