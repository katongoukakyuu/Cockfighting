using UnityEngine;
using System.Collections;

public class ChickenFight : MonoBehaviour {

	Animator anim;
	/*int velXHash = Animator.StringToHash("Velocity X");
	int velZHash = Animator.StringToHash("Velocity Y");
	int velAngleHash = Animator.StringToHash("Velocity Angle");
	int rotYHash = Animator.StringToHash("Rotation Y");
	int faintHash = Animator.StringToHash("Faint");
	int peckHash = Animator.StringToHash("Peck");
	int flyingTalonHash = Animator.StringToHash("Flying Talon");
	*/
	int posY = Animator.StringToHash("Position Y");


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(anim != null) {
			transform.position = transform.position + new Vector3(0,anim.GetFloat(posY),0);
		}

	}
}
