using UnityEngine;
using System.Collections;

public class NunchuckWoodchuckController : AIBaseController {
	


	public override void Start () {
		currentState = ControllerState.StartingAnimation;
		isRun = true;
		attackFrequency = 3.0f;

		lightAttackInfo = new AttackInformation(1, 1);
	}

	protected override IEnumerator Attack ()
	{
		yield return StartCoroutine(base.Attack());
		if(IsInRange())
			LightAttack();
	}

}
