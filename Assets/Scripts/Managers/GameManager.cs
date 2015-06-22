using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour {

	public IDictionary<string,object> player;

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
	}

	public Dictionary<string, object> RegisterAccount(string username, string email) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{"type", Constants.DB_TYPE_ACCOUNT},
			{"username", username},
			{"password", "test"},
			{"email", email},
			{"created at", System.DateTime.UtcNow.ToString()},
			{"matches won", 0},
			{"matches lost", 0},
			{"matches tied", 0},
			{"coin", Constants.COIN_START},
			{"cash", Constants.CASH_START}
		};
		return d;
	}

	public Dictionary<string, object> GenerateChicken(string username, string gender) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{"type", Constants.DB_TYPE_CHICKEN},
			{"owner", username},
			{"created at", System.DateTime.UtcNow.ToString()},
			{"breed", "Kelso"},
			{"gender", gender},
			{"notes", ""},
			{"attack", Constants.CHICKEN_ATTACK_DEFAULT_START},
			{"defense", Constants.CHICKEN_DEFENSE_DEFAULT_START},
			{"hp", Constants.CHICKEN_HP_DEFAULT_START},
			{"agility", Constants.CHICKEN_AGILITY_DEFAULT_START},
			{"gameness", Constants.CHICKEN_GAMENESS_DEFAULT_START},
			{"aggression", Constants.CHICKEN_AGGRESSION_DEFAULT_START},
			{"attack max", Constants.CHICKEN_ATTACK_DEFAULT_MAX},
			{"defense max", Constants.CHICKEN_DEFENSE_DEFAULT_MAX},
			{"hp max", Constants.CHICKEN_HP_DEFAULT_MAX},
			{"agility max", Constants.CHICKEN_AGILITY_DEFAULT_MAX},
			{"gameness max", Constants.CHICKEN_GAMENESS_DEFAULT_MAX},
			{"aggression max", Constants.CHICKEN_AGGRESSION_DEFAULT_MAX},
			{"genetic id", "00000000F1"},
			{"life stage", Constants.LIFE_STAGE_STAG}
		};
		return d;
	}
}
