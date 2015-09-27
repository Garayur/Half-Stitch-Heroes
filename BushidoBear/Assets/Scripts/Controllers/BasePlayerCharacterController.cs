using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasePlayerCharacterController : MonoBehaviour 
{
    Queue<ControllerActions> ComboQueue = new Queue<ControllerActions>();

    public void LightAttack(bool isJumping) { }

    public void HeavyAttack(bool isJumping) { }

    public void Block(bool isJumping) { }

    public void SpecialAction(bool isJumping) { }

    public void Grab(bool isJumping) { }
}
