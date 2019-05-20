using UnityEngine;
using System.Collections;

public class Minion : BaseHumanoid {

	public enum MoveState {idle, go, back};
	public MoveState moveState = MoveState.idle;
	public float nearestDistanceToTarget = 0.2f;
	float inputMove = 0;

	public GameObject target;





	protected override void Awake () {
		base.Awake ();
	}



	protected override void Start () {
		base.Start ();
	}



	protected override void OnEnable () {
		base.OnEnable ();
	}



	protected override void FixedUpdate () {
		base.FixedUpdate ();
	}



	protected override void Update () {
		base.Update ();

		MoveSet ();
		JumpSet ();
	}





	void MoveSet () {
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

			if (transform.position.x < target.transform.position.x - nearestDistanceToTarget) {
				if (inputMove <= 0)
					inputMove = 0.1f;
				else
					inputMove *= 1.1f;
			} else if (transform.position.x > target.transform.position.x + nearestDistanceToTarget) {
				if (inputMove >= 0)
					inputMove = -0.1f;
				else
					inputMove *= 1.1f;
			} else if (Mathf.Abs(transform.position.x - target.transform.position.x) < nearestDistanceToTarget) {
				if (Mathf.Abs (inputMove) > 0.1)
					inputMove *= 0.9f;
				else
					inputMove = 0;
			}
			break;

		case MoveState.back:
			if (target == null)
				return;

			if (transform.position.x < target.transform.position.x) {
				if (inputMove >= 0)
					inputMove = -0.1f;
				else
					inputMove *= 1.1f;
			} else if (transform.position.x > target.transform.position.x) {
				if (inputMove <= 0)
					inputMove = 0.1f;
				else
					inputMove *= 1.1f;
			}
			break;
		}

		//Clamping
		if (inputMove > 1)
			inputMove = 1;
		else if (inputMove < -1)
			inputMove = -1;

		Move (inputMove);
	}

	void JumpSet () {
		bool isWall = Physics2D.Raycast (transform.position, Vector2.right * transform.localScale.x, 2, gc.whatIsGround);
		if (isWall)
			Jump ();
	}
}
