using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterProfile : MonoBehaviour {
	// Character's profile text event.

	[TextArea]
	public string[] charProfiles;
	public Text text;

	public void ChangeProfile (int index) {
		text.text = charProfiles [index];
	}
}
