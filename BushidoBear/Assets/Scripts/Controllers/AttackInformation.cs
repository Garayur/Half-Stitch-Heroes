using UnityEngine;
using System.Collections;

public class AttackInformation
{
    protected int animationNumber;
    protected int attackDamage;

    public AttackInformation(int animationNumber, int attackDamage)
    {
        this.animationNumber = animationNumber;
        this.attackDamage = attackDamage;
    }

    public int GetAnimationNumber()
    {
        return animationNumber;
    }

    public int GetAttackDamage ()
    {
        return attackDamage;
    }
}
