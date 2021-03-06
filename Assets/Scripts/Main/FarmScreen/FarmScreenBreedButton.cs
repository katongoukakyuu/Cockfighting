﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class FarmScreenBreedButton : MonoBehaviour {
	
	public Canvas mainCanvas;
	public Canvas breedCanvas;

	public Animator mainCanvasAnimation;
	
	public void ButtonPressed() {

		mainCanvasAnimation.SetBool("isHidden", true);
		Invoke ("BreedFunctions", 0.2f);

	}

	void BreedFunctions()
	{
		CameraControls.Instance.freeCamera = false;
		mainCanvas.gameObject.SetActive (false);
		breedCanvas.gameObject.SetActive (true);
		BreedsManager.Instance.Initialize ();
	}
	
}

