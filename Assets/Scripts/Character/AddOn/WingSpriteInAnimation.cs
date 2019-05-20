using UnityEngine;
using System.Collections;

public class WingSpriteInAnimation : MonoBehaviour {

	public SpriteRenderer headSR, wingFSR, wingBSR;
	public Sprite[] wingFSprites, wingBSprites;



	public void WingSpriteChangeInAnim (int index) {

		if (index < 0)	//index Clamping.
			index = 0;

		if (index < wingFSprites.Length)
			wingFSR.sprite = wingFSprites [index];
		else
			wingFSR.sprite = wingFSprites [wingFSprites.Length - 1];

		if (index < wingBSprites.Length)
			wingBSR.sprite = wingBSprites [index];
		else
			wingBSR.sprite = wingBSprites [wingBSprites.Length - 1];

		if (index == 0) {
			if (headSR != null && wingFSR != null && wingFSR.sortingOrder != headSR.sortingOrder + 1)
				wingFSR.sortingOrder = headSR.sortingOrder + 1;
			if (headSR != null && wingBSR != null && wingBSR.sortingOrder != headSR.sortingOrder + 1)
				wingBSR.sortingOrder = headSR.sortingOrder + 1;
		} else {
			if (headSR != null && wingFSR != null && wingFSR.sortingOrder != headSR.sortingOrder + 1)
				wingFSR.sortingOrder = headSR.sortingOrder + 1;
			if (headSR != null && wingBSR != null && wingBSR.sortingOrder != headSR.sortingOrder - 1)
				wingBSR.sortingOrder = headSR.sortingOrder - 1;
		}
	}
}
