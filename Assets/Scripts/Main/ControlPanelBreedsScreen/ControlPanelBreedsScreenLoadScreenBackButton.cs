﻿using UnityEngine;
using System.Collections;

public class ControlPanelBreedsScreenLoadScreenBackButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject loadBreedPanel;

	public void ButtonPressed() {
		mainPanel.SetActive (true);
		loadBreedPanel.SetActive (false);
	}

}