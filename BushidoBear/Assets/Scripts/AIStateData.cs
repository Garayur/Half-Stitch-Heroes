using UnityEngine;
using System.Collections;

public class AIStateData {
	
	public AIState state;
	public AIBaseController owner;
	public GameObject target;

	public AIStateData(AIState newState, AIBaseController newOwner, GameObject newTarget) {
		state = newState;
		owner = newOwner;
		target = newTarget;
	}
}


