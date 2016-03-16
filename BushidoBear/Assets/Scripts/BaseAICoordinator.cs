using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseAICoordinator : MonoBehaviour {

	public List<BaseAIController> AISquad;
	Dictionary<GameObject, List<BaseAIController>> aiTargetAssignments = new Dictionary<GameObject, List<BaseAIController>>();

	public float avoidanceDistance = 4;
	public float unavailableAvoidanceDistance = 2;
	public float maxDistance = 10;

	public int xmin = -6;
	public int xmax = 6;
	public int zmin = -1;
	public int zmax = 14;

	public delegate void CoordinatorDead (BaseAICoordinator coordinator);
	public static CoordinatorDead CoordinatorDeath; 

	protected float movementUpdateInterval = 0.3f;
	protected bool hasBeenTriggered = false;



	protected virtual void OnEnable() {
		BaseAIController.OnAIStateChange += CheckSquadAssignments;
	}

	protected virtual void OnDisable() {
		BaseAIController.OnAIStateChange -= CheckSquadAssignments;
	}

	protected virtual void OnTriggerEnter(Collider other){
		if(!hasBeenTriggered){
			hasBeenTriggered = true;
		}
	}

	protected virtual void FindPlayers() { //might need to be updated when players can die and respawn, call on player revival?
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			aiTargetAssignments.Add(player, new List<BaseAIController>());
		}
	}

	protected virtual void DestroySelfOnSquadDeath() {
		CoordinatorDeath (this);
		StopCoroutine("AssignMovementVector");
		Destroy(this);
	}

	protected virtual void AssignAIToTarget(GameObject target) {
		BaseAIController closestAIToTarget = null;
		float closestDistanceToTarget = 1000f;
		float temporaryDistanceToTarget;

		foreach (BaseAIController AI in AISquad){
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

	protected virtual void AssignAttackers() { 
		foreach(GameObject target in aiTargetAssignments.Keys) {
			if(aiTargetAssignments[target].Count <= 0)
				AssignAIToTarget(target);
		}
	}

	protected virtual void ReassignAI(BaseAIController ai, GameObject target) {
		if(target != null) {
			aiTargetAssignments[target].Remove(ai);
			if(aiTargetAssignments[target].Count <= 0)
				AssignAIToTarget(target);
		}
	}

	protected virtual void AlertSquad(){
	}

	//update with new states
	protected virtual void CheckSquadAssignments(AIStateData aiState) {
		if(AISquad.Contains(aiState.owner)) {

			switch (aiState.state) {
			case ControllerState.StartingAnimation:
				AlertSquad();
				break;
			case ControllerState.Attacking:
			case ControllerState.Grappled:
				break;
			case ControllerState.Dead:
				AISquad.Remove(aiState.owner);
				DestroySelfOnSquadDeath();
				break;
			default:
				ReassignAI(aiState.owner, aiState.target);
				break;
			}

		}
	}

	protected IEnumerator AssignMovementVector(){
		yield return new WaitForSeconds (movementUpdateInterval);
		Vector3 centroid, movementVector;

		centroid = CalculateCentroid();

		foreach(BaseAIController ai in AISquad){
			if(ai.IsAvailable()) {
				movementVector = CalculateMovementVector(centroid, CalculateRepulsion(ai.gameObject), ai.gameObject);
				movementVector = ApplyBoundaries(movementVector, ai.gameObject);
				ai.AssignMovementVector(movementVector);
			}
		}

		StartCoroutine ("AssignMovementVector");

	}

	protected virtual Vector3 CalculateCentroid() {
		Vector3 centroid = Vector3.zero;
		int count = 0;

		foreach (BaseAIController ai in AISquad) {
			if(ai.IsAvailable()) {
				centroid += ai.gameObject.transform.position;
				count++;
			}
		}

		centroid.x = centroid.x / count;
		centroid.y = 0;
		centroid.z = centroid.z / count;

		return centroid;
	}

	protected virtual Vector3 CalculateRepulsion(GameObject primary) {
		Vector3 repulsion = Vector3.zero;

		foreach (BaseAIController ai in AISquad) {
			if(ai.gameObject != primary) {
				if(AITooClose(primary, ai)) {
					repulsion += primary.transform.position - ai.gameObject.transform.position;
				}
			}
		}

		return repulsion;

	}

	protected virtual Vector3 CalculateMovementVector(Vector3 centroid, Vector3 repulsion, GameObject primary) {
		Vector3 movementVector = Vector3.zero;

		if(Vector3.Distance(centroid, primary.transform.position) > maxDistance) {
			movementVector += centroid - primary.transform.position;
		}

		movementVector += repulsion;

		return movementVector;
	}


	protected virtual bool AITooClose(GameObject primary, BaseAIController secondary){
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

	protected virtual Vector3 ApplyBoundaries(Vector3 movementVector, GameObject ai) {
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
