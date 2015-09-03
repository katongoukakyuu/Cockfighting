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

	public Canvas messageCanvas;

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
				 	properties[Constants.DB_KEYWORD_OWNER].ToString() == PlayerManager.Instance.player[Constants.DB_KEYWORD_USERNAME].ToString())) {
					DatabaseManager.Instance.UpdatePlayer(PlayerManager.Instance.player[Constants.DB_KEYWORD_USERNAME].ToString (),
					                                      PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString());
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
		UpdateChickens ();
		UpdateBuildingsOwned ();
	}

	public void UpdateScreen() {
		coinText.text = PlayerManager.Instance.player[Constants.DB_KEYWORD_COIN].ToString();
		cashText.text = PlayerManager.Instance.player[Constants.DB_KEYWORD_CASH].ToString();
	}

	public void UpdateChickens() {
		foreach(GameObject g in listChickens) {
			Destroy (g);
		}

		int xMax = TileMap.Instance.tiles.GetLength (0);
		int yMax = TileMap.Instance.tiles.GetLength (1);

		foreach(IDictionary<string,object> i in PlayerManager.Instance.playerChickens) {
			GameObject g = Instantiate (chicken) as GameObject;
			g.AddComponent<Chicken>();
			g.GetComponent<Chicken>().chicken = i;
			g.transform.position = new Vector3(Random.Range (0,xMax),
			                                   0,
			                                   Random.Range (0,yMax));
			listChickens.Add (g);
		}
	}

	public void UpdateBuildingsOwned() {
		foreach(GameObject g in listBuildings) {
			Destroy (g);
		}

		foreach(IDictionary<string,object> i in PlayerManager.Instance.playerBuildings) {
			IDictionary<string,object> bldg = DatabaseManager.Instance.LoadBuilding(i[Constants.DB_KEYWORD_NAME].ToString());
			GameObject g = Instantiate (Resources.Load ("Prefabs/" + bldg[Constants.DB_KEYWORD_PREFAB_NAME])) as GameObject;
			g.transform.position = new Vector3(int.Parse(i[Constants.DB_KEYWORD_X_POSITION].ToString()),
			                                   0,
			                                   int.Parse(i[Constants.DB_KEYWORD_Y_POSITION].ToString()));
			switch(i[Constants.DB_KEYWORD_ORIENTATION].ToString()) {
			case Constants.ORIENTATION_NORTH:
				g.transform.Rotate (new Vector3(0.0f,0.0f,0.0f));
				break;
			case Constants.ORIENTATION_EAST:
				g.transform.Rotate (new Vector3(0.0f,90.0f,0.0f));
				break;
			case Constants.ORIENTATION_SOUTH:
				g.transform.Rotate (new Vector3(0.0f,180.0f,0.0f));
				break;
			case Constants.ORIENTATION_WEST:
				g.transform.Rotate (new Vector3(0.0f,270.0f,0.0f));
				break;
			default:
				break;
			}
			listBuildings.Add (g);
		}
	}

	public void UpdateSelectedObject(GameObject g) {
		print ("tag is " + g.tag);
		if (state == Constants.FARM_MANAGER_STATE_FREE_SELECT) {
			if (g.tag == "Chicken" && state == Constants.FARM_MANAGER_STATE_FREE_SELECT) {
				selectedObject = g;
				mainCameraDummy = Instantiate(Camera.main.gameObject);
				mainCameraDummy.SetActive(false);
				CameraControls.Instance.freeCamera = false;
				UpdateChickenStats(selectedObject.GetComponent<Chicken>().chicken);
				chickenStatsCanvas.SetActive(true);
				StartCoroutine(SwitchCamera(Camera.main, Camera.main.transform, g.transform.Find("Camera Stand/Camera").transform,
				                            g.transform.FindChild("Camera Stand"), false, 0.5f, 20));
			}
			else {
				selectedObject = null;
				SwitchToFreeCamera();
			}
		}
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
		int xMax = TileMap.Instance.tiles.GetLength (0);
		int yMax = TileMap.Instance.tiles.GetLength (1);
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
		/*print ("north orientation bldg tiles:");
		foreach (Vector2 v in GameManager.Instance.GetBuildingTiles (pos, bldgCenter, bldgSize, Constants.ORIENTATION_NORTH)) {
			print(v.x + " " + v.y);
		}

		print ("east orientation bldg tiles:");
		foreach (Vector2 v in GameManager.Instance.GetBuildingTiles (pos, bldgCenter, bldgSize, Constants.ORIENTATION_EAST)) {
			print(v.x + " " + v.y);
		}

		print ("south orientation bldg tiles:");
		foreach (Vector2 v in GameManager.Instance.GetBuildingTiles (pos, bldgCenter, bldgSize, Constants.ORIENTATION_SOUTH)) {
			print(v.x + " " + v.y);
		}

		print ("west orientation bldg tiles:");
		foreach (Vector2 v in GameManager.Instance.GetBuildingTiles (pos, bldgCenter, bldgSize, Constants.ORIENTATION_WEST)) {
			print(v.x + " " + v.y);
		}
		*/
	}

	public void BuildStructure(IDictionary<string,object> building, int[] pos, string orientation) {
		Dictionary<string, object> dic = GameManager.Instance.GenerateBuildingOwnedByPlayer (
			building[Constants.DB_KEYWORD_NAME].ToString(),
			PlayerManager.Instance.player[Constants.DB_KEYWORD_USERNAME].ToString(),
			"default",
			pos[0],
			pos[1],
			orientation
		);
		DatabaseManager.Instance.SaveBuildingOwnedByPlayer (dic);
	}

	private void SwitchToFreeCamera() {
		if(!CameraControls.Instance.freeCamera) {
			CameraControls.Instance.freeCamera = true;
			chickenStatsCanvas.SetActive(false);
			StartCoroutine(SwitchCamera(Camera.main, Camera.main.transform, mainCameraDummy.transform,
			                            null, true, 0.5f, 20));
		}
	}

	private IEnumerator SwitchCamera(Camera camera, Transform cameraFrom, Transform cameraTo,
	                                 Transform parentTo, bool destroyCameraTo,
	                                 float animDuration, int animSteps) {	
		while(isAnimating)
			yield return new WaitForSeconds(0.1f);
		camera.transform.parent = parentTo;
		print (cameraFrom.localPosition);
		print (cameraTo.localPosition);
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
}
