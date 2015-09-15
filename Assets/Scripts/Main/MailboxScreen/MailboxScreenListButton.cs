using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MailboxScreenListButton : MonoBehaviour {

	void Start() {
		EventTrigger trigger = GetComponentInParent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			MailboxManager.Instance.SetSelected (this.name);
		});
		trigger.triggers.Add (entry);
	}
}
