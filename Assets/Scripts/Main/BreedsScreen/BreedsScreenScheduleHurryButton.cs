using UnityEngine;
using System.Collections;

public class BreedsScreenScheduleHurryButton : MonoBehaviour {

	public int index = -1;

	public void ButtonPressed() {
		BreedsManager.Instance.HurrySchedule(index);
	}
}
