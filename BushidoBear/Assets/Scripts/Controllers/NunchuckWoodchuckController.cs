using UnityEngine;
using System.Collections;

public class NunchuckWoodchuckController : AIBaseController {
	


	public override void Start () {
		currentState = ControllerState.StartingAnimation;
		isRun = true;
		attackFrequency = 5.0f;

		lightAttackInfo = new AttackInformation(1, 1);
	}

	protected override void Attack ()
	{
		LightAttack();
		base.Attack();
	}

}
