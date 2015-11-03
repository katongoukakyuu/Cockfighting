using UnityEngine;
using System.Collections;

public class FeedsScreenScheduleHurryButton : MonoBehaviour {

	public int index = -1;

	public void ButtonPressed() {
		FeedsManager.Instance.HurrySchedule(index);
	}
}
