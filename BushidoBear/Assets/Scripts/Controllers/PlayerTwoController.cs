using UnityEngine;
using System.Collections;

public class PlayerTwoController : BasePlayerController 
{
    
    public override void Start()
    {
		base.Start();
        gamePad = 2;
    }

}

/*//this is for testing input listening
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
         }*/
