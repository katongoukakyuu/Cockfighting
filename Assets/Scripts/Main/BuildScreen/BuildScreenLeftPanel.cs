using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildScreenLeftPanel : MonoBehaviour {

	public BuildScreenImagePanel imagePanel;

	public void ButtonPressed() {
		imagePanel.AnimateLeft();
	}

}
