using UnityEngine;
using System.Collections;

public class PlayerNetwork : Photon.MonoBehaviour {

	PhotonView pv = null;
	BasePlayer bp;
	Animator anim;

	//Network Transfer Data
	float animParaMove, animParaCursorAngle;
	int animParaWeaponKind, animUpperHash;
	bool animParaGrounded, animParaSit, animParaBackStep;

	float changeHp, changeMp; //Prepare this and bp.hp, If they are different each other, Set changeHp to bp.hp.

	SpriteRenderer[] srs;
	Color srsColor;



	// Use this for initialization
	void Awake () {
		pv = GetComponent<PhotonView> ();
		pv.synchronization = ViewSynchronization.UnreliableOnChange;

		bp = GetComponent<BasePlayer> ();
		anim = GetComponent<Animator> ();

		srs = GetComponentsInChildren<SpriteRenderer> ();
		srsColor.a = 1;

		if (pv.isMine) {
			UI.instance.SetPlayer (gameObject);
		} else {	// Other user in my screen doesn't use Rigidbody.
			GetComponent<Rigidbody2D> ().isKinematic = true;
		}

		changeHp = bp.hp;
		changeMp = bp.mp;

	}



	void OnEnable () {
		if (pv.isMine)	//Are you Player? Then Camera follows you.
			CameraFollow.instance.target = gameObject;

		StartCoroutine (CheckStatus ());
	}



	// For only status : Send RPC -> HP, MP, SCORE
	IEnumerator CheckStatus () {
		while (true) {
			// When you died, you send this to you in Other user's screen.
			if (pv.isMine && bp.hp <= 0)
				pv.RPC ("DeleteAlreadyDeadUser", PhotonTargets.Others);
			
			if (bp.hp != changeHp && !pv.isMine)
				pv.RPC ("ChangeHp", PhotonTargets.All, bp.hp);
			if (bp.mp != changeMp && !pv.isMine)
				pv.RPC ("ChangeMp", PhotonTargets.All, bp.mp);
			if (pv.isMine)
				pv.RPC ("ChangeScore", PhotonTargets.All, bp.score);
			
			yield return new WaitForSeconds (0.1f);
		}
	}

	[PunRPC]
	void DeleteAlreadyDeadUser () {
		if (gameObject.activeInHierarchy)
			StartCoroutine (DisappearLikeGhost ());
		//gameObject.SetActive (false);
	}

	[PunRPC]
	void ChangeHp (float cHp) {
		// Delete other dead user. 
		if (!pv.isMine && cHp <= 0)
			StartCoroutine (DisappearLikeGhost ());
		
		bp.hp = cHp;
		changeHp = cHp;
	}

	[PunRPC]
	void ChangeMp (float cMp) {
		bp.mp = cMp;
		changeMp = cMp;
	}

	[PunRPC]
	void ChangeScore (int cScore) {
		bp.score = cScore;
	}


	// For only animation
	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {	//This activates well. But Values isn't transmitted.
		if (stream.isWriting && pv.isMine) {			//Send data
			stream.SendNext (anim.GetFloat ("move"));
			stream.SendNext (anim.GetFloat ("cursorAngle"));
			stream.SendNext (anim.GetBool ("backStep"));
			stream.SendNext (anim.GetBool ("grounded"));
			stream.SendNext (anim.GetBool ("sit"));
			stream.SendNext (anim.GetInteger ("weaponKind"));
			stream.SendNext (anim.GetCurrentAnimatorStateInfo (anim.GetLayerIndex ("Upper")).fullPathHash);

		} else if (stream.isReading && !pv.isMine) {	//Receive data
			animParaMove = (float)stream.ReceiveNext ();
			animParaCursorAngle = (float)stream.ReceiveNext ();
			animParaBackStep = (bool)stream.ReceiveNext ();
			animParaGrounded = (bool)stream.ReceiveNext ();
			animParaSit = (bool)stream.ReceiveNext ();
			animParaWeaponKind = (int)stream.ReceiveNext ();
			animUpperHash = (int)stream.ReceiveNext ();

			if (animUpperHash == Animator.StringToHash ("Upper.Knife Slash"))
				anim.CrossFade ("Knife Slash", 0.1f);
			else if (animUpperHash == Animator.StringToHash ("Upper.Sword Slash"))
				anim.CrossFade ("Sword Slash", 0.1f);
			else if (animUpperHash == Animator.StringToHash ("Upper.Pistol Shot"))
				anim.CrossFade ("Pistol Shot", 0.1f);
			else if (animUpperHash == Animator.StringToHash ("Upper.Pistol Reload"))
				anim.CrossFade ("Pistol Reload", 0.1f);
			else if (animUpperHash == Animator.StringToHash ("Upper.Rifle Shot"))
				anim.CrossFade ("Rifle Shot", 0.1f);
			else if (animUpperHash == Animator.StringToHash ("Upper.Rifle Reload"))
				anim.CrossFade ("Rifle Reload", 0.1f);
			else if (animUpperHash == Animator.StringToHash ("Upper.Shotgun Shot"))
				anim.CrossFade ("Shotgun Shot", 0.1f);
			else if (animUpperHash == Animator.StringToHash ("Upper.Shotgun Reload"))
				anim.CrossFade ("Shotgun Reload", 0.1f);

			if (gameObject.activeInHierarchy) {	//Bug fix "Animation has not been initialized"
				anim.SetFloat ("move", animParaMove);
				anim.SetFloat ("cursorAngle", animParaCursorAngle);
				anim.SetBool ("backStep", animParaBackStep);
				anim.SetBool ("grounded", animParaGrounded);
				anim.SetBool ("sit", animParaSit);
				anim.SetInteger ("weaponKind", animParaWeaponKind);
			}
		}
	}

	// Delete object(Other user).
	public IEnumerator DisappearLikeGhost () {
		bp.EffectSet (0); // Effect on.
		while (srsColor.a > 0) { // Alpha decrease.
			srsColor.a -= 0.1f;
			foreach (SpriteRenderer sr in srs) {
				sr.color = srsColor;
			}
			yield return new WaitForSeconds (0.2f);
		}
		gameObject.SetActive (false);
	}
}
