using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class ItemObject {

    public int ItemId;
    public int Version;
    public int Slot;
    public int ItemType;
    public int Level;

    public string ItemName;
    public int Blocks;
    public int StatusName;
    public int StatBrave;
    public int StatCEvade;

    public int StatCunning;
    public int StatFaith;
    public int StatLife;
    public int StatJump;
    public int StatMEvade;

    public int StatMA;
    public int StatMove;
    public int StatMP;
    public int StatPEvade;
    public int StatPA;

    public int StatSpeed;
    public int StatWEvade;
    public int StatWP;
    public int ElementalType;
    public int OnHitEffect;

    public int OnHitChance;
    public int StatAgi;
    public string Description;
    public int Gold;

   

    public ItemObject()
    {

    }

    public ItemObject(ItemData id)
    {
        this.ItemId = id.item_id;
        this.Version = id.version;
        this.Slot = id.slot;
        this.ItemType = id.item_type;
        this.Level = id.level;

        this.ItemName = id.item_name;
        this.Blocks = id.blocks;
        this.StatusName = id.status_name;
        this.StatBrave = id.stat_brave;
        this.StatCEvade = id.stat_c_evade;

        this.StatCunning = id.stat_cunning;
        this.StatFaith = id.stat_faith;
        this.StatLife = id.stat_life;
        this.StatJump = id.stat_jump;
        this.StatMEvade = id.stat_m_evade;

        this.StatMA = id.stat_ma;
        this.StatMove = id.stat_move;
        this.StatMP = id.stat_mp;
        this.StatPEvade = id.stat_p_evade;
        this.StatPA = id.stat_pa;

        this.StatSpeed = id.stat_speed;
        this.StatWEvade = id.stat_w_evade;
        this.StatWP = id.stat_wp;
        this.ElementalType = id.elemental_type;
        this.OnHitEffect = id.on_hit_effect;

        this.OnHitChance = id.on_hit_chance;
        this.StatAgi = id.stat_agi;
        this.Description = id.description;
        
        this.Gold = 0; //not implemented in ItemData yet
    }

    public ItemObject(int zId)
    {
        this.ItemId = zId;
        this.Version = NameAll.VERSION_AURELIAN;
        this.Slot = NameAll.ITEM_SLOT_ACCESSORY;
        this.ItemType = NameAll.ITEM_ITEM_TYPE_BRACELET;
        this.Level = 1;

        this.ItemName = "Custom Item " + zId;
        this.Blocks = NameAll.STATUS_ID_NONE;
        this.StatusName = NameAll.STATUS_ID_NONE;
        this.StatBrave = 0;
        this.StatCEvade = 0;

        this.StatCunning = 0;
        this.StatFaith = 0;
        this.StatLife = 0;
        this.StatJump = 0;
        this.StatMEvade = 0;

        this.StatMA = 0;
        this.StatMove = 0;
        this.StatMP = 0;
        this.StatPEvade = 0;
        this.StatPA = 0;

        this.StatSpeed = 0;
        this.StatWEvade = 0;
        this.StatWP = 0;
        this.ElementalType = NameAll.ITEM_ELEMENTAL_NONE;
        this.OnHitEffect = NameAll.STATUS_ID_NONE;

        this.OnHitChance = 0;
        this.StatAgi = 0;
        this.Description = "";
        this.Gold = 0;
    }
    
    public string GetSlotName()
    {
        if (this.Slot == 0)
            return "Weapon";
        else if (this.Slot == 1)
            return "Offhand";
        else if (this.Slot == 2)
            return "Head";
        else if (this.Slot == 3)
            return "Body";
        else
            return "Accessory";
    }

    

    public void ChangeValueFromScrollList(int type, int value)
    {
        if( type == ItemConstants.INPUT_ITEM_ELEMENTAL_TYPE)
        {
            this.ElementalType = value;
        }
        else if( type == ItemConstants.INPUT_ITEM_BLOCKS)
        {
            this.Blocks = value;
        }
        else if (type == ItemConstants.INPUT_ITEM_TYPE)
        {
            this.ItemType = value;
        }
        else if (type == ItemConstants.INPUT_ITEM_VERSION)
        {
            this.Version = value;
            this.ItemType = NameAll.ITEM_ITEM_TYPE_NONE;
        }
        else if (type == ItemConstants.INPUT_ITEM_SLOT)
        {
            this.Slot = value;
            this.ItemType = NameAll.ITEM_ITEM_TYPE_NONE;
        }
        else if (type == ItemConstants.INPUT_ITEM_STATUS_NAME)
        {
            this.StatusName = value;
        }
        else if(type == ItemConstants.INPUT_ITEM_ON_HIT_EFFECT)
        {
            this.OnHitEffect = value;
        }
    }

    

    public void ChangeValueFromUserInput(int type, string value)
    {
        if (type == ItemConstants.INPUT_ITEM_NAME)
        {
            this.ItemName = value;
        }
        else if (type == ItemConstants.INPUT_ITEM_DESCRIPTION)
        {
            this.Description = value;
        }
    }

    
    public void ChangeValueFromUserInput(int type, int value)
    {
        //Debug.Log("submitting user input " + type + " " + value);
        if (type == ItemConstants.INPUT_ITEM_CUNNING)
            this.StatCunning = value;
        else if (type == ItemConstants.INPUT_ITEM_LIFE)
            this.StatLife = value;
        else if (type == ItemConstants.INPUT_ITEM_MOVE)
            this.StatMove = value;
        else if (type == ItemConstants.INPUT_ITEM_M_EVADE)
            this.StatMEvade = value;
        else if (type == ItemConstants.INPUT_ITEM_P_EVADE)
            this.StatPEvade = value;
        else if (type == ItemConstants.INPUT_ITEM_SPEED)
            this.StatSpeed = value;
        else if (type == ItemConstants.INPUT_ITEM_LEVEL)
        {
            this.Level = value; //Debug.Log("submitting user input " + type + " " + value);
        }
        else if (type == ItemConstants.INPUT_ITEM_W_EVADE)
            this.StatWEvade = value;
        else if (type == ItemConstants.INPUT_ITEM_WP)
            this.StatWP = value;
        else if (type == ItemConstants.INPUT_ITEM_PA)
            this.StatPA = value;
        else if (type == ItemConstants.INPUT_ITEM_AGI)
            this.StatAgi = value;
        else if (type == ItemConstants.INPUT_ITEM_BRAVE)
            this.StatBrave = value;
        else if (type == ItemConstants.INPUT_ITEM_C_EVADE)
            this.StatCEvade = value;
        else if (type == ItemConstants.INPUT_ITEM_FAITH)
            this.StatFaith = value;
        else if (type == ItemConstants.INPUT_ITEM_JUMP)
            this.StatJump = value;
        else if (type == ItemConstants.INPUT_ITEM_MA)
            this.StatMA = value;
        else if (type == ItemConstants.INPUT_ITEM_MP)
            this.StatMP = value;
        else if (type == ItemConstants.INPUT_ITEM_ON_HIT_CHANCE)
            this.OnHitChance = value;
        else if (type == ItemConstants.INPUT_ITEM_GOLD)
            this.Gold = value;
    }

}


