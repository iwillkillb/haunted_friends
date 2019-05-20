using UnityEngine;
using System.Collections;

public class ItemNetwork : Photon.MonoBehaviour {

	Item item;

	// Use this for initialization
	void Awake () {
		item = GetComponent<Item> ();
	}

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {	//This activates well. But Values isn't transmitted.
		BaseHumanoid bh = GetComponentInParent<BaseHumanoid> ();

		if (stream.isWriting) {			//Send data
			stream.SendNext (item.active);
			stream.SendNext (item.isEquiped);
			if (bh != null)
				stream.SendNext (bh.transform.position + transform.position);

		} else if (stream.isReading) {	//Receive data
			item.active = (bool)stream.ReceiveNext ();
			item.isEquiped = (bool)stream.ReceiveNext ();
			if (bh != null)
				transform.position = (Vector3)stream.ReceiveNext ();
		}


	}
}
