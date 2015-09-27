using UnityEngine;
using System.Collections;

public class ComboNode 
{
    int length;
    int animationNumber = 0;
    ControllerActions[] comboSequence;

    public int GetAnimation ()
    {
        return animationNumber;
    }

    public bool isMatchingCombo (ControllerActions[] actions)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            if (comboSequence[i] != actions[i])
            {
                return false;
            }
        }
        return true;
    }
}
