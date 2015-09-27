using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Action
{
    LIGHTATTACK,
    HEAVYATTACK
};

public abstract class BaseControllerOld : MonoBehaviour
{
    /*[System.Serializable]
    public struct Action
    {
        public string m_name;		//Action Animation Name
        public KeyCode m_keyCode;	//Input Keycode
    };*/

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
    public GameObject m_cameraTarget = null;

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
    private Vector3 moveDir = Vector3.zero;
    private Vector3 dir = Vector3.zero;
    private Vector3 turnDir = Vector3.zero;
    private Vector3 move = Vector3.zero;
    private int actionValue;
    private int tempActionValue = 0;
    private float moveCoolDownTimer = 1;
    private bool isCoolDown = true;
    private bool isReset = false;
    private ComboStateMachine currentState;
    private ComboStateMachine startState;
    private Queue<Action> moveList = new Queue<Action>(); 
    private List<ComboStateMachine> states;

    //----------------
    //abstract methods
    //----------------
    protected abstract void CheckMoveSet();

    protected void Awake()
    {
        //for testing purposes
        states = new TestControllerState().SetUp(this);
        startState = states[0];
        startState.SetUp(this);
        currentState = startState;

        animator = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
    }

    protected virtual void Update()
    {
        //------------------
        //Parameters Reset
        //------------------
        moveDir = Vector3.zero;

        //------------------
        //Update Control
        //------------------
        if (enableControl == true)
        {
            moveSpeedScale = animator.GetFloat("SpeedScale");
            UpdateControl();
        }

        //---------------------
        //Animation Moveset Test
        //---------------------
        CheckMoveSet();

        //------------------
        //Parameters sync
        //------------------
        float speed = moveDir.magnitude;
        if (isRun == true) speed *= runSpeedScale;

        animator.SetFloat("Speed", speed, 0.05f, Time.deltaTime);
        animator.SetBool("Ground", charController.isGrounded);
        animator.SetFloat("AirTime", airTime);
    }

    protected virtual void FixedUpdate()
    {
        airTime = animator.GetBool("Ground") ? 0.0f : airTime + Time.deltaTime;
    }

    private void UpdateControl()
    {
        UpdateMoveControl();
        UpdateActionControl();
        UpdateQueue();
        actionValue = 0;
    }

    public void UpdateMovement()
    {
        // move
        if (move != Vector3.zero)
        {
            charController.Move(move);
            move = Vector3.zero;
        }
    }

    public void UpdateTurning()
    {
        // turn
        // make seperate from movement
        if (turnDir.magnitude > 0.01f)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, turnDir, turnSpeed * Time.deltaTime, turnSpeed);
            move += moveDir * Time.deltaTime * moveSpeed * moveSpeedScale;
        }
    }

    private void UpdateMoveControl()
    {
        //movement direction
        dir = Vector3.zero;
        

        dir.x = h;
        dir.z = v;

        dir.y = 0.0f;

        dir = Vector3.ClampMagnitude(dir, 1.0f);

        moveDir = dir;

        turnDir = Vector3.zero;

        //Turn Direction
        turnDir.x = tH;
        turnDir.z = tV;

        turnDir.y = 0.0f;

        turnDir = Vector3.ClampMagnitude(turnDir, 1.0f);

        //Run key check
        isRun = true;

        // run
        if (isRun == true)
        {
            moveDir *= runSpeedScale;
            turnDir *= runSpeedScale;
        }


        // default gravity
        move = Vector3.down * 0.5f * Time.deltaTime;

        // jump
        if (isJumping && charController.isGrounded)
        {
            isJumping = false;
            animator.SetTrigger("Jump");
        }
    }

    protected void LightAttack()
    {
        if (moveList.Count <= 5)
        {
            moveList.Enqueue(Action.LIGHTATTACK);

            /*if (!isCoolDown)
            {
                StartCoroutine(MoveCoolDown());
                isCoolDown = true;
            }
            else
            {
                moveCoolDownTimer += 1;
            }*/
        }
    }

    protected void HeavyAttack()
    {
        if (moveList.Count <= 5)
        {
            moveList.Enqueue(Action.HEAVYATTACK);

        }
    }

    protected void UpdateQueue()
    {
        if (moveList.Count > 0 && isCoolDown)
        {
            currentState.onInput(moveList.Dequeue());
            print("Stuff");
            StopCoroutine("ResetCombo");
            isCoolDown = false;
            StartCoroutine(MoveCoolDown());
        }
        else if (isCoolDown && actionValue != 0/*currentState != startState*/)
        {
            
            print("should reset here");
            //SetState(startState);
        }
        else if (isCoolDown && isReset)
        {
            StartCoroutine("ResetCombo");
        }
    }

    internal void SetState(ComboStateMachine state)
    {

  
        switch (state.moveNumber)
        {
            case 0:
                //animator.SetInteger("Action", 0);
                actionValue = 0;
                print("idle");
                break;
            case 1:
                //animator.SetInteger("Action", 1);
                actionValue = 1;
                print("light hit 1");
                break;
            case 2:
                actionValue = 4;
                print("Heavy Hit");
                //animator.SetInteger("Action", 4);
                break;
            case 3:
                actionValue = 2;
                print("Combo 2");
                //animator.SetInteger("Action", 2);
                break;
            case 4:
                actionValue = 2;
                print("Combo2");
                //animator.SetInteger("Action", 2);
                break;
            case 5:
                actionValue = 3;
                print("light hit 3");
                //animator.SetInteger("Action", 3);
                break;
            case 6:
                actionValue = 4;
                print("finale");
                //animator.SetInteger("Action", 4);
                break;
            case 7:
                actionValue = 4;
                print("finale");
                //animator.SetInteger("Action", 4);
                break;
            default:
                break;
        }

        animator.SetInteger("Action", actionValue);
        currentState = state;

        tempActionValue = actionValue;

        

        StartCoroutine(MoveCoolDown());
        StartCoroutine("ResetCombo");
    }

    public void SetEndCombo()
    {
        StartCoroutine("ResetCombo");
    }

    // Check Action Input
    private void UpdateActionControl()
    {
        AnimatorStateInfo tempInfo;

        
        /*if (tempActionValue != actionValue)
        {
            
            tempInfo = animator.GetCurrentAnimatorStateInfo(0);
            moveCoolDownTimer = tempInfo.length;
            print("stuff: " + tempInfo.length);
        }*/
        /*int actionValue = 0;

        for (int i = 0; i < actionList.Length; i++)
        {
            if (Input.GetKey(actionList[i].m_keyCode) == true)
            {
                actionValue = i + 1;
                break;
            }
        }*/
        /*if (true) //reset
        {
            currentState = startState;
            actionValue = 0;
        }*/

        //animator.SetInteger("Action", actionValue);
    }

    //Animation Events
    private void EventSkill(string skillName)
    {
        SendMessage(skillName, SendMessageOptions.DontRequireReceiver);
    }

    //Animation Events
    private void EventAttack()
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
            BaseControllerOld charControl = col.GetComponent<BaseControllerOld>();
            if (charControl == null)
                continue;

            if (charControl == this)
                continue;

            charControl.TakeDamage(this, center, transform.forward, 1.0f);
        }
    }

    public virtual void TakeDamage(BaseControllerOld other, Vector3 hitPosition, Vector3 hitDirection, float amount)
    {
        //-------------------------
        // Please enter your code.
        //
        // - 
        // - animation reaction
        // - Direction
        // ...
        //-------------------------

        if ((health -= amount) < 0)
        {
            Death();
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

    protected virtual void Death()
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator MoveCoolDown()
    {
        AnimatorStateInfo tempInfo;
        tempInfo = animator.GetCurrentAnimatorStateInfo(0);
        moveCoolDownTimer = tempInfo.length;

        yield return new WaitForSeconds(moveCoolDownTimer);

        actionValue = 0;
        animator.SetInteger("Action", actionValue);
        print("moveOver");

        //currentState = startState;
        isCoolDown = true;
        isReset = true;
    }

    private IEnumerator ResetCombo()
    {
        isReset = false;
        yield return new WaitForSeconds(1);
        isReset = true;
        print("Reset");
        SetState(startState);
    }

    //old code that im still referencing. soon to be phased out.
    //----------------------------------------------------------------------------

    /*protected void Move(float h, float v)
    {
        bool isRunning = false;

        movement.Set(h, 0f, v);

        movement = movement.normalized * speed * Time.deltaTime;

        playerRigidbody.MovePosition(transform.position + movement);

        isRunning = (h == 0.0f && v == 0.0f) ? false : true;
        anim.SetBool("IsRunning", isRunning);
    }

    protected void Turning(float h, float v)
    {
        if (h != 0 || v != 0)
        {

            Vector3 playerLook = (new Vector3(h, 0f, v).normalized * rotationSpeed * Time.deltaTime) + transform.position;
            playerLook.y = 0;

            Quaternion newPlayerLook = Quaternion.LookRotation(playerLook);

            playerRigidbody.MoveRotation(newPlayerLook);
        }
    }

    protected void Jump()
    {
        isNotJumping = Physics.Raycast(new Ray(gameObject.transform.position, Vector3.down), jumpRayLength, floorMask);
        playerRigidbody.velocity *= System.Convert.ToInt32(!isNotJumping);
        playerRigidbody.AddForce(Vector3.up * jumpForce * System.Convert.ToInt32(isNotJumping));
    }

    protected void LightAttack()
    {

    }

    protected void HeavyAttack()
    {

    }

    protected void OnTriggerEnter(Collider other)
    {
        if (isPlayer)
        {
            if (other.tag == "Enemy")
            {
                damageTarget = other.gameObject;
            }
        }
        else
        {
            if (other.tag == "Player")
            {
                damageTarget = other.gameObject;
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        damageTarget = (damageTarget == other.gameObject) ? damageTarget = null : null;
    }

    public void DealDamage()
    {

    }

    public void TakeDamage(float value)
    {
        if ((health -= value) < 0)
        {
            Death();
        }
    }

    private void Death()
    {

    }*/
}
