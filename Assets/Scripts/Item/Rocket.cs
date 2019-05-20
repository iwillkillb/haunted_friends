using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {

	float lifeTimeCount = 0;
	public float lifeTime = 10f;
	public LayerMask hitLayerMask;
	public GameObject effectObjectAfterCrash;

	[System.NonSerialized]public RocketLauncher launcher;
	[System.NonSerialized]public int launcherId = -1;

	bool canExplosion = false, active = false;



	void OnEnable () {
		lifeTimeCount = 0;
		canExplosion = true;
	}

	void FixedUpdate () {
		if (canExplosion) {
			if (lifeTimeCount >= lifeTime) {
				lifeTimeCount = 0;
				canExplosion = false;
				gameObject.SetActive (false);
			} else {
				lifeTimeCount += Time.deltaTime;
			}
		}

		if (!launcher.isEquiped && transform.parent != null) {
			if (transform.localPosition != Vector3.zero)
				transform.localPosition = Vector3.zero;
			if (transform.localEulerAngles != Vector3.zero)
				transform.localEulerAngles = Vector3.zero;
		}
	}

	void OnCollisionEnter2D (Collision2D col) { // If this contacts other object...
		if (canExplosion && ((1 << col.gameObject.layer) & hitLayerMask) != 0) {
			if (launcher == col.transform.GetComponentInChildren<RocketLauncher> ())
				return;
			canExplosion = false;

			//Spawn Boom effect. -> Use object pool.
			if (effectObjectAfterCrash != null)
				ObjectPooling.instance.MakeEffectBoom (transform.position, transform.rotation, launcherId);
			/*
			if (effectObjectAfterCrash != null)
				ObjectPooling.instance.MakeFind (effectObjectAfterCrash.name, transform.position, transform.rotation);
			*/
			gameObject.SetActive (false);
		}
	}

	public bool Active {	//This is activated by Rocket Launcher Obj.
		get {
			return active;
		}
		set {
			active = value;
			foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>()) {
				if (active) {
					ps.Play ();
				} else {
					ps.Stop ();
					launcherId = -1;	// Delete launcher's id.
				}
			}
			GetComponent<Collider2D> ().enabled = active;
			GetComponent<Rigidbody2D> ().isKinematic = !active;
			GetComponent<ConstantForce2D> ().enabled = active;
			canExplosion = active;	//Rocket Launcher limits their Explosion Mode.
		}
	}
}
