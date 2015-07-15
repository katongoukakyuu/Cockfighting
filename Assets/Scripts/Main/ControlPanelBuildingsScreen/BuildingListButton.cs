using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class BuildingListButton : MonoBehaviour {

	void Start() {
		EventTrigger trigger = GetComponentInParent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			LoadBuildingsScreenManager.Instance.SetSelected (this.GetComponentInChildren<Text>().text);
		});
		trigger.triggers.Add (entry);
	}
}
