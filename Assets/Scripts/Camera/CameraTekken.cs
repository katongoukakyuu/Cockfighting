using UnityEngine;
using System.Collections;

public class CameraTekken: MonoBehaviour {

	GameObject[] targets = new GameObject[2];
	Vector3 midpoint;
	Vector3 separation;
	Vector3 perpendicularLine;
	bool isInitialized = false;
	
	// Update is called once per frame
	void Update () {
		if(!isInitialized)	return;

		midpoint = Vector3.Lerp(targets[0].transform.position, targets[1].transform.position, 0.5f) + new Vector3(0, 0.4f, 0);
		transform.LookAt(midpoint);

		separation = targets[0].transform.position - targets[1].transform.position;
		perpendicularLine = Vector3.Cross(separation, Vector3.up).normalized;

		transform.position = perpendicularLine * (2f * separation.magnitude) + new Vector3(0, 0.4f, 0);
	}

	public void Initialize(GameObject g1, GameObject g2) {
		targets[0] = g1;
		targets[1] = g2;

		isInitialized = true;
	}
}
