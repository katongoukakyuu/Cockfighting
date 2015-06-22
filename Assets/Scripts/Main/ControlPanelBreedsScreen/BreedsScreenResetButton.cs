using UnityEngine;
using System.Collections;

public class BreedsScreenResetButton : MonoBehaviour {

	public void ButtonPressed() {
		ControlPanelBreedsManager.Instance.ResetFields ();
	}

}
