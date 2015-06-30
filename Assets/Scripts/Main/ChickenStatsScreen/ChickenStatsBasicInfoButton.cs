using UnityEngine;
using System.Collections;

public class ChickenStatsBasicInfoButton : MonoBehaviour {

	public GameObject basicInfoPanel;

	public void ButtonPressed() {
		basicInfoPanel.SetActive (!basicInfoPanel.activeSelf);
	}
}
