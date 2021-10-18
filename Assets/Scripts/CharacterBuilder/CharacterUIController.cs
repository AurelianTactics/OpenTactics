using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

//main script that runs the CharacterBuilderScene
//handles a PlayerUnit (pu) that is modified by the various menus including ItemScrollList, AbilityScrollList, and CharacterClassPopup
//handles menu buttons for save, new, load, options, and a list of available units
//units for campaigns are silo'd into their own lists that can be loaded

public class CharacterUIController : MonoBehaviour {

    #region variables
    public Button charClassButton;
    public Button charAbilityButton;
    public Button charItemButton;
    public Button optionsButton;
    public Button randomButton;
    public Button newButton;
    public Button saveButton;

    public GameObject abilityGameObject;
    public GameObject itemGameObject;
    public GameObject classGameObject;

    [SerializeField]
    private CharacterClassPopup popupClass;
    [SerializeField]
    private AbilityScrollList popupAbility;
    [SerializeField]
    private ItemScrollList popupItem;
    [SerializeField]
    private UIUnitInfoPanel statPanel;

    int modVersion;

    GameObject playerUnitObject;

    public static PlayerUnit pu;

    //options scroll lsit
    public GameObject OptionsPanel;
    public Transform optionsContentPanel;
    public GameObject sampleButton;
    static readonly int OPTIONS_LIST_DEFAULT = 1;
    static readonly int OPTIONS_SELECT_VERSION = 14;
    static readonly int OPTIONS_SELECT_DIRECTORY = 13;
    static readonly int OPTIONS_SELECT_DELETE = 12;

    int currentOptionsType = OPTIONS_LIST_DEFAULT;
    string currentUnitDirectoryName = "";
    string currentUnitDirectoryPath;
    int currentSaveUnitId = NameAll.NULL_UNIT_ID; //ie either save to existing file or create a new one
    Dictionary<int, PlayerUnit> unitDict;
    bool currentIsCampaign = false;
    int currentCampaignId = 0;

    const string CharacterBuilderNotification = "CharacterBuilderNotification";
    const string EquipAbilityNotification = "EquipAbilityNotification";
    //const string MiscUnitNotification = "MiscUnitNotification";

    StorySave currentStorySave = null; //used when going between this scene and story mode scene
    #endregion

    #region monobehaviour
    void Awake()
    {
        //Debug.Log("testing awake");
        modVersion = PlayerPrefs.GetInt(NameAll.PP_MOD_VERSION, NameAll.VERSION_AURELIAN);
        ItemManager.Instance.SetIoType(NameAll.ITEM_MANAGER_SIMPLE); //lets the item button load the lists
        AbilityManager.Instance.SetIoType(NameAll.ITEM_MANAGER_SIMPLE);
    }

    void Start()
    {
        //Debug.Log("testing start");
        
        int entryInt = PlayerPrefs.GetInt(NameAll.PP_EDIT_UNIT_ENTRY,NameAll.SCENE_MAIN_MENU);
        //if ( entryInt == NameAll.SCENE_STORY_MODE)
        //{
        //    SetMenuButtons(entryInt);
        //    currentStorySave = CalcCode.LoadStorySave(NameAll.NULL_INT, true);

        //    unitListText.text = "Units";
        //    unitDict = currentStorySave.GetPlayerUnitDict();
        //    PopulateUnitScrollList();
        //    SetCurrentUnit();
        //}
        //else
        //{
            SetDirectoryDefault();

            //allows entering and exiting from CustomGame scene, not currently implemented
            int z1 = PlayerPrefs.GetInt(NameAll.PP_EDIT_UNIT_ENTRY, NameAll.SCENE_MAIN_MENU);
            if (z1 == NameAll.SCENE_CUSTOM_GAME)
            {
                PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_EDIT_UNIT);
                //go = GameObject.Find("CustomGameController");
                //go.SetActive(false);
            }
        //}        
    }

    void SetMenuButtons(int entryInt)
    {
        if( entryInt == NameAll.SCENE_STORY_MODE)
        {
            charClassButton.gameObject.SetActive(false);
            optionsButton.gameObject.SetActive(false);
            randomButton.gameObject.SetActive(false);
            newButton.gameObject.SetActive(false);
            saveButton.gameObject.SetActive(false);
            //charAbilityButton;
            //charItemButton;
            //exit button always there
        }
    }

    //unit saved in default classic/aurelian directory or a custom campaign directory
    void SetDirectoryDefault()
    {
        if (modVersion == NameAll.VERSION_CLASSIC)
        {
            currentUnitDirectoryName = "Default Classic Directory";
            currentUnitDirectoryPath = Application.dataPath + "/Custom/Units/Classic/";
        }
        else
        {
            currentUnitDirectoryName = "Default Aurelian Directory";
            currentUnitDirectoryPath = Application.dataPath + "/Custom/Units/Aurelian/";
        }
        unitListText.text = currentUnitDirectoryName;
        unitDict = CalcCode.LoadPlayerUnitDict(modVersion, false, 0);
        PopulateUnitScrollList();
        SetCurrentUnit();
    }
    #endregion

    #region notifications
    //called from CharacterClassPopup, ItemScrollList and AbilityScrollList on pu change
    void OnEnable()
    {
        this.AddObserver(OnCharacterBuilderNotification, CharacterBuilderNotification);
        this.AddObserver(OnEquipAbilityNotification, EquipAbilityNotification);
        //this.AddObserver(OnMiscUnitNotification, MiscUnitNotification);
    }

    void OnDisable()
    {
        this.RemoveObserver(OnCharacterBuilderNotification, CharacterBuilderNotification);
        this.RemoveObserver(OnEquipAbilityNotification, EquipAbilityNotification);
        //this.RemoveObserver(OnMiscUnitNotification, MiscUnitNotification);
    }


    void OnCharacterBuilderNotification(object sender, object args)
    {
        SetStatPanel();
    }

    void OnEquipAbilityNotification(object sender, object args)
    {
        var tempList = args as List<int>;
        int slot = tempList[0];
        int index = tempList[1];
        if (slot == NameAll.ABILITY_SLOT_MOVEMENT)
            pu.EquipMovementAbility(index);
        else if (slot == NameAll.ABILITY_SLOT_SECONDARY)
            pu.AbilitySecondaryCode = index;
        else if (slot == NameAll.ABILITY_SLOT_REACTION)
            pu.AbilityReactionCode = index;
        else if (slot == NameAll.ABILITY_SLOT_SUPPORT)
            pu.EquipSupportAbility(index);
        else if (slot == NameAll.ABILITY_SLOT_PRIMARY)
            pu.SetClassIdStatsUnequip(index);
        SetStatPanel();
    }

    void OnMiscUnitNotification(object sender, object args)
    {
		print("misc notification args are " + args);
        PlayerUnit newPU = args as PlayerUnit;
        pu = newPU;
        
        SetStatPanel();
    }
    #endregion

    #region onClick menubuttons
    public void ExitScene()
    {
        int z1 = PlayerPrefs.GetInt(NameAll.PP_EDIT_UNIT_ENTRY, NameAll.SCENE_MAIN_MENU);
        if( z1 == NameAll.SCENE_CUSTOM_GAME)
        {
            PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_EDIT_UNIT);
            //go.SetActive(true);
        }
        //else if( z1 == NameAll.SCENE_STORY_MODE)
        //{
        //    //save the changes to the unit before going back to teh story
        //    currentStorySave.UnitDictToList(unitDict);
        //    CalcCode.SaveTempStorySave(currentStorySave);
        //}
        SceneManager.LoadScene(z1);
    }

    public void OnClickClass()
    {
        //Debug.Log("Asdf");
        if (classGameObject.activeSelf)
        {
            popupClass.Close();
        }
        else
        {
            ClosePopups("class");
            popupClass.Open();
        }
    }


    public void OnClickAbility()
    {
        if (abilityGameObject.activeSelf)
        {
            popupAbility.Close();
        }
        else
        {
            ClosePopups("ability");
            popupAbility.Open(pu);
        }
    }

    public void OnClickItem()
    {
        if (itemGameObject.activeSelf)
        {
            popupItem.Close();
        }
        else
        {
            //Debug.Log("Asdf");
            popupItem.Open(pu);
            //Debug.Log("Asdf");
            ClosePopups("item");
        }
    }

    public void OnClickSave()
    {
        SaveCurrentUnit(currentIsCampaign,currentCampaignId);
        try
        {
            unitDict[currentSaveUnitId] = pu; //= CalcCode.LoadPlayerUnitDict(modVersion, false, 0);
        }
        catch (Exception e)
        {

        }
        PopulateUnitScrollList();
        
    }

    public void OnClickNew()
    {
        ClosePopups();
        pu = new PlayerUnit(modVersion); //Debug.Log("creating new pu");
        SetUpPlayerManager();
        SetStatPanel();
        SetUpPlayerManager();
        currentSaveUnitId = NameAll.NULL_UNIT_ID;
    }

    public void OnClickRandom()
    {
        ClosePopups();
        pu = new PlayerUnit(modVersion,true); //Debug.Log("creating new pu");
        SetUpPlayerManager();
        SetStatPanel();
        SetUpPlayerManager();
        currentSaveUnitId = NameAll.NULL_UNIT_ID;
    }
    #endregion

    #region convenience functions
    //sets up the playermanager to manage the current pu and puo
    private void SetUpPlayerManager()
    {
        PlayerManager.Instance.ClearPlayerLists();
        PlayerManager.Instance.AddPlayerUnit(pu);
        PlayerManager.Instance.AddPlayerObject(playerUnitObject);
    }

    public void SetStatPanel()
    {
        LoadPUO();
        statPanel.Open(true);//disables close
        statPanel.PopulatePlayerInfo(pu, false);

    }

    void CheckVersionChanged()
    {
        int z1 = PlayerPrefs.GetInt(NameAll.PP_MOD_VERSION, NameAll.VERSION_AURELIAN);
        if( z1 != modVersion)
        {
            modVersion = z1;
            AbilityManager.Instance.SetIoType(NameAll.ITEM_MANAGER_SIMPLE);
            ItemManager.Instance.SetIoType(NameAll.ITEM_MANAGER_SIMPLE);
            ClosePopups();
        }
    }

    void ClosePopups( string zString = "all")
    {
        if( zString == "all")
        {
            popupClass.Close();
            popupAbility.Close();
            popupItem.Close();
            OptionsPanel.SetActive(false);
        }
        else if( zString == "ability")
        {
            popupClass.Close();
            //popupAbility.Close();
            popupItem.Close();
            OptionsPanel.SetActive(false);
        }
        else if( zString == "item")
        {
            //Debug.Log("Asdf");
            //popupClass.Close();
            classGameObject.SetActive(false);
            popupAbility.Close();
            //popupItem.Close();
            OptionsPanel.SetActive(false);
        }
        else if( zString == "options")
        {
            popupClass.Close();
            popupAbility.Close();
            popupItem.Close();
            //OptionsPanel.SetActive(false);
        }
        else if( zString == "class")
        {
            //popupClass.Close();
            popupAbility.Close();
            popupItem.Close();
            OptionsPanel.SetActive(false);
        }
        else
        {
            popupClass.Close();
            popupAbility.Close();
            popupItem.Close();
            OptionsPanel.SetActive(false);
        }
    }

    void LoadPUO()
    {
        Destroy(playerUnitObject);
        string puoString = NameAll.GetPUOString(pu.ClassId); //Debug.Log("asdf " + puoString);
        playerUnitObject = Instantiate(Resources.Load(puoString)) as GameObject;
        playerUnitObject.SetActive(true);
        SetUpPlayerManager();
        StatusManager.Instance.GenerateAllStatusLasting(); //creates the lastign statuses from items and abilities at the beginning of the round
    }

    //set the current unit, lets it know if a save should be to a new file or existing
    void SetCurrentUnit()
    {
        if (unitDict.Count > 0)
        {
            foreach (KeyValuePair<int, PlayerUnit> kvp in unitDict)
            {
                pu = kvp.Value;
                currentSaveUnitId = kvp.Key;
                break;
            }
            SetStatPanel();
        }
        else
        {
            OnClickNew();
        }
    }

    void SaveCurrentUnit(bool isCampaign, int campaignId)
    {
        bool isReloadDict = false;
        if (currentSaveUnitId == NameAll.NULL_UNIT_ID)
        {
            currentSaveUnitId = FindAvailableSaveId();
            isReloadDict = true;
        }
        string savePath = currentUnitDirectoryPath + "unit_" + currentSaveUnitId + ".dat";
        Serializer.Save<PlayerUnit>(savePath, pu);
        
        if (isReloadDict)
        {
            unitDict = CalcCode.LoadPlayerUnitDict(modVersion, isCampaign, campaignId);
            PopulateUnitScrollList();
        }
    }

    //when saving a unit that hasn't been saved before, finds the first available slot
    int FindAvailableSaveId()
    {
        for (int i = 0; i <= 100; i++)
        {
            string filePath = currentUnitDirectoryPath + "unit_" + i + ".dat";
            if (!File.Exists(filePath))
            {
                return i;
            }
        }
        return 100;
    }
    #endregion

    #region OptionsPanel

    public void OnOptionsButtonClick()
    {
        if (OptionsPanel.activeSelf)
        {
            OptionsPanel.gameObject.SetActive(false);
        }
        else
        {
            ClosePopups("options");
            OptionsPanel.gameObject.SetActive(true);
            PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);
        }
    }

    void PopulateOptionsScrollList(int type)
    {
        foreach (Transform child in optionsContentPanel)
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
            tb.transform.SetParent(optionsContentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonOptionsClicked(tempInt));
        }
    }

    List<AbilityBuilderObject> BuildOptionsList(int type)
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;
        currentOptionsType = type;

        if (type == OPTIONS_LIST_DEFAULT)
        {
            abo = new AbilityBuilderObject("Change Version", CalcCode.GetVersionFromInt(modVersion), OPTIONS_SELECT_VERSION);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Change Unit Save Directory", "Current Directory is " + currentUnitDirectoryName, OPTIONS_SELECT_DIRECTORY);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Delete Current Unit", "Permanently Delete " + CalcCode.GetTitleFromPlayerUnit(pu), OPTIONS_SELECT_DELETE);
            retValue.Add(abo);
        }
        else if (type == OPTIONS_SELECT_DIRECTORY)
        {
            abo = new AbilityBuilderObject("No Campaign", "Saves to default directory", NameAll.NULL_UNIT_ID);
            retValue.Add(abo);
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
            if ( retValue.Count == 1)
            {
                abo = new AbilityBuilderObject("No Campaigns Found", "Use the campaign editor to create campaigns. Save units to the campaign here", OPTIONS_LIST_DEFAULT);
                retValue.Add(abo);
            }
        }
        else if (type == OPTIONS_SELECT_VERSION)
        {
            abo = new AbilityBuilderObject("Version: Aurelian", "", NameAll.VERSION_AURELIAN);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Version: Classic", "", NameAll.VERSION_CLASSIC);
            retValue.Add(abo);
        }

        return retValue;
    }

    void ButtonOptionsClicked(int select)
    {
        //if( select >= OPTIONS_SELECT_VC && select <= OPTIONS_SELECT_VERSION)
        //{
        //    PopulateOptionsScrollList(select);
        //}
        if (currentOptionsType == OPTIONS_LIST_DEFAULT)
        {
            if( select == OPTIONS_SELECT_DELETE)
            {
                DeleteUnit();
                PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);
            }
            else
            {
                PopulateOptionsScrollList(select);
            }
            
        }
        else if (currentOptionsType == OPTIONS_SELECT_VERSION)
        {
            currentIsCampaign = false;
            currentCampaignId = 0;
            //modVersion = select; //DON"T DO THIS HERE DO THIS IN CHECK VERSION CHANGED
            PlayerPrefs.SetInt(NameAll.PP_MOD_VERSION, select);
            CheckVersionChanged();
            SetDirectoryDefault();
            PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);
            //Debug.Log("probably need to change directory here if not tied to a campaign");
        }
        else if (currentOptionsType == OPTIONS_SELECT_DIRECTORY)
        {
            if( select == NameAll.NULL_UNIT_ID)
            {
                currentIsCampaign = false;
                currentCampaignId = 0;
                SetDirectoryDefault();
                PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);
                return;
            }

            ChangeCurrentCampaign(select);

            PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);
        }
    }

    //campaigns each have their own list of units
    void ChangeCurrentCampaign(int campaignId)
    {
        //Debug.Log("changing current campaign");

        CampaignCampaign cc = CalcCode.LoadCampaignCampaign(campaignId);
        if (modVersion != cc.Version)
        {
            //DON"T DO THIS HERE modVersion = select;
            PlayerPrefs.SetInt(NameAll.PP_MOD_VERSION, campaignId);
            CheckVersionChanged();
        }
        currentUnitDirectoryName = "Directory for " + cc.CampaignName;
        unitListText.text = currentUnitDirectoryName;

        currentIsCampaign = true;
        currentCampaignId = campaignId;
        //sees if directory exists and if not creates it
        string campaignFilePath = Application.dataPath + "/Custom/Units/Campaign_" + campaignId + "/";
        if (!Directory.Exists(campaignFilePath))
            Directory.CreateDirectory(campaignFilePath);
        //changes the saveDirectory
        currentUnitDirectoryPath = campaignFilePath;

        //loads the dictionary of current units
        unitDict = CalcCode.LoadPlayerUnitDict(modVersion, true, campaignId);
        PopulateUnitScrollList();

        //load a unit if there is one
        SetCurrentUnit();
    }

    void DeleteUnit()
    {
        if (currentSaveUnitId == NameAll.NULL_UNIT_ID)
            return;

        string filePath = currentUnitDirectoryPath + "unit_" + currentSaveUnitId + ".dat";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
    #endregion

    #region UnitScrollList
    public GameObject UnitScrollListPanel;
    public Transform unitListContentPanel;
    public Text unitListText;

    void PopulateUnitScrollList()
    {
        if (!UnitScrollListPanel.activeSelf)
            UnitScrollListPanel.SetActive(true);

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
        foreach(KeyValuePair<int,PlayerUnit> kvp in unitDict)
        {
            abo = new AbilityBuilderObject(CalcCode.GetTitleFromPlayerUnit(kvp.Value), CalcCode.GetDetailsFromPlayerUnit(kvp.Value), kvp.Key);
            retValue.Add(abo);
        }

        return retValue;
    }

    void ButtonUnitClicked(int select)
    {
        ClosePopups();
        currentSaveUnitId = select;
        pu = unitDict[select];
        SetStatPanel();
    }
    #endregion

}
