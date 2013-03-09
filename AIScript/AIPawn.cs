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


	// Use this for initialization
	void Start () {
        animComp.animation[walkAnim].layer = 1;
        animComp.animation[runAnim].layer = 1;
        animComp.animation[idleAnim].layer = 1;
        animComp.animation[attackAnim].layer = 10;
        animComp.animation[hitAnim].layer = 10;
        animComp.animation[dieAnim].layer = 100;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool IsDead(){
        return hp <= 0;
    }
}
