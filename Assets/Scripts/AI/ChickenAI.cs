using UnityEngine;
using System.Collections;

public class ChickenAI : MonoBehaviour {

	Animator anim;
	NavMeshAgent navMeshAgent;
	GridOverlay gridOverlay;
	int xSize, ySize;


	void Start() {
		anim = GetComponent<Animator>();
		navMeshAgent = GetComponent<NavMeshAgent>();
		gridOverlay = Camera.main.GetComponent<GridOverlay>();
		xSize = gridOverlay.GetTiles().GetLength(0);
		ySize = gridOverlay.GetTiles().GetLength(1);

		navMeshAgent.avoidancePriority = Random.Range (1,101);
	}

	void Update() {
		if(!navMeshAgent.hasPath) {
			navMeshAgent.SetDestination(gridOverlay.GetTiles()[Random.Range(0,xSize),Random.Range(0,ySize)].transform.position);
		}

		anim.SetFloat("Velocity Z", navMeshAgent.velocity.magnitude);
	}
}
