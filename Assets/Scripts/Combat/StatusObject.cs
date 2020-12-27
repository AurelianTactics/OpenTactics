using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
//can group statuses better and more efficiently probably
    //performing/defending/charging as one
objects that get added to static arrays in status lab and treated there
 basically two types, tickable and those tied to equipment/unit
 */

public class StatusObject
{
    private int unitId;
    private int ticksLeft; //expires at 0
    private int statusId;
    private int tickType;
    private int itemId;
    private int statusItemId;
    private int itemSlot; //same item in two slots

    public StatusObject(int player_id, int statusId) //non item/lasting statuses
    {
        this.itemId = 999999;
        this.unitId = player_id;
        this.statusId = statusId;
        if (CreateStatusObject(statusId))
        {
            //status shit set, bunch of them default to 0,0 and are skipped in the function
        }
        else {
            this.ticksLeft = 0;
            this.tickType = 0;
        }

    }

    public StatusObject( int zUnitId, int zItemId, int statusId, int zItemSlot) //for item statuses, code down below changes it
    {
        
        this.unitId = zUnitId;
        this.ticksLeft = 0;
        this.statusId = statusId;
        this.tickType = 0;
        this.itemId = zItemId;
        //not sure wtf statusItemId does
        this.itemSlot = zItemSlot;   
    }

    private bool CreateStatusObject(int statusId)
    {

        //if (status.Equals("charging") || status.Equals("defending") || status.Equals("performing") ||
        //    status.Equals("reraise") || status.Equals("darkness"))
        //{
        //    this.tickType = 0;
        //    this.ticksLeft = 0;
        //    return true;
        //}
        //else if (status.Equals("float_move") || status.Equals("float") )
        //{
        //    this.tickType = 0;
        //    this.ticksLeft = 0;
        //    return true;
        //}
        //else if (status.Equals("blood_suck") || status.Equals("confusion")
        //    || status.Equals("chicken") || status.Equals("berserk") )
        //{
        //    this.tickType = 0;
        //    this.ticksLeft = 0;
        //    return true;
        //}
        //frog, morbol, petrify, silence, undead, critical, invite, quick, skipped as the default values are fine
        if (statusId == NameAll.STATUS_ID_DEAD || statusId == NameAll.STATUS_ID_DEATH_SENTENCE)
        {
            this.tickType = 0;
            this.ticksLeft = 3;
            return true;
        }
        else if(statusId == NameAll.STATUS_ID_PROTECT)
        {
            this.tickType = 1;
            this.ticksLeft = 32;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_SHELL)
        {
            this.tickType = 1;
            this.ticksLeft = 32;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_HASTE)
        {
            this.tickType = 1;
            this.ticksLeft = 32;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_REGEN)
        {
            this.tickType = 1;
            this.ticksLeft = 36;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_CHARM)
        {
            this.tickType = 1;
            this.ticksLeft = 32;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_DONT_ACT)
        {
            this.tickType = 1;
            this.ticksLeft = 24;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_DONT_MOVE)
        {
            this.tickType = 1;
            this.ticksLeft = 24;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_POISON)
        {
            this.tickType = 1;
            this.ticksLeft = 36;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_SLEEP)
        {
            this.tickType = 1;
            this.ticksLeft = 60;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_SLOW)
        {
            this.tickType = 1;
            this.ticksLeft = 24;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_STOP)
        {
            this.tickType = 1;
            this.ticksLeft = 20;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_OIL)
        {
            this.tickType = 1;
            this.ticksLeft = 32;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_FAITH)
        {
            this.tickType = 1;
            this.ticksLeft = 32;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_INNOCENT)
        {
            this.tickType = 1;
            this.ticksLeft = 32;
            return true;
        }
        else if (statusId == NameAll.STATUS_ID_REFLECT)
        {
            this.tickType = 1;
            this.ticksLeft = 32;
            return true;
        }
        return false;
    }

    public bool RemoveStatusObject(int unit_id, int statusId)
    {
        if (this.unitId == unit_id && this.statusId == statusId )
        {
            return true;
        }
        return false;
    }

    public int GetUnitId()
    {
        return unitId;
    }

    public int GetTicksLeft()
    {
        return ticksLeft;
    }

    public int GetStatusId()
    {
        return statusId;
    }

    public int GetTickType()
    {
        return tickType;
    }

    public int GetItemId()
    {
        return itemId;
    }

    public int GetItemSlot()
    {
        return itemSlot;
    }

    public void SubtractTick()
    {
        this.ticksLeft -= 1; //Debug.Log("status ID and ticks left " + this.statusId + ", " + this.ticksLeft);
    }
}

public static class StatusObjectCreator
{

    //fucking shitty way to do this, called in StatusManager, based on the itemId creates multiple statuses
    public static void CreateStatusObjectListFromItem(int unitId, int itemId, int slot ) //use the itemStatusName to enter the status
    {
        List<StatusObject> outList = new List<StatusObject>();
        //Debug.Log("asdf" + slot + itemId);
        ItemObject io = ItemManager.Instance.GetItemObjectById(itemId, slot);
        if( !io.StatusName.Equals(""))
        {
            outList = CreateStatusName(io.StatusName, unitId, itemId, slot);
        }
        foreach (StatusObject so in outList)
        {
            StatusManager.Instance.AddStatusLastingByStatusObject(so);
            //DO IT BY STATUS BELOW //PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
        }

        outList.Clear();
        if (!io.Blocks.Equals(""))
        {
            outList = CreateStatusBlocks(io.Blocks, unitId, itemId, slot);
        }

        foreach (StatusObject so in outList)
        {
            StatusManager.Instance.AddStatusLastingByStatusObject(so);
        }


    }

    private static List<StatusObject> CreateStatusName(int statusId, int unitId, int itemId, int slot) //statusname
    {
        List<StatusObject> outList = new List<StatusObject>();
        //strengthen_all
        //auto_float_reflect
        //auto_protect_shell
        //auto_regen_reraise
        //auto_haste
        //reflect_item
        //reraise_start
        //auto_undead
        //auto_float
        //auto_reflect
        //absorb_earth_strengthen_earth
        //absorb_light
        //half_fire_lightning_ice
        //strengthen_fire_lightning_ice
        //auto_regen
        //start_petrify
        //excalibur
        //auto_protect
        //auto_shell
        //auto_faith
        //strengthen_fire
        //strengthen_ice
        //strengthen_lightning
        //absorb_fire_half_ice_weak_water
        //absorb_ice_half_fire_weak_lightning
        StatusObject so;
        if ( statusId == NameAll.STATUS_ID_AUTO_HASTE )
        {
            so = new StatusObject(unitId, itemId, statusId, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_ICE_HALF_FIRE_WEAK_LIGHTNING )
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_ABSORB_ICE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_HALF_FIRE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_WEAK_LIGHTNING, slot);
            outList.Add(so);
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_FIRE_HALF_ICE_WEAK_WATER )
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_ABSORB_FIRE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_HALF_ICE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_WEAK_WATER, slot);
            outList.Add(so);
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_ICE )
        {
            so = new StatusObject(unitId, itemId, statusId, slot);
            outList.Add(so);
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_LIGHTNING )
        {
            so = new StatusObject(unitId, itemId, statusId, slot);
            outList.Add(so);
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_FIRE)
        {
            so = new StatusObject(unitId, itemId, statusId, slot);
            outList.Add(so);
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_FAITH)
        {
            so = new StatusObject(unitId, itemId, statusId, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_SHELL)
        {
            so = new StatusObject(unitId, itemId, statusId, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
        }
        else if (statusId == NameAll.STATUS_ID_EXCALIBUR)
        {
            //Auto-Haste, Absorb: Holy, Strengthen: Holy
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_AUTO_HASTE, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_ABSORB_LIGHT, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_STRENGTHEN_LIGHT, slot);
            outList.Add(so);
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_PROTECT)
        {
            so = new StatusObject(unitId, itemId, statusId, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_REGEN)
        {
            so = new StatusObject(unitId, itemId, statusId, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_FIRE_LIGHTNING_ICE)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_STRENGTHEN_FIRE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_STRENGTHEN_LIGHTNING, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_STRENGTHEN_LIGHTNING, slot);
            outList.Add(so);
        }
        else if (statusId == NameAll.STATUS_ID_HALF_FIRE_LIGHTNING_ICE)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_HALF_FIRE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_HALF_LIGHTNING, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_HALF_ICE, slot);
            outList.Add(so);
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_LIGHT)
        {
            so = new StatusObject(unitId, itemId, statusId, slot);
            outList.Add(so);
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_EARTH_STRENGTHEN_EARTH)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_STRENGTHEN_EARTH, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_ABSORB_EARTH, slot);
            outList.Add(so);
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_REFLECT)
        {
            so = new StatusObject(unitId, itemId, statusId, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_FLOAT )
        {
            so = new StatusObject(unitId, itemId, statusId, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_UNDEAD)
        {
            so = new StatusObject(unitId, itemId, statusId, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
        }
        else if (statusId == NameAll.STATUS_ID_RERAISE_START)
        {
            //so = new StatusObject(unitId, itemId, statusId, slot);
            //outList.Add(so);
            //PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
            StatusManager.Instance.AddStatusAndOverrideOthers(0, unitId, NameAll.STATUS_ID_RERAISE);
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_REFLECT)
        {
            so = new StatusObject(unitId, itemId, statusId, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_FLOAT_REFLECT)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_AUTO_FLOAT, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_AUTO_REFLECT, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_REGEN_RERAISE)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_AUTO_REGEN, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
            //so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_RERAISE_START, slot);
            //outList.Add(so);
            //PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
            StatusManager.Instance.AddStatusAndOverrideOthers(0, unitId, NameAll.STATUS_ID_RERAISE);
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_PROTECT_SHELL)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_AUTO_PROTECT, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_AUTO_SHELL, slot);
            outList.Add(so);
            PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
        }
        else if (statusId == NameAll.STATUS_ID_BERSERK_START)
        {
            //so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_AUTO_BERSERK, slot);
            //outList.Add(so);
            //PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId());
            //MORE LIKE START BERSERK
            StatusManager.Instance.AddStatusAndOverrideOthers(0, unitId, NameAll.STATUS_ID_BERSERK);
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_ALL)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_STRENGTHEN_EARTH, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_STRENGTHEN_FIRE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_STRENGTHEN_ICE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_STRENGTHEN_WIND, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_STRENGTHEN_WATER, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_STRENGTHEN_DARK, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_STRENGTHEN_LIGHT, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_STRENGTHEN_LIGHTNING, slot);
            outList.Add(so);
        }

        return outList;
    }

    private static List<StatusObject> CreateStatusBlocks(int blockId, int unitId, int itemId, int slot)//bn for blockname
    {
        List<StatusObject> outList = new List<StatusObject>();
        //block_slow
        //block_confusion_charm
        //block_dont_move_dont_act
        //block_petrify_stop
        //block_undead_blood_suck
        //block_silence_berserk
        //block_sleep_death_sentence
        //block_dead_darkness
        //block_invite
        //block_dont_move_lightning
        //block_lightning
        //block_stop
        //block_dead
        //block_silence
        //block_darkness_sleep
        //block_cachusa
        //block_ribbon
        //block_barette

        StatusObject so;
        if ( blockId == NameAll.STATUS_ID_BLOCK_SLOW )
        {
            so = new StatusObject(unitId, itemId, blockId, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_DONT_MOVE_DONT_ACT)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DONT_MOVE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DONT_ACT, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_PETRIFY_STOP)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_PETRIFY, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_STOP, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_CONFUSION_CHARM)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_CONFUSION, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_CHARM, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_UNDEAD_BLOOD_SUCK)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_UNDEAD, slot);
            outList.Add(so);
            //so = new StatusObject(unitId, itemId, "block_blood_suck", slot);
            //outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_SILENCE_BERSERK)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_SILENCE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_BERSERK, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_SLEEP_DEATH_SENTENCE)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DEATH_SENTENCE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_SLEEP, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_DEAD_DARKNESS)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DEAD, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DARKNESS, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_INVITE)
        {
            so = new StatusObject(unitId, itemId, blockId, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_DONT_MOVE_LIGHTNING)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DONT_MOVE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_LIGHTNING, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_LIGHTNING)
        {
            so = new StatusObject(unitId, itemId, blockId, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_DEAD)
        {
            so = new StatusObject(unitId, itemId, blockId, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_STOP)
        {
            so = new StatusObject(unitId, itemId, blockId, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_SILENCE)
        {
            so = new StatusObject(unitId, itemId, blockId, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_DARKNESS_SLEEP)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DARKNESS, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_SLEEP, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_CONFUSION_CHARM)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_CONFUSION, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_CHARM, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_CACHUSA)
        {
            //    A9 Cachusha         20    0  20000  (50)  Block: Dead, Petrify, Invite,
            //                                                 Confusion, Blood Suck, Stop,
            //                                                 Berserk, Charm, Sleep
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_CONFUSION, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_SLEEP, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_CHARM, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_BERSERK, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_STOP, slot);
            outList.Add(so);
            //so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_B, slot);
            //outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DEAD, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_PETRIFY, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_INVITE, slot);
            outList.Add(so);
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_RIBBON)
        {
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_UNDEAD, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DARKNESS, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_SILENCE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_FROG, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_POISON, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_SLOW, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DONT_ACT, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DONT_MOVE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DEATH_SENTENCE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_CONFUSION, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_SLEEP, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_CHARM, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_BERSERK, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_STOP, slot);
            outList.Add(so);
            //so = new StatusObject(unitId, itemId, "block_blood_suck", slot);
            //outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DEAD, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_PETRIFY, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_INVITE, slot);
            outList.Add(so);
            //AB Ribbon           10    0  60000  (52)  Block: Dead, Undead, Petrify, Invite,
            //                                                 Darkness, Confusion, Silence,
            //                                                 Blood Suck, Berserk, Frog,
            //                                                 Poison, Slow, Stop, Charm,
            //                                                 Sleep, Don't Move, Don't Act,
            //                                                 Death Sentence
        }
        else if (blockId == NameAll.STATUS_ID_BLOCK_BARETTE)
        {
            //AA Barette          20    0  20000  (51)  Block: Undead, Darkness, Silence, 
            //                                                 Frog, Poison, Slow, Don't Act,
            //                                                 Don't Move, Death Sentence
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_UNDEAD, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DARKNESS, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_SILENCE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_FROG, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_POISON, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_SLOW, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DONT_ACT, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DONT_MOVE, slot);
            outList.Add(so);
            so = new StatusObject(unitId, itemId, NameAll.STATUS_ID_BLOCK_DEATH_SENTENCE, slot);
            outList.Add(so);
        }
        return outList;
    }

}

