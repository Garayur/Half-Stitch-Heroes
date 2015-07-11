using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AICoordinator : MonoBehaviour {

	public List<AIBaseController> AISquad;
	Dictionary<GameObject, List<AIBaseController>> aiTargetAssignments = new Dictionary<GameObject, List<AIBaseController>>();
	

	protected void OnEnable() {
		AIBaseController.OnAIStateChange += CheckSquadAssignments;
	}

	protected void OnDisable() {
		AIBaseController.OnAIStateChange -= CheckSquadAssignments;
	}

	void OnTriggerEnter(Collider other){
		BeginCombat();
	}


	protected virtual void BeginCombat() {
		FindPlayers();
		AssignAttackers();
	}

	protected virtual void FindPlayers() {
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			aiTargetAssignments.Add(player, new List<AIBaseController>());
		}
	}

	protected void DestroySelfOnSquadDeath() {
		if(AISquad.Count <= 0) {
			Destroy(gameObject);
		}
	}

	protected void AssignAIToTarget(GameObject target) {
		AIBaseController closestAIToTarget = null;
		float closestDistanceToTarget = 1000f;
		float temporaryDistanceToTarget;

		foreach (AIBaseController AI in AISquad){
			if(AI.IsAvailable()) {
				temporaryDistanceToTarget = Vector3.Distance(AI.gameObject.transform.position, target.transform.position);

				if(temporaryDistanceToTarget < closestDistanceToTarget) {
					closestDistanceToTarget = temporaryDistanceToTarget;
					closestAIToTarget = AI;
				}
			}
		}
		if(closestAIToTarget != null) {
			aiTargetAssignments[target].Add(closestAIToTarget);
			closestAIToTarget.AttackNewTarget(target);
		}

	}

	protected void AssignAttackers() {
		foreach(GameObject target in aiTargetAssignments.Keys) {
			if(aiTargetAssignments[target].Count <= 0)
				AssignAIToTarget(target);
		}
	}

	protected void ReassignAI(AIBaseController ai, GameObject target) {
		if(target != null) {
			aiTargetAssignments[target].Remove(ai);
			AssignAIToTarget(target);
		}
	}

	protected void AlertSquad(){
		foreach (AIBaseController ai in AISquad) {
			ai.StartCombatPositioning();
		}
	}

	protected void CheckSquadAssignments(AIStateData aiState) {
		if(AISquad.Contains(aiState.owner)) {

			switch (aiState.state) {
			case AIState.StartingAnimation:
				AlertSquad();
				break;
			case AIState.Positioning:
				ReassignAI(aiState.owner, aiState.target);
				break;
			case AIState.Attacking:
				break;
			case AIState.Flinching:
				ReassignAI(aiState.owner, aiState.target);
				break;
			case AIState.Fallen:
				ReassignAI(aiState.owner, aiState.target);
				break;
			case AIState.Dying:
				ReassignAI(aiState.owner, aiState.target);
				break;
			case AIState.Dead:
				AISquad.Remove(aiState.owner);
				DestroySelfOnSquadDeath();
				break;
			}

		}
	}
	
}
