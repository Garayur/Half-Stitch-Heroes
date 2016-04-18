//script written by Michael Withers
using UnityEngine;
using System.Collections;

//base class for classes that need to use the invoke function without the influence of timescale
public abstract class NoTimescaleInvokeable : MonoBehaviour 
{
	//provide layer of abstraction for function, allowing passing of variables from another script
	public virtual void UnscaledInvoke(string methodName, float time)
	{
		gameObject.SetActive(true);
		StartCoroutine(UnscaledInvokeCoroutine(methodName, time));
	}

	//delayable invoke function unnaffected by timescale
	protected virtual IEnumerator UnscaledInvokeCoroutine(string methodName, float time)
	{
		yield return UnscaledWaitForSeconds(time);
		Invoke(methodName, 0.0f);
	}

	//coroutine to wait for a set duration
	public static IEnumerator UnscaledWaitForSeconds(float time)
	{
		float start = Time.realtimeSinceStartup;
		while(Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
	}
}
