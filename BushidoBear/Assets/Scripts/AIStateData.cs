using UnityEngine;
using System.Collections;

public class AIStateData {
	
	public ControllerState state;
	public AIBaseController owner;
	public GameObject target;

	public AIStateData(ControllerState newState, AIBaseController newOwner, GameObject newTarget) {
		state = newState;
		owner = newOwner;
		target = newTarget;
	}
}


