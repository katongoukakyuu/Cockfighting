using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class FarmScreenFeedButton : MonoBehaviour {
	
	public Canvas mainCanvas;
	public Canvas feedCanvas;

	public Animator mainCanvasAnimation;
	
	public void ButtonPressed() {
		mainCanvasAnimation.SetBool("isHidden", true);
		Invoke ("FeedFunctions", 0.2f);
	}

	void FeedFunctions()
	{
		CameraControls.Instance.freeCamera = false;
		mainCanvas.gameObject.SetActive (false);
		feedCanvas.gameObject.SetActive (true);
		FeedsManager.Instance.Initialize ();
	}
	
}

