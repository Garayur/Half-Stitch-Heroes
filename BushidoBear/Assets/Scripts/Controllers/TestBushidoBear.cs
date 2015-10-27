using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestBushidoBear : BasePlayerCharacterController 
{
    int currentAnimationNumber = 0;

	// Use this for initialization
	void Start () 
    {
        

	}

	public override void LightAttack(bool isJumping, int animationNumber)
    {
        if (!isJumping)
        {
            comboQueue.Enqueue(ControllerActions.LIGHTATTACK);
			if(!ActivateCombo()) {

			}
        }
    }

    bool ComboCheck(List<ComboNode> nodes)
    {
		currentAnimationNumber = -1;

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
		bool isValidCombo = false;
        switch (length)
        {
            case 2:
                isValidCombo = ComboCheck(twoButtonCombo);
                break;
            case 3:
				isValidCombo =ComboCheck(threeButtonCombo);
                break;
            case 4:
				isValidCombo =ComboCheck(fourButtonCombo);
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
        base.twoButtonCombo.Add(new ComboNode(2, 0, false,
            new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.HEAVYATTACK }));

        base.twoButtonCombo.Add(new ComboNode(2, 0, false,
            new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK }));

        base.twoButtonCombo.Add(new ComboNode(2, 0, false,
                    new ControllerActions[] { ControllerActions.HEAVYATTACK, ControllerActions.HEAVYATTACK }));
    }
}
