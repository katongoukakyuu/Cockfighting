using UnityEngine;
using System.Collections;

public class ControlPanelFeedsScreenBackButton : MonoBehaviour {

	public void ButtonPressed() {
		Application.LoadLevel (Constants.SCENE_CONTROL_PANEL);
	}
}
