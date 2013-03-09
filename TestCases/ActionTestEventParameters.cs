using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("TestCases")]
public class ActionTestEventParameters : FsmStateAction {

    // Code that runs on entering the state.
    public override void OnEnter() {
        Debug.Log("The int data is " + Fsm.EventData.IntData);
        Debug.Log("The gameobject data is " + Fsm.EventData.GameObjectData);
    }

    // Code that runs every frame.
    public override void OnUpdate() {
    }

    // Code that runs when exiting the state.
    public override void OnExit() {

    }


}
