using UnityEngine;
using System.Collections;

// this component must be the first one!

public class NetworkComponent : MonoBehaviour {

    float _initHiddenTimer = 2.0f / 30.0f;
    AIPawn _pawn;
    PMAIComponent _aiComp;

    bool _isOwner;
    bool _isReady = false;
    Vector3 _syncedTarget;
    Quaternion _syncedRotation;

    float _syncTimerCD = 0.0f;
    float _syncTimer = 0;

    void Awake(){
        _pawn = GetComponent<AIPawn>();
        _aiComp = GetComponent<PMAIComponent>();        
    }

	// Use this for initialization
	void Start () {        
        _syncedTarget = _pawn.currentTarget;
        _syncedRotation = transform.rotation;

        if(IsOwner()){
            StartCoroutine(TransformUpdateLoop(2.0f));
        }        
	}
	
	// Update is called once per frame
	void Update () {
        if(_initHiddenTimer > 0 && _isReady){
            _initHiddenTimer -= Time.deltaTime;
            if(_initHiddenTimer <= 0){
                if(!IsOwner()){
                    _pawn.enabled = true;
                    _pawn.animComp.gameObject.SetActive(true);
                    if(_pawn.IsDead()){
                        MakeDeadPose();
                    }                    
                }
            }
        }

        if(_pawn.IsDead()) return;

        if(IsOwner()) {
            _syncTimer -= Time.deltaTime;
            if(_syncTimer <= 0) {
                _syncedTarget = _pawn.currentTarget;
                _syncedRotation = transform.rotation;

                _syncTimer = _syncTimerCD;
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

    [RPC]
    void RPC_TransformUpdate(Vector3 position, Quaternion rotation){
        float threshold = 0.3f;        
        if((transform.position - position).sqrMagnitude > threshold) transform.position = position;
    }

    [RPC]
    void RPC_RequestFullUpdate(NetworkPlayer sender){
        networkView.RPC("RPC_FullUpdate", sender, transform.position, transform.rotation, _pawn.currentTarget, _pawn.hp);
        Debug.Log("Full update request received from:" + sender);
    }

    [RPC]
    void RPC_FullUpdate(Vector3 position, Quaternion rotation, Vector3 target, int hp){
        transform.position = position;
        transform.rotation = rotation;
        _pawn.currentTarget = target;
        _pawn.hp = hp;

        _isReady = true;
        Debug.Log("full update received");
    }

    IEnumerator TransformUpdateLoop(float wait){
        while(true){
            //Debug.Log( (_pawn.currentTarget-transform.position).sqrMagnitude);
            networkView.RPC("RPC_TransformUpdate", RPCMode.Others, transform.position, transform.rotation);
            yield return new WaitForSeconds(wait);
        }
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

    IEnumerator OnPawnDead(){
        gameObject.GetComponent<CharacterController>().enabled = false;
        _pawn.enabled = false;
        
        yield return new WaitForSeconds(5.0f);
        Network.Destroy(gameObject);
    }    

    bool IsOwner(){
        return _isOwner;
    }

    void OnNetworkInstantiate(NetworkMessageInfo info) {
        if(networkView.isMine) _isOwner = true;
        else _isOwner = false;

        Debug.Log("net inited");

        if(!IsOwner()) {
            PlayMakerFSM fsm = GetComponent<PlayMakerFSM>();
            if(fsm != null) fsm.enabled = false;

            if(_aiComp != null) _aiComp.enabled = false;

            _pawn.enabled = false;
            _pawn.animComp.gameObject.SetActive(false);            

            // request full update
            networkView.RPC("RPC_RequestFullUpdate", info.sender, Network.player);

            _isReady = false;
        }
        else {
            _isReady = true;
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
        if(stream.isWriting) {
            //Debug.Log("is writing:" + _pawn.hp);                        
            stream.Serialize(ref _pawn.hp);
            stream.Serialize(ref _syncedTarget);
            stream.Serialize(ref _syncedRotation);
        }        
        else {            
            stream.Serialize(ref _pawn.hp);
            stream.Serialize(ref _syncedTarget);
            stream.Serialize(ref _syncedRotation);
            //Debug.Log("is receiving:" + _pawn.hp + " " + _syncedTarget);
            if(IsOwner()){
                Debug.LogError("Owner is receiving!!");
            }
            _pawn.currentTarget = _syncedTarget;
            _pawn.transform.rotation = _syncedRotation;
        }
    }
}
