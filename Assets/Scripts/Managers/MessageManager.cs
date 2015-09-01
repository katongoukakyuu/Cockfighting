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

	public void DisplayMessage(string title, string message, ButtonDelegate bd) {
		messageCanvas.gameObject.SetActive(true);
		GameObject.Find("Title Text").GetComponent<Text>().text = title;
		GameObject.Find("Message Text").GetComponent<Text>().text = message;

		GameObject okButton = GameObject.Find("Msg OK Button").gameObject;
		EventTrigger trigger = okButton.GetComponentInParent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			bd ();
			ClearMessage();
		});
		trigger.triggers.Add (entry);

		GameObject cancelButton = GameObject.Find("Msg Cancel Button").gameObject;
		trigger = cancelButton.GetComponentInParent<EventTrigger> ();
		entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			ClearMessage();
		});
		trigger.triggers.Add (entry);
	}

	public void ClearMessage() {
		GameObject okButton = GameObject.Find("Msg OK Button").gameObject;
		EventTrigger trigger = okButton.GetComponentInParent<EventTrigger> ();
		GameObject cancelButton = GameObject.Find("Msg Cancel Button").gameObject;
		EventTrigger trigger2 = cancelButton.GetComponentInParent<EventTrigger> ();

		GameObject.Find("Title Text").GetComponent<Text>().text = "";
		GameObject.Find("Message Text").GetComponent<Text>().text = "";
		trigger.triggers.Clear();
		trigger2.triggers.Clear();
		messageCanvas.gameObject.SetActive(false);
	}
}