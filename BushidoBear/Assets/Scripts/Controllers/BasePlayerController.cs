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
    protected int gamePad;
    protected List<AIBaseController> targetList = new List<AIBaseController>();

    private int actionValue;

    override protected void Update()
    {
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

	protected override void Grab(int animationNumber = 0)
	{
        if (acceptAttackInput)
        {
            PredictAttack();
            SendControllerEvent(ControllerActions.GRAB, this);
            if (targetList[0].Grapple(this))
            {
                isGrappling = true;
                GrappledTarget = targetList[0].gameObject;
                //play grappling anim
            }
            else
            {
                isGrappling = false;
                //play grapplefail anim
            }
            character.Grab(isJumping); 
        }
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


	public override bool Grapple(BaseController grappler){
		isGrappled = true;
		grappledBy = grappler;
		return isGrappled;
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
