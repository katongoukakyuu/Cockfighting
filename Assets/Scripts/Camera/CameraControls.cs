using UnityEngine;
using System.Collections;

public class CameraControls: MonoBehaviour {

	public Camera[] cameras;

	public float dragSpeed = 0.1f;
	public float perspectiveZoomSpeed = 0.5f;
	public float orthoZoomSpeed = 0.5f;
	public float keyboardMoveSpeed = 5f;
	public float positionOffsetTop = -1f;
	public float positionOffsetLeft = 0f;
	public float positionOffsetRight = 0f;
	public float positionOffsetBot = 1f;

	public bool freeCamera = true;
	public GridOverlay gridOverlay;

	private Vector2[] boardCorners = new Vector2[2];

	private static CameraControls instance;
	private CameraControls() {}
	
	public static CameraControls Instance {
		get {
			if (instance == null) {
				instance = (CameraControls)GameObject.FindObjectOfType (typeof(CameraControls));
			}
			return instance;
		}
	}

	void Start() {
		boardCorners = gridOverlay.GetBoardCorners();
	}

	void Update() {
		if (!freeCamera)
			return;
		if(Input.GetKey(KeyCode.W)) {
			Camera.main.transform.position += new Vector3(0,
			                                              0,
			                                              keyboardMoveSpeed) * dragSpeed * Time.deltaTime;
		}
		if(Input.GetKey(KeyCode.S)) {
			Camera.main.transform.position += new Vector3(0,
			                                              0,
			                                              -keyboardMoveSpeed) * dragSpeed * Time.deltaTime;
		}
		if(Input.GetKey(KeyCode.A)) {
			Camera.main.transform.position += new Vector3(-keyboardMoveSpeed,
			                                              0,
			                                              0) * dragSpeed * Time.deltaTime;
		}
		if(Input.GetKey(KeyCode.D)) {
			Camera.main.transform.position += new Vector3(keyboardMoveSpeed,
			                                              0,
			                                              0) * dragSpeed * Time.deltaTime;
		}
		if(Input.touchCount == 1) {
			Touch touch = Input.GetTouch (0);

			Vector2 touchPrevPos = touch.position - touch.deltaPosition;
			Vector2 touchDelta = touchPrevPos - touch.position;
			
			Camera.main.transform.position += new Vector3(touchDelta.x,
			                                              0,
			                                              touchDelta.y) * dragSpeed * Time.deltaTime;
		}
		/*else if(Input.touchCount == 2) {
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
		}*/
		
		if(Camera.main.transform.position.x < boardCorners[0].x - positionOffsetLeft) {
			Camera.main.transform.position = new Vector3(boardCorners[0].x - positionOffsetLeft,
			                                   Camera.main.transform.position.y,
			                                   Camera.main.transform.position.z);
		}
		if(Camera.main.transform.position.z < boardCorners[0].y - positionOffsetBot) {
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
			                                   Camera.main.transform.position.y,
			                                   boardCorners[0].y - positionOffsetBot);
		}
		if(Camera.main.transform.position.x > boardCorners[1].x + positionOffsetRight) {
			Camera.main.transform.position = new Vector3(boardCorners[1].x + positionOffsetRight,
			                                   Camera.main.transform.position.y,
			                                   Camera.main.transform.position.z);
		}
		if(Camera.main.transform.position.z > boardCorners[1].y + positionOffsetTop) {
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
			                                   Camera.main.transform.position.y,
			                                   boardCorners[1].y + positionOffsetTop);
		}
	}

	public void SwitchToCamera(int i) {
		if(i >= cameras.Length)	return;

		for(int x = 0; x < cameras.Length; x++) {
			cameras[x].enabled = false;
		}

		cameras[i].enabled = true;
	}

}
