using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StoreManager : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas storeCanvas;
	public Animator storeAnimator;

	public GameObject storePanel;
	public GameObject detailsPanel;

	public GameObject storeIcon;
	public GameObject buyButton;

	private List<IDictionary<string,object>> items;
	private List<GameObject> listItems = new List<GameObject>();
	private IDictionary<string,object> selectedItem;

	private static StoreManager instance;
	private StoreManager() {}
	
	public static StoreManager Instance {
		get {
			if(instance == null) {
				instance = (StoreManager)GameObject.FindObjectOfType(typeof(StoreManager));
			}
			return instance;
		}
	}

	public void Initialize(int x) {
		foreach (GameObject g in listItems) {
			Destroy (g);
		}
		listItems.Clear ();

		List<IDictionary<string,object>> filteredItems;
		switch(x) {
		case 0:
			items = DatabaseManager.Instance.LoadStoreItems(null);
			filteredItems = new List<IDictionary<string,object>>(items);
			break;
		case 1:
			filteredItems = items.Where(p => p[Constants.DB_KEYWORD_SUBTYPE].ToString() == Constants.DB_TYPE_FEEDS).ToList();
			break;
		case 2:
			filteredItems = items.Where(p => p[Constants.DB_KEYWORD_SUBTYPE].ToString() == Constants.DB_TYPE_TREATS).ToList();
			break;
		case 3:
			filteredItems = items.Where(p => p[Constants.DB_KEYWORD_SUBTYPE].ToString() == Constants.DB_TYPE_VITAMINS).ToList();
			break;
		case 4:
			filteredItems = items.Where(p => p[Constants.DB_KEYWORD_SUBTYPE].ToString() == Constants.DB_TYPE_SHOTS).ToList();
			break;
		default:
			items = DatabaseManager.Instance.LoadStoreItems(null);
			filteredItems = new List<IDictionary<string,object>>(items);
			break;
		}


		foreach (IDictionary<string, object> i in filteredItems) {
			GameObject g = Instantiate (storeIcon);
			listItems.Add (g);
			g.name = i[Constants.DB_COUCHBASE_ID].ToString();
			g.transform.GetChild(0).GetComponent<Image>().sprite = 
				Resources.Load<Sprite> (
					Constants.PATH_SPRITES+i[Constants.DB_KEYWORD_IMAGE_NAME].ToString()
				);
			g.SetActive(true);
			g.transform.SetParent (storePanel.transform,false);
		}
	}

	public void SetSelectedItem(string s) {
		foreach(IDictionary<string,object> i in items) {
			if(s == i[Constants.DB_COUCHBASE_ID].ToString()) {
				selectedItem = i;
				break;
			}
		}

		detailsPanel.transform.FindChild(Constants.STORE_DETAILS_PANEL_ICON).gameObject.SetActive(true);
		detailsPanel.transform.FindChild(Constants.STORE_DETAILS_PANEL_NAME).gameObject.SetActive(true);
		detailsPanel.transform.FindChild(Constants.STORE_DETAILS_PANEL_CATEGORY).gameObject.SetActive(true);
		detailsPanel.transform.FindChild(Constants.STORE_DETAILS_PANEL_PRICE).gameObject.SetActive(true);
		detailsPanel.transform.FindChild(Constants.STORE_DETAILS_PANEL_DESCRIPTION).gameObject.SetActive(true);

		detailsPanel.transform.FindChild(Constants.STORE_DETAILS_PANEL_ICON_IMAGE).GetComponent<Image>().sprite = 
			Resources.Load<Sprite> (Constants.PATH_SPRITES+selectedItem[Constants.DB_KEYWORD_IMAGE_NAME].ToString());
		detailsPanel.transform.FindChild(Constants.STORE_DETAILS_PANEL_NAME).GetComponent<Text>().text = selectedItem[Constants.DB_KEYWORD_NAME].ToString();
		detailsPanel.transform.FindChild(Constants.STORE_DETAILS_PANEL_CATEGORY).GetComponent<Text>().text = Utility.ToProperCase(selectedItem[Constants.DB_KEYWORD_SUBTYPE].ToString());
		detailsPanel.transform.FindChild(Constants.STORE_DETAILS_PANEL_COIN_PRICE).GetComponent<Text>().text = selectedItem[Constants.DB_KEYWORD_COIN_COST].ToString();
		detailsPanel.transform.FindChild(Constants.STORE_DETAILS_PANEL_CASH_PRICE).GetComponent<Text>().text = selectedItem[Constants.DB_KEYWORD_CASH_COST].ToString();
		detailsPanel.transform.FindChild(Constants.STORE_DETAILS_PANEL_DESCRIPTION).GetComponent<Text>().text = selectedItem[Constants.DB_KEYWORD_DESCRIPTION].ToString();
	}

	public void ButtonBuyCoin() {

	}

	public void ButtonBuyCash() {
		
	}

	public void ButtonBack() {
		storeAnimator.SetBool("isHidden", true);
		Invoke ("BackButtonFunction",0.2f);
	}

	void BackButtonFunction()
	{
		CameraControls.Instance.freeCamera = true;
		mainCanvas.gameObject.SetActive (true);
		storeCanvas.gameObject.SetActive (false);
	}
}