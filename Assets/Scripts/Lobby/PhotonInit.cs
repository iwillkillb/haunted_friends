using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PhotonInit : Photon.MonoBehaviour {
	// This code is most important in Lobby scene!
	// This connects to Photon Network and loads room array's data.

	public static PhotonInit instance; // This is Singletone code.

	public string version = "0.1";

	public InputField userId;
	public InputField roomName;

	// For list of rooms.
	public GameObject scrollContents;
	public GameObject roomItem;

	public int charNum = 0; // Selected character's index.

	[System.NonSerialized]public bool canClickBtn = false;
	public string roomSceneName;	// What load scene's name in the room?
	public GameObject loadingObj;	// Loading image.

	void Awake () {
		// For singletone function.
		if (instance) {
			DestroyImmediate (gameObject);
			return;
		}
		instance = this;

		if (!PhotonNetwork.connected) {	//Exit from Field -> ReConnect
			PhotonNetwork.ConnectUsingSettings (version);
		}
		userId.text = GetUserId (); // Get ID.

		PhotonNetwork.ConnectUsingSettings (version);	//Connect to Photon Server
		roomName.text = "ROOM_" + Random.Range (0,999).ToString ("000");
	}



	// Print PhotonNetwork's status in screen's Top-Left side.
	void OnGUI () {
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());
	}



	void OnJoinedLobby () { // Entered Lobby.
		userId.text = GetUserId (); // Get ID.
		canClickBtn = true; // You can click a button.
		loadingObj.SetActive (!canClickBtn); // canClickBtn set -> loadingObj.SetActive = !canClickBtn
	}

	string GetUserId () {	//Return Player's name saved in local or instantiate it.
		string userId = PlayerPrefs.GetString ("USER_ID"); // Set ID.
		// If ID field is null, Then ID field sets automatically.
		if (string.IsNullOrEmpty (userId))
			userId = "USER_" + Random.Range (0, 999).ToString ("000");
		
		return userId;
	}



	void OnPhotonRandomJoinFailed () { // No Room! -> Create new room.
		PhotonNetwork.CreateRoom ("MyRoom");
	}



	public virtual void OnJoinedRoom () { // Enter Room.
		StartCoroutine (LoadField ());
	}

	IEnumerator LoadField () {
		PhotonNetwork.isMessageQueueRunning = false; //When System changes Scene, Stop receiving Network massage to Photon Cloud Server.
		AsyncOperation ao = SceneManager.LoadSceneAsync(roomSceneName); // Scene change by roomSceneName.
		
		yield return ao;
	}



	void PreRoom () {	//Set Nickname etc...
		//This is used by CharacterSelect.cs in Character Panel Object.
		PhotonNetwork.player.NickName = userId.text;
		PlayerPrefs.SetString ("USER_ID", userId.text);
		PlayerPrefs.SetInt ("USER_CHAR", charNum);
	}


	/*
	public void OnClickJoinRandomRoom () { // Event of Random Room Button.
		if (canClickBtn == false)
			return;
		else
			canClickBtn = false;
		loadingObj.SetActive (!canClickBtn);
		
		PreRoom ();
		PhotonNetwork.JoinRandomRoom ();
	}
	*/


	public void OnClickCreateRoom () {	//Event of Make Room Button.
		// Loading panel on -> You can't click any button.
		if (canClickBtn == false)
			return;
		else
			canClickBtn = false;
		loadingObj.SetActive (!canClickBtn);

		string _roomName = roomName.text;
		if (string.IsNullOrEmpty (roomName.text)) {
			_roomName = "ROOM_" + Random.Range (0, 999).ToString ("000");
		}
		_roomName = roomSceneName + '@' + _roomName;	//Save scene name in the room.
		// room's name = [Scene's name]@[Room's name].
		// System checks room's scene by room's name!

		PreRoom ();

		RoomOptions roomOptions = new RoomOptions ();	//Set condition of room.
		roomOptions.IsOpen = true;
		roomOptions.IsVisible = true;
		roomOptions.MaxPlayers = 20;
		PhotonNetwork.CreateRoom (_roomName, roomOptions, TypedLobby.Default);	//Create Room.
	}



	public void OnReceivedRoomListUpdate () { // Get list of rooms.

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("RoomItem")) {	//Reset Room List.
			Destroy (obj);
		}

		int rowCount = 0;	//Value for Constrait Count by "Grid Layout Group" Component.
		scrollContents.GetComponent<RectTransform> ().sizeDelta = Vector2.zero;

		//Take room list in Network -> Show the list.
		foreach (RoomInfo _room in PhotonNetwork.GetRoomList()) {
			GameObject room = (GameObject)Instantiate (roomItem);
			room.transform.SetParent (scrollContents.transform, false);

			//Transfer text data. Set room's RoomData component.
			RoomData roomData = room.GetComponent<RoomData> ();
			roomData.roomName = _room.Name;
			roomData.connectPlayer = _room.PlayerCount;
			roomData.maxPlayers = _room.MaxPlayers;

			roomData.DispRoomData ();	//Display room data.
			// Give event to room button.
			roomData.GetComponent<UnityEngine.UI.Button> ().onClick.AddListener (delegate {
				OnClickRoomItem (roomData);
			});

			scrollContents.GetComponent<GridLayoutGroup> ().constraintCount = ++rowCount;
			scrollContents.GetComponent<RectTransform> ().sizeDelta += new Vector2 (0, 20);
		}
	}



	public void OnClickRoomItem (RoomData room) {	//RoomItem Click Event.
		if (canClickBtn == false)
			return;
		else
			canClickBtn = false;
		loadingObj.SetActive (!canClickBtn);

		PreRoom ();

		// room's name = [Scene's name]@[Room's name].
		// This line takes [Scene's name] in room's name.
		string[] spliteds = room.roomName.Split('@');
		roomSceneName = spliteds [0];

		PhotonNetwork.JoinRoom (room.roomName);
	}
}
