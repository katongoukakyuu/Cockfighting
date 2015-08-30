using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BreedsManager : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas breedsCanvas;
	public Canvas messageCanvas;

	public Button addScheduleButton;
	public Button confirmButton;
	public Button backButton;

	public GameObject schedulePanel;
	public GameObject detailsPanel;

	public Toggle scheduleTab;
	public Toggle detailsTab;

	public GameObject listPanel;
	public Button listButton;

	public GameObject scheduleListPanel;
	public GameObject scheduleListItem;

	public GameObject detailsScreenPanel;

	private string state = Constants.BREEDS_MANAGER_STATE_FREE_SELECT;

	private List<Button> listButtons = new List<Button>();
	private IDictionary<string,object> selectedChicken;
	private IDictionary<string,object> selectedMate;

	private List<GameObject> scheduleListItems = new List<GameObject>();
	private List<IDictionary<string,object>> selectedSchedules;

	private IDictionary<string,object> details;

	private List<IEnumerator> countdowns = new List<IEnumerator>();

	private delegate void ButtonDelegate();
	private int x;

	private static BreedsManager instance;
	private BreedsManager() {}
	
	public static BreedsManager Instance {
		get {
			if(instance == null) {
				instance = (BreedsManager)GameObject.FindObjectOfType(typeof(BreedsManager));
			}
			return instance;
		}
	}

	public void Initialize() {
		foreach (Button b in listButtons) {
			Destroy (b.gameObject);
		}
		foreach (IEnumerator ie in countdowns) {
			StopCoroutine(ie);
		}
		listButtons.Clear ();
		countdowns.Clear ();
		
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
		if(state == Constants.BREEDS_MANAGER_STATE_FREE_SELECT) {
			addScheduleButton.interactable = true;
			foreach(IDictionary<string,object> i in PlayerManager.Instance.playerChickens) {
				if(i[Constants.DB_KEYWORD_NAME].ToString() == s) {
					selectedChicken = i;
					selectedSchedules = DatabaseManager.Instance.LoadBreedsSchedule(i[Constants.DB_COUCHBASE_ID].ToString());
					break;
				}
			}
			selectedSchedules.RemoveAll(x => x[Constants.DB_KEYWORD_IS_COMPLETED].ToString() != Constants.GENERIC_FALSE);
			InitializeSchedules ();
		}
		else if(state == Constants.BREEDS_MANAGER_STATE_ASSIGN_MATE) {
			foreach(IDictionary<string,object> i in PlayerManager.Instance.playerChickens) {
				if(i[Constants.DB_KEYWORD_NAME].ToString() == s) {
					selectedMate = i;
					confirmButton.interactable = true;
					DisplayDetails(selectedMate);
					break;
				}
			}
		}
	}

	public void SetSelectedMate() {
		DisplayMessage(Constants.MESSAGE_SCHEDULE_BREED_TITLE,
		               "Do you want to breed\n" + selectedChicken[Constants.DB_KEYWORD_NAME].ToString() + 
		               "\nwith\n" + selectedMate[Constants.DB_KEYWORD_NAME].ToString() + "?",
		               FinalizeSelectedItem);
	}

	private void FinalizeSelectedItem() {
		System.DateTime dt = TrimMilli(System.DateTime.Now.ToUniversalTime());
		foreach (IDictionary<string,object> i in selectedSchedules) {
			System.DateTime dtTemp = System.DateTime.Parse(i[Constants.DB_KEYWORD_END_TIME].ToString());
			if(dtTemp.CompareTo(dt) > 0) {
				dt = dtTemp;
			}
		}
		
		dt = dt.AddDays (Constants.BREED_DURATION_DEFAULT_DAYS);
		dt = dt.AddHours (Constants.BREED_DURATION_DEFAULT_HOURS);
		dt = dt.AddMinutes (Constants.BREED_DURATION_DEFAULT_MINUTES);
		dt = dt.AddSeconds (Constants.BREED_DURATION_DEFAULT_SECONDS);

		DatabaseManager.Instance.SaveBreedsSchedule (
			GameManager.Instance.GenerateBreedsSchedule(
				selectedChicken[Constants.DB_COUCHBASE_ID].ToString(),
				selectedMate[Constants.DB_COUCHBASE_ID].ToString(),
				dt.ToString()
			)
		);
		EndSelectMateState();
	}

	private void EndSelectMateState() {
		state = Constants.BREEDS_MANAGER_STATE_FREE_SELECT;
		SetSelected(selectedChicken[Constants.DB_KEYWORD_NAME].ToString());
		selectedMate = null;
		scheduleTab.isOn = true;
		addScheduleButton.interactable = true;
		foreach (IDictionary<string, object> i in PlayerManager.Instance.playerChickens) {
			listButtons[PlayerManager.Instance.playerChickens.IndexOf(i)].interactable = true;
		}
	}

	public void AddScheduleToList(GameObject g, IDictionary<string, object> schedule) {
		scheduleListItems.Add (g);
		g.GetComponentInChildren<BreedsScreenScheduleCancelButton>().index = scheduleListItems.IndexOf(g);
		if (schedule != null) {
			IDictionary<string,object> mate;
			if(selectedChicken[Constants.DB_COUCHBASE_ID].ToString() == DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString())[Constants.DB_COUCHBASE_ID].ToString()) {
				mate = DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString());
			}
			else {
				mate = DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString());
			}
			g.transform.FindChild(Constants.SCHEDULE_PANEL_NAME).GetComponent<Text>().text = mate[Constants.DB_KEYWORD_NAME].ToString();
			g.transform.FindChild(Constants.SCHEDULE_PANEL_STATS).GetComponent<Text>().text = GenerateStatsString(mate);
			System.DateTime dt1 = TrimMilli(System.DateTime.Now.ToUniversalTime());
			System.DateTime dt2 = TrimMilli(System.DateTime.Parse(schedule[Constants.DB_KEYWORD_END_TIME].ToString()));
			g.transform.FindChild(Constants.SCHEDULE_PANEL_TIMER).GetComponent<Text>().text = "" + (dt2 - dt1);
			IEnumerator ie = DisplayCountdown(g.transform.FindChild(Constants.SCHEDULE_PANEL_TIMER).GetComponent<Text>(),
			                                  dt2);
			StartCoroutine(ie);
			countdowns.Add (ie);
		} else {
			g.transform.FindChild(Constants.SCHEDULE_PANEL_NAME).GetComponent<Text>().text = Constants.MESSAGE_SCHEDULE_PANEL_2;
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

			state = Constants.BREEDS_MANAGER_STATE_FREE_SELECT;
			scheduleTab.interactable = true;
			detailsTab.interactable = true;
			addScheduleButton.gameObject.SetActive(true);
			confirmButton.gameObject.SetActive(false);
			backButton.GetComponentInChildren<Text>().text = Constants.GENERIC_BACK;

			schedulePanel.SetActive (true);
			detailsPanel.SetActive (false);
		}
	}

	public void SwitchToDetails(Toggle t) {
		if (t.isOn) {
			schedulePanel.SetActive (false);
			detailsPanel.SetActive (true);
		}
	}

	public void SwitchToAssignMateMode() {
		state = Constants.BREEDS_MANAGER_STATE_ASSIGN_MATE;
		detailsTab.isOn = true;

		scheduleTab.interactable = false;
		detailsTab.interactable = false;
		addScheduleButton.gameObject.SetActive(false);
		confirmButton.gameObject.SetActive(true);
		confirmButton.interactable = false;
		backButton.GetComponentInChildren<Text>().text = Constants.GENERIC_CANCEL;

		DisplayDetails(null);
		string gender = selectedChicken[Constants.DB_KEYWORD_GENDER].ToString();
		foreach (IDictionary<string, object> i in PlayerManager.Instance.playerChickens) {
			if(i[Constants.DB_KEYWORD_GENDER].ToString() == gender) {
				listButtons[PlayerManager.Instance.playerChickens.IndexOf(i)].interactable = false;
			}
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
			DisplayMessage(Constants.MESSAGE_SCHEDULE_CANCEL_TITLE, 
			               Constants.MESSAGE_SCHEDULE_CANCEL,
			               FinalizeCancelSchedule);
		}
	}

	private void FinalizeCancelSchedule() {
		IDictionary<string,object> schedule = selectedSchedules[x];
		schedule[Constants.DB_KEYWORD_IS_COMPLETED] = Constants.GENERIC_CANCELED;
		DatabaseManager.Instance.EditBreedsSchedule(schedule);
		SetSelected(selectedChicken[Constants.DB_KEYWORD_NAME].ToString());
	}

	public void ButtonBack() {
		if(state == Constants.BREEDS_MANAGER_STATE_FREE_SELECT) {
			mainCanvas.gameObject.SetActive (true);
			breedsCanvas.gameObject.SetActive (false);
		}
		else if(state == Constants.BREEDS_MANAGER_STATE_ASSIGN_MATE) {
			EndSelectMateState();
		}
	}

	private void DisplayDetails(IDictionary<string,object> i) {
		Text t = detailsScreenPanel.GetComponentInChildren<Text>();
		string s = "";
		if(i != null) {
			s += "Name: " + i[Constants.DB_KEYWORD_NAME].ToString() + "\n";
			s += "Breed: " + i[Constants.DB_TYPE_BREED].ToString() + "\n";
			s += "Stage: " + i[Constants.DB_KEYWORD_LIFE_STAGE].ToString() + "\n";
			s += "Notes: " + i[Constants.DB_KEYWORD_NOTES].ToString() + "\n";
			s += "Attack: " + i[Constants.DB_KEYWORD_ATTACK].ToString() + " / " + i[Constants.DB_KEYWORD_ATTACK_MAX].ToString() + "\n";
			s += "Defense: " + i[Constants.DB_KEYWORD_DEFENSE].ToString() + " / " + i[Constants.DB_KEYWORD_DEFENSE_MAX].ToString() + "\n";
			s += "HP: " + i[Constants.DB_KEYWORD_HP].ToString() + " / " + i[Constants.DB_KEYWORD_HP_MAX].ToString() + "\n";
			s += "Agility: " + i[Constants.DB_KEYWORD_AGILITY].ToString() + " / " + i[Constants.DB_KEYWORD_AGILITY_MAX].ToString() + "\n";
			s += "Gameness: " + i[Constants.DB_KEYWORD_GAMENESS].ToString() + " / " + i[Constants.DB_KEYWORD_GAMENESS_MAX].ToString() + "\n";
			s += "Aggression: " + i[Constants.DB_KEYWORD_AGGRESSION].ToString() + " / " + i[Constants.DB_KEYWORD_AGGRESSION_MAX].ToString();
		}
		else {
			s += Constants.MESSAGE_SCHEDULE_PANEL_3;
		}
		t.text = s;
	}

	private void RestrictChoice(string type) {
		/*if(type == null) {
			foreach(GameObject g in listDetails) {
				g.GetComponent<Button>().interactable = true;
			}
			return;
		}

		foreach(IDictionary<string, object> i in details) {
			string subtype = DatabaseManager.Instance.LoadItem(i[Constants.DB_KEYWORD_ITEM_ID].ToString())[Constants.DB_KEYWORD_SUBTYPE].ToString();
			if(subtype != type) {
				listDetails[details.IndexOf(i)].GetComponent<Button>().interactable = false;
			}
		}*/
	}

	private void DisplayMessage(string title, string message, ButtonDelegate bd) {
		messageCanvas.gameObject.SetActive(true);
		GameObject.Find("Title Text").GetComponent<Text>().text = title;
		GameObject.Find("Message Text").GetComponent<Text>().text = message;

		GameObject okButton = GameObject.Find("Msg OK Button").gameObject;
		EventTrigger trigger = okButton.GetComponentInParent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			bd ();
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