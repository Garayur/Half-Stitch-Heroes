using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasePlayerCharacterController : MonoBehaviour
{
	protected AttackInformation currentAttackInformation;
	protected float comboClearTime = 1.0f;

    protected Queue<ControllerActions> comboQueue = new Queue<ControllerActions>();

    protected List<ComboNode> twoButtonCombo = new List<ComboNode>();
    protected List<ComboNode> threeButtonCombo = new List<ComboNode>();
    protected List<ComboNode> fourButtonCombo = new List<ComboNode>();
    protected List<ComboNode> fiveButtonCombo = new List<ComboNode>();

    protected AttackInformation lightAttackInformation = new AttackInformation(2,5);
    protected AttackInformation HeavyAttackInformation = new AttackInformation(1, 10);
	protected AttackInformation grapplePunchAttackInformation = new AttackInformation(2,5);
	protected AttackInformation grappleThrowAttackInfo = new AttackInformation (2, 5);

    private bool isTimer = false;

    public virtual AttackInformation LightAttack(bool isJumping)
    {
        if (!isJumping)
        {
            if(isTimer)
            {
                StopCoroutine("ComboTimer");
            }
            StartCoroutine("ComboTimer");

            comboQueue.Enqueue(ControllerActions.LIGHTATTACK);
            if (!ActivateCombo())
            {
                return lightAttackInformation;
            }
            else
            {
                return currentAttackInformation;
            }
        }
        return new AttackInformation(0, 0);
    }

	public virtual AttackInformation HeavyAttack(bool isJumping)
    {
        if (!isJumping)
        {
			if(isTimer)
			{
				StopCoroutine("ComboTimer");
			}
			StartCoroutine("ComboTimer");

            comboQueue.Enqueue(ControllerActions.HEAVYATTACK);
            if (!ActivateCombo())
            {
                return HeavyAttackInformation;
            }
            else
            {
				return currentAttackInformation;
            }
        }
        return new AttackInformation(0, 0);
    }

	public virtual AttackInformation HitGrappleTarget()
	{
		return grapplePunchAttackInformation;
	}

	public virtual AttackInformation ThrowGrapple(){
		return grappleThrowAttackInfo;
	}

	public virtual int Block(bool isJumping) { return 0; }

	public virtual int SpecialAction(bool isJumping) { return 0; }

	public virtual int Grab(bool isJumping) { return 0; }

    protected bool ComboCheck(List<ComboNode> nodes)
    {
        currentAttackInformation = null;

        foreach (ComboNode i in nodes)
        {
            if (i.isMatchingCombo(comboQueue.ToArray()))
            {

				currentAttackInformation = new AttackInformation(i.GetAnimation(), i.GetDamage(), i.GetEffect());

                if (i.IsLastCombo())
                {
                    ClearComboQueue();
					break;
                }
            }
        }

        if (currentAttackInformation == null)
        {
            ClearComboQueue();
            return false;
        }
        else
        {
            return true;
        }

    }

    protected bool ActivateCombo()
    {
        int length = comboQueue.Count;
        bool isValidCombo = false;
        switch (length)
        {
            case 2:
                isValidCombo = ComboCheck(twoButtonCombo);
                break;
            case 3:
                isValidCombo = ComboCheck(threeButtonCombo);
                break;
            case 4:
                isValidCombo = ComboCheck(fourButtonCombo);
                break;
            case 5:
                isValidCombo = ComboCheck(fiveButtonCombo);
                break;
            default:
                isValidCombo = false;
                break;
        }

        return isValidCombo;
    }

    public void ClearComboQueue()
    {
        comboQueue.Clear();
    }

    protected void ComboSetUp()
    {
        TwoButtonCombos();
		ThreeButtonCombos();
    }

    protected virtual void TwoButtonCombos() { }
    protected virtual void ThreeButtonCombos() { }
    protected virtual void FourButtonCombos() { }
    protected virtual void FiveButtonCombos() { }

    IEnumerator ComboTimer ()
    {
        isTimer = true;

        yield return new WaitForSeconds(comboClearTime);

        ClearComboQueue();
        isTimer = false;
    }
}
