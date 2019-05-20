using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour {
	// Change/Reset Scene or Exit by game.

	public string sceneName;
	BaseCharacter ps;

	// If the object with this code have Collider, Then this changes scene when living player contacts this collider.
	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			ps = col.transform.parent.GetComponentInParent<BaseCharacter> ();
			if (ps.hp > 0) {
				ChangeScene ();
			}
		}
	}

	public void ChangeScene () {
		SceneManager.LoadScene (sceneName);
	}
	public void ResetScene () {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
	public void GameQuit () {
		Application.Quit ();
	}
}
