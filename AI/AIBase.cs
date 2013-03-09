using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("AI")]
public abstract class AIBase : FsmStateAction {

    protected Animation animComp;
    protected AIPawn pawnComp;
    protected PMAIComponent aiComp;
    
    protected FsmGameObject _target;
    protected FsmFloat _attackCDTimer;
    protected FsmFloat _hitRecoverTimer;

    public override void Awake(){        
        GameObject pawnObj = Fsm.GetFsmGameObject("pawnObj").Value;
        if(pawnObj) {
            animComp = pawnObj.GetComponent<Animation>();            
        }

        aiComp = Fsm.Owner.GetComponent<PMAIComponent>();
        pawnComp = Fsm.Owner.GetComponent<AIPawn>();        
        _target = Fsm.GetFsmGameObject("_target");
        _attackCDTimer = Fsm.GetFsmFloat("_attackCDTimer");
        _hitRecoverTimer = Fsm.GetFsmFloat("_hitRecoverTimer");

        //if(animComp == null){
        //    Debug.LogError("Animation component on AI pawn not found!");
        //}
    }

    // Code that runs every frame.
    public override void OnUpdate() {
        // update timers
        if(_attackCDTimer.Value > 0) _attackCDTimer.Value -= Time.deltaTime;
        if(_hitRecoverTimer.Value > 0) _hitRecoverTimer.Value -= Time.deltaTime;
    }


    #region helper functions

    protected void FindTarget(){
        // find a target, temp
        GameObject target = null;
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Monster");
        foreach(GameObject go in targets) {
            if(go != Owner && !IsTargetDead(go)) {
                target = go;
                break;
            }
        }
        if(target == null) return;

        Vector3 vecToTarget = target.transform.position - Fsm.Owner.transform.position;
        if(vecToTarget.sqrMagnitude < pawnComp.awareRange * pawnComp.awareRange) {
            Fsm.EventData.GameObjectData = target;
            Fsm.Event("awareTarget");
        }
    }

    protected bool IsTargetDead(GameObject target){
        if(target != null){
            AIPawn pawn = target.GetComponent<AIPawn>();
            if(pawn != null && !pawn.IsDead()) return false;
        }
        return true;
    }

    #endregion
}
