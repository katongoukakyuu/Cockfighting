using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ReplayManager : MonoBehaviour {

	public GameObject chicken;

	private GameObject[] chickens = new GameObject[2];
	private Vector3[] pos = new Vector3[2];
	private float dist;
	private bool isAnimating = false;

	private static ReplayManager instance;
	private ReplayManager() {}

	public static ReplayManager Instance {
		get {
			if(instance == null) {
				instance = (ReplayManager)GameObject.FindObjectOfType(typeof(ReplayManager));
			}
			return instance;
		}
	}

	void Start() {
		DontDestroyOnLoad(this);
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
			return;
		}
	}

	void Update() {
		if(chickens[0] != null && chickens[0].transform.position != pos[0]) {
			isAnimating = true;
			chickens[0].transform.position = Vector3.Lerp(chickens[0].transform.position, pos[0], Time.deltaTime);
		}
		else {
			isAnimating = false;
		}
		if(chickens[1] != null && chickens[1].transform.position != pos[1]) {
			isAnimating = false;
			chickens[1].transform.position = Vector3.Lerp(chickens[1].transform.position, pos[1], Time.deltaTime);
		}
		else {
			isAnimating = false;
		}
	}

	public IEnumerator PlayReplay(IDictionary<string,object> replay) {


		chickens[0] = (GameObject) Instantiate(chicken, pos[0], Quaternion.identity);
		chickens[1] = (GameObject) Instantiate(chicken, pos[1], Quaternion.Euler(0, 180, 0));
		
		yield break;
	}
}
