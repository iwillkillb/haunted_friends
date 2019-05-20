using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI : MonoBehaviour {
	// HP/MP Bar and Head image.

	public static UI instance; // Singletone

	// For display hp and mp.
	Image barHp, barMp;

	// For display character's face in UI.
	Image imageHead;
	SpriteRenderer headSprR;

	// This displays gun's ammo(When you equip gun) and score.
	Text textAmmo, textScore;

	public Image imageItemGetMode;
	public Sprite sprItemGetModeOn, sprItemGetModeOff;

	BaseHumanoid compoPlayer;

	// Use this for initialization
	void Awake () {
		// Singletone
		if (instance) {
			DestroyImmediate (gameObject);
			return;
		}
		instance = this;

		barHp = transform.FindChild ("Bar HP").GetComponent<Image> ();
		barMp = transform.FindChild ("Bar MP").GetComponent<Image> ();

		imageHead = transform.FindChild ("Mask Head").transform.FindChild ("Image Head").GetComponent<Image> ();

		textAmmo = transform.FindChild ("Text Ammo").GetComponent<Text> ();
		textScore = transform.FindChild ("Text Score").GetComponent<Text> ();
	}

	public void SetPlayer (GameObject setPlayer) {
		compoPlayer = setPlayer.GetComponent<BaseHumanoid> ();
		headSprR = compoPlayer.head.GetComponent<SpriteRenderer> (); // Don't use GetComponent repeatly.
		StartCoroutine (DisplayPlayerInfo ());
	}

	IEnumerator DisplayPlayerInfo () {
		while (compoPlayer != null) {
			barHp.fillAmount = compoPlayer.hp / compoPlayer.hpOrigin;
			barMp.fillAmount = compoPlayer.mp / compoPlayer.mpOrigin;
			if(compoPlayer.head != null)
				imageHead.sprite = headSprR.sprite;

			if (compoPlayer.weaponKind >= Item.WeaponKind.Pistol) {
				if(!textAmmo.gameObject.activeInHierarchy)
					textAmmo.gameObject.SetActive (true);
				
				textAmmo.text = 
					"Ammo : " + compoPlayer.selectedWeaponItem.ammo + " / " + compoPlayer.selectedWeaponItem.ammoOrigin +
					"\nRemain Ammo : " + compoPlayer.selectedWeaponItem.ammoRemain;
			} else {
				if(textAmmo.gameObject.activeInHierarchy)
					textAmmo.gameObject.SetActive (false);
			}

			textScore.text =  "Score : " + compoPlayer.score;

			if (imageItemGetMode != null) {
				if (compoPlayer.getItem)
					imageItemGetMode.sprite = sprItemGetModeOn;
				else
					imageItemGetMode.sprite = sprItemGetModeOff;
			}

			yield return new WaitForSeconds (0.2f);
		}
	}


}
