using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnObject {

    //custom object fields: actorId, mapTileIndex(have both but only use one or the other), effectXY, effectZ
    int turnId;
    public int actorId; //for tie breakers
    public int targetX;
    public int targetY;
    int effectXY;
    int effectZ;
    string title;
    int teamId;
    public int CTR; //ticks til resolution
	public SpellName spellName;

	public TurnObject(int zOrder, int zActorId, int zTargetX, int zTargetY, int zEffectXY, int zEffectZ, string zTitle, int zCTR, SpellName sn)
    {
        this.turnId = zOrder;
        this.actorId = zActorId;
        this.targetX = zTargetX;
        this.targetY = zTargetY;
        this.effectXY = zEffectXY;
        this.effectZ = zEffectZ;
        this.title = zTitle;
        this.CTR = zCTR;
		this.spellName = sn;
        SetTeamId(this.actorId);
    }

    public TurnObject(ShadowSS sss, int zOrder, int zCTR)
    {
        this.turnId = zOrder;
        this.actorId = sss.actorId;

        this.targetX = sss.targetX;
        this.targetY = sss.targetY;
        this.effectXY = sss.effectXY;
        this.effectZ = sss.effectZ;
        this.title =  sss.name;
        this.CTR = zCTR;
		this.spellName = sss.spellName;
        SetTeamId(this.actorId);
    }

    public TurnObject(ShadowPU spu, int zOrder, int zCTR)
    {
        this.turnId = zOrder;
        this.actorId = spu.actorId;
        this.targetX = spu.targetX;
        this.targetY = spu.targetY;
        this.effectXY = 1;
        this.effectZ = 1919;
        this.title = spu.name + " ("+spu.actorId+")";
        this.CTR = zCTR;
		this.spellName = null;
        SetTeamId(this.actorId);
    }

    //used for inserting a hypothetical ability
    public TurnObject( PlayerUnit pu, SpellName sn, int zCTR, int zTeamId = -19)
    {
        this.turnId = NameAll.NULL_UNIT_ID;
        this.actorId = pu.TurnOrder;
        this.targetX = pu.TileX;
        this.targetY = pu.TileY;
        this.effectXY = sn.EffectXY;
        this.effectZ = sn.EffectZ;
        this.title = "Unselected: " + sn.AbilityName;
        this.CTR = zCTR;
		this.spellName = sn;
        if( zTeamId != NameAll.NULL_UNIT_ID)
        {
            SetTeamId(this.actorId);
        }
        
    }

    public int GetTurnId()
    {
        return this.turnId;
    }
    public int GetActorId()
    {
        return this.actorId;
    }
 
    public int GetEffectXY()
    {
        return this.effectXY;
    }
    public int GetEffectZ()
    {
        return this.effectZ;
    }
    public string GetTitle()
    {
        return this.title;
    }

    public int GetTeamId()
    {
        return this.teamId;
    }

    void SetTeamId( int actorId)
    {
        this.teamId = PlayerManager.Instance.GetPlayerUnit(actorId).TeamId;
    }

    public void AddToTitle(string str)
    {
        this.title += str;
    }

}
