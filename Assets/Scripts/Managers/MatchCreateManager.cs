using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Couchbase.Lite;

public class MatchCreateManager : MonoBehaviour {

	public Canvas matchCreateCanvas;

	public GameObject listPanel;
	public GameObject statsPanel;
	public GameObject optionsPanel;
	public ScrollRect scrollRect;

	//private string state = Constants.FIGHT_MANAGER_STATE_CATEGORY_SELECT;

	public Button listChickenButton;
	private List<Button> listChickenButtons = new List<Button> ();
	private IDictionary<string,object> selectedChicken;

	private IDictionary<string,object> selectedMatch;

	public Button okButton;
	public Button cancelButton;

	private System.EventHandler<DatabaseChangeEventArgs> eventHandler;

	private static MatchCreateManager instance;
	private MatchCreateManager() {}
	
	public static MatchCreateManager Instance {
		get {
			if(instance == null) {
				instance = (MatchCreateManager)GameObject.FindObjectOfType(typeof(MatchCreateManager));
			}
			return instance;
		}
	}

	void Start() {
		eventHandler = (sender, e) => {
			var changes = e.Changes.ToList();
			foreach (DocumentChange change in changes) {
				IDictionary<string,object> properties = DatabaseManager.Instance.GetDatabase().GetDocument(change.DocumentId).Properties;
				if((properties[Constants.DB_KEYWORD_TYPE].ToString() == Constants.DB_TYPE_ACCOUNT &&
				    properties[Constants.DB_COUCHBASE_ID].ToString() == PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString())) {
					UpdatePlayer();
					return;
				}
			}
		};
		
		DatabaseManager.Instance.GetDatabase().Changed += eventHandler;
	}
	
	void OnDestroy() {
		if (DatabaseManager.Instance != null) {
			DatabaseManager.Instance.GetDatabase().Changed -= eventHandler;
		}
	}

	public void Initialize(IDictionary<string, object> match) {
		matchCreateCanvas.gameObject.SetActive(true);
		foreach (Button b in listChickenButtons) {
			Destroy (b.gameObject);
		}
		listChickenButtons.Clear ();

		foreach (IDictionary<string, object> i in PlayerManager.Instance.playerChickens) {
			if((i[Constants.DB_KEYWORD_LIFE_STAGE].ToString() != Constants.LIFE_STAGE_STAG &&
			   i[Constants.DB_KEYWORD_LIFE_STAGE].ToString() != Constants.LIFE_STAGE_COCK) || 
			   bool.Parse(i[Constants.DB_KEYWORD_IS_QUEUED_FOR_MATCH].ToString())) {
				continue;
			}
			Button b = Instantiate(listChickenButton);
			listChickenButtons.Add (b);
			b.GetComponentInChildren<Text> ().text = i[Constants.DB_KEYWORD_NAME].ToString();
			b.GetComponent<MatchCreateScreenListButton>().MainScroll = scrollRect;
			b.transform.SetParent(listPanel.transform,false);
		}

		if(listChickenButtons.Count == 0) {
			MessageManager.Instance.DisplayMessage(Constants.MESSAGE_MATCH_CREATE_NO_COCKS_AVAILABLE_TITLE,
			                                       Constants.MESSAGE_MATCH_CREATE_NO_COCKS_AVAILABLE,
			                                       ButtonBack, false);
			return;
		}
		
		SetSelected(listChickenButtons[0].GetComponentInChildren<Text> ().text);
		UpdatePlayer();
		if(match != null) {
			selectedMatch = match;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_BUTTON).GetComponent<Toggle>().interactable = false;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_BUTTON).GetComponent<Toggle>().interactable = false;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_BUTTON).GetComponent<Toggle>().interactable = false;

			if(match[Constants.DB_KEYWORD_BETTING_OPTION].ToString() == Constants.BETTING_OPTION_SINGLE_BETTING) {
				IDictionary<string,object> initialBet = DatabaseManager.Instance.LoadBet(match[Constants.DB_COUCHBASE_ID].ToString(), match[Constants.DB_KEYWORD_PLAYER_ID_1].ToString());
				int initialBetAmount = int.Parse (initialBet[Constants.DB_KEYWORD_BET_AMOUNT].ToString());

				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_BUTTON).GetComponent<Toggle>().isOn = true;
				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_AMOUNT_SLIDER).GetComponent<Slider>().interactable = false;
				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_AMOUNT_SLIDER).GetComponent<Slider>().value = (int) initialBetAmount / 
					optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_AMOUNT_SLIDER).GetComponent<FightScreenChickenStatAutoAdjust>().multiplier;
			}
			else if(match[Constants.DB_KEYWORD_BETTING_OPTION].ToString() == Constants.BETTING_OPTION_SPECTATOR_BETTING) {
				IDictionary<string,object> initialBet = DatabaseManager.Instance.LoadBet(match[Constants.DB_COUCHBASE_ID].ToString(), match[Constants.DB_KEYWORD_PLAYER_ID_1].ToString());
				int initialBetAmount = int.Parse (initialBet[Constants.DB_KEYWORD_BET_AMOUNT].ToString());

				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_BUTTON).GetComponent<Toggle>().isOn = true;
				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_AMOUNT_SLIDER).GetComponent<Slider>().interactable = true;
				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_AMOUNT_SLIDER).GetComponent<Slider>().value = (int) (initialBetAmount * Constants.MINIMUM_BET_RATIO / 
					optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_AMOUNT_SLIDER).GetComponent<FightScreenChickenStatAutoAdjust>().multiplier);
				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_AMOUNT_SLIDER).GetComponent<Slider>().minValue = 
					(int) (optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_AMOUNT_SLIDER).GetComponent<Slider>().value);
				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_WAIT_DURATION_PANEL).gameObject.SetActive(false);
			}
			if(match[Constants.DB_KEYWORD_PASSWORD].ToString() != "") {				
				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_BUTTON).GetComponent<Toggle>().isOn = true;
				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_PASSWORD).GetComponent<InputField>().interactable = true;
				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_PASSWORD).GetComponent<InputField>().text = "";
			}
			else {
				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_BUTTON).GetComponent<Toggle>().isOn = false;
			}
		}
		else {
			selectedMatch = null;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_BUTTON).GetComponent<Toggle>().interactable = true;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_BUTTON).GetComponent<Toggle>().interactable = true;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_BUTTON).GetComponent<Toggle>().interactable = true;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_BUTTON).GetComponent<Toggle>().isOn = false;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_BUTTON).GetComponent<Toggle>().isOn = false;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_BUTTON).GetComponent<Toggle>().isOn = false;

			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_AMOUNT_SLIDER).GetComponent<Slider>().interactable = true;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_AMOUNT_SLIDER).GetComponent<Slider>().value = 0;

			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_AMOUNT_SLIDER).GetComponent<Slider>().interactable = true;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_AMOUNT_SLIDER).GetComponent<Slider>().minValue = 0;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_AMOUNT_SLIDER).GetComponent<Slider>().value = 0;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_WAIT_DURATION_PANEL).gameObject.SetActive(true);

			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_PASSWORD).GetComponent<InputField>().interactable = true;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_PASSWORD).GetComponent<InputField>().text = "";
		}
	}

	private void UpdatePlayer() {
		optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_AMOUNT_SLIDER).GetComponent<Slider>().maxValue = (int) int.Parse (PlayerManager.Instance.player[Constants.DB_KEYWORD_COIN].ToString()) / 100;
		optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_AMOUNT_SLIDER).GetComponent<Slider>().maxValue = (int) int.Parse (PlayerManager.Instance.player[Constants.DB_KEYWORD_COIN].ToString()) / 100;
	}

	public void SetSelected(string s) {
		okButton.interactable = true;
		foreach (IDictionary<string, object> i in PlayerManager.Instance.playerChickens) {
			if(i[Constants.DB_KEYWORD_NAME].ToString() == s) {
				selectedChicken = i;
				break;
			}
		}

		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_ATK_SLIDER).GetComponent<Slider>().maxValue = float.Parse(selectedChicken[Constants.DB_KEYWORD_ATTACK_MAX].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_DEF_SLIDER).GetComponent<Slider>().maxValue = float.Parse(selectedChicken[Constants.DB_KEYWORD_DEFENSE_MAX].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_HP_SLIDER).GetComponent<Slider>().maxValue = float.Parse(selectedChicken[Constants.DB_KEYWORD_HP_MAX].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_AGI_SLIDER).GetComponent<Slider>().maxValue = float.Parse(selectedChicken[Constants.DB_KEYWORD_AGILITY_MAX].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_GAM_SLIDER).GetComponent<Slider>().maxValue = float.Parse(selectedChicken[Constants.DB_KEYWORD_GAMENESS_MAX].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_AGG_SLIDER).GetComponent<Slider>().maxValue = float.Parse(selectedChicken[Constants.DB_KEYWORD_AGGRESSION_MAX].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_CON_SLIDER).GetComponent<Slider>().maxValue = Constants.CHICKEN_CONDITIONING_DEFAULT_MAX;

		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_ATK_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_ATTACK].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_DEF_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_DEFENSE].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_HP_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_HP].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_AGI_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_AGILITY].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_GAM_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_GAMENESS].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_AGG_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_AGGRESSION].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_CON_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_CONDITIONING].ToString());
	}

	private void ConfirmOk() {
		string chickenId = selectedChicken[Constants.DB_COUCHBASE_ID].ToString();
		string playerId = PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString();
		string categoryId = FightManager.Instance.GetSelectedCategory()[Constants.DB_COUCHBASE_ID].ToString();
		string bettingOption = Constants.BETTING_OPTION_NONE;
		int bettingAmount = 0;
		string password = "";
		System.DateTime dt = System.DateTime.MinValue;
		if(optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_BUTTON).GetComponent<Toggle>().isOn) {
			// save betting info
			bettingOption = Constants.BETTING_OPTION_SINGLE_BETTING;
			bettingAmount = (int)optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_AMOUNT_SLIDER).GetComponent<Slider>().value * 
				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_AMOUNT_SLIDER).GetComponent<FightScreenChickenStatAutoAdjust>().multiplier;
		}
		if(optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_BUTTON).GetComponent<Toggle>().isOn) {
			// save betting info
			bettingOption = Constants.BETTING_OPTION_SPECTATOR_BETTING;
			bettingAmount = (int)optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_AMOUNT_SLIDER).GetComponent<Slider>().value * 
				optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_AMOUNT_SLIDER).GetComponent<FightScreenChickenStatAutoAdjust>().multiplier;
			dt = Utility.TrimMilli(System.DateTime.Now.ToUniversalTime());
			dt = dt.AddMinutes (System.Convert.ToDouble(optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_WAIT_DURATION_SLIDER).GetComponent<Slider>().value * 
			                                            optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_WAIT_DURATION_SLIDER).GetComponent<FightScreenChickenStatAutoAdjust>().multiplier));
		}
		if(optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_BUTTON).GetComponent<Toggle>().isOn) {
			// save private info
			password = optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_PASSWORD).GetComponent<InputField>().text;
		}
		
		if(selectedMatch == null) {
			IDictionary<string,object> savedMatch = DatabaseManager.Instance.SaveEntry(
				GameManager.Instance.GenerateMatch(chickenId, "", playerId, "", categoryId, bettingOption, 
			                                   DatabaseManager.Instance.LoadBettingOdds(0)[Constants.DB_COUCHBASE_ID].ToString(), password, dt.ToString())
				);
			if(bettingOption != Constants.BETTING_OPTION_NONE) {
				DatabaseManager.Instance.SaveEntry(GameManager.Instance.GenerateBet(savedMatch[Constants.DB_COUCHBASE_ID].ToString(), playerId, 
				                                                                  DatabaseManager.Instance.LoadBettingOdds(0)[Constants.DB_COUCHBASE_ID].ToString(), chickenId, Constants.BETTED_CHICKEN_STATUS_LLAMADO,
				                                                                  bettingAmount));
				IDictionary<string,object> player = PlayerManager.Instance.player;
				player[Constants.DB_KEYWORD_COIN] = (int.Parse(player[Constants.DB_KEYWORD_COIN].ToString()) - bettingAmount);
				DatabaseManager.Instance.EditAccount(player);
			}
		}
		else {
			if(bettingOption != Constants.BETTING_OPTION_NONE) {
				DatabaseManager.Instance.SaveEntry(GameManager.Instance.GenerateBet(selectedMatch[Constants.DB_COUCHBASE_ID].ToString(), playerId, 
				                                                                  DatabaseManager.Instance.LoadBettingOdds(0)[Constants.DB_COUCHBASE_ID].ToString(), chickenId, Constants.DB_KEYWORD_DEHADO,
				                                                                  bettingAmount));
				IDictionary<string,object> player = PlayerManager.Instance.player;
				player[Constants.DB_KEYWORD_COIN] = (int.Parse(player[Constants.DB_KEYWORD_COIN].ToString()) - bettingAmount);
				DatabaseManager.Instance.EditAccount(player);
			}
			selectedMatch[Constants.DB_KEYWORD_CHICKEN_ID_2] = chickenId;
			selectedMatch[Constants.DB_KEYWORD_PLAYER_ID_2] = playerId;
			selectedMatch[Constants.DB_KEYWORD_STATUS] = Constants.MATCH_STATUS_OPPONENT_FOUND;
			DatabaseManager.Instance.EditMatch(selectedMatch);
			MatchViewManager.Instance.Deinitialize();
		}

		matchCreateCanvas.gameObject.SetActive (false);
		FightManager.Instance.InitializeMatches();
	}

	public void ButtonOk() {
		if(selectedMatch != null &&
		   optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_PASSWORD).GetComponent<InputField>().text != selectedMatch[Constants.DB_KEYWORD_PASSWORD].ToString()) {
			MessageManager.Instance.DisplayMessage(Constants.MESSAGE_MATCH_CREATE_INCORRECT_PASSWORD_TITLE,
			                                       Constants.MESSAGE_MATCH_CREATE_INCORRECT_PASSWORD,
			                                       null, false);
		}
		else {
			MessageManager.Instance.DisplayMessage(Constants.MESSAGE_MATCH_CREATE_CONFIRM_TITLE,
			                                       Constants.MESSAGE_MATCH_CREATE_CONFIRM,
			                                       ConfirmOk, true);
		}
	}

	public void ButtonBack() {
		matchCreateCanvas.gameObject.SetActive (false);
	}

	public void ButtonBetting(Toggle t) {
		optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_PANEL).gameObject.SetActive(t.isOn);
		if(t.isOn) {
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_BUTTON).GetComponent<Toggle>().isOn = false;
		}
	}

	public void ButtonBetting2(Toggle t) {
		optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_PANEL).gameObject.SetActive(t.isOn);
		if(t.isOn) {
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_BUTTON).GetComponent<Toggle>().isOn = false;
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_BUTTON).GetComponent<Toggle>().isOn = false;
		}
	}

	public void ButtonPrivate(Toggle t) {
		optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_PANEL).gameObject.SetActive(t.isOn);
		if(t.isOn) {
			optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_BUTTON).GetComponent<Toggle>().isOn = false;
		}
	}
}