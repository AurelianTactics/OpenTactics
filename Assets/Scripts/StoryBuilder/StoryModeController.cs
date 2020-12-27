using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryModeController : MonoBehaviour {

    StoryObject currentStory;
    StoryPoint currentStoryPoint;
    StoryCutScene currentStoryCutScene;
    StorySave currentStorySave;
    StoryPointInt currentStoryPointInt;

    static readonly int SCROLL_LIST_LOAD = 1;
    static readonly int SCROLL_LIST_NEW = 2;
    static readonly int SCROLL_LIST_OPTIONS = 3;
    static readonly int INPUT_OPTIONS_SKIP = 4;
    static readonly int SCROLL_LIST_PARTY_UNIT = 5;
    static readonly int SCROLL_LIST_SELECTED_UNIT = 6;
    static readonly int INPUT_NEW_SAVE = -1;

    //loadScrollList
    public GameObject scrollList;
    public Transform contentPanel;
    public GameObject scrollListButton;

    public Image mapImage;
    public GameObject mapPointPanel; //contains button and label. when button clicked story menu opens
    float mapImageWidth; //used to scale where map points are
    float mapImageHeight;
    List<GameObject> mapPointPanelList; //holds the mapPoints

    //story map menu
    public GameObject storyMapMenu;
    public Text storyMapMenuHeader;
    public Button storyMapMenuPartyButton;
    public Button storyMapMenuCutSceneButton;
    public Button storyMapMenuBattleButton;
    public Button storyMapMenuShopButton;

    //menu panel
    public Button saveButton;
    public Button loadButton;
    public Button newGameButton;
    public Button optionsButton;
    public Button skipButton;
    bool enableSkipMode; //for testing purposes a skip button is added to menu to skip cutscenes/battles to see how the story progresses

    //unit lists for starting battle
    public GameObject partyUnitScrollList;
    public Transform partyUnitScrollListContentPanel;
    public GameObject selectedUnitScrollList;
    public Transform selectedUnitScrollListContentPanel;
    public Text selectedUnitScrollListText;
    public GameObject unitScrollListButton;
    List<PlayerUnit> selectedUnitList; //needed for playermanager for next scene;
    List<PlayerUnit> partyUnitList;

    
    #region set up functions
    // Use this for initialization
    void Start () {

        mapPointPanelList = new List<GameObject>();
        selectedUnitList = new List<PlayerUnit>();
        SetCurrentToNull();

        mapImageWidth = mapImage.GetComponent<RectTransform>().rect.width;
        mapImageHeight = mapImage.GetComponent<RectTransform>().rect.height;

        

        int entryId = PlayerPrefs.GetInt(NameAll.PP_STORY_MODE_ENTRY, NameAll.SCENE_MAIN_MENU);
        if ( entryId == NameAll.SCENE_EDIT_UNIT || entryId == NameAll.SCENE_STORY_SHOP)
        {
            //Debug.Log("loading from edit unit");
            //on exit, saves any edits to the temp save
            LoadStory(NameAll.NULL_INT, NameAll.NULL_INT, false, true, NameAll.NULL_INT);
        }
        else if( entryId == NameAll.SCENE_CUT_SCENE)
        {
            LoadStory(NameAll.NULL_INT, NameAll.NULL_INT, false, true, PlayerPrefs.GetInt(NameAll.PP_PROGRESSION_INT, NameAll.NULL_INT));
        }
        else if( entryId == NameAll.SCENE_COMBAT)
        {
            LoadStory(NameAll.NULL_INT, NameAll.NULL_INT, false, true, PlayerPrefs.GetInt(NameAll.PP_PROGRESSION_INT, NameAll.NULL_INT));
        }
        else
        {
            OnGameStartFromMenu();
        }

        if (currentStory != null)
            enableSkipMode = currentStory.EnableSkipMode;
        else
            enableSkipMode = false;
        ToggleSkipMode(enableSkipMode);

        //partyUnitList = new List<PlayerUnit>();
    }


    void SetCurrentToNull()
    {
        currentStory = null;
        currentStoryPoint = null;
        currentStoryCutScene = null;
        currentStorySave = null;
        currentStoryPointInt = null;
    }

    void ToggleSkipMode(bool isEnabled)
    {
        skipButton.gameObject.SetActive(isEnabled);
    }

    void OnGameStartFromMenu()
    {
        mapImage.gameObject.SetActive(false);
        scrollList.SetActive(false); //let user click new or load
        saveButton.gameObject.SetActive(false);
        optionsButton.gameObject.SetActive(false);
        loadButton.gameObject.SetActive(true);
        newGameButton.gameObject.SetActive(true);

    }

    void OnGameLoad()
    {
        scrollList.SetActive(false); //not needed, closes after input
        saveButton.gameObject.SetActive(true);
        optionsButton.gameObject.SetActive(true);
        loadButton.gameObject.SetActive(true);
        newGameButton.gameObject.SetActive(false);

        enableSkipMode = false;
        ToggleSkipMode(enableSkipMode);
        SetCurrentToNull();
    }

    #endregion

    #region loading saves
    //loads the story
    //from either a tempsave (save used when going between scenes like StoryMode -> Combat -> Story Mode
    //or from a save file (after clicking a load button)
    //or creating a new game (so new story save)
    void LoadStory(int storyId, int saveId, bool isNewGame, bool isTempSave, int progressionInt )
    {
        OnGameLoad();
        
        if (isTempSave)
        {
            currentStorySave = CalcCode.LoadStorySave(NameAll.NULL_INT, true);
            LoadStoryObjectsFromStorySave();
            if( progressionInt != NameAll.NULL_INT) 
            {
                currentStorySave.StoryInt = currentStory.GetNewStoryInt(currentStorySave.StoryInt, progressionInt);
            }
                
        }
        else
        {
            if (isNewGame)
            {
                currentStory = CalcCode.LoadStoryObject(storyId);
                currentStoryPoint = CalcCode.LoadStoryPoint(storyId);
                currentStoryCutScene = CalcCode.LoadStoryCutScene(storyId);
                currentStorySave = new StorySave(currentStory.StoryId, 0, 0, currentStoryPoint.storyPointObjectList, currentStory.startingPlayerUnitList);
                currentStorySave.StoryInt = 0;
            }
            else
            {
                currentStorySave = CalcCode.LoadStorySave(saveId, false);
                LoadStoryObjectsFromStorySave();
            }
        }
        

        SetMapImage();

        OnStorySaveLoad(currentStorySave);

        //scrollList.gameObject.SetActive(false); //done previously
    }

    void LoadStoryObjectsFromStorySave()
    {
        if (currentStorySave != null)
        {
            currentStory = CalcCode.LoadStoryObject(currentStorySave.StoryId);
            currentStoryPoint = CalcCode.LoadStoryPoint(currentStorySave.StoryId);
            currentStoryCutScene = CalcCode.LoadStoryCutScene(currentStorySave.StoryId);
        }
        else
        {
            Debug.Log("ERROR: couldn't find story save at that position");
            OnGameStartFromMenu();
        }
    }

    //story save loaded, show/hide the points
    //for each storypointobject, find the largest spi <= current story int
    //called on loading a story save (which happens on all progressions) or when clicking skip button
    void OnStorySaveLoad(StorySave ss)
    {
        ClearMapPointsList();
        //can be called from skip, so need to deactivate these
        currentStoryPointInt = null;

        storyMapMenu.gameObject.SetActive(false);

        int storyInt = ss.StoryInt;
        foreach( StoryPointObject spo in ss.storyPointObjectList)
        {
            StoryPointInt s = null; //Debug.Log("cycling through spo " + spo.PointId);
            foreach(StoryPointInt spi in spo.storyIntList)
            {
                //Debug.Log("cycling through spi " + spi.StoryPointIntId + " " + spi.StoryInt + " " + storyInt);
                if ( spi.StoryInt <= storyInt)
                {
                    if (s == null || s.StoryInt < spi.StoryInt)
                        s = spi;
                }
            }
            if (s != null)
                SetStoryPointObject(spo,s);
        }
    }

    //decides to show or not show the storypointobject on the maps
    void SetStoryPointObject(StoryPointObject spo, StoryPointInt spi)
    {
        //Debug.Log("showing storypoint object" + spo.PointId + " " + spi.StoryPointIntId);
        if(spi.IsShown)
        {
            //Debug.Log("showing storypoint object" + spo.PointId + " " + spi.StoryPointIntId);
            SetMapPointGameObject(spo, spi);
        }
        else
        {
            //hide it. since loaded from file don't think I need to hide it as they all start hidden
        }
    }

    //map point is being shown. display the point and enable its functionality when it's clicked
    void SetMapPointGameObject(StoryPointObject spo, StoryPointInt spi)
    {
        //if this doesn't work, try this http://gamedev.stackexchange.com/questions/103718/what-are-the-steps-to-instantiate-a-button-in-unity
        GameObject mapPointPanelInstance = Instantiate(mapPointPanel);
        mapPointPanelList.Add(mapPointPanelInstance);
        mapPointPanelInstance.transform.SetParent(mapImage.transform);
        //mapPointGameObject.GetComponentInChildren<Text>().text = currentStoryPoint.GetPointName(pointId);
        mapPointPanelInstance.GetComponentInChildren<Text>().text = spo.PointName;

        Button pointButton = mapPointPanelInstance.GetComponentInChildren<Button>();

        //0,0 is the mid point of the mapImage
        float x = ((float)(spo.ScreenX-50)) / 100.00f * mapImageWidth; //pointX and pointY are scaled
        float y = ((float)(spo.ScreenY-50)) / 100.00f * mapImageHeight;
        RectTransform trans = mapPointPanelInstance.GetComponent<RectTransform>();
        trans.localPosition = new Vector3(x, y, trans.localPosition.z);
        //if (spo == null)
        //    Debug.Log("spo is null");
        //if (spi == null)
        //    Debug.Log("spi is null");
        pointButton.onClick.AddListener(() => SetMapMenu(spo,spi));
    }

    //a map point has been clicked, show the menu to handle more user input
    void SetMapMenu(StoryPointObject spo, StoryPointInt spi)
    {
        selectedUnitList.Clear();
        currentStorySave.PointId = spo.PointId; 
        currentStoryPointInt = spi;
        
        storyMapMenu.SetActive(true);
        storyMapMenuHeader.text = spo.PointName;
        storyMapMenuPartyButton.gameObject.SetActive(true);

        if (spi.IsHasCutScene)
        {
            storyMapMenuCutSceneButton.gameObject.SetActive(true);
            //add view cut scene
        }
        else
            storyMapMenuCutSceneButton.gameObject.SetActive(false);

        if (spi.IsHasStoryBattle)
        {
            storyMapMenuBattleButton.gameObject.SetActive(true);
            //add start battle
        }
        else
            storyMapMenuBattleButton.gameObject.SetActive(false);

        if (spi.IsHasShop)
            storyMapMenuShopButton.gameObject.SetActive(true);
        else
            storyMapMenuShopButton.gameObject.SetActive(false);
    }

    //show the map image
    void SetMapImage()
    {
        mapImage.gameObject.SetActive(true);
        mapImage.sprite = Resources.Load<Sprite>("MapImages/story_map_" + currentStory.MapId);
    }
    #endregion

    #region OnClickMenu
    //main menu clicks
    public void OnClickSave()
    {
        CalcCode.SaveStorySave(false, currentStorySave);
    }

    public void OnClickLoad()
    {
        if (scrollList.activeSelf)
            scrollList.SetActive(false);
        else
            PopulateScrollList(SCROLL_LIST_LOAD);
    }

    public void OnClickNewGame()
    {
        if (scrollList.activeSelf)
            scrollList.SetActive(false);
        else
            PopulateScrollList(SCROLL_LIST_NEW);
    }

    public void OnClickExit()
    {
        SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
    }

    public void OnClickOptions()
    {
        if (scrollList.activeSelf)
            scrollList.SetActive(false);
        else
            PopulateScrollList(SCROLL_LIST_OPTIONS);
    }

    public void OnClickSkip()
    {
        //Debug.Log("clicking on onclick skip outer");
        if( currentStoryPointInt != null)
        {
            //Debug.Log("currentStory save details " + currentStorySave.StoryInt + " " + currentStoryPointInt.StoryInt + " " + currentStoryPointInt.StoryPointId + " " + currentStoryPointInt.IsHasCutScene + " " + currentStoryPointInt.IsHasStoryBattle);

            if (currentStoryPointInt.IsHasCutScene)
            {
                InnerOnClickCutScene(); //Debug.Log("has cut scene");
            }
            else if (currentStoryPointInt.IsHasStoryBattle)
            {
                InnerOnClickBattle(); //Debug.Log("has battle ");
            }

            //clears the map points. fine in non-skip since they are cleared when the scene is deleted
            ClearMapPointsList();
            //Debug.Log("progression int is " + PlayerPrefs.GetInt(NameAll.PP_PROGRESSION_INT, NameAll.NULL_INT));
            LoadStory(NameAll.NULL_INT, NameAll.NULL_INT, false, true, PlayerPrefs.GetInt(NameAll.PP_PROGRESSION_INT, NameAll.NULL_INT));

            enableSkipMode = true;
            ToggleSkipMode(enableSkipMode);
            //Debug.Log("currentStoryInt is " + currentStorySave.StoryInt);
        }
    }

    void ClearMapPointsList()
    {
        
        foreach( GameObject go in mapPointPanelList)
        {
            Destroy(go);
        }
    }

    //story map menu shown when story point is clicked
    public void OnClickParty()
    {
        //save current info, launch edit units scene. when returning here, load the tempsave
        CalcCode.SaveTempStorySave(currentStorySave);
        PlayerPrefs.SetInt(NameAll.PP_EDIT_UNIT_ENTRY, NameAll.SCENE_STORY_MODE);
        PlayerPrefs.SetInt(NameAll.PP_MOD_VERSION, currentStory.Version);
        PlayerPrefs.SetInt(NameAll.PP_STORY_MODE_ENTRY, NameAll.SCENE_EDIT_UNIT);
        SceneManager.LoadScene(NameAll.SCENE_EDIT_UNIT);
    }

    public void OnClickShop()
    {
        CalcCode.SaveTempStorySave(currentStorySave);
        SceneManager.LoadScene(NameAll.SCENE_STORY_SHOP);
    }

    public void OnClickStartCutScene()
    {
        if (currentStoryPointInt != null)
        {
            InnerOnClickCutScene();
            SceneManager.LoadScene(NameAll.SCENE_CUT_SCENE);
        }
        else
            Debug.Log("ERROR: couldn't fidn currentStoryPoitnInt");
    }
    
    //also called by skip
    void InnerOnClickCutScene()
    {
        currentStorySave.ConsumeStoryPointInt(currentStoryPointInt, false, true);
        CalcCode.SaveTempStorySave(currentStorySave);

        PlayerPrefs.SetInt(NameAll.PP_CUT_SCENE_STORY_ID, currentStory.StoryId); //needed int SCENE CUT SCENE
        PlayerPrefs.SetInt(NameAll.PP_CUT_SCENE_CUT_SCENE_ID, currentStoryPointInt.CutSceneId); //needed int SCENE CUT SCENE
        PlayerPrefs.SetInt(NameAll.PP_PROGRESSION_INT, currentStoryPointInt.ProgressionIntCutScene);
        PlayerPrefs.SetInt(NameAll.PP_CUT_SCENE_ENTRY, NameAll.SCENE_STORY_MODE);
        PlayerPrefs.SetInt(NameAll.PP_STORY_MODE_ENTRY, NameAll.SCENE_CUT_SCENE);
    }

    public void OnClickStartBattle()
    {
        if (currentStoryPointInt != null)
        {
            
            if (selectedUnitList.Count > 0)
            {
                GameObject pmo = Instantiate(Resources.Load("PlayerManagerObject")) as GameObject;
                DontDestroyOnLoad(pmo);
                PlayerManager.Instance.SetPhotonNetworkOfflineMode(true); //not sure why this is needed but I guess it defaults to online

                InnerOnClickBattle();

                //loading player team
                SpellManager.Instance.SetSpellLearnedType(NameAll.SPELL_LEARNED_TYPE_PLAYER_1);
                foreach (PlayerUnit punit in selectedUnitList)
                {
                    PlayerManager.Instance.EditTeamLists(punit, NameAll.TEAM_ID_GREEN, NameAll.TEAM_LIST_ADD);
                    var tempList = currentStorySave.GetPrimaryAbilityLearnedList(punit.TurnOrder, punit.ClassId, punit.AbilitySecondaryCode);
                    SpellManager.Instance.AddToSpellLearnedList(punit.TurnOrder, tempList);
                }

                //loading enemy team
                CampaignSpawn cs = CalcCode.LoadCampaignSpawn(currentStory.CampaignId);//CalcCode.Load
                if (cs != null)
                {
                    List<SpawnObject> spawnList = cs.GetSpawnListByLevel(currentStoryPointInt.StoryBattleId);
                    List<PlayerUnit> puList = CalcCode.GetPlayerUnitListForCampaign(0, currentStory.CampaignId);
                    foreach (SpawnObject so in spawnList)
                    {
                        PlayerUnit pu = CalcCode.GetPlayerUnitFromSpawnObject(so, puList);
                        if (pu != null && so.Team == NameAll.TEAM_ID_RED)
                        {
                            PlayerManager.Instance.EditTeamLists(pu, so.Team, NameAll.TEAM_LIST_ADD);
                        }
                    }
                    
                    SceneManager.LoadScene(NameAll.SCENE_COMBAT);
                }

                
            }
            else
            {
                selectedUnitList.Clear();
                partyUnitList = new List<PlayerUnit>();
                var unitList = currentStorySave.GetPlayerUnitList();
                for( int i = 0; i < unitList.Count; i++)
                {
                    //currentStorySave.unitList[i].TurnOrder = i; //can't modify the turn order here, stores the unique id. need the turn order for adding abilities
                    partyUnitList.Add(unitList[i]);
                }
                //foreach(PlayerUnit pu in currentStorySave.unitList)
                //{
                //    partyUnitList.Add(pu);
                //} 
                PopulateStartBattleScrollLists();
            }

            
        }
        else
            Debug.Log("ERROR: couldn't fidn currentStoryPoitnInt");
    }

    void InnerOnClickBattle()
    {
        currentStorySave.ConsumeStoryPointInt(currentStoryPointInt, true,false);
        CalcCode.SaveTempStorySave(currentStorySave);

        PlayerPrefs.SetInt(NameAll.PP_PROGRESSION_INT, currentStoryPointInt.ProgressionIntBattle);
        PlayerPrefs.SetInt(NameAll.PP_COMBAT_ENTRY, NameAll.SCENE_STORY_MODE);
        PlayerPrefs.SetInt(NameAll.PP_STORY_MODE_ENTRY, NameAll.SCENE_COMBAT);

        //for combat to load the correct map need to let it know 3 things related to the story's linked CAMPAIGN
        //1: which directory
        if (currentStory.CampaignId >= NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE)
            PlayerPrefs.SetString(NameAll.PP_LEVEL_DIRECTORY, NameAll.LEVEL_DIRECTORY_CAMPAIGN_CUSTOM); //Debug.Log(" directory is " +)
        else
            PlayerPrefs.SetString(NameAll.PP_LEVEL_DIRECTORY, NameAll.LEVEL_DIRECTORY_CAMPAIGN_AURELIAN);
        //2: campaingnLevel
        PlayerPrefs.SetInt(NameAll.PP_COMBAT_LEVEL, currentStoryPointInt.StoryBattleId);
        //3: campaignId
        PlayerPrefs.SetInt(NameAll.PP_COMBAT_CAMPAIGN_LOAD, currentStory.CampaignId);

        //PlayerPrefs.GetString(NameAll.PP_LEVEL_DIRECTORY, NameAll.LEVEL_DIRECTORY_AURELIAN);
    }

    #endregion

    #region scrollList and related functions
    void PopulateScrollList(int panelType)
    {
        scrollList.gameObject.SetActive(true);

        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildStoryPanelList(panelType);
        
        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(scrollListButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel); 

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonStoryPanelClicked(panelType, tempInt));
        }
    }

    List<AbilityBuilderObject> BuildStoryPanelList(int panelType)
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;

        //Debug.Log("new game click: populating options " + panelType);
        if ( panelType == SCROLL_LIST_LOAD)
        {
            if( currentStory != null)
            {
                abo = new AbilityBuilderObject("Create New Save File", "", INPUT_NEW_SAVE);
                retValue.Add(abo);
            }

            var tempList = CalcCode.LoadStorySaveList();
            foreach (StorySave s in tempList)
            {
                StoryObject so = CalcCode.LoadStoryObject(s.StoryId);
                abo = new AbilityBuilderObject("Story: " + so.StoryName, "Load file at: Story #" + s.StoryInt, s.StorySaveId);
                retValue.Add(abo);
            }
        }
        else if( panelType == SCROLL_LIST_NEW)
        {
            //start a new game
            var tempList = CalcCode.LoadStoryObjectList(); //Debug.Log("new game click: populating options " + tempList.Count);
            foreach (StoryObject so in tempList)
            {
                abo = new AbilityBuilderObject(so.StoryName, "Start new game", so.StoryId);
                retValue.Add(abo);
            }
        }
        else if( panelType == SCROLL_LIST_OPTIONS)
        {
            if(currentStory.EnableSkipMode)
            {
                abo = new AbilityBuilderObject("Toggle Skip Mode", "Current: " + enableSkipMode, INPUT_OPTIONS_SKIP);
                retValue.Add(abo);
            }
            
        }

        return retValue;
    }


    void ButtonStoryPanelClicked(int panelType, int clickId)
    {
        scrollList.gameObject.SetActive(false);
        if ( panelType == SCROLL_LIST_LOAD)
        {
            if( clickId == INPUT_NEW_SAVE)
            {
                CalcCode.SaveStorySave(true, currentStorySave);
                return;
            }
            else
            {
                LoadStory(NameAll.NULL_INT,clickId, false,false, NameAll.NULL_INT); //load by saveId
                return;
            }
            
        }
        else if( panelType == SCROLL_LIST_NEW)
        {
            LoadStory(clickId, NameAll.NULL_INT, true,false, NameAll.NULL_INT); //create a new by storyId
            return;
        }
        else if(panelType == SCROLL_LIST_OPTIONS)
        {
            enableSkipMode = !enableSkipMode;
            ToggleSkipMode(enableSkipMode);
            return;
        }
    }
    #endregion

    #region UnitLists for starting battles
    void PopulateStartBattleScrollLists()
    {
        PopulateUnitScrollList(SCROLL_LIST_PARTY_UNIT);
        PopulateUnitScrollList(SCROLL_LIST_SELECTED_UNIT);
    }

    int playerPartySlots;

    void TallyPlayerSlots()
    {
        playerPartySlots = 0;
        //see number of player slots
        CampaignSpawn cs = CalcCode.LoadCampaignSpawn(currentStory.CampaignId);//CalcCode.Load
        if (cs != null)
        {
            List<SpawnObject> spawnList = cs.GetSpawnListByLevel(currentStoryPointInt.StoryBattleId);

            foreach (SpawnObject so in spawnList)
            {
                if (so.Team == NameAll.TEAM_ID_GREEN)
                    playerPartySlots += 1;
            }
        }
    }

    void PopulateUnitScrollList(int panelType)
    {
        

        if (panelType == SCROLL_LIST_PARTY_UNIT)
        {
            partyUnitScrollList.gameObject.SetActive(true);
            foreach (Transform child in partyUnitScrollListContentPanel)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        else
        {
            TallyPlayerSlots();
            selectedUnitScrollListText.text = "Selected Units " + selectedUnitList.Count + "/" + playerPartySlots;
            selectedUnitScrollList.gameObject.SetActive(true);
            foreach (Transform child in selectedUnitScrollListContentPanel)
            {
                GameObject.Destroy(child.gameObject);
            }
        }


        List<AbilityBuilderObject> aboList = BuildPartyUnitList(panelType);

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(unitScrollListButton) as GameObject;
            MPPickBanButton tb = newButton.GetComponent<MPPickBanButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;

            if (panelType == SCROLL_LIST_PARTY_UNIT)
                tb.transform.SetParent(partyUnitScrollListContentPanel);
            else
                tb.transform.SetParent(selectedUnitScrollListContentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonPartyUnitListClicked(panelType, tempInt));
        }
    }

    List<AbilityBuilderObject> BuildPartyUnitList(int panelType)
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;
        List<PlayerUnit> tempList = new List<PlayerUnit>();


        if (panelType == SCROLL_LIST_PARTY_UNIT)
            tempList = partyUnitList;
        else
            tempList = selectedUnitList;

        for (int i = 0; i < tempList.Count; i++)
        {
            abo = new AbilityBuilderObject(CalcCode.GetTitleFromPlayerUnit(tempList[i]), CalcCode.GetDetailsFromPlayerUnit(tempList[i]), i);
            retValue.Add(abo);
        }

        return retValue;
    }


    void ButtonPartyUnitListClicked(int panelType, int clickId)
    {
        //clicking on a unit moves it from one list to the other
        if (panelType == SCROLL_LIST_PARTY_UNIT)
        {
            if( playerPartySlots > selectedUnitList.Count)
            {
                PlayerUnit pu = partyUnitList[clickId];
                partyUnitList.Remove(pu);
                selectedUnitList.Add(pu);
                //selectedUnitScrollListText.text = "Selected Units " + selectedUnitList.Count + "/" + playerPartySlots;
            }
        }
        else
        {
            PlayerUnit pu = selectedUnitList[clickId];
            partyUnitList.Add(pu);
            selectedUnitList.Remove(pu);
            //selectedUnitScrollListText.text = "Selected Units " + selectedUnitList.Count + "/" + playerPartySlots;
        }

        PopulateStartBattleScrollLists();
    }

    #endregion

}
