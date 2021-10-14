using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// DEPRECATED OLD WAY OF DOING MULTIPLAYER
/// </summary>
//controls the MP "lobby" scene. connects to network, allows player to find game, create game or chat
//to do
//add a JoinOrCreateRoom function for private rooms, tutorial page about matchmaking has more details
public class MPMenuController : MonoBehaviour //: Photon.PunBehaviour
{
    //[SerializeField]
    //GameObject mpMenuPanel;
    [SerializeField]
    private Dropdown dropdownVersion;
    [SerializeField]
    private Dropdown dropdownDraft;
    [SerializeField]
    private InputField inputName;
    [SerializeField]
    private InputField inputPrivateGame;

    public Button joinButton;
    public Button privateGameButton;
    bool inGame = false;
    public Text statusText;
    string privateGameName;

    //fucking photon chat
    //public ChatHandler ch;
    //public ChatGUI chatGUI;

    void Awake()
    {
        SetUpDropdowns();
        //PhotonNetwork.logLevel = NetworkLogLevel.Full;

        //Connect to the main photon server. This is the only IP and port we ever need to set(!)
        //if (!PhotonNetwork.connected)
        //{
        //    PhotonNetwork.ConnectUsingSettings("1"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)
        //}
            

        ////Load name from PlayerPrefs
        //PhotonNetwork.playerName = PlayerPrefs.GetString(NameAll.PP_MP_OPTIONS_NAME, "Guest" + Random.Range(1, 9999));

        statusText.text = "Status: Joining Lobby";
    }

    // Use this for initialization
    void Start () {

        joinButton.gameObject.SetActive(false);
        privateGameButton.gameObject.SetActive(false);
    }

    void SetUpDropdowns()
    {
        
        dropdownVersion.options.Clear();
        List<string> myList = new List<string>();
        myList.Add("Aurelian");
        myList.Add("Classic");
        //myList.Add("Either");
        dropdownVersion.AddOptions(myList);

        dropdownDraft.options.Clear();
        myList = new List<string>();
        //myList.Add("Any");
        //myList.Add("Free Pick");
        myList.Add("Timed Pick");
        myList.Add("Random Draft");
        dropdownDraft.AddOptions(myList);

        dropdownDraft.onValueChanged.AddListener(delegate {
            MyDropdownDraftChangedHandler(dropdownDraft);
        });
        dropdownVersion.onValueChanged.AddListener(delegate {
            MyDropdownVersionChangedHandler(dropdownVersion);
        });

        int z1 = PlayerPrefs.GetInt(NameAll.PP_MP_OPTIONS_VERSION, NameAll.VERSION_AURELIAN);
        if (z1 == NameAll.VERSION_AURELIAN)
        {
            z1 = 0;
        }
        else if( z1 == NameAll.VERSION_CLASSIC)
        {
            z1 = 1;

        }
        else
        {
            z1 = 0;
        }
        dropdownVersion.value = z1;

        z1 = PlayerPrefs.GetInt(NameAll.PP_MP_OPTIONS_DRAFT, NameAll.DRAFT_TYPE_TIMED_PICK);
        if (z1 == NameAll.DRAFT_TYPE_RANDOM_DRAFT)
        {
            z1 = 1;
        }
        else if (z1 == NameAll.DRAFT_TYPE_TIMED_PICK)
        {
            z1 = 0;
        }
        else
        {
            z1 = 0;
        }
        dropdownDraft.value = z1;

        InputField.SubmitEvent submitNameEvent = new InputField.SubmitEvent();
        submitNameEvent.AddListener(OnSubmitName);
        inputName.onEndEdit = submitNameEvent;
        inputName.text = PlayerPrefs.GetString(NameAll.PP_MP_OPTIONS_NAME, "Guest");

        privateGameName = PlayerPrefs.GetString(NameAll.PP_MP_OPTIONS_PRIVATE_GAME, "room " + Random.Range(0, 100000));
        InputField.SubmitEvent submitPrivateGameEvent = new InputField.SubmitEvent();
        submitPrivateGameEvent.AddListener(OnSubmitPrivateGame);
        inputPrivateGame.onEndEdit = submitPrivateGameEvent;
        inputPrivateGame.text = privateGameName;
    }

    private void MyDropdownVersionChangedHandler(Dropdown target)
    {
        //Debug.Log("selected: " + target.value);
        int z1 = NameAll.VERSION_AURELIAN;
        if (target.value == 1)
            z1 = NameAll.VERSION_CLASSIC;

        PlayerPrefs.SetInt(NameAll.PP_MP_OPTIONS_VERSION, z1);
        //Debug.Log("wtf the version is " + PlayerPrefs.GetInt(NameAll.PP_MP_OPTIONS_VERSION, target.value));
    }

    private void MyDropdownDraftChangedHandler(Dropdown target)
    {
        
        int z1 = NameAll.DRAFT_TYPE_TIMED_PICK;
        if (target.value == 1)
            z1 = NameAll.DRAFT_TYPE_RANDOM_DRAFT;
        //Debug.Log("selected: " + target.value + " " + z1);
        PlayerPrefs.SetInt(NameAll.PP_MP_OPTIONS_DRAFT, z1);
    }

    void OnSubmitName(string name)
    {
        string zString = name;
        if( zString.Length > 10)
        {
            zString.Substring(0, 10);
        }
        inputName.text = zString;
        PlayerPrefs.SetString(NameAll.PP_MP_OPTIONS_NAME, zString);
        //ch.ChatUsername = zString;
    }

    void OnSubmitPrivateGame(string name)
    {
        string zString = name;
        if (zString.Length > 10)
        {
            zString.Substring(0, 20);
        }
        privateGameName = zString;
        inputPrivateGame.text = privateGameName;
        PlayerPrefs.SetString(NameAll.PP_MP_OPTIONS_PRIVATE_GAME, privateGameName);
    }

    // Update is called once per frame
    //void Update () {
    //    //if (!PhotonNetwork.connected)
    //    //{
    //    //    Debug.Log("not connected");
    //    //    return;   //Wait for a connection
    //    //}
    //    //Debug.Log("connected");
    //}

    //public override void OnJoinedLobby()
    //{
    //    //Debug.Log("Lobby joined");

    
    //    PhotonNetwork.automaticallySyncScene = true;
    //    joinButton.gameObject.SetActive(true);
    //    joinButton.GetComponentInChildren<Text>().text = "Join Game";
    //    privateGameButton.gameObject.SetActive(true);
    //    privateGameButton.GetComponentInChildren<Text>().text = "Private Game";
    //    statusText.text = "Status: Lobby Joined";
    //    //PhotonNetwork.JoinRandomRoom();
    //}

    public void ClickToJoin()
    {
        //based on settings look for a game ot join, for now just find a random one
        //Debug.Log("clicked to join random room");
        joinButton.gameObject.SetActive(false);
        privateGameButton.gameObject.SetActive(false);
        inGame = !inGame;
        if(inGame)
        {       
            statusText.text = "Status: Joining Game";

            //ExitGames.Client.Photon.Hashtable expectedProperties = new ExitGames.Client.Photon.Hashtable();
            //int version = PlayerPrefs.GetInt(NameAll.PP_MP_OPTIONS_VERSION,1);
            //int draft = PlayerPrefs.GetInt(NameAll.PP_MP_OPTIONS_DRAFT, 1);
            //expectedProperties.Add(RoomProperty.Draft, draft);
            //expectedProperties.Add(RoomProperty.Version, version);
            //expectedProperties.Add("curScn", "MultiplayerMenu");
            //ExitGames.Client.Photon.Hashtable tempHash = new ExitGames.Client.Photon.Hashtable();
            //tempHash = CreateRoomProperties();
            //Debug.Log(" expected properties are " + tempHash.ToStringFull() );
            //byte zByte = (byte)2;
            //PhotonNetwork.JoinRandomRoom(tempHash,2); //if join fails, random room created
            
            //PhotonNetwork.JoinRandomRoom(,)
            //PhotonNetwork.JoinOrCreateRoom(name, roomOptions);
            
        }
        else
        {
            statusText.text = "Status: Leaving Game";
            //ch.LeaveRoomChannels();
            //PhotonNetwork.LeaveRoom(); //leave the room
        }
        
    }

    public void OnClickPrivateGame()
    {
        //Debug.Log("clicked to join private game");
        joinButton.gameObject.SetActive(false);
        privateGameButton.gameObject.SetActive(false);
        inGame = !inGame;
        if (inGame)
        {
            statusText.text = "Status: Joining Private Game";

            //Debug.Log(" creating private game with  " + CreateRoomProperties().ToStringFull());

            //RoomOptions roomOptions = new RoomOptions();
            //roomOptions.MaxPlayers = 2;
            //roomOptions.IsVisible = false;
            //roomOptions.CustomRoomProperties = CreateRoomProperties();
            //roomOptions.CustomRoomPropertiesForLobby = CreateRoomPropertiesForLobby();
            //PhotonNetwork.JoinOrCreateRoom(privateGameName, roomOptions, TypedLobby.Default);
     
        }
        else //already had clicked to join a game, now leaving the room
        {
            statusText.text = "Status: Leaving Game";
            //ch.LeaveRoomChannels();
            //PhotonNetwork.LeaveRoom(); //leave the room
        }
    }

    public void OnPhotonRandomJoinFailed()
    {
        //PhotonNetwork.JoinRandomRoom(); return;
        //RoomOptions roomOptions = new RoomOptions(); //roomOptions.customRoomProperties.Add()
        //roomOptions.MaxPlayers = 2;
        //roomOptions.customRoomProperties = new ExitGames.Client.Photon.Hashtable();
        //int version = PlayerPrefs.GetInt(NameAll.PP_MP_OPTIONS_VERSION, 1);
        //int draft = PlayerPrefs.GetInt(NameAll.PP_MP_OPTIONS_DRAFT, 1);
        //roomOptions.customRoomProperties.Add(RoomProperty.Version, version);
        //roomOptions.customRoomProperties.Add(RoomProperty.Draft, draft);
        //roomOptions.CustomRoomProperties = CreateRoomProperties();
        //roomOptions.CustomRoomPropertiesForLobby = CreateRoomPropertiesForLobby();


        //Debug.Log(" customRoom properties are " + roomOptions.CustomRoomProperties.ToStringFull() );
        //PhotonNetwork.CreateRoom(null, roomOptions, null);
    }

    //public override void OnJoinedRoom()
    //{
    //    //Debug.Log("joined a room, name is " + PhotonNetwork.room.GetHashCode());
    //    //Debug.Log("joined a room, name is " + PhotonNetwork.room.ToStringFull());
    //    //ch.JoinRoomDecoud();
    //    //chatGUI.hideChatInfo = true;

    //    joinButton.gameObject.SetActive(true);
    //    joinButton.GetComponentInChildren<Text>().text = "Leave Game";

    //    statusText.text = "Status: Game Joined. 1/2 Players.";
    //    // game logic: if this is the only player, we're "it"
        
    //    //Debug.Log("random room joined "); //PhotonNetwork.room.maxPlayers = 8;
    //    //Debug.Log("random room joined " + PhotonNetwork.room.maxPlayers );
        
    //    if (PhotonNetwork.playerList.Length == 2) //testing
    //    {
    //        PhotonNetwork.room.open = false; //can't joint room anymore
    //        PhotonNetwork.room.visible = false;
    //        statusText.text = "Status: Game Joined. 2/2 Players.";
    //        //Debug.Log("two players in room, time to launch next scene?");
    //        PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_TYPE, NameAll.CUSTOM_GAME_ONLINE); //lets it know that it's multiplayer
    //        PhotonNetwork.LoadLevel(NameAll.SCENE_CUSTOM_GAME);
    //    }
        
    //}

    //public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    //{
    //    //Debug.Log("another player joined");
    //    if (PhotonNetwork.playerList.Length == 2) //testing
    //    {
    //        PhotonNetwork.room.open = false; //can't joint room anymore
    //        PhotonNetwork.room.visible = false;
    //        statusText.text = "Status: Game Joined. 2/2 Players.";
    //        //Debug.Log("second player joined, about to join scene");
    //        PhotonNetwork.LoadLevel(NameAll.SCENE_CUSTOM_GAME);
    //    }
    //}

    public void OnQuitClick()
    {
        //ch.Connect()
        //ch.DisconnectDecoud();
        GameObject go = GameObject.Find("ChatGameObject");
        if (go != null)
        {
            //Destroy(go);//can't destroy due to bug between photon and unity
            go.SetActive(false);
        }
        //else
        //    Debug.Log("ERROR: couldn't find chat game object before leaving scene");
        //PhotonNetwork.Disconnect();

        SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
    }

    //ExitGames.Client.Photon.Hashtable CreateRoomProperties()
    //{
    //    return new ExitGames.Client.Photon.Hashtable
    //    {
    //        {RoomProperty.Version, PlayerPrefs.GetInt(NameAll.PP_MP_OPTIONS_VERSION, NameAll.VERSION_AURELIAN)},
    //        {RoomProperty.Draft, PlayerPrefs.GetInt(NameAll.PP_MP_OPTIONS_DRAFT, NameAll.DRAFT_TYPE_TIMED_PICK)}
    //    };

    //    //roomOptions.customRoomProperties = new ExitGames.Client.Photon.Hashtable();
    //    //int version = PlayerPrefs.GetInt(NameAll.PP_MP_OPTIONS_VERSION, 1);
    //    //int draft = PlayerPrefs.GetInt(NameAll.PP_MP_OPTIONS_DRAFT, 1);
    //    //roomOptions.customRoomProperties.Add(RoomProperty.Version, version);
    //    //roomOptions.customRoomProperties.Add(RoomProperty.Draft, draft);
    //}

    string[] CreateRoomPropertiesForLobby()
    {
        return new string[]
        {
            RoomProperty.Version,RoomProperty.Draft
        };
    }

    //void OnGUI()
    //{
    //    if (!PhotonNetwork.connected)
    //    {
    //        ShowConnectingGUI();
    //        return;   //Wait for a connection
    //    }

    //}

    //void ShowConnectingGUI()
    //{
    //    GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

    //    GUILayout.Label("Connecting to Photon server.");
    //    GUILayout.Label("Hint: This demo uses a settings file and logs the server address to the console.");

    //    GUILayout.EndArea();
    //}
}
