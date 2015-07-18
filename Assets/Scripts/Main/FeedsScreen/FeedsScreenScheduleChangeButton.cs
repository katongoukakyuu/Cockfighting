using UnityEngine;
using System.Collections;

public class FeedsScreenScheduleChangeButton : MonoBehaviour {

	public void ButtonPressed() {
		FeedsManager.Instance.SwitchToAssignItemMode ();
	}
}
