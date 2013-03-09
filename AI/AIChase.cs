using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("AI")]
public class AIChase : AIBase
{        
    float _chaseEndRange;

    public override void Reset() {
        base.Reset();        
    }

	// Code that runs on entering the state.
	public override void OnEnter()
	{
        Debug.Log("Start Chasing");
        animComp.CrossFade("run", 0.1f);

        if(Fsm.EventData.GameObjectData != null){
            _target.Value = Fsm.EventData.GameObjectData;            
        }

        _chaseEndRange = pawnComp.attackRange - 0.2f;
	}

    float _timer = 1.0f;
	// Code that runs every frame.
	public override void OnUpdate()
	{
        base.OnUpdate();

        if(IsTargetDead(_target.Value)) {
            Fsm.Event("targetDied");
            return;
        }

        if(_target.Value != null){
            Vector3 vecToTarget = _target.Value.transform.position - Owner.transform.position;
            if(vecToTarget.sqrMagnitude < _chaseEndRange * _chaseEndRange) {
                Fsm.Event("targetInRange");
            }
            else{
                Vector3 targetPos = _target.Value.transform.position;
                float speed = 4;
                Owner.GetComponent<CharacterController>().SimpleMove((targetPos - Owner.transform.position).normalized * speed);
                Owner.transform.LookAt(targetPos);
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
