using UnityEngine;
using System.Collections;

public class AIPawn : MonoBehaviour {

    public int hp;

    public float awareRange;
    public float chaseRange;
    public float attackRange;
    public float attackCD;
    public float hitCD;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool IsDead(){
        return hp <= 0;
    }
}
