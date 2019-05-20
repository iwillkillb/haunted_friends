using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

	//They are need to spawn player object.
	public GameObject[] player;
	public Transform[] playerSpawnPoint;

	//UI...
	public Text textConnect, textLogMsg, textRank;
	public GameObject objTextWinner;

	bool isBattleStart = false; // This becomes true when at least 2 players connect.
	BasePlayer winnerPlayer;

	PhotonView pv;

	public static GameManager instance;	//Singletone

	void Awake () {
		if (instance) {
			DestroyImmediate (gameObject);
			return;
		}
		instance = this;

		pv = GetComponent<PhotonView> ();

		CreatePlayer ();
		PhotonNetwork.isMessageQueueRunning = true;	//This must be true after false.

		GetConnectPlayerCount ();
	}



	void Start () {
		//string msg = "\n<color=#00ff00>[" + PhotonNetwork.player.NickName + "] Connected</color>";
		//pv.RPC ("LogMsg", PhotonTargets.AllViaServer, msg);
		PrintLogMsg ("[" + PhotonNetwork.player.NickName + "] connected.", "#00ff00");
	}

	void OnEnable () {
		StartCoroutine (DisplayRank ());
	}

	void OnDisable () {
		StopCoroutine (DisplayRank ());
	}



	void CreatePlayer () {
		int selectSpawnPointNum = (PhotonNetwork.countOfPlayersInRooms + Random.Range (0, playerSpawnPoint.Length)) % playerSpawnPoint.Length;

		if(PlayerPrefs.GetInt ("USER_CHAR") < player.Length)
			PhotonNetwork.Instantiate (player[PlayerPrefs.GetInt ("USER_CHAR")].name, playerSpawnPoint[selectSpawnPointNum].position, Quaternion.identity, 0);
		else
			PhotonNetwork.Instantiate (player[0].name, playerSpawnPoint[selectSpawnPointNum].position, Quaternion.identity, 0);
	}



	void GetConnectPlayerCount () {
		Room currRoom = PhotonNetwork.room;
		textConnect.text = currRoom.PlayerCount.ToString () + "/20";
	}

	void OnPhotonPlayerConnected (PhotonPlayer newPlayer) {
		GetConnectPlayerCount ();
	}

	void OnPhotonPlayerDisconnected (PhotonPlayer outPlayer) {
		GetConnectPlayerCount ();
	}



	public void OnClickExitRoom () {
		//string msg = "\n<color=#ff0000>[" + PhotonNetwork.player.NickName + "] Disconnected</color>";
		//pv.RPC ("LogMsg", PhotonTargets.All, msg);
		PrintLogMsg ("[" + PhotonNetwork.player.NickName + "] has left the game.", "#00ff00");

		PhotonNetwork.LeaveRoom ();
	}

	void OnLeftRoom () {
		Debug.Log ("Exit room.");
		SceneManager.LoadScene ("Lobby");
	}

	void OnApplicationQuit () {
		PhotonNetwork.CloseConnection (PhotonNetwork.player);
	}



	// This can be called by both this code and outer code.
	public void PrintLogMsg (string msg) {
		msg = "\n" + msg;
		pv.RPC ("LogMsg", PhotonTargets.AllViaServer, msg);
	}
	// With color.
	public void PrintLogMsg (string msg, string colorCode) {
		msg = "\n<color=" + colorCode + ">" + msg + "</color>";
		pv.RPC ("LogMsg", PhotonTargets.AllViaServer, msg);
	}

	[PunRPC]
	void LogMsg (string msg) {
		textLogMsg.text = textLogMsg.text + msg;
	}



	// Display score rank in UI.
	// And, Finding winner in game is here.
	IEnumerator DisplayRank () {
		while (true) {
			string rankText = "<color=#ffc000>Score Rank\n</color>";
			BasePlayer[] bps = FindObjectsOfType<BasePlayer> ();

			// Battle start!!
			if (bps.Length >= 2 && isBattleStart == false) {
				isBattleStart = true;
			}

			BasePlayer temp;	//Sort by score.
			for (int a = 0; a < bps.Length - 1; a++) {
				for (int b = a; b < bps.Length; b++) {
					if (bps [a].score < bps [b].score) {
						temp = bps [a];
						bps [a] = bps [b];
						bps [b] = temp;
					}
				}
			}

			for (int a = 0; a < bps.Length; a++) {
				if (bps [a].GetComponent<PhotonView> () != null)
					rankText = "<color=#ffffff>" + rankText + bps [a].GetComponent<PhotonView> ().owner.NickName + " </color><color=#ffff00>" + bps [a].score + "\n</color>";
				winnerPlayer = bps [0];	// Save winner data for FindWinner().
			}
			textRank.text = rankText;

			// Find winner. Maybe 1st player is winner if it living.
			if (isBattleStart && bps.Length <= 1 && !IsInvoking("FindWinner")) {
				Invoke ("FindWinner", 3f);
			}

			yield return new WaitForSeconds (1f);
		}
	}

	void FindWinner () {
		bool allDied = true;
		BasePlayer[] bps = FindObjectsOfType<BasePlayer> ();

		foreach (BasePlayer bp in bps) {
			if (bp.hp > 0) {
				allDied = false;
				break;
			}
		}

		if (allDied) {
			pv.RPC ("Winner", PhotonTargets.AllViaServer, "<color=#ff0000>All died.</color>");
		} else {
			pv.RPC ("Winner", PhotonTargets.AllViaServer, "<color=#00ff00>" + winnerPlayer.GetComponent<PhotonView> ().owner.NickName + "</color> won.");
		}
	}







	[PunRPC]
	void Winner (string winnerText) {
		if (objTextWinner != null) {
			objTextWinner.SetActive (true);

			// Use winner's name.
			objTextWinner.GetComponentInChildren<Text> ().text = winnerText;

			// Everyone, Get out room.
			if (!IsInvoking("GetOutRoom"))
				Invoke("GetOutRoom", 3f);
		}
	}

	void GetOutRoom () {
		PhotonNetwork.LeaveRoom ();
	}
}
