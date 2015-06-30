using UnityEngine;
using System.Collections;

public class BuildingsScreenResetButton : MonoBehaviour {

	public void ButtonPressed() {
		ControlPanelBuildingsManager.Instance.ResetFields ();
	}

}
