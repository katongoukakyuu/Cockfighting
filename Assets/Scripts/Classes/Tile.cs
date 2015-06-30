using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	public IDictionary<string,object> tile;

	void Start() {
		MouseHandler.Instance.OnMouseClick += OnClick;
	}

	private void OnClick(GameObject g) {
		if (g == this.gameObject) {
			FarmManager.Instance.UpdateSelectedObject(this.gameObject);
		}
	}

}
