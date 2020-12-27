
//takes a spellName in and returns a series of classifications that the AI uses to better choose its CombatPlanOfAttack
//a new one is created in each CombatComputerPlayer Evaluate function
public class SpellNameAI {

    public bool isAbleToFight; //if true, should only target isAbleToFight Units
    public int isAbleToFightStatusId; //if isAbleToFight is false, what status does it target for healing (dead, petrify, blood suck (invite handled differently)

    public Targets targetType; //foe/ally
    public bool isReviveType;
    public bool isCureType;

    public bool isDamageType; //used for damaging eneym
    public int spellId; //used for quick access for the AI

    public bool isHitSelf; //can the ability hit the caster

    public SpellNameAI(SpellName sn)
    {
        this.spellId = sn.SpellId;
        this.isReviveType = CheckReviveType(sn);
        
        if( this.isReviveType)
        {
            this.isAbleToFight = false;
            this.isAbleToFightStatusId = sn.StatusType;
        }
        else
        {
            this.isAbleToFight = CheckAbleToFight(sn);
            if (isAbleToFight)
                this.isAbleToFightStatusId = NameAll.STATUS_ID_NONE;
            else
                this.isAbleToFightStatusId = sn.StatusType;
        }
        
        this.targetType = CheckTargetType(sn);
        this.isCureType = CheckCureType(sn);
        this.isDamageType = CheckDamageType(sn);
        this.isHitSelf = CheckHitSelf(sn);
    }

    bool CheckHitSelf(SpellName sn)
    {
        if (sn.RangeXYMin <= 0)
        {
            if (sn.CasterImmune == 1)
                return false;
            else if (sn.EffectXY == NameAll.SPELL_EFFECT_ENEMIES)
                return false;
            else if (sn.AlliesType == NameAll.ALLIES_TYPE_ENEMIES)
                return false;

            return true;
        }

        return false;
    }

    bool CheckDamageType(SpellName sn)
    {
        if( sn.RemoveStat == NameAll.REMOVE_STAT_REMOVE || sn.RemoveStat == NameAll.REMOVE_STAT_ABSORB)
        {
            if( sn.HitsStat == NameAll.HITS_STAT || sn.HitsStat == NameAll.HITS_STAT_PERCENTAGE)
            {
                if( sn.StatType == NameAll.STAT_TYPE_HP || sn.StatType == NameAll.STAT_TYPE_MAX_HP)
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool CheckReviveType(SpellName sn)
    {
        if (StatusManager.Instance.IfStatusCuredBySpell(NameAll.STATUS_ID_DEAD, sn))
        {
            return true;
        }
        return false;
    }

    bool CheckAbleToFight(SpellName sn)
    {
        //revive type checked before
        if (StatusManager.Instance.IfStatusCuredBySpell(NameAll.STATUS_ID_DEAD, sn))
        {
            return false;
        }
        else if (StatusManager.Instance.IfStatusCuredBySpell(NameAll.STATUS_ID_BLOOD_SUCK, sn))
        {
            return false;
        }

        return true;
    }

    Targets CheckTargetType(SpellName sn)
    {
        if( sn.RemoveStat == NameAll.REMOVE_STAT_HEAL )
            return Targets.Ally;

        return Targets.Foe;
    }

    bool CheckCureType(SpellName sn)
    {
        if( sn.RemoveStat == NameAll.REMOVE_STAT_HEAL && sn.HitsStat != NameAll.HITS_STAT_NONE 
            && (sn.StatType == NameAll.STAT_TYPE_HP || sn.StatType == NameAll.STAT_TYPE_MAX_HP ))
        {
            return true;
        }
        return false;
    }
}

