using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("AI")]
public class AIHit : AIBase {        

    // Code that runs on entering the state.
    public override void OnEnter() {
        Debug.Log("Hit");
        Fsm.KillDelayedEvents();
        animComp.Play("beating", PlayMode.StopAll);
        _hitRecoverTimer.Value = pawnComp.hitCD;
    }

    // Code that runs every frame.
    public override void OnUpdate() {
        base.OnUpdate();
        
        if(_hitRecoverTimer.Value <= 0) {
            Fsm.Event("hitRecovered");            
        }
    }

    public override void OnFixedUpdate() {

    }

    public override void OnLateUpdate() {

    }

    // Code that runs when exiting the state.
    public override void OnExit() {

    }
}
