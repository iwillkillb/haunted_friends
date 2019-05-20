using UnityEngine;
using System.Collections;

public class RocketLauncher : Item {

	public GameObject rocket;

	protected override void Awake () {	//Spawn Rocket objects.
		base.Awake ();

		// Spawn rocket objects.
		if (rocket != null) {
			for (int a = 0; a < ammoRemain + ammo; a++) {
				GameObject obj = Instantiate (rocket, shotPoint) as GameObject;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localEulerAngles = Vector3.zero;
				obj.GetComponent<Rocket> ().Active = false;
				obj.GetComponent<Rocket> ().launcher = this;
				obj.GetComponent<Rocket> ().launcherId = itemUserId;
				obj.GetComponent<SpriteRenderer> ().sortingLayerID = spriteSortingLayerIdBackup;
				obj.GetComponent<SpriteRenderer> ().sortingOrder = spriteSortingOrderBackup - 1;
			}
		}
	}

	protected override void Update () {	//Empty ammo -> Hide rocket's sprite.
		base.Update ();

		if (ammo <= 0) {
			foreach (Rocket rk in GetComponentsInChildren<Rocket>()) {
				rk.GetComponent<SpriteRenderer> ().enabled = false;
				rk.Active = false;
			}
		} else if (active && 
			GetComponentInChildren<Rocket> () != null &&
			GetComponentInChildren<Rocket> ().GetComponent<SpriteRenderer> () != null) {

			if (!GetComponentInChildren<Rocket> ().GetComponent<SpriteRenderer> ().enabled) {
				// This displays rocket sprite when Rocketlauncher what has at least one rocket is enabled.
				GetComponentInChildren<Rocket> ().GetComponent<SpriteRenderer> ().enabled = true;
			}
		}
	}

	public override bool Active {	//active value controls rocket's Sprite Renderer too.
		get {
			return active;
		}
		set {
			active = value;
			sr.enabled = active;
			GetComponent<Collider2D> ().enabled = active;

			foreach (Rocket rk in GetComponentsInChildren<Rocket>()) {
				if (active != rk.GetComponent<SpriteRenderer> ().enabled) {
					rk.GetComponent<SpriteRenderer> ().enabled = active;
				}
				// Rocket's sprite order resetting.
				rk.GetComponent<SpriteRenderer> ().sortingLayerID = sr.sortingLayerID;
				rk.GetComponent<SpriteRenderer> ().sortingOrder = sr.sortingOrder - 1;
			}
		}
	}

	public override void Shot (LayerMask attackLayerMask, LayerMask blockLayerMask) {
		ammo--;
		Active = true;

		float weaponEZ = transform.parent.eulerAngles.z - 90;
		if (weaponEZ < 0)
			weaponEZ += 360;

		Rocket launchedRocket = GetComponentInChildren<Rocket> ();	//Rocket ON
		if (launchedRocket != null) {
			launchedRocket.transform.parent = null;
			launchedRocket.Active = true;
			if(transform.lossyScale.x < 0)
				launchedRocket.GetComponent<ConstantForce2D> ().relativeForce *= -1;
			audioSource.PlayOneShot (soundUse);								//Sound effect
		}
	}
}
