using UnityEngine;
using System.Collections;

public class BreedsScreenBackButton : MonoBehaviour {

	public void ButtonPressed() {
		Application.LoadLevel (Constants.SCENE_CONTROL_PANEL);
	}
}
