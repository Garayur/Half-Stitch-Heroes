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

    public override void LightAttack(bool isJumping)
    {
        if (!isJumping)
        {
            comboQueue.Enqueue(ControllerActions.LIGHTATTACK);
            ActivateCombo();
        }
    }

    void ComboCheck(List<ComboNode> nodes)
    {
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
    }

    void ActivateCombo ()
    {
        int length = comboQueue.Count;
        switch (length)
        {
            case 2:
                ComboCheck(twoButtonCombo);
                break;
            case 3:
                ComboCheck(threeButtonCombo);
                break;
            case 4:
                ComboCheck(fourButtonCombo);
                break;
            case 5:
                ComboCheck(fiveButtonCombo);
                break;
            default:
                break;
        }
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
