using UnityEngine;
using System.Collections;

public class Aardwolf : BasePlayer {
	
	bool dash = false;
	public float mpdashReduce = 10, dashSpeed = 100, dashTimeOrigin = 0.2f, delayDashOrigin = 1f;
	float dashTime = 0, delayDash = 1f;

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

	protected override void FixedUpdate () {
		base.FixedUpdate ();

		if (delayDash < delayDashOrigin && !dash)
			delayDash += Time.deltaTime;
	}

	protected override void Jump () {	//Jump and dash
		if (gc.grounded && jumpCount != 0)
			jumpCount = 0;	//Grounded, Not jump -> 0
		if (jumpCount == 0 && !gc.grounded)	//End of First Input -> You can Second Input
			jumpCount = 1;	//Grounded -> jump -> 1
		if (Input.GetButtonDown ("Vertical") && Input.GetAxisRaw ("Vertical") > 0) {	//Jump with dashing
			if (jumpCount < 2) {
				if (gc.grounded && !sit) {	//First Input, Key:Up
					rigi.velocity = new Vector2 (rigi.velocity.x, jumpPower);
				}
				if (jumpCount == 1 && mp >= mpdashReduce && Input.GetAxisRaw ("Horizontal") != 0 && delayDash >= delayDashOrigin) {	//dash on
					if(!dash)
						dash = true;
					if(soundJump != null)
						audioSource.PlayOneShot (soundJump);
					EffectSet (0);
				}
			}
		}

		if (dash) {										//dashing...
			dashTime += Time.deltaTime;
			mp -= Time.deltaTime * mpdashReduce;
			if(delayDash != 0)
				delayDash = 0;

			if (Input.GetAxisRaw ("Horizontal") > 0)		//Right dash
				rigi.velocity = new Vector2 (dashSpeed, jumpPower * 0.5f);
			else if (Input.GetAxisRaw ("Horizontal") < 0)	//Left dash
				rigi.velocity = new Vector2 (-dashSpeed, jumpPower * 0.5f);
		}

		if (jumpCount == 0 || Input.GetButtonUp ("Vertical") || Input.GetButtonUp ("Horizontal") || dashTime >= dashTimeOrigin) {	//dash, fall off
			if(dash)
				dash = false;
			dashTime = 0;
			if(fall)
				fall = false;
			if(rigi.gravityScale != gravityBackup)
				rigi.gravityScale = gravityBackup;
			EffectSet (2);
		}

		if (jumpCount > 0 && Input.GetButtonDown ("Vertical") && Input.GetAxisRaw ("Vertical") < 0) {		//Falling
			if(!fall)
				fall = true;
			if(rigi.gravityScale != gravityBackup * 10)
				rigi.gravityScale = gravityBackup * 10;
		}
	}
}
