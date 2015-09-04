using UnityEngine;
using System.Collections;

public class FightScreenQueueForMatch : MonoBehaviour {

	public Canvas createMatchCanvas;
	
	public void ButtonPressed() {
		createMatchCanvas.gameObject.SetActive (true);
		MatchCreateManager.Instance.Initialize(null);
	}
}
