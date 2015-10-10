using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasePlayerCharacterController : MonoBehaviour
{
    protected Queue<ControllerActions> comboQueue = new Queue<ControllerActions>();

    protected List<ComboNode> twoButtonCombo = new List<ComboNode>();
    protected List<ComboNode> threeButtonCombo = new List<ComboNode>();
    protected List<ComboNode> fourButtonCombo = new List<ComboNode>();
    protected List<ComboNode> fiveButtonCombo = new List<ComboNode>();

    public virtual void LightAttack(bool isJumping) { }

    public virtual void HeavyAttack(bool isJumping) { }

    public virtual void Block(bool isJumping) { }

    public virtual void SpecialAction(bool isJumping) { }

    public virtual void Grab(bool isJumping) { }
}
