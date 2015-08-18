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
	}

	public void StopAllProcesses() {
		foreach(IEnumerator ie in listCoroutines) {
			StopCoroutine(ie);
		}
	}

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
		
		chicken[Constants.DB_KEYWORD_ATTACK] = "" + (int.Parse (chicken[Constants.DB_KEYWORD_ATTACK].ToString()) + int.Parse (feeds[Constants.DB_KEYWORD_ATTACK].ToString()));
		chicken[Constants.DB_KEYWORD_DEFENSE] = "" + (int.Parse (chicken[Constants.DB_KEYWORD_DEFENSE].ToString()) + int.Parse (feeds[Constants.DB_KEYWORD_DEFENSE].ToString()));
		chicken[Constants.DB_KEYWORD_HP] = "" + (int.Parse (chicken[Constants.DB_KEYWORD_HP].ToString()) + int.Parse (feeds[Constants.DB_KEYWORD_HP].ToString()));
		chicken[Constants.DB_KEYWORD_AGILITY] = "" + (int.Parse (chicken[Constants.DB_KEYWORD_AGILITY].ToString()) + int.Parse (feeds[Constants.DB_KEYWORD_AGILITY].ToString()));
		chicken[Constants.DB_KEYWORD_GAMENESS] = "" + (int.Parse (chicken[Constants.DB_KEYWORD_GAMENESS].ToString()) + int.Parse (feeds[Constants.DB_KEYWORD_GAMENESS].ToString()));
		chicken[Constants.DB_KEYWORD_AGGRESSION] = "" + (int.Parse (chicken[Constants.DB_KEYWORD_AGGRESSION].ToString()) + int.Parse (feeds[Constants.DB_KEYWORD_AGGRESSION].ToString()));

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
						print("Schedule " + change.DocumentId + " has been canceled! Details below.");
						foreach(KeyValuePair<string,object> kv in properties) {
							print (kv.Key + ": " + kv.Value);
						}
						FlagScheduleAsCanceled(properties);
					}
				}
			}
		};
		yield break;
	}

	private void FlagScheduleAsCanceled(IDictionary<string,object> schedule) {
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

}
