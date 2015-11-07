using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AIState {StartingAnimation, Positioning, Attacking, Flinching, Fallen, Dying, Dead, Grappled, Grappling};

public class AIBaseController : BaseController {

	public float flinchDuration = 1.0f;
	public float attackFrequency = 2.0f;
	public float maxAttackRange = 3;
	public float midAttackRange = 2.5f;
	public float minAttackRange = 1.5f;
	
	protected AIState currentState;
	protected float stateTimer;
	protected float targetingTimer = 0;
	protected GameObject target = null;
	protected Vector3 vectorToTarget;
	protected Vector3 VectorToTarget;
	protected float distanceToTarget;
	protected Vector3 aiMovementVector;
	protected int grappledHitCount = 0;

	protected int currentComboStep = 0;
	protected List<ComboNode> currentCombo;
	
	public delegate void AIStateChanged(AIStateData newState);
	public static event AIStateChanged OnAIStateChange;
	
	public virtual void Start () {
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
		case AIState.Grappled:
			Grappled();
			break;
		case AIState.Grappling:
			Grappling();
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

		h = aiMovementVector.x;
		v = aiMovementVector.z;

		tH = target.transform.position.x - gameObject.transform.position.x;
		tV = target.transform.position.z - gameObject.transform.position.z;

	}
	
	protected virtual void Attacking() {
		vectorToTarget = target.transform.position - gameObject.transform.position;
		distanceToTarget = Vector3.Distance(gameObject.transform.position, target.transform.position);
		
		if (ApproachedTargetUntilInRange()) {
			stateTimer -= Time.deltaTime;
			if(stateTimer <= 0) {
				Attack();
			}
		}
	}

	protected virtual void Attack() {
		animationFinishedDelegate = ResetAttackTimer;
	}

	protected void ResetAttackTimer() {
		stateTimer = attackFrequency;
	}

	protected void ExecuteCombo(List<ComboNode> comboSequence) {
		currentCombo = comboSequence;
		currentComboStep = 0;
		ExecuteMove(currentCombo[currentComboStep].GetComboSequence()[0], currentCombo[currentComboStep].GetAnimation());
		animationFinishedDelegate = ContinueCombo;
	}
	
	protected void ContinueCombo() {
		ExecuteMove(currentCombo[currentComboStep].GetComboSequence()[(currentCombo[currentComboStep].GetComboSequence().Length -1)], currentCombo[currentComboStep].GetAnimation()); //last move in the combo
			if(currentCombo[currentComboStep].IsLastCombo()) {
				animationFinishedDelegate = ResetAttackTimer;
		}
		else{
			animationFinishedDelegate = ContinueCombo;
			currentComboStep++;
		}
			
	}

	protected void ExecuteMove(ControllerActions action, int animationNumber){
		switch(action) {
		case ControllerActions.BLOCK:
			Block(animationNumber);
			break;
		case ControllerActions.GRAB:
			Grab(animationNumber);
			break;
		case ControllerActions.HEAVYATTACK:
			HeavyAttack(animationNumber);
			break;
		case ControllerActions.JUMP:
			Jump(animationNumber);
			break;
		case ControllerActions.LIGHTATTACK:
			LightAttack(animationNumber);
			break;
		case ControllerActions.SPECIAL:
			SpecialAction(animationNumber);
			break;
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

	protected override void Grappled() {
		if(grappledHitCount >= 3)
			BreakGrapple();
	}

	protected override void Grab(int animationNumber = 0) {
		if(target.GetComponent<BaseController>().Grapple(this)) {
			isGrappling = true;
			//play grappling anim
		}
		else{
			isGrappling = false;
			//play grapplefail anim
		}
	}

	protected virtual void Grappling(){

	}

	public override void BreakGrapple() {
		isGrappled = false;
		grappledBy = null;
		currentState = AIState.Fallen;
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
		else if(distanceToTarget < midAttackRange) {
			h = 0;
			v = 0;
			tH = h;
			tV = v;
			return true;
		}
		else
			return true;
	}
	
	private bool IsInRange() {
		if(distanceToTarget < maxAttackRange)
			return true;
		else
			return false;
	}

	public override bool Grapple(BaseController grappler) {
		switch(currentState) {
		case AIState.Dead:
		case AIState.Dying:
		case AIState.Fallen:
		case AIState.Grappled:
			isGrappled = false;
			break;
		default:
			isGrappled = true;
			break;
		}
		grappledBy = grappler;
		grappledHitCount = 0;
		return isGrappled;
		
	}

	public override void Thrown(Vector3 direction){
		BreakGrapple();
		currentState = AIState.Fallen;
		//apply velocity to self in direction. if side of screen is hit fall down. 
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


	public override void TakeDamage(BaseController other, Vector3 hitPosition, Vector3 hitDirection, float amount) {
		if(currentState == AIState.StartingAnimation) {
			SendStateChangeEvent();
		}
		else if(currentState == AIState.Attacking) {
			currentState = AIState.Flinching;
			stateTimer = flinchDuration;
			SendStateChangeEvent();
		}
		if(currentState == AIState.Grappled){
			if(other == grappledBy)
				grappledHitCount++;
		}
		base.TakeDamage(other, hitPosition, hitDirection, amount);
	}

	public void AssignMovementVector(Vector3 newMovementVector) {
		aiMovementVector = newMovementVector;
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


	void OnEnable() {
		BasePlayerController.OnPlayerEvent += HandlePlayerEvent;
	}
	
	void OnDisable() {
		BasePlayerController.OnPlayerEvent -= HandlePlayerEvent;
	}

	protected void HandlePlayerEvent(ControllerActions action, BaseController player, List<AIBaseController> targetList){
		if(player.gameObject == target && targetList.Contains(this)) {

			switch (action) {
			case ControllerActions.BLOCK:
				TargetBlocking();
				break;
			case ControllerActions.GRAB:
				TargetGrabbing(player);
				break;
			case ControllerActions.HEAVYATTACK:
				TargetHeavyAttacking();
				break;
			case ControllerActions.JUMP:
				TargetJumping();
				break;
			case ControllerActions.LIGHTATTACK:
				TargetLightAttacking();
				break;
			case ControllerActions.SPECIAL:
				TargetSpecialAttacking();
				break;
			}
		}
	}


	protected virtual void TargetBlocking(){}

	protected virtual void TargetGrabbing(BaseController player){}

	protected virtual void TargetHeavyAttacking(){}

	protected virtual void TargetJumping(){}

	protected virtual void TargetLightAttacking(){}

	protected virtual void TargetSpecialAttacking(){}





}