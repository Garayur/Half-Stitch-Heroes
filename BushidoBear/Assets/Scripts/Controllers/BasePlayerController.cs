using UnityEngine;
using System;
using System.Collections;

public class BasePlayerController : BaseController
{
    public delegate void PlayerAction(ControllerActions controllerAction);
    public static event PlayerAction OnPlayerEvent;

    public BasePlayerCharacterController character;

    protected int gamePad;

    private int actionValue;

    override protected void Update()
    {
        h = Input.GetAxisRaw("HorizontalP2");
        v = Input.GetAxisRaw("VerticalP2");

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
        SendControllerEvent(ControllerActions.LIGHTATTACK);
        character.LightAttack(isJumping);
    }

    protected override void HeavyAttack()
    {
        SendControllerEvent(ControllerActions.HEAVYATTACK);
        character.HeavyAttack(isJumping);
    }

    protected override void Grab()
    {
        SendControllerEvent(ControllerActions.GRAB);
        character.Grab(isJumping);
    }

    protected override void Block()
    {
        SendControllerEvent(ControllerActions.BLOCK);
        character.Block(isJumping);
    }

    protected override void SpecialAction()
    {
        SendControllerEvent(ControllerActions.SPECIAL);
        character.SpecialAction(isJumping);
    }

    protected override void Jump()
    {
        SendControllerEvent(ControllerActions.JUMP);    
        base.Jump();
    }
    
    private void SendControllerEvent (ControllerActions action)
    {
        try
        {
            OnPlayerEvent(action);
        }
        catch(NullReferenceException)
        {
            //Do nothing
        }
    }
}
