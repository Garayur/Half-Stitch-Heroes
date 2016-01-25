using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BasePlayerController : BaseController
{
    public delegate void PlayerAction(ControllerActions controllerAction, BaseController player, List<AIBaseController> targetList);
    public static event PlayerAction OnPlayerEvent;

    public BasePlayerCharacterController character;
	protected GameObject GrappledTarget;
    protected bool acceptAttackInput = true;
	protected bool isLeftTriggerPressed =  false;
	protected bool isRightTriggerPressed = false;
    protected int gamePad;
    protected List<AIBaseController> targetList = new List<AIBaseController>();

    private int actionValue;

	public virtual void Start() {
		currentState = ControllerState.Attacking;
	}

    override protected void Update()
    {
		if(currentState == ControllerState.Grappling) {
			if(Input.GetKeyDown("joystick " + gamePad + " button 2")) {
				HitGrappleTarget();
			}

			if(Input.GetKeyDown("joystick " + gamePad + " button 3")) {
				ThrowGrapple();
			}

		}
		else if(currentState == ControllerState.Grappled) {

		}
		else{
	        h = Input.GetAxisRaw("HorizontalP" + gamePad);
	        v = Input.GetAxisRaw("VerticalP" + gamePad);

	        tH = h;
	        tV = v;

	        if (Input.GetKeyDown("joystick " + gamePad + " button 0"))
	        {
	            Jump();
	        }

	        if (Input.GetKeyDown("joystick " + gamePad + " button 2"))
	        {
	            LightAttack();
	        }

	        if (Input.GetKeyDown("joystick " + gamePad + " button 3"))
	        {
	            HeavyAttack();
	        }

			if (Input.GetAxis("LTP" + gamePad) > 0)
			{
				if(!isLeftTriggerPressed) {
					isLeftTriggerPressed = true;
					Grab();
				}
			}
			else
			{
				isLeftTriggerPressed = false;
			}
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

	protected virtual void Grab(int animationNumber = 8){
		base.Grab(animationNumber);
		SendControllerEvent(ControllerActions.GRAB, this);
	}

	protected override void HitGrappleTarget(int animationNumber = 2) {
		currentAttackInfo = character.HitGrappleTarget();
		animator.SetInteger("Action", currentAttackInfo.GetAnimationNumber());
	}

	protected override void Block(int animationNumber = 0)
	{
        SendControllerEvent(ControllerActions.BLOCK, this);
		character.Block(isJumping);
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
		currentState = ControllerState.Attacking;
	}

	public virtual void ThrowGrapple(){
		grappleTarget.BreakGrapple();
		BreakGrapple();
		Debug.Log("Thrown");
		//apply velocity to self in direction. if side of screen is hit fall down. 
	}

	public override bool GetGrabbed(BaseController grappler){
		switch(currentState) {
		case ControllerState.Dead:
		case ControllerState.Dying:
		case ControllerState.Fallen:
		case ControllerState.Grappled:
			return false;
		default:
			BeginGrappled(grappler);
			return true;
		}
	}

    protected virtual void AttackTargetList()
    {
        //deal damage to all in target list then clear
        Vector3 center = transform.TransformPoint(Vector3.zero);

        foreach (AIBaseController tar in targetList)
        {
            tar.TakeDamage(this, center, transform.forward, 1.0f);
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
            AIBaseController charControl = col.GetComponent<AIBaseController>();
            if (charControl == null)
                continue;

            if (charControl == this)
                continue;

            targetList.Add(charControl);
        }
    }
}
