using UnityEngine;
using System.Collections;

public class ControlPanelFeedsScreenResetButton : MonoBehaviour {

	public void ButtonPressed() {
		ControlPanelFeedsManager.Instance.ResetFields ();
	}

}
