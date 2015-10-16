using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MessageManager : MonoBehaviour {

	public Canvas messageCanvas;
	public Canvas messageCanvasFinal;
	public GameObject[] panels;

	public Button okButton;
	public Button cancelButton;

	public Button okButtonFinal;
	public Button cancelButtonFinal;

	public delegate void ButtonDelegate();

	private static MessageManager instance;
	private MessageManager() {}
	
	public static MessageManager Instance {
		get {
			if(instance == null) {
				instance = (MessageManager)GameObject.FindObjectOfType(typeof(MessageManager));
			}
			return instance;
		}
	}

	public void DisplayMessage(string title, string message, ButtonDelegate bd, bool allowCancel) {
		messageCanvas.gameObject.SetActive(true);
		messageCanvas.transform.FindChild(Constants.MESSAGE_PANEL_TITLE).GetComponent<Text>().text = title;
		messageCanvas.transform.FindChild(Constants.MESSAGE_PANEL_MESSAGE).GetComponent<Text>().text = message;
		
		EventTrigger trigger = okButton.GetComponentInParent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			ClearMessage(allowCancel);
			if(bd != null) bd ();
		});
		trigger.triggers.Add (entry);
		
		if(allowCancel) {
			cancelButton.gameObject.SetActive(true);
			trigger = cancelButton.GetComponentInParent<EventTrigger> ();
			entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.Select;
			entry.callback.AddListener ((eventData) => {
				ClearMessage(allowCancel);
			});
			trigger.triggers.Add (entry);
		}
		else {
			cancelButton.gameObject.SetActive(false);
		}
	}

	public void ClearMessage(bool allowCancel) {
		EventTrigger trigger = okButton.GetComponentInParent<EventTrigger> ();
		EventTrigger trigger2;
		
		messageCanvas.transform.FindChild(Constants.MESSAGE_PANEL_TITLE).GetComponent<Text>().text = "";
		messageCanvas.transform.FindChild(Constants.MESSAGE_PANEL_MESSAGE).GetComponent<Text>().text = "";
		trigger.triggers.Clear();

		if(allowCancel) {
			trigger2 = cancelButton.GetComponentInParent<EventTrigger> ();
			trigger2.triggers.Clear();
		}
		cancelButton.gameObject.SetActive(true);
		messageCanvas.gameObject.SetActive(false);
	}

	public void ClearMessageFinal() {
		EventTrigger trigger = okButtonFinal.GetComponentInParent<EventTrigger> ();
		EventTrigger trigger2 = cancelButtonFinal.GetComponentInParent<EventTrigger> ();

		trigger.triggers.Clear();
		trigger2.triggers.Clear();
		
		messageCanvasFinal.gameObject.SetActive(false);
	}

	public void OpenStoreDialog(IDictionary<string,object> item, string keyword, ButtonDelegate bd) {
		messageCanvasFinal.gameObject.SetActive(true);
		messageCanvasFinal.transform.FindChild(Constants.MESSAGE_PANEL_FINAL_TITLE).GetComponent<Text>().text = "Store";

		for(int x = 0; x < panels.Length; x++) {
			panels[x].SetActive(false);
		}

		GameObject storePanel = GameObject.Find(Constants.STORE_QUANTITY_PANEL);
		storePanel.SetActive(true);

		storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_PLAYER_COIN).GetComponent<Text>().text = PlayerManager.Instance.player[Constants.DB_KEYWORD_COIN].ToString();
		storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_PLAYER_CASH).GetComponent<Text>().text = PlayerManager.Instance.player[Constants.DB_KEYWORD_CASH].ToString();
		storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_ICON).GetComponent<Image>().sprite = 
			Resources.Load<Sprite> (Constants.PATH_SPRITES+item[Constants.DB_KEYWORD_IMAGE_NAME].ToString());
		storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_NAME).GetComponent<Text>().text = item[Constants.DB_KEYWORD_NAME].ToString();
		storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_CATEGORY).GetComponent<Text>().text = Utility.ToProperCase(item[Constants.DB_KEYWORD_SUBTYPE].ToString());
		if(keyword == Constants.DB_KEYWORD_COIN) {
			storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_COIN_PRICE).gameObject.SetActive(true);
			storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_COIN_IMAGE).gameObject.SetActive(true);
			storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_CASH_PRICE).gameObject.SetActive(false);
			storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_CASH_IMAGE).gameObject.SetActive(false);
			storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_COIN_PRICE).GetComponent<Text>().text = item[Constants.DB_KEYWORD_COIN_COST].ToString();
		} else if(keyword == Constants.DB_KEYWORD_CASH) {
			storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_COIN_PRICE).gameObject.SetActive(false);
			storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_COIN_IMAGE).gameObject.SetActive(false);
			storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_CASH_PRICE).gameObject.SetActive(true);
			storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_CASH_IMAGE).gameObject.SetActive(true);
			storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_CASH_PRICE).GetComponent<Text>().text = item[Constants.DB_KEYWORD_CASH_COST].ToString();
		}
		storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_DESCRIPTION).GetComponent<Text>().text = item[Constants.DB_KEYWORD_DESCRIPTION].ToString();
		storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_SLIDER).GetComponent<Slider>().value = 1;
		storePanel.transform.FindChild(Constants.STORE_QUANTITY_PANEL_SLIDER).GetComponent<StoreScreenQuantitySlider>().Initialize(item);

		EventTrigger trigger = okButtonFinal.GetComponentInParent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			if(bd != null) bd ();
		});
		trigger.triggers.Add (entry);
		
		trigger = cancelButtonFinal.GetComponentInParent<EventTrigger> ();
		entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			ClearMessageFinal();
		});
		trigger.triggers.Add (entry);
	}
}