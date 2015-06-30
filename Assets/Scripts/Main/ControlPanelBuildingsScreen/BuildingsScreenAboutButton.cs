using UnityEngine;
using System.Collections;

public class BuildingsScreenAboutButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject mainAboutPanel;

	public void ButtonPressed() {
		mainPanel.SetActive (false);
		mainAboutPanel.SetActive (true);
	}
}
