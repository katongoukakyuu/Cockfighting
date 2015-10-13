using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class FarmScreenStoreButton : MonoBehaviour {
	
	public Canvas mainCanvas;
	public Canvas storeCanvas;

	public Animator mainCanvasAnimation;
	
	public void ButtonPressed() {
		mainCanvasAnimation.SetBool("isHidden", true);
		Invoke ("StoreFunction", 0.2f);
	}

	void StoreFunction()
	{
		CameraControls.Instance.freeCamera = false;
		mainCanvas.gameObject.SetActive (false);
		storeCanvas.gameObject.SetActive (true);
		StoreManager.Instance.Initialize (0);
	}
	
}

