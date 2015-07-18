using UnityEngine;
using System.Collections;

public class FeedsScreenAddSchedule : MonoBehaviour {
	
	public GameObject scheduleListItem;

	public void ButtonPressed() {
		GameObject g = Instantiate(scheduleListItem);
		FeedsManager.Instance.AddScheduleToList (g, null);
	}
}
