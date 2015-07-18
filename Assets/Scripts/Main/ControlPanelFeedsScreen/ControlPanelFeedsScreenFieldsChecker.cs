using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlPanelFeedsScreenFieldsChecker : MonoBehaviour {

	private InputField text;

	void Start() {
		text = GetComponent<InputField> ();
	}

	public void OnEndEdit(int max) {
		if (text.text == "" || (int.Parse (text.text) < 0 && max != 0)) {
			text.text = "0";
		} else if (int.Parse (text.text) > max && max != 0) {
			text.text = "" + max;
		}
	}

}
