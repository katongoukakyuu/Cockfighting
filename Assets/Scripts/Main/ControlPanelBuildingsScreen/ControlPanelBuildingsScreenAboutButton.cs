using UnityEngine;
using System.Collections;

public class ControlPanelBuildingsScreenAboutButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject mainAboutPanel;

	public void ButtonPressed() {
		mainPanel.SetActive (false);
		mainAboutPanel.SetActive (true);
	}
}
