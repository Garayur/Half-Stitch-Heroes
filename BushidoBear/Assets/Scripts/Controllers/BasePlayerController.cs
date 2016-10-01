using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BasePlayerController : BaseController
{

	//Events
    public delegate void PlayerAction(ControllerActions controllerAction, BaseController player, List<BaseAIController> targetList);
    public static event PlayerAction OnPlayerEvent;

	public delegate void PlayerDeath(BasePlayerController player);
	public static event PlayerDeath OnPlayerDeathEvent;

	public delegate void PlayerBlock(BasePlayerController player);
	public static event PlayerBlock OnPlayerBlockEvent;



    public BasePlayerCharacterController character;
	public int getGrip{get{return grip;}}
	
    protected bool acceptAttackInput = true;
	protected bool isGrabPressed =  false;
	protected bool isRightTriggerPressed = false;
    protected int gamePad;
    protected List<BaseAIController> targetList = new List<BaseAIController>();
	protected int grip = 0;
	protected int defaultPlayerGrip = 10;
	protected int defaultNPCGrip = 5;
	protected int maxGrip = 20;
	protected float loosenGripStartingTime = 0.5f;
	protected float loosenGripDecrementAmount = 0.01f;
	protected float loosenGripTimer;

    private int actionValue;

	public virtual void Start() {
		currentState = ControllerState.Positioning;
		loosenGripStartingTime = 0.5f;
		loosenGripDecrementAmount = 0.01f;
		moveSpeed = 4.5f;
		reactsToCollision = false;
	}

    override protected void Update()
    {
		switch (currentState) {
		case ControllerState.Grappling:
			if (Input.GetButtonDown ("Light Attack" + gamePad)) {
				HitGrappleTarget ();
				grip++;
			}
			
			if (Input.GetButtonDown ("Heavy Attack" + gamePad)) {
				if (grappleTarget.CanBeThrown ()) {
					if (Input.GetAxisRaw ("Horizontal" + gamePad) > 0) {
						tH = 1;
						grappleTarget.transform.position = gameObject.transform.position + new Vector3 (1.5f, 0, 0);
					} else if (Input.GetAxisRaw ("Horizontal" + gamePad) < 0) {
						tH = -1;
						grappleTarget.transform.position = gameObject.transform.position + new Vector3 (-1.5f, 0, 0);
					}
					ThrowGrapple ();
				} else {
					grappleTarget.BreakGrapple();
					BreakGrapple();
				}
			}
			
			if (Input.GetButtonDown ("Defend" + gamePad)) {
				grip++;
				if (grip > maxGrip)
					grip = maxGrip;
				//press repeatedly to hold
			}
			break;
		case ControllerState.Grappled:
			if (Input.GetButtonDown ("Defend" + gamePad)) {
				grip--;
				if (grip <= 0) {
					grappledBy.BreakGrapple ();
					BreakGrapple ();
					StopCoroutine ("BreakGrip" + gamePad);
				}
				//press repeatedly to break grip
			}
			break;
		case ControllerState.Blocking:
			if(Input.GetButtonUp("Defend" + gamePad))
			{
				EndBlock();
			}
			break;
		case ControllerState.Prone:
		case ControllerState.Thrown:
		case ControllerState.Standing:
		case ControllerState.Dying:
		case ControllerState.Dead:
		//case ControllerState.Jumping:
			break;
		default:
			h = Input.GetAxisRaw("Horizontal" + gamePad);
			v = Input.GetAxisRaw("Vertical" + gamePad);
			
			tH = h;
			tV = v;

			if(Input.GetButtonDown("Jump" + gamePad)){
				Jump();
			}
			else if(Input.GetButtonDown("Light Attack" + gamePad)){
				LightAttack();
			}
			else if(Input.GetButtonDown("Heavy Attack" + gamePad)){
				HeavyAttack();
			}
			else if(Input.GetButtonDown("Defend" + gamePad)){
				Block();
			}
			else if(Input.GetButtonDown("Grab" + gamePad)) {
				Grab();
			}
			else if (Input.GetAxis("Grab" + gamePad) > 0) {
				if(!isGrabPressed) {
					isGrabPressed = true;
					Grab();
				}
			}
			else {
				isGrabPressed = false;
			}

			break;
		
		}


        base.Update();
    }

    //============================================
    //PlayerActions
    //============================================
	protected override void LightAttack(int animationNumber = 0)
	{
        if (acceptAttackInput)
        {
			PredictAttack(ControllerActions.LIGHTATTACK);
            currentAttackInfo = character.LightAttack(isJumping);
            animator.SetInteger("Action", currentAttackInfo.GetAnimationNumber());
        }
    }

	protected override void HeavyAttack(int animationNumber = 0)
	{
        if (acceptAttackInput)
        {
			PredictAttack(ControllerActions.HEAVYATTACK);
            currentAttackInfo = character.HeavyAttack(isJumping);
            animator.SetInteger("Action", currentAttackInfo.GetAnimationNumber()); 
        }
    }

	protected override void Grab(int animationNumber = 8){
		base.Grab(animationNumber);
		character.ClearComboQueue();
	}

	protected override void HitGrappleTarget(int animationNumber = 2) {
		currentAttackInfo = character.HitGrappleTarget();
		animator.SetInteger("Action", currentAttackInfo.GetAnimationNumber());
	}

	public override void ThrowGrapple(){
		currentAttackInfo = character.ThrowGrapple ();
		base.ThrowGrapple ();
	}

	protected override void Block(int animationNumber = 0)
	{ 
		SendPlayerBlockEvent (this);
		base.Block(animationNumber);
    }

	protected override void EndBlock(){
		currentState = ControllerState.Positioning;
		base.EndBlock ();
	}

	protected override void SpecialAction(int animationNumber = 0)
	{
		PredictAttack(ControllerActions.SPECIAL);
		character.SpecialAction(isJumping);
    }

	protected override void Jump(int animationNumber = 0)
	{    
        base.Jump();
    }


	public override void BreakGrapple(){
		base.BreakGrapple();
		currentState = ControllerState.Positioning;
		StopCoroutine ("EscapeGrip");
		StopCoroutine ("LoosenGrip");
		//End grapple visual indication
		GrappleIndicationHandler.EndGrapple(gameObject);
	}

	public override bool GetGrabbed(BaseController grappler){
		switch(currentState) {
		case ControllerState.Dead:
		case ControllerState.Dying:
		case ControllerState.Prone:
		case ControllerState.Grappled:
			return false;
		default:
			character.ClearComboQueue();
			BeginGrappled(grappler);
			return true;
		}
	}

	protected override void BeginGrappling (BaseController target)
	{
		base.BeginGrappling (target);
		grip = defaultPlayerGrip;
		loosenGripTimer = loosenGripStartingTime;
		StartCoroutine ("LoosenGrip");
		//Display grapple indicator
		GrappleIndicationHandler.BeginGrapple(gameObject,target.gameObject,true);
	}

	protected override void BeginGrappled (BaseController grappler)
	{
		base.BeginGrappled (grappler);
		grip = defaultNPCGrip;
		loosenGripTimer = loosenGripStartingTime;
		StartCoroutine ("EscapeGrip");
		//Display grapple indicator, if player is grappling player allow player initiating grapple to handle display
		if(!(grappler is BasePlayerController))
		{
			GrappleIndicationHandler.BeginGrapple(grappler.gameObject,gameObject,false);
		}
	}

	public virtual IEnumerator LoosenGrip() {
		yield return new WaitForSeconds (loosenGripTimer);
		grip--;
		if (grip <= 0) {
			grappleTarget.BreakGrapple();
			BreakGrapple ();
			StopCoroutine ("LoosenGrip");
		}
		else {
			loosenGripTimer -= loosenGripDecrementAmount;
			if(loosenGripTimer <= loosenGripDecrementAmount){
				loosenGripTimer = loosenGripDecrementAmount;
			}
			StartCoroutine ("LoosenGrip");
		}
	}

	public virtual IEnumerator EscapeGrip() {
		yield return new WaitForSeconds (loosenGripTimer);
		grip++;
		loosenGripTimer -= loosenGripDecrementAmount;
		if(loosenGripTimer <= loosenGripDecrementAmount){
			loosenGripTimer = loosenGripDecrementAmount;
		}
		StartCoroutine ("EscapeGrip");
	}
    
	private void PredictAttack(ControllerActions action)
    {
		Vector3 point1 = transform.TransformPoint (new Vector3 (0, charController.height, 0) + attackOffset) + new Vector3(0, - attackRadius, 0);
		Vector3 point2 = transform.TransformPoint (attackOffset) + new Vector3 (0, attackRadius, 0);


		collidersFound = Physics.OverlapCapsuleNonAlloc (point1, point2, attackRadius, attackCollisionResults, LayerMask.GetMask ("Mob"));
		BaseAIController charControl;
		for (int i = 0; i < collidersFound; i++) {
			charControl = attackCollisionResults[i].GetComponent<BaseAIController>();
			if (charControl == null)
				continue;

			if (charControl == this)
				continue;

			charControl.HandlePlayerAction (action, this);
			charControl = null;
		}
    }
		
	public override void TakeDamage(BaseController other, Vector3 hitPosition, Vector3 hitDirection, float amount, AttackEffect effect) {
		switch (currentState) {
		case ControllerState.Grappling:
			grappleTarget.BreakGrapple ();
			BreakGrapple ();
			if (effect == AttackEffect.SumoKnockdown) {
				if(TakeSumoKnockdown())
					base.TakeDamage (other, hitPosition, hitDirection, amount, AttackEffect.None);
			} else {
				base.TakeDamage (other, hitPosition, hitDirection, amount, AttackEffect.None);
			}
			break;
		case ControllerState.Blocking:
			if (effect == AttackEffect.SumoKnockdown) {
				if(TakeSumoKnockdown())
					base.TakeDamage (other, hitPosition, hitDirection, amount, AttackEffect.None);
			}
			character.ClearComboQueue();
			break;
		default:
			character.ClearComboQueue ();
			if (effect == AttackEffect.Knockdown) {
				FallProne ();
			} else if (effect == AttackEffect.SumoKnockdown) {
				if (TakeSumoKnockdown ()) {
					base.TakeDamage (other, hitPosition, hitDirection, amount, AttackEffect.None);
				}
			} else {
				base.TakeDamage(other, hitPosition, hitDirection, amount, AttackEffect.None);
			}
			break;
		}
		
	}

	public override void FallDead(){
		base.FallDead ();
		animationFinishedDelegate = Dead;
		SendPlayerDeathEvent (this);
	}

	protected virtual void Dead(){
		currentState = ControllerState.Dead;
	}

	protected override void BeginDeath(){
		bool isAnyPlayerAlive = false;
		FallDead ();
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			if (player.GetComponent<BasePlayerController> ().GetState () != ControllerState.Dying && player.GetComponent<BasePlayerController> ().GetState () != ControllerState.Dead)
				isAnyPlayerAlive = true;
		}

		if (!isAnyPlayerAlive) {
			Debug.Log ("Game Over");
		}
	}


	private void SendControllerEvent (ControllerActions action, BaseController player)
	{
		try
		{
			OnPlayerEvent(action, player, targetList);
		}
		catch(NullReferenceException)
		{
			//Do nothing
		}
	}

	private void SendPlayerDeathEvent(BasePlayerController player){
		try{
			OnPlayerDeathEvent(player);
		}
		catch(NullReferenceException){
		}
	}

	private void SendPlayerBlockEvent(BasePlayerController player){
		try{
			OnPlayerBlockEvent(player);
		}
		catch(NullReferenceException){
		
		}
	}
}
