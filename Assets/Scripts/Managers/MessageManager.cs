using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MessageManager : MonoBehaviour {

	public Canvas messageCanvas;

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
		GameObject.Find(Constants.MESSAGE_PANEL_TITLE).GetComponent<Text>().text = title;
		GameObject.Find(Constants.MESSAGE_PANEL_MESSAGE).GetComponent<Text>().text = message;

		GameObject okButton = GameObject.Find(Constants.MESSAGE_PANEL_OK_BUTTON).gameObject;
		EventTrigger trigger = okButton.GetComponentInParent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			bd ();
			ClearMessage(allowCancel);
		});
		trigger.triggers.Add (entry);

		GameObject cancelButton = GameObject.Find(Constants.MESSAGE_PANEL_CANCEL_BUTTON).gameObject;
		if(allowCancel) {
			cancelButton.SetActive(true);
			trigger = cancelButton.GetComponentInParent<EventTrigger> ();
			entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.Select;
			entry.callback.AddListener ((eventData) => {
				ClearMessage(allowCancel);
			});
			trigger.triggers.Add (entry);
		}
		else {
			cancelButton.SetActive(false);
		}
	}

	public void ClearMessage(bool allowCancel) {
		GameObject okButton = GameObject.Find(Constants.MESSAGE_PANEL_OK_BUTTON).gameObject;
		EventTrigger trigger = okButton.GetComponentInParent<EventTrigger> ();
		GameObject cancelButton;
		EventTrigger trigger2;


		GameObject.Find(Constants.MESSAGE_PANEL_TITLE).GetComponent<Text>().text = "";
		GameObject.Find(Constants.MESSAGE_PANEL_MESSAGE).GetComponent<Text>().text = "";
		trigger.triggers.Clear();

		if(allowCancel) {
			cancelButton = GameObject.Find(Constants.MESSAGE_PANEL_CANCEL_BUTTON).gameObject;
			trigger2 = cancelButton.GetComponentInParent<EventTrigger> ();
			trigger2.triggers.Clear();
		}

		messageCanvas.gameObject.SetActive(false);
	}
}