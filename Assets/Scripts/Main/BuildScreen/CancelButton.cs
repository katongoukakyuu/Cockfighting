using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CancelButton : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas buildStructuresCanvas;

	public void ButtonPressed() {
		mainCanvas.gameObject.SetActive (true);
		buildStructuresCanvas.gameObject.SetActive (false);
	}

}
