using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MatchBetManager : MonoBehaviour {

	public Canvas matchBetCanvas;

	public GameObject mainPanel;

	public Button okButton;
	public Button cancelButton;

	private IDictionary<string,object> selectedMatch;
	private IDictionary<string,object> selectedBet;
	private bool isLlamado = true;
	private int multiplier = 1;

	private static MatchBetManager instance;
	private MatchBetManager() {}
	
	public static MatchBetManager Instance {
		get {
			if(instance == null) {
				instance = (MatchBetManager)GameObject.FindObjectOfType(typeof(MatchBetManager));
			}
			return instance;
		}
	}

	public void Initialize(IDictionary<string, object> match, IDictionary<string, object> chicken) {
		matchBetCanvas.gameObject.SetActive(true);
		selectedMatch = match;
		selectedBet = DatabaseManager.Instance.LoadBettingOdds(selectedMatch[Constants.DB_KEYWORD_BETTING_ODDS_ID].ToString());
		if(selectedMatch[Constants.DB_KEYWORD_LLAMADO].ToString() == chicken[Constants.DB_COUCHBASE_ID].ToString()) {
			isLlamado = true;
		}
		else {
			isLlamado = false;
		}
		multiplier = mainPanel.transform.FindChild(Constants.BET_MATCH_BET_SLIDER).GetComponent<FightScreenChickenStatAutoAdjust>().multiplier;
		mainPanel.transform.FindChild(Constants.BET_MATCH_BET_SLIDER).GetComponent<Slider>().maxValue = 
			(int) int.Parse (PlayerManager.Instance.player[Constants.DB_KEYWORD_COIN].ToString()) / multiplier;
		mainPanel.transform.FindChild(Constants.BET_MATCH_BET_SLIDER).GetComponent<Slider>().value = 1;
	}

	public void Deinitialize() {
		matchBetCanvas.gameObject.SetActive(false);
	}

	private void ConfirmOk() {
		MatchViewManager.Instance.CreateBet((int)mainPanel.transform.FindChild(Constants.BET_MATCH_BET_SLIDER).GetComponent<Slider>().value * multiplier);
	}

	public void ButtonOk() {
		MessageManager.Instance.DisplayMessage(Constants.MESSAGE_MATCH_BET_CONFIRM_TITLE, 
		                                       Constants.MESSAGE_MATCH_BET_CONFIRM,
		                                       ConfirmOk, true);
	}

	public void ButtonCancel() {
		matchBetCanvas.gameObject.SetActive (false);
	}

	public void UpdateMessage() {
		string s = Constants.MESSAGE_MATCH_BET_MESSAGE_1;
		int prize = 0;
		s += selectedBet[Constants.DB_KEYWORD_LLAMADO_ODDS].ToString() + " : " + selectedBet[Constants.DB_KEYWORD_DEHADO_ODDS].ToString();
		if(isLlamado) {
			s += Constants.MESSAGE_MATCH_BET_MESSAGE_2A;
			prize = (int) Utility.GetPayout((int) mainPanel.transform.FindChild(Constants.BET_MATCH_BET_SLIDER).GetComponent<Slider>().value * multiplier,
			                          selectedBet, true) / 2;
		}
		else {
			s += Constants.MESSAGE_MATCH_BET_MESSAGE_2B;
			prize = (int) Utility.GetPayout((int) mainPanel.transform.FindChild(Constants.BET_MATCH_BET_SLIDER).GetComponent<Slider>().value * multiplier,
			                          selectedBet, false) / 2;
		}
		s += prize;
		s += Constants.MESSAGE_MATCH_BET_MESSAGE_3;
		mainPanel.transform.FindChild(Constants.BET_MATCH_MESSAGE).GetComponent<Text>().text = s;

	}

}