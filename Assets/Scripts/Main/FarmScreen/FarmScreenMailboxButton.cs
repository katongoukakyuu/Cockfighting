﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class FarmScreenMailboxButton : MonoBehaviour {
	
	public Canvas mainCanvas;
	public Canvas mailboxCanvas;

	public Animator mainCanvasAnimation;
	
	public void ButtonPressed() {
		mainCanvasAnimation.SetBool("isHidden", true);
		Invoke ("MailFunctions", 0.2f);
	}

	void MailFunctions()
	{
		CameraControls.Instance.freeCamera = false;
		mainCanvas.gameObject.SetActive (false);
		mailboxCanvas.gameObject.SetActive (true);
		MailboxManager.Instance.Initialize ();
	}
	
}

