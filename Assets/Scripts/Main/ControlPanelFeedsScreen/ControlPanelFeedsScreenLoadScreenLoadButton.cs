using UnityEngine;
using System.Collections;

public class ControlPanelFeedsScreenLoadScreenLoadButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject loadFeedsPanel;

	public void ButtonPressed() {
		LoadFeedsScreenManager.Instance.LoadFeeds ();

		mainPanel.SetActive (true);
		loadFeedsPanel.SetActive (false);
	}

}
