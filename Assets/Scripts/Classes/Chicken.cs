using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chicken : MonoBehaviour {

	public IDictionary<string,object> chicken;

	void Start() {
		MouseHandler.Instance.OnMouseClick += OnClick;
	}

	private void OnClick(GameObject g) {
		if (g == this.gameObject) {
			FarmManager.Instance.UpdateSelectedObject(this.transform.root.gameObject);
		}
	}

}
