using UnityEngine;
using System.Collections;

public class BuildingsScreenBackButton : MonoBehaviour {

	public void ButtonPressed() {
		Application.LoadLevel (Constants.SCENE_CONTROL_PANEL);
	}
}
