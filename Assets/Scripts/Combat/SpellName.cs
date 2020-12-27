using System;
using UnityEngine;

[Serializable]
public class SpellName {

    public int Index { get; set; }

    public int SpellId { get; set; }
    public int Version { get; set; }
    public string AbilityName { get; set; }
    public int CommandSet { get; set; }
    public int DamageFormulaType { get; set; }

    public int Mod { get; set; }
    public int CTR { get; set; }
    public int MP { get; set; }
    public int RemoveStat { get; set; }
    public int BaseHit { get; set;}
   
    public int BaseQ { get; set; }
    public int HitsStat { get; set; }
    public int StatType { get; set; }
    public int AddsStatus { get; set; }
    public int StatusType { get; set; }

    public int PMType { get; set; }
    public int EvasionReflect { get; set; }
    public int CalculateMimic { get; set; }
    public int StatusCancel { get; set; }
    public int CounterType { get; set; }

    public int ElementType { get; set; }
    public int CasterImmune { get; set; }
    public int AlliesType { get; set; }
    public int IgnoresDefense { get; set; }
    public int RangeXYMin { get; set; }

    public int RangeXYMax { get; set; }
    public int RangeZ { get; set; }
    public int EffectXY { get; set; }
    public int EffectZ { get; set; }

    public SpellName() { }

    public SpellName(SpellNameData snd)
    {
        this.Index = snd.Index; //Debug.Log("snd index is " + snd.Index + "," + snd.AbilityName);

        this.SpellId = snd.SpellId;
        this.Version = snd.Version;
        this.AbilityName = snd.AbilityName;
        this.CommandSet = snd.CommandSet;
        this.DamageFormulaType = snd.DamageFormulaType;

        this.Mod = snd.Mod;
        this.CTR = snd.CTR;
        this.MP = snd.MP;
        this.RemoveStat = snd.RemoveStat;
        this.BaseHit = snd.BaseHit;

        this.BaseQ = snd.BaseQ;
        this.HitsStat = snd.HitsStat;
        this.StatType = snd.StatType;
        this.AddsStatus = snd.AddsStatus;
        this.StatusType = snd.StatusType;

        this.PMType = snd.PMType;
        this.EvasionReflect = snd.EvasionReflect;
        this.CalculateMimic = snd.CalculateMimic;
        this.CounterType = snd.CounterType;
        this.StatusCancel = snd.StatusCancel;

        this.ElementType = snd.ElementType;
        this.CasterImmune = snd.CasterImmune;
        this.AlliesType = snd.AlliesType;
        this.IgnoresDefense = snd.IgnoresDefense;
        this.RangeXYMin = snd.RangeXYMin;

        this.RangeXYMax = snd.RangeXYMax;
        this.RangeZ = snd.RangeZ;
        this.EffectXY = snd.EffectXY;
        this.EffectZ = snd.EffectZ;
    }

    public SpellName(int customSNID, int commandSet )
    {
        this.Index = customSNID;

        this.SpellId = customSNID;
        this.Version = NameAll.VERSION_AURELIAN;
        this.AbilityName = "custom ability";
        this.CommandSet = commandSet;
        this.DamageFormulaType = 0;

        this.Mod = NameAll.MOD_PHYSICAL;
        this.CTR = 0;
        this.MP = 0;
        this.RemoveStat = NameAll.REMOVE_STAT_REMOVE;
        this.BaseHit = 80;

        this.BaseQ = 10;
        this.HitsStat = NameAll.HITS_STAT;
        this.StatType = NameAll.STAT_TYPE_HP;
        this.AddsStatus = 0;
        this.StatusType = NameAll.STATUS_ID_NONE;

        this.PMType = NameAll.PM_TYPE_PHYSICAL;
        this.EvasionReflect = 0;
        this.CalculateMimic = 0;
        this.CounterType = 0;
        this.StatusCancel = 0;

        this.ElementType = 0;
        this.CasterImmune = 0;
        this.AlliesType = 0;
        this.IgnoresDefense = 0;
        this.RangeXYMin = 1;

        this.RangeXYMax = 3;
        this.RangeZ = 2;
        this.EffectXY = 1;
        this.EffectZ = 0;
    }

    public bool IsCalculate()
    {
        if (this.CalculateMimic == 1 || this.CalculateMimic == 2)
        {
            return true;
        }
        return false;
    }

    //does the unit's facing direction change the area of the spell cast? if so then it's not direction independeint
    public bool IsDirectionIndependent()
    {

        if ((this.EffectXY >= NameAll.SPELL_EFFECT_CONE_BASE && this.EffectXY <= NameAll.SPELL_EFFECT_CONE_MAX)
            || (this.EffectXY >= NameAll.SPELL_EFFECT_LINE_2 && this.EffectXY <= NameAll.SPELL_EFFECT_LINE_8))
            return false;

        return true;
    }

}
