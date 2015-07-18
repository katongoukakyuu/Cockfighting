using UnityEngine;
using System.Collections;

public class ControlPanelFeedsScreenLoadScreenBackButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject loadFeedsPanel;

	public void ButtonPressed() {
		mainPanel.SetActive (true);
		loadFeedsPanel.SetActive (false);
	}

}
