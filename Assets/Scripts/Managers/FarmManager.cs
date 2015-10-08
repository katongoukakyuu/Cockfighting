using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Couchbase.Lite;

public class FarmManager : MonoBehaviour {

	public Text coinText;
	public Text cashText;
	public GameObject chicken;
	public Animator mainAnim;
	public Animator ChickenStatAnim;

	public GridOverlay gridOverlay;

	public Canvas messageCanvas;

	public GameObject mainCanvas;
	public GameObject chickenStatsCanvas;
	public Text[] chickenStatsFields;
	
	private System.EventHandler<DatabaseChangeEventArgs> eventHandler;

	private string state = Constants.FARM_MANAGER_STATE_FREE_SELECT;
	private bool isAnimating = false;
	private GameObject selectedObject;
	private GameObject mainCameraDummy;

	private List<GameObject> listChickens = new List<GameObject> ();
	private List<GameObject> listBuildings = new List<GameObject> ();

	private static FarmManager instance;
	private FarmManager() {}

	public static FarmManager Instance {
		get {
			if(instance == null) {
				instance = (FarmManager)GameObject.FindObjectOfType(typeof(FarmManager));
			}
			return instance;
		}
	}

	void Start() {
		eventHandler = (sender, e) => {
			var changes = e.Changes.ToList();
			foreach (DocumentChange change in changes) {
				IDictionary<string,object> properties = DatabaseManager.Instance.GetDatabase().GetDocument(change.DocumentId).Properties;
				if((properties[Constants.DB_KEYWORD_TYPE].ToString() == Constants.DB_TYPE_ACCOUNT &&
				   properties[Constants.DB_COUCHBASE_ID].ToString() == PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString()) ||
				   (properties[Constants.DB_KEYWORD_TYPE].ToString() == Constants.DB_TYPE_CHICKEN &&
				 	properties[Constants.DB_KEYWORD_USER_ID].ToString() == PlayerManager.Instance.player[Constants.DB_KEYWORD_USER_ID].ToString())) {
					DatabaseManager.Instance.UpdatePlayer(PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString());
					UpdateScreen();
					return;
				}
			}
		};

		DatabaseManager.Instance.GetDatabase().Changed += eventHandler;
		UpdatePlayer ();
	}

	void OnDestroy() {
		if (DatabaseManager.Instance != null) {
			DatabaseManager.Instance.GetDatabase().Changed -= eventHandler;
		}
	}

	public string State() {
		return state;
	}

	public void SwitchState(string s) {
		state = s;
	}

	public void UpdatePlayer() {
		UpdateScreen();
		UpdateBuildingsOwned ();
		UpdateChickens ();
	}

	public void UpdateScreen() {
		coinText.text = PlayerManager.Instance.player[Constants.DB_KEYWORD_COIN].ToString();
		cashText.text = PlayerManager.Instance.player[Constants.DB_KEYWORD_CASH].ToString();
	}

	public void UpdateChickens() {
		foreach(GameObject g in listChickens) {
			Destroy (g);
		}
		
		if(gridOverlay != null) {
			int xMax = gridOverlay.GetTiles().GetLength(0);
			int yMax = gridOverlay.GetTiles().GetLength(1);
			int x = 0;
			// print ("Grid overlay xy: " + xMax + ", " + yMax);
			foreach(IDictionary<string,object> i in PlayerManager.Instance.playerChickens) {
				GameObject g;
				if(x < listBuildings.Count && listBuildings[x] != null) {
					g = Instantiate (chicken,
					                 listBuildings[x].transform.position,
					                 Quaternion.identity) as GameObject;
				}
				else {
					g = Instantiate (chicken,
					                 gridOverlay.GetTiles()[Random.Range (0,xMax), Random.Range (0,yMax)].transform.position,
					                 Quaternion.identity) as GameObject;
				}
				g.AddComponent<Chicken>();
				g.GetComponent<Chicken>().chicken = i;
				listChickens.Add (g);
				x++;
			}
		}
	}

	public void UpdateBuildingsOwned() {
		foreach(GameObject g in listBuildings) {
			Destroy (g);
		}

		foreach(IDictionary<string,object> i in PlayerManager.Instance.playerBuildings) {
			IDictionary<string,object> bldg = DatabaseManager.Instance.LoadBuilding(i[Constants.DB_KEYWORD_NAME].ToString());
			GameObject g = Instantiate (Resources.Load (Constants.PATH_PREFABS_BUILDINGS + bldg[Constants.DB_KEYWORD_PREFAB_NAME])) as GameObject;
			Tile t = gridOverlay.GetTiles()[int.Parse(i[Constants.DB_KEYWORD_X_POSITION].ToString()), int.Parse(i[Constants.DB_KEYWORD_Y_POSITION].ToString())].GetComponent<Tile>();
			g.transform.position = new Vector3(t.transform.position.x,
			                                   g.transform.position.y,
			                                   t.transform.position.z);
			NavMeshObstacle nvo = g.GetComponent<NavMeshObstacle>();
			if(nvo != null) {
				nvo.enabled = true;
			}
			switch(i[Constants.DB_KEYWORD_ORIENTATION].ToString()) {
			case Constants.ORIENTATION_NORTH:
				g.transform.Rotate (new Vector3(0.0f,0.0f,0.0f), Space.World);
				break;
			case Constants.ORIENTATION_EAST:
				g.transform.Rotate (new Vector3(0.0f,90.0f,0.0f), Space.World);
				break;
			case Constants.ORIENTATION_SOUTH:
				g.transform.Rotate (new Vector3(0.0f,180.0f,0.0f), Space.World);
				break;
			case Constants.ORIENTATION_WEST:
				g.transform.Rotate (new Vector3(0.0f,270.0f,0.0f), Space.World);
				break;
			default:
				break;
			}
			listBuildings.Add (g);
		}
	}

	public void UpdateSelectedObject(GameObject g) {
		// print ("tag is " + g.tag);
		if (state == Constants.FARM_MANAGER_STATE_FREE_SELECT) {
			if (g.tag == "Chicken" && state == Constants.FARM_MANAGER_STATE_FREE_SELECT) {
				selectedObject = g;
				//mainCameraDummy = Instantiate(Camera.main.gameObject);
				//mainCameraDummy.SetActive(false);
				//CameraControls.Instance.freeCamera = false;
				CameraControls.Instance.freeCamera = false;
				UpdateChickenStats(selectedObject.GetComponent<Chicken>().chicken);
				mainAnim.SetBool("isHidden", true);
				chickenStatsCanvas.SetActive(true);
				Invoke ("ChickenStatOpenFunction", 0.2f);
				/*StartCoroutine(SwitchCamera(Camera.main, Camera.main.transform, g.transform.Find("Camera Stand/Camera").transform,
				                            g.transform.FindChild("Camera Stand"), false, 0.5f, 20));*/
			}
			else {
				selectedObject = null;
				SwitchToFreeCamera();
			}
		}
	}

	public void UpdateClick(GameObject g) {
		if(g.tag != "Chicken" && chickenStatsCanvas.activeSelf) {
			SwitchToFreeCamera();
		}
	}

	void ChickenStatCloseFunction()
	{
		mainCanvas.SetActive(true);
		chickenStatsCanvas.SetActive(false);
	}

	void ChickenStatOpenFunction()
	{
		mainCanvas.SetActive(false);
		chickenStatsCanvas.SetActive(true);
	}

	private void UpdateChickenStats(IDictionary<string,object> dic) {
		if (chickenStatsFields.Length == 11) {
			chickenStatsFields[0].text = dic[Constants.DB_KEYWORD_NAME].ToString();
			chickenStatsFields[1].text = dic[Constants.DB_TYPE_BREED].ToString();
			chickenStatsFields[2].text = dic[Constants.DB_KEYWORD_GENDER].ToString();
			chickenStatsFields[3].text = dic[Constants.DB_KEYWORD_LIFE_STAGE].ToString();
			chickenStatsFields[4].text = dic[Constants.DB_KEYWORD_NOTES].ToString();
			chickenStatsFields[5].text = dic[Constants.DB_KEYWORD_ATTACK] + " / " + dic[Constants.DB_KEYWORD_ATTACK_MAX];
			chickenStatsFields[6].text = dic[Constants.DB_KEYWORD_DEFENSE] + " / " + dic[Constants.DB_KEYWORD_DEFENSE_MAX];
			chickenStatsFields[7].text = dic[Constants.DB_KEYWORD_HP] + " / " + dic[Constants.DB_KEYWORD_HP_MAX];
			chickenStatsFields[8].text = dic[Constants.DB_KEYWORD_AGILITY] + " / " + dic[Constants.DB_KEYWORD_AGILITY_MAX];
			chickenStatsFields[9].text = dic[Constants.DB_KEYWORD_GAMENESS] + " / " + dic[Constants.DB_KEYWORD_GAMENESS_MAX];
			chickenStatsFields[10].text = dic[Constants.DB_KEYWORD_AGGRESSION] + " / " + dic[Constants.DB_KEYWORD_AGGRESSION_MAX];
		}
	}

	public bool CheckBuildable(IDictionary<string,object> building, int[] pos, string orientation) {
		int xMax = gridOverlay.GetTiles().GetLength(0);
		int yMax = gridOverlay.GetTiles().GetLength(1);
		int[] bldgCenter = new int[] {
			int.Parse(building [Constants.DB_KEYWORD_X_CENTER].ToString()),
			int.Parse(building [Constants.DB_KEYWORD_Y_CENTER].ToString())
		};
		int[] bldgSize = new int[] {
			int.Parse(building [Constants.DB_KEYWORD_X_SIZE].ToString()),
			int.Parse(building [Constants.DB_KEYWORD_Y_SIZE].ToString()),
		};

		List<Vector2> bldgTiles = GameManager.Instance.GetBuildingTiles (pos, bldgCenter, bldgSize, orientation);
		foreach (Vector2 v in bldgTiles) {
			if(v.x < 0 || v.y < 0 || v.x >= xMax || v.y >= yMax ||
			   PlayerManager.Instance.playerOccupiedTiles.Contains(v)) {
				return false;
			}
		}
		return true;
		
		// debug
		/*// print ("north orientation bldg tiles:");
		foreach (Vector2 v in GameManager.Instance.GetBuildingTiles (pos, bldgCenter, bldgSize, Constants.ORIENTATION_NORTH)) {
			// print(v.x + " " + v.y);
		}

		// print ("east orientation bldg tiles:");
		foreach (Vector2 v in GameManager.Instance.GetBuildingTiles (pos, bldgCenter, bldgSize, Constants.ORIENTATION_EAST)) {
			// print(v.x + " " + v.y);
		}

		// print ("south orientation bldg tiles:");
		foreach (Vector2 v in GameManager.Instance.GetBuildingTiles (pos, bldgCenter, bldgSize, Constants.ORIENTATION_SOUTH)) {
			// print(v.x + " " + v.y);
		}

		// print ("west orientation bldg tiles:");
		foreach (Vector2 v in GameManager.Instance.GetBuildingTiles (pos, bldgCenter, bldgSize, Constants.ORIENTATION_WEST)) {
			// print(v.x + " " + v.y);
		}
		*/
	}

	public void BuildStructure(IDictionary<string,object> building, int[] pos, string orientation) {
		Dictionary<string, object> dic = GameManager.Instance.GenerateBuildingOwnedByPlayer (
			building[Constants.DB_KEYWORD_NAME].ToString(),
			PlayerManager.Instance.player[Constants.DB_KEYWORD_USER_ID].ToString(),
			"default",
			pos[0],
			pos[1],
			orientation
		);
		DatabaseManager.Instance.SaveEntry (dic);
	}

	private void SwitchToFreeCamera() {
		CameraControls.Instance.freeCamera = true;
		mainCanvas.SetActive(true);
		ChickenStatAnim.SetBool("isHidden", true);
		Invoke ("ChickenStatCloseFunction", 0.2f);
		/*if(!CameraControls.Instance.freeCamera) {
			CameraControls.Instance.freeCamera = true;
			chickenStatsCanvas.SetActive(false);
			StartCoroutine(SwitchCamera(Camera.main, Camera.main.transform, mainCameraDummy.transform,
			                            null, true, 0.5f, 20));
		}*/
	}

	private IEnumerator SwitchCamera(Camera camera, Transform cameraFrom, Transform cameraTo,
	                                 Transform parentTo, bool destroyCameraTo,
	                                 float animDuration, int animSteps) {	
		while(isAnimating)
			yield return new WaitForSeconds(0.1f);
		camera.transform.parent = parentTo;
		// print (cameraFrom.localPosition);
		// print (cameraTo.localPosition);
		float posIncX = (cameraTo.localPosition.x - cameraFrom.localPosition.x) / animSteps;
		float posIncY = (cameraTo.localPosition.y - cameraFrom.localPosition.y) / animSteps;
		float posIncZ = (cameraTo.localPosition.z - cameraFrom.localPosition.z) / animSteps;
		float rotIncX = (cameraTo.localEulerAngles.x - cameraFrom.localEulerAngles.x) / animSteps;
		float rotIncY = (cameraTo.localEulerAngles.y - cameraFrom.localEulerAngles.y) / animSteps;
		float rotIncZ = (cameraTo.localEulerAngles.z - cameraFrom.localEulerAngles.z) / animSteps;
		float animWait = animDuration / animSteps;
		
		isAnimating = true;

		for(int x = 0; x < animSteps; x++) {
			camera.transform.localPosition = new Vector3(camera.transform.localPosition.x + posIncX,
			                                             camera.transform.localPosition.y + posIncY,
			                                             camera.transform.localPosition.z + posIncZ);
			camera.transform.localEulerAngles = new Vector3(camera.transform.localEulerAngles.x + rotIncX,
			                                                camera.transform.localEulerAngles.y + rotIncY,
			                                                camera.transform.localEulerAngles.z + rotIncZ);
			yield return new WaitForSeconds(animWait);
		}
		
		isAnimating = false;
		if (destroyCameraTo)
			Destroy (cameraTo.gameObject);
	}

	public void Logout() {
		Application.LoadLevel(Constants.SCENE_LOGIN);
	}
}
