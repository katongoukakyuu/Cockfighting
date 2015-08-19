using UnityEngine;
using System.Collections;

public class BreedsScreenScheduleCancelButton : MonoBehaviour {

	public int index = -1;

	public void ButtonPressed() {
		FeedsManager.Instance.CancelSchedule(index);
	}
}
