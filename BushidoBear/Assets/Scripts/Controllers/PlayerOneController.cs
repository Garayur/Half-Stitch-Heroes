using UnityEngine;
using System.Collections;

public class PlayerOneController : BaseController
{
    void Start()
    {
        //jumpRayLength = 0.3f;
    }

	void Update () 
    {
        float h = Input.GetAxisRaw("HorizontalP1");
        float v = Input.GetAxisRaw("VerticalP1");

        //Move(h, v);

        //Turning(h, v);

        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            //Jump();
        }
	}

    void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, transform.position - (Vector3.up * jumpRayLength));
    }
}
