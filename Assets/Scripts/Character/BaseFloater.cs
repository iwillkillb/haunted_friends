using UnityEngine;
using System.Collections;

public class BaseFloater : BaseCharacter {

	public enum SpriteChange {none, flip, rotate};
	public SpriteChange spriteChange = SpriteChange.none;

	public enum MoveState {idle, go, back};
	public MoveState moveState = MoveState.idle;

	public Transform target;
	public float nearestDistanceToTarget = 0.2f;
	protected float inputMove = 0;	// + : Go to target / - : Back from target

	protected float goalAngle = 0, moveAngle = 0, rotationPerFrame = 10f;





	protected override void Awake () {
		base.Awake ();
	}



	protected override void Start () {
		base.Start ();
	}



	protected override void OnEnable () {
		base.OnEnable ();
	}



	// Update is called once per frame
	protected override void Update () {
		base.Update ();

		switch (spriteChange) {
		case SpriteChange.flip:	//Input direction == Character's direction
			if ((transform.position.x < target.position.x && !facingRight) || (transform.position.x > target.position.x && facingRight))
				Flip ();
			break;
		case SpriteChange.rotate:
			transform.eulerAngles = new Vector3 (0, 0, moveAngle);
			break;
		}

		MoveSet ();
	}



	protected override void FixedUpdate () {
		base.FixedUpdate ();

		if (delayAttack < delayAttackOrigin)
			delayAttack += Time.deltaTime;
	}





	void ChaseAngle () {
		float dx, dy;
		dx = target.position.x - transform.position.x;
		dy = target.position.y - transform.position.y;
		goalAngle = Mathf.Atan2 (dy, dx) * Mathf.Rad2Deg;	//-180 ~ 180, 0 is right.

		//Rotation
		if (moveAngle < goalAngle - 5)
			moveAngle += rotationPerFrame;
		else if (moveAngle > goalAngle + 5)
			moveAngle -= rotationPerFrame;
	}
	void MoveSet () {
		ChaseAngle ();

		switch (moveState) {
		case MoveState.idle:
			if (Mathf.Abs (inputMove) > 0.1)
				inputMove *= 0.9f;
			else
				inputMove = 0;
			break;

		case MoveState.go:
			if (target == null)
				return;

			if (Vector3.Distance(transform.position, target.position) > nearestDistanceToTarget) {
				if (inputMove <= 0)
					inputMove = 0.1f;
				else
					inputMove *= 1.1f;
			} else {
				if (Mathf.Abs (inputMove) > 0.1)
					inputMove *= 0.9f;
				else
					inputMove = 0;
			}
			break;

		case MoveState.back:
			if (target == null)
				return;

			if (inputMove >= 0)
				inputMove = -0.1f;
			else
				inputMove *= 1.1f;
			break;
		}

		//Clamping
		if (inputMove > 1)
			inputMove = 1;
		else if (inputMove < -1)
			inputMove = -1;

		//Physics moving
		rigi.velocity = Quaternion.Euler (0, 0, moveAngle) * Vector2.right * speed * inputMove;
	}
}
