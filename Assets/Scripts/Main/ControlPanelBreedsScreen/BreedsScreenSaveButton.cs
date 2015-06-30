using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BreedsScreenSaveButton : MonoBehaviour {

	public InputField[] fieldsToVerify;
	public InputField[] fieldsToSave;

	public void ButtonPressed() {
		for(int i = 0; i < fieldsToVerify.Length; i++) {
			if(fieldsToVerify[i].text == "") {
				ControlPanelBreedsManager.Instance.SetMessage(Constants.GENERIC_ERROR_FIELDS);
				return;
			}
		}
		if (fieldsToSave.Length == 13) {
			ControlPanelBreedsManager.Instance.SaveBreed(
				fieldsToSave[0].text,
				fieldsToSave[1].text,int.Parse (fieldsToSave[2].text),fieldsToSave[3].text,
				fieldsToSave[4].text,int.Parse (fieldsToSave[5].text),fieldsToSave[6].text,
				fieldsToSave[7].text,int.Parse (fieldsToSave[8].text),fieldsToSave[9].text,
				fieldsToSave[10].text,int.Parse (fieldsToSave[11].text),fieldsToSave[12].text
			);
		}
		else {
			print ("Error in saving Breed, not enough fields to save.");
		}
	}

}
