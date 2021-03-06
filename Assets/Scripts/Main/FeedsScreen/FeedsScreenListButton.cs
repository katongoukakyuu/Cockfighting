﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class FeedsScreenListButton : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler,  IDragHandler, IEndDragHandler, IScrollHandler {
	
	public ScrollRect MainScroll;
	private bool canClick = true;
	
	public void OnPointerDown (PointerEventData eventData)
	{
		if (MainScroll.velocity.magnitude > 2f) {
			canClick = false;
		}
		MainScroll.StopMovement();
	}
	
	public void OnPointerClick (PointerEventData eventData)
	{
		
		if(canClick) {
			FeedsManager.Instance.SetSelected (this.GetComponentInChildren<Text>().text);
		}
		canClick = true;
	}
	
	public void OnBeginDrag(PointerEventData eventData)
	{
		MainScroll.OnBeginDrag(eventData);
		canClick = false;
	}
	
	public void OnDrag(PointerEventData eventData)
	{
		MainScroll.OnDrag(eventData);
		canClick = false;
	}
	
	public void OnEndDrag(PointerEventData eventData)
	{
		MainScroll.OnEndDrag(eventData);
		canClick = true;
	}
	
	public void OnScroll(PointerEventData data)
	{
		MainScroll.OnScroll(data);
		canClick = false;
	}
}
