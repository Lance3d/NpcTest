using UnityEngine;
using System.Collections;

using HutongGames.PlayMaker;

[RequireComponent(typeof(PlayMakerFSM))]
public class PMAIComponent : MonoBehaviour {

    AIPawn _pawn;
    Fsm _fsm;    

	// Use this for initialization
	void Start () {
        _pawn = GetComponent<AIPawn>();
        _fsm = GetComponent<PlayMakerFSM>().Fsm;        
	}
	
	// Update is called once per frame
	void Update () {

	}

    #region AIComponent interface methods

    public void Hitby(PMAIComponent attacker){
        int damage = 1;
        _pawn.hp -= damage;

        if(_pawn.hp > 0) SendFsmEvent("getHit");
        else SendFsmEvent("died");
    }

    #endregion

    #region PlayMaker Fsm relative functions

    void SendFsmEvent(string eventName){
        _fsm.Event(eventName);
    }

    public void SendFsmEventDelayed(string eventName, float delay){
        StartCoroutine(SendDelayedEvent(eventName, delay));
    }

    IEnumerator SendDelayedEvent(string eventName, float delay){
        yield return new WaitForSeconds(delay);
        SendFsmEvent(eventName);
    }

    #endregion
}
