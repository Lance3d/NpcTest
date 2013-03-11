using UnityEngine;
using System.Collections;

public class AIPawn : MonoBehaviour {

    public int hp;

    public float awareRange;
    public float chaseRange;
    public float attackRange;
    public float attackCD;
    public float hitCD;

    public Animation animComp;
    public string walkAnim;
    public string runAnim;
    public string idleAnim;
    public string attackAnim;
    public string hitAnim;
    public string dieAnim;

    public int attackFrame;

    [HideInInspector]
    public Vector3 currentTarget;   // 主要在状态机中修改。在NetworkComponent中同步    
    CharacterController _Controller;


	// Use this for initialization
	void Start () {
        animComp.animation[walkAnim].layer = 1;
        animComp.animation[runAnim].layer = 1;
        animComp.animation[idleAnim].layer = 1;
        animComp.animation[attackAnim].layer = 10;
        animComp.animation[hitAnim].layer = 10;
        animComp.animation[dieAnim].layer = 100;

        _Controller = GetComponent<CharacterController>();
        currentTarget = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 dir = currentTarget - transform.position;
        if(dir.sqrMagnitude > 0.01f){
            MoveTo(currentTarget);
        }
	}

    public void MoveTo(Vector3 target) {
        float speed = 4;
        _Controller.SimpleMove((target - transform.position).normalized * speed);
        transform.LookAt(target);        
    }

    public bool IsDead(){
        return hp <= 0;
    }
}
