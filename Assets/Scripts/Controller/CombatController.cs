//attached to object in scenes: Combat and WalkAround
//contains many objects that are globally accessible in both modes
    //these objects tie into CombatState. need to declare them ehre and set methods in CombatState
//reads from PlayerPrefs to either go to InitCombatState or WalkAroundInitState


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AurelianTactics.BlackBoxRL;

public class CombatController : StateMachine
{
    //public CameraRig cameraRig;
    public UICameraMenu cameraMain;
    public Board board;
    //public LevelData levelData; //Might be able to comment this out
    public Transform tileSelectionIndicator;
    public Point pos;
    public Tile currentTile { get { return board.GetTile(pos); } }
    //public AbilityMenuPanelController abilityMenuPanelController; //can comment this out later, using activeMenu
    public UIAbilityScrollList abilityMenu;
    public UIActiveTurnMenu activeMenu;
    //public StatPanelController statPanelController; //can comment this out later, using actorPanel
    public CombatUITarget actorPanel;
    public CombatUITarget targetPanel;
    public CombatUITarget previewPanel;
    
    //public HitSuccessIndicator hitSuccessIndicator; //using preview panel
    public BattleMessageController battleMessageController;
    public FacingIndicator facingIndicator;
    //public Turn turn = new Turn();
    public CombatTurn turn = new CombatTurn();
    public CombatTurn localTurn = new CombatTurn();//each player gets their own localTurn
    //public List<Unit> units = new List<Unit>();
    //public IEnumerator round;
    public CombatComputerPlayer cpu;
    public CalculationMono calcMono; //allows access to CalculationMono functions from any state

    public GameObject markerTeam1Prefab;
    public GameObject markerTeam2Prefab;
	//public GameObject markerTeamHeroPrefab;
	//public GameObject markerTeamAlliedPrefab;
	//public GameObject markerTeamEnemyPrefab;
	//public GameObject markerTeamNeutralPrefab;

	public int combatMode; //helps with various game logic depending on how combat was entered and what game mode it is in
	public bool isFirstCombatInit; //allows reseting from CombatStateInit
	public int renderMode; //none for faster game play
	public bool isRLMode; //true sets RL drivers for units
	//RL black box related parts
	public bool isRLBlackBoxMode; //RL env is a blackbox communicating with an outside source
	public WorldTimeManager worldTimeManager; //when isRLBlackBoxMode is true; manager RL stuff through this object
	public RLBlackBoxActions blackBoxActions; //when isRLBlackBoxMode is true helps handle actions

    void Start()
    {
		//cpu = new CombatComputerPlayer();
		isFirstCombatInit = true;
        int initState = PlayerPrefs.GetInt(NameAll.PP_INIT_STATE, NameAll.INIT_STATE_COMBAT);
		renderMode = PlayerPrefs.GetInt(NameAll.PP_RENDER_MODE, NameAll.PP_RENDER_NORMAL);
		isRLMode = PlayerPrefs.GetInt(NameAll.PP_RL_MODE, NameAll.PP_RL_MODE_FALSE) != 0;
		isRLBlackBoxMode = false;
		PlayerManager.Instance.SetRenderMode(renderMode);

		PlayerManager.Instance.SetGameMode(initState); //sets game mode so no longer have to read off of PP
        if (initState == NameAll.INIT_STATE_COMBAT)
        {
			//Debug.Log("starting combat, not WA mode");
            StatusManager.Instance.SetDeathMode(NameAll.DEATH_MODE_DEATH);
            ChangeState<CombatStateInit>();
        }
        else
        {
			//If no time int data is in PlayerPrefs, then create new map data. If time_int is in PP then load that
			int mapSeed = PlayerPrefs.GetInt(NameAll.PP_WA_MAP_SEED, 0);
			int timeInt = PlayerPrefs.GetInt(NameAll.PP_WA_MAP_TIME_INT, 0);
			if( timeInt == 0) //new game
			{
				timeInt = (int)Time.time;
				PlayerManager.Instance.InitializeMapDictionary(mapSeed, timeInt);
			}
			else //load game
			{
				CalcCode.LoadWalkAroundMapData(mapSeed, timeInt); //load map data
				var mapXY = PlayerManager.Instance.GetMapXY();
				var puList = CalcCode.LoadWalkAroundPlayerUnitList(mapSeed, timeInt, mapXY.Item1, mapXY.Item2, NameAll.WA_UNIT_SAVE_PLAYER_LIST); //load player's unit data
				PlayerManager.Instance.SetTeamList(puList, NameAll.TEAM_ID_WALK_AROUND_GREEN);
			}
			StatusManager.Instance.SetDeathMode(NameAll.DEATH_MODE_UNCONSCIOUS);
            ChangeState<WalkAroundInitState>();
        }
        
    }
}