using UnityEngine;
using System.Collections;

public class ChickenFightAI : MonoBehaviour {

	public int index = -1;
	public int[] newDestination;
	public bool moveImmediately = false;
	public bool isCurrentlyMoving = true;
	public bool isAttacking = false;
	public bool hasFainted = false;

	Animator anim;
	NavMeshAgent navMeshAgent;
	GridOverlay gridOverlay;
	int[] oldDestination = new int[2];
	Vector3 targetPosition;

	int animStateFaint = Animator.StringToHash("Faint");

	void Start() {
		anim = GetComponent<Animator>();
		navMeshAgent = GetComponent<NavMeshAgent>();
		gridOverlay = GameObject.Find("Main Camera").GetComponent<GridOverlay>();

		navMeshAgent.avoidancePriority = Random.Range (1,101);
	}

	void FixedUpdate() {
		if(anim.GetCurrentAnimatorStateInfo(0).shortNameHash == animStateFaint) {
			navMeshAgent.Stop();
			newDestination[0] = -1;
			newDestination[1] = -1;
			isCurrentlyMoving = false;
			hasFainted = true;
		}
		else if((!navMeshAgent.hasPath || (Vector3.Distance(transform.position,targetPosition) < 0.1f && !isAttacking)) 
		        && !hasFainted /*&& !isAttacking*/) {
			if(!isAttacking) {
				navMeshAgent.ResetPath();
			}
			if(newDestination[0] != -1 && newDestination[1] != -1) {
				if(moveImmediately) {
					// print ("moving immediately, target is " + gridOverlay.GetTiles()[newDestination[0],newDestination[1]].transform.position);
					navMeshAgent.Warp(gridOverlay.GetTiles()[newDestination[0],newDestination[1]].transform.position);
					moveImmediately = false;
					oldDestination[0] = newDestination[0];
					oldDestination[1] = newDestination[1];
					newDestination[0] = -1;
					newDestination[1] = -1;
				}
				else {
					// print ("moving normally, target is " + gridOverlay.GetTiles()[newDestination[0],newDestination[1]].transform.position);
					navMeshAgent.SetDestination(gridOverlay.GetTiles()[newDestination[0],newDestination[1]].transform.position);
					targetPosition = gridOverlay.GetTiles()[newDestination[0],newDestination[1]].transform.position;
					oldDestination[0] = newDestination[0];
					oldDestination[1] = newDestination[1];
					newDestination[0] = -1;
					newDestination[1] = -1;
				}
				isCurrentlyMoving = true;
			}
			else {
				isCurrentlyMoving = false;
			}
		}
		else {
			isCurrentlyMoving = true;
		}

		anim.SetFloat("Velocity Z", navMeshAgent.velocity.magnitude);
	}

	public void HitConfirm() {
		// print ("hit confirm!");
		ReplayManager.Instance.UpdateUI((index+1)%2, false, true);
	}

	public void HitFinish() {
		if(oldDestination[0] != -1 && oldDestination[1] != -1 && isAttacking) {
			navMeshAgent.SetDestination(gridOverlay.GetTiles()[oldDestination[0],oldDestination[1]].transform.position);
			isAttacking = false;
		}
	}
}
