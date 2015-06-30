using UnityEngine;
using System.Collections;

public class LoadBuildingsScreenLoadButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject loadBuildingPanel;

	public void ButtonPressed() {
		LoadBuildingsScreenManager.Instance.LoadBuilding ();

		mainPanel.SetActive (true);
		loadBuildingPanel.SetActive (false);
	}

}
