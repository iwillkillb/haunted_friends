using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BGMButton : MonoBehaviour {

	public Sprite btnBgmOn, btnBgmOff;
	public AudioSource bgmAudioSource;
	public Image btnImg;

	// Bgm button's event.
	public void OnClickBgmButton () {
		if (bgmAudioSource.isPlaying) {
			btnImg.sprite = btnBgmOff;
			bgmAudioSource.Pause ();

		} else {
			btnImg.sprite = btnBgmOn;
			bgmAudioSource.Play ();
		}
	}
}
