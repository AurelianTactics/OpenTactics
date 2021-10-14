using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatRandomAbilityPicker : CombatBaseAbilityPicker
{
	//public List<BaseAbilityPicker> pickers;

	public CombatPlanOfAttack PickRandom ( CombatPlanOfAttack combatPlan, PlayerUnit pu)
	{
        //List<SpellName> primaryList = SpellManager.Instance.GetSpellNamesByCommandSet(pu.ClassId, pu);
        //List<SpellName> secondaryList = SpellManager.Instance.GetSpellNamesByCommandSet(pu.AbilitySecondaryCode, pu);

        int z1 = Random.Range(0, 4); //Debug.Log("FORCING AI TO CHOOSE A SECONDARY ABILITY, DISABLE THIS AFTER TESTING " + z1);
        if( z1 == 0) //default/attack
        {
            combatPlan.spellName = Default(pu); //Debug.Log("turn back into true random when testing done");
            combatPlan.target = Targets.Foe;
        }
        else if( z1 == 1) //grab from secondary
        {
            if (pu.secondaryAbilityIdList.Count <= 0)
            {
                combatPlan.spellName = Default(pu); //Debug.Log("grabbing from secondary but no abilities " + puo.secondaryAbilityIdList.Count);
                combatPlan.target = Targets.Foe;
            }
            else
            {
                int z2 = Random.Range(0, (pu.secondaryAbilityIdList.Count - 1)); //Debug.Log("random ability " + z2 + "," + puo.secondaryAbilityIdList[z2]);
                combatPlan.spellName = SpellManager.Instance.GetSpellNameByIndex(pu.secondaryAbilityIdList[z2]);
                if (SpellManager.Instance.IsSpellPositive(combatPlan.spellName))
                {
                    combatPlan.target = Targets.Ally;
                }
                else
                {
                    combatPlan.target = Targets.Foe;
                }
            }
            
            
        }
        else //grab from primary
        {
            if( pu.primaryAbilityIdList.Count <= 0)
            {
                combatPlan.spellName = Default(pu);
                combatPlan.target = Targets.Foe;
            }
            else
            {
                int z2 = Random.Range(0, (pu.primaryAbilityIdList.Count - 1));
                combatPlan.spellName = SpellManager.Instance.GetSpellNameByIndex(pu.primaryAbilityIdList[z2]); //Debug.Log("random ability " + z2 + "," + puo.primaryAbilityIdList[z2]);
                combatPlan.target = Targets.Foe;

                if (SpellManager.Instance.IsSpellPositive(combatPlan.spellName))
                {
                    combatPlan.target = Targets.Ally;
                }
                else
                {
                    combatPlan.target = Targets.Foe;
                }
            }
            
        }
        //Debug.Log("grabbing random ability " + combatPlan.spellName.AbilityName + "," + combatPlan.spellName.Index + "," + z1);
        //Debug.Log("FORCING AI TO USE EITHER, FIX THIS");
        //combatPlan.target = Targets.Ally;
        //combatPlan.spellName = SpellManager.Instance.GetSpellNameByIndex(149);
        return combatPlan;

        //int index = Random.Range(0, pickers.Count);
        //BaseAbilityPicker p = pickers[index];
        //p.Pick(plan);
    }

    public CombatPlanOfAttack PickAIStatus( CombatPlanOfAttack combatPlan, PlayerUnit pu, bool IsBerserk, bool IsConfusion, bool IsCharm )
    {
        if(IsBerserk)
        {
            combatPlan.spellName = Default(pu); //Debug.Log("is berserk/confusion/charm: " + IsBerserk + " " + IsConfusion + " " + IsCharm);
            combatPlan.target = Targets.Foe;
            if(IsConfusion)
            {
                if(Random.Range(0, 2) == 0)
                {
                    combatPlan.target = Targets.Ally;
                }
            }
            else if( IsCharm)
            {
                combatPlan.target = Targets.Ally;
            }
            
        }
        else if(IsConfusion)
        {
            combatPlan = PickRandom(combatPlan, pu);
            if (Random.Range(0, 2) == 0)
            {
                combatPlan.target = Targets.Ally;
            }
            else
            {
                combatPlan.target = Targets.Foe;
            }
        }
        else if (IsCharm)
        {
            combatPlan = PickRandom(combatPlan, pu);
            if (combatPlan.target == Targets.Ally)
                combatPlan.target = Targets.Foe;
            else if (combatPlan.target == Targets.Foe)
                combatPlan.target = Targets.Ally;
        }
        combatPlan.POASummary();
        return combatPlan;
    }

    public CombatPlanOfAttack PickEnemyHurt(CombatPlanOfAttack poa, PlayerUnit pu, int attempt)
    {
        poa.spellName = null; //have to reset as this is called multiple times and the spellName will persist between attempts
        attempt = attempt % 3; //Debug.Log("attempt is " + attempt);
        if(attempt == 0)
        {
            poa.spellName = Default(pu); //Debug.Log("choosing attack in order to hurt an enemy");
        }
        else
        {
            foreach(SpellNameAI snai in pu.aiSpellList)
            {
                if( snai.isDamageType) //looks for next is damage type spell, called multiple times so don't watn to keep grabbign the same ability (ie 1st time grab 1st ability, 2nd time 2nd ability etc)
                {
                    if(attempt <= 1)
                    {
                        poa.spellName = SpellManager.Instance.GetSpellNameByIndex(snai.spellId);
                        break;
                    }
                    else
                    {
                        attempt -= 1;
                    }
                }
            }
        }

        poa.target = Targets.Foe;
        return poa;
    }

    public CombatPlanOfAttack PickAllyHurt(CombatPlanOfAttack poa, PlayerUnit pu)
    {
        poa.target = Targets.Ally;
        List<SpellName> tempList = new List<SpellName>();
        foreach (SpellNameAI snai in pu.aiSpellList)
        {
            if (snai.isCureType)
            {
                tempList.Add(SpellManager.Instance.GetSpellNameByIndex(snai.spellId));
                
            }
        }

        if( tempList.Count > 1)
        {
            poa.spellName = tempList[Random.Range(0,(tempList.Count-1))];
        }
        else
        {
            if(tempList.Count > 0) //probably not necessary
                poa.spellName = tempList[0];
        }
        
        return poa;
    }

    public CombatPlanOfAttack PickAllyDead(CombatPlanOfAttack poa, PlayerUnit pu)
    {
        poa.target = Targets.Ally;
        List<SpellName> tempList = new List<SpellName>();
        foreach (SpellNameAI snai in pu.aiSpellList)
        {
            if (snai.isReviveType)
            {
                tempList.Add(SpellManager.Instance.GetSpellNameByIndex(snai.spellId));

            }
        }

        if (tempList.Count > 1)
        {
            poa.spellName = tempList[Random.Range(0, (tempList.Count - 1))];
        }
        else
        {
            if (tempList.Count > 0) //probably not necessary
                poa.spellName = tempList[0];
        }

        return poa;
    }
}