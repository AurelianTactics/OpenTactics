using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CampaignSpawn
{
    public int CampaignId { get; set; }
    public List<SpawnObject> spawnList;

    public CampaignSpawn(int campaignId)
    {
        this.CampaignId = campaignId;
        this.spawnList = new List<SpawnObject>();
    }


    //VALUES MATCH THOSE IN CAMPAIGN EDIT
    static readonly int SPAWN_TYPE = 21;
    static readonly int SPAWN_TEAM = 22;
    static readonly int SPAWN_UNIT_1 = 23;
    static readonly int SPAWN_UNIT_2 = 24;
    static readonly int SPAWN_UNIT_3 = 25;
    static readonly int SPAWN_UNIT_4 = 26;
    static readonly int SPAWN_UNIT_5 = 27;

    public List<SpawnObject> GetSpawnListByLevel(int level)
    {
        var enumerator = this.spawnList.Where(d => d.Level == level);
        return enumerator.ToList();
    }

    public void DeleteSpawnByLevel(int level)
    {
        this.spawnList.RemoveAll(d => d.Level == level);
    }

    public SpawnObject GetSpawn(int spawnId, int level)
    {
        var tempVar = GetSpawnListByLevel(level);
        foreach( SpawnObject s in tempVar)
        {
            if (s.SpawnId == spawnId)
                return s;
        }
        return null;
    }

    public void UpdateValue(int spawnId, int level, int category, int value)
    {
        for( int i = 0; i < this.spawnList.Count; i++)
        {
            if( this.spawnList[i].Level == level && this.spawnList[i].SpawnId == spawnId)
            {
                if( category == SPAWN_TEAM)
                {
                    this.spawnList[i].Team = value;
                }
                else if( category == SPAWN_TYPE)
                {
                    this.spawnList[i].SpawnType = value;
                }
                else if (category == SPAWN_UNIT_1)
                {
                    this.spawnList[i].Unit1 = value;
                }
                else if (category == SPAWN_UNIT_2)
                {
                    this.spawnList[i].Unit2 = value;
                }
                else if (category == SPAWN_UNIT_3)
                {
                    this.spawnList[i].Unit3 = value;
                }
                else if (category == SPAWN_UNIT_4)
                {
                    this.spawnList[i].Unit4 = value;
                }
                else if (category == SPAWN_UNIT_5)
                {
                    this.spawnList[i].Unit5 = value;
                }
            }
        }
    }
}

[Serializable]
public class SpawnObject
{
    public int SpawnId { get; set; }
    public int Level { get; set; }
    public int Team { get; set; }
    public int SpawnType { get; set; }
    public int Unit1 { get; set; }
    public int Unit2 { get; set; }
    public int Unit3 { get; set; }
    public int Unit4 { get; set; }
    public int Unit5 { get; set; }

    public SpawnObject()
    {
        this.SpawnId = 0;
        this.Level = 0;
        this.Team = NameAll.TEAM_ID_GREEN;
        this.SpawnType = NameAll.SPAWN_TYPE_RANDOM;
        this.Unit1 = 0;
        this.Unit2 = NameAll.CAMPAIGN_SPAWN_UNIT_NULL;
        this.Unit3 = NameAll.CAMPAIGN_SPAWN_UNIT_NULL;
        this.Unit4 = NameAll.CAMPAIGN_SPAWN_UNIT_NULL;
        this.Unit5 = NameAll.CAMPAIGN_SPAWN_UNIT_NULL;
    }

    public SpawnObject(int level, int teamId, int spawnId)
    {
        this.SpawnId = spawnId;
        this.Level = level;
        this.Team = teamId;
        this.SpawnType = NameAll.SPAWN_TYPE_RANDOM;
        this.Unit1 = 0;
        this.Unit2 = NameAll.CAMPAIGN_SPAWN_UNIT_NULL;
        this.Unit3 = NameAll.CAMPAIGN_SPAWN_UNIT_NULL;
        this.Unit4 = NameAll.CAMPAIGN_SPAWN_UNIT_NULL;
        this.Unit5 = NameAll.CAMPAIGN_SPAWN_UNIT_NULL;
    }

}