//initializes the board and units for a particular section of the map
//entered from CombatController (which is attached to scene object)
    //CombatController knows to go here or WalkAround based on something saved in playerprefs
//exits to WalkAroundMainState after setting up the board

//Map Generation and units:
	//Goes through WAMapGenerator then LevelData
	//uses seed and x,y coordinates to generate the map and the map units
	//using PlayerManager's sMapDictionary to save visits to map and currentMap
	//player's units are saved between board transitions by using PlayerManager's sGreenList
	//save and load functions in CalcCode to handle loading and saving of various maps and units
	//map data saved in map dict
	//unit data saved per map (for now basically just which map units have been killed or not), can be expanded


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class WalkAroundInitState : CombatState
{
    //bool isOffline;

    public override void Enter()
    {
        base.Enter();
        //isOffline = PlayerManager.Instance.IsOfflineGame();
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
		ClearMap(); //clears PlayerManager, gameObjects, etc if loading a new map from an old map
        InitBoard(); //loads the level for the board object, spawns the players, sets the initial marker
        yield return null;
        owner.ChangeState<WalkAroundMainState>();



        //AddVictoryCondition();//checks size of team lists, do this before initboard or change the check
        //InitBoard(); //loads the level for the board object, spawns the players, sets the initial marker
        //yield return null;
        //don't want combatcutscene state, walkAround sends its own messages
        //owner.ChangeState<CombatCutSceneState>();

        //board.Load(levelData);
        //Point p = new Point((int)levelData.tiles[0].x, (int)levelData.tiles[0].z);
        //SelectTile(p);
        //SpawnTestUnits();
        //AddVictoryCondition();
        //owner.round = owner.gameObject.AddComponent<TurnOrderController>().Round();
        //yield return null;
        //owner.ChangeState<CutSceneState>();
    }

	//clear board before loading a new board on transition between board
	void ClearMap()
	{
		MapTileManager.Instance.ClearNewBoard();
		PlayerManager.Instance.ClearNewBoard();
		StatusManager.Instance.ClearLists();
		GameObject playerHolder = GameObject.Find("PlayerHolder"); //rather than find could store these game objects in CombatController and destroy by accessing that
		GameObject markerHolder = GameObject.Find("MarkerHolder");
		GameObject tileHolder = GameObject.Find("TileHolder");
		Destroy(playerHolder);//destroy game objects
		Destroy(markerHolder);//destroy game objects
		Destroy(tileHolder);//destroy game objects
	}

	//initialize the board on a new board being loaded
    void InitBoard()
    {
		//these values used for saving/loading maps
		int seed = PlayerManager.Instance.GetWalkAroundSeed(); //sets seeds if seeds not generated, if not returns values needed for seeds
		int timeInt = PlayerManager.Instance.GetWalkAroundTimeInt(); //time int used for saving if using the same seed
		Tuple<int, int> map_xy = PlayerManager.Instance.GetMapXY();
		bool isFirstMapVisit = PlayerManager.Instance.IsFirstMapVisit(map_xy);
		Debug.Log("in init board. map x,y,first visit: " + map_xy.Item1 + ", " + map_xy.Item2 + ", " + isFirstMapVisit);
		LevelData ld;
		ld = LoadLevel(seed, timeInt, map_xy.Item1, map_xy.Item2, isFirstMapVisit); //placeholder until load level code is written
         
        board.Load(ld);
        SpawnUnits(ld.spList, ld.mapUnitList); //spawn player controlled units and units belonging to the map
        Point p = new Point((int)ld.tiles[0].x, (int)ld.tiles[0].y);
        SelectTile(p);
        cameraMain.Open();

		CalcCode.SaveWalkAroundMapData();
		PlayerManager.Instance.SaveWalkAroundPlayerUnits(NameAll.WA_UNIT_SAVE_PLAYER_LIST); //save player's units
		PlayerManager.Instance.AddXYToMapDict(map_xy); //map has now been visited, note this in mapDict. can expand in future

        //LevelData ld = LoadLevel();
        //if( ld == null)
        //{
        //    Debug.Log("couldn't find level, loading main menu");
        //    SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
        //    return;
        //}
        //board.Load(ld);

        ////need to add the PU lists
        //List<PlayerUnit> tempList = new List<PlayerUnit>();
        //SpawnUnits(PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_GREEN),
        //    PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_RED), ld.spList);
        //PlayerManager.Instance.EditTeamLists(null, 0, NameAll.TEAM_LIST_CLEAR);//clears the red and green team lists

        //Point p = new Point((int)ld.tiles[0].x, (int)ld.tiles[0].y);
        //SelectTile(p);

        //cameraMain.Open();
        ////cameraMain.Close();
    }

    LevelData LoadLevel(int seed, int timeInt, int map_x, int map_y, bool isFirstMapVisit)
    {
        WalkAroundMapGenerator wamg = new WalkAroundMapGenerator();
        LevelData ld = wamg.BuildLevel(NameAll.LEVEL_BUILDER_RANDOM, seed, timeInt, map_x, map_y, isFirstMapVisit); //empty string for defaul, can do custom strings for custom maps

        return ld;

        //LevelData ld = null;

        //string levelLoadType = PlayerPrefs.GetString(NameAll.PP_LEVEL_DIRECTORY, NameAll.LEVEL_DIRECTORY_AURELIAN); //Debug.Log("levelLoadType is " + levelLoadType);
        //string levelFileName = Application.dataPath;
        //int levelKey = PlayerPrefs.GetInt(NameAll.PP_COMBAT_LEVEL, 0);
        //if (levelKey >= 1000)
        //    levelKey -= 1000;
        
        //if (levelLoadType == NameAll.LEVEL_DIRECTORY_AURELIAN)
        //{
        //    levelFileName += "/Custom/Levels/Aurelian/custom_" + levelKey + ".dat";
        //}
        //else if (levelLoadType == NameAll.LEVEL_DIRECTORY_CUSTOM)
        //{
        //    levelFileName += "/Custom/Levels/Custom/custom_" + levelKey + ".dat";
        //}
        //else if (levelLoadType == NameAll.LEVEL_DIRECTORY_CAMPAIGN_AURELIAN || levelLoadType == NameAll.LEVEL_DIRECTORY_CAMPAIGN_CUSTOM)
        //{
        //    int campaignId = PlayerPrefs.GetInt(NameAll.PP_COMBAT_CAMPAIGN_LOAD, 0);
        //    var campaignLevel = CalcCode.LoadCampaignLevel(campaignId);
        //    int mapNumber = campaignLevel.GetMap(levelKey); //Debug.Log("loading level campaignId, mapNumber, levelKey are " + campaignId + "," + mapNumber + "," + levelKey);

        //    int battleXP = campaignLevel.GetBattleXP(levelKey);
        //    int battleAP = campaignLevel.GetBattleAP(levelKey);
        //    InitializeCombatStats(battleXP,battleAP);

        //    var tempList = campaignLevel.mapList;
        //    //foreach( int i in tempList)
        //    //{
        //    //    Debug.Log("iterating through mapList, i is " + i);
        //    //}

        //    levelFileName += "/Custom/Levels/Campaign_" + campaignId +"/custom_" + mapNumber + ".dat";
        //}
        //else
        //{
        //    levelFileName += "/Custom/Levels/Aurelian/custom_" + levelKey + ".dat";
        //}

        //if (!File.Exists(levelFileName))
        //{
        //    Debug.Log("No saved map at that location " + levelFileName); //need redundancy in case of failed load
        //    return null;
        //}
        //else
        //{
        //    ld = Serializer.Load<LevelData>(levelFileName); //Debug.Log(levelFileName);
        //    return ld;
        //}
    }

    //grab players from list
    //add players to player manager
    //place players on the board
    //place the playerHolder and markerHolder and add those to the player
    void SpawnUnits(List<SerializableVector3> spawnList, List<PlayerUnit> mapPUList) //bool isPersistent = false, bool isRandomUnits = true
	{
		//Debug.Log("Spawning units for WA mode. Not fully implemented. Need to set up Turn Order, teams, etc.");
		GameObject playerUnitObject;
		List<PlayerUnit> tempUnitList = new List<PlayerUnit>();
		List<PlayerUnit> puList = new List<PlayerUnit>();
		int tempTurnOrder = 0;

		//units saved in sGreenList between map transitions, if no units just generate a random unit
		tempUnitList = PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_WALK_AROUND_GREEN);
		if(tempUnitList.Count == 0)
		{
			Debug.Log("No units found in Green List, generating random units 0");
			WalkAroundMapGenerator wamg = new WalkAroundMapGenerator();
			puList = wamg.GenerateRandomUnits(NameAll.TEAM_ID_WALK_AROUND_GREEN, numUnits: 1);
			if (puList.Count > 0)
			{
				Debug.Log("No units found in Green List, generating random units 1");
				foreach (PlayerUnit p in puList)
				{
					tempUnitList.Add(p);
					Debug.Log("No units found in Green List, generating random units 2");
				}
			}
		}

		//load map playerunits if any
		foreach (PlayerUnit p in mapPUList)
		{
			tempUnitList.Add(p);
			Debug.Log("Adding units from mapPUList");
		}
			

		//if (isPersistent)
		//{
		//	puList = PlayerManager.Instance.GetPersistentUnits();
		//	if(puList.Count > 0)
		//	{
		//		foreach (PlayerUnit p in puList)
		//			tempUnitList.Add(p);
		//	}
		//}

		//if (isRandomUnits)
		//{
		//	//Debug.Log("not generating random units while testing map to map movment");
		//	WalkAroundMapGenerator wamg = new WalkAroundMapGenerator();
		//	puList = wamg.GenerateRandomUnits();
		//	if (puList.Count > 0)
		//	{
		//		foreach (PlayerUnit p in puList)
		//			tempUnitList.Add(p);
		//	}
		//}

		//random player unit for now, can generate more at some point
		Transform playerHolder = new GameObject("PlayerHolder").transform; //holds the graphical playerUnits
        
        PlayerManager.Instance.SetLocalTeamId(NameAll.TEAM_ID_WALK_AROUND_GREEN); //placeholder until I figure out team assignments

        //place each unit on the first available spawnpoint in spawnList
        foreach (PlayerUnit playerUnit in tempUnitList)
        {
			playerUnit.TurnOrder = tempTurnOrder;
			tempTurnOrder += 1;
            //GameObject marker;
            //if (playerUnit.TeamId == NameAll.TEAM_ID_GREEN)
            //{
            //    marker = Instantiate(markerTeam1Prefab) as GameObject;
            //}
            //else
            //{
            //    marker = Instantiate(markerTeam2Prefab) as GameObject;
            //}
            //MapTileManager.Instance.AddMarker(marker); //Debug.Log("adding marker to list...");
            //marker.transform.SetParent(markerHolder);

            string puoString = NameAll.GetPUOString(playerUnit.ClassId); //Debug.Log("asdf " + puoString);
            playerUnitObject = Instantiate(Resources.Load(puoString)) as GameObject; //Debug.Log("is puo active" + playerUnitObject.GetActive());

            foreach (Vector3 vec in spawnList.ToList())
            {
				//Debug.Log("checking spawn points list A1 " + vec.x + " " + vec.y + " " + vec.z + " ");
				if (playerUnit.TeamId == (int)vec.z)
                {
					//Debug.Log("checking spawn points list A2 " + playerUnit.TeamId );
					Point p = new Point((int)vec.x, (int)vec.y);
                    Tile startTile = board.GetTile(p);
                    playerUnit.SetUnitTile(startTile, true);
                    //tells the tiles that someone is on them
                    board.GetTile(playerUnit).UnitId = playerUnit.TurnOrder;
                    spawnList.Remove(vec);
                    Vector3 vecTemp = startTile.transform.position;
                    vecTemp.y = vecTemp.y * 2.0f;
                    playerUnitObject.transform.position = vecTemp;
                    //MapTileManager.Instance.MoveMarker(playerUnit.TurnOrder, startTile);
                    break;
                }
            }
            PlayerUnitObject puo = playerUnitObject.GetComponent<PlayerUnitObject>();
            puo.UnitId = playerUnit.TurnOrder;
            //puo.SetDriver(SetDrivers(playerUnit.TeamId)); 
            PlayerManager.Instance.AddPlayerObject(playerUnitObject);
            playerUnitObject.transform.SetParent(playerHolder);
            PlayerManager.Instance.SetInitialFacingDirection(playerUnit.TurnOrder);
            PlayerManager.Instance.AssignTeamIdToPlayerUnit(playerUnit.TurnOrder);
        }

		foreach (PlayerUnit playerUnit in tempUnitList)
		{
			PlayerManager.Instance.AssignTeamIdToPlayerUnit(playerUnit.TurnOrder);
			playerUnit.SetDriver(SetDrivers(playerUnit.TeamId));
			PlayerManager.Instance.AddPlayerUnit(playerUnit);
			//playerUnit.SetDriver(SetDrivers(playerUnit.TeamId, isRLDriver));
		}

		//sets the teams alliances
		PlayerManager.Instance.AssignAlliances();
        //sets the markers based on each alliance
        SetPlayerUnitMarkers();
        //sets the mime shit
        PlayerManager.Instance.SetMimeOnTeam();
        //adds the lasting statuses
        PlayerManager.Instance.SetLastingStatuses();
        //sets the twosword eligible flag, onmoveeffect flag
        PlayerManager.Instance.InitializePlayerUnits();
        //Debug.Log("" + MapTileMan);

        
    }

    void SetPlayerUnitMarkers()
    {
        Transform markerHolder = new GameObject("MarkerHolder").transform; //holds the graphical markers that indicate the team the playerUnit is own
        GameObject marker;

        var tempPUList = PlayerManager.Instance.GetPlayerUnitList();
        foreach(PlayerUnit pu in tempPUList)
        {
            var alliancesCheck = PlayerManager.Instance.GetAlliances(pu.TeamId);
            if (alliancesCheck == Alliances.Hero)
            {
                marker = Instantiate(Resources.Load("MarkerTeamHero")) as GameObject;
                //marker = Instantiate(markerTeamHeroPrefab) as GameObject;
            }
            else if (alliancesCheck == Alliances.Enemy)
            {
                marker = Instantiate(Resources.Load("MarkerTeamEnemy")) as GameObject;
                //marker = Instantiate(markerTeamEnemyPrefab) as GameObject;
            }
            else if (alliancesCheck == Alliances.Allied)
            {
                marker = Instantiate(Resources.Load("MarkerTeamAllied")) as GameObject;
                //marker = Instantiate(markerTeamAlliedPrefab) as GameObject;
            }
            else
            {
                marker = Instantiate(Resources.Load("MarkerTeamNeutral")) as GameObject;
                //marker = Instantiate(markerTeamNeutralPrefab) as GameObject;
            }
			//Debug.Log("adding marker to list...");
			MapTileManager.Instance.AddMarker(marker); 
            marker.transform.SetParent(markerHolder);

            MapTileManager.Instance.MoveMarker(pu.TurnOrder, PlayerManager.Instance.GetPlayerUnitTile(board,pu.TurnOrder));
        }
        
        
    }

    //void AddVictoryCondition()
    //{
    //    int victoryType;
    //    if(isOffline)
    //    {
    //        victoryType = PlayerPrefs.GetInt(NameAll.PP_VICTORY_TYPE, NameAll.VICTORY_TYPE_DEFEAT_PARTY); //Debug.Log("victory type is " + victoryType );
    //        if (PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_GREEN).Count == 0 || PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_RED).Count == 0)
    //        {
    //            victoryType = NameAll.VICTORY_TYPE_NONE; //Debug.Log("victoryType set to none");
    //        }
    //    }
    //    else
    //    {
    //        victoryType = NameAll.VICTORY_TYPE_DEFEAT_PARTY;
    //    }

    //    CombatVictoryCondition vc = owner.gameObject.AddComponent<CombatVictoryCondition>();
    //    vc.VictoryType = victoryType;

    //}

    //offline: takes into account player preferences
    //online: both sides human
    Drivers SetDrivers(int teamId)
    {
        return Drivers.Human;

        //if (isOffline)
        //{
        //    int aiType = PlayerPrefs.GetInt(NameAll.PP_AI_TYPE, NameAll.AI_TYPE_HUMAN_VS_AI);
        //    int zEntry = PlayerPrefs.GetInt(NameAll.PP_COMBAT_ENTRY, NameAll.SCENE_CUSTOM_GAME);
        //    if (aiType == NameAll.AI_TYPE_HUMAN_VS_AI || zEntry == NameAll.SCENE_STORY_MODE || zEntry == NameAll.SCENE_CAMPAIGNS)
        //    {
        //        if (teamId == NameAll.TEAM_ID_GREEN)
        //            return Drivers.Human;
        //        else
        //            return Drivers.Computer;
        //    }
        //    else if (aiType == NameAll.AI_TYPE_HUMAN_VS_HUMAN)
        //    {
        //        return Drivers.Human;
        //    }
        //    else if (aiType == NameAll.AI_TYPE_AI_VS_AI)
        //    {
        //        return Drivers.Computer;
        //    }
        //    else
        //    {
        //        if (teamId == NameAll.TEAM_ID_GREEN)
        //            return Drivers.Human;
        //        else
        //            return Drivers.Computer;
        //    }
        //}
        //else
        //{
        //    return Drivers.Human;
        //}
        

        
    }

    //void InitializeCombatStats(int xp, int ap)
    //{

    //    int zEntry = PlayerPrefs.GetInt(NameAll.PP_COMBAT_ENTRY, NameAll.SCENE_CUSTOM_GAME);
    //    if (zEntry == NameAll.SCENE_STORY_MODE)
    //    {
    //        PlayerManager.Instance.InitializeCombatStats(false, xp, ap);
    //    }
    //    else
    //    {
    //        PlayerManager.Instance.InitializeCombatStats(true, 0, 0);
    //    }
    //}
}