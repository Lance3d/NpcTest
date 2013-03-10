using UnityEngine;
using System.Collections;

public class AIComponentRemote : MonoBehaviour {

    float _initHiddenTimer = 2.0f / 30.0f;

    void Awake(){
        if(!Network.isServer) {
            PlayMakerFSM fsm = GetComponent<PlayMakerFSM>();
            if(fsm != null) fsm.enabled = false;

            PMAIComponent aiComp = GetComponent<PMAIComponent>();
            if(aiComp != null) aiComp.enabled = false;            

            AIPawn pawn = GetComponent<AIPawn>();
            Animation anim = pawn.animComp;
            anim.gameObject.SetActive(false);
        }
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if(_initHiddenTimer > 0){
            _initHiddenTimer -= Time.deltaTime;
            if(_initHiddenTimer <= 0){
                if(!Network.isServer){
                    //NetworkView[] netViews = GetComponents<NetworkView>();
                    //foreach(NetworkView view in netViews) view.enabled = false;

                    AIPawn pawn = GetComponent<AIPawn>();
                    pawn.animComp.gameObject.SetActive(true);
                    MakeDeadPose();
                }
            }
        }
	}

    [RPC]
    void RPC_PawnDead(){
        Debug.Log("RPC_PawnDead called");
        //NetworkView[] netViews = GetComponents<NetworkView>();
        //foreach(NetworkView view in netViews) view.enabled = false;
        //GetComponent<AIPawn>().animComp.Play(GetComponent<AIPawn>().dieAnim);
    }

    void MakeDeadPose(){
        AIPawn pawn = GetComponent<AIPawn>();        
        Animation anim = pawn.animComp;
        anim.Stop();
        anim[pawn.dieAnim].enabled = true;
        anim[pawn.dieAnim].normalizedTime = 1.0f;
        anim[pawn.dieAnim].weight = 1.0f;
        anim.Sample();        
    }
}
