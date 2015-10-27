using UnityEngine;
using System.Collections;

public class JudoJaguarController : AIBaseController {

	protected override void TargetGrabbing(BaseController target) {
		//rotate to face target and grab
		Grab();
	}

	protected override void TargetHeavyAttacking() {
		if(Random.Range(0, 4) == 0) //25% chance
			Block();
	}

	//special attack knocks player to ground(leg sweep
	protected override void Attack() {
		SpecialAction();
		base.Attack();
	}
}
