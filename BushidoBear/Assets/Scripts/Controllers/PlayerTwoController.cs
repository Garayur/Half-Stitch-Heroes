using UnityEngine;
using System.Collections;

public class PlayerTwoController : BaseController 
{
    KeyCode player2 = KeyCode.Joystick2Button0;

    void Start()
    {
        jumpRayLength = 0.3f;
    }

    void Update()
    {
        //this is for testing input listening
         for (int i = 0; i <=19; i++) 
         {
             if (Input.GetKeyDown("joystick 1 button " + i)) { Debug.Log("joystick 1"); }
             if (Input.GetKeyDown("joystick 2 button " + i)) { Debug.Log("joystick 2"); }
             if (Input.GetKeyDown("joystick 3 button " + i)) { Debug.Log("joystick 3"); }
             if (Input.GetKeyDown("joystick 4 button " + i)) { Debug.Log("joystick 4"); }
             if (Input.GetKeyDown("joystick 5 button " + i)) { Debug.Log("joystick 5"); }
             if (Input.GetKeyDown("joystick 6 button " + i)) { Debug.Log("joystick 6"); }
             if (Input.GetKeyDown("joystick 7 button " + i)) { Debug.Log("joystick 7"); }
             if (Input.GetKeyDown("joystick 8 button " + i)) { Debug.Log("joystick 8"); }
         }

        float h = Input.GetAxisRaw("HorizontalP2");
        float v = Input.GetAxisRaw("VerticalP2");

        Move(h, v);

        Turning(h, v);

        if (Input.GetKeyDown(KeyCode.Joystick2Button0))
        {
            Jump();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position - (Vector3.up * jumpRayLength));
    }
}
