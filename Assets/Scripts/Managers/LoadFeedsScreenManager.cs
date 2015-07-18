using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LoadFeedsScreenManager : MonoBehaviour {

	public GameObject listPanel;
	public Button listButton;
	public Text infoText;
	public Button loadButton;

	private List<Button> feedsListButtons;
	private string selectedFeeds;

	private static LoadFeedsScreenManager instance;
	private LoadFeedsScreenManager() {}
	
	public static LoadFeedsScreenManager Instance {
		get {
			if(instance == null) {
				instance = (LoadFeedsScreenManager)GameObject.FindObjectOfType(typeof(LoadFeedsScreenManager));
			}
			return instance;
		}
	}

	void Start() {
		feedsListButtons = new List<Button> ();
	}

	public void LoadFeedsList(List<string> l) {
		foreach (Button b in feedsListButtons) {
			Destroy (b.gameObject);
		}
		feedsListButtons.Clear ();
		infoText.text = "";
		loadButton.interactable = false;

		foreach (string s in l) {
			Button b = Instantiate(listButton);
			feedsListButtons.Add (b);
			b.GetComponentInChildren<Text> ().text = s;
			b.transform.SetParent(listPanel.transform,false);
		}
	}

	public void SetSelected(string s) {
		string st = "";
		IDictionary<string, object> dic = ControlPanelFeedsManager.Instance.LoadFeeds (s);
		foreach (KeyValuePair<string, object> kv in dic) {
			st += kv.Key + ": " + kv.Value + "\n";
		}
		infoText.text = st;
		selectedFeeds = s;
		loadButton.interactable = true;
	}

	public void LoadFeeds() {
		ControlPanelFeedsManager.Instance.LoadFeedsForEditing (selectedFeeds);
	}

}
