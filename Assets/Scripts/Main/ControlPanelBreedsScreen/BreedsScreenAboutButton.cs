using UnityEngine;
using System.Collections;

public class BreedsScreenAboutButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject mainAboutPanel;

	public void ButtonPressed() {
		mainPanel.SetActive (false);
		mainAboutPanel.SetActive (true);
	}
}
