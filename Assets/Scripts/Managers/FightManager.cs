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

		List<IDictionary<string,object>> l = new List<IDictionary<string,object>> ();
		string[] move = new string[2];
		move[0] = AssignNextMove (null, 0);
		move[1] = AssignNextMove (null, 1);
		l.Add (RecordTurn(move));

		while(hp[0] > 0 && hp[1] > 0) {
			int pN, eN;
			if (agi[0] > agi[1]) pN = 0;
			else pN = 1;
			eN = Mathf.Abs(pN - 1);
			move[pN] = AssignNextMove (c1Moves, pN);
			ProcessMove(move[pN], pN);
			move[eN] = AssignNextMove (c1Moves, eN);
			ProcessMove(move[eN], eN);
			l.Add (RecordTurn(move));
		}

		Dictionary<string, object> d = GameManager.Instance.GenerateReplay (
			new string[] {c1[Constants.DB_COUCHBASE_ID].ToString(), c2[Constants.DB_COUCHBASE_ID].ToString()},
			l
		);
		var savedReplay = DatabaseManager.Instance.SaveReplay (d);

		var loadedReplay = DatabaseManager.Instance.LoadReplay (savedReplay [Constants.DB_COUCHBASE_ID].ToString ());
		foreach(KeyValuePair<string,object> kv in loadedReplay) {
			print (kv.Key + ": " + kv.Value);
		}
		/*foreach (IDictionary<string,object> id in l) {
			foreach(KeyValuePair<string,object> kv in id) {
				print (kv.Key + ": " + kv.Value);
			}
		}
		print ("replay list has " + l.Count () + " elements.");*/
	}

	private string AssignNextMove(List<IDictionary<string, object>> moves, int playerNum) {
		float[] movePercent = new float[] {0f, 0f, 0f, 0f};
		float[] moveStrength = new float[] {0f, 0f, 0f, 0f};
		float moveStrengthTotal = 0;

		if (hp [playerNum] <= 0 || moves == null) {
			return Constants.FIGHT_MOVE_NONE;
		}
		
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
		float lowLim;
		float hiLim = 0.0f;
		print ("randomizer is " + r);
		for (int i = 0; i < movePercent.Length; i++)
		{
			movePercent[i] = moveStrength[i] / moveStrengthTotal;
			print ("move percent of " + name[i] + " is " + movePercent[i]);
			lowLim = hiLim;
			hiLim += movePercent[i];
			if (r >= lowLim && r < hiLim) {
				return name[i];
			}
		}
		return null;
	}

	private float AnalyzeMoveStrength(string name, int playerNum) {
		float strength = 0f;
		float prefDist = 0f;
		switch (name) {
		case Constants.FIGHT_MOVE_DASH:
			prefDist = Mathf.Pow (3f,2);
			if(dist > prefDist) {
				strength += (dist);
			}
			return strength;
		case Constants.FIGHT_MOVE_FLYING_TALON:
			prefDist = Mathf.Pow (5f,2);
			if(dist < prefDist) {
				strength += 5f;
			}
			return strength;
		case Constants.FIGHT_MOVE_SIDESTEP:
			prefDist = Mathf.Pow (7f,2);
			if(dist < prefDist) {
				strength += 2.5f;
			}
			return strength;
		case Constants.FIGHT_MOVE_PECK:
			prefDist = Mathf.Pow (3f,2);
			if(dist < prefDist) {
				strength += 10f;
			}
			return strength;
		default:
			return strength;
		}
	}

	private void ProcessMove(string name, int pN) {
		print ("move used by chicken " + pN + " is " + name);
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
			print ("updated hp of chicken " + eN + " is " + hp[eN]);
			break;
		case Constants.FIGHT_MOVE_SIDESTEP:
			print ("Sidestep!");
			break;
		case Constants.FIGHT_MOVE_PECK:
			hp[eN] -= (int)(atk[pN] * 0.4f);
			print ("updated hp of chicken " + eN + " is " + hp[eN]);
			break;
		default:
			break;
		}
	}

	private void UpdateDistance() {
		dist = Vector3.Distance(pos[0], pos[1]);
		print ("Distance of 2 chickens between each other is " + dist);
	}

	private Dictionary<string, object> RecordTurn(string[] move) {
		Dictionary<string, object> d = new Dictionary<string, object>() {
			{Constants.DB_KEYWORD_TYPE, Constants.DB_TYPE_REPLAY_TURN},
			{Constants.REPLAY_ATK1, atk[0]},
			{Constants.REPLAY_DEF1, def[0]},
			{Constants.REPLAY_HP1, hp[0]},
			{Constants.REPLAY_AGI1, agi[0]},
			{Constants.REPLAY_GAM1, gam[0]},
			{Constants.REPLAY_AGG1, agg[0]},
			{Constants.REPLAY_X1, pos[0].x},
			{Constants.REPLAY_Y1, pos[0].y},
			{Constants.REPLAY_MOVE1, move[0]},
			{Constants.REPLAY_ATK2, atk[1]},
			{Constants.REPLAY_DEF2, def[1]},
			{Constants.REPLAY_HP2, hp[1]},
			{Constants.REPLAY_AGI2, agi[1]},
			{Constants.REPLAY_GAM2, gam[1]},
			{Constants.REPLAY_AGG2, agg[1]},
			{Constants.REPLAY_X2, pos[1].x},
			{Constants.REPLAY_Y2, pos[1].y},
			{Constants.REPLAY_MOVE2, move[1]}
		};
		return d;
	}
}
