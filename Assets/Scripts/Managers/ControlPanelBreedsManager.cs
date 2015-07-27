using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Couchbase.Lite;

public class ControlPanelBreedsManager : MonoBehaviour {

	public InputField[] inputFields;
	public Text messageText;
	public float messageDuration = 5.0f;

	private List<string> breedList;
	private string selectedBreed = "";

	private Manager manager;
	private Database db;

	private static ControlPanelBreedsManager instance;
	private ControlPanelBreedsManager() {}

	public static ControlPanelBreedsManager Instance {
		get {
			if(instance == null) {
				instance = (ControlPanelBreedsManager)GameObject.FindObjectOfType(typeof(ControlPanelBreedsManager));
			}
			return instance;
		}
	}

	void Awake () {
		// initialize fields
		breedList = new List<string> ();

		// initialize database
		manager = Manager.SharedInstance;
		db = manager.GetDatabase(Constants.DB_NAME);
		db.Changed += (sender, e) => {
			var changes = e.Changes.ToList();
			foreach (DocumentChange change in changes) {
				print("Document " + change.DocumentId + " changed");
			}
		};

		// initialize views
		View view = db.GetView(Constants.DB_TYPE_BREED);
		view.SetMap ((doc, emit) => {
			if(doc[Constants.DB_KEYWORD_TYPE].ToString () == Constants.DB_TYPE_BREED)
				emit(doc[Constants.DB_KEYWORD_NAME], null);
		}, "1");
	}

	public Database GetDatabase() {
		return db;
	}

	public List<string> GetBreedList() {
		return breedList;
	}

	public void SaveBreed(string name,
	                      string headColor1, int headColor1Chance, string headColor2,
	                      string bodyColor1, int bodyColor1Chance, string bodyColor2,
	                      string wingColor1, int wingColor1Chance, string wingColor2,
	                      string tailColor1, int tailColor1Chance, string tailColor2) {
		print (db);
		Document d = db.CreateDocument ();
		if (selectedBreed == "") {
			var properties = new Dictionary<string, object> () {
				{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_BREED},
				{Constants.DB_KEYWORD_NAME, name},
				{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.UtcNow.ToString()},
				{Constants.DB_KEYWORD_HEAD_COLOR_1, headColor1},
				{Constants.DB_KEYWORD_HEAD_COLOR_1_CHANCE, headColor1Chance},
				{Constants.DB_KEYWORD_HEAD_COLOR_2, headColor2},
				{Constants.DB_KEYWORD_HEAD_COLOR_2_CHANCE, 100 - headColor1Chance},
				{Constants.DB_KEYWORD_BODY_COLOR_1, bodyColor1},
				{Constants.DB_KEYWORD_BODY_COLOR_1_CHANCE, bodyColor1Chance},
				{Constants.DB_KEYWORD_BODY_COLOR_2, bodyColor2},
				{Constants.DB_KEYWORD_BODY_COLOR_2_CHANCE, 100 - bodyColor1Chance},
				{Constants.DB_KEYWORD_WING_COLOR_1, wingColor1},
				{Constants.DB_KEYWORD_WING_COLOR_1_CHANCE, wingColor1Chance},
				{Constants.DB_KEYWORD_WING_COLOR_2, wingColor2},
				{Constants.DB_KEYWORD_WING_COLOR_2_CHANCE, 100 - wingColor1Chance},
				{Constants.DB_KEYWORD_TAIL_COLOR_1, tailColor1},
				{Constants.DB_KEYWORD_TAIL_COLOR_1_CHANCE, tailColor1Chance},
				{Constants.DB_KEYWORD_TAIL_COLOR_2, tailColor2},
				{Constants.DB_KEYWORD_TAIL_COLOR_2_CHANCE, 100 - tailColor1Chance}
			};
			d.PutProperties (properties);
			SetMessage("Entry created.");
		} else {
			d = db.GetDocument(LoadBreedId(selectedBreed));
			d.Update((UnsavedRevision newRevision) => {
				var properties = newRevision.Properties;
				properties[Constants.DB_KEYWORD_HEAD_COLOR_1] = headColor1;
				properties[Constants.DB_KEYWORD_HEAD_COLOR_1_CHANCE] = headColor1Chance;
				properties[Constants.DB_KEYWORD_HEAD_COLOR_2] = headColor2;
				properties[Constants.DB_KEYWORD_HEAD_COLOR_2_CHANCE] = 100 - headColor1Chance;
				properties[Constants.DB_KEYWORD_BODY_COLOR_1] = bodyColor1;
				properties[Constants.DB_KEYWORD_BODY_COLOR_1_CHANCE] = bodyColor1Chance;
				properties[Constants.DB_KEYWORD_BODY_COLOR_2] = bodyColor2;
				properties[Constants.DB_KEYWORD_BODY_COLOR_2_CHANCE] = 100 - bodyColor1Chance;
				properties[Constants.DB_KEYWORD_WING_COLOR_1] = wingColor1;
				properties[Constants.DB_KEYWORD_WING_COLOR_1_CHANCE] = wingColor1Chance;
				properties[Constants.DB_KEYWORD_WING_COLOR_2] = wingColor2;
				properties[Constants.DB_KEYWORD_WING_COLOR_2_CHANCE] = 100 - wingColor1Chance;
				properties[Constants.DB_KEYWORD_TAIL_COLOR_1] = tailColor1;
				properties[Constants.DB_KEYWORD_TAIL_COLOR_1_CHANCE] = tailColor1Chance;
				properties[Constants.DB_KEYWORD_TAIL_COLOR_2] = tailColor2;
				properties[Constants.DB_KEYWORD_TAIL_COLOR_2_CHANCE] = 100 - tailColor1Chance;
				return true;
			});
			SetMessage("Entry updated.");
		}
	}

	public void LoadBreed() {
		List<string> l = new List<string> ();
		var query = db.GetView (Constants.DB_TYPE_BREED).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			print (row.Key);
			l.Add (row.Key.ToString ());
		}
		breedList = l;
	}

	public string LoadBreedId(string name) {
		var query = db.GetView (Constants.DB_TYPE_BREED).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			print (row.Key);
			if(row.Key.ToString() == name) {
				print ("breed found!");
				return row.DocumentId;
			}
		}
		return null;
	}

	public IDictionary<string, object> LoadBreed(string name) {
		var query = db.GetView (Constants.DB_TYPE_BREED).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			print (row.Key);
			if(row.Key.ToString() == name) {
				print ("breed found!");
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public bool LoadBreedForEditing(string name) {
		selectedBreed = name;
		IDictionary<string, object> dic = LoadBreed (name);
		if (inputFields.Length == 17) {
			inputFields[0].text = dic[Constants.DB_KEYWORD_NAME].ToString();
			inputFields[1].text = dic[Constants.DB_KEYWORD_HEAD_COLOR_1].ToString();
			inputFields[2].text = dic[Constants.DB_KEYWORD_HEAD_COLOR_1_CHANCE].ToString();
			inputFields[3].text = dic[Constants.DB_KEYWORD_HEAD_COLOR_2].ToString();
			inputFields[4].text = dic[Constants.DB_KEYWORD_HEAD_COLOR_2_CHANCE].ToString();
			inputFields[5].text = dic[Constants.DB_KEYWORD_BODY_COLOR_1].ToString();
			inputFields[6].text = dic[Constants.DB_KEYWORD_BODY_COLOR_1_CHANCE].ToString();
			inputFields[7].text = dic[Constants.DB_KEYWORD_BODY_COLOR_2].ToString();
			inputFields[8].text = dic[Constants.DB_KEYWORD_BODY_COLOR_2_CHANCE].ToString();
			inputFields[9].text = dic[Constants.DB_KEYWORD_WING_COLOR_1].ToString();
			inputFields[10].text = dic[Constants.DB_KEYWORD_WING_COLOR_1_CHANCE].ToString();
			inputFields[11].text = dic[Constants.DB_KEYWORD_WING_COLOR_2].ToString();
			inputFields[12].text = dic[Constants.DB_KEYWORD_WING_COLOR_2_CHANCE].ToString();
			inputFields[13].text = dic[Constants.DB_KEYWORD_TAIL_COLOR_1].ToString();
			inputFields[14].text = dic[Constants.DB_KEYWORD_TAIL_COLOR_1_CHANCE].ToString();
			inputFields[15].text = dic[Constants.DB_KEYWORD_TAIL_COLOR_2].ToString();
			inputFields[16].text = dic[Constants.DB_KEYWORD_TAIL_COLOR_2_CHANCE].ToString();

			inputFields[0].interactable = false;
			SetMessage("Editing " + selectedBreed);
			return true;
		}
		return false;
	}

	public void ResetFields() {
		if (inputFields.Length == 17) {
			inputFields[0].text = "";
			inputFields[1].text = "";
			inputFields[2].text = "" + 100;
			inputFields[3].text = "";
			inputFields[4].text = "" + 0;
			inputFields[5].text = "";
			inputFields[6].text = "" + 100;
			inputFields[7].text = "";
			inputFields[8].text = "" + 0;
			inputFields[9].text = "";
			inputFields[10].text = "" + 100;
			inputFields[11].text = "";
			inputFields[12].text = "" + 0;
			inputFields[13].text = "";
			inputFields[14].text = "" + 100;
			inputFields[15].text = "";
			inputFields[16].text = "" + 0;

			inputFields[0].interactable = true;
			selectedBreed = "";
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
