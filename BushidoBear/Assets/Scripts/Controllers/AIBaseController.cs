using UnityEngine;
using System.Collections;

public enum AIState {StartingAnimation, Positioning, Combat, Flinching, Fallen, Dying};

public class AIBaseController : BaseController {
	
	private AIState currentState;
	private float timer;
	public GameObject target = null;
	private Vector3 vectorToTarget;
	private Vector3 normalizedVectorToTarget;
	private float maxAttackRange = 3;
	private float minAttackRange = 2;
	private float distanceToTarget;
	
	
	public delegate void AIStateChanged(AIStateData newState);
	public static event AIStateChanged OnAIStateChange;
	
	void Start () {
		currentState = AIState.StartingAnimation;
		isRun = true;
	}
	
	protected override void Update () {
		base.Update();
		switch (currentState) {
		case AIState.StartingAnimation:
			StartAnimation();
			break;
		case AIState.Positioning:
			Positioning();
			break;
		case AIState.Combat:
			Combat ();
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
		}
		
	}
	
	
	protected virtual void StartAnimation() {
		//enemy actions if already on screen when player reaches AI
		
		currentState = AIState.Positioning;

		if(OnAIStateChange != null)
			OnAIStateChange(new AIStateData(currentState, this, target));
	}
	
	protected virtual void Positioning() {
		//moving around the screen when AI has entered combat
		currentState = AIState.Combat;

		if(OnAIStateChange != null)
			OnAIStateChange(new AIStateData(currentState, this, target));
	}
	
	protected virtual void Combat() {
		vectorToTarget = target.transform.position - gameObject.transform.position;
		distanceToTarget = Vector3.Distance(gameObject.transform.position, target.transform.position);
		
		if (ApproachedTargetUntilInRange()) {
			//attack
		}
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
	
	protected virtual void Flinch(){
		//has flinched notify aicoordinator
	}
	
	protected virtual void Fall() {
		//has fallen to the ground will get up again notify aicoordinator
	}
	
	protected virtual void Dying() {
		//has died, play any dying animations at the end of this remove this character from screen notify aicontroller
	}
	
	public virtual void AssignTarget(GameObject newTarget) {
		target = newTarget;
		currentState = AIState.Combat;
	}
	
	public virtual bool IsAvailable() {
		if(currentState == AIState.Positioning)
			return true;
		else
			return false;
	}
	
	
}