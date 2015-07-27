using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FightManager : MonoBehaviour {

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
	                            List<IDictionary<string, object>> c2Moves,
	                            Vector2 c1Pos, Vector2 c2Pos) {
		int atk1 = int.Parse (c1[Constants.DB_KEYWORD_ATTACK].ToString());
		int def1 = int.Parse (c1[Constants.DB_KEYWORD_DEFENSE].ToString());
		int hp1 = int.Parse (c1[Constants.DB_KEYWORD_HP].ToString());
		int agi1 = int.Parse (c1[Constants.DB_KEYWORD_AGILITY].ToString());
		int gam1 = int.Parse (c1[Constants.DB_KEYWORD_GAMENESS].ToString());
		int agg1 = int.Parse (c1[Constants.DB_KEYWORD_AGGRESSION].ToString());

		int atk2 = int.Parse (c2[Constants.DB_KEYWORD_ATTACK].ToString());
		int def2 = int.Parse (c2[Constants.DB_KEYWORD_DEFENSE].ToString());
		int hp2 = int.Parse (c2[Constants.DB_KEYWORD_HP].ToString());
		int agi2 = int.Parse (c2[Constants.DB_KEYWORD_AGILITY].ToString());
		int gam2 = int.Parse (c2[Constants.DB_KEYWORD_GAMENESS].ToString());
		int agg2 = int.Parse (c2[Constants.DB_KEYWORD_AGGRESSION].ToString());

		print ("automatic fight started");
		print ("c1 moves count is " + c1Moves.Count ());
		print ("c2 moves count is " + c2Moves.Count ());
		AssignNextMove (c1, c2, c1Moves, c1Pos, c2Pos);
		/*
		while(hp1 > 0 && hp2 > 0) {

		}
		*/
	}

	private void AssignNextMove(IDictionary<string,object> c1,
	                            IDictionary<string,object> c2,
	                            List<IDictionary<string, object>> c1Moves,
	                            Vector2 c1Pos, Vector2 c2Pos) {
		float[] movePercent = new float[] {0f, 0f, 0f, 0f};
		int[] moveStrength = new int[] {0, 0, 0, 0};
		int moveStrengthTotal = 0;

		print ("assigning next move");
		foreach (IDictionary<string,object> id in c1Moves) {
			string name = DatabaseManager.Instance.LoadFightingMove (
				id[Constants.DB_KEYWORD_FIGHTING_MOVE_ID].ToString()
			) [Constants.DB_KEYWORD_NAME].ToString ();
			moveStrength[c1Moves.IndexOf(id)] = AnalyzeMoveStrength(name, c1, c2, c1Pos, c2Pos);
			moveStrengthTotal += moveStrength[c1Moves.IndexOf(id)];
			print ("move strength of " + name + " is " + 
			       moveStrength[c1Moves.IndexOf(id)]);
		}
		print ("assigning next move 2");
		for(int x = 0; x < movePercent.Length; x++) {
			movePercent[x] = moveStrength[x] / moveStrengthTotal;
			print ("move percent of: " + name + " is " + movePercent[x]);
		}
	}

	private int AnalyzeMoveStrength(string name,
	                                IDictionary<string,object> c1,
	                                IDictionary<string,object> c2,
	                                Vector2 c1Pos, Vector2 c2Pos) {
		print ("analyzing move strength of " + name);
		int strength = 0;
		float dist = Mathf.Pow((c2Pos.x - c1Pos.x),2f) + Mathf.Pow((c2Pos.y - c1Pos.y),2f);
		float prefDist = 0f;
		switch (name) {
		case Constants.FIGHT_MOVE_DASH:
			prefDist = 1000;
			if(dist > prefDist) {
				strength += (int)(dist - prefDist);
			}
			return strength;
		case Constants.FIGHT_MOVE_FLYING_TALON:
			prefDist = 500;
			if(dist < prefDist) {
				strength += 200;
			}
			return strength;
		case Constants.FIGHT_MOVE_SIDESTEP:
			prefDist = 500;
			if(dist < prefDist) {
				strength += 100;
			}
			return strength;
		case Constants.FIGHT_MOVE_PECK:
			prefDist = 200;
			if(dist < prefDist) {
				strength += 400;
			}
			return strength;
		default:
			return strength;
		}
	}
}
