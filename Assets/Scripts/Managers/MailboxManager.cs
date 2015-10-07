using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MailboxManager : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas mailboxCanvas;
	public Animator MailAnimator;

	public GameObject mailboxPanelMain;
	public GameObject mailPanel;
	public GameObject mailboxPanel;
	public GameObject replayButton;
	public GameObject deleteButton;

	private string state = Constants.MAILBOX_MANAGER_STATE_VIEW_LIST;

	public GameObject mailboxEntry;
	private List<GameObject> listMailboxEntries = new List<GameObject> ();
	private List<IDictionary<string,object>> listEntries = new List<IDictionary<string,object>> ();
	private IDictionary<string,object> selectedEntry;

	private static MailboxManager instance;
	private MailboxManager() {}
	
	public static MailboxManager Instance {
		get {
			if(instance == null) {
				instance = (MailboxManager)GameObject.FindObjectOfType(typeof(MailboxManager));
			}
			return instance;
		}
	}

	public void Initialize() {
		mailboxPanelMain.gameObject.SetActive(true);
		mailPanel.gameObject.SetActive(false);
		deleteButton.GetComponent<Button>().interactable = false;

		foreach (GameObject g in listMailboxEntries) {
			Destroy (g);
		}

		listMailboxEntries.Clear ();

		listEntries = DatabaseManager.Instance.LoadMails (PlayerManager.Instance.player[Constants.DB_COUCHBASE_ID].ToString());
		foreach (IDictionary<string, object> i in listEntries) {
			GameObject g = Instantiate(mailboxEntry);
			listMailboxEntries.Add (g);
			g.name = i[Constants.DB_COUCHBASE_ID].ToString();
			g.transform.FindChild(Constants.MAIL_PANEL_LIST_TITLE).GetComponent<Text>().text = i[Constants.DB_KEYWORD_TITLE].ToString();
			g.transform.FindChild(Constants.MAIL_PANEL_LIST_MESSAGE).GetComponent<Text>().text = i[Constants.DB_KEYWORD_MESSAGE].ToString();
			g.transform.SetParent(mailboxPanel.transform,false);
		}
	}

	public void SetSelected(string s) {
		state = Constants.MAILBOX_MANAGER_STATE_VIEW_MAIL;
		mailboxPanelMain.gameObject.SetActive (false);
		mailPanel.gameObject.SetActive (true);

		foreach(IDictionary<string,object> id in listEntries) {
			if(id[Constants.DB_COUCHBASE_ID].ToString() == s) {
				selectedEntry = id;
				break;
			}
		}
		LoadEntry();
	}

	private void LoadEntry() {
		mailPanel.transform.FindChild(Constants.MAIL_PANEL_MAIL_TITLE).GetComponent<Text>().text = selectedEntry[Constants.DB_KEYWORD_TITLE].ToString();
		mailPanel.transform.FindChild(Constants.MAIL_PANEL_MAIL_FROM).GetComponent<Text>().text = selectedEntry[Constants.DB_KEYWORD_FROM].ToString();
		mailPanel.transform.FindChild(Constants.MAIL_PANEL_MAIL_TO).GetComponent<Text>().text = selectedEntry[Constants.DB_KEYWORD_TO].ToString();
		mailPanel.transform.FindChild(Constants.MAIL_PANEL_MAIL_MESSAGE).GetComponent<Text>().text = selectedEntry[Constants.DB_KEYWORD_MESSAGE].ToString();

		List<string> mailTypes = (selectedEntry [Constants.DB_KEYWORD_MAIL_TYPE] as Newtonsoft.Json.Linq.JArray).ToObject<List<string>> ();
		mailPanel.transform.FindChild(Constants.MAIL_PANEL_MAIL_FROM).gameObject.SetActive(!mailTypes.Contains(Constants.MAIL_TYPE_NOTIFICATION));
		mailPanel.transform.FindChild(Constants.MAIL_PANEL_MAIL_TO).gameObject.SetActive(!mailTypes.Contains(Constants.MAIL_TYPE_NOTIFICATION));
		replayButton.gameObject.SetActive(mailTypes.Contains(Constants.MAIL_TYPE_REPLAY));

		deleteButton.GetComponent<Button>().interactable = true;
	}

	public void CheckDeleteCount() {
		foreach(GameObject g in listMailboxEntries) {
			if(g.transform.FindChild(Constants.MAIL_PANEL_LIST_TOGGLE).GetComponent<Toggle>().isOn) {
				deleteButton.GetComponent<Button>().interactable = true;
				return;
			};
		}
		deleteButton.GetComponent<Button>().interactable = false;
	}

	public void ButtonBack() {
		switch(state) {
		case Constants.MAILBOX_MANAGER_STATE_VIEW_LIST:
			MailAnimator.SetBool ("isHidden",true);
			Invoke ("MainButtonFunction", 0.2f);
			break;
		case Constants.MAILBOX_MANAGER_STATE_VIEW_MAIL:
			state = Constants.MAILBOX_MANAGER_STATE_VIEW_LIST;
			mailboxPanelMain.gameObject.SetActive (true);
			mailPanel.gameObject.SetActive (false);
			
			CheckDeleteCount();
			replayButton.gameObject.SetActive(false);
			break;
		default:
			break;
		}
	}

	void MainButtonFunction(){
		CameraControls.Instance.freeCamera = true;
		mainCanvas.gameObject.SetActive (true);
		mailboxCanvas.gameObject.SetActive (false);
	}

	public void ButtonReplay() {
		PlayerManager.Instance.selectedReplay = DatabaseManager.Instance.LoadReplay(selectedEntry[Constants.DB_KEYWORD_REPLAY_ID].ToString());
		Application.LoadLevel(Constants.SCENE_FIGHT_RING);
	}

	public void ButtonDelete() {
		mainCanvas.gameObject.SetActive (true);
		mailboxCanvas.gameObject.SetActive (false);
	}

	public void ButtonCheckAll() {
		foreach(GameObject g in listMailboxEntries) {
			g.transform.FindChild(Constants.MAIL_PANEL_LIST_TOGGLE).GetComponent<Toggle>().isOn = true;
		}
	}

	public void ButtonUncheckAll() {
		foreach(GameObject g in listMailboxEntries) {
			g.transform.FindChild(Constants.MAIL_PANEL_LIST_TOGGLE).GetComponent<Toggle>().isOn = false;
		}
	}
}