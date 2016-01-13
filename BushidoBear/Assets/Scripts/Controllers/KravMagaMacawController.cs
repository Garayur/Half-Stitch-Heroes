using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KravMagaMacawController : AIBaseController {

	List<ComboNode> rapidStrike = new List<ComboNode>();

	public override void Start(){
		base.Start();
		rapidStrike.Add(new ComboNode(2, 1, 5, false, new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK }));
		rapidStrike.Add(new ComboNode(3, 4, 5, true, new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK }));
	}

	protected override void TargetBlocking() {
		//grab and hold down
		//if close enough grab
		Grab();
	}

	//lots of light attacks, often chained
	protected override IEnumerator Attack ()
	{
		yield return new WaitForSeconds(attackFrequency);
		if(IsInRange()) {
			Debug.Log("Krav used Rapid Strike!");
			ExecuteCombo(rapidStrike);
		}
	}


}
