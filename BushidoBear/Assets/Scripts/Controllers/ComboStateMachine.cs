using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class ComboStateMachine 
{
    public struct InputWrapper
    {
        public float delay;
        public ComboStateMachine state;
    }

    static BaseController controller;
    Dictionary<Action, InputWrapper> paths = new Dictionary<Action,InputWrapper>();

    public int moveNumber;

    static public Stopwatch timer;

	public void SetUp(BaseController baseController)
    {
        controller = baseController;
    }

    public void AddPath (Action key, ComboStateMachine combo, float delay)
    {
        //UnityEngine.Debug.Log(combo.moveNumber);
        InputWrapper temp = new InputWrapper();
        temp.delay = delay;
        temp.state =  combo;
        paths.Add(key, temp);
    }

    public void onInput(Action input) 
    {
        float delay;
        if (timer == null)
        {
            delay = -1;
        }
        else
        {
            timer.Stop();
            delay = timer.Elapsed.Seconds;
        }

        if (paths.ContainsKey(input))
        {
            if(paths[input].delay < delay  || delay == -1) 
            {
                controller.setState(paths[input].state);
                timer = new Stopwatch();
                timer.Start();
            }
            else
            {
                delay = 0;
                timer = null;
            }
        }
        else
        {
            timer = new Stopwatch();
            timer.Start();
        }
    }
}
