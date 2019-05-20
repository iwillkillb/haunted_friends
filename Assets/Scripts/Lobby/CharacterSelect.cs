using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterSelect : MonoBehaviour {
	// For Panel of character select, This code defines character selecting function and button's color.
	
	Button[] buttons; // All character buttons.

	PhotonInit pi;

	void Awake () {
		buttons = GetComponentsInChildren<Button> ();
	}

	// Use this for initialization
	void Start () {
		pi = FindObjectOfType<PhotonInit> ();
	}

	public void SelectCharacterButton (Button pressedButton) {	//Activate by Child button object.
		// To all buttons...
		for (int a = 0; a < buttons.Length; a++) {
			if (pressedButton == buttons [a]) { // Is this selected button?
				pi.charNum = a; // To PhotonInit Component...
			}
		}
	}
}
