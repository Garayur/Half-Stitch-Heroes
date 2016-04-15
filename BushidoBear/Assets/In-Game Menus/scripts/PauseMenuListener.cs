//script written by Michael Withers
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//listens for press of the pause button, and pauses or resumes the game as needed,
//pausing and resuming are done by adjusting timescale and executing inspector-set callback functions
public class PauseMenuListener : MonoBehaviour 
{
	//----------
	//private
	private bool isGamePaused = false;

	//----------
	//public
	public EventTrigger.TriggerEvent OnPause;
	public EventTrigger.TriggerEvent OnResume;

	//call onpause functions
	public void ExecuteOnPauseCallbacks()
	{
		BaseEventData data = new BaseEventData(EventSystem.current);
		data.selectedObject = gameObject;
		OnPause.Invoke(data);
	}

	//call onresume functions
	public void ExecuteOnResumeCallbacks()
	{
		BaseEventData data = new BaseEventData(EventSystem.current);
		data.selectedObject = gameObject;
		OnResume.Invoke(data);
	}

	// Update is called once per frame
	void Update() 
	{
		if(Input.GetButtonDown("Pause"))
		{
			if(isGamePaused)
			{
				UnPause();
			}
			else
			{
				Pause();
			}
		}
	}

	//pause the game
	public void Pause()
	{
		Time.timeScale = 0;
		ExecuteOnPauseCallbacks();
		isGamePaused = true;
	}

	//resume the game
	public void UnPause()
	{
		Time.timeScale = 1;
		ExecuteOnResumeCallbacks();
		isGamePaused = false;
	}
}
