using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellReaction
{

    public int ActorId {get; set; } //reactor
    public int SpellIndex { get; set; }  //for easy access to the spell
    public int TargetId { get; set; }  //unit reacting on for reactions and target tile for MimeQueue
    public int Effect { get; set; }

    private int targetX;
    public int TargetX
    {
        get { return targetX; }
        set { targetX = value; }
    }

    private int targetY;
    public int TargetY
    {
        get { return targetY; }
        set { targetY = value; }
    }

    public SpellReaction(int zUnitId, int zSpellIndex, int zTargetUnitId, int zEffect = 0)
    {
        this.ActorId = zUnitId;
        this.SpellIndex = zSpellIndex;
        this.TargetId = zTargetUnitId;
        this.Effect = zEffect;
        //targets a unit, don't need the coordinates
        this.TargetX = NameAll.NULL_INT;
        this.TargetY = NameAll.NULL_INT;
    }

    public SpellReaction(int zUnitId, int zSpellIndex, Tile t, int zEffect = 0)
    {
        this.ActorId = zUnitId;
        this.SpellIndex = zSpellIndex;
        this.TargetId = NameAll.NULL_UNIT_ID;//targets a tile, don't need the targetId
        this.Effect = zEffect;
        this.TargetX = t.pos.x;
        this.TargetY = t.pos.y;
    }



    //public int GetActorId()
    //{
    //    return actorId;
    //}

    //public int GetSpellIndex()
    //{
    //    return spellIndex;
    //}

    //public int GetTargetId()
    //{
    //    return targetId;
    //}

    //public int GetEffect()
    //{
    //    return effect;
    //}

}

//used in Multiplayer. P2 does not get the actual SpellReaction but wants to show some details from the cast before the result comes in
public class ReactionDetails
{
    public int ActorId { get; set; } //reactor
    public int SpellIndex { get; set; }  //for easy access to the spell in case want to show all target tiles
    public int TargetX { get; set; }
    public int TargetY { get; set; }
    public string DisplayName { get; set; } //reaction/mime and spell name

    public ReactionDetails( int actorId, int spellIndex, int targetX, int targetY, string displayName)
    {
        this.ActorId = actorId;
        this.SpellIndex = spellIndex;
        this.TargetX = targetX;
        this.TargetY = targetY;
        this.DisplayName = displayName;
    }
}
