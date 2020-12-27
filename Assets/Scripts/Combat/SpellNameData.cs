using UnityEngine;
using System;

public class SpellNameData : ScriptableObject
{
    //must be public, can't use getters and setters
    public int Index ;

    public int SpellId ;
    public int Version ;
    public string AbilityName ;
    public int CommandSet ;
    public int DamageFormulaType ;

    public int Mod ;
    public int CTR ;
    public int MP ;
    public int RemoveStat ;
    public int BaseHit ;

    public int BaseQ ;
    public int HitsStat ;
    public int StatType ;
    public int AddsStatus ;
    public int StatusType ;

    public int PMType ;
    public int EvasionReflect ;
    public int CalculateMimic ;
    public int CounterType ;
    public int StatusCancel ;
    
    public int ElementType ;
    public int CasterImmune ;
    public int AlliesType ;
    public int IgnoresDefense ;
    public int RangeXYMin ;

    public int RangeXYMax ;
    public int RangeZ ;
    public int EffectXY ;
    public int EffectZ ;

    //loads from a .csv placed in the resources file
    public void Load(string line)
    {
        //Debug.Log(" loading an item data " + line);
        string[] elements = line.Split(',');

        this.Index = Convert.ToInt32(elements[0]); //Debug.Log("asdf," + elements[0]);

        this.SpellId = Convert.ToInt32(elements[1]);
        this.Version = Convert.ToInt32(elements[2]);
        this.AbilityName = elements[3]; //Debug.Log("asdf," + elements[3]);
        this.CommandSet = Convert.ToInt32(elements[4]);
        this.DamageFormulaType = Convert.ToInt32(elements[5]);

        this.Mod = Convert.ToInt32(elements[6]);
        this.CTR = Convert.ToInt32(elements[7]);
        this.MP = Convert.ToInt32(elements[8]);
        this.RemoveStat = Convert.ToInt32(elements[9]);
        this.BaseHit = Convert.ToInt32(elements[10]);
        
        this.BaseQ = Convert.ToInt32(elements[11]);
        this.HitsStat = Convert.ToInt32(elements[12]);
        this.StatType = Convert.ToInt32(elements[13]);
        this.AddsStatus = Convert.ToInt32(elements[14]);
        this.StatusType = Convert.ToInt32(elements[15]);

        this.PMType = Convert.ToInt32(elements[16]);
        this.EvasionReflect = Convert.ToInt32(elements[17]);
        this.CalculateMimic = Convert.ToInt32(elements[18]);
        this.CounterType = Convert.ToInt32(elements[19]);
        this.StatusCancel = Convert.ToInt32(elements[20]);
        
        this.ElementType = Convert.ToInt32(elements[21]);
        this.CasterImmune = Convert.ToInt32(elements[22]);
        this.AlliesType = Convert.ToInt32(elements[23]);
        this.IgnoresDefense = Convert.ToInt32(elements[24]);
        this.RangeXYMin = Convert.ToInt32(elements[25]);
        
        this.RangeXYMax = Convert.ToInt32(elements[26]);
        this.RangeZ = Convert.ToInt32(elements[27]);
        this.EffectXY = Convert.ToInt32(elements[28]);
        this.EffectZ = Convert.ToInt32(elements[29]);
    }
}
