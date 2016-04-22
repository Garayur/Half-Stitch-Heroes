using UnityEngine;
using System.Collections;

public class RangedAI : BaseAIController {

	public float meleeRange;

	public override void OnEnable(){
		base.OnEnable ();
		currentState = ControllerState.StartingAnimation;
		isRun = true;
		attackFrequency = 3.0f;

		lightAttackInfo = new AttackInformation (1, 1);
		rangedAttackInfo = new AttackInformation(1, 1);
	}

	protected override IEnumerator Attack ()
	{
		yield return StartCoroutine(base.Attack());
		if (IsInRange ()) { 
			if (distanceToTarget > meleeRange) {
				RangedAttack ();
				SpawnProjectile (); //this should be called by the animation when we get one
			}
			else
				LightAttack ();
		}
	}

}
