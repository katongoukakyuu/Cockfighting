using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class FarmScreenFightButton : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas fightCanvas;


	public Animator mainCanvasAnimation;
	

	public void ButtonPressed() {

		mainCanvasAnimation.SetBool("isHidden", true);
		Invoke ("FightFunctions", 0.2f);
	}

	void FightFunctions()
	{
		CameraControls.Instance.freeCamera = false;
		fightCanvas.gameObject.SetActive (true);
		mainCanvas.gameObject.SetActive (false);
		FightManager.Instance.Initialize ();
	}

}

