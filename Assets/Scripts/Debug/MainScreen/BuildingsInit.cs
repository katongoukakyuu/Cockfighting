using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class BuildingsInit : MonoBehaviour {
	
	private string PATH;
	private Buildings buildings = new Buildings();
	
	private void Start () {
		PATH = Application.dataPath + "/../json/";
		/*{"buildings":[{"id":"1","name":"Hen Coop","xSize":"1","ySize":"2","xCenter":"0","yCenter":"0","prefabName":"Hen Coop","imageName":"Hen Coop.jpg"}]}*/
		Building b = new Building ();
		b.id = 1;
		b.name = "Hen Coop";
		b.xSize = 2;
		b.ySize = 1;
		b.xCenter = 0;
		b.yCenter = 0;
		b.prefabName = "Hen Coop";
		b.imageName = "Hen Coop.jpg";
		buildings.b.Add (b);
		string data = JsonWriter.Serialize (buildings);
		if(!Directory.Exists (PATH)) {
			Directory.CreateDirectory(PATH);
		}
		var streamWriter = new StreamWriter(PATH + "buildings.txt");
		streamWriter.Write (data);
		streamWriter.Close ();
	}
	
}
