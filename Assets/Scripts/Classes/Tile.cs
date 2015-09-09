using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	public Vector2 position;
	public Material matSelected;
	public Material matDeselected;

	void Start() {
		if(MouseHandler.Instance != null) {
			MouseHandler.Instance.OnMouseClick += OnClick;
		}
		GetComponent<Renderer>().material = matDeselected;
	}

	private void OnClick(GameObject g) {
		if (g == this.gameObject) {
			print ("i am selected");
			GetComponent<Renderer>().material = matSelected;
			if(FarmManager.Instance != null) {
				FarmManager.Instance.UpdateSelectedObject(this.gameObject);
			}
		}
		else {
			GetComponent<Renderer>().material = matDeselected;
		}

	}

}
