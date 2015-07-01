using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ImagePanel : MonoBehaviour {

	public Image[] images;
	public float animDuration = 0.1f;
	public int animSteps = 10;

	private List<IDictionary<string, object>> bldgList;
	private int bldgIndex = 0;

	private float smallIconXSize;
	private float smallIconYSize;
	private float largeIconXSize;
	private float largeIconYSize;
	
	private float smallIconXPos;
	private float smallIconYPos;
	private float largeIconXPos;
	private float largeIconYPos;
	
	private bool isAnimating = false;

	void Start() {
		if (images.Length == 5) {
			smallIconXSize = images[1].rectTransform.rect.width;
			smallIconYSize = images[1].rectTransform.rect.height;
			largeIconXSize = images[2].rectTransform.rect.width;
			largeIconYSize = images[2].rectTransform.rect.height;
			
			smallIconXPos = images[1].rectTransform.anchoredPosition.x;
			smallIconYPos = images[1].rectTransform.anchoredPosition.y;
			largeIconXPos = images[2].rectTransform.anchoredPosition.x;
			largeIconYPos = images[2].rectTransform.anchoredPosition.y;
		}
	}

	public void SetBuildings(List<IDictionary<string, object>> bldgList) {
		this.bldgList = bldgList;
		UpdateSelection ();
	}

	public IDictionary<string, object> GetSelectedBuilding() {
		return bldgList [bldgIndex];
	}

	private void UpdateSelection() {
		int x = bldgIndex;
		print (bldgList[x][Constants.DB_KEYWORD_IMAGE_NAME]);
		images [2].sprite = Resources.Load ("Sprites/" + bldgList[x][Constants.DB_KEYWORD_IMAGE_NAME], typeof(Sprite)) as Sprite;

		if (bldgIndex-1 > 0)
			x = bldgIndex - 1;
		else if(bldgList.Count - 2 > 0)
			x = bldgList.Count - 2;
		else
			x = bldgList.Count - 1;
		images [0].sprite = Resources.Load ("Sprites/" + bldgList[x][Constants.DB_KEYWORD_IMAGE_NAME], typeof(Sprite)) as Sprite;

		if (bldgIndex > 0)
			x = bldgIndex - 1;
		else
			x = bldgList.Count - 1;
		images [1].sprite = Resources.Load ("Sprites/" + bldgList[x][Constants.DB_KEYWORD_IMAGE_NAME], typeof(Sprite)) as Sprite;

		if (bldgIndex + 1 < bldgList.Count)
			x = bldgIndex + 1;
		else
			x = 0;
		images [3].sprite = Resources.Load ("Sprites/" + bldgList[x][Constants.DB_KEYWORD_IMAGE_NAME], typeof(Sprite)) as Sprite;

		if (bldgIndex + 2 < bldgList.Count)
			x = bldgIndex + 2;
		else
			x = 0;
		images [3].sprite = Resources.Load ("Sprites/" + bldgList[x][Constants.DB_KEYWORD_IMAGE_NAME], typeof(Sprite)) as Sprite;
	}

	public void AnimateLeft() {
		if (!isAnimating)
			StartCoroutine (AnimateLeft(animDuration, animSteps));
	}

	private IEnumerator AnimateLeft(float animDuration, int animSteps) {
		float sizeIncX = (smallIconXSize - largeIconXSize) / animSteps;
		float sizeIncY = (smallIconYSize - largeIconYSize) / animSteps;
		float posIncX = (smallIconXPos - largeIconXPos) / animSteps;
		float posIncY = (smallIconYPos - largeIconYPos) / animSteps;
		float alphaInc = (0 - images[2].color.a) / animSteps;
		float animWait = animDuration / animSteps;
		Color c;
		
		isAnimating = true;
		
		images [4].gameObject.SetActive(true);
		c = images[4].color;
		c.a = 0;
		images[4].color = c;
		
		for(int x = 0; x < animSteps; x++) {
			c = images[1].color;
			c.a += alphaInc;
			images[1].color = c;
			
			images[2].rectTransform.sizeDelta = new Vector2(images[2].rectTransform.rect.width + sizeIncX,
			                                                images[2].rectTransform.rect.height + sizeIncY);
			images[2].rectTransform.anchoredPosition = new Vector2(images[2].rectTransform.anchoredPosition.x + posIncX,
			                                                       images[2].rectTransform.anchoredPosition.y + posIncY);
			
			images[3].rectTransform.sizeDelta = new Vector2(images[3].rectTransform.rect.width - sizeIncX,
			                                                images[3].rectTransform.rect.height - sizeIncY);
			images[3].rectTransform.anchoredPosition = new Vector2(images[3].rectTransform.anchoredPosition.x + posIncX,
			                                                       images[3].rectTransform.anchoredPosition.y + posIncY);
			
			c = images[4].color;
			c.a -= alphaInc;
			images[4].color = c;
			
			yield return new WaitForSeconds(animWait);
		}
		
		images [0].rectTransform.anchoredPosition = images [4].rectTransform.anchoredPosition;
		images [1].gameObject.SetActive (false);
		
		Image temp = images[0];
		for(int x = 0; x < 4; x++) {
			images[x] = images[x+1];
		}
		images [4] = temp;
		if (bldgIndex + 1 < bldgList.Count)
			bldgIndex++;
		else
			bldgIndex = 0;
		UpdateSelection ();
		isAnimating = false;
	}

	public void AnimateRight() {
		if (!isAnimating)
			StartCoroutine (AnimateRight(animDuration, animSteps));
	}

	private IEnumerator AnimateRight(float animDuration, int animSteps) {
		float sizeIncX = (smallIconXSize - largeIconXSize) / animSteps;
		float sizeIncY = (smallIconYSize - largeIconYSize) / animSteps;
		float posIncX = (smallIconXPos - largeIconXPos) / animSteps;
		float posIncY = (smallIconYPos - largeIconYPos) / animSteps;
		float alphaInc = (0 - images[2].color.a) / animSteps;
		float animWait = animDuration / animSteps;
		Color c;
		
		isAnimating = true;
		
		images [0].gameObject.SetActive(true);
		c = images[0].color;
		c.a = 0;
		images[0].color = c;
		
		for(int x = 0; x < animSteps; x++) {
			c = images[3].color;
			c.a += alphaInc;
			images[3].color = c;
			
			images[2].rectTransform.sizeDelta = new Vector2(images[2].rectTransform.rect.width + sizeIncX,
			                                                images[2].rectTransform.rect.height + sizeIncY);
			images[2].rectTransform.anchoredPosition = new Vector2(images[2].rectTransform.anchoredPosition.x - posIncX,
			                                                       images[2].rectTransform.anchoredPosition.y - posIncY);
			
			images[1].rectTransform.sizeDelta = new Vector2(images[1].rectTransform.rect.width - sizeIncX,
			                                                images[1].rectTransform.rect.height - sizeIncY);
			images[1].rectTransform.anchoredPosition = new Vector2(images[1].rectTransform.anchoredPosition.x - posIncX,
			                                                       images[1].rectTransform.anchoredPosition.y - posIncY);
			
			c = images[0].color;
			c.a -= alphaInc;
			images[0].color = c;
			
			yield return new WaitForSeconds(animWait);
		}
		
		images [4].rectTransform.anchoredPosition = images [0].rectTransform.anchoredPosition;
		images [3].gameObject.SetActive (false);
		
		Image temp = images[4];
		for(int x = 3; x >= 0; x--) {
			images[x+1] = images[x];
		}
		images [0] = temp;
		if (bldgIndex - 1 >= 0)
			bldgIndex--;
		else
			bldgIndex = bldgList.Count - 1;
		UpdateSelection ();
		isAnimating = false;
	}

}
