using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BuildScreenBuildBldgButton : MonoBehaviour {

	public Canvas mainCanvas;
	public GameObject mainCanvasLeft;
	public GameObject mainCanvasRight;
	public Canvas buildStructuresCanvas;
	public Canvas buildingPlacementCanvas;
	public BuildScreenImagePanel imagePanel;
	public BuildingPlacementManager buildingPlacementManager;

	private IDictionary<string,object> building;

	public void ButtonPressed() {
		CameraControls.Instance.freeCamera = false;
		CameraControls.Instance.SwitchToCamera(1);

		building = imagePanel.GetSelectedBuilding ();
		buildingPlacementManager.Initialize (building);
		mainCanvas.gameObject.SetActive (true);
		mainCanvasLeft.gameObject.SetActive (false);
		mainCanvasRight.gameObject.SetActive (false);
		buildStructuresCanvas.gameObject.SetActive (false);
		buildingPlacementCanvas.gameObject.SetActive (true);
	}

}
