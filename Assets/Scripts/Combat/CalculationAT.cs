using System.Collections.Generic;
using System;
using UnityEngine;

/*
Convenience functions, notes on top of functions say where they are called
    */
public static class CalculationAT {
    static int bugTest = 0;

    
    //called in CombatConfirmAbilityTargetState, gets a string array which is fed into the preview panel
    public static List<string> GetPreviewList(SpellName sn, PlayerUnit actor, Tile targetTile, List<int> outEffect)
    {
        string spellName = sn.AbilityName;
        sn = ModifySlowActionSpellName(sn, actor.TurnOrder); //in case of charge, not used for anything else currently
        string hitString = "";
        string effectString = "";
        string statusString = "";
        string reaction = "";

        int targetId = targetTile.UnitId;

        if( targetId != NameAll.NULL_UNIT_ID)
        {
            reaction = AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_REACTION, PlayerManager.Instance.GetPlayerUnit(targetId).AbilityReactionCode);
        }

        string statHit = "";
        int effect = outEffect[1];
        //different spell types:
        //damage stat only, show the Hit % and the effect
        //add status only, show the Status Hit % and the status to add
        //does both, show all
        //0 for no 1 for specific 1. 2 for hp dmg/ heal. 3 for % of a stat, need a stat name. 4 for restores hp and mp.does the spell damage / aid a stat.equipment break is a hits stat type, if no equipment in that slot defaults to an hp attack
        int hitsStat = sn.HitsStat;
        if (hitsStat > 0)
        {
            //zString += "Hit %: " + outEffect[0] + ",";

            hitString = "" + outEffect[0];
            string statSign;
            int removeStat = CalculationResolveAction.CheckForSpellNameAbsorb(actor, sn);
            //int removeStat = sn.GetRemoveStat();
            List<int> tempList = CalculationResolveAction.CheckForSpellNameHPRestore(actor, sn);
            if (removeStat != NameAll.REMOVE_STAT_ABSORB)
            {
                removeStat = tempList[0];
            }

            if (removeStat == 1 && effect >= 0) //does damage
            {
                if (sn.ElementType == NameAll.ITEM_ELEMENTAL_UNDEAD && targetId != NameAll.NULL_UNIT_ID && StatusManager.Instance.IsUndead(targetId))
                {
                    statSign = "+";
                }
                else
                {
                    statSign = "-";
                }

            }
            else if (removeStat == 0) //adds the stat
            {
                if (sn.ElementType == NameAll.ITEM_ELEMENTAL_UNDEAD && targetId != NameAll.NULL_UNIT_ID && StatusManager.Instance.IsUndead(targetId))
                {
                    statSign = "-";
                }
                else
                {
                    statSign = "+";
                }
            }
            else if (removeStat == 1 && effect < 0) //heals (absorbing damage, ie subtracting negative damage)
            {
                statSign = "+";
                effect = Math.Abs(effect);
            }
            else //absorbs the stat
            {
                statSign = "+/-";
            }

            //statHit = GetStatPreviewEquipmentName(effect, sn.GetStatType(), mapTileIndex); //old way had equipment breaks by name
            statHit = effect + " " + NameAll.GetStatTypeString(sn.StatType);

            if (hitsStat == 4)
            {
                effectString = statSign + outEffect[1] + " HP " + outEffect[3] + " MP"; //"Effect: " + statSign + statHit + ",";
            }
            else
            {
                effectString = statSign + statHit; //"Effect: " + statSign + statHit + ",";
            }


            if (sn.AddsStatus == 1)
            {
                if (outEffect[4] >= 1) //blocked
                {
                    statusString = "Status Hit %: BLOCKED ";
                }
                else
                {
                    statusString = "Status Hit %: " + "100 ";
                }
                statusString += "Add Status: " + NameAll.GetStatusString(sn.StatusType);
            }
            else if (sn.AddsStatus > 100)
            {
                if (outEffect[4] == 2) //status blocked by immunity from item
                {
                    statusString = "Status Hit %: " + "0 ";
                }
                else
                {
                    statusString = "Status Hit %: " + (sn.AddsStatus - 100) + " ";
                }
                statusString += "Add Status: " + NameAll.GetStatusString(sn.StatusType);
            }
        }
        else if (hitsStat == 0)
        {
            if (outEffect[4] >= 1) //blocked
            {
                statusString = "Status Hit %: BLOCKED ";
            }
            else
            {
                hitString = "" + outEffect[0];
                effectString = NameAll.GetStatusString(sn.StatusType);
                statusString = "Status Hit %: " + outEffect[0] + " ";
            }
            statusString += "Add Status: " + NameAll.GetStatusString(sn.StatusType);
        }

        var retValue = new List<string>();
        retValue.Add(spellName);
        retValue.Add(hitString);
        retValue.Add(effectString);
        retValue.Add(statusString);
        retValue.Add(reaction);
        return retValue;
    }

    //called in slowaction state, modifies a spellslow for usage
    public static SpellName ModifySlowActionSpellName(SpellName sn, int actorId)
    {
        int commandSet = sn.CommandSet;
        //Debug.Log("getting attack spell, range is " + sn.GetRangeXYMax() + " " + sn.GetBaseQ());
        //Debug.Log("asdf " + NameAll.COMMAND_SET_CHARGE);
        if (commandSet == NameAll.COMMAND_SET_CHARGE)
        {
            //gets the chargeBaseQ, gets the attack spellName, sets the attack spellName to active, and copies in the chargeBaseQ
            int chargeK = sn.BaseQ; //then get the attack sn and 
            int spellIndex = GetAttackSpellIndex(actorId);
            sn = SpellManager.Instance.GetSpellNameByIndex(spellIndex);
            sn.BaseQ = chargeK;// sn.ModifyBaseQForAttack(chargeK);
            //sn = SpellManager.Instance.SetBaseQForAttack(SceneCreate.activeSpellIndex, chargeK);
            //Debug.Log("testing command set charge" + sn.Index);
        }

        return sn;
    }

    //called in reaction state, the sr.GetEffect() can modify the temp effect
    //some info is passed to the sr.getEffect in CalculationResolveAction that helps get the correct SpellReaction
    public static SpellName ModifyReactionSpellName(SpellReaction sr, int actorId)
    {
        int tempEffect = sr.Effect;
        SpellName sn = SpellManager.Instance.GetSpellNameByIndex(sr.SpellIndex);
        if (tempEffect != 0)
        {
            //sn.ModifyBaseQForAttack(tempEffect);
            sn.BaseQ = tempEffect;
        }
        return sn;
    }

    //called here and in CalculationResolveAction, used to get the spellIndex for the attacking unit
    public static int GetAttackSpellIndex(int actorId)
    {
        //gets the attack spell name based on the actor
        if (StatusManager.Instance.IfStatusByUnitAndId(actorId, NameAll.STATUS_ID_FROG, true))
        {
            //SpellName sn = SpellManager.Instanace.GetSpellNameByIndex(NameAll.FROG_ATTACK_SPELL_INDEX);
            int classId = PlayerManager.Instance.GetPlayerUnit(actorId).ClassId;
            if (classId >= NameAll.CLASS_FIRE_MAGE)
            {
                return NameAll.SPELL_INDEX_WEAK_ATTACK;
            }
            else
            {
                return NameAll.FROG_ATTACK_SPELL_INDEX;
            }

        }
        else
        {
            PlayerUnit actor = PlayerManager.Instance.GetPlayerUnit(actorId);
            int weapon_type = ItemManager.Instance.GetItemType(actor.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON);
            //Debug.Log("weapoin type is" + weapon_type);
            SpellName sn = SpellManager.Instance.GetSpellAttackByWeaponType(weapon_type, actor.ClassId); //gets the SpellName
                                                                                                   //Debug.Log(" sn info is " + sn.GetIndex() + sn.GetSpellName());
                                                                                                   //SpellManager.Instance.SetBaseQForAttack(sn.GetIndex(), 0); //sometimes this is modified but I need to make sure it is always 0 at the start
                                                                                                   //Debug.Log("getting attack spell, range is " + sn.GetRangeXYMax() + " " + sn.GetBaseQ());
                                                                                                   //Debug.Log(" sn info is " + sn.GetIndex() + sn.GetSpellName());
            return sn.Index;
        }
    }

    //called in CombatPerformAbilityState, UIAbilityScrollList and in the SpellSlow creators
    //modifies the base CTR due to abilities the actor PlayerUnit may have
    public static int CalculateCTR(PlayerUnit actor, SpellName sn)
    {

        int ctr = sn.CTR;
        int version = sn.Version;

        if ( ctr == 0)
        {
            return ctr;
        }

        if (ctr > 100)
        {
            int z1 = ctr - 100;
            ctr = z1 / actor.StatTotalSpeed;
        }

        if ( version == NameAll.VERSION_CLASSIC)
        {
            //Debug.Log("ctr is " + ctr);

            int commandSet = sn.CommandSet;
            if (commandSet != NameAll.COMMAND_SET_CHARGE && commandSet != NameAll.COMMAND_SET_JUMP && commandSet != NameAll.COMMAND_SET_SING && commandSet != NameAll.COMMAND_SET_DANCE) //jump and charge don't have short charge working for them
            {
                //Debug.Log("ctr is " + ctr);
                if (PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.SUPPORT_SHORT_CHARGE, NameAll.ABILITY_SLOT_SUPPORT)) //short charge
                {
                    //rounds up
                    //Debug.Log("ctr is " + ctr);
                    ctr = (ctr / 2);
                }
            }

            //Debug.Log("ctr is " + ctr);
            if (PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.SUPPORT_NO_CHARGE, NameAll.ABILITY_SLOT_SUPPORT)) //no charge
            {
                ctr = 0; //Debug.Log("ctr is " + ctr);
            }
        }
        else
        {
            if (ctr > NameAll.MAX_CTR)
            {
                ctr = NameAll.MAX_CTR;
            }

            if (PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.SUPPORT_QUICK_CAST, NameAll.ABILITY_SLOT_SUPPORT))
            {
                ctr = ((ctr * 3) / 4); //rounds down
            }

            if (ctr < 1)
                ctr = 1; //all charge spells have a min time I guess
        }

        return ctr;
    }

    
    //tests if the ability is auto target, if yes moves to MoveToConfirmState
    public static bool IsAutoTargetAbility(SpellName sn)
    {
        //Debug.Log("in is auto target ability " + sn.RangeXYMin);
        if (sn.RangeXYMin == NameAll.SPELL_RANGE_MIN_SELF_TARGET)
        {
            //Debug.Log("in is auto target ability true ");
            return true;
        }

        return false;
    }

    //gets an array of strings so that the preview panel can be updated
    //offline: called in CombatConfirmAbilityTargetState
    //online: called in GameLoopState for other
    public static List<string> GetHitPreview(Board board, CombatTurn turn, Tile targetTile)
    {
        List<int> outEffect;
        List<string> strList;
        if (turn.spellName.CommandSet == NameAll.COMMAND_SET_MATH_SKILL)
        {
            outEffect = CalculationResolveAction.GetFastActionPreview(board, turn.actor, turn.spellName2, targetTile);
            strList = GetPreviewList(turn.spellName2, turn.actor, targetTile, outEffect);
            strList[0] = turn.spellName.AbilityName + ": " + turn.spellName2.AbilityName;
        }
        else
        {
            outEffect = CalculationResolveAction.GetFastActionPreview(board, turn.actor, turn.spellName, targetTile);
            strList = GetPreviewList(turn.spellName, turn.actor, targetTile, outEffect);
        }

        return strList;
        //previewPanel.SetHitPreview(strList[0], strList[1], strList[2], strList[3], strList[4]);
    }

	

}
