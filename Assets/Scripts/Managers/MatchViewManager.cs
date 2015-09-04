using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MatchViewManager : MonoBehaviour {

	public Canvas matchViewCanvas;

	public GameObject viewPanel;
	public GameObject statsPanel;
	public GameObject infoPanel;

	//private string state = Constants.FIGHT_MANAGER_STATE_CATEGORY_SELECT;

	private List<IDictionary<string,object>> chickens = new List<IDictionary<string,object>>();
	private List<IDictionary<string,object>> players = new List<IDictionary<string,object>>();
	private IDictionary<string,object> selectedMatch;
	private IDictionary<string,object> selectedChicken;
	private IDictionary<string,object> selectedPlayer;

	public Button[] viewPanelButtons;
	public Button[] statPanelButtons;
	public Button fightButton;
	public Button betButton;
	public Button cancelMatchButton;
	public Button backButton;

	private static MatchViewManager instance;
	private MatchViewManager() {}
	
	public static MatchViewManager Instance {
		get {
			if(instance == null) {
				instance = (MatchViewManager)GameObject.FindObjectOfType(typeof(MatchViewManager));
			}
			return instance;
		}
	}

	public void Initialize(IDictionary<string,object> match) {
		selectedMatch = match;
		matchViewCanvas.gameObject.SetActive(true);
		chickens.Add (DatabaseManager.Instance.LoadChicken(match[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString()));
		players.Add (DatabaseManager.Instance.LoadPlayer(match[Constants.DB_KEYWORD_PLAYER_ID_1].ToString()));
		if(match[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString() != "") {
			chickens.Add (DatabaseManager.Instance.LoadChicken(match[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString()));
			players.Add (DatabaseManager.Instance.LoadPlayer(match[Constants.DB_KEYWORD_PLAYER_ID_2].ToString()));
			foreach(Button b in viewPanelButtons) {
				b.interactable = true;
			}
		}
		else {
			fightButton.interactable = true;
			if(match[Constants.DB_KEYWORD_PLAYER_ID_1].ToString() != PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString()) {

			}
			foreach(Button b in viewPanelButtons) {
				b.interactable = false;
			}
		}
		SetSelected(0);
	}

	public void Deinitialize() {
		matchViewCanvas.gameObject.SetActive(false);
	}

	public void SetSelected(int i) {
		selectedChicken = chickens[i];
		selectedPlayer = players[i];

		viewPanel.transform.FindChild(Constants.VIEW_MATCH_CHICKEN_VIEW_NAME).GetComponent<Text>().text = selectedChicken[Constants.DB_KEYWORD_NAME].ToString();

		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_ATK_SLIDER).GetComponent<Slider>().maxValue = float.Parse(selectedChicken[Constants.DB_KEYWORD_ATTACK_MAX].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_DEF_SLIDER).GetComponent<Slider>().maxValue = float.Parse(selectedChicken[Constants.DB_KEYWORD_DEFENSE_MAX].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_HP_SLIDER).GetComponent<Slider>().maxValue = float.Parse(selectedChicken[Constants.DB_KEYWORD_HP_MAX].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_AGI_SLIDER).GetComponent<Slider>().maxValue = float.Parse(selectedChicken[Constants.DB_KEYWORD_AGILITY_MAX].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_GAM_SLIDER).GetComponent<Slider>().maxValue = float.Parse(selectedChicken[Constants.DB_KEYWORD_GAMENESS_MAX].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_AGG_SLIDER).GetComponent<Slider>().maxValue = float.Parse(selectedChicken[Constants.DB_KEYWORD_AGGRESSION_MAX].ToString());

		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_ATK_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_ATTACK].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_DEF_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_DEFENSE].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_HP_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_HP].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_AGI_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_AGILITY].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_GAM_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_GAMENESS].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_AGG_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_AGGRESSION].ToString());

		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_FARM_NAME).GetComponent<Text>().text = selectedPlayer[Constants.DB_KEYWORD_FARM_NAME].ToString();
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_OWNER).GetComponent<Text>().text = selectedPlayer[Constants.DB_KEYWORD_USERNAME].ToString();
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_RECORD).GetComponent<Text>().text = 
			selectedPlayer[Constants.DB_KEYWORD_MATCHES_WON].ToString() + " - " + selectedPlayer[Constants.DB_KEYWORD_MATCHES_LOST].ToString() + " - " + selectedPlayer[Constants.DB_KEYWORD_MATCHES_TIED].ToString();

		infoPanel.transform.FindChild(Constants.VIEW_MATCH_INFO_BETTING_STYLE).GetComponent<Text>().text = selectedMatch[Constants.DB_KEYWORD_BETTING_OPTION].ToString();
		if(chickens[0][Constants.DB_COUCHBASE_ID].ToString() == selectedMatch[Constants.DB_KEYWORD_LLAMADO].ToString()) {
			infoPanel.transform.FindChild(Constants.VIEW_MATCH_INFO_FAVORITE).GetComponent<Text>().text = chickens[0][Constants.DB_KEYWORD_NAME].ToString();
		} else {
			infoPanel.transform.FindChild(Constants.VIEW_MATCH_INFO_FAVORITE).GetComponent<Text>().text = chickens[1][Constants.DB_KEYWORD_NAME].ToString();
		}

		IDictionary<string,object> bettingOdds = DatabaseManager.Instance.LoadBettingOdds(selectedMatch[Constants.DB_KEYWORD_BETTING_ODDS_ID].ToString());
		infoPanel.transform.FindChild(Constants.VIEW_MATCH_INFO_ODDS).GetComponent<Text>().text = bettingOdds[Constants.DB_KEYWORD_LLAMADO_ODDS].ToString() + " : " + bettingOdds[Constants.DB_KEYWORD_DEHADO_ODDS].ToString();

	}

	private void ConfirmFight() {
		MatchCreateManager.Instance.Initialize(selectedMatch);
	}

	public void ButtonFight() {
		MessageManager.Instance.DisplayMessage(Constants.MESSAGE_MATCH_VIEW_FIGHT_CONFIRM_TITLE,
		                                       Constants.MESSAGE_MATCH_VIEW_FIGHT_CONFIRM,
		                                       ConfirmFight, true);
	}

	public void ButtonBet() {

	}

	public void ButtonCancelMatch() {
		
	}

	public void ButtonBack() {
		matchViewCanvas.gameObject.SetActive (false);
	}

	public void ButtonSwitchView() {
		SetSelected((chickens.IndexOf(selectedChicken) + 1) % 2);
	}

	public void ButtonSwitchStat() {
		if(statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_PANEL_1).gameObject.activeSelf) {
			statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_VIEW_NAME).GetComponent<Text>().text = Constants.VIEW_MATCH_STAT_PANEL_2_NAME;
			statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_PANEL_1).gameObject.SetActive(false);
			statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_PANEL_2).gameObject.SetActive(true);
		}
		else {
			statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_VIEW_NAME).GetComponent<Text>().text = Constants.VIEW_MATCH_STAT_PANEL_1_NAME;
			statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_PANEL_1).gameObject.SetActive(true);
			statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_PANEL_2).gameObject.SetActive(false);
		}
	}

}