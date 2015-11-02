using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Couchbase.Lite;

public class ServerManager : MonoBehaviour {

	private List<IEnumerator> listCoroutines = new List<IEnumerator>();

	private Manager manager;
	private Database db;

	private static ServerManager instance;
	private ServerManager() {}

	public static ServerManager Instance {
		get {
			if(instance == null) {
				instance = (ServerManager)GameObject.FindObjectOfType(typeof(ServerManager));
			}
			return instance;
		}
	}

	void Start () {
		DontDestroyOnLoad(this);
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
			return;
		}
		manager = Manager.SharedInstance;
		db = manager.GetDatabase("cockfighting");

		StartCoroutine(ProcessFeedSchedules());
		StartCoroutine(ProcessFeedScheduleCanceling());

		StartCoroutine(ProcessBreedSchedules());
		StartCoroutine(ProcessBreedScheduleCanceling());

		StartCoroutine(ProcessConditioningDecay());

		StartCoroutine(ProcessMatchesStatusChanges());
		StartCoroutine(ProcessMatchSchedules());
	}

	public void StopAllProcesses() {
		foreach(IEnumerator ie in listCoroutines) {
			StopCoroutine(ie);
		}
	}

	// FEED SCHEDULES START

	private IEnumerator ProcessFeedSchedules() {
		List<IDictionary<string,object>> listSchedules = new List<IDictionary<string,object>>();
		List<System.DateTime> listEndTimes = new List<System.DateTime>();
		System.DateTime earliestEndTime = System.DateTime.MinValue;
		int earliestEndTimeIndex = 0;

		ProcessFeedSchedulesInitialize(ref listSchedules, ref listEndTimes, ref earliestEndTime, ref earliestEndTimeIndex);

		while(true) {
			if(earliestEndTime.CompareTo(System.DateTime.MinValue) != 0 && earliestEndTime.CompareTo(System.DateTime.Now.ToUniversalTime()) < 0) {
				ProcessFeedSchedulesApplySchedule(listSchedules[earliestEndTimeIndex]);
				ProcessFeedSchedulesInitialize(ref listSchedules, ref listEndTimes, ref earliestEndTime, ref earliestEndTimeIndex);
			}
			yield return new WaitForSeconds(1);
		}
	}

	private void ProcessFeedSchedulesInitialize(ref List<IDictionary<string,object>> listSchedules,
	                                            ref List<System.DateTime> listEndTimes,
	                                            ref System.DateTime earliestEndTime,
	                                            ref int earliestEndTimeIndex) {
		listSchedules = DatabaseManager.Instance.LoadFeedsSchedule(null);
		listEndTimes.Clear ();
		earliestEndTime = System.DateTime.MinValue;
		earliestEndTimeIndex = 0;

		listSchedules.RemoveAll(i => i[Constants.DB_KEYWORD_IS_COMPLETED].ToString() != Constants.GENERIC_FALSE);

		foreach(IDictionary<string,object> i in listSchedules) {
			System.DateTime dtTemp = System.DateTime.Parse (i[Constants.DB_KEYWORD_END_TIME].ToString());
			listEndTimes.Add (dtTemp);
			if(earliestEndTime.CompareTo(System.DateTime.MinValue) == 0 || earliestEndTime.CompareTo(dtTemp) > 0) {
				earliestEndTime = dtTemp;
				earliestEndTimeIndex = listSchedules.IndexOf(i);
			}
		}
	}

	private void ProcessFeedSchedulesApplySchedule(IDictionary<string,object> schedule) {
		IDictionary<string,object> chicken = DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID].ToString());
		IDictionary<string,object> feeds = DatabaseManager.Instance.LoadFeeds(schedule[Constants.DB_KEYWORD_FEEDS_ID].ToString());

		int[] stats = new int[7];
		int[] statsMax = new int[7];
		int[] feedsStats = new int[7];
		string[] statsStrings = {
			Constants.DB_KEYWORD_ATTACK,
			Constants.DB_KEYWORD_DEFENSE,
			Constants.DB_KEYWORD_HP,
			Constants.DB_KEYWORD_AGILITY,
			Constants.DB_KEYWORD_GAMENESS,
			Constants.DB_KEYWORD_AGGRESSION,
			Constants.DB_KEYWORD_CONDITIONING
		};
		string[] statsStringsMax = {
			Constants.DB_KEYWORD_ATTACK_MAX,
			Constants.DB_KEYWORD_DEFENSE_MAX,
			Constants.DB_KEYWORD_HP_MAX,
			Constants.DB_KEYWORD_AGILITY_MAX,
			Constants.DB_KEYWORD_GAMENESS_MAX,
			Constants.DB_KEYWORD_AGGRESSION_MAX,
			Constants.CHICKEN_CONDITIONING_DEFAULT_MAX.ToString()
		};

		for(int i = 0; i < statsStrings.Length; i++) {
			stats[i] = int.Parse (chicken[statsStrings[i]].ToString());
			statsMax[i] = int.Parse (chicken[statsStringsMax[i]].ToString());
			feedsStats[i] = int.Parse (feeds[statsStrings[i]].ToString());
			chicken[statsStrings[i]] = stats[i] + feedsStats[i] <= statsMax[i] ? stats[i] + feedsStats[i] : statsMax[i];
		}

		DatabaseManager.Instance.EditChicken(chicken);

		schedule[Constants.DB_KEYWORD_IS_COMPLETED] = Constants.GENERIC_TRUE;
		DatabaseManager.Instance.EditFeedsSchedule(schedule);

	}

	private IEnumerator ProcessFeedScheduleCanceling() {
		db.Changed += (sender, e) => {
			var changes = e.Changes.ToList();
			foreach (DocumentChange change in changes) {
				IDictionary<string,object> properties = db.GetDocument(change.DocumentId).Properties;
				if(properties[Constants.DB_KEYWORD_TYPE].ToString() == Constants.DB_TYPE_FEEDS_SCHEDULE) {
					if(properties[Constants.DB_KEYWORD_IS_COMPLETED].ToString() == Constants.GENERIC_CANCELED) {
						// print("Schedule " + change.DocumentId + " has been canceled! Details below.");
						foreach(KeyValuePair<string,object> kv in properties) {
							// print (kv.Key + ": " + kv.Value);
						}
						FlagFeedScheduleAsCanceled(properties);
					}
				}
			}
		};
		yield break;
	}

	private void FlagFeedScheduleAsCanceled(IDictionary<string,object> schedule) {
		IDictionary<string,object> chicken = DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID].ToString());
		IDictionary<string,object> feeds = DatabaseManager.Instance.LoadFeeds(schedule[Constants.DB_KEYWORD_FEEDS_ID].ToString());
		List<IDictionary<string,object>> s = DatabaseManager.Instance.LoadFeedsSchedule(chicken[Constants.DB_COUCHBASE_ID].ToString());
		List<IDictionary<string,object>> schedules = new List<IDictionary<string,object>>();
		System.DateTime dt = System.DateTime.Parse(schedule[Constants.DB_KEYWORD_END_TIME].ToString());

		foreach(IDictionary<string, object> i in s) {
			if(i[Constants.DB_KEYWORD_IS_COMPLETED].ToString() == Constants.GENERIC_FALSE &&
			   System.DateTime.Parse(i[Constants.DB_KEYWORD_END_TIME].ToString()).CompareTo(dt) > 0) {
				schedules.Add (i);
			}
		}
		System.TimeSpan duration = new System.TimeSpan(int.Parse (feeds[Constants.DB_KEYWORD_DURATION_DAYS].ToString ()),
		                                          int.Parse (feeds[Constants.DB_KEYWORD_DURATION_HOURS].ToString ()),
		                                          int.Parse (feeds[Constants.DB_KEYWORD_DURATION_MINUTES].ToString ()),
		                                          int.Parse (feeds[Constants.DB_KEYWORD_DURATION_SECONDS].ToString ()));

		System.TimeSpan ts = System.DateTime.Parse(schedule[Constants.DB_KEYWORD_END_TIME].ToString()).Subtract(duration) - System.DateTime.Now.ToUniversalTime();
		if(ts.Ticks < 0) {
			ts = System.DateTime.Parse(schedule[Constants.DB_KEYWORD_END_TIME].ToString()) - System.DateTime.Now.ToUniversalTime();
		}

		foreach(IDictionary<string, object> i in schedules) {
			i[Constants.DB_KEYWORD_END_TIME] = System.DateTime.Parse(i[Constants.DB_KEYWORD_END_TIME].ToString()).Subtract(ts);
			DatabaseManager.Instance.EditFeedsSchedule(i);
		}

	}

	// FEED SCHEDULES END

	// CONDITIONING DECAY START
	
	private IEnumerator ProcessConditioningDecay() {
		List<IDictionary<string,object>> listSchedules = new List<IDictionary<string,object>>();
		List<System.DateTime> listEndTimes = new List<System.DateTime>();
		System.DateTime earliestEndTime = System.DateTime.MinValue;
		int earliestEndTimeIndex = 0;
		
		ProcessConditioningDecayInitialize(ref listSchedules, ref listEndTimes, ref earliestEndTime, ref earliestEndTimeIndex);
		
		while(true) {
			if(earliestEndTime.CompareTo(System.DateTime.MinValue) != 0 && earliestEndTime.CompareTo(System.DateTime.Now.ToUniversalTime()) < 0) {
				ProcessConditioningDecayApplySchedule(listSchedules[earliestEndTimeIndex]);
				ProcessConditioningDecayInitialize(ref listSchedules, ref listEndTimes, ref earliestEndTime, ref earliestEndTimeIndex);
			}
			yield return new WaitForSeconds(1);
		}
	}
	
	private void ProcessConditioningDecayInitialize(ref List<IDictionary<string,object>> listSchedules,
	                                            ref List<System.DateTime> listEndTimes,
	                                            ref System.DateTime earliestEndTime,
	                                            ref int earliestEndTimeIndex) {
		listSchedules = DatabaseManager.Instance.LoadFeedsSchedule(null);
		listEndTimes.Clear ();
		earliestEndTime = System.DateTime.MinValue;
		earliestEndTimeIndex = 0;
		
		listSchedules.RemoveAll(i => i[Constants.DB_KEYWORD_IS_COMPLETED].ToString() != Constants.GENERIC_FALSE);
		
		foreach(IDictionary<string,object> i in listSchedules) {
			System.DateTime dtTemp = System.DateTime.Parse (i[Constants.DB_KEYWORD_END_TIME].ToString());
			listEndTimes.Add (dtTemp);
			if(earliestEndTime.CompareTo(System.DateTime.MinValue) == 0 || earliestEndTime.CompareTo(dtTemp) > 0) {
				earliestEndTime = dtTemp;
				earliestEndTimeIndex = listSchedules.IndexOf(i);
			}
		}
	}
	
	private void ProcessConditioningDecayApplySchedule(IDictionary<string,object> schedule) {
		IDictionary<string,object> chicken = DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID].ToString());

		if(int.Parse (chicken[Constants.DB_KEYWORD_CONDITIONING].ToString()) > 50 &&
		   chicken[Constants.DB_KEYWORD_LIFE_STAGE].ToString() != Constants.LIFE_STAGE_EGG) {
			chicken[Constants.DB_KEYWORD_CONDITIONING] = "" + (int.Parse (chicken[Constants.DB_KEYWORD_CONDITIONING].ToString()) - Constants.CHICKEN_CONDITIONING_DEFAULT_DECAY_AMOUNT);
			DatabaseManager.Instance.EditChicken(chicken);
		}

		System.DateTime dtTemp = System.DateTime.Parse (schedule[Constants.DB_KEYWORD_END_TIME].ToString());
		dtTemp = dtTemp.AddHours (Constants.CHICKEN_CONDITIONING_DEFAULT_DECAY_TIMER);
		schedule[Constants.DB_KEYWORD_END_TIME] = dtTemp;
		DatabaseManager.Instance.EditConditioningDecaySchedule(schedule);
	}
	
	// CONDITIONING DECAY END

	// BREED SCHEDULES START

	private IEnumerator ProcessBreedSchedules() {
		List<IDictionary<string,object>> listSchedules = new List<IDictionary<string,object>>();
		List<System.DateTime> listEndTimes = new List<System.DateTime>();
		System.DateTime earliestEndTime = System.DateTime.MinValue;
		int earliestEndTimeIndex = 0;
		
		ProcessBreedSchedulesInitialize(ref listSchedules, ref listEndTimes, ref earliestEndTime, ref earliestEndTimeIndex);
		
		while(true) {
			if(earliestEndTime.CompareTo(System.DateTime.MinValue) != 0 && earliestEndTime.CompareTo(System.DateTime.Now.ToUniversalTime()) < 0) {
				ProcessBreedSchedulesApplySchedule(listSchedules[earliestEndTimeIndex]);
				ProcessBreedSchedulesInitialize(ref listSchedules, ref listEndTimes, ref earliestEndTime, ref earliestEndTimeIndex);
			}
			yield return new WaitForSeconds(1);
		}
	}

	private void ProcessBreedSchedulesInitialize(ref List<IDictionary<string,object>> listSchedules,
	                                            ref List<System.DateTime> listEndTimes,
	                                            ref System.DateTime earliestEndTime,
	                                            ref int earliestEndTimeIndex) {
		listSchedules = DatabaseManager.Instance.LoadBreedsSchedule(null);
		listEndTimes.Clear ();
		earliestEndTime = System.DateTime.MinValue;
		earliestEndTimeIndex = 0;
		
		listSchedules.RemoveAll(i => i[Constants.DB_KEYWORD_IS_COMPLETED].ToString() != Constants.GENERIC_FALSE);
		
		foreach(IDictionary<string,object> i in listSchedules) {
			System.DateTime dtTemp = System.DateTime.Parse (i[Constants.DB_KEYWORD_END_TIME].ToString());
			listEndTimes.Add (dtTemp);
			if(earliestEndTime.CompareTo(System.DateTime.MinValue) == 0 || earliestEndTime.CompareTo(dtTemp) > 0) {
				earliestEndTime = dtTemp;
				earliestEndTimeIndex = listSchedules.IndexOf(i);
			}
		}
	}

	private void ProcessBreedSchedulesApplySchedule(IDictionary<string,object> schedule) {
		IDictionary<string,object> chicken1 = DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString());
		IDictionary<string,object> chicken2 = DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString());
		
		string breed;
		string[] generationId = new string[2];
		int conditioning;
		int[] generationCount = new int[2];
		if(chicken1[Constants.DB_KEYWORD_GENDER].ToString() == Constants.GENDER_MALE) {
			breed = chicken1[Constants.DB_TYPE_BREED].ToString();
			generationId[0] = chicken1[Constants.DB_KEYWORD_GENERATION_ID].ToString();
			generationCount[0] = int.Parse(chicken1[Constants.DB_KEYWORD_GENERATION_COUNT].ToString());
			conditioning = int.Parse(chicken2[Constants.DB_KEYWORD_CONDITIONING].ToString());
		}
		else {
			breed = chicken2[Constants.DB_TYPE_BREED].ToString();
			generationId[0] = chicken2[Constants.DB_KEYWORD_GENERATION_ID].ToString();
			generationCount[0] = int.Parse(chicken2[Constants.DB_KEYWORD_GENERATION_COUNT].ToString());
			conditioning = int.Parse(chicken1[Constants.DB_KEYWORD_CONDITIONING].ToString());
		}

		// inbreeding code
		float inbreedingPenalty = 0f;
		if(generationId[0] == generationId[1]) {
			int generationGap = Mathf.Abs(generationCount[0] - generationCount[1]);
			if(generationGap <= 2) {
				generationGap = 3 - generationGap;
				inbreedingPenalty = 5f * Mathf.Pow(2, generationGap-1) * 0.01f;
			}
		}

		IDictionary<string,object> chickenChild = DatabaseManager.Instance.SaveEntry(GameManager.Instance.GenerateChicken ("Child", 
		                                                      chicken1 [Constants.DB_KEYWORD_USER_ID].ToString(), 
		                                                      Random.Range(0,2) == 0 ? Constants.GENDER_MALE : Constants.GENDER_FEMALE, 
		                                                      breed, generationId[0], generationCount[0]+1,
		                                                      Constants.LIFE_STAGE_EGG));

		/* 
		 * formula for base stats:
		 * (parent_1_stat/2 + parent_2_stat/2) * (0.9 to 1.1) * (1 - (1 - hen_conditioning)/2) * 0.01 * (1 - inbreed_penalty)
		 */
		float conditioningMultiplier = (1f-(1f-conditioning*0.01f)/2f) * 0.01f;
		float inbreedingMultiplier = (1f - inbreedingPenalty);
		int[,] pStats = new int[2,6];
		int[,] pMax = new int[2,6];
		string[] statsStrings = {
			Constants.DB_KEYWORD_ATTACK,
			Constants.DB_KEYWORD_DEFENSE,
			Constants.DB_KEYWORD_HP,
			Constants.DB_KEYWORD_AGILITY,
			Constants.DB_KEYWORD_GAMENESS,
			Constants.DB_KEYWORD_AGGRESSION,
		};
		string[] statsStringsMax = {
			Constants.DB_KEYWORD_ATTACK_MAX,
			Constants.DB_KEYWORD_DEFENSE_MAX,
			Constants.DB_KEYWORD_HP_MAX,
			Constants.DB_KEYWORD_AGILITY_MAX,
			Constants.DB_KEYWORD_GAMENESS_MAX,
			Constants.DB_KEYWORD_AGGRESSION_MAX,
		};

		for(int i = 0; i < statsStrings.Length; i++) {
			pStats[0,i] = int.Parse (chicken1[statsStrings[i]].ToString());
			pStats[1,i] = int.Parse (chicken2[statsStrings[i]].ToString());
			pMax[0,i] = int.Parse (chicken1[statsStringsMax[i]].ToString());
			pMax[1,i] = int.Parse (chicken2[statsStringsMax[i]].ToString());

			chickenChild[statsStrings[i]] = (int)((pStats[0,i]/2f + pStats[1,i]/2f) * Random.Range(0.9f,1.1f) * conditioningMultiplier * inbreedingMultiplier);
			chickenChild[statsStringsMax[i]] = (int)((pMax[0,i] + pMax[1,i]) * conditioningMultiplier);
		}
	
		DatabaseManager.Instance.EditChicken(chickenChild);
		
		if(chickenChild[Constants.DB_KEYWORD_GENDER].ToString() != Constants.GENDER_FEMALE) {
			DatabaseManager.Instance.SaveEntry (GameManager.Instance.GenerateFightingMoveOwnedByChicken (
				chickenChild[Constants.DB_COUCHBASE_ID].ToString(),
				DatabaseManager.Instance.LoadFightingMove(Constants.FIGHT_MOVE_DASH)[Constants.DB_COUCHBASE_ID].ToString()
				));
			DatabaseManager.Instance.SaveEntry (GameManager.Instance.GenerateFightingMoveOwnedByChicken (
				chickenChild[Constants.DB_COUCHBASE_ID].ToString(),
				DatabaseManager.Instance.LoadFightingMove(Constants.FIGHT_MOVE_FLYING_TALON)[Constants.DB_COUCHBASE_ID].ToString()
				));
			DatabaseManager.Instance.SaveEntry (GameManager.Instance.GenerateFightingMoveOwnedByChicken (
				chickenChild[Constants.DB_COUCHBASE_ID].ToString(),
				DatabaseManager.Instance.LoadFightingMove(Constants.FIGHT_MOVE_SIDESTEP)[Constants.DB_COUCHBASE_ID].ToString()
				));
			DatabaseManager.Instance.SaveEntry (GameManager.Instance.GenerateFightingMoveOwnedByChicken (
				chickenChild[Constants.DB_COUCHBASE_ID].ToString(),
				DatabaseManager.Instance.LoadFightingMove(Constants.FIGHT_MOVE_PECK)[Constants.DB_COUCHBASE_ID].ToString()
				));
		}
		
		schedule[Constants.DB_KEYWORD_IS_COMPLETED] = Constants.GENERIC_TRUE;
		DatabaseManager.Instance.EditBreedsSchedule(schedule);
		
	}

	private IEnumerator ProcessBreedScheduleCanceling() {
		db.Changed += (sender, e) => {
			var changes = e.Changes.ToList();
			foreach (DocumentChange change in changes) {
				IDictionary<string,object> properties = db.GetDocument(change.DocumentId).Properties;
				if(properties[Constants.DB_KEYWORD_TYPE].ToString() == Constants.DB_TYPE_BREED_SCHEDULE) {
					if(properties[Constants.DB_KEYWORD_IS_COMPLETED].ToString() == Constants.GENERIC_CANCELED) {
						// print("Schedule " + change.DocumentId + " has been canceled! Details below.");
						foreach(KeyValuePair<string,object> kv in properties) {
							// print (kv.Key + ": " + kv.Value);
						}
						FlagBreedScheduleAsCanceled(properties);
					}
				}
			}
		};
		yield break;
	}

	private void FlagBreedScheduleAsCanceled(IDictionary<string,object> schedule) {
		IDictionary<string,object> chicken1 = DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString());
		List<IDictionary<string,object>> s = DatabaseManager.Instance.LoadBreedsSchedule(chicken1[Constants.DB_COUCHBASE_ID].ToString());
		List<IDictionary<string,object>> schedules = new List<IDictionary<string,object>>();
		System.DateTime dt = System.DateTime.Parse(schedule[Constants.DB_KEYWORD_END_TIME].ToString());
		
		foreach(IDictionary<string, object> i in s) {
			if(i[Constants.DB_KEYWORD_IS_COMPLETED].ToString() == Constants.GENERIC_FALSE &&
			   System.DateTime.Parse(i[Constants.DB_KEYWORD_END_TIME].ToString()).CompareTo(dt) > 0) {
				schedules.Add (i);
			}
		}

		System.TimeSpan duration = new System.TimeSpan(int.Parse (Constants.BREED_DURATION_DEFAULT_DAYS.ToString()),
		                                               int.Parse (Constants.BREED_DURATION_DEFAULT_HOURS.ToString()),
		                                               int.Parse (Constants.BREED_DURATION_DEFAULT_MINUTES.ToString()),
		                                               int.Parse (Constants.BREED_DURATION_DEFAULT_SECONDS.ToString()));
		
		System.TimeSpan ts = System.DateTime.Parse(schedule[Constants.DB_KEYWORD_END_TIME].ToString()).Subtract(duration) - System.DateTime.Now.ToUniversalTime();
		if(ts.Ticks < 0) {
			ts = System.DateTime.Parse(schedule[Constants.DB_KEYWORD_END_TIME].ToString()) - System.DateTime.Now.ToUniversalTime();
		}
		
		foreach(IDictionary<string, object> i in schedules) {
			i[Constants.DB_KEYWORD_END_TIME] = System.DateTime.Parse(i[Constants.DB_KEYWORD_END_TIME].ToString()).Subtract(ts);
			DatabaseManager.Instance.EditBreedsSchedule(i);
		}
		
	}

	// BREED SCHEDULES END

	// MATCHES START

	private IEnumerator ProcessMatchSchedules() {
		List<IDictionary<string,object>> listSchedules = new List<IDictionary<string,object>>();
		List<System.DateTime> listEndTimes = new List<System.DateTime>();
		System.DateTime earliestEndTime = System.DateTime.MinValue;
		int earliestEndTimeIndex = 0;
		
		ProcessMatchSchedulesInitialize(ref listSchedules, ref listEndTimes, ref earliestEndTime, ref earliestEndTimeIndex);
		
		while(true) {
			if(earliestEndTime.CompareTo(System.DateTime.MinValue) != 0 && earliestEndTime.CompareTo(System.DateTime.Now.ToUniversalTime()) < 0) {
				UpdateMatchSchedule(listSchedules[earliestEndTimeIndex]);
				ProcessMatchSchedulesInitialize(ref listSchedules, ref listEndTimes, ref earliestEndTime, ref earliestEndTimeIndex);
			}
			yield return new WaitForSeconds(1);
		}
	}

	private void ProcessMatchSchedulesInitialize(ref List<IDictionary<string,object>> listSchedules,
	                                             ref List<System.DateTime> listEndTimes,
	                                             ref System.DateTime earliestEndTime,
	                                             ref int earliestEndTimeIndex) {
		listSchedules = DatabaseManager.Instance.LoadMatch(null);
		listEndTimes.Clear ();
		earliestEndTime = System.DateTime.MinValue;
		earliestEndTimeIndex = 0;
		
		listSchedules.RemoveAll(i => i[Constants.DB_KEYWORD_STATUS].ToString() != Constants.MATCH_STATUS_BETTING_PERIOD);
		
		foreach(IDictionary<string,object> i in listSchedules) {
			System.DateTime dtTemp = System.DateTime.Parse (i[Constants.DB_KEYWORD_INTERVAL_TIME].ToString());
			listEndTimes.Add (dtTemp);
			if(earliestEndTime.CompareTo(System.DateTime.MinValue) == 0 || earliestEndTime.CompareTo(dtTemp) > 0) {
				earliestEndTime = dtTemp;
				earliestEndTimeIndex = listSchedules.IndexOf(i);
			}
		}
	}

	private void UpdateMatchSchedule(IDictionary<string,object> schedule) {
		if(schedule[Constants.DB_KEYWORD_STATUS].ToString() == Constants.MATCH_STATUS_CANCELED) {
			return;
		}

		System.DateTime intervalTime = Utility.TrimMilli(System.DateTime.Parse(schedule[Constants.DB_KEYWORD_INTERVAL_TIME].ToString()));
		System.TimeSpan interval = Utility.TrimMilli(System.TimeSpan.FromTicks(long.Parse(schedule[Constants.DB_KEYWORD_INTERVAL].ToString())));
		int bettingOddsOrder = int.Parse (DatabaseManager.Instance.LoadBettingOdds(schedule[Constants.DB_KEYWORD_BETTING_ODDS_ID].ToString())[Constants.DB_KEYWORD_ORDER].ToString());
		int intervalsLeft = int.Parse (schedule[Constants.DB_KEYWORD_INTERVALS_LEFT].ToString());

		if(intervalsLeft == 0) {
			ResolveMatch(schedule);
		}
		else {
			intervalTime = intervalTime.Add (interval);
			schedule[Constants.DB_KEYWORD_INTERVALS_LEFT] = (intervalsLeft - 1);
			schedule[Constants.DB_KEYWORD_INTERVAL_TIME] = intervalTime;
			schedule[Constants.DB_KEYWORD_BETTING_ODDS_ID] = DatabaseManager.Instance.LoadBettingOdds(bettingOddsOrder+1)[Constants.DB_COUCHBASE_ID].ToString();
			DatabaseManager.Instance.EditMatch(schedule);
		}
	}

	private IEnumerator ProcessMatchesStatusChanges() {
		db.Changed += (sender, e) => {
			var changes = e.Changes.ToList();
			foreach (DocumentChange change in changes) {
				IDictionary<string,object> properties = db.GetDocument(change.DocumentId).Properties;
				if(properties[Constants.DB_KEYWORD_TYPE].ToString() == Constants.DB_TYPE_MATCH) {
					if(properties[Constants.DB_KEYWORD_STATUS].ToString() == Constants.MATCH_STATUS_WAITING_FOR_OPPONENT ||
					   properties[Constants.DB_KEYWORD_STATUS].ToString() == Constants.MATCH_STATUS_OPPONENT_FOUND) {
						// print("Match " + change.DocumentId + " status has been changed!");
						FlagChickensAsQueuedInMatch(properties);
					}
				}
				if(properties[Constants.DB_KEYWORD_TYPE].ToString() == Constants.DB_TYPE_BET) {
					LlamadoDehadoCheck(properties[Constants.DB_KEYWORD_MATCH_ID].ToString(), properties);
				}
				if(properties[Constants.DB_KEYWORD_TYPE].ToString() == Constants.DB_TYPE_MATCH) {
					if(properties[Constants.DB_KEYWORD_STATUS].ToString() == Constants.MATCH_STATUS_CANCELED) {
						// print("Match " + change.DocumentId + " has been canceled!");
						FlagMatchAsCanceled(properties);
					}
				}
			}
		};
		yield break;
	}

	private void FlagChickensAsQueuedInMatch(IDictionary<string,object> schedule) {
		IDictionary<string,object> chicken1 = DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString());
		IDictionary<string,object> chicken2 = null;

		chicken1[Constants.DB_KEYWORD_IS_QUEUED_FOR_MATCH] = true;
		DatabaseManager.Instance.EditChicken(chicken1);

		if(schedule[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString() != "") {
			chicken2 = DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString());
			chicken2[Constants.DB_KEYWORD_IS_QUEUED_FOR_MATCH] = true;
			DatabaseManager.Instance.EditChicken(chicken2);
			if(schedule[Constants.DB_KEYWORD_BETTING_OPTION].ToString() == Constants.BETTING_OPTION_SPECTATOR_BETTING) {
				SwitchToBettingPeriod(schedule);
			}
			else {
				ResolveMatch(schedule);
			}
		}
	}

	private void FlagMatchAsCanceled(IDictionary<string,object> schedule) {
		List<IDictionary<string,object>> chickens = new List<IDictionary<string,object>>();
		
		chickens.Add (DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString()));
		chickens.Add (DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString()));
		
		foreach(IDictionary<string,object> i in chickens) {
			i[Constants.DB_KEYWORD_IS_QUEUED_FOR_MATCH] = false;
			DatabaseManager.Instance.EditChicken(i);
		}
		
		List<IDictionary<string,object>> bets = DatabaseManager.Instance.LoadBets(schedule[Constants.DB_COUCHBASE_ID].ToString());
		foreach(IDictionary<string,object> i in bets) {			
			int prize = int.Parse (i[Constants.DB_KEYWORD_BET_AMOUNT].ToString());
			IDictionary<string,object> player = DatabaseManager.Instance.LoadPlayer(i[Constants.DB_KEYWORD_PLAYER_ID].ToString());
			player[Constants.DB_KEYWORD_COIN] = (int.Parse (player[Constants.DB_KEYWORD_COIN].ToString()) + prize);
			DatabaseManager.Instance.EditAccount(player);
		}
	}

	private void SwitchToBettingPeriod(IDictionary<string,object> schedule) {
		System.DateTime startTime = Utility.TrimMilli(System.DateTime.Parse(schedule[Constants.DB_KEYWORD_CREATED_AT].ToString()));
		System.DateTime endTime = Utility.TrimMilli(System.DateTime.Parse(schedule[Constants.DB_KEYWORD_END_TIME].ToString()));
		System.TimeSpan interval = endTime - startTime;
		System.DateTime intervalTime;

		interval = Utility.TrimMilli(new System.TimeSpan(interval.Ticks/Constants.BETTING_ODDS_COUNT));
		intervalTime = startTime.Add (interval);

		schedule[Constants.DB_KEYWORD_INTERVAL] = interval.Ticks;
		schedule[Constants.DB_KEYWORD_INTERVALS_LEFT] = (Constants.BETTING_ODDS_COUNT - 1);
		schedule[Constants.DB_KEYWORD_INTERVAL_TIME] = intervalTime;
		schedule[Constants.DB_KEYWORD_BETTING_ODDS_ID] = DatabaseManager.Instance.LoadBettingOdds(0)[Constants.DB_COUCHBASE_ID].ToString();
		schedule[Constants.DB_KEYWORD_STATUS] = Constants.MATCH_STATUS_BETTING_PERIOD;
		DatabaseManager.Instance.EditMatch(schedule);
	}

	private void ResolveMatch(IDictionary<string,object> schedule) {
		int winner = 0, loser = 0;
		List<IDictionary<string,object>> chickens = new List<IDictionary<string,object>>();
		List<IDictionary<string,object>> players = new List<IDictionary<string,object>>();

		chickens.Add (DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString()));
		chickens.Add (DatabaseManager.Instance.LoadChicken(schedule[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString()));
		players.Add (DatabaseManager.Instance.LoadPlayer(schedule[Constants.DB_KEYWORD_PLAYER_ID_1].ToString()));
		players.Add (DatabaseManager.Instance.LoadPlayer(schedule[Constants.DB_KEYWORD_PLAYER_ID_2].ToString()));

		winner = ServerFightManager.Instance.AutomateFight (
			DatabaseManager.Instance.LoadChicken(chickens[0][Constants.DB_COUCHBASE_ID].ToString()),
			DatabaseManager.Instance.LoadChicken(chickens[1][Constants.DB_COUCHBASE_ID].ToString()),
			DatabaseManager.Instance.LoadFightingMovesOwned (chickens[0][Constants.DB_COUCHBASE_ID].ToString()),
			DatabaseManager.Instance.LoadFightingMovesOwned (chickens[1][Constants.DB_COUCHBASE_ID].ToString())
		);
		loser = Mathf.Abs (winner - 1);

		foreach(IDictionary<string,object> i in chickens) {
			i[Constants.DB_KEYWORD_IS_QUEUED_FOR_MATCH] = false;
			DatabaseManager.Instance.EditChicken(i);
		}

		List<IDictionary<string,object>> bets = DatabaseManager.Instance.LoadBets(schedule[Constants.DB_COUCHBASE_ID].ToString());
		foreach(IDictionary<string,object> i in bets) {
			if(i[Constants.DB_KEYWORD_BETTED_CHICKEN_ID].ToString() != chickens[winner][Constants.DB_COUCHBASE_ID].ToString()) {
				continue;
			}

			IDictionary<string,object> odds = DatabaseManager.Instance.LoadBettingOdds(i[Constants.DB_KEYWORD_BETTING_ODDS_ID].ToString());
			int prize = 0;
			if(i[Constants.DB_KEYWORD_BETTED_CHICKEN_STATUS].ToString() == Constants.BETTED_CHICKEN_STATUS_LLAMADO) {
				prize = Utility.GetPayout(int.Parse (i[Constants.DB_KEYWORD_BET_AMOUNT].ToString()), odds, true);
			}
			else {
				prize = Utility.GetPayout(int.Parse (i[Constants.DB_KEYWORD_BET_AMOUNT].ToString()), odds, false);
			}

			IDictionary<string,object> player = DatabaseManager.Instance.LoadPlayer(i[Constants.DB_KEYWORD_PLAYER_ID].ToString());
			player[Constants.DB_KEYWORD_COIN] = (int.Parse (player[Constants.DB_KEYWORD_COIN].ToString()) + prize);
			DatabaseManager.Instance.EditAccount(player);
		}

		if(winner != -1) {
			players[winner][Constants.DB_KEYWORD_MATCHES_WON] = int.Parse (players[winner][Constants.DB_KEYWORD_MATCHES_WON].ToString()) + 1;
			players[loser][Constants.DB_KEYWORD_MATCHES_LOST] = int.Parse (players[loser][Constants.DB_KEYWORD_MATCHES_LOST].ToString()) + 1;
		}
		else {
			players[winner][Constants.DB_KEYWORD_MATCHES_TIED] = int.Parse (players[winner][Constants.DB_KEYWORD_MATCHES_TIED].ToString()) + 1;
			players[loser][Constants.DB_KEYWORD_MATCHES_TIED] = int.Parse (players[loser][Constants.DB_KEYWORD_MATCHES_TIED].ToString()) + 1;
		}


		schedule[Constants.DB_KEYWORD_STATUS] = Constants.MATCH_STATUS_FINISHED;
		DatabaseManager.Instance.EditMatch(schedule);
	}

	private void LlamadoDehadoCheck(string matchId, IDictionary<string,object> bet) {
		if(bet[Constants.DB_COUCHBASE_ID].ToString() == DatabaseManager.Instance.LoadBettingOdds(0)[Constants.DB_COUCHBASE_ID].ToString()) {
			return;
		}

		IDictionary<string,object> match = DatabaseManager.Instance.LoadMatchById(matchId);
		if(match[Constants.DB_KEYWORD_BETTING_OPTION].ToString() != Constants.BETTING_OPTION_SPECTATOR_BETTING) {
			return;
		}

		List<IDictionary<string,object>> matchBets = DatabaseManager.Instance.LoadBets(matchId);
		int[] bets = {0, 0};

		foreach(IDictionary<string,object> i in matchBets) {
			if(i[Constants.DB_KEYWORD_BETTED_CHICKEN_ID].ToString() == match[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString()) {
				bets[0] += int.Parse(i[Constants.DB_KEYWORD_BET_AMOUNT].ToString());
			}
			else {
				bets[1] += int.Parse(i[Constants.DB_KEYWORD_BET_AMOUNT].ToString());
			}
		}
		if(bets[0] > bets[1]) {
			match[Constants.DB_KEYWORD_LLAMADO] = match[Constants.DB_KEYWORD_CHICKEN_ID_1].ToString();
		}
		else {
			match[Constants.DB_KEYWORD_LLAMADO] = match[Constants.DB_KEYWORD_CHICKEN_ID_2].ToString();
		}
		DatabaseManager.Instance.EditMatch(match);
	}

	// MATCHES END
}