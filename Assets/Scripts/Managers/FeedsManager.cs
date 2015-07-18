using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FeedsManager : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas feedsCanvas;

	public GameObject schedulePanel;
	public GameObject inventoryPanel;

	public Toggle scheduleTab;
	public Toggle inventoryTab;

	public GameObject listPanel;
	public Button listButton;

	public GameObject scheduleListPanel;
	public GameObject scheduleListItem;

	private string state = Constants.FEEDS_MANAGER_STATE_FREE_SELECT;

	private List<Button> listButtons = new List<Button>();
	private string selectedItem;

	private List<GameObject> scheduleListItems = new List<GameObject>();
	private List<IDictionary<string,object>> selectedSchedules;

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
		
		foreach (IDictionary<string, object> i in PlayerManager.Instance.playerChickens) {
			Button b = Instantiate(listButton);
			listButtons.Add (b);
			b.GetComponentInChildren<Text> ().text = i[Constants.DB_KEYWORD_NAME].ToString();
			b.transform.SetParent(listPanel.transform,false);
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
		selectedItem = s;
		foreach(IDictionary<string,object> i in PlayerManager.Instance.playerChickens) {
			if(i[Constants.DB_KEYWORD_NAME].ToString() == s) {
				selectedSchedules = DatabaseManager.Instance.LoadFeedsSchedule(i[Constants.DB_KEYWORD_ID].ToString());
				break;
			}
		}
		InitializeSchedules ();
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
	}

	public void SwitchToSchedule(Toggle t) {
		if (t.isOn) {
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

	public void SwitchToAssignItemMode() {
		state = Constants.FEEDS_MANAGER_STATE_ASSIGN_ITEM;
		inventoryTab.isOn = true;
	}

	public void ButtonBack() {
		mainCanvas.gameObject.SetActive (true);
		feedsCanvas.gameObject.SetActive (false);
	}
}