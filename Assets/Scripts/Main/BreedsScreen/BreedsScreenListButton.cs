﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class BreedsScreenListButton : MonoBehaviour {

	void Start() {
		EventTrigger trigger = GetComponentInParent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			BreedsManager.Instance.SetSelected (this.GetComponentInChildren<Text>().text);
		});
		trigger.triggers.Add (entry);
	}
}
