using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using System.IO;
using System.Linq;
using Couchbase.Lite;

public class DatabaseManager : MonoBehaviour {

	private string PATH;
	private Manager manager;
	private Database db;

	private static DatabaseManager instance;
	private DatabaseManager() {}

	public static DatabaseManager Instance {
		get {
			if(instance == null) {
				instance = (DatabaseManager)GameObject.FindObjectOfType(typeof(DatabaseManager));
			}
			return instance;
		}
	}

	void Start () {
		DontDestroyOnLoad(this);
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
			return;
		}
		manager = Manager.SharedInstance;
		db = manager.GetDatabase("cockfighting");
		db.Changed += (sender, e) => {
			var changes = e.Changes.ToList();
			foreach (DocumentChange change in changes) {
				print("Document " + change.DocumentId + " changed");
			}
		};

		// initialize views
		// account username-password
		View viewAccount = db.GetView(Constants.DB_TYPE_ACCOUNT);
		viewAccount.SetMap ((doc, emit) => {
			if(doc["type"].ToString () == Constants.DB_TYPE_ACCOUNT)
				emit(doc["username"], doc["password"]);
		}, "1");

		// chicken name-owner
		View viewChicken = db.GetView(Constants.DB_TYPE_CHICKEN);
		viewChicken.SetMap ((doc, emit) => {
			if(doc["type"].ToString () == Constants.DB_TYPE_CHICKEN)
				emit(doc["name"], doc["owner"]);
		}, "1");

		View viewBuilding = db.GetView(Constants.DB_TYPE_BUILDING);
		viewBuilding.SetMap ((doc, emit) => {
			if(doc["type"].ToString () == Constants.DB_TYPE_BUILDING)
				emit(doc["name"], null);
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

	public List<IDictionary<string,object>> LoadBuildings() {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_BUILDING).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			l.Add (db.GetDocument ("building_" + row.Key).Properties);
		}
		return l;
	}

	public IDictionary<string,object> LoadBuilding(string name) {
		var query = db.GetView (Constants.DB_TYPE_BUILDING).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Key.ToString() == name) {
				return db.GetDocument ("building_" + row.Key).Properties;
			}
		}
		return null;
	}

	public void RegisterAccount(Dictionary<string, object> dic) {
		Document d = db.GetDocument("account_" + dic["username"]);
		var properties = dic;
		var rev = d.PutProperties(properties);

		GenerateChicken(GameManager.Instance.GenerateChicken ("Larry", 
		                                      dic ["username"].ToString(), 
		                                      Constants.GENDER_MALE, 
		                                      "Kelso", 
		                                      Constants.LIFE_STAGE_COCK));
		GenerateChicken(GameManager.Instance.GenerateChicken ("Gary", 
		                                      dic ["username"].ToString(), 
		                                      Constants.GENDER_MALE, 
		                                      "Kelso", 
		                                      Constants.LIFE_STAGE_COCK));
		GenerateChicken(GameManager.Instance.GenerateChicken ("Mary", 
		                                      dic ["username"].ToString(), 
		                                      Constants.GENDER_FEMALE, 
		                                      "Kelso", 
		                                      Constants.LIFE_STAGE_HEN));
		if (rev != null)
			print ("Account registry complete!");
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
		PlayerManager.Instance.player = db.GetDocument ("account_" + username).Properties;
		PlayerManager.Instance.playerChickens = LoadChickens (username);
		print ("Account details:");
		foreach (KeyValuePair<string, object> item in PlayerManager.Instance.player)
			print(item.Key + ": " + item.Value);
		print ("Account's chicken details:");
		foreach (IDictionary<string,object> i in PlayerManager.Instance.playerChickens) {
			foreach (KeyValuePair<string, object> item in i)
				print(item.Key + ": " + item.Value);
		}
	}

	public void GenerateChicken(Dictionary<string, object> dic) {
		Document d = db.GetDocument("chicken_" + dic["owner"] + "_" + dic["name"]);
		var properties = dic;
		var rev = d.PutProperties(properties);
		if (rev != null)
			print ("Chicken generation complete!");
	}

	public List<IDictionary<string,object>> LoadChickens(string username) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_CHICKEN).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			print (row.Key + " " + row.Value);
			if(row.Value.ToString() == username) {
				l.Add (db.GetDocument ("chicken_" + row.Value + "_" + row.Key).Properties);
			}
		}
		return l;
	}

}
