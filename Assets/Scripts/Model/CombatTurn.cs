using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
To do
fill out the doc strings particularly for parameters
 */

/// <summary>
/// When a PlayerUnit has a turn, a full turn consists of many components
/// across many States. This object stores the details used to execute a turn.
/// </summary>
public class CombatTurn
{
	/// <summary>
	/// The unit that has the active turn
	/// </summary>
	public PlayerUnit actor;

	public bool hasUnitMoved;
	public bool hasUnitActed;

	/// <summary>
	/// The unitId of the target. May be null if not yet selected or if
	/// ability is targetting the map
	/// </summary>
	public int targetUnitId; //targets unit rather than targetting the map

	/// <summary>
	/// Tiles targetted by the ability. can be empty
	/// </summary>
	public List<Tile> targets;

	/// <summary>
	/// AI turns use a plan to execute actions
	/// </summary>
	public CombatPlanOfAttack plan;

	/// <summary>
	/// Starting direction unit is facing. Current direction of PlayerUnit
	/// may change as potential turns are considered, thus need to store the
	/// starting direction.
	/// </summary>
	Directions startDir;

	/// <summary>
	/// End direction of acting PlayerUnit. also used for targetting for line/cone
	/// and directional abilities
	/// </summary>
	public Directions endDir; //also used for targetting dir for line/cone abilities

	/// <summary>
	/// Is the actor using the primary or secondary skill set
	/// </summary>
	public bool isPrimary; //using primary/secondary

	/// <summary>
	/// SpellName object of ability actor has selected.
	/// </summary>
	public SpellName spellName;

	/// <summary>
	/// Second ability actor has selected. Some turns can have chained abilities.
	/// </summary>
	public SpellName spellName2;
	//used to see if mime/reaction shortcut, ie mid turn mime/reaction interrupt. 
	//starts at 0. set to 1 after the activeUnit performs an action
	//gameLoop will then check reaction/mime shit and if it sees a phaseStart = 1 afterwards then it will go back to the unit's midTurn shit


	/// <summary>
	/// to do: use the enum? is this used anywhere?
	/// to do: better explanation
	/// PhaseStart int is used in GameLoop.cs to track what part of the game loop
	/// the game is in. This is a copy of that int. Needed here because of reaction and
	/// mime abilities can alter the flow of actions.
	/// Starts at 0. Set to 1 after the active PlayerUnit performs an action.
	/// GameLoop.cs will then check reaction/mime stuff and if it sees a phaseStart = 1 
	/// afterwards then it will go back to the unit's midTurn checks
	/// </summary>
	//public int phaseStart; DEPRECATED

	/// <summary>
	/// If the ability targets a tile, the tile that is being targetted.
	/// </summary>
	public Tile targetTile;

	/// <summary>
	/// walkaround mode, tile that unit is going to move to
	/// </summary>
	public Tile walkAroundMoveTile;

	/// <summary>
	/// walkaround mode, is unit moving or acting first?
	/// </summary>
	public bool isWalkAroundMoveFirst;

	/// <summary>
	/// Stores what phase of the activeturn the unit is in
	/// </summary>
	public CombatActiveTurnPhases activeTurnPhase;

	public CombatTurn() { }

	/// <summary>
	/// used in slowactionstate for slowaction resolve
	/// </summary>
	public CombatTurn(SpellSlow ss, Board board)
	{
		this.actor = PlayerManager.Instance.GetPlayerUnit(ss.UnitId);
		this.spellName = SpellManager.Instance.GetSpellNameByIndex(ss.SpellIndex);
		Point p;
		if (ss.TargetUnitId != NameAll.NULL_UNIT_ID)
		{
			PlayerUnit targetPU = PlayerManager.Instance.GetPlayerUnit(ss.TargetUnitId);
			p = new Point(targetPU.TileX, targetPU.TileY);
		}
		else
		{
			p = new Point(ss.TargetX, ss.TargetY);
		}

		this.targetTile = board.GetTile(p);
		CombatAbilityArea caa = new CombatAbilityArea();
		this.targets = caa.GetTilesInArea(board, this.actor, this.spellName, this.targetTile, this.actor.Dir);

	}

	/// <summary>
	/// used in MimeState and ReactionState to create a CombatTurn
	/// </summary>
	public CombatTurn(SpellReaction sr, Board board, bool isMimeSpellReaction)
	{
		if (isMimeSpellReaction)
		{
			//SpellReaction created in CalculationResolveAction AddToMimeQueue
			//SpellReaction sr = new SpellReaction(mimePU.TurnOrder, sn.Index, mimeTargetTile);
			this.actor = PlayerManager.Instance.GetPlayerUnit(sr.ActorId);
			this.spellName = SpellManager.Instance.GetSpellNameByIndex(sr.SpellIndex);
			this.targetTile = board.GetTile(sr.TargetX, sr.TargetY);
			CombatAbilityArea caa = new CombatAbilityArea();
			this.targets = caa.GetTilesInArea(board, this.actor, this.spellName, this.targetTile, this.actor.Dir);
		}
		else
		{
			this.actor = PlayerManager.Instance.GetPlayerUnit(sr.ActorId);
			this.spellName = SpellManager.Instance.GetSpellNameByIndex(sr.SpellIndex);
			this.targetTile = board.GetTile(PlayerManager.Instance.GetPlayerUnit(sr.TargetId));
			CombatAbilityArea caa = new CombatAbilityArea();
			this.targets = caa.GetTilesInArea(board, this.actor, this.spellName, this.targetTile, this.actor.Dir);
		}

	}

	/// <summary>
	/// resets the CombatTurn so that nothing carries over from prior CombatTurn inputs.
	/// In WA mode called as the unit is selected
	/// </summary>
	public void Change(PlayerUnit current)
	{
		actor = current;
		hasUnitMoved = false;
		hasUnitActed = false;
		//lockMove = false;
		startDir = actor.Dir;
		endDir = actor.Dir;
		plan = null;
		isPrimary = true;
		spellName = null;
		spellName2 = null;
		targetUnitId = NameAll.NULL_UNIT_ID;
		//phaseStart = 0;
		targetTile = null;
		this.walkAroundMoveTile = null;
		this.isWalkAroundMoveFirst = false;
		this.activeTurnPhase = CombatActiveTurnPhases.NewActiveTurn;
	}

	/// <summary>
	/// Some statuses alters a CombatTurn component.
	/// Ie a can't move status, means a unit does not get to move this turn
	/// </summary>
	public void CheckStatuses()
	{
		if (this.hasUnitMoved == false && StatusManager.Instance.IfStatusByUnitAndId(this.actor.TurnOrder, NameAll.STATUS_ID_DONT_MOVE, true))
		{
			this.hasUnitMoved = true;
		}
		if (this.hasUnitActed == false && StatusManager.Instance.IfStatusByUnitAndId(this.actor.TurnOrder, NameAll.STATUS_ID_DONT_ACT, true))
		{
			this.hasUnitActed = true;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public void UndoMove()
	{
		Debug.Log("Undo Move not implemented");
		//hasUnitMoved = false;
		//actor.Place(startTile);
		//actor.dir = startDir;
		//actor.Match();
	}
}

/// <summary>
/// Old code from when Multiplayer used to work. Not implemented
/// master intepreting other's move. sends this in a notification so master can proceed to the correct state
/// </summary>
public class CombatMultiplayerTurn
{
	//    communicate a move(actorId, tileX, tileY)
	//communicate a wait(simple RPC: unitId and direction int)
	//communicate an action(unitId, targetId (or targetX and targetY), maybe some sort of

	//direction is needed, spellIndex, possibly spellIndex2)
	public bool IsMove { get; set; }
	public bool IsAct { get; set; }
	public bool IsWait { get; set; }

	public int ActorId { get; set; } //used in all of them
	public int TileX { get; set; } //move tileX or action targetTileX
	public int TileY { get; set; } //move tileY or action targetTileY
	public int DirectionInt { get; set; } //direction on a wait or for some actions

	public int TargetId { get; set; }
	public int SpellIndex { get; set; }
	public int SpellIndex2 { get; set; } //some actions have two spellIndices (Mathskill)

	public CombatMultiplayerTurn(bool zIsMove, bool zIsAct, bool zIsWait, int zActorId, int zTileX, int zTileY, int zDirectionInt,
		int zTargetId, int zSpellIndex, int zSpellIndex2)
	{
		this.IsMove = zIsMove;
		this.IsAct = zIsAct;
		this.IsWait = zIsWait;

		this.ActorId = zActorId;
		this.TileX = zTileX;
		this.TileY = zTileY;
		this.DirectionInt = zDirectionInt;

		this.TargetId = zTargetId;
		this.SpellIndex = zSpellIndex;
		this.SpellIndex2 = zSpellIndex2;
	}
}

/// <summary>
/// Old code from when Multiplayer used to work. Not implemented
/// </summary>
public class CombatMultiplayerMove
{
	// communicates the actual move that was done and passed from master to P2

	public int ActorId { get; set; } //used in all of them
	public int TileX { get; set; } //move tileX or action targetTileX
	public int TileY { get; set; } //move tileY or action targetTileY
	public bool IsClassicClass { get; set; }
	public int SwapUnitId { get; set; } //unit being swapped. if == NameAll.NULL_INT then no swap
	public bool IsKnockback { get; set; }//unit is not moving but is being knockedback

	public CombatMultiplayerMove(int actorId, int tileX, int tileY, bool isClassicClass, int swapUnitId, bool isKnockback)
	{
		this.ActorId = actorId;
		this.TileX = tileX;
		this.TileY = tileY;
		this.IsClassicClass = isClassicClass;
		this.SwapUnitId = swapUnitId;
		this.IsKnockback = isKnockback;
	}
}