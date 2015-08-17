using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class FeedsManager : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas feedsCanvas;
	public Canvas messageCanvas;

	public Button addScheduleButton;

	public GameObject schedulePanel;
	public GameObject inventoryPanel;

	public Toggle scheduleTab;
	public Toggle inventoryTab;

	public GameObject listPanel;
	public Button listButton;

	public GameObject scheduleListPanel;
	public GameObject scheduleListItem;

	public GameObject inventoryScreenPanel;
	public GameObject inventoryIcon;

	private string state = Constants.FEEDS_MANAGER_STATE_FREE_SELECT;

	private List<Button> listButtons = new List<Button>();
	private string selectedChicken;

	private List<GameObject> scheduleListItems = new List<GameObject>();
	private List<IDictionary<string,object>> selectedSchedules;

	private List<IDictionary<string,object>> inventory;
	private List<GameObject> listInventory = new List<GameObject>();
	private IDictionary<string,object> selectedItem;

	private static FeedsManager instance;
	private FeedsManager() {}
	
	public static FeedsManager Instance {
		get {
			if(instance == null) {
				instance = (FeedsManager)GameObject.FindObjectOfType(typeof(FeedsManager));
			}
			return instance;
		}
	}

	public void Initialize() {
		foreach (Button b in listButtons) {
			Destroy (b.gameObject);
		}
		listButtons.Clear ();
		listInventory.Clear ();
		
		foreach (IDictionary<string, object> i in PlayerManager.Instance.playerChickens) {
			Button b = Instantiate(listButton);
			listButtons.Add (b);
			b.GetComponentInChildren<Text> ().text = i[Constants.DB_KEYWORD_NAME].ToString();
			b.transform.SetParent(listPanel.transform,false);
		}

		inventory = DatabaseManager.Instance.LoadItemsOwnedByPlayer(PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString());
		foreach (IDictionary<string, object> i in inventory) {
			GameObject g = Instantiate (inventoryIcon);
			listInventory.Add (g);
			g.name = i[Constants.DB_KEYWORD_ITEM_ID].ToString();
			g.GetComponent<Image>().sprite = 
				Resources.Load (
					"Sprites/"+DatabaseManager.Instance.LoadItem(i[Constants.DB_KEYWORD_ITEM_ID].ToString())[Constants.DB_KEYWORD_IMAGE_NAME].ToString(),
					typeof(Sprite)
				) as Sprite;
			g.SetActive(true);
			g.GetComponentInChildren<Text> ().text = i[Constants.DB_KEYWORD_QUANTITY].ToString();
			g.transform.SetParent (inventoryScreenPanel.transform,false);
		}
	}

	public void InitializeSchedules() {
		foreach (GameObject g in scheduleListItems) {
			Destroy (g.gameObject);
		}
		scheduleListItems.Clear ();
		
		foreach (IDictionary<string, object> i in selectedSchedules) {
			GameObject g = Instantiate(scheduleListItem);
			AddScheduleToList(g, i);
			scheduleListItems.Add (g);
		}
	}

	public void SetSelected(string s) {
		selectedChicken = s;
		addScheduleButton.interactable = true;
		foreach(IDictionary<string,object> i in PlayerManager.Instance.playerChickens) {
			if(i[Constants.DB_KEYWORD_NAME].ToString() == s) {
				selectedSchedules = DatabaseManager.Instance.LoadFeedsSchedule(i[Constants.DB_KEYWORD_ID].ToString());
				break;
			}
		}
		InitializeSchedules ();
	}

	public void SetSelectedItem(string s) {
		selectedItem = DatabaseManager.Instance.LoadItem(s);
		DisplayMessage(selectedItem[Constants.DB_KEYWORD_NAME].ToString(), 
		               selectedItem[Constants.DB_KEYWORD_DESCRIPTION].ToString() + 
		               "\n\n Do you want to feed your chicken with this?");
	}

	private void FinalizeSelectedItem() {
		print ("finalizing selected item!");
		print("Now: " + System.DateTime.Now);
		System.DateTime dt = System.DateTime.Now;
		print (System.Convert.ToDouble(selectedItem[Constants.DB_KEYWORD_DURATION_DAYS].ToString()));
		dt = dt.AddDays (System.Convert.ToDouble(selectedItem[Constants.DB_KEYWORD_DURATION_DAYS].ToString()));
		dt = dt.AddHours (System.Convert.ToDouble(selectedItem[Constants.DB_KEYWORD_DURATION_HOURS].ToString()));
		dt = dt.AddMinutes (System.Convert.ToDouble(selectedItem[Constants.DB_KEYWORD_DURATION_MINUTES].ToString()));
		dt = dt.AddSeconds (System.Convert.ToDouble(selectedItem[Constants.DB_KEYWORD_DURATION_SECONDS].ToString()));
		print("Now + feed duration: " + dt);
	}

	public void AddScheduleToList(GameObject g, IDictionary<string, object> schedule) {
		scheduleListItems.Add (g);
		if (schedule != null) {
			IDictionary<string,object> feeds = DatabaseManager.Instance.LoadFeeds(schedule[Constants.DB_KEYWORD_FEEDS_ID].ToString());
			g.transform.FindChild (Constants.SCHEDULE_PANEL_ICON).GetComponent<Image> ().sprite = 
				Resources.Load (Constants.PATH_SPRITES + feeds [Constants.DB_KEYWORD_IMAGE_NAME]) as Sprite;
		} else {
			g.transform.FindChild (Constants.SCHEDULE_PANEL_ICON).GetComponent<Image> ().sprite = null;
			g.transform.FindChild(Constants.SCHEDULE_PANEL_ICON_COUNT).GetComponent<Text>().text = "";
			g.transform.FindChild(Constants.SCHEDULE_PANEL_NAME).GetComponent<Text>().text = Constants.MESSAGE_SCHEDULE_PANEL_1;
			g.transform.FindChild(Constants.SCHEDULE_PANEL_STATS).GetComponent<Text>().text = "";
			g.transform.FindChild(Constants.SCHEDULE_PANEL_TIMER).GetComponent<Text>().text = Constants.TIMER_DEFAULT;
		}
		g.transform.SetParent(scheduleListPanel.transform,false);
		if (g.transform.GetSiblingIndex() != 0) {
			g.transform.SetSiblingIndex (g.transform.GetSiblingIndex() - 1);
		}
		g.SetActive(true);
	}

	public void SwitchToSchedule(Toggle t) {
		if (t.isOn) {
			RestrictChoice(null);

			state = Constants.FEEDS_MANAGER_STATE_FREE_SELECT;
			schedulePanel.SetActive (true);
			inventoryPanel.SetActive (false);
		}
	}

	public void SwitchToInventory(Toggle t) {
		if (t.isOn) {
			schedulePanel.SetActive (false);
			inventoryPanel.SetActive (true);
		}
	}

	public void SwitchToAssignItemMode(string itemType) {
		state = Constants.FEEDS_MANAGER_STATE_ASSIGN_ITEM;
		inventoryTab.isOn = true;

		if(itemType != null) {
			RestrictChoice(itemType);
		}
	}

	public void ButtonBack() {
		mainCanvas.gameObject.SetActive (true);
		feedsCanvas.gameObject.SetActive (false);
	}

	private void RestrictChoice(string type) {
		if(type == null) {
			foreach(GameObject g in listInventory) {
				g.GetComponent<Button>().interactable = true;
			}
			return;
		}

		foreach(IDictionary<string, object> i in inventory) {
			string subtype = DatabaseManager.Instance.LoadItem(i[Constants.DB_KEYWORD_ITEM_ID].ToString())[Constants.DB_KEYWORD_SUBTYPE].ToString();
			if(subtype != type) {
				listInventory[inventory.IndexOf(i)].GetComponent<Button>().interactable = false;
			}
		}
	}

	private void DisplayMessage(string title, string message) {
		messageCanvas.gameObject.SetActive(true);
		GameObject.Find("Title Text").GetComponent<Text>().text = title;
		GameObject.Find("Message Text").GetComponent<Text>().text = message;

		GameObject okButton = GameObject.Find("Msg OK Button").gameObject;
		EventTrigger trigger = okButton.GetComponentInParent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			FeedsManager.Instance.FinalizeSelectedItem ();
			ClearMessage();
		});
		trigger.triggers.Add (entry);

		GameObject cancelButton = GameObject.Find("Msg Cancel Button").gameObject;
		trigger = cancelButton.GetComponentInParent<EventTrigger> ();
		entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			ClearMessage();
		});
		trigger.triggers.Add (entry);

	}

	private void ClearMessage() {
		GameObject okButton = GameObject.Find("Msg OK Button").gameObject;
		EventTrigger trigger = okButton.GetComponentInParent<EventTrigger> ();
		GameObject cancelButton = GameObject.Find("Msg Cancel Button").gameObject;
		EventTrigger trigger2 = cancelButton.GetComponentInParent<EventTrigger> ();

		GameObject.Find("Title Text").GetComponent<Text>().text = "";
		GameObject.Find("Message Text").GetComponent<Text>().text = "";
		trigger.triggers.Clear();
		trigger2.triggers.Clear();
		messageCanvas.gameObject.SetActive(false);
	}
}