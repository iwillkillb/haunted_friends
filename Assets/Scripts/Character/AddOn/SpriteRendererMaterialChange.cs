using UnityEngine;
using System.Collections;

public class SpriteRendererMaterialChange : MonoBehaviour {

	public Material spriteRendererMaterial;

	SpriteRenderer[] srs;

	// Change sprite renderer's material.
	void OnEnable () {
		if (spriteRendererMaterial != null) {
			srs = GetComponentsInChildren<SpriteRenderer> ();

			foreach (SpriteRenderer sr in srs) {
				sr.material = spriteRendererMaterial;
			}
		}
	}

}
