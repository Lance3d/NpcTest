using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("AI")]
public class AIAttack : AIBase
{    
	// Code that runs on entering the state.
	public override void OnEnter()
	{
        Debug.Log("Start Attacking");        
	}
    
    // Code that runs every frame.
    public override void OnUpdate() {
        base.OnUpdate();

        if(IsTargetDead(_target.Value)){
            Fsm.Event("targetDied");
            return;
        }
        
        if(_attackCDTimer.Value <= 0) {
            // in range?
            Vector3 dis = _target.Value.transform.position - Owner.transform.position;
            if(dis.sqrMagnitude > pawnComp.attackRange * pawnComp.attackRange) {
                Fsm.Event("targetOutOfRange");
            }
            else{                
                Debug.Log("attacking");
                animComp.Play(pawnComp.attackAnim);//, 0.1f);                

                aiComp.SendFsmEventDelayed("attackFrame", pawnComp.attackFrame / 30.0f);
            }
            _attackCDTimer.Value = pawnComp.attackCD;
        }
        else{
            if(animComp[pawnComp.attackAnim].weight == 0){
                animComp.CrossFade(pawnComp.idleAnim, 0.1f);
            }
        }
    }

	public override void OnFixedUpdate()
	{
		
	}

	public override void OnLateUpdate()
	{
		
	}

	// Code that runs when exiting the state.
	public override void OnExit()
	{
		
	}
}
