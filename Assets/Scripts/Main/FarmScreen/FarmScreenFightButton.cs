using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class FarmScreenFightButton : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas fightCanvas;

	public void ButtonPressed() {
		mainCanvas.gameObject.SetActive (false);
		fightCanvas.gameObject.SetActive (true);
		FightManager.Instance.Initialize ();
	}
	
}

