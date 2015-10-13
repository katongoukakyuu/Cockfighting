using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlPanelFeedsScreenSaveButton : MonoBehaviour {

	public InputField[] fieldsToVerify;
	public InputField[] fieldsToSave;

	public void ButtonPressed() {
		for(int i = 0; i < fieldsToVerify.Length; i++) {
			if(fieldsToVerify[i].text == "") {
				ControlPanelFeedsManager.Instance.SetMessage(Constants.GENERIC_ERROR_FIELDS);
				return;
			}
		}
		if (fieldsToSave.Length == 15) {
			ControlPanelFeedsManager.Instance.SaveFeeds(
				fieldsToSave[0].text, fieldsToSave[1].text,
				int.Parse (fieldsToSave[2].text), int.Parse (fieldsToSave[3].text), true,
				int.Parse (fieldsToSave[4].text), int.Parse (fieldsToSave[5].text),
				int.Parse (fieldsToSave[6].text), int.Parse (fieldsToSave[7].text),
				int.Parse (fieldsToSave[8].text), int.Parse (fieldsToSave[9].text),
				int.Parse (fieldsToSave[10].text), int.Parse (fieldsToSave[11].text),
				int.Parse (fieldsToSave[12].text), int.Parse (fieldsToSave[13].text),
				fieldsToSave[14].text
			);
		}
		else {
			// print ("Error in saving Feeds, not enough fields to save.");
		}
	}

}
