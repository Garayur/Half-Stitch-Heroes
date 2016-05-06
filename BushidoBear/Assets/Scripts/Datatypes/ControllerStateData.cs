using UnityEngine;
using System.Collections;

public class ControllerStateData {
	
	public ControllerState state;
	public BaseAIController owner;
	public GameObject target;

	public ControllerStateData(ControllerState newState, BaseAIController newOwner, GameObject newTarget) {
		state = newState;
		owner = newOwner;
		target = newTarget;
	}
}


