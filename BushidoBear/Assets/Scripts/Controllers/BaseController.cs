using UnityEngine;
using System.Collections;

public enum ControllerActions { LIGHTATTACK, HEAVYATTACK, BLOCK, GRAB, SPECIAL, JUMP, JUMPINGLIGHTATTACK, JUMPINGHEAVYATTACK };
public enum ControllerState { StartingAnimation, Positioning, Attacking, Flinching, Fallen, Dying, Dead, Grappled, Grappling };

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

    public float attackRadius = 1.0f;
    public float airTime = 0.0f;
    public float health = 100f;
    public float lightAttackDamage = 5f;
    public float heavyAttackDamage = 10f;

    public GameObject m_hitEffect = null;

    public string[] damageReaction;
    public Action[] actionList;

    //---------------
    // protected
    //---------------
    protected Animator animator = null;
    protected bool isRun = false;
    protected bool isJumping = false;
    protected float moveSpeedScale = 1.0f;
    protected float h, v, tH, tV;
	protected bool isGrappled;
	protected bool isGrappling;
	protected BaseController grappledBy;
	protected BaseController grappledTarget;
    protected AttackInformation currentAttackInfo;

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

	protected virtual void Block(int animationNumber = 0){}

	protected virtual void SpecialAction(int animationNumber = 0){}

	protected virtual void Grab(int animationNumber = 0){}

	protected virtual void Jump(int animationNumber = 0)
	{
        if (charController.isGrounded)
        {
            animator.SetTrigger("Jump");
        }
    }

    public void EndAnimation()
    {
        animator.SetInteger("Action", 0);
    }

    protected void EventAttack()
    {
        Vector3 center = transform.TransformPoint(attackOffset);
        float radius = attackRadius;


        Debug.DrawRay(center, transform.forward, Color.red, 0.5f);

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

    public virtual void TakeDamage(BaseController other, Vector3 hitPosition, Vector3 hitDirection, float amount)
    {
        if ((health -= amount) < 0)
        {
            //Death();
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
        string reaction = damageReaction[Random.Range(0, damageReaction.Length)];		// random damage animation test
        animator.CrossFade(reaction, 0.1f, 0, 0.0f);


        //--------------------
        // hitFX 
        GameObject.Instantiate(m_hitEffect, hitPosition, Quaternion.identity);
    }

	public virtual void BreakGrapple() {
		if(isGrappled) {
			isGrappled = false;
			grappledBy = null;
		}
		else if(isGrappling){
			grappledTarget = null;
			grappledTarget = null;
		}
		grappledTarget.BreakGrapple();
	}

	protected virtual void Grappled() {
	}

	public virtual bool Grapple(BaseController grappler) {
		return false;
	}

	public virtual void Thrown(Vector3 direction){
		BreakGrapple();
		//apply velocity to self in direction. if side of screen is hit fall down. 
	}


}
