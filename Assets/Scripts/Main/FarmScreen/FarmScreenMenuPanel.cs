using UnityEngine;
using System.Collections;

public class FarmScreenMenuPanel : MonoBehaviour {

	void Awake() {
		MouseHandler.Instance.OnMouseClick += OnMouseClick;
	}

	void OnMouseClick(GameObject g) {
		gameObject.SetActive (false);
	}
}
