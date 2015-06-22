using UnityEngine;
using System.Collections;

public class BaseController : MonoBehaviour
{
    public float health = 100f;
    public float speed = 6f;                        // The speed that the player will move at.
    public float lightAttackDamage = 5f;
    public float heavyAttackDamage = 10f;
    public float rotationSpeed = 10000f;
    public float jumpForce = 300f;

    protected GameObject damageTarget = null;
    protected Vector3 movement;                     // The vector to store the direction of the player's movement.
    protected Animator anim;                        // Reference to the animator component.
    protected Rigidbody playerRigidbody;
    protected int floorMask;                        // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
    protected float jumpRayLength = 1.3f;           // The length of the ray checking if the player has Jumped
    protected bool isNotJumping = true;
    protected bool isPlayer;

    protected void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");

        anim = GetComponent<Animator>();

        playerRigidbody = GetComponent<Rigidbody>();
    }

    protected void Move(float h, float v)
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

    }
}
