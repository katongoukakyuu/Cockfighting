using UnityEngine;
using System.Collections;

public class ControlPanelFeedsScreenAboutButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject mainAboutPanel;

	public void ButtonPressed() {
		mainPanel.SetActive (false);
		mainAboutPanel.SetActive (true);
	}
}
