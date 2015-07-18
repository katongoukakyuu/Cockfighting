﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BuildScreenBuildBldgButton : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas buildStructuresCanvas;
	public Canvas buildingPlacementCanvas;
	public BuildScreenImagePanel imagePanel;
	public BuildingPlacementManager buildingPlacementManager;

	private IDictionary<string,object> building;

	public void ButtonPressed() {
		building = imagePanel.GetSelectedBuilding ();
		buildingPlacementManager.Initialize (building);
		mainCanvas.gameObject.SetActive (true);
		buildStructuresCanvas.gameObject.SetActive (false);
		buildingPlacementCanvas.gameObject.SetActive (true);
	}

}