using UnityEngine;
using System;

//[System.Serializable]
public class ItemData : ScriptableObject
{

    public int item_id;
    public int version;
    public int slot;
    public int item_type;
    public int level;

    public string item_name;
    public int blocks;
    public int status_name;
    public int stat_brave;
    public int stat_c_evade;

    public int stat_cunning;
    public int stat_faith;
    public int stat_life;
    public int stat_jump;
    public int stat_m_evade;

    public int stat_ma;
    public int stat_move;
    public int stat_mp;
    public int stat_p_evade;
    public int stat_pa;

    public int stat_speed;
    public int stat_w_evade;
    public int stat_wp;
    public int elemental_type;
    public int on_hit_effect;

    public int on_hit_chance;
    public int stat_agi;
    public string description;

    //loads from a .csv placed in the resources file
    public void Load(string line)
    {
        //Debug.Log(" loading an item data " + line);
        string[] elements = line.Split(',');

        this.item_id = Convert.ToInt32(elements[0]);
        this.version = Convert.ToInt32(elements[1]);
        this.slot = Convert.ToInt32(elements[2]);
        this.item_type = Convert.ToInt32(elements[3]);
        this.level = Convert.ToInt32(elements[4]);

        this.item_name = elements[5];
        this.blocks = Convert.ToInt32(elements[6]);
        this.status_name = Convert.ToInt32(elements[7]);
        this.stat_brave = Convert.ToInt32(elements[8]);
        this.stat_c_evade = Convert.ToInt32(elements[9]);

        this.stat_cunning = Convert.ToInt32(elements[10]);
        this.stat_faith = Convert.ToInt32(elements[11]);
        this.stat_life = Convert.ToInt32(elements[12]);
        this.stat_jump = Convert.ToInt32(elements[13]);
        this.stat_m_evade = Convert.ToInt32(elements[14]);

        this.stat_ma = Convert.ToInt32(elements[15]);
        this.stat_move = Convert.ToInt32(elements[16]);
        this.stat_mp = Convert.ToInt32(elements[17]);
        this.stat_p_evade = Convert.ToInt32(elements[18]);
        this.stat_pa = Convert.ToInt32(elements[19]);

        this.stat_speed = Convert.ToInt32(elements[20]);
        this.stat_w_evade = Convert.ToInt32(elements[21]);
        this.stat_wp = Convert.ToInt32(elements[22]);
        this.elemental_type = Convert.ToInt32(elements[23]);
        this.on_hit_effect = Convert.ToInt32(elements[24]);

        this.on_hit_chance = Convert.ToInt32(elements[25]);
        this.stat_agi = Convert.ToInt32(elements[26]);
        this.description = elements[27];
    }
    
    //public string GetItemType()
    //{
    //    return this.item_type;
    //}

    //public int GetItemId()
    //{
    //    return this.item_id;
    //}

    //public string GetItemName()
    //{
    //    return this.item_name;
    //}
    ////public int stat_brave;
    ////public int stat_c_evade;
    ////public int stat_cunning;
    ////public int stat_faith;
    ////public int stat_life;
    //public int GetStatBrave()
    //{
    //    return this.stat_brave;
    //}
    //public int GetStatCEvade()
    //{
    //    return this.stat_c_evade;
    //}
    //public int GetStatCunning()
    //{
    //    return this.stat_cunning;
    //}
    //public int GetStatFaith()
    //{
    //    return this.stat_faith;
    //}
    //public int GetStatLife()
    //{
    //    return this.stat_life;
    //}
    ////public int stat_jump;
    ////public int stat_m_evade;
    ////public int stat_ma;
    ////public int stat_move;
    ////public int stat_mp;
    //public int GetStatJump()
    //{
    //    return this.stat_jump;
    //}
    //public int GetStatMEvade()
    //{
    //    return this.stat_m_evade;
    //}
    //public int GetStatMA()
    //{
    //    return this.stat_ma;
    //}
    //public int GetStatMove()
    //{
    //    return this.stat_move;
    //}
    //public int GetStatMP()
    //{
    //    return this.stat_mp;
    //}
    ////public int stat_p_evade;
    ////public int stat_pa;
    ////public int stat_speed;
    ////public int stat_w_evade;
    ////public int stat_wp;
    //public int GetStatPEvade()
    //{
    //    return this.stat_p_evade;
    //}
    //public int GetStatPA()
    //{
    //    return this.stat_pa;
    //}

    //public int GetStatSpeed()
    //{
    //    return this.stat_speed;
    //}
    //public int GetStatWEvade()
    //{
    //    return this.stat_w_evade;
    //}
    //public int GetStatWP()
    //{
    //    return this.stat_wp;
    //}

    //public string GetElementalType()
    //{
    //    return this.elemental_type;
    //}

    //public string GetStatusName()
    //{
    //    return this.status_name;
    //}

    //public string GetStatusBlocks()
    //{
    //    return this.blocks;
    //}

    //public string GetOnHitEffect()
    //{
    //    return this.on_hit_effect;
    //}

    //public int GetOnHitChance()
    //{
    //    return this.on_hit_chance;
    //}

    //public int GetStatAgi()
    //{
    //    return this.stat_agi;
    //}

    //public string GetSlot()
    //{
    //    return this.slot;
    //}
}
