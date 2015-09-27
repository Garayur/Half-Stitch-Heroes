using UnityEngine;
using System.Collections;

public class DummyController : BaseControllerOld 
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        base.Update();
	}
    protected override void CheckMoveSet()
    {
        Debug.Log("not Implemented Yet");
    }
}
