using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BasePlayerController : BaseController
{
    public delegate void PlayerAction(ControllerActions controllerAction, BaseController player, List<BaseAIController> targetList);
    public static event PlayerAction OnPlayerEvent;

    public BasePlayerCharacterController character;
	
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
		reactsToCollision = false;
	}

    override protected void Update()
    {
		switch (currentState) {
		case ControllerState.Grappling:
			if (Input.GetButtonDown ("Light Attack")) {
				HitGrappleTarget ();
				grip++;
			}
			
			if (Input.GetButtonDown ("Heavy Attack")) {
				if(Input.GetAxisRaw("Horizontal") > 0){
					tH = 1;
					grappleTarget.transform.position = gameObject.transform.position + new Vector3(1.5f, 0, 0);
				}
				else if(Input.GetAxisRaw("Horizontal") < 0){
					tH = -1;
					grappleTarget.transform.position = gameObject.transform.position + new Vector3(-1.5f, 0, 0);
				}
				ThrowGrapple ();
			}
			
			if (Input.GetButtonDown ("Defend")) {
				grip++;
				if (grip > maxGrip)
					grip = maxGrip;
				//press repeatedly to hold
			}
			break;
		case ControllerState.Grappled:
			if (Input.GetButtonDown ("Defend")) {
				grip--;
				if (grip <= 0) {
					grappledBy.BreakGrapple ();
					BreakGrapple ();
					StopCoroutine ("BreakGrip");
				}
				//press repeatedly to break grip
			}
			break;
		case ControllerState.Blocking:
			if(Input.GetButtonUp("Defend"))
			{
				EndBlock();
			}
			break;
		case ControllerState.Prone:
		case ControllerState.Thrown:
		case ControllerState.Standing:
		case ControllerState.Jumping:
			break;
		default:
			h = Input.GetAxisRaw("Horizontal");
			v = Input.GetAxisRaw("Vertical");
			
			tH = h;
			tV = v;

			if(Input.GetButtonDown("Jump")){
				Jump();
			}
			else if(Input.GetButtonDown("Light Attack")){
				LightAttack();
			}
			else if(Input.GetButtonDown("Heavy Attack")){
				HeavyAttack();
			}
			else if(Input.GetButtonDown("Defend")){
				Block();
			}
			else if(Input.GetButtonDown("Grab")) {
				Grab();
			}
			else if (Input.GetAxis("Grab") > 0) {
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
            PredictAttack();
            SendControllerEvent(ControllerActions.LIGHTATTACK, this);
            currentAttackInfo = character.LightAttack(isJumping);
            animator.SetInteger("Action", currentAttackInfo.GetAnimationNumber());
        }
    }

	protected override void HeavyAttack(int animationNumber = 0)
	{
        if (acceptAttackInput)
        {
            PredictAttack();
            SendControllerEvent(ControllerActions.HEAVYATTACK, this);
            currentAttackInfo = character.HeavyAttack(isJumping);
            animator.SetInteger("Action", currentAttackInfo.GetAnimationNumber()); 
        }
    }

	protected override void Grab(int animationNumber = 8){
		base.Grab(animationNumber);
		character.ClearComboQueue();
		SendControllerEvent(ControllerActions.GRAB, this);
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
        SendControllerEvent(ControllerActions.BLOCK, this);
		base.Block(animationNumber);
    }

	protected override void EndBlock(){
		currentState = ControllerState.Positioning;
		base.EndBlock ();
	}

	protected override void SpecialAction(int animationNumber = 0)
	{
        PredictAttack();
        SendControllerEvent(ControllerActions.SPECIAL, this);
		character.SpecialAction(isJumping);
    }

	protected override void Jump(int animationNumber = 0)
	{
        SendControllerEvent(ControllerActions.JUMP, this);    
        base.Jump();
    }


	public override void BreakGrapple(){
		base.BreakGrapple();
		currentState = ControllerState.Positioning;
		StopCoroutine ("EscapeGrip");
		StopCoroutine ("LoosenGrip");
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
	}

	protected override void BeginGrappled (BaseController grappler)
	{
		base.BeginGrappled (grappler);
		grip = defaultNPCGrip;
		loosenGripTimer = loosenGripStartingTime;
		StartCoroutine ("EscapeGrip");
	}

	public virtual IEnumerator LoosenGrip() {
		Debug.Log ("Grip is Loosening");
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
				Debug.Log("Grapple break attempts at peak");
			}
			StartCoroutine ("LoosenGrip");
		}
	}

	public virtual IEnumerator EscapeGrip() {
		Debug.Log ("Grip is breaking");
		yield return new WaitForSeconds (loosenGripTimer);
		grip++;
		loosenGripTimer -= loosenGripDecrementAmount;
		if(loosenGripTimer <= loosenGripDecrementAmount){
			loosenGripTimer = loosenGripDecrementAmount;
			Debug.Log("Grapple break attempts at peak");
		}
		StartCoroutine ("EscapeGrip");
	}

    protected virtual void AttackTargetList()
    {
        //deal damage to all in target list then clear
        Vector3 center = transform.TransformPoint(Vector3.zero);

        foreach (BaseAIController tar in targetList)
        {
            tar.TakeDamage(this, center, transform.forward, 1.0f, currentAttackInfo.GetAttackEffect());
        }
        targetList.Clear();
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

    private void PredictAttack()
    {
        Vector3 center = transform.TransformPoint(Vector3.zero);
        float radius = 1.0f;

        Debug.DrawRay(center, transform.forward, Color.red, 0.5f);

        Collider[] cols = Physics.OverlapSphere(center, radius);


        //------------------------
        //Check Enemy Hit Collider
        //------------------------
        foreach (Collider col in cols)
        {
            BaseAIController charControl = col.GetComponent<BaseAIController>();
            if (charControl == null)
                continue;

            if (charControl == this)
                continue;

            targetList.Add(charControl);
        }
    }

	public override void TakeDamage(BaseController other, Vector3 hitPosition, Vector3 hitDirection, float amount, AttackEffect effect) {
		switch (currentState) {
		case ControllerState.Grappling:
			grappleTarget.BreakGrapple();
			BreakGrapple();
			base.TakeDamage(other, hitPosition, hitDirection, amount, AttackEffect.None);
			break;
		case ControllerState.Blocking:
			character.ClearComboQueue();
			break;
		default:
			character.ClearComboQueue();
			if(effect == AttackEffect.Knockdown){
				FallProne();
			}
			base.TakeDamage(other, hitPosition, hitDirection, amount, AttackEffect.None);
			break;
		}
		
	}
}
