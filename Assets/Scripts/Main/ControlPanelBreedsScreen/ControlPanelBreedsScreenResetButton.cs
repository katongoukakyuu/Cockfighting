using UnityEngine;
using System.Collections;

public class ControlPanelBreedsScreenResetButton : MonoBehaviour {

	public void ButtonPressed() {
		ControlPanelBreedsManager.Instance.ResetFields ();
	}

}
