using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapSelect : MonoBehaviour {
	// For map select panel...

	[System.Serializable]public class Map
	{
		public string name; // Map's name = Scene file name.
		public Sprite image;
	}
	public Map[] maps;

	[System.NonSerialized]
	public int selectIndex = 0; // maps's index

	[Header("Components")]
	public Text textMapName;
	public Image imageMap;



	void Awake () {
		SetMapNameAndImage ();
	}

	// They are Event of buttons. They change index number one by one.
	public void OnClickIndexPlus () {
		// When Loading, You can't click button.
		if (!PhotonInit.instance.canClickBtn)
			return;

		// selectIndex plus.
		if (selectIndex < maps.Length - 1) {
			selectIndex++;
		} else {
			selectIndex = 0;
		}
		SetMapNameAndImage ();
	}

	public void OnClickIndexMinus () {
		// When Loading, You can't click button.
		if (!PhotonInit.instance.canClickBtn)
			return;

		// selectIndex minus.
		if (selectIndex > 0) {
			selectIndex--;
		} else {
			selectIndex = maps.Length - 1;
		}
		SetMapNameAndImage ();
	}

	// Change panel's map text and image.
	void SetMapNameAndImage () {
		textMapName.text = maps [selectIndex].name;
		imageMap.sprite = maps [selectIndex].image;
		PhotonInit.instance.roomSceneName = maps [selectIndex].name;	//Scene name select.
	}

}
