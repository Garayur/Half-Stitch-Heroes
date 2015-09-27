using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestControllerState
{
    public List<ComboStateMachine> SetUp(BaseControllerOld controller)
    {
        List<ComboStateMachine> list = new List<ComboStateMachine>(new ComboStateMachine[8]);

        list[0] = new ComboStateMachine();
        list[1] = new ComboStateMachine();
        list[2] = new ComboStateMachine();
        list[3] = new ComboStateMachine();
        list[4] = new ComboStateMachine();
        list[5] = new ComboStateMachine();
        list[6] = new ComboStateMachine();
        list[7] = new ComboStateMachine();

        list[0].AddPath(Action.LIGHTATTACK, list[1]);
        list[0].AddPath(Action.HEAVYATTACK, list[2]);
        list[0].moveNumber = 0;

        list[1].AddPath(Action.LIGHTATTACK, list[3]);
        list[1].AddPath(Action.HEAVYATTACK, list[4]);
        list[1].moveNumber = 1;

        //nothing connects from 2
        list[2].moveNumber = 2;

        list[3].AddPath(Action.LIGHTATTACK, list[5]);
        //no heavy attack
        list[3].moveNumber = 3;

        //no light attck
        list[4].AddPath(Action.HEAVYATTACK, list[6]);
        list[4].moveNumber = 4;

        //no light attack
        list[5].AddPath(Action.HEAVYATTACK, list[7]);
        list[5].moveNumber = 5;

        //nothing connects to 6 or 7
        list[6].moveNumber = 6; 
        list[7].moveNumber = 7;

        return list;
    }
}
