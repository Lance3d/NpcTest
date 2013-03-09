using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("AI")]
public class AIDead : AIBase
{

	// Code that runs on entering the state.
	public override void OnEnter()
	{
        Debug.Log("dead");
        animComp.Play(pawnComp.dieAnim, PlayMode.StopAll);        
	}

	// Code that runs every frame.
	public override void OnUpdate()
	{
        base.OnUpdate();
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
