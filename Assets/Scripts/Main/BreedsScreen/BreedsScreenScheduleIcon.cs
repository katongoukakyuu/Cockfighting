using UnityEngine;
using System.Collections;

public class BreedsScreenScheduleIcon : MonoBehaviour {

	public void ButtonPressed() {
		BreedsManager.Instance.SwitchToAssignMateMode ();
	}
}
