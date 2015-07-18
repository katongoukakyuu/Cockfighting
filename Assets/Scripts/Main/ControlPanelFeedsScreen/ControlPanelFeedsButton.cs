using UnityEngine;
using System.Collections;

public class ControlPanelFeedsButton : MonoBehaviour {

	public void ButtonPressed() {
		Application.LoadLevel (Constants.SCENE_CONTROL_PANEL_FEEDS);
	}
}
