using UnityEngine;
using System.Collections;
//individual moves in combo to be set up in TestBushido bear
public class ComboNode 
{
    private int length;
    private int animationNumber = 0;
    private int damage = 0;
    private bool clearsQueue = false;
	private AttackEffect effect = AttackEffect.None;
    private ControllerActions[] comboSequence;

    public ComboNode (int length, int animNumber, int damage, bool clearsQueue, AttackEffect effect, ControllerActions[] comboList)
    {
        this.length = length;
        animationNumber = animNumber;
        this.damage = damage;
        this.clearsQueue = clearsQueue;
		this.effect = effect;
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

	public AttackEffect GetEffect(){
		return effect;
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
