using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ServerFightManager : MonoBehaviour {

	public bool debug;

	private List<string> moveName = new List<string>();
	private int[] atk = new int[2], def = new int[2], hp = new int[2], agi = new int[2], gam = new int[2], agg = new int[2];
	private Vector2[] pos = new Vector2[2];
	private int dist;

	private Vector2 ringSize = new Vector2(10,6);

	private static ServerFightManager instance;
	private ServerFightManager() {}

	public static ServerFightManager Instance {
		get {
			if(instance == null) {
				instance = (ServerFightManager)GameObject.FindObjectOfType(typeof(ServerFightManager));
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

	public int AutomateFight(IDictionary<string,object> c1,
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
		
		pos[0] = new Vector3(Random.Range(0,(int)ringSize.x), Random.Range (0, (int)ringSize.y));
		pos[1] = new Vector3(Random.Range(0,(int)ringSize.x), Random.Range (0, (int)ringSize.y));
		UpdateDistance();

		List<IDictionary<string,object>> l = new List<IDictionary<string,object>> ();
		string[] move = new string[2];
		move[0] = AssignNextMove (null, 0);
		move[1] = AssignNextMove (null, 1);
		l.Add (RecordTurn(move));

		while(hp[0] > 0 && hp[1] > 0) {
		//for(int i = 0; i < 2; i++) {
			int pN, eN;
			if (agi[0] > agi[1]) pN = 0;
			else pN = 1;
			eN = Mathf.Abs(pN - 1);
			move[pN] = AssignNextMove (c1Moves, pN);
			ProcessMove(move[pN], pN);
			move[eN] = AssignNextMove (c1Moves, eN);
			ProcessMove(move[eN], eN);
			l.Add (RecordTurn(move));
			UpdateDistance ();
		}

		Dictionary<string, object> d = GameManager.Instance.GenerateReplay (
			new string[] {c1[Constants.DB_COUCHBASE_ID].ToString(), c2[Constants.DB_COUCHBASE_ID].ToString()},
			l
		);
		var savedReplay = DatabaseManager.Instance.SaveEntry (d);

		var loadedReplay = DatabaseManager.Instance.LoadReplay (savedReplay [Constants.DB_COUCHBASE_ID].ToString ());
		StartCoroutine(ReplayManager.Instance.PlayReplay(loadedReplay));
		
		if(hp[0] <= 0) return 0;
		else return 1;
	}

	private string AssignNextMove(List<IDictionary<string, object>> moves, int playerNum) {
		float[] movePercent = new float[] {0f, 0f, 0f, 0f};
		float[] moveStrength = new float[] {0f, 0f, 0f, 0f};
		float moveStrengthTotal = 0;

		if (hp [playerNum] <= 0 || moves == null) {
			return Constants.FIGHT_MOVE_NONE;
		}
		
		foreach (IDictionary<string,object> id in moves) {
			moveName.Add (DatabaseManager.Instance.LoadFightingMove (
				id[Constants.DB_KEYWORD_FIGHTING_MOVE_ID].ToString()
			) [Constants.DB_KEYWORD_NAME].ToString ());
			moveStrength[moves.IndexOf(id)] = AnalyzeMoveStrength(moveName[moves.IndexOf(id)], playerNum);
			moveStrengthTotal += moveStrength[moves.IndexOf(id)];
			if(debug) print ("move strength of " + moveName[moves.IndexOf(id)] + " is " + 
			       moveStrength[moves.IndexOf(id)]);
		}

		if(moveStrengthTotal == 0) return null;
		float r = Random.Range(0.0f, 1.0f);
		float lowLim;
		float hiLim = 0.0f;
		if(debug) print ("randomizer is " + r);
		for (int i = 0; i < movePercent.Length; i++)
		{
			movePercent[i] = moveStrength[i] / moveStrengthTotal;
			if(debug) print ("move percent of " + moveName[i] + " is " + movePercent[i]);
			lowLim = hiLim;
			hiLim += movePercent[i];
			if (r >= lowLim && r < hiLim) {
				return moveName[i];
			}
		}
		return null;
	}

	private float AnalyzeMoveStrength(string moveName, int playerNum) {
		float strength = 0f;
		int minDist = 0;
		int maxDist = 0;
		switch (moveName) {
		case Constants.FIGHT_MOVE_DASH:
			minDist = FindGridDistance(new Vector2(0,0), 
			                           new Vector2(0,2));
			maxDist = FindGridDistance(new Vector2(0,0), 
			                           new Vector2(ringSize.x,ringSize.y));
			if(maxDist >= dist && dist >= minDist) {
				strength += dist;
			}
			return strength;
		case Constants.FIGHT_MOVE_FLYING_TALON:
			minDist = FindGridDistance(new Vector2(0,0), 
			                           new Vector2(0,0));
			maxDist = FindGridDistance(new Vector2(0,0), 
			                           new Vector2(0,2));
			if(maxDist >= dist && dist >= minDist) {
				strength += 5f;
			}
			return strength;
		case Constants.FIGHT_MOVE_SIDESTEP:
			minDist = FindGridDistance(new Vector2(0,0), 
			                           new Vector2(0,2));
			maxDist = FindGridDistance(new Vector2(0,0), 
			                           new Vector2(ringSize.x,ringSize.y));
			if(maxDist >= dist && dist >= minDist) {
				strength += 5f;
			}
			return strength;
		case Constants.FIGHT_MOVE_PECK:
			minDist = FindGridDistance(new Vector2(0,0), 
			                           new Vector2(0,0));
			maxDist = FindGridDistance(new Vector2(0,0), 
			                           new Vector2(0,1));
			if(maxDist >= dist && dist >= minDist) {
				strength += 5f;
			}
			return strength;
		default:
			return strength;
		}
	}

	private void ProcessMove(string moveName, int pN) {
		if(debug) print ("move used by chicken " + pN + " is " + moveName);
		int eN = Mathf.Abs(pN-1);
		switch (moveName) {
		case Constants.FIGHT_MOVE_DASH:
			pos[pN] = StepTo(pos[pN], pos[eN], 2, 2);
			UpdateDistance ();
			break;
		case Constants.FIGHT_MOVE_FLYING_TALON:
			pos[pN] = StepTo(pos[pN], pos[eN], 1, 1);
			UpdateDistance ();
			hp[eN] -= (int)(atk[pN] * 0.7f);
			if(debug) print ("updated hp of chicken " + eN + " is " + hp[eN]);
			break;
		case Constants.FIGHT_MOVE_SIDESTEP:
			pos[pN] = StepTo(pos[pN], pos[eN] + new Vector2(Random.Range(-4,5), 0), 5, 1);
			UpdateDistance ();
			if(debug) print ("Sidestep!");
			break;
		case Constants.FIGHT_MOVE_PECK:
			hp[eN] -= (int)(atk[pN] * 0.4f);
			if(debug) print ("updated hp of chicken " + eN + " is " + hp[eN]);
			break;
		default:
			break;
		}
	}

	private void UpdateDistance() {
		dist = FindGridDistance(pos[0], pos[1]);
		print ("Distance of " + pos[0] + " and " + pos[1] + " is " + dist);
	}

	private Vector2 StepTo(Vector2 start, Vector2 dest, int numSteps, int minDistSpacing) {
		Vector2 newPos = start;
		print ("old pos is " + start + ", dest is " + dest + ", min dist spacing is " + minDistSpacing);
		for(int i = 0; i < numSteps; i++) {
			if(FindGridDistance(newPos, dest) <= minDistSpacing) {
				break;
			}
			if((int)dest.x > (int)newPos.x) {
				print ("dest x is greater than start x");
				newPos += new Vector2(1, 0);
			}
			else {
				print ("dest x is less than start x");
				newPos -= new Vector2(1, 0);
			}
			if((int)dest.y > (int)newPos.y) {
				print ("dest y is greater than start y");
				newPos += new Vector2(0, 1);
			}
			else {
				print ("dest y is less than start y");
				newPos -= new Vector2(0, 1);
			}

		}
		print ("new pos is " + newPos);
		return newPos;
	}

	private int FindGridDistance(Vector2 start, Vector2 end) {
		return Mathf.Max(Mathf.Abs((int)start.x - (int)end.x),
		                 Mathf.Abs((int)start.y - (int)end.y));
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
