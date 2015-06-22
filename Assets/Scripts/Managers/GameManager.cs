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
}
