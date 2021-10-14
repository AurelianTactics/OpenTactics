using UnityEngine;
//using UnityEngine.Random;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mess of a class. Should be overhauled.
/// Does some math based on the SpellName, actor, target, and key variable and returns 4 things for calculation mod.
/// Based on the output turns into what the action result actually is
/// returns 4 things which can vary. generally: hit chance, effect value, crit, misc (last thing used for MP effect twice and throw once)
/// </summary>
/*
  //returns hitDamage numbers based on the damage formula type
   * unknown:
 *  //assuming float doesn't prevent fall damage but idk
 */
public static class CalculationHitDamage
{
    static int bugTest = 0;
    public static List<int> GetHitDamage(int xa, SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        List<int> out_effect = new List<int>();
        out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);
        if( bugTest == 1)
        {
            Debug.Log("testing gethitdmg top. dft is " + sn.DamageFormulaType);
        }
        int z1;
        int dft = sn.DamageFormulaType;
        if (dft == 0)
        { //accumulate
            out_effect[0] = sn.BaseHit;
            out_effect[1] = sn.BaseQ;
            //out_effect.set(0, sn.BaseHit);
            //out_effect.set(1, sn.BaseQ);
        }
        else if (dft == 1)
        { //songs: Restore (MA + 20) MP
            out_effect[0] = sn.BaseHit;
            z1 = sn.BaseQ + actor.StatTotalMA;
            out_effect[1] = z1;
        }
        else if (dft == 2)
        { //dance //Damage to MP = PA + [(PA * Br) / 100]
            out_effect[0] = sn.BaseHit;
            z1 = actor.StatTotalPA + ((actor.StatTotalPA * actor.StatTotalBrave) / 100);
            out_effect[1] = z1;
        }
        else if (dft == 3)
        {
            //charm heart, have to be different sexes
            if (actor.Sex.Equals(target.Sex))
            {
                z1 = 0;
            }
            else {
                z1 = sn.BaseHit + CalculationZodiac.GetZodiacMod3(actor.StatTotalMA, sn.BaseHit, actor, target, false);
            }
            out_effect[0] = z1;
            out_effect[1] = sn.BaseQ;
        }
        else if (dft == 4)
        { //talk skill
            z1 = sn.BaseHit + CalculationZodiac.GetZodiacMod3(actor.StatTotalMA, sn.BaseHit, actor, target, false);
            if (PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.REACTION_FINGER_GUARD, NameAll.ABILITY_SLOT_REACTION) )
            {
                //base % - brave %
                z1 = z1 - actor.StatTotalBrave;
                if (z1 < 0)
                {
                    z1 = 0;
                }   
            }
            out_effect[0] = z1;
            out_effect[1] = sn.BaseQ;
        }
        else if (dft == 5)
        { //basic skill throw stone, dash PA with modified random number
            out_effect[0] = sn.BaseHit;
            //base q is the min and max numbers to get a random number from
            //stored in the form to get both the max and min 2001 (1..2)
            //divide by 1000 to get the max, then subtract max * 1000 to get the min
            z1 = sn.BaseQ / 1000;
            if (z1 <= 0)
            { //this should never happen
                z1 = 1;
            }
            int z2 = sn.BaseQ - (1000 * z1);
            z1 = UnityEngine.Random.Range(z2, z1+1); //z1 = r.nextInt(z1) + z2;
            z1 = z1 * xa;
            out_effect[1] = z1;
            //checks for knock back
            if ( sn.Index != 58) //repeating fist doesn't knock back !sn.getSpell_id().equals("repeating_fist")
            {
                //Debug.Log("KNOCKBACK IS SET FOR TRUE, disable" + z1);
                //out_effect[2] = 2;
                z1 = UnityEngine.Random.Range(0, 2); //inclusive/exclusive
                if (z1 == 1)
                {
                    out_effect[2] = 2; //knocks the unit back for sure, if this was 1 then it would have to pass the 50/50 crit check
                }
            }
            //Debug.Log("Calculation Hit Damageout_effect is " + out_effect[0] + " " + out_effect[1] + " " + out_effect[2] + " " + out_effect[3]);
        }
        else if (dft == 6)
        { //xa + base for hit, q for dmg, punch art secret fist, stigma magic, speed break etc
            z1 = xa + sn.BaseHit;
            out_effect[0] = z1;
            out_effect[1] = sn.BaseQ;
        }
        else if (dft == 7)
        { //xa + base,  revive % of health, mp break % of mp
            z1 = xa + sn.BaseHit;
            out_effect[0] = z1;

            int z2 = 100 / sn.BaseQ;
            if (100 % sn.BaseQ == 0 && target.StatTotalMaxMP % z2 == 0)
            {
                z2 = 0; //will not round up since it equals a whole number
            }
            else
            {
                z2 = 1; //round up, add 1 point of life, this is only for FFT (all things divide equally into 100 and have a baseQ <=50
                //will have an effect in not FFT of adding 1 point to all HP and MP damage things
            }
            
            if ( sn.StatType == NameAll.STAT_TYPE_MP)
            {
                z1 = target.StatTotalMaxMP * sn.BaseQ / 100 + z2;
            }
            else
            {
                z1 = target.StatTotalMaxLife * sn.BaseQ / 100 + z2;
            }
            
            out_effect[1] = z1;
            if( bugTest == 1)
            {
                Debug.Log("in dft 7, outeffect 1,2 are " + out_effect[0] + ", " + out_effect[1]);
            }
            
        }
        else if (dft == 8)
        { //spin fist, earth slash
            out_effect[0] = sn.BaseHit;
            z1 = xa * (actor.StatTotalPA / 2);
            out_effect[1] = z1;
        }
        else if (dft == 9)
        {
            out_effect[0] = sn.BaseHit;
            //base q is the min and max numbers to get a random number from
            //stored in the form to get both the max and min 2001 (1..2)
            //divide by 1000 to get the max, then subtract max * 1000 to get the min
            z1 = sn.BaseQ / 1000;
            if (z1 <= 0)
            { //this should never happen
                z1 = 1;
            }
            int z2 = sn.BaseQ - (1000 * z1);
            z1 = UnityEngine.Random.Range(z2, z1+1);
            z1 = z1 * xa;
            out_effect[1] = z1;
        }
        else if (dft == 10)
        { //wave fist
            out_effect[0] = sn.BaseHit;
            z1 = xa * (actor.StatTotalPA / 2 + 1);
            out_effect[1] = z1;
        }
        else if (dft == 11)
        { //chakra magic
            out_effect[0] = sn.BaseHit;
            z1 = xa * 5;
            out_effect[1] = z1; //hp
            z1 = (xa * 5) / 2;
            out_effect[3] = z1; //mp
        }
        else if (dft == 12)
        { //xa + base + wp for hit, q for dmg, knight equipment breaks
            //nullified by maintenance
            if ( target.IsAbilityEquipped( NameAll.SUPPORT_MAINTENANCE ,NameAll.ABILITY_SLOT_SUPPORT) ) //target.getAbility_support_code().equals("as_mai_1")
            {
                out_effect[0] = 0;
            }
            else {
                bool battleSkill = false;
                if( sn.CommandSet == NameAll.COMMAND_SET_BATTLE_SKILL)
                {
                    battleSkill = true; //battle skill doesn't use WP on barehanded attacks
                }
                z1 = xa + sn.BaseHit + PlayerManager.Instance.GetWeaponPower(actor.TurnOrder,battleSkill); //the base WP addition by zodiac is modified in ZodiacMod3 but still need the wp here
                out_effect[0] = z1;
            }
            out_effect[1] = sn.BaseQ;
        }
        else if (dft == 13)
        { //reserved, handling it in mod 5

        }
        else if (dft == 14)
        { //reserved, handling hit in mod 6

        }
        else if (dft == 15)
        { //handling hit in mod 6, % stat reduction
            out_effect[0] = sn.BaseHit;//for non mod6 like move HP up, sets the hit. for mod 6 this is overriden elsewhere (I think)
            
            //seeing if this needs to round up or not but keeping the flexibility of having % that don't divide directly into 100 (like a 33% hp reduction)
            int z2 = 100 / sn.BaseQ;
            if( 100 % sn.BaseQ == 0 && target.StatTotalMaxLife % z2 == 0)
            {
                z2 = 0; //will not round up since it equals a whole number
            }
            else
            {
                z2 = 1; //round up, add 1 point of life, this is only for FFT (all things divide equally into 100 and have a baseQ <=50
                //will have an effect in not FFT of adding 1 point to all HP and MP damage things
            }
            z1 = target.StatTotalMaxLife * sn.BaseQ / 100 + z2;
            out_effect[1] = z1;
            if (bugTest == 1)
            {
                Debug.Log("in dft 15 ; z1 is " + out_effect[1] + " baseQ is " + sn.BaseQ);
            }
        }
        else if (dft == 16)
        { //reserved, mod 16 golem type

        }
        else if (dft == 17)
        { //reserved, mod 6 damages brave, hit rate found prior
            //using 0 for now but could be activated
            //out_effect.set(1,sn.BaseQ);
        }
        else if (dft == 18)
        { //reserved, mod 6 damages % of mp, hit rate found prior;
            out_effect[0] = sn.BaseHit;//for non mod6 like move MP up, sets the hit. for mod 6 this is overriden elsewhere (I think)
            //Debug.Log("in dft 18 ; z1 is " + out_effect[1] + " baseQ is " + sn.BaseQ);
            //seeing if this needs to round up or not but keeping the flexibility of having % that don't divide directly into 100 (like a 33% hp reduction)
            int z2 = 100 / sn.BaseQ;
            if (100 % sn.BaseQ == 0 && actor.StatTotalMaxMP % z2 == 0)
            {
                z2 = 0; //will not round up since it equals a whole number
            }
            else
            {
                z2 = 1; //round up, add 1 point of life, this is only for FFT (all things divide equally into 100 and have a baseQ <=50
                //will have an effect in not FFT of adding 1 point to all HP and MP damage things
            }
            z1 = actor.StatTotalMaxMP * sn.BaseQ / 100 + z2;
            out_effect[1] = z1;
            if (bugTest == 1)
            {
                Debug.Log("in dft 18 ; z1 is " + out_effect[1] + " baseQ is " + sn.BaseQ);
            }
        }
        else if (dft == 20)
        { //thief steal, like 6 except maintencne
            //nullified by maintenance
            if (target.IsAbilityEquipped(NameAll.SUPPORT_MAINTENANCE, NameAll.ABILITY_SLOT_SUPPORT))
            {
                out_effect[0] = 0;
            }
            else {
                if( target.IsSlotEmptyForItemAttack(sn.StatType) )
                {
                    z1 = 0;
                }
                else
                {
                    z1 = xa + sn.BaseHit;
                }
                
                out_effect[0] = z1;
            }
            out_effect[1] = sn.BaseQ;
        }
        else if (dft == 21)
        { //throw
            //hit rate is base_hit, damage is item wp * sp
            //4th variable is the range
            out_effect[0] = sn.BaseHit;
            out_effect[1] = xa * sn.BaseQ;
            out_effect[3] = actor.StatTotalMove;
        }
        else if (dft == 22)
        {
            //reserved for jump
            
        }
        else if (dft == 23)
        {
            //item, returns the defaults
            out_effect[0] = sn.BaseHit;
            out_effect[1] = sn.BaseQ;
        }
        else if (dft == 24)
        { //reserved for charge

        }
        else if (dft == 25) //RESERVED FOR ATTACK
        {
            //NO LONGER USED DOES THE CALCULATION IN MOD 10 
            //int wp = PlayerManager.Instance.GetWeaponPower(actor.TurnOrder);
            //string weapon_type = ItemManager.Instance.GetItemType(actor.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON);//actor.GetWeaponType();
            //int chargeK = sn.BaseQ;
            //the weapon specific parts are done in CalculationMod10, at this point it is simply modified xa * WP
            //Debug.Log("base hit is " + sn.BaseHit);
            out_effect[0] = sn.BaseHit;
            out_effect[1] = xa * sn.BaseQ; //modified in mod 10 with WP for all except for fists (pa), charge is stored in baseQ but utilized earlier in Mod10
        }
        else if (dft == 26)
        { //reserved for magic gun

        }
        else if(dft == 27)//reaction
        {
            out_effect[0] = sn.BaseHit; //eh I think this is fine, not sure if it will fuck other things up
            out_effect[1] = sn.BaseQ;
        }
        else if (dft == 28)//movement
        {
            
        }
        else if (dft == 29)//weak/frog attack
        {
            //based on pa
            out_effect[0] = sn.BaseHit;
            z1 = ((actor.StatTotalPA * actor.StatTotalBrave) / 100);
            out_effect[1] = z1;
        }
        else if( dft == 30) //elemental
        {
            //[(PA + 2) / 2] * MA
            out_effect[0] = sn.BaseHit;
            out_effect[1] = ((actor.StatTotalPA + 2) / 2) * actor.StatTotalMA; Debug.Log("in dft 30");
        }
        else if (dft == 31) //draw out
        {
            //baseq * MA
            out_effect[0] = sn.BaseHit;
            out_effect[1] = sn.BaseQ * actor.StatTotalMA;
        }
        else if (dft == 33) //phoenix down
        {
            //baseq * MA
            out_effect[0] = sn.BaseHit;
            out_effect[1] = UnityEngine.Random.Range(1,21);
        }
        else if (dft == 34) //elixir
        {
            //baseq * MA
            out_effect[0] = sn.BaseHit;
            out_effect[1] = target.StatTotalMaxLife;
            out_effect[3] = target.StatTotalMaxMP;
        }

        //Debug.Log("check if float " + StatusManager.Instance.CheckIfFloat(target.TurnOrder));
        //earth elemental and float check
        if ( sn.ElementType == NameAll.ITEM_ELEMENTAL_EARTH && StatusManager.Instance.CheckIfFloat(target.TurnOrder) )
        {            
            out_effect[1] = 0;
        }
        //undead and revive check
        if( sn.StatusType == NameAll.STATUS_ID_LIFE )
        {
            if ( StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEAD) )
            {
                if(StatusManager.Instance.IsUndead(target.TurnOrder))
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
        else if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEAD))
        {
            out_effect[0] = 0;
            out_effect[1] = 0;
        }

        return out_effect;
    }

    //4th variable of knockback array has fall damage
    public static int GetFallDamage(PlayerUnit unit, int height)
    {
        //fall_damage = RU{((fall_distance - Jump) * MaxHP) / 10}
        if (unit.StatTotalJump >= height)
        {
            return 0;
        }
        if (unit.IsAbilityEquipped(NameAll.MOVEMENT_FLY, NameAll.ABILITY_SLOT_MOVEMENT)) //fly
        {
            return 0;
        }
        double zDouble = (double)((height - unit.StatTotalJump) * unit.StatTotalMaxLife);
        int z1 = (int)Math.Ceiling(zDouble / 10); 
        return z1;
    }

    //called form calculationMod, takes in the outEffect, modifies it based on sn characteristics and returns it
    public static List<int> GetHitAurelian(List<int> outEffect, SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        //List<int> out_effect = new List<int>();
        //out_effect.Add(0); out_effect.Add(0); out_effect.Add(0); out_effect.Add(0);
        int mod = sn.Mod % 10;
        int dft = sn.DamageFormulaType;

        int modifiedBaseHit = GetBaseHitModification(sn, actor, target); //some types have the base hit modified by the zodiac
        int modifiedBaseQ = GetBaseQModification(sn, target); //for types that remove a % of a stat

        int z1 = 0;
        int xa = outEffect[0]; //stored here for physical, magical, neutral, and null

        if (dft == 0)
        {
            outEffect[0] = modifiedBaseHit;
            outEffect[1] = modifiedBaseQ;
        }
        else if (dft == 1) //faith based, damage variable
        {
            outEffect[0] = modifiedBaseHit;

            long actor_faith = StatusManager.Instance.GetModFaith(actor.TurnOrder, actor.StatTotalFaith);
            long target_faith = StatusManager.Instance.GetModFaith(target.TurnOrder, target.StatTotalFaith);
            long damage = (actor_faith * target_faith * xa * modifiedBaseQ) / (10000); //elemental done later
            z1 = (int)damage;
            outEffect[1] = z1;
        }
        else if (dft == 2) //faith based, hit rate variable
        {
            long actor_faith = StatusManager.Instance.GetModFaith(actor.TurnOrder, actor.StatTotalFaith);
            long target_faith = StatusManager.Instance.GetModFaith(target.TurnOrder, target.StatTotalFaith);

            long success = (actor_faith * target_faith * (xa + modifiedBaseHit)) / (10000);
            z1 = (int)success;
            outEffect[0] = z1;

            outEffect[1] = modifiedBaseQ;
        }
        else if (dft == 3) //xa only changes the chance it hits
        {
            outEffect[0] = modifiedBaseHit + xa;
            outEffect[1] = modifiedBaseQ;
        }
        else if (dft == 4) //xa changes chance it hits and amount
        {
            outEffect[0] = modifiedBaseHit + xa;
            outEffect[1] = modifiedBaseQ * xa;
        }
        else if (dft == 5) //faith based, hit variable and dmg xa based
        {
            long actor_faith = StatusManager.Instance.GetModFaith(actor.TurnOrder, actor.StatTotalFaith);
            long target_faith = StatusManager.Instance.GetModFaith(target.TurnOrder, target.StatTotalFaith);
            long success = (actor_faith * target_faith * (xa + modifiedBaseHit)) / (10000); //not sure about the n and d here
            z1 = (int)success;
            outEffect[0] = z1;

            outEffect[1] = (modifiedBaseQ + xa);
        }
        else if (dft == 6) //applies the zodiac, typically null
        {
            outEffect[0] = modifiedBaseHit + CalculationZodiac.GetZodiacMain(xa, actor, target);
            outEffect[1] = modifiedBaseQ + CalculationZodiac.GetZodiacMain(xa, actor, target);
        }
        else if (dft == 7) //applies the zodiac, typically null
        {
            //TO MODIFY THE BASE HIT BY ZODIAC SET THE BASE HIT OVER 1000
            outEffect[0] = modifiedBaseHit + CalculationZodiac.GetZodiacMain(xa, actor, target);
            outEffect[1] = modifiedBaseQ;
        }
        else if (dft == 8)
        {
            //like jump, maybe do something weapon dependent at some point
            outEffect[0] = modifiedBaseHit;
            outEffect[1] = xa * (actor.StatTotalPA + modifiedBaseQ);
        }
        else if (dft == 9)
        {
            outEffect[0] = modifiedBaseHit + xa;
            outEffect[1] = modifiedBaseQ + xa;
        }
        else if (dft == 10) //knockback guaranteed
        {
            outEffect[0] = modifiedBaseHit + xa;
            outEffect[1] = modifiedBaseQ * xa;
            outEffect[2] = 2;
        }
        else if (dft == 11)
        {
            // damage = PA! * [PA / 2] 
            outEffect[0] = modifiedBaseHit;
            if (sn.Mod % 10 == NameAll.MOD_NEUTRAL)
            {
                outEffect[1] = xa * (actor.StatTotalAgi / 2 + modifiedBaseQ);
            }
            else if (sn.Mod % 10 == NameAll.MOD_MAGICAL)
            {
                outEffect[1] = xa * (actor.StatTotalMA / 2 + modifiedBaseQ);
            }
            else
            {
                outEffect[1] = xa * (actor.StatTotalPA / 2 + modifiedBaseQ);
            }

        }
        //else if (dft == 12) //empty, can be resused
        //{
        //    //damage = PA! * ([PA / 2] + 1)  
        //    outEffect[0] = modifiedBaseHit;
        //    if( sn.Mod % 10 == NameAll.MOD_NEUTRAL)
        //    {
        //        outEffect[1] = xa * (actor.StatTotalAgi / 2 + 1);
        //    }
        //    else
        //    {
        //        outEffect[1] = xa * (actor.StatTotalPA / 2 + 1);
        //    }

        //}
        //else if (dft == 13) //became 8, can be reused
        //{
        //    outEffect[0] = modifiedBaseHit;
        //    outEffect[1] = xa * (actor.StatTotalPA);
        //}
        //else if (dft == 14) //became 8, can be reused
        //{
        //    //damage = PA! * (PA + 1)    
        //    outEffect[0] = modifiedBaseHit;
        //    outEffect[1] = xa * (actor.StatTotalPA + 5);
        //}
        else if (dft == 15)
        {
            outEffect[0] = modifiedBaseHit;
            if (sn.Mod % 10 == NameAll.MOD_NEUTRAL)
            {
                outEffect[1] = xa * (actor.StatTotalAgi / 4 + modifiedBaseQ);
            }
            else if (sn.Mod % 10 == NameAll.MOD_MAGICAL)
            {
                outEffect[1] = xa * (actor.StatTotalMA / 4 + modifiedBaseQ);
            }
            else
            {
                outEffect[1] = xa * (actor.StatTotalPA / 4 + modifiedBaseQ);
            }
        }
        else if (dft == 16)
        {
            outEffect[0] = modifiedBaseHit;
            outEffect[1] = xa * ((actor.StatTotalPA + actor.StatTotalAgi + modifiedBaseQ) / 4);
        }
        else if (dft == 17) //drawout
        {
            outEffect[0] = modifiedBaseHit;
            outEffect[1] = modifiedBaseQ * xa;
        }
        else if (dft == 18)
        {
            outEffect[0] = modifiedBaseHit + xa;

            ////base q is the min and max numbers to get a random number from
            ////stored in the form to get both the max and min 2001 (1..2)
            ////divide by 1000 to get the max, then subtract max * 1000 to get the min
            //OLD: damage = (1..9) * (PA! + [PA / 2])  
            //NEW: damage = 5...15 * (PA + (pa+agi)/4)
            z1 = sn.BaseQ / 1000;
            if (z1 <= 0)
            { //this should never happen
                z1 = 1;
            }

            int z2 = sn.BaseQ - (1000 * z1);
            if (z2 <= 0)
            {
                z2 = 1;
            }

            if (z2 <= z1)
            {
                z1 = UnityEngine.Random.Range(z2, z1 + 1); //z1 = r.nextInt(z1) + z2;
            }
            else
            {
                z1 = UnityEngine.Random.Range(z1, z2 + 1); //z1 = r.nextInt(z1) + z2;
            }

            z1 = z1 * (xa + (actor.StatTotalPA + actor.StatTotalAgi + 1) / 4);
            outEffect[1] = z1;
        }
        else if (dft == 20) //chakra
        {
            outEffect[0] = modifiedBaseHit;
            //recovery = (5 * PA) HP; [(5 * PA) / 2] MP  
            outEffect[1] = 5 * xa;
            outEffect[3] = (3 * xa) / 2;
        }
        else if (dft == 21)
        {
            outEffect[0] = modifiedBaseHit + CalculationZodiac.GetZodiacMain(xa, actor, target);
            outEffect[1] =  (CalculationZodiac.GetZodiacMain(xa, actor, target) + modifiedBaseQ) * xa;
        }
        else if (dft == 22)
        {
            int wp = outEffect[1]; //stored here
            outEffect[0] = modifiedBaseHit;
            outEffect[1] = wp * xa;
        }
        else if (dft == 23) //direction based
        {
            outEffect[0] = modifiedBaseHit;
            outEffect[1] = DirectionsExtensions.GetDirectionPU(actor, target, sn.BaseQ); //Debug.Log("in calc hit dmg, doing a direction calculation " + outEffect[1]);
        }


        //Debug.Log("doing dead checks " + sn.StatusType );
        if (sn.StatusType == NameAll.STATUS_ID_LIFE )
        {
            if(StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEAD) ) //target is dead
            {
                //life doesn't work on an undead target, only zombie_life
                if ( sn.ElementType == NameAll.ITEM_ELEMENTAL_UNDEAD && StatusManager.Instance.IsUndead(target.TurnOrder) ) 
                {
                    outEffect[0] = 0;
                    outEffect[1] = 0;
                }
            }
            else //target is alive, shit doesn't work unless undead
            {
                if (!StatusManager.Instance.IsUndead(target.TurnOrder))
                {
                    outEffect[0] = 0;
                    outEffect[1] = 0;
                }
            }
        }
        else if(sn.StatusType == NameAll.STATUS_ID_ZOMBIE_LIFE)
        {
            //Debug.Log("doing dead checks " + sn.StatusType);
            //zombie life only works on dead targets that are zombies
            if (!StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEAD)) 
            {
                //Debug.Log("doing dead checks " + sn.StatusType);
                outEffect[0] = 0;
                outEffect[1] = 0;
            }
        }
        else if(sn.StatusType == NameAll.STATUS_ID_CRYSTAL)
        {
            //Debug.Log("doing dead checks " + sn.StatusType);
            if (!StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEAD)) 
            {
                //Debug.Log("doing dead checks " + sn.StatusType);
                outEffect[0] = 0;
                outEffect[1] = 0;
            }
        }
        else if (StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder, NameAll.STATUS_ID_DEAD))
        {
            outEffect[0] = 0;
            outEffect[1] = 0;
        }
        //Debug.Log("outEffect for CalcHitDamage " + outEffect[0] + " " + outEffect[1]);
        return outEffect;
    }

    //called in aurelian, for abilities that take a % of a stat converts the baseQ and statType into the proper value
    static int GetBaseQModification(SpellName sn, PlayerUnit target)
    {
        int z1 = sn.BaseQ; //Debug.Log(" z1 is " + z1);
        if( sn.HitsStat == NameAll.HITS_STAT_PERCENTAGE)
        {
            if (sn.StatType == NameAll.STAT_TYPE_MAX_MP)
            {
                Debug.Log(" z1, stat total are " + z1 + " " + target.StatTotalMaxMP);
                z1 = target.StatTotalMaxMP * z1 / 100;
            }
            else if (sn.StatType == NameAll.STAT_TYPE_MAX_HP)
            {
                z1 = target.StatTotalMaxLife * z1 / 100;
            }
            else if ( sn.StatType == NameAll.STAT_TYPE_HP) //still based on maxHP
            {
                z1 = target.StatTotalLife * z1 / 100;
            }
            else if (sn.StatType == NameAll.STAT_TYPE_MP) //still based on maxMP
            {
                z1 = target.StatTotalMP * z1 / 100; //Debug.Log(" z1 is " + z1);
            }
            else if (sn.StatType == NameAll.STAT_TYPE_SPEED) 
            {
                z1 =  target.StatTotalSpeed * z1 / 100; 
            }
            else if (sn.StatType == NameAll.STAT_TYPE_PA)
            {
                z1 = target.StatTotalPA*z1 / 100;
            }
            else if (sn.StatType == NameAll.STAT_TYPE_AGI)
            {
                z1 = target.StatTotalAgi*z1 / 100;
            }
            else if (sn.StatType == NameAll.STAT_TYPE_MA)
            {
                z1 = target.StatTotalMA*z1 / 100;
            }
            else if (sn.StatType == NameAll.STAT_TYPE_BRAVE)
            {
                z1 = target.StatTotalBrave*z1 / 100;
            }
            else if (sn.StatType == NameAll.STAT_TYPE_FAITH)
            {
                z1 = target.StatTotalFaith*z1 / 100;
            }
            else if (sn.StatType == NameAll.STAT_TYPE_CUNNING)
            {
                z1 = target.StatTotalCunning*z1 / 100;
            }
            else if (sn.StatType == NameAll.STAT_TYPE_CT)
            {
                z1 = target.CT*z1 / 100;
            }
            else if (sn.StatType == NameAll.STAT_TYPE_C_EVADE)
            {
                z1 = target.StatTotalCEvade*z1 / 100;
            }
            else if (sn.StatType == NameAll.STAT_TYPE_MOVE)
            {
                z1 = target.StatTotalMove*z1 / 100;
            }
            else if (sn.StatType == NameAll.STAT_TYPE_JUMP)
            {
                z1 = target.StatTotalJump*z1 / 100;
            }
            else
            {
                z1 = 1; //equipment breaks, some evades, direction, etc
            }

        }
        if (z1 < 1)
            z1 = 1;
        //Debug.Log("leaving modified baseQ : " + z1);
        return z1;
    }

    //modifies the base hit by zodiac in some cases, called in aurelian mod
    static int GetBaseHitModification(SpellName sn, PlayerUnit actor, PlayerUnit target)
    {
        int z1 = sn.BaseHit;
        if (z1 > 1000)
        {
            z1 = z1 - 1000;
            z1 = CalculationZodiac.GetZodiacMain(z1, actor, target);
        }

        if (PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.SUPPORT_SADIST, NameAll.ABILITY_SLOT_SUPPORT))
        {
            if (StatusManager.Instance.IsSadist(target.TurnOrder))
            {
                z1 += 5; Debug.Log("sadist added " + z1);
            }
        }
        else if (PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.SUPPORT_ENVY, NameAll.ABILITY_SLOT_SUPPORT))
        {
            if (StatusManager.Instance.IsPositiveStatus(target.TurnOrder))
            {
                z1 += 5; Debug.Log("envy added " + z1);
            }
        }

        return z1;
    }

    //public static List<int> GetAurelianZodiac(List<int> outEffect, int mod)
    //{
    //    return outEffect;
    //}


}


//round up of baseQ <= 50, truncate on higher
//if( sn.BaseQ >= 50) 
//{
//    int z2 = 100 / sn.BaseQ; //will always be
//    z1 = (actor.StatTotalMaxLife + z2 - 1) / (100 / sn.BaseQ);// 
//}
//else
//{
//    z1 = actor.StatTotalMaxLife * sn.BaseQ / 100;
//}

//if( sn.ElementType == NameAll.ITEM_ELEMENTAL_WEAPON) //dooing this in calculation mod now
//{
//    z1 = actor.GetWeaponPower(true, true); //used in equipment breaks
//    z1 = CalculationZodiac.GetZodiacMain(z1, actor, target);
//    xa += z1;
//}

//if( mod == NameAll.MOD_ATTACK)
//{
//    int wp = outEffect[1]; //stored here
//    //use the dft to find
//    if( dft == 0)
//    {
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = wp * xa;
//    }
//}
//else if( mod == NameAll.MOD_PHYSICAL)
//{
//    if( dft == 0)
//    {
//        outEffect[0] = modifiedBaseHit + xa;
//        outEffect[1] = modifiedBaseQ * xa;
//    }
//    else if( dft == 1)
//    {
//        outEffect[0] = modifiedBaseHit + xa;
//        outEffect[1] = modifiedBaseQ;
//    }
//    else if (dft == 2)
//    {
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = modifiedBaseQ * xa;
//    }
//    else if (dft == 3)
//    {
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = modifiedBaseQ;
//    }
//    else if (dft == 4)
//    {
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = modifiedBaseQ + xa;
//    }
//    else if (dft == 5)
//    {
//        outEffect[0] = modifiedBaseHit + xa;
//        outEffect[1] = modifiedBaseQ + xa;
//    }
//    else if( dft == 21) //knockback guaranteed
//    {
//        outEffect[0] = modifiedBaseHit + xa;
//        outEffect[1] = modifiedBaseQ * xa;
//        outEffect[2] = 2;

//    }
//    else if (dft == 23) 
//    {
//        //damage = PA! * [PA / 2] 
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = xa * ( (actor.StatTotalPA + actor.StatTotalAgi + 1) / 4);
//    }
//    else if (dft == 25) 
//    {
//        // damage = PA! * [PA / 2] 
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = xa * (actor.StatTotalPA / 2 + 2);
//    }
//    else if (dft == 26)
//    {
//        //damage = PA! * ([PA / 2] + 1)  
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = xa * (actor.StatTotalPA / 2 + 1);
//    }
//    else if (dft == 27)
//    {
//        //damage = PA! * ([PA / 2] + 1)  
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = xa * (actor.StatTotalPA);
//    }
//    else if (dft == 28)
//    {
//        //damage = PA! * (PA + 1)    
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = xa * (actor.StatTotalPA + 5);
//    }
//    else if (dft == 29)
//    {
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = xa * (actor.StatTotalPA / 4 + 1);
//    }
//    else if( dft == 31)
//    {
//        //like jump, maybe do something weapon dependent at some point
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = xa * (actor.StatTotalPA + 1);
//    }

//}
//else if (mod == NameAll.MOD_MAGICAL)
//{
//    Debug.Log("in hit dmg mod magical");
//    //doing dft 0 for both stats modified by xa faith, dft 1 for hit only, dft 2 for damage only, 3 for both 1 and 2

//    //for now both hit and dmg modified, here in future can possibly:
//        //            -MEH do dmg like drawout where it's just baseQ times the number (eh really?, can sorta do 
//        //like it with a Mod0 ma based attack)
//        //-modify the dmg
//        //-modify the hit %
//        //-modify both(this is the default for now)
//    //long actor_faith = StatusManager.Instance.GetModFaith(actor.TurnOrder, actor.StatTotalFaith);
//    //long target_faith = StatusManager.Instance.GetModFaith(target.TurnOrder, target.StatTotalFaith);

//    //int n = 1; //numerator used below
//    //int d = 1; //denominator used below

//    //int elemental_type = SpellManager.Instance.GetSpellElementalByIndex(sn.Index);
//    //if (elemental_type != NameAll.ITEM_ELEMENTAL_NONE)
//    //{
//    //    //weak
//    //    if (StatusManager.Instance.IsUnitWeakByElement(target.TurnOrder, elemental_type))
//    //    {
//    //        n *= 2;
//    //    }
//    //    //half
//    //    if (StatusManager.Instance.IsUnitHalfByElement(target, elemental_type))
//    //    {
//    //        d *= 2;
//    //    }
//    //    //absorb
//    //    if (StatusManager.Instance.IsUnitAbsorbByElement(target.TurnOrder, elemental_type))
//    //    {
//    //        n *= -1;
//    //    }
//    //}

//    //if (dft == 0)
//    //{
//    //    long success = (actor_faith * target_faith * (xa + modifiedBaseHit) ) / (10000 ); //not sure about the n and d here
//    //    z1 = (int)success;
//    //    outEffect[0] = z1;

//    //    long damage = (actor_faith * target_faith * xa * modifiedBaseQ * n) / (10000 * d);
//    //    z1 = (int)damage;
//    //    outEffect[1] = z1;
//    //    //outEffect[0] = modifiedBaseHit + xa;
//    //    //outEffect[1] = modifiedBaseQ * xa;
//    //}
//    //else if (dft == 1)
//    //{
//    //    long success = (actor_faith * target_faith * (xa + modifiedBaseHit) ) / (10000 ); //not sure about the n and d here
//    //    z1 = (int)success;
//    //    outEffect[0] = z1;

//    //    outEffect[1] = modifiedBaseQ * n / d;
//    //    //outEffect[0] = modifiedBaseHit + xa;
//    //    //outEffect[1] = modifiedBaseQ;
//    //}
//    //else if (dft == 2)
//    //{

//    //    outEffect[0] = (modifiedBaseHit);// * n / d;


//    //    long damage = (actor_faith * target_faith * xa * modifiedBaseQ * n) / (10000 * d);
//    //    Debug.Log(" " + damage + " " + actor_faith + " " + target_faith + " " + xa + " " + modifiedBaseQ);
//    //    z1 = (int)damage;
//    //    outEffect[1] = z1;
//    //    Debug.Log("in dft 2 " + outEffect[0] + " "+ outEffect[1]);

//    //    //outEffect[0] = modifiedBaseHit;
//    //    //outEffect[1] = modifiedBaseQ * xa;
//    //}
//    //else if (dft == 3)
//    //{
//    //    outEffect[0] = (modifiedBaseHit);// * n / d;
//    //    outEffect[1] = modifiedBaseQ * n / d;
//    //    //outEffect[0] = modifiedBaseHit;
//    //    //outEffect[1] = modifiedBaseQ;
//    //}
//    //else if (dft == 4)
//    //{
//    //    outEffect[0] = (modifiedBaseHit);// * n / d;
//    //    outEffect[1] = (modifiedBaseQ + xa) * n / d;
//    //    //outEffect[0] = modifiedBaseHit;
//    //    //outEffect[1] = modifiedBaseQ + xa;
//    //}
//    //else if (dft == 5)
//    //{
//    //    outEffect[0] = (modifiedBaseHit + xa);// * n / d;
//    //    outEffect[1] = (modifiedBaseQ + xa) * n / d;
//    //    //outEffect[0] = modifiedBaseHit + xa;
//    //    //outEffect[1] = modifiedBaseQ + xa;
//    //}
//    //else if (dft == 6)
//    //{
//    //    long success = (actor_faith * target_faith * (xa + modifiedBaseHit) * n) / (10000 * d); //not sure about the n and d here
//    //    z1 = (int)success;
//    //    outEffect[0] = z1;

//    //    outEffect[1] = (modifiedBaseQ + xa);// * n / d;
//    //    //outEffect[0] = modifiedBaseHit + xa;
//    //    //outEffect[1] = modifiedBaseQ + xa;
//    //}
//    //else if( dft == 7) //sort of draw out ish, used in centurion raise standard
//    //{
//    //    outEffect[0] = (modifiedBaseHit);// * n / d;
//    //    outEffect[1] = (modifiedBaseQ * xa) * n / d;
//    //}


//}
//else if (mod == NameAll.MOD_NEUTRAL)
//{
//    Debug.Log("in hit dmg mod neutral");
//    //doing dft 0 for both stats modified by xa faith, dft 1 for hit only, dft 2 for damage only, 3 for both 1 and 2


//    if (dft == 0)
//    {
//        outEffect[0] = modifiedBaseHit + xa;
//        outEffect[1] = modifiedBaseQ * xa;
//    }
//    else if( dft == 1)
//    {
//        outEffect[0] = modifiedBaseHit + xa;
//        outEffect[1] = modifiedBaseQ;
//    }
//    else if( dft == 2)
//    {
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = modifiedBaseQ * xa;
//    }
//    else if (dft == 3)
//    {
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = modifiedBaseQ;
//    }
//    else if (dft == 4)
//    {
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = modifiedBaseQ + xa;
//    }
//    else if (dft == 5)
//    {
//        outEffect[0] = modifiedBaseHit + xa;
//        outEffect[1] = modifiedBaseQ + xa;
//    }
//    else if( dft == 22)
//    {
//        outEffect[0] = modifiedBaseHit;
//        //recovery = (5 * PA) HP; [(5 * PA) / 2] MP  
//        outEffect[1] = 5 * xa;
//        outEffect[3] = (3 * xa)/2;
//    }
//    else if (dft == 23)
//    {
//        //damage = PA! * [PA / 2] 
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = xa * ((actor.StatTotalPA + actor.StatTotalAgi + 1) / 4);
//    }
//    else if( dft == 24) //random component to it
//    {
//        outEffect[0] = modifiedBaseHit + xa;

//        ////base q is the min and max numbers to get a random number from
//        ////stored in the form to get both the max and min 2001 (1..2)
//        ////divide by 1000 to get the max, then subtract max * 1000 to get the min
//        //OLD: damage = (1..9) * (PA! + [PA / 2])  
//        //NEW: damage = 5...15 * (PA + (pa+agi)/4)
//        z1 = sn.BaseQ / 1000;
//        if (z1 <= 0)
//        { //this should never happen
//            z1 = 1;
//        }
//        int z2 = sn.BaseQ - (1000 * z1);
//        z1 = UnityEngine.Random.Range(z2, z1 + 1); //z1 = r.nextInt(z1) + z2;
//        z1 = z1 * (xa + (actor.StatTotalPA + actor.StatTotalAgi + 1) / 4);
//        outEffect[1] = z1;

//    }
//}
//else if( mod == NameAll.MOD_NULL)
//{
//    if (dft == 0) //straight up values from sn, used for the stat increase walks and others
//    {
//        outEffect[0] = modifiedBaseHit;
//        outEffect[1] = modifiedBaseQ;
//    }
//    else if (dft == 1) //zodiac matters
//    {
//        outEffect[0] = modifiedBaseHit + CalculationZodiac.GetZodiacMain(xa, actor, target);
//        outEffect[1] = modifiedBaseQ * CalculationZodiac.GetZodiacMain(xa, actor, target);
//    }
//    else if( dft == 2)//zodiac and xa matter
//    {
//        outEffect[0] = modifiedBaseHit + CalculationZodiac.GetZodiacMain(xa, actor, target);
//        outEffect[1] = modifiedBaseQ + CalculationZodiac.GetZodiacMain(xa, actor, target);
//    }
//    else if( dft == 30)
//    {
//        outEffect[0] = modifiedBaseHit + CalculationZodiac.GetZodiacMain(xa, actor, target);
//        outEffect[1] = CalculationZodiac.GetZodiacMain(xa, actor, target) * CalculationZodiac.GetZodiacMain(xa, actor, target);
//    }

//}