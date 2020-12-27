using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;
//using UnityEditor;
using System;

public class LevelLoad : MonoBehaviour {

    //loading level
    [SerializeField]
    private Dropdown levelDropdownSelect;
    [SerializeField]
    private Text labelText;
    private string levelFilePathName; //used to check if path exists and load
    private string levelFileResourceName = "Levels/Aurelian/level_0"; //used to load in resources string
    private string currentLevelName = "";
    string currentLevelPath = "Aurelian"; //which package to load a level from, loaded from PlayerPrefs

    Dictionary<int, string> levelDict;
    List<string> levelList;
    string levelSelectName; //used to dropdown list
    string innerLevelSelectName; //used to dropdown list
    int maxDefaultLevels = 5; //levels that come with the game
    int maxCustomLevels = 25; //max number of levels player can edit and save
    LevelData myLevelData;
    List<Vector3> spawnPointsList = new List<Vector3>();
    int team2SpawnPoints = 0;
    int team3SpawnPoints = 0;

    private int LOAD_DEFAULT_LEVELS = 0;
    private int LOAD_ALL_LEVELS = 1;
    private int LOAD_CUSTOM_LEVELS = 2;
    private int levelType = 0;

    //PhotonView photonView;

    void Awake()
    {
        
        //photonView = PhotonView.Get(this);
        currentLevelPath = PlayerPrefs.GetString(NameAll.PP_LEVEL_DIRECTORY, "Aurelian"); //get the correct Levels/... directory
    }

    public LevelData GetLevelData()
    {
        return myLevelData;
    }

    public void InitializeLevelDropdown(int loadType)
    {
        string zString = "level_0";// Debug.Log("testing 1");
        if ( loadType == LOAD_ALL_LEVELS || loadType == LOAD_CUSTOM_LEVELS)
        {
            levelType = loadType; //Debug.Log("level type is " + levelType);
            if( levelType == LOAD_CUSTOM_LEVELS)
            {
                zString = "custom_0";
            }
        }
        else
        {
            loadType = LOAD_DEFAULT_LEVELS;
        }
        //currentLevelName = zString;
        SetLevelDropdowns();
        SetFileNameForLevelLoad(zString, true); //checks for playerprefs default first in certain maps
        LoadLevel();
        
        //Debug.Log(levelFileResourceName);
    }

    public void SetFileNameForLevelLoad(string levelString, bool isAtStart = false)
    {
        levelFileResourceName = "/Custom/Levels/" + currentLevelPath + "/" + levelString;

        if ( levelType != LOAD_CUSTOM_LEVELS)
        {
            int z1;
            z1 = levelDict.Values.ToList().IndexOf(levelString); //levelList.IndexOf(levelDict[levelString]);
            levelDropdownSelect.value = z1;
            //Debug.Log("level type is " + levelType + levelString);
        }
        
        string filePath = Application.dataPath + "/Custom/Levels/" + currentLevelPath;
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);
        levelFilePathName = filePath + "/" + levelString + ".dat";

        currentLevelName = levelString;
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("UpdateDraftDropdown", PhotonTargets.Others, new object[] { currentLevelName });
        //}
    }

    void SetLevelDropdowns()
    {
        levelDropdownSelect.onValueChanged.AddListener(delegate {
            myLevelDropdownValueChangedHandler(levelDropdownSelect);
        });
        levelDropdownSelect.options.Clear();
        levelDict = new Dictionary<int, string>();
        levelList = new List<string>();
        LevelDropdownListLoad();
        levelDropdownSelect.AddOptions(levelList);
    }

    void LevelDropdownListLoad()
    {
        string zString;

        if( levelType == LOAD_DEFAULT_LEVELS || levelType == LOAD_ALL_LEVELS)
        {
            for (int i = 0; i < maxDefaultLevels; i++)
            {
                zString = "level_" + i;
                SetFileNameForLevelDropdown(zString);
                if (!IsLevelLoadable(levelSelectName))
                {
                    return;
                }
                levelDict.Add(i, zString);
                levelList.Add(zString);
            }
        }

        if (levelType == LOAD_CUSTOM_LEVELS || levelType == LOAD_ALL_LEVELS)
        {
            for (int i = maxDefaultLevels; i < maxCustomLevels; i++)
            {
                zString = "custom_" + (i-maxDefaultLevels);
                SetFileNameForLevelDropdown(zString);
                if( levelType != LOAD_CUSTOM_LEVELS) //only allow actual levels to be loaded
                {
                    if (!IsLevelLoadable(levelSelectName))
                    {
                        return;
                    }
                }
                levelDict.Add(i, zString);
                levelList.Add(zString);
            }
        }
    }

    void SetFileNameForLevelDropdown(string levelString)
    {
        levelSelectName = Application.dataPath + "/Custom/Levels/" + currentLevelPath + "/" + levelString + ".dat";
    }

    //if the level exists, allow it to be loaded
    public bool IsLevelLoadable()
    {
        if (File.Exists(levelFilePathName))
        {
            //Debug.Log("No saved characters");
            return true;
        }
        return false;
    }

    bool IsLevelLoadable(string str)
    {
        if (File.Exists(str))
        {
            //Debug.Log("No saved characters");
            return true;
        }
        return false;
    }

    void LoadLevel()
    {
        
        if (!File.Exists(levelFilePathName))
        {
            Debug.Log("No saved map at that location" + levelFilePathName);
            return;
        }
        myLevelData = Serializer.Load<LevelData>(levelFilePathName);
        if( myLevelData == null)
        {
            Debug.Log("level data still not loadable");
        }
        //Debug.Log("asdf" + myLevelData);//+ "asdf" + myLevelData.spList.Count);
        CountSpawnPoints();
    }

    public List<int> GetSpawnPoints()
    {
        List<int> tempList = new List<int>();
        tempList.Add(team2SpawnPoints);
        tempList.Add(team3SpawnPoints);
        return tempList;
    }

    //set the spawn point numbers based on the map
    private void CountSpawnPoints()
    {
        team2SpawnPoints = 0;
        team3SpawnPoints = 0;
        foreach (SerializableVector3 v in myLevelData.spList)
        {
            int z1 = (int)v.z;
            TallySpawnPoints(1, z1);
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

    private void myLevelDropdownValueChangedHandler(Dropdown target)
    {
        //Debug.Log("in select value changed handler start");
        string zString = levelList[target.value];
        foreach (KeyValuePair<int, string> pair in levelDict)
        {
            //Debug.Log("in select value changed handler " + pair.Value);
            if (pair.Value == zString)
            {
                //z1 = pair.Key; // Found
                //Debug.Log("in select value changed handler " + zString);
                SetFileNameForLevelLoad(zString); //sets the file name which will be loaded in load default 
                LoadLevel();
                break;
            }
        }
        //Debug.Log("in select value changed handler end ");
    }

    //called in mapCreator, saves the level
    public void SaveLevel(LevelData board, string filePath )
    {
        //Debug.Log("saving level " + filePath); //Debug.Log("board sp size is " + board.spList.Count + " -- " + board.tiles.Count);// adsf //asdf asdf
        Serializer.Save<LevelData>(filePath, board);
    }

    

    //called in mapCreator, finds the first slot to save a map in then saves it
    //public string SaveLevelByPath(LevelData board, string pathName )
    //{
    //    if( pathName == NameAll.MAP_SAVE_CUSTOM_DEFAULT)
    //    {
    //        for (int i = 0; i < maxCustomLevels; i++)
    //        {
    //            string filePath = Application.dataPath + "/Levels/Custom/custom_" + i + ".dat"; //Debug.Log("file path is " + filePath);
    //            if (!File.Exists(filePath))
    //            {
    //                pathName = filePath;
    //                break;
    //            }
    //        }

    //        if (pathName == NameAll.MAP_SAVE_CUSTOM_DEFAULT)
    //            return NameAll.MAP_SAVE_CUSTOM_DEFAULT;

    //        Debug.Log("saving level " + pathName); //Debug.Log("board sp size is " + board.spList.Count + " -- " + board.tiles.Count);// adsf //asdf asdf
    //        Serializer.Save<LevelData>(pathName, board);
    //        return pathName;
    //    }
    //    else
    //    {
    //        Debug.Log("saving level for campaign" + pathName); //Debug.Log("board sp size is " + board.spList.Count + " -- " + board.tiles.Count);// adsf //asdf asdf
    //        Serializer.Save<LevelData>(pathName, board);
    //        return pathName;
    //    }
    //}

    //for draft modes or MP when level cannot be changed
    public void Toggle(bool showDropdown = true)
    {
        if (showDropdown)
        {
            labelText.text = "Level:";
            levelDropdownSelect.gameObject.SetActive(true);
        }
        else
        {
            levelDropdownSelect.gameObject.SetActive(false);
            labelText.text = "Level: " + currentLevelName;
            //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
            //{
            //    photonView.RPC("UpdateDraftDropdown", PhotonTargets.Others, new object[] { currentLevelName });
            //}
        }
    }

    //[PunRPC]
    public void UpdateDraftDropdown(string name)
    {
        labelText.text = "Level: " + name;
    }

    //accessed in building a campaing
    public List<Vector3> GetSpawnPointsForCustomMap(int mapId)
    {
        List<Vector3> retValue = new List<Vector3>();
        string filePath = Application.dataPath + "/Custom/Levels/Custom/custom_" +mapId + ".dat";
        if (!File.Exists(filePath))
        {
            //Debug.Log("No saved map at that location " + filePath);
            //return;
        }
        else
        {
            myLevelData = Serializer.Load<LevelData>(filePath);
            if (myLevelData != null)
            {
                foreach( SerializableVector3 sv in myLevelData.spList)
                {
                    retValue.Add(new Vector3(sv.x,sv.y,sv.z));
                }
            }
        }
        return retValue;     
    }

    public string GetMapName(int mapId)
    {
        string retValue = "no map found";
        string filePath = Application.dataPath + "/Custom/Levels/Custom/custom_" + mapId + ".dat"; //Debug.Log(filePath);
        if (!File.Exists(filePath))
        {
            retValue = "no map found"; //Debug.Log("map not found");
        }
        else
        {
            myLevelData = Serializer.Load<LevelData>(filePath); //Debug.Log("map loading 1");
            if (myLevelData != null)
            {
                retValue = myLevelData.levelName; //Debug.Log("map loading 2");
            }
        }
        return retValue;
    }

    public Dictionary<int,LevelData> GetMapDict()
    {
        var retValue = new Dictionary<int,LevelData>();
        for (int i = 0; i < maxCustomLevels; i++)
        {
            string filePath = Application.dataPath + "/Custom/Levels/Custom/custom_" + i + ".dat";
            if (File.Exists(filePath))
            {
                myLevelData = Serializer.Load<LevelData>(filePath); //Debug.Log("map loading 1");
                if (myLevelData != null)
                {
                    retValue.Add(i,myLevelData);
                }
            }
        }
        return retValue;
    }

    
}
