using UnityEngine;
using System.Collections;

/// <summary>
/// ActiveTurn Phases the unit selecting the turn can be
/// </summary>
public enum CombatActiveTurnPhases
{
	/// <summary>
	/// New active turn assigned
	/// </summary>
    NewActiveTurn,

	/// <summary>
	/// Select between move, ability, wait
	/// </summary>
	TopSelectPhase,

    /// <summary>
    /// Select direction to end turn
    /// </summary>
    WaitDirectionSelectPhase,

    /// <summary>
    /// Select movement tile
    /// </summary>
    MoveTileSelectPhase,

    /// <summary>
    /// Select ability
    /// </summary>
    AbilitySelectPhase,

    /// <summary>
    /// Select the tile the ability targets
    /// </summary>
    AbilityTileSelectPhase,

    /// <summary>
    /// Select between targeting tile or unit on abilities that can target either
    /// </summary>
    AbilityTileUnitSelectPhase,

    /// <summary>
    /// Some abilities can continue into next turn. Continue the ability or not
    /// </summary>
    ContinueLastAbilitySelectPhase,

	/// <summary>
	/// ActiveTurn is over
	/// </summary> 
    TurnCompleted,

	/// <summary>
	/// No phase
	/// </summary>
	None,
}

