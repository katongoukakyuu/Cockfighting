using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StoreScreenQuantitySlider : SliderText {

	public Text[] playerCurrencyText;
	public Text[] itemCostText;
	public Button okButton;

	[HideInInspector] public IDictionary<string,object> item;
	private int[] costs = new int[2];

	public void Initialize(IDictionary<string,object> item) {
		this.item = item;
		costs[0] = int.Parse(item[Constants.DB_KEYWORD_COIN_COST].ToString());
		costs[1] = int.Parse(item[Constants.DB_KEYWORD_CASH_COST].ToString());
		itemCostText[0].text = "" + costs[0];
		itemCostText[1].text = "" + costs[1];
	}

	public override void OnChange() {
		base.OnChange();

		for(int x = 0; x < playerCurrencyText.Length; x++) {
			if(itemCostText[x].isActiveAndEnabled) {
				itemCostText[x].text = "" + (int.Parse(text.text) * costs[x]);
				if(int.Parse(text.text) * costs[x] > int.Parse (playerCurrencyText[x].text)) {
					okButton.interactable = false;
					return;
				}
			}
		}
		okButton.interactable = true;
	}

}

