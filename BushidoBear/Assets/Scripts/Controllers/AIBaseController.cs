using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIBaseController : BaseController {

	public float flinchDuration = 1.0f;
	public float attackFrequency = 1.0f;
	public float deadDuration = 3.0f;
	public float blockDuration = 1.0f;
	public float maxAttackRange = 3;
	public float midAttackRange = 1.9f;
	public float minAttackRange = 1.5f;

	protected GameObject target = null;
	protected Vector3 vectorToTarget;
	protected Vector3 VectorToTarget;
	protected float distanceToTarget;
	protected Vector3 aiMovementVector;
	protected Vector2 centerOfField;
	protected int grappledHitCount = 0;
	protected float deadTimer;

	protected AttackInformation lightAttackInfo = new AttackInformation(2, 5);
	protected AttackInformation heavyAttackInfo = new AttackInformation(1, 10);
	protected AttackInformation grapplePunchAttackInfo = new AttackInformation(2, 5);
	protected AttackInformation grappleThrowAttackInfo = new AttackInformation (2, 5);

	protected int currentComboStep = 0;
	protected List<ComboNode> currentCombo;
	
	public delegate void AIStateChanged(AIStateData newState);
	public static event AIStateChanged OnAIStateChange;


	public virtual void Start () {
		currentState = ControllerState.StartingAnimation;
		isRun = true;
		attackRadius = midAttackRange;
	}
	
	protected override void Update () {
		switch (currentState) {
		case ControllerState.StartingAnimation:
			StartAnimation();
			break;
		case ControllerState.Positioning:
			Positioning();
			break;
		case ControllerState.Attacking:
			Attacking();
			break;
		case ControllerState.Prone:
			Fall();
			break;
		case ControllerState.Grappled:
			Grappled();
			break;
		case ControllerState.Grappling:
			Grappling();
			break;
		default:
			break;
		}
		base.Update();
		UpdateTurning();
		UpdateMovement();

	}
	
	
	protected virtual void StartAnimation() {

	}
	
	protected virtual void Positioning() {
		StartCoroutine(FindAndAssignFacingTarget());

		h = aiMovementVector.x;
		v = aiMovementVector.z;

		tH = target.transform.position.x - gameObject.transform.position.x;
		tV = target.transform.position.z - gameObject.transform.position.z;

	}
	
	protected virtual void Attacking() {
		vectorToTarget = target.transform.position - gameObject.transform.position;
		distanceToTarget = Vector3.Distance(gameObject.transform.position, target.transform.position);
		
		ApproachTargetUntilInRange();
	}

	protected virtual IEnumerator Attack(){
		yield return new WaitForSeconds(attackFrequency);
		StartCoroutine("Attack");
	}
	

	protected void ExecuteCombo(List<ComboNode> comboSequence) {
		currentCombo = comboSequence;
		currentComboStep = 0;
		ExecuteMove(currentCombo[currentComboStep].GetComboSequence()[0], -1);
		animationFinishedDelegate = ContinueCombo;
	}
	
	protected void ContinueCombo() {
		ExecuteMove(currentCombo[currentComboStep].GetComboSequence()[(currentCombo[currentComboStep].GetComboSequence().Length -1)], currentCombo[currentComboStep].GetAnimation()); //last move in the combo
		if(currentCombo[currentComboStep].IsLastCombo()) {
			StartCoroutine("Attack");
			animationFinishedDelegate = null;
		}
		else{
			animationFinishedDelegate = ContinueCombo;
			currentComboStep++;
		}
			
	}

	//animationNumber == -1 results in default animation
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
		default:
			break;
		}
	}

	protected override void Flinch(){
		switch(currentState)
		{
		case ControllerState.Positioning:
		case ControllerState.Attacking:
			currentState = ControllerState.Flinching;
			StopAllCoroutines();
			animationFinishedDelegate = EndFlinch;
			SendStateChangeEvent();
			animator.SetInteger("Action", 6);
			h = 0;
			v = 0;
			break;
		case ControllerState.Flinching:
			break;
		}
	}

	protected virtual void EndFlinch() {
		currentState = ControllerState.Positioning;
		//animator.SetInteger("Action", 0);
		SendStateChangeEvent();
		animationFinishedDelegate = null;
	}
	
	protected virtual void Fall() {
		//has fallen to the ground will get up again notify aicoordinator

		SendStateChangeEvent();
	}
	
	protected override void BeginDeath() {
		if(currentState != ControllerState.Dying || currentState != ControllerState.Dead) {
			currentState = ControllerState.Dying;
			animator.SetInteger("Action", 7);
			h = 0;
			v = 0;
			animationFinishedDelegate = Dying;
			deadTimer = deadDuration;
			SendStateChangeEvent(); 
		}
	}

	protected virtual void Dying(){
		StartCoroutine("Dead");
	}

	protected IEnumerator Dead() {
		currentState = ControllerState.Dead;
		SendStateChangeEvent();
		yield return new WaitForSeconds(deadTimer);
		Destroy(gameObject);
	}



	protected override void Grappled() {
		if(grappledHitCount >= 3) {
			grappledHitCount = 0;
			grappledBy.BreakGrapple();
			BreakGrapple();
		}

		if (tH > 0) {
			tH = 1;
			tV = 0;
		}
		else {
			tH = -1;
			tV = 0;
		}

		//tH = VectorToTarget.x;
	//	tV = VectorToTarget.z;
	}
	
	protected virtual void Grappling(){
		if (tH > 0) {
			tH = 1;
			tV = 0;
		}
		else {
			tH = -1;
			tV = 0;
		}
	}

	public override void BreakGrapple() {
		base.BreakGrapple();
		currentState = ControllerState.Positioning;

	}

	protected virtual void ThrowGrappleToCenter() {
		if(gameObject.transform.position.x > centerOfField.x)
			grappleTarget.transform.position = gameObject.transform.position + new Vector3(-1.5f, 0, 0);
		else
			grappleTarget.transform.position = gameObject.transform.position + new Vector3(1.5f, 0, 0);

		ThrowGrapple ();
	}

	public override void ThrowGrapple(){
		currentAttackInfo = grappleThrowAttackInfo;
		base.ThrowGrapple ();
	}

	public override bool GetGrabbed(BaseController grappler){
		switch(currentState) {
		case ControllerState.Dead:
		case ControllerState.Dying:
		case ControllerState.Prone:
		case ControllerState.Grappled:
			return false;
		case ControllerState.Flinching:
			EndFlinch();
			BeginGrappled(grappler);
			return true;
		default:
			BeginGrappled(grappler);
			return true;
		}
	}

	protected virtual void ApproachTargetUntilInRange() {
		VectorToTarget = vectorToTarget;
		
		if(distanceToTarget > maxAttackRange) {
			h = VectorToTarget.x;
			v = VectorToTarget.z;
			tH = h;
			tV = v;
		}
		else if(distanceToTarget < minAttackRange) {
			h = -VectorToTarget.x;
			v = -VectorToTarget.z;
			tH = VectorToTarget.x;
			tV = VectorToTarget.z;
		}
		else if(distanceToTarget < midAttackRange) {
			h = 0;
			v = 0;
			tH = h;
			tV = v;
		}
	}
	
	protected bool IsInRange() {
		if(distanceToTarget < maxAttackRange)
			return true;
		else
			return false;
	}


	public virtual void AttackNewTarget(GameObject newTarget) {
		target = newTarget;
		currentState = ControllerState.Attacking;
		StopAllCoroutines();
		StartCoroutine("Attack");
		isRun = true;
		SendStateChangeEvent();
	}
	
	public virtual bool IsAvailable() {
		if(currentState == ControllerState.Positioning || currentState == ControllerState.StartingAnimation) {
			return true; }
		else 
			return false;

	}

	protected void SendStateChangeEvent() {
		if(OnAIStateChange != null)
			OnAIStateChange(new AIStateData(currentState, this, target));
	}

	public void StartCombatPositioning(){
		if(currentState == ControllerState.StartingAnimation) {
			currentState = ControllerState.Positioning;
			isRun = false;
			SendStateChangeEvent();
		}
	}

	protected override void LightAttack(int animationNumber = -1){
		currentAttackInfo = lightAttackInfo;
		if(animationNumber < 0)
			animationNumber = lightAttackInfo.GetAnimationNumber();
		animator.SetInteger("Action", animationNumber); 
	}

	protected override void HeavyAttack(int animationNumber = -1){
		currentAttackInfo = heavyAttackInfo;
		if(animationNumber < 0)
			animationNumber = heavyAttackInfo.GetAnimationNumber();
		animator.SetInteger("Action", animationNumber);
	}

	protected override void HitGrappleTarget(int animationNumber = -1) {
		currentAttackInfo = grapplePunchAttackInfo;
		if(animationNumber < 0)
			animationNumber = grapplePunchAttackInfo.GetAnimationNumber();
		animator.SetInteger("Action", animationNumber);
	}

	protected IEnumerator BlockTimer(){
	
		yield return new WaitForSeconds (blockDuration);
		EndBlock ();
	}

	protected override void EndBlock(){
		currentState = ControllerState.Positioning;
		base.EndBlock ();
	}
	
	
	public override void TakeDamage(BaseController other, Vector3 hitPosition, Vector3 hitDirection, float amount) {
		switch (currentState) {
		case ControllerState.StartingAnimation:
			SendStateChangeEvent();
			base.TakeDamage(other, hitPosition, hitDirection, amount);
			break;
		case ControllerState.Grappled:
			if(other == grappledBy)
				grappledHitCount++;
			base.TakeDamage(other, hitPosition, hitDirection, amount);
			break;
		case ControllerState.Grappling:
			grappleTarget.BreakGrapple();
			BreakGrapple();
			base.TakeDamage(other, hitPosition, hitDirection, amount);
			break;
		case ControllerState.Blocking:
			break;
		default:
			Flinch();
			base.TakeDamage(other, hitPosition, hitDirection, amount);
			break;
		}

	}

	public void AssignMovementVector(Vector3 newMovementVector) {
		aiMovementVector = newMovementVector;
	}

	protected IEnumerator FindAndAssignFacingTarget() {
		float shortestDistanceToPlayer = 100;
		float distanceToPlayer = 0;
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			distanceToPlayer = Vector3.Distance(player.transform.position, gameObject.transform.position);
			if(distanceToPlayer < shortestDistanceToPlayer) {
				shortestDistanceToPlayer = distanceToPlayer;
				target = player;
			}
		}
		yield return new WaitForSeconds(1.0f);

		StartCoroutine(FindAndAssignFacingTarget());
	}

	public void AssignCenterPoint(Vector2 center){
		centerOfField = center;
	}

	public override void EndAnimation(){
		if(currentState != ControllerState.Dying || currentState != ControllerState.Dead) {
			base.EndAnimation();
			if(animationFinishedDelegate != null) {
				animationFinishedDelegate();
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
				if(Vector3.Distance(gameObject.transform.position, target.transform.position) < 2)
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
			default:
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