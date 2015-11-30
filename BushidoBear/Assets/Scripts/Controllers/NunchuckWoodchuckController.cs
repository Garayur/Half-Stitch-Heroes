using UnityEngine;
using System.Collections;

public class NunchuckWoodchuckController : AIBaseController {
	
	protected AttackInformation lightAttackInfo = new AttackInformation(1, 1);

	public override void Start () {
		currentState = ControllerState.StartingAnimation;
		isRun = true;
		attackFrequency = 5.0f;
	}

	protected override void Attack ()
	{
		LightAttack();
		base.Attack();
	}

	protected override void LightAttack(int animationNumber = 1){
		currentAttackInfo = lightAttackInfo;
		animator.SetInteger("Action", currentAttackInfo.GetAnimationNumber());
	}

}
