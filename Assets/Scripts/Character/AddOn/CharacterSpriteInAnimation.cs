using UnityEngine;
using System.Collections;

public class CharacterSpriteInAnimation : MonoBehaviour {

	[System.Serializable]public class VariableSprite {
		[System.NonSerialized]public SpriteRenderer sr;
		public Sprite[] sprites;

		public void SpriteChange(int index) {
			if (sr != null && index >= 0 && index < sprites.Length) {
				if(sr.sprite != sprites [index])
					sr.sprite = sprites [index];
			}
		}
		public void SpriteChange(Sprite spr) {
			if (sr != null) {
				if(sr.sprite != spr)
					sr.sprite = spr;
			}
		}
	}
	public Sprite headSpriteDead;
	public VariableSprite headDamage, handF, handB, footF, footB;

	Animator anim;
	BaseHumanoid cb;

	int headDamageIndex = 0;

	//Animator Hash
	protected readonly static int animHash_Stand = Animator.StringToHash ("Base.Stand");
	protected readonly static int animHash_Fly = Animator.StringToHash ("Base.Fly");
	protected readonly static int animHash_Low = Animator.StringToHash ("Base.Low");
	protected readonly static int animHash_Back = Animator.StringToHash ("Base.Back");

	protected readonly static int animHash_No_Weapon = Animator.StringToHash ("Upper.No Weapon");

	protected readonly static int animHash_Knife_Idle = Animator.StringToHash ("Upper.Knife Idle");
	protected readonly static int animHash_Knife_Slash = Animator.StringToHash ("Upper.Knife Slash");

	protected readonly static int animHash_Sword_Idle = Animator.StringToHash ("Upper.Sword Idle");
	protected readonly static int animHash_Sword_Slash = Animator.StringToHash ("Upper.Sword Slash");

	protected readonly static int animHash_Pistol = Animator.StringToHash ("Upper.Pistol Idle");
	protected readonly static int animHash_Pistol_Shot = Animator.StringToHash ("Upper.Pistol Shot");
	protected readonly static int animHash_Pistol_Reload = Animator.StringToHash ("Upper.Pistol Reload");

	protected readonly static int animHash_Rifle = Animator.StringToHash ("Upper.Rifle Idle");
	protected readonly static int animHash_Rifle_Shot = Animator.StringToHash ("Upper.Rifle Shot");
	protected readonly static int animHash_Rifle_Reload = Animator.StringToHash ("Upper.Rifle Reload");

	protected readonly static int animHash_Shotgun = Animator.StringToHash ("Upper.Shotgun Idle");
	protected readonly static int animHash_Shotgun_Shot = Animator.StringToHash ("Upper.Shotgun Shot");
	protected readonly static int animHash_Shotgun_Reload = Animator.StringToHash ("Upper.Shotgun Reload");

	void Awake () {
		anim = GetComponent<Animator> ();
		cb = GetComponent<BaseHumanoid> ();

		headDamage.sr = transform.FindChild("Pelvis").transform.FindChild ("Chest").FindChild ("Head").GetComponent<SpriteRenderer> ();
		footF.sr = transform.FindChild ("Pelvis").FindChild ("LegFU").FindChild ("LegFL").FindChild ("FootF").GetComponent<SpriteRenderer> ();
		footB.sr = transform.FindChild ("Pelvis").FindChild ("LegBU").FindChild ("LegBL").FindChild ("FootB").GetComponent<SpriteRenderer> ();
		handF.sr = transform.FindChild ("Pelvis").FindChild ("Chest").FindChild ("ArmFU").FindChild ("ArmFL").FindChild ("HandF").GetComponent<SpriteRenderer> ();
		handB.sr = transform.FindChild ("Pelvis").FindChild ("Chest").FindChild ("ArmBU").FindChild ("ArmBL").FindChild ("HandB").GetComponent<SpriteRenderer> ();
	}

	void OnEnable () {
		StartCoroutine (SpriteChange ());
	}
	void OnDisable () {
		StopCoroutine (SpriteChange ());
	}

	IEnumerator SpriteChange () {
		while (true) {
			//Foot sprite change in animation.
			if (anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Base")).fullPathHash == animHash_Stand) {
				if (anim.GetFloat ("move") > 0.1f) {
					footF.SpriteChange (1);
					footB.SpriteChange (1);
				} else {
					footF.SpriteChange (0);
					footB.SpriteChange (1);
				}
			} else {
				footF.SpriteChange (1);
				footB.SpriteChange (1);
			}



			//Hand sprite change in animation.
			if (anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash == animHash_No_Weapon) {
				handF.SpriteChange (0);
				handB.SpriteChange (0);
			} else {
				handF.SpriteChange (2);
				handB.SpriteChange (1);
			}



			//headDamage sprite change in HP status (Use BaseHumanoid.cs).
			if (cb.hp > 0)
				headDamageSpriteChangeByHp ();
			else if (headSpriteDead != null)
				headDamage.SpriteChange (headSpriteDead);

			yield return new WaitForSeconds (0.2f);
		}
	}


	void headDamageSpriteChangeByHp () {
		if (headDamage.sprites.Length <= 1)
			return;
		
		float hpDevided = cb.hpOrigin / headDamage.sprites.Length;
		float hpMinThisStatus = hpDevided * (headDamage.sprites.Length - (headDamageIndex + 1));
		float hpMaxThisStatus = hpDevided * (headDamage.sprites.Length - headDamageIndex);
		// Example : HP 100, Sprite 5 -> hpDevided = 20
		// HP 55/100 -> [0~20] [20~40] [40~60] [60~80] [80~100] -> 55 in [40~60] -> goal of index : [2]

		if (cb.hp < hpMinThisStatus) {
			headDamageIndex++;
		} else if (cb.hp > hpMaxThisStatus) {
			headDamageIndex--;
		} else
			headDamage.SpriteChange (headDamageIndex);
	}
}
