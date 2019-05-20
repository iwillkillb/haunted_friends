using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterButton : MonoBehaviour {
	// This code and "CharacterSelect" code are for only Button of character.

	public void OnClickCharacterButton () {
		GetComponentInParent<CharacterSelect> ().SelectCharacterButton (GetComponent<Button>());
	}
}
