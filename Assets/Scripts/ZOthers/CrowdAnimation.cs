using UnityEngine;
using System.Collections;

public class CrowdAnimation : MonoBehaviour {

	public Animator crowdAnimation;

	public int animTrack = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(animTrack == 1)
		{
			crowdAnimation.SetBool("isClapping", true);
		}

		if(animTrack == 2)
		{
			crowdAnimation.SetBool("isIdleStance",true);
		}
	
	}
}
