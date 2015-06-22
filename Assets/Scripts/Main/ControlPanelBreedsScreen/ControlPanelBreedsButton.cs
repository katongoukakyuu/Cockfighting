using UnityEngine;
using System.Collections;

public class ControlPanelBreedsButton : MonoBehaviour {

	public void ButtonPressed() {
		Application.LoadLevel (Constants.SCENE_CONTROL_PANEL_BREEDS);
	}
}
