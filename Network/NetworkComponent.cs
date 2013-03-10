using UnityEngine;
using System.Collections;

public class NetworkComponent : MonoBehaviour {

    float _initHiddenTimer = 2.0f / 30.0f;
    AIPawn _pawn;

    void Awake(){
        _pawn = GetComponent<AIPawn>();

        if(!Network.isServer) {
            PlayMakerFSM fsm = GetComponent<PlayMakerFSM>();
            if(fsm != null) fsm.enabled = false;

            PMAIComponent aiComp = GetComponent<PMAIComponent>();
            if(aiComp != null) aiComp.enabled = false;            
            
            _pawn.animComp.gameObject.SetActive(false);            
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
                    _pawn.animComp.gameObject.SetActive(true);
                    if(_pawn.IsDead()){
                        MakeDeadPose();
                    }                    
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

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
        if(stream.isWriting) {
            //Debug.Log("is writing");                        
            stream.Serialize(ref _pawn.hp);
        }        
        else {
            //Debug.Log("is receiving");            
            stream.Serialize(ref _pawn.hp);
        }
    }
}
