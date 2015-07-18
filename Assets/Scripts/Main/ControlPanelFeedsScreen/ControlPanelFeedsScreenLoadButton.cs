using UnityEngine;
using System.Collections;

public class ControlPanelFeedsScreenLoadButton : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject loadFeedsPanel;

	public void ButtonPressed() {
		ControlPanelFeedsManager.Instance.LoadFeeds ();
		LoadFeedsScreenManager.Instance.LoadFeedsList (ControlPanelFeedsManager.Instance.GetFeedsList ());

		mainPanel.SetActive (false);
		loadFeedsPanel.SetActive (true);
	}

}
