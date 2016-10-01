using UnityEngine;
using System.Collections;

public class SumoSquirrelController : BaseAIController {

	public float meleeRange;
	public float sumoJumpDamage = 5.0f;
	protected float bodySlamDistanceTraveled;
	protected float bodySlamDistanceMax = 2.0f;
	protected Vector3 lastPosition;
	protected bool bodySlamming = false;

	public override void OnEnable(){
		base.OnEnable ();
		currentState = ControllerState.StartingAnimation;
		isRun = true;
		attackFrequency = 3.0f;
		jumpStrength = 14.0f;
		turnSpeed = 2.5f;

		attackRadius = 1.0f;
		attackOffset = new Vector3 (0,0, 0.8f);


		rangedAttackInfo = new AttackInformation(1, 1);
		heavyAttackInfo = new AttackInformation (1, 10);
	}


	protected override void Jump(int animationNumber = 0)
	{
		base.Jump(animationNumber);
			//determine if faceing left or right, and rotate to opposite while jumping.
			if (tH > 0) {
				tH = -1;
				tV = 0;
			}
			else {
				tH = 1;
				tV = 0;
			}
	}

	//called by jump animation
	public override void JumpEnded() {
		base.JumpEnded ();
	    ApplyJumpSmashDamage();
	}


	protected void ApplyJumpSmashDamage(){
		GameObject[] victims = GameObject.FindGameObjectsWithTag("Mob");
		foreach (GameObject victim in victims) {
			if (victim != this.gameObject)
				victim.GetComponent<BaseController> ().TakeDamage (this, transform.TransformPoint(Vector3.zero), transform.forward, sumoJumpDamage, AttackEffect.SumoKnockdown);
		}

		victims = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject victim in victims) {
			victim.GetComponent<BaseController>().TakeDamage(this, transform.TransformPoint(Vector3.zero), transform.forward, sumoJumpDamage, AttackEffect.SumoKnockdown);
		}
	}

	protected void BeginBodySlam(){
		//set speed
		lastPosition = transform.position;
		bodySlamDistanceTraveled = 0;
		h = transform.forward.x;
		v = transform.forward.z;
		tH = h;
		tV = v;
		//set state to bodyslamming

	}

	protected void BodySlamming(){
		bodySlamDistanceTraveled += Vector3.Distance (transform.position, lastPosition);
		lastPosition = transform.position;

		if (bodySlamDistanceTraveled >= bodySlamDistanceMax) {
			//change state to default
			//set speed to normal
			h = 0;
			v = 0;
		}
	}
		
	public override void OnControllerColliderHit(ControllerColliderHit hit){
		if (currentState == ControllerState.Thrown || bodySlamming == true) {  // || bodyslammingState
			if (hit.gameObject.GetComponent<BaseController> () != null) {
				CollideWithController (hit.gameObject.GetComponent<BaseController> ());
			}
		}
	}

	protected override void CollideWithController(BaseController collisionTarget){
		Vector3 force;
		force = Vector3.Normalize (collisionTarget.gameObject.transform.position - gameObject.transform.position);
		force.y = 0;
		force *= momentum.magnitude * 0.8f;
		force.y = 8;
		collisionTarget.GetThrown (force, 5);
	}
		

	protected override IEnumerator Attack ()
	{
		yield return StartCoroutine(base.Attack());
		Jump ();
////		if (IsInRange ()) { 
////			if (distanceToTarget > meleeRange) {
////				RangedAttack ();
////				SpawnProjectile (); //this should be called by the animation when we get one
////			}
////		}
	}
}
