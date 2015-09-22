using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildScreenCancelButton : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas buildStructuresCanvas;

	public Animator buildAnimator;

	public void ButtonPressed() {
		buildAnimator.SetBool("isHidden", true);
		Invoke ("CancelBuildFunctions", 0.2f);
	}

	void CancelBuildFunctions()
	{
		mainCanvas.gameObject.SetActive (true);
		buildStructuresCanvas.gameObject.SetActive (false);
	}

}
