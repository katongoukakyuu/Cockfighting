using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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
}
