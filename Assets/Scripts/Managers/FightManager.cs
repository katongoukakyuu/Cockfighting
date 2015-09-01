using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class FightManager : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas fightCanvas;

	public GameObject listPanel;
	public GameObject queueButton;

	private string state = Constants.FIGHT_MANAGER_STATE_CATEGORY_SELECT;

	public Button matchmakingCategoryButton;
	private List<Button> listMMCategoryButtons = new List<Button> ();
	private List<IDictionary<string,object>> listMMCategories = new List<IDictionary<string,object>> ();
	private IDictionary<string,object> selectedMMCategory;

	public Button matchButton;
	private List<GameObject> listMatchButtons = new List<GameObject> ();
	private List<IDictionary<string,object>> listMatches = new List<IDictionary<string,object>> ();
	private IDictionary<string,object> selectedMatch;

	private List<IEnumerator> countdowns = new List<IEnumerator>();

	private static FightManager instance;
	private FightManager() {}
	
	public static FightManager Instance {
		get {
			if(instance == null) {
				instance = (FightManager)GameObject.FindObjectOfType(typeof(FightManager));
			}
			return instance;
		}
	}

	public void Initialize() {
		foreach (Button b in listMMCategoryButtons) {
			Destroy (b.gameObject);
		}
		foreach (IEnumerator ie in countdowns) {
			StopCoroutine(ie);
		}
		listMMCategoryButtons.Clear ();
		countdowns.Clear ();

		listMMCategories = DatabaseManager.Instance.LoadMatchmakingCategories ();
		foreach (IDictionary<string, object> i in listMMCategories) {
			Button b = Instantiate(matchmakingCategoryButton);
			listMMCategoryButtons.Add (b);
			b.name = i[Constants.DB_COUCHBASE_ID].ToString();
			b.GetComponentInChildren<Text> ().text = i[Constants.DB_KEYWORD_NAME].ToString();
			b.transform.SetParent(listPanel.transform,false);

			if(i[Constants.DB_KEYWORD_IS_PVP].Equals(true) && 
			   i[Constants.DB_KEYWORD_CUSTOM_MATCHES_ALLOWED].Equals(false) &&
			   DatabaseManager.Instance.LoadMatchesByCategory(i[Constants.DB_COUCHBASE_ID].ToString()).Count == 0) {
				b.interactable = false;
			}
			else {
				b.interactable = true;
			}
		}
	}

	private void InitializeMatches() {
		if(selectedMMCategory[Constants.DB_KEYWORD_CUSTOM_MATCHES_ALLOWED].Equals(false)) {
			queueButton.SetActive(false);
		}
		else {
			queueButton.SetActive(true);
		}

	}

	public void SetSelected(string s) {
		switch(state) {
		case Constants.FIGHT_MANAGER_STATE_CATEGORY_SELECT:
			state = Constants.FIGHT_MANAGER_STATE_MATCH_SELECT;
			foreach(IDictionary<string,object> id in listMMCategories) {
				if(id[Constants.DB_COUCHBASE_ID].ToString() == s) {
					selectedMMCategory = id;
					break;
				}
			}
			InitializeMatches();
			break;
		case Constants.FIGHT_MANAGER_STATE_MATCH_SELECT:

			break;
		default:
			break;
		}
	}

	public void AddMatchToList(GameObject g, IDictionary<string, object> match) {
		listMatchButtons.Add (g);
		/*g.GetComponentInChildren<FeedsScreenScheduleCancelButton>().index = scheduleListItems.IndexOf(g);
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
		*/
	}

	public void ButtonBack() {
		mainCanvas.gameObject.SetActive (true);
		fightCanvas.gameObject.SetActive (false);
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