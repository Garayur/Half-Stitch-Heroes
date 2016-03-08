using UnityEngine;
using System.Collections;

public class NunchuckWoodchuckController : BaseAIController {


	public override void OnEnable(){
		base.OnEnable ();
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
