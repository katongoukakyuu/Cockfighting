using UnityEngine;
using System.Collections;

public class TouchCameraControl : MonoBehaviour 
{

	private Vector2 worldStartPoint;
	
	void Update () {
		
		// only work with one touch
		if (Input.touchCount == 1) {
			Touch currentTouch = Input.GetTouch(0);
			
			if (currentTouch.phase == TouchPhase.Began) {
				this.worldStartPoint = this.getWorldPoint(currentTouch.position);
			}
			
			if (currentTouch.phase == TouchPhase.Moved) {
				Vector2 worldDelta = this.getWorldPoint(currentTouch.position) - this.worldStartPoint;
				
				Camera.main.transform.Translate(
					-worldDelta.x,
					-worldDelta.y,
					0
					);
			}
		}
	}
	
	// convert screen point to world point
	private Vector2 getWorldPoint (Vector2 screenPoint) {
		RaycastHit hit;
		Physics.Raycast(Camera.main.ScreenPointToRay(screenPoint), out hit);
		return hit.point;
	}
}