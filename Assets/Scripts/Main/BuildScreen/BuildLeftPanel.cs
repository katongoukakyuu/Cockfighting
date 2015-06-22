using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildLeftPanel : MonoBehaviour {

	public ImagePanel imagePanel;

	public void ButtonPressed() {
		imagePanel.AnimateLeft();
	}

}
