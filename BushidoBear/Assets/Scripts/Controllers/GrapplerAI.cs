using UnityEngine;
using System.Collections;

public class GrapplerAI : BaseAIController {


	//main attack style is grappling player
	//if allies are available holds for the allies to hit the player
	//if no allies hit the character yourself
	//countergrab

	public float grapplingAttackFrequency = 1.0f;

	void Reset(){
		attackFrequency = 3.0f;
	}

	protected override IEnumerator Attack() {
		yield return StartCoroutine(base.Attack());
		Grab ();
	}

	protected IEnumerator AttackGrappledTarget() {
		HitGrappleTarget ();
		yield return new WaitForSeconds (grapplingAttackFrequency);
		StartCoroutine ("AttackGrappledTarget");
	}

	protected override void BeginGrappling(BaseController target){
		StopCoroutine ("Attack"); 
		StartCoroutine ("AttackGrappledTarget");
		base.BeginGrappling (target);
	}

	public override void BreakGrapple() {
		StopCoroutine ("AttackGrappledTarget");
		base.BreakGrapple();
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
