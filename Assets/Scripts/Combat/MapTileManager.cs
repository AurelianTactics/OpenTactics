using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//current version basically just holds the markes for each unit (which could just be attached to the PlayerUnitObjects)
//few convenience functions as well
//used differently in old version
public class MapTileManager : Singleton<MapTileManager>
{

    private List<GameObject> sMarkerList;

    //[SerializeField]
    //GameObject groundCrystalPrefab;

    protected MapTileManager()
    { // guarantee this will be always a singleton only - can't use the constructor!
        sMarkerList = new List<GameObject>();
    }

	//on new map for WA mode, clear the marke list
	public void ClearNewBoard()
	{
		sMarkerList = new List<GameObject>();
	}

    public void MoveMarker(int unitId, Tile t)
    {
        GameObject go = GetMarkerByIndex(unitId);
        Vector3 vec = t.transform.position;
        vec += new Vector3(0, vec.y, 0);
        go.transform.position = vec;
    }

    public GameObject GetMarkerByIndex(int index)
    {
        try
        {
            return sMarkerList[index];
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.LogError("ERROR: " + e.ToString());
        }
        return null;
    }

    public void AddMarker(GameObject marker)
    {
        //Debug.Log("in addmarker" + sMarkerList.Count);
        sMarkerList.Add(marker);
        //Debug.Log("in addmarker" + sMarkerList.Count);
    }

    //called from PlayerManager when a unit crystalizes
    public void DisableMarker(int unitId)
    {
        GetMarkerByIndex(unitId).transform.position = new Vector3(-5, -5, -5);
    }

    //used on invite 
    //public void ChangeMarker(int teamId, int markerIndex)
    //{
    //    GameObject marker;
    //    if (teamId == NameAll.TEAM_ID_GREEN)
    //    {
    //        marker = Instantiate(Resources.Load("MarkerTeam1")) as GameObject;
    //    }
    //    else
    //    {
    //        marker = Instantiate(Resources.Load("MarkerTeam2")) as GameObject;
    //    }
    //    GameObject oldMarker = sMarkerList[markerIndex];
    //    Destroy(oldMarker);
    //    sMarkerList.RemoveAt(markerIndex);
    //    sMarkerList.Insert(markerIndex, marker);
    //    MoveMarker(markerIndex, startTile);
    //}

    //returns distance in tiles between tiles
    public int GetDistanceBetweenPlayerUnits(PlayerUnit pu1, PlayerUnit pu2)
    {
        return (Math.Abs(pu1.TileX - pu2.TileX) + Math.Abs(pu1.TileY - pu2.TileY));
    }

    //returns distance in tiles between tiles
    public int GetDistanceBetweenTiles(int x1, int y1, int x2, int y2)
    {
        return (Math.Abs(x1 - x2) + Math.Abs(y1 - y2));
    }

    public int GetDistanceBetweenTiles(Tile t1, Tile t2)
    {
        return (Math.Abs(t1.pos.x - t2.pos.x) + Math.Abs(t1.pos.y - t2.pos.y));
    }

    public bool IsTileInAttackRange(PlayerUnit actor, PlayerUnit target, SpellName sn)
    {
        int originX = actor.TileX; 
        int originY = actor.TileY;
        float originZ = actor.TileZ;
        int targetX = target.TileX;
        int targetY = target.TileY;
        float targetZ = target.TileZ;
        int spellMinRange = sn.RangeXYMin;
        int spellMaxRange = sn.RangeXYMax;
        int spellZRange = sn.RangeZ;

        if (Math.Abs(originX - targetX) + Math.Abs(originY - targetY) >= spellMinRange
            && Math.Abs(originX - targetX) + Math.Abs(originY - targetY) <= spellMaxRange
            && Math.Abs(originZ - targetZ) <= spellZRange)
        {
            return true;
        }
        return false;
    }

}
