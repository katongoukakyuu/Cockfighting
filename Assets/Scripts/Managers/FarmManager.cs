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

	public GameObject chickenStatsCanvas;
	public Text[] chickenStatsFields;
	
	private System.EventHandler<DatabaseChangeEventArgs> eventHandler;

	private bool isAnimating = false;
	private GameObject selectedObject;
	private GameObject mainCameraDummy;

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
				if(change.DocumentId.Contains(PlayerManager.Instance.player["username"].ToString ())) {
					DatabaseManager.Instance.UpdatePlayer((string)PlayerManager.Instance.player["username"]);
					UpdateScreen();
					return;
				}
			}
		};

		DatabaseManager.Instance.GetDatabase().Changed += eventHandler;
		UpdateScreen();
		UpdateChickens ();
	}

	void OnDestroy() {
		DatabaseManager.Instance.GetDatabase().Changed -= eventHandler;
	}

	private void UpdateScreen() {
		coinText.text = PlayerManager.Instance.player["coin"].ToString();
		cashText.text = PlayerManager.Instance.player["cash"].ToString();
	}

	private void UpdateChickens() {
		int xMax = TileMap.Instance.tiles.GetLength (0);
		int yMax = TileMap.Instance.tiles.GetLength (1);

		foreach(IDictionary<string,object> i in PlayerManager.Instance.playerChickens) {
			GameObject g = Instantiate (chicken) as GameObject;
			g.AddComponent<Chicken>();
			g.GetComponent<Chicken>().chicken = i;
			g.transform.position = new Vector3(Random.Range (0,xMax),
			                                   0,
			                                   Random.Range (0,yMax));
		}
	}

	public void UpdateSelectedObject(GameObject g) {
		print ("tag is " + g.tag);
		if (g.tag == "Chicken") {
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
			CameraControls.Instance.freeCamera = true;
			chickenStatsCanvas.SetActive(false);
			StartCoroutine(SwitchCamera(Camera.main, Camera.main.transform, mainCameraDummy.transform,
			                            null, true, 0.5f, 20));

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
