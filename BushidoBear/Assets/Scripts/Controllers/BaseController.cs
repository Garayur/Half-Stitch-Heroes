using UnityEngine;
using System.Collections;

public class BaseController : MonoBehaviour
{
    [System.Serializable]
    public struct Action
    {
        public string m_name;		//Action Animation Name
        public KeyCode m_keyCode;	//Input Keycode
    };

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
    protected bool isRun = false;
    protected bool isJumping = false;
    protected float moveSpeedScale = 1.0f;
    protected float h, v;

    //---------------
    // private
    //---------------
    private CharacterController charController = null;
    private Animator animator = null;
    private Vector3 moveDir = Vector3.zero;
    private int actionValue;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
    }

    protected void Update()
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

        //------------------
        //Parameters sync
        //------------------
        float speed = moveDir.magnitude;
        if (isRun == true) speed *= runSpeedScale;

        animator.SetFloat("Speed", speed, 0.05f, Time.deltaTime);
        animator.SetBool("Ground", charController.isGrounded);
        animator.SetFloat("AirTime", airTime);
    }

    protected void FixedUpdate()
    {
        airTime = animator.GetBool("Ground") ? 0.0f : airTime + Time.deltaTime;
    }

    private void UpdateControl()
    {
        UpdateMoveControl();
        UpdateActionControl();
        actionValue = 0;
    }

    private void UpdateMoveControl()
    {
        Vector3 dir = Vector3.zero;
        Vector3 move = Vector3.zero;

        dir.x = h;
        dir.z = v;

        dir.y = 0.0f;

        dir = Vector3.ClampMagnitude(dir, 1.0f);

        moveDir = dir;

        //Run key check
        isRun = true;

        // run
        if (isRun == true)
        {
            dir *= runSpeedScale;
        }


        // default gravity
        move = Vector3.down * 0.5f * Time.deltaTime;


        // turn
        if (dir.magnitude > 0.01f)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, dir, turnSpeed * Time.deltaTime, turnSpeed);
            move += dir * Time.deltaTime * moveSpeed * moveSpeedScale;
        }

        // jump
        if (isJumping && charController.isGrounded)
        {
            isJumping = false;
            animator.SetTrigger("Jump");
        }


        // move
        if (move != Vector3.zero)
        {
            charController.Move(move);
        }

    }

    protected void LightAttack()
    {
        actionValue = 1;
    }

    protected void HeavyAttack()
    {
        actionValue = 2;
    }

    private void ComboStateMachine (int attackType)
    {
        switch (actionValue)
        {
            case 1:
                break;
            case 2:
                break;
            default:
                break;
        }
    }

    // Check Action Input
    private void UpdateActionControl()
    {
        /*int actionValue = 0;

        for (int i = 0; i < actionList.Length; i++)
        {
            if (Input.GetKey(actionList[i].m_keyCode) == true)
            {
                actionValue = i + 1;
                break;
            }
        }*/

        animator.SetInteger("Action", actionValue);
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
            BaseController charControl = col.GetComponent<BaseController>();
            if (charControl == null)
                continue;

            if (charControl == this)
                continue;

            charControl.TakeDamage(this, center, transform.forward, 1.0f);
        }
    }

    public void TakeDamage(BaseController other, Vector3 hitPosition, Vector3 hitDirection, float amount)
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

    private void Death()
    {
        throw new System.NotImplementedException();
    }

    public string GetHelpText()
    {
        string text = "";

        foreach (Action action in actionList)
        {
            text += action.m_keyCode.ToString() + " : " + action.m_name + "\n";
        }

        return text;
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
