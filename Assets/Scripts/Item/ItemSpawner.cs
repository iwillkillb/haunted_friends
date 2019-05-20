using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour {

	public GameObject[] items;

	// Use this for initialization
	void Start () {
		int selectIndex = Random.Range (0, items.Length);

		if (items[selectIndex] != null) {
			Instantiate (items [selectIndex], transform.position, new Quaternion (0,0,0,0));
		}
	}
}
