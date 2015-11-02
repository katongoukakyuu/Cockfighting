using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class MatchViewManager : MonoBehaviour {

	public Canvas matchViewCanvas;

	public GameObject viewPanel;
	public GameObject statsPanel;
	public GameObject infoPanel;

	//private string state = Constants.FIGHT_MANAGER_STATE_CATEGORY_SELECT;

	private List<IDictionary<string,object>> chickens = new List<IDictionary<string,object>>();
	private List<IDictionary<string,object>> players = new List<IDictionary<string,object>>();
	private IDictionary<string,object> selectedMatch;
	private IDictionary<string,object> bettingOption;
	private IDictionary<string,object> initialBet;
	private int minimumBet;
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
		matchViewCanvas.gameObject.SetActive(true);
		selectedMatch = match;
		chickens.Clear();
		players.Clear();
		chickens.Add (DatabaseManager.Instance.LoadChicken(match[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString()));
		players.Add (DatabaseManager.Instance.LoadPlayer(match[Constants.DB_KEYWORD_PLAYER_ID_1].ToString()));
		initialBet = DatabaseManager.Instance.LoadBet(selectedMatch[Constants.DB_COUCHBASE_ID].ToString(),players[0][Constants.DB_COUCHBASE_ID].ToString());
		if(initialBet != null) {
			minimumBet = (int)(int.Parse(initialBet[Constants.DB_KEYWORD_BET_AMOUNT].ToString()) * Constants.MINIMUM_BET_RATIO);
		}
		if(match[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString() != "") {
			chickens.Add (DatabaseManager.Instance.LoadChicken(match[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString()));
			players.Add (DatabaseManager.Instance.LoadPlayer(match[Constants.DB_KEYWORD_PLAYER_ID_2].ToString()));
			fightButton.interactable = false;
			if(match[Constants.DB_KEYWORD_STATUS].ToString() == Constants.MATCH_STATUS_BETTING_PERIOD && 
			   match[Constants.DB_KEYWORD_PLAYER_ID_1].ToString() != PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString() &&
			   match[Constants.DB_KEYWORD_PLAYER_ID_2].ToString() != PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString()) {
				betButton.interactable = true;
			}
			else {
				betButton.interactable = false;
			}
			if(match[Constants.DB_KEYWORD_PLAYER_ID_1].ToString() != PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString()) {
				cancelMatchButton.interactable = false;
			}
			else {
				cancelMatchButton.interactable = true;
			}
			foreach(Button b in viewPanelButtons) {
				b.interactable = true;
			}
		}
		else {
			if(match[Constants.DB_KEYWORD_PLAYER_ID_1].ToString() != PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString()) {
				fightButton.interactable = true;
				cancelMatchButton.interactable = false;
			}
			else {
				fightButton.interactable = false;
				cancelMatchButton.interactable = true;
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
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_CON_SLIDER).GetComponent<Slider>().maxValue = Constants.CHICKEN_CONDITIONING_DEFAULT_MAX;

		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_ATK_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_ATTACK].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_DEF_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_DEFENSE].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_HP_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_HP].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_AGI_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_AGILITY].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_GAM_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_GAMENESS].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_AGG_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_AGGRESSION].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_CON_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_CONDITIONING].ToString());

		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_FARM_NAME).GetComponent<Text>().text = selectedPlayer[Constants.DB_KEYWORD_FARM_NAME].ToString();
		FBGetName(selectedPlayer[Constants.DB_KEYWORD_USER_ID].ToString());
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_RECORD).GetComponent<Text>().text = 
			selectedPlayer[Constants.DB_KEYWORD_MATCHES_WON].ToString() + " - " + selectedPlayer[Constants.DB_KEYWORD_MATCHES_LOST].ToString() + " - " + selectedPlayer[Constants.DB_KEYWORD_MATCHES_TIED].ToString();

		infoPanel.transform.FindChild(Constants.VIEW_MATCH_INFO_BETTING_STYLE).GetComponent<Text>().text = selectedMatch[Constants.DB_KEYWORD_BETTING_OPTION].ToString();
		if(selectedMatch[Constants.DB_KEYWORD_BETTING_OPTION].ToString() == Constants.BETTING_OPTION_SINGLE_BETTING) {
			infoPanel.transform.FindChild(Constants.VIEW_MATCH_INFO_BETTING_STYLE).GetComponent<Text>().text += " - " + initialBet[Constants.DB_KEYWORD_BET_AMOUNT].ToString();
		}
		if(chickens[0][Constants.DB_COUCHBASE_ID].ToString() == selectedMatch[Constants.DB_KEYWORD_LLAMADO].ToString()) {
			infoPanel.transform.FindChild(Constants.VIEW_MATCH_INFO_FAVORITE).GetComponent<Text>().text = chickens[0][Constants.DB_KEYWORD_NAME].ToString() + " - " + players[0][Constants.DB_KEYWORD_FARM_NAME].ToString();
		} else {
			infoPanel.transform.FindChild(Constants.VIEW_MATCH_INFO_FAVORITE).GetComponent<Text>().text = chickens[1][Constants.DB_KEYWORD_NAME].ToString() + " - " + players[1][Constants.DB_KEYWORD_FARM_NAME].ToString();
		}

		IDictionary<string,object> bettingOdds = DatabaseManager.Instance.LoadBettingOdds(selectedMatch[Constants.DB_KEYWORD_BETTING_ODDS_ID].ToString());
		infoPanel.transform.FindChild(Constants.VIEW_MATCH_INFO_ODDS).GetComponent<Text>().text = bettingOdds[Constants.DB_KEYWORD_LLAMADO_ODDS].ToString() + " : " + bettingOdds[Constants.DB_KEYWORD_DEHADO_ODDS].ToString();

	}

	public void CreateBet(int amount) {
		string isLlamado;
		if(selectedMatch[Constants.DB_KEYWORD_LLAMADO].ToString() == selectedChicken[Constants.DB_COUCHBASE_ID].ToString()) {
			isLlamado = Constants.BETTED_CHICKEN_STATUS_LLAMADO;
		}
		else {
			isLlamado = Constants.DB_KEYWORD_DEHADO;
		}
		DatabaseManager.Instance.SaveEntry(GameManager.Instance.GenerateBet(selectedMatch[Constants.DB_COUCHBASE_ID].ToString(), PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString(), 
		                                                                  selectedMatch[Constants.DB_KEYWORD_BETTING_ODDS_ID].ToString(), selectedChicken[Constants.DB_COUCHBASE_ID].ToString(), isLlamado,
		                                                                  amount));
		IDictionary<string,object> player = PlayerManager.Instance.player;
		player[Constants.DB_KEYWORD_COIN] = (int.Parse(player[Constants.DB_KEYWORD_COIN].ToString()) - amount);
		DatabaseManager.Instance.EditAccount(player);
		Deinitialize();
	}

	private void ConfirmFight() {
		MatchCreateManager.Instance.Initialize(selectedMatch);
	}

	public void ButtonFight() {
		string title, message;
		if((selectedMatch[Constants.DB_KEYWORD_BETTING_OPTION].ToString() == Constants.BETTING_OPTION_SINGLE_BETTING && 
		   	int.Parse (PlayerManager.Instance.player[Constants.DB_KEYWORD_COIN].ToString()) < int.Parse (initialBet[Constants.DB_KEYWORD_BET_AMOUNT].ToString())) ||
		   (selectedMatch[Constants.DB_KEYWORD_BETTING_OPTION].ToString() == Constants.BETTING_OPTION_SPECTATOR_BETTING && 
		 	int.Parse (PlayerManager.Instance.player[Constants.DB_KEYWORD_COIN].ToString()) < minimumBet)) {
			title = Constants.MESSAGE_MATCH_CREATE_NOT_ENOUGH_COINS_TITLE;
			message = Constants.MESSAGE_MATCH_CREATE_NOT_ENOUGH_COINS;
			MessageManager.Instance.DisplayMessage(title, message, null, false);
			return;
		}
		if(selectedMatch[Constants.DB_KEYWORD_BETTING_OPTION].ToString() == Constants.BETTING_OPTION_SINGLE_BETTING) {
			title = Constants.MESSAGE_MATCH_VIEW_CONFIRM_FIGHT_TITLE;
			message = Constants.MESSAGE_MATCH_VIEW_CONFIRM_SINGLE_BET_1 + initialBet[Constants.DB_KEYWORD_BET_AMOUNT].ToString() + Constants.MESSAGE_MATCH_VIEW_CONFIRM_SINGLE_BET_2;
		}
		else if(selectedMatch[Constants.DB_KEYWORD_BETTING_OPTION].ToString() == Constants.BETTING_OPTION_SPECTATOR_BETTING) {
			title = Constants.MESSAGE_MATCH_VIEW_CONFIRM_FIGHT_TITLE;
			message = Constants.MESSAGE_MATCH_VIEW_CONFIRM_SPECTATOR_BET_1 + minimumBet + Constants.MESSAGE_MATCH_VIEW_CONFIRM_SPECTATOR_BET_2;
		}
		else {
			title = Constants.MESSAGE_MATCH_VIEW_CONFIRM_FIGHT_TITLE;
			message = Constants.MESSAGE_MATCH_VIEW_CONFIRM_FIGHT;
		}
		MessageManager.Instance.DisplayMessage(title, message, ConfirmFight, true);
	}

	public void ButtonBet() {
		MatchBetManager.Instance.Initialize(selectedMatch, selectedChicken);
	}

	private void ConfirmCancel() {
		selectedMatch[Constants.DB_KEYWORD_STATUS] = Constants.MATCH_STATUS_CANCELED;
		DatabaseManager.Instance.EditMatch(selectedMatch);
		Deinitialize();
		FightManager.Instance.InitializeMatches();
	}

	public void ButtonCancelMatch() {
		MessageManager.Instance.DisplayMessage(Constants.MESSAGE_MATCH_VIEW_CANCEL_FIGHT_TITLE, 
		                                       Constants.MESSAGE_MATCH_VIEW_CANCEL_FIGHT, 
		                                       ConfirmCancel, true);
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

	private void FBGetName(string userId) {
		FB.API ("/"+userId,HttpMethod.GET, GetNameCallback);
	}
	
	private void GetNameCallback(IResult result) {
		print ("GetNameCallback result: " + result.RawResult);
		IDictionary id = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as IDictionary;
		statsPanel.transform.FindChild(Constants.VIEW_MATCH_STAT_OWNER).GetComponent<Text>().text = id["name"].ToString();
	}

}