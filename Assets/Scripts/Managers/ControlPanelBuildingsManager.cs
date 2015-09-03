using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Couchbase.Lite;

public class ControlPanelBuildingsManager : MonoBehaviour {

	public InputField[] inputFields;
	public Text messageText;
	public float messageDuration = 5.0f;

	private List<string> buildingList;
	private string selectedBuilding = "";

	private Manager manager;
	private Database db;

	private static ControlPanelBuildingsManager instance;
	private ControlPanelBuildingsManager() {}

	public static ControlPanelBuildingsManager Instance {
		get {
			if(instance == null) {
				instance = (ControlPanelBuildingsManager)GameObject.FindObjectOfType(typeof(ControlPanelBuildingsManager));
			}
			return instance;
		}
	}

	void Awake () {
		// initialize fields
		buildingList = new List<string> ();

		// initialize database
		manager = Manager.SharedInstance;
		db = manager.GetDatabase(Constants.DB_NAME);

		// initialize views
		View view = db.GetView(Constants.DB_TYPE_BUILDING);
		view.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_BUILDING)
				emit(doc[Constants.DB_KEYWORD_NAME], null);
		}, "1");
	}

	public Database GetDatabase() {
		return db;
	}

	public List<string> GetBuildingList() {
		return buildingList;
	}

	public void SaveBuilding(string name, string description,
	                         int coinCost, int cashCost,
	                         int xSize, int ySize,
	                         int xCenter, int yCenter,
	                         string prefabName, string imageName) {
		Document d = db.CreateDocument();
		if (selectedBuilding == "") {
			var properties = new Dictionary<string, object> () {
				{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_BUILDING},
				{Constants.DB_KEYWORD_NAME, name},
				{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()},
				{Constants.DB_KEYWORD_DESCRIPTION, description},
				{Constants.DB_KEYWORD_COIN_COST, coinCost},
				{Constants.DB_KEYWORD_CASH_COST, cashCost},
				{Constants.DB_KEYWORD_X_SIZE, xSize},
				{Constants.DB_KEYWORD_Y_SIZE, ySize},
				{Constants.DB_KEYWORD_X_CENTER, xCenter},
				{Constants.DB_KEYWORD_Y_CENTER, yCenter},
				{Constants.DB_KEYWORD_PREFAB_NAME, prefabName},
				{Constants.DB_KEYWORD_IMAGE_NAME, imageName}
			};
			d.PutProperties (properties);
			SetMessage("Entry created.");
		} else {
			d = db.GetDocument(LoadBuildingId(selectedBuilding));
			d.Update((UnsavedRevision newRevision) => {
				var properties = newRevision.Properties;
				properties[Constants.DB_KEYWORD_DESCRIPTION] = description;
				properties[Constants.DB_KEYWORD_COIN_COST] = coinCost;
				properties[Constants.DB_KEYWORD_CASH_COST] = cashCost;
				properties[Constants.DB_KEYWORD_X_SIZE] = xSize;
				properties[Constants.DB_KEYWORD_Y_SIZE] = ySize;
				properties[Constants.DB_KEYWORD_X_CENTER] = xCenter;
				properties[Constants.DB_KEYWORD_Y_CENTER] = yCenter;
				properties[Constants.DB_KEYWORD_PREFAB_NAME] = prefabName;
				properties[Constants.DB_KEYWORD_IMAGE_NAME] = imageName;
				return true;
			});
			SetMessage("Entry updated.");
		}
	}

	public void LoadBuilding() {
		List<string> l = new List<string> ();
		var query = db.GetView (Constants.DB_TYPE_BUILDING).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			print (row.Key);
			l.Add (row.Key.ToString ());
		}
		buildingList = l;
	}

	public IDictionary<string, object> LoadBuilding(string name) {
		var query = db.GetView (Constants.DB_TYPE_BUILDING).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			print (row.Key);
			if(row.Key.ToString() == name) {
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public string LoadBuildingId(string name) {
		var query = db.GetView (Constants.DB_TYPE_BUILDING).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			print (row.Key);
			if(row.Key.ToString() == name) {
				return row.DocumentId;
			}
		}
		return null;
	}

	public bool LoadBuildingForEditing(string name) {
		selectedBuilding = name;
		IDictionary<string, object> dic = LoadBuilding (name);
		if (inputFields.Length == 10) {
			inputFields[0].text = dic[Constants.DB_KEYWORD_NAME].ToString();
			inputFields[1].text = dic[Constants.DB_KEYWORD_DESCRIPTION].ToString();
			inputFields[2].text = dic[Constants.DB_KEYWORD_COIN_COST].ToString();
			inputFields[3].text = dic[Constants.DB_KEYWORD_CASH_COST].ToString();
			inputFields[4].text = dic[Constants.DB_KEYWORD_X_SIZE].ToString();
			inputFields[5].text = dic[Constants.DB_KEYWORD_Y_SIZE].ToString();
			inputFields[6].text = dic[Constants.DB_KEYWORD_X_CENTER].ToString();
			inputFields[7].text = dic[Constants.DB_KEYWORD_Y_CENTER].ToString();
			inputFields[8].text = dic[Constants.DB_KEYWORD_PREFAB_NAME].ToString();
			inputFields[9].text = dic[Constants.DB_KEYWORD_IMAGE_NAME].ToString();
			inputFields[0].interactable = false;
			SetMessage("Editing " + selectedBuilding);
			return true;
		}
		return false;
	}

	public void ResetFields() {
		if (inputFields.Length == 10) {
			inputFields[0].text = "";
			inputFields[1].text = "";
			inputFields[2].text = "" + 0;
			inputFields[3].text = "" + 0;
			inputFields[4].text = "" + 0;
			inputFields[5].text = "" + 0;
			inputFields[6].text = "" + 0;
			inputFields[7].text = "" + 0;
			inputFields[8].text = "";
			inputFields[9].text = "";
			inputFields[0].interactable = true;
			selectedBuilding = "";
			SetMessage("Fields reset. Currently on Create mode.");
		}
	}

	public void SetMessage(string s) {
		StopAllCoroutines ();
		StartCoroutine (CoroutineSetMessage (s));
	}

	private IEnumerator CoroutineSetMessage(string s) {
		if (messageText != null) {
			messageText.text = s;
			yield return new WaitForSeconds(messageDuration);
			messageText.text = "";
		}
	}

}
