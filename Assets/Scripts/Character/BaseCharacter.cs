using UnityEngine;
using System.Collections;

public class BaseCharacter : MonoBehaviour {

	//Components
	protected GroundCheck gc;
	protected Rigidbody2D rigi;
	protected Animator anim;
	protected AudioSource audioSource;

	[Header("Status")]
	public float hpOrigin = 100f;
	public float mpOrigin = 100f;
	[System.NonSerialized]public float hp, mp;
	public float speed = 10f, jumpPower = 10f;	//Horizontal speed.
	protected bool isDied = false; // Check dying.

	[System.NonSerialized]public float delayAttack = 0; 
	public float delayAttackOrigin = 1;

	protected bool facingRight = true;	//See Left or Right

	[System.NonSerialized]public int score = 0;	//Killed enemy -> Plus score.
	[System.NonSerialized]public BaseCharacter youAlreadyKilledThisChar = null;		//Don't repeat taking score.



	protected virtual void Awake () {
		if(transform.FindChild ("Ground Check") != null)
			gc = transform.FindChild ("Ground Check").GetComponent<GroundCheck> ();
		rigi = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		audioSource = GetComponent<AudioSource> ();

		hp = hpOrigin;
		mp = mpOrigin;
	}



	protected virtual void Start () {

		//SpriteRenderer's Order in layer 100 set. Each Character's sprites isn't filed.
		BaseCharacter[] bcs = FindObjectsOfType<BaseCharacter> ();
		SpriteRenderer[] sps = GetComponentsInChildren<SpriteRenderer> ();
		int myNum = 0;

		foreach (BaseCharacter bc in bcs) {
			if (this == bc)
				break;
			else
				myNum++;
		}

		foreach (SpriteRenderer sp in sps) {
			sp.sortingOrder -= ((bcs.Length - 1 + myNum) % 100) * 100;	//First drawed -> Forward.
		}
	}



	protected virtual void OnEnable () {	//This function is Used by other object's function.
		if (hp > 0)
			return;

		hp = hpOrigin;
		mp = mpOrigin;

		//gameObject.SetActive (false);	//Bug fix "Animation has not been initialized"
		//gameObject.SetActive (true);
	}



	protected virtual void Update () {
		
	}



	protected virtual void FixedUpdate () {
		//HP Clamping
		if (hp <= 0) {
			hp = 0;
			if (!isDied)
				Die ();
		}
		if (hp > hpOrigin)
			hp = hpOrigin;

		// MP Clamping
		if (mp < 0)
			mp = 0;
		if (mp > mpOrigin)
			mp = mpOrigin;

		//MP Restoration
		if (mp < mpOrigin) {
			mp += Time.deltaTime;
		}
	}



	protected virtual void Die () {	//This function is defined by child's code.
		isDied = true;
	}





	protected virtual void Move () {
		rigi.velocity = new Vector2 (transform.localScale.x * speed, rigi.velocity.y);
	}



	protected virtual void Jump () {
		if(gc.grounded)
			rigi.velocity = new Vector2 (rigi.velocity.x, jumpPower);
	}



	protected void Flip () {	//Character's sprite reverse.
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
