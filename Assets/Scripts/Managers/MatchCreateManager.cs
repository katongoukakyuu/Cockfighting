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
	}

	public void SetSelected(string s) {
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
		print ("selected chicken name: " + selectedChicken[Constants.DB_KEYWORD_NAME].ToString());
		print ("wait duration: " + optionsPanel.transform.FindChild(Constants.CREATE_MATCH_WAIT_DURATION_SLIDER).GetComponent<Slider>().value + " minutes");
		if(optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_BUTTON).GetComponent<Toggle>().isOn) {
			// save betting info
			print ("betting amount: " + optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_AMOUNT_SLIDER).GetComponent<Slider>().value);
			print ("betting odds: " + optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_ODDS_SLIDER).GetComponent<Slider>().value);
		}
		if(optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_BUTTON).GetComponent<Toggle>().isOn) {
			// save private info
			print ("password: " + optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_PASSWORD).GetComponent<InputField>().text);
		}

		//matchCreateCanvas.gameObject.SetActive (false);
	}

	public void ButtonBack() {
		matchCreateCanvas.gameObject.SetActive (false);
	}

	public void ButtonBetting(Toggle t) {
		optionsPanel.transform.FindChild(Constants.CREATE_MATCH_BETTING_PANEL).gameObject.SetActive(t.isOn);
	}

	public void ButtonPrivate(Toggle t) {
		optionsPanel.transform.FindChild(Constants.CREATE_MATCH_PRIVATE_PANEL).gameObject.SetActive(t.isOn);
	}
}