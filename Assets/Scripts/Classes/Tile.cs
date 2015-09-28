using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	public Vector2 position;
	public Material matSelected;
	public Material matDeselected;
	public bool isSelectable = false;

	void Start() {
		if(MouseHandler.Instance != null) {
			MouseHandler.Instance.OnMouseClick += OnClick;
		}
		if(matDeselected != null) {
			GetComponent<Renderer>().material = matDeselected;
		}
	}

	private void OnClick(GameObject g) {
		if(!isSelectable) return;
		if (g == this.gameObject) {
			if(Constants.DEBUG) print ("i am selected");
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
