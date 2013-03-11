using UnityEngine;
using System.Collections;

public class NetworkComponent : MonoBehaviour {

    float _initHiddenTimer = 2.0f / 30.0f;
    AIPawn _pawn;
    PMAIComponent _aiComp;

    Vector3 _syncedTarget;
    Quaternion _syncedRotation;

    float _syncTimerCD = 1.0f;
    float _syncTimer = 0;

    void Awake(){
        _pawn = GetComponent<AIPawn>();
        _aiComp = GetComponent<PMAIComponent>();

        if(!Network.isServer) {
            PlayMakerFSM fsm = GetComponent<PlayMakerFSM>();
            if(fsm != null) fsm.enabled = false;
            
            if(_aiComp != null) _aiComp.enabled = false;            
            
            _pawn.animComp.gameObject.SetActive(false);            
        }
    }

	// Use this for initialization
	void Start () {
        _syncedTarget = _pawn.currentTarget;
        _syncedRotation = transform.rotation;
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

        if(_pawn.IsDead()) return;

        _syncTimer -= Time.deltaTime;
        if(_syncTimer <= 0){
            _syncedTarget = _pawn.currentTarget;
            _syncedRotation = transform.rotation;

            _syncTimer = _syncTimerCD;
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
            Debug.Log("is writing:" + _pawn.hp);                        
            stream.Serialize(ref _pawn.hp);
            stream.Serialize(ref _syncedTarget);
            stream.Serialize(ref _syncedRotation);
        }        
        else {            
            stream.Serialize(ref _pawn.hp);
            stream.Serialize(ref _syncedTarget);
            stream.Serialize(ref _syncedRotation);
            Debug.Log("is receiving:" + _pawn.hp + " " + _syncedTarget);
            _pawn.currentTarget = _syncedTarget;
            _pawn.transform.rotation = _syncedRotation;
        }
    }
}
