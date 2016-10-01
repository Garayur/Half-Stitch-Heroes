using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugUIScript : MonoBehaviour {

	Text text;
	public GameObject target;

	void Awake(){
		text = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "Grounded: " + target.GetComponent<BaseController>().IsGrounded() ;
	}
}
