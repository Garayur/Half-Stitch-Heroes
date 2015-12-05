using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestBushidoBear : BasePlayerCharacterController 
{
    

	// Use this for initialization
	void Start () 
    {
        ComboSetUp();

    }
    
    protected override void TwoButtonCombos()
    {
        base.twoButtonCombo.Add(new ComboNode(2, 4, 10, true,
            new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.HEAVYATTACK }));

        base.twoButtonCombo.Add(new ComboNode(2, 4, 10, true,
            new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK }));

        base.twoButtonCombo.Add(new ComboNode(2, 4, 10, true,
                    new ControllerActions[] { ControllerActions.HEAVYATTACK, ControllerActions.HEAVYATTACK }));
    }
}
