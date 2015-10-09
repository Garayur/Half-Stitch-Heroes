using UnityEngine;
using System;
using System.Collections;

public class BasePlayerController : BaseController
{
    public delegate void PlayerAction(ControllerActions controllerAction, BaseController player, float range);
    public static event PlayerAction OnPlayerEvent;

    public BasePlayerCharacterController character;

    protected int gamePad;

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
    protected override void LightAttack()
    {
        SendControllerEvent(ControllerActions.LIGHTATTACK, this);
        character.LightAttack(isJumping);
    }

    protected override void HeavyAttack()
    {
        SendControllerEvent(ControllerActions.HEAVYATTACK, this);
        character.HeavyAttack(isJumping);
    }

    protected override void Grab()
    {
        SendControllerEvent(ControllerActions.GRAB, this);
        character.Grab(isJumping);
    }

    protected override void Block()
    {
        SendControllerEvent(ControllerActions.BLOCK, this);
        character.Block(isJumping);
    }

    protected override void SpecialAction()
    {
        SendControllerEvent(ControllerActions.SPECIAL, this);
        character.SpecialAction(isJumping);
    }

    protected override void Jump()
    {
        SendControllerEvent(ControllerActions.JUMP, this);    
        base.Jump();
    }
    
    private void SendControllerEvent (ControllerActions action, BaseController player, float range = 1)
    {
        try
        {
            OnPlayerEvent(action, player, range);
        }
        catch(NullReferenceException)
        {
            //Do nothing
        }
    }
}
