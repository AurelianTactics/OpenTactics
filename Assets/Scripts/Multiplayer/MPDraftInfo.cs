using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles logic and UI interactions when drafting units for teams
/// </summary>
/// <remarks>
/// still need to implement: in incrementdraft, add the logic that tells whose turn (red/green) based on the pickOrder, first pick and number of spawn points and what part of the draft (pick/ban)
/// 
/// </remarks>

public class MPDraftInfo : MonoBehaviour {

    public Text title;
    public Text timerLabel;
    public Button teamColor;
    public Text teamTurn;
    public Button startButton;
    public Text startLabel;

    public GameObject greenConfirm;
    public GameObject redConfirm;

    [SerializeField]
    MPGameController mpGameScript;

    string draftType = "";
    int firstPick;
    int pickOrder;
    bool isPicking;// = false;
    int lastPick;
    int greenPicks;// = 0;
    int redPicks;// = 0;
    int pickingTeam;

    private float time;
    private float timedPickTime = 10.0f;
    private float randomDraftTimeStart = 20.0f;//60.0f;
    private float randomDraftTime = 10.0f;

    //random draft
    [SerializeField]
    CustomGameTeamScrollList banListObject;
    [SerializeField]
    CustomGameTeamScrollList pickListObject;
    List<PlayerUnit> puAvailableList;
    List<PlayerUnit> puBanList;
    PlayerUnit pu;

    //PhotonView photonView;

    void Awake()
    {
        //photonView = PhotonView.Get(this);
        isPicking = false;
    }

    void Update()
    {
        if (true)//(PhotonNetwork.offlineMode || PhotonNetwork.isMasterClient)
        {

            if (isPicking)
            {
                if (time > 0)
                {
                    CountdownTime(); //P2 gets the update from a PUNRPC every second
                }
                else
                {
                    //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
                    //{
                    //    photonView.RPC("UpdateTime", PhotonTargets.Others, new object[] { 0.0f });
                    //}
                    time = timedPickTime; //for random draft reset in randomdraft time
                                          //isPicking = false;
                    if (draftType.Equals("timedPick"))
                    {
                        ForcePick(pickingTeam, ""); //increments draft phase in there
                    }
                    else if (draftType.Equals("randomDraft"))
                    {
                        Debug.Log("reached end of time in update");
                        if (((pickOrder > 4 && pickOrder < 9) || (pickOrder > 12)))
                        {
                            //not a ban, force a pick
                            if (puAvailableList.Count > 0)
                            {
                                ForceRandomDraftPick(pickingTeam);
                                //PlayerUnit puTemp = puAvailableList[UnityEngine.Random.Range(0, puAvailableList.Count)];
                                //puAvailableList.Remove(puTemp);
                                //ForcePick(pickingTeam);
                            }
                        }
                        IncrementDraftPhase();

                    }
                }
            }
        }
            
    }

    void CountdownTime()
    {
        float startTime = time;
        time -= Time.deltaTime;
        if (Mathf.Ceil(startTime) - Mathf.Ceil(time) > 0)
        {
            //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
            //{
            //    photonView.RPC("UpdateTime", PhotonTargets.Others, new object[] { time });
            //}
        }
        var minutes = time / 60; //Divide the guiTime by sixty to get the minutes.
        var seconds = time % 60;//Use the euclidean division for the seconds.
                                //var fraction = (time * 100) % 100;
                                //update the label value
                                //timerLabel.text = string.Format("{0:00} : {1:00} : {2:000}", minutes, seconds, fraction);
        timerLabel.text = string.Format("{0:00} : {1:00}", minutes, seconds);//, fraction);
    }

    //[PunRPC]
    void UpdateTime(float zFloat)
    {
        var minutes = zFloat / 60; //Divide the guiTime by sixty to get the minutes.
        var seconds = zFloat % 60;//Use the euclidean division for the seconds.
                                //var fraction = (time * 100) % 100;
                                //update the label value
                                //timerLabel.text = string.Format("{0:00} : {1:00} : {2:000}", minutes, seconds, fraction);
        timerLabel.text = string.Format("{0:00} : {1:00}", minutes, seconds);//, fraction);
    }

    public void IncrementDraftPhase()
    {
        //time = timedPickTime;
        pickOrder += 1; //Debug.Log("pick order is " + pickOrder);
        if (draftType.Equals("randomDraft"))
        {
            
            time = randomDraftTime;
            //pickListObject.PopulatePickBanNames(puAvailableList, pickOrder,lastPick);
            //0f,0,0f,0, 1f,1,1,1f, 0f,0,0f,0, 1,1f,1f,1 then all picks
            if ( pickOrder <= 1 || pickOrder == 3 || pickOrder == 9 || pickOrder == 11)
            {
                if( firstPick == NameAll.TEAM_ID_GREEN)
                {
                    SetTeamTurn(NameAll.TEAM_ID_GREEN,0);
                    pickingTeam = NameAll.TEAM_ID_GREEN;
                }
                else
                {
                    SetTeamTurn(NameAll.TEAM_ID_RED, 0);
                    pickingTeam = NameAll.TEAM_ID_RED;
                }
            }
            else if( pickOrder == 2 || pickOrder == 4 || pickOrder == 10 || pickOrder == 12)
            {
                if (firstPick == NameAll.TEAM_ID_RED)
                {
                    SetTeamTurn(NameAll.TEAM_ID_GREEN, 0);
                    pickingTeam = NameAll.TEAM_ID_GREEN;
                }
                else
                {
                    SetTeamTurn(NameAll.TEAM_ID_RED, 0);
                    pickingTeam = NameAll.TEAM_ID_RED;
                }
            }
            else if (pickOrder == 5 || pickOrder == 8)
            {
                if (firstPick == NameAll.TEAM_ID_GREEN)
                {
                    CheckIfSlot(NameAll.TEAM_ID_GREEN);
                    //SetTeamTurn(NameAll.TEAM_ID_GREEN, 1);
                }
                else
                {
                    CheckIfSlot(NameAll.TEAM_ID_RED);
                    //SetTeamTurn(NameAll.TEAM_ID_RED, 1);
                }
            }
            else if (pickOrder == 6 || pickOrder == 7)
            {
                
                if (firstPick == NameAll.TEAM_ID_RED)
                {
                    CheckIfSlot(NameAll.TEAM_ID_GREEN);
                    //SetTeamTurn(NameAll.TEAM_ID_GREEN, 0); Debug.Log("fp red, pickOrder is " + pickOrder);
                }
                else
                {
                    CheckIfSlot(NameAll.TEAM_ID_RED);
                    //SetTeamTurn(NameAll.TEAM_ID_RED, 0); Debug.Log("fp green, pickOrder is " + pickOrder);
                }
            }
            else
            {
                int z1 = pickOrder % 4; //first pick goes 2 and 3; starts at pick 13
                if ( (firstPick == NameAll.TEAM_ID_GREEN && (z1 == 2 || z1 == 3))
                    || (firstPick == NameAll.TEAM_ID_RED && (z1 == 0 || z1 == 1) ) )
                {
                    CheckIfSlot(NameAll.TEAM_ID_GREEN);
                    //SetTeamTurn(NameAll.TEAM_ID_GREEN, 1);
                }
                else
                {
                    CheckIfSlot(NameAll.TEAM_ID_RED);
                    //SetTeamTurn(NameAll.TEAM_ID_RED, 1);
                }
            }
            PopulatePicksList(pickOrder, lastPick, pickingTeam);
        }
        else if (draftType.Equals("timedPick"))
        {
            time = timedPickTime;
            int z1 = pickOrder % 4;
            if( firstPick == NameAll.TEAM_ID_GREEN )
            {
                if( z1 == 0 || z1 == 1)
                {
                    CheckIfSlot(NameAll.TEAM_ID_GREEN);
                    //SetTeamTurn(NameAll.TEAM_ID_GREEN, 1);
                }
                else
                {
                    Debug.Log("checking for red turn " + z1);
                    CheckIfSlot(NameAll.TEAM_ID_RED);
                    //SetTeamTurn(NameAll.TEAM_ID_RED, 1);
                }
            }
            else
            {
                if (z1 == 0 || z1 == 1)
                {
                    CheckIfSlot(NameAll.TEAM_ID_RED);
                    //SetTeamTurn(NameAll.TEAM_ID_RED, 1);
                }
                else
                {
                    CheckIfSlot(NameAll.TEAM_ID_GREEN);
                    //SetTeamTurn(NameAll.TEAM_ID_GREEN, 1);
                }
            }
        }
        //Debug.Log("pick order is " + pickOrder + " picking team is " + pickingTeam);
        //isPicking = true;
    }

    //[PunRPC]
    public void Populate(string type, int greenSpawnPoints, int redSpawnPoints, int modVersion, int zFirstPick = 2)
    {
        //zFirstPick = 2; //testing
        //if( !PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient )
        //{
        //    photonView.RPC("Populate", PhotonTargets.Others, new object[] { type, greenSpawnPoints,redSpawnPoints,modVersion,zFirstPick});
        //}
        firstPick = zFirstPick; //Debug.Log("first pick is " + firstPick); //startButton.gameObject.SetActive(true);
        pickingTeam = firstPick;
        draftType = type;
        pickOrder = 0;
        time = randomDraftTimeStart;
        lastPick = greenSpawnPoints + redSpawnPoints;
        greenPicks = greenSpawnPoints;
        redPicks = redSpawnPoints;
        if(true)//( PhotonNetwork.offlineMode)
        {
            startLabel.text = "Pause Draft";
        }
        else
        {
            startButton.gameObject.SetActive(false);
        }

        if (type.Equals("timedPick"))
        {
            title.text = "Timed Pick"; //Debug.Log("in here");
            if( firstPick == NameAll.TEAM_ID_GREEN)
            {
                SetTeamTurn(NameAll.TEAM_ID_GREEN, 1);
            }
            else
            {
                SetTeamTurn(NameAll.TEAM_ID_RED, 1);
            }
            time = timedPickTime;
            isPicking = true;
            IncrementDraftPhase();
        }
        else if(type.Equals("randomDraft"))
        {
            lastPick += 8;
            teamColor.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_neutral");
            InitializePickBanLists();
            if(true)//if( PhotonNetwork.offlineMode || PhotonNetwork.isMasterClient)
            {
                GenerateRandomPU(greenSpawnPoints + redSpawnPoints, modVersion);
            }
            title.text = "Random Draft";
            //teamTurn.text = "Pre-Draft Phase";
            if (firstPick == NameAll.TEAM_ID_GREEN)
            {
                //SetTeamTurn(NameAll.TEAM_ID_GREEN, 0);
                teamTurn.text = "Pre-Draft Phase (Team 1 First Pick)";
                //teamColor.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_team_1");
            }
            else
            {
                //SetTeamTurn(NameAll.TEAM_ID_RED, 0);
                teamTurn.text = "Pre-Draft Phase (Team 2 First Pick)";
                //teamColor.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_team_2");
            }
            time = randomDraftTimeStart;
            isPicking = true;
        }
    }

    public void StartDraft()
    {
        
        isPicking = !isPicking; //Debug.Log("draft started?" + isPicking);
        if (isPicking)
        {
            //startButton.GetComponent<Text>().text = "Pause Draft";
            startLabel.text = "Pause Draft";
        }
        else
        {
            //startButton.GetComponent<Text>().text = "Start Draft";
            startLabel.text = "Resume Draft";
        }
    }

    //[PunRPC]
    void SetTeamTurn(int team, int pick)
    {
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("SetTeamTurn", PhotonTargets.Others, new object[] { team, pick});
        //}
        if ( team == NameAll.TEAM_ID_GREEN )
        {
            if( pick == 1)
            {
                teamTurn.text = "Team 1 Pick";
            }
            else
            {
                teamTurn.text = "Team 1 Ban";
            }
            teamColor.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_team_1");
        }
        else if( team == NameAll.TEAM_ID_RED)
        {
            if (pick == 1)
            {
                teamTurn.text = "Team 2 Pick";
            }
            else
            {
                teamTurn.text = "Team 2 Ban";
            }
            teamColor.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_team_2");
        }
        else
        {
            teamTurn.text = "Draft Over";
            teamColor.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_neutral");
        }
    }

    void CheckIfSlot(int teamId)
    {
        if( greenPicks == 0 && redPicks == 0)
        {
            isPicking = false;
            SetTeamTurn(NameAll.NULL_INT, NameAll.NULL_INT); //draft is over message
            return;
        }

        if( teamId == NameAll.TEAM_ID_GREEN && greenPicks > 0)
        {
            pickingTeam = NameAll.TEAM_ID_GREEN;
            EnablePicks(teamId);
            SetTeamTurn(NameAll.TEAM_ID_GREEN, 1);
        }
        else if( teamId == NameAll.TEAM_ID_RED && redPicks > 0)
        {
            pickingTeam = NameAll.TEAM_ID_RED; //Debug.Log("red about to pick, about to enable red");
            EnablePicks(teamId);
            SetTeamTurn(NameAll.TEAM_ID_RED, 1);
        }
        else if (teamId == NameAll.TEAM_ID_RED && redPicks == 0)
        {
            CheckIfSlot(NameAll.TEAM_ID_GREEN);
        }
        else if (teamId == NameAll.TEAM_ID_GREEN && greenPicks == 0)
        {
            CheckIfSlot(NameAll.TEAM_ID_RED);
        }
    }

    //[PunRPC]
    void EnablePicks(int teamId)
    {
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient && teamId == NameAll.TEAM_ID_RED)
        //{
        //    Debug.Log("sending enablepicks rpc to P2, " + teamId);
        //    photonView.RPC("EnablePicks", PhotonTargets.Others, new object[] { teamId });
        //}

        if(true)//(PhotonNetwork.offlineMode)
        {
            if (draftType.Equals("timedPick"))
            {
                if (teamId == NameAll.TEAM_ID_GREEN)
                {
                    greenConfirm.SetActive(true);
                    redConfirm.SetActive(false);
                }
                else
                {
                    greenConfirm.SetActive(false);
                    redConfirm.SetActive(true);
                }
            }
        }
        else
        {
            //Debug.Log("received call to enable picks " + teamId);
            //if (draftType.Equals("timedPick"))
            //{
            //    //Debug.Log("received call to enable picks " + teamId);
            //    if (teamId == NameAll.TEAM_ID_GREEN && PhotonNetwork.isMasterClient)
            //    {
            //        greenConfirm.SetActive(true);
            //        //redConfirm.SetActive(false);
            //    }
            //    else if( teamId == NameAll.TEAM_ID_RED && !PhotonNetwork.isMasterClient)
            //    {
            //        //Debug.Log("received call to enable picks " + teamId);
            //        //greenConfirm.SetActive(false);
            //        redConfirm.SetActive(true);
            //    }
            //}
        }
        
    }

    //[PunRPC]
    public void DecrementPick( int teamId)
    {
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("DecrementPick", PhotonTargets.Others, new object[] { teamId });
        //}
        if ( teamId == NameAll.TEAM_ID_GREEN)
        {
            greenPicks -= 1;
            if( greenPicks < 0)
            {
                greenPicks = 0;
            }
        }
        else
        {
            redPicks -= 1;
            if (redPicks < 0)
            {
                redPicks = 0;
            }
        }
    }

    //[PunRPC]
    public void ForcePick(int zPickingTeam, string puCode)
    {
        //if master client pick, flows to mc shit and populates list then updates red
        //if P2 pick
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient && zPickingTeam == NameAll.TEAM_ID_RED)
        //{ 
        //    photonView.RPC("ForcePick", PhotonTargets.Others, new object[] { zPickingTeam, puCode });
        //    return;
        //}

        if (true)//(PhotonNetwork.offlineMode)
		{
            if (draftType.Equals("timedPick"))
            {
                mpGameScript.ForceTimedDraftPick(zPickingTeam);
                //IncrementDraftPhase(); //incremented after list is populated
            }
            else if (draftType.Equals("randomDraft"))
            {
                PlayerUnit puTemp = CalcCode.BuildPlayerUnit(puCode);
                mpGameScript.ForceRandomDraftPick(zPickingTeam, puTemp);
            }
        }
        else
        {
            if (draftType.Equals("timedPick"))
            {
                mpGameScript.ForceTimedDraftPick(zPickingTeam);
                //IncrementDraftPhase();
            }
            else if (draftType.Equals("randomDraft"))
            {
                //generate a random number, get the pu and force a pick
                //if (puTemp == null)
                //{
                //    puTemp = puAvailableList[UnityEngine.Random.Range(0, puAvailableList.Count)];
                //    puAvailableList.Remove(puTemp);
                //}
                PlayerUnit puTemp = CalcCode.BuildPlayerUnit(puCode);
                mpGameScript.ForceRandomDraftPick(zPickingTeam, puTemp);
            }
        }
        
    }

    void ForceRandomDraftPick( int zPickingTeam)
    {
        int z1 = UnityEngine.Random.Range(0, puAvailableList.Count);
        PlayerUnit puTemp = puAvailableList[z1];
        RemoveRandomDraftPlayerUnit(z1);
        //puAvailableList.Remove(puTemp);
        ForcePick(zPickingTeam, CalcCode.BuildStringFromPlayerUnit(puTemp));
    }

    //[PunRPC] 
    public void RemoveRandomDraftPlayerUnit(int index, bool sendRPC = true)
    {
        //if (!PhotonNetwork.offlineMode)
        //{
        //    if (sendRPC)
        //    {
        //        photonView.RPC("RemoveRandomDraftPlayerUnit", PhotonTargets.Others, new object[] { index, false });
        //    }

        //}
        puAvailableList.RemoveAt(index);
    }

    //only done in master then adds the pu's to p2
    void GenerateRandomPU(int spawnPoints, int modVersion)
    {
        AbilityManager.Instance.SetIoType(NameAll.ITEM_MANAGER_SIMPLE);
        ItemManager.Instance.SetIoType(NameAll.ITEM_MANAGER_SIMPLE);
        int totalUnits = spawnPoints + 8 + 4; //8 bans, 4 extra //Debug.Log("TESTING PU CODE ADDING 100 ADDITONAL UNITS"); totalUnits += 1000;
        while( totalUnits > 0)
        {
            //real code
            totalUnits -= 1;
            PlayerUnit puTemp = new PlayerUnit(modVersion, true, false);
            puAvailableList.Add(puTemp);
            //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
            //{
            //    string puCode = CalcCode.BuildStringFromPlayerUnit(puTemp);
            //    photonView.RPC("AddPUForPlayer2", PhotonTargets.Others, new object[] { puCode });
            //}

            //code to test for bad puCodeLength
            //totalUnits -= 1;
            //PlayerUnit puTemp = new PlayerUnit(modVersion, true, false);
            //string puCodeTest = CalcCode.BuildStringFromPlayerUnit(puTemp);
            //if( puCodeTest.Length != 105)
            //{
            //    Debug.Log("ERROR: TEST 1 puCode length is too long " + puCodeTest.Length + " " + puTemp.ClassId + " " + puTemp.AbilitySecondaryCode + " at position " + totalUnits );
            //    Debug.Log(puCodeTest);
            //    puAvailableList.Add(puTemp);
            //}
            //else
            //{
            //    PlayerUnit puTemp2 = CalcCode.BuildPlayerUnit(puCodeTest);
            //    puCodeTest = CalcCode.BuildStringFromPlayerUnit(puTemp2);
            //    if (puCodeTest.Length != 105)
            //    {
            //        Debug.Log("ERROR: TEST 2 puCode length is too long " + puCodeTest.Length + " " + puTemp.ClassId + " " + puTemp.AbilitySecondaryCode + " at position " + totalUnits);
            //        Debug.Log(puCodeTest);
            //        puAvailableList.Add(puTemp);
            //    }
            //}
        }
        PopulatePicksList(pickOrder, lastPick, pickingTeam);
        //pickListObject.PopulatePickBanNames(puAvailableList, pickOrder, lastPick); //pre-draft phase
        AbilityManager.Instance.SetIoType(NameAll.NULL_INT); //clears the lists
        ItemManager.Instance.SetIoType(NameAll.NULL_INT); //clears the lists
    }

    //[PunRPC]
    public void PopulatePicksList(int zPickOrder, int zLastPick, int zPickingTeam )
    {
        if (true)//(PhotonNetwork.offlineMode)
		{
            pickListObject.PopulatePickBanNames(puAvailableList, pickOrder, lastPick, pickingTeam, true, false);
        }
        else
        {
            //if (PhotonNetwork.isMasterClient)
            //{
            //    photonView.RPC("PopulatePicksList", PhotonTargets.Others, new object[] { pickOrder, lastPick, pickingTeam });
            //    pickListObject.PopulatePickBanNames(puAvailableList, pickOrder, lastPick, pickingTeam, PhotonNetwork.offlineMode, PhotonNetwork.isMasterClient);
            //}
            //else
            //{
            //    pickListObject.PopulatePickBanNames(puAvailableList, zPickOrder, zLastPick, zPickingTeam, PhotonNetwork.offlineMode, PhotonNetwork.isMasterClient);
            //}
        }
    }

    //[PunRPC]
    public void AddPUForPlayer2( string puCode)
    {
        PlayerUnit puTemp = CalcCode.BuildPlayerUnit(puCode);
        puAvailableList.Add(puTemp);
    }

    public void DisableRandomDraftObject()
    {
        banListObject.gameObject.SetActive(false);
        pickListObject.gameObject.SetActive(false);
    }

    public void UpdatePickList( int index, int zPickingTeam)//PlayerUnit puTemp)
    {
        PlayerUnit puTemp = puAvailableList[index];
        RemoveRandomDraftPlayerUnit(index);
        //puAvailableList.RemoveAt(index);
        //PopulatePicksList(pickOrder, lastPick,pickingTeam);
        //pickListObject.PopulatePickBanNames(puAvailableList, pickOrder, lastPick);
        ForcePick(zPickingTeam, CalcCode.BuildStringFromPlayerUnit(puTemp));

        HandleIncrementDraftPhase();
    }

    //[PunRPC]
    public void HandleIncrementDraftPhase(bool forceMasterClient = false)
    {
		if (true)//if(PhotonNetwork.offlineMode) 
		{
            IncrementDraftPhase();
        }
        else
        {
            //if( PhotonNetwork.isMasterClient || forceMasterClient)
            //{
            //    IncrementDraftPhase();
            //}
            //else
            //{
            //    photonView.RPC("HandleIncrementDraftPhase", PhotonTargets.MasterClient, new object[] { true });
            //}
        }
    }

    public void UpdateBanList(int index)//PlayerUnit puTemp)
    {
        PlayerUnit puTemp = puAvailableList[index];
        string puCode = CalcCode.BuildStringFromPlayerUnit(puTemp);
        //if (!PhotonNetwork.offlineMode )//P2 does it to P1 here too //&& PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("AddToBanList", PhotonTargets.Others, new object[] { puCode });
        //}
        puBanList.Add(puTemp);
        banListObject.PopulateNames(puBanList);
        //Debug.Log("count is " + puAvailableList.Count + " " + puAvailableList.Contains(puTemp));
        RemoveRandomDraftPlayerUnit(index);
        //Debug.Log("count is " + puAvailableList.Count);
        HandleIncrementDraftPhase();
    }

    //[PunRPC]
    public void AddToBanList(string puCode)
    {
        PlayerUnit puTemp = CalcCode.BuildPlayerUnit(puCode);
        puBanList.Add(puTemp);
        banListObject.PopulateNames(puBanList);
        //RemoveRandomDraftPlayerUnit(index);
    }

    void InitializePickBanLists()
    {
        pickListObject.gameObject.SetActive(true);
        banListObject.gameObject.SetActive(true);
        puAvailableList = new List<PlayerUnit>();
        puBanList = new List<PlayerUnit>();
        PopulatePicksList(pickOrder, lastPick, pickingTeam);
        //pickListObject.PopulatePickBanNames(puAvailableList, pickOrder,lastPick, pickingTeam, PhotonNetwork.offlineMode,false); //only needed here 
        banListObject.PopulateNames(puBanList);
    }

    //void EnableStartButton()
    //{

    //}
    //need it to be flexible to randomize who goes first
    //then fills up the rest of the slots with number of picks that can be made
    //maybe use the script in setup map
}
