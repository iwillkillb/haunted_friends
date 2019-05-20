using UnityEngine;
using System.Collections;

public class AfterFunction : MonoBehaviour {
	// This code defines effects after delay.

	public enum Effect {SetActive, Destroy, Spawn, PoolCall}; // You can select after effect.
	public Effect effect = Effect.SetActive;

	public float afterTime = 1f; // Effect's delay time.
	float lifeDelay = 0; // Effect's life time.

	public bool afterSetActive = false; // After delay, effect on? or off?

	public GameObject spawnChild; // After delay, What is spawned?
	
	// Update is called once per frame
	void FixedUpdate () {
		if (lifeDelay < afterTime) {
			lifeDelay += Time.deltaTime;
		} else {
			lifeDelay = 0; // After delay...
			switch (effect) {
			case Effect.SetActive: // Set active in hierarchy.
				gameObject.SetActive (afterSetActive);
				break;

			case Effect.Destroy: // Delete object in memory.
				Destroy (gameObject);
				break;

			case Effect.Spawn: // Spawn someone.
				if (spawnChild != null)
					Instantiate (spawnChild, transform.position, transform.rotation);
				gameObject.SetActive (afterSetActive);
				break;

			case Effect.PoolCall: // Call "Effect Boom" Object to Object pool.
				ObjectPooling.instance.MakeFind("Effect Boom", transform.position, transform.rotation);
				gameObject.SetActive (afterSetActive);
				break;
			}
		}
	}

	public void Reset () { // Reset delay.
		lifeDelay = 0;
	}
}
