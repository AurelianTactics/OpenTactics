using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Combat and WalkAround scenes function by going from state to state
/// depending on what is occuring in the game.
/// Inherited by most states in Combat and WalkAround scenes
/// Has much functionality that other states rely on easy access to
/// input listeners, menus for pop ups,
/// Basically any object that you want to access in any states can be put in this
/// and then inherited in any states.
/// </summary>
public abstract class CombatState : State
{
	protected CombatController owner;

	/// <summary>
	/// I think the Drivers control AI
	/// </summary>
	protected Drivers driver;

	/// <summary>
	/// Controls the camera
	/// </summary>    
	public UICameraMenu cameraMain { get { return owner.cameraMain; } }

	/// <summary>
	/// The board contains the tiles and represents the game map
	/// </summary>
	public Board board { get { return owner.board; } }

	/// <summary>
	/// An indicator that appears around the current tile/point of focus
	/// </summary>
	public Transform tileSelectionIndicator { get { return owner.tileSelectionIndicator; } }

	/// <summary>
	/// The current point that is of focus. Points sometimes carry over between states.
	/// </summary>
	public Point pos { get { return owner.pos; } set { owner.pos = value; } }

	/// <summary>
	/// The main focused tile. Sometimes focused tile carries over between states.
	/// </summary>
	public Tile currentTile { get { return owner.currentTile; } }

	/// <summary>
	/// Panel that shows the actor's abilities.
	/// </summary>
	public UIAbilityScrollList abilityMenu { get { return owner.abilityMenu; } }

	/// <summary>
	/// Panel that shows actor details. 
	/// </summary>
	public CombatUITarget actorPanel { get { return owner.actorPanel; } }

	/// <summary>
	/// Panel that shows the target details. 
	/// </summary>
	public CombatUITarget targetPanel { get { return owner.targetPanel; } }

	/// <summary>
	/// Panel that previews details like the effect of an action.
	/// </summary>
	public CombatUITarget previewPanel { get { return owner.previewPanel; } }

	/// <summary>
	/// Menu for selecting actions 
	/// </summary>
	public UIActiveTurnMenu activeMenu { get { return owner.activeMenu; } }

	/// <summary>
	/// Details on what the current actor is doing are stored in the turn object.
	/// This info carries over between states.
	/// </summary>
	public CombatTurn turn { get { return owner.turn; } }

	/// <summary>
	/// used in WA mode, idea is to have a local turn for multiplayer games as multiple turn inputs can occure
	/// </summary>
	public CombatTurn localTurn { get { return owner.localTurn; } }

	/// <summary>
	/// Marks team 1 objects 
	/// </summary>
	public GameObject markerTeam1Prefab { get { return owner.markerTeam1Prefab; } }

	/// <summary>
	/// Marks team 2 units
	/// </summary>
	public GameObject markerTeam2Prefab { get { return owner.markerTeam2Prefab; } }

	/// <summary>
	/// Shared Unity Mono class used for various calculations.
	/// </summary>
	public CalculationMono calcMono { get { return owner.calcMono; } }

	/// <summary>
	/// Combat scene can be called in different game modes which affect
	/// some of the rules of the game.
	/// </summary>
	public int combatMode { get { return owner.combatMode; } }

	/// <summary>
	/// When Combat is entered the first time, it is different than when combat
	/// is entered multiple times in certain game modes.
	/// </summary>
	public bool isFirstCombatInit { get { return owner.isFirstCombatInit; } }

	/// <summary>
	/// 
	/// </summary>
	protected virtual void Awake()
	{
		owner = GetComponent<CombatController>();
	}

	/// <summary>
	/// Adds listeners to listen for button clicks
	/// </summary>
	protected override void AddListeners()
	{
		if (driver == Drivers.None || driver == Drivers.Human)
		{
			InputController.moveEvent += OnMove;
			InputController.fireEvent += OnFire;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	protected override void RemoveListeners()
	{
		InputController.moveEvent -= OnMove;
		InputController.fireEvent -= OnFire;
	}

	/// <summary>
	/// Called when entering a state
	/// </summary>
	public override void Enter()
	{
		Debug.Log("entering state");
		//driver = (turn.actor != null) ? turn.actor.GetComponent<Driver>() : null;
		if (turn.actor != null)
		{
			driver = PlayerManager.Instance.GetPlayerUnit(turn.actor.TurnOrder).puDriver;
		}
		else
		{
			driver = Drivers.None;
		}
		base.Enter();
	}

	/// <summary>
	/// Called when a directional button is pressed.
	/// </summary>
	protected virtual void OnMove(object sender, InfoEventArgs<Point> e)
	{

	}

	/// <summary>
	/// Called when a button is pressed.
	/// </summary>
	protected virtual void OnFire(object sender, InfoEventArgs<int> e)
	{

	}

	/// <summary>
	/// Select the tile by moving the tile indicator to that position
	/// </summary>
	/// <param>
	/// <c>p</c> the Point containing the coordinates to center the tile indicator.
	/// </param>
	protected virtual void SelectTile(Point p)
	{
		//Debug.Log("1: SelectTile " + p.x + "," + p.y);
		if (pos == p || !board.tiles.ContainsKey(p))
			return;
		//Debug.Log("2: SelectTile " + p.x + "," + p.y);
		pos = p;
		tileSelectionIndicator.localPosition = board.tiles[p].center;
	}

	/// <summary>
	/// Select the tile of the PlayerUnit.
	/// </summary>
	/// <param>
	/// <c>unit</c> the PlayerUnit whose tile should be selected
	/// </param>
	protected virtual void SelectTile(PlayerUnit unit)
	{
		Point p = new Point(unit.TileX, unit.TileY);
		SelectTile(p);
	}

	/// <summary>
	/// Change the shade of the tile to highlight/un-highlight the tile of actor.
	/// Actor is the PlayerUnit that currently is performing their turn.
	/// </summary>
	/// <param>
	/// <c>pu</c> the PlayerUnit that is the current actor.
	/// </param>
	/// <param>
	/// <c>isHighlight</c> should the tile be highlighted or un-highlighted
	/// </param>
	protected virtual void HighlightActorTile(PlayerUnit pu, bool isHighlight)
	{
		Point p = new Point(pu.TileX, pu.TileY);
		board.HighlightTile(p, pu.TeamId, isHighlight);
	}


	/// <summary>
	/// Update the stat panel
	/// </summary>
	/// <param>
	/// <c>Point p</c> is the point to get details from on the Tile/Unit
	/// </param>
	protected virtual void RefreshPrimaryStatPanel(Point p)
	{
		//Unit target = GetUnit(p);
		//if (target != null)
		//    statPanelController.ShowPrimary(target.gameObject);
		//else
		//    statPanelController.HidePrimary();
	}

	/// <summary>
	/// Update the stat panel
	/// </summary>
	/// <param>
	/// <c>Point p</c> is the point to get details from on the Tile/Unit
	/// </param>
	protected virtual void RefreshSecondaryStatPanel(Point p)
	{
		//Unit target = GetUnit(p);
		//if (target != null)
		//    statPanelController.ShowSecondary(target.gameObject);
		//else
		//    statPanelController.HideSecondary();
	}

	/// <summary>
	/// Check if victory conditions have been met and combat is over.
	/// </summary>
	protected virtual bool IsBattleOver()
	{
		//var zBool = owner.GetComponent<CombatVictoryCondition>().Victor != Teams.None;
		//Debug.Log("testing for is battle over " + zBool);
		return owner.GetComponent<CombatVictoryCondition>().Victor != Teams.None;
	}

	#region
	// Deprecated
	//protected virtual Unit GetUnit(Point p)
	//{
	//    Tile t = board.GetTile(p);
	//    GameObject content = t != null ? t.content : null;
	//    return content != null ? content.GetComponent<Unit>() : null;
	//}
	//public List<Unit> units { get { return owner.units; } }
	//public HitSuccessIndicator hitSuccessIndicator { get { return owner.hitSuccessIndicator; } }
	//public Turn turn { get { return owner.turn; } }
	//public StatPanelController statPanelController { get { return owner.statPanelController; } }
	//public AbilityMenuPanelController abilityMenuPanelController { get { return owner.abilityMenuPanelController; } }
	//public LevelData levelData { get { return owner.levelData; } }
	//protected Driver driver;
	//public CameraRig cameraRig { get { return owner.cameraRig; } }
	//shouldn't be using this, should be relying on teams, if single player in future modify appropriately
	//protected virtual bool DidPlayerWin()
	//{
	//    return owner.GetComponent<BaseVictoryCondition>().Victor == Alliances.Hero;
	//}


	#endregion
}