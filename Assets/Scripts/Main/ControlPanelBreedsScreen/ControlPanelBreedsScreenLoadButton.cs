using UnityEngine;
using System.Collections;

public class ControlPanelBreedsScreenLoadButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject loadBreedPanel;

	public void ButtonPressed() {
		ControlPanelBreedsManager.Instance.LoadBreed ();
		LoadBreedsScreenManager.Instance.LoadBreedsList (ControlPanelBreedsManager.Instance.GetBreedList ());

		mainPanel.SetActive (false);
		loadBreedPanel.SetActive (true);
	}

}
