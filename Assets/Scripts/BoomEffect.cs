using UnityEngine;
using System.Collections;

public class BoomEffect : MonoBehaviour {
	// Boom effect object is called by Rocket weapon or Bomb etc.

	public float damage = 100f, colliderCheckTimeOrigin = 0.1f;
	public int rayCount = 16; // When Boom is activated, This makes many rays.
	public LayerMask blockLayerMask; // ray can't colliders have this layer mask.
	public AudioClip soundBoom; // Audio.

	AudioSource audioSource; // radio. This makes sound.
	ParticleSystem [] ps; // light and smoke effect.
	CircleCollider2D lengthCol; //This collider sets Point Effector.

	float lifeTime = 0.1f, colliderCheckTime = 0;
	bool init = false;

	[System.NonSerialized]public int boomMakerId; // Who makes this boom effect?



	void Awake () {
		ps = GetComponentsInChildren<ParticleSystem> ();
		lengthCol = GetComponent<CircleCollider2D> ();
		audioSource = GetComponent<AudioSource> ();

		// Lifetime = The loggest lifetime in array of particles.
		foreach (ParticleSystem psin in ps) {
			lifeTime = lifeTime > psin.startLifetime ? lifeTime : psin.startLifetime;
		}
	}

	void FixedUpdate () {
		// After colliderCheckTime, Collider turn it off.
		if (colliderCheckTime < colliderCheckTimeOrigin)
			colliderCheckTime += Time.deltaTime;
		else {
			lengthCol.enabled = false;
		}
	}

	void OnEnable () {
		// When Object is activated, this code is activated.

		// This part blocks wrong call in Game start time.
		if (!init) {
			init = true;
			gameObject.SetActive (false);
			return;
		}

		// All Particles ON.
		if (ps != null) {
			foreach (ParticleSystem pars in ps) {
				pars.Play ();
			}
		}
		
		Boom ();

		// After lifetime, This turn it off automatically.
		Invoke ("Delete", lifeTime);
	}

	void Delete () {
		// Collider turn it on for next call time.
		lengthCol.enabled = true;
		colliderCheckTime = 0;

		gameObject.SetActive (false);
	}

	void Boom () {
		// Count of ray collider = rayCount * Count of penetrated collider by a ray.
		RaycastHit2D[][] aimRay = new RaycastHit2D [rayCount][];
		float weaponEZ = 0; // Ray's angle(degree). First is zero.
		Vector3 rotV; //What Vector will rotate?
		BaseCharacter bc; // Hitted Collider's Character component.

		// aimRay.Length = Count of ray = rayCount.
		for (int a = 0; a < aimRay.Length; a++) {
			// Control vector by angle(weaponEZ).
			rotV = Quaternion.Euler (0, 0, weaponEZ) * Vector3.right;

			// One penetratable ray.
			aimRay [a] = Physics2D.RaycastAll (transform.position, rotV, lengthCol.radius);
			for (int b = 0; b < aimRay [a].Length; b++) {
				// No penetrate wall...
				if (aimRay [a] [b].collider != null && ((1 << aimRay [a] [b].collider.gameObject.layer) & blockLayerMask) != 0)
					break;
				
				// Give damage to enemy, But don't shoot myself.
				bc = aimRay [a] [b].collider.GetComponentInParent<BaseCharacter> ();
				if (bc != null && bc != GetComponent<BaseCharacter> ()) {
					bc.hp -= damage;
				}
			}
			// Change angle.
			weaponEZ += 360 / aimRay.Length;
		}
		// Sound
		audioSource.PlayOneShot (soundBoom);

		//PlusBoomMakerScore ();
	}

	// BoomMaker score plus
	void PlusBoomMakerScore () {
		BasePlayer[] bps = FindObjectsOfType<BasePlayer> ();
		foreach (BasePlayer bp in bps) {
			if (bp.GetComponent<PhotonView> () != null && bp.GetComponent<PhotonView> ().viewID == boomMakerId) {
				bp.score++;
			}
		}
	}

}
