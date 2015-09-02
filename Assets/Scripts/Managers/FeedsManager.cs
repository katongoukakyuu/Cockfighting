using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class FeedsManager : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas feedsCanvas;

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
	private IDictionary<string,object> selectedChicken;

	private List<GameObject> scheduleListItems = new List<GameObject>();
	private List<IDictionary<string,object>> selectedSchedules;

	private List<IDictionary<string,object>> inventory;
	private List<GameObject> listInventory = new List<GameObject>();
	private IDictionary<string,object> selectedItem;

	private List<IEnumerator> countdowns = new List<IEnumerator>();

	private int x;

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
		foreach (GameObject g in listInventory) {
			Destroy (g);
		}
		foreach (IEnumerator ie in countdowns) {
			StopCoroutine(ie);
		}
		listButtons.Clear ();
		listInventory.Clear ();
		countdowns.Clear ();
		
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
				Resources.Load<Sprite> (
					Constants.PATH_SPRITES+DatabaseManager.Instance.LoadItem(i[Constants.DB_KEYWORD_ITEM_ID].ToString())[Constants.DB_KEYWORD_IMAGE_NAME].ToString()
				);
			g.SetActive(true);
			g.GetComponentInChildren<Text> ().text = i[Constants.DB_KEYWORD_QUANTITY].ToString();
			g.transform.SetParent (inventoryScreenPanel.transform,false);
		}
	}

	public void InitializeSchedules() {
		foreach (GameObject g in scheduleListItems) {
			Destroy (g.gameObject);
		}
		foreach (IEnumerator ie in countdowns) {
			StopCoroutine(ie);
		}
		scheduleListItems.Clear ();
		countdowns.Clear ();
		
		foreach (IDictionary<string, object> i in selectedSchedules) {
			GameObject g = Instantiate(scheduleListItem);
			AddScheduleToList(g, i);
		}
	}

	public void SetSelected(string s) {
		addScheduleButton.interactable = true;
		foreach(IDictionary<string,object> i in PlayerManager.Instance.playerChickens) {
			if(i[Constants.DB_KEYWORD_NAME].ToString() == s) {
				selectedChicken = i;
				selectedSchedules = DatabaseManager.Instance.LoadFeedsSchedule(i[Constants.DB_COUCHBASE_ID].ToString());
				break;
			}
		}
		selectedSchedules.RemoveAll(x => x[Constants.DB_KEYWORD_IS_COMPLETED].ToString() != Constants.GENERIC_FALSE);
		InitializeSchedules ();
	}

	public void SetSelectedItem(string s) {
		selectedItem = DatabaseManager.Instance.LoadItem(s);
		MessageManager.Instance.DisplayMessage(selectedItem[Constants.DB_KEYWORD_NAME].ToString(), 
		               selectedItem[Constants.DB_KEYWORD_DESCRIPTION].ToString() + 
		               "\n\n " + Constants.MESSAGE_SCHEDULE_FEED,
		               FinalizeSelectedItem, true);
	}

	private void FinalizeSelectedItem() {
		System.DateTime dt = TrimMilli(System.DateTime.Now.ToUniversalTime());
		foreach (IDictionary<string,object> i in selectedSchedules) {
			System.DateTime dtTemp = System.DateTime.Parse(i[Constants.DB_KEYWORD_END_TIME].ToString());
			if(dtTemp.CompareTo(dt) > 0) {
				dt = dtTemp;
			}
		}
		dt = dt.AddDays (System.Convert.ToDouble(selectedItem[Constants.DB_KEYWORD_DURATION_DAYS].ToString()));
		dt = dt.AddHours (System.Convert.ToDouble(selectedItem[Constants.DB_KEYWORD_DURATION_HOURS].ToString()));
		dt = dt.AddMinutes (System.Convert.ToDouble(selectedItem[Constants.DB_KEYWORD_DURATION_MINUTES].ToString()));
		dt = dt.AddSeconds (System.Convert.ToDouble(selectedItem[Constants.DB_KEYWORD_DURATION_SECONDS].ToString()));
		DatabaseManager.Instance.SaveFeedsSchedule (
			GameManager.Instance.GenerateFeedsSchedule(
				selectedChicken[Constants.DB_COUCHBASE_ID].ToString(),
				selectedItem[Constants.DB_COUCHBASE_ID].ToString(),
				dt.ToString()
			)
		);
		SetSelected(selectedChicken[Constants.DB_KEYWORD_NAME].ToString());
		scheduleTab.isOn = true;
		addScheduleButton.interactable = true;
	}

	public void AddScheduleToList(GameObject g, IDictionary<string, object> schedule) {
		scheduleListItems.Add (g);
		g.GetComponentInChildren<FeedsScreenScheduleCancelButton>().index = scheduleListItems.IndexOf(g);
		if (schedule != null) {
			IDictionary<string,object> feeds = DatabaseManager.Instance.LoadFeeds(schedule[Constants.DB_KEYWORD_FEEDS_ID].ToString());
			g.transform.FindChild (Constants.SCHEDULE_PANEL_ICON).GetComponent<Image> ().sprite = 
				Resources.Load<Sprite>(Constants.PATH_SPRITES + feeds [Constants.DB_KEYWORD_IMAGE_NAME].ToString ());
			foreach (IDictionary<string, object> i in inventory) {
				if(feeds[Constants.DB_COUCHBASE_ID].ToString() == 
				   i[Constants.DB_KEYWORD_ITEM_ID].ToString()) {
					g.transform.FindChild(Constants.SCHEDULE_PANEL_ICON_COUNT).GetComponent<Text>().text = i[Constants.DB_KEYWORD_QUANTITY].ToString();
					g.transform.FindChild(Constants.SCHEDULE_PANEL_NAME).GetComponent<Text>().text = feeds[Constants.DB_KEYWORD_NAME].ToString();
					g.transform.FindChild(Constants.SCHEDULE_PANEL_STATS).GetComponent<Text>().text = GenerateStatsString(feeds);
					System.DateTime dt1 = TrimMilli(System.DateTime.Now.ToUniversalTime());
					System.DateTime dt2 = TrimMilli(System.DateTime.Parse(schedule[Constants.DB_KEYWORD_END_TIME].ToString()));
					g.transform.FindChild(Constants.SCHEDULE_PANEL_TIMER).GetComponent<Text>().text = "" + (dt2 - dt1);
					IEnumerator ie = DisplayCountdown(g.transform.FindChild(Constants.SCHEDULE_PANEL_TIMER).GetComponent<Text>(),
					                                      dt2);
					StartCoroutine(ie);
					countdowns.Add (ie);
					break;
				}
			}
			g.transform.FindChild(Constants.SCHEDULE_PANEL_ICON).GetComponent<Button>().enabled = false;
		} else {
			g.transform.FindChild(Constants.SCHEDULE_PANEL_ICON).GetComponent<Image> ().sprite = null;
			g.transform.FindChild(Constants.SCHEDULE_PANEL_ICON_COUNT).GetComponent<Text>().text = "";
			g.transform.FindChild(Constants.SCHEDULE_PANEL_NAME).GetComponent<Text>().text = Constants.MESSAGE_SCHEDULE_PANEL_1;
			g.transform.FindChild(Constants.SCHEDULE_PANEL_STATS).GetComponent<Text>().text = "";
			g.transform.FindChild(Constants.SCHEDULE_PANEL_TIMER).GetComponent<Text>().text = Constants.TIMER_DEFAULT;
			addScheduleButton.interactable = false;
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

	public void CancelSchedule(int index) {
		if(index >= selectedSchedules.Count) {
			GameObject g = scheduleListItems[index];
			scheduleListItems.Remove(g);
			Destroy(g);
			addScheduleButton.interactable = true;
		}
		else {
			x = index;
			MessageManager.Instance.DisplayMessage(Constants.MESSAGE_SCHEDULE_CANCEL_TITLE, 
			               Constants.MESSAGE_SCHEDULE_CANCEL,
			               FinalizeCancelSchedule, true);
		}
	}

	private void FinalizeCancelSchedule() {
		IDictionary<string,object> schedule = selectedSchedules[x];
		schedule[Constants.DB_KEYWORD_IS_COMPLETED] = Constants.GENERIC_CANCELED;
		DatabaseManager.Instance.EditFeedsSchedule(schedule);
		SetSelected(selectedChicken[Constants.DB_KEYWORD_NAME].ToString());
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

	private string GenerateStatsString(IDictionary<string, object> i) {
		string s = "";
		int result = 0;
		if (int.TryParse (i [Constants.DB_KEYWORD_ATTACK].ToString (), out result)) {
			s += "| ATK: " + result + " ";
		}
		if (int.TryParse (i [Constants.DB_KEYWORD_DEFENSE].ToString (), out result)) {
			s += "| DEF: " + result + " ";
		}
		if (int.TryParse (i [Constants.DB_KEYWORD_HP].ToString (), out result)) {
			s += "| HP: " + result + " ";
		}
		if (int.TryParse (i [Constants.DB_KEYWORD_AGILITY].ToString (), out result)) {
			s += "| AGI: " + result + " ";
		}
		if (int.TryParse (i [Constants.DB_KEYWORD_GAMENESS].ToString (), out result)) {
			s += "| GAM: " + result + " ";
		}
		if (int.TryParse (i [Constants.DB_KEYWORD_AGGRESSION].ToString (), out result)) {
			s += "| AGG: " + result + " ";
		}
		s += "|";
		return s;
	}

	private System.DateTime TrimMilli(System.DateTime dt)
	{
		return new System.DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, dt.Kind);
	}

	private IEnumerator DisplayCountdown(Text display, System.DateTime target) {
		while (true) {
			System.TimeSpan diff = target - TrimMilli (System.DateTime.Now.ToUniversalTime ());
			if (diff.CompareTo (System.TimeSpan.Zero) <= 0) {
				display.text = "" + System.TimeSpan.Zero;
				yield break;
			}
			display.text = "" + (target - TrimMilli(System.DateTime.Now.ToUniversalTime()));
			yield return new WaitForSeconds(1f);
		}
	}
}