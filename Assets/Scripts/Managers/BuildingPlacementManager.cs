using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(TileMap))]
[RequireComponent (typeof(TileMapMouse))]
public class BuildingPlacementManager : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas buildingPlacementCanvas;

	private IDictionary<string,object> building;
	private GameObject bldgObject;

	private static BuildingPlacementManager instance;
	private BuildingPlacementManager() {}
	
	public static BuildingPlacementManager Instance {
		get {
			if(instance == null) {
				instance = (BuildingPlacementManager)GameObject.FindObjectOfType(typeof(BuildingPlacementManager));
			}
			return instance;
		}
	}

	public void Initialize(IDictionary<string,object> building) {
		this.building = building;
		TileMapMouse.Instance.enabled = true;
		bldgObject = Instantiate (Resources.Load ("Prefabs/"+building[Constants.DB_KEYWORD_PREFAB_NAME],typeof(GameObject))) as GameObject;
	}

	void Update() {
		if(TileMapMouse.Instance.enabled)
			bldgObject.transform.position = TileMapMouse.Instance.position;
	}

	public void ButtonCancel() {
		Destroy (bldgObject);
		mainCanvas.gameObject.SetActive (true);
		buildingPlacementCanvas.gameObject.SetActive (false);
		TileMapMouse.Instance.enabled = false;
	}

	public void ButtonRotateCCW() {
		bldgObject.transform.Rotate (new Vector3(0.0f,-90.0f,0.0f));
		if (bldgObject.transform.eulerAngles.y <= -360.0f)
			bldgObject.transform.eulerAngles = new Vector3(bldgObject.transform.eulerAngles.x,
			                                               0,
			                                               bldgObject.transform.eulerAngles.z);
	}

	public void ButtonRotateCW() {
		bldgObject.transform.Rotate (new Vector3(0.0f,90.0f,0.0f));
		if (bldgObject.transform.eulerAngles.y >= 360.0f)
			bldgObject.transform.eulerAngles = new Vector3(bldgObject.transform.eulerAngles.x,
			                                               0,
			                                               bldgObject.transform.eulerAngles.z);
	}
}