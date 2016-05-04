using UnityEngine;
using System.Collections;

public enum ControllerActions { LIGHTATTACK, HEAVYATTACK, BLOCK, GRAB, SPECIAL, JUMP, JUMPINGLIGHTATTACK, JUMPINGHEAVYATTACK };
public enum ControllerState { StartingAnimation, Positioning, Attacking, Flinching, Prone, Standing, Dying, Dead, Grappled, Grappling, Thrown, Blocking, Jumping };
public enum AttackEffect {None, Knockdown};

public class BaseController : MonoBehaviour 
{
    //---------------
    // public
    //---------------

    public bool enableControl = true;

    public float turnSpeed = 10.0f;
    public float moveSpeed = 2.0f;
    public float runSpeedScale = 2.0f;
	public float standupDelay = 2.0f;

    public Vector3 attackOffset = Vector3.zero;

    public float attackRadius = 1.5f;
    public float airTime = 0.0f;
    public float health = 100f;
    public float lightAttackDamage = 5f;
    public float heavyAttackDamage = 10f;

	protected Vector3 throwForce = new Vector3 (12, 15, 12);
	protected float gravity = -9.8f;
	protected float drag = 10.0f;
	protected Vector3 momentum;
	protected float charStandingHeight = 2.18f;
	protected float charLayingHeight = 0.5f;

	protected bool reactsToCollision = true;

	public ParticleSystem m_hitEffect = null;

    public string[] damageReaction;

	public delegate void AnimationFinishedDelegate();
	public AnimationFinishedDelegate animationFinishedDelegate;

	public GameObject projectile;
	public Vector3 projectileRelativeSpawnPosition;
	public Vector2 projectileForce;
	
    //---------------
    // protected
    //---------------
    protected Animator animator = null;
    protected bool isRun = false;
    protected bool isJumping = false;
    protected float moveSpeedScale = 1.0f;
    protected float h, v, tH, tV;
    protected AttackInformation currentAttackInfo;
	protected ControllerState currentState;

	protected BaseController grappledBy;
	protected BaseController grappleTarget;

    //---------------
    // private
    //---------------
    private CharacterController charController = null;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 turnDirection = Vector3.zero;
    private Vector3 movementVector = Vector3.zero;

    private float speed = 0;

    //-------------
    //Methods
    //-------------
    protected void Awake()
    {
        animator = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();

        //Run key check
        isRun = true;
		animationFinishedDelegate = null;
    }

    protected virtual void Update()
    {
        //------------------
        //Parameters Reset
        //------------------
        moveDirection = Vector3.zero;

        //------------------
        //Update Control
        //------------------
        if (enableControl == true) {
			moveSpeedScale = animator.GetFloat ("SpeedScale");
			UpdateMoveControl ();
		} 

		UpdatePhysics();
        UpdateTurning();
        UpdateMovement();
        UpdateAnimationVariables();

		isJumping = charController.isGrounded;
	}
	
	protected virtual void FixedUpdate()
    {
        airTime = animator.GetBool("Ground") ? 0.0f : airTime + Time.deltaTime;
    }

    private void UpdateAnimationVariables()
    {
        animator.SetFloat("Speed", speed, 0.05f, Time.deltaTime);
        animator.SetBool("Ground", charController.isGrounded);
        animator.SetFloat("AirTime", airTime);
    }

    //==============================================================
    //Movement
    //==============================================================
    public void UpdateMovement()
    {
        if (movementVector != Vector3.zero)
        {
            charController.Move(movementVector);
            movementVector = Vector3.zero;
        }
    }

    public void UpdateTurning()
    {
        if (turnDirection.magnitude > 0.01f)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, turnDirection, turnSpeed * Time.deltaTime, turnSpeed);
            movementVector += moveDirection * Time.deltaTime * moveSpeed * moveSpeedScale;
        }
    }

    private void UpdateMoveControl()
    {
        //movement direction
        moveDirection = Vector3.zero;

        moveDirection.x = h;
        moveDirection.z = v;

        moveDirection.y = 0.0f;

        moveDirection = Vector3.Normalize(moveDirection);

        turnDirection = Vector3.zero;

        //Turn Direction
        turnDirection.x = tH;
        turnDirection.z = tV;

        turnDirection.y = 0.0f;

        turnDirection = Vector3.Normalize(turnDirection);

        // run
        if (isRun == true)
        {
            moveDirection *= runSpeedScale;
            turnDirection *= runSpeedScale;
        }

        speed = moveDirection.magnitude;
        if (isRun == true) speed *= runSpeedScale;
    }

	protected virtual void ApplyMomentum(){
		Vector3 dragVector = momentum.normalized;
		dragVector *= drag;
		momentum.x -=  dragVector.x * Time.deltaTime;
		momentum.z -= dragVector.z * Time.deltaTime;
		momentum.y += gravity * Time.deltaTime;

		if (momentum.x < 0.5f && momentum.x > -0.5f)
			momentum.x = 0;

		if (momentum.z < 0.5f && momentum.z > -0.5f)
			momentum.z = 0;

		if (momentum.y < 0)
			momentum.y = 0;

		movementVector += momentum * Time.deltaTime;
	}

	protected virtual void ApplyGravity(){
		movementVector.y += gravity * Time.deltaTime;
	}

	protected virtual void UpdatePhysics(){
		ApplyGravity ();
		if (!enableControl)
			ApplyMomentum ();
	}

	public virtual void AddForce(Vector3 force){
		momentum += force;
	}

    //================================================
    //ControllerActions
    //================================================
    protected virtual void LightAttack(int animationNumber = 1){}

	protected virtual void HeavyAttack(int animationNumber = 1){}

	protected virtual void Block(int animationNumber = 0){
		h = 0;
		v = 0;
		currentState = ControllerState.Blocking;
		animator.SetBool ("Blocking", true);
	}

	protected virtual void EndBlock(){
		animator.SetBool ("Blocking", false);
	}

	protected virtual void SpecialAction(int animationNumber = 0){}

	protected virtual void Jump(int animationNumber = 0)
	{
		if (charController.isGrounded)
		{
			enableControl = false;
			AddForce(new Vector3(0, 14, 0));
			animator.SetTrigger("Jump");
			h = 0;
			v = 0;
			currentState = ControllerState.Jumping;
		}
	}

	//called by jump animation
	public virtual void JumpEnded() {
		enableControl = true;
		currentState = ControllerState.Positioning;
	}

	public ControllerState GetState(){
		return currentState;
	}


	//================================================
	//Grappling functions
	//================================================


	protected virtual void Grab(int animationNumber = 8){
		Vector3 center = transform.TransformPoint(attackOffset);
		float radius = attackRadius;

		
		Collider[] cols = Physics.OverlapSphere(center, radius);

		foreach (Collider col in cols)
		{
			BaseController charControl = col.GetComponent<BaseController>();
			if (charControl == null)
				continue;
			
			if (charControl == this)
				continue;
			
			if(charControl.GetGrabbed(this)) {
				BeginGrappling(charControl);
			}
			break;
		}
		animator.SetInteger("Action", 8);

	}

	protected virtual void HitGrappleTarget(int animationNumber = 2) {
	}

	protected virtual void Grappled() {
	}
	
	
	public virtual bool GetGrabbed(BaseController grappler){
		return true;
	}

	protected virtual void BeginGrappling(BaseController target){
		currentState = ControllerState.Grappling;
		grappleTarget = target;
		animator.SetBool("Grappling", true);
		h = 0;
		v = 0;

		if (tH > 0) {
			tH = 1;
			tV = 0;
			grappleTarget.gameObject.transform.position = gameObject.transform.position + new Vector3(1.5f, 0, 0);
		}
		else {
			tH = -1;
			tV = 0;
			grappleTarget.transform.position = gameObject.transform.position + new Vector3(-1.5f, 0, 0);
		}
	}
	
	protected virtual void BeginGrappled(BaseController grappler) {
		currentState = ControllerState.Grappled;
		grappledBy = grappler;
		animator.SetBool("Grappled", true);
		h = 0;
		v = 0;

		if (Vector3.Normalize(grappledBy.transform.position - gameObject.transform.position).x > 0) {
			tH = 1;
			tV = 0;
		}
		else {
			tH = -1;
			tV = 0;
		}

	}

	public virtual void BreakGrapple() {
		animator.SetBool("Grappling", false);
		animator.SetBool("Grappled", false);
		animator.SetInteger("Action", 0);
	}

	public virtual void ThrowGrapple(){
		Vector3 throwVector;
		throwVector = grappleTarget.transform.position - gameObject.transform.position;
		throwVector.Normalize ();
		throwVector = new Vector3 (throwVector.x * throwForce.x, throwForce.y, throwVector.z * throwForce.z);
		grappleTarget.BreakGrapple();
		BreakGrapple();
		grappleTarget.GetThrown (throwVector, currentAttackInfo.GetAttackDamage());
	}
	
	public virtual void GetThrown(Vector3 force, int damage) {
		AddForce (force);
		enableControl = false;
		charController.height = charLayingHeight;
		animator.SetTrigger ("Thrown");
		StartCoroutine ("BeingThrown", damage);
		currentState = ControllerState.Thrown;
	}

	public virtual IEnumerator BeingThrown(int damage) {
		yield return null;
		if (!charController.isGrounded) {
			StartCoroutine ("BeingThrown", damage);
		}
		else {
			TakeDamage( null , transform.TransformPoint(attackOffset), transform.forward, damage, AttackEffect.None);
			currentState = ControllerState.Prone;
			StartCoroutine("StandFromProne", standupDelay);
		}
	}

	public void OnControllerColliderHit(ControllerColliderHit hit){
		if (currentState == ControllerState.Thrown) {
			if (hit.gameObject.GetComponent<BaseController> () != null) {
				CollideWithController (hit.gameObject.GetComponent<BaseController> ());
			}
		}
	}

	protected void CollideWithController(BaseController collisionTarget){
		if (collisionTarget.reactsToCollision && collisionTarget.GetState() != ControllerState.Thrown) {
			Vector3 force;
			force = Vector3.Normalize (collisionTarget.gameObject.transform.position - gameObject.transform.position);
			force.y = 0;
			force *= momentum.magnitude * 0.8f;
			force.y = 8;
			collisionTarget.GetThrown (force, 5);
		} 
		else {
			momentum = Vector3.zero;
		}
	}

	public virtual void FallProne(){
		animator.SetTrigger ("FallProne");
		currentState = ControllerState.Prone;
		charController.height = charLayingHeight;
		StartCoroutine("StandFromProne", standupDelay);
	}

	public virtual IEnumerator StandFromProne(float recoverDelay){
		yield return new WaitForSeconds (recoverDelay);
		charController.height = charStandingHeight;
		currentState = ControllerState.Standing;
		animator.SetTrigger ("Stand");
		charController.height = charStandingHeight;
	}

	public virtual void ResumeCombat(){
		enableControl = true;
		currentState = ControllerState.Positioning;
	}
	 

    public virtual void EndAnimation()
    {
        animator.SetInteger("Action", 0);
    }

    protected void EventAttack()
    {
        Vector3 center = transform.TransformPoint(attackOffset);
        float radius = attackRadius;

		if(currentState == ControllerState.Grappling) {
			grappleTarget.TakeDamage(this, center, transform.forward, currentAttackInfo.GetAttackDamage(), AttackEffect.None);
		}
		else {
	       // Debug.DrawRay(center, transform.forward, Color.red, 3.5f);

	        Collider[] cols = Physics.OverlapSphere(center, radius);


	        //------------------------
	        //Check Enemy Hit Collider
	        //------------------------
	        foreach (Collider col in cols)
	        {
	            BaseController charControl = col.GetComponent<BaseController>();
	            if (charControl == null)
	                continue;

	            if (charControl == this)
	                continue;

	            charControl.TakeDamage(this, center, transform.forward, currentAttackInfo.GetAttackDamage(), currentAttackInfo.GetAttackEffect());
	        }
		}
    }

    public virtual void TakeDamage(BaseController other, Vector3 hitPosition, Vector3 hitDirection, float amount, AttackEffect effect)
    {
        if ((health -= amount) <= 0)
        {
			BeginDeath();
            print("Ya dead son");
        }

        //--------------------
        // direction
        if (other != null)
        {
            transform.forward = -other.transform.forward;
        }
        else
        {
            hitDirection.y = 0.0f;
            transform.forward = -hitDirection.normalized;
        }

        //--------------------
        // reaction  

		if(damageReaction.Length > 0) {
        	string reaction = damageReaction[Random.Range(0, damageReaction.Length)];		// random damage animation test
			animator.CrossFade(reaction, 0.1f, 0, 0.0f);
		}

		Flinch();

        //--------------------
        // if particle system is present, set particle release rate, set direction and play particle system
		if(m_hitEffect)
		{
			m_hitEffect.gameObject.transform.rotation = other.transform.rotation;
			ParticleSystem.EmissionModule temp = m_hitEffect.emission;
			ParticleSystem.MinMaxCurve tempRate = new ParticleSystem.MinMaxCurve();
			tempRate.constantMin = amount;
			tempRate.constantMax = amount * 3;
			temp.rate = tempRate;
			m_hitEffect.Play();
		}
    }

	protected virtual void Flinch(){}

	protected virtual void BeginDeath(){}
	


}
