using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour {

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

	public Dictionary<string, object> RegisterAccount(string userId, string farmName) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_ACCOUNT},
			{Constants.DB_KEYWORD_USER_ID, userId},
			{Constants.DB_KEYWORD_FARM_NAME, farmName},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()},
			{Constants.DB_KEYWORD_MATCHES_WON, 0},
			{Constants.DB_KEYWORD_MATCHES_LOST, 0},
			{Constants.DB_KEYWORD_MATCHES_TIED, 0},
			{Constants.DB_KEYWORD_COIN, Constants.COIN_START},
			{Constants.DB_KEYWORD_CASH, Constants.CASH_START}
		};
		return d;
	}

	public Dictionary<string, object> GenerateChicken(string name, string userId, string gender, string breed, string lifeStage) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_CHICKEN},
			{Constants.DB_KEYWORD_NAME, name},
			{Constants.DB_KEYWORD_USER_ID, userId},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()},
			{Constants.DB_TYPE_BREED, breed},
			{Constants.DB_KEYWORD_GENERATION_ID, name + userId},
			{Constants.DB_KEYWORD_GENERATION_COUNT, 0},
			{Constants.DB_KEYWORD_GENDER, gender},
			{Constants.DB_KEYWORD_NOTES, ""},
			{Constants.DB_KEYWORD_ATTACK, Constants.CHICKEN_ATTACK_DEFAULT_START},
			{Constants.DB_KEYWORD_DEFENSE, Constants.CHICKEN_DEFENSE_DEFAULT_START},
			{Constants.DB_KEYWORD_HP, Constants.CHICKEN_HP_DEFAULT_START},
			{Constants.DB_KEYWORD_AGILITY, Constants.CHICKEN_AGILITY_DEFAULT_START},
			{Constants.DB_KEYWORD_GAMENESS, Constants.CHICKEN_GAMENESS_DEFAULT_START},
			{Constants.DB_KEYWORD_AGGRESSION, Constants.CHICKEN_AGGRESSION_DEFAULT_START},
			{Constants.DB_KEYWORD_CONDITIONING, Constants.CHICKEN_CONDITIONING_DEFAULT_START},
			{Constants.DB_KEYWORD_ATTACK_MAX, Constants.CHICKEN_ATTACK_DEFAULT_MAX},
			{Constants.DB_KEYWORD_DEFENSE_MAX, Constants.CHICKEN_DEFENSE_DEFAULT_MAX},
			{Constants.DB_KEYWORD_HP_MAX, Constants.CHICKEN_HP_DEFAULT_MAX},
			{Constants.DB_KEYWORD_AGILITY_MAX, Constants.CHICKEN_AGILITY_DEFAULT_MAX},
			{Constants.DB_KEYWORD_GAMENESS_MAX, Constants.CHICKEN_GAMENESS_DEFAULT_MAX},
			{Constants.DB_KEYWORD_AGGRESSION_MAX, Constants.CHICKEN_AGGRESSION_DEFAULT_MAX},
			{Constants.DB_KEYWORD_CONDITIONING_MAX, Constants.CHICKEN_CONDITIONING_DEFAULT_MAX},
			{Constants.DB_KEYWORD_LIFE_STAGE, lifeStage},
			{Constants.DB_KEYWORD_IS_QUEUED_FOR_MATCH, false},
		};
		return d;
	}

	public Dictionary<string, object> GenerateChicken(string name, string userId, string gender, string breed, string genId, int genCount, string lifeStage) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_CHICKEN},
			{Constants.DB_KEYWORD_NAME, name},
			{Constants.DB_KEYWORD_USER_ID, userId},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()},
			{Constants.DB_TYPE_BREED, breed},
			{Constants.DB_KEYWORD_GENERATION_ID, genId},
			{Constants.DB_KEYWORD_GENERATION_COUNT, genCount},
			{Constants.DB_KEYWORD_GENDER, gender},
			{Constants.DB_KEYWORD_NOTES, ""},
			{Constants.DB_KEYWORD_ATTACK, Constants.CHICKEN_ATTACK_DEFAULT_START},
			{Constants.DB_KEYWORD_DEFENSE, Constants.CHICKEN_DEFENSE_DEFAULT_START},
			{Constants.DB_KEYWORD_HP, Constants.CHICKEN_HP_DEFAULT_START},
			{Constants.DB_KEYWORD_AGILITY, Constants.CHICKEN_AGILITY_DEFAULT_START},
			{Constants.DB_KEYWORD_GAMENESS, Constants.CHICKEN_GAMENESS_DEFAULT_START},
			{Constants.DB_KEYWORD_AGGRESSION, Constants.CHICKEN_AGGRESSION_DEFAULT_START},
			{Constants.DB_KEYWORD_CONDITIONING, Constants.CHICKEN_CONDITIONING_DEFAULT_START},
			{Constants.DB_KEYWORD_ATTACK_MAX, Constants.CHICKEN_ATTACK_DEFAULT_MAX},
			{Constants.DB_KEYWORD_DEFENSE_MAX, Constants.CHICKEN_DEFENSE_DEFAULT_MAX},
			{Constants.DB_KEYWORD_HP_MAX, Constants.CHICKEN_HP_DEFAULT_MAX},
			{Constants.DB_KEYWORD_AGILITY_MAX, Constants.CHICKEN_AGILITY_DEFAULT_MAX},
			{Constants.DB_KEYWORD_GAMENESS_MAX, Constants.CHICKEN_GAMENESS_DEFAULT_MAX},
			{Constants.DB_KEYWORD_AGGRESSION_MAX, Constants.CHICKEN_AGGRESSION_DEFAULT_MAX},
			{Constants.DB_KEYWORD_CONDITIONING_MAX, Constants.CHICKEN_CONDITIONING_DEFAULT_MAX},
			{Constants.DB_KEYWORD_LIFE_STAGE, lifeStage},
			{Constants.DB_KEYWORD_IS_QUEUED_FOR_MATCH, false},
		};
		return d;
	}

	public Dictionary<string, object> GenerateBuildingOwnedByPlayer(string name, string userId, string mapName, int xPos, int yPos, string orientation) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_BUILDING_OWNED},
			{Constants.DB_KEYWORD_NAME, name},
			{Constants.DB_KEYWORD_USER_ID, userId},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()},
			{Constants.DB_KEYWORD_MAP_NAME, mapName},
			{Constants.DB_KEYWORD_X_POSITION, xPos},
			{Constants.DB_KEYWORD_Y_POSITION, yPos},
			{Constants.DB_KEYWORD_ORIENTATION, orientation},
		};
		return d;
	}

	public Dictionary<string, object> GenerateFeedsSchedule(string chickenId, string feedsId, string endTime) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_FEEDS_SCHEDULE},
			{Constants.DB_KEYWORD_CHICKEN_ID, chickenId},
			{Constants.DB_KEYWORD_FEEDS_ID, feedsId},
			{Constants.DB_KEYWORD_END_TIME, endTime},
			{Constants.DB_KEYWORD_IS_COMPLETED, Constants.GENERIC_FALSE},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()}
		};
		return d;
	}

	public Dictionary<string, object> GenerateBreedsSchedule(string chickenId1, string chickenId2, string endTime) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_BREED_SCHEDULE},
			{Constants.DB_KEYWORD_CHICKEN_ID_1, chickenId1},
			{Constants.DB_KEYWORD_CHICKEN_ID_2, chickenId2},
			{Constants.DB_KEYWORD_END_TIME, endTime},
			{Constants.DB_KEYWORD_IS_COMPLETED, Constants.GENERIC_FALSE},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()}
		};
		return d;
	}

	public Dictionary<string, object> GenerateConditioningDecaySchedule(string chickenId, string endTime) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_CONDITIONING_DECAY_SCHEDULE},
			{Constants.DB_KEYWORD_CHICKEN_ID, chickenId},
			{Constants.DB_KEYWORD_END_TIME, endTime},
			{Constants.DB_KEYWORD_IS_COMPLETED, Constants.GENERIC_FALSE},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()}
		};
		return d;
	}

	public Dictionary<string, object> GenerateFightingMove(string name) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_FIGHTING_MOVE},
			{Constants.DB_KEYWORD_NAME, name},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()}
		};
		return d;
	}

	public Dictionary<string, object> GenerateFightingMoveOwnedByChicken(string chickenId, string moveId) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_FIGHTING_MOVE_OWNED},
			{Constants.DB_KEYWORD_CHICKEN_ID, chickenId},
			{Constants.DB_KEYWORD_FIGHTING_MOVE_ID, moveId},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()}
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
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()}
		};
		return d;
	}

	public Dictionary<string, object> GenerateItem(string name, string subType, string description,
	                                               int coinCost, int cashCost, bool availableAtStore,
	                                               int durD, int durH, int durM, int durS,
	                                               int effAtk, int effDef, int effHp, int effAgi, int effGam, int effAgg, int effCon,
	                                               string imageName) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_ITEM},
			{Constants.DB_KEYWORD_SUBTYPE, subType},
			{Constants.DB_KEYWORD_NAME, name},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()},
			{Constants.DB_KEYWORD_DESCRIPTION, description},
			{Constants.DB_KEYWORD_COIN_COST, coinCost},
			{Constants.DB_KEYWORD_CASH_COST, cashCost},
			{Constants.DB_KEYWORD_AVAILABLE_AT_STORE, availableAtStore},
			{Constants.DB_KEYWORD_DURATION_DAYS, durD},
			{Constants.DB_KEYWORD_DURATION_HOURS, durH},
			{Constants.DB_KEYWORD_DURATION_MINUTES, durM},
			{Constants.DB_KEYWORD_DURATION_SECONDS, durS},
			{Constants.DB_KEYWORD_ATTACK, effAtk},
			{Constants.DB_KEYWORD_DEFENSE, effDef},
			{Constants.DB_KEYWORD_HP, effHp},
			{Constants.DB_KEYWORD_AGILITY, effAgi},
			{Constants.DB_KEYWORD_GAMENESS, effGam},
			{Constants.DB_KEYWORD_AGGRESSION, effAgg},
			{Constants.DB_KEYWORD_CONDITIONING, effCon},
			{Constants.DB_KEYWORD_IMAGE_NAME, imageName}
		};
		return d;
	}

	public Dictionary<string, object> GenerateItemOwnedByPlayer(string playerId, string itemId, string quantity) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_ITEM_OWNED},
			{Constants.DB_KEYWORD_PLAYER_ID, playerId},
			{Constants.DB_KEYWORD_ITEM_ID, itemId},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()},
			{Constants.DB_KEYWORD_QUANTITY, quantity}
		};
		return d;
	}

	public Dictionary<string, object> GenerateMatchmakingCategory(string name, 
	                                                              bool isPvp, 
	                                                              bool customMatchesAllowed) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_MATCHMAKING_CATEGORY},
			{Constants.DB_KEYWORD_NAME, name},
			{Constants.DB_KEYWORD_IS_PVP, isPvp},
			{Constants.DB_KEYWORD_CUSTOM_MATCHES_ALLOWED, customMatchesAllowed},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()}
		};
		return d;
	}

	public Dictionary<string, object> GenerateMatch(string chickenId1, string chickenId2,
	                                                string playerId1, string playerId2,
	                                                string categoryId, string bettingOption, string bettingOddsId, 
	                                                string password, string endTime) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_MATCH},
			{Constants.DB_KEYWORD_CHICKEN_ID_1, chickenId1},
			{Constants.DB_KEYWORD_CHICKEN_ID_2, chickenId2},
			{Constants.DB_KEYWORD_PLAYER_ID_1, playerId1},
			{Constants.DB_KEYWORD_PLAYER_ID_2, playerId2},
			{Constants.DB_KEYWORD_LLAMADO, chickenId1},
			{Constants.DB_KEYWORD_CATEGORY_ID, categoryId},
			{Constants.DB_KEYWORD_BETTING_OPTION, bettingOption},
			{Constants.DB_KEYWORD_BETTING_ODDS_ID, bettingOddsId},
			{Constants.DB_KEYWORD_PASSWORD, password},
			{Constants.DB_KEYWORD_END_TIME, endTime},
			{Constants.DB_KEYWORD_STATUS, Constants.MATCH_STATUS_WAITING_FOR_OPPONENT},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()}
		};
		return d;
	}

	public Dictionary<string, object> GenerateBettingOdds(string name, int llamadoOdds, int dehadoOdds, int order) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_BETTING_ODDS},
			{Constants.DB_KEYWORD_NAME, name},
			{Constants.DB_KEYWORD_LLAMADO_ODDS, llamadoOdds},
			{Constants.DB_KEYWORD_DEHADO_ODDS, dehadoOdds},
			{Constants.DB_KEYWORD_ORDER, order},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()}
		};
		return d;
	}

	public Dictionary<string, object> GenerateBet(string matchId, string playerId,
	                                              string bettingOddsId, string bettedChickenId,
	                                              string bettedChickenStatus, int betAmount) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_BET},
			{Constants.DB_KEYWORD_MATCH_ID, matchId},
			{Constants.DB_KEYWORD_PLAYER_ID, playerId},
			{Constants.DB_KEYWORD_BETTING_ODDS_ID, bettingOddsId},
			{Constants.DB_KEYWORD_BETTED_CHICKEN_ID, bettedChickenId},
			{Constants.DB_KEYWORD_BETTED_CHICKEN_STATUS, bettedChickenStatus},
			{Constants.DB_KEYWORD_BET_AMOUNT, betAmount},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()}
		};
		return d;
	}

	public Dictionary<string, object> GenerateMail(string playerIdFrom, string playerIdTo,
	                                               string title, string message,
	                                               List<string> mailType,
	                                               string replayId) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_MAIL},
			{Constants.DB_KEYWORD_FROM, playerIdFrom},
			{Constants.DB_KEYWORD_TO, playerIdTo},
			{Constants.DB_KEYWORD_TITLE, title},
			{Constants.DB_KEYWORD_MESSAGE, message},
			{Constants.DB_KEYWORD_MAIL_TYPE, mailType},
			{Constants.DB_KEYWORD_STATUS, Constants.MAIL_STATUS_UNREAD},
			{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()}
		};
		if(replayId != null) {
			d.Add (Constants.DB_KEYWORD_REPLAY_ID, replayId);
		}
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
