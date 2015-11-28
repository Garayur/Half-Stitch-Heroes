using UnityEngine;
using System.Collections;
//individual moves in combo to be set up in TestBushido bear
public class ComboNode 
{
    int length;
    int animationNumber = 0;
    bool clearsQueue = false;
    ControllerActions[] comboSequence;

    public ComboNode (int length, int animNumber, bool clearsQueue, ControllerActions[] comboList)
    {
        this.length = length;
        animationNumber = animNumber;
        this.clearsQueue = clearsQueue;
        comboSequence = comboList;
    }

    public int GetAnimation ()
    {
        return animationNumber;
    }

    public bool IsLastCombo()
    {
        return clearsQueue;
    }

    public bool isMatchingCombo (ControllerActions[] actions)
    {
        Debug.Log("is this working");
        for (int i = 0; i < actions.Length; i++)
        {
            Debug.Log("Combo Sequence: " + comboSequence[i] + "\nActions: " + actions[i]);
            if (comboSequence[i] != actions[i])
            {
                return false;
            }
        }
        return true;
    }

	public ControllerActions[] GetComboSequence(){
		return comboSequence;
	}

	public int GetLength() {
		return length;
	}
}
