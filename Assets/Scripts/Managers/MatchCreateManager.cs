using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MatchCreateManager : MonoBehaviour {

	public Canvas matchCreateCanvas;

	public GameObject listPanel;
	public GameObject statsPanel;
	public GameObject optionsPanel;

	private string state = Constants.FIGHT_MANAGER_STATE_CATEGORY_SELECT;

	public Button listChickenButton;
	private List<Button> listChickenButtons = new List<Button> ();
	private IDictionary<string,object> selectedChicken;

	public Button okButton;
	public Button cancelButton;

	private List<IEnumerator> countdowns = new List<IEnumerator>();

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

	public void Initialize() {
		foreach (Button b in listChickenButtons) {
			Destroy (b.gameObject);
		}
		foreach (IEnumerator ie in countdowns) {
			StopCoroutine(ie);
		}
		listChickenButtons.Clear ();
		countdowns.Clear ();
		
		foreach (IDictionary<string, object> i in PlayerManager.Instance.playerChickens) {
			if((i[Constants.DB_KEYWORD_LIFE_STAGE].ToString() != Constants.LIFE_STAGE_STAG &&
			   i[Constants.DB_KEYWORD_LIFE_STAGE].ToString() != Constants.LIFE_STAGE_COCK) || 
			   i[Constants.DB_KEYWORD_IS_QUEUED_FOR_MATCH].Equals(true)) {
				continue;
			}
			Button b = Instantiate(listChickenButton);
			listChickenButtons.Add (b);
			b.GetComponentInChildren<Text> ().text = i[Constants.DB_KEYWORD_NAME].ToString();
			b.transform.SetParent(listPanel.transform,false);
		}
		if(listChickenButtons.Count > 0) {
			SetSelected(listChickenButtons[0].GetComponentInChildren<Text> ().text);
		}
		else {
			MessageManager.Instance.DisplayMessage(Constants.MESSAGE_MATCH_CREATE_NO_COCKS_AVAILABLE_TITLE,
			                                       Constants.MESSAGE_MATCH_CREATE_NO_COCKS_AVAILABLE,
			                                       ButtonBack, false);
		} 
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

		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_ATK_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_ATTACK].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_DEF_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_DEFENSE].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_HP_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_HP].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_AGI_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_AGILITY].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_GAM_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_GAMENESS].ToString());
		statsPanel.transform.FindChild(Constants.CREATE_MATCH_STAT_AGG_SLIDER).GetComponent<Slider>().value = float.Parse(selectedChicken[Constants.DB_KEYWORD_AGGRESSION].ToString());
	}

	public void ButtonOk() {
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
			bettingAmount = (int)optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_AMOUNT_SLIDER).GetComponent<Slider>().value;
		}
		if(optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_BUTTON).GetComponent<Toggle>().isOn) {
			// save betting info
			bettingOption = Constants.BETTING_OPTION_SPECTATOR_BETTING;
			bettingAmount = (int)optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_AMOUNT_SLIDER).GetComponent<Slider>().value;
			dt = TrimMilli(System.DateTime.Now.ToUniversalTime());
			dt = dt.AddMinutes (System.Convert.ToDouble(optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_2_WAIT_DURATION_SLIDER).GetComponent<Slider>().value));
		}
		if(optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_BUTTON).GetComponent<Toggle>().isOn) {
			// save private info
			password = optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_PASSWORD).GetComponent<InputField>().text;
		}

		IDictionary<string,object> savedMatch = DatabaseManager.Instance.SaveMatch(GameManager.Instance.GenerateMatch(chickenId, "", playerId, "", categoryId, bettingOption, password, dt.ToString()));
		foreach(KeyValuePair<string,object> kv in savedMatch) {
			print (kv.Key + ": " + kv.Value);
		}
		if(bettingOption != Constants.BETTING_OPTION_NONE) {
			IDictionary<string,object> savedBet = DatabaseManager.Instance.SaveBet(GameManager.Instance.GenerateBet(savedMatch[Constants.DB_COUCHBASE_ID].ToString(), playerId, 
			                                 DatabaseManager.Instance.LoadBettingOdds(0)[Constants.DB_COUCHBASE_ID].ToString(), chickenId, Constants.BETTED_CHICKEN_STATUS_LLAMADO,
			                                 bettingAmount));
			foreach(KeyValuePair<string,object> kv in savedBet) {
				print (kv.Key + ": " + kv.Value);
			}
		}

		matchCreateCanvas.gameObject.SetActive (false);
		FightManager.Instance.InitializeMatches();
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

	private System.DateTime TrimMilli(System.DateTime dt)
	{
		return new System.DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, dt.Kind);
	}
}