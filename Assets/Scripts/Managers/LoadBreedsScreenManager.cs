using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LoadBreedsScreenManager : MonoBehaviour {

	public GameObject listPanel;
	public Button listButton;
	public Text infoText;
	public Button loadButton;

	private List<Button> breedListButtons;
	private string selectedBreed;

	private static LoadBreedsScreenManager instance;
	private LoadBreedsScreenManager() {}
	
	public static LoadBreedsScreenManager Instance {
		get {
			if(instance == null) {
				instance = (LoadBreedsScreenManager)GameObject.FindObjectOfType(typeof(LoadBreedsScreenManager));
			}
			return instance;
		}
	}

	void Start() {
		breedListButtons = new List<Button> ();
	}

	public void LoadBreedsList(List<string> l) {
		foreach (Button b in breedListButtons) {
			Destroy (b.gameObject);
		}
		breedListButtons.Clear ();
		infoText.text = "";
		loadButton.interactable = false;

		foreach (string s in l) {
			Button b = Instantiate(listButton);
			breedListButtons.Add (b);
			b.GetComponentInChildren<Text> ().text = s;
			b.transform.SetParent(listPanel.transform,false);
		}
	}

	public void SetSelected(string s) {
		string st = "";
		IDictionary<string, object> dic = ControlPanelBreedsManager.Instance.LoadBreed (s);
		foreach (KeyValuePair<string, object> kv in dic) {
			st += kv.Key + ": " + kv.Value + "\n";
		}
		infoText.text = st;
		selectedBreed = s;
		loadButton.interactable = true;
	}

	public void LoadBreed() {
		ControlPanelBreedsManager.Instance.LoadBreedForEditing (selectedBreed);
	}

}
