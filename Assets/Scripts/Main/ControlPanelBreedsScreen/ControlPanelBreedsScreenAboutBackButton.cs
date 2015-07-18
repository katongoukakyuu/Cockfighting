using UnityEngine;
using System.Collections;

public class ControlPanelBreedsScreenAboutBackButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject mainAboutPanel;

	public void ButtonPressed() {
		mainPanel.SetActive (true);
		mainAboutPanel.SetActive (false);
	}
}
