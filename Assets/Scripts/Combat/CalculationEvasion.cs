using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**
 
 */

public static class CalculationEvasion {

    #region Aurelian

    //need to return a list since how the evasion can be dodged is stored in this
    public static List<int> GetAurelianEvasion(int hitChance, Board board, SpellName sn, PlayerUnit actor, PlayerUnit target, bool forceFacing = false)
    {
        List<int> outHit = new List<int>();
        outHit.Add(SetInitialHitRange(hitChance));
        outHit.Add(0); outHit.Add(0); outHit.Add(0);

        if ( sn.EvasionReflect== NameAll.SPELL_EVASION_REFLECT || sn.EvasionReflect == NameAll.SPELL_EVASION)
        {
            if( sn.PMType == NameAll.SPELL_TYPE_PHYSICAL)
            {
                outHit = GetAurelianPhysicalEvasion(outHit, board, actor, target, forceFacing);
            }
            else if (sn.PMType == NameAll.SPELL_TYPE_MAGICAL)
            {
                outHit[0] = GetAurelianMagicalEvasion(outHit[0], sn, actor, target);
            }
            else if (sn.PMType == NameAll.SPELL_TYPE_NEUTRAL)
            {
                outHit = GetAurelianNeutralEvasion(outHit, board, actor, target, forceFacing);
            }
        }
        outHit[0] = SetHitRange(outHit[0]);
        return outHit;
    }

    static bool ConcentrateTest(int type, int actorSupport)
    {
        if( type == NameAll.SPELL_TYPE_MAGICAL && actorSupport == NameAll.SUPPORT_CHANNEL)
        {
            return true;
        }
        else if (type == NameAll.SPELL_TYPE_PHYSICAL && ( actorSupport == NameAll.SUPPORT_CONCENTRATE || actorSupport == NameAll.SUPPORT_FOCUS ) )
        {
            return true;
        }
        else if (type == NameAll.SPELL_TYPE_NEUTRAL && actorSupport == NameAll.SUPPORT_HONE)
        {
            return true;
        }
        return false;
    }

    private static List<int> GetAurelianNeutralEvasion(List<int> outEffect, Board board, PlayerUnit actor, PlayerUnit target, bool forceFacing = false)
    {
        double class_evade = target.StatTotalCEvade/2;
        double offhand_evade = (target.StatItemOffhandPEvade + target.StatItemOffhandMEvade)/2;//target.stat_item_offhand_physical_evade;
        double accessory_evade = (target.StatItemAccessoryPEvade + target.StatItemAccessoryMEvade)/2;  //target.getItemStat("accessory", "p_evade");
        double weapon_evade = target.StatItemWEvade/2;

        double hit_chance_double = outEffect[0];
        double base_hit_double = outEffect[0];

        //can't hit petrify or dead on physical
        if (StatusManager.Instance.IfNoHit(target.TurnOrder)) //statusLab.ifNoHit(target.getTurn_order())
        {
            outEffect[0] = 0;
            return outEffect;
        }

        //Debug.Log("1 outHit is " + outHit[0]);
        //evasion and reaction abilities do not trigger w/ any of these status's
        if (StatusManager.Instance.IfNoEvade(target.TurnOrder)) //statusLab.ifNoEvade(target.getTurn_order(), 1)
        {
            return outEffect;
        }

        if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEFENDING, true))
        {
            class_evade *= 2;
            offhand_evade *= 2;
            accessory_evade *= 2;
            weapon_evade *= 2;
        }

        if (StatusManager.Instance.IfStatusByUnitAndId(actor.TurnOrder, NameAll.STATUS_ID_DARKNESS, true))
        {
            class_evade *= 2;
            offhand_evade *= 2;
            accessory_evade *= 2;
            weapon_evade *= 2;
        }

        if (StatusManager.Instance.IfStatusByUnitAndId(actor.TurnOrder, NameAll.STATUS_ID_CONFUSION, true))
        {
            class_evade *= 2;
            offhand_evade *= 2;
            accessory_evade *= 2;
            weapon_evade *= 2;
        }

        if (ConcentrateTest(NameAll.SPELL_TYPE_NEUTRAL, actor.AbilitySupportCode))
        {
            class_evade = class_evade / 2;
            offhand_evade = offhand_evade / 2;
            accessory_evade = class_evade / 2;
            weapon_evade = class_evade / 2;
        }

        string zString;
        if (forceFacing)
        {
            zString = "front";
        }
        else
        {
            zString = GetAttackSide(board, actor, target);
        }

        if (zString.Equals("front"))
        {
            //hitChance = doubleBaseHit * ( 100 - cEvade ) * ( 100 - offHandPhyEvade ) * ( 100 - accPhyEvade ) * ( 100 - weaponEvade ) / 100000000;
            hit_chance_double = base_hit_double * (100 - class_evade) * (100 - offhand_evade) * (100 - accessory_evade) * (100 - weapon_evade) / 100000000;
        }
        else if (zString.Equals("back"))
        {
            //hitChance = ( ( doubleBaseHit  * ( 100 - accPhyEvade) ) / 100 );
            hit_chance_double = base_hit_double * (100 - accessory_evade / 2) / 100;
        }
        else {
            //hitChance = doubleBaseHit * ( 100 - offHandPhyEvade ) * ( 100 - accPhyEvade ) * ( 100 - weaponEvade ) / 1000000;
            hit_chance_double = base_hit_double * (100 - class_evade / 2) * (100 - offhand_evade) * (100 - accessory_evade) * (100 - weapon_evade / 2) / 100000000;
        }
        hit_chance_double = Math.Floor(hit_chance_double);

        if (hit_chance_double > 100)
        {
            hit_chance_double = 100;
        }
        else if (hit_chance_double < 0)
        {
            hit_chance_double = 0;
        }

        outEffect[0] = (int)hit_chance_double;

        return outEffect;
    }

    private static List<int> GetAurelianPhysicalEvasion(List<int> outEffect, Board board, PlayerUnit actor, PlayerUnit target, bool forceFacing = false)
    {
        double class_evade = target.StatTotalCEvade;
        double offhand_evade = target.StatItemOffhandPEvade;//target.stat_item_offhand_physical_evade;
        double accessory_evade = target.StatItemAccessoryPEvade;  //target.getItemStat("accessory", "p_evade");
        double weapon_evade = target.StatItemWEvade;

        double hit_chance_double = outEffect[0];
        double base_hit_double = outEffect[0];

        //can't hit petrify or dead on physical
        if (StatusManager.Instance.IfNoHit(target.TurnOrder)) //statusLab.ifNoHit(target.getTurn_order())
        {
            outEffect[0] = 0;
            return outEffect;
        }

        //Debug.Log("1 outHit is " + outHit[0]);
        //evasion and reaction abilities do not trigger w/ any of these status's
        if (StatusManager.Instance.IfNoEvade(target.TurnOrder)) //statusLab.ifNoEvade(target.getTurn_order(), 1)
        {
            return outEffect;
        }

        //check for gun and magic gun, does take into account blade grasp
        int weaponType = ItemManager.Instance.GetItemType(actor.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON );
        if ( weaponType == NameAll.ITEM_ITEM_TYPE_GUN || weaponType == NameAll.ITEM_ITEM_TYPE_PISTOL) //weaponType == NameAll.ITEM_ITEM_TYPE_MAGIC_GUN
        {
            return outEffect;
        }

        if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEFENDING, true))
        {
            class_evade *= 2;
            offhand_evade *= 2;
            accessory_evade *= 2;
            weapon_evade *= 2;
        }

        if (StatusManager.Instance.IfStatusByUnitAndId(actor.TurnOrder, NameAll.STATUS_ID_DARKNESS, true))
        {
            class_evade *= 2;
            offhand_evade *= 2;
            accessory_evade *= 2;
            weapon_evade *= 2;
        }

        if (StatusManager.Instance.IfStatusByUnitAndId(actor.TurnOrder, NameAll.STATUS_ID_CONFUSION, true))
        {
            class_evade *= 2;
            offhand_evade *= 2;
            accessory_evade *= 2;
            weapon_evade *= 2;
        }

        if (ConcentrateTest(NameAll.SPELL_TYPE_PHYSICAL, actor.AbilitySupportCode))
        {
            class_evade = class_evade / 2;
            offhand_evade = offhand_evade / 2;
            accessory_evade = class_evade / 2;
            weapon_evade = class_evade / 2;
        }

        string zString;
        if (forceFacing)
        {
            zString = "front";
        }
        else
        {
            zString = GetAttackSide(board, actor, target);
        }

        if (zString.Equals("front"))
        {
            //hitChance = doubleBaseHit * ( 100 - cEvade ) * ( 100 - offHandPhyEvade ) * ( 100 - accPhyEvade ) * ( 100 - weaponEvade ) / 100000000;
            hit_chance_double = base_hit_double * (100 - class_evade) * (100 - offhand_evade) * (100 - accessory_evade) * (100 - weapon_evade) / 100000000;
        }
        else if (zString.Equals("back"))
        {
            //hitChance = ( ( doubleBaseHit  * ( 100 - accPhyEvade) ) / 100 );
            hit_chance_double = base_hit_double * (100 - accessory_evade/2) / 100;
        }
        else {
            //hitChance = doubleBaseHit * ( 100 - offHandPhyEvade ) * ( 100 - accPhyEvade ) * ( 100 - weaponEvade ) / 1000000;
            hit_chance_double = base_hit_double * (100 - class_evade/2) * (100 - offhand_evade) * (100 - accessory_evade) * (100 - weapon_evade/2) / 100000000;
        }
        hit_chance_double = Math.Floor(hit_chance_double);

        if (hit_chance_double > 100)
        {
            hit_chance_double = 100;
        }
        else if (hit_chance_double < 0)
        {
            hit_chance_double = 0;
        }

        outEffect[0] = (int)hit_chance_double;
        //check for within one panel and apply one panel reaction here (blade grasp?)
        if (target.IsAbilityEquipped(NameAll.REACTION_DEFLECT_CLOSE, NameAll.ABILITY_SLOT_REACTION)
            && MapTileManager.Instance.GetDistanceBetweenPlayerUnits(actor, target) <= 2) //concentrate
        {
            outEffect[0] = ((100 - target.GetReactionNumber()) * outEffect[0]) / 100;
            outEffect[3] = (int)base_hit_double - ((int)(base_hit_double - hit_chance_double)) - outEffect[0]; //miss chance from reaction
        }
        //check for projectile and apply projectile reaction here
        if (target.IsAbilityEquipped(NameAll.REACTION_DEFLECT_FAR, NameAll.ABILITY_SLOT_REACTION)
            && MapTileManager.Instance.GetDistanceBetweenPlayerUnits(actor,target) >= 2) //concentrate
        {
            outEffect[0] = ((100 - target.GetReactionNumber()) * outEffect[0]) / 100;
            outEffect[3] = (int)base_hit_double - ((int)(base_hit_double - hit_chance_double)) - outEffect[0]; //miss chance from reaction
        }
        outEffect[1] = (int)(base_hit_double - hit_chance_double); //chance of evasion or block, meh for separating them

        return outEffect;
    }

    private static int GetAurelianMagicalEvasion(int base_hit, SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        double hit_chance = 0;

        //can't hit petrify or dead unless certain spells which cure it with no evasion
        if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_PETRIFY, true)) //statusLab.ifStatusByUnitAndId(target.getTurn_order(), "petrify", true)
        {
            if (StatusManager.Instance.IfStatusCuredBySpell(NameAll.STATUS_ID_PETRIFY, sn)) //statusLab.ifStatusCuredBySpell("petrify", sn)
            {
                //don't think this is evadable
                return base_hit;
            }
            else {
                return 0;
            }
        }

        if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEAD, true) ) //statusLab.ifStatusByUnitAndId(target.getTurn_order(), "dead", true)
        {
            if (StatusManager.Instance.IfStatusCuredBySpell(NameAll.STATUS_ID_DEAD, sn)  ) //statusLab.ifStatusCuredBySpell("dead", sn)
            {
                //don't think this is evadable
                return base_hit;
            }
            else {
                return 0;
            }
        }

        if (StatusManager.Instance.IfNoHit(target.TurnOrder)) //statusLab.ifNoHit(target.getTurn_order())
        {
            return 0;
        }

        if (StatusManager.Instance.IfNoEvade(target.TurnOrder, 1)) //statusLab.ifNoEvade(target.getTurn_order(), 1)
        {
            return base_hit;
        }

        double offhand_evade = target.StatItemOffhandMEvade;// target.stat_item_offhand_magic_evade; //stat_item_offhand_magic_evade
        double accessory_evade = target.StatItemAccessoryMEvade;
        if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEFENDING, true)) // statusLab.ifStatusByUnitAndId(target.getTurn_order(), "defending", true) 
        {
            offhand_evade *= 2;
            accessory_evade *= 2;
        }
        //if (target.IsAbilityEquipped(NameAll.REACTION_ABANDON, "reaction")) //abandon //target.isAbilityEquipped("ar_aba_1", "reaction")
        //{
        //    offhand_evade *= 2;
        //    accessory_evade *= 2;
        //}
        if (StatusManager.Instance.IfStatusByUnitAndId(actor.TurnOrder, NameAll.STATUS_ID_CONFUSION, true)) //statusLab.ifStatusByUnitAndId(actor.getTurn_order(), "confusion", true)
        {
            offhand_evade *= 2;
            accessory_evade *= 2;
        }

        if( ConcentrateTest(sn.PMType,actor.AbilitySupportCode ))
        {
            offhand_evade = offhand_evade/2;
            accessory_evade = accessory_evade/2;
        }

        hit_chance = Math.Floor(base_hit * (100 - offhand_evade) * (100 - accessory_evade) / 10000);
        if (hit_chance > 100)
        {
            hit_chance = 100;
        }
        else if (hit_chance < 0)
        {
            hit_chance = 0;
        }
        int z1 = (int)hit_chance;
        return z1;
    }

    #endregion


    public static List<int> GetEvasionHitChance(int modified_hit, Board board, SpellName sn, PlayerUnit actor, PlayerUnit target, bool allowBladeGrasp = true, bool forceFacing = false) //last two, two swords nonsense
    {
        List<int> outHit = new List<int>(); //Debug.Log("in evasion hit chance");
        outHit.Add(modified_hit);
        outHit.Add(0); outHit.Add(0); outHit.Add(0);

        int hit_chance = modified_hit;

        if (sn.EvasionReflect == 1 || sn.EvasionReflect == 2)
        {
            if (sn.PMType == 0)
            {
                bool ninjaThrow = false;
                if (sn.CommandSet == NameAll.COMMAND_SET_THROW)
                {
                    ninjaThrow = true;
                }
                outHit = GetPhysicalEvasion(modified_hit, board, actor, target, ninjaThrow, allowBladeGrasp, forceFacing);
            }
            else if (sn.PMType == 1)
            {
                hit_chance = GetMagicalEvasion(hit_chance, sn, actor, target);
                outHit[0] = hit_chance;
            }
        }
        else if( sn.CommandSet == NameAll.COMMAND_SET_JUMP) //jump has its own rules
        {
            outHit[0] = GetJumpEvasion(sn.BaseHit, target, actor);
        }

        outHit[0] = SetHitRange(outHit[0]);
        return outHit;
    }

    public static int SetHitRange(int hit_chance)
    {
        if (hit_chance > 100)
        {
            hit_chance = 100;
        }
        else if (hit_chance < 0)
        {
            hit_chance = 0;
        }
        return hit_chance;
    }

    public static int SetInitialHitRange(int hit_chance)
    {
        if (hit_chance > 105)
        {
            hit_chance = 105;
        }
        else if (hit_chance < 0)
        {
            hit_chance = 0;
        }
        return hit_chance;
    }

    //using shortcut and having the evasion already calculated
    //magic evasion calculation is:
    //hitChance = ( baseHit * ( 100 - shieldEvade ) * ( 100 - accEvade ) / 10000 );
    private static int GetMagicalEvasion(int base_hit, SpellName sn, PlayerUnit actor, PlayerUnit target )
    {
        double hit_chance = 0;

        //can't hit petrify or dead unless certain spells which cure it with no evasion
        if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_PETRIFY, true) ) //statusLab.ifStatusByUnitAndId(target.getTurn_order(), "petrify", true)
        {
            if ( StatusManager.Instance.IfStatusCuredBySpell(NameAll.STATUS_ID_PETRIFY, sn) ) //statusLab.ifStatusCuredBySpell("petrify", sn)
            {
                //don't think this is evadable
                return base_hit;
            }
            else {
                return 0;
            }
        }

        if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEAD, true) ) //statusLab.ifStatusByUnitAndId(target.getTurn_order(), "dead", true)
        {
            if (StatusManager.Instance.IfStatusCuredBySpell(NameAll.STATUS_ID_DEAD, sn) ) //statusLab.ifStatusCuredBySpell("dead", sn)
            {
                //don't think this is evadable
                return base_hit;
            }
            else {
                return 0;
            }
        }

        if (StatusManager.Instance.IfNoHit(target.TurnOrder) ) //statusLab.ifNoHit(target.getTurn_order())
        {
            return 0;
        }

        if (StatusManager.Instance.IfNoEvade(target.TurnOrder,1) ) //statusLab.ifNoEvade(target.getTurn_order(), 1)
        {
            return base_hit;
        }

        double offhand_evade = target.StatItemOffhandMEvade;// target.stat_item_offhand_magic_evade; //stat_item_offhand_magic_evade
        double accessory_evade = target.StatItemAccessoryMEvade;
        if ( StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEFENDING, true) ) // statusLab.ifStatusByUnitAndId(target.getTurn_order(), "defending", true) 
        {
            offhand_evade *= 2;
            accessory_evade *= 2;
        }
        if ( target.IsAbilityEquipped( NameAll.REACTION_ABANDON,NameAll.ABILITY_SLOT_REACTION) ) //abandon //target.isAbilityEquipped("ar_aba_1", "reaction")
        {
            offhand_evade *= 2;
            accessory_evade *= 2;
        }
        if (StatusManager.Instance.IfStatusByUnitAndId(actor.TurnOrder, NameAll.STATUS_ID_CONFUSION, true) ) //statusLab.ifStatusByUnitAndId(actor.getTurn_order(), "confusion", true)
        {
            offhand_evade *= 2;
            accessory_evade *= 2;
        }
        
        hit_chance = Math.Floor(base_hit * (100 - offhand_evade) * (100 - accessory_evade) / 10000);
        if (hit_chance > 100)
        {
            hit_chance = 100;
        }
        else if (hit_chance < 0)
        {
            hit_chance = 0;
        }
        int z1 = (int)hit_chance;
        return z1;
    }

    private static List<int> GetPhysicalEvasion(int base_hit, Board board, PlayerUnit actor, PlayerUnit target, bool ninjaThrow = false, bool allowBladeGrasp = true, bool forceFacing = false)
    {
        List<int> outHit = new List<int>(); //returns the 4 variables: total hit chance, evasion chance, block chance, counter chance
        outHit.Add(base_hit); //Debug.Log("in physical evasion");
        outHit.Add(0); outHit.Add(0); outHit.Add(0); //defaults to 0
        int hit_chance = base_hit;
        double class_evade = target.StatTotalCEvade;
        double offhand_evade = target.StatItemOffhandPEvade;//target.stat_item_offhand_physical_evade;
        double accessory_evade = target.StatItemAccessoryPEvade;  //target.getItemStat("accessory", "p_evade");
        double weapon_evade = 0;
        if ( target.IsAbilityEquipped(NameAll.REACTION_WEAPON_GUARD,NameAll.ABILITY_SLOT_REACTION) ) //weapon guard target.isAbilityEquipped("ar_wea_1", "reaction")
        {
            weapon_evade = target.StatItemWEvade; //target.getWeaponEvade("hand");
        }
        double base_hit_double = base_hit;
        double hit_chance_double = hit_chance;
        double target_brave = target.StatTotalBrave;

        //can't hit petrify or dead on physical
        if (StatusManager.Instance.IfNoHit(target.TurnOrder) ) //statusLab.ifNoHit(target.getTurn_order())
        {
            outHit[0] = 0;
            return outHit;
        }
        //Debug.Log("1 outHit is " + outHit[0]);
        //evasion and reaction abilities do not trigger w/ any of these status's
        if (StatusManager.Instance.IfNoEvade(target.TurnOrder) ) //statusLab.ifNoEvade(target.getTurn_order(), 1)
        {
            outHit[0] = base_hit; //Debug.Log(" 2outHit is " + outHit[0]);
            return outHit;
        }

        //check for gun and magic gun, does take into account blade grasp
        int weaponType = ItemManager.Instance.GetItemType( actor.ItemSlotWeapon , NameAll.ITEM_SLOT_WEAPON );
        if( weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_MAGIC_GUN )
        {
            return outHit;
        }
        else if(weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_GUN)
        {
            if (target.IsAbilityEquipped(NameAll.REACTION_BLADE_GRASP, NameAll.ABILITY_SLOT_REACTION) )//bladegrasp
            {
                base_hit_double = Math.Floor(base_hit_double * (100 - target_brave) / 100);
                hit_chance = (int)base_hit_double;
                //return hit_chance;
                outHit[0] = hit_chance;
                outHit[3] = base_hit - hit_chance; //only way it can be dodged is blade grasp or the ability just not working
                return outHit;
            }
            else {
                return outHit;
            }
        }
        
        if (target.IsAbilityEquipped(NameAll.REACTION_ABANDON, NameAll.ABILITY_SLOT_REACTION))//abandon
        {
            class_evade *= 2;
            offhand_evade *= 2;
            accessory_evade *= 2;
            weapon_evade *= 2;
        }
        if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEFENDING, true) )
        {
            class_evade *= 2;
            offhand_evade *= 2;
            accessory_evade *= 2;
            weapon_evade *= 2;
        }
        if (StatusManager.Instance.IfStatusByUnitAndId(actor.TurnOrder, NameAll.STATUS_ID_DARKNESS, true))
        { 
            class_evade *= 2; Debug.Log("actor is blind, evade is being doubled " + class_evade);
            offhand_evade *= 2;
            accessory_evade *= 2;
            weapon_evade *= 2;
        }
        if (StatusManager.Instance.IfStatusByUnitAndId(actor.TurnOrder, NameAll.STATUS_ID_CONFUSION, true))
        {
            class_evade *= 2;
            offhand_evade *= 2;
            accessory_evade *= 2;
            weapon_evade *= 2;
        }

        //blade grasp and arrow guard handled in other function
        if (actor.IsAbilityEquipped(NameAll.SUPPORT_CONCENTRATE, NameAll.ABILITY_SLOT_SUPPORT) || actor.ClassId == NameAll.CLASS_MIME) //concentrate
        {
            hit_chance = GetBladeGraspEvasion(base_hit, actor, target, ninjaThrow, allowBladeGrasp);
            outHit[0] = hit_chance;
            outHit[3] = base_hit - hit_chance; //only way it can be dodged is blade grasp or the ability just not working
            return outHit;
        }

        string zString;
        if( forceFacing)
        {
            zString = "front";
        }
        else
        {
            zString = GetAttackSide(board, actor, target);
        }
        
        if (zString.Equals("front"))
        {
            //hitChance = doubleBaseHit * ( 100 - cEvade ) * ( 100 - offHandPhyEvade ) * ( 100 - accPhyEvade ) * ( 100 - weaponEvade ) / 100000000;
            hit_chance_double = base_hit_double * (100 - class_evade) * (100 - offhand_evade) * (100 - accessory_evade) * (100 - weapon_evade) / 100000000;
            //Debug.Log("in attack from front " + hit_chance_double + " " + class_evade);
        }
        else if (zString.Equals("back"))
        {
            //hitChance = ( ( doubleBaseHit  * ( 100 - accPhyEvade) ) / 100 );
            hit_chance_double = base_hit_double * (100 - accessory_evade) / 100;
        }
        else {
            //hitChance = doubleBaseHit * ( 100 - offHandPhyEvade ) * ( 100 - accPhyEvade ) * ( 100 - weaponEvade ) / 1000000;
            hit_chance_double = base_hit_double * (100 - offhand_evade) * (100 - accessory_evade) * (100 - weapon_evade) / 1000000;
        }
        hit_chance_double = Math.Floor(hit_chance_double);
        if (hit_chance_double > 100)
        {
            hit_chance_double = 100;
        }
        else if (hit_chance_double < 0)
        {
            hit_chance_double = 0;
        }
        hit_chance = (int)hit_chance_double;
        outHit[1] = base_hit - hit_chance;//this is evasion or block, meh for seperating them for now
        hit_chance = GetBladeGraspEvasion(hit_chance, actor, target, ninjaThrow, allowBladeGrasp);
        outHit[3] = base_hit - outHit[1] - hit_chance; //base - dodge chance - remaining hit chance = amount blade grasp counts for evasion
        outHit[0] = hit_chance;
        if( outHit[1] < 0)
        {
            outHit[1] = 0;
        }
        if (outHit[2] < 0)
        {
            outHit[2] = 0;
        }
        if (outHit[3] < 0)
        {
            outHit[4] = 0;
        }
        return outHit;
    }

    public static string GetAttackSide(Board board, PlayerUnit actor, PlayerUnit target)
    {
        //Debug.Log("fix new mtm shit here");
        string zString;
        Tile tileActor = board.GetTile(actor);
        Tile tileTarget = board.GetTile(target);

        int z1 = tileActor.pos.x - tileTarget.pos.x;
        int z2 = tileActor.pos.y - tileTarget.pos.y;
        int z3 = Math.Abs(z1);
        int z4 = Math.Abs(z2);
        //Debug.Log(" in Get attack side: " + z1 + " " + z2 + " " + z3 + " " + z4 + " " + target.GetFacing_direction());
        if (target.Dir == Directions.North)
        { //n
            if (z3 > z4)
            {
                zString = "side";
            }
            else if (z2 < 0)
            {
                zString = "back";
            }
            else {
                zString = "front";
            }
        }
        else if (target.Dir == Directions.East)
        { //east
            if (z3 < z4)
            {
                zString = "side";
            }
            else if (z1 < 0)
            {
                zString = "back";
            }
            else {
                zString = "front";
            }
        }
        else if (target.Dir == Directions.South)
        { //s
            if (z3 > z4)
            {
                zString = "side";
            }
            else if (z2 < 0)
            {
                zString = "front";
            }
            else {
                zString = "back";
            }
        }
        else { //west
            if (z3 < z4)
            {
                zString = "side";
            }
            else if (z1 < 0)
            {
                zString = "front";
            }
            else {
                zString = "back";
            }
        }

        return zString;
    }

    //called in physical check, this also covers catch and arrow guard
    //for two swords if blade grasp has been triggered this doesn't return
    private static int GetBladeGraspEvasion(int base_hit, PlayerUnit actor, PlayerUnit target, bool ninja_throw, bool allowBladeGrasp)
    {
        int hit_chance = base_hit;
        double base_hit_double = base_hit;
        double hit_chance_double = base_hit;
        double target_brave = target.StatTotalBrave;

        if (target.IsAbilityEquipped(NameAll.REACTION_BLADE_GRASP, NameAll.ABILITY_SLOT_REACTION) && allowBladeGrasp ) //bladegrasp, disabled if triggered on earlier Two Swords attack
        {
            hit_chance_double = (base_hit_double * (100 - target_brave)) / 100;
            hit_chance_double = Math.Floor(hit_chance_double);
            if (hit_chance_double > 100)
            {
                hit_chance_double = 100;
            }
            else if (hit_chance_double < 0)
            {
                hit_chance_double = 0;
            }
            hit_chance = (int)hit_chance_double;
            return hit_chance;
        }
        else if (ninja_throw && target.IsAbilityEquipped(NameAll.REACTION_CATCH, NameAll.ABILITY_SLOT_REACTION)) //catch
        {
            hit_chance_double = (base_hit_double * (100 - target_brave)) / 100;
            hit_chance_double = Math.Floor(hit_chance_double);
            if (hit_chance_double > 100)
            {
                hit_chance_double = 100;
            }
            else if (hit_chance_double < 0)
            {
                hit_chance_double = 0;
            }
            hit_chance = (int)hit_chance_double;
            return hit_chance;
        }
        else if (target.IsAbilityEquipped(NameAll.REACTION_ARROW_GUARD, NameAll.ABILITY_SLOT_REACTION)) //Arrow Guard
        {
            int weaponType = ItemManager.Instance.GetItemType(actor.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON);
            if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_LONGBOW || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_CROSSBOW )
            {
                hit_chance_double = (base_hit_double * (100 - target_brave)) / 100;
                hit_chance_double = Math.Floor(hit_chance_double);
                if (hit_chance_double > 100)
                {
                    hit_chance_double = 100;
                }
                else if (hit_chance_double < 0)
                {
                    hit_chance_double = 0;
                }
                hit_chance = (int)hit_chance_double;
                return hit_chance;
            }
        }

        return hit_chance;
    }

    private static int GetJumpEvasion(int base_hit, PlayerUnit target, PlayerUnit actor)
    {
        int hit_chance = base_hit;
        //double base_hit_double = base_hit;
        //double hit_chance_double = base_hit;
        //double target_brave = target.stat_total_brave;

        if (target.IsAbilityEquipped(NameAll.REACTION_BLADE_GRASP, NameAll.ABILITY_SLOT_REACTION)) //bladegrasp
        {
            hit_chance = 100 - target.StatTotalBrave;
            return hit_chance;
        }
        else if (target.IsAbilityEquipped(NameAll.REACTION_ARROW_GUARD, NameAll.ABILITY_SLOT_REACTION)) //arrowguard
        {
            int weaponType = ItemManager.Instance.GetItemType(actor.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON);
            if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_LONGBOW || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_CROSSBOW)
            {
                hit_chance = 100 - target.StatTotalBrave;
                return hit_chance;
            }
        }

        hit_chance = 100;
        return hit_chance;
    }

}
