using UnityEngine;
using System.Collections.Generic;
using System;

//Each StoryObject has a StoryPoint object associated with it.
//StoryPoint holds a collection of StoryPointObjects
//StoryPointObjects hold details on user can do there (battle, shop, watch cutscene etc)
//represent map points in some instances

//for now, points don't have neighbors or travel paths, at some point can add them and time it takes to get between the points

[Serializable]
public class StoryPoint {

    int uniquePointId = 0;
    public int StoryId { get; set; }
    public List<StoryPointObject> storyPointObjectList;

    public StoryPoint(int storyId)
    {
        this.StoryId = storyId;
        storyPointObjectList = new List<StoryPointObject>();
        AddStoryPointObject();
    }

    //add a new storyPointObject to storyList (gets a unique Id for that spo)
    public void AddStoryPointObject()
    {
        StoryPointObject spo = new StoryPointObject(GetNewPointId());
        storyPointObjectList.Add(spo);
    }

    public void DeleteStoryPointObject(int zPointId)
    {
        foreach (StoryPointObject spo in storyPointObjectList)
        {
            if (spo.PointId == zPointId)
            {
                storyPointObjectList.Remove(spo);
                return;
            }
        }
    }

    public void DeleteStoryPointInt(int zPointId, int zStoryPointIntId)
    {
        foreach (StoryPointObject spo in storyPointObjectList)
        {
            if (spo.PointId == zPointId)
            {
                foreach(StoryPointInt spi in spo.storyIntList)
                {
                    if( spi.StoryPointIntId == zStoryPointIntId)
                    {
                        spo.storyIntList.Remove(spi);
                        return;
                    }
                }                
            }
        }
    }

    int GetNewPointId()
    {
        uniquePointId += 1;
        return uniquePointId;
    }

    public int GetFirstPointId()
    {
        if (storyPointObjectList.Count > 0)
            return storyPointObjectList[0].PointId;
        else
            return NameAll.NULL_INT;
    }

    public string GetPointName(int zPointId)
    {
        if (zPointId == NameAll.NULL_INT)
            return "No Story Point Selected";

        foreach(StoryPointObject spo in storyPointObjectList)
        {
            if (spo.PointId == zPointId)
                return spo.PointName;
        }

        return "No Story Point Selected";
    }

    public List<int> GetPointXY(int zPointId)
    {
        List<int> retValue = new List<int>();
        foreach (StoryPointObject spo in storyPointObjectList)
        {
            if (spo.PointId == zPointId)
            {
                retValue.Add(spo.ScreenX);
                retValue.Add(spo.ScreenY);
                return retValue;
            }
        }
        Debug.Log("ERROR: no point found");
        return null;
    }

    public void SetPointX(int zPointId, int x)
    {
        foreach (StoryPointObject spo in storyPointObjectList)
        {
            if (spo.PointId == zPointId)
            {
                spo.SetScreenX(x);
            }
        }
    }

    public void SetPointY(int zPointId, int y)
    {
        foreach (StoryPointObject spo in storyPointObjectList)
        {
            if (spo.PointId == zPointId)
            {
                spo.SetScreenY(y);
            }
        }
    }

    public List<StoryPointInt> GetStoryPointIntList( int zPointId)
    {
        var retValue = new List<StoryPointInt>();
        foreach (StoryPointObject spo in storyPointObjectList)
        {
            if (spo.PointId == zPointId)
            {
                return spo.storyIntList;
            }
        }

        return retValue;
    }

    public void AddStoryPointInt(int zPointId)
    {
        foreach (StoryPointObject spo in storyPointObjectList)
        {
            if (spo.PointId == zPointId)
            {
                spo.AddStoryPointInt(zPointId);
            }
        }
    }

    public StoryPointObject GetStoryPointObject(int zPointId)
    {

        foreach (StoryPointObject spo in storyPointObjectList)
        {
            if (spo.PointId == zPointId)
            {
                return spo;
            }
        }

        return null;
    }

    public StoryPointInt GetStoryPointInt(int zPointId, int zStoryPointIntId)
    {
        if (zPointId == NameAll.NULL_INT || zStoryPointIntId == NameAll.NULL_INT)
            return null;

        foreach (StoryPointObject spo in storyPointObjectList)
        {
            if (spo.PointId == zPointId)
            {
                foreach(StoryPointInt spi in spo.storyIntList)
                {
                    if (spi.StoryPointIntId == zStoryPointIntId)
                        return spi;
                }
            }
        }

        return null;
    }
}

[Serializable]
public class StoryPointObject
{
    int uniqueStoryPointIntId = 0;

    public int PointId { get; set; }
    public string PointName { get; set; }
    public int ScreenX { get; private set; } //how far along the x axis point is. 0,0 is the origin. normalized from 1-100
    public int ScreenY { get; private set; } //how far along the y axis point is
    public List<StoryPointInt> storyIntList; //show is positive numbers, hide is negative numbers. when to hide/show the point on the map based on the overall story progress
    //example: if story progress is 10 and last show point is 7 and last hide point is -6, compare the absolute values. 7 > 6 so show. If 5 and -6 then hide (5 < 6)

    //public int PointIcon { get; set; } //not doing yet

    public StoryPointObject(int pointId)
    {
        this.PointId = pointId;
        this.PointName = "Point " + pointId;
        this.ScreenX = 50 + UnityEngine.Random.Range(-10,10);
        this.ScreenY = 50 + UnityEngine.Random.Range(-10, 10);
        this.storyIntList = new List<StoryPointInt>();
    }

    public void SetScreenX(int z1)
    {
        this.ScreenX = Mathf.Clamp(z1, 0, 98); //100 could result in point being off the map
    }

    public void SetScreenY(int z1)
    {
        this.ScreenY = Mathf.Clamp(z1, 0, 98);
    }

    public void AddStoryPointInt(int storyPointId)
    {
        StoryPointInt spi = new StoryPointInt(GetNewStoryPointIntId(),0,storyPointId);
        this.storyIntList.Add(spi);
        //Debug.Log("current storypoint int is " + spi.StoryPointIntId );
    }

    int GetNewStoryPointIntId()
    {
        //Debug.Log("current storypoint int is " + storyPointIntId);
        uniqueStoryPointIntId += 1; //Debug.Log("current storypoint int is " + storyPointIntId);
        return uniqueStoryPointIntId;
    }

}

//each story point can change as the story progresses. list of these are in each StoryPointObject. Depending on the storyInt, the latest storyPointInt is shown
[Serializable]
public class StoryPointInt
{
    public int StoryPointIntId { get; set; } //unique identifier
    public int StoryInt { get; set; } //trigger to decide which of the list of StoryPointInts is shown. Goes with the largest value <= StoryInt
    public bool IsShown { get; set; } //is shown on map
    public int StoryPointId { get; set; } //list is also saved in StorySave so need to associate it with a Point

    public bool IsHasShop { get; set; } //has a shop
    public bool IsHasStoryBattle { get; set; } //has a story battle. set to false when story battle is consumed
    public int StoryBattleId { get; set; } //identifier to know which story battle to load

    public bool IsHasRandomBattle { get; set; } //has a chance at random battle (if no story battle)
    public int RandomBattleId { get; set; } //identifier to know which random battle to load
    public bool IsHasCutScene { get; set; } //has a cut scene on trigger. once used, set to false (or the cutscene will be repeated)
    public int CutSceneId { get; set; }

    //public int RandomBattleChance { get; set; } //implement later
    //public int ShopId {get; set; } //implement later

    public int ProgressionIntBattle { get; set; } //ProgressionInt combined with StoryInt lead to new StoryInt. On battle completed, this is how the the progression int returned
    public int ProgressionIntCutScene { get; set; } //on cut scene watched, this is how the storyInt progresses

    //only has storyInt, thus not being shown and no details
    public StoryPointInt( int zStoryPointIntId, int zStoryInt, int zStoryPointId )
    {
        this.StoryPointIntId = zStoryPointIntId;

        this.StoryInt = zStoryInt;
        this.StoryPointId = zStoryPointId;
        this.IsShown = false;
        this.IsHasShop = false;
        this.IsHasStoryBattle = false;
        this.StoryBattleId = NameAll.NULL_INT;

        this.IsHasRandomBattle = false;
        this.RandomBattleId = NameAll.NULL_INT;
        this.IsHasCutScene = false;
        this.CutSceneId = NameAll.NULL_INT;

        this.ProgressionIntBattle = NameAll.NULL_INT;
        this.ProgressionIntCutScene = NameAll.NULL_INT;
    }

    public StoryPointInt(int storyPointIntId, int storyInt, int storyPointId, bool isShown, bool isShop, bool isStoryBattle, int storyBattleId, bool isRandomBattle, int randomBattleId,
        bool isCutScene, int cutSceneId, int proIntBattle, int proIntCutScene)
    {

        this.StoryPointIntId = StoryPointIntId;

        this.StoryInt = storyInt;
        this.StoryPointId = storyPointId;
        this.IsShown = isShown;
        this.IsHasShop = isShop;
        this.IsHasStoryBattle = isStoryBattle;
        this.StoryBattleId = storyBattleId;

        this.IsHasRandomBattle = isRandomBattle;
        this.RandomBattleId = randomBattleId;
        this.IsHasCutScene = isCutScene;
        this.CutSceneId = cutSceneId;

        this.ProgressionIntBattle = proIntBattle;
        this.ProgressionIntCutScene = proIntCutScene;
    }

    public string ShowStoryPointIntDetails()
    {
        string zString = "";
        if( this.IsShown)
        {
            zString = "Show point at Story #" + this.StoryInt;
        }
        else
        {
            zString = "Hide point at Story #" + this.StoryInt;
        }

        return zString;
    }

    //public string ShowIsShown()
    //{
    //    string zString = "";
    //    if (this.IsShown)
    //    {
    //        zString = "Point is shown";
    //    }
    //    else
    //    {
    //        zString = "Hide point at Story #" + this.StoryInt;
    //    }

    //    return zString;
    //}
}
