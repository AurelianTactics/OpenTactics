using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class StoryItem {

    public int StoryId {get; set;}
    List<StoryItemObject> storyItemList;

    public StoryItem(int zId)
    {
        this.StoryId = zId;
        this.storyItemList = new List<StoryItemObject>();
    }
	
    public void AddStoryItemObject(int zPointId, int zStoryInt, int zType, List<ItemObject> zList)
    {
        foreach( StoryItemObject sio in this.storyItemList.ToList())
        {
            if (sio.PointId == zPointId && sio.StoryInt == zStoryInt)
            {
                this.storyItemList.Remove(sio); //new one added below
                break;
            }
        }
        storyItemList.Add(new StoryItemObject(zPointId, zStoryInt, zType, zList));
    }

    public void DeleteStoryItemObject(int zPointId, int zStoryInt)
    {
        foreach(StoryItemObject sio in this.storyItemList.ToList())
        {
            if (sio.PointId == zPointId && sio.StoryInt == zStoryInt)
            {
                this.storyItemList.Remove(sio);
                break;
            }
        }
    }

    public List<ItemObject> GetStoryItemListForShop(int zPointId, int zStoryInt)
    {
        foreach (StoryItemObject sio in this.storyItemList.ToList())
        {
            if (sio.PointId == zPointId && sio.StoryInt == zStoryInt)
            {
                return sio.storyItemObjectList;
            }
        }
        return null;
    }

    public string GetShopDetails(int zPointId, int zStoryInt)
    {
        foreach (StoryItemObject sio in this.storyItemList.ToList())
        {
            if (sio.PointId == zPointId && sio.StoryInt == zStoryInt)
            {
                return "All Items Level " + sio.ShopType + " and below included";
            }
        }
        return "unknown";
    }
}

[Serializable]
public class StoryItemObject
{
    public int PointId { get; set; }
    public int StoryInt { get; set; }
    public int ShopType { get; set; }
    public List<ItemObject> storyItemObjectList;

    public StoryItemObject(int zPointId, int zStoryInt, int zType, List<ItemObject> zList)
    {
        this.PointId = zPointId;
        this.StoryInt = zStoryInt;
        this.ShopType = zType;
        this.storyItemObjectList = zList;
    }

}
