using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Couchbase.Lite;

public class DatabaseManager : MonoBehaviour {

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
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_ACCOUNT)
				emit(doc[Constants.DB_KEYWORD_USERNAME], doc[Constants.DB_KEYWORD_PASSWORD]);
		}, "1");

		// chicken name-owner
		View viewChicken = db.GetView(Constants.DB_TYPE_CHICKEN);
		viewChicken.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_CHICKEN)
				emit(doc[Constants.DB_KEYWORD_NAME], doc[Constants.DB_KEYWORD_OWNER]);
		}, "1");

		// building name
		View viewBuilding = db.GetView(Constants.DB_TYPE_BUILDING);
		viewBuilding.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_BUILDING)
				emit(doc[Constants.DB_KEYWORD_NAME], null);
		}, "1");

		// buildings owned by player
		View viewBuildingOwned = db.GetView(Constants.DB_TYPE_BUILDING_OWNED);
		viewBuildingOwned.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_BUILDING_OWNED)
				emit(doc[Constants.DB_KEYWORD_NAME], doc[Constants.DB_KEYWORD_OWNER]);
		}, "1");

		// feeds
		View viewFeeds = db.GetView(Constants.DB_TYPE_FEEDS);
		viewFeeds.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_FEEDS)
				emit(doc[Constants.DB_KEYWORD_NAME], null);
		}, "1");

		// feeds schedule
		View viewFeedsSchedule = db.GetView(Constants.DB_TYPE_FEEDS_SCHEDULE);
		viewFeedsSchedule.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_FEEDS_SCHEDULE)
				emit(doc[Constants.DB_KEYWORD_CHICKEN_ID], doc[Constants.DB_KEYWORD_FEEDS_ID]);
		}, "1");

		// fighting move
		View viewFightingMove = db.GetView(Constants.DB_TYPE_FIGHTING_MOVE);
		viewFightingMove.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_FIGHTING_MOVE)
				emit(doc[Constants.DB_KEYWORD_NAME], null);
		}, "1");

		// fighting moves owned by chickens
		View viewFightingMovesOwned = db.GetView(Constants.DB_TYPE_FIGHTING_MOVE_OWNED);
		viewFightingMovesOwned.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_FIGHTING_MOVE_OWNED)
				emit(doc[Constants.DB_KEYWORD_CHICKEN_ID], doc[Constants.DB_KEYWORD_FIGHTING_MOVE_ID]);
		}, "1");

		// delete functions, use caution
		//DeleteBuildingsOwnedByPlayer (null);

		// initialize database, use Unity inspector to change value in GameManager
		if (GameManager.Instance.initializeDatabase) {
			InitializeDatabase();
		}
		Destroy (ControlPanelBreedsManager.Instance);
		Destroy (ControlPanelBuildingsManager.Instance);
		Destroy (ControlPanelFeedsManager.Instance);

		if(FightManager.Instance != null) {
			FightManager.Instance.AutomateFight (
				LoadChicken("Gary", "test"),
				LoadChicken("Larry", "test"),
				LoadFightingMovesOwned (LoadChicken("Gary", "test")[Constants.DB_COUCHBASE_ID].ToString()),
				LoadFightingMovesOwned (LoadChicken("Larry", "test") [Constants.DB_COUCHBASE_ID].ToString())
				);
		}
	}

	private void InitializeDatabase() {
		SaveFightingMove (GameManager.Instance.GenerateFightingMove (
			Constants.FIGHT_MOVE_DASH
		));
		SaveFightingMove (GameManager.Instance.GenerateFightingMove (
			Constants.FIGHT_MOVE_FLYING_TALON
		));
		SaveFightingMove (GameManager.Instance.GenerateFightingMove (
			Constants.FIGHT_MOVE_SIDESTEP
		));
		SaveFightingMove (GameManager.Instance.GenerateFightingMove (
			Constants.FIGHT_MOVE_PECK
		));

		RegisterAccount(GameManager.Instance.RegisterAccount ("test", "test@test.com"));
		ControlPanelBreedsManager.Instance.SaveBreed (
			"Kelso",
			"white",80,"black",
			"white",60,"black",
			"white",40,"black",
			"white",50,"black"
		);
		ControlPanelBuildingsManager.Instance.SaveBuilding (
			"Hen Coop", "A coop to house hens with.",
			200, 1000,
			2, 1,
			0, 0,
			"Hen Coop", "Hen Coop"
		);
		ControlPanelFeedsManager.Instance.SaveFeeds (
			"Uber Feeds", "Feeds for uber chickens.",
			10, 10,
			1, 1, 1, 1,
			10, -20, 10, -10, 10, 10,
			"Hen Coop"
		);
	}

	public Database GetDatabase() {
		return db;
	}

	public List<IDictionary<string,object>> LoadBuildings() {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_BUILDING).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			l.Add (db.GetDocument (row.DocumentId).Properties);
		}
		return l;
	}

	public IDictionary<string,object> LoadBuilding(string name) {
		var query = db.GetView (Constants.DB_TYPE_BUILDING).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Key.ToString() == name) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public IDictionary<string,object> RegisterAccount(Dictionary<string, object> dic) {
		Document d = db.CreateDocument();
		var properties = dic;
		var rev = d.PutProperties(properties);

		List<IDictionary<string,object>> chicken = new List<IDictionary<string,object>>();
		chicken.Add (GenerateChicken(GameManager.Instance.GenerateChicken ("Larry", 
		                                      dic [Constants.DB_KEYWORD_USERNAME].ToString(), 
		                                      Constants.GENDER_MALE, 
		                                      "Kelso", 
		                                      Constants.LIFE_STAGE_COCK)));

		chicken.Add (GenerateChicken(GameManager.Instance.GenerateChicken ("Gary", 
		                                      dic [Constants.DB_KEYWORD_USERNAME].ToString(), 
		                                      Constants.GENDER_MALE, 
		                                      "Kelso", 
		                                      Constants.LIFE_STAGE_COCK)));
		chicken.Add (GenerateChicken(GameManager.Instance.GenerateChicken ("Mary", 
		                                      dic [Constants.DB_KEYWORD_USERNAME].ToString(), 
		                                      Constants.GENDER_FEMALE, 
		                                      "Kelso", 
		                                      Constants.LIFE_STAGE_HEN)));

		foreach(IDictionary<string,object> id in chicken) {
			IDictionary<string,object> move;
			if(id[Constants.DB_KEYWORD_LIFE_STAGE].ToString() != Constants.LIFE_STAGE_HEN) {
				SaveFightingMoveOwned (GameManager.Instance.GenerateFightingMoveOwnedByChicken (
					id[Constants.DB_COUCHBASE_ID].ToString(),
					LoadFightingMove(Constants.FIGHT_MOVE_DASH)[Constants.DB_COUCHBASE_ID].ToString()
					));
				SaveFightingMoveOwned (GameManager.Instance.GenerateFightingMoveOwnedByChicken (
					id[Constants.DB_COUCHBASE_ID].ToString(),
					LoadFightingMove(Constants.FIGHT_MOVE_FLYING_TALON)[Constants.DB_COUCHBASE_ID].ToString()
					));
				SaveFightingMoveOwned (GameManager.Instance.GenerateFightingMoveOwnedByChicken (
					id[Constants.DB_COUCHBASE_ID].ToString(),
					LoadFightingMove(Constants.FIGHT_MOVE_SIDESTEP)[Constants.DB_COUCHBASE_ID].ToString()
					));
				SaveFightingMoveOwned (GameManager.Instance.GenerateFightingMoveOwnedByChicken (
					id[Constants.DB_COUCHBASE_ID].ToString(),
					LoadFightingMove(Constants.FIGHT_MOVE_PECK)[Constants.DB_COUCHBASE_ID].ToString()
					));
			}
		}

		if (rev != null) {
			print ("Account registry complete!");
			return rev.Properties;
		}
		return null;
	}

	public bool LoginAccount(string username, string password) {
		var query = db.GetView (Constants.DB_TYPE_ACCOUNT).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			//print (row.Key + " " + row.Value);
			if(row.Key.ToString() == username && row.Value.ToString() == password) {
				print ("account found!");
				UpdatePlayer (username, row.DocumentId);
				return true;
			}
		}
		return false;
	}

	public void UpdatePlayer(string username, string id) {
		PlayerManager.Instance.player = db.GetDocument (id).Properties;
		PlayerManager.Instance.playerChickens = LoadChickens (username);
		PlayerManager.Instance.playerBuildings = LoadBuildingsOwnedByPlayer (username);
		PlayerManager.Instance.playerOccupiedTiles = LoadPlayerOccupiedTiles (username);
		/*
		print ("Account details:");
		foreach (KeyValuePair<string, object> item in PlayerManager.Instance.player)
			print(item.Key + ": " + item.Value);
		print ("Account's chicken details:");
		foreach (IDictionary<string,object> i in PlayerManager.Instance.playerChickens) {
			foreach (KeyValuePair<string, object> item in i)
				print(item.Key + ": " + item.Value);
		}
		print ("Player occupied tiles:");
		foreach (Vector2 v in PlayerManager.Instance.playerOccupiedTiles) {
			print(v.x + " " + v.y);
		}
		*/
	}

	public IDictionary<string,object> GenerateChicken(Dictionary<string, object> dic) {
		Document d = db.CreateDocument();
		var properties = dic;
		var rev = d.PutProperties(properties);
		if (rev != null) {
			print ("Chicken generation complete!");
			return rev.Properties;
		}
		return null;
	}

	public List<IDictionary<string,object>> LoadChickens(string username) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_CHICKEN).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			//print (row.Key + " " + row.Value);
			if(row.Value.ToString() == username) {
				l.Add (db.GetDocument (row.DocumentId).Properties);
			}
		}
		return l;
	}

	public IDictionary<string, object> LoadChicken(string name, string owner) {
		var query = db.GetView (Constants.DB_TYPE_CHICKEN).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			//print (row.Key);
			if(row.Key.ToString() == name && row.Value.ToString() == owner) {
				print ("chicken found!");
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public IDictionary<string,object> SaveBuildingOwnedByPlayer(Dictionary<string, object> dic) {
		Document d = db.CreateDocument();
		var properties = dic;
		var rev = d.PutProperties(properties);
		if (rev != null) {
			print ("Building owned is saved!");
			return rev.Properties;
		}
		return null;
	}

	public List<IDictionary<string,object>> LoadBuildingsOwnedByPlayer(string username) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_BUILDING_OWNED).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			//print (row.Key + " " + row.Value);
			if(row.Value.ToString() == username) {
				l.Add (db.GetDocument (row.DocumentId).Properties);
			}
		}
		return l;
	}

	public List<Vector2> LoadPlayerOccupiedTiles(string username) {
		List<Vector2> occupiedTiles = new List<Vector2> ();
		foreach (IDictionary<string,object> bldg in PlayerManager.Instance.playerBuildings) {
			IDictionary<string,object> building = LoadBuilding(bldg[Constants.DB_KEYWORD_NAME].ToString());
			occupiedTiles.AddRange(
				GameManager.Instance.GetBuildingTiles(
					new int[] {int.Parse(bldg[Constants.DB_KEYWORD_X_POSITION].ToString()), int.Parse(bldg[Constants.DB_KEYWORD_Y_POSITION].ToString())},
					new int[] {int.Parse(building[Constants.DB_KEYWORD_X_CENTER].ToString()), int.Parse(building[Constants.DB_KEYWORD_Y_CENTER].ToString())},
					new int[] {int.Parse(building[Constants.DB_KEYWORD_X_SIZE].ToString()), int.Parse(building[Constants.DB_KEYWORD_Y_SIZE].ToString())},
					bldg[Constants.DB_KEYWORD_ORIENTATION].ToString()
				)
			);
		}
		occupiedTiles = occupiedTiles.Distinct ().ToList ();
		return occupiedTiles;
	}

	public IDictionary<string, object> LoadFeeds(string name) {
		var query = db.GetView (Constants.DB_TYPE_FEEDS).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			//print (row.Key);
			if(row.Key.ToString() == name || row.DocumentId == name) {
				print ("feeds found!");
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public IDictionary<string,object> SaveFeedsSchedule(Dictionary<string, object> dic) {
		Document d = db.CreateDocument();
		var properties = dic;
		var rev = d.PutProperties(properties);
		if (rev != null) {
			print ("Feeds schedule is saved!");
			return rev.Properties;
		}
		return null;
	}

	public List<IDictionary<string, object>> LoadFeedsSchedule(string chickenId) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_FEEDS_SCHEDULE).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			//print (row.Key);
			if(row.Key.ToString() == chickenId) {
				l.Add (db.GetDocument (row.DocumentId).Properties);
			}
		}
		return l;
	}

	public IDictionary<string,object> SaveFightingMove(Dictionary<string, object> dic) {
		Document d = db.CreateDocument();
		var properties = dic;
		var rev = d.PutProperties(properties);
		if (rev != null) {
			print ("Fighting move is saved!");
			return rev.Properties;
		}
		return null;
	}
	
	public IDictionary<string, object> LoadFightingMove(string name) {
		var query = db.GetView (Constants.DB_TYPE_FIGHTING_MOVE).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			//print (row.Key);
			if(row.Key.ToString() == name || row.DocumentId == name) {
				//print ("move found!");
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public IDictionary<string,object> SaveFightingMoveOwned(Dictionary<string, object> dic) {
		Document d = db.CreateDocument();
		var properties = dic;
		var rev = d.PutProperties(properties);
		if (rev != null) {
			print ("Fighting move owned is saved!");
			return rev.Properties;
		}
		return null;
	}

	public List<IDictionary<string, object>> LoadFightingMovesOwned(string chickenId) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_FIGHTING_MOVE_OWNED).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			//print (row.Key);
			if(row.Key.ToString() == chickenId) {
				l.Add (db.GetDocument (row.DocumentId).Properties);
			}
		}
		return l;
	}

	// delete functions, use sparingly!
	public void DeleteBuildingsOwnedByPlayer(string username) {
		var query = db.GetView (Constants.DB_TYPE_BUILDING_OWNED).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(username != null) {
				if(row.Value.ToString() == username) {
					db.GetDocument(row.DocumentId).Delete();
				}
			}
			else {
				db.GetDocument(row.DocumentId).Delete();
			}
		}
	}
	// end delete functions

}
