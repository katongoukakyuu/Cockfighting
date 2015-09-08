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

	void Awake() {
		Manager.SharedInstance = new Manager (new DirectoryInfo (Application.persistentDataPath), ManagerOptions.Default);
	}

	void Start () {
		DontDestroyOnLoad(this);
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
			return;
		}
		manager = Manager.SharedInstance;
		print ("manager directory is " + manager.Directory);
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
			if(doc.ContainsKey(Constants.DB_KEYWORD_SUBTYPE) && doc[Constants.DB_KEYWORD_SUBTYPE].ToString () == Constants.DB_TYPE_FEEDS)
				emit(doc[Constants.DB_KEYWORD_NAME], null);
		}, "1");

		// feeds schedule
		View viewFeedsSchedule = db.GetView(Constants.DB_TYPE_FEEDS_SCHEDULE);
		viewFeedsSchedule.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_FEEDS_SCHEDULE)
				emit(doc[Constants.DB_KEYWORD_CHICKEN_ID], doc[Constants.DB_KEYWORD_FEEDS_ID]);
		}, "1");

		// breeds
		View view = db.GetView(Constants.DB_TYPE_BREED);
		view.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_BREED)
				emit(doc[Constants.DB_KEYWORD_NAME], null);
		}, "1");

		// breeds schedule
		View viewBreedsSchedule = db.GetView(Constants.DB_TYPE_BREED_SCHEDULE);
		viewBreedsSchedule.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_BREED_SCHEDULE)
				emit(doc[Constants.DB_KEYWORD_CHICKEN_ID_1], doc[Constants.DB_KEYWORD_CHICKEN_ID_2]);
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

		// replay
		View viewReplay = db.GetView(Constants.DB_KEYWORD_REPLAY);
		viewReplay.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_KEYWORD_REPLAY)
				emit(doc[Constants.DB_KEYWORD_CHICKEN_ID_1], doc[Constants.DB_KEYWORD_CHICKEN_ID_2]);
		}, "1");

		// items
		View viewItems = db.GetView(Constants.DB_TYPE_ITEM);
		viewItems.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_ITEM)
				emit(doc[Constants.DB_KEYWORD_NAME], null);
		}, "1");

		// items owned by player
		View viewItemsOwned = db.GetView(Constants.DB_TYPE_ITEM_OWNED);
		viewItemsOwned.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_ITEM_OWNED)
				emit(doc[Constants.DB_KEYWORD_PLAYER_ID], doc[Constants.DB_KEYWORD_ITEM_ID]);
		}, "1");

		// matchmaking category
		View viewMatchmaking = db.GetView(Constants.DB_TYPE_MATCHMAKING_CATEGORY);
		viewMatchmaking.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_MATCHMAKING_CATEGORY)
				emit(doc[Constants.DB_KEYWORD_NAME], null);
		}, "1");

		// match
		View viewMatch = db.GetView(Constants.DB_TYPE_MATCH);
		viewMatch.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_MATCH)
				emit(doc[Constants.DB_KEYWORD_PLAYER_ID_1], doc[Constants.DB_KEYWORD_PLAYER_ID_2]);
		}, "1");

		// betting odds
		View viewBetingOdds = db.GetView(Constants.DB_TYPE_BETTING_ODDS);
		viewBetingOdds.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_BETTING_ODDS)
				emit(doc[Constants.DB_KEYWORD_NAME], doc[Constants.DB_KEYWORD_ORDER]);
		}, "1");

		// bet
		View viewBet = db.GetView(Constants.DB_TYPE_BET);
		viewBet.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_BET)
				emit(doc[Constants.DB_KEYWORD_MATCH_ID], doc[Constants.DB_KEYWORD_PLAYER_ID]);
		}, "1");

		// initialize database, use Unity inspector to change value in GameManager
		if (GameManager.Instance.initializeDatabase) {
			InitializeDatabase();
		}
	}

	private void InitializeDatabase() {
		SaveEntry (GameManager.Instance.GenerateFightingMove (
			Constants.FIGHT_MOVE_DASH
		));
		SaveEntry (GameManager.Instance.GenerateFightingMove (
			Constants.FIGHT_MOVE_FLYING_TALON
		));
		SaveEntry (GameManager.Instance.GenerateFightingMove (
			Constants.FIGHT_MOVE_SIDESTEP
		));
		SaveEntry (GameManager.Instance.GenerateFightingMove (
			Constants.FIGHT_MOVE_PECK
		));

		RegisterAccount(GameManager.Instance.RegisterAccount ("test", "test@test.com", "Savior Studios"));
		RegisterAccount(GameManager.Instance.RegisterAccount ("test2", "test@test.com", "Devil Studios"));
		RegisterAccount(GameManager.Instance.RegisterAccount ("test3", "test@test.com", "Purgatory Studios"));
		ControlPanelBreedsManager.Instance.SaveBreed (
			"Kelso",
			1.3f, 1.2f, 1.0f, 0.9f, 1.1f, 1.0f
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
		ControlPanelFeedsManager.Instance.SaveFeeds (
			"Mini Feeds", "Feeds for mini chickens.",
			10, 10,
			0, 0, 0, 5,
			10, -20, 10, -10, 10, 10,
			"Hen Coop"
		);
		SaveEntry(GameManager.Instance.GenerateItemOwnedByPlayer(
			LoadPlayer("test")[Constants.DB_COUCHBASE_ID].ToString (),
			LoadFeeds("Uber Feeds")[Constants.DB_COUCHBASE_ID].ToString (),
			"50"
		));

		SaveEntry (GameManager.Instance.GenerateMatchmakingCategory (
			"Beginner", false, false
		));
		SaveEntry (GameManager.Instance.GenerateMatchmakingCategory (
			"Player vs. Player", true, true
		));
		SaveEntry (GameManager.Instance.GenerateMatchmakingCategory (
			"Event", true, false
		));

		SaveEntry(GameManager.Instance.GenerateBettingOdds(
			"Even Odds", 10, 10, 0
		));
		SaveEntry(GameManager.Instance.GenerateBettingOdds(
			"Ten-Nine", 10, 9, 1
		));
		SaveEntry(GameManager.Instance.GenerateBettingOdds(
			"Nine-Eight", 9, 8, 2
		));
		SaveEntry(GameManager.Instance.GenerateBettingOdds(
			"Ten-Eight", 10, 8, 3
		));
		SaveEntry(GameManager.Instance.GenerateBettingOdds(
			"Eight-Six", 8, 6, 4
		));
		SaveEntry(GameManager.Instance.GenerateBettingOdds(
			"Eleven-Eight", 11, 8, 5
		));
		SaveEntry(GameManager.Instance.GenerateBettingOdds(
			"Three-Two", 3, 2, 6
		));
		SaveEntry(GameManager.Instance.GenerateBettingOdds(
			"Ten-Six", 10, 6, 7
		));
		SaveEntry(GameManager.Instance.GenerateBettingOdds(
			"Double Odds", 10, 5, 8
		));
		SaveEntry(GameManager.Instance.GenerateBettingOdds(
			"Ten-Four", 10, 4, 9
		));
		SaveEntry(GameManager.Instance.GenerateBettingOdds(
			"Ten-Three", 10, 3, 10
		));
		SaveEntry(GameManager.Instance.GenerateBettingOdds(
			"Ten-Two", 10, 2, 11
		));
		SaveEntry(GameManager.Instance.GenerateBettingOdds(
			"Ten-One", 10, 1, 12
		));

		Destroy (ControlPanelBreedsManager.Instance);
		Destroy (ControlPanelBuildingsManager.Instance);
		Destroy (ControlPanelFeedsManager.Instance);
	}

	public void ReinitializeDatabase() {
		var query = db.CreateAllDocumentsQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			db.GetDocument(row.DocumentId).Delete();
		}
		gameObject.AddComponent<ControlPanelBreedsManager>();
		gameObject.AddComponent<ControlPanelBuildingsManager>();
		gameObject.AddComponent<ControlPanelFeedsManager>();
		InitializeDatabase();
	}

	public Database GetDatabase() {
		return db;
	}

	public IDictionary<string,object> SaveEntry(Dictionary<string, object> dic) {
		Document d = db.CreateDocument();
		var properties = dic;
		var rev = d.PutProperties(properties);
		if (rev != null) {
			return rev.Properties;
		}
		return null;
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
		chicken.Add (SaveEntry(GameManager.Instance.GenerateChicken ("Larry", 
		                                      dic [Constants.DB_KEYWORD_USERNAME].ToString(), 
		                                      Constants.GENDER_MALE, 
		                                      "Kelso", 
		                                      Constants.LIFE_STAGE_COCK)));

		chicken.Add (SaveEntry(GameManager.Instance.GenerateChicken ("Gary", 
		                                      dic [Constants.DB_KEYWORD_USERNAME].ToString(), 
		                                      Constants.GENDER_MALE, 
		                                      "Kelso", 
		                                      Constants.LIFE_STAGE_COCK)));
		chicken.Add (SaveEntry(GameManager.Instance.GenerateChicken ("Mary", 
		                                      dic [Constants.DB_KEYWORD_USERNAME].ToString(), 
		                                      Constants.GENDER_FEMALE, 
		                                      "Kelso", 
		                                      Constants.LIFE_STAGE_HEN)));

		foreach(IDictionary<string,object> id in chicken) {
			if(id[Constants.DB_KEYWORD_LIFE_STAGE].ToString() != Constants.LIFE_STAGE_HEN) {
				SaveEntry (GameManager.Instance.GenerateFightingMoveOwnedByChicken (
					id[Constants.DB_COUCHBASE_ID].ToString(),
					LoadFightingMove(Constants.FIGHT_MOVE_DASH)[Constants.DB_COUCHBASE_ID].ToString()
					));
				SaveEntry (GameManager.Instance.GenerateFightingMoveOwnedByChicken (
					id[Constants.DB_COUCHBASE_ID].ToString(),
					LoadFightingMove(Constants.FIGHT_MOVE_FLYING_TALON)[Constants.DB_COUCHBASE_ID].ToString()
					));
				SaveEntry (GameManager.Instance.GenerateFightingMoveOwnedByChicken (
					id[Constants.DB_COUCHBASE_ID].ToString(),
					LoadFightingMove(Constants.FIGHT_MOVE_SIDESTEP)[Constants.DB_COUCHBASE_ID].ToString()
					));
				SaveEntry (GameManager.Instance.GenerateFightingMoveOwnedByChicken (
					id[Constants.DB_COUCHBASE_ID].ToString(),
					LoadFightingMove(Constants.FIGHT_MOVE_PECK)[Constants.DB_COUCHBASE_ID].ToString()
					));
			}
		}

		if (rev != null) {
			return rev.Properties;
		}
		return null;
	}

	public IDictionary<string, object> LoadPlayer(string name) {
		var query = db.GetView (Constants.DB_TYPE_ACCOUNT).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Key.ToString() == name || row.DocumentId == name) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public bool LoginAccount(string username, string password) {
		var query = db.GetView (Constants.DB_TYPE_ACCOUNT).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Key.ToString() == username && row.Value.ToString() == password) {
				UpdatePlayer (username, row.DocumentId);
				return true;
			}
		}
		return false;
	}

	public void EditAccount(IDictionary<string, object> dic) {
		Document d = db.GetDocument(dic[Constants.DB_COUCHBASE_ID].ToString());
		d.Update((UnsavedRevision newRevision) => {
			var properties = newRevision.Properties;
			properties[Constants.DB_KEYWORD_PASSWORD] = dic[Constants.DB_KEYWORD_PASSWORD].ToString();
			properties[Constants.DB_KEYWORD_EMAIL] = dic[Constants.DB_KEYWORD_EMAIL].ToString();
			properties[Constants.DB_KEYWORD_FARM_NAME] = dic[Constants.DB_KEYWORD_FARM_NAME].ToString();
			properties[Constants.DB_KEYWORD_MATCHES_WON] = dic[Constants.DB_KEYWORD_MATCHES_WON].ToString();
			properties[Constants.DB_KEYWORD_MATCHES_LOST] = dic[Constants.DB_KEYWORD_MATCHES_LOST].ToString();
			properties[Constants.DB_KEYWORD_MATCHES_TIED] = dic[Constants.DB_KEYWORD_MATCHES_TIED].ToString();
			properties[Constants.DB_KEYWORD_COIN] = dic[Constants.DB_KEYWORD_COIN].ToString();
			properties[Constants.DB_KEYWORD_CASH] = dic[Constants.DB_KEYWORD_CASH].ToString();
			return true;
		});
	}

	public void UpdatePlayer(string username, string id) {
		PlayerManager.Instance.player = db.GetDocument (id).Properties;
		PlayerManager.Instance.playerChickens = LoadChickens (username);
		PlayerManager.Instance.playerBuildings = LoadBuildingsOwnedByPlayer (username);
		PlayerManager.Instance.playerOccupiedTiles = LoadPlayerOccupiedTiles (username);
	}

	public void EditChicken(IDictionary<string, object> dic) {
		Document d = db.GetDocument(dic[Constants.DB_COUCHBASE_ID].ToString());
		d.Update((UnsavedRevision newRevision) => {
			var properties = newRevision.Properties;
			properties[Constants.DB_KEYWORD_NAME] = dic[Constants.DB_KEYWORD_NAME].ToString();
			properties[Constants.DB_KEYWORD_OWNER] = dic[Constants.DB_KEYWORD_OWNER].ToString();
			properties[Constants.DB_KEYWORD_NOTES] = dic[Constants.DB_KEYWORD_NOTES].ToString();
			properties[Constants.DB_KEYWORD_ATTACK] = dic[Constants.DB_KEYWORD_ATTACK].ToString();
			properties[Constants.DB_KEYWORD_DEFENSE] = dic[Constants.DB_KEYWORD_DEFENSE].ToString();
			properties[Constants.DB_KEYWORD_HP] = dic[Constants.DB_KEYWORD_HP].ToString();
			properties[Constants.DB_KEYWORD_AGILITY] = dic[Constants.DB_KEYWORD_AGILITY].ToString();
			properties[Constants.DB_KEYWORD_GAMENESS] = dic[Constants.DB_KEYWORD_GAMENESS].ToString();
			properties[Constants.DB_KEYWORD_AGGRESSION] = dic[Constants.DB_KEYWORD_AGGRESSION].ToString();
			properties[Constants.DB_KEYWORD_ATTACK_MAX] = dic[Constants.DB_KEYWORD_ATTACK_MAX].ToString();
			properties[Constants.DB_KEYWORD_DEFENSE_MAX] = dic[Constants.DB_KEYWORD_DEFENSE_MAX].ToString();
			properties[Constants.DB_KEYWORD_HP_MAX] = dic[Constants.DB_KEYWORD_HP_MAX].ToString();
			properties[Constants.DB_KEYWORD_AGILITY_MAX] = dic[Constants.DB_KEYWORD_AGILITY_MAX].ToString();
			properties[Constants.DB_KEYWORD_GAMENESS_MAX] = dic[Constants.DB_KEYWORD_GAMENESS_MAX].ToString();
			properties[Constants.DB_KEYWORD_AGGRESSION_MAX] = dic[Constants.DB_KEYWORD_AGGRESSION_MAX].ToString();
			properties[Constants.DB_KEYWORD_LIFE_STAGE] = dic[Constants.DB_KEYWORD_LIFE_STAGE].ToString();
			properties[Constants.DB_KEYWORD_IS_QUEUED_FOR_MATCH] = dic[Constants.DB_KEYWORD_IS_QUEUED_FOR_MATCH].ToString();
			return true;
		});
	}

	public List<IDictionary<string,object>> LoadChickens(string username) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_CHICKEN).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Value.ToString() == username || username == null) {
				l.Add (db.GetDocument (row.DocumentId).Properties);
			}
		}
		return l;
	}

	public IDictionary<string, object> LoadChicken(string name, string owner) {
		var query = db.GetView (Constants.DB_TYPE_CHICKEN).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Key.ToString() == name && row.Value.ToString() == owner) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public IDictionary<string, object> LoadChicken(string chickenId) {
		var query = db.GetView (Constants.DB_TYPE_CHICKEN).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.DocumentId == chickenId) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public List<IDictionary<string,object>> LoadBuildingsOwnedByPlayer(string username) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_BUILDING_OWNED).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
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
			if(row.Key.ToString() == name || row.DocumentId == name) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public void EditFeedsSchedule(IDictionary<string, object> dic) {
		Document d = db.GetDocument(dic[Constants.DB_COUCHBASE_ID].ToString());
		d.Update((UnsavedRevision newRevision) => {
			var properties = newRevision.Properties;
			properties[Constants.DB_KEYWORD_END_TIME] = dic[Constants.DB_KEYWORD_END_TIME].ToString();
			properties[Constants.DB_KEYWORD_IS_COMPLETED] = dic[Constants.DB_KEYWORD_IS_COMPLETED].ToString();
			return true;
		});
	}

	public List<IDictionary<string, object>> LoadFeedsSchedule(string chickenId) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_FEEDS_SCHEDULE).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(chickenId == null || row.Key.ToString() == chickenId) {
				l.Add (db.GetDocument (row.DocumentId).Properties);
			}
		}
		return l;
	}

	public IDictionary<string, object> LoadBreed(string name) {
		var query = db.GetView (Constants.DB_TYPE_BREED).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Key.ToString() == name || row.DocumentId == name) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}
	
	public void EditBreedsSchedule(IDictionary<string, object> dic) {
		Document d = db.GetDocument(dic[Constants.DB_COUCHBASE_ID].ToString());
		d.Update((UnsavedRevision newRevision) => {
			var properties = newRevision.Properties;
			properties[Constants.DB_KEYWORD_END_TIME] = dic[Constants.DB_KEYWORD_END_TIME].ToString();
			properties[Constants.DB_KEYWORD_IS_COMPLETED] = dic[Constants.DB_KEYWORD_IS_COMPLETED].ToString();
			return true;
		});
	}
	
	public List<IDictionary<string, object>> LoadBreedsSchedule(string chickenId) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_BREED_SCHEDULE).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(chickenId == null || row.Key.ToString() == chickenId || row.Value.ToString() == chickenId) {
				l.Add (db.GetDocument (row.DocumentId).Properties);
			}
		}
		return l;
	}
	
	public IDictionary<string, object> LoadFightingMove(string name) {
		var query = db.GetView (Constants.DB_TYPE_FIGHTING_MOVE).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Key.ToString() == name || row.DocumentId == name) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public List<IDictionary<string, object>> LoadFightingMovesOwned(string chickenId) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_FIGHTING_MOVE_OWNED).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Key.ToString() == chickenId) {
				l.Add (db.GetDocument (row.DocumentId).Properties);
			}
		}
		return l;
	}

	public List<IDictionary<string, object>> LoadReplayList(string chickenId1, string chickenId2) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_REPLAY).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if((chickenId2 != null && (row.Key.ToString() == chickenId1 && row.Value.ToString() == chickenId2)) ||
			   (chickenId2 != null && (row.Key.ToString() == chickenId2 && row.Value.ToString() == chickenId1)) ||
			   (chickenId2 == null && (row.Key.ToString() == chickenId1 || row.Value.ToString() == chickenId1))) {
				l.Add (db.GetDocument (row.DocumentId).Properties);
			}
		}
		return l;
	}

	public IDictionary<string, object> LoadReplay(string replayId) {
		var query = db.GetView (Constants.DB_TYPE_REPLAY).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.DocumentId.ToString() == replayId) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public IDictionary<string, object> LoadItem(string name) {
		var query = db.GetView (Constants.DB_TYPE_ITEM).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Key.ToString() == name || row.DocumentId == name) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}
	
	public List<IDictionary<string,object>> LoadItemsOwnedByPlayer(string playerId) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_ITEM_OWNED).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Key.ToString() == playerId) {
				l.Add (db.GetDocument (row.DocumentId).Properties);
			}
		}
		return l;
	}
	
	public List<IDictionary<string,object>> LoadMatchmakingCategories() {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_MATCHMAKING_CATEGORY).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			l.Add (db.GetDocument (row.DocumentId).Properties);
		}
		return l;
	}

	public void EditMatch(IDictionary<string, object> dic) {
		Document d = db.GetDocument(dic[Constants.DB_COUCHBASE_ID].ToString());
		d.Update((UnsavedRevision newRevision) => {
			var properties = newRevision.Properties;
			properties[Constants.DB_KEYWORD_CHICKEN_ID_1] = dic[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString();
			properties[Constants.DB_KEYWORD_CHICKEN_ID_2] = dic[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString();
			properties[Constants.DB_KEYWORD_PLAYER_ID_1] = dic[Constants.DB_KEYWORD_PLAYER_ID_1].ToString();
			properties[Constants.DB_KEYWORD_PLAYER_ID_2] = dic[Constants.DB_KEYWORD_PLAYER_ID_2].ToString();
			properties[Constants.DB_KEYWORD_LLAMADO] = dic[Constants.DB_KEYWORD_LLAMADO].ToString();
			properties[Constants.DB_KEYWORD_CATEGORY_ID] = dic[Constants.DB_KEYWORD_CATEGORY_ID].ToString();
			properties[Constants.DB_KEYWORD_BETTING_OPTION] = dic[Constants.DB_KEYWORD_BETTING_OPTION].ToString();
			properties[Constants.DB_KEYWORD_BETTING_ODDS_ID] = dic[Constants.DB_KEYWORD_BETTING_ODDS_ID].ToString();
			properties[Constants.DB_KEYWORD_PASSWORD] = dic[Constants.DB_KEYWORD_PASSWORD].ToString();
			properties[Constants.DB_KEYWORD_END_TIME] = dic[Constants.DB_KEYWORD_END_TIME].ToString();
			properties[Constants.DB_KEYWORD_STATUS] = dic[Constants.DB_KEYWORD_STATUS].ToString();
			if(dic.ContainsKey(Constants.DB_KEYWORD_INTERVAL)) {
				properties[Constants.DB_KEYWORD_INTERVAL] = dic[Constants.DB_KEYWORD_INTERVAL].ToString();
			}
			if(dic.ContainsKey(Constants.DB_KEYWORD_INTERVALS_LEFT)) {
				properties[Constants.DB_KEYWORD_INTERVALS_LEFT] = dic[Constants.DB_KEYWORD_INTERVALS_LEFT].ToString();
			}
			if(dic.ContainsKey(Constants.DB_KEYWORD_INTERVAL_TIME)) {
				properties[Constants.DB_KEYWORD_INTERVAL_TIME] = dic[Constants.DB_KEYWORD_INTERVAL_TIME].ToString();
			}
			if(dic.ContainsKey(Constants.DB_KEYWORD_BETTING_ODDS_ID)) {
				properties[Constants.DB_KEYWORD_BETTING_ODDS_ID] = dic[Constants.DB_KEYWORD_BETTING_ODDS_ID].ToString();
			}
			return true;
		});
	}
	
	public List<IDictionary<string,object>> LoadMatch(string playerId) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_MATCH).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(playerId == null || row.Key.ToString() == playerId || row.Value.ToString() == playerId) {
				l.Add (db.GetDocument (row.DocumentId).Properties);
			}
		}
		return l;
	}

	public IDictionary<string,object> LoadMatchById(string matchId) {
		var query = db.GetView (Constants.DB_TYPE_MATCH).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.DocumentId == matchId) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public List<IDictionary<string,object>> LoadMatchesByCategory(string categoryId) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		foreach(IDictionary<string,object> id in LoadMatch(null)) {
			if(id[Constants.DB_KEYWORD_CATEGORY_ID].ToString() == categoryId) {
				l.Add (id);
			}
		}
		return l;
	}

	public List<IDictionary<string,object>> LoadBettingOdds() {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_BETTING_ODDS).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			l.Add (db.GetDocument (row.DocumentId).Properties);
		}
		return l;
	}

	public IDictionary<string, object> LoadBettingOdds(string bettingOddsId) {
		var query = db.GetView (Constants.DB_TYPE_BETTING_ODDS).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.DocumentId == bettingOddsId) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public IDictionary<string, object> LoadBettingOdds(int bettingOddsOrder) {
		var query = db.GetView (Constants.DB_TYPE_BETTING_ODDS).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Value.ToString() == bettingOddsOrder.ToString()) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public IDictionary<string, object> LoadBet(string matchId, string playerId) {
		var query = db.GetView (Constants.DB_TYPE_BET).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Key.ToString() == matchId && row.Value.ToString() == playerId) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public List<IDictionary<string,object>> LoadBets(string id) {
		List<IDictionary<string,object>> l = new List<IDictionary<string,object>>();
		var query = db.GetView (Constants.DB_TYPE_BET).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			if(row.Key.ToString() == id || row.Value.ToString() == id) {
				l.Add (db.GetDocument (row.DocumentId).Properties);
			}
		}
		return l;
	}

}
