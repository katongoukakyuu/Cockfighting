using UnityEngine;
using System.Collections;

public class BreedsScreenAddSchedule : MonoBehaviour {
	
	public GameObject scheduleListItem;

	public void ButtonPressed() {
		GameObject g = Instantiate(scheduleListItem);
		BreedsManager.Instance.AddScheduleToList (g, null);
	}
}
