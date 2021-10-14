using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
//basic point of this Is to aid in the slow spell resolution, fast spell resolution and fast spell display
    //spellname et al go in and chance to hit and effect are returned in an int array
        //can get more complicated (knockback, spells with multiple effects etc)
        //typically statuses (except those integral to the mod calculations) are handled outside of this
            //reflect, reaction etc handled elsewhere


//has knockback and crit calculations in here too

 //to do
    when spellObject class Is created, fix mod 2
    mod attack and two hands, which weapon etch will likely need to be updated
 */

public class CalculationMod {
    static int bugTest = 0;
    //wrapper for the mod calculations
    //returns: chance it hit, main_effect, Is_knockback, main_effect2 etc
    public static List<int> GetModCalculation(SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        List<int> out_effect = new List<int>();
        List<int> temp = new List<int>(); //handles outputs form the mods
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);
        int mod = sn.Mod;

        if( sn.Version != NameAll.VERSION_CLASSIC)
        {
            mod = mod % 10;
            if (mod == NameAll.MOD_ATTACK)
            {
                out_effect = GetModAttack(out_effect, sn, actor, target);
            }
            else if (mod == NameAll.MOD_PHYSICAL)
            {
                out_effect = GetModPhysical(out_effect, sn, actor, target);
            }
            else if( mod == NameAll.MOD_MAGICAL)
            {
                out_effect = GetModMagical(out_effect, sn, actor, target); //Debug.Log("getting mod magical");
            }
            else if (mod == NameAll.MOD_NEUTRAL)
            {
                out_effect = GetModNeutral(out_effect, sn, actor, target);
            }
            else if (mod == NameAll.MOD_NULL) //goes straight to dft, zodiac usually matters
            {
                out_effect = GetModNull(out_effect, sn, actor, target);
            }

        }
        else
        {
            if (mod == 10)
            { //weapon attack
                out_effect = GetMod10(sn, actor, target);
            }
            else if (mod == 0)
            {
                out_effect = GetMod0(sn, actor, target);
            }
            else if (mod == 1)
            {
                out_effect = GetMod1(sn, actor, target);
            }
            else if (mod == 2)
            {
                out_effect = GetMod2(sn, actor, target);
            }
            else if (mod == 3)
            {
                out_effect = GetMod3(sn, actor, target);
            }
            else if (mod == 4)
            {
                out_effect = GetMod4(sn, actor, target);
            }
            else if (mod == 5)
            {
                out_effect = GetMod5(sn, actor, target);
            }
            else if (mod == 6)
            {
                out_effect = GetMod6(sn, actor, target);
            }
            else if (mod == 8)
            {
                out_effect = GetMod8(sn, actor, target);
            }
            else if (mod == 9)
            {
                out_effect = GetMod9(sn, actor, target);
            }
            else if (mod == 11)
            { //magic gun
                out_effect = GetMod11(sn, actor, target);
            }
            else if (mod == 16)
            {
                out_effect = GetMod16(sn, actor);
            }
            else if (mod == 7)
            {
                out_effect = GetMod7(sn, actor, target);
            }
        }
        
        return out_effect;
    }

    #region Aurelian Mods

    private static List<int> GetModAttack(List<int> outEffect, SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        int xa = 0; int z1;

        int wp = PlayerManager.Instance.GetWeaponPower(actor.TurnOrder, false, true);
        int weaponType = ItemManager.Instance.GetItemType(actor.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON);//actor.GetWeaponType();
        int chargeK = sn.BaseQ; //charge attack, BaseQ is turned to WP later in this formula
        //Debug.Log("chargeK is " + chargeK);
//Attack(scales)

        if (weaponType == NameAll.ITEM_ITEM_TYPE_FIST )
        {
            xa = (actor.StatTotalPA + chargeK) * actor.StatTotalBrave / 100;
            //Debug.Log("in calculating xa is " + xa);
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_SWORD || weaponType == NameAll.ITEM_ITEM_TYPE_GREATSWORD
              || weaponType == NameAll.ITEM_ITEM_TYPE_SPEAR )
        {
            xa = actor.StatTotalPA + chargeK;
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_DAGGER || weaponType == NameAll.ITEM_ITEM_TYPE_BOW
              || weaponType == NameAll.ITEM_ITEM_TYPE_KATANA)
        {
            xa = actor.StatTotalAgi + chargeK;
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_MACE)
        {
            //Random r = new Random();
            z1 = actor.StatTotalPA + chargeK;
            if (z1 > 0)
            {
                int z2 = z1 / 2;
                xa = UnityEngine.Random.Range((z1 - z2), (z1 + z2));
            }
            else {
                xa = 1;
            }
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_STICK )
        {
            xa = actor.StatTotalMA + chargeK;
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_GUN || weaponType == NameAll.ITEM_ITEM_TYPE_PISTOL)
        {
            xa = wp + chargeK;
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_CROSSBOW || weaponType == NameAll.ITEM_ITEM_TYPE_WHIP)
        {
            xa = (actor.StatTotalPA + chargeK + actor.StatTotalAgi + chargeK + 2) / 2;
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_WAND)
        {
            xa = (actor.StatTotalMA + chargeK) * actor.StatTotalFaith / 100; ;
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_DECK)
        {
            xa = (actor.StatTotalAgi + chargeK) * actor.StatTotalCunning / 100; ;
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_INSTRUMENT )
        {
            xa = (actor.StatTotalMA + chargeK + actor.StatTotalAgi + chargeK + 2) / 2;
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_SCALES)
        {
            xa = GetXABlend(actor.StatTotalPA, actor.StatTotalMA, actor.StatTotalAgi);
        }
        else
        {
            //assuming it's barehanded
            xa = (actor.StatTotalPA + chargeK) * actor.StatTotalBrave / 100;
            Debug.Log("in calculating xa 2 is " + xa);
        }
        //Debug.Log(" xa is " + xa);
        //can all crit
        List<int> temp = CritCalculator(xa, target.TurnOrder, sn, actor);
        int crit_holder = temp[1]; //is it a crit, set to out_effect 2 later
        xa = temp[0]; //new xa value on crit
        
        int elemental_type = actor.GetWeaponElementType();
        //checks if the attack has an element and if actor has that element strengthend
        
        xa = IsAttackElementalStrengthened(xa, elemental_type, actor); //Debug.Log(" xa is " + xa);

        //if actor has attack up/magic attack up
        //assuming physical attacks gets attack up and magic get magic attack up
        xa = IsAttackSupportStrengthened(xa, sn.PMType, actor); //Debug.Log(" xa is " + xa);
        //if caster has Martial Arts and barehanded
        xa = IsAttackMartialArtsAndBareHanded(xa, actor); //Debug.Log(" xa is " + xa);
        //is berserk
        if (sn.PMType == NameAll.SPELL_TYPE_PHYSICAL)
        {
            xa = IsAttackBerserk(xa, actor); //Debug.Log(" xa is " + xa);
        }
        xa = AurelianXAAll(xa, actor, target, sn); //Debug.Log(" xa is " + xa); //sadist

        //defense up for target and corresponding phys/magic type
        xa = IsTargetDefenseStrengthened(xa, sn, target); //Debug.Log(" xa is " + xa);
        xa = IsTargetProtectShell(xa, sn, target); //Debug.Log(" xa is " + xa);

        if (sn.PMType == NameAll.SPELL_TYPE_PHYSICAL)
        {
            xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHARGING); //Debug.Log(" xa is " + xa);
            xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_SLEEP); //Debug.Log(" xa is " + xa);
            xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHICKEN); //Debug.Log(" xa is " + xa);//handled specially below
        }
        
        //zodiac compat
        xa = CalculationZodiac.GetZodiacMain(xa, actor, target); //Debug.Log(" xa is " + xa);

        //use damage_formula_type
        outEffect[0] = xa;
        outEffect[1] = wp;
        if (bugTest == 1)
        {
            Debug.Log("In get ModAttackPreview " + outEffect[0] + " " + outEffect[1] + " " + outEffect[2] + " " + outEffect[3] + " ");
        }
        outEffect = CalculationHitDamage.GetHitAurelian(outEffect, sn, actor, target);

        outEffect[1] = GetDamageElemental(outEffect[1], elemental_type, target);

        //loads in the crit, some CalculationHitDamage already check for this
        if (outEffect[2] == 0)
        {
            outEffect[2] = crit_holder;
        }
        //has knockback equipped and target does not have anti knockback equipped
        if(PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.SUPPORT_KNOCKBACK, NameAll.ABILITY_SLOT_SUPPORT) && 
            !PlayerManager.Instance.IsAbilityEquipped(target.TurnOrder,NameAll.SUPPORT_STAND_GROUND, NameAll.ABILITY_SLOT_SUPPORT))
        {
            outEffect[2] = NameAll.CRIT_KNOCKBACK_SUCCESS;
        }

        if (bugTest == 1)
        {
            Debug.Log("In get ModAttackPreview " + outEffect[0] + " " + outEffect[1] + " " + outEffect[2] + " " + outEffect[3] + " ");
        }

        return outEffect;
    }

    //Null or Zodiac and maybe xa depending on the dft
    private static List<int> GetModNull(List<int> outEffect, SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        int xa;
        if (sn.DamageFormulaType == 0) //xa doesn't matter, just going to use baseQ (doing this saves some calculations)
        {
            xa = 0;
        }
        else
        {
            xa = GetAurelianXA(sn.Mod, actor); //xa defaults to balance
            xa = AurelianXAAll(xa, actor, target, sn); //sadist
        }

        outEffect[0] = xa;
        outEffect = CalculationHitDamage.GetHitAurelian(outEffect, sn, actor, target);
        return outEffect;
    }

    static int GetAurelianXA(int snMod, PlayerUnit actor)
    {
        int xa = 0;
        if( snMod == NameAll.MOD_PHYSICAL )
        {
            xa = actor.StatTotalPA;
        }
        else if (snMod == NameAll.MOD_NEUTRAL)
        {
            xa = actor.StatTotalAgi;
        }
        else if (snMod == NameAll.MOD_MAGICAL)
        {
            xa = actor.StatTotalMA;
        }
        else if( snMod == NameAll.MOD_NULL)
        {
            xa = GetXABlend(actor.StatTotalPA, actor.StatTotalMA, actor.StatTotalAgi);
        }
        else if (snMod == NameAll.MOD_PHYSICAL_MAGICAL || snMod == NameAll.MOD_MAGICAL_PHYSICAL)
        {
            xa = GetXABlend(actor.StatTotalPA,actor.StatTotalMA);
        }
        else if (snMod == NameAll.MOD_PHYSICAL_NEUTRAL || snMod == NameAll.MOD_NEUTRAL_PHYSICAL)
        {
            xa = GetXABlend(actor.StatTotalPA, actor.StatTotalAgi);
        }
        else if (snMod == NameAll.MOD_MAGICAL_NEUTRAL || snMod == NameAll.MOD_NEUTRAL_MAGICAL)
        {
            xa = GetXABlend(actor.StatTotalAgi, actor.StatTotalMA);
        }
        else if (snMod == NameAll.MOD_PHYSICAL_AGI )
        {
            xa = actor.StatTotalAgi; //does the physical mod and dft but with agi as the primary stat
        }

        return xa;
    }

    static int GetXABlend(int stat1, int stat2, int stat3 = 0)
    {
        int xa = 0;
        if( stat3 == 0)
        {
            xa = (1 + stat1 + stat2) / 2;
            int z1 = Math.Abs(stat1 - stat2);
            if( z1 <= 3)
            {
                xa = (5 * xa) / 4; //boost for being within 3
                //if ( z1 == 0)
                //{
                //    xa = (5 * xa) / 4; //boost for being equal
                //}
            }
        }
        else
        {
            int z1 = Math.Abs(stat1 - stat2);
            z1 += Math.Abs(stat1 - stat3);
            z1 += Math.Abs(stat2 - stat3);
            if ( z1 <= 3)
            {
                xa = Mathf.Max(stat1, stat2, stat3); //old way: xa = (2 + stat1 + stat2 + stat3)/3 * ((6-z1)/3); //perfect balance *2, off by 1-3 *1, off by >3, min stat
                xa = (5 * xa) / 4; //boost for being within 3
                if ( z1 == 0) //boost for being equal
                {
                    xa = (5*xa) / 4;
                }
            }
            else
            {
                xa = Math.Min(stat1, stat2);
                xa = Math.Min(xa, stat3);
            }
            
        }

        return xa;
    }

    private static List<int> GetModPhysical(List<int> outEffect, SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        int xa = GetAurelianXA(sn.Mod, actor);

        //crit chance
        List<int> temp = new List<int>();
        temp = CritCalculator(xa, target.TurnOrder, sn, actor);
        int crit_holder = temp[1]; //is it a crit, set to out_effect 2 later
        xa = temp[0]; //new xa value on crit

        //checks if the attack has an element and if actor has that element strengthend
        xa = IsAttackElementalStrengthened(xa, sn, actor);
        xa = IsAurelianAttackSupportStrengthened(xa, NameAll.MOD_PHYSICAL, actor); //physical attack up for this
        xa = IsAttackMartialArtsAndBareHanded(xa, actor); //martial arts has to be barehanded or it's just a superior version to physattackup
        xa = IsAttackBerserk(xa, actor); //is berserk
        xa = AurelianXAAll(xa, actor, target, sn); //sadist
        xa = IsAurelianTargetDefenseStrengthened(xa, sn, NameAll.MOD_PHYSICAL, target); //defense up for target and corresponding phys/magic type
        xa = IsTargetProtectShell(xa, sn, target);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHARGING);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_SLEEP);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHICKEN); //handled specially below

        xa = CalculationZodiac.GetZodiacMain(xa, actor, target); //zodiac compat, modifies the xa
        outEffect[0] = xa;
        outEffect = CalculationHitDamage.GetHitAurelian(outEffect, sn, actor, target);
        
        //run the damage through to see if any weaknesses
        outEffect[1] = GetDamageElemental(outEffect[1], sn.ElementType, target);

        //loads in the crit, some CalculationHitDamage already check for this
        if (outEffect[2] == 0)
        {
            outEffect[2] = crit_holder;
        }

        if (bugTest == 1)
        {
            Debug.Log("In get ModPhysicalPreview " + outEffect[0] + " " + outEffect[1] + " " + outEffect[2] + " " + outEffect[3] + " ");
        }

        outEffect = GetBraveCunningModifier(outEffect, 1, actor.StatTotalBrave, target.StatTotalCunning );

        if (bugTest == 1)
        {
            Debug.Log("In get ModPhysicalPreview " + outEffect[0] + " " + outEffect[1] + " " + outEffect[2] + " " + outEffect[3] + " ");
        }

        return outEffect;
    }

    static int GetDamageElemental(int damage, int elementType, PlayerUnit target)
    {
        int targetId = target.TurnOrder;
        if ( elementType != NameAll.ITEM_ELEMENTAL_NONE && damage != 0)
        {
            //weak
            if (StatusManager.Instance.IsUnitWeakByElement(targetId, elementType ))
            {
                damage = damage * 2;
            }
            //half
            if (StatusManager.Instance.IsUnitHalfByElement(target, elementType))
            {
                damage = damage * 1 / 2;
            }
            //absorb
            if (StatusManager.Instance.IsUnitAbsorbByElement(targetId, elementType))
            {
                damage = damage * -1;
            }
        }
        return damage;
    }

    private static List<int> GetModMagical(List<int> outEffect, SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        //Debug.Log(" in mod moagical?");
        //for now assumes xa is ma
        int xa = GetAurelianXA(sn.Mod, actor); //Debug.Log(xa);
        xa = IsAttackElementalStrengthened(xa, sn, actor);
        xa = IsAurelianAttackSupportStrengthened(xa, NameAll.MOD_MAGICAL, actor);
        xa = AurelianXAAll(xa, actor,target, sn); //sadist
        xa = IsAurelianTargetDefenseStrengthened(xa, sn, NameAll.MOD_MAGICAL, target);
        xa = IsTargetProtectShell(xa, sn, target);
        xa = CalculationZodiac.GetZodiacMain(xa, actor, target);
        outEffect[0] = xa;
        if (bugTest == 1)
        {
            Debug.Log("In get ModMagical preview prior to hit " + outEffect[0] + " " + outEffect[1] + " " + outEffect[2] + " " + outEffect[3] + " ");
        }
        //numerator, denomiator, and element effects done in following formula
        outEffect = CalculationHitDamage.GetHitAurelian(outEffect, sn, actor, target);

        //long actor_faith = StatusManager.Instance.GetModFaith(actor.TurnOrder, actor.StatTotalFaith);
        //long target_faith = StatusManager.Instance.GetModFaith(target.TurnOrder, target.StatTotalFaith);

        //if ( sn.BaseHit > 1000) //base hit modified by faith
        //{
        //    long success = (actor_faith * target_faith * (xa + outEffect[0])) / (10000);
        //    outEffect[0] = (int)success;
        //}
        
        //run the damage through to see if any weaknesses
        outEffect[1] = GetDamageElemental(outEffect[1], sn.ElementType, target); // does this in the hit damage

        if (bugTest == 1)
        {
            Debug.Log("In get ModMagical after hit " + outEffect[0] + " " + outEffect[1] + " " + outEffect[2] + " " + outEffect[3] + " ");
        }
        return outEffect;
    }

    private static List<int> GetModNeutral(List<int> outEffect, SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        int xa = GetAurelianXA(sn.Mod, actor);
        int randomFactor = sn.BaseHit;
        if (randomFactor >= 1000)
            randomFactor -= 1000;

        xa += UnityEngine.Random.Range(0, (randomFactor % 10 + 1)); //random component; 10% of BaseHit as BaseQ is likely too strong
        //crit chance
        //List<int> temp = new List<int>();
        //temp = CritCalculator(xa);
        //int crit_holder = temp[1]; //is it a crit, set to out_effect 2 later
        //xa = temp[0]; //new xa value on crit

        //checks if the attack has an element and if actor has that element strengthend
        xa = IsAttackElementalStrengthened(xa, sn, actor);
        xa = IsAurelianAttackSupportStrengthened(xa, NameAll.MOD_NEUTRAL, actor); //physical attack up for this
        //xa = IsAttackMartialArtsAndBareHanded(xa, actor); //martial arts has to be barehanded or it's just a superior version to physattackup
        //xa = IsAttackBerserk(xa, actor); //is berserk
        xa = AurelianXAAll(xa, actor, target, sn); //sadist
        xa = IsAurelianTargetDefenseStrengthened(xa, sn, NameAll.MOD_NEUTRAL, target); //defense up for target and corresponding phys/magic type
        //xa = IsTargetProtectShell(xa, sn, target);
        //xa = IsTargetStatus(xa, target, "charging"); //is target charging, sleeping
        //xa = IsTargetStatus(xa, target, "sleeping");
        //xa = IsTargetStatus(xa, target, "chicken_frog"); //handled specially below

        xa = CalculationZodiac.GetZodiacMain(xa, actor, target); //zodiac compat, modifies the xa
        outEffect[0] = xa;
        if (bugTest == 1)
        {
            Debug.Log("In get ModPhysicalNeutral pre hit " + outEffect[0] + " " + outEffect[1] + " " + outEffect[2] + " " + outEffect[3] + " ");
        }
        outEffect = CalculationHitDamage.GetHitAurelian(outEffect, sn, actor, target);

        //run the damage through to see if any weaknesses
        outEffect[1] = GetDamageElemental(outEffect[1], sn.ElementType, target);

        //loads in the crit, some CalculationHitDamage already check for this
        //if (outEffect[2] == 0)
        //{
        //    outEffect[2] = crit_holder;
        //}

        outEffect = GetBraveCunningModifier(outEffect, 0, actor.StatTotalCunning, target.StatTotalBrave);

        if (bugTest == 1)
        {
            Debug.Log("In get ModPhysicalNeutral after hit " + outEffect[0] + " " + outEffect[1] + " " + outEffect[2] + " " + outEffect[3] + " ");
        }

        return outEffect;
    }

    //called in modPhysical and modNeutral
    static List<int> GetBraveCunningModifier(List<int> outEffect, int type, int actorStat, int targetStat)
    {
        //type 0 modifies hit, type 1 modifies dmg, type 2 modifies both
        //modifies the damage based on how large a % the unit is away from 50
        if( type == 1 || type == 2)
        {
            int damage = outEffect[1];
            int percentChange = (actorStat - 50) + (targetStat-50)*-1; //higher stat leaves you more vulnerable
            if (percentChange > 50)
            {
                percentChange = 50;
            }
            else if (percentChange < -50)
            {
                percentChange = -50;
            }
            damage = ((100 + percentChange) * damage) / 100;
            outEffect[1] = damage;
        }

        if (type == 0 || type == 2)
        {
            int hitChance = outEffect[0];
            int percentChange = ((actorStat - 50) + (targetStat-50) * -1)/2;
            if (percentChange > 25)
            {
                percentChange = 25;
            }
            else if (percentChange < -25)
            {
                percentChange = -25;
            }
            hitChance = ((100 + percentChange) * hitChance) / 100;
            outEffect[0] = hitChance;
        }

        return outEffect;
    }

    static int AurelianXAAll(int xa, PlayerUnit actor, PlayerUnit target, SpellName sn)
    {
        //Debug.Log("start testing sadist and envy " + xa);
        //sadist
        if( PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.SUPPORT_SADIST, NameAll.ABILITY_SLOT_SUPPORT))
        {
            if( StatusManager.Instance.IsSadist(target.TurnOrder ))
            {
                xa = (xa * 5) / 4; //Debug.Log("sadist added " + xa);
            }
        }
        else if (PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.SUPPORT_ENVY, NameAll.ABILITY_SLOT_SUPPORT))
        {
            if (StatusManager.Instance.IsPositiveStatus(target.TurnOrder))
            {
                xa = (xa * 5) / 4; //Debug.Log("envy added " + xa);
            }
        }

        if( sn.ElementType == NameAll.ITEM_ELEMENTAL_WEAPON && sn.CommandSet != NameAll.COMMAND_SET_ATTACK_AURELIAN) //WP added to xa, last mod before defense
        {
            xa += actor.GetWeaponPower(true, true);
        }
        //Debug.Log("end testing sadist and envy " + xa);
        return xa;
    }
    #endregion

    //mod 10 attack
    //charge +k is stored in sn.BaseQ, the baseQ is added during the CalculationHitDamage portion
    private static List<int> GetMod10(SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);

        int xa; int z1;
        //if(bugTest == 1)
        //{
        //    Debug.Log("testing mod10");
        //}
        int wp = PlayerManager.Instance.GetWeaponPower(actor.TurnOrder,false,true);
        int weaponType = ItemManager.Instance.GetItemType(actor.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON);//actor.GetWeaponType();
        int chargeK = sn.BaseQ; //charge attack, BaseQ is turned to WP later in this formula
        //Debug.Log("chargeK is " + chargeK);
        if (weaponType == NameAll.ITEM_ITEM_TYPE_FIST || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_KATANA
                || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_KNIGHT)
        {
            xa = (actor.StatTotalPA + chargeK) * actor.StatTotalBrave / 100;
            //Debug.Log("in calculating xa is " + xa);
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_DAGGER || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_NINJA
              || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_LONGBOW)
        {
            xa = (actor.StatTotalPA + chargeK + actor.StatTotalSpeed + chargeK) / 2;
            //Debug.Log("in calculating xa is " + xa);
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_SWORD || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_ROD
              || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_SPEAR || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_CROSSBOW)
        {
            xa = actor.StatTotalPA + chargeK;
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_AXE || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_BAG
              || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_HAMMER)
        {
            //Random r = new Random();
            z1 = actor.StatTotalPA + chargeK;
            if (z1 > 0)
            {
                xa = UnityEngine.Random.Range(1, z1);
            }
            else {
                xa = 1;
            }
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_STAFF || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_STICK)
        {
            xa = actor.StatTotalMA + chargeK;
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_GUN)
        {
            xa = wp + chargeK;
        }
        else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_CLOTH || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_DICTIONARY
              || weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_HARP)
        {
            xa = (actor.StatTotalPA + chargeK + actor.StatTotalMA + chargeK) / 2;
        }
        else {
            //assuming it's barehanded
            xa = (actor.StatTotalPA + chargeK) * actor.StatTotalBrave / 100;
            Debug.Log("in calculating xa 2 is " + xa);
        }

        //can all crit except magic gun which Is handled elsewhere
        List<int> temp = CritCalculator(xa, target.TurnOrder, sn, actor);
        int crit_holder = temp[1]; //is it a crit, set to out_effect 2 later
        xa = temp[0]; //new xa value on crit

        int elemental_type = actor.GetWeaponElementType(); //Debug.Log("elemental type in attack is " + elemental_type);
        //checks if the attack has an element and if actor has that element strengthend

        xa = IsAttackElementalStrengthened(xa, elemental_type, actor);

        //if actor has attack up/magic attack up
        //assuming physical attacks gets attack up and magic get magic attack up
        xa = IsAttackSupportStrengthened(xa, sn.PMType, actor);

        //if caster has Martial Arts and barehanded
        xa = IsAttackMartialArtsAndBareHanded(xa, actor);
        //is berserk
        xa = IsAttackBerserk(xa, actor );
        //defense up for target and corresponding phys/magic type
        xa = IsTargetDefenseStrengthened(xa, sn, target);
        xa = IsTargetProtectShell(xa, sn, target);
        //is target charging, sleeping
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHARGING);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_SLEEP);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHICKEN); //handled specially below
        //zodiac compat
        xa = CalculationZodiac.GetZodiacMain(xa, actor, target);
        if (bugTest == 1)
        {
            Debug.Log("In calculation mod 10 xa is " + xa );
        }

        //no longeruses the calculation hit damage and modifying the SN (replacing baseQ with the wp), instead just does the calculation here
        out_effect[0] = sn.BaseHit;
        out_effect[1] = xa * wp;

        //override snbaseq with units WP
        //SpellManager.Instance.SetBaseQForAttack(sn.Index, wp); //intentional, overrides the chargeK and is used to find hit damage in calculationhitdamage
        //out_effect = CalculationHitDamage.GetHitDamage(xa, sn, actor, target);
        //SpellManager.Instance.SetBaseQForAttack(sn.Index, 0); //because I modified the original object

        if (bugTest == 1)
        {
            Debug.Log("In calculation mod 10 end out effect array is " + out_effect[0] + "," + out_effect[1] + "," + out_effect[2] + "," + out_effect[3]);
        }
        //run the damage through to see if any weaknesses
        out_effect[1] = GetDamageElemental(out_effect[1], elemental_type, target); //elemental_type since its based on weapon

        //loads in the crit, some CalculationHitDamage already check for this
        if (out_effect[2] == 0)
        {
            out_effect[2] = crit_holder;
        }

        if (bugTest == 1)
        {
            Debug.Log("In calculation mod 10 end out effect array is " + out_effect[0] + "," + out_effect[1] + "," + out_effect[2] + "," + out_effect[3]);
        }

        return out_effect;
    }

    //doesn't do anything
    private static List<int> GetMod0(SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);
        out_effect = CalculationHitDamage.GetHitDamage(0, sn, actor, target);
        return out_effect;
    }

    private static List<int> GetMod1(SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        //success% = (MA + K + Z)
        //only depends on zodiac compat Z which Is equal to the zodiac compat portion of MA and K
        //zodaic mod 3 takes the MA, and K from above and does MA + Z
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);
        out_effect = CalculationHitDamage.GetHitDamage(0, sn, actor, target);
        return out_effect;
    }

    //[MOD 2] ~ physical attacks : damage variable
    //returning the modified xa value which goes into a ability specific formula for the final output
    //example spin fist, modifying the first PA value
    //returning ArrayList similar to the mod wrapper
    //Chakra returns two damage numbers
    //handle knockback outside
    private static List<int> GetMod2(SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);

        int xa = actor.StatTotalPA;
        //List<int> damage = new List<int>(); //just doing a single int

        //crits possible on all mod 2 except dash and throw, if not will have to write some new code
        //this Is not the case for dash and throw stone but they still do knockback, which Is handled in the calculationhitdamage
        //meh letting these crit
        List<int> temp = new List<int>();
        temp = CritCalculator(xa, target.TurnOrder, sn, actor);
        int crit_holder = temp[1]; //is it a crit, set to out_effect 2 later
        xa = temp[0]; //new xa value on crit

        //checks if the attack has an element and if actor has that element strengthend
        xa = IsAttackElementalStrengthened(xa, sn, actor);

        //if actor has attack up/magic attack up
        //assuming physical attacks gets attack up and magic get magic attack up
        xa = IsAttackSupportStrengthened(xa, sn.PMType, actor);

        //if caster has Martial Arts and this Is not a wpn-elemental attack
        //basically weapon elemantals are the Knight ability and some holy sword abilities
        //the remaining mod2's are a couple of squire things and more importantly the monk abilities
        //monks have innate martial art
        xa = IsAttackMartialArtsSupport(xa, sn.ElementType, actor);
        //is berserk
        xa = IsAttackBerserk(xa, actor);

        //defense up for target and corresponding phys/magic type
        xa = IsTargetDefenseStrengthened(xa, sn, target);
        xa = IsTargetProtectShell(xa, sn, target );

        //is target charging, sleeping
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHARGING);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_SLEEP);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHICKEN); //handled specially below

        //zodiac compat
        xa = CalculationZodiac.GetZodiacMain(xa, actor, target);

        //use damage_formula_type
        out_effect = CalculationHitDamage.GetHitDamage(xa, sn, actor, target);

        //run the damage through to see if any weaknesses
        out_effect[1] = GetDamageElemental(out_effect[1], SpellManager.Instance.GetSpellElementalByIndex(sn.Index), target);

        //loads in the crit, some CalculationHitDamage already check for this
        if (out_effect[2] == 0)
        {
            out_effect[2] = crit_holder;
        }
        if (bugTest == 1)
        {
            Debug.Log("In get Mod2Preview " + out_effect[0] + " " + out_effect[1] + " " + out_effect[2] + " " + out_effect[3] + " ");
        }

        return out_effect;
    }

    //USED FOR SPELLS
    public static int IsAttackElementalStrengthened(int xa, SpellName sn, PlayerUnit actor)
    {
        int elemental_type = SpellManager.Instance.GetSpellElementalByIndex(sn.Index);
        if ( elemental_type != NameAll.ITEM_ELEMENTAL_NONE)
        {
            //is an elemental type, now checks to see if unit has strengthen elemental
            if( StatusManager.Instance.IsUnitStrengthenByElement(actor,elemental_type))
            {
                xa = (xa * 5) / 4;
            }
            //depecrated way, checks by item
            //if (actor.IsElementStrengthenedByItem(elemental_type ))
            //{
            //    xa = (xa * 5) / 4;
            //}
        }
        return xa;
    }

    //used for weapons where the elemental_type Is found elsewhere
    //USED FOR WEAPONS WHERE THE WEAPON IS AN ELEMENTAL
    public static int IsAttackElementalStrengthened(int xa, int elemental_type, PlayerUnit actor)
    {
        if (elemental_type != NameAll.ITEM_ELEMENTAL_NONE)
        {
            //is an elemental type, now checks to see if unit has strengthen elemental
            if (StatusManager.Instance.IsUnitStrengthenByElement(actor, elemental_type))
            {
                xa = (xa * 5) / 4;
            }
            //depecrated way, checks by item rather than status lasting
            //if (actor.IsElementStrengthenedByItem(elemental_type ))
            //{
            //    xa = (xa * 5) / 4;
            //}
        }
        return xa;
    }

    //classic
    public static int IsAttackSupportStrengthened(int xa, int type, PlayerUnit actor)
    {
        //Debug.Log("xa prior " + xa);
        //0 for physical, 1 for magical, 2 for neutral, 3 for null
        if (type == NameAll.SPELL_TYPE_PHYSICAL && actor.IsAbilityEquipped(NameAll.SUPPORT_ATTACK_UP, NameAll.ABILITY_SLOT_SUPPORT))
        {
            xa = (xa * 4) / 3;
        }
        else if (type == NameAll.SPELL_TYPE_MAGICAL && actor.IsAbilityEquipped(NameAll.SUPPORT_MAGIC_ATTACK_UP, NameAll.ABILITY_SLOT_SUPPORT) )
        {
            xa = (xa * 4) / 3;
        }
        //Debug.Log("xa after " + xa);
        return xa;
    }

    //aurelian
    public static int IsAurelianAttackSupportStrengthened(int xa, int type, PlayerUnit actor)
    {
        //Debug.Log("xa prior " + xa);
        //0 for physical, 1 for magical, 2 for neutral, 3 for null
        if (type == NameAll.MOD_PHYSICAL && actor.IsAbilityEquipped(NameAll.SUPPORT_PHYSICAL_ATK_UP, NameAll.ABILITY_SLOT_SUPPORT))
        {
            xa = (xa * 4) / 3;
        }
        else if (type == NameAll.MOD_MAGICAL && actor.IsAbilityEquipped(NameAll.SUPPORT_MAGIC_ATK_UP, NameAll.ABILITY_SLOT_SUPPORT))
        {
            xa = (xa * 4) / 3;
        }
        else if (type == NameAll.MOD_NEUTRAL && actor.IsAbilityEquipped(NameAll.SUPPORT_NEUTRAL_ATK_UP, NameAll.ABILITY_SLOT_SUPPORT))
        {
            xa = (xa * 4) / 3;
        }
        //Debug.Log("xa after " + xa);
        return xa;
    }

    public static int IsAttackMartialArtsSupport(int xa, int elemental_type, PlayerUnit actor)
    {
        if ((AbilityManager.Instance.IsInnateAbility(actor.ClassId,NameAll.SUPPORT_MARTIAL_ARTS,NameAll.ABILITY_SLOT_SUPPORT) 
            || actor.IsAbilityEquipped(NameAll.SUPPORT_MARTIAL_ARTS, NameAll.ABILITY_SLOT_SUPPORT))
                && elemental_type != NameAll.ITEM_ELEMENTAL_WEAPON )
        {
            xa = (xa * 3) / 2;
        }
        return xa;
    }

    public static int IsAttackMartialArtsSupport(int xa, PlayerUnit actor)
    {
        if (AbilityManager.Instance.IsInnateAbility(actor.ClassId, NameAll.SUPPORT_MARTIAL_ARTS, NameAll.ABILITY_SLOT_SUPPORT)
            || actor.IsAbilityEquipped(NameAll.SUPPORT_MARTIAL_ARTS, NameAll.ABILITY_SLOT_SUPPORT))
        {
            xa = (xa * 3) / 2;
        }
        return xa;
    }

    public static int IsAttackMartialArtsAndBareHanded(int xa, PlayerUnit actor)
    {
        if ((AbilityManager.Instance.IsInnateAbility(actor.ClassId, NameAll.SUPPORT_MARTIAL_ARTS, NameAll.ABILITY_SLOT_SUPPORT)
            || actor.IsAbilityEquipped(NameAll.SUPPORT_MARTIAL_ARTS, NameAll.ABILITY_SLOT_SUPPORT))
                && actor.IsFistEquipped())
        {
            xa = (xa * 3) / 2;
        }
        return xa;
    }

    public static int IsAttackBerserk(int xa, PlayerUnit actor )
    {
        //if actor berserk
        if (StatusManager.Instance.IfStatusByUnitAndId(actor.TurnOrder, NameAll.STATUS_ID_BERSERK, true))
        {
            xa = (xa * 3) / 2;
        }
        return xa;
    }

    //classic
    static int IsTargetDefenseStrengthened(int xa, SpellName sn, PlayerUnit target)
    {
        //if target defense up
        //Debug.Log("xa prior " + xa);
        if (sn.PMType == NameAll.SPELL_TYPE_PHYSICAL && sn.IgnoresDefense == 0  && 
            target.IsAbilityEquipped(NameAll.SUPPORT_DEFENSE_UP, NameAll.ABILITY_SLOT_SUPPORT))
        {
            xa = (xa * 2) / 3;
        }
        else if (sn.PMType == NameAll.SPELL_TYPE_MAGICAL && sn.IgnoresDefense == 0 && 
            target.IsAbilityEquipped(NameAll.SUPPORT_MAGIC_DEFEND_UP, NameAll.ABILITY_SLOT_SUPPORT) )
        {
            xa = (xa * 2) / 3;
        }
        
        //Debug.Log("xa after " + xa);
        return xa;
    }

    //aurelian
    static int IsAurelianTargetDefenseStrengthened(int xa, SpellName sn, int spellType, PlayerUnit target)
    {
        //if target defense up
        //Debug.Log("xa prior " + xa);
        if (spellType == NameAll.MOD_PHYSICAL && sn.IgnoresDefense == 0 &&
            (target.IsAbilityEquipped(NameAll.SUPPORT_PHYSICAL_DEF_UP, NameAll.ABILITY_SLOT_SUPPORT)
            || (target.IsAbilityEquipped(NameAll.SUPPORT_FOR_THE_IMPERIUM, NameAll.ABILITY_SLOT_SUPPORT) && StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_CRITICAL))))
        {
            xa = (xa * 2) / 3;
        }
        else if (spellType == NameAll.MOD_MAGICAL && sn.IgnoresDefense == 0 &&
            ( target.IsAbilityEquipped(NameAll.SUPPORT_MAGIC_DEF_UP, NameAll.ABILITY_SLOT_SUPPORT)
            || (target.IsAbilityEquipped(NameAll.SUPPORT_FOR_THE_IMPERIUM, NameAll.ABILITY_SLOT_SUPPORT) && StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_CRITICAL))))
        {
            xa = (xa * 2) / 3;
        }
        else if (spellType == NameAll.MOD_NEUTRAL && sn.IgnoresDefense == 0 &&
            (target.IsAbilityEquipped(NameAll.SUPPORT_NEUTRAL_DEF_UP, NameAll.ABILITY_SLOT_SUPPORT)
            || (target.IsAbilityEquipped(NameAll.SUPPORT_FOR_THE_IMPERIUM, NameAll.ABILITY_SLOT_SUPPORT) && StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_CRITICAL))))
        {
            xa = (xa * 2) / 3;
        }
        //Debug.Log("xa after " + xa);
        return xa;
    }

    static int IsTargetProtectShell(int xa, SpellName sn, PlayerUnit target)
    {
        //Debug.Log("in IsTargetProtect Shell starting xa " + xa);
        //if target defense up
        if (sn.PMType == 0 && sn.IgnoresDefense == 0
                && (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_PROTECT, true)
                || StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_AUTO_PROTECT, false))
                 )
        {
            xa = (xa * 2) / 3; //Debug.Log("in IsTargetProtect Shell protect applied " + xa);
        }
        else if (sn.PMType == 1 && sn.IgnoresDefense == 0
              && (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_SHELL, true)
              || StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_AUTO_SHELL, false))
              )
        {
            xa = (xa * 2) / 3; //Debug.Log("in IsTargetProtect Shell shell applied " + xa);
        }
        //Debug.Log("in IsTargetProtect Shell ending xa " + xa);
        return xa;
    }

    public static int IsTargetStatus(int xa, PlayerUnit target, int statusId)
    {
        //if charging/sleeping; anythign that isn't labeleld "chicken_frog" is sent here (ie sleeping or charging) chicken or frog sent to the other one
        if (statusId != NameAll.STATUS_ID_CHICKEN)
        {
            if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, statusId, true))
            {
                if(!StatusManager.Instance.IsShowMust(target.TurnOrder, statusId) )
                {
                    xa = (xa * 3) / 2;
                } 
            }
        }
        else {
            //put in chicken but actually testing for chicken or frog
            if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_CHICKEN, true)
                    || StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_FROG, true))
            {
                //Debug.Log("unit is a chicken, doing MORE damage");
                xa = (xa * 3) / 2;
            }
            //Debug.Log("unit is not a chicken, doing LESS damage");
        }
        return xa;
    }

    //    [MOD 3] ~ physical attacks : success rate variable
    private static List<int> GetMod3(SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);

        int xa = actor.StatTotalPA;

        //if actor has attack up
        xa = IsAttackSupportStrengthened(xa, sn.PMType, actor);
        xa = IsAttackMartialArtsAndBareHanded(xa, actor); //barehanded and MA
        xa = IsTargetDefenseStrengthened(xa, sn, target); //if target defense up
        xa = IsTargetProtectShell(xa, sn, target );
        //is target charging, sleeping
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHARGING);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_SLEEP);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHICKEN); //handled specially below

        // % = xa + base + k where k Is the zodiac modified base and xa
        //zodiac returns to xa the xa and k, the calculationhitdamage does the rest
        if (sn.DamageFormulaType == 12)
        {//uses wp, turns into an attack if there is no item on the target
            xa = CalculationZodiac.GetZodiacMod3(xa, sn.BaseHit, actor, target, true);
        }
        else {
            xa = CalculationZodiac.GetZodiacMod3(xa, sn.BaseHit, actor, target, false);
        }

        //use damage_formula_type
        out_effect = CalculationHitDamage.GetHitDamage(xa, sn, actor, target);

        return out_effect;
    }

    //[MOD 4] ~ steal
    private static List<int> GetMod4(SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);
        int xa = actor.StatTotalSpeed;

        xa = IsAttackSupportStrengthened(xa, sn.PMType, actor);
        xa = IsAttackMartialArtsSupport(xa, actor); //barehanded and MA
        xa = IsTargetDefenseStrengthened(xa, sn, target); //if target defense up
        xa = IsTargetProtectShell(xa, sn, target );
        //is target charging, sleeping
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHARGING);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_SLEEP);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHICKEN); //handled specially below

        xa = CalculationZodiac.GetZodiacMod3(xa, sn.BaseHit, actor, target, false);

        //use damage_formula_type
        out_effect = CalculationHitDamage.GetHitDamage(xa, sn, actor, target);

        return out_effect;
    }

    //throw
    //before entering this into the wrapper, need to alter the sn.GetBaseQ with the thrown WP
    //also have to override the sn.GetElemental_type() with the thrown weapon elemental_type
    private static List<int> GetMod8(SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);
        int xa = actor.StatTotalSpeed;

        xa = IsTargetDefenseStrengthened(xa, sn, target); //if target defense up
        xa = IsTargetProtectShell(xa, sn, target );
        //is target charging, sleeping
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHARGING);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_SLEEP);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHICKEN); //handled specially below

        xa = CalculationZodiac.GetZodiacMain(xa, actor, target);
        //use damage_formula_type
        out_effect = CalculationHitDamage.GetHitDamage(xa, sn, actor, target);

        //run the damage through to see if any weaknesses
        out_effect[1] = GetDamageElemental(out_effect[1], sn.ElementType, target);

        if (bugTest == 1)
        {
            Debug.Log("In get Mod8Preview " + out_effect[0] + " " + out_effect[1] + " " + out_effect[2] + " " + out_effect[3] + " ");
        }
        return out_effect;
    }

    //jump
    private static List<int> GetMod9(SpellName sn, PlayerUnit actor, PlayerUnit target )
    {
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);
        int xa = actor.StatTotalPA;

        xa = IsTargetDefenseStrengthened(xa, sn, target); //if target defense up
        xa = IsTargetProtectShell(xa, sn, target );
        //is target charging, sleeping
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHARGING);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_SLEEP);
        xa = IsTargetStatus(xa, target, NameAll.STATUS_ID_CHICKEN); //handled specially below

        xa = IsWeaponType(xa, actor, NameAll.ITEM_ITEM_TYPE_CLASSIC_SPEAR);

        xa = CalculationZodiac.GetZodiacMain(xa, actor, target);
        //use damage_formula_type
        out_effect = CalculationHitDamage.GetHitDamage(xa, sn, actor, target);

        //jump is special, custom command set just uses the out_effect from CalculationHitDamage
        if( sn.CommandSet == NameAll.COMMAND_SET_JUMP)
        {
            int damage;
            if (actor.IsFistEquipped())
            {
                damage = xa * ((actor.StatTotalPA * actor.StatTotalBrave) / 100);
            }
            else {
                damage = xa * actor.GetWeaponPower();
            }
            out_effect[0] = sn.BaseHit;
            out_effect[1] = damage;
        }

        if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEAD))
        {
            out_effect[0] = 0;
            out_effect[1] = 0;
        }
        
        if (bugTest == 1)
        {
            Debug.Log("In get Mod9Preview " + out_effect[0] + " " + out_effect[1] + " " + out_effect[2] + " " + out_effect[3] + " ");
        }
        return out_effect;
    }

    public static int IsWeaponType(int xa, PlayerUnit actor, int weapon_type)
    {
        if (weapon_type == ItemManager.Instance.GetItemType(actor.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON) )
        {
            xa = (xa * 3) / 2;
        }
        return xa;
    }

    //MOD 5] ~ magical attacks : damage variable
    private static List<int> GetMod5(SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);
        int xa = actor.StatTotalMA;

        xa = IsAttackElementalStrengthened(xa, sn, actor  ); //Debug.Log("xa is " + xa);
        xa = IsAttackSupportStrengthened(xa, sn.PMType, actor); //Debug.Log("xa is " + xa);
        xa = IsTargetDefenseStrengthened(xa, sn, target);
        xa = IsTargetProtectShell(xa, sn, target ); //Debug.Log("xa is " + xa);
        xa = CalculationZodiac.GetZodiacMain(xa, actor, target); //Debug.Log("xa is " + xa);

        //check if actor has faith or innocent
        long actor_faith = StatusManager.Instance.GetModFaith(actor.TurnOrder, actor.StatTotalFaith);
        long target_faith = StatusManager.Instance.GetModFaith(target.TurnOrder, target.StatTotalFaith);

        int n = 1; //numerator used below
        int d = 1; //denominator used below
        //next Is weather status, ignoring (snowstorm, thunderstorm)

        //next Is elemental //weak, half, absorb //apply the elemental damages
        int elemental_type = sn.ElementType; 
        if (elemental_type != NameAll.ITEM_ELEMENTAL_NONE)
        {
            //weak
            if (StatusManager.Instance.IsUnitWeakByElement(target.TurnOrder,elemental_type))
            {
                n *= 2;
            }
            //half
            if (StatusManager.Instance.IsUnitHalfByElement(target,elemental_type))
            {
                d *= 2;
            }
            //absorb
            if (StatusManager.Instance.IsUnitAbsorbByElement(target.TurnOrder,elemental_type))
            {
                n *= -1;
            }
        }

        
        if( sn.CommandSet == NameAll.COMMAND_SET_ELEMENTAL || sn.CommandSet == NameAll.COMMAND_SET_DRAW_OUT || sn.CommandSet >= NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE)
        {
            out_effect = CalculationHitDamage.GetHitDamage(xa, sn, actor, target);
        }
        else
        {
            //damage = [(CFa * TFa * Q * MA5 * N) / (10000 * D)]
            long damage = (actor_faith * target_faith * xa * sn.BaseQ * n) / (10000 * d);
            int z1 = (int)damage;
            out_effect[1] = z1;
            out_effect[0] = sn.BaseHit;
        }
   
        return out_effect;

    }

    //MOD 11, using this for magic gun
    private static List<int> GetMod11(SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);

        //        BLACK MAGIC spell with the WP of the magical gun substituted for MA:
        //
        //        damage = [(CFa * TFa * Q * WP) / 10000]
        //
        //        60% of the time, the magical gun will deliver a level 1 spell (Q = 14), 30% of
        //        the time, it will deliver a level 2 spell (Q = 18), and 10% of the time, it wil
        //        deliver a level 3 spell (Q = 24). Magical guns cannot score conventional
        //        critical hits.

        int xa = actor.GetWeaponPower();
        int elemental_type = actor.GetWeaponElementType();
        int magic_gun_q; //does a role to determine the strength
        //Random r = new Random();
        int z1 = UnityEngine.Random.Range(1,100);
        if (z1 <= 60)
        {
            magic_gun_q = 14;
        }
        else if (z1 <= 90)
        {
            magic_gun_q = 18;
        }
        else {
            magic_gun_q = 24;
        }

        xa = IsAttackElementalStrengthened(xa, elemental_type, actor  );
        xa = IsAttackSupportStrengthened(xa, sn.PMType, actor);
        xa = IsTargetDefenseStrengthened(xa, sn, target);
        xa = IsTargetProtectShell(xa, sn, target );
        xa = CalculationZodiac.GetZodiacMain(xa, actor, target);

        //check if actor/target has faith or innocent
        long actor_faith = StatusManager.Instance.GetModFaith(actor.TurnOrder, actor.StatTotalFaith);
        long target_faith = StatusManager.Instance.GetModFaith(target.TurnOrder, target.StatTotalFaith);

        int n = 1; //numerator used below
        int d = 1; //denominator used below
        //next Is weather status, ignoring (snowstorm, thunderstorm)

        //next Is elemental
        //weak, half, absorb
        //apply the elemental damages
        if (elemental_type != NameAll.ITEM_ELEMENTAL_NONE)
        {
            //weak
            if (StatusManager.Instance.IsUnitWeakByElement(target.TurnOrder,elemental_type))
            {
                n *= 2;
            }
            //half
            if (StatusManager.Instance.IsUnitHalfByElement(target,elemental_type))
            {
                d *= 2;
            }
            //absorb
            if (StatusManager.Instance.IsUnitAbsorbByElement(target.TurnOrder,elemental_type))
            {
                n *= -1;
            }
        }

        //damage = [(CFa * TFa * Q * MA5 * N) / (10000 * D)]
        long damage = (actor_faith * target_faith * xa * magic_gun_q * n) / (10000 * d);
        z1 = (int)damage;
        out_effect[1] = z1;
        out_effect[0] = sn.BaseHit;

        return out_effect;

    }

    //[MOD 6] magical attacks : success rate variable
    private static List<int> GetMod6(SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);
        int xa = actor.StatTotalMA;

        xa = IsAttackElementalStrengthened(xa, sn, actor  ); //Debug.Log("xa is " + xa);
        xa = IsAttackSupportStrengthened(xa, sn.PMType, actor); //Debug.Log("xa is " + xa);
        xa = IsTargetDefenseStrengthened(xa, sn, target);
        xa = IsTargetProtectShell(xa, sn, target ); 
        //zodiac
        xa = CalculationZodiac.GetZodiacMod3(xa, sn.BaseHit, actor, target, false);

        //check if actor/target has faith or innocent
        long actor_faith = StatusManager.Instance.GetModFaith(actor.TurnOrder, actor.StatTotalFaith);
        long target_faith = StatusManager.Instance.GetModFaith(target.TurnOrder, target.StatTotalFaith);

        

        //use damage_formula_type, then override the hit % with the number found below
        //ignoring the elemental cancel/block of Lich and Dark
        out_effect = CalculationHitDamage.GetHitDamage(xa, sn, actor, target);

        //success% = [(CFa * TFa * (MA4 + Y + Z)) / 10000]
        long success = (actor_faith * target_faith * (xa + sn.BaseHit)) / 10000;
        int z1 = (int)success;
        out_effect[0] = z1;

        //undead and revive check
        if (sn.StatusType == NameAll.STATUS_ID_LIFE)
        {
            if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEAD))
            {
                if (StatusManager.Instance.IsUndead(target.TurnOrder))
                {
                    out_effect[0] = 0;
                    out_effect[1] = 0;
                }
            }
            else
            {
                if (!StatusManager.Instance.IsUndead(target.TurnOrder))
                {
                    out_effect[0] = 0;
                    out_effect[1] = 0;
                }
            }
        }

        return out_effect;
    }

    //mod 16 (6a in bmg, using this for golem
    private static List<int> GetMod16(SpellName sn, PlayerUnit actor )
    {
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);
        int xa = actor.StatTotalMA;
        xa = IsAttackSupportStrengthened(xa, sn.PMType, actor);

        //check if actor/target has faith or innocent
        long actor_faith = StatusManager.Instance.GetModFaith(actor.TurnOrder, actor.StatTotalFaith);

        //success% = [(CFa * (MA1 + Q)) / 100]
        long success = (actor_faith * (xa + sn.BaseHit)) / 100;
        int z1 = (int)success;
        out_effect[0] = z1;
        z1 = actor.StatTotalMaxLife; //can make this more generic if needed
        out_effect[1] = z1;

        return out_effect;
    }

    //mod7, only depends on zodiac compat
    private static List<int> GetMod7(SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);
        int z1 = CalculationZodiac.GetZodiacMain(sn.BaseHit, actor, target); //0 since no weapon power calculation for thi
        out_effect[0] = z1;
        out_effect[1] = sn.BaseQ;
        return out_effect;

    }

    //crit function
    //first value Is new xa value, 2nd value Is whether a crit happened or not
    public static List<int> CritCalculator(int xa, int targetId, SpellName sn, PlayerUnit actor)
    {
        List<int> ia = new List<int>();
        ia.Add(xa);
        ia.Add(0);//whether it's a crit or not, needed to tell if knockback
        if( PlayerManager.Instance.IsAbilityEquipped(targetId,NameAll.SUPPORT_STAND_GROUND, NameAll.ABILITY_SLOT_SUPPORT))
        {
            return ia;
        }

        int critTest = 5;
        if (PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.SUPPORT_CRIT_HIT_UP, NameAll.ABILITY_SLOT_SUPPORT))
        {
            critTest = 20;
        }
		int rollResult = PlayerManager.Instance.GetRollResult(critTest, 1, 100, NameAll.NULL_INT, NameAll.COMBAT_LOB_SUBTYPE_CRIT_ROLL, sn, actor);
        if (rollResult <= critTest)
        { //5% chance of crit
            ia[1] = 1;
            //modified_XA = normal_XA + (0..(normal_XA-1))
            if (xa > 0)
            {
				xa += UnityEngine.Random.Range(0, xa - 1);
            }
            ia[0] = xa;
        }
        return ia;
    }

    public static Tile KnockbackCheck(Board board, SpellName sn, PlayerUnit actor, PlayerUnit target, int rollChance, int rollSpread, bool autoKnockback = false)
    {
        if (autoKnockback || KnockbackSuccess(rollChance, rollSpread, sn, actor, target) )
        {
            //Debug.Log("in knockback check 1 ");
            //finds the locations where knockback can happen and if it can happen
            return GetKnockbackMapTileIndex(board, actor, target);
        }

        return null;
    }

    public static bool KnockbackSuccess(int rollChance, int rollSpread, SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
		return PlayerManager.Instance.IsRollSuccess(rollChance, 1, rollSpread, NameAll.NULL_INT, NameAll.COMBAT_LOG_SUBTYPE_KNOCKBACK_SUCCESS, sn, actor, target);
    }


    //returns null Tile if no tile found, if Tile is found function handles the knockback
    public static Tile GetKnockbackMapTileIndex(Board board, PlayerUnit actor, PlayerUnit target)
    {
        int xDiff = actor.TileX - target.TileX;
        int yDiff = actor.TileY - target.TileY;
        int z_check = 2; //can't get knocked up more than 1.5 I guess

        int newX = target.TileX;
        int newY = target.TileY;
        //Debug.Log("knockback1 " + newX + "," + newY);
        if (Math.Abs(xDiff) > Math.Abs(yDiff))
        {
            //move along the x-axis
            if (xDiff > 0)
            {
                newX += -1;
            }
            else {
                newX += 1;
            }
        }
        else {
            //move along the y-axis
            if (yDiff > 0)
            {
                newY += -1;
            }
            else {
                newY += 1;
            }
        }
        //Debug.Log("knockback2 " + newX + "," + newY);
        //Debug.Log(" checking for tile to knockback to 1");
        Tile t = board.GetTile(newX, newY);
        if( t == null)
        {
            return null;
        }
        //Debug.Log(" checking for tile to knockback to 2 " + board.GetTile(target).height + ", " + t.height);
        if ( t.height - board.GetTile(target).height >= z_check)
        {
            return null;
        }
        //Debug.Log(" checking for tile to knockback to 3 " + t.UnitId);
        if( t.UnitId == NameAll.NULL_UNIT_ID)
        {
            return t;
        }
        //Debug.Log(" checking for tile to knockback to 4");
        return null;  
    }




}

