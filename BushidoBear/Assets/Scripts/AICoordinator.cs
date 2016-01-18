using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AICoordinator : MonoBehaviour {

	public List<AIBaseController> AISquad;
	Dictionary<GameObject, List<AIBaseController>> aiTargetAssignments = new Dictionary<GameObject, List<AIBaseController>>();

	public float avoidanceDistance = 8;
	public float unavailableAvoidanceDistance = 4;
	public float maxDistance = 10;
	protected float movementUpdateInterval = 0.5f;
	protected float timer = 0.25f;
	protected bool alertTriggered = false;

	public int xmin = 1;
	public int xmax = 19;
	public int zmin = -5;
	public int zmax = 15;

	//temp destroy with ontriggerenter when box is no longer being used to trigger begincombat
	protected bool hasBeenTriggered = false;

	protected void OnEnable() {
		AIBaseController.OnAIStateChange += CheckSquadAssignments;
	}

	protected void OnDisable() {
		AIBaseController.OnAIStateChange -= CheckSquadAssignments;
	}

	void OnTriggerEnter(Collider other){
		if(!hasBeenTriggered){
			hasBeenTriggered = true;
			BeginCombat();
		}
	}

	void Start() {
		timer = movementUpdateInterval;
	}

	void Update() {
		if(alertTriggered) {
			timer -= Time.deltaTime;
			if(timer <= 0) {
				AssignMovementVector();
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
			if(aiTargetAssignments[target].Count <= 0)
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
			case ControllerState.StartingAnimation:
				AlertSquad();
				break;
			case ControllerState.Positioning:
				ReassignAI(aiState.owner, aiState.target);
				break;
			case ControllerState.Attacking:
				break;
			case ControllerState.Flinching:
				ReassignAI(aiState.owner, aiState.target);
				break;
			case ControllerState.Fallen:
				ReassignAI(aiState.owner, aiState.target);
				break;
			case ControllerState.Dying:
				ReassignAI(aiState.owner, aiState.target);
				break;
			case ControllerState.Dead:
				AISquad.Remove(aiState.owner);
				DestroySelfOnSquadDeath();
				break;
			}

		}
	}

	protected void AssignMovementVector(){
		Vector3 centroid, movementVector;

		centroid = CalculateCentroid();

		foreach(AIBaseController ai in AISquad){
			if(ai.IsAvailable()) {
				movementVector = CalculateMovementVector(centroid, CalculateRepulsion(ai.gameObject), ai.gameObject);
				movementVector = ApplyBoundaries(movementVector, ai.gameObject);
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

//		foreach (GameObject player in aiTargetAssignments.Keys ) {
//			centroid += player.transform.position;
//			count++;
//		}

		centroid.x = centroid.x / count;
		centroid.y = 0;
		centroid.z = centroid.z / count;

		return centroid;
	}

	protected Vector3 CalculateRepulsion(GameObject primary) {
		Vector3 repulsion = Vector3.zero;

		foreach (AIBaseController ai in AISquad) {
			if(ai.gameObject != primary) {
				if(AITooClose(primary, ai)) {
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

		return movementVector;
	}


	protected bool AITooClose(GameObject primary, AIBaseController secondary){
		float distance;

		if(secondary.IsAvailable())
			distance = avoidanceDistance;
		else
			distance = unavailableAvoidanceDistance;

		if(Vector3.Distance(primary.transform.position, secondary.gameObject.transform.position) > distance)
			return false;
		else {
			return true;
		}
	}

	protected Vector3 ApplyBoundaries(Vector3 movementVector, GameObject ai) {
		if(ai.transform.position.x < xmin) {
			movementVector.x = 10;
		}
		else if(ai.transform.position.x > xmax) {
			movementVector.x = -10;
		}

		if(ai.transform.position.z < zmin) {
			movementVector.z = 10;
		}
		else if (ai.transform.position.z > zmax) {
			movementVector.z = -10;
		}

		return movementVector;
	}
				
}
