using UnityEngine;
using System.Collections;

public class OtherUserSpriteChange : MonoBehaviour {
	// This code changes sprites of other user.

	public GameObject mobObj;
	public Sprite[] sprHead, sprHandF, sprHandB;	// There is only one sprite for hands -> This have to Empty.
	public Sprite[] sprFootF, sprFootB;

	public Sprite[] wingFSprites, wingBSprites;	// For Shoebill only...

	void Awake () {
		// This code is activated to other users...
		if (GetComponent<PhotonView> ().isMine)
			this.enabled = false;
	}

	// Use this for initialization
	void Start () {
		SpriteRenderer[] sprs = transform.GetComponentsInChildren<SpriteRenderer> ();
		SpriteRenderer[] mobSprs = mobObj.GetComponentsInChildren<SpriteRenderer> ();
		// Same name part -> Change sprite.
		foreach (SpriteRenderer sr in sprs) {
			foreach (SpriteRenderer mobSr in mobSprs) {
				if (sr.name == mobSr.name) {
					sr.sprite = mobSr.sprite;
					break;
				}
			}
		}

		CharacterSpriteInAnimation csia = GetComponent<CharacterSpriteInAnimation> ();
		if (csia != null) {
			csia.headDamage.sprites = sprHead;
			csia.handF.sprites = sprHandF;
			csia.handB.sprites = sprHandB;

			csia.footF.sprites = sprFootF;
			csia.footB.sprites = sprFootB;
		}

		// For Shoebill only...
		WingSpriteInAnimation wsia = GetComponent<WingSpriteInAnimation> ();
		if (wsia != null) {
			wsia.wingFSprites = wingFSprites;
			wsia.wingBSprites = wingBSprites;
		}
	}
}
