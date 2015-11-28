using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestBushidoBear : BasePlayerCharacterController 
{
    int currentAnimationNumber = 0;

	// Use this for initialization
	void Start () 
    {
        TwoButtonCombos();
    }

	public override AttackInformation LightAttack(bool isJumping)
    {
        if (isJumping)
        {
            comboQueue.Enqueue(ControllerActions.LIGHTATTACK);
            if (!ActivateCombo())
            {
                print("no Combo");
                return new AttackInformation(1,0.0f);
            }
            else
            {
                print("Attack");
                return new AttackInformation(currentAnimationNumber, 1.0f);
            }
        }
        print("Jumping");
        return new AttackInformation(0, 0.0f);
    }

    bool ComboCheck(List<ComboNode> nodes)
    {
		currentAnimationNumber = -1;

        print(nodes.Count);
        foreach (ComboNode i in nodes)
        {
            if (i.isMatchingCombo(comboQueue.ToArray()))
            {
                currentAnimationNumber = i.GetAnimation();

                if(i.IsLastCombo())
                {
                    ClearComboQueue();
                }
            }
        }

		if(currentAnimationNumber == -1)
			return false;
		else
			return true;
    }

    bool ActivateCombo ()
    {
        int length = comboQueue.Count;
        print("Current Queue " + length);
		bool isValidCombo = false;
        switch (length)
        {
            case 2:
                isValidCombo = ComboCheck(twoButtonCombo);
                print(isValidCombo);
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

    void ClearComboQueue()
    {
        comboQueue.Clear();
    }

    void ComboSetUp()
    {
        TwoButtonCombos();
    }

    void TwoButtonCombos()
    {
        base.twoButtonCombo.Add(new ComboNode(2, 4, true,
            new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.HEAVYATTACK }));

        base.twoButtonCombo.Add(new ComboNode(2, 4, true,
            new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK }));

        base.twoButtonCombo.Add(new ComboNode(2, 4, true,
                    new ControllerActions[] { ControllerActions.HEAVYATTACK, ControllerActions.HEAVYATTACK }));
    }
}
