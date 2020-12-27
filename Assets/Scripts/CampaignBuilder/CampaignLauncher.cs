using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class CampaignLauncher : MonoBehaviour {

    #region variables

    CampaignCampaign currentCampaign = null;
    CampaignLevel currentLevel = null;
    int currentLevelIndex = 0;

    //scrollObject
    public GameObject sampleButton; //fills most scrolllists
    public Transform contentPanel;

    //header text
    public Text campaignHeader;
    public Text levelHeader;
    string campaignHeaderDefault = "Campaign: ";
    string levelHeaderDefault = "Level: ";

    public GameObject startButton;
    GameObject pmo;

    //unit selection items
    //unit scroll list
    public GameObject UnitScrollListPanel;
    public Transform unitListContentPanel;
    public Text unitListText;
    public GameObject unitScrollListSelectButton; //for the UnitScrollList, has an extra button for adding to selectedUnitList
    Dictionary<int, PlayerUnit> unitDict; //for the UnitScrollList
    List<PlayerUnit> selectedUnitList; //units selected for Combat
    //unit info panel
    static readonly int UNIT_SELECTION_DEFAULT_MESSAGE = -1;
    static readonly int UNIT_SELECTION_ADD = -2;
    static readonly int UNIT_SELECTION_CLEAR = -3;
    public UIUnitInfoPanel statPanel;
    PlayerUnit pu;

    int levelTotalPlayerUnits;
    int levelPlayerSelectableUnits;
    #endregion

    void Start()
    {
        RefreshHeader();
        startButton.SetActive(false);
        pmo = Instantiate(Resources.Load("PlayerManagerObject")) as GameObject;
        selectedUnitList = new List<PlayerUnit>();
        unitDict = new Dictionary<int, PlayerUnit>();
        levelTotalPlayerUnits = 0;
        levelPlayerSelectableUnits = 0;
    }

    #region OnClickButtons

    public void OnClickStart()
    {
        //Debug.Log("need to set the levels and what not correctly");
        if (currentLevel != null && currentLevel.GetName(currentLevelIndex) != NameAll.LEVEL_ERROR_MESSAGE)
        {
            //if the user has to select some units, add them
            if( selectedUnitList.Count > 0)
            {
                foreach(PlayerUnit punit in selectedUnitList)
                {
                    PlayerManager.Instance.EditTeamLists(punit, NameAll.TEAM_ID_GREEN, NameAll.TEAM_LIST_ADD);
                }
            }

            if( currentCampaign.CampaignId >= NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE)
            {
                PlayerPrefs.SetString(NameAll.PP_LEVEL_DIRECTORY, NameAll.LEVEL_DIRECTORY_CAMPAIGN_CUSTOM); //Debug.Log(" directory is " +)
            }
            else
            {
                PlayerPrefs.SetString(NameAll.PP_LEVEL_DIRECTORY, NameAll.LEVEL_DIRECTORY_CAMPAIGN_AURELIAN);
            }
            //PlayerPrefs.GetInt(NameAll.PP_COMBAT_LEVEL, 0)
            PlayerPrefs.SetInt(NameAll.PP_COMBAT_LEVEL, currentLevelIndex); //Debug.Log("setting combat level: " + currentLevelIndex);
            PlayerPrefs.SetInt(NameAll.PP_COMBAT_CAMPAIGN_LOAD, currentCampaign.CampaignId);
            PlayerManager.Instance.SetPhotonNetworkOfflineMode(true); //defaulting to online
            DontDestroyOnLoad(pmo);
            PlayerPrefs.SetInt(NameAll.PP_COMBAT_ENTRY, NameAll.SCENE_CAMPAIGNS);
            SceneManager.LoadScene(NameAll.SCENE_COMBAT);
        }
            
    }

    //scrollist is populated with loadable objects
    public void OnClickCampaigns()
    {
        statPanel.Close();
        UnitScrollListPanel.SetActive(false);
        PopulateCampaignScrollList();
    }

    public void OnClickLevels()
    {
        statPanel.Close();
        UnitScrollListPanel.SetActive(false);
        PopulateLevelScrollList();
    }

    public void OnClickUnits()
    {
        statPanel.Close();
        UnitScrollListPanel.SetActive(false);
        PopulateUnitSelectionScrollList();
        CheckStartButtonEnabledDisabled();
    }

    public void OnClickExit()
    {
        SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
    }

    #endregion

    #region PopulateScrollLists
    //selecting unit selection
    void PopulateUnitSelectionScrollList()
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildUnitSelectionList();

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonUnitSelectionClicked(tempInt));
        }
    }

    List<AbilityBuilderObject> BuildUnitSelectionList()
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;

        if( currentLevel == null)
        {
            abo = new AbilityBuilderObject("Select a level.","", UNIT_SELECTION_DEFAULT_MESSAGE);
            retValue.Add(abo);
            return retValue;
        }

        abo = new AbilityBuilderObject("Total Units Allowed: " + levelTotalPlayerUnits,
            "Selected Units: " + selectedUnitList.Count + "/" + levelPlayerSelectableUnits, UNIT_SELECTION_DEFAULT_MESSAGE);
        retValue.Add(abo);
        if (selectedUnitList.Count > 0)
        {
            abo = new AbilityBuilderObject("Clear Selected Units", "", UNIT_SELECTION_CLEAR);
            retValue.Add(abo);
        }
        if (selectedUnitList.Count < levelPlayerSelectableUnits)
        {
            abo = new AbilityBuilderObject("Select Units", "", UNIT_SELECTION_ADD);
            retValue.Add(abo);
        }
        
        for( int i = 0; i < selectedUnitList.Count; i++)
        {
            abo = new AbilityBuilderObject(CalcCode.GetTitleFromPlayerUnit(selectedUnitList[i]), CalcCode.GetDetailsFromPlayerUnit(selectedUnitList[i]), i);
            retValue.Add(abo);
        }

        return retValue;
    }

    void ButtonUnitSelectionClicked(int clickId)
    {
        //set the current campaign
        //LoadCampaign(clickId);

        if( clickId == UNIT_SELECTION_ADD)
        {
            UnitScrollListPanel.SetActive(true);
            PopulateUnitScrollList();
            CheckStartButtonEnabledDisabled();
            return;
        }
        else if( clickId == UNIT_SELECTION_CLEAR)
        {
            selectedUnitList.Clear();
            OnClickUnits();
            CheckStartButtonEnabledDisabled();
            return;
        }
        else if( clickId == UNIT_SELECTION_DEFAULT_MESSAGE)
        {
            return;
        }
        else
        {
            statPanel.Open();
            pu = selectedUnitList[clickId];
            statPanel.PopulatePlayerInfo(pu, false);
            CheckStartButtonEnabledDisabled();
            return;
        }
    }

    //selecting the campaign
    void PopulateCampaignScrollList()
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildCampaignList();

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonCampaignClicked(tempInt));
        }
    }

    List<AbilityBuilderObject> BuildCampaignList()
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;

        //load the Aurelian campaigns
        for (int i = 0; i < NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE; i++)
        {
            CampaignCampaign cc = CalcCode.LoadCampaignCampaign(i);
            if (cc != null)
            {
                abo = new AbilityBuilderObject("Aurelian Campaign: " + cc.CampaignName, "", cc.CampaignId);
                retValue.Add(abo);
            }
            else
            {
                break;
            }
        }

        int zBreak = 0;
        for (int i = NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE; i <= (NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE + 100); i++)
        {
            CampaignCampaign cc = CalcCode.LoadCampaignCampaign(i); //Debug.Log("reached create new campaign " + i);
            if (cc != null)
            {
                abo = new AbilityBuilderObject("Campaign: " + cc.CampaignName, "", cc.CampaignId); //Debug.Log("testing load");
                retValue.Add(abo);
            }
            else
            {
                zBreak += 1;
            }
            if (zBreak >= 5)
                break;
        }
        return retValue;
    }

    void ButtonCampaignClicked(int clickId)
    {
        //set the current campaign
        LoadCampaign(clickId);
        //for now just loading from teh default direction, in the future add some campaign options for this
        unitDict = CalcCode.LoadPlayerUnitDict(currentCampaign.Version,false, currentCampaign.CampaignId);
    }

    //selecting the level items
    void PopulateLevelScrollList()
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildLevelList();

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonLevelClicked(tempInt));
        }
    }

    List<AbilityBuilderObject> BuildLevelList()
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;

        if (currentCampaign == null)
        {
            //abo = new AbilityBuilderObject("No Campaign is selected.", "Select a campaign or create a new campaign to add a level", INPUT_LEVEL_NONE);
            //retValue.Add(abo);
            return retValue;
        }

        //abo = new AbilityBuilderObject("Add New Level ", "", INPUT_LEVEL_NEW);
        //retValue.Add(abo);
        //Debug.Log("current level size is " + currentLevel.orderList.Count);
        foreach (int i in currentLevel.orderList)
        {
            abo = new AbilityBuilderObject(currentLevel.GetName(i), "Level Order: " + (i + 1), i);
            retValue.Add(abo);
        }
        return retValue;
    }

    void ButtonLevelClicked(int levelId)
    {
        //if (levelId == INPUT_LEVEL_NONE)
        //{
        //    return;
        //}
        //else if (levelId == INPUT_LEVEL_NEW)
        //{
        //    //creates a newLevel and returns the levelIndex associated with it
        //    currentLevelIndex = currentLevel.AddNew();
        //    RefreshHeader();
        //    OnClickLevels();
        //    return;
        //}
        //else
        //{
        //    currentLevelIndex = levelId;
        //}
        currentLevelIndex = levelId;
        RefreshHeader();
        ResetPlayerUnitLists();
   
    }
    #endregion

    #region other
    void LoadCampaign(int id)
    {
        currentCampaign = CalcCode.LoadCampaignCampaign(id);
        currentLevel = CalcCode.LoadCampaignLevel(id);
        currentLevelIndex = 0;
        RefreshHeader();

        if(currentLevel != null && currentLevel.GetName(currentLevelIndex) != NameAll.LEVEL_ERROR_MESSAGE)
        {
            ResetPlayerUnitLists();
        }
            
        //currentSpawn = LoadCampaignSpawn(id);
        //currentDialogue = LoadCampaignDialogue(id);

        //currentInput = 0;
        //currentSubCategory = 0;
        //currentSpawnId = 0;
        //currentDialogueObject = null;
        //currentLevelIndex = 0;

        ////LevelLoad ll = new LevelLoad();
        //currentMapSpawnList = GetSpawnPointsFromDict(currentLevel.GetMap(currentLevelIndex));//ll.GetSpawnPointsForCustomMap(currentLevel.GetMap(currentLevelIndex));

        //BuildInitialSpawn(currentMapSpawnList);
        //RefreshHeader();

        //CharacterSelectPopup csp = new CharacterSelectPopup();
        //puListAurelian = csp.GetPlayerUnitListForCampaign(currentCampaign.Version, currentCampaign.CampaignId);
        //puListClassic = csp.GetPlayerUnitListForCampaign(currentCampaign.Version, currentCampaign.CampaignId);

        //Debug.Log("testing spawns ");
        //foreach(SpawnObject s in currentSpawn.spawnList)
        //{
        //    Debug.Log("spawn is " + s.Level + "," + s.SpawnId);
        //}
    }

    void RefreshHeader()
    {
        if (currentCampaign == null)
        {
            campaignHeader.text = "No Campaign Selected";
        }
        else
        {
            campaignHeader.text = campaignHeaderDefault + currentCampaign.CampaignName;
        }

        if (currentLevel == null || currentLevel.GetName(currentLevelIndex) == NameAll.LEVEL_ERROR_MESSAGE)
        {
            levelHeader.text = "No Level Selected";
        }
        else
        {
            levelHeader.text = levelHeaderDefault + currentLevel.GetName(currentLevelIndex);
        }
    }

    //on campaign and level change, changes the units that are in the list that will be loaded for the combat
    void ClearPlayerUnitLists()
    {
        PlayerManager.Instance.EditTeamLists(null,0,NameAll.TEAM_LIST_CLEAR);
    }

    void ResetPlayerUnitLists()
    {
        ClearPlayerUnitLists();
        levelPlayerSelectableUnits = 0;
        levelTotalPlayerUnits = 0;
        if( currentCampaign != null)
        {

            CampaignSpawn cs = CalcCode.LoadCampaignSpawn(currentCampaign.CampaignId);//CalcCode.Load
            if(cs != null)
            {
                List<SpawnObject> spawnList = cs.GetSpawnListByLevel(currentLevelIndex);    
                List<PlayerUnit> puList = CalcCode.GetPlayerUnitListForCampaign(0, currentCampaign.CampaignId);
                foreach ( SpawnObject so in spawnList)
                {
                    PlayerUnit pu = CalcCode.GetPlayerUnitFromSpawnObject(so, puList);
                    if( pu != null)
                    {
                        PlayerManager.Instance.EditTeamLists(pu, so.Team, NameAll.TEAM_LIST_ADD);
                    }

                    if (so.Team == NameAll.TEAM_ID_GREEN)
                    {
                        levelTotalPlayerUnits += 1;
                        if (so.SpawnType == NameAll.SPAWN_TYPE_USER_SELECT)
                            levelPlayerSelectableUnits += 1;
                    }
                        
                }
            }

            if (PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_GREEN).Count > 0)
            {
                startButton.SetActive(true);
            }
            else
                startButton.SetActive(false);
        }
    }

    void CheckStartButtonEnabledDisabled()
    {
        if (selectedUnitList.Count > 0 || PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_GREEN).Count > 0)
            startButton.SetActive(true);
        else
            startButton.SetActive(false);
        
    }
    #endregion

    #region UnitScrollList

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
            GameObject newButton = Instantiate(unitScrollListSelectButton) as GameObject;
            MPPickBanButton tb = newButton.GetComponent<MPPickBanButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(unitListContentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonUnitClicked(tempInt));

            tb.pickBanButton.onClick.AddListener(() => ButtonSelectClicked(tempInt));
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

        return retValue;
    }

    void ButtonUnitClicked(int select)
    {
        pu = unitDict[select];
        SetStatPanel();
    }

    void ButtonSelectClicked( int select)
    {
        selectedUnitList.Add(unitDict[select]);
        OnClickUnits();
    }

    public void SetStatPanel()
    {
        statPanel.Open(true);//disables close
        statPanel.PopulatePlayerInfo(pu, false);

    }
    #endregion
}
