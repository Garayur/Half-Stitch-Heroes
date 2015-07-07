using UnityEngine;
using System.Collections;

public class PlayerTwoController : BaseController 
{
    KeyCode player2 = KeyCode.Joystick2Button0;

    void Start()
    {
        /*actionList = new Action[1];

        Action lightAttackCombo = new Action();
        lightAttackCombo.m_name = "LightAttack";
        lightAttackCombo.m_keyCode = KeyCode.Joystick2Button2;

        actionList[0] = lightAttackCombo;*/
    }

    void Update()
    {
        h = Input.GetAxisRaw("HorizontalP2");
        v = Input.GetAxisRaw("VerticalP2");

        if (Input.GetKeyDown(KeyCode.Joystick2Button0))
        {
            isJumping = true;
        }

        if (Input.GetKey(KeyCode.Joystick2Button2))
        {
            LightAttack();
        }

        base.Update();
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
