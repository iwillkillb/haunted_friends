using UnityEngine;
using System.Collections;

public class ButtonSound : MonoBehaviour {
	// Play setted sound.

	public AudioSource audioS;
	public AudioClip soundClick;
	
	public void OnClickSound () {
		audioS.PlayOneShot (soundClick);
	}
}
