using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingPlacementManager : MonoBehaviour {

	public Canvas mainCanvas;
	public GameObject mainCanvasLeft;
	public GameObject mainCanvasRight;
	public Canvas buildingPlacementCanvas;
	public Animator BuildPlacementAnimator;

	private GridOverlay gridOverlay;

	private bool isInitialized = false;
	private IDictionary<string,object> building;
	private GameObject bldgObject;
	private string orientation = Constants.ORIENTATION_NORTH;

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

	void Start() {
		gridOverlay = Camera.main.GetComponent<GridOverlay>();

		MouseHandler.Instance.OnMouseClick += OnClick;
		gridOverlay.OnTileHoverChange += OnTileHoverChange;
	}
	
	private void OnClick(GameObject g) {
		if (isInitialized && FarmManager.Instance.State() == Constants.FARM_MANAGER_STATE_BUILD_STRUCTURE) {
			int[] pos = new int[] {
				(int)gridOverlay.GetSelectedTile().position.x,
				(int)gridOverlay.GetSelectedTile().position.y
			};
			if(FarmManager.Instance.CheckBuildable(building, pos, orientation)) {
				FarmManager.Instance.BuildStructure(building, pos, orientation);
				ButtonCancel();
				DatabaseManager.Instance.UpdatePlayer(PlayerManager.Instance.player[Constants.DB_KEYWORD_USERNAME].ToString (),
				                                      PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString());
				FarmManager.Instance.UpdateBuildingsOwned();
			}
		}
	}

	private void OnTileHoverChange() {
		if (isInitialized && FarmManager.Instance.State () == Constants.FARM_MANAGER_STATE_BUILD_STRUCTURE) {
			Tile hoveredTile = gridOverlay.GetSelectedTile();
			bldgObject.transform.position = new Vector3 (hoveredTile.gameObject.transform.position.x,
			                                             bldgObject.gameObject.transform.position.y,
			                                             hoveredTile.gameObject.transform.position.z);
			int[] pos = new int[] {
				(int)gridOverlay.GetSelectedTile().position.x,
				(int)gridOverlay.GetSelectedTile().position.y
			};
			if(FarmManager.Instance.CheckBuildable(building, pos, orientation)) {
				bldgObject.GetComponentInChildren<Renderer>().material.color = Color.green;
			}
			else {
				bldgObject.GetComponentInChildren<Renderer>().material.color = Color.red;
			}
		}
	}

	public void Initialize(IDictionary<string,object> building) {
		FarmManager.Instance.SwitchState (Constants.FARM_MANAGER_STATE_BUILD_STRUCTURE);

		this.building = building;
		orientation = Constants.ORIENTATION_NORTH;
		gridOverlay.ToggleCanRenderLines(true);
		gridOverlay.ToggleCanHoverOnMap(true);
		gridOverlay.ToggleCanClickOnMap(true);
		//MouseHandler.Instance.enabled = true;
		bldgObject = Instantiate (Resources.Load (Constants.PATH_PREFABS_BUILDINGS + building[Constants.DB_KEYWORD_PREFAB_NAME],typeof(GameObject))) as GameObject;
		OnTileHoverChange ();
		isInitialized = true;
	}

	private void ChangeOrientation(string direction) {
		if (direction == "cw") {
			switch (orientation) {
			case Constants.ORIENTATION_NORTH:
				orientation = Constants.ORIENTATION_EAST;
				break;
			case Constants.ORIENTATION_EAST:
				orientation = Constants.ORIENTATION_SOUTH;
				break;
			case Constants.ORIENTATION_SOUTH:
				orientation = Constants.ORIENTATION_WEST;
				break;
			case Constants.ORIENTATION_WEST:
				orientation = Constants.ORIENTATION_NORTH;
				break;
			default:
				break;
			}
		} else if (direction == "ccw") {
			switch (orientation) {
			case Constants.ORIENTATION_NORTH:
				orientation = Constants.ORIENTATION_WEST;
				break;
			case Constants.ORIENTATION_EAST:
				orientation = Constants.ORIENTATION_NORTH;
				break;
			case Constants.ORIENTATION_SOUTH:
				orientation = Constants.ORIENTATION_EAST;
				break;
			case Constants.ORIENTATION_WEST:
				orientation = Constants.ORIENTATION_SOUTH;
				break;
			default:
				break;
			}
		}
	}

	public void ButtonCancel() {
		FarmManager.Instance.SwitchState (Constants.FARM_MANAGER_STATE_FREE_SELECT);

		Destroy (bldgObject);
		mainCanvas.gameObject.SetActive (true);
		mainCanvasLeft.gameObject.SetActive (true);
		mainCanvasRight.gameObject.SetActive (true);
		buildingPlacementCanvas.gameObject.SetActive (false);
		gridOverlay.ToggleCanRenderLines(false);
		gridOverlay.ToggleCanHoverOnMap(false);
		gridOverlay.ToggleCanClickOnMap(false);
		//MouseHandler.Instance.enabled = false;
		isInitialized = false;
	}

	public void ButtonRotateCCW() {
		bldgObject.transform.Rotate (new Vector3(0.0f,-90.0f,0.0f), Space.World);
		if (bldgObject.transform.eulerAngles.y <= -360.0f)
			bldgObject.transform.eulerAngles = new Vector3(bldgObject.transform.eulerAngles.x,
			                                               0,
			                                               bldgObject.transform.eulerAngles.z);
		ChangeOrientation ("ccw");
		OnTileHoverChange ();
	}

	public void ButtonRotateCW() {
		bldgObject.transform.Rotate (new Vector3(0.0f,90.0f,0.0f), Space.World);
		if (bldgObject.transform.eulerAngles.y >= 360.0f)
			bldgObject.transform.eulerAngles = new Vector3(bldgObject.transform.eulerAngles.x,
			                                               0,
			                                               bldgObject.transform.eulerAngles.z);
		ChangeOrientation ("cw");
		OnTileHoverChange ();
	}
}