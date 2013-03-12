using UnityEngine;
using System.Collections;

public class ConnectionGUITest : MonoBehaviour {

    public Font gameFont;

    private string gameName = "PeguinWar";
    private float lastHostListRequest = -1000.0f;
    private float hostListRefreshTimeout = 10.0f;
    private float timer = 0.0f;
    private int serverPort = 25002;
    private bool useNat = false;

    private Rect windowRect;
    private Rect serverListRect = new Rect(0, 300, PlatformUtils.GetReferenceScreenResolution().x, PlatformUtils.GetReferenceScreenResolution().y - 350);
    private bool hideTest = false;
    private bool filterNATHosts = false;
    private bool probingPublicIP = false;
    private bool doneTesting = false;
    private ConnectionTesterStatus connectionTestResult = ConnectionTesterStatus.Undetermined;
    private string testMessage = "Undetermined NAT capabilities";

    string uiInfo = "正在连接...";
    bool showUIInfo = true;
    bool showStartServerButton = false;


    void Awake() {
        //GameState.Instance();
        connectionTestResult = Network.TestConnection();

        // What kind of IP does this machine have? TestConnection also indicates this in the
        // test results
        if(Network.HavePublicAddress())
            Debug.Log("This machine has a public IP address");
        else
            Debug.Log("This machine has a private IP address");
    }

    // Use this for initialization
    void Start() {
        StartCoroutine(ConnectionLoop());
    }

    // Update is called once per frame
    void Update() {
        // If test is undetermined, keep running
        if(!doneTesting)
            TestConnection();
    }

    IEnumerator ConnectionLoop(){
        MasterServer.RequestHostList(gameName);
        while(!doneTesting){
            yield return null;
        }

        Debug.Log("Done Test");

        float timer = 1.0f;
        while(timer >= 0){
            timer -= Time.deltaTime;
            if(MasterServer.PollHostList().Length > 0) break;
            yield return null;
        }

        HostData[] servers = MasterServer.PollHostList();
        if(servers.Length > 0){
            Debug.Log("has server");
            NetworkConnectionError err = Network.Connect(servers[0]);
            Debug.Log(err.ToString());
        }
        else{
            Debug.Log("no server");
            showStartServerButton = true;
        }        
    }

    // Enable this if not running a client on the server machine
    //MasterServer.dedicatedServer = true;

    void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
        Debug.Log(info);
        ShowUIInfo("服务器连接失败...");
    }

    void OnFailedToConnect(NetworkConnectionError info) {
        Debug.Log(info);
        ShowUIInfo("服务器连接失败...");
    }

    void OnGUI() {
        GUI.skin.font = gameFont;
        if(!doneTesting) {            
            ShowUIInfo("正在连接...");
            //return;
        }
        else{
            HideUIInfo();
        }

        if(showUIInfo){
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 30, 300, 60), uiInfo);
        }
        
        if(showStartServerButton && Network.peerType == NetworkPeerType.Disconnected) {
            if(GUI.Button(new Rect(Screen.width / 2 - 140, Screen.height / 2 + 80, 280, 60), "创建游戏")) {
                Network.InitializeServer(15, serverPort, useNat);
                MasterServer.RegisterHost(gameName, "企鹅大战", "等待玩家");
            }
        }

        //PlatformUtils.ScaleGUI();

        //windowRect = GUILayout.Window(0, windowRect, MakeWindow, "Server Controls");

        //if(Network.peerType == NetworkPeerType.Disconnected && MasterServer.PollHostList().Length != 0)
        //    serverListRect = GUILayout.Window(1, serverListRect, MakeClientWindow, "Server List");
    }

    void ShowUIInfo(string info){
        uiInfo = info;
        showUIInfo = true;
    }

    void HideUIInfo(){
        showUIInfo = false;
    }

    private void MakeWindow(int id) {
        hideTest = GUILayout.Toggle(hideTest, "Hide test info");

        if(!hideTest) {
            GUILayout.Label(testMessage);
            if(GUILayout.Button("Retest connection")) {
                Debug.Log("Redoing connection test");
                probingPublicIP = false;
                doneTesting = false;
                connectionTestResult = Network.TestConnection(true);
            }
        }

        if(Network.peerType == NetworkPeerType.Disconnected) {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            // Start a new server
            if(GUILayout.Button("创建房间")) {
                Network.InitializeServer(15, serverPort, useNat);
                MasterServer.RegisterHost(gameName, "企鹅大战", "等待玩家");
            }

            // Refresh hosts
            if(GUILayout.Button("刷新房间列表") || Time.realtimeSinceStartup > lastHostListRequest + hostListRefreshTimeout) {
                MasterServer.RequestHostList(gameName);
                lastHostListRequest = Time.realtimeSinceStartup;
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }
        else {
            if(GUILayout.Button("Disconnect")) {
                Network.Disconnect();
                MasterServer.UnregisterHost();
            }
            string msg = Network.connections.Length + " players connected.";
            GUILayout.Label(msg);
            GUILayout.FlexibleSpace();

            if(Network.connections.Length == 1) {
                if(GUILayout.Button("开始游戏")) {
                    //GameState.Instance().StartMultiplayerGame();
                }
            }
        }
        GUI.DragWindow(new Rect(0, 0, 1000, 1000));
    }

    void MakeClientWindow(int id) {
        GUILayout.Space(5);

        HostData[] data = MasterServer.PollHostList();
        foreach(HostData element in data) {
            GUILayout.BeginHorizontal();

            // Do not display NAT enabled games if we cannot do NAT punchthrough
            if(!(filterNATHosts && element.useNat)) {
                var connections = element.connectedPlayers + "/" + element.playerLimit;
                GUILayout.Label(element.gameName);
                GUILayout.Space(5);
                GUILayout.Label(connections);
                GUILayout.Space(5);
                var hostInfo = "";

                // Indicate if NAT punchthrough will be performed, omit showing GUID
                if(element.useNat) {
                    GUILayout.Label("NAT");
                    GUILayout.Space(5);
                }
                // Here we display all IP addresses, there can be multiple in cases where
                // internal LAN connections are being attempted. In the GUI we could just display
                // the first one in order not confuse the end user, but internally Unity will
                // do a connection check on all IP addresses in the element.ip list, and connect to the
                // first valid one.
                foreach(string host in element.ip)
                    hostInfo = hostInfo + host + ":" + element.port + " ";

                //GUILayout.Label("[" + element.ip + ":" + element.port + "]");	
                GUILayout.Label(hostInfo);
                GUILayout.Space(5);
                GUILayout.Label(element.comment);
                GUILayout.Space(5);
                GUILayout.FlexibleSpace();
                if(GUILayout.Button("Connect")) {
                    NetworkConnectionError err = Network.Connect(element);
                    Debug.Log(err.ToString());
                }
            }
            GUILayout.EndHorizontal();
        }
        GUI.DragWindow(new Rect(0, 300, 640, 640));
    }

    void TestConnection() {        
        // Start/Poll the connection test, report the results in a label and react to the results accordingly
        connectionTestResult = Network.TestConnection();
        switch(connectionTestResult) {
            case ConnectionTesterStatus.Error:
                testMessage = "Problem determining NAT capabilities";
                doneTesting = true;
                break;

            case ConnectionTesterStatus.Undetermined:
                testMessage = "Undetermined NAT capabilities";
                doneTesting = false;
                break;

            case ConnectionTesterStatus.PublicIPIsConnectable:
                testMessage = "Directly connectable public IP address.";
                useNat = false;
                doneTesting = true;
                break;

            // This case is a bit special as we now need to check if we can 
            // circumvent the blocking by using NAT punchthrough
            case ConnectionTesterStatus.PublicIPPortBlocked:
                testMessage = "Non-connectble public IP address (port " + serverPort + " blocked), running a server is impossible.";
                useNat = false;
                // If no NAT punchthrough test has been performed on this public IP, force a test
                if(!probingPublicIP) {
                    Debug.Log("Testing if firewall can be circumvented");
                    connectionTestResult = Network.TestConnectionNAT();
                    probingPublicIP = true;
                    timer = Time.time + 10;
                }
                // NAT punchthrough test was performed but we still get blocked
                else if(Time.time > timer) {
                    probingPublicIP = false; 		// reset
                    useNat = true;
                    doneTesting = true;
                }
                break;
            case ConnectionTesterStatus.PublicIPNoServerStarted:
                testMessage = "Public IP address but server not initialized, it must be started to check server accessibility. Restart connection test when ready.";
                break;

            case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
                Debug.Log("LimitedNATPunchthroughPortRestricted");
                testMessage = "Limited NAT punchthrough capabilities. Cannot connect to all types of NAT servers.";
                useNat = true;
                doneTesting = true;
                break;

            case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
                Debug.Log("LimitedNATPunchthroughSymmetric");
                testMessage = "Limited NAT punchthrough capabilities. Cannot connect to all types of NAT servers. Running a server is ill adviced as not everyone can connect.";
                useNat = true;
                doneTesting = true;
                break;

            case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
            case ConnectionTesterStatus.NATpunchthroughFullCone:
                Debug.Log("NATpunchthroughAddressRestrictedCone || NATpunchthroughFullCone");
                testMessage = "NAT punchthrough capable. Can connect to all servers and receive connections from all clients. Enabling NAT punchthrough functionality.";
                useNat = true;
                doneTesting = true;
                break;

            default:
                testMessage = "Error in test routine, got " + connectionTestResult;
                break;
        }
        //Debug.Log(connectionTestResult + " " + probingPublicIP + " " + doneTesting);
    }

}
