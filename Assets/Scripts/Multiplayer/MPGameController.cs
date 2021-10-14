using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.SceneManagement;


/// <summary>
/// Handles UI actions on CustomGame scene, which sets up the units and config for a battle
/// </summary>
/// <remarks>
/// handles the scene CustomGame and is the pre-game for multiplayer
/// handles the menu button clicks (start, exit, options, changing level, etc) and the team scroll lists (ie what units each team will have in the upcoming battle)
/// works with MPDraftInfo script for draft info behavior
/// creates an object called pmo (PlayerManagerObject) that persists from this scene to combat launching, one of the scripts attached to that is PlayerManager which has the unit info for each team
/// green is team one (sometimes orange), red is team 2 (sometimes purple)
/// bunch of old code in here for when online used to work
/// </remarks>


public class MPGameController : MonoBehaviour
{
    //menu buttons
    public Button startButton;
    public Button randomDraftButton;
    public Button timedPickButton;

    //team scroll lists
    public Text greenText;
    public GameObject greenTeamScrollListPanel;
    public Button greenTeamAddButton;
    public Button greenTeamRemoveButton;
    CustomGameTeamScrollList greenListComponent;
    public Text redText;
    public GameObject redTeamScrollListPanel;
    public Button redTeamAddButton;
    public Button redTeamRemoveButton;
    CustomGameTeamScrollList redListComponent;

    //displays pu information
    PlayerUnit pu = null;
    Dictionary<int, PlayerUnit> unitDict; //for loading pu and populating UnitScrollList
    [SerializeField]
    private UIUnitInfoPanel statPanel;

    //handles game mode: free pick, timed pick, and random draft. draftInfoPanel launches on the last two
    [SerializeField]
    private GameObject draftInfoPanel;
    MPDraftInfo dpScript; // = draftInfoPanel.GetComponent<MPDraftInfo>();
    bool isDraft = false;
    string draftType = "";

    //optionsPanel and options related constatns
    public GameObject OptionsPanel;
    public Transform contentPanel;
    public GameObject sampleButton;
    int victoryType;
    int aiType;
    int pickType;

    //loading level
    Dictionary<int, LevelData> levelDict;
    LevelData currentLevel;
    int team2SpawnPoints = 0;
    int team3SpawnPoints = 0;

    //multiplayer things
    //PhotonView photonView;
    bool isReady; //= false;
    bool isOpponentReady; //= false;
    bool mpDraftReady; //= false;
    bool mpOpponentDraftReady;// = false;
    GameObject pmo;
    bool isPMLoaded; //= false;
    int modVersion;
    int draftInt;
    [SerializeField]
    GameObject playersConnected; //text that tells how many users are active in the room
    int connectedPlayersTotal;

    #region monobehaviour
    //variables used in MP to make sure client and host are ready before proceeding to various steps (starting draft, starting game, etc)
    void Awake()
    {
        isReady = false; 
        isOpponentReady = false;
        mpDraftReady = false;
        mpOpponentDraftReady = false;
        isPMLoaded = false; //used to tell client that pmo is loaded
        connectedPlayersTotal = 1;
    }

    void Start()
    {
        //load the levels
        levelDict = CalcCode.LoadLevelDict(false, 0); //gets the aurelian levels
        //photonView = PhotonView.Get(this);
        dpScript = draftInfoPanel.GetComponent<MPDraftInfo>();

        //is MP game or not
        if (PlayerPrefs.GetInt(NameAll.PP_CUSTOM_GAME_TYPE, NameAll.CUSTOM_GAME_OFFLINE) == NameAll.CUSTOM_GAME_ONLINE)
        {
            //PhotonNetwork.offlineMode = false; Debug.Log("offline mode is " + PhotonNetwork.offlineMode);
            currentLevel = SetCurrentLevel(0); //defaults to first map

            //if ( PhotonNetwork.room.customProperties != null)
            //{
            //    ExitGames.Client.Photon.Hashtable roomHashtable = PhotonNetwork.room.customProperties;

            //    if(roomHashtable.ContainsKey(RoomProperty.Draft))
            //        draftInt = (int)roomHashtable[RoomProperty.Draft];
            //    else
            //    {
            //        Debug.Log("ERROR: No Custom room properties for draft not found");
            //        draftInt = NameAll.DRAFT_TYPE_TIMED_PICK;
            //    }
                    

            //    if (roomHashtable.ContainsKey(RoomProperty.Version))
            //        modVersion = (int)roomHashtable[RoomProperty.Version];
            //    else
            //    {
            //        Debug.Log("ERROR: No Custom room properties for version not found");
            //        modVersion = NameAll.VERSION_AURELIAN;
            //    }
                    
            //}
            //else
            //{
            //    Debug.Log("ERROR: No Custom room properties found");
            //    modVersion = NameAll.VERSION_AURELIAN;
            //    draftInt = NameAll.DRAFT_TYPE_TIMED_PICK;
            //}
            SetUIForMP();

            //set up the pmo, which is the object that holds data for the next scene
            //if (PhotonNetwork.isMasterClient) //client pmo set up in OnFixedUpdate()
            //{
            //    Vector3 vec = new Vector3(); Debug.Log("ONLINE mode");
            //    pmo = PhotonNetwork.InstantiateSceneObject("PlayerManagerObject", vec, Quaternion.identity, 0, null) as GameObject;
            //    DontDestroyOnLoad(pmo);
            //    isPMLoaded = true;
            //    PlayerManager.Instance.SetMPOnline(PhotonNetwork.isMasterClient); //let's PlayerManager know its an online game
            //    redTeamAddButton.gameObject.SetActive(false);
            //    redTeamRemoveButton.gameObject.SetActive(false);
            //}
            //else
            //{
            //    greenTeamAddButton.gameObject.SetActive(false);
            //    greenTeamRemoveButton.gameObject.SetActive(false);
            //}

            startButton.GetComponentInChildren<Text>().text = "Start (0/2)";
            playersConnected.SetActive(true);
        }
        else
        {
            //PhotonNetwork.offlineMode = true; //Debug.Log("offline mode is " + PhotonNetwork.offlineMode);

            //get custom levels and set the current default level
            Dictionary<int, LevelData> tempDict = CalcCode.GetCustomLevelDict(); //gets the custom levels
            foreach (KeyValuePair<int, LevelData> kvp in tempDict)
            {
                levelDict.Add(kvp.Key + 1000, kvp.Value); //so when map is launched know what type of level
            }
            int tempLevel = PlayerPrefs.GetInt(NameAll.PP_COMBAT_LEVEL, 0);
            currentLevel = SetCurrentLevel(tempLevel);

            modVersion = PlayerPrefs.GetInt(NameAll.PP_MOD_VERSION, NameAll.VERSION_AURELIAN);
            draftInt = PlayerPrefs.GetInt(NameAll.PP_MP_OPTIONS_DRAFT, NameAll.DRAFT_TYPE_FREE_PICK);

            //loading options for game and draft type
            victoryType = PlayerPrefs.GetInt(NameAll.PP_VICTORY_TYPE, NameAll.VICTORY_TYPE_DEFEAT_PARTY);
            aiType = PlayerPrefs.GetInt(NameAll.PP_AI_TYPE, NameAll.AI_TYPE_HUMAN_VS_AI);
            pickType = NameAll.DRAFT_TYPE_FREE_PICK;
            

            //set up the pmo
            pmo = Instantiate(Resources.Load("PlayerManagerObject")) as GameObject; //Debug.Log("OFFLINE mode");
            startButton.GetComponentInChildren<Text>().text = "Start"; //Debug.Log("asdf " + team2SpawnPoints);
            isPMLoaded = true; //used in some other parts
        }

        //setspawn points
        team2SpawnPoints = currentLevel.GetTeamSpawnPointsCount(NameAll.TEAM_ID_GREEN);
        team3SpawnPoints = currentLevel.GetTeamSpawnPointsCount(NameAll.TEAM_ID_RED);
        greenListComponent = greenTeamScrollListPanel.GetComponent<CustomGameTeamScrollList>();
        redListComponent = redTeamScrollListPanel.GetComponent<CustomGameTeamScrollList>(); 
        startButton.gameObject.SetActive(false); //start button disabled until game is ready to go
        SetHeaderText(NameAll.TEAM_ID_GREEN);
        SetHeaderText(NameAll.TEAM_ID_RED);

		//populates the unit scroll list
		//unitDict = CalcCode.LoadPlayerUnitDict(modVersion,false,0,PhotonNetwork.offlineMode);
		unitDict = CalcCode.LoadPlayerUnitDict(modVersion, false, 0, true);
		PopulateUnitScrollList();
        if( unitDict.Count > 0)
        {
            foreach(KeyValuePair<int,PlayerUnit> kvp in unitDict)
            {
                pu = kvp.Value;
                break;
            }
        }
        //show pu info
        SetStatPanel();
    }

    void FixedUpdate()
    {
        //populate names from Player 2
        //if (!PhotonNetwork.offlineMode)
        //{
        //    if( !isPMLoaded && !PhotonNetwork.isMasterClient ) //clones the pmo object
        //    {
        //        HashSet<GameObject> temp = PhotonNetwork.FindGameObjectsWithComponent(typeof(PlayerManager)); //Debug.Log("hash set size is " + temp.Count);
        //        List<GameObject> hList = temp.ToList();
        //        if( hList.Count > 0)
        //        {
        //            pmo = hList[0];
        //            DontDestroyOnLoad(pmo);
        //            isPMLoaded = true;
        //            PlayerManager.Instance.SetMPOnline(PhotonNetwork.isMasterClient); //let's PlayerManager know its an online game
        //        }
        //        return;
        //    }

        //    //not ideal. don't have the timing down for updating the headers on client (player 2)
        //    //SetHeaderText(NameAll.TEAM_ID_GREEN); 
        //    //SetHeaderText(NameAll.TEAM_ID_RED);

        //    if (connectedPlayersTotal != PhotonNetwork.playerList.Length)
        //    {
        //        connectedPlayersTotal = PhotonNetwork.playerList.Length;

        //        if (PhotonNetwork.playerList.Length == 2)
        //        {
        //            playersConnected.GetComponentInChildren<Text>().text = "2/2 Players Connected";
        //        }
        //        else
        //        {
        //            playersConnected.GetComponentInChildren<Text>().text = "1/2 Players Connected";
        //        }
        //    }
            
        //}
    }

    #endregion

    #region team specific scroll lists
    //sets the text at the top of the header (how many units selected/how many available to select) ie 5/5
    //[PunRPC]
    void SetHeaderText(int teamId)
    {
        if( isPMLoaded)
        {
            List<PlayerUnit> tempList = PlayerManager.Instance.GetTeamList(teamId);
            if (teamId == NameAll.TEAM_ID_GREEN)
            {
                greenText.text = "Team 1:\n" + tempList.Count + "/" + team2SpawnPoints;
            }
            else
            {
                redText.text = "Team 2:\n" + tempList.Count + "/" + team3SpawnPoints;
            }
        }
        
    }

    //adds and removes units from the team lists, updates the scroll lists and headers
    public void EditTeamList(PlayerUnit puTemp, int teamId, int type)
    {
        if( type == NameAll.TEAM_LIST_ADD)
        {
            if(puTemp != null)
            {
                PlayerManager.Instance.EditTeamLists(puTemp, teamId,type);
            }
        }
        else if( type == NameAll.TEAM_LIST_REMOVE)
        {
            PlayerManager.Instance.EditTeamLists(puTemp, teamId, type);
        }
        PopulateNames(teamId);
        SetHeaderText(teamId);

        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("SetHeaderText", PhotonTargets.Others, new object[] { teamId});
        //}
    }

    //[PunRPC]
    void PopulateNames(int teamId)
    {
        //if( !PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient )
        //{
        //    photonView.RPC("PopulateNames", PhotonTargets.Others, new object[] { teamId });
        //}

        if (teamId == NameAll.TEAM_ID_GREEN)
        {
            greenListComponent.PopulateNames(PlayerManager.Instance.GetTeamList(teamId));
        }
        else if (teamId == NameAll.TEAM_ID_RED)
        {
            redListComponent.PopulateNames(PlayerManager.Instance.GetTeamList(teamId));
        }
    }

    //add unit to red list
    //offline: just takes the current pu and adds to list
    //online: tells the master client that a unit has been added, master adds it and tells client
    //either does it with the currently selected pu by client or if a puCode is entered, the puCode is passed to the masterclient
    //[PunRPC]
    public void AddRedUnitAndPopulateList(string puCode = "" )
    {
        //if (!PhotonNetwork.offlineMode && !PhotonNetwork.isMasterClient)
        //{
        //    //Debug.Log("testing " + draftType.Equals("timedPick"));
        //    redTeamAddButton.gameObject.SetActive(false);
        //    if ( draftType.Equals("timedPick")) //only called from button press or from forcedPick, either way get the info from the pu
        //    {
        //        string puCode2 = CalcCode.BuildStringFromPlayerUnit(pu);// Debug.Log("in this part of code " + puCode2);
        //        photonView.RPC("AddRedUnitAndPopulateList", PhotonTargets.MasterClient, new object[] { puCode2 });
        //    }
        //    else
        //    {
        //        photonView.RPC("AddRedUnitAndPopulateList", PhotonTargets.MasterClient, new object[] { puCode });
        //    }
        //    SetStartButtonActive();
        //    return;
        //}

        if (PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_RED).Count < team3SpawnPoints)
        {
            //Debug.Log("testing puCode" + puCode);
            if( !puCode.Equals(""))
            {
                PlayerUnit puTemp = CalcCode.BuildPlayerUnit(puCode); //Debug.Log("testing puCode 2" + puCode);
                EditTeamList(puTemp, NameAll.TEAM_ID_RED, NameAll.TEAM_LIST_ADD);
            }
            else
            {
                EditTeamList(pu, NameAll.TEAM_ID_RED, NameAll.TEAM_LIST_ADD); //Debug.Log("asdf 2");
            }
   
            if (isDraft)
            {
                dpScript.DecrementPick(NameAll.TEAM_ID_RED); //Debug.Log("asdf 3");
                if (draftType.Equals("timedPick"))
                {
                    redTeamAddButton.gameObject.SetActive(false); //Debug.Log("asdf 4");
                    dpScript.IncrementDraftPhase();
                }
            }
        }

        SetStartButtonActive();
    }

    //called in adding and removing team lists. For adding to red, both call this locally. For adding to green, MasterClient tells other
    //[PunRPC]
    void SetStartButtonActive()
    {
        if(true)//if(PhotonNetwork.offlineMode)
        {
            if (PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_GREEN).Count > 0 || PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_RED).Count > 0)
            {
                startButton.gameObject.SetActive(true); //can start whenever I guess. Can wait til end of draft using check below
            }
            else
            {
                startButton.gameObject.SetActive(false);
            }
        }
        else
        {
            if (PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_GREEN).Count > 0 && PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_RED).Count > 0)
            {
                startButton.gameObject.SetActive(true); //can start whenever I guess. Can wait til end of draft using check below
            }
            else
            {
                startButton.gameObject.SetActive(false);
            }
        }
    }

    //called in RemoveFromRedListAndPopulate; client tells host to editteamlist
    //[PunRPC]
    public void RemoveFromList( int teamId, int type)
    {
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    EditTeamList(null, teamId, type);
        //}
    }

    //add a unit to the green (player 1/host/masterClient) list
    public void AddGreenUnitAndPopulateList()
    {
        if (PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_GREEN).Count < team2SpawnPoints)
        {
            EditTeamList(pu, NameAll.TEAM_ID_GREEN, NameAll.TEAM_LIST_ADD);
            if (isDraft)
            {
                dpScript.DecrementPick(NameAll.TEAM_ID_GREEN);
                if( draftType.Equals("timedPick"))
                {
                    greenTeamAddButton.gameObject.SetActive(false);
                    dpScript.IncrementDraftPhase();
                }
                
            }
        }

        SetStartButtonActive();

        //tells non master client to check to see if start button should be activated
        //if (!PhotonNetwork.offlineMode)
        //{
        //    photonView.RPC("SetStartButtonActive", PhotonTargets.Others, new object[] { });
        //}
    }

    //clear the team lists in PlayerManager, remove the scroll lists, reset the headers
    void ClearPlayerLists()
    {
		//Debug.Log(PhotonNetwork.offlineMode + "asdf" + PhotonNetwork.isMasterClient);
		if (true)//if (PhotonNetwork.offlineMode)
		{
            PlayerManager.Instance.ClearTeamLists();
            PopulateNames(NameAll.TEAM_ID_GREEN);
            PopulateNames(NameAll.TEAM_ID_RED);
            SetHeaderText(NameAll.TEAM_ID_GREEN);
            SetHeaderText(NameAll.TEAM_ID_RED);
        }
        else
        {
            //if (PhotonNetwork.isMasterClient)
            //{
            //    PlayerManager.Instance.ClearTeamLists();
            //    PopulateNames(NameAll.TEAM_ID_GREEN);
            //    PopulateNames(NameAll.TEAM_ID_RED);
            //    SetHeaderText(NameAll.TEAM_ID_GREEN);
            //    SetHeaderText(NameAll.TEAM_ID_RED);
            //    //ClearPlayerLists called on both sides so shoudln't need to send out an RPC
            //    //photonView.RPC("SetHeaderText", PhotonTargets.Others, new object[] { NameAll.TEAM_ID_GREEN });
            //    //photonView.RPC("SetHeaderText", PhotonTargets.Others, new object[] { NameAll.TEAM_ID_RED });
            //}
            //else
            //{
            //    SetHeaderText(NameAll.TEAM_ID_GREEN);
            //    SetHeaderText(NameAll.TEAM_ID_RED);
            //}
        }
        startButton.gameObject.SetActive(false);
    }

    //remove unit from red team
    public void RemoveRedUnitAndPopulateList()
    {
		EditTeamList(pu, NameAll.TEAM_ID_RED, NameAll.TEAM_LIST_REMOVE);

		//if (!PhotonNetwork.offlineMode && !PhotonNetwork.isMasterClient)
		//{
		//    photonView.RPC("RemoveFromList", PhotonTargets.MasterClient, new object[] { NameAll.TEAM_ID_RED, NameAll.TEAM_LIST_REMOVE });      
		//}
		//else
		//{
		//    EditTeamList(pu, NameAll.TEAM_ID_RED, NameAll.TEAM_LIST_REMOVE);
		//}
		SetStartButtonActive();
    }

    //remove a unit from the green team
    public void RemoveGreenUnitAndPopulateList()
    {
        EditTeamList(pu, NameAll.TEAM_ID_GREEN, NameAll.TEAM_LIST_REMOVE);

        //if (!PhotonNetwork.offlineMode)
        //{
        //    //only called from MasterClient, tells other to check to see if start button should still be active
        //    photonView.RPC("SetStartButtonActive", PhotonTargets.Others, new object[] { });
        //}

        SetStartButtonActive();
    }
    #endregion

    #region menu buttons
    public void OnStartButtonClick()
    {
		if (true)//if( PhotonNetwork.offlineMode)
		{
            PlayerPrefs.SetInt(NameAll.PP_COMBAT_ENTRY, NameAll.SCENE_CUSTOM_GAME);
            DontDestroyOnLoad(pmo);
            SceneManager.LoadScene(NameAll.SCENE_COMBAT);
        }
        else
        {
            SetGameReadyStatus(!isReady);
        }
        
    }

    public void OnOptionsButtonClick()
    {
        if( OptionsPanel.activeSelf)
        {
            OptionsPanel.gameObject.SetActive(false);
        }
        else
        {
            OptionsPanel.gameObject.SetActive(true);
            PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);
        }
    }

    public void ExitScene()
    {
        //Debug.Log("asdf");
        GetComponent<DialogController>().Show("Quit Game", "Are you sure you want to quit?", ConfirmExit, null);
    }

    void ConfirmExit()
    {
		//Debug.Log("confiming exit");
		if (false)//if(!PhotonNetwork.offlineMode)
		{
            //GameObject go = GameObject.Find("ChatGameObject");
            //if (go != null)
            //{
            //    go.SetActive(false);
            //    //Debug.Log("why the fuck can't I leave the fucking chat channel?");
            //    //go.GetComponent<ChatHandler>().LeaveRoomChannels();
            //    //go.GetComponent<ChatHandler>().JoinLobbyDecoud();
            //}
            ////else
            //    //Debug.Log("ERROR: couldn't find chat game object before leaving scene");

            if (pmo != null)
                Destroy(pmo);

            //PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(NameAll.SCENE_MP_MENU);
        }
        else
        {
            int z1 = PlayerPrefs.GetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
            PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, 0); //for using the character edit button, this needs to start fresh every time
            SceneManager.LoadScene(z1); 
        }
        
    }
    #endregion

    #region multiplayer

    //update ready status and let opponent know. game starts when both ready
    void SetGameReadyStatus(bool readyStatus)
    {
        isReady = readyStatus;
        SetStartButtonReadyText();
        //photonView.RPC("SendGameReadyStatusToOpponent", PhotonTargets.Others, new object[] { readyStatus });

        //if (isReady && isOpponentReady)
        //{
        //    PhotonNetwork.LoadLevel(NameAll.SCENE_COMBAT);
        //}
    }

    //[PunRPC]
    void SendGameReadyStatusToOpponent(bool readyStatus)
    {
        isOpponentReady = readyStatus;
        SetStartButtonReadyText();
        //if (isReady && isOpponentReady)
        //{
        //    PhotonNetwork.LoadLevel(NameAll.SCENE_COMBAT);
        //}
    }

    void SetStartButtonReadyText()
    {
        if (isReady && isOpponentReady)
            startButton.GetComponentInChildren<Text>().text = "Start (2/2)";
        else if( !isReady && !isOpponentReady)
            startButton.GetComponentInChildren<Text>().text = "Start (0/2)";
        else
            startButton.GetComponentInChildren<Text>().text = "Start (1/2)";
    }

    //update draft ready status and let opponent know. draft starts when both ready
    void SetDraftReadyStatus(bool readyStatus)
    {
        mpDraftReady = readyStatus;
        SetDraftButtonText();
        //photonView.RPC("SendDraftReadyStatusToOpponent", PhotonTargets.Others, new object[] { readyStatus });

        if (mpDraftReady && mpOpponentDraftReady) //starts draft when both ready
        {
            if (!isDraft)
            {
                isDraft = true;
                if (draftInt == NameAll.DRAFT_TYPE_TIMED_PICK)
                    StartMPTimedPick();
                else
                    StartMPRandomDraft();
            }
        }
    }

    //[PunRPC]
    void SendDraftReadyStatusToOpponent(bool readyStatus)
    {
        mpOpponentDraftReady = readyStatus;
        SetDraftButtonText();
        if (mpDraftReady && mpOpponentDraftReady) //starts draft when both ready
        {
            if (!isDraft)
            {
                isDraft = true;
                if (draftInt == NameAll.DRAFT_TYPE_TIMED_PICK)
                    StartMPTimedPick();
                else
                    StartMPRandomDraft();
            }
        }
    }

    void SetDraftButtonText()
    {
        if (randomDraftButton.IsActive())
        {
            if (mpDraftReady && mpOpponentDraftReady)
                randomDraftButton.GetComponentInChildren<Text>().text = "Start Draft (2/2)";
            else if (!mpDraftReady && !mpOpponentDraftReady)
                randomDraftButton.GetComponentInChildren<Text>().text = "Start Draft (0/2)";
            else
                randomDraftButton.GetComponentInChildren<Text>().text = "Start Draft (1/2)";
        }
        else if (timedPickButton.IsActive())
        {
            if (timedPickButton.IsActive())
            {
                if (mpDraftReady && mpOpponentDraftReady)
                    timedPickButton.GetComponentInChildren<Text>().text = "Start Picks (2/2)";
                else if (!mpDraftReady && !mpOpponentDraftReady)
                    timedPickButton.GetComponentInChildren<Text>().text = "Start Picks (0/2)";
                else
                    timedPickButton.GetComponentInChildren<Text>().text = "Start Picks (1/2)";
            }
        }
    }


    void SetUIForMP()
    {
        //Debug.Log("setting UI for MP " + draftInt);
        //hide draft buttons
        if (draftInt == NameAll.DRAFT_TYPE_TIMED_PICK)
        {
            randomDraftButton.gameObject.SetActive(false); //Debug.Log("setting UI for MP " + draftInt);
            timedPickButton.GetComponentInChildren<Text>().text = "Start Picks (0/2)";
        }
        else
        {
            timedPickButton.gameObject.SetActive(false); //Debug.Log("setting UI for MP " + draftInt);
            randomDraftButton.GetComponentInChildren<Text>().text = "Start Draft (0/2)";
        }

        DisableTeamButtons(false);
    }
    #endregion

    #region misc functions (on X changed, updates)

    //sets the currentlevel
    LevelData SetCurrentLevel(int dictKey)
    {
        try
        {
            if (dictKey >= 1000)
            {
                PlayerPrefs.SetString(NameAll.PP_LEVEL_DIRECTORY, NameAll.LEVEL_DIRECTORY_CUSTOM);
            }
            else
            {
                PlayerPrefs.SetString(NameAll.PP_LEVEL_DIRECTORY, NameAll.LEVEL_DIRECTORY_AURELIAN);
            }
            var testVar = levelDict[dictKey]; //just testing it before setting the PP
            PlayerPrefs.SetInt(NameAll.PP_COMBAT_LEVEL, dictKey);
            return levelDict[dictKey];
        }
        catch (Exception e) //returns to default level select
        {
            PlayerPrefs.SetInt(NameAll.PP_COMBAT_LEVEL, 0);
            PlayerPrefs.SetString(NameAll.PP_LEVEL_DIRECTORY, NameAll.LEVEL_DIRECTORY_AURELIAN);
            return levelDict[0];
        }
    }

    public void SetStatPanel()
    {
        statPanel.Open(true);
        statPanel.PopulatePlayerInfo(pu, false);
    }

    //called in OptionsScrollList when version is potentially changed
    void CheckVersionChanged()
    {
        int z1 = PlayerPrefs.GetInt(NameAll.PP_MOD_VERSION, NameAll.VERSION_AURELIAN);
        if (z1 != modVersion)
        {
            modVersion = z1;
            startButton.gameObject.SetActive(false);

            ClearPlayerLists();

            unitDict = CalcCode.LoadPlayerUnitDict(modVersion, false, 0);
            PopulateUnitScrollList();
            if (unitDict.Count > 0)
            {
                foreach (KeyValuePair<int, PlayerUnit> kvp in unitDict)
                {
                    pu = kvp.Value;
                    break;
                }
            }
            SetStatPanel();
        }
    }
    
    //user changes the level. in online, both users can change between default levels but only if neither player indicates readiness (or draft readiness)
    //[PunRPC]
    void OnLevelChanged(int levelKey, bool isTellOpponent = false)
    {
        //if (!PhotonNetwork.offlineMode && isTellOpponent)
        //{
        //    photonView.RPC("OnLevelChanged", PhotonTargets.Others, new object[] { levelKey, false });
        //}
        currentLevel = SetCurrentLevel(levelKey); //changes the PlayerPrefs

        team2SpawnPoints = currentLevel.GetTeamSpawnPointsCount(NameAll.TEAM_ID_GREEN);
        team3SpawnPoints = currentLevel.GetTeamSpawnPointsCount(NameAll.TEAM_ID_RED);
        ClearPlayerLists(); //Debug.Log("level changed");
    }
    #endregion

    #region draft related functions and draft popup related functions
    //called from menu by user when clicking button
    public void OpenTimedPick()
    {
		if (true)//if(PhotonNetwork.offlineMode)
		{
            isDraft = !isDraft;
            if (isDraft)
            {
                DisableTeamButtons(false);
                draftInfoPanel.SetActive(true);
                ClearPlayerLists();
                dpScript.Populate("timedPick", team2SpawnPoints, team3SpawnPoints, modVersion);
                draftType = "timedPick";
            }
            else
            {
                draftInfoPanel.SetActive(false);
                ClearPlayerLists();
                DisableTeamButtons(true);
                draftType = "";
                dpScript.DisableRandomDraftObject();
            }
        }
        else
        {
            //changes draft ready status to opposite and notifies opponent
            SetDraftReadyStatus(!mpDraftReady);
        }
        
    }

    //called from menu by user when clicking button
    public void OpenRandomDraft()
    {
		if (true)//if( PhotonNetwork.offlineMode)
		{
            isDraft = !isDraft;
            if (isDraft)
            {
                draftType = "randomPick";
                //need to disable the player dropdown and unit info panel
                ToggleRandomDraftMode(true);
                DisableTeamButtons(false);
                draftInfoPanel.SetActive(true);
                ClearPlayerLists();
                dpScript.Populate("randomDraft", team2SpawnPoints, team3SpawnPoints, modVersion);
            }
            else
            {
                draftType = "";
                //need to re-enable toe player dropdown and unit info panel
                ToggleRandomDraftMode(false);
                draftInfoPanel.SetActive(false);
                dpScript.DisableRandomDraftObject();
                ClearPlayerLists();
                DisableTeamButtons(true);
            }
        }
        else
        {
            //changes draft ready status to opposite and notifies opponent
            SetDraftReadyStatus(!mpDraftReady);
        }
    }

    
    //called when both players indicate draft readiness
    void StartMPRandomDraft()
    {
        draftType = "randomPick";
        ToggleRandomDraftMode(true);
        DisableTeamButtons(false);
        draftInfoPanel.SetActive(true);
        ClearPlayerLists();

        //if (!PhotonNetwork.offlineMode)
        //{
        //    randomDraftButton.GetComponentInChildren<Text>().text = "Draft (2/2)";
        //    if (PhotonNetwork.isMasterClient)
        //    {
        //        dpScript.Populate("randomDraft", team2SpawnPoints, team3SpawnPoints, modVersion, UnityEngine.Random.Range(2, 4));
        //    }
        //}
        //else
        //{
        //    randomDraftButton.GetComponentInChildren<Text>().text = "Draft";
        //    dpScript.Populate("randomDraft", team2SpawnPoints, team3SpawnPoints, modVersion, UnityEngine.Random.Range(2, 4));
        //}
    }

    //called when both players indicate draft readiness
    void StartMPTimedPick()
    {
        
        DisableTeamButtons(false);
        draftInfoPanel.SetActive(true);
        ClearPlayerLists();
        draftType = "timedPick";

        //if( !PhotonNetwork.offlineMode )
        //{
        //    timedPickButton.GetComponentInChildren<Text>().text = "Picks (2/2)";
        //    if (PhotonNetwork.isMasterClient)
        //    {
        //        dpScript.Populate("timedPick", team2SpawnPoints, team3SpawnPoints, modVersion, UnityEngine.Random.Range(2, 4));
        //    }   
        //}
        //else
        //{
        //    timedPickButton.GetComponentInChildren<Text>().text = "Picks";
        //    dpScript.Populate("timedPick", team2SpawnPoints, team3SpawnPoints, modVersion, UnityEngine.Random.Range(2, 4));
        //}
    }

    public void CloseDraft()
    {
        draftInfoPanel.SetActive(false);
    }

    public void DisableTeamButtons(bool zBool = false)
    { 
        greenTeamAddButton.gameObject.SetActive(zBool);
        greenTeamRemoveButton.gameObject.SetActive(zBool);
        redTeamAddButton.gameObject.SetActive(zBool);
        redTeamRemoveButton.gameObject.SetActive(zBool);
    }

    public void ForceTimedDraftPick(int teamId)
    {
        if( teamId == NameAll.TEAM_ID_GREEN)
        {
            AddGreenUnitAndPopulateList();
        }
        else
        {
            //if(!PhotonNetwork.offlineMode)
            //{
            //    //gets P2 to populate with it's default, which then gets the puCode back for p1
            //    Debug.Log("forcign p2 to do a pick ");
            //    photonView.RPC("AddRedUnitAndPopulateList",PhotonTargets.Others,new object[] { "" });
            //    //AddRedUnitAndPopulateList(CalcCode.BuildStringFromPlayerUnit(pu));
            //}
            //else
            //{
            //    AddRedUnitAndPopulateList();
            //}
        }
    }

    public void ForceRandomDraftPick(int teamId, PlayerUnit puRD)
    {
        pu = puRD;
        if (teamId == NameAll.TEAM_ID_GREEN)
        {
            AddGreenUnitAndPopulateList();
        }
        else
        {
            //if (!PhotonNetwork.offlineMode)
            //{
            //    AddRedUnitAndPopulateList(CalcCode.BuildStringFromPlayerUnit(puRD));
            //}
            //else
            //{
            //    AddRedUnitAndPopulateList();
            //}
        }
    }

    public void ToggleRandomDraftMode(bool zBool)
    {
        if(zBool)
        {
            //popupSelect.gameObject.SetActive(false);
            pu = null;
            statPanel.gameObject.SetActive(false);
        }
        else
        {
            //SetStatPanel(); //think it's called elsewhere
        }
    }

    public void SetPlayerUnit(PlayerUnit puRD)
    {
        pu = puRD;
        SetStatPanel();
    }

    
    #endregion

    #region Optionspanel

    static readonly int OPTIONS_LIST_DEFAULT = 0;
    static readonly int OPTIONS_SELECT_VC = 10;
    static readonly int OPTIONS_SELECT_LEVEL = 11;
    static readonly int OPTIONS_SELECT_AI = 12;
    static readonly int OPTIONS_SELECT_PICK_TYPE = 13;
    static readonly int OPTIONS_SELECT_VERSION = 14;
    static readonly int OPTIONS_SELECT_MP_MESSAGE = 15;

    int currentType = OPTIONS_LIST_DEFAULT;
    int currentSubType = OPTIONS_LIST_DEFAULT;
    

    void PopulateOptionsScrollList( int type)
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildOptionsList(type);

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonOptionsClicked(tempInt));
        }
    }

    List<AbilityBuilderObject> BuildOptionsList(int type)
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;
        currentType = type;

        if ( type == OPTIONS_LIST_DEFAULT)
        {
            abo = new AbilityBuilderObject("Change Level", "Level: " + currentLevel.levelName, OPTIONS_SELECT_LEVEL);
            retValue.Add(abo);
			if (true)//if (PhotonNetwork.offlineMode)
			{
                abo = new AbilityBuilderObject("Change Version", CalcCode.GetVersionFromInt(modVersion), OPTIONS_SELECT_VERSION);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Change Victory Conditions", CalcCode.GetVCString(victoryType), OPTIONS_SELECT_VC);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Change Pick Type", CalcCode.GetPickType(pickType), OPTIONS_SELECT_PICK_TYPE);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Change Multiplayer/AI", CalcCode.GetAIType(aiType), OPTIONS_SELECT_AI);
                retValue.Add(abo);
            }
            else
            {
                if (isReady || mpDraftReady || isOpponentReady || mpOpponentDraftReady)
                {
                    abo = new AbilityBuilderObject("Cannot Change Level When Ready", "Cannot change until you and opponent are not ready", OPTIONS_SELECT_MP_MESSAGE);
                    retValue.Add(abo);
                    return retValue;
                }
            }
            
        }
        else if( type == OPTIONS_SELECT_LEVEL)
        {
            foreach(KeyValuePair<int,LevelData> kvp in levelDict)
            {
                abo = new AbilityBuilderObject("Level: " + kvp.Value.levelName, "", kvp.Key);
                retValue.Add(abo);
            }
        }
        else if( type == OPTIONS_SELECT_VERSION)
        {
            
            abo = new AbilityBuilderObject("Version: Aurelian", "", NameAll.VERSION_AURELIAN);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Version: Classic", "", NameAll.VERSION_CLASSIC);
            retValue.Add(abo);
        }
        else if (type == OPTIONS_SELECT_PICK_TYPE)
        {
            abo = new AbilityBuilderObject(CalcCode.GetPickType(NameAll.DRAFT_TYPE_FREE_PICK), "", NameAll.DRAFT_TYPE_FREE_PICK);
            retValue.Add(abo);
            abo = new AbilityBuilderObject(CalcCode.GetPickType(NameAll.DRAFT_TYPE_TIMED_PICK), "Players alternate picks.", NameAll.DRAFT_TYPE_TIMED_PICK);
            retValue.Add(abo);
            abo = new AbilityBuilderObject(CalcCode.GetPickType(NameAll.DRAFT_TYPE_RANDOM_DRAFT), "Players alternate picks from a random creation of units.", NameAll.DRAFT_TYPE_RANDOM_DRAFT);
            retValue.Add(abo);
        }
        else if( type == OPTIONS_SELECT_VC)
        {
            abo = new AbilityBuilderObject("Victory Conditions: " + CalcCode.GetVCString(NameAll.VICTORY_TYPE_DEFEAT_PARTY), "", NameAll.VICTORY_TYPE_DEFEAT_PARTY);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Victory Conditions: " + CalcCode.GetVCString(NameAll.VICTORY_TYPE_NONE), "Game ends when user quits", NameAll.VICTORY_TYPE_NONE);
            retValue.Add(abo);
        }
        else if( type == OPTIONS_SELECT_AI)
        {
            abo = new AbilityBuilderObject(CalcCode.GetAIType(NameAll.AI_TYPE_HUMAN_VS_HUMAN), "", NameAll.AI_TYPE_HUMAN_VS_HUMAN);
            retValue.Add(abo);
            abo = new AbilityBuilderObject(CalcCode.GetAIType(NameAll.AI_TYPE_HUMAN_VS_AI), "", NameAll.AI_TYPE_HUMAN_VS_AI);
            retValue.Add(abo);
            abo = new AbilityBuilderObject(CalcCode.GetAIType(NameAll.AI_TYPE_AI_VS_AI), "", NameAll.AI_TYPE_AI_VS_AI);
            retValue.Add(abo);
        }

        return retValue;
    }

    void ButtonOptionsClicked(int select)
    {
		if (true)//if(PhotonNetwork.offlineMode)
		{
            if (currentType == OPTIONS_LIST_DEFAULT)
            {
                PopulateOptionsScrollList(select);
            }
            else if (currentType == OPTIONS_SELECT_LEVEL)
            {
                OnLevelChanged(select, true);
                PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);
            }
            else if (currentType == OPTIONS_SELECT_AI)
            {
                aiType = select;
                PlayerPrefs.SetInt(NameAll.PP_AI_TYPE, select);
                PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);
            }
            else if (currentType == OPTIONS_SELECT_VC)
            {
                victoryType = select;
                PlayerPrefs.SetInt(NameAll.PP_VICTORY_TYPE, select);
                PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);
            }
            else if (currentType == OPTIONS_SELECT_PICK_TYPE)
            {
                pickType = select;
                PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);
            }
            else if (currentType == OPTIONS_SELECT_VERSION)
            {
                //modVersion = select; //DON"T DO THIS HERE DO THIS IN CHECKVERSION CHANGED
                PlayerPrefs.SetInt(NameAll.PP_MOD_VERSION, select);
                CheckVersionChanged();
                PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);

            }
        }
        else
        {
            //can't change level or do anything if you already ready'd up
            if( currentType == OPTIONS_SELECT_MP_MESSAGE || isReady || mpDraftReady || isOpponentReady || mpOpponentDraftReady )
            {
                OnOptionsButtonClick(); //close the options list
                return;
            }

            if (currentType == OPTIONS_LIST_DEFAULT)
            {
                PopulateOptionsScrollList(select);
            }
            else if (currentType == OPTIONS_SELECT_LEVEL)
            {
                OnLevelChanged(select, true);
                PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);
            }
        }
        
    }
    #endregion

    #region UnitScrollList
    public GameObject UnitScrollListPanel;
    public Transform unitListContentPanel;
    public Text unitListText;

    void PopulateUnitScrollList()
    {
        foreach (Transform child in unitListContentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildUnitList();
        //Debug.Log("trying to populate scroll list" + aboList.Count);
        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(unitListContentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonUnitClicked(tempInt));
        }
    }

    List<AbilityBuilderObject> BuildUnitList()
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;

        //Debug.Log("unit dict size is " + unitDict.Count);
        foreach (KeyValuePair<int, PlayerUnit> kvp in unitDict)
        {
            abo = new AbilityBuilderObject(CalcCode.GetTitleFromPlayerUnit(kvp.Value), CalcCode.GetDetailsFromPlayerUnit(kvp.Value), kvp.Key);
            retValue.Add(abo);
        }

        if( unitDict.Count == 0)
        {
            pu = new PlayerUnit(modVersion);
            unitDict.Add(0, pu);
        }

        return retValue;
    }

    void ButtonUnitClicked(int select)
    {
        pu = unitDict[select];
        SetStatPanel();
    }

    
    #endregion

   
}
