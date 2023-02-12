using UnityEngine;
using System.Collections;

/// <summary>
/// ActiveTurn Phases the unit selecting the turn can be
/// </summary>
public enum CombatActiveTurnPhases
{
    /// <summary>
    /// Select between move, ability, wait
    /// </summary>
    TopSelectPhase = 0,

    /// <summary>
    /// Select direction to end turn
    /// </summary>
    WaitDirectionSelectPhase = 1,

    /// <summary>
    /// Select movement tile
    /// </summary>
    MoveTileSelectPhase = 2,

    /// <summary>
    /// Select ability
    /// </summary>
    AbilitySelectPhase = 3,

    /// <summary>
    /// Select the tile the ability targets
    /// </summary>
    AbilityTileSelectPhase = 4,

    /// <summary>
    /// Select between targeting tile or unit on abilities that can target either
    /// </summary>
    AbilityTileUnitSelectPhase = 5,

    /// <summary>
    /// Some abilities can continue into next turn. Continue the ability or not
    /// </summary>
    ContinueLastAbilitySelectPhase = 6,

    /// <summary>
    /// No phase
    /// </summary>
    None = 7,
}

