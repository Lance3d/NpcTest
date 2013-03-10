using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public GameObject prefab1;
    public GameObject prefab2;

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
            if(GUI.Button(new Rect(refWidth / 2 - 140, refHeight / 2, 240, 60), "Ë¢¹Ö1")) {
                Vector3 offset = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                Network.Instantiate(prefab1, transform.position + offset, Quaternion.identity, 0);
            }

            if(GUI.Button(new Rect(refWidth / 2 + 140, refHeight / 2, 240, 60), "Ë¢¹Ö2")) {
                Vector3 offset = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                Network.Instantiate(prefab2, transform.position + offset, Quaternion.identity, 0);
            }
        }
    }
}
