using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AICoordinator : MonoBehaviour {

	public List<AIBaseController> AISquad;
	Dictionary<GameObject, List<AIBaseController>> aiTargetAssignments = new Dictionary<GameObject, List<AIBaseController>>();
	

	protected void Start(){
		FindPlayers();
		AssignAttackers();

	}

	protected void OnEnable() {
		AIBaseController.OnAIStateChange += CheckSquadAssignments;
	}

	protected void OnDisable() {
		AIBaseController.OnAIStateChange -= CheckSquadAssignments;
	}

	protected void Update() {

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
		}

	}

	protected void AssignAttackers() {
		foreach(GameObject target in aiTargetAssignments.Keys) {
			if(aiTargetAssignments[target].Count <= 0)
				AssignAIToTarget(target);
		}
	}

	protected void CheckSquadAssignments(AIStateData aiState) {
		if(aiState.state != AIState.Combat)
			aiTargetAssignments[aiState.target].Remove(aiState.owner);

		AssignAIToTarget(aiState.target);

		DestroySelfOnSquadDeath();
	}
	
}
