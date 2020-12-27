using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;

public class CampaignEditController : MonoBehaviour
{
    //currentValues. used to keep track of what inputs and values are currently selected
    CampaignCampaign currentCampaign = null;
    CampaignLevel currentLevel = null;
    CampaignDialogue currentDialogue = null;
    DialogueObject currentDialogueObject = null;
    CampaignSpawn currentSpawn = null;
    List<Vector3> currentMapSpawnList = null;
    List<PlayerUnit> puListClassic = null;
    List<PlayerUnit> puListAurelian = null;
    Dictionary<int, LevelData> levelDict;


    int currentInput = 0;
    int currentSubCategory = 0;
    int currentLevelIndex = 0;
    int currentSpawnId = 0; //used with level to find the specific spawn
    //int currentDialogueIndex = 0; //using dialogue object instead

    //unique identifiers so scroll lists know what values to populate and what values to update
    static readonly int INPUT_LEVEL_INFO = 0;
    static readonly int INPUT_DIALOGUE = 1;
    static readonly int INPUT_SPAWN = 2;
    //second level dialogue clicks
    static readonly int INPUT_DIALOGUE_2 = 10;
    static readonly int INPUT_DIALOGUE_LEVEL = 11; //is the current level
    static readonly int INPUT_DIALOGUE_ORDER = 12;
    static readonly int INPUT_DIALOGUE_DIALOGUE = 13;
    static readonly int INPUT_DIALOGUE_PHASE = 14;
    static readonly int INPUT_DIALOGUE_ICON = 15;
    static readonly int INPUT_DIALOGUE_SIDE = 16;
    //second level spawn clicks
    static readonly int INPUT_SPAWN_2 = 20;
    static readonly int INPUT_SPAWN_TYPE = 21;
    static readonly int INPUT_SPAWN_TEAM = 22;
    static readonly int INPUT_SPAWN_UNIT_1 = 23;
    static readonly int INPUT_SPAWN_UNIT_2 = 24;
    static readonly int INPUT_SPAWN_UNIT_3 = 25;
    static readonly int INPUT_SPAWN_UNIT_4 = 26;
    static readonly int INPUT_SPAWN_UNIT_5 = 27;

    static readonly int INPUT_CAMPAIGN_EDIT = 30;
    static readonly int INPUT_CAMPAIGN_EDIT_2 = 31;
    static readonly int INPUT_CAMPAIGN_NAME = 32;
    static readonly int INPUT_CAMPAIGN_VERSION = 33;

    static readonly int INPUT_LEVEL_NEW = -1;
    static readonly int INPUT_LEVEL_INFO_NONE = -2;
    static readonly int INPUT_DIALOGUE_NONE = -3;
    static readonly int INPUT_SPAWN_NONE = -4;
    static readonly int INPUT_DIALOGUE_NEW = -5;
    static readonly int INPUT_CAMPAIGN_DELETE = -6;
    static readonly int INPUT_LEVEL_DELETE = -7;
    static readonly int INPUT_SPAWN_DELETE = -8;
    static readonly int INPUT_DIALOGUE_DELETE = -9;
    static readonly int INPUT_LEVEL_NONE = -10;
    static readonly int INPUT_CAMPAIGN_NEW = -11;
    static readonly int INPUT_LEVEL_INFO_MAP_NONE = -12;
    static readonly int INPUT_SPAWN_CANT_EDIT = -13;
    static readonly int INPUT_BATTLE_XP = -14;
    static readonly int INPUT_BATTLE_AP = -15;

    static readonly int LEVEL_INFO_NAME = 0;
    static readonly int LEVEL_INFO_ORDER = 1;
    static readonly int LEVEL_INFO_MAP = 2;
    static readonly int LEVEL_INFO_VC = 3;
    static readonly int LEVEL_INFO_XP = 4;
    static readonly int LEVEL_INFO_AP = 5;

    //static readonly string LEVEL_ERROR_MESSAGE = "errorLevel19";

    //input object
    public GameObject abilityInput;
    public Text aiTitle;
    public Text aiDetails;
    public InputField aiInputField;

    //info object 
    public GameObject infoPanel;
    public Text infoTitle;
    public Text infoDetails;

    //scrollObject
    public GameObject sampleButton;
    public Transform contentPanel;

    //header text
    public Text campaignHeader;
    public Text levelHeader;
    string campaignHeaderDefault = "Campaign: ";
    string levelHeaderDefault = "Level: ";

    void Awake()
    {
        AddInputFieldListener();
    }

    void Start()
    {
        puListClassic = CalcCode.GetPlayerUnitListForCampaign(NameAll.VERSION_CLASSIC, NameAll.NULL_UNIT_ID);
        puListAurelian = CalcCode.GetPlayerUnitListForCampaign(NameAll.VERSION_AURELIAN, NameAll.NULL_UNIT_ID);

        levelDict = new Dictionary<int, LevelData>();

        campaignHeader.text = "No Campaign Selected";
        levelHeader.text = "No Level Selected";
    }

    #region populating scroll lists, building the lists, and handling input

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

        if ( currentCampaign == null)
        {
            abo = new AbilityBuilderObject("No Campaign is selected.", "Select a campaign or create a new campaign to add a level", INPUT_LEVEL_NONE);
            retValue.Add(abo);
            return retValue;
        }
        
        abo = new AbilityBuilderObject("Add New Level ", "", INPUT_LEVEL_NEW);
        retValue.Add(abo);
        //Debug.Log("current level size is " + currentLevel.orderList.Count);
        foreach( int i in currentLevel.orderList)
        {
            abo = new AbilityBuilderObject( currentLevel.GetName(i), "Level Order: " + (i+1), i);
            retValue.Add(abo);
        }
        return retValue;
    }

    void ButtonLevelClicked(int levelId)
    {
        if( levelId == INPUT_LEVEL_NONE)
        {
            return;
        }
        else if( levelId == INPUT_LEVEL_NEW)
        {
            //creates a newLevel and returns the levelIndex associated with it
            currentLevelIndex = currentLevel.AddNew();
            RefreshHeader();
            OnClickLevels();
            return;
        }
        else
        {
            currentLevelIndex = levelId;
        }
        RefreshHeader();
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

        abo = new AbilityBuilderObject("Create New Campaign", "", INPUT_CAMPAIGN_NEW); //Debug.Log("reached create new campaign "+ NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE + " ");
        retValue.Add(abo);

        if ( currentCampaign != null)
        {
            abo = new AbilityBuilderObject("Delete Current Campaign", "Permanently delete campaign and all associated info (levels, dialogue, and spawns).", INPUT_CAMPAIGN_DELETE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Current Campaign: " + currentCampaign.CampaignName, "Version: " + CalcCode.GetVersionFromInt(currentCampaign.Version), INPUT_CAMPAIGN_EDIT);
            retValue.Add(abo);
        }

        int zBreak = 0;
        for( int i = NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE; i <= (NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE + 100); i++)
        {
            CampaignCampaign cc = CalcCode.LoadCampaignCampaign(i); //Debug.Log("reached create new campaign " + i);
            if ( cc != null)
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
        if( clickId == INPUT_CAMPAIGN_NEW)
        {
            OnClickNew();
            return;
        }
        else if( clickId == INPUT_CAMPAIGN_DELETE)
        {
            DeleteCampaign(currentCampaign.CampaignId, INPUT_CAMPAIGN_DELETE);
            return;
        }
        else if( clickId == INPUT_CAMPAIGN_EDIT)
        {
            currentInput = INPUT_CAMPAIGN_EDIT;
            currentSubCategory = INPUT_CAMPAIGN_EDIT;
            PopulateInputScrollList(clickId, clickId);
            return;
        }

        LoadCampaign(clickId);
    }

    //spawns
    void PopulateSpawnScrollList()
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildSpawnList();

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonSpawnClicked(tempInt));
        }
    }

    List<AbilityBuilderObject> BuildSpawnList()
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;

        if( currentLevel == null || currentLevel.GetName(currentLevelIndex) == NameAll.LEVEL_ERROR_MESSAGE )
        {
            abo = new AbilityBuilderObject("No Level is selected.", "Select a level and a map to edit spawn points.", INPUT_SPAWN_NONE);
            retValue.Add(abo);
            return retValue;
        }
        else if( currentSpawn.spawnList.Count == 0)
        {
            abo = new AbilityBuilderObject("Current Level's Map has no spawn points", "", INPUT_SPAWN_NONE);
            retValue.Add(abo);
            return retValue;
        }

        List<SpawnObject> tempList = currentSpawn.GetSpawnListByLevel(currentLevelIndex);
        if( tempList.Count == 0) //tries to build the spawn points based on the current level
        {
            currentMapSpawnList = GetSpawnPointsFromDict(currentLevel.GetMap(currentLevelIndex));
            BuildInitialSpawn(currentMapSpawnList);
            tempList = currentSpawn.GetSpawnListByLevel(currentLevelIndex);
            if (tempList.Count == 0) //checks if newly built spawn points still 0
            {
                abo = new AbilityBuilderObject("Current Level's Map has no spawn points", "", INPUT_SPAWN_NONE);
                retValue.Add(abo);
                return retValue;
            }
        }

        foreach (SpawnObject s in tempList)
        {
            abo = new AbilityBuilderObject(CalcCode.GetSpawnTeamString(s.Team),
                "Id: " + s.SpawnId + " Units: "
                + GetPlayerUnitString(s.Unit1, true) + ", " + GetPlayerUnitString(s.Unit2, true) + ", " + GetPlayerUnitString(s.Unit3, true) + ", "
                + GetPlayerUnitString(s.Unit4, true) + ", " + GetPlayerUnitString(s.Unit5, true), s.SpawnId);
            retValue.Add(abo);
        }

        return retValue;
    }

    //button on PopulateSpawnScrollList Clicked, goes to second level
    void ButtonSpawnClicked(int clickId)
    {
        if (clickId == INPUT_SPAWN_NONE)
            return;

        currentInput = INPUT_SPAWN;
        currentSubCategory = clickId; //holds the spawn Id
        currentSpawnId = clickId;
        PopulateInputScrollList(INPUT_SPAWN, clickId);
    }

    //dialogue
    void PopulateDialogueScrollList()
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildDialogueList();

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonDialogueClicked(tempInt));
        }
    }

    List<AbilityBuilderObject> BuildDialogueList()
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;

        if (currentLevel == null || currentLevel.GetName(currentLevelIndex) == NameAll.LEVEL_ERROR_MESSAGE)
        {
            abo = new AbilityBuilderObject("No Level is selected.", "Select a level to add dialogues.", INPUT_DIALOGUE_NONE);
            retValue.Add(abo);
            return retValue;
        }

        abo = new AbilityBuilderObject("Create New Dialogue", "", INPUT_DIALOGUE_NEW);
        retValue.Add(abo);
        List<DialogueObject> tempList = currentDialogue.GetDialogueListByLevel(currentLevelIndex);
        int dialogueCount = tempList.Count;
        for( int i = 0; i < dialogueCount; i++)
        {
            DialogueObject d = tempList[i];
            abo = new AbilityBuilderObject("Level: " + (d.Level+1) + " Phase: " + CalcCode.GetDialoguePhase(d.Phase) + " Order: " + (d.Order+1) + " Side: " + CalcCode.GetDialogueSide(d.Side) ,d.Dialogue,i);
            retValue.Add(abo);
        }

        return retValue;
    }

    void ButtonDialogueClicked(int clickId)
    {
        if( clickId == INPUT_DIALOGUE_NONE)
        {
            return;
        }
        else if(clickId == INPUT_DIALOGUE_NEW)
        {
            currentDialogueObject = currentDialogue.AddNewDialogue(currentLevelIndex);
        }
        else
        {
            currentDialogueObject = currentDialogue.GetDialogueListByLevel(currentLevelIndex)[clickId];
        }
        currentInput = INPUT_DIALOGUE;
        //currentDialogueIndex = clickId;
        
        PopulateInputScrollList(INPUT_DIALOGUE, 0); //relies on currentDialogueObject 
    }


    //level info
    void PopulateLevelInfoScrollList()
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildLevelInfoList();

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonLevelInfoClicked(tempInt));
        }
    }

    List<AbilityBuilderObject> BuildLevelInfoList()
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;
       
        if ( currentLevel == null || currentLevel.orderList.Count <= 0 || currentLevelIndex >= currentLevel.orderList.Count 
            || currentLevel.GetName(currentLevelIndex) == NameAll.LEVEL_ERROR_MESSAGE)
        {
            abo = new AbilityBuilderObject("No Level is selected.", "Select a level to see level info.", INPUT_LEVEL_INFO_NONE);
            retValue.Add(abo);
            return retValue; //nothing to populate or a population error
        }

        abo = new AbilityBuilderObject("Delete Level", "Permanently delete level", INPUT_LEVEL_DELETE);
        retValue.Add(abo);
        
        abo = new AbilityBuilderObject("Level Name",currentLevel.GetName(currentLevelIndex),LEVEL_INFO_NAME);
        retValue.Add(abo);
        abo = new AbilityBuilderObject("Campaign Order", (currentLevel.GetOrder(currentLevelIndex) + 1), LEVEL_INFO_ORDER); //for display so it looks like it starts at 1
        retValue.Add(abo);
        abo = new AbilityBuilderObject("Map", GetMapStringFromDict(currentLevel.GetMap(currentLevelIndex)), LEVEL_INFO_MAP);
        retValue.Add(abo);
        abo = new AbilityBuilderObject("Victory Conditions", currentLevel.GetVCString(currentLevelIndex), LEVEL_INFO_VC);
        retValue.Add(abo);
        abo = new AbilityBuilderObject("Battle XP", currentLevel.GetBattleXP(currentLevelIndex), LEVEL_INFO_XP);
        retValue.Add(abo);
        abo = new AbilityBuilderObject("Battle AP", currentLevel.GetBattleAP(currentLevelIndex), LEVEL_INFO_AP);
        retValue.Add(abo);
        return retValue;
    }

    //button on PopulateEditScrollList Clicked
    void ButtonLevelInfoClicked(int clickId)
    {
        if( clickId == INPUT_LEVEL_INFO_NONE)
        {
            return;
        }
        else if (clickId == INPUT_LEVEL_DELETE)
        {
            DeleteCampaign(currentCampaign.CampaignId, INPUT_LEVEL_DELETE);
            return;
        }
        currentInput = INPUT_LEVEL_INFO;
        currentSubCategory = clickId;
        OpenLevelInfoInput(clickId);
        //set the current campaign
        //Debug.Log("each level info object, decide on what to open/how to open");
    }

    //nested lists, ie one list clicked, need to go to a further list 
    void PopulateInputScrollList(int inputType, int subCategory)
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        

        List<AbilityBuilderObject> aboList = BuildInputList(inputType,subCategory);

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonInputClicked(tempInt));
        }
    }

    List<AbilityBuilderObject> BuildInputList(int inputType, int subCategory)
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;
        if (inputType == INPUT_LEVEL_INFO)
        {
            if (subCategory == LEVEL_INFO_MAP)
            {
                if( levelDict.Count == 0)
                {
                    abo = new AbilityBuilderObject("No maps available to select", "", INPUT_LEVEL_INFO_MAP_NONE);
                    retValue.Add(abo);
                }
                else
                {
                    foreach (KeyValuePair<int, LevelData> kvp in levelDict)
                    {
                        abo = new AbilityBuilderObject(kvp.Value.levelName, "", kvp.Key);
                        retValue.Add(abo);
                    }
                }
                
            }
            else if (subCategory == LEVEL_INFO_VC)
            {
                abo = new AbilityBuilderObject("Defeat All Enemies", "",NameAll.VICTORY_TYPE_DEFEAT_PARTY);
                retValue.Add(abo);
            }
            else if (subCategory == LEVEL_INFO_ORDER)
            {
                foreach( int i in currentLevel.orderList)
                {
                    abo = new AbilityBuilderObject("Select Campaign Level Order: " + (i + 1), "Current Level: " + currentLevel.GetName(i), i);
                    retValue.Add(abo);
                }
                //int count = levelList.Count;
                //if( count > 0)
                //{
                //    int orderMax = 0;
                //    for (int i = 0; i < count; i++)
                //    {
                //        abo = new AbilityBuilderObject("Select Campaign Level Order: " + levelList[i].Order,"Current Level: " + levelList[i].LevelName, levelList[i].Order);
                //        retValue.Add(abo);
                //        if(levelList[i].Order > orderMax)
                //        {
                //            orderMax = levelList[i].Order;
                //        }
                //    }
                //    //abo = new AbilityBuilderObject("Select Campaign Level Order: " + (orderMax + 1), "Current Level: None", orderMax); //for last order, select the last thing on the list
                //}
                //else
                //{
                //    abo = new AbilityBuilderObject("Select Campaign Level Order: 1", "Current Level: None", 1);
                //    retValue.Add(abo);
                //}

            }
        }
        else if( inputType == INPUT_DIALOGUE)
        {
            //DialogueObject d = currentDialogue.dialogueList[subCategory];
            DialogueObject d = currentDialogueObject;
            //abo = new AbilityBuilderObject("Level",d.Level,INPUT_DIALOGUE_LEVEL);
            //retValue.Add(abo);
            abo = new AbilityBuilderObject("Change Order", (d.Order+1) + " (Level: " + (d.Level+1) + ")" , INPUT_DIALOGUE_ORDER);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Change Text", d.Dialogue, INPUT_DIALOGUE_DIALOGUE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Change Phase", CalcCode.GetDialoguePhase(d.Phase), INPUT_DIALOGUE_PHASE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Change Icon", d.Icon, INPUT_DIALOGUE_ICON);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Change Side", CalcCode.GetDialogueSide(d.Side), INPUT_DIALOGUE_SIDE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Delete Dialogue", "Permanently delete this dialogue.", INPUT_DIALOGUE_DELETE);
            retValue.Add(abo);
        }
        else if (inputType == INPUT_DIALOGUE_2)
        {
            if( subCategory == INPUT_DIALOGUE_ICON)
            {
                abo = new AbilityBuilderObject("Icon 0", "", NameAll.DIALOGUE_ICON_0);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Icon 1", "", NameAll.DIALOGUE_ICON_1);
                retValue.Add(abo);
            }
            else if (subCategory == INPUT_DIALOGUE_ORDER)
            {
                var tempList = currentDialogue.GetDialogueListByLevel(currentLevelIndex);
                foreach( DialogueObject d in tempList)
                {
                    abo = new AbilityBuilderObject("Order: " + (d.Order+1), "", d.Order);
                    retValue.Add(abo);
                }
                
            }
            else if (subCategory == INPUT_DIALOGUE_PHASE)
            {
                abo = new AbilityBuilderObject("Phase: Start of Battle", "", NameAll.DIALOGUE_PHASE_START);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Phase: End of Battle", "", NameAll.DIALOGUE_PHASE_END);
                retValue.Add(abo);
            }
            else if (subCategory == INPUT_DIALOGUE_SIDE)
            {
                abo = new AbilityBuilderObject("Side: Left", "", NameAll.DIALOGUE_SIDE_LEFT);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Side: Right", "", NameAll.DIALOGUE_SIDE_RIGHT);
                retValue.Add(abo);
            }
        }
        else if( inputType == INPUT_SPAWN)
        {
            //Debug.Log("current level index is " + currentLevelIndex + " " + subCategory);

            //other value is the spawnId that was clicked
            SpawnObject s = currentSpawn.GetSpawn(subCategory,currentLevelIndex);
       
            abo = new AbilityBuilderObject("Change Type", CalcCode.GetSpawnTypeString(s.SpawnType), INPUT_SPAWN_TYPE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Change Unit 1", GetPlayerUnitString(s.Unit1), INPUT_SPAWN_UNIT_1);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Change Unit 2", GetPlayerUnitString(s.Unit2), INPUT_SPAWN_UNIT_2);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Change Unit 3", GetPlayerUnitString(s.Unit3), INPUT_SPAWN_UNIT_3);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Change Unit 4", GetPlayerUnitString(s.Unit4), INPUT_SPAWN_UNIT_4);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Change Unit 5", GetPlayerUnitString(s.Unit5), INPUT_SPAWN_UNIT_5);
            retValue.Add(abo);
            //if ( s.Team == NameAll.TEAM_ID_GREEN)
            //{
            //    abo = new AbilityBuilderObject("Player (Team 1) spawn points cannot be edited here", "Edit the map to change how many units player can place.",INPUT_SPAWN_CANT_EDIT);
            //    retValue.Add(abo);
            //}
            //else
            //{
            //    abo = new AbilityBuilderObject("Change Type", CalcCode.GetSpawnTypeString(s.SpawnType), INPUT_SPAWN_TYPE);
            //    retValue.Add(abo);
            //    abo = new AbilityBuilderObject("Change Unit 1", GetPlayerUnitString(s.Unit1), INPUT_SPAWN_UNIT_1);
            //    retValue.Add(abo);
            //    abo = new AbilityBuilderObject("Change Unit 2", GetPlayerUnitString(s.Unit2), INPUT_SPAWN_UNIT_2);
            //    retValue.Add(abo);
            //    abo = new AbilityBuilderObject("Change Unit 3", GetPlayerUnitString(s.Unit3), INPUT_SPAWN_UNIT_3);
            //    retValue.Add(abo);
            //    abo = new AbilityBuilderObject("Change Unit 4", GetPlayerUnitString(s.Unit4), INPUT_SPAWN_UNIT_4);
            //    retValue.Add(abo);
            //    abo = new AbilityBuilderObject("Change Unit 5", GetPlayerUnitString(s.Unit5), INPUT_SPAWN_UNIT_5);
            //    retValue.Add(abo);
            //}
            
        }
        else if( inputType == INPUT_SPAWN_2)
        {
            if( subCategory == INPUT_SPAWN_TYPE)
            {
                abo = new AbilityBuilderObject("Spawn Type: Random", "", NameAll.SPAWN_TYPE_RANDOM);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Spawn Type: User Select", "User selects unit before level starts. Only works for player's team.", NameAll.SPAWN_TYPE_USER_SELECT);
                retValue.Add(abo);
            }
            else
            {
                //do whatever is done for the player list
                abo = new AbilityBuilderObject("Unit: None", "If randomly selected, no unit will appear in the battle.", NameAll.CAMPAIGN_SPAWN_UNIT_NONE);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Unit: Null", "Unit will be selected between the other units.", NameAll.CAMPAIGN_SPAWN_UNIT_NULL);
                retValue.Add(abo);
                List<PlayerUnit> tempList;
                if( currentCampaign.Version == NameAll.VERSION_CLASSIC)
                {
                    tempList = puListClassic;
                }
                else
                {
                    tempList = puListAurelian;
                }

                foreach( PlayerUnit pu in tempList)
                {
                    abo = new AbilityBuilderObject("Unit: " + pu.UnitName + " (" + pu.TurnOrder + ") ",
                        "Lvl: " + pu.Level + " " + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY,pu.ClassId)
                        + "/" + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_SECONDARY, pu.AbilitySecondaryCode), 
                        pu.TurnOrder);
                    retValue.Add(abo);
                }
            }
        }
        else if( inputType == INPUT_CAMPAIGN_EDIT)
        {
            abo = new AbilityBuilderObject("Change Campaign Name", currentCampaign.CampaignName, INPUT_CAMPAIGN_NAME);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Change Campaign Version", CalcCode.GetVersionFromInt(currentCampaign.Version), INPUT_CAMPAIGN_VERSION);
            retValue.Add(abo);
        }
        else if( inputType == INPUT_CAMPAIGN_EDIT_2)
        {
            if(subCategory == INPUT_CAMPAIGN_VERSION)
            {
                abo = new AbilityBuilderObject("Version: Aurelian", "", NameAll.VERSION_AURELIAN);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Version: Classic", "", NameAll.VERSION_CLASSIC);
                retValue.Add(abo);
            }
        }
        return retValue;
    }
    
    void ButtonInputClicked(int clickId)
    {
        //Debug.Log(" currentCategory, subCategory, and click id are " + currentInput + "," + currentSubCategory + "," + clickId);
        if (currentInput == INPUT_LEVEL_INFO)
        {
            if (currentSubCategory == LEVEL_INFO_MAP)
            {
                if (clickId == INPUT_LEVEL_INFO_MAP_NONE)
                {
                    OnClickLevelInfo();
                    return;
                }
                else
                {
                    currentLevel.SetMap(currentLevelIndex, clickId);
                    RefreshSpawnOnMapChange(currentLevelIndex);
                }
                OnClickLevelInfo();
            }
            else if (currentSubCategory == LEVEL_INFO_VC)
            {
                currentLevel.SetVC(currentLevelIndex, clickId);
                OnClickLevelInfo();
            }
            else if (currentSubCategory == LEVEL_INFO_ORDER)
            {
                currentLevel.ReorderLevel(currentLevelIndex, clickId);
                currentLevelIndex = clickId;
                RefreshHeader();
                OnClickLevelInfo();
            }
            else if( currentSubCategory == LEVEL_INFO_XP)
            {
                currentInput = INPUT_LEVEL_INFO;
                currentSubCategory = INPUT_BATTLE_XP;
                OpenAbilityInput(INPUT_LEVEL_INFO, INPUT_BATTLE_XP);
            }
            else if (currentSubCategory == LEVEL_INFO_AP)
            {
                currentInput = INPUT_LEVEL_INFO;
                currentSubCategory = INPUT_BATTLE_AP;
                OpenAbilityInput(INPUT_LEVEL_INFO, INPUT_BATTLE_AP);
            }
        }
        else if (currentInput == INPUT_DIALOGUE)
        {
            if (clickId == INPUT_DIALOGUE_DELETE)
            {
                DeleteCampaign(currentCampaign.CampaignId, INPUT_DIALOGUE_DELETE);
                return;
            }

            currentInput = INPUT_DIALOGUE_2;
            currentSubCategory = clickId;

            if (clickId == INPUT_DIALOGUE_DIALOGUE)
            {
                OpenAbilityInput(INPUT_DIALOGUE, INPUT_DIALOGUE_DIALOGUE);
            }
            else
            {
                PopulateInputScrollList(INPUT_DIALOGUE_2, clickId);
            }


        }
        else if (currentInput == INPUT_DIALOGUE_2)
        {
            if (currentSubCategory == INPUT_DIALOGUE_ORDER)
            {
                currentDialogue.UpdateOrder(currentDialogueObject, clickId);
                currentDialogueObject.Order = clickId;
            }
            else if (currentSubCategory == INPUT_DIALOGUE_PHASE)
            {
                currentDialogue.UpdatePhase(currentDialogueObject, clickId);
                currentDialogueObject.Phase = clickId;
            }
            else if (currentSubCategory == INPUT_DIALOGUE_ICON)
            {
                currentDialogue.UpdateIcon(currentDialogueObject, clickId);
                currentDialogueObject.Icon = clickId;
            }
            else if (currentSubCategory == INPUT_DIALOGUE_SIDE)
            {
                currentDialogue.UpdateSide(currentDialogueObject, clickId);
                currentDialogueObject.Side = clickId;
            }
            OnClickDialogue();
        }
        else if (currentInput == INPUT_SPAWN)
        {
            if (clickId == INPUT_SPAWN_CANT_EDIT)
            {
                OnClickSpawns();
                return;
            }
            //refreshes the list based on what spawn was pressed
            currentInput = INPUT_SPAWN_2;
            currentSubCategory = clickId;

            PopulateInputScrollList(INPUT_SPAWN_2, clickId);
        }
        else if (currentInput == INPUT_SPAWN_2)
        {
            currentSpawn.UpdateValue(currentSpawnId, currentLevelIndex, currentSubCategory, clickId);
            OnClickSpawns();
        }
        else if (currentInput == INPUT_CAMPAIGN_EDIT)
        {
            if (clickId == INPUT_CAMPAIGN_NAME)
            {
                currentInput = INPUT_CAMPAIGN_EDIT;
                currentSubCategory = INPUT_CAMPAIGN_NAME;
                OpenAbilityInput(INPUT_CAMPAIGN_EDIT, INPUT_CAMPAIGN_NAME);
            }
            else if (clickId == INPUT_CAMPAIGN_VERSION)
            {
                currentInput = INPUT_CAMPAIGN_EDIT_2;
                currentSubCategory = clickId;
                PopulateInputScrollList(INPUT_CAMPAIGN_EDIT_2,INPUT_CAMPAIGN_VERSION);
            }
        }
        else if( currentInput == INPUT_CAMPAIGN_EDIT_2)
        {
            if( currentSubCategory == INPUT_CAMPAIGN_VERSION)
            {
                if( clickId != currentCampaign.Version)
                {
                    currentCampaign.Version = clickId;
                    RefreshSpawnOnVersionChange(currentCampaign.CampaignId);
                }
                
                OnClickLoad();
            }
                
            
        }
    }

    #endregion

    #region functions called in assisting on loading/selecting

    //load all objects associate with campaign, ie load this specific campaing
    void LoadCampaign(int id)
    {
        currentCampaign = CalcCode.LoadCampaignCampaign(id);
        currentLevel = CalcCode.LoadCampaignLevel(id);
        if (currentLevel == null)
            currentLevel = new CampaignLevel(id);
        currentSpawn = CalcCode.LoadCampaignSpawn(id);
        if (currentSpawn == null)
            currentSpawn = new CampaignSpawn(id);
        currentDialogue = CalcCode.LoadCampaignDialogue(id);

        currentInput = 0;
        currentSubCategory = 0;
        currentSpawnId = 0;
        currentDialogueObject = null;
        currentLevelIndex = 0;

        levelDict = CalcCode.LoadLevelDict(true, currentCampaign.CampaignId);

        //LevelLoad ll = new LevelLoad();
        currentMapSpawnList = GetSpawnPointsFromDict(currentLevel.GetMap(currentLevelIndex));//ll.GetSpawnPointsForCustomMap(currentLevel.GetMap(currentLevelIndex));

        BuildInitialSpawn(currentMapSpawnList);
        RefreshHeader();

        puListAurelian = CalcCode.GetPlayerUnitListForCampaign(currentCampaign.Version, currentCampaign.CampaignId);
        puListClassic = CalcCode.GetPlayerUnitListForCampaign(currentCampaign.Version, currentCampaign.CampaignId);

        //Debug.Log("testing spawns ");
        //foreach(SpawnObject s in currentSpawn.spawnList)
        //{
        //    Debug.Log("spawn is " + s.Level + "," + s.SpawnId);
        //}
    }

    void DeleteCampaign(int id, int type)
    {
        if( type == INPUT_CAMPAIGN_DELETE)
        {
            GetComponent<DialogController>().Show("Delete Campaign", "Campaign and all related files will be deleted. Confirm?", DeleteCampaignInner, null);
        }
        else if( type == INPUT_LEVEL_DELETE)
        {
            GetComponent<DialogController>().Show("Delete Current Level", "Confirm?", DeleteLevelInner, null);
        }
        //else if( type == INPUT_SPAWN_DELETE)
        //{
        //    //can't really do this
        //}
        else if( type == INPUT_DIALOGUE_DELETE)
        {
            GetComponent<DialogController>().Show("Delete Selected Dialogue", "Confirm?", DeleteDialogueInner, null);
        }

    }

    void DeleteCampaignInner()
    {
        string filePathBase = Application.dataPath + "/Custom/Campaigns/Custom/" + currentCampaign.CampaignId;
        string filePath = filePathBase + "_level.dat";
        if (File.Exists(filePath))
            File.Delete(filePath);
        filePath = filePathBase + "_dialogue.dat";
        if (File.Exists(filePath))
            File.Delete(filePath);
        filePath = filePathBase + "_spawn.dat";
        if (File.Exists(filePath))
            File.Delete(filePath);
        filePath = filePathBase + "_campaign.dat";
        if (File.Exists(filePath))
            File.Delete(filePath);

        //how to delete files
        currentCampaign = null;
        currentDialogueObject = null;
        currentLevelIndex = 0;
        currentLevel = null;
        currentMapSpawnList = null;
        currentInput = 0;
        currentSpawn = null;
        currentSpawnId = 0;
        currentSubCategory = 0;

        RefreshHeader();
        OnClickLoad();
    }

    void DeleteLevelInner()
    {

        currentLevel.DeleteLevel(currentLevelIndex);
        currentLevelIndex = 0;

        RefreshHeader();
        OnClickLevels();
    }

    void DeleteDialogueInner()
    {
        currentDialogue.DeleteDialogue(currentDialogueObject);
        currentDialogueObject = null;
        OnClickDialogue();
    }

    void NewCampaign(int id)
    {
        currentCampaign = new CampaignCampaign(id);
        currentLevel = new CampaignLevel(id);
        currentDialogue = new CampaignDialogue(id);
        currentSpawn = new CampaignSpawn(id);

        currentInput = 0;
        currentSubCategory = 0;
        currentSpawnId = 0;
        currentDialogueObject = null;
        currentLevelIndex = 0;

        levelDict = new Dictionary<int, LevelData>();

        currentMapSpawnList = GetSpawnPointsFromDict(currentLevel.GetMap(currentLevelIndex));
        BuildInitialSpawn(currentMapSpawnList);
        RefreshHeader();
    }

    //if campaign version is changed, need to refresh the spawn points, this builds the new list. As maps are changed/loaded the new spawn points will be changed
    void RefreshSpawnOnVersionChange(int campaignId)
    {
        currentSpawn = new CampaignSpawn(campaignId);
        currentMapSpawnList = GetSpawnPointsFromDict(currentLevel.GetMap(currentLevelIndex));
        BuildInitialSpawn(currentMapSpawnList);
        currentSpawnId = 0;
    }

    //map for the level changes, need to delete the old spawn points and add the new onws
    void RefreshSpawnOnMapChange(int levelId)
    {
        currentSpawn.DeleteSpawnByLevel(levelId);
        currentMapSpawnList = GetSpawnPointsFromDict(currentLevel.GetMap(levelId));
        BuildInitialSpawn(currentMapSpawnList);
        currentSpawnId = 0;
    }


    void RefreshHeader()
    {
        if( currentCampaign == null)
        {
            campaignHeader.text = "No Campaign Selected";
        }
        else
        {
            campaignHeader.text = campaignHeaderDefault + currentCampaign.CampaignName;
        }
        
        if( currentLevel == null || currentLevel.GetName(currentLevelIndex) == NameAll.LEVEL_ERROR_MESSAGE)
        {
            levelHeader.text = "No Level Selected";
        }
        else
        {
            levelHeader.text = levelHeaderDefault + currentLevel.GetName(currentLevelIndex);
        }
    }


    //matches the spawnList from the map when a new map is chose
    void BuildInitialSpawn(List<Vector3> lv)
    {
        if (lv.Count == 0)
        {
            currentSpawn = new CampaignSpawn(currentCampaign.CampaignId);
        }

        var tempList = currentSpawn.GetSpawnListByLevel(currentLevelIndex);
        //rebuilds the list on a new one or a change in the map count
        if (tempList.Count <= 0)
        {
            int i = 0;
            foreach (Vector3 v in lv)
            {
                SpawnObject so = new SpawnObject(currentLevelIndex, (int)v.z, i); //Debug.Log("building spawn list " + so.Team + "," + v.z );
                currentSpawn.spawnList.Add(so);
                i++;
            }
        }
        else if (tempList.Count != lv.Count)
        {
            currentSpawn.DeleteSpawnByLevel(currentLevelIndex);
            int i = 0;
            foreach (Vector3 v in lv)
            {
                SpawnObject so = new SpawnObject(currentLevelIndex, (int)v.z, i);
                currentSpawn.spawnList.Add(so); //Debug.Log("building spawn list " + so.Team + "," + v.z);
                i++;
            }
        }
    }

    void OpenLevelInfoInput(int id)
    {
        if (id == LEVEL_INFO_NAME)
        {
            abilityInput.SetActive(true);
            aiTitle.text = "Level Name";
            aiInputField.text = "" + currentLevel.GetName(currentLevelIndex);
            aiInputField.gameObject.SetActive(true);
            aiInputField.characterLimit = 30;
            aiInputField.characterValidation = InputField.CharacterValidation.None;
        }
        else
        {
            PopulateInputScrollList(INPUT_LEVEL_INFO, id);
        }
    }

    void OpenAbilityInput(int category, int subCategory)
    {
        if (category == INPUT_DIALOGUE)
        {
            if (subCategory == INPUT_DIALOGUE_DIALOGUE)
            {
                abilityInput.SetActive(true);
                aiTitle.text = "Dialogue";
                aiInputField.text = currentDialogueObject.Dialogue;
                aiInputField.gameObject.SetActive(true);
                aiInputField.characterLimit = 1000;
                aiInputField.characterValidation = InputField.CharacterValidation.None;
            }
        }
        else if( category == INPUT_CAMPAIGN_EDIT)
        {
            if( subCategory == INPUT_CAMPAIGN_NAME)
            {
                abilityInput.SetActive(true);
                aiTitle.text = "Campaign Name";
                aiInputField.text = currentCampaign.CampaignName;
                aiInputField.gameObject.SetActive(true);
                aiInputField.characterLimit = 50;
                aiInputField.characterValidation = InputField.CharacterValidation.None;

            }
        }
        else if (category == INPUT_LEVEL_INFO)
        {
            if (subCategory == INPUT_BATTLE_XP)
            {
                abilityInput.SetActive(true);
                aiTitle.text = "Battle XP";
                aiInputField.text = "" + currentLevel.GetBattleXP(currentLevelIndex);
                aiInputField.gameObject.SetActive(true);
                aiInputField.characterLimit = 5;
                aiInputField.characterValidation = InputField.CharacterValidation.Integer;

            }
            else if (subCategory == INPUT_BATTLE_AP)
            {
                abilityInput.SetActive(true);
                aiTitle.text = "Battle AP";
                aiInputField.text = "" + currentLevel.GetBattleXP(currentLevelIndex);
                aiInputField.gameObject.SetActive(true);
                aiInputField.characterLimit = 5;
                aiInputField.characterValidation = InputField.CharacterValidation.Integer;

            }
        }
    }
    #endregion


    #region OnClickButtons

    public void OnClickLevels()
    {
        PopulateLevelScrollList();
    }

    public void OnClickLevelInfo()
    {
        PopulateLevelInfoScrollList();
    }

    public void OnClickDialogue()
    {
        PopulateDialogueScrollList();
    }

    public void OnClickSpawns()
    {
        PopulateSpawnScrollList();
    }
    
    //scrollist is populated with loadable objects
    public void OnClickLoad()
    {
        PopulateCampaignScrollList();
    }

    //new class is created and ready to edit
    public void OnClickNew()
    {
        int cIndex = NameAll.CUSTOM_CLASS_ID_START_VALUE;//PlayerPrefs.GetInt(NameAll.PP_CUSTOM_CAMPAIGN_MAX_INDEX, NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE);
        
        for( int i = cIndex; i <= (cIndex + 100); i++)
        {
            string fileName = Application.dataPath + "/Custom/Campaigns/Custom/" + i + "_campaign.dat";
            if (!File.Exists(fileName))
            {
                cIndex = i;
                break;
            }
        }

        NewCampaign(cIndex);
        OnClickLoad();
    }

    public void OnClickExit()
    {
        SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
    }
    //saves the current class
    public void OnClickSave()
    {
        string filePath = Application.dataPath + "/Custom/Campaigns/Custom/";
        //save campaign
        Serializer.Save<CampaignCampaign>((filePath + currentCampaign.CampaignId + "_campaign.dat"), currentCampaign);
        //save level info
        Serializer.Save<CampaignLevel>((filePath + currentCampaign.CampaignId + "_level.dat"), currentLevel);
        //save dialogue
        Serializer.Save<CampaignDialogue>((filePath + currentCampaign.CampaignId + "_dialogue.dat"), currentDialogue);
        Serializer.Save<CampaignSpawn>((filePath + currentCampaign.CampaignId + "_spawn.dat"), currentSpawn);


    }

    public void OnClickConfirm()
    {
        abilityInput.SetActive(false);
    }
    #endregion


    #region input fields
    void AddInputFieldListener()
    {
        InputField.SubmitEvent aiInputEvent = new InputField.SubmitEvent();
        aiInputEvent.AddListener(OnSubmitAIInput);
        aiInputField.onEndEdit = aiInputEvent;
        //aiInputField.characterValidation = InputField.CharacterValidation.Alphanumeric;
    }

    //input field has been changed, updates the current value and the value stored in the current ceObject
    void OnSubmitAIInput(string zString)
    {
        //Debug.Log(zString + currentInput + currentSubCategory);
        if( currentInput == INPUT_LEVEL_INFO)
        {
            if( currentSubCategory == LEVEL_INFO_NAME)
            {
                currentLevel.SetName(currentLevelIndex, zString);
                aiInputField.text = zString;
                OnClickLevelInfo();
            }
         
        }
        else if( currentInput == INPUT_DIALOGUE_2)
        {
            if( currentSubCategory == INPUT_DIALOGUE_DIALOGUE)
            {
                currentDialogue.UpdateDialogue(currentDialogueObject, zString);
                aiInputField.text = zString;
                OnClickDialogue();
            }
        }
        else if( currentInput == INPUT_CAMPAIGN_EDIT)
        {
            if(currentSubCategory == INPUT_CAMPAIGN_NAME)
            {
                currentCampaign.CampaignName = zString;
                aiInputField.text = zString;
                campaignHeader.text = "Campaign: " + zString;
                OnClickLoad();
            }
        }
        else if (currentInput == INPUT_LEVEL_INFO)
        {
            if (currentSubCategory == INPUT_BATTLE_XP)
            {
                int z1 = Int32.Parse(zString);
                if (z1 < 0)
                    z1 = 0;
                currentLevel.SetBattleXP(currentLevelIndex, z1);
                aiInputField.text = zString;
                OnClickLevelInfo();
            }
            else if (currentSubCategory == INPUT_BATTLE_AP)
            {
                int z1 = Int32.Parse(zString);
                if (z1 < 0)
                    z1 = 0;
                currentLevel.SetBattleAP(currentLevelIndex, z1);
                aiInputField.text = zString;
                OnClickLevelInfo();
            }
        }

    }

    //closes the panel, reloads the OnClickEdit list (could simplify it so only the new field changes)
    public void OnInputPanelClose()
    {
        abilityInput.SetActive(false);
        //OnClickEdit();
    }

    string GetMapStringFromDict(int key)
    {
        try
        {
            return levelDict[key].levelName;
        }
        catch( Exception e)
        {
            return "error, level not found";
        }
    }

    List<Vector3> GetSpawnPointsFromDict(int key)
    {
        try
        {
            var retValue = new List<Vector3>();
            var tempList = levelDict[key].spList;
            foreach (SerializableVector3 sv in tempList)
            {
                retValue.Add(new Vector3(sv.x, sv.y, sv.z)); //Debug.Log(" " + sv.x +" " + sv.y + " " + sv.z);
            }
            return retValue;
        }
        catch (Exception e)
        {
            return new List<Vector3>();
        }
    }

    string GetPlayerUnitString(int id, bool IsShortText = false)
    {
       
        if( id == NameAll.CAMPAIGN_SPAWN_UNIT_NULL)
        {
            if(IsShortText)
            {
                return "null";
            }
            return "null (this unit cannot be randomly selected)";
        }
        else if( id == NameAll.CAMPAIGN_SPAWN_UNIT_NONE)
        {
            if (IsShortText)
            {
                return "none";
            }
            return "none (chance no unit will appear)";
        }

        try
        {
            PlayerUnit pu = null;
            if( currentCampaign.Version == NameAll.VERSION_CLASSIC)
            {
                foreach(PlayerUnit p in puListClassic)
                {
                    if(p.TurnOrder == id)
                    {
                        pu = p;
                        break;
                    }
                }
            }
            else
            {
                foreach (PlayerUnit p in puListAurelian)
                {
                    if (p.TurnOrder == id)
                    {
                        pu = p;
                        break;
                    }
                }
            }

            string zString = "Current Unit: error, player not found";
            if ( pu != null)
            {
                if (IsShortText)
                {
                    return pu.UnitName + " (" + pu.TurnOrder + ")";
                }
                zString = "Current Unit: " + pu.UnitName + " (" + pu.TurnOrder + ") " +
                        "Lvl: " + pu.Level + " " + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY, pu.ClassId)
                        + "/" + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_SECONDARY, pu.AbilitySecondaryCode);
            }
            return zString;
            //abo = new AbilityBuilderObject("Unit: " + pu.UnitName + "(" + pu.TurnOrder + ")",
            //            "Lvl: " + pu.Level + " " + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY, pu.ClassId)
            //            + "/" + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_SECONDARY, pu.AbilitySecondaryCode),
            //            pu.TurnOrder);
            //var tempList = levelDict[key].spList;
            //foreach (SerializableVector3 sv in tempList)
            //{
            //    retValue.Add(new Vector3(sv.x, sv.y, sv.z)); Debug.Log(" " + sv.x + " " + sv.y + " " + sv.z);
            //}
            //return retValue;
        }
        catch (Exception e)
        {
            return "error";
        }

    }

    #endregion


}

