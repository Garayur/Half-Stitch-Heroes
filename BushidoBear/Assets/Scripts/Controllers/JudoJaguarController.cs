using UnityEngine;
using System.Collections;

public class JudoJaguarController : BaseAIController {


	protected override void BeingHeavyAttacked(BasePlayerController player) {
		if (Random.Range (0, 4) == 0) { //25% chance 
			Block ();
			StartCoroutine ("BlockTimer");
		}
	}

	//special attack knocks player to ground(leg sweep
	protected override IEnumerator Attack() {
		yield return StartCoroutine(base.Attack());
		if(IsInRange())
			SpecialAction();
	}

	public override bool GetGrabbed(BaseController grappler){
		switch(currentState) {
		case ControllerState.Dead:
		case ControllerState.Dying:
		case ControllerState.Prone:
		case ControllerState.Grappled:
			return false;
		case ControllerState.Positioning:
		case ControllerState.Attacking:
			if(grappler.GetGrabbed(this)) {
				BeginGrappling(grappler);
				Debug.Log("Counter Grapple");
				StartCoroutine("CounterGrapple");
			}
			return false;
		default:
			BeginGrappled(grappler);
			return true;
		}
	}


	protected virtual IEnumerator CounterGrapple(){
		yield return new WaitForSeconds(0.5f);
		ThrowGrappleToCenter ();
	}
}
