using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public GameObject prefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI(){

        float refWidth = 640.0f;
        float refHeight = 960.0f;

        if(Network.peerType != NetworkPeerType.Disconnected && !Application.isLoadingLevel) {
            if(GUI.Button(new Rect(refWidth / 2 - 140, refHeight / 2, 280, 60), "Ë¢¹Ö")) {
                Vector3 offset = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                Network.Instantiate(prefab, transform.position + offset, Quaternion.identity, 0);
            }
        }
    }
}
