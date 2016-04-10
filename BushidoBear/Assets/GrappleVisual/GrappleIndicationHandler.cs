//script written by Michael Withers

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


/*
 * IMPORTANT: A properly linked prefab MUST exist in a scene for this script to function properly,
 * do not call the static begin and end functions without one.
 */
public class GrappleIndicationHandler : MonoBehaviour 
{
	//----------
	//public
	public static List<GrappleIndicationHandler> instances = new List<GrappleIndicationHandler>();

	public float buttonPressIndicatorFrequency = 0.1f;
	public bool showPlayerBreakProgress = false;

	//ALL these things need to be assigned in the inspector for this script to function
	public GameObject indicatorPosition = null;
	public Canvas indicatorCanvas = null;
	public Image buttonUp = null;
	public Scrollbar pressIndicatorScrollBar = null;
	public Scrollbar breakDistanceScrollbar = null;


	//-----------
	//private
	private float timeToNextButtonToggle = 0.0f;
	private GameObject grappleSource = null;
	private GameObject grappleTarget = null;
	private bool isGrappling = false;
	private float maxGrappleProgressAmount = 20;
	private bool playerIsGrappler = false;


	//call this function from anywhere to begin a grapple instantiates and pools more prefabs if multiple grapples occur simultaneously
	public static void BeginGrapple(GameObject source, GameObject target, bool _playerIsGrappler)
	{
		//grab unused instance of grapple indicator prefab, or create one if none exist
		GrappleIndicationHandler instance = null;
		for(int i = 0; i < instances.Count; i++)
		{
			if(!instances[i].gameObject.activeSelf)
			{
				instance = instances[i];
			}
		}
		if(instance == null)
		{
			if(instances[0] == null)
			{
				Debug.LogError("No Grapple Visual Feedback object was found in the scene.");
			}
			GameObject temp = Instantiate(instances[0].gameObject);
			instance = temp.GetComponent<GrappleIndicationHandler>();
		}

		instance.isGrappling = true;
		instance.gameObject.SetActive(true);
		instance.grappleSource = source;
		instance.grappleTarget = target;
		instance.playerIsGrappler = _playerIsGrappler;

		//only enable success indicator if player initiated grapple, otherwise show break progress indicator(if enabled)
		if(instance.playerIsGrappler)
		{
			instance.pressIndicatorScrollBar.gameObject.SetActive(true);
			instance.breakDistanceScrollbar.gameObject.SetActive(false);
		}
		else
		{
			instance.pressIndicatorScrollBar.gameObject.SetActive(false);
			if(instance.showPlayerBreakProgress)
			{
				instance.breakDistanceScrollbar.gameObject.SetActive(true);
			}
			else
			{
				instance.breakDistanceScrollbar.gameObject.SetActive(false);
			}
		}
	}

	//call this function from anywhere to end grapple
	public static void EndGrapple(GameObject grappler)
	{
		foreach(GrappleIndicationHandler indicator in instances)
		{
			if(indicator.gameObject.activeSelf && (indicator.grappleSource == grappler || indicator.grappleTarget == grappler))
			{
				indicator.isGrappling = false;
				indicator.gameObject.SetActive(false);
			}
		}

	}


	//add self to pool of grapple indicators
	void Start()
	{
		instances.Add(this);
		if(!isGrappling)
		{
			gameObject.SetActive(false);
		}
	}

	//handle display of grappling information
	void LateUpdate() 
	{
		//if grappling, flash button press and move indicator between the grapplers
		if(isGrappling)
		{
			//find correct location to display indicator
			Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, grappleSource.transform.position - (grappleSource.transform.position - grappleTarget.transform.position)/2);
			RectTransformUtility.ScreenPointToWorldPointInRectangle(indicatorCanvas.transform as RectTransform, screenPosition, indicatorCanvas.worldCamera, out screenPosition);
			indicatorPosition.transform.position = screenPosition;

			if(timeToNextButtonToggle <= 0)
			{
				timeToNextButtonToggle = buttonPressIndicatorFrequency;
				buttonUp.gameObject.SetActive(!buttonUp.gameObject.activeSelf);
			}
			else
			{
				timeToNextButtonToggle -= Time.deltaTime;
			}

			if(playerIsGrappler)
			{
				pressIndicatorScrollBar.value = grappleSource.GetComponent<BasePlayerController>().getGrip / maxGrappleProgressAmount;
			}
			else if(showPlayerBreakProgress)
			{
				breakDistanceScrollbar.value = (maxGrappleProgressAmount - grappleTarget.GetComponent<BasePlayerController>().getGrip) / maxGrappleProgressAmount;
			}
		}
	}
}
