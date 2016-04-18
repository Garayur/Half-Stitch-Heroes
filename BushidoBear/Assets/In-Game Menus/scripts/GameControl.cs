//script written by Michael Withers
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

//a collection of useful functions to control aspects of the game,
//all functions exist as both instance based and static functions
//in order to be able to be called from either code or an inspector-set callback
public class GameControl : MonoBehaviour 
{
	//load the main menu
	public void ILoadMainMenu()
	{
		SceneManager.LoadScene(0);
	}

	public static void LoadMainMenu()
	{
		SceneManager.LoadScene(0);
	}

	//quit the game
	public void IQuitGame()
	{
		Application.Quit();
	}

	public static void QuitGame()
	{
		Application.Quit();
	}
}
