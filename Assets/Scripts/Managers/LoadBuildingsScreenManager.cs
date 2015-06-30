using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LoadBuildingsScreenManager : MonoBehaviour {

	public GameObject listPanel;
	public Button listButton;
	public Text infoText;
	public Button loadButton;

	private List<Button> buildingListButtons;
	private string selectedBuilding;

	private static LoadBuildingsScreenManager instance;
	private LoadBuildingsScreenManager() {}
	
	public static LoadBuildingsScreenManager Instance {
		get {
			if(instance == null) {
				instance = (LoadBuildingsScreenManager)GameObject.FindObjectOfType(typeof(LoadBuildingsScreenManager));
			}
			return instance;
		}
	}

	void Start() {
		buildingListButtons = new List<Button> ();
	}

	public void LoadBuildingsList(List<string> l) {
		foreach (Button b in buildingListButtons) {
			Destroy (b.gameObject);
		}
		buildingListButtons.Clear ();
		infoText.text = "";
		loadButton.interactable = false;

		foreach (string s in l) {
			Button b = Instantiate(listButton);
			buildingListButtons.Add (b);
			b.GetComponentInChildren<Text> ().text = s;
			b.transform.SetParent(listPanel.transform,false);
		}
	}

	public void SetSelected(string s) {
		string st = "";
		IDictionary<string, object> dic = ControlPanelBuildingsManager.Instance.LoadBuilding (s);
		foreach (KeyValuePair<string, object> kv in dic) {
			st += kv.Key + ": " + kv.Value + "\n";
		}
		infoText.text = st;
		selectedBuilding = s;
		loadButton.interactable = true;
	}

	public void LoadBuilding() {
		ControlPanelBuildingsManager.Instance.LoadBuildingForEditing (selectedBuilding);
	}

}
