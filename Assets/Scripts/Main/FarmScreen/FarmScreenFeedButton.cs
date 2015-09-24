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
		mainCanvas.gameObject.SetActive (false);
		feedCanvas.gameObject.SetActive (true);
		Camera.main.GetComponent<GridOverlay>().ToggleCanHoverOnMap(false);
		FeedsManager.Instance.Initialize ();
	}
	
}

