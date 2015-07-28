using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FightManager : MonoBehaviour {

	public Transform chicken1;
	public Transform chicken2;

	private List<string> name = new List<string>();
	private int[] atk = new int[2], def = new int[2], hp = new int[2], agi = new int[2], gam = new int[2], agg = new int[2];
	private Vector3[] pos = new Vector3[2];
	private float dist;

	private static FightManager instance;
	private FightManager() {}

	public static FightManager Instance {
		get {
			if(instance == null) {
				instance = (FightManager)GameObject.FindObjectOfType(typeof(FightManager));
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

	public void AutomateFight(IDictionary<string,object> c1,
	                            IDictionary<string,object> c2,
	                            List<IDictionary<string, object>> c1Moves,
	                            List<IDictionary<string, object>> c2Moves) {
		atk[0] = int.Parse (c1[Constants.DB_KEYWORD_ATTACK].ToString());
		def[0] = int.Parse (c1[Constants.DB_KEYWORD_DEFENSE].ToString());
		hp[0] = int.Parse (c1[Constants.DB_KEYWORD_HP].ToString());
		agi[0] = int.Parse (c1[Constants.DB_KEYWORD_AGILITY].ToString());
		gam[0] = int.Parse (c1[Constants.DB_KEYWORD_GAMENESS].ToString());
		agg[0] = int.Parse (c1[Constants.DB_KEYWORD_AGGRESSION].ToString());

		atk[1] = int.Parse (c2[Constants.DB_KEYWORD_ATTACK].ToString());
		def[1] = int.Parse (c2[Constants.DB_KEYWORD_DEFENSE].ToString());
		hp[1] = int.Parse (c2[Constants.DB_KEYWORD_HP].ToString());
		agi[1] = int.Parse (c2[Constants.DB_KEYWORD_AGILITY].ToString());
		gam[1] = int.Parse (c2[Constants.DB_KEYWORD_GAMENESS].ToString());
		agg[1] = int.Parse (c2[Constants.DB_KEYWORD_AGGRESSION].ToString());

		pos[0] = chicken1.position;
		pos[1] = chicken2.position;
		UpdateDistance();
		print (pos[0]);
		print (pos[1]);
		
		string move = AssignNextMove (c1Moves, 0);
		ProcessMove(move, 0);
		/*
		while(hp1 > 0 && hp2 > 0) {

		}
		*/
	}

	private string AssignNextMove(List<IDictionary<string, object>> moves, int playerNum) {
		float[] movePercent = new float[] {0f, 0f, 0f, 0f};
		float[] moveStrength = new float[] {0f, 0f, 0f, 0f};
		float moveStrengthTotal = 0;
		
		foreach (IDictionary<string,object> id in moves) {
			name.Add (DatabaseManager.Instance.LoadFightingMove (
				id[Constants.DB_KEYWORD_FIGHTING_MOVE_ID].ToString()
			) [Constants.DB_KEYWORD_NAME].ToString ());
			moveStrength[moves.IndexOf(id)] = AnalyzeMoveStrength(name[moves.IndexOf(id)], 1);
			moveStrengthTotal += moveStrength[moves.IndexOf(id)];
			print ("move strength of " + name[moves.IndexOf(id)] + " is " + 
			       moveStrength[moves.IndexOf(id)]);
		}

		if(moveStrengthTotal == 0) return null;
		float r = Random.Range(0.0f, 1.0f);
		print ("randomizer is " + r);
		for(int x = 0; x < movePercent.Length; x++) {
			movePercent[x] = moveStrength[x] / moveStrengthTotal;
			print ("move percent of " + name[x] + " is " + movePercent[x]);
			if(r > (1.0f - movePercent[x])) {
				return name[x];
			}
		}
		return null;
	}

	private float AnalyzeMoveStrength(string name, int playerNum) {
		float strength = 0f;
		float prefDist = 0f;
		switch (name) {
		case Constants.FIGHT_MOVE_DASH:
			prefDist = Mathf.Pow (5f,2);
			if(dist > prefDist) {
				strength += (dist);
			}
			return strength;
		case Constants.FIGHT_MOVE_FLYING_TALON:
			prefDist = Mathf.Pow (3f,2);
			if(dist < prefDist) {
				strength += 5f;
			}
			return strength;
		case Constants.FIGHT_MOVE_SIDESTEP:
			prefDist = Mathf.Pow (5f,2);
			if(dist < prefDist) {
				strength += 2.5f;
			}
			return strength;
		case Constants.FIGHT_MOVE_PECK:
			prefDist = Mathf.Pow (1.5f,2);
			if(dist < prefDist) {
				strength += 10f;
			}
			return strength;
		default:
			return strength;
		}
	}

	private void ProcessMove(string name, int pN) {
		print ("move used is " + name);
		int eN = Mathf.Abs(pN-1);
		switch (name) {
		case Constants.FIGHT_MOVE_DASH:
			pos[pN] = Vector3.MoveTowards(pos[pN],pos[eN],5.0f);
			UpdateDistance ();
			break;
		case Constants.FIGHT_MOVE_FLYING_TALON:
			pos[pN] = Vector3.MoveTowards(pos[pN],pos[eN],2.0f);
			UpdateDistance ();
			hp[eN] -= (int)(atk[pN] * 1.1f);
			print ("updated hp is " + hp[eN]);
			break;
		case Constants.FIGHT_MOVE_SIDESTEP:
			print ("Sidestep!");
			break;
		case Constants.FIGHT_MOVE_PECK:
			hp[eN] -= (int)(atk[pN] * 0.4f);
			print ("updated hp is " + hp[eN]);
			break;
		default:
			break;
		}
	}

	private void UpdateDistance() {
		dist = Vector3.Distance(pos[0], pos[1]);
		print ("Distance of 2 chickens between each other is " + dist);
	}
}
