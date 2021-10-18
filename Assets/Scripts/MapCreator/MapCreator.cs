using UnityEngine;
//using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Controller for the MapBuilder scene
/// Maps (levels) are used in combat for the various game modes
/// </summary>



public class MapCreator : MonoBehaviour
{
    #region Fields / Properties
    [SerializeField]
    GameObject tileViewPrefab;
    [SerializeField]
    GameObject tileSelectionIndicatorPrefab;
    //[SerializeField]
    int width = 10;
    //[SerializeField]
    int depth = 10;
    //[SerializeField]
    int height = 10;
    [SerializeField]
    Point pos = new Point(0,0);
    [SerializeField]
    LevelData currentLevelData;
    Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();

    [SerializeField]
    private InputField inputName;
    [SerializeField]
    private InputField inputX;
    [SerializeField]
    private InputField inputY;
    [SerializeField]
    private InputField inputZ;
    int team2SpawnPoints = 0;
    int team3SpawnPoints = 0;
    List<SerializableVector3> spawnPointsList = new List<SerializableVector3>();
    string levelName = "custom";

    //loading level
    //load scrollList
    public GameObject LoadPanel;
    public Transform contentPanel;
    public GameObject sampleButton;
    Dictionary<int, LevelData> levelDict = new Dictionary<int, LevelData>();
    LevelLoad levelLoad;//used for saving the level
    string currentSaveFilePath = NameAll.MAP_SAVE_CUSTOM_DEFAULT;

    Transform marker
    {
        get
        {
            if (_marker == null)
            {
                if(GameObject.Find("Tile Selection Indicator"))
                {
                    _marker = GameObject.Find("Tile Selection Indicator").transform; Debug.Log("testing");
                    return _marker;
                }

                GameObject instance = Instantiate(tileSelectionIndicatorPrefab) as GameObject;
                _marker = instance.transform;
            }
            return _marker;
        }
    }
    Transform _marker;

    static readonly int OPTIONS_LIST_DEFAULT = 1;
    static readonly int OPTIONS_SELECT_DELETE = 12;
    static readonly int OPTIONS_SELECT_DIRECTORY = 13;
    int currentOptionsType = OPTIONS_LIST_DEFAULT;
    string currentCampaignDirectoryName = "";
    string currentCampaignDirectoryPath;
    bool currentIsCampaign = false;
    int currentCampaignId = 0;
    int currentLevelId; //used for saving a level into the first open slot
    public Text scrollPanelText;

    int maxTeam2SpawnPoints = 10;
    int maxTeam3SpawnPoints = 10;
    #endregion

    void Awake()
    {
        levelLoad = this.GetComponent<LevelLoad>();
    }

    void Start()
    {
        SetDirectoryDefault();//clears the board too
        //set marker initial location
        if (tiles.Count > 0)
        {
            foreach (Point p1 in tiles.Keys)
            {
                SelectTile(p1);
                break;
            }
        }
        else
        {
            marker.transform.position = new Vector3(0,0,0);
        }
        inputName.text = levelName;
        inputX.text = width.ToString();
        inputY.text = depth.ToString(); 
        inputZ.text = height.ToString();

        // Add listener to catch the submit
        InputField.SubmitEvent submitNameEvent = new InputField.SubmitEvent();
        submitNameEvent.AddListener(OnSubmitName);
        inputName.onEndEdit = submitNameEvent;
        // Add validation
        inputName.characterValidation = InputField.CharacterValidation.None;

    }

    
    void OnEnable()
    {
        InputController.moveEvent += OnMoveEvent;
        InputController.fireEvent += OnFireEvent;
    }

    void OnDisable()
    {
        InputController.moveEvent -= OnMoveEvent;
        InputController.fireEvent -= OnFireEvent;
    }

    void OnMoveEvent(object sender, InfoEventArgs<Point> e)
    {
        //Debug.Log("Move " + e.info.ToString());
        SelectTile(e.info + pos);
    }

    void OnFireEvent(object sender, InfoEventArgs<int> e)
    {
        //Debug.Log("Fire " + e.info.ToString());
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (e.info == 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                Tile tile = hitObject.GetComponent<Tile>();
                if (tile != null)
                {
                    SelectTile(tile.pos);
                }
            }
        }
        else if ( e.info == 2)
        {
            Grow();
        }
        else if (e.info == 1)
        {
            Shrink();
        }
    }

    public void SelectTile(Point p)
    {
        if (pos == p)
        {
            //Debug.Log("Move test");
            return;
        }
        else if (p.x < 0 || p.y < 0 || p.x >= width || p.y >= depth)
        {
            return;
        }
        else if (!tiles.ContainsKey(p))
        {
            GrowSingle(p);
        }
        

        pos = p;
        marker.localPosition = tiles[p].center;
    }

    #region Public
    public void Grow()
    {
        GrowSingle(pos);
    }

    public void Shrink()
    {
        ShrinkSingle(pos);
    }

    public void GrowArea()
    {
        Rect r = RandomRect();
        GrowRect(r);
    }

    public void ShrinkArea()
    {
        Rect r = RandomRect();
        ShrinkRect(r);
    }

    public void UpdateMarker()
    {
        Tile t = tiles.ContainsKey(pos) ? tiles[pos] : null;
        marker.localPosition = t != null ? t.center : new Vector3(pos.x, 0, pos.y);
    }

    public void Load()
    {

        OnClickClear(false);
        SetInputFieldValues();
        
        foreach (Vector3 v in currentLevelData.tiles)
        {
            Tile t = Create();
            t.Load(v);
            tiles.Add(t.pos, t);
        }
        ShowSpawnPoints();
    }

    public void SetInputFieldValues()
    {
        inputName.text = currentLevelData.levelName;
        inputX.text = "" + currentLevelData.GetMaxX();
        inputY.text = "" + currentLevelData.GetMaxY();
        inputZ.text = "" + currentLevelData.GetMaxZ();
    }
    #endregion

    #region Private
    Rect RandomRect()
    {
        int x = UnityEngine.Random.Range(0, width);
        int y = UnityEngine.Random.Range(0, depth);
        int w = UnityEngine.Random.Range(1, width - x + 1);
        int h = UnityEngine.Random.Range(1, depth - y + 1);
        return new Rect(x, y, w, h);
    }

    void GrowRect(Rect rect)
    {
        for (int y = (int)rect.yMin; y < (int)rect.yMax; ++y)
        {
            for (int x = (int)rect.xMin; x < (int)rect.xMax; ++x)
            {
                Point p = new Point(x, y);
                GrowSingle(p);
            }
        }
    }

    void ShrinkRect(Rect rect)
    {
        for (int y = (int)rect.yMin; y < (int)rect.yMax; ++y)
        {
            for (int x = (int)rect.xMin; x < (int)rect.xMax; ++x)
            {
                Point p = new Point(x, y);
                ShrinkSingle(p);
            }
        }
    }

    Tile Create()
    {
        GameObject instance = Instantiate(tileViewPrefab) as GameObject;
        instance.transform.parent = transform;
        return instance.GetComponent<Tile>();
    }

    Tile GetOrCreate(Point p)
    {
        if (tiles.ContainsKey(p))
            return tiles[p];

        Tile t = Create();
        t.Load(p, 0);
        tiles.Add(p, t);

        return t;
    }

    void GrowSingle(Point p)
    {
        Tile t = GetOrCreate(p);
        if (t.height < height)
            t.Grow();
    }

    void ShrinkSingle(Point p)
    {
        if (!tiles.ContainsKey(p))
            return;

        Tile t = tiles[p];
        t.Shrink();

        if (t.height <= 0)
        {
            tiles.Remove(p);
            DestroyImmediate(t.gameObject);
        }
    }

    //void CreateSaveDirectory()
    //{
    //    string filePath = Application.dataPath + "/Resources";
    //    if (!Directory.Exists(filePath))
    //        System.IO.Directory.CreateDirectory(filePath);
    //    filePath += "/Levels";
    //    if (!Directory.Exists(filePath))
    //        System.IO.Directory.CreateDirectory(filePath);

    //    //string filePath = Application.dataPath + "/Resources";
    //    //if (!Directory.Exists(filePath))
    //    //    AssetDatabase.CreateFolder("Assets", "Resources");
    //    //filePath += "/Levels";
    //    //if (!Directory.Exists(filePath))
    //    //    AssetDatabase.CreateFolder("Assets/Resources", "Levels");
    //    //AssetDatabase.Refresh();
    //}
    #endregion

    #region UserInputs
    public void OnSubmitX(string x)
    {
        width = GetMapMinMax(x);
        inputX.text = width.ToString();
    }

    public void OnSubmitY(string x)
    {
        depth = GetMapMinMax(x);
        inputY.text = depth.ToString();
    }

    public void OnSubmitZ(string x)
    {
        height = GetMapMinMax(x);
        inputZ.text = height.ToString();
    }

    public void OnSubmitName(string x)
    {
        if( x.Length > 25)
        {
            x = x.Substring(0,25);
        }
        levelName = x;
        inputName.text = levelName;
    }

    int GetMapMinMax(string zString)
    {
        int z1 = 10;
        bool result = Int32.TryParse(zString, out z1);
        if (result)
        {
            if (z1 < 0)
            {
                z1 = 1;
            }
            else if (z1 > 20)
            {
                z1 = 20;
            }
        }
        else
        {
            z1 = 10;
        }
        return z1;
    }

    //spawn points
    public void ToggleTeam2SpawnPoint()
    {
        //use pos to find the position of the tile
        //check the dict to see if that spawn point exists
        //if it doesn't add it, if it does remove it
        AddOrRemoveSpawnPoint(2);
    }

    public void ToggleTeam3SpawnPoint()
    {
        //use pos to find the position of the tile
        //check the dict to see if that spawn point exists
        //if it doesn't add it, if it does remove it
        AddOrRemoveSpawnPoint(3);
    }

    private void AddOrRemoveSpawnPoint(int teamId)
    {
        Tile t = GetOrCreate(pos);
        Vector3 v2 = new Vector3(t.pos.x, t.pos.y, 2);
        Vector3 v3 = new Vector3(t.pos.x, t.pos.y, 3);
        if (spawnPointsList.Contains(v2))
        {
            spawnPointsList.Remove(v2);
            TallySpawnPoints(-1, 2);
            t.RevertTile();
        }
        else if (spawnPointsList.Contains(v3))
        {
            spawnPointsList.Remove(v3);
            TallySpawnPoints(-1, 3);
            t.RevertTile();
        }
        else
        {
            if (IsAbleToAddSpawnPoint(teamId))
            {
                SerializableVector3 v = new Vector3(t.pos.x, t.pos.y, teamId);
                spawnPointsList.Add(v);
                TallySpawnPoints(1, teamId);
                //code to fuck around with tile points
                t.HighlightTile(teamId);
            }
        }
    }

    private void TallySpawnPoints(int z1, int teamId)
    {
        if (teamId == 2)
        {
            team2SpawnPoints += z1;
        }
        else
        {
            team3SpawnPoints += z1;
        }
    }

    private bool IsAbleToAddSpawnPoint(int teamId)
    {
        if (teamId == 2)
        {
            if (team2SpawnPoints < 10)
            {
                return true;
            }
        }
        else
        {
            if (team3SpawnPoints < 10)
            {
                return true;
            }
        }
        return false;
    }

    private void ShowSpawnPoints()
    {
        team2SpawnPoints = 0;
        team3SpawnPoints = 0;

        spawnPointsList = new List<SerializableVector3>(currentLevelData.spList.Count);
        foreach (SerializableVector3 v in currentLevelData.spList)
        {
            spawnPointsList.Add(new SerializableVector3(v.x, v.y, v.z));
        }
        //spawnPointsList = myLevelData.spList;
        foreach (SerializableVector3 v in spawnPointsList)
        {
            Point p = new Point((int)v.x, (int)v.y);
            if (tiles.ContainsKey(p))
            {
                Tile t = tiles[p];
                t.HighlightTile((int)v.z); //the teamid
                TallySpawnPoints(1, (int)v.z);
            }
            else
            {
                Debug.Log("ERROR: key not found in spawn points, spawn point not being drawn");
            }
        }
        
    }


    #endregion

    #region OnClickButtons
    public void OnClickQuit()
    {
        //QUIT WORKS ONLY IN BUILT VERSIONS OF THE GAME Debug.Log("Asdf ");
        SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
    }

    public void OnClickClear(bool getNewLevelId = true)
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
            DestroyImmediate(transform.GetChild(i).gameObject);
        tiles.Clear();
        spawnPointsList.Clear();
        team2SpawnPoints = 0;
        team3SpawnPoints = 0;

        if(getNewLevelId)
            GetFirstAvailableLevelId();
        
    }

    

    public void OnClickOptions()
    {
        if (LoadPanel.activeSelf)
        {
            LoadPanel.SetActive(false);
        }
        else
        {
            LoadPanel.SetActive(true);
            PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);
        }
    }

    public void OnClickSave()
    {
        //string filePath = Application.dataPath + "/Resources/Levels";
        //if (!Directory.Exists(filePath))
        //    CreateSaveDirectory();

        LevelData board = new LevelData();//ScriptableObject.CreateInstance<LevelData>();
        board.tiles = new List<SerializableVector3>(tiles.Count);
        foreach (Tile t in tiles.Values)
            board.tiles.Add(new SerializableVector3(t.pos.x, t.height, t.pos.y));

        board.levelName = levelName;
        board.spList = new List<SerializableVector3>(spawnPointsList.Count);
        foreach (SerializableVector3 v in spawnPointsList)
        {
            board.spList.Add(new SerializableVector3(v.x, v.y, v.z));
        }
        //Debug.Log("board sp size is " + board.spList.Count + " -- " + spawnPointsList.Count);// adsf //asdf asdf
        levelLoad.SaveLevel(board, currentSaveFilePath);

        RefreshLevelDict();
        
    }

    void RefreshLevelDict()
    {
        if (currentIsCampaign)
        {
            levelDict = CalcCode.LoadLevelDict(true, currentCampaignId);
        }
        else
        {
            levelDict = levelLoad.GetMapDict();
        }
    }

    public void OnClickLoad()
    {
        if( LoadPanel.activeSelf)
        {
            LoadPanel.SetActive(false);
        }
        else
        {
            LoadPanel.SetActive(true);
            PopulateLoadScrollList();
        }
    }
    #endregion

    #region LoadScrollList
    void PopulateLoadScrollList()
    {
        scrollPanelText.text = "Load Map";
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildLoadList();

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonLoadClicked(tempInt));
        }
    }

    List<AbilityBuilderObject> BuildLoadList()
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;

        foreach( KeyValuePair<int,LevelData> kvp in levelDict)
        {
            abo = new AbilityBuilderObject("Level: " + kvp.Value.levelName, "", kvp.Key);
            retValue.Add(abo);
        }

        return retValue;
    }

    void ButtonLoadClicked(int levelKey)
    {

        LoadPanel.SetActive(false);
        currentLevelData = levelDict[levelKey];

        if (currentLevelData == null)
            return;
        team2SpawnPoints = 0;
        team3SpawnPoints = 0;
        currentSaveFilePath = currentCampaignDirectoryPath + "custom_" + levelKey + ".dat"; //Debug.Log(" " + currentSaveFilePath);
        Load();
    }

    void PopulateOptionsScrollList(int type)
    {
        scrollPanelText.text = "Options";
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
        currentOptionsType = type;

        if (type == OPTIONS_LIST_DEFAULT)
        {
            abo = new AbilityBuilderObject("Change Map Save Directory", "Current Directory is " + currentCampaignDirectoryName, OPTIONS_SELECT_DIRECTORY);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Delete Current Level", "Permanently Delete " + currentLevelData.levelName, OPTIONS_SELECT_DELETE);
            retValue.Add(abo);
        }
        else if (type == OPTIONS_SELECT_DIRECTORY)
        {
            abo = new AbilityBuilderObject("Default Map Directory", "Saves to default directory", NameAll.NULL_UNIT_ID);
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

        }

        return retValue;
    }

    void ButtonOptionsClicked(int select)
    {
        if (currentOptionsType == OPTIONS_LIST_DEFAULT)
        {
            if (select == OPTIONS_SELECT_DELETE)
            {
                DeleteLevel();
                PopulateOptionsScrollList(OPTIONS_LIST_DEFAULT);
            }
            else
            {
                PopulateOptionsScrollList(select); //shows available levels to select
            }
        }
        else if (currentOptionsType == OPTIONS_SELECT_DIRECTORY)
        {
            if (select == NameAll.NULL_UNIT_ID)
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

    void SetDirectoryDefault()
    {
        currentCampaignDirectoryName = "Default Level Directory";
        currentCampaignDirectoryPath = Application.dataPath + "/Custom/Levels/Custom/";
        levelDict = levelLoad.GetMapDict();
        OnClickClear();

    }

    void ChangeCurrentCampaign(int campaignId)
    {
        Debug.Log("changing current campaign");

        CampaignCampaign cc = CalcCode.LoadCampaignCampaign(campaignId);
        currentCampaignDirectoryName = "Directory for " + cc.CampaignName;
        currentIsCampaign = true;
        currentCampaignId = campaignId;

        //sees if directory exists and if not creates it
        string campaignFilePath = Application.dataPath + "/Custom/Levels/Campaign_" + campaignId + "/";
        if (!Directory.Exists(campaignFilePath))
            Directory.CreateDirectory(campaignFilePath);
        //changes the saveDirectory
        currentCampaignDirectoryPath = campaignFilePath;

        //loads the dictionary of current units
        levelDict = CalcCode.LoadLevelDict(isCampaign:true,campaignId:currentCampaignId);
        GetFirstAvailableLevelId();
    }

    //calledin onClickClear and on a directory change, gets the first available Id for saving the new level, also
    void GetFirstAvailableLevelId()
    {
        for (int i = 0; i < 100; i++)
        {
            try
            {
                LevelData ld = levelDict[i];
            }
            catch (Exception e)
            {
                currentLevelId = i;
                currentSaveFilePath = currentCampaignDirectoryPath + "custom_" + currentLevelId + ".dat"; //Debug.Log(" " + currentSaveFilePath);
                break;
            }
        }
    }

    void DeleteLevel()
    {
        if (File.Exists(currentSaveFilePath))
        {
            File.Delete(currentSaveFilePath);
            //intentionally not calling below, in case of user error allows the user to select the deleted level and save it
            //RefreshLevelDict();
        }


    }
    #endregion

 

}
