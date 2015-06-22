using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Couchbase.Lite;

public class FarmManager : MonoBehaviour {

	public Text coinText;
	public Text cashText;

	private System.EventHandler<DatabaseChangeEventArgs> eventHandler;

	private static FarmManager instance;
	private FarmManager() {}

	public static FarmManager Instance {
		get {
			if(instance == null) {
				instance = (FarmManager)GameObject.FindObjectOfType(typeof(FarmManager));
			}
			return instance;
		}
	}

	void Start() {
		eventHandler = (sender, e) => {
			var changes = e.Changes.ToList();
			foreach (DocumentChange change in changes) {
				if(change.DocumentId == "account_" + GameManager.Instance.player["username"]) {
					JsonManager.Instance.UpdatePlayer((string)GameManager.Instance.player["username"]);
					UpdateScreen();
					return;
				}
			}
		};

		JsonManager.Instance.GetDatabase().Changed += eventHandler;
		UpdateScreen();
	}

	void OnDestroy() {
		JsonManager.Instance.GetDatabase().Changed -= eventHandler;
	}

	private void UpdateScreen() {
		coinText.text = GameManager.Instance.player["coin"].ToString();
		cashText.text = GameManager.Instance.player["cash"].ToString();
	}

}
