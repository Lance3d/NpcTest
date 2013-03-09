using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("AI")]
public class AIAttackFrame : AIBase {

    // Code that runs on entering the state.
    public override void OnEnter() {
        Debug.Log("attack frame");

        PMAIComponent tgtAiComp = _target.Value.GetComponent<PMAIComponent>();
        tgtAiComp.Hitby(aiComp);

        Finish();
    }

    // Code that runs when exiting the state.
    public override void OnExit() {

    }
}
