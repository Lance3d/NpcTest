using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("AI")]
public class AIPatrol : AIBase
{

	// Code that runs on entering the state.
	public override void OnEnter()
	{
        Debug.Log("start patrol");
        _timer = 2.0f;

        animComp.CrossFade(pawnComp.idleAnim, 0.1f);
        _target.Value = null;
	}

    float _timer = 2.0f;
	// Code that runs every frame.
	public override void OnUpdate()
	{
        base.OnUpdate();

        FindTarget();
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
