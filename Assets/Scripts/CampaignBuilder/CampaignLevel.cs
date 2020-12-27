
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CampaignLevel {
    public int CampaignId { get; set; }

    public List<int> orderList; //order is the id
    public List<int> mapList;
    public List<string> nameList;
    public List<int> vcList;
    public List<int> battleXP;
    public List<int> battleAP;

    public CampaignLevel( int id)
    {
        this.CampaignId = id;
        this.orderList = new List<int>();
        this.mapList = new List<int>();
        this.nameList = new List<string>();
        this.vcList = new List<int>();
        this.battleXP = new List<int>();
        this.battleAP = new List<int>();
    }

    //creates a newLevel and returns the levelIndex associated with it
    public int AddNew()
    {
        int z1 = this.orderList.Count;
        this.orderList.Add(z1);
        this.mapList.Add(0);
        this.nameList.Add("Level " + (z1+1));
        this.vcList.Add(NameAll.VICTORY_TYPE_DEFEAT_PARTY);
        return z1;
    }

    public void DeleteLevel(int index)
    {
        
        if( orderList.Count <= 0 || index >= orderList.Count)
        {
            return;
        }
        //delete the last one from order, delete that index from teh list
        int z1 = orderList.Count - 1;
        orderList.RemoveAt(z1);
        nameList.RemoveAt(index);
        mapList.RemoveAt(index);
        vcList.RemoveAt(index);
    }

    //public void AddLevel()
    //{
    //    int z1 = orderList.Count;
    //    this.orderList.Add(z1);
    //    this.nameList.Add("Level " + (z1 + 1));
    //    this.mapList.Add(0);
    //    this.vcList.Add(NameAll.VICTORY_TYPE_DEFEAT_PARTY);
    //}

    public void ReorderLevel(int currentIndex, int newIndex)
    {
        //orderList stays the same
        string zString = GetName(currentIndex);
        int z1 = GetMap(currentIndex);
        int z2 = GetVC(currentIndex);
        if( newIndex > currentIndex)
        {
            //Debug.Log("testing currentIndex, new Index: " + currentIndex +"," + newIndex);
            if( newIndex +1 == nameList.Count)
            {
                //Debug.Log("testing 2");
                nameList.Add(zString);
                mapList.Add(z1);
                vcList.Add(z2);

            }
            else
            {
                nameList.Insert(newIndex, zString);
                mapList.Insert(newIndex, z1);
                vcList.Insert(newIndex, z2);
            }
            
            nameList.RemoveAt(currentIndex);
            mapList.RemoveAt(currentIndex);
            vcList.RemoveAt(currentIndex);
        }
        else if( newIndex < currentIndex)
        {
            //Debug.Log("testing 2");
            nameList.RemoveAt(currentIndex);
            nameList.Insert(newIndex, zString);

            mapList.RemoveAt(currentIndex);
            mapList.Insert(newIndex, z1);

            vcList.RemoveAt(currentIndex);
            vcList.Insert(newIndex, z2);
            
        }
        
    }

    //order is basically immutable, just a list of indexes, can only add or delete
    //public void SetOrder(int index, int value)
    //{
    //    try
    //    {
    //        orderList[index] = value;
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.Log(e.ToString());
    //    }
    //}

    public void SetName(int index, string value)
    {
        try
        {
            nameList[index] = value;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void SetMap(int index, int value)
    {
        try
        {
            mapList[index] = value;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void SetVC(int index, int value)
    {
        try
        {
            vcList[index] = value;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void SetBattleAP(int index, int value)
    {
        try
        {
            battleAP[index] = value;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void SetBattleXP(int index, int value)
    {
        try
        {
            battleXP[index] = value;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public int GetOrder(int index)
    {
        try
        {
            return orderList[index];
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return 0;
        }
    }

    public string GetName(int index)
    {
        try
        {
            return nameList[index];
        }
        catch (Exception e)
        {
            //Debug.Log(e.ToString());
            return NameAll.LEVEL_ERROR_MESSAGE;
        }
    }

    public int GetMap(int index)
    {
        try
        {
            return mapList[index];
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return 0;
        }
    }

    public int GetVC(int index)
    {
        try
        {
            return vcList[index];
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return 0;
        }
    }

    public string GetVCString(int index)
    {
        try
        {
            int z1 = vcList[index];
            return CalcCode.GetVCString(z1);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return "error";
        }
    }

    public string GetMapString(int index)
    {
        try
        {
            int z1 = mapList[index];
            return CalcCode.GetMapString(z1);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return "error: no map found";
        }
    }

    public int GetBattleXP(int index)
    {
        try
        {
            return battleXP[index];
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return 0;
        }
    }

    public int GetBattleAP(int index)
    {
        try
        {
            return battleAP[index];
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return 0;
        }
    }

    //public int LevelId { get; set; }

    //public int Order { get; set; }
    //public int Map { get; set; }
    //public string LevelName { get; set; }
    //public int VictoryCondition { get; set; }




    //public void SaveCampaignLevel()
    //{
    //    string filePath = Application.dataPath + "/Campaigns/Custom/";
    //    Serializer.Save<CampaignLevel>((filePath + this.CampaignId + "_level_" + this.LevelId + ".dat"), this);
    //}
}
