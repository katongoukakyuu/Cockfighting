using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ReplayManager : MonoBehaviour {

	public GameObject chicken;
	public GameObject hitParticle;
	public GridOverlay gridOverlay;
	public float moveSpeed;
	public float yOffset;

	public GameObject topPanel;
	public Color healthColorFull;
	public Color healthColorEmpty;

	private GameObject[] chickens = new GameObject[2];
	private Animator[] animators = new Animator[2];
	private ChickenFightAI[] agents = new ChickenFightAI[2];
	private NavMeshAgent[] navMeshAgents = new NavMeshAgent[2];

	private GameObject[] hitParticles = new GameObject[2];
	private ParticleSystem[] particleSystems = new ParticleSystem[2];

	private IDictionary<string,object> currentRoundInfo;
	private int[] hpMax = new int[2];
	private List<int>[] hpQueue = new List<int>[2];
	private List<bool>[] hpQueueChangedByMove = new List<bool>[2];
	private bool[] hpQueueWaitingForMoveProc = new bool[2] {false, false};
	
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
			hpQueue[i] = new List<int>();
			hpQueueChangedByMove[i] = new List<bool>();
		}
		
		if(PlayerManager.Instance != null &&
		   PlayerManager.Instance.selectedReplay != null) {
			StartCoroutine(PlayReplay(PlayerManager.Instance.selectedReplay));
		}

		// DEBUG FOR QUICK MATCH
		/*ServerFightManager.Instance.AutomateFight (
			DatabaseManager.Instance.LoadChicken("Gary", "test"),
			DatabaseManager.Instance.LoadChicken("Larry", "test2"),
			DatabaseManager.Instance.LoadFightingMovesOwned (DatabaseManager.Instance.LoadChicken("Gary", "test")[Constants.DB_COUCHBASE_ID].ToString()),
			DatabaseManager.Instance.LoadFightingMovesOwned (DatabaseManager.Instance.LoadChicken("Larry", "test2")[Constants.DB_COUCHBASE_ID].ToString())
		);*/
		// END DEBUG FOR QUICK MATCH
	}

	void FixedUpdate() {

		for(int i = 0; i < 2; i++) {
			if(!agents[i].hasFainted) {
				chickens[i].transform.LookAt(chickens[Mathf.Abs(i-1)].transform.position);
				animators[i].SetFloat("Rotation Y",chickens[i].transform.rotation.eulerAngles.y);
			}
			if(chickens[i] != null && chickens[i].transform.position != pos[i]) {
				//isAnimating[i] = true;
				Vector3 posPrev = chickens[i].transform.position;
				/*chickens[i].transform.position = Vector3.Lerp(posOld[i], 
				                                              pos[i], 
				                                              ((Time.time - startTime[i]) * moveSpeed));*/
				if(Constants.DEBUG) print ("pos old " + i + " is: " + posOld[i] + ", pos " + i + " is: " + pos[i] + ", start time is " + startTime[i] + ", distance is: " + posDistance[i]);
				if(animators[i] != null) {
					animators[i].SetFloat("Velocity X",(chickens[i].transform.position.x - posPrev.x) * 10);
					animators[i].SetFloat("Velocity Z",(chickens[i].transform.position.z - posPrev.z) * 10);
					animators[i].SetFloat("Velocity Angle", Mathf.Atan2(animators[i].GetFloat("Velocity X"), animators[i].GetFloat("Velocity Z")) * Mathf.Rad2Deg);
				}
			}
			else {
				//startTime[i] = Time.time;
				if(animators[i] != null) {
					animators[i].SetFloat("Velocity X",0f);
					animators[i].SetFloat("Velocity Z",0f);
					animators[i].SetFloat("Velocity Angle", Mathf.Atan2(animators[i].GetFloat("Velocity X"), animators[i].GetFloat("Velocity Z")) * Mathf.Rad2Deg);
				}
				//isAnimating[i] = false;
			}
		}
	}

	public IEnumerator PlayReplay(IDictionary<string,object> replay) {
		for(int i = 0; i < 2; i++) {
			chickens[i] = (GameObject) Instantiate(chicken);
			chickens[i].name = "Chicken " + i;
			chickens[i].GetComponent<ChickenAI>().enabled = false;
			animators[i] = chickens[i].GetComponent<Animator>();
			agents[i] = chickens[i].GetComponent<ChickenFightAI>();
			agents[i].enabled = true;
			agents[i].index = i;
			navMeshAgents[i] = chickens[i].GetComponent<NavMeshAgent>();
			posOld[i] = pos[i];

			hitParticles[i] = (GameObject) Instantiate(hitParticle);
			particleSystems[i] = hitParticles[i].GetComponent<ParticleSystem>();
		}



		bool isImmediate = true;

		topPanel.transform.FindChild(Constants.FIGHT_RING_UI_CHICKEN_1).GetComponent<Text>().text = 
			DatabaseManager.Instance.LoadChicken(replay[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString())[Constants.DB_KEYWORD_NAME].ToString();
		topPanel.transform.FindChild(Constants.FIGHT_RING_UI_CHICKEN_2).GetComponent<Text>().text = 
			DatabaseManager.Instance.LoadChicken(replay[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString())[Constants.DB_KEYWORD_NAME].ToString();

		List<IDictionary<string,object>> moves = (replay [Constants.DB_KEYWORD_REPLAY] as Newtonsoft.Json.Linq.JArray).ToObject<List<IDictionary<string,object>>> ();
		foreach(IDictionary<string,object> id in moves) {
			//Utility.PrintDictionary(id);
			//if(Constants.DEBUG) print ("----- NEWLINE -----");
			currentRoundInfo = id;

			if(moves.IndexOf(id) != 0) {
				isImmediate = false;
			}
			else {
				topPanel.transform.FindChild(Constants.FIGHT_RING_UI_HP_1_SLIDER).GetComponent<Slider>().maxValue = int.Parse(id[Constants.REPLAY_HP1].ToString());
				topPanel.transform.FindChild(Constants.FIGHT_RING_UI_HP_2_SLIDER).GetComponent<Slider>().maxValue = int.Parse(id[Constants.REPLAY_HP2].ToString());
				topPanel.transform.FindChild(Constants.FIGHT_RING_UI_HP_1_SLIDER).GetComponent<Slider>().value = int.Parse(id[Constants.REPLAY_HP1].ToString());
				topPanel.transform.FindChild(Constants.FIGHT_RING_UI_HP_2_SLIDER).GetComponent<Slider>().value = int.Parse(id[Constants.REPLAY_HP2].ToString());
				hpMax[0] = int.Parse(id[Constants.REPLAY_HP1].ToString());
				hpMax[1] = int.Parse(id[Constants.REPLAY_HP2].ToString());
			}

			for(int i = 0; i < 2; i++) {
				int hp = (i == 0) ? int.Parse(id[Constants.REPLAY_HP1].ToString()) : int.Parse(id[Constants.REPLAY_HP2].ToString());
				hpQueue[i].Add(hp);
				hpQueueChangedByMove[i].Add(false);
				if(isImmediate) {
					UpdateUI(i, false, false);
				}
				int[] newDest = new int[2];
				newDest[0] = (i == 0) ? int.Parse (id[Constants.REPLAY_X1].ToString()) : int.Parse (id[Constants.REPLAY_X2].ToString());
				newDest[1] = (i == 0) ? int.Parse (id[Constants.REPLAY_Y1].ToString()) : int.Parse (id[Constants.REPLAY_Y2].ToString());

				agents[i].moveImmediately = isImmediate;
				agents[i].newDestination = newDest;
			}

			do {
				yield return new WaitForSeconds(0.5f);
			} while(agents[0].isCurrentlyMoving || agents[1].isCurrentlyMoving);

			for(int i = 0; i < 2; i++) {
				int hp = (i == 0) ? int.Parse(id[Constants.REPLAY_HP1].ToString()) : int.Parse(id[Constants.REPLAY_HP2].ToString());
				hpQueue[i].Add(hp);
				if(hp <= 0) {
					animators[i].SetTrigger("Faint");
					hpQueue[i].Clear();
					hpQueueChangedByMove[i].Clear();
					hpQueue[i].Add(hp);
					hpQueueChangedByMove[i].Add(false);
				}
				else {
					string move = (i == 0) ? id[Constants.REPLAY_MOVE1].ToString() : id[Constants.REPLAY_MOVE2].ToString();
					SetMoveAnimTrigger(i, move);
					if(Constants.DEBUG) print ("move for " + i + " is " + move + ", hp to display is " + hpQueue[i][0]);
				}
				UpdateUI(i, false, false);
			}
		}
		if(Constants.DEBUG) print ("end");

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

	private void SetMoveAnimTrigger(int index, string move) {
		if(animators[index] != null) {
			switch (move) {
			case Constants.FIGHT_MOVE_FLYING_TALON:
				if(Constants.DEBUG) print ("chicken " + index + " is now attacking!");
				animators[index].SetTrigger(move);
				agents[index].isAttacking = true;
				navMeshAgents[index].SetDestination(
					Vector3.Lerp(chickens[index].transform.position,
				             chickens[(index+1)%2].transform.position,
				             0.7f)
				);
				hpQueueChangedByMove[index].Add(true);

				hitParticles[(index+1)%2].transform.position = chickens[(index+1)%2].transform.position;
				particleSystems[(index+1)%2].Play();
				break;
			case Constants.FIGHT_MOVE_PECK:
				if(Constants.DEBUG) print ("chicken " + index + " is now attacking!");
				animators[index].SetTrigger(move);
				agents[index].isAttacking = true;
				navMeshAgents[index].SetDestination(
					Vector3.Lerp(chickens[index].transform.position,
				             chickens[(index+1)%2].transform.position,
				             0.7f)
				);
				hpQueueChangedByMove[index].Add(true);

				hitParticles[(index+1)%2].transform.position = chickens[(index+1)%2].transform.position;
				particleSystems[(index+1)%2].Play();
				break;
			default:
				hpQueueChangedByMove[index].Add(false);
				break;
			}
		}
	}

	private Vector3 TransposeToBoard(Vector3 pos) {
		// if(Constants.DEBUG) print (pos + " transposed to " + gridOverlay.GetTiles () [(int)pos.x, (int)pos.z].transform.position);
		return gridOverlay.GetTiles()[(int)pos.x,(int)pos.z].transform.position;
	}

	public void ButtonBack() {
		Application.LoadLevel(Constants.SCENE_FARM);
	}

	public void UpdateUI(int i, bool isImmediate, bool calledFromMoveProc) {
		/*if(Constants.DEBUG) print ("Update UI called (immediate? " + isImmediate + ", called from proc? " + calledFromMoveProc + "). HP list for chicken " + i + ":");
		string s = "HP:";
		foreach(int hp in hpQueue[i]) {
			s += " " + hp;
		}
		if(Constants.DEBUG) print (s);
		s = "To proc by move?:";
		foreach(bool b in hpQueueChangedByMove[i]) {
			s += " " + b;
		}
		if(Constants.DEBUG) print (s);*/

		if(Constants.DEBUG) print (i + " queue count: " + hpQueue[i].Count + ", change queue count: " + hpQueueChangedByMove[i].Count);
		if(hpQueue[i].Count > 0 && hpQueueChangedByMove[i].Count > 0) {
			if(hpQueueWaitingForMoveProc[i] && calledFromMoveProc) {
				if(Constants.DEBUG) print (i + " has been called by a move proc!");
				hpQueueWaitingForMoveProc[i] = false;
			}
			else if(hpQueueChangedByMove[i][0]) {
				hpQueueWaitingForMoveProc[i] = true;
				if(Constants.DEBUG) print (i + " is now waiting for a move proc");
				return;
			}

			int currentHP = hpQueue[i][0];
			if(!isImmediate) {
				hpQueue[i].RemoveAt(0);
				hpQueueChangedByMove[i].RemoveAt(0);
			}
			Color c = Color.Lerp (healthColorEmpty,healthColorFull,currentHP/hpMax[i]);
			if(i == 0) {
				topPanel.transform.FindChild(Constants.FIGHT_RING_UI_HP_1_FILL_SLIDER).GetComponent<Image>().color = c;
				topPanel.transform.FindChild(Constants.FIGHT_RING_UI_HP_1_SLIDER).GetComponent<Slider>().value = currentHP;
			}
			else {
				topPanel.transform.FindChild(Constants.FIGHT_RING_UI_HP_2_FILL_SLIDER).GetComponent<Image>().color = c;
				topPanel.transform.FindChild(Constants.FIGHT_RING_UI_HP_2_SLIDER).GetComponent<Slider>().value = currentHP;
			}
		}
	}

	public GameObject[] GetChickens() {
		return chickens;
	}
}
