using UnityEngine;
using System.Collections;

public enum Phases
{
    StatusTick = 0,
    SlowActionTick,
    SlowAction,
    CTIncrement,
    ActiveTurn,
    EndActiveTurn, //poison, regn
    Mime, //after slowaction or activeturn
    Reaction, //same as mime
    Quick,
    Standby, //used in MP, in GameLoopState waiting for opponent
    Prephase, //used in MP, prior to phases being started
    WaitTick, //used in WalkAround, gives players time between ticks to input
    NonCombat, //used in WalkAround, move around the map, can check menus and do actions
	RLWait //sent notification to RL, waiting to receive input options back
}

