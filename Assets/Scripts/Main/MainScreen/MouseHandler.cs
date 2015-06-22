using UnityEngine;
using System.Collections;

public class MouseHandler : MonoBehaviour {

	private static MouseHandler instance;
	private MouseHandler() {}

	public static MouseHandler Instance {
		get {
			if(instance == null) {
				instance = (MouseHandler)GameObject.FindObjectOfType(typeof(MouseHandler));
			}
			return instance;
		}
	}
	
	public delegate void OnMouseClickEvent();
	public event OnMouseClickEvent OnMouseClick;

	void Update() {
		if(Input.GetMouseButton(0)) {
			Instance.OnMouseClick();
		}
	}

}
