using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ReplayManager : MonoBehaviour {

	public GameObject chicken;
	public GridOverlay gridOverlay;
	public float moveSpeed;
	public float yOffset;

	private GameObject[] chickens = new GameObject[2];
	private Animator[] animators = new Animator[2];
	
	private Vector3[] pos = new Vector3[2];
	private Vector3[] posOld = new Vector3[2];
	private float[] posDistance = {0f, 0f};

	private float[] startTime = {0f,0f};
	private bool[] isAnimating = {false, false};
	private float dist;

	//tile 0 0: (-2.5, 0, -1.5)
	// tile 9 9: (2.5, 0, 3.5)

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

		for(int i = 0; i < 2; i++) {
			startTime[i] = Time.time;
		}
	}

	void Update() {
		for(int i = 0; i < 2; i++) {
			chickens[i].transform.LookAt(chickens[Mathf.Abs(i-1)].transform.position);
			animators[i].SetFloat("Rotation Y",chickens[i].transform.rotation.eulerAngles.y);
			if(chickens[i] != null && chickens[i].transform.position != pos[i]) {
				isAnimating[i] = true;
				Vector3 posPrev = chickens[i].transform.position;
				chickens[i].transform.position = Vector3.Lerp(posOld[i], pos[i], ((Time.time - startTime[i]) * moveSpeed));
				//print ("pos old " + i + " is: " + posOld[i] + ", pos " + i + " is: " + pos[i] + ", start time is " + startTime[i] + ", distance is: " + posDistance[i]);
				if(animators[i] != null) {
					animators[i].SetFloat("Velocity X",(chickens[i].transform.position.x - posPrev.x) * 10);
					animators[i].SetFloat("Velocity Z",(chickens[i].transform.position.z - posPrev.z) * 10);
					animators[i].SetFloat("Velocity Angle", Mathf.Atan2(animators[i].GetFloat("Velocity X"), animators[i].GetFloat("Velocity Z")) * Mathf.Rad2Deg);
				}
			}
			else {
				startTime[i] = Time.time;
				if(animators[i] != null) {
					animators[i].SetFloat("Velocity X",0f);
					animators[i].SetFloat("Velocity Z",0f);
					animators[i].SetFloat("Velocity Angle", Mathf.Atan2(animators[i].GetFloat("Velocity X"), animators[i].GetFloat("Velocity Z")) * Mathf.Rad2Deg);
				}
				isAnimating[i] = false;
			}
		}
	}

	public IEnumerator PlayReplay(IDictionary<string,object> replay) {
		chickens[0] = (GameObject) Instantiate(chicken, pos[0], Quaternion.identity);
		chickens[1] = (GameObject) Instantiate(chicken, pos[1], Quaternion.identity);
		for(int i = 0; i < 2; i++) {
			animators[i] = chickens[i].GetComponent<Animator>();
			posOld[i] = pos[i];
		}

		bool isImmediate = true;
		List<IDictionary<string,object>> moves = (replay [Constants.DB_KEYWORD_REPLAY] as Newtonsoft.Json.Linq.JArray).ToObject<List<IDictionary<string,object>>> ();
		foreach(IDictionary<string,object> id in moves) {
			//Utility.PrintDictionary(id);
			//print ("----- NEWLINE -----");
			if(moves.IndexOf(id) != 0) {
				isImmediate = false;
			}
			UpdateDistance(new Vector3(float.Parse (id[Constants.REPLAY_X1].ToString()),0,float.Parse (id[Constants.REPLAY_Y1].ToString())), 0, isImmediate);
			UpdateDistance(new Vector3(float.Parse (id[Constants.REPLAY_X2].ToString()),0,float.Parse (id[Constants.REPLAY_Y2].ToString())), 1, isImmediate);
			while(isAnimating[0] && isAnimating[1]) {
				yield return new WaitForSeconds(0.2f);
			}
		}
		print ("end");

		yield break;
	}

	private void UpdateDistance(Vector3 newPos, int i, bool isImmediate) {
		if(isImmediate) {
			chickens[i].transform.position = TransposeToBoard(newPos);
			chickens[i].transform.LookAt(chickens[Mathf.Abs(i-1)].transform.position);
		}
		else {
			posOld[i] = chickens[i].transform.position;
			pos[i] = TransposeToBoard(newPos);
			posDistance[i] = Vector3.Distance(pos[i], posOld[i]);
			isAnimating[i] = true;
		}
	}

	private Vector3 TransposeToBoard(Vector3 pos) {
		print (pos + " transposed to " + gridOverlay.GetTiles () [(int)pos.x, (int)pos.z].transform.position);
		return gridOverlay.GetTiles()[(int)pos.x,(int)pos.z].transform.position;
	}
}
