using UnityEngine;
using System.Collections;

public class JudoJaguarController : AIBaseController {


	protected override void TargetHeavyAttacking() {
		if(Random.Range(0, 4) == 0) //25% chance
			Block();
	}

	//special attack knocks player to ground(leg sweep
	protected override void Attack() {
		SpecialAction();
		base.Attack();
	}

	public override bool Grapple(BaseController grappler) { //return false and if possible counter grab. 
		switch(currentState) {
		case AIState.Dead:
		case AIState.Dying:
		case AIState.Fallen:
		case AIState.Grappled:
			isGrappled = false;
			break;
		default:
			isGrappled = false;
			target = grappler.gameObject;
			Grab();
			//Grapple grappler //face target and grapple. 
			break;
		}
		return isGrappled;
	}

	protected override void Grappling(){
		//throw grappled target
	}
}
