using UnityEngine;
using System.Collections;

public class CombatBoundaries {

	public float leftBoundary;
	public float rightBoundary;
	public float closeBoundary;
	public float farBoundary;

	public CombatBoundaries(float leftBoundary = 0, float rightBoundary = 0, float closeBoundary = -1, float farBoundary = 14){
		this.leftBoundary = leftBoundary;
		this.rightBoundary = rightBoundary;
		this.closeBoundary = closeBoundary;
		this.farBoundary = farBoundary;
	}

}
