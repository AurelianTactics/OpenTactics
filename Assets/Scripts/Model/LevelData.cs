//class for creating level data
//used for generating maps

//WA Mode:
//called in WalkAroundMapGenerator for walk around mode
//Future: build a map/board based on a string, various parts of the string decoded into various values
//currently maps based on a seed, mapId and starting seed determine which map to build

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public class LevelData : ScriptableObject 
//{
//	public List<Vector3> tiles;
//}

[System.Serializable]
public class LevelData
{

    public List<SerializableVector3> tiles;
    public string levelName;
    public List<SerializableVector3> spList; //spawn points and teamID that can spawn there //(point.x,point.y, teamId) fucking serialization
	public List<int> tileTypeList; //what tile type, used for exiting maps in WA. can be used for other things (traps, treasures, etc. later)
	public List<PlayerUnit> mapUnitList = new List<PlayerUnit>(); //list of pu for the map

    public LevelData(){}

	//set size, used for gridworld
	public LevelData(int mapX, int mapY)
	{
		this.levelName = "gridworldMap";
		this.tileTypeList = new List<int>();
		this.spList = new List<SerializableVector3>();
		this.mapUnitList = new List<PlayerUnit>();

		int x = mapX;
		int y = mapY;
		int z = 1;
		this.tiles = new List<SerializableVector3>();

		for (int i = 0; i < x; i++)
		{
			for (int j = 0; j < y; j++)
			{
				this.tiles.Add(new SerializableVector3(i, z, j)); //height is second, y is third //board.tiles.Add(new SerializableVector3(t.pos.x, t.height, t.pos.y));
				this.tileTypeList.Add(NameAll.TILE_TYPE_DEFAULT);
			}
		}

	}

	//randomized, used for WA mode
	public LevelData(string level_string, int seed, int timeInt, int mapX, int mapY, bool isFirstMapVisit )
    {
		Debug.Log("Test Map Generation random number for seed  " + seed +", x: " + mapX +" y: " + mapY);
		Random.InitState(seed);
		int x_offset = UnityEngine.Random.Range(0, 1000) * mapX;
		int y_offset = UnityEngine.Random.Range(1001, 10000) * mapY;
		Random.InitState(x_offset + y_offset);

		this.levelName = level_string;
        //add code here to build a custom level, for now just random
        int x = UnityEngine.Random.Range(7, 15);
        int y = UnityEngine.Random.Range(7, 15);
        int z = 1;
        this.tiles = new List<SerializableVector3>();
		this.tileTypeList = new List<int>();
        
        for ( int i = 0; i < x; i++)
        {
            for( int j = 0; j < y; j++)
            {
                this.tiles.Add(new SerializableVector3(i, z, j)); //height is second, y is third //board.tiles.Add(new SerializableVector3(t.pos.x, t.height, t.pos.y));
				if(i == 0 || j == 0 || i == x-1 || j == y - 1) //set tile type. default for now except edges let you exit map
				{
					this.tileTypeList.Add(NameAll.TILE_TYPE_EXIT_MAP);
				}
				else
				{
					this.tileTypeList.Add(NameAll.TILE_TYPE_DEFAULT);
				}
				
            }
			//Debug.Log("wtf is up with these tiles " + this.tiles[i].ToString());
		}

        this.spList = new List<SerializableVector3>(1);
		//placeholder, x/y/z doesn't matter for first two coordinates, just teh actual values held there
		//this.spList.Add(new SerializableVector3(this.tiles[0].x, this.tiles[0].z, NameAll.TEAM_ID_WALK_AROUND_GREEN));
		this.spList.Add(new SerializableVector3(3, 2, NameAll.TEAM_ID_WALK_AROUND_GREEN));
		this.spList.Add(new SerializableVector3(this.tiles[1].x, this.tiles[1].z, NameAll.TEAM_ID_WALK_AROUND_GREEN));
		this.spList.Add(new SerializableVector3(this.tiles[4].x, this.tiles[4].z, NameAll.TEAM_ID_WALK_AROUND_NEUTRAL));
		this.spList.Add(new SerializableVector3(this.tiles[this.tiles.Count-1].x, this.tiles[this.tiles.Count - 1].z, NameAll.TEAM_ID_WALK_AROUND_RED));

		//placeholder for unit generation, just generating one red unit
		//no unit generated for map if unit has already been defeated
		if (isFirstMapVisit)
		{
			WalkAroundMapGenerator wamg = new WalkAroundMapGenerator();
			this.mapUnitList = wamg.GenerateRandomUnits(NameAll.TEAM_ID_WALK_AROUND_RED, numUnits: 1);
		}
		else
		{
			//load existing units from saved data
			this.mapUnitList = CalcCode.LoadWalkAroundPlayerUnitList(seed, timeInt, mapX, mapY, NameAll.WA_UNIT_SAVE_MAP_LIST);
		}
	}

    public int GetTeamSpawnPointsCount(int teamId)
    {
        int retValue = 0;
        foreach( SerializableVector3 sv in spList)
        {
            if ((int)sv.z == teamId)
                retValue += 1;
        }
        return retValue;
    }

    public int GetMaxZ()
    {
        int retValue = 10;
        foreach( SerializableVector3 t in tiles)
        {
            if (t.z > retValue)
                retValue = (int)t.z;
        }
        return retValue;
    }

    public int GetMaxX()
    {
        int retValue = 3;
        foreach (SerializableVector3 t in tiles)
        {
            if (t.x > retValue)
                retValue = (int)t.x;
        }
        return retValue + 1;
    }

    public int GetMaxY()
    {
        int retValue = 3;
        foreach (SerializableVector3 t in tiles)
        {
            if (t.y > retValue)
                retValue = (int)t.y;
        }
        return retValue + 1;
    }

}
