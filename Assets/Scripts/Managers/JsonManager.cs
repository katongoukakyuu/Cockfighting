using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using System.IO;
using System.Linq;
using Couchbase.Lite;

public class JsonManager : MonoBehaviour {

	private string PATH;
	private Manager manager;
	private Database db;

	private static JsonManager instance;
	private JsonManager() {}

	public static JsonManager Instance {
		get {
			if(instance == null) {
				instance = (JsonManager)GameObject.FindObjectOfType(typeof(JsonManager));
			}
			return instance;
		}
	}

	void Start () {
		DontDestroyOnLoad(this);
		Instance.PATH = Application.dataPath + "/../json/";
		manager = Manager.SharedInstance;
		db = manager.GetDatabase("cockfighting");
		db.Changed += (sender, e) => {
			var changes = e.Changes.ToList();
			foreach (DocumentChange change in changes) {
				print("Document " + change.DocumentId + " changed");
			}
		};

		// initialize views
		View view = db.GetView(Constants.DB_TYPE_ACCOUNT);
		view.SetMap ((doc, emit) => {
			if(doc["type"].ToString () == Constants.DB_TYPE_ACCOUNT)
				emit(doc["username"], doc["password"]);
		}, "1");
	}

	public Database GetDatabase() {
		return db;
	}

	public void SaveBuildings(Buildings b) {
		Instance.PATH = Application.dataPath + "/../json/";
		string data = JsonWriter.Serialize (b);
		if(!Directory.Exists (Instance.PATH)) {
			Directory.CreateDirectory(Instance.PATH);
		}
		var streamWriter = new StreamWriter(Instance.PATH + "buildings.txt");
		streamWriter.Write (data);
		streamWriter.Close ();
	}

	public Buildings LoadBuildings() {
		var streamReader = new StreamReader(Instance.PATH + "buildings.txt");
		string data = streamReader.ReadToEnd();
		streamReader.Close ();
		
		Buildings b = JsonReader.Deserialize<Buildings>(data);
		return b;
	}

	public void RegisterAccount(Dictionary<string, object> dic) {
		Document d = db.GetDocument("account_" + dic["username"]);
		var properties = dic;
		var rev = d.PutProperties(properties);


	}

	public bool LoginAccount(string username, string password) {
		var query = db.GetView (Constants.DB_TYPE_ACCOUNT).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			print (row.Key + " " + row.Value);
			if(row.Key.ToString() == username && row.Value.ToString() == password) {
				print ("account found!");
				UpdatePlayer (username);
				return true;
			}
		}
		return false;
	}

	public void UpdatePlayer(string username) {
		GameManager.Instance.player = db.GetDocument ("account_" + username).Properties;
		foreach (KeyValuePair<string, object> item in GameManager.Instance.player)
			print(item.Key + ": " + item.Value);
	}

}
