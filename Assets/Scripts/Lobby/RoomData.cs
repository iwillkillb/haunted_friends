using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomData : MonoBehaviour {
	// This displays Room's name, count of players...

	[HideInInspector]public string roomName = "";
	[HideInInspector]public int connectPlayer = 0;
	[HideInInspector]public int maxPlayers = 0;

	public Text textRoomName, textConnectInfo;



	public void DispRoomData () {
		// room's name = [Scene's name]@[Room's name].
		string printSceneName = roomName.Substring(0, roomName.IndexOf("@"));
		string printRoomName = roomName.Substring(roomName.IndexOf("@") + 1);

		textRoomName.text = "<color=#22814c>[" + printSceneName + "]</color> " + printRoomName;
		textConnectInfo.text = "(" + connectPlayer.ToString() + "/" + maxPlayers.ToString() + ")";
	}
}
