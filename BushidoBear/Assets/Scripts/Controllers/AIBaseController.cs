using UnityEngine;
using System.Collections;

public enum AIState {StartingAnimation, Positioning, Attacking, Flinching, Fallen, Dying, Dead};

public class AIBaseController : BaseController {

	public float flinchDuration = 1.0f;
	public float attackFrequency = 2.0f;
	public float maxAttackRange = 3;
	public float minAttackRange = 2;
	
	private AIState currentState;
	private float stateTimer;
	private GameObject target = null;
	private Vector3 vectorToTarget;
	private Vector3 normalizedVectorToTarget;
	private float distanceToTarget;
	
	
	public delegate void AIStateChanged(AIStateData newState);
	public static event AIStateChanged OnAIStateChange;
	
	void Start () {
		currentState = AIState.Positioning;
		isRun = true;
	}
	
	protected void Update () {
		switch (currentState) {
		case AIState.StartingAnimation:
			StartAnimation();
			break;
		case AIState.Positioning:
			Positioning();
			break;
		case AIState.Attacking:
			Attacking ();
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
	}
	
	
	protected virtual void StartAnimation() {
		currentState = AIState.Positioning;
		SendStateChangeEvent();
	}
	
	protected virtual void Positioning() {
		//moving around the screen when AI has entered combat

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
		normalizedVectorToTarget = Vector3.Normalize(vectorToTarget);
		
		if(distanceToTarget > maxAttackRange) {
			h = normalizedVectorToTarget.x;
			v = normalizedVectorToTarget.z;
			return false;
		}
		else if(distanceToTarget < minAttackRange) {
			h = -normalizedVectorToTarget.x;
			v = -normalizedVectorToTarget.z;
			return true;
		}
		else {
			h = 0;
			v = 0;
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
			SendStateChangeEvent();
		}
	}

	public void TakeDamage(BaseController other, Vector3 hitPosition, Vector3 hitDirection, float amount) {
		base.TakeDamage(other, hitPosition, hitDirection, amount);
		if(currentState == AIState.StartingAnimation) {
			currentState = AIState.Positioning;
			SendStateChangeEvent();
		}
		else if(currentState == AIState.Attacking) {
			currentState = AIState.Flinching;
			stateTimer = flinchDuration;
			SendStateChangeEvent();
		}
	}


	

}