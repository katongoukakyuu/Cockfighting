using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class FightManager : MonoBehaviour {

	public Canvas fightCanvas;
	public Canvas messageCanvas;

	public GameObject listPanel;

	private string state = Constants.FEEDS_MANAGER_STATE_FREE_SELECT;

	public Button matchmakingCategoryButton;
	private List<Button> listMMCategoryButtons = new List<Button> ();
	private List<IDictionary<string,object>> listMMCategories = new List<IDictionary<string,object>> ();
	private IDictionary<string,object> selectedMMCategory;

	private List<IEnumerator> countdowns = new List<IEnumerator>();

	private delegate void ButtonDelegate();

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
		Initialize ();
	}

	public void Initialize() {
		foreach (Button b in listMMCategoryButtons) {
			Destroy (b.gameObject);
		}
		foreach (IEnumerator ie in countdowns) {
			StopCoroutine(ie);
		}
		listMMCategoryButtons.Clear ();
		countdowns.Clear ();

		listMMCategories = DatabaseManager.Instance.LoadMatchmakingCategories ();
		foreach (IDictionary<string, object> i in listMMCategories) {
			Button b = Instantiate(matchmakingCategoryButton);
			listMMCategoryButtons.Add (b);
			b.GetComponentInChildren<Text> ().text = i[Constants.DB_KEYWORD_NAME].ToString();
			b.transform.SetParent(listPanel.transform,false);
		}
	}

	public void SetSelected(string s) {

	}

	public void ButtonBack() {
		Application.LoadLevel (Constants.SCENE_FARM);
	}

	private void DisplayMessage(string title, string message, ButtonDelegate bd) {
		messageCanvas.gameObject.SetActive(true);
		GameObject.Find("Title Text").GetComponent<Text>().text = title;
		GameObject.Find("Message Text").GetComponent<Text>().text = message;

		GameObject okButton = GameObject.Find("Msg OK Button").gameObject;
		EventTrigger trigger = okButton.GetComponentInParent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			bd ();
			ClearMessage();
		});
		trigger.triggers.Add (entry);

		GameObject cancelButton = GameObject.Find("Msg Cancel Button").gameObject;
		trigger = cancelButton.GetComponentInParent<EventTrigger> ();
		entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener ((eventData) => {
			ClearMessage();
		});
		trigger.triggers.Add (entry);
	}

	private void ClearMessage() {
		GameObject okButton = GameObject.Find("Msg OK Button").gameObject;
		EventTrigger trigger = okButton.GetComponentInParent<EventTrigger> ();
		GameObject cancelButton = GameObject.Find("Msg Cancel Button").gameObject;
		EventTrigger trigger2 = cancelButton.GetComponentInParent<EventTrigger> ();

		GameObject.Find("Title Text").GetComponent<Text>().text = "";
		GameObject.Find("Message Text").GetComponent<Text>().text = "";
		trigger.triggers.Clear();
		trigger2.triggers.Clear();
		messageCanvas.gameObject.SetActive(false);
	}

	private System.DateTime TrimMilli(System.DateTime dt)
	{
		return new System.DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, dt.Kind);
	}

	private IEnumerator DisplayCountdown(Text display, System.DateTime target) {
		while (true) {
			System.TimeSpan diff = target - TrimMilli (System.DateTime.Now.ToUniversalTime ());
			if (diff.CompareTo (System.TimeSpan.Zero) <= 0) {
				display.text = "" + System.TimeSpan.Zero;
				yield break;
			}
			display.text = "" + (target - TrimMilli(System.DateTime.Now.ToUniversalTime()));
			yield return new WaitForSeconds(1f);
		}
	}
}