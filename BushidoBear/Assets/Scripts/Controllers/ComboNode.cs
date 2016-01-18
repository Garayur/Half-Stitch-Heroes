using UnityEngine;
using System.Collections;
//individual moves in combo to be set up in TestBushido bear
public class ComboNode 
{
    int length;
    int animationNumber = 0;
    int damage = 0;
    bool clearsQueue = false;
    ControllerActions[] comboSequence;

    public ComboNode (int length, int animNumber, int damage, bool clearsQueue, ControllerActions[] comboList)
    {
        this.length = length;
        animationNumber = animNumber;
        this.damage = damage;
        this.clearsQueue = clearsQueue;
        comboSequence = comboList;
    }

    public int GetDamage()
    {
        return damage;
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
        for (int i = 0; i < actions.Length; i++)
		{
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
