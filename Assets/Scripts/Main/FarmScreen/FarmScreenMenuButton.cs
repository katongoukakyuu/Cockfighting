using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FarmScreenMenuButton : MonoBehaviour {

	public GameObject panel;

	public void ButtonPressed() {
		panel.SetActive (!panel.activeSelf);
	}
}
