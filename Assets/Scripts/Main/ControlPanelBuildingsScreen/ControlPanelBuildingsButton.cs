using UnityEngine;
using System.Collections;

public class ControlPanelBuildingsButton : MonoBehaviour {

	public void ButtonPressed() {
		Application.LoadLevel (Constants.SCENE_CONTROL_PANEL_BUILDINGS);
	}
}
