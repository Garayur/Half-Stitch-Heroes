using UnityEngine;
using System.Collections;

enum AIState {StartingAnimation, Positioning, Combat, Flinching, Fallen, Dying};

public class AIControllerBase : BaseController {

	AIState currentState;
	float timer;
	
	void Start () {
		currentState = AIState.StartingAnimation;
	}

	void Update () {
		switch (currentState) {
		case AIState.StartingAnimation:
			break;
		case AIState.Positioning:
			break;
		case AIState.Combat:
			break;
		case AIState.Flinching:
			break;
		case AIState.Fallen:
			break;
		case AIState.Dying:
			break;
		}

	}

	protected virtual void StartAnimation() {
		//enemy actions if already on screen when player reaches AI
	}

	protected virtual void Positioning() {
		//moving around the screen when AI has entered combat
	}

	protected virtual void Combat() {
		//attacking a player
	}

	protected virtual void Flinch(){
		//has flinched
	}

	protected virtual void Fall() {
		//has fallen to the ground will get up again
	}

	protected virtual void Dying() {
		//has died, play any dying animations at the end of this remove this character from screen
	}
}
