using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("TestCases")]
public class ActionTestLoop : FsmStateAction {

    // Code that runs on entering the state.
    public override void OnEnter() {
        Debug.Log("Waiting for " + _timer + " seconds.");
    }

    float _timer = 1.0f;
    // Code that runs every frame.
    public override void OnUpdate() {
        _timer -= Time.deltaTime;
        if(_timer <= 0.0f) {
            Fsm.EventData.IntData = 5;
            Fsm.Event("awareTarget");
        }
    }

    // Code that runs when exiting the state.
    public override void OnExit() {
        Debug.Log("i am done");
    }


}
