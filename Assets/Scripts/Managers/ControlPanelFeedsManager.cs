using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Couchbase.Lite;

public class ControlPanelFeedsManager : MonoBehaviour {

	public InputField[] inputFields;
	public Text messageText;
	public float messageDuration = 5.0f;

	private List<string> feedsList;
	private string selectedFeeds = "";

	private Manager manager;
	private Database db;

	private static ControlPanelFeedsManager instance;
	private ControlPanelFeedsManager() {}

	public static ControlPanelFeedsManager Instance {
		get {
			if(instance == null) {
				instance = (ControlPanelFeedsManager)GameObject.FindObjectOfType(typeof(ControlPanelFeedsManager));
			}
			return instance;
		}
	}

	void Awake () {
		// initialize fields
		feedsList = new List<string> ();

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
		View view = db.GetView(Constants.DB_TYPE_FEEDS);
		view.SetMap ((doc, emit) => {
			if(doc.ContainsKey(Constants.DB_KEYWORD_SUBTYPE) && doc[Constants.DB_KEYWORD_SUBTYPE].ToString () == Constants.DB_TYPE_FEEDS)
				emit(doc[Constants.DB_KEYWORD_NAME], null);
		}, "1");
	}

	public Database GetDatabase() {
		return db;
	}

	public List<string> GetFeedsList() {
		return feedsList;
	}

	public void SaveFeeds(string name, string description,
	                         int coinCost, int cashCost,
	                         int durD, int durH, int durM, int durS,
	                         int effAtk, int effDef, int effHp, int effAgi, int effGam, int effAgg, 
	                         string imageName) {
		Document d = db.CreateDocument();
		if (selectedFeeds == "") {
			var properties = new Dictionary<string, object> () {
				{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_ITEM},
				{Constants.DB_KEYWORD_SUBTYPE, Constants.DB_TYPE_FEEDS},
				{Constants.DB_KEYWORD_NAME, name},
				{Constants.DB_KEYWORD_CREATED_AT, System.DateTime.Now.ToUniversalTime().ToString ()},
				{Constants.DB_KEYWORD_DESCRIPTION, description},
				{Constants.DB_KEYWORD_COIN_COST, coinCost},
				{Constants.DB_KEYWORD_CASH_COST, cashCost},
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
				{Constants.DB_KEYWORD_IMAGE_NAME, imageName}
			};
			d.PutProperties (properties);
			SetMessage("Entry created.");
		} else {
			d = db.GetDocument(LoadFeedsId(selectedFeeds));
			d.Update((UnsavedRevision newRevision) => {
				var properties = newRevision.Properties;
				properties[Constants.DB_KEYWORD_DESCRIPTION] = description;
				properties[Constants.DB_KEYWORD_COIN_COST] = coinCost;
				properties[Constants.DB_KEYWORD_CASH_COST] = cashCost;
				properties[Constants.DB_KEYWORD_DURATION_DAYS] = durD;
				properties[Constants.DB_KEYWORD_DURATION_HOURS] = durH;
				properties[Constants.DB_KEYWORD_DURATION_MINUTES] = durM;
				properties[Constants.DB_KEYWORD_DURATION_SECONDS] = durS;
				properties[Constants.DB_KEYWORD_ATTACK] = effAtk;
				properties[Constants.DB_KEYWORD_DEFENSE] = effDef;
				properties[Constants.DB_KEYWORD_HP] = effHp;
				properties[Constants.DB_KEYWORD_AGILITY] = effAgi;
				properties[Constants.DB_KEYWORD_GAMENESS] = effGam;
				properties[Constants.DB_KEYWORD_AGGRESSION] = effAgg;
				properties[Constants.DB_KEYWORD_IMAGE_NAME] = imageName;
				return true;
			});
			SetMessage("Entry updated.");
		}
	}

	public void LoadFeeds() {
		List<string> l = new List<string> ();
		var query = db.GetView (Constants.DB_TYPE_FEEDS).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			print (row.Key);
			l.Add (row.Key.ToString ());
		}
		feedsList = l;
	}

	public IDictionary<string, object> LoadFeeds(string name) {
		var query = db.GetView (Constants.DB_TYPE_FEEDS).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			print (row.Key);
			if(row.Key.ToString() == name) {
				print ("feeds found!");
				return db.GetDocument (row.DocumentId).Properties;
			}
		}
		return null;
	}

	public string LoadFeedsId(string name) {
		var query = db.GetView (Constants.DB_TYPE_FEEDS).CreateQuery();
		var rows = query.Run ();
		foreach(var row in rows) {
			print (row.Key);
			if(row.Key.ToString() == name) {
				print ("feeds found!");
				return row.DocumentId;
			}
		}
		return null;
	}

	public bool LoadFeedsForEditing(string name) {
		selectedFeeds = name;
		IDictionary<string, object> dic = LoadFeeds (name);
		if (inputFields.Length == 15) {
			inputFields[0].text = dic[Constants.DB_KEYWORD_NAME].ToString();
			inputFields[1].text = dic[Constants.DB_KEYWORD_DESCRIPTION].ToString();
			inputFields[2].text = dic[Constants.DB_KEYWORD_COIN_COST].ToString();
			inputFields[3].text = dic[Constants.DB_KEYWORD_CASH_COST].ToString();
			inputFields[4].text = dic[Constants.DB_KEYWORD_DURATION_DAYS].ToString();
			inputFields[5].text = dic[Constants.DB_KEYWORD_DURATION_HOURS].ToString();
			inputFields[6].text = dic[Constants.DB_KEYWORD_DURATION_MINUTES].ToString();
			inputFields[7].text = dic[Constants.DB_KEYWORD_DURATION_SECONDS].ToString();
			inputFields[8].text = dic[Constants.DB_KEYWORD_ATTACK].ToString();
			inputFields[9].text = dic[Constants.DB_KEYWORD_DEFENSE].ToString();
			inputFields[10].text = dic[Constants.DB_KEYWORD_HP].ToString();
			inputFields[11].text = dic[Constants.DB_KEYWORD_AGILITY].ToString();
			inputFields[12].text = dic[Constants.DB_KEYWORD_GAMENESS].ToString();
			inputFields[13].text = dic[Constants.DB_KEYWORD_AGGRESSION].ToString();
			inputFields[14].text = dic[Constants.DB_KEYWORD_IMAGE_NAME].ToString();
			inputFields[0].interactable = false;
			SetMessage("Editing " + selectedFeeds);
			return true;
		}
		return false;
	}

	public void ResetFields() {
		if (inputFields.Length == 15) {
			inputFields[0].text = "";
			inputFields[1].text = "";
			inputFields[2].text = "" + 0;
			inputFields[3].text = "" + 0;
			inputFields[4].text = "" + 0;
			inputFields[5].text = "" + 0;
			inputFields[6].text = "" + 0;
			inputFields[7].text = "" + 0;
			inputFields[8].text = "" + 0;
			inputFields[9].text = "" + 0;
			inputFields[10].text = "" + 0;
			inputFields[11].text = "" + 0;
			inputFields[12].text = "" + 0;
			inputFields[13].text = "" + 0;
			inputFields[14].text = "";
			inputFields[0].interactable = true;
			selectedFeeds = "";
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
