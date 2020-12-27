using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class StoryBuilderController : MonoBehaviour {

    //StoryEditPanel
    //scrollObject
    public GameObject scrollListButton;
    public Transform contentPanel;
    //text fields
    public Text leftStoryPanelText;
    public Text rightStoryPanelText;

    //points scroll list
    public GameObject pointsScrollList;
    public GameObject pointsScrollListButton;
    public Transform pointsScrollListContentPanel;

    //Map Panel and Image
    public GameObject mapPanel;
    public Image mapImage;
    public Button deletePointButton;
    public InputField XPointInputField;
    public InputField YPointInputField;
    public GameObject mapPointPrefab; //base prefab that will be instantiated
    GameObject mapPointGameObject;
    float mapPanelWidth; //used to scale where map points are
    float mapPanelHeight;

    //user input
    //input object
    public GameObject userInputPanel;
    public Text userInputTitle;
    public Text userInputDetails;
    public InputField userInputField;

    

    StoryObject currentStory;
    StoryPoint currentStoryPoint;
    StoryCutScene currentStoryCutScene;
    StoryItem currentStoryItem;
    int currentStoryPointId; //used to find the StoryPointObject in the currentStoryPoint.storyPointObjectList
    int currentStoryPointIntId; //used to find the StoryPointInt in the StoryPointObject.storyIntList
    int currentCutSceneId; //when editing cut scene details, used to load the proper cut scene
    int currentInputType; //for inputs used for OnSubmitUserInput
    

    static readonly int INPUT_STORY_OBJECT_LOAD = 1;
    static readonly int INPUT_STORY_EDIT_MAP_LIST = 2; //populate the list, allow further clicks
    static readonly int INPUT_STORY_EDIT_MAP_POINTS = 3; //opens the map, pointsList and allows editting
    static readonly int INPUT_STORY_CHANGE_MAP_LIST = 5; //populate the list with potential maps
    static readonly int INPUT_STORY_POINT_DETAILS = 6;
    static readonly int INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS = 7; //populate list with a specific story point int for editting
    static readonly int INPUT_STORY_CUT_SCENE = 8; //clicked on cutscene in the panel

    static readonly int INPUT_STORY_POINT_NONE = 10;
    static readonly int INPUT_STORY_POINT_NEW = 11;
    static readonly int INPUT_STORY_POINT_DELETE = 12;

    static readonly int INPUT_STORY_POINT_INT_STORY_INT = 20;
    static readonly int INPUT_STORY_POINT_INT_STORY_BATTLE_ID = 21;
    static readonly int INPUT_STORY_POINT_INT_PROGRESSION_CUT_SCENE_INT = 22;
    static readonly int INPUT_STORY_POINT_INT_PROGRESSION_BATTLE_INT = 23;
    static readonly int INPUT_STORY_POINT_INT_IS_SHOWN = 24;
    static readonly int INPUT_STORY_POINT_INT_IS_STORY_BATTLE = 25;
    static readonly int INPUT_STORY_POINT_INT_IS_CUT_SCENE = 27;
    static readonly int INPUT_STORY_POINT_INT_CUT_SCENE_ID = 28;
    static readonly int INPUT_STORY_POINT_INT_CREATE_STORY_INT_CUT_SCENE_PROGRESSION_INT = 29;
    static readonly int INPUT_STORY_POINT_INT_CREATE_STORY_INT_BATTLE_PROGRESSION_INT = 30;
    static readonly int INPUT_STORY_POINT_INT_STORY_PROGRESSION_LIST = 31;
    static readonly int INPUT_STORY_POINT_INT_IS_SHOP = 26;
    static readonly int INPUT_STORY_POINT_INT_SHOP_TYPE = 32;

    static readonly int INPUT_STORY_CUT_SCENE_DETAILS = 39; //a specific cutscene was clicked, show the details and allow for editing
    static readonly int INPUT_STORY_CUT_SCENE_DETAILS_IMAGE = 40;
    static readonly int INPUT_STORY_CUT_SCENE_DETAILS_NEXT_SCENE = 41;
    static readonly int INPUT_STORY_CUT_SCENE_DETAILS_TITLE = 42;
    static readonly int INPUT_STORY_CUT_SCENE_DETAILS_TEXT = 43;
    static readonly int INPUT_STORY_CUT_SCENE_DETAILS_DIALOGUE_LEVEL = 44;

    static readonly int INPUT_STORY_OBJECT_CAMPAIGN_ID = 50;
    static readonly int INPUT_STORY_OBJECT_TOGGLE_ENABLE_SKIP = 51;
    static readonly int INPUT_STORY_OBJECT_VERSION = 52;

    static readonly int SCROLL_LIST_STORY_UNITS = 60;

    static readonly int INPUT_STORY_STORY_DELETE = -1;
    static readonly int INPUT_STORY_POINT_DETAILS_CREATE_STORY_POINT_INT = -2; //has to be negative as other click options are >= 0
    static readonly int INPUT_STORY_OBJECT_NAME = -3;
    static readonly int INPUT_STORY_POINT_NAME = -4;
    static readonly int INPUT_STORY_CUT_SCENE_NEW = -5;
    static readonly int INPUT_STORY_CUT_SCENE_DELETE = -6;
    static readonly int INPUT_STORY_UNITS_ADD = -7;
    static readonly int INPUT_STORY_UNITS_REMOVE_ALL = -8;
    static readonly int INPUT_STORY_POINT_INT_DELETE_STORY_INT = -9;

    // Use this for initialization
    void Start () {

        mapPointGameObject = Instantiate(mapPointPrefab) as GameObject;

        leftStoryPanelText.text = "No Story Selected";
        rightStoryPanelText.text = "No Story Point Selected";

        currentStory = null;
        currentStoryPoint = null;
        currentStoryCutScene = null;
        currentStoryItem = null;
        currentStoryPointId = NameAll.NULL_INT;
        currentInputType = NameAll.NULL_INT;
        currentCutSceneId = NameAll.NULL_INT;

        mapPanelWidth = mapImage.GetComponent<RectTransform>().rect.width;
        mapPanelHeight = mapImage.GetComponent<RectTransform>().rect.height;

        XPointInputField.characterLimit = 12;
        //XPointInputField.characterValidation = InputField.CharacterValidation.Integer;
        YPointInputField.characterLimit = 12;
        //YPointInputField.characterValidation = InputField.CharacterValidation.Integer;

        AddInputFieldListeners();
        
    }


    #region StoryPanelClick

    public void OnClickUnits()
    {
        CloseMapPanel();
        if( currentStory != null)
        {
            PopulateStoryPanelList(SCROLL_LIST_STORY_UNITS);
        }
    }

    public void OnClickLoadStoryStoryPanel()
    {
        CloseMapPanel();
        PopulateStoryPanelList(INPUT_STORY_OBJECT_LOAD);
    }

    //causes points scroll list to be shown. first one populates this list. can edit the point details
    public void OnClickPointsStoryPanel()
    {
        CloseMapPanel();
        if (currentStory != null)
        {
            OpenPointsScrollList();
            PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS);
        }    
    }

    public void OnClickCutSceneStoryPanel()
    {
        CloseMapPanel();
        if (currentStory == null)
            return;
        PopulateStoryPanelList(INPUT_STORY_CUT_SCENE);
    }

    public void OnClickEditMapStoryPanel()
    {
        CloseMapPanel();
        if ( currentStory != null)
            PopulateStoryPanelList(INPUT_STORY_EDIT_MAP_LIST);
    }

    public void OnClickNewStory()
    {
        CloseMapPanel();
        currentStory = CalcCode.GetNewStoryObject();
        currentStoryPoint = new StoryPoint(currentStory.StoryId);
        currentStoryCutScene = new StoryCutScene(currentStory.StoryId);
        currentStoryItem = new StoryItem(currentStory.StoryId);
        SetCurrentStoryText(currentStory.StoryName);
        SetCurrentStoryPointId(INPUT_STORY_POINT_NONE);
    }

    void SetCurrentStoryText(string zString)
    {
        leftStoryPanelText.text = "Story: " + zString;
    }
    #endregion

    #region StoryObject and PopulateStoryPanel functions

    //opulate the list in the StoryPanel
    void PopulateStoryPanelList(int panelType)
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildStoryPanelList(panelType);

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(pointsScrollListButton) as GameObject;
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

        if( panelType == INPUT_STORY_OBJECT_LOAD) //click on Load Story, shows list of available story's to load
        {
            if( currentStory != null)
            {
                abo = new AbilityBuilderObject("DELETE Story", "Permanently DELETE story and associated file.", INPUT_STORY_STORY_DELETE);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Rename Current Story", "Current name: " + currentStory.StoryName, INPUT_STORY_OBJECT_NAME);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Edit Associated Campaign", currentStory.GetAssociatedCampaignIdString(), INPUT_STORY_OBJECT_CAMPAIGN_ID);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Toggle Enable Skip", "Allows testing in story mode. Currently: " + currentStory.EnableSkipMode, INPUT_STORY_OBJECT_TOGGLE_ENABLE_SKIP);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Toggle Version", "Selected version is: " + CalcCode.GetVersionFromInt(currentStory.Version), INPUT_STORY_OBJECT_VERSION);
                retValue.Add(abo);
            }

            var tempList = CalcCode.LoadStoryObjectList();
            foreach( StoryObject so in tempList)
            {
                abo = new AbilityBuilderObject("Load Story: " + so.StoryName, "", so.StoryId);
                retValue.Add(abo);
            }
        }
        else if( panelType == INPUT_STORY_EDIT_MAP_LIST) //click on Edit Map in panel, opens up options to change the map or edit map points
        {
            abo = new AbilityBuilderObject("Change Map", "", INPUT_STORY_CHANGE_MAP_LIST);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Map Story Points", "", INPUT_STORY_EDIT_MAP_POINTS);
            retValue.Add(abo);
        }
        else if( panelType == INPUT_STORY_CHANGE_MAP_LIST) //click on Edit Map -> Change Map
        {
            abo = new AbilityBuilderObject("Story Map 0", "", NameAll.STORY_MAP_0);
            retValue.Add(abo);
            //abo = new AbilityBuilderObject("Story Map 1", "", NameAll.STORY_MAP_1);
            //retValue.Add(abo);
        }
        else if( panelType == INPUT_STORY_POINT_DETAILS) //Click on Point Details in panel
        {
            currentStoryPointIntId = NameAll.NULL_INT;

            abo = new AbilityBuilderObject("Rename Current Story Point", "Current name: " + currentStoryPoint.GetPointName(currentStoryPointId), INPUT_STORY_POINT_NAME);
            retValue.Add(abo);

            abo = new AbilityBuilderObject("View Story Number and Progression Number Links", 
                "Links allow story to progress when matching battles/cut scenes are completed.", INPUT_STORY_POINT_INT_STORY_PROGRESSION_LIST);
            retValue.Add(abo);

            abo = new AbilityBuilderObject("Create Story Point Event", "Story point events allows various story triggers to occur here", INPUT_STORY_POINT_DETAILS_CREATE_STORY_POINT_INT);
            retValue.Add(abo);

            var tempList = currentStoryPoint.GetStoryPointIntList(currentStoryPointId);
            foreach (StoryPointInt spi in tempList)
            {
                abo = new AbilityBuilderObject("Event " + spi.StoryPointIntId, spi.ShowStoryPointIntDetails() , spi.StoryPointIntId);
                retValue.Add(abo);
            }
        }
        else if( panelType == INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS)
        { //click on Point Details -> then on a specific Story Point Int. can edit the story point int here
            StoryPointInt spi = currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId);
            if( spi != null) //redundant, not needed
            {
                abo = new AbilityBuilderObject("DELETE Story Point Event", "", INPUT_STORY_POINT_INT_DELETE_STORY_INT);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Edit Story Number", "Story Number: " + spi.StoryInt, INPUT_STORY_POINT_INT_STORY_INT);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Edit Is Point Shown?", spi.IsShown.ToString(), INPUT_STORY_POINT_INT_IS_SHOWN);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Edit Has Shop", spi.IsHasShop.ToString(), INPUT_STORY_POINT_INT_IS_SHOP);
                retValue.Add(abo);
                if( spi.IsHasShop)
                {
                    abo = new AbilityBuilderObject("Edit Shop Type", currentStoryItem.GetShopDetails(currentStoryPointId, currentStoryPointIntId), INPUT_STORY_POINT_INT_SHOP_TYPE);
                    retValue.Add(abo);
                }

                abo = new AbilityBuilderObject("Edit Has Story Battle", spi.IsHasStoryBattle.ToString(), INPUT_STORY_POINT_INT_IS_STORY_BATTLE);
                retValue.Add(abo);
                if(spi.IsHasStoryBattle)
                {
                    abo = new AbilityBuilderObject("Edit Story Battle Id", "Current Battle Id: " + spi.StoryBattleId, INPUT_STORY_POINT_INT_STORY_BATTLE_ID);
                    retValue.Add(abo);
                }

                abo = new AbilityBuilderObject("Edit Has Cut Scene", spi.IsHasCutScene.ToString(), INPUT_STORY_POINT_INT_IS_CUT_SCENE);
                retValue.Add(abo);
                if (spi.IsHasCutScene)
                {
                    abo = new AbilityBuilderObject("Edit Cut Scene Id", "Current Cut Scene Id: " + spi.CutSceneId, INPUT_STORY_POINT_INT_CUT_SCENE_ID);
                    retValue.Add(abo);
                }

                abo = new AbilityBuilderObject("Edit Battle Progression Number", "Current Number: " + spi.ProgressionIntBattle, INPUT_STORY_POINT_INT_PROGRESSION_BATTLE_INT);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Edit Cut Scene Progression Number", "Current Number: " + spi.ProgressionIntCutScene, INPUT_STORY_POINT_INT_PROGRESSION_CUT_SCENE_INT);
                retValue.Add(abo);

                //third value (storyIntNew after the trigger) is added in the input window
                abo = new AbilityBuilderObject("Create Story Number and Battle Progression Link", "", INPUT_STORY_POINT_INT_CREATE_STORY_INT_BATTLE_PROGRESSION_INT);
                retValue.Add(abo);
                abo = new AbilityBuilderObject("Create Story Number and Cut Scene Progression Link", "", INPUT_STORY_POINT_INT_CREATE_STORY_INT_CUT_SCENE_PROGRESSION_INT);
                retValue.Add(abo);


                //to add later
                ////public int RandomBattleChance { get; set; } //implement later
                ////public int ShopId {get; set; } //implement later
                //public bool IsHasRandomBattle { get; set; } //has a chance at random battle (if no story battle)
                //public int RandomBattleId { get; set; } //identifier to know which random battle to load
            }
        }
        else if( panelType == INPUT_STORY_POINT_INT_STORY_PROGRESSION_LIST)
        {
            //loads the dictionary of storypointint to progression int link
            //these values allow the story to progress when certain requirements are met (meeting the progression int tied to the cut scene or the battle)
            var tempList = currentStory.storyIntProgressionList;
            for( int i = 0; i < tempList.Count; i++)
            {
                StoryIntProgression sip = tempList[i];
                abo = new AbilityBuilderObject("DELETE Progression: Story Number and Progression Number to New Story Number", 
                    "Delete: " + sip.StoryInt +" and " + sip.ProgressionInt + " leads to " + sip.StoryIntNew,i);
                retValue.Add(abo);
            }
        }
        else if( panelType == INPUT_STORY_CUT_SCENE)
        {
            //story panel cut scene button pressed
            //load available cut scenes
            currentCutSceneId = NameAll.NULL_INT;

            abo = new AbilityBuilderObject("Add New Cut Scene", "", INPUT_STORY_CUT_SCENE_NEW);
            retValue.Add(abo);
            var tempList = currentStoryCutScene.cutSceneList;
            foreach (StoryCutSceneObject s in tempList)
            {
                abo = new AbilityBuilderObject("Cut Scene " + s.CutSceneId, "Title: " + s.BackgroundTitle,s.CutSceneId);
                retValue.Add(abo);
            }
        }
        else if( panelType == INPUT_STORY_CUT_SCENE_DETAILS)
        {
            StoryCutSceneObject sc = currentStoryCutScene.GetCutSceneObject(currentCutSceneId);

            abo = new AbilityBuilderObject("DELETE Cut Scene", "", INPUT_STORY_CUT_SCENE_DELETE);
            retValue.Add(abo);
            //abo = new AbilityBuilderObject("DELETE Cut Scene", "", INPUT_STORY_CUT_SCENE_DETAILS_IMAGE);
            //retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Cut Scene Title", sc.BackgroundTitle, INPUT_STORY_CUT_SCENE_DETAILS_TITLE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Cut Scene Text", sc.BackgroundText, INPUT_STORY_CUT_SCENE_DETAILS_TEXT);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Dialogue", "Play dialogue from campaign level: " + sc.DialogueLevel, INPUT_STORY_CUT_SCENE_DETAILS_DIALOGUE_LEVEL);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit On Cut Scene Watched ", sc.GetNextCutSceneString(), INPUT_STORY_CUT_SCENE_DETAILS_NEXT_SCENE);
            retValue.Add(abo);

        }
        else if( panelType == INPUT_STORY_OBJECT_CAMPAIGN_ID)
        {
            //link a campaign to storyobject. allows user to select battles and dialogue
            abo = new AbilityBuilderObject("No Campaign Assoicated", "Unable to choose battles or cut scene dialogues.", NameAll.NULL_INT);
            retValue.Add(abo);
            var tempList = CalcCode.LoadCampaignList();
            foreach( CampaignCampaign cc in tempList)
            {
                abo = new AbilityBuilderObject("Campaign Id: " + cc.CampaignId, cc.CampaignName, cc.CampaignId);
                retValue.Add(abo);
            }
        }
        else if (panelType == INPUT_STORY_POINT_INT_STORY_BATTLE_ID)
        {
            //link a campaign battle
            abo = new AbilityBuilderObject("No Battle", "", NameAll.NULL_INT);
            retValue.Add(abo);
            CampaignLevel cl = CalcCode.LoadCampaignLevel(currentStory.CampaignId);
            if( cl != null)
            {
                var tempList = cl.orderList;
                foreach (int i in tempList)
                {
                    abo = new AbilityBuilderObject("Level Id: " + i, "Get battle associated with this level", i);
                    retValue.Add(abo);
                }
            }
        }
        else if (panelType == INPUT_STORY_POINT_INT_CUT_SCENE_ID)
        {
            abo = new AbilityBuilderObject("No Cut Scene", "", NameAll.NULL_INT);
            retValue.Add(abo);
            var tempList = currentStoryCutScene.cutSceneList;
            foreach (StoryCutSceneObject sc in tempList)
            {
                abo = new AbilityBuilderObject("Cut Scene Id: " + sc.CutSceneId, "Title: " + sc.BackgroundTitle , sc.CutSceneId);
                retValue.Add(abo);
            }
        }
        else if( panelType == INPUT_STORY_CUT_SCENE_DETAILS_DIALOGUE_LEVEL)
        {
            //in associated campaign, see what available dialogues to choose from
            abo = new AbilityBuilderObject("No Cut Scene Dialogue", "", NameAll.NULL_INT);
            retValue.Add(abo);
            CampaignLevel cl = CalcCode.LoadCampaignLevel(currentStory.CampaignId);
            if (cl != null)
            {
                var tempList = cl.orderList;
                foreach (int i in tempList)
                {
                    abo = new AbilityBuilderObject("Level Id: " + i, "Get DIALOGUE ONLY associated with this level", i);
                    retValue.Add(abo);
                }
            }
        }
        else if( panelType == INPUT_STORY_CUT_SCENE_DETAILS_NEXT_SCENE)
        {
            //goes to list of available cut scenes to see which one is next
            abo = new AbilityBuilderObject("Return to Story", "After cut scene is shown, return to story.", NameAll.NULL_INT);
            retValue.Add(abo);
            var tempList = currentStoryCutScene.cutSceneList;
            foreach (StoryCutSceneObject s in tempList)
            {
                abo = new AbilityBuilderObject("Go to CutScene " + s.CutSceneId, "Plays this cut scene next, does not return to story.", s.CutSceneId);
                retValue.Add(abo);
            }
        }
        else if( panelType == SCROLL_LIST_STORY_UNITS)
        {
            abo = new AbilityBuilderObject("Remove All Starting Units", "Clears starting unit list.", INPUT_STORY_UNITS_REMOVE_ALL);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Add Starting Units", "Player begins the game with these units.", INPUT_STORY_UNITS_ADD);
            retValue.Add(abo);

            var tempList = currentStory.startingPlayerUnitList;
            for( int i = 0; i <tempList.Count; i++)
            {
                var pu = tempList[i];
                abo = new AbilityBuilderObject(CalcCode.GetTitleFromPlayerUnit(pu), CalcCode.GetDetailsFromPlayerUnit(pu), i);
                retValue.Add(abo);
            }
        }
        else if( panelType == INPUT_STORY_UNITS_ADD)
        {
            if( currentStory.CampaignId == NameAll.NULL_INT)
            {
                abo = new AbilityBuilderObject("Select Campaign Link to Select Units", "No campaign is linked to this story. Units cannot be selected.", INPUT_STORY_UNITS_ADD);
                retValue.Add(abo);
            }
            else
            {
                var unitDict = CalcCode.LoadPlayerUnitDict(currentStory.Version, true, currentStory.CampaignId);
                foreach (KeyValuePair<int, PlayerUnit> kvp in unitDict)
                {
                    abo = new AbilityBuilderObject(CalcCode.GetTitleFromPlayerUnit(kvp.Value), CalcCode.GetDetailsFromPlayerUnit(kvp.Value), kvp.Key);
                    retValue.Add(abo);
                }
            }
        }
        

        return retValue;
    }


    void ButtonStoryPanelClicked(int panelType, int clickId)
    {
        if( panelType == INPUT_STORY_OBJECT_LOAD)
        {
            if (clickId == INPUT_STORY_STORY_DELETE)
                DeleteStory();
            else if (clickId == INPUT_STORY_OBJECT_NAME)
            {
                OpenUserInput(clickId);
                return;
            }
            else if (clickId == INPUT_STORY_OBJECT_CAMPAIGN_ID)
            {
                PopulateStoryPanelList(INPUT_STORY_OBJECT_CAMPAIGN_ID);
                return;
            }
            else if (clickId == INPUT_STORY_OBJECT_TOGGLE_ENABLE_SKIP)
                currentStory.EnableSkipMode = !currentStory.EnableSkipMode;
            else if( clickId == INPUT_STORY_OBJECT_VERSION)
            {
                if (currentStory.Version == NameAll.VERSION_AURELIAN)
                    currentStory.Version = NameAll.VERSION_CLASSIC;
                else
                    currentStory.Version = NameAll.VERSION_AURELIAN;
            }
            else
                LoadStoryObject(clickId);

            PopulateStoryPanelList(INPUT_STORY_OBJECT_LOAD);
            return;
        }
        else if( panelType == INPUT_STORY_EDIT_MAP_LIST)
        {
            if( clickId == INPUT_STORY_EDIT_MAP_POINTS)
            {
                OpenPointsScrollList();
                OpenEditMap();
                return;
            }
            else if( clickId == INPUT_STORY_CHANGE_MAP_LIST)
            {
                PopulateStoryPanelList(clickId); //loads the available maps to change
                return;
            }
        }
        else if (panelType == INPUT_STORY_CHANGE_MAP_LIST)
        {
            currentStory.MapId = clickId;
            PopulateStoryPanelList(INPUT_STORY_EDIT_MAP_LIST);
            return;
        }
        else if( panelType == INPUT_STORY_POINT_DETAILS)
        {
            if ( clickId == INPUT_STORY_POINT_DETAILS_CREATE_STORY_POINT_INT)
            {
                currentStoryPoint.AddStoryPointInt(currentStoryPointId);
                PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS);
                return;
            }
            else if( clickId == INPUT_STORY_POINT_INT_STORY_PROGRESSION_LIST)
            {
                PopulateStoryPanelList(INPUT_STORY_POINT_INT_STORY_PROGRESSION_LIST);
                return;
            }
            else if( clickId == INPUT_STORY_POINT_NAME)
            {
                OpenUserInput(clickId);
                return;
            }
            else
            {
                //the click Id is the actualy story point int that has been selected, show details on that
                currentStoryPointIntId = clickId;
                PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS);
                
                return;
            }
            return;
        }
        else if( panelType == INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS)
        {
            if(clickId == INPUT_STORY_POINT_INT_DELETE_STORY_INT)
            {
                currentStoryPoint.DeleteStoryPointInt(currentStoryPointId, currentStoryPointIntId);
                currentStoryPointIntId = NameAll.NULL_INT;
                PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS);
                return;
            }
            else if (clickId == INPUT_STORY_POINT_INT_STORY_INT)
            {
                OpenUserInput(clickId);
                return;
            }
            else if (clickId == INPUT_STORY_POINT_INT_STORY_BATTLE_ID)
            {
                PopulateStoryPanelList(INPUT_STORY_POINT_INT_STORY_BATTLE_ID);
                //OpenUserInput(clickId);
                return;
            }
            else if (clickId == INPUT_STORY_POINT_INT_PROGRESSION_CUT_SCENE_INT)
            {
                OpenUserInput(clickId);
                return;
            }
            else if (clickId == INPUT_STORY_POINT_INT_PROGRESSION_BATTLE_INT)
            {
                OpenUserInput(clickId);
                return;
            }
            else if (clickId == INPUT_STORY_POINT_INT_IS_SHOWN)
            {
                //on click turns it to the opposite
                currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).IsShown 
                    = !currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).IsShown;
                PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS);
                return;
            }
            else if (clickId == INPUT_STORY_POINT_INT_IS_STORY_BATTLE)
            {//on click turns it to the opposite
                currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).IsHasStoryBattle
                    = !currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).IsHasStoryBattle;
                PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS);
                return;
            }
            else if (clickId == INPUT_STORY_POINT_INT_IS_SHOP)
            {
                bool zBool = !currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).IsHasShop;
                currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).IsHasShop
                    = zBool;
                if (zBool)
                {
                    var tempList = ItemManager.Instance.GetStoryItemList(currentStory.Version, 1);
                    currentStoryItem.AddStoryItemObject(currentStoryPointId, currentStoryPointIntId, 1, tempList);
                } 
                else
                    currentStoryItem.DeleteStoryItemObject(currentStoryPointId, currentStoryPointIntId);

                PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS);
                return;
            }
            else if( clickId == INPUT_STORY_POINT_INT_SHOP_TYPE)
            {
                OpenUserInput(clickId);
                return;
            }
            else if (clickId == INPUT_STORY_POINT_INT_IS_CUT_SCENE)
            {
                currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).IsHasCutScene
                    = !currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).IsHasCutScene;
                PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS);
                return;
            }
            else if (clickId == INPUT_STORY_POINT_INT_CUT_SCENE_ID)
            {
                PopulateStoryPanelList(INPUT_STORY_POINT_INT_CUT_SCENE_ID);
                //OpenUserInput(clickId);
                return;
            }
            else if (clickId == INPUT_STORY_POINT_INT_CREATE_STORY_INT_BATTLE_PROGRESSION_INT)
            {
                OpenUserInput(clickId);
                return;
            }
            else if (clickId == INPUT_STORY_POINT_INT_CREATE_STORY_INT_CUT_SCENE_PROGRESSION_INT)
            {
                OpenUserInput(clickId);
                return;
            }
            return;
        }
        else if( panelType == INPUT_STORY_POINT_INT_STORY_PROGRESSION_LIST)
        {
            currentStory.DeleteStoryIntProgression(clickId);
            PopulateStoryPanelList(INPUT_STORY_POINT_INT_STORY_PROGRESSION_LIST);
            return;
        }
        else if( panelType == INPUT_STORY_CUT_SCENE)
        {
            if( clickId == INPUT_STORY_CUT_SCENE_NEW)
            {
                //Debug.Log("adding a new cut scene");
                currentStoryCutScene.AddStoryCutSceneObject();
                PopulateStoryPanelList(panelType);
            }
            else
            {
                currentCutSceneId = clickId;
                PopulateStoryPanelList(INPUT_STORY_CUT_SCENE_DETAILS);
            }
            return;
        }
        else if( panelType == INPUT_STORY_CUT_SCENE_DETAILS)
        {
            if( clickId == INPUT_STORY_CUT_SCENE_DELETE)
            {
                currentStoryCutScene.DeleteStoryCutSceneObject(currentCutSceneId);
                PopulateStoryPanelList(INPUT_STORY_CUT_SCENE);
                return;
            }
            else if (clickId == INPUT_STORY_CUT_SCENE_DETAILS_NEXT_SCENE)
            {
                PopulateStoryPanelList(INPUT_STORY_CUT_SCENE_DETAILS_NEXT_SCENE);
                return;
            }
            else if (clickId == INPUT_STORY_CUT_SCENE_DETAILS_DIALOGUE_LEVEL)
            {
                //reload the list but with available dialogues to select
                PopulateStoryPanelList(INPUT_STORY_CUT_SCENE_DETAILS_DIALOGUE_LEVEL);
                return;
            }
            else
            {
                OpenUserInput(clickId);
                return;
            }
            //else if( clickId == INPUT_STORY_CUT_SCENE_DETAILS_IMAGE)
            //{
            //    OpenUserInput(clickId);
            //    return;
            //}
            //else if (clickId == INPUT_STORY_CUT_SCENE_DETAILS_TITLE)
            //{
            //    OpenUserInput(clickId);
            //    return;
            //}
            //else if (clickId == INPUT_STORY_CUT_SCENE_DETAILS_TEXT)
            //{
            //    OpenUserInput(clickId);
            //    return;
            //}
            

            //return;
        }
        else if( panelType == INPUT_STORY_CUT_SCENE_DETAILS_NEXT_SCENE)
        {
            //Debug.Log("clickId is " + clickId);
            currentStoryCutScene.GetCutSceneObject(currentCutSceneId).NextCutSceneId = clickId;
            PopulateStoryPanelList(INPUT_STORY_CUT_SCENE_DETAILS);
            return;
        }
        else if( panelType == INPUT_STORY_CUT_SCENE_DETAILS_DIALOGUE_LEVEL)
        {
            currentStoryCutScene.GetCutSceneObject(currentCutSceneId).DialogueLevel = clickId;
            PopulateStoryPanelList(INPUT_STORY_CUT_SCENE_DETAILS);
            return;
        }
        else if (panelType == INPUT_STORY_POINT_INT_STORY_BATTLE_ID)
        {
            currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).StoryBattleId = clickId;
            PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS);
            return;
        }
        else if (panelType == INPUT_STORY_POINT_INT_CUT_SCENE_ID)
        {
            currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).CutSceneId = clickId;
            PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS);
            return;
        }
        else if (panelType == INPUT_STORY_OBJECT_CAMPAIGN_ID)
        {
            currentStory.CampaignId = clickId;
            OnClickLoadStoryStoryPanel();
            return;
        }
        else if( panelType == SCROLL_LIST_STORY_UNITS)
        {
            if (clickId == INPUT_STORY_UNITS_REMOVE_ALL)
            {
                currentStory.startingPlayerUnitList.Clear();
                PopulateStoryPanelList(SCROLL_LIST_STORY_UNITS);
            }
            else if (clickId == INPUT_STORY_UNITS_ADD)
                PopulateStoryPanelList(INPUT_STORY_UNITS_ADD);
            return;
        }
        else if( panelType == INPUT_STORY_UNITS_ADD)
        {
            var unitDict = CalcCode.LoadPlayerUnitDict(currentStory.Version, true, currentStory.CampaignId);
            currentStory.startingPlayerUnitList.Add(unitDict[clickId]);
            PopulateStoryPanelList(SCROLL_LIST_STORY_UNITS);
            return;
        }
    }

    void LoadStoryObject(int storyId)
    {
        currentStory = CalcCode.LoadStoryObject(storyId);
        currentStoryPoint = CalcCode.LoadStoryPoint(storyId);
        currentStoryCutScene = CalcCode.LoadStoryCutScene(storyId);
        currentStoryItem = CalcCode.LoadStoryItem(storyId);
        SetCurrentStoryText(currentStory.StoryName);
        SetCurrentStoryPointId(INPUT_STORY_POINT_NONE);
    }



    void DeleteStory()
    {
        CalcCode.DeleteStory(currentStory.StoryId);
    }
    #endregion

    #region StoryPointList and PopulateStoryPointList functions

    void OpenPointsScrollList()
    {
        pointsScrollList.SetActive(true);

        SetCurrentStoryPointId(currentStoryPointId);
        PopulateStoryPointList();

        //Debug.Log("number of story points is " + currentStoryPoint.storyPointObjectList.Count);
    }

    void SetCurrentStoryPointId(int pointId)
    {
        //just loaded the point list, grab the first one
        if (pointId == INPUT_STORY_POINT_NONE || pointId == NameAll.NULL_INT)
        {
            if (currentStoryPoint != null)
            {
                //grabbing the first story point
                currentStoryPointId = currentStoryPoint.GetFirstPointId();
                rightStoryPanelText.text = "Point: " + currentStoryPoint.GetPointName(currentStoryPointId);
            }
            else
            {
                currentStoryPointId = NameAll.NULL_INT;
                rightStoryPanelText.text = "No Story Point Selected";
            }
        }
        else
        {
            //loading from the storyList
            currentStoryPointId = pointId;
            rightStoryPanelText.text = "Point: " + currentStoryPoint.GetPointName(currentStoryPointId);
        }

    }

    void PopulateStoryPointList()
    {
        foreach (Transform child in pointsScrollListContentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        List<AbilityBuilderObject> aboList = BuildStoryPointList();

        foreach (AbilityBuilderObject i in aboList)
        {
            
            GameObject newButton = Instantiate(pointsScrollListButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(pointsScrollListContentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonStoryPointClicked(tempInt));
        }
    }

    List<AbilityBuilderObject> BuildStoryPointList()
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;
        
        if (currentStory == null)
        {
            abo = new AbilityBuilderObject("No Story Selected", "Create or Load a Story to see Story Points associated with it", INPUT_STORY_POINT_NONE);
            retValue.Add(abo);
            return retValue;
        }

        abo = new AbilityBuilderObject("Create Story Point", "", INPUT_STORY_POINT_NEW);
        retValue.Add(abo);
        
        if ( currentStoryPoint != null)
        {
            if( currentStoryPointId != NameAll.NULL_INT)
            {
                abo = new AbilityBuilderObject("DELETE Current Point", "Delete point and details", INPUT_STORY_POINT_DELETE);
                retValue.Add(abo);
            }

            var tempList = currentStoryPoint.storyPointObjectList;
            foreach( StoryPointObject spo in tempList)
            {
                abo = new AbilityBuilderObject(spo.PointName, "Point Id: " + spo.PointId + "  x,y: " + spo.ScreenX + "," + spo.ScreenY, spo.PointId);
                retValue.Add(abo);
            }
        }
       
        return retValue;
    }

    void ButtonStoryPointClicked(int clickId)
    {
        if (clickId == INPUT_STORY_POINT_NONE)
            return;
        else if( clickId == INPUT_STORY_POINT_NEW)
        {
            AddStoryPoint();
            return;
        }
        else if( clickId == INPUT_STORY_POINT_DELETE)
        {
            DeletePoint(); //handles null int
            return;
        }

        SetCurrentStoryPointId(clickId);
        if (!mapPanel.activeSelf)
            PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS); //so proper name shows
        else
            SetMapImageStoryPoint();

        return;
    }

    #endregion

    #region OnClickMainMenu

    public void OnClickExit()
    {
        SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
    }

    //saves the current class
    public void OnClickSave()
    {
        string filePath = Application.dataPath + "/Custom/Stories/Custom/";
        //save StoryObject
        Serializer.Save<StoryObject>((filePath + currentStory.StoryId + "_story_object.dat"), currentStory);
        //save StoryPoint
        Serializer.Save<StoryPoint>((filePath + currentStory.StoryId + "_story_point.dat"), currentStoryPoint);
        //save StoryCutScene
        Serializer.Save<StoryCutScene>((filePath + currentStory.StoryId + "_story_cut_scene.dat"), currentStoryCutScene);
        //save StoryItem
        Serializer.Save<StoryItem>((filePath + currentStory.StoryId + "_story_item.dat"), currentStoryItem);
    }

    #endregion

    #region MapPanel functions

    void OpenEditMap()
    {
        OpenMapPanel();
    }

    void OpenMapPanel()
    {
        SetMapImage();
        OpenPointsScrollList(); //also sets the currentSToryPointId
        SetMapImageStoryPoint(); //if is a currentStoryPointId, sets that on map.
    }

    void SetMapImage()
    {
        mapPanel.SetActive(true);

        string zString = CalcCode.GetStoryMapImageString(currentStory.MapId);
        mapImage.sprite = Resources.Load<Sprite>(zString);
    }

    void SetMapImageStoryPoint()
    {
        if (currentStoryPointId == NameAll.NULL_INT)
        {
            //deletePointButton.gameObject.SetActive(false);
            XPointInputField.gameObject.SetActive(false);
            YPointInputField.gameObject.SetActive(false);
            mapPointGameObject.SetActive(false);
            Debug.Log("setting map image story point, null int");
        }
        else
        {
            Debug.Log("setting map image story point with point Id");
            //deletePointButton.gameObject.SetActive(true);
            XPointInputField.gameObject.SetActive(true);
            YPointInputField.gameObject.SetActive(true);
            
            SetMapPointGameObject(currentStoryPointId);
        }
    }

    //on the map image, a smaller image that shows the currently selected story point is shown
    //this updates that smaller image's place on the map
    void SetMapPointGameObject(int pointId)
    {
        mapPointGameObject.SetActive(true);
        mapPointGameObject.GetComponentInChildren<Text>().text = currentStoryPoint.GetPointName(pointId);
        mapPointGameObject.transform.SetParent(mapPanel.transform);

        var tempList = currentStoryPoint.GetPointXY(pointId);
        XPointInputField.text = "Point X: " + tempList[0].ToString();
        YPointInputField.text = "Point Y: " + tempList[1].ToString();

        //0,0 is the mid point of the mapImage
        float x = ((float) (tempList[0] - 50)) / 100.00f * mapPanelWidth; //pointX and pointY are scaled
        float y = ((float) (tempList[1] - 50)) / 100.00f * mapPanelHeight;

        //Debug.Log("trying to set the map point game object (" + x + ","  + y +")" );
        
        RectTransform trans = mapPointGameObject.GetComponent<RectTransform>();
        trans.localPosition = new Vector3(x, y, trans.localPosition.z);
    }

    public void CloseMapPanel()
    {
        mapPanel.SetActive(false);
        pointsScrollList.SetActive(false);
    }

    void AddInputFieldListeners()
    {
        InputField.SubmitEvent XPointInputEvent = new InputField.SubmitEvent();
        XPointInputEvent.AddListener(OnSubmitXPointInput);
        XPointInputField.onEndEdit = XPointInputEvent;

        InputField.SubmitEvent YPointInputEvent = new InputField.SubmitEvent();
        YPointInputEvent.AddListener(OnSubmitYPointInput);
        YPointInputField.onEndEdit = YPointInputEvent;

        InputField.SubmitEvent userInputEvent = new InputField.SubmitEvent();
        userInputEvent.AddListener(OnSubmitUserInput);
        userInputField.onEndEdit = userInputEvent;
    }

    void OnSubmitXPointInput(string zString)
    {
        int z1 = 50;
        if (Int32.TryParse(zString, out z1))
        {
            if (z1 < 0)
            {
                z1 = 0;
            }
            else if (z1 > 99)
            {
                z1 = 99;
            }
        }
        //XPointInputField.text = "Point X: " + z1;
        currentStoryPoint.SetPointX(currentStoryPointId, z1);
        SetMapPointGameObject(currentStoryPointId);//text updated here too
        PopulateStoryPointList();
    }

    void OnSubmitYPointInput(string zString)
    {
        int z1 = 50;
        if (Int32.TryParse(zString, out z1))
        {
            if (z1 < 0)
            {
                z1 = 0;
            }
            else if (z1 > 99)
            {
                z1 = 99;
            }
        }
        //YPointInputField.text = "Point Y: " + z1;
        currentStoryPoint.SetPointY(currentStoryPointId, z1);
        SetMapPointGameObject(currentStoryPointId);//text updated here too
        PopulateStoryPointList();
    }

    int GetIntFromString(string zString, int defaultInt = 0, int minInt = 0, int maxInt = 10000)
    {
        int z1 = defaultInt;
        if (Int32.TryParse(zString, out z1))
        {
            if (z1 < minInt)
            {
                z1 = minInt;
            }
            else if (z1 > maxInt)
            {
                z1 = maxInt;
            }
        }

        return z1;
    }

    public void DeletePoint()
    {
        if (currentStoryPoint != null && currentStoryPointId != NameAll.NULL_INT)
        {
            currentStoryPoint.DeleteStoryPointObject(currentStoryPointId);//handles nullInt
            SetCurrentStoryPointId(INPUT_STORY_POINT_NONE); //sets the currentStoryPointId in here
            PopulateStoryPointList();
            if (!mapPanel.activeSelf)
                PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS);
        }
            
    }

    public void AddStoryPoint()
    {
        if (currentStoryPoint != null)
        {
            currentStoryPoint.AddStoryPointObject();
            PopulateStoryPointList();
        }
            
    }
    #endregion

    #region userInput

    void CloseUserInput()
    {
        userInputPanel.SetActive(false);
    }

    void OpenUserInput(int type)
    {
        userInputPanel.SetActive(true);
        currentInputType = type;

        if (type == INPUT_STORY_OBJECT_NAME || type == INPUT_STORY_POINT_NAME 
            || type == INPUT_STORY_CUT_SCENE_DETAILS_TEXT || type == INPUT_STORY_CUT_SCENE_DETAILS_TITLE)
        {
            userInputField.characterValidation = InputField.CharacterValidation.None;
            userInputField.characterLimit = 30;
        }
        else
        {
            userInputField.characterValidation = InputField.CharacterValidation.Integer;
            userInputField.characterLimit = 6;
        }

        StoryPointInt spi = currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId);
        if (type == INPUT_STORY_POINT_INT_STORY_INT)
        {
            userInputTitle.text = "Story Number To Trigger";
            userInputField.text = spi.StoryInt.ToString();
        }
        else if (type == INPUT_STORY_POINT_INT_PROGRESSION_CUT_SCENE_INT)
        {
            userInputTitle.text = "Story Number To Trigger";
            userInputField.text = spi.ProgressionIntCutScene.ToString();
        }
        else if (type == INPUT_STORY_POINT_INT_PROGRESSION_BATTLE_INT)
        {
            userInputTitle.text = "Story Number To Trigger";
            userInputField.text = spi.ProgressionIntBattle.ToString();
        }
        //else if (type == INPUT_STORY_POINT_INT_IS_SHOWN)
        //{
        //just loading the list again with two input types
        //}
        //else if (type == INPUT_STORY_POINT_INT_IS_CUT_SCENE)
        //{

        //}
        //else if (type == INPUT_STORY_POINT_INT_IS_SHOP)
        //{

        //}
        //else if (type == INPUT_STORY_POINT_INT_IS_STORY_BATTLE)
        //{

        //}
        //else if (type == INPUT_STORY_POINT_INT_CUT_SCENE_ID)
        //{
        //    //load from list of available ones
        //}
        //else if (type == INPUT_STORY_POINT_INT_STORY_BATTLE_ID)
        //{
        //    //load from list of available ones
        //}
        else if (type == INPUT_STORY_POINT_INT_CREATE_STORY_INT_CUT_SCENE_PROGRESSION_INT)
        {
            userInputTitle.text = "Story Number To Trigger";
            userInputField.text = "";
        }
        else if (type == INPUT_STORY_POINT_INT_CREATE_STORY_INT_BATTLE_PROGRESSION_INT)
        {
            userInputTitle.text = "Story Number To Trigger";
            userInputField.text = "";
        }
        else if (type == INPUT_STORY_POINT_INT_SHOP_TYPE)
        {
            userInputTitle.text = "Shop Type";
            userInputField.text = "";
        }
        else if (type == INPUT_STORY_OBJECT_NAME)
        {
            userInputTitle.text = "Enter Story Name";
            userInputField.text = currentStory.StoryName;
        }
        else if (type == INPUT_STORY_POINT_NAME)
        {
            userInputTitle.text = "Enter Story Point Name";
            userInputField.text = currentStoryPoint.GetPointName(currentStoryPointId);
        }
        else if (type == INPUT_STORY_CUT_SCENE_DETAILS_TITLE)
        {
            userInputTitle.text = "Enter Cut Scene Title";
            userInputField.text = currentStoryPoint.GetPointName(currentStoryPointId);
        }
        else if (type == INPUT_STORY_CUT_SCENE_DETAILS_TEXT)
        {
            userInputTitle.text = "Enter Cut Scene Text";
            userInputField.text = currentStoryPoint.GetPointName(currentStoryPointId);
            userInputField.characterLimit = 1000;
        }
        //else if( type == INPUT_STORY_CUT_SCENE_DETAILS_IMAGE)
        //{

        //}

    }

    
    void OnSubmitUserInput(string zString)
    {

        //Debug.Log(" on submitUserInput " + zString + " " + currentInputType);
        int z1;

        if (currentInputType == INPUT_STORY_POINT_INT_STORY_INT)
        {
            z1 = GetIntFromString(zString);
            currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).StoryInt = z1;
            PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS);
        }
        else if (currentInputType == INPUT_STORY_POINT_INT_PROGRESSION_CUT_SCENE_INT)
        {
            z1 = GetIntFromString(zString);
            currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).ProgressionIntCutScene = z1;
            PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS);
        }
        else if (currentInputType == INPUT_STORY_POINT_INT_PROGRESSION_BATTLE_INT)
        {
            z1 = GetIntFromString(zString);
            currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).ProgressionIntBattle = z1;
            PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS);
        }
        else if (currentInputType == INPUT_STORY_POINT_INT_CREATE_STORY_INT_CUT_SCENE_PROGRESSION_INT)
        {
            z1 = GetIntFromString(zString);
            currentStory.AddStoryIntProgression(currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).StoryInt,
                currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).ProgressionIntCutScene, z1);
            PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS);
        }
        else if (currentInputType == INPUT_STORY_POINT_INT_CREATE_STORY_INT_BATTLE_PROGRESSION_INT)
        {
            z1 = GetIntFromString(zString);
            currentStory.AddStoryIntProgression(currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).StoryInt,
                currentStoryPoint.GetStoryPointInt(currentStoryPointId, currentStoryPointIntId).ProgressionIntBattle, z1);
            PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS);
        }
        else if( currentInputType == INPUT_STORY_POINT_INT_SHOP_TYPE)
        {
            z1 = GetIntFromString(zString, 1, 1, 99);
            currentStoryItem.DeleteStoryItemObject(currentStoryPointId, currentStoryPointIntId);
            var zList = ItemManager.Instance.GetStoryItemList(currentStory.Version, z1);
            currentStoryItem.AddStoryItemObject(currentStoryPointId, currentStoryPointIntId, z1, zList);
            PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS_STORY_POINT_INT_DETAILS);
        }
        else if (currentInputType == INPUT_STORY_OBJECT_NAME)
        {
            currentStory.StoryName = zString;
            SetCurrentStoryText(currentStory.StoryName);
            PopulateStoryPanelList(INPUT_STORY_OBJECT_LOAD);
        }
        else if (currentInputType == INPUT_STORY_POINT_NAME)
        {
            currentStoryPoint.GetStoryPointObject(currentStoryPointId).PointName = zString;
            PopulateStoryPanelList(INPUT_STORY_POINT_DETAILS);
            PopulateStoryPointList();
        }
        else if( currentInputType == INPUT_STORY_CUT_SCENE_DETAILS_TEXT)
        {
            currentStoryCutScene.GetCutSceneObject(currentCutSceneId).BackgroundText = zString;
            PopulateStoryPanelList(INPUT_STORY_CUT_SCENE_DETAILS);
        }
        else if( currentInputType == INPUT_STORY_CUT_SCENE_DETAILS_TITLE)
        {
            currentStoryCutScene.GetCutSceneObject(currentCutSceneId).BackgroundTitle = zString;
            PopulateStoryPanelList(INPUT_STORY_CUT_SCENE_DETAILS);
        }

        CloseUserInput();
    }

    public void OnClickCloseUserInput()
    {
        OnSubmitUserInput(userInputField.text);
    }
    #endregion

}
