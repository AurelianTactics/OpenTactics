
using System.Collections.Generic;
using System.Linq;

//each match has combat stats
//for now determining unit XP and AP
//categories: dmg done, healing done, statuses inflicted, statuses cured, deaths caused, deaths cured
//then award xp/ap based on units % in that category
//for now, only used in single player story mode (so only stores P1 stats)


public class CombatStats {

    List<int> playerUnitIdList;
    int BattleAP;
    int BattleXP;

    public List<CombatStatObject> statList;

    public CombatStats(List<PlayerUnit> playerUnitList, int battleAP, int battleXP)
    {
        statList = new List<CombatStatObject>();

        this.playerUnitIdList = new List<int>();
        foreach( PlayerUnit pu in playerUnitList)
        {
            this.playerUnitIdList.Add(pu.TurnOrder);
        }

        this.BattleAP = battleAP;
        this.BattleXP = battleXP;
    }

    //call this from playermanager when dmg/healing/etc is done
    public void AddStatValue(int unitId, int teamId, int statType, int statValue)
    {
        var tempList = statList.Where(s => s.UnitId == unitId && s.StatType == statType);// sSpellCommandSetList.Where(s => s.CommandSet == commandSetId);
        if( tempList.Count() > 0)
        {
            tempList.ToList().ForEach(s=> s.StatValue += statValue);
        }
        else
        {
            CombatStatObject cso = new CombatStatObject(unitId, teamId, statType, statValue);
            statList.Add(cso);
        }

        //foreach ( CombatStatObject cso in statList)
        //{
        //    if( unitId == cso.UnitId && statType == cso.StatType)
        //    {

        //        break;
        //    }
        //}
    }

    //at end of battle, return this to see how much ap/xp was gained
    //half the ap and xp is automatically assigned to each unit
    //test to see what % each unit did in each of the categories. that weight is then normalized by the teams's weight and multiplied by half the xp/ap to get that number
    public List<CombatStatObject> GetTeamAPXP(List<int> unitIdList, int battleAP, int battleXP, int teamId)
    {
        List<CombatStatObject> retValue = new List<CombatStatObject>();

        Dictionary<int, float> unitWeightDict = new Dictionary<int, float>();

        foreach( int i in unitIdList)
        {
            unitWeightDict.Add(i, 0.0f);
            retValue.Add(new CombatStatObject(i, teamId, battleAP / 2, battleXP / 2));
        }

        int totalDmg = 0;
        //int totalHealing = 0;
        int statusesDone = 0;
        //int statusesHealed = 0;
        int killsDone = 0;
        //int killsCured = 0;

        var tempList = statList.Where(s => s.TeamId == teamId);

        var subList = tempList.Where(s => s.StatType == NameAll.STATS_DAMAGE_DONE || s.StatType == NameAll.STATS_DAMAGE_HEALED).ToList();
        foreach( CombatStatObject cso in subList)
        {
            totalDmg += cso.StatValue;
        }
        unitWeightDict = ChangeWeightDict(unitWeightDict, subList, totalDmg);

        subList = tempList.Where(s => s.StatType == NameAll.STATS_STATUSES_DONE || s.StatType == NameAll.STATS_STATUSES_HEALED).ToList();
        foreach (CombatStatObject cso in subList)
        {
            statusesDone += cso.StatValue;
        }
        unitWeightDict = ChangeWeightDict(unitWeightDict, subList, statusesDone);

        subList = tempList.Where(s => s.StatType == NameAll.STATS_KILLS_DONE || s.StatType == NameAll.STATS_KILLS_HEALED).ToList();
        foreach (CombatStatObject cso in subList)
        {
            killsDone += cso.StatValue;
        }
        unitWeightDict = ChangeWeightDict(unitWeightDict, subList, killsDone);

        float totalWeight = 0.0f;
        foreach (KeyValuePair<int, float> kvp in unitWeightDict)
        {
            totalWeight += kvp.Value;
        }

        if( totalWeight > 0)
        {
            foreach( CombatStatObject cso in retValue)
            {
                cso.StatType += (int)(unitWeightDict[cso.UnitId] / totalWeight * battleAP / 2);
                cso.StatValue += (int)(unitWeightDict[cso.UnitId] / totalWeight * battleXP / 2);
            }
        }


        return retValue;
    }

    Dictionary<int,float> ChangeWeightDict(Dictionary<int,float> weightDict, List<CombatStatObject> csoList, int totalStat)
    {
        if (totalStat > 0)
        {
            float zFloat = (float)totalStat;
            foreach (CombatStatObject cso in csoList)
            {
                foreach (KeyValuePair<int, float> kvp in weightDict)
                {
                    if (cso.UnitId == kvp.Key)
                    {
                        weightDict[kvp.Key] += (float)cso.StatValue / zFloat;
                        break;
                    }
                }
            }
        }
        return weightDict;
    }

    public List<AbilityBuilderObject> GetDisplayList(int teamId)
    {
        var retValue = new List<AbilityBuilderObject>();
        var statsList = GetTeamAPXP(this.playerUnitIdList,this.BattleAP,this.BattleXP,teamId);
        AbilityBuilderObject abo;

        //first thing in the list is the total available party XP and AP
        abo = new AbilityBuilderObject("Party XP and AP " + this.BattleXP + " and " + this.BattleAP,"",NameAll.NULL_INT);
        retValue.Add(abo);
        //gets XP and AP available by unit
        foreach ( CombatStatObject cso in statsList)
        {
            PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(cso.UnitId);
            abo = new AbilityBuilderObject(pu.UnitName  , "Gains " + cso.StatValue + " xp and " + cso.StatType + " ap", cso.UnitId);
            
        }
        return retValue;
    }

}

public class CombatStatObject
{
    public int UnitId { get; set; }
    public int TeamId { get; set; } //meh just get by unitId
    public int StatType { get; set; }
    public int StatValue { get; set; }

    public CombatStatObject(int zId, int zTeam, int zType, int zValue)
    {
        this.UnitId = zId;
        this.TeamId = zTeam;
        this.StatType = zType;
        this.StatValue = zValue;
    }
}
