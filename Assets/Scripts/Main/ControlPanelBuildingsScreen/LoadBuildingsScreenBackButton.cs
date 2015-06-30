using UnityEngine;
using System.Collections;

public class LoadBuildingsScreenBackButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject loadBuildingPanel;

	public void ButtonPressed() {
		mainPanel.SetActive (true);
		loadBuildingPanel.SetActive (false);
	}

}
