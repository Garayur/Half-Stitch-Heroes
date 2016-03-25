using UnityEngine;
using System.Collections;

public class AIStateData {
	
	public ControllerState state;
	public BaseAIController owner;
	public GameObject target;

	public AIStateData(ControllerState newState, BaseAIController newOwner, GameObject newTarget) {
		state = newState;
		owner = newOwner;
		target = newTarget;
	}
}


