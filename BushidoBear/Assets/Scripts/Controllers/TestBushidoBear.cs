﻿using UnityEngine;
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
        base.twoButtonCombo.Add(new ComboNode(2, 4, 10, true, AttackEffect.Knockdown,
            new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.HEAVYATTACK }));

        base.twoButtonCombo.Add(new ComboNode(2, 3, 10, false, AttackEffect.None,
            new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK }));

        base.twoButtonCombo.Add(new ComboNode(2, 4, 10, true, AttackEffect.Knockdown,
                    new ControllerActions[] { ControllerActions.HEAVYATTACK, ControllerActions.HEAVYATTACK }));
    }

	protected override void ThreeButtonCombos() {
		base.threeButtonCombo.Add(new ComboNode(3, 4, 20, true, AttackEffect.None,
		            new ControllerActions[] {ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK, ControllerActions.HEAVYATTACK}));

		base.threeButtonCombo.Add(new ComboNode(3, 3, 20, true, AttackEffect.None,
		            new ControllerActions[] {ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK}));
	}
}
