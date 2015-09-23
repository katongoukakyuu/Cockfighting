using UnityEngine;
using System.Collections;

public class ChickenFightAI : MonoBehaviour {

	public int index = -1;
	public int[] newDestination;
	public bool moveImmediately = false;
	public bool isCurrentlyMoving = true;
	public bool hasFainted = false;

	Animator anim;
	NavMeshAgent navMeshAgent;
	GridOverlay gridOverlay;
	int xSize, ySize;

	int animStateFaint = Animator.StringToHash("Faint");

	void Start() {
		anim = GetComponent<Animator>();
		navMeshAgent = GetComponent<NavMeshAgent>();
		gridOverlay = Camera.main.GetComponent<GridOverlay>();
		xSize = gridOverlay.GetTiles().GetLength(0);
		ySize = gridOverlay.GetTiles().GetLength(1);

		navMeshAgent.avoidancePriority = Random.Range (1,101);
	}

	void Update() {
		if(anim.GetCurrentAnimatorStateInfo(0).shortNameHash == animStateFaint) {
			navMeshAgent.Stop();
			newDestination[0] = -1;
			newDestination[1] = -1;
			isCurrentlyMoving = false;
			hasFainted = true;
		}
		else if(!navMeshAgent.hasPath && !hasFainted) {
			if(newDestination[0] != -1 && newDestination[1] != -1) {
				if(moveImmediately) {
					//print ("moving immediately, target is " + gridOverlay.GetTiles()[newDestination[0],newDestination[1]].transform.position);
					navMeshAgent.Warp(gridOverlay.GetTiles()[newDestination[0],newDestination[1]].transform.position);
					moveImmediately = false;
					newDestination[0] = -1;
					newDestination[1] = -1;
				}
				else {
					//print ("moving normally, target is " + gridOverlay.GetTiles()[newDestination[0],newDestination[1]].transform.position);
					navMeshAgent.SetDestination(gridOverlay.GetTiles()[newDestination[0],newDestination[1]].transform.position);
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
		print ("hit confirm!");
		ReplayManager.Instance.UpdateUI((index+1)%2, false, true);
	}
}
