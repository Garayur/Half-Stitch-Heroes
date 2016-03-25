using UnityEngine;
using System.Collections;

public class AttackInformation
{
    protected int animationNumber;
    protected int attackDamage;
	protected AttackEffect effect;

    public AttackInformation(int animationNumber, int attackDamage, AttackEffect effect = AttackEffect.None)
    {
        this.animationNumber = animationNumber;
        this.attackDamage = attackDamage;
		this.effect = effect;
    }

    public int GetAnimationNumber() {
        return animationNumber;
    }

    public int GetAttackDamage () {
        return attackDamage;
    }

	public AttackEffect GetAttackEffect (){
		return effect;
	}
}
