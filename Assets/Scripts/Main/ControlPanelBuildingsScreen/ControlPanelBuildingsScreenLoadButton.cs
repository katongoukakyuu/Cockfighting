using UnityEngine;
using System.Collections;

public class ControlPanelBuildingsScreenLoadButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject loadBuildingPanel;

	public void ButtonPressed() {
		ControlPanelBuildingsManager.Instance.LoadBuilding ();
		LoadBuildingsScreenManager.Instance.LoadBuildingsList (ControlPanelBuildingsManager.Instance.GetBuildingList ());

		mainPanel.SetActive (false);
		loadBuildingPanel.SetActive (true);
	}

}
