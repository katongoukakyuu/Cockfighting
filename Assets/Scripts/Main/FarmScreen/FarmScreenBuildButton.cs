using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class FarmScreenBuildButton : MonoBehaviour {
	
	public Canvas mainCanvas;
	public Canvas buildStructuresCanvas;

	public ImagePanel imagePanel;

	public void ButtonPressed() {
		imagePanel.SetBuildings (DatabaseManager.Instance.LoadBuildings());

		mainCanvas.gameObject.SetActive (false);
		buildStructuresCanvas.gameObject.SetActive (true);
	}
	
}

