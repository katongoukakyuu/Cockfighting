using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class FightManager : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas fightCanvas;

	public Animator FightCanvasAnimation;

	public GameObject listPanel;
	public GameObject matchmakingPanel;
	public GameObject matchmakingListPanel;
	public GameObject queueButton;
	public ScrollRect scrollRect;


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

			if(bool.Parse(i[Constants.DB_KEYWORD_IS_PVP].ToString()) && 
			   !bool.Parse(i[Constants.DB_KEYWORD_CUSTOM_MATCHES_ALLOWED].ToString()) &&
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

		if(!bool.Parse(selectedMMCategory[Constants.DB_KEYWORD_CUSTOM_MATCHES_ALLOWED].ToString())) {
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
			if(i[Constants.DB_KEYWORD_STATUS].ToString() != Constants.MATCH_STATUS_WAITING_FOR_OPPONENT && 
			   i[Constants.DB_KEYWORD_STATUS].ToString() != Constants.MATCH_STATUS_BETTING_PERIOD) {
				continue;
			}
			IDictionary<string, object> chicken = DatabaseManager.Instance.LoadChicken(i[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString());
			IDictionary<string,object> player = DatabaseManager.Instance.LoadPlayer(i[Constants.DB_KEYWORD_PLAYER_ID_1].ToString());
			IDictionary<string,object> bettingOdds = DatabaseManager.Instance.LoadBettingOdds(i[Constants.DB_KEYWORD_BETTING_ODDS_ID].ToString());

			GameObject g = Instantiate(matchButton);
			listMatchButtons.Add (g);

			g.name = i[Constants.DB_COUCHBASE_ID].ToString();
			g.GetComponentInChildren<FightScreenViewMatch>().SetMatch(i);
			g.transform.FindChild(Constants.MATCH_PANEL_TIMER).GetComponent<Text>().text = "";

			g.transform.FindChild(Constants.MATCH_PANEL_IDLE_1).gameObject.SetActive(false);
			g.transform.FindChild(Constants.MATCH_PANEL_INFO_1).gameObject.SetActive(true);
			g.transform.FindChild(Constants.MATCH_PANEL_WLD_1).gameObject.SetActive(true);
			g.transform.FindChild(Constants.MATCH_PANEL_CHICKEN_1).GetComponent<Text>().text = chicken[Constants.DB_KEYWORD_NAME].ToString();
			g.transform.FindChild(Constants.MATCH_PANEL_FARM_1).GetComponent<Text>().text = player[Constants.DB_KEYWORD_FARM_NAME].ToString();
			g.transform.FindChild(Constants.MATCH_PANEL_WIN_1).GetComponent<Text>().text = "W: " + player[Constants.DB_KEYWORD_MATCHES_WON].ToString();
			g.transform.FindChild(Constants.MATCH_PANEL_LOSE_1).GetComponent<Text>().text = "L: " + player[Constants.DB_KEYWORD_MATCHES_LOST].ToString();
			g.transform.FindChild(Constants.MATCH_PANEL_DRAW_1).GetComponent<Text>().text = "D: " + player[Constants.DB_KEYWORD_MATCHES_TIED].ToString();
			if(i[Constants.DB_KEYWORD_LLAMADO].ToString() == chicken[Constants.DB_COUCHBASE_ID].ToString()) {
				g.transform.FindChild(Constants.MATCH_PANEL_ODDS_1).GetComponent<Text>().text = "O: " + bettingOdds[Constants.DB_KEYWORD_LLAMADO_ODDS].ToString();
			}
			else {
				g.transform.FindChild(Constants.MATCH_PANEL_ODDS_1).GetComponent<Text>().text = "O: " + bettingOdds[Constants.DB_KEYWORD_DEHADO_ODDS].ToString();
			}


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
				g.transform.FindChild(Constants.MATCH_PANEL_WIN_2).GetComponent<Text>().text = "W: " + player[Constants.DB_KEYWORD_MATCHES_WON].ToString();
				g.transform.FindChild(Constants.MATCH_PANEL_LOSE_2).GetComponent<Text>().text = "L: " + player[Constants.DB_KEYWORD_MATCHES_LOST].ToString();
				g.transform.FindChild(Constants.MATCH_PANEL_DRAW_2).GetComponent<Text>().text = "D: " + player[Constants.DB_KEYWORD_MATCHES_TIED].ToString();
				if(i[Constants.DB_KEYWORD_LLAMADO].ToString() == chicken[Constants.DB_COUCHBASE_ID].ToString()) {
					g.transform.FindChild(Constants.MATCH_PANEL_ODDS_2).GetComponent<Text>().text = "O: " + bettingOdds[Constants.DB_KEYWORD_LLAMADO_ODDS].ToString();
				}
				else {
					g.transform.FindChild(Constants.MATCH_PANEL_ODDS_2).GetComponent<Text>().text = "O: " + bettingOdds[Constants.DB_KEYWORD_DEHADO_ODDS].ToString();
				}

				System.DateTime dt1 = Utility.TrimMilli(System.DateTime.Now.ToUniversalTime());
				System.DateTime dt2 = Utility.TrimMilli(System.DateTime.Parse(i[Constants.DB_KEYWORD_END_TIME].ToString()));
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

	public void ButtonBack() {
		switch(state) {
		case Constants.FIGHT_MANAGER_STATE_CATEGORY_SELECT:
				FightCanvasAnimation.SetBool("isHidden", true);
				Invoke ("FightButtonFunction",0.2f);
			break;
		case Constants.FIGHT_MANAGER_STATE_MATCH_SELECT:
			state = Constants.FIGHT_MANAGER_STATE_CATEGORY_SELECT;
			listPanel.gameObject.SetActive (true);
			matchmakingPanel.gameObject.SetActive (false);
			break;
		default:
			break;
		}
	}

	void FightButtonFunction()
	{
		switch(state) {
		case Constants.FIGHT_MANAGER_STATE_CATEGORY_SELECT:
			CameraControls.Instance.freeCamera = true;
			mainCanvas.gameObject.SetActive (true);
			listPanel.gameObject.SetActive (true);
			matchmakingPanel.gameObject.SetActive (false);
			fightCanvas.gameObject.SetActive (false);
			break;
		case Constants.FIGHT_MANAGER_STATE_MATCH_SELECT:
			state = Constants.FIGHT_MANAGER_STATE_CATEGORY_SELECT;
			listPanel.gameObject.SetActive (true);
			matchmakingPanel.gameObject.SetActive (false);
			break;
		default:
			break;
		}
	}

	private IEnumerator DisplayCountdown(Text display, System.DateTime target) {
		while (true) {
			System.TimeSpan diff = target - Utility.TrimMilli (System.DateTime.Now.ToUniversalTime ());
			if (diff.CompareTo (System.TimeSpan.Zero) <= 0) {
				display.text = "" + System.TimeSpan.Zero;
				yield break;
			}
			display.text = "" + (target - Utility.TrimMilli(System.DateTime.Now.ToUniversalTime()));
			yield return new WaitForSeconds(1f);
		}
	}
}