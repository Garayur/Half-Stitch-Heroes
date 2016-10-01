using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KravMagaMacawController : BaseAIController {

	List<ComboNode> rapidStrike = new List<ComboNode>();

	public override void OnEnable ()
	{
		base.OnEnable ();
		rapidStrike.Add(new ComboNode(2, 3, 5, false, AttackEffect.None, new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK }));
		rapidStrike.Add(new ComboNode(3, 2, 5, true, AttackEffect.None, new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK }));
	}

	public virtual void OnDisable() {
		BasePlayerController.OnPlayerBlockEvent -= HandlePlayerBlockingEvent;
	} 


	protected void HandlePlayerBlockingEvent(BasePlayerController player){
		if (target = player.gameObject)
			TargetBlocking ();
	}

	protected void TargetBlocking() {
		//grab and hold down
		//if close enough grab
		Grab();
	}

	//lots of light attacks, often chained
	protected override IEnumerator Attack ()
	{
		yield return new WaitForSeconds(attackFrequency);
		if(IsInRange()) {
			ExecuteCombo(rapidStrike);
		}
	}


}
