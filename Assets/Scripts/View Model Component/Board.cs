using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Board : MonoBehaviour 
{
	#region Fields / Properties
	[SerializeField] GameObject tilePrefab;
	public Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();
	public Point min { get { return _min; }}
	public Point max { get { return _max; }}
	Point _min;
	Point _max;
	Point[] dirs = new Point[4]
	{
		new Point(0, 1),
		new Point(0, -1),
		new Point(1, 0),
		new Point(-1, 0)
	};
	Color selectedTileColor = new Color(0, 1, 1, 1);
	Color defaultTileColor = new Color(1, 1, 1, 1);
    Color highlightTileColor = new Color(0, 1, 1, 1);
	Color exitMapTileColor = new Color(1, 1, 0, 1);

    [SerializeField]
    GameObject groundCrystalPrefab;
    private Dictionary<Tile, GameObject> groundObjectDict = new Dictionary<Tile, GameObject>(); //key is maptileindex, value is the type of object it is
	private Dictionary<Tile, int> groundTileDict = new Dictionary<Tile, int>(); //this and groundObjectDict duplicate each other, this stores type
	readonly int GROUND_TILE = 1;

	//int proximityValue = 5;
	Transform tileHolder;//holds tiles

	public bool isGridworldBoardSet = false; //so gridworld agent stuff doesn't start until board is ready

	public List<SerializableVector3> spawnList; //spawn points and teamID that can spawn there //(point.x,point.y, teamId) fucking serialization

	#endregion

	#region Public
	public void Load (LevelData data, int renderMode=0)
	{
		//loads the leveldata into physical tiles. The tiles themselves are placed in t.Load (Tile.Load) with the Match() function;
		tiles = new Dictionary<Point, Tile>();
		_min = new Point(int.MaxValue, int.MaxValue);
		_max = new Point(int.MinValue, int.MinValue);
		if( renderMode != NameAll.PP_RENDER_NONE) {
			tileHolder = new GameObject("TileHolder").transform; //holds tiles
			tileHolder.SetParent(transform);
		}

		Tile t;
		for (int i = 0; i < data.tiles.Count; ++i)
		{
			
			if (renderMode != NameAll.PP_RENDER_NONE)
			{
				GameObject instance = Instantiate(tilePrefab) as GameObject;
				instance.transform.SetParent(tileHolder); //tileHolder as parent
				t = instance.GetComponent<Tile>();
			}
			else
			{
				Debug.Log("ERROR INC, TILES INHERIT FROM MONO BEHAVIOR SO NEED TO DO IT DIFFERENTLY ");
				t = new Tile();
			}
			t.Load(data.tiles[i], renderMode);
			tiles.Add(t.pos, t);
			if (data.tileTypeList == null)
				t.TileType = NameAll.TILE_TYPE_DEFAULT;
			else
				t.TileType = data.tileTypeList[i];
			_min.x = Mathf.Min(_min.x, t.pos.x);
			_min.y = Mathf.Min(_min.y, t.pos.y);
			_max.x = Mathf.Max(_max.x, t.pos.x);
			_max.y = Mathf.Max(_max.y, t.pos.y);
		}
	}

    public Tile GetTile(int x, int y)
    {
        Point p = new Point(x, y);
        return tiles.ContainsKey(p) ? tiles[p] : null;
    }

    //called in CalculationProjectile to see if projectile is hindered crossing this tile
    public Tile GetTileCollision(float x, float y, float z, Tile startTile, float unitHeight = 4.0f)
    {
        Tile tempTile = GetTile((int)Mathf.Round(x), (int)Mathf.Round(y));
        if(tempTile != null && tempTile != startTile)
        {
            if (z <= tempTile.height)
                return tempTile;
            else if(tempTile.UnitId != NameAll.NULL_UNIT_ID && z <= tempTile.height + unitHeight)
            {
                return tempTile;
            }
        }
        return null;
    }

    public Tile GetTile (Point p)
	{
		return tiles.ContainsKey(p) ? tiles[p] : null;
	}

    public Tile GetTile(Tile t)
    {
        return tiles.ContainsKey(t.pos) ? tiles[t.pos] : null;
    }

    public Tile GetTile(PlayerUnit target)
    {
        Point p = new Point();
        p.x = target.TileX;
        p.y = target.TileY;
        return GetTile(p);
    }

    public List<Tile> Search(Tile start, Func<Tile, Tile, bool> addTile)
    {
        List<Tile> retValue = new List<Tile>();
        retValue.Add(start);

        ClearSearch();
        Queue<Tile> checkNext = new Queue<Tile>();
        Queue<Tile> checkNow = new Queue<Tile>();

        start.distance = 0;
        checkNow.Enqueue(start);

        while (checkNow.Count > 0)
        {
            Tile t = checkNow.Dequeue();
            for (int i = 0; i < 4; ++i)
            {
                Tile next = GetTile(t.pos + dirs[i]);
                if (next == null || next.distance <= t.distance + 1)
                    continue;

                if (addTile(t, next))
                {
                    next.distance = t.distance + 1;
                    next.prev = t;
                    checkNext.Enqueue(next);
                    retValue.Add(next);
                }
            }

            if (checkNow.Count == 0)
                SwapReference(ref checkNow, ref checkNext);
        }

        return retValue;
    }

    //Combat uses search
    //WalkAroundMode has multiple units so needs distanceDict and prevDict for each unitId
    public List<Tile> SearchWalkAround(Tile start, int unitId, Func<Tile, Tile, bool> addTile)
    {
        List<Tile> retValue = new List<Tile>();
        retValue.Add(start);
        //Debug.Log("reaching here " + unitId);
        ClearSearchWalkAround(unitId);
        Queue<Tile> checkNext = new Queue<Tile>();
        Queue<Tile> checkNow = new Queue<Tile>();

        //start.distance = 0;
        start.distanceDict[unitId] = 0;
        checkNow.Enqueue(start);

        while (checkNow.Count > 0)
        {
            Tile t = checkNow.Dequeue();
            for (int i = 0; i < 4; ++i)
            {
                Tile next = GetTile(t.pos + dirs[i]);
                //if (next == null || next.distance <= t.distance + 1)
                //    continue;
                //continue if tile does not exist or if the tile has already been added to the distanceDict
                if (next == null )
                    continue;
                if (next.distanceDict.ContainsKey(unitId) && t.distanceDict.ContainsKey(unitId) && next.distanceDict[unitId] <= t.distanceDict[unitId] + 1)
                    continue;

                if (addTile(t, next))
                {
                    //next.distance = t.distance + 1;
                    //next.prev = t;
                    next.distanceDict[unitId] = t.distanceDict[unitId] + 1;
                    next.prevDict[unitId] = t;
                    checkNext.Enqueue(next);
                    retValue.Add(next);
                }
            }

            if (checkNow.Count == 0)
                SwapReference(ref checkNow, ref checkNext);
        }

        return retValue;
    }

    public void SelectTiles (List<Tile> tiles)
	{
		for (int i = tiles.Count - 1; i >= 0; --i)
        {
            //tiles[i].GetComponent<Renderer>().material.SetColor("_Color", selectedTileColor);
            tiles[i].HighlightTile(0);
        }
			
	}

	public void DeSelectTiles (List<Tile> tiles)
	{
		for (int i = tiles.Count - 1; i >= 0; --i)
        {
            //tiles[i].GetComponent<Renderer>().material.SetColor("_Color", defaultTileColor);
            tiles[i].RevertTile();
        }
			
	}

    public void UpdatePlayerUnitTile(PlayerUnit pu, Tile newTile)
    {
        Point p = new Point(pu.TileX, pu.TileY);
        GetTile(p).UnitId = NameAll.NULL_UNIT_ID;
        GetTile(newTile.pos).UnitId = pu.TurnOrder;
    }

    public void UpdatePlayerUnitTileSwap(PlayerUnit pu, Tile newTile)
    {
        //Point p = new Point(pu.TileX, pu.TileY); //this tile has already been updated with the actor who swapped in
        //GetTile(p).UnitId = NameAll.NULL_UNIT_ID;
        GetTile(newTile.pos).UnitId = pu.TurnOrder;
    }

    //called in combatState to highlight certain tiles
    public void HighlightTile(Point p, int teamId, bool doHighlight)
    {
        Tile t = GetTile(p);
        if (t != null)
        {
            if (doHighlight)
            {
                //t.GetComponent<Renderer>().material.SetColor("_Color", highlightTileColor);
                //Debug.Log("highlighting tile");
                t.HighlightTile(teamId);
            }
            else
            {
                //t.GetComponent<Renderer>().material.SetColor("_Color", defaultTileColor);
                t.RevertTile();
            }
        }
    }

	public void UnhighlightTile(PlayerUnit pu)
	{
		Tile t = GetTile(pu);
		if (t != null)
		{
			t.RevertTile();
		}
	}

	//get reflect tile
	public Tile GetReflectTile(PlayerUnit actor, PlayerUnit target)
    {
        int cast_x = actor.TileX;
        int cast_y = actor.TileY;
        int target_x = target.TileX;
        int target_y = target.TileY;

        int x1, y1;
        if (cast_x <= target_x)
        {
            x1 = target_x + Math.Abs(target_x - cast_x);
        }
        else {
            x1 = target_x - Math.Abs(target_x - cast_x);
        }
        if (cast_y <= target_y)
        {
            y1 = target_y + Math.Abs(target_y - cast_y);
        }
        else {
            y1 = target_y - Math.Abs(target_y - cast_y);
        }
        Point p = new Point(x1, y1);
        return GetTile(p);
    }

    //find the tile furtherst away from the nearest enemey
    //in future can be used to find tile furthest away from all enemies
    public Point GetFarthestPoint( List<Tile> moveOptions, Tile nearestFoe, Tile currentTile )
    {
        Point retValue = currentTile.pos;
        int z1 = moveOptions.Count;
        int zMax = 0;
        for( int i = 0; i < z1; i++)
        {
            int z2 = Math.Abs(nearestFoe.pos.x - moveOptions[i].pos.x) + Math.Abs(nearestFoe.pos.y - moveOptions[i].pos.y);
            if( z2 > zMax)
            {
                retValue = moveOptions[i].pos;
            }
        }

        return retValue;
    }

    //get mimetargettile, called in CalcResolveAction
    public Tile GetMimeTargetTile(PlayerUnit actor, PlayerUnit mime, Tile targetTile)
    {
        Tile actorTile = GetTile(actor);
        Tile mimeTile = GetTile(mime);

        //targeting self, retur the mime coordinates
        if ( actorTile == targetTile)
        {
            return mimeTile;
        }

        int forwardOffset;
        int sideOffset;
        int xOffset = Mathf.Abs(targetTile.pos.x - actorTile.pos.x);
        int yOffset = Mathf.Abs(targetTile.pos.y - actorTile.pos.y);
        bool isRight; //is sideOffset to the right or left of the actor. if dead on, this doesn't matter (just adding 0)
        int xMimeChange = 0; //added at the end to get the new point
        int yMimeChange = 0; //added at the end to get the new point

        //gets the direction the actor will be facing
        Directions attackDir = actorTile.GetDirectionAttack(targetTile);
        
        if ( attackDir == Directions.East )
        {
            //side offset is >= N/S offset, thus the forward offset is the xOffset
            forwardOffset = xOffset;
            sideOffset = yOffset;
            if (targetTile.pos.y < actorTile.pos.y)
                isRight = true;
            else
                isRight = false;
        }
        else if (attackDir == Directions.West)
        {
            //side offset is >= N/S offset, thus the forward offset is the xOffset
            forwardOffset = xOffset;
            sideOffset = yOffset;
            if (targetTile.pos.y > actorTile.pos.y)
                isRight = true;
            else
                isRight = false;
        }
        else if (attackDir == Directions.North)
        {
            //side offset is < N/S offset, thus the forward offset is the y offset
            forwardOffset = yOffset;
            sideOffset = xOffset;
            if (targetTile.pos.x > actorTile.pos.x)
                isRight = true;
            else
                isRight = false;
        }
        else //SOUTH
        {
            //side offset is < N/S offset, thus the forward offset is the y offset
            forwardOffset = yOffset;
            sideOffset = xOffset;
            if (targetTile.pos.x < actorTile.pos.x)
                isRight = true;
            else
                isRight = false;
        }

        if (mime.Dir == Directions.East)
        {
            xMimeChange = forwardOffset;
            if (isRight)
                yMimeChange = sideOffset * -1; //off to the right is DOWN the y axis
            else
                yMimeChange = sideOffset;
            Debug.Log("E " + forwardOffset + " " + sideOffset);
        }
        else if (mime.Dir == Directions.West)
        {
            xMimeChange = forwardOffset * -1;
            if (isRight)
                yMimeChange = sideOffset; //off to the right is UP the y axis
            else
                yMimeChange = sideOffset * -1;
            Debug.Log("W " + forwardOffset + " " + sideOffset);
        }
        else if (mime.Dir == Directions.North)
        {
            yMimeChange = forwardOffset;
            if (isRight)
                xMimeChange = sideOffset; //off to the right is UP the x axis
            else
                xMimeChange = sideOffset * -1;
            Debug.Log("N " + forwardOffset + " " + sideOffset);
        }
        else //south
        {
            yMimeChange = forwardOffset * -1;
            if (isRight)
                xMimeChange = sideOffset * -1; //off to the right is DOWN the x axis
            else
                xMimeChange = sideOffset;
            Debug.Log("S " + forwardOffset + " " + sideOffset);
        }
    
        Point p = new Point(mimeTile.pos.x + xMimeChange, mimeTile.pos.y + yMimeChange);
        return GetTile(p); //can return null, null check done in CalcResolveAction
    }

    public void SetTilePickUp(int tileX, int tileY, bool isEnable, int renderMode, int pickUpId = 1)
    {
        Tile t = GetTile(tileX, tileY);
        if(isEnable)
        {
            t.PickUpId = pickUpId;   
        }
        else
        {
            t.PickUpId = 0;
        }

        if (pickUpId == 1)
        {
            SetPickUpObject(t, isEnable, renderMode);
        }
    }

    public void DisableUnit(PlayerUnit pu)
    {
        GetTile(pu).UnitId = NameAll.NULL_UNIT_ID;
    }

    void SetPickUpObject(Tile t, bool isEnable, int renderMode)
    {
        if(isEnable)
        {
			if(renderMode != NameAll.PP_RENDER_NONE)
			{
				GameObject go = Instantiate(groundCrystalPrefab) as GameObject;
				Vector3 vec = t.transform.position;
				vec += new Vector3(0, (vec.y + 0.3f), 0);
				go.transform.position = vec;
				groundObjectDict.Add(t, go);
			}
            
			groundTileDict.Add(t, GROUND_TILE);
        }
        else
        {
			if (renderMode != NameAll.PP_RENDER_NONE)
			{
				Destroy(groundObjectDict[t]);
				groundObjectDict.Remove(t);
			}
			groundTileDict.Remove(t);
		}
    }

	//move a tile pick up to a specific tile
	void MoveTilePickUp(Tile t, GameObject go)
	{
		Vector3 vec = t.transform.position;
		vec += new Vector3(0, (vec.y + 0.3f), 0);
		go.transform.position = vec;
	}


	public void ResetCrystal()
	{
		int x = UnityEngine.Random.Range(0, this.max.x);
		int y = UnityEngine.Random.Range(0, this.max.y);
		while (this.IsCrystalOnTile(x, y))
		{
			x = UnityEngine.Random.Range(0, this.max.x);
			y = UnityEngine.Random.Range(0, this.max.y);
		}
		this.MoveCrystal(x, y);
	}

	//called in Gridworld, moves first crystal found to the new position
		//yes this is obviously a stupid way to do it
	void MoveCrystal(int newX, int newY)
	{

		Tile t = this.GetTile(newX, newY);
		List<Tile> tileKeys = new List<Tile>(groundObjectDict.Keys);
		foreach (Tile tKey in tileKeys)
		{
			//if (tKey.pos.x == t.pos.x && tKey.pos.y == t.pos.y) //don't need to move starting on same spot
			//	break;

			//get and move gameobject
			GameObject go = groundObjectDict[tKey];
			groundObjectDict[t] = go;
			Vector3 vec = t.transform.position;
			vec += new Vector3(0, (vec.y + 0.3f), 0);
			go.transform.position = vec;
			groundObjectDict.Remove(tKey);
			break;
		}
	
	}


    //called in CombatMoveSequenceState for WindsOfFate, returns a random tile that doesn't have a unit on it
    public Point GetRandomPoint(Point p)
    {
        //Debug.Log("in get random point");
        var tempList = RandomValues(this.tiles).ToList();
        Tile t = tempList[0];

        if (t.UnitId == NameAll.NULL_UNIT_ID)
        {
            return t.pos;
        }

        return p; //defaults to entered value unless new one is found
    }

    //called in WalkAroundMainState for WindsOfFate, returns a random tile that doesn't have a unit on it
    public Tile GetRandomTile(Tile t)
    {
        //Debug.Log("in get random point");
        var tempList = RandomValues(this.tiles).ToList();
        Tile t_new = tempList[0];

        if (t_new.UnitId == NameAll.NULL_UNIT_ID)
        {
            return t_new;
        }

        return t; //defaults to entered value unless new one is found
    }

    //grabs a random entry for a dictionary
    public IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
    {
        System.Random rand = new System.Random();
        List<TValue> values = Enumerable.ToList(dict.Values);
        int size = dict.Count;
        yield return values[rand.Next(size)];
        //while (true)
        //{
        //    yield return values[rand.Next(size)];
        //}
    }

    #endregion

    #region Private
    void ClearSearch ()
	{
		foreach (Tile t in tiles.Values)
		{
			t.prev = null;
			t.distance = int.MaxValue;
		}
	}

    void ClearSearchWalkAround(int unitId)
    {
        foreach (Tile t in tiles.Values)
        {
            //t.prev = null;
            //t.distance = int.MaxValue;
            t.prevDict.Remove(unitId);
            t.distanceDict.Remove(unitId);
        }
    }

	//reset the board when the level is reset
	public void ResetBoard(int renderMode)
	{
		ResetGroundDict(renderMode);
		ClearUnitId();
	}

	void ResetGroundDict(int renderMode)
	{
		if(renderMode != NameAll.PP_RENDER_NONE)
		{
			foreach (var entry in groundObjectDict)
			{
				Destroy(entry.Value); //have to destroy any ground objects
			}
		}
		groundObjectDict = new Dictionary<Tile, GameObject>();
		groundTileDict = new Dictionary<Tile, int>();
	}

	//clears all unitIds, like for in Duel RL when board is reset
	public void ClearUnitId()
	{
		foreach (Tile t in tiles.Values)
		{
			t.UnitId = NameAll.NULL_UNIT_ID;
		}
	}



	void SwapReference (ref Queue<Tile> a, ref Queue<Tile> b)
	{
		Queue<Tile> temp = a;
		a = b;
		b = temp;
	}
	#endregion

	

	//is unit on tile that allows you to move to different map, called in WA Mode
	public bool IsMapMove(PlayerUnit pu)
	{
		if (GetTile(pu).TileType == NameAll.TILE_TYPE_EXIT_MAP)
			return true;

		return false;
	}

	//check if there is a crystal on the tile. used in gridworld for placing unit after placing crystal
	public bool IsCrystalOnTile(Tile t)
	{
		if (groundTileDict.ContainsKey(t) && groundTileDict[t] == GROUND_TILE)
			return true;
		//if (groundObjectDict.ContainsKey(t))
		//	return true;
		return false;
	}
	//check if there is a crystal on the tile. used in gridworld for placing unit after placing crystal
	public bool IsCrystalOnTile(Point p)
	{
		Tile t = this.GetTile(p);
		if (groundTileDict.ContainsKey(t) && groundTileDict[t] == GROUND_TILE)
			return true;
		//if (groundObjectDict.ContainsKey(t))
		//	return true;
		return false;
	}
	//check if there is a crystal on the tile. used in gridworld for placing unit after placing crystal
	public bool IsCrystalOnTile(int x, int y)
	{
		Tile t = this.GetTile(x, y);
		if (groundTileDict.ContainsKey(t) && groundTileDict[t] == GROUND_TILE)
			return true;
		//if (groundObjectDict.ContainsKey(t))
		//	return true;
		return false;
	}

	//find gridworld tile that has the goal and convert it to index of that tile
		//0,0 is 0; move up x-axis increases index by 1, moving up y axis increases index by 4
	public int GetGridworldGoal()
	{

		int retValue = NameAll.NULL_INT;
		foreach (var entry in groundObjectDict)
		{
			//in gridworld should only be one object in this
			retValue = entry.Key.pos.x + entry.Key.pos.y * (this.max.x + 1);
			return retValue;
		}

		return retValue;
	}

	//move gridworld tile to new location and return index of that new tile location
		//assume player is on this tile, so moving to any other random tile
	public int ResetGridworldGoal()
	{
		while (true)
		{
			int x = UnityEngine.Random.Range(0, this.max.x+1);
			int y = UnityEngine.Random.Range(0, this.max.y+1);
			Tile t = this.GetTile(x, y);
			if(groundObjectDict.ContainsKey(t))
			{
				//continue loop
			}
			else
			{
				this.MoveGridworldGoal(t);
				return t.pos.x + t.pos.y * (this.max.x + 1);
			}
			
		}
		
	}

	//move gridworld goal by updating groundObjectDict and moving the GameObject
		//only item in groundObjectDict should be the crystal
		//updating the tile in the dictionary it is stored at
	void MoveGridworldGoal(Tile t)
	{
		GameObject go;
		foreach (var entry in groundObjectDict)
		{
			//in gridworld should only be one object in this
			go = entry.Value;
			groundObjectDict.Remove(entry.Key);
			groundObjectDict[t] = go;
			MoveTilePickUp(t, go);
			return;
		}
	}


	#region Outputs for Reinforcement Learning (RL) Observations
	//incomplete code that I don't think I ever implemented
	//public Dictionary<string,int> GetRLBoardInfo()
	//   {
	//       Dictionary<string, int> retDict = new Dictionary<string, int>();
	//       retDict["x_min"] = this.min.x;
	//       retDict["x_max"] = this.max.x;
	//       retDict["y_min"] = this.min.y;
	//       retDict["y_max"] = this.max.y;
	//       retDict["pickup_crystal_id"] = NameAll.GROUND_CRYSTAL_ID;
	//       return retDict;
	//   }

	//public void GetRLInfoTerrain()
	//{
	//    Debug.Log("Terrain Dict not implemented");
	//}

	//returns panels in order of implementation, use GetBoardInfo min/max x and y to reconstruct the board
	//public Dictionary<Point, int> GetRLInfoBoardHeight()
	//{
	//    Dictionary<Point, int> retDict = new Dictionary<Point, int>();
	//    foreach (var entry in tiles)
	//    {
	//        retDict.Add(entry.Key, entry.Value.height);
	//    }

	//    return retDict;
	//}

	//things on board that can change
	//for now just crystals, PlayerUnits done through theirs tring
	//in future can do things like terrain changes, standable changes, traversable changes etc
	//public Dictionary<Point,int> GetRLInfoBoardStep()
	//{
	//    Dictionary<Point, int> retDict = new Dictionary<Point, int>();
	//    foreach(var entry in groundObjectDict)
	//    {
	//        retDict[new Point(entry.Key.pos.x, entry.Key.pos.y)] = NameAll.GROUND_CRYSTAL_ID;
	//    }


	//    return retDict;
	//}
	#endregion

	//DEPRECATED functions for checkign for CombatStart in WalkAround mode, now done through PUO and PlayerManager

	//checks if units that are hostile to each other are in proximity to each other thus triggering combat
	//if true sends notification to WalkAroundMainSTate
	//uses SearchWalkAround and a dummy id (unitId + 1000) to get list of tiles within proximity range
	//uses that list of tiles to see if combat check

	//  public void WalkAroundCheckProximityCombatTrigger(Tile t, int unitId)
	//  {
	//      SetCombatProximityValue();
	//      Dictionary<int, int> teamIdDict = new Dictionary<int, int>();
	//      var tileList = SearchWalkAround(t, unitId + 1000, ExpandSearchBase);
	//Debug.Log("WalkAroundCheckProximityCombatTrigger 0... " + tileList.Count);
	//foreach (Tile ti in tileList)
	//      {
	//	Debug.Log("WalkAroundCheckProximityCombatTrigger 1..." + ti.GetTileSummary());
	//	if (ti.UnitId != NameAll.NULL_UNIT_ID && ti.UnitId != unitId)
	//          {
	//		Debug.Log("WalkAroundCheckProximityCombatTrigger 2..." + ti.UnitId + " " + unitId );
	//		teamIdDict[PlayerManager.Instance.GetPlayerUnit(ti.UnitId).TeamId] = 1;
	//              if (IsTeamIdHostile(teamIdDict))
	//              {
	//			Debug.Log("WalkAroundCheckProximityCombatTrigger 3...");
	//			SendCombatTrigger();
	//                  break;
	//              }
	//          }
	//      }
	//  }

	//called in WalkAroundCheckProximityCombatTrigger as an argument for SearchWalkAround
	//
	//  protected bool ExpandSearchBase(Tile from, Tile to)
	//  {
	//return (from.distance + 1) <= 3; //this.proximityValue;
	//  }

	//used to check if hostile units are too close to each other. for now just a constant, in future make it unit/context dependent
	//is Set instead of Get due to how ExpandSearchBase is called
	//void SetCombatProximityValue()
	//{
	//    this.proximityValue = 1;
	//}

	////used to check if units that are close to each other are in fact hostile
	////for now simply if on different teams then they are hostile
	//bool IsTeamIdHostile(Dictionary<int,int> teamIdDict)
	//{

	//    if (teamIdDict.Count() >= 1)
	//        return true;

	//    return false;
	//}

	//const string CombatStart = "PlayerManager.CombatStart";

	//  void SendCombatTrigger()
	//  {
	//Debug.Log("sending combatTrigger from board");
	//      this.PostNotification(CombatStart, null);
	//  }
}