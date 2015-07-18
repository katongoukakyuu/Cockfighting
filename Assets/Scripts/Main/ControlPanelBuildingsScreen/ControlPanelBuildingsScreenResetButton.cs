using UnityEngine;
using System.Collections;

public class ControlPanelBuildingsScreenResetButton : MonoBehaviour {

	public void ButtonPressed() {
		ControlPanelBuildingsManager.Instance.ResetFields ();
	}

}
