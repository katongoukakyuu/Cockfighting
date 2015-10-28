using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ServerFightManager : MonoBehaviour {

	public bool debug;

	private List<string> moveName = new List<string>();
	private int[] atk = new int[2], def = new int[2], hp = new int[2], agi = new int[2], gam = new int[2], agg = new int[2], con = new int[2];
	private float[] critChance = new float[2];
	private Vector2[] pos = new Vector2[2];
	private float dist;

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
		con[0] = int.Parse (c1[Constants.DB_KEYWORD_CONDITIONING].ToString());

		atk[1] = int.Parse (c2[Constants.DB_KEYWORD_ATTACK].ToString());
		def[1] = int.Parse (c2[Constants.DB_KEYWORD_DEFENSE].ToString());
		hp[1] = int.Parse (c2[Constants.DB_KEYWORD_HP].ToString());
		agi[1] = int.Parse (c2[Constants.DB_KEYWORD_AGILITY].ToString());
		gam[1] = int.Parse (c2[Constants.DB_KEYWORD_GAMENESS].ToString());
		agg[1] = int.Parse (c2[Constants.DB_KEYWORD_AGGRESSION].ToString());
		con[1] = int.Parse (c2[Constants.DB_KEYWORD_CONDITIONING].ToString());

		// apply aggression and conditioning buffs
		for(int i = 0; i < 2; i++) {
			float conPenalty = 1f - (1f - con[0]/100f)/2f;
			agg[i] = (int) (agg[i] * conPenalty);
			float aggBonus = agg[i] * 0.01f * Mathf.Log (agg[i],2f);
			atk[i] = (int) (atk[i] * conPenalty + aggBonus);
			def[i] = (int) (def[i] * conPenalty + aggBonus);
			hp[i] = (int) (hp[i] * conPenalty + aggBonus);
			agi[i] = (int) (agi[i] * conPenalty + aggBonus);
			gam[i] = (int) (gam[i] * conPenalty + aggBonus);
			critChance[i] = gam[i] * 0.000025f;
		}
		
		pos[0] = new Vector3(Random.Range(0,(int)ringSize.x), Random.Range (0, (int)ringSize.y));
		pos[1] = new Vector3(Random.Range(0,(int)ringSize.x), Random.Range (0, (int)ringSize.y));
		UpdateDistance();

		List<IDictionary<string,object>> l = new List<IDictionary<string,object>> ();
		string[] move = new string[2];
		int atbBarMax = 5000;
		int[] atbBar = new int[2] {0,0};

		move[0] = AssignNextMove (null, 0);
		move[1] = AssignNextMove (null, 1);
		l.Add (RecordTurn(move));

		while(hp[0] > 0 && hp[1] > 0) {
		//for(int i = 0; i < 5; i++) {
			int pN, eN;
			if (agi[0] > agi[1]) pN = 0;
			else pN = 1;
			eN = Mathf.Abs(pN - 1);

			atbBar[pN] += agi[pN];
			if(atbBar[pN] >= atbBarMax) {
				move[pN] = AssignNextMove (c1Moves, pN);
				atbBar[pN] -= atbBarMax;
				ProcessMove(move[pN], pN);
			}
			else {
				move[pN] = AssignNextMove (null, pN);
			}

			atbBar[eN] += agi[eN];
			if(atbBar[eN] >= atbBarMax) {
				move[eN] = AssignNextMove (c2Moves, eN);
				atbBar[eN] -= atbBarMax;
				ProcessMove(move[eN], eN);
			}
			else {
				move[eN] = AssignNextMove (null, eN);
			}

			if(move[pN] == Constants.FIGHT_MOVE_NONE &&
			   move[eN] == Constants.FIGHT_MOVE_NONE) {
				continue;
			}
			else {
				l.Add (RecordTurn(move));
				UpdateDistance ();
			}
		}

		Dictionary<string, object> d = GameManager.Instance.GenerateReplay (
			new string[] {c1[Constants.DB_COUCHBASE_ID].ToString(), c2[Constants.DB_COUCHBASE_ID].ToString()},
			l
		);
		var savedReplay = DatabaseManager.Instance.SaveEntry (d);

		IDictionary<string,object>[] chickens = {c1, c2};
		for(int x = 0; x < 2; x++) {
			List<string> mailTypes = new List<string>();
			mailTypes.Add (Constants.MAIL_STATUS_UNREAD);
			mailTypes.Add (Constants.MAIL_TYPE_NOTIFICATION);
			mailTypes.Add (Constants.MAIL_TYPE_REPLAY);
			Dictionary<string, object> mail = GameManager.Instance.GenerateMail(""+0, DatabaseManager.Instance.LoadPlayer(chickens[x][Constants.DB_KEYWORD_USER_ID].ToString())[Constants.DB_COUCHBASE_ID].ToString(),
															                 Constants.MAIL_FIGHT_CONCLUDED_TITLE,
															                 Constants.MAIL_FIGHT_CONCLUDED_MESSAGE_1 + chickens[x][Constants.DB_KEYWORD_NAME].ToString() + 
															                 Constants.MAIL_FIGHT_CONCLUDED_MESSAGE_2 + chickens[(x+1)%2][Constants.DB_KEYWORD_NAME].ToString() + 
															                 Constants.MAIL_FIGHT_CONCLUDED_MESSAGE_3 + DatabaseManager.Instance.LoadPlayer(chickens[(x+1)%2][Constants.DB_KEYWORD_USER_ID].ToString())[Constants.DB_KEYWORD_FARM_NAME].ToString() + 
																		     Constants.MAIL_FIGHT_CONCLUDED_MESSAGE_4,
			                                                                 mailTypes, savedReplay [Constants.DB_COUCHBASE_ID].ToString ());
			DatabaseManager.Instance.SaveEntry(mail);
		}

		// DEBUG FOR QUICK MATCH
		if(ReplayManager.Instance != null) {
			var loadedReplay = DatabaseManager.Instance.LoadReplay (savedReplay [Constants.DB_COUCHBASE_ID].ToString ());
			StartCoroutine(ReplayManager.Instance.PlayReplay(loadedReplay));
		}
		// END DEBUG FOR QUICK MATCH
		
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
			// print ("move strength of " + moveName[moves.IndexOf(id)] + " is " + moveStrength[moves.IndexOf(id)]);
		}

		if(moveStrengthTotal == 0) return null;
		float r = Random.Range(0.0f, 1.0f);
		float lowLim;
		float hiLim = 0.0f;
		// print ("randomizer is " + r);
		for (int i = 0; i < movePercent.Length; i++)
		{
			movePercent[i] = moveStrength[i] / moveStrengthTotal;
			// print ("move percent of " + moveName[i] + " is " + movePercent[i]);
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
		float minDist = 0f;
		float maxDist = 0f;
		switch (moveName) {
		case Constants.FIGHT_MOVE_DASH:
			minDist = Vector2.Distance(new Vector2(0,0), 
			                           new Vector2(0,2));
			maxDist = Vector2.Distance(new Vector2(0,0), 
			                           new Vector2(ringSize.x,ringSize.y));
			if(maxDist >= dist && dist >= minDist) {
				strength += dist;
			}
			return strength;
		case Constants.FIGHT_MOVE_FLYING_TALON:
			minDist = Vector2.Distance(new Vector2(0,0), 
			                           new Vector2(0,0));
			maxDist = Vector2.Distance(new Vector2(0,0), 
			                           new Vector2(0,2));
			if(maxDist >= dist && dist >= minDist) {
				strength += 5f;
			}
			return strength;
		case Constants.FIGHT_MOVE_SIDESTEP:
			minDist = Vector2.Distance(new Vector2(0,0), 
			                           new Vector2(0,2));
			maxDist = Vector2.Distance(new Vector2(0,0), 
			                           new Vector2(ringSize.x,ringSize.y));
			if(maxDist >= dist && dist >= minDist) {
				strength += 5f;
			}
			return strength;
		case Constants.FIGHT_MOVE_PECK:
			minDist = Vector2.Distance(new Vector2(0,0), 
			                           new Vector2(0,0));
			maxDist = Vector2.Distance(new Vector2(0,0), 
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
		// print ("move used by chicken " + pN + " is " + moveName);
		int eN = Mathf.Abs(pN-1);
		switch (moveName) {
		case Constants.FIGHT_MOVE_DASH:
			// print ("DASH, Distance of " + pos[0] + " and " + pos[1] + " is " + dist);
			// print ("old pos: " + pos[pN] + ", enemy pos: " + pos[eN]);
			pos[pN] = Utility.AStar(pos[pN],pos[eN],ringSize,2,false);
			// print ("new pos: " + pos[pN]);
			UpdateDistance ();
			// print ("AFTER DASH, Distance of " + pos[0] + " and " + pos[1] + " is " + dist);
			break;
		case Constants.FIGHT_MOVE_FLYING_TALON:
			// print ("FLYING TALON, Distance of " + pos[0] + " and " + pos[1] + " is " + dist);
			// print ("old pos: " + pos[pN] + ", enemy pos: " + pos[eN]);
			pos[pN] = Utility.AStar(pos[pN],pos[eN],ringSize,1,false);
			// print ("new pos: " + pos[pN]);
			UpdateDistance ();
			// print ("AFTER FLYING TALON, Distance of " + pos[0] + " and " + pos[1] + " is " + dist);
			hp[eN] -= CalculateDamage(0.8f, atk[pN], def[eN], critChance[pN], gam[pN]);
			// print ("updated hp of chicken " + eN + " is " + hp[eN]);
			break;
		case Constants.FIGHT_MOVE_SIDESTEP:
			// print ("SIDESTEP, Distance of " + pos[0] + " and " + pos[1] + " is " + dist);
			// print ("old pos: " + pos[pN] + ", enemy pos: " + pos[eN]);

			Vector2 sidestepPosition;
			do {
				sidestepPosition = new Vector2(Mathf.Clamp ((int)(pos[eN].x + Random.Range(-4,5)),0,(int)ringSize.x-1),
				                               pos[eN].y);
			} while(sidestepPosition == pos[eN]);
			pos[pN] = Utility.AStar(pos[pN],sidestepPosition,ringSize,5,true);
			// print ("new pos: " + pos[pN]);
			UpdateDistance ();
			// print ("AFTER SIDESTEP, Distance of " + pos[0] + " and " + pos[1] + " is " + dist);
			// print ("Sidestep!");
			break;
		case Constants.FIGHT_MOVE_PECK:
			// print ("PECK, Distance of " + pos[0] + " and " + pos[1] + " is " + dist);
			hp[eN] -= CalculateDamage(0.5f, atk[pN], def[eN], critChance[pN], gam[pN]);
			// print ("updated hp of chicken " + eN + " is " + hp[eN]);
			break;
		default:
			break;
		}
	}

	private int CalculateDamage(float mult, int pAtk, int eDef, float pCrit, int pGam) {
		if(Random.value <= pCrit) {
			return (int)(pAtk * mult * (1000f / (1000f + eDef)) * (2f + pGam * 0.0001f));
		}
		else {
			return (int)(pAtk * mult * (1000f / (1000f + eDef)));
		}
	}

	private void UpdateDistance() {
		dist = Vector2.Distance(pos[0], pos[1]);
		// print ("Distance of " + pos[0] + " and " + pos[1] + " is " + dist);
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
