using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class BuildButton : MonoBehaviour {
	
	public Canvas mainCanvas;
	public Canvas buildStructuresCanvas;

	public ImagePanel imagePanel;

	private string PATH;
	private Buildings buildings;

	void Start () {
		PATH = Application.dataPath + "/../json/";
	}

	public void ButtonPressed() {
		var streamReader = new StreamReader(PATH + "/buildings.txt");
		string data = streamReader.ReadToEnd();
		streamReader.Close ();
		
		buildings = JsonReader.Deserialize<Buildings>(data);

		imagePanel.SetBuildings (buildings.b);

		mainCanvas.gameObject.SetActive (false);
		buildStructuresCanvas.gameObject.SetActive (true);
	}
	
}

