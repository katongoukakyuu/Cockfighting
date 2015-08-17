using UnityEngine;
using System.Collections;

public class FeedsScreenScheduleIcon : MonoBehaviour {

	public void ButtonPressed() {
		FeedsManager.Instance.SwitchToAssignItemMode (Constants.DB_TYPE_FEEDS);
	}
}
