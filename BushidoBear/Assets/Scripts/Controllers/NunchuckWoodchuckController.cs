using UnityEngine;
using System.Collections;

public class NunchuckWoodchuckController : AIBaseController {

	public override void Start () {
		currentState = AIState.StartingAnimation;
		isRun = true;
		attackFrequency = 1.0f;
	}

	protected override void Attack ()
	{
		LightAttack();
		base.Attack();
	}

}
