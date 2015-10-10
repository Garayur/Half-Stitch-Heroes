using UnityEngine;
using System.Collections;

public enum ControllerActions
{
    LIGHTATTACK,
    HEAVYATTACK,
    BLOCK,
    GRAB,
    SPECIAL,
    JUMP
};

public class BaseController : MonoBehaviour 
{
    //---------------
    // public
    //---------------

    public bool enableControl = true;

    public float turnSpeed = 10.0f;
    public float moveSpeed = 5.0f;
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
    protected virtual void LightAttack(){}

    protected virtual void HeavyAttack(){}

    protected virtual void Block(){}

    protected virtual void SpecialAction(){}

    protected virtual void Grab(){}

    protected virtual void Jump()
    {
        if (charController.isGrounded)
        {
            animator.SetTrigger("Jump");
        }
    }
}
