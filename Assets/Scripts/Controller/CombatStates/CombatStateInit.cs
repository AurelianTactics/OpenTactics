using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class CombatStateInit : CombatState
{
    bool isOffline;

    public override void Enter()
    {
        base.Enter();
        isOffline = PlayerManager.Instance.IsOfflineGame();

		var m_Scene = SceneManager.GetActiveScene();
		if( m_Scene.name == "GridworldAT")
		{
			owner.combatMode = NameAll.COMBAT_MODE_DEFAULT;
			//gridworld specific
			int boardXLength = 5;
			int boardYLength = 5;
			InitGridworldLevel(boardXLength, boardYLength);
		}
		else if(m_Scene.name == "DuelRL")
		{
			//Debug.Log("am in duelRL");
			owner.combatMode = NameAll.COMBAT_MODE_RL_DUEL;
			//duel specific
			int boardXLength = 2;
			int boardYLength = 2;
			StartCoroutine(InitDuel(owner.isFirstCombatInit, boardXLength, boardYLength));
		}
		else
		{
			owner.combatMode = NameAll.COMBAT_MODE_DEFAULT;
			//standard combat scene
			StartCoroutine(Init());
		}
	}

	#region RLDuel
	// init board and set up reinforcement learning mode for duel
	IEnumerator InitDuel(bool isFirstInit, int boardXLength, int boardYLength)
	{
		LevelData ld;

		//load the board
		if (isFirstInit)
		{
			//add victory conditions
			AddVictoryCondition(NameAll.VICTORY_TYPE_RL_RESET_EPISODE);

			ld = new LevelData(boardXLength, boardYLength);
			board.Load(ld);

			Point p = new Point((int)ld.tiles[0].x, (int)ld.tiles[0].y);
			SelectTile(p);
			cameraMain.Open();
		}
		else
		{
			//add victory conditions
			AddVictoryCondition(NameAll.VICTORY_TYPE_RL_RESET_EPISODE, isFirstInit);

			//already been to CombatStateInit before, doing some clean up before resetting parts of the map
			ClearMap();
		}

		//create the playerUnits, one each on green and red
		List<PlayerUnit> puListGreen = new List<PlayerUnit>();
		List<PlayerUnit> puListRed = new List<PlayerUnit>();
		//knight with no equipment or ability
		puListGreen.Add(new PlayerUnit(NameAll.VERSION_CLASSIC, true, NameAll.CLASS_KNIGHT, true, 10, true));
		puListRed.Add(new PlayerUnit(NameAll.VERSION_CLASSIC, true, NameAll.CLASS_KNIGHT, true, 10, true));
		//Debug.Log("TESTING WITH BRAVE = 100, TURN THIS OFF, also TESTING WITH RANDOM ACTIONS TURN THIS OFF TOO");
		puListGreen[0].SetStatUnitBrave(100);
		//puListRed[0].SetStatUnitBrave(100);
		puListGreen[0].UnitName = "greenA";
		puListRed[0].UnitName = "redB";

		// create the spawn points
		List<SerializableVector3> spawnList = new List<SerializableVector3>();
		spawnList.Add(new SerializableVector3(0, 0, NameAll.TEAM_ID_GREEN));
		spawnList.Add(new SerializableVector3(1, 0, NameAll.TEAM_ID_RED));

		//void SpawnUnits(List<PlayerUnit> green, List<PlayerUnit> red, List<SerializableVector3> spawnList)
		//Debug.Log("setting drivers to RL drivers");
		SpawnUnits(puListGreen, puListRed, spawnList, isRLDriver:true);

		yield return null;
		
		owner.isFirstCombatInit = false;
		owner.ChangeState<GameLoopState>();
	}



	//clear map before resetting
	void ClearMap()
	{
		MapTileManager.Instance.ClearNewBoard(); //clear static lists
		PlayerManager.Instance.ClearDuelRLBoard();
		StatusManager.Instance.ClearLists(); //clear static lists
											 //do not need to clear spells in this case since no slow actions in this mode
		board.ClearUnitId(); //clear unitIds from the map
		GameObject playerHolder = GameObject.Find("PlayerHolder"); //rather than find could store these game objects in CombatController and destroy by accessing that
		GameObject markerHolder = GameObject.Find("MarkerHolder");
		Destroy(playerHolder);//destroy game objects, reset with new units
		Destroy(markerHolder);//destroy game objects, reset with new marker holders

		//GameObject tileHolder = GameObject.Find("TileHolder");

		//delete children
		//foreach(Transform child in playerHolder.transform)
		//{
		//	GameObject.Destroy(child.gameObject);
		//}
		//foreach (Transform child in markerHolder.transform)
		//{
		//	GameObject.Destroy(child.gameObject);
		//}
		//Destroy(playerHolder);//destroy game objects, reset with new units
		//Destroy(markerHolder);//destroy game objects, reset with new marker holders
		//Destroy(tileHolder);//destroy game objects
	}
	#endregion

	// init board and set up reinforcement learning mode for GridworldAT
	void InitGridworldLevel(int boardXLength, int boardYLength)
	{

		//load the board
		LevelData ld = new LevelData(boardXLength, boardYLength); 
		board.Load(ld);

		//create the playerUnit
		List<PlayerUnit> gridworldPUList = new List<PlayerUnit>();
		gridworldPUList.Add(new PlayerUnit(NameAll.VERSION_CLASSIC, true));

		//place goal marker
		int x = UnityEngine.Random.Range(0, boardXLength);
		int y = UnityEngine.Random.Range(0, boardYLength);
		board.SetTilePickUp(x, y, true);

		//randomly place unit any other place than goal andd populates units in player manager
		SpawnUnitsGridworld(gridworldPUList, boardXLength, boardYLength);

		Point p = new Point((int)ld.tiles[0].x, (int)ld.tiles[0].y);
		SelectTile(p);
		cameraMain.Open();

		board.isGridworldBoardSet = true;
		owner.isFirstCombatInit = false;
	}

    IEnumerator Init()
    {
        AddVictoryCondition();//checks size of team lists, do this before initboard or change the check
        InitBoard(); //loads the level for the board object, spawns the players, sets the initial marker
        yield return null;
		owner.isFirstCombatInit = false;
		owner.ChangeState<CombatCutSceneState>();

        //board.Load(levelData);
        //Point p = new Point((int)levelData.tiles[0].x, (int)levelData.tiles[0].z);
        //SelectTile(p);
        //SpawnTestUnits();
        //AddVictoryCondition();
        //owner.round = owner.gameObject.AddComponent<TurnOrderController>().Round();
        //yield return null;
        //owner.ChangeState<CutSceneState>();
    }

    void InitBoard()
    {

        LevelData ld = LoadLevel();
        if( ld == null)
        {
            Debug.Log("couldn't find level, loading main menu");
            SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
            return;
        }
        board.Load(ld);

        //need to add the PU lists
        List<PlayerUnit> tempList = new List<PlayerUnit>();
        SpawnUnits(PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_GREEN),
            PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_RED), ld.spList);
        PlayerManager.Instance.EditTeamLists(null, 0, NameAll.TEAM_LIST_CLEAR);//clears the red and green team lists

        Point p = new Point((int)ld.tiles[0].x, (int)ld.tiles[0].y);
        SelectTile(p);
        
        cameraMain.Open();
        //cameraMain.Close();
    }

    LevelData LoadLevel()
    {
        LevelData ld = null;

        string levelLoadType = PlayerPrefs.GetString(NameAll.PP_LEVEL_DIRECTORY, NameAll.LEVEL_DIRECTORY_AURELIAN); //Debug.Log("levelLoadType is " + levelLoadType);
        string levelFileName = Application.dataPath;
        int levelKey = PlayerPrefs.GetInt(NameAll.PP_COMBAT_LEVEL, 0);
        if (levelKey >= 1000)
            levelKey -= 1000;
        
        if (levelLoadType == NameAll.LEVEL_DIRECTORY_AURELIAN)
        {
            levelFileName += "/Custom/Levels/Aurelian/custom_" + levelKey + ".dat";
        }
        else if (levelLoadType == NameAll.LEVEL_DIRECTORY_CUSTOM)
        {
            levelFileName += "/Custom/Levels/Custom/custom_" + levelKey + ".dat";
        }
        else if (levelLoadType == NameAll.LEVEL_DIRECTORY_CAMPAIGN_AURELIAN || levelLoadType == NameAll.LEVEL_DIRECTORY_CAMPAIGN_CUSTOM)
        {
            int campaignId = PlayerPrefs.GetInt(NameAll.PP_COMBAT_CAMPAIGN_LOAD, 0);
            var campaignLevel = CalcCode.LoadCampaignLevel(campaignId);
            int mapNumber = campaignLevel.GetMap(levelKey); //Debug.Log("loading level campaignId, mapNumber, levelKey are " + campaignId + "," + mapNumber + "," + levelKey);

            int battleXP = campaignLevel.GetBattleXP(levelKey);
            int battleAP = campaignLevel.GetBattleAP(levelKey);
            InitializeCombatStats(battleXP,battleAP);

            var tempList = campaignLevel.mapList;
            //foreach( int i in tempList)
            //{
            //    Debug.Log("iterating through mapList, i is " + i);
            //}

            levelFileName += "/Custom/Levels/Campaign_" + campaignId +"/custom_" + mapNumber + ".dat";
        }
        else
        {
            levelFileName += "/Custom/Levels/Aurelian/custom_" + levelKey + ".dat";
        }

        if (!File.Exists(levelFileName))
        {
            Debug.Log("No saved map at that location " + levelFileName); //need redundancy in case of failed load
            return null;
        }
        else
        {
            ld = Serializer.Load<LevelData>(levelFileName); //Debug.Log(levelFileName);
            return ld;
        }
    }

    void SpawnUnits(List<PlayerUnit> green, List<PlayerUnit> red, List<SerializableVector3> spawnList, bool isRLDriver = false)
    {
      
        List<PlayerUnit> temp = new List<PlayerUnit>(); 
        PlayerUnit pu;
        Transform playerHolder = new GameObject("PlayerHolder").transform;
		PlayerManager.Instance.SetLocalTeamId(NameAll.TEAM_ID_GREEN); 
		Transform markerHolder = new GameObject("MarkerHolder").transform;
        GameObject playerUnitObject;

        int zBreak = 0; //temp
        int turn = NameAll.TEAM_ID_GREEN; //green goes first, then snake draft
        int unitsToAdd = 1; //first time only adds one unit, then 2 (snake draft)
        int currentIndex = 0;
        int greenLearnedIndex = 0; //have to change the list in SpellManager based on the new turn order. old number is stored by order in the list
        //set up turn order for PlayerUnits
        while (green.Count > 0 || red.Count > 0)
        {
            if (turn == NameAll.TEAM_ID_GREEN)
            {
                turn = NameAll.TEAM_ID_RED;
                if (green.Count > 0)
                {
                    while (unitsToAdd > 0 && green.Count > 0)
                    {
                        //for some modes need to only let player cast learned abilities. abilities loaded but have to modify the uniqueId with the turnOrder
                        //so change the oldTurnOrder with the newly assigned turnOrder (the old turn order is a place holder set when the spells are added to the spellmanager)
                        if (SpellManager.Instance.spellLearnedType == NameAll.SPELL_LEARNED_TYPE_PLAYER_1)
                            SpellManager.Instance.AlterSpellLearnedList(currentIndex, green[0].TurnOrder);

                        unitsToAdd -= 1;
                        pu = new PlayerUnit(green[0]);
                        green.RemoveAt(0);
                        pu.TurnOrder = currentIndex; // SetTurn_order(currentIndex);
                        pu.TeamId = NameAll.TEAM_ID_GREEN; // SetTeam_id(NameAll.TEAM_ID_GREEN);
                        temp.Add(pu);
                        temp[currentIndex] = pu; //redundant
                        currentIndex += 1;

                        
                    }
                }
                else
                {
                    continue;
                }
            }
            else
            {
                turn = NameAll.TEAM_ID_GREEN;
                if (red.Count > 0)
                {
                    while (unitsToAdd > 0 && red.Count > 0)
                    {
                        unitsToAdd -= 1;
                        pu = new PlayerUnit(red[0]);
                        red.RemoveAt(0);
                        pu.TurnOrder = currentIndex;// SetTurn_order(currentIndex);
                        pu.TeamId = NameAll.TEAM_ID_RED;// SetTeam_id(NameAll.TEAM_ID_RED);
                        temp.Add(pu);
                        temp[currentIndex] = pu;
                        currentIndex += 1;
                    }
                }
                else
                {
                    continue;
                }
            }
            unitsToAdd = 2;
            zBreak += 1;
            if (zBreak > 100)
            {
                break;
            }
            //Debug.Log("in while loop " + green.Count + " " + red.Count);
        }

        //physically place the units on the map
        //spList is x, y, and teamId
        foreach (PlayerUnit playerUnit in temp)
        {
            GameObject marker;
            if (playerUnit.TeamId == NameAll.TEAM_ID_GREEN)
            {
                marker = Instantiate(markerTeam1Prefab) as GameObject;
            }
            else
            {
                marker = Instantiate(markerTeam2Prefab) as GameObject;
            }
            MapTileManager.Instance.AddMarker(marker); //Debug.Log("adding marker to list...");
            marker.transform.SetParent(markerHolder);

            string puoString = NameAll.GetPUOString(playerUnit.ClassId); //Debug.Log("asdf " + puoString);
            playerUnitObject = Instantiate(Resources.Load(puoString)) as GameObject; //Debug.Log("is puo active" + playerUnitObject.GetActive());

            foreach (Vector3 vec in spawnList.ToList())
            {
                if (playerUnit.TeamId == (int)vec.z)
                {
                    Point p = new Point((int)vec.x, (int)vec.y);
                    Tile startTile = board.GetTile(p);
                    playerUnit.SetUnitTile(startTile, true);
                    //tells the tiles that someone is on them
                    board.GetTile(playerUnit).UnitId = playerUnit.TurnOrder;

                    spawnList.Remove(vec);
                    Vector3 vecTemp = startTile.transform.position;
                    vecTemp.y = vecTemp.y * 2.0f;
                    playerUnitObject.transform.position = vecTemp;
                    MapTileManager.Instance.MoveMarker(playerUnit.TurnOrder,startTile);
                    break;
                }
            }

            PlayerManager.Instance.AddPlayerUnit(playerUnit);
            PlayerUnitObject puo = playerUnitObject.GetComponent<PlayerUnitObject>();
            puo.UnitId = playerUnit.TurnOrder;
            puo.SetDriver(SetDrivers(playerUnit.TeamId, isRLDriver));
            PlayerManager.Instance.AddPlayerObject(playerUnitObject);
            playerUnitObject.transform.SetParent(playerHolder);
            PlayerManager.Instance.SetInitialFacingDirection(playerUnit.TurnOrder);
			PlayerManager.Instance.AssignTeamIdToPlayerUnit(playerUnit.TurnOrder);

		}
		//sets the teams alliances
		PlayerManager.Instance.AssignAlliances();
		//sets the mime shit
		PlayerManager.Instance.SetMimeOnTeam();
        //adds the lasting statuses
        PlayerManager.Instance.SetLastingStatuses();
        //sets the twosword eligible flag, onmoveeffect flag
        PlayerManager.Instance.InitializePlayerUnits();
        //Debug.Log("" + MapTileMan);
    }

	//spawns units for the gridworld set up
	void SpawnUnitsGridworld(List<PlayerUnit> puList, int boardXLength, int boardYLength)
	{
		Transform playerHolder = new GameObject("PlayerHolder").transform;
		//PlayerManager.Instance.SetLocalTeamId(NameAll.TEAM_ID_GREEN);
		//Transform markerHolder = new GameObject("MarkerHolder").transform;
		GameObject playerUnitObject;
		int currentIndex = 0;
		foreach (PlayerUnit playerUnit in puList)
		{
			playerUnit.TurnOrder = currentIndex;
			currentIndex++;
			//initialize the physical representation of the player unit
			string puoString = NameAll.GetPUOString(playerUnit.ClassId); //Debug.Log("asdf " + puoString);
			playerUnitObject = Instantiate(Resources.Load(puoString)) as GameObject; //Debug.Log("is puo active" + playerUnitObject.GetActive());

			//randomize unit starting spot
			while (true)
			{
				int x = UnityEngine.Random.Range(0, boardXLength);
				int y = UnityEngine.Random.Range(0, boardYLength);
				//check that starting spot is empty
				if( !board.IsCrystalOnTile(x,y))
				{
					//tile is empty, place unit
					Tile startTile = board.GetTile(x,y);
					playerUnit.SetUnitTile(startTile, true);
					//tells the tiles that someone is on them
					board.GetTile(playerUnit).UnitId = playerUnit.TurnOrder;
					Vector3 vecTemp = startTile.transform.position;
					vecTemp.y = vecTemp.y * 2.0f;
					playerUnitObject.transform.position = vecTemp;
					//MapTileManager.Instance.MoveMarker(playerUnit.TurnOrder, startTile);
					break;
				}
			}
			

			PlayerManager.Instance.AddPlayerUnit(playerUnit);
			PlayerUnitObject puo = playerUnitObject.GetComponent<PlayerUnitObject>();
			puo.UnitId = playerUnit.TurnOrder;
			PlayerManager.Instance.AddPlayerObject(playerUnitObject);
			playerUnitObject.transform.SetParent(playerHolder);
			PlayerManager.Instance.SetInitialFacingDirection(playerUnit.TurnOrder);
		}
		
	}

    void AddVictoryCondition(int victoryInt = -1919, bool isFirstInit = true)
    {
		int victoryType;
		if(isFirstInit)
		{
			CombatVictoryCondition vc = owner.gameObject.AddComponent<CombatVictoryCondition>();
			if (victoryInt != NameAll.NULL_INT)
			{
				victoryType = victoryInt;
				vc.Victor = Teams.None;
			}
			else
			{
				if (isOffline)
				{
					victoryType = PlayerPrefs.GetInt(NameAll.PP_VICTORY_TYPE, NameAll.VICTORY_TYPE_DEFEAT_PARTY); //Debug.Log("victory type is " + victoryType );
					if (PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_GREEN).Count == 0 || PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_RED).Count == 0)
					{
						victoryType = NameAll.VICTORY_TYPE_NONE; //Debug.Log("victoryType set to none");
					}
				}
				else
				{
					victoryType = NameAll.VICTORY_TYPE_DEFEAT_PARTY;
				}
			}
			vc.VictoryType = victoryType;
		}
		else
		{	
			owner.GetComponent<CombatVictoryCondition>().Victor = Teams.None;
		}
		
    }

    //offline: takes into account player preferences
    //online: both sides human
    Drivers SetDrivers(int teamId, bool isRLDriver = false)
    {
		//Debug.Log("setting RL driver " + isRLDriver);
		//AI is trainable by Machine Learning, set drivers accordingly
		if (isRLDriver) 
			return Drivers.ReinforcementLearning;

		//Debug.Log("setting RL driver after " + isRLDriver);

		if (isOffline)
        {
            int aiType = PlayerPrefs.GetInt(NameAll.PP_AI_TYPE, NameAll.AI_TYPE_HUMAN_VS_AI);
            int zEntry = PlayerPrefs.GetInt(NameAll.PP_COMBAT_ENTRY, NameAll.SCENE_CUSTOM_GAME);
            if (aiType == NameAll.AI_TYPE_HUMAN_VS_AI || zEntry == NameAll.SCENE_STORY_MODE || zEntry == NameAll.SCENE_CAMPAIGNS)
            {
                if (teamId == NameAll.TEAM_ID_GREEN)
                    return Drivers.Human;
                else
                    return Drivers.Computer;
            }
            else if (aiType == NameAll.AI_TYPE_HUMAN_VS_HUMAN)
            {
                return Drivers.Human;
            }
            else if (aiType == NameAll.AI_TYPE_AI_VS_AI)
            {
                return Drivers.Computer;
            }
            else
            {
                if (teamId == NameAll.TEAM_ID_GREEN)
                    return Drivers.Human;
                else
                    return Drivers.Computer;
            }
        }
        else
        {
            return Drivers.Human;
        }
        

        
    }

    void InitializeCombatStats(int xp, int ap)
    {
        int zEntry = PlayerPrefs.GetInt(NameAll.PP_COMBAT_ENTRY, NameAll.SCENE_CUSTOM_GAME);
        if( zEntry == NameAll.SCENE_STORY_MODE)
        {
            PlayerManager.Instance.InitializeCombatStats(false,xp, ap);
        }
        else
        {
            PlayerManager.Instance.InitializeCombatStats(true,0,0);
        }
    }
}