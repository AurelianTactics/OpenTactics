using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using AurelianTactics.BlackBoxRL;

public class CombatStateInit : CombatState
{
    bool isOffline;
	bool isRLMode = false;
	bool isRLBlackBoxMode = false; //testing RL Black Box MODE

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
			if (isRLBlackBoxMode)
			{
				Debug.Log("WARNING: starting in RLBlackBoxMode, json load will override starting settings");
				owner.isRLBlackBoxMode = true;
				owner.worldTimeManager = new WorldTimeManager();
				StartCoroutine(InitRLBlackBox(owner.isFirstCombatInit));
				

			}
			else
			{
				//standard combat scene
				StartCoroutine(Init());
			}
			
			
		}
	}

	//to do: way to start combat scene without the ienumerator
	//set the drivers and control of units and the render mode from the config
	//use the config to set board size, unit types etc
	//be able to load a game from midway
	//not ahve to read from start for the games starting stuff
		//way too much work done every reset that should only be done once
	IEnumerator InitRLBlackBox(bool isFirstInit)
	{
		//Load text from a JSON file (Assets/Resources/JSONFiles/default_combat.json)
		var jsonTextFile = Resources.Load<TextAsset>("JSONFiles/default_combat").ToString();
		//Then use JsonUtility.FromJson<T>() to deserialize jsonTextFile into an object
		var envConfig = JsonUtility.FromJson<RLEnvConfig>(jsonTextFile);

		LevelData ld;

		//load the board
		if (isFirstInit)
		{
			//add victory conditions
			AddVictoryCondition(NameAll.VICTORY_TYPE_RL_RESET_EPISODE);
			//TO DO for now assumes victory type is defeat other part. at some point add other conditons
			//int victoryType = NameAll.VICTORY_TYPE_DEFEAT_PARTY;
			//AddVictoryCondition(victoryType, isFirstCombatInit);

			//to do: read from envConfig
			ld = new LevelData(2, 2);
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
		//SpawnUnits(puListGreen, puListRed, spawnList, isRLDriver:true);
		Debug.Log("changed spawn stuff, need to rewrite this if using it");

		yield return null;

		owner.isFirstCombatInit = false;
		owner.ChangeState<GameLoopState>();
	}

	IEnumerator Init()
    {
		Debug.Log("render mode is " + owner.renderMode);
        AddVictoryCondition();//checks size of team lists, do this before initboard or change the check
        InitBoard(); //loads the level for the board object, spawns the players, sets the initial marker
        yield return null;
		owner.isFirstCombatInit = false;
		if(owner.renderMode == NameAll.PP_RENDER_NONE)
			owner.ChangeState<GameLoopState>();
		else
			owner.ChangeState<CombatCutSceneState>();

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
		board.spawnList = ld.spList;
        //need to add the PU lists
        List<PlayerUnit> tempList = new List<PlayerUnit>();
		calcMono.SpawnUnits(board, PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_GREEN), PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_RED), markerTeam1Prefab,
			markerTeam2Prefab, isOffline, owner.isRLMode, true, owner.renderMode);
        //SpawnUnits(PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_GREEN),
        //    PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_RED), ld.spList, isRLMode, renderMode);
        //PlayerManager.Instance.EditTeamLists(null, 0, NameAll.TEAM_LIST_CLEAR);//clears the red and green team lists

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


	//clear map before resetting. used only in RL mode I think
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

	#region Reinforcement Learning Inits
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
		//SpawnUnits(puListGreen, puListRed, spawnList, isRLDriver:true);
		Debug.Log("changed spawn stuff, need to rewrite this if using it");

		yield return null;

		owner.isFirstCombatInit = false;
		owner.ChangeState<GameLoopState>();
	}


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
		board.SetTilePickUp(x, y, true, owner.renderMode);

		//randomly place unit any other place than goal andd populates units in player manager
		//SpawnUnitsGridworld(gridworldPUList, boardXLength, boardYLength);
		Debug.Log("changed spawn stuff, need to rewrite this if using it");

		Point p = new Point((int)ld.tiles[0].x, (int)ld.tiles[0].y);
		SelectTile(p);
		cameraMain.Open();

		board.isGridworldBoardSet = true;
		owner.isFirstCombatInit = false;
	}
	#endregion
}