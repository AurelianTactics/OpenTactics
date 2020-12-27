using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellSlow {

    static int currentId = 0;

    public int CTR { get; set; }
    public int UnitId { get; set; } //caster
    public int SpellIndex { get; set; } //for easy access to the spell
    public int TargetUnitId { get; set; } //if this is not NameAll.NULL_UNIT_ID, targets map
    public int UniqueId { get; private set; }

    public int TargetX { get; set; }
    public int TargetY { get; set; }

    public SpellSlow(CombatTurn turn)
    {
        this.CTR = CalculationAT.CalculateCTR(turn.actor,turn.spellName);
        this.UnitId = turn.actor.TurnOrder;
        this.SpellIndex = turn.spellName.Index;
        this.TargetUnitId = turn.targetUnitId;
        this.TargetX = turn.targetTile.pos.x;
        this.TargetY = turn.targetTile.pos.y;
        this.UniqueId = CreateUniqueId();
    }

    //called in the PunRPC, should only be used for creating duplicates
    public SpellSlow(int zCtr, int zUnitId, int zSpellIndex, int zTargetUnitId, int zTargetX, int zTargetY, int zUniqueId)
    {
        this.CTR = zCtr;
        this.UnitId = zUnitId;
        this.SpellIndex = zSpellIndex;
        this.TargetUnitId = zTargetUnitId;
        this.TargetX = zTargetX;
        this.TargetY = zTargetY;
        this.UniqueId = zUniqueId;
    }

    public SpellSlow(SpellSlow ss)
    {
        this.CTR = CalculationAT.CalculateCTR(PlayerManager.Instance.GetPlayerUnit(ss.UnitId),SpellManager.Instance.GetSpellNameByIndex(ss.SpellIndex)); //
        this.UnitId = ss.UnitId;
        this.SpellIndex = ss.SpellIndex;
        this.TargetUnitId = ss.TargetUnitId;
        this.TargetX = ss.TargetX;
        this.TargetY = ss.TargetY;
        this.UniqueId = CreateUniqueId();
    }

    private int CreateUniqueId()
    {
        currentId += 1;
        return currentId;
    }

    public SpellSlow DecrementCtrAndReturn()
    {
        this.CTR -= 1;
        if (this.CTR == 0)
        {
            return this;
        }
        return null;
    }

    public void DecrementCtrOnly()
    {
        this.CTR -= 1;
    }

}
