using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlPanelBuildingsScreenSaveButton : MonoBehaviour {

	public InputField[] fieldsToVerify;
	public InputField[] fieldsToSave;

	public void ButtonPressed() {
		for(int i = 0; i < fieldsToVerify.Length; i++) {
			if(fieldsToVerify[i].text == "") {
				ControlPanelBuildingsManager.Instance.SetMessage(Constants.GENERIC_ERROR_FIELDS);
				return;
			}
		}
		if (fieldsToSave.Length == 10) {
			ControlPanelBuildingsManager.Instance.SaveBuilding(
				fieldsToSave[0].text, fieldsToSave[1].text,
				int.Parse (fieldsToSave[2].text), int.Parse (fieldsToSave[3].text),
				int.Parse (fieldsToSave[4].text), int.Parse (fieldsToSave[5].text),
				int.Parse (fieldsToSave[6].text), int.Parse (fieldsToSave[7].text),
				fieldsToSave[8].text, fieldsToSave[9].text
			);
		}
		else {
			// print ("Error in saving Building, not enough fields to save.");
		}
	}

}
