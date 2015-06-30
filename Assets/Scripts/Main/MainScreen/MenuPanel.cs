using UnityEngine;
using System.Collections;

public class MenuPanel : MonoBehaviour {

	void Awake() {
		MouseHandler.Instance.OnMouseClick += OnMouseClick;
	}

	void OnMouseClick(GameObject g) {
		gameObject.SetActive (false);
	}
}
