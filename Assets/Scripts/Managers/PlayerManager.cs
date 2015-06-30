using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PlayerManager : MonoBehaviour {

	public IDictionary<string,object> player;
	public List<IDictionary<string,object>> playerChickens;

	private static PlayerManager instance;
	private PlayerManager() {}

	public static PlayerManager Instance {
		get {
			if(instance == null) {
				instance = (PlayerManager)GameObject.FindObjectOfType(typeof(PlayerManager));
			}
			return instance;
		}
	}

	void Start() {
		DontDestroyOnLoad(this);
	}

}
