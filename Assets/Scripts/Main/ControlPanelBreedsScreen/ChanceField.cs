using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChanceField : MonoBehaviour {

	public InputField otherInputField;

	private InputField thisInputField;

	void Start() {
		thisInputField = GetComponent<InputField> ();
	}

	public void OnEditField() {
		if(thisInputField.text == "" || int.Parse(thisInputField.text) < 0) {
			thisInputField.text = "0";
		}
		else if(int.Parse(thisInputField.text) > 100) {
			thisInputField.text = "100";
		}
		otherInputField.text = "" + (100 - int.Parse (thisInputField.text));
	}

}
