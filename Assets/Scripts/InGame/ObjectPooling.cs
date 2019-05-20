using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooling : MonoBehaviour {
	// This singletone class take charge object's recycle for saving memory.
	// Call from out -> Object on -> Use it -> Object off -> Other call -> Object on ...

	public static ObjectPooling instance;
	
	[System.Serializable]public class Pool {
		public GameObject poolObj;   // Recycle what?
		public GameObject parentObj; // Who is object's parent?
		public int amount; // First time, How many object do you make?
		[System.NonSerialized] public List <GameObject> listPool = new List <GameObject> ();
		[System.NonSerialized] public int callIndex = 0;

		public void Init () { // First time, This makes value "amount" number of object.
			if (poolObj != null) {
				for (int a = 0; a < amount; a++) {
					GameObject obj = Instantiate (poolObj) as GameObject;	// Make object.
					if(parentObj != null)
						obj.transform.parent = parentObj.transform; // Parent setting.
					obj.SetActive (false);
					listPool.Add (obj);
				}
			}
		}
	}
	public Pool [] pools; // All Callable objects.

	PhotonView pv;





	void Awake () { // For Singletone.
		if (instance) {
			DestroyImmediate (gameObject);
			return;
		}
		instance = this;

		pv = GetComponent<PhotonView> ();
	}



	void Start () { // Null parent -> Parent is ObjectPool. And Init().
		foreach (Pool pool in pools) {
			if (pool.parentObj == null)
				pool.parentObj = gameObject;
			pool.Init ();
		}
	}





	public void MakeFind (string name, Vector3 pos, Quaternion ros) {   // Find by name -> Call it. Activated by Out of code.
		pv.RPC ("CallObj", PhotonTargets.AllViaServer, name, pos, ros); // Use Network.
	}
	[PunRPC]
	void CallObj (string name, Vector3 pos, Quaternion ros) {
		// Find object whose name is "name" in pools.
		Pool myPool = null;
		foreach (Pool pool in pools) {
			if (pool.poolObj.name == name) {
				myPool = pool;
				break;
			}
		}
		// If there is no object in pools, Quit function.
		if (myPool == null)
			return;

		GameObject makeWhat = null;
		if (myPool.callIndex < myPool.listPool.Count) {	//FIFO call.
			if (myPool.listPool [myPool.callIndex].activeInHierarchy == false)
				makeWhat = myPool.listPool [myPool.callIndex];
			myPool.callIndex++;
		}
		else {
			myPool.callIndex = 0;
			if (myPool.listPool [myPool.callIndex].activeInHierarchy == false)
				makeWhat = myPool.listPool [myPool.callIndex];
		}
		if (makeWhat == null)
			return;

		// Call object.
		// Set position to "pos", rotation to "ros", and parent reset.
		makeWhat.transform.position = pos;
		makeWhat.transform.rotation = ros;
		if(myPool.parentObj != null)
			makeWhat.transform.parent = myPool.parentObj.transform;

		// Rigidbody speed is zero.
		Rigidbody2D rigi = makeWhat.GetComponent<Rigidbody2D> ();
		if (rigi != null)
			rigi.velocity = Vector2.zero;

		// Object on.
		makeWhat.SetActive (true);
	}



	// For only Effect Gun Line.
	public void MakeEffectGunLine (Vector3 start, Vector3 end) {
		pv.RPC ("CallEffectGunLine", PhotonTargets.AllViaServer, start, end); // Use Network.
	}
	[PunRPC]
	void CallEffectGunLine (Vector3 start, Vector3 end) {
		// Find object whose name is "Effect Gun Line" in pools.
		Pool myPool = null;
		foreach (Pool pool in pools) {
			if (pool.poolObj.name == "Effect Gun Line") {
				myPool = pool;
				break;
			}
		}
		// If there is no object in pools, Quit function.
		if (myPool == null)
			return;

		GameObject makeWhat = null;
		if (myPool.callIndex < myPool.listPool.Count) {	//FIFO call.
			if (myPool.listPool [myPool.callIndex].activeInHierarchy == false)
				makeWhat = myPool.listPool [myPool.callIndex];
			myPool.callIndex++;
		}
		else {
			myPool.callIndex = 0;
			if (myPool.listPool [myPool.callIndex].activeInHierarchy == false)
				makeWhat = myPool.listPool [myPool.callIndex];
		}
		if (makeWhat == null)
			return;

		// Call object.
		// Parent reset.
		if(myPool.parentObj != null)
			makeWhat.transform.parent = myPool.parentObj.transform;

		// Setting Line Renderer. This is most important!
		LineRenderer lr = makeWhat.GetComponent<LineRenderer> ();
		if (lr != null) {
			lr.SetPosition (0, start);
			lr.SetPosition (1, end);
		}

		// Rigidbody speed is zero.
		Rigidbody2D rigi = makeWhat.GetComponent<Rigidbody2D> ();
		if (rigi != null)
			rigi.velocity = Vector2.zero;

		// Object on.
		makeWhat.SetActive (true);
	}



	// For only Effect Boom.
	public void MakeEffectBoom (Vector3 pos, Quaternion ros, int boomMakerId) {
		pv.RPC ("CallEffectBoom", PhotonTargets.AllViaServer, pos, ros, boomMakerId); // Use Network.
	}
	[PunRPC]
	void CallEffectBoom (Vector3 pos, Quaternion ros, int boomMakerId) {
		// Find object whose name is "Effect Gun Line" in pools.
		Pool myPool = null;
		foreach (Pool pool in pools) {
			if (pool.poolObj.name == "Effect Boom") {
				myPool = pool;
				break;
			}
		}
		// If there is no object in pools, Quit function.
		if (myPool == null)
			return;

		GameObject makeWhat = null;
		if (myPool.callIndex < myPool.listPool.Count) {	//FIFO call.
			if (myPool.listPool [myPool.callIndex].activeInHierarchy == false)
				makeWhat = myPool.listPool [myPool.callIndex];
			myPool.callIndex++;
		}
		else {
			myPool.callIndex = 0;
			if (myPool.listPool [myPool.callIndex].activeInHierarchy == false)
				makeWhat = myPool.listPool [myPool.callIndex];
		}
		if (makeWhat == null)
			return;

		// Call object.
		// Set position to "pos", rotation to "ros", and parent reset.
		makeWhat.transform.position = pos;
		makeWhat.transform.rotation = ros;
		if(myPool.parentObj != null)
			makeWhat.transform.parent = myPool.parentObj.transform;

		// Setting boomMakerId in BoomEffect.cs
		BoomEffect be = makeWhat.GetComponent<BoomEffect> ();
		if (be != null) {
			be.boomMakerId = boomMakerId;
		}

		// Rigidbody speed is zero.
		Rigidbody2D rigi = makeWhat.GetComponent<Rigidbody2D> ();
		if (rigi != null)
			rigi.velocity = Vector2.zero;

		// Object on.
		makeWhat.SetActive (true);
	}
}
