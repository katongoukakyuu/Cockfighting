using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class FightManager : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas fightCanvas;

	public GameObject listPanel;
	public GameObject matchmakingPanel;
	public GameObject matchmakingListPanel;
	public GameObject queueButton;

	private string state = Constants.FIGHT_MANAGER_STATE_CATEGORY_SELECT;

	public Button matchmakingCategoryButton;
	private List<Button> listMMCategoryButtons = new List<Button> ();
	private List<IDictionary<string,object>> listMMCategories = new List<IDictionary<string,object>> ();
	private IDictionary<string,object> selectedMMCategory;

	public GameObject matchButton;
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

	public IDictionary<string,object> GetSelectedCategory() {
		return selectedMMCategory;
	}

	public void Initialize() {
		listPanel.gameObject.SetActive(true);
		matchmakingPanel.gameObject.SetActive(false);

		foreach (Button b in listMMCategoryButtons) {
			Destroy (b.gameObject);
		}

		listMMCategoryButtons.Clear ();

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

	public void InitializeMatches() {
		listPanel.gameObject.SetActive(false);
		matchmakingPanel.gameObject.SetActive(true);

		if(selectedMMCategory[Constants.DB_KEYWORD_CUSTOM_MATCHES_ALLOWED].Equals(false)) {
			queueButton.SetActive(false);
		}
		else {
			queueButton.SetActive(true);
		}

		foreach (GameObject g in listMatchButtons) {
			Destroy (g);
		}
		foreach (IEnumerator ie in countdowns) {
			StopCoroutine(ie);
		}
		listMatchButtons.Clear ();
		countdowns.Clear ();
		
		listMatches = DatabaseManager.Instance.LoadMatchesByCategory(selectedMMCategory[Constants.DB_COUCHBASE_ID].ToString());
		foreach (IDictionary<string, object> i in listMatches) {
			IDictionary<string, object> chicken = DatabaseManager.Instance.LoadChicken(i[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString());
			IDictionary<string,object> player = DatabaseManager.Instance.LoadPlayer(i[Constants.DB_KEYWORD_PLAYER_ID_1].ToString());
			GameObject g = Instantiate(matchButton);
			listMatchButtons.Add (g);
			g.name = i[Constants.DB_COUCHBASE_ID].ToString();
			g.transform.FindChild(Constants.MATCH_PANEL_TIMER).GetComponent<Text>().text = "";

			g.transform.FindChild(Constants.MATCH_PANEL_IDLE_1).gameObject.SetActive(false);
			g.transform.FindChild(Constants.MATCH_PANEL_INFO_1).gameObject.SetActive(true);
			g.transform.FindChild(Constants.MATCH_PANEL_WLD_1).gameObject.SetActive(true);
			g.transform.FindChild(Constants.MATCH_PANEL_CHICKEN_1).GetComponent<Text>().text = chicken[Constants.DB_KEYWORD_NAME].ToString();
			g.transform.FindChild(Constants.MATCH_PANEL_FARM_1).GetComponent<Text>().text = player[Constants.DB_KEYWORD_FARM_NAME].ToString();
			g.transform.FindChild(Constants.MATCH_PANEL_WIN_1).GetComponent<Text>().text = player[Constants.DB_KEYWORD_MATCHES_WON].ToString();
			g.transform.FindChild(Constants.MATCH_PANEL_LOSE_1).GetComponent<Text>().text = player[Constants.DB_KEYWORD_MATCHES_LOST].ToString();
			g.transform.FindChild(Constants.MATCH_PANEL_DRAW_1).GetComponent<Text>().text = player[Constants.DB_KEYWORD_MATCHES_TIED].ToString();

			if(i[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString() == "") {
				g.transform.FindChild(Constants.MATCH_PANEL_IDLE_2).gameObject.SetActive(true);
				g.transform.FindChild(Constants.MATCH_PANEL_INFO_2).gameObject.SetActive(false);
				g.transform.FindChild(Constants.MATCH_PANEL_WLD_2).gameObject.SetActive(false);
			}
			else {
				chicken = DatabaseManager.Instance.LoadChicken(i[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString());
				player = DatabaseManager.Instance.LoadPlayer(i[Constants.DB_KEYWORD_PLAYER_ID_2].ToString());
				g.transform.FindChild(Constants.MATCH_PANEL_IDLE_2).gameObject.SetActive(false);
				g.transform.FindChild(Constants.MATCH_PANEL_INFO_2).gameObject.SetActive(true);
				g.transform.FindChild(Constants.MATCH_PANEL_WLD_2).gameObject.SetActive(true);
				g.transform.FindChild(Constants.MATCH_PANEL_CHICKEN_2).GetComponent<Text>().text = chicken[Constants.DB_KEYWORD_NAME].ToString();
				g.transform.FindChild(Constants.MATCH_PANEL_FARM_2).GetComponent<Text>().text = player[Constants.DB_KEYWORD_FARM_NAME].ToString();
				g.transform.FindChild(Constants.MATCH_PANEL_WIN_2).GetComponent<Text>().text = player[Constants.DB_KEYWORD_MATCHES_WON].ToString();
				g.transform.FindChild(Constants.MATCH_PANEL_LOSE_2).GetComponent<Text>().text = player[Constants.DB_KEYWORD_MATCHES_LOST].ToString();
				g.transform.FindChild(Constants.MATCH_PANEL_DRAW_2).GetComponent<Text>().text = player[Constants.DB_KEYWORD_MATCHES_TIED].ToString();
				System.DateTime dt1 = TrimMilli(System.DateTime.Now.ToUniversalTime());
				System.DateTime dt2 = TrimMilli(System.DateTime.Parse(i[Constants.DB_KEYWORD_END_TIME].ToString()));
				if(dt2 != System.DateTime.MinValue) {
					g.transform.FindChild(Constants.SCHEDULE_PANEL_TIMER).GetComponent<Text>().text = "" + (dt2 - dt1);
					IEnumerator ie = DisplayCountdown(g.transform.FindChild(Constants.MATCH_PANEL_TIMER).GetComponent<Text>(), dt2);
					StartCoroutine(ie);
					countdowns.Add (ie);
				}
			}
			
			g.transform.SetParent(matchmakingListPanel.transform,false);
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
		switch(state) {
		case Constants.FIGHT_MANAGER_STATE_CATEGORY_SELECT:
			mainCanvas.gameObject.SetActive (true);
			listPanel.gameObject.SetActive (true);
			matchmakingPanel.gameObject.SetActive (false);
			fightCanvas.gameObject.SetActive (false);
			break;
		case Constants.FIGHT_MANAGER_STATE_MATCH_SELECT:
			listPanel.gameObject.SetActive (true);
			matchmakingPanel.gameObject.SetActive (false);
			break;
		default:
			break;
		}

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