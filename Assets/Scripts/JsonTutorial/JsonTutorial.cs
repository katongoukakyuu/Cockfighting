using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class JsonTutorial : MonoBehaviour {

	public string fileName;
	public Sandwiches sandwiches;

	private string PATH;

	// Use this for initialization
	private void Start () {
		PATH = Application.dataPath + "/../testData/";
	}
	
	private void OnGUI() {
		if(GUILayout.Button("SAVE")) {
			SerializeAndSave();
		}
		if(GUILayout.Button("LOAD")) {
			LoadAndDeserialize();
		}
	}

	private void SerializeAndSave() {
		string data = JsonWriter.Serialize (sandwiches);
		if(!Directory.Exists (PATH)) {
			Directory.CreateDirectory(PATH);
		}
		var streamWriter = new StreamWriter(PATH + fileName + ".txt");
		streamWriter.Write (data);
		streamWriter.Close ();
	}

	private void LoadAndDeserialize() {
		var streamReader = new StreamReader(PATH + fileName + ".txt");
		string data = streamReader.ReadToEnd();
		streamReader.Close ();

		sandwiches = JsonReader.Deserialize<Sandwiches>(data);
	}
}
