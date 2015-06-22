using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildRightPanel : MonoBehaviour {

	public ImagePanel imagePanel;
	
	public void ButtonPressed() {
		imagePanel.AnimateRight();
	}

}
