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

    public virtual AttackInformation LightAttack(bool isJumping) { return new AttackInformation(0, 0.0f); }

	public virtual int HeavyAttack(bool isJumping) { return 0; }

	public virtual int Block(bool isJumping) { return 0; }

	public virtual int SpecialAction(bool isJumping) { return 0; }

	public virtual int Grab(bool isJumping) { return 0; }
}
