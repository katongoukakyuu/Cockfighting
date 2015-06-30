using UnityEngine;
using UnityEngine.EventSystems;
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
	
	public delegate void OnMouseClickEvent(GameObject g);
	public event OnMouseClickEvent OnMouseClick;

	void Update() {
		if(Input.GetMouseButtonDown(0)) {
			Ray screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			RaycastHit hit;
			if (Physics.Raycast(screenRay, out hit))
			{
				OnMouseClick(hit.collider.transform.root.gameObject);
				OnMouseClick(hit.collider.transform.gameObject);
			}
		}
	}

}
