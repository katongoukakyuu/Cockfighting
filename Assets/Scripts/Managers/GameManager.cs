using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour {

	public bool initializeDatabase = false;

	private static GameManager instance;
	private GameManager() {}

	public static GameManager Instance {
		get {
			if(instance == null) {
				instance = (GameManager)GameObject.FindObjectOfType(typeof(GameManager));
			}
			return instance;
		}
	}

	void Start() {
		DontDestroyOnLoad(this);
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
			return;
		}
	}

	public Dictionary<string, object> RegisterAccount(string username, string email) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_ACCOUNT},
			{Constants.DB_KEYWORD_USERNAME, username},
			{Constants.DB_KEYWORD_PASSWORD, "test"},
			{Constants.DB_KEYWORD_EMAIL, email},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.UtcNow.ToString()},
			{Constants.DB_KEYWORD_MATCHES_WON, 0},
			{Constants.DB_KEYWORD_MATCHES_LOST, 0},
			{Constants.DB_KEYWORD_MATCHES_TIED, 0},
			{Constants.DB_KEYWORD_COIN, Constants.COIN_START},
			{Constants.DB_KEYWORD_CASH, Constants.CASH_START}
		};
		return d;
	}

	public Dictionary<string, object> GenerateChicken(string name, string owner, string gender, string breed, string lifeStage) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_CHICKEN},
			{Constants.DB_KEYWORD_NAME, name},
			{Constants.DB_KEYWORD_OWNER, owner},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.UtcNow.ToString()},
			{Constants.DB_TYPE_BREED, breed},
			{Constants.DB_KEYWORD_GENDER, gender},
			{Constants.DB_KEYWORD_NOTES, ""},
			{Constants.DB_KEYWORD_ATTACK, Constants.CHICKEN_ATTACK_DEFAULT_START},
			{Constants.DB_KEYWORD_DEFENSE, Constants.CHICKEN_DEFENSE_DEFAULT_START},
			{Constants.DB_KEYWORD_HP, Constants.CHICKEN_HP_DEFAULT_START},
			{Constants.DB_KEYWORD_AGILITY, Constants.CHICKEN_AGILITY_DEFAULT_START},
			{Constants.DB_KEYWORD_GAMENESS, Constants.CHICKEN_GAMENESS_DEFAULT_START},
			{Constants.DB_KEYWORD_AGGRESSION, Constants.CHICKEN_AGGRESSION_DEFAULT_START},
			{Constants.DB_KEYWORD_ATTACK_MAX, Constants.CHICKEN_ATTACK_DEFAULT_MAX},
			{Constants.DB_KEYWORD_DEFENSE_MAX, Constants.CHICKEN_DEFENSE_DEFAULT_MAX},
			{Constants.DB_KEYWORD_HP_MAX, Constants.CHICKEN_HP_DEFAULT_MAX},
			{Constants.DB_KEYWORD_AGILITY_MAX, Constants.CHICKEN_AGILITY_DEFAULT_MAX},
			{Constants.DB_KEYWORD_GAMENESS_MAX, Constants.CHICKEN_GAMENESS_DEFAULT_MAX},
			{Constants.DB_KEYWORD_AGGRESSION_MAX, Constants.CHICKEN_AGGRESSION_DEFAULT_MAX},
			{Constants.DB_KEYWORD_LIFE_STAGE, lifeStage}
		};
		return d;
	}

	public Dictionary<string, object> GenerateBuildingOwnedByPlayer(string name, string owner, string mapName, int xPos, int yPos, string orientation) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_BUILDING_OWNED},
			{Constants.DB_KEYWORD_NAME, name},
			{Constants.DB_KEYWORD_OWNER, owner},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.UtcNow.ToString()},
			{Constants.DB_KEYWORD_MAP_NAME, mapName},
			{Constants.DB_KEYWORD_X_POSITION, xPos},
			{Constants.DB_KEYWORD_Y_POSITION, yPos},
			{Constants.DB_KEYWORD_ORIENTATION, orientation},
		};
		return d;
	}

	public Dictionary<string, object> GenerateFeedsSchedule(string chickenId, string feedsId, int order) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_FEEDS_SCHEDULE},
			{Constants.DB_KEYWORD_CHICKEN_ID, chickenId},
			{Constants.DB_KEYWORD_FEEDS_ID, feedsId},
			{Constants.DB_KEYWORD_ORDER, order},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.UtcNow.ToString()}
		};
		return d;
	}

	public Dictionary<string, object> GenerateFightingMove(string name) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_FIGHTING_MOVE},
			{Constants.DB_KEYWORD_NAME, name},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.UtcNow.ToString()}
		};
		return d;
	}

	public Dictionary<string, object> GenerateFightingMoveOwnedByChicken(string chickenId, string moveId) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_FIGHTING_MOVE_OWNED},
			{Constants.DB_KEYWORD_CHICKEN_ID, chickenId},
			{Constants.DB_KEYWORD_FIGHTING_MOVE_ID, moveId},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.UtcNow.ToString()}
		};
		return d;
	}

	public Dictionary<string, object> GenerateReplay(string[] chickenId,
	                                                 List<IDictionary<string,object>> replay) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_KEYWORD_REPLAY},
			{Constants.DB_KEYWORD_CHICKEN_ID_1, chickenId[0]},
			{Constants.DB_KEYWORD_CHICKEN_ID_2, chickenId[1]},
			{Constants.DB_KEYWORD_REPLAY, replay},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.UtcNow.ToString()}
		};
		return d;
	}

	public Dictionary<string, object> GenerateItemOwnedByPlayer(string playerId, string itemId, string quantity) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_ITEM_OWNED},
			{Constants.DB_KEYWORD_PLAYER_ID, playerId},
			{Constants.DB_KEYWORD_ITEM_ID, itemId},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.UtcNow.ToString()},
			{Constants.DB_KEYWORD_QUANTITY, quantity}
		};
		return d;
	}

	public List<Vector2> GetBuildingTiles(int[] pos, int[] center, int[] size, string orientation) {
		List<Vector2> bldgTiles = new List<Vector2> ();
		int[] posZero = new int[] {
			pos [0] - center [0],
			pos [1] - center [1]
		};
		switch (orientation) {
		case Constants.ORIENTATION_NORTH:
			for(int x = center[0]; x >= 0; x--) {
				for(int y = 0; y < size[1]; y++) {
					Vector2 v = new Vector2(posZero[0] + x, posZero[1] + y);
					bldgTiles.Add (v);
				}
			}
			for(int x = center[0]+1; x < size[0]; x++) {
				for(int y = 0; y < size[1]; y++) {
					Vector2 v = new Vector2(posZero[0] + x, posZero[1] + y);
					bldgTiles.Add (v);
				}
			}
			break;
		case Constants.ORIENTATION_EAST:
			for(int x = center[1], x2 = center[1]; x2 >= 0; x++, x2--) {
				for(int y = 0, y2 = 0; y2 < size[0]; y--, y2++) {
					Vector2 v = new Vector2(posZero[0] + x, posZero[1] + y);
					bldgTiles.Add (v);
				}
			}
			for(int x = center[1]-1, x2 = center[1]+1; x2 < size[1]; x--, x2++) {
				for(int y = 0, y2 = 0; y2 < size[0]; y--, y2++) {
					Vector2 v = new Vector2(posZero[0] + x, posZero[1] + y);
					bldgTiles.Add (v);
				}
			}
			break;
		case Constants.ORIENTATION_SOUTH:
			for(int x = center[0], x2 = center[0]; x2 >= 0; x++, x2--) {
				for(int y = 0; y < size[1]; y++) {
					Vector2 v = new Vector2(posZero[0] + x, posZero[1] + y);
					bldgTiles.Add (v);
				}
			}
			for(int x = center[0]-1, x2 = center[0]+1; x2 < size[0]; x--, x2++) {
				for(int y = 0; y < size[1]; y++) {
					Vector2 v = new Vector2(posZero[0] + x, posZero[1] + y);
					bldgTiles.Add (v);
				}
			}
			break;
		case Constants.ORIENTATION_WEST:
			for(int x = center[1]; x >= 0; x--) {
				for(int y = 0; y < size[0]; y++) {
					Vector2 v = new Vector2(posZero[0] + x, posZero[1] + y);
					bldgTiles.Add (v);
				}
			}
			for(int x = center[1]+1; x < size[1]; x++) {
				for(int y = 0; y < size[0]; y++) {
					Vector2 v = new Vector2(posZero[0] + x, posZero[1] + y);
					bldgTiles.Add (v);
				}
			}
			break;
		default:
			break;
		}
		bldgTiles = bldgTiles.Distinct ().ToList ();
		return bldgTiles;
	}
}
