using UnityEngine;
using System.Collections;

public class TileMapMouse : MonoBehaviour {

	public Vector3 position = Vector3.zero;

	public delegate void OnTileHoverChangeEvent();
	public event OnTileHoverChangeEvent OnTileHoverChange;

	private static TileMapMouse instance;
	private TileMapMouse() {}
	
	public static TileMapMouse Instance {
		get {
			if(instance == null) {
				instance = (TileMapMouse)GameObject.FindObjectOfType(typeof(TileMapMouse));
			}
			return instance;
		}
	}

	void Update() {
		Ray screenRay;
		if(Input.touchCount > 0) {
			screenRay = Camera.main.ScreenPointToRay(new Vector3(Input.GetTouch (0).position.x,
			                                                     Input.GetTouch (0).position.y,
			                                                     0));
		}
		else {
			screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		}
		
		RaycastHit hit;
		if (Physics.Raycast(screenRay, out hit))
		{
			if(hit.collider.gameObject.tag == "Map") {
				Vector3 pos = hit.collider.gameObject.transform.position;
				Vector3 newPos = new Vector3(Mathf.FloorToInt(pos.x),
				                       Mathf.FloorToInt(pos.y),
				                       Mathf.FloorToInt(pos.z));
				if(position != newPos) {
					position = newPos;
					OnTileHoverChange();
					Debug.Log ("map tile is " + position);
				}
			}
		}

		if(Input.GetMouseButton(0)) {
			Debug.Log ("clicked on " + TileMapManager.Instance.GetPosition());
		}
	}

}
