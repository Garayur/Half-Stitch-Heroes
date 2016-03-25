using UnityEngine;
using System.Collections;

public class AISquad : BaseAICoordinator {



	protected override void OnTriggerEnter(Collider other){
		if(!hasBeenTriggered){
			BeginCombat();
			base.OnTriggerEnter (other);
		}
	}

	protected virtual void BeginCombat() {
		FindPlayers();
		AlertSquad();
		AssignAttackers();
		StartCoroutine ("AssignMovementVector");
	}
		

	protected override void AlertSquad(){
		foreach (BaseAIController ai in AISquad) {
			ai.AssignCenterPoint(new Vector2((boundaries.leftBoundary + boundaries.rightBoundary) /2, (boundaries.leftBoundary + boundaries.rightBoundary) / 2));
			ai.StartCombatPositioning();
		}
		
		StartCoroutine ("AssignMovementVector");
	}

}
