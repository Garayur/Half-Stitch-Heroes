//script written by Michael Withers
//partial dependency on MenuTransitioner.cs
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;


//relay class for handling callbacks, call ExecuteCallbacks to invoke all referenced functions,
//setting pausebetweencallbacks to true will set a delay for the previous function to be complete before executing the next,
//but only works with menutransition functions and callbackrelays, default delay is the default time to delay between callbacks,
//will be overriden if pausebetweencallbacks is true and a menutransition function was called
//
//if used to call a function on a non-menutransition script, invoke's delay IS affected by timescale!
public class CallBackRelay : NoTimescaleInvokeable 
{
	//---------
	//public
	//
	//name to help identify which relay is for which purpose, inspector use only
	public string relayName = "";
	//standard delay to use if items are not callbackrelays or menutransitioners
	public float defaultDelay = 0.0f;
	//if false, this script will return 0 in place of it's completion time
	public bool includeThisInDelayChecks = true;
	//whether ExecuteCallbacks should add appropriate delay to the invocation of its stored functions to allow each to complete before moving to the next
	public bool pauseBetweenCallbackFunctions = false;
	public EventTrigger.TriggerEvent callBacks;

	//executes all held callback functions
	public void ExecuteCallbacks()
	{
		BaseEventData data = new BaseEventData(EventSystem.current);
		data.selectedObject = gameObject;
		float delay = 0.0f;
		for(int i = 0; i < callBacks.GetPersistentEventCount(); i++)
		{
			//call function specified
			if(callBacks.GetPersistentTarget(i) is NoTimescaleInvokeable && delay > 0)
			{
				(callBacks.GetPersistentTarget(i) as NoTimescaleInvokeable).UnscaledInvoke(callBacks.GetPersistentMethodName(i),delay);
			}
			else
			{
				(callBacks.GetPersistentTarget(i) as MonoBehaviour).Invoke(callBacks.GetPersistentMethodName(i),delay);
			}

			//add delay to next function if desired
			if(pauseBetweenCallbackFunctions)
			{
				if(callBacks.GetPersistentTarget(i) is MenuTransitioner)
				{
					if(callBacks.GetPersistentMethodName(i) == "TransitionIn")
					{
						delay += (callBacks.GetPersistentTarget(i) as MenuTransitioner).transitionInType.time;
					}
					else if(callBacks.GetPersistentMethodName(i) == "TransitionOut")
					{
						delay += (callBacks.GetPersistentTarget(i) as MenuTransitioner).transitionOutType.time;
					}
					else
					{
						delay += defaultDelay;
					}
				}
				else if(callBacks.GetPersistentTarget(i) is CallBackRelay)
				{
					delay += (callBacks.GetPersistentTarget(i) as CallBackRelay).GetTotalDelay();
				}
				else
				{
					delay += defaultDelay;
				}
			}
		}
	}

	//returns time needed for this callbackrelay to execute its funciton list
	public float GetTotalDelay()
	{
		float delaySum = 0.0f;
		if(includeThisInDelayChecks)
		{
			for(int i = 0; i < callBacks.GetPersistentEventCount(); i++)
			{
				if(callBacks.GetPersistentTarget(i) is MenuTransitioner)
				{
					if(callBacks.GetPersistentMethodName(i) == "TransitionIn")
					{
						if(pauseBetweenCallbackFunctions)
						{
							delaySum += (callBacks.GetPersistentTarget(i) as MenuTransitioner).transitionInType.time;
						}
						else if ((callBacks.GetPersistentTarget(i) as MenuTransitioner).transitionInType.time > delaySum)
						{
							delaySum = (callBacks.GetPersistentTarget(i) as MenuTransitioner).transitionInType.time;
						}
					}
					else if(callBacks.GetPersistentMethodName(i) == "TransitionOut")
					{
						if(pauseBetweenCallbackFunctions)
						{
							delaySum += (callBacks.GetPersistentTarget(i) as MenuTransitioner).transitionOutType.time;
						}
						else if((callBacks.GetPersistentTarget(i) as MenuTransitioner).transitionOutType.time > delaySum)
						{
							delaySum = (callBacks.GetPersistentTarget(i) as MenuTransitioner).transitionOutType.time;
						}
					}
				}
				else if(callBacks.GetPersistentTarget(i) is CallBackRelay)
				{
					if(pauseBetweenCallbackFunctions)
					{
						delaySum += (callBacks.GetPersistentTarget(i) as CallBackRelay).GetTotalDelay();
					}
					else if((callBacks.GetPersistentTarget(i) as CallBackRelay).GetTotalDelay() > delaySum)
					{
						delaySum = (callBacks.GetPersistentTarget(i) as CallBackRelay).GetTotalDelay();
					}
				}
			}
		}
		return delaySum;
	}
}
