using UnityEngine;
using System.Collections;

public class ControlPanelBuildingsScreenBackButton : MonoBehaviour {

	public void ButtonPressed() {
		Application.LoadLevel (Constants.SCENE_CONTROL_PANEL);
	}
}
