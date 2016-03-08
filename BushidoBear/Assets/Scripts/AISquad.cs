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

	protected override void DestroySelfOnSquadDeath() {
		if(AISquad.Count <= 0) {
			base.DestroySelfOnSquadDeath();
		}
	}

	protected override void AlertSquad(){
		foreach (BaseAIController ai in AISquad) {
			ai.AssignCenterPoint(new Vector2((xmax + xmin) /2, (zmax + zmin) / 2));
			ai.StartCombatPositioning();
		}
		
		StartCoroutine ("AssignMovementVector");
	}

}
