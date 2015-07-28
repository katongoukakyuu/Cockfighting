using UnityEngine;
using System.Collections;

public class MobileSwipe : MonoBehaviour {

	public int cameraSpeed = 100;
	private Touch initialTouch = new Touch();
	private float distance = 0;
	private bool hasSwiped = false;
	
	void FixedUpdate()
	{
		foreach(Touch t in Input.touches)
		{
			if (t.phase == TouchPhase.Began)
			{
				initialTouch = t;
			}
			else if (t.phase == TouchPhase.Moved && !hasSwiped)
			{
				float deltaX = initialTouch.position.x - t.position.x;
				float deltaY = initialTouch.position.y - t.position.y;
				distance = Mathf.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
				bool swipedSideways = Mathf.Abs(deltaX) > Mathf.Abs(deltaY);
				
				if (distance > 0f)
				{
					if (swipedSideways && deltaX > 0) //swiped left
					{
						this.transform.Translate(Vector3.right  * cameraSpeed * Time.deltaTime);
					}
					else if (swipedSideways && deltaX <= 0) //swiped right
					{
						this.transform.Translate(Vector3.left  * cameraSpeed * Time.deltaTime);
					}
					else if (!swipedSideways && deltaY > 0) //swiped down
					{
						this.transform.Translate(Vector3.forward  * cameraSpeed * Time.deltaTime, Space.World);
					}
					else if (!swipedSideways && deltaY <= 0)  //swiped up
					{
						this.transform.Translate(Vector3.back  * cameraSpeed * Time.deltaTime, Space.World);
					}
					
					hasSwiped = true;
				}
				
			}
			else if (t.phase == TouchPhase.Ended)
			{
				initialTouch = new Touch();
				hasSwiped = false;
			}
		}
	}
}