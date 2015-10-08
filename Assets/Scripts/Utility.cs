using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Facebook.Unity;

public class Utility : MonoBehaviour {

	public static void PrintDictionary(IDictionary<string,object> id) {
		foreach(KeyValuePair<string,object> kv in id) {
			// print(kv.Key.ToString() + ": " + kv.Value.ToString());
		}
	}

	public static void FBGetName(string userId) {
		FB.API ("/"+userId,HttpMethod.GET, GetNameCallback);
	}

	private static void GetNameCallback(IResult result) {
		print ("GetNamecallBack result: " + result.RawResult);
		IDictionary id = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as IDictionary;
		print ("name: " + id["name"]);
	}

	public static float Modulo(float a,float b) {
		return a - b * Mathf.Floor(a / b);
	}

	public static int GetPayout(int betAmount, IDictionary<string,object> bettingOdds, bool isLlamado) {
		if(isLlamado) {
			return (int) betAmount * 2 * int.Parse (bettingOdds[Constants.DB_KEYWORD_DEHADO_ODDS].ToString()) / int.Parse (bettingOdds[Constants.DB_KEYWORD_LLAMADO_ODDS].ToString());
		}
		else {
			return (int) betAmount * 2 * int.Parse (bettingOdds[Constants.DB_KEYWORD_LLAMADO_ODDS].ToString()) / int.Parse (bettingOdds[Constants.DB_KEYWORD_DEHADO_ODDS].ToString());
		}
	}

	public static Vector2 AStar(Vector2 start, Vector2 goal, Vector2 setSize, int step, bool canStepToGoal) {
		List<Vector2> cameFrom = AStar (start, goal, setSize);
		/*foreach(Vector2 v in cameFrom) {
			// print("came from: " + v);
		}*/
		if (step >= cameFrom.Count-1) {
			if(cameFrom.Last () == goal && !canStepToGoal && cameFrom.Count > 1) {
				// print ("cannot step to goal, step over limit!");
				return cameFrom[cameFrom.Count-2];
			}
			// print ("can step to goal, step over limit!");
			return cameFrom.Last ();
		}
		if (canStepToGoal) {
			// print ("can step to goal!");
			return cameFrom [step + 1];
		} else {
			// print ("cannot step to goal!");
			return cameFrom [step];
		}
	}

	public static List<Vector2> AStar(Vector2 start, Vector2 goal, Vector2 setSize) {
		List<Vector2> closedSet = new List<Vector2> ();
		List<Vector2> openSet = new List<Vector2> ();
		IDictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2> ();
		openSet.Add (start);

		IDictionary<Vector2, float> gScore = new Dictionary<Vector2, float> ();
		IDictionary<Vector2, float> fScore = new Dictionary<Vector2, float> ();
		for(int x = 0; x < (int)setSize.x; x++) {
			for(int y = 0; y < (int)setSize.y; y++) {
				gScore[new Vector2(x,y)] = 999f;
				fScore[new Vector2(x,y)] = 999f;
			}
		}
		gScore [start] = 0f;
		fScore [start] = gScore [start] + AStarHeuristic(start, goal);
		
		while (openSet.Count > 0) {
			Vector2 current = fScore.OrderBy(k => k.Value).First().Key;
			foreach(Vector2 v in openSet) {
				if(!openSet.Contains(current) || fScore[v] < fScore[current]) {
					current = v;
				}
			}
			/*// print ("current node is : " + current + ", goal node is : " + goal + ". Equal? " + 
			       (current == goal));*/

			if(current == goal) {
				return AStarReconstructPath(cameFrom, goal);
			}

			openSet.Remove(current);
			closedSet.Add (current);
			foreach(Vector2 neighbor in AStarGetNeighbors(current, setSize)) {
				if(closedSet.Contains(neighbor))
				   continue;

				float tempGScore = gScore[current] + AStarHeuristic(current, neighbor);

				if(!openSet.Contains(neighbor) || tempGScore < gScore[neighbor]) {
					cameFrom[neighbor] = current;
					gScore[neighbor] = tempGScore;
					fScore[neighbor] = gScore[neighbor] + AStarHeuristic(neighbor,goal);
					if(!openSet.Contains(neighbor))
						openSet.Add (neighbor);
				}
			}
		}

		return null;
	}

	private static float AStarHeuristic(Vector2 start, Vector2 goal) {
		// Using Manhattan Distance
		return Mathf.Abs ((int)goal.x - (int)start.x) + Mathf.Abs ((int)goal.y - (int)start.y);
	}

	private static List<Vector2> AStarReconstructPath(IDictionary<Vector2, Vector2> cameFrom, Vector2 current) {
		List<Vector2> totalPath = new List<Vector2> ();
		totalPath.Add (current);
		while(cameFrom.Keys.Contains(current)) {
			current = cameFrom[current];
			totalPath.Add (current);
		}
		totalPath.Reverse ();
		return totalPath;
	}

	private static List<Vector2> AStarGetNeighbors(Vector2 current, Vector2 setSize) {
		List<Vector2> neighbors = new List<Vector2> ();
		for (int x = (int)current.x - 1; x <= (int)current.x + 1; x++) {
			for (int y = (int)current.y - 1; y <= (int)current.y + 1; y++) {
				if(x >= 0 && x < setSize.x && y >= 0 && y < setSize.y) {
					neighbors.Add (new Vector2(x,y));
				}
			}
		}
		return neighbors;
	}
}
