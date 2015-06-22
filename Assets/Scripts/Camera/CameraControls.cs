using UnityEngine;
using System.Collections;

public class CameraControls: MonoBehaviour {

	public float dragSpeed = 0.1f;
	public float perspectiveZoomSpeed = 0.5f;
	public float orthoZoomSpeed = 0.5f;

	void Update() {
		if(Input.touchCount == 1) {
			Touch touch = Input.GetTouch (0);

			Vector2 touchPrevPos = touch.position - touch.deltaPosition;
			Vector2 touchDelta = touchPrevPos - touch.position;
			
			Camera.main.transform.position += new Vector3(touchDelta.x,
			                                              0,
			                                              touchDelta.y) * dragSpeed;
		}
		else if(Input.touchCount == 2) {
			Touch touchZero = Input.GetTouch (0);
			Touch touchOne = Input.GetTouch (1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			if(Camera.main.orthographic) {
				Camera.main.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
				Camera.main.orthographicSize = Mathf.Max (Camera.main.orthographicSize, 0.1f);
			}
			else {
				Camera.main.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
				Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 0.3f, 89.9f);
			}
		}
	}

}
