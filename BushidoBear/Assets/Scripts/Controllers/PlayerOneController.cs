using UnityEngine;
using System.Collections;

public class PlayerOneController : BasePlayerController
{
	public override void Start()
	{
		base.Start();
		gamePad = 1;
	}
}
