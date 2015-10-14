using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KravMagaMacawController : AIBaseController {

	List<ComboNode> rapidStrike = new List<ComboNode>();

	public override void Start(){
		base.Start();
		rapidStrike.Add(new ComboNode(2, 0, false, new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK }));
		rapidStrike.Add(new ComboNode(3, 0, true, new ControllerActions[] { ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK, ControllerActions.LIGHTATTACK }));
	}

	protected override void TargetBlocking() {
		//grab and hold down
		//if close enough grab
		Grab();
	}

	//lots of light attacks, often chained
	protected override void Attack ()
	{
		ExecuteCombo(rapidStrike);
	}
	
}
