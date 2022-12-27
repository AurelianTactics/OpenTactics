using UnityEngine;
using System.Collections;

/// <summary>
/// Phases GameLoop.cs can be in
/// </summary>
public enum Phases
{
    /// <summary>
    /// Each status that is active and has a counter has a tick decremented from it. Statuses fall off and end at 0.
    /// </summary>
    StatusTick = 0,

    /// <summary>
    /// Each SlowAction in the queue has a tick decremented from it. When a SlowAction reaches 0, it resolves in SlowAction
    /// </summary>
    SlowActionTick,

    /// <summary>
    /// Actions that don't resolve on the current turn
    /// </summary>
    SlowAction,

    /// <summary>
    /// Each PU has speed added to their CT stat
    /// </summary>
    CTIncrement,

    /// <summary>
    /// PU is selecting thier turn
    /// </summary>
    ActiveTurn,

    /// <summary>
    /// Turn has ended but some statuses can occur like poison and regen
    /// </summary>
    EndActiveTurn,

    /// <summary>
    /// occurs after slowaction or activeturn
    /// </summary>
    Mime,

    /// <summary>
    /// occurs after slowaction or activeturn and before mime
    /// </summary>
    Reaction,

    /// <summary>
    /// Quick flag causes unit to jump into next ActiveTurn
    /// </summary>
    Quick,

    /// <summary>
    /// used in MP, in GameLoopState waiting for opponent
    /// </summary>
    Standby,

    /// <summary>
    /// used in MP in prior version of this game. Phase GameLoop hangs in prior to phases being started
    /// </summary>
    Prephase,

    /// <summary>
    /// used in WalkAround, gives players time between ticks to input
    /// </summary>
    WaitTick,

    /// <summary>
    /// used in WalkAround, move around the map, can check menus and do actions
    /// </summary>
    NonCombat,

    /// <summary>
    /// sent notification to RL, waiting to receive input options back
    /// </summary>
    RLWait
}

