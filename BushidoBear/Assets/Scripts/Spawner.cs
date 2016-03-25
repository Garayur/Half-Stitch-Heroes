using UnityEngine;
using System.Collections;

public class Spawner : BaseAICoordinator {

	public int quantity = 1;
	public float interspawnDelay = 1.0f;
	public float prespawnDelay = 0f;
	public GameObject mob;
	public GameObject spawnPoint;
	public enum Trigger {CollisionBox, AICoordinatorDeath, };
	public Trigger triggerType;
	public BaseAICoordinator targetDeathTriggerCoordinator;

	protected int spawnDecrementer = 0;



	protected void Enable(){
		spawnDecrementer = quantity;
	}

	protected override void OnEnable() {
		base.OnEnable ();
		BaseAICoordinator.CoordinatorDeath += CoordinatorDeathEvent;
	}
	
	protected override void OnDisable() {
		base.OnDisable();
		BaseAICoordinator.CoordinatorDeath += CoordinatorDeathEvent;
	}


	protected override void OnTriggerEnter(Collider other){
		if(!hasBeenTriggered && triggerType == Trigger.CollisionBox){
			StartCoroutine ("BeginSpawning");
			base.OnTriggerEnter (other);
		}
	}

	protected IEnumerator BeginSpawning(){
		yield return new WaitForSeconds (prespawnDelay);
		FindPlayers();
		StartCoroutine ("Spawn");
		StartCoroutine ("AssignMovementVector");
	}

	protected IEnumerator Spawn(){
		GameObject newMob;
		newMob = (GameObject)Instantiate (mob, spawnPoint.transform.position, Quaternion.identity);
		AddAI(newMob.GetComponent<BaseAIController>());
		spawnDecrementer--;
		yield return new WaitForSeconds (interspawnDelay);
		if (spawnDecrementer > 0) {
			StartCoroutine ("Spawn");
		}
	}

	protected void AddAI(BaseAIController newAI){
		AISquad.Add (newAI);
		newAI.AssignCenterPoint(new Vector2((boundaries.leftBoundary + boundaries.rightBoundary) /2, (boundaries.leftBoundary + boundaries.rightBoundary) / 2));
		newAI.StartCombatPositioning();
		AssignAttackers ();
	}

	protected override void DestroySelfOnSquadDeath() {
		if (AISquad.Count <= 0 && spawnDecrementer > 0) {
			base.DestroySelfOnSquadDeath();
		}
	}

	protected virtual void CoordinatorDeathEvent(BaseAICoordinator coordinator){
		if (triggerType == Trigger.AICoordinatorDeath) {
			if(targetDeathTriggerCoordinator != null){
				if(targetDeathTriggerCoordinator == coordinator)
					StartCoroutine ("BeginSpawning");
			}
			else{
				StartCoroutine ("BeginSpawning");
			}
		}
	}
	
}
