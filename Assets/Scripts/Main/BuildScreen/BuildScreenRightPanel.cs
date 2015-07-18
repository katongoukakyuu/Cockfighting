using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildScreenRightPanel : MonoBehaviour {

	public BuildScreenImagePanel imagePanel;
	
	public void ButtonPressed() {
		imagePanel.AnimateRight();
	}

}
