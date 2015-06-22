using UnityEngine;
using System.Collections;

[RequireComponent (typeof(TileMap))]
[RequireComponent (typeof(TileMapMouse))]
public class BuildingPlacementManager : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas buildingPlacementCanvas;

	private GameObject[,] tiles;
	private TileMapMouse tileMapMouse;
	private Building building;
	private GameObject bldgObject;

	void Awake() {
		tiles = GetComponent<TileMap> ().tiles;
		tileMapMouse = GetComponent<TileMapMouse> ();
	}

	public void Initialize(Building building) {
		this.building = building;
		tileMapMouse.enabled = true;
		bldgObject = Instantiate (Resources.Load ("Prefabs/"+building.prefabName,typeof(GameObject))) as GameObject;
	}

	void Update() {
		if(tileMapMouse.enabled)
			bldgObject.transform.position = tileMapMouse.position;
	}

	public void ButtonCancel() {
		Destroy (bldgObject);
		mainCanvas.gameObject.SetActive (true);
		buildingPlacementCanvas.gameObject.SetActive (false);
		tileMapMouse.enabled = false;
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