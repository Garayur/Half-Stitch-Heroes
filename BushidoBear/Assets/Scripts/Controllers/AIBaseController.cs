using UnityEngine;
using System.Collections;

public enum AIState {StartingAnimation, Positioning, Attacking, Flinching, Fallen, Dying, Dead};

public class AIBaseController : BaseControllerOld {

	public float flinchDuration = 1.0f;
	public float attackFrequency = 2.0f;
	public float maxAttackRange = 3;
	public float minAttackRange = 1;
	
	protected AIState currentState;
	protected float stateTimer;
	protected float targetingTimer = 0;
	protected GameObject target = null;
	protected Vector3 vectorToTarget;
	protected Vector3 VectorToTarget;
	protected float distanceToTarget;
	protected Vector3 movementVector;
	
	
	public delegate void AIStateChanged(AIStateData newState);
	public static event AIStateChanged OnAIStateChange;
	
	void Start () {
		currentState = AIState.StartingAnimation;
		isRun = true;
	}
	
	protected override void Update () {
		switch (currentState) {
		case AIState.StartingAnimation:
			StartAnimation();
			break;
		case AIState.Positioning:
			Positioning();
			break;
		case AIState.Attacking:
			Attacking();
			break;
		case AIState.Flinching:
			Flinch();
			break;
		case AIState.Fallen:
			Fall();
			break;
		case AIState.Dying:
			Dying();
			break;
		case AIState.Dead:
			Dead();
			break;
		}
		base.Update();
		UpdateTurning();
		UpdateMovement();

	}
	
	
	protected virtual void StartAnimation() {

	}
	
	protected virtual void Positioning() {
		targetingTimer -= Time.deltaTime;

		if(targetingTimer <= 0) {
			FindAndAssignFacingTarget();
			targetingTimer = 1.0f;
		}

		h = movementVector.x;
		v = movementVector.z;

		tH = target.transform.position.x - gameObject.transform.position.x;
		tV = target.transform.position.z - gameObject.transform.position.z;

	}
	
	protected virtual void Attacking() {
		vectorToTarget = target.transform.position - gameObject.transform.position;
		distanceToTarget = Vector3.Distance(gameObject.transform.position, target.transform.position);
		
		if (ApproachedTargetUntilInRange()) {
			stateTimer -= Time.deltaTime;
			if(stateTimer <= 0) {
				LightAttack();
				stateTimer = attackFrequency;
			}
		}
	}
	
	protected virtual void Flinch() {
		stateTimer -= Time.deltaTime;
		if(stateTimer <= 0) {
			currentState = AIState.Positioning;
			SendStateChangeEvent();
		}
	}
	
	protected virtual void Fall() {
		//has fallen to the ground will get up again notify aicoordinator

		SendStateChangeEvent();
	}
	
	protected virtual void Dying() {
		//playing dying animations

		SendStateChangeEvent();
	}

	protected virtual void Dead() {
		Destroy(this);
	}

	protected virtual bool ApproachedTargetUntilInRange() {
		VectorToTarget = vectorToTarget;
		
		if(distanceToTarget > maxAttackRange) {
			h = VectorToTarget.x;
			v = VectorToTarget.z;
			tH = h;
			tV = v;
			return false;
		}
		else if(distanceToTarget < minAttackRange) {
			h = -VectorToTarget.x;
			v = -VectorToTarget.z;
			tH = VectorToTarget.x;
			tV = VectorToTarget.z;
			return true;
		}
		else {
			h = 0;
			v = 0;
			tH = h;
			tV = v;
			return true;
		}
	}
	
	private bool IsInRange() {
		if(distanceToTarget < maxAttackRange)
			return true;
		else
			return false;
	}

	public virtual void AttackNewTarget(GameObject newTarget) {
		target = newTarget;
		currentState = AIState.Attacking;
		isRun = true;
		SendStateChangeEvent();
	}
	
	public virtual bool IsAvailable() {
		if(currentState == AIState.Positioning || currentState == AIState.StartingAnimation) {
			return true; }
		else 
			return false;

	}

	protected void SendStateChangeEvent() {
		if(OnAIStateChange != null)
			OnAIStateChange(new AIStateData(currentState, this, target));
	}

	public void StartCombatPositioning(){
		if(currentState == AIState.StartingAnimation) {
			currentState = AIState.Positioning;
			isRun = false;
			SendStateChangeEvent();
		}
	}

<<<<<<< HEAD
	public void TakeDamage(BaseControllerOld other, Vector3 hitPosition, Vector3 hitDirection, float amount) {
		base.TakeDamage(other, hitPosition, hitDirection, amount);
=======
	public override void TakeDamage(BaseController other, Vector3 hitPosition, Vector3 hitDirection, float amount) {
>>>>>>> a5888dacde7f54fe9fc8431182f81e4b290e29a1
		if(currentState == AIState.StartingAnimation) {
			SendStateChangeEvent();
		}
		else if(currentState == AIState.Attacking) {
			currentState = AIState.Flinching;
			stateTimer = flinchDuration;
			SendStateChangeEvent();
		}
		base.TakeDamage(other, hitPosition, hitDirection, amount);
	}

	public void AssignMovementVector(Vector3 newMovementVector) {
		movementVector = newMovementVector;
	}

	protected void FindAndAssignFacingTarget() {
		float shortestDistanceToPlayer = 100;
		float distanceToPlayer = 0;

		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			distanceToPlayer = Vector3.Distance(player.transform.position, gameObject.transform.position);
			if(distanceToPlayer < shortestDistanceToPlayer) {
				shortestDistanceToPlayer = distanceToPlayer;
				target = player;
			}
		}


	}

	
	protected override void CheckMoveSet()
	{
		if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion") ||
		    this.animator.GetCurrentAnimatorStateInfo(0).IsName("AttackIdle"))
		{
			UpdateQueue();
		}
	}


	

}