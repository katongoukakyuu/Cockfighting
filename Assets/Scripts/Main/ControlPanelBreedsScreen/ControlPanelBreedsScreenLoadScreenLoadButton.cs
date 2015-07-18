using UnityEngine;
using System.Collections;

public class ControlPanelBreedsScreenLoadScreenLoadButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject loadBreedPanel;

	public void ButtonPressed() {
		LoadBreedsScreenManager.Instance.LoadBreed ();

		mainPanel.SetActive (true);
		loadBreedPanel.SetActive (false);
	}

}
