using UnityEngine;
using System.Collections;

using HutongGames.PlayMaker;

public class AIVision : MonoBehaviour {

    Fsm _fsm;

    GameObject _target;
    float _visionRange;

	// Use this for initialization
	void Start () {
        _fsm = GetComponent<PlayMakerFSM>().Fsm;
        _visionRange = GetComponent<AIPawn>().awareRange;

        // find a target, temp
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Monster");
        foreach(GameObject go in targets){
            if(go != this.gameObject){
                _target = go;
                break;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        // check for target visibility. do this at fixed interval
        if(_target != null){
            Vector3 vecToTarget = _target.transform.position - transform.position;
            if(vecToTarget.sqrMagnitude < _visionRange * _visionRange) {
                awareTarget(_target);
            }
        }
	}

    void awareTarget(GameObject target){
        Fsm.EventData.GameObjectData = target;
        _fsm.Event("awareTarget");
    }
}
