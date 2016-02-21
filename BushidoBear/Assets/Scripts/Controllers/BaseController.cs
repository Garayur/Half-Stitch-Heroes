using UnityEngine;
using System.Collections;

public enum ControllerActions { LIGHTATTACK, HEAVYATTACK, BLOCK, GRAB, SPECIAL, JUMP, JUMPINGLIGHTATTACK, JUMPINGHEAVYATTACK };
public enum ControllerState { StartingAnimation, Positioning, Attacking, Flinching, Prone, Dying, Dead, Grappled, Grappling, Thrown, Blocking };

public class BaseController : MonoBehaviour 
{
    //---------------
    // public
    //---------------

    public bool enableControl = true;

    public float turnSpeed = 10.0f;
    public float moveSpeed = 2.0f;
    public float runSpeedScale = 2.0f;

    public Vector3 attackOffset = Vector3.zero;

    public float attackRadius = 1.5f;
    public float airTime = 0.0f;
    public float health = 100f;
    public float lightAttackDamage = 5f;
    public float heavyAttackDamage = 10f;

    public GameObject m_hitEffect = null;

    public string[] damageReaction;

	public delegate void AnimationFinishedDelegate();
	public AnimationFinishedDelegate animationFinishedDelegate;
	
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
	protected Vector3 throwForce;

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
        if (enableControl == true)
        {
            moveSpeedScale = animator.GetFloat("SpeedScale");
            UpdateMoveControl();
        }

        isJumping = charController.isGrounded;

        UpdateTurning();
        UpdateMovement();
        UpdateAnimationVariables();
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

        // default gravity
        movementVector = Vector3.down * 0.5f * Time.deltaTime;

        speed = moveDirection.magnitude;
        if (isRun == true) speed *= runSpeedScale;
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
			animator.SetTrigger("Jump");
		}
	}


	//================================================
	//Grappling functions
	//================================================


	protected virtual void Grab(int animationNumber = 8){
		Debug.Log("Grab");
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
		}
		else {
			tH = -1;
			tV = 0;
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
			gameObject.transform.position = grappledBy.gameObject.transform.position + new Vector3(-1.5f, 0, 0);
		}
		else {
			tH = -1;
			tV = 0;
			gameObject.transform.position = grappledBy.gameObject.transform.position + new Vector3(1.5f, 0, 0);
		}

	}

	public virtual void BreakGrapple() {
		animator.SetBool("Grappling", false);
		animator.SetBool("Grappled", false);
		animator.SetInteger("Action", 0);
	}

	public virtual void ThrowGrapple(){
		Vector3 throwForce;
		throwForce = grappleTarget.transform.position - gameObject.transform.position;
		throwForce.Normalize ();
		throwForce = new Vector3 (throwForce.x * 15, 9, throwForce.z * 15);
		grappleTarget.BreakGrapple();
		BreakGrapple();
		grappleTarget.GetThrown (throwForce);
	}
	
	public virtual void GetThrown(Vector3 force) {
		throwForce = force;
		charController.Move (new Vector3 (0,0.1f,0));
		StartCoroutine ("BeingThrown");
	}

	public virtual IEnumerator BeingThrown() {
		throwForce.y += Physics.gravity.y * Time.deltaTime;
		charController.Move (throwForce * Time.deltaTime);
		yield return null;
		if (!charController.isGrounded) {
			StartCoroutine ("BeingThrown");
		}
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
			grappleTarget.TakeDamage(this, center, transform.forward, currentAttackInfo.GetAttackDamage());
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

	            charControl.TakeDamage(this, center, transform.forward, currentAttackInfo.GetAttackDamage());
	        }
		}
    }

    public virtual void TakeDamage(BaseController other, Vector3 hitPosition, Vector3 hitDirection, float amount)
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
        // hitFX 
        GameObject.Instantiate(m_hitEffect, hitPosition, Quaternion.identity);
    }

	protected virtual void Flinch(){}

	protected virtual void BeginDeath(){}
	


}
