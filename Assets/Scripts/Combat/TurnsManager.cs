using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//produces the turns object for the turnmenu
//in the future maybe list the upcoming turns more easily
//in the future have potential turns be shown in their place in this
//in future might want to keep the always updated rather thand scratch adn build from start all the time
//in future shade by team
//in future show the effect panels with better info (caster immune etc)

public class ShadowPU //fake PU units for generating the turns list
{
    //for now just doing speed and ct
    //in future statuses that effect speed
    //in future statuses that effect ability to get a turn (dead, etc)
    public int speed;
    public int ct;
    public string name;
    public int targetX;
    public int targetY;
    public int actorId; //f
    //not needed
    //int effectXY;
    //int effectZ;
    public ShadowPU(PlayerUnit pu)
    {
        this.speed = pu.StatTotalSpeed;
        this.ct = pu.CT;
        this.targetX = pu.TileX;
        this.targetY = pu.TileY;
        this.name = pu.UnitName;
        this.actorId = pu.TurnOrder;
    }
}

//public TurnObject(int zOrder, int zActorId, int zIndex, int zEffectXY, int zEffectZ, string zTitle)
//{
//    this.turnId = zOrder;
//    this.actorId = zActorId;
//    this.mapTileIndex = zIndex;
//    this.effectXY = zEffectXY;
//    this.effectZ = zEffectZ;
//    this.title = zTitle;
//}

public class ShadowSS //fake SpellSlows from teh spell slow queue
{
    public int ctr;
    public int actorId;
    public string name;
    public int targetX;
    public int targetY;
    public int effectXY;
    public int effectZ;
    public int uniqueId;
	public SpellName spellName;

    public ShadowSS(SpellSlow ss)
    {
        this.ctr = ss.CTR;
        this.actorId = ss.UnitId;
		this.spellName = SpellManager.Instance.GetSpellNameByIndex(ss.SpellIndex);
        this.name = spellName.AbilityName;
        this.targetX = ss.TargetX;
        this.targetY = ss.TargetY;
        this.effectXY = spellName.EffectXY;
        this.effectZ = spellName.EffectZ;
        this.uniqueId = ss.UniqueId;
    }
}

public class TurnsManager : Singleton<TurnsManager>
{ 

    private static List<TurnObject> sTurnsList;
    private static List<ShadowPU> sPUList;
    private static List<ShadowSS> sSSList;

    protected TurnsManager()
    { // guarantee this will be always a singleton only - can't use the constructor!
        sTurnsList = new List<TurnObject>();
        sPUList = new List<ShadowPU>();
        sSSList = new List<ShadowSS>();
    }

    //turn object clicked on, return the object so it can be show in the map
    public TurnObject GetTurnObject(int index)
    {
        return sTurnsList[index];
    }

    //return the full turns list for generating the TurnsScrollList
    public List<TurnObject> GetTurnsList()
    {
        return sTurnsList;
    }

    //populates sTurnsList for scratch
    public void RecreateTurnsList()
    {
        sTurnsList.Clear();
        sPUList.Clear();
        sSSList.Clear();
        ShadowSS sss;
        ShadowPU spu;

        foreach (PlayerUnit pu in PlayerManager.Instance.GetPlayerUnitList())
        {
            spu = new ShadowPU(pu);
            sPUList.Add(spu);
            //Debug.Log("Test: old pu and new pNew values at start are: " + spu.speed + " " + pu.StatTotalSpeed);
            //spu.speed += 15;
            //Debug.Log("Test: old pu and new pNew values after edit are: " + spu.speed + " " + pu.StatTotalSpeed);
        }

        foreach (SpellSlow s in SpellManager.Instance.GetSpellSlowList())
        {
            sss = new ShadowSS(s);
            sSSList.Add(sss);
        }

        //now generate 20 turn objects based the shadow objects
        int atMaxNumber = 20;
        int atNumber = 0;
        int tickMax = 100;
        int startTick = 0;
        int zBreak = 0;
        TurnObject t;
        
        while (atNumber < atMaxNumber && startTick < tickMax)
        {
            //Debug.Log("atNumber is " + atNumber + "startTick is " + startTick);
            DecrementShadowCtr();
            sss = GetNextShadowSS();
            while (sss != null)
            {
                if (sss != null)
                {
                    t = new TurnObject(sss, atNumber, startTick);
                    sTurnsList.Add(t);
                    atNumber += 1;
                    sSSList.Remove(sss);//removes it from the queue
                }
                sss = GetNextShadowSS();

                zBreak += 1;
                if (zBreak > 1000)
                {
                    break;
                }
            }

            AddShadowCT();
            spu = GetNextShadowPU();
            while (spu != null)
            {
                if (spu != null)
                {
                    t = new TurnObject(spu, atNumber, startTick);
                    if( StatusManager.Instance.IfStatusByUnitAndId(spu.actorId,NameAll.STATUS_ID_DEAD))
                    {
                        t.AddToTitle(" (DEAD)");
                    }
                    sTurnsList.Add(t);
                    atNumber += 1;
                    DecrementTurnCT(spu.actorId);
                }
                spu = GetNextShadowPU();

                zBreak += 1;
                if (zBreak > 1000)
                {
                    break;
                }
            }
            startTick += 1;
            zBreak += 1;
            if(zBreak > 1000)
            {
                break;
            }
        }
    }

    private ShadowPU GetNextShadowPU()
    {
        int z1 = 9999;
        foreach (ShadowPU s in sPUList)
        {
            if (s.ct >= 100)
            {
                if (s.actorId <= z1) //tiebreaker
                {
                    z1 = s.actorId;
                }
            }
        }
        if (z1 != 9999)
        {
            return sPUList[z1];
        }
        return null;
    }

    private ShadowSS GetNextShadowSS()
    {
        int z1 = 9999;
        int z2 = 99999999;
        ShadowSS ssOut = null;
        foreach(ShadowSS s in sSSList)
        {
            //Debug.Log("in getnextshow ss " + s.actorId +" " + s.name + " " + s.uniqueId);

            if( s.ctr == 0)
            {
                if(s.actorId <= z1) 
                {
                    if( s.actorId == z1) //tiebreaker 
                    {
                        if(s.uniqueId < z2)
                        {
                            z1 = s.actorId;
                            z2 = s.uniqueId;
                            ssOut = s;
                        }
                    }
                    else
                    {
                        //Debug.Log("ss set");
                        z1 = s.actorId;
                        z2 = s.uniqueId;
                        ssOut = s;
                    }
                }
            }
        }
        //foreach (ShadowSS s in sSSList)
        //{
        //    Debug.Log("in getnextshow ss " + s.actorId + " " + s.name + " " + s.uniqueId);
        //}
        //if (ssOut != null)
        //{
        //    ShadowSS s = ssOut;
        //    Debug.Log("in getnextshow ss " + s.actorId + " " + s.name + " " + s.uniqueId);
        //}
        return ssOut;
    }

    private void AddShadowCT()
    {
        foreach (ShadowPU s in sPUList)
        {
            if( s.ct < 100)
            {
                if(!StatusManager.Instance.IsCTHalted(s.actorId))
                {
                    int z1 = s.speed;
                    z1 = StatusManager.Instance.ModifySpeed(s.actorId, z1);
                    s.ct += z1;
                }
            }
        }
    }

    

    private void DecrementShadowCtr()
    {
        foreach(ShadowSS s in sSSList)
        {
            s.ctr -= 1;
        }
    }

    private void DecrementTurnCT(int zActorId)
    {
        //pretty sure it is default order
        sPUList[zActorId].ct -= 100;
    }

    //called from UITurnsScrollList on ability click to see where a hypothetical turn in the turn list will be
    public void InsertTurn(PlayerUnit pu, SpellName sn)
    {
        
        int zCTR = CalculationAT.CalculateCTR(pu, sn);
        TurnObject t = new TurnObject(pu,sn, zCTR);
        if (zCTR == 0)
        {
            sTurnsList.Insert(0, t);
        }
        else
        {
            for( int i = 0; i < sTurnsList.Count; i++)
            {
                if(sTurnsList[i].CTR > zCTR)
                {
                    sTurnsList.Insert(i, t);
                    break;
                }
                else if( sTurnsList[i].CTR == zCTR && sTurnsList[i].actorId > t.actorId)
                {
                    sTurnsList.Insert(i, t);
                    break;
                }
            }
        }
        

        
    }
}
