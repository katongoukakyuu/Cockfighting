using UnityEngine;
using System.Collections;

public class FeedsScreenScheduleCancelButton : MonoBehaviour {

	public int index = -1;

	public void ButtonPressed() {
		FeedsManager.Instance.CancelSchedule(index);
	}
}
