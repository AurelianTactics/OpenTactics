//In WalkAround mode not set turn orders. PlayerManager has a queue, as actions are decided they are coded as CombatTurns, then changed into WalkAroundActionObjects and added to the queue
//object for PlayerUnit actions in WalkAround mode
//held in a queue in PlayerManager, which sorts through them for various turns

using UnityEngine;
using System.Collections;

public class WalkAroundActionObject {
    public CombatTurn turn;
    public Tile moveTile;

    public WalkAroundActionObject(CombatTurn tu)
    {
        this.turn = tu; //have to have a turn so I have the playerunitId
        this.moveTile = tu.walkAroundMoveTile;
    }

    public int GetTurnOrder()
    {
        return this.turn.actor.TurnOrder;
    }

    public void UpdateWalkAroundObject(CombatTurn tu)
    {
        this.turn = tu;
        this.moveTile = tu.walkAroundMoveTile;
    }
}
