using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class FarmScreenFeedButton : MonoBehaviour {
	
	public Canvas mainCanvas;
	public Canvas feedCanvas;
	
	public void ButtonPressed() {
		mainCanvas.gameObject.SetActive (false);
		feedCanvas.gameObject.SetActive (true);
		FeedsManager.Instance.Initialize ();
	}
	
}

