using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour {

	//Ground check...
	[System.NonSerialized]public bool grounded = true;

	public Vector2 direction = Vector2.down;
	public float distanceCheck = 2f;
	public LayerMask whatIsGround;

	void FixedUpdate () {
		grounded = Physics2D.Raycast (transform.position, direction, distanceCheck, whatIsGround);
	}
}
