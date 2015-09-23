using UnityEngine;
using System.Collections;

public class CameraRotate : MonoBehaviour {

	public Vector3 rotatePoint = Vector3.zero;
	public float rotateSpeed = 5.0f;
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(rotatePoint, Vector3.up, rotateSpeed * Time.deltaTime);
	}
}
