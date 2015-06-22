using UnityEngine;
using System.Collections;

public class TileMapMouse : MonoBehaviour {

	public Vector3 position = Vector3.zero;

	void Update() {
		Ray screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		RaycastHit hit;
		if (Physics.Raycast(screenRay, out hit))
		{
			if(hit.collider.gameObject.tag == "Map") {
				Vector3 pos = hit.collider.gameObject.transform.position;
				position = new Vector3(Mathf.FloorToInt(pos.x),
				                       Mathf.FloorToInt(pos.y),
				                       Mathf.FloorToInt(pos.z));
				Debug.Log ("map tile is " + position);
			}
		}

		if(Input.GetMouseButton(0)) {
			Debug.Log ("clicked on " + TileMapManager.Instance.GetPosition());
		}
	}

}
