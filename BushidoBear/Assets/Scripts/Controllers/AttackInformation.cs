using UnityEngine;
using System.Collections;

public class AttackInformation
{
    protected int animationNumber;
    protected float attackDamage;

    public AttackInformation(int animationNumber, float attackDamage)
    {
        this.animationNumber = animationNumber;
        this.attackDamage = attackDamage;
    }

    public int GetAnimationNumber()
    {
        return animationNumber;
    }

    public float GetAttackDamage ()
    {
        return attackDamage;
    }
}
