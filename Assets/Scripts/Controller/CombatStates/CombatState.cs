using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CombatState : State
{
    protected CombatController owner;
    protected Drivers driver;
    //protected Driver driver;
    //public CameraRig cameraRig { get { return owner.cameraRig; } }
    public UICameraMenu cameraMain { get { return owner.cameraMain; } }
    public Board board { get { return owner.board; } }
    //public LevelData levelData { get { return owner.levelData; } }
    public Transform tileSelectionIndicator { get { return owner.tileSelectionIndicator; } }
    public Point pos { get { return owner.pos; } set { owner.pos = value; } }
    public Tile currentTile { get { return owner.currentTile; } }
    //public AbilityMenuPanelController abilityMenuPanelController { get { return owner.abilityMenuPanelController; } }
    public UIAbilityScrollList abilityMenu { get { return owner.abilityMenu; } }
    //public StatPanelController statPanelController { get { return owner.statPanelController; } }
    public CombatUITarget actorPanel { get { return owner.actorPanel; } }
    public CombatUITarget targetPanel { get { return owner.targetPanel; } }
    public CombatUITarget previewPanel { get { return owner.previewPanel; } }
    public UIActiveTurnMenu activeMenu { get { return owner.activeMenu; } }
    //public HitSuccessIndicator hitSuccessIndicator { get { return owner.hitSuccessIndicator; } }
    //public Turn turn { get { return owner.turn; } }
    public CombatTurn turn { get { return owner.turn; } }
    public CombatTurn localTurn { get { return owner.localTurn; } } //used in WA mode, idea is to have a local turn for multiplayer games as multiple turn inputs
    //public List<Unit> units { get { return owner.units; } }

    public GameObject markerTeam1Prefab { get { return owner.markerTeam1Prefab; } }
    public GameObject markerTeam2Prefab { get { return owner.markerTeam2Prefab; } }
    public CalculationMono calcMono { get { return owner.calcMono; } }

	public int combatMode { get { return owner.combatMode; } }
	public bool isFirstCombatInit { get { return owner.isFirstCombatInit; } }

	protected virtual void Awake()
    {
        owner = GetComponent<CombatController>();
    }

    protected override void AddListeners()
    {
        if (driver == Drivers.None || driver == Drivers.Human)
        {
            InputController.moveEvent += OnMove;
            InputController.fireEvent += OnFire;
        }
    }

    protected override void RemoveListeners()
    {
        InputController.moveEvent -= OnMove;
        InputController.fireEvent -= OnFire;
    }

    public override void Enter()
    {
		Debug.Log("entering state");
        //driver = (turn.actor != null) ? turn.actor.GetComponent<Driver>() : null;
        if( turn.actor != null)
        {
			driver = PlayerManager.Instance.GetPlayerUnit(turn.actor.TurnOrder).puDriver;
        }
        else
        {
            driver = Drivers.None;
        }
        base.Enter(); 
    }

    protected virtual void OnMove(object sender, InfoEventArgs<Point> e)
    {

    }

    protected virtual void OnFire(object sender, InfoEventArgs<int> e)
    {

    }

    protected virtual void SelectTile(Point p)
    {
        //Debug.Log("1: SelectTile " + p.x + "," + p.y);
        if (pos == p || !board.tiles.ContainsKey(p))
            return;
        //Debug.Log("2: SelectTile " + p.x + "," + p.y);
        pos = p;
        tileSelectionIndicator.localPosition = board.tiles[p].center;
    }

    protected virtual void SelectTile(PlayerUnit unit)
    {
        Point p = new Point(unit.TileX, unit.TileY);
        SelectTile(p);
    }

    protected virtual void HighlightActorTile(PlayerUnit pu, bool isHighlight)
    {
        Point p = new Point(pu.TileX, pu.TileY);
        board.HighlightTile(p, pu.TeamId, isHighlight);
    }

    //protected virtual Unit GetUnit(Point p)
    //{
    //    Tile t = board.GetTile(p);
    //    GameObject content = t != null ? t.content : null;
    //    return content != null ? content.GetComponent<Unit>() : null;
    //}

    protected virtual void RefreshPrimaryStatPanel(Point p)
    {
        //Unit target = GetUnit(p);
        //if (target != null)
        //    statPanelController.ShowPrimary(target.gameObject);
        //else
        //    statPanelController.HidePrimary();
    }

    protected virtual void RefreshSecondaryStatPanel(Point p)
    {
        //Unit target = GetUnit(p);
        //if (target != null)
        //    statPanelController.ShowSecondary(target.gameObject);
        //else
        //    statPanelController.HideSecondary();
    }

    //shouldn't be using this, should be relying on teams, if single player in future modify appropriately
    //protected virtual bool DidPlayerWin()
    //{
    //    return owner.GetComponent<BaseVictoryCondition>().Victor == Alliances.Hero;
    //}

    protected virtual bool IsBattleOver()
    {
		//var zBool = owner.GetComponent<CombatVictoryCondition>().Victor != Teams.None;
		//Debug.Log("testing for is battle over " + zBool);
        return owner.GetComponent<CombatVictoryCondition>().Victor != Teams.None;
    }
}