using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AICoordinator : MonoBehaviour {

	public List<AIBaseController> AISquad;
	Dictionary<GameObject, List<AIBaseController>> aiTargetAssignments = new Dictionary<GameObject, List<AIBaseController>>();

	public float avoidanceDistance = 10;
	public float maxDistance = 10;
	protected float movementUpdateInterval = 0.35f;
	protected float timer = 0.25f;
	protected bool alertTriggered = false;

	public int xmin, xmax, zmin, zmax;

	protected void OnEnable() {
		AIBaseController.OnAIStateChange += CheckSquadAssignments;
	}

	protected void OnDisable() {
		AIBaseController.OnAIStateChange -= CheckSquadAssignments;
	}

	void OnTriggerEnter(Collider other){
		BeginCombat();
	}

	void Start() {
		timer = movementUpdateInterval;
	}

	void Update() {
		if(alertTriggered) {
			timer -= Time.deltaTime;
			if(timer <= 0) {
				AssignNormalizedMovementVector();
				timer = movementUpdateInterval;
			}
		}
	}


	protected virtual void BeginCombat() {
		FindPlayers();
		AlertSquad();
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

		alertTriggered = true;
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

	protected void AssignNormalizedMovementVector(){
		Vector3 centroid, movementVector;

		centroid = CalculateCentroid();

		foreach(AIBaseController ai in AISquad){
			if(ai.IsAvailable()) {
				movementVector = CalculateMovementVector(centroid, CalculateRepulsion(ai.gameObject), ai.gameObject);
				ai.AssignMovementVector(movementVector);
			}
		}

	}

	protected Vector3 CalculateCentroid() {
		Vector3 centroid = Vector3.zero;
		int count = 0;

		foreach (AIBaseController ai in AISquad) {
			if(ai.IsAvailable()) {
				centroid += ai.gameObject.transform.position;
				count++;
			}
		}

		foreach (GameObject player in aiTargetAssignments.Keys ) {
			centroid += player.transform.position;
			count++;
		}

		centroid.x = centroid.x / count;
		centroid.y = 0;
		centroid.z = centroid.z / count;

		return centroid;
	}

	protected Vector3 CalculateRepulsion(GameObject primary) {
		Vector3 repulsion = Vector3.zero;

		foreach (AIBaseController ai in AISquad) {
			if(ai.gameObject != primary) {
				if(TooClose(primary, ai.gameObject)) {
					repulsion += primary.transform.position - ai.gameObject.transform.position;
				}
			}
		}

		return repulsion;

	}

	protected Vector3 CalculateMovementVector(Vector3 centroid, Vector3 repulsion, GameObject primary) {
		Vector3 movementVector = Vector3.zero;

		if(Vector3.Distance(centroid, primary.transform.position) > maxDistance) {
			movementVector += centroid - primary.transform.position;
		}

		movementVector += repulsion;

		return Vector3.Normalize(movementVector);
	}


	protected bool TooClose(GameObject primary, GameObject secondary){
		if(Vector3.Distance(primary.transform.position, secondary.transform.position) > avoidanceDistance)
			return false;
		else {
			return true;
		}
	}
				
}
