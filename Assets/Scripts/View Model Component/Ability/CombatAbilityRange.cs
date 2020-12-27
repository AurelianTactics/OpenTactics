using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatAbilityRange 
{
    //public int horizontal = 1;
    //public int vertical = int.MaxValue;
    //public virtual bool positionOriented { get { return true; } }
    //public virtual bool directionOriented { get { return false; } }
    //protected Unit unit { get { return GetComponentInParent<Unit>(); } }

    readonly int SELF_TARGET = -1;
    readonly int CASTER_IMMUNE = 1;
    readonly int SPELL_RANGE_LINE = 102;

    public bool isSearchTypeAbility;
    public int horizontalMin;
    public int horizontalMax;
    public int vertical;
    public int tileHeight;
    Tile startSearchTile; //used in setting minimum horizontal search

    public bool directionOriented { get; set; }
    public bool positionOriented { get; set; }

    public List<Tile> GetTilesInRange(Board board, PlayerUnit actor, SpellName sn, Directions dir, Tile fakeActorTile = null)
    {
        List<Tile> retValue = new List<Tile>(); //Debug.Log("am I here? " + sn.RangeXYMin );
        //List<Tile> tempTiles;
        Tile actorTile;
        if (fakeActorTile == null)
            actorTile = GetPlayerTile(board, actor);
        else
            actorTile = fakeActorTile;

        isSearchTypeAbility = true;
        SetSpellHorizontalVertical(actor,sn);
        //horizontalMin = GetSpellRange(sn.RangeXYMin, actor, sn);
        //horizontalMax = GetSpellRange(sn.RangeXYMax, actor, sn);
        //vertical = GetSpellRange(sn.RangeZ, actor, sn);
        tileHeight = GetTileHeight(board, actor, sn);
        directionOriented = false;
        positionOriented = true;

        if (sn.EffectXY == NameAll.SPELL_EFFECT_ALLIES || sn.EffectXY == NameAll.SPELL_EFFECT_ENEMIES)
            positionOriented = false;

        if( isSearchTypeAbility) //turned to false in SetSpellHorizontalVertical for certain combos
        {
            startSearchTile = board.GetTile(actor); //Debug.Log("am I here? " + sn.RangeXYMin + " " + horizontalMax);
            retValue = GetSearchTilesInRange(board, actorTile);
            //check for caster immune on targeting
            retValue = GetValidTiles(retValue, actor, sn);
        }
        else
        {
            if (sn.RangeXYMin == SELF_TARGET)
            {
                retValue.Add(actorTile); //always targets self, can move onto effect
            }
            else if (sn.EffectXY >= NameAll.SPELL_EFFECT_CONE_BASE && sn.EffectXY <= NameAll.SPELL_EFFECT_CONE_MAX)
            {
                //horizontalMax = GetSpellRange(NameAll.SPELL_EFFECT_CONE_BASE, actor, sn); //different case for cones
                retValue = GetConeTilesInRange(board, actorTile, dir, horizontalMax, vertical);
                directionOriented = true;
            }
            else if (sn.RangeXYMax == SPELL_RANGE_LINE)
            {
                //horizontalMax = GetSpellRange(SPELL_RANGE_LINE, actor, sn); //different case for lines
                retValue = GetLineTilesInRange(board, actorTile, dir, horizontalMax, vertical);
                directionOriented = true;
            }
            else if (sn.RangeXYMax == NameAll.SPELL_RANGE_SPEAR)
            {
                retValue = GetSpearTilesInRange(board, actorTile, horizontalMax, vertical);
                directionOriented = false; //only targets one tile, not an entire line
            }
            else if (sn.RangeZ <= NameAll.SPELL_RANGE_Z_HEIGHT_MAX && sn.RangeZ >= NameAll.SPELL_RANGE_Z_HEIGHT_BASE)
            {
                retValue = GetHeightAbilityTiles(board, actor, sn); //Debug.Log("am I here? " + sn.RangeXYMin + " " + horizontalMax);
            }
            else
            {
                startSearchTile = board.GetTile(actor);//Debug.Log("am I here? " + sn.RangeXYMin + " " + horizontalMax);
                retValue = GetSearchTilesInRange(board, actorTile);
                //check for caster immune on targeting
                retValue = GetValidTiles(retValue, actor, sn);
            }
        }
        
        //Debug.Log("CombatAbility Range retValue size is " + retValue.Count);
        return retValue;
    }

    //for now just doing caster immune check and removing the caster immune tile
    List<Tile> GetValidTiles(List<Tile> tileList, PlayerUnit actor, SpellName sn)
    {
        
        if( sn.CasterImmune == 1)
        {
            List<Tile> retValue = new List<Tile>();

            for (int i = 0; i < tileList.Count; i++)
            {
                //Debug.Log("cycling through valid tiles " + tileList[i].pos.x + "," + tileList[i].pos.y);
                if (tileList[i].UnitId != actor.TurnOrder)
                {
                    retValue.Add(tileList[i]);
                }
            }
            return retValue;
        }
        else
        {
            return tileList;
        }
        
    }

    List<Tile> GetSpearTilesInRange(Board board, Tile startTile, int horizontalRange, int verticalRange)
    {
        List<Tile> retValue = new List<Tile>();
        Point startPos = startTile.pos;
        //Point endPos;
        
        for( int i = -horizontalRange; i <= horizontalRange; i++)
        {
            for(int j = -horizontalRange; j <= horizontalRange; j++)
            {
                int z1 = Mathf.Abs(i);
                int z2 = Mathf.Abs(j);
                if (z1 == z2)
                    continue; //can't attack diagnolly
                else if (z1 + z2 > horizontalRange)
                    continue; //can't attack more than horizontalRange away
                else
                {
                    Tile t = board.GetTile(startPos.x + i, startPos.y + j);
                    if (t != null && Mathf.Abs(t.height - startTile.height) <= verticalRange)
                        retValue.Add(t);
                }
            }
        }
        return retValue;
    }

    List<Tile> GetLineTilesInRange(Board board, Tile startTile, Directions dir, int horizontalRange, int verticalRange)
    {
        Point startPos = startTile.pos;
        Point endPos;
        List<Tile> retValue = new List<Tile>();

        switch (dir)
        {
            case Directions.North:
                endPos = new Point(startPos.x, board.max.y);
                break;
            case Directions.East:
                endPos = new Point(board.max.x, startPos.y);
                break;
            case Directions.South:
                endPos = new Point(startPos.x, board.min.y);
                break;
            default: // West
                endPos = new Point(board.min.x, startPos.y);
                break;
        }

        int dist = 0;
        while (startPos != endPos)
        {
            if (startPos.x < endPos.x) startPos.x++;
            else if (startPos.x > endPos.x) startPos.x--;

            if (startPos.y < endPos.y) startPos.y++;
            else if (startPos.y > endPos.y) startPos.y--;

            Tile t = board.GetTile(startPos);
            if (t != null && Mathf.Abs(t.height - startTile.height) <= verticalRange)
                retValue.Add(t);

            dist++;
            if (dist >= horizontalRange)
                break;
        }

        return retValue;
    }

    List<Tile> GetConeTilesInRange(Board board, Tile startTile, Directions unitDir, int horizontalRange, int verticalRange)
    {
        Point pos = startTile.pos;
        List<Tile> retValue = new List<Tile>();
        int dir = (unitDir == Directions.North || unitDir == Directions.East) ? 1 : -1;
        int lateral = 1;

        if (unitDir == Directions.North || unitDir == Directions.South)
        {
            for (int y = 1; y <= horizontalRange; ++y)
            {
                int min = -(lateral / 2);
                int max = (lateral / 2);
                for (int x = min; x <= max; ++x)
                {
                    Point next = new Point(pos.x + x, pos.y + (y * dir));
                    Tile tile = board.GetTile(next);
                    if (ValidTile(tile, startTile, verticalRange))
                        retValue.Add(tile);
                }
                lateral += 2;
            }
        }
        else
        {
            for (int x = 1; x <= horizontalRange; ++x)
            {
                int min = -(lateral / 2);
                int max = (lateral / 2);
                for (int y = min; y <= max; ++y)
                {
                    Point next = new Point(pos.x + (x * dir), pos.y + y);
                    Tile tile = board.GetTile(next);
                    if (ValidTile(tile,startTile,verticalRange))
                        retValue.Add(tile);
                }
                lateral += 2;
            }
        }

        return retValue;
    }

    //used for abilities like a bow
    public List<Tile> GetHeightAbilityTiles(Board board, PlayerUnit actor, SpellName sn)
    {
        List<Tile> retValue = new List<Tile>();
        int heightFactor = sn.RangeZ - NameAll.SPELL_RANGE_Z_HEIGHT_BASE;
        
        Tile startTile = board.GetTile(actor);
        //horizontalMin is constant, horizontal max can vary based on the height
        int tempMax;
        int distance;
        foreach( KeyValuePair<Point,Tile> kvp in board.tiles)
        {
            tempMax = horizontalMax + (startTile.height - kvp.Value.height) / heightFactor; //Debug.Log(" tempMax is " + tempMax + " " + kvp.Value.height + "x,y " + kvp.Value.pos.x +"," + kvp.Value.pos.y);
            distance = MapTileManager.Instance.GetDistanceBetweenTiles(kvp.Value, startTile);
            if( distance >= horizontalMin && distance <= tempMax)
            {
                retValue.Add(kvp.Value);
            } 
        }
        return retValue;
    }

    bool ValidTile(Tile t, Tile startTile, int verticalRange)
    {
        return t != null && Mathf.Abs(t.height - startTile.height) <= verticalRange;
    }

    void SetSpellHorizontalVertical(PlayerUnit actor, SpellName sn)
    {
        //Debug.Log("why am I reaching here?");
        horizontalMin = sn.RangeXYMin;
        horizontalMax = sn.RangeXYMax;
        vertical = sn.RangeZ;

        if( sn.RangeXYMin == NameAll.SPELL_RANGE_MIN_SELF_TARGET)
        {
            isSearchTypeAbility = false;
            return;
        }

        if( sn.RangeXYMin == NameAll.SPELL_RANGE_MIN_WEAPON) //&& sn.RangeXYMax == NameAll.SPELL_RANGE_MAX_WEAPON; these two are coupled together
        {
            List<int> tempList = GetAttackSpellRange(actor.TurnOrder);
            horizontalMin = tempList[0]; //Debug.Log("min spell range is" + retValue);
            horizontalMax = tempList[1]; //Debug.Log("min spell range is" + retValue);
            vertical = tempList[2];
        }
        else if (sn.RangeXYMax == SPELL_RANGE_LINE)
        {
            isSearchTypeAbility = false;
            horizontalMax = sn.EffectXY - 100;
        }
        else if (sn.EffectXY >= NameAll.SPELL_EFFECT_CONE_BASE && sn.EffectXY <= NameAll.SPELL_EFFECT_CONE_MAX)
        {
            isSearchTypeAbility = false;
            horizontalMax = sn.EffectXY - NameAll.SPELL_EFFECT_CONE_BASE;
            horizontalMax = Mathf.Clamp(horizontalMax, 1, 9);
        }
        else if (sn.RangeXYMax == NameAll.SPELL_RANGE_MAX_MOVE)
        {
            horizontalMax = actor.StatTotalMove;
        }
        else if (sn.RangeXYMax == NameAll.SPELL_RANGE_SPEAR)
        {
            isSearchTypeAbility = false;
            horizontalMax = 2;
        }
        else if (sn.RangeXYMax == NameAll.SPELL_RANGE_MAX_THROW_ITEM)
        {
            //Debug.Log("am I here?" + spellRange);
            if (AbilityManager.Instance.IsInnateAbility(actor.ClassId, NameAll.SUPPORT_THROW_ITEM, NameAll.ABILITY_SLOT_SUPPORT)
                || PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.SUPPORT_THROW_ITEM, NameAll.ABILITY_SLOT_SUPPORT))
            {
                horizontalMax = 4;
            }
            else
            {
                horizontalMax = 1;
            }
        }

        if (sn.RangeZ <= NameAll.SPELL_RANGE_Z_HEIGHT_MAX && sn.RangeZ >= NameAll.SPELL_RANGE_Z_HEIGHT_BASE)
        {
            isSearchTypeAbility = false;
        }
    }

    //int GetSpellRange(int spellRange, PlayerUnit actor, SpellName sn)
    //{
    //    int retValue = spellRange; //Debug.Log("am I here?" + spellRange);

    //    if( retValue < 100 && retValue >= 0)
    //    {
    //        return retValue;
    //    }

    //    if (spellRange == NameAll.SPELL_RANGE_MIN_WEAPON)
    //    {
    //        List<int> tempList = GetAttackSpellRange(actor.TurnOrder);
    //        retValue = tempList[0]; //Debug.Log("min spell range is" + retValue);
    //    }
    //    else if (spellRange == NameAll.SPELL_RANGE_MAX_WEAPON)
    //    {
    //        List<int> tempList = GetAttackSpellRange(actor.TurnOrder);
    //        retValue = tempList[1];
    //    }
    //    else if (spellRange == SPELL_RANGE_LINE)
    //    {
    //        retValue = sn.EffectXY - 100;
    //        retValue = Mathf.Clamp(retValue, 2, 8);
    //    }
    //    else if( spellRange == NameAll.SPELL_EFFECT_CONE_BASE)
    //    {
    //        retValue = sn.EffectXY - NameAll.SPELL_EFFECT_CONE_BASE;
    //        retValue = Mathf.Clamp(retValue, 1, 9);
    //    }
    //    else if (spellRange == NameAll.SPELL_RANGE_MAX_MOVE)
    //    {
    //        retValue = actor.StatTotalMove;
    //    }
    //    else if (spellRange == NameAll.SPELL_RANGE_Z_WEAPON)
    //    {
    //        List<int> tempList = GetAttackSpellRange(actor.TurnOrder);
    //        retValue = tempList[2];
    //    }
    //    else if (spellRange == NameAll.SPELL_RANGE_MIN_SELF_TARGET)
    //    {
    //        retValue = 0;
    //    }
    //    else if( spellRange == NameAll.SPELL_RANGE_MAX_THROW_ITEM)
    //    {
    //        //Debug.Log("am I here?" + spellRange);
    //        if (  AbilityManager.Instance.IsInnateAbility(actor.ClassId,NameAll.SUPPORT_THROW_ITEM,NameAll.ABILITY_SLOT_SUPPORT) 
    //            || PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.SUPPORT_THROW_ITEM, NameAll.ABILITY_SLOT_SUPPORT))
    //        {
    //            retValue = 4;
    //        }
    //        else
    //        {
    //            retValue = 1;
    //        }
    //    }
    //    else if (spellRange == NameAll.SPELL_RANGE_SPEAR)
    //    {
    //        retValue = 2;
    //    }
    //    return retValue;
    //}

    List<int> GetAttackSpellRange(int actorId)
    {
        //gets the attack spell name based on the actor
        PlayerUnit actor = PlayerManager.Instance.GetPlayerUnit(actorId);
        int weapon_type = ItemManager.Instance.GetItemType(actor.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON);
        SpellName sn = SpellManager.Instance.GetSpellAttackByWeaponType(weapon_type, actor.ClassId); //gets the SpellName
        List<int> tempList = new List<int>();
        tempList.Add(sn.RangeXYMin); //Debug.Log("adf " + sn.RangeXYMin);
        tempList.Add(sn.RangeXYMax); //Debug.Log("adf " + sn.RangeXYMax);
        tempList.Add(sn.RangeZ);
        //Debug.Log("getting attack spell, range is " + sn.RangeXYMax + " " + sn.GetBaseQ());
        return tempList;
    }

    int GetTileHeight(Board board, PlayerUnit actor, SpellName sn)
    {
        int retValue = GetPlayerTile(board, actor).height;

        return retValue;
    }

    //get the tile the player is on
    Tile GetPlayerTile(Board board, PlayerUnit actor)
    {
        Point p = new Point(actor.TileX, actor.TileY);
        return board.GetTile(p);
    }

    //his functions
    List<Tile> GetSearchTilesInRange(Board board, Tile startTile)
    {
        //Debug.Log("searching for tiles " + horizontalMin + " " + horizontalMax);
        if( horizontalMin > 0)
        {
            List<Tile> retValue = board.Search(startTile, ExpandSearch);
            retValue = RemoveHorizontalMinTiles(startTile,retValue,horizontalMin);
            //gets all the tiles then goes through them and filters out the ones less than horizontal min
            //retValue.RemoveAt(0); //board auto adds startTile, need to take this out if horizontal min > 0
            return retValue;
        }
        else
        {
            return board.Search(startTile, ExpandSearch);
        }
        
    }

    public bool ExpandSearch(Tile from, Tile to)
    {
        return (from.distance + 1) <= horizontalMax && Mathf.Abs(to.height - tileHeight) <= vertical;
    }

    public List<Tile> RemoveHorizontalMinTiles(Tile startTile, List<Tile> tileList, int zMin)
    {
        List<Tile> retValue = new List<Tile>();
        int count = tileList.Count;
        for( int i = 0; i < count; i++)
        {
            if (MapTileManager.Instance.GetDistanceBetweenTiles(tileList[i], startTile) >= zMin)
                retValue.Add(tileList[i]);
        }
        return retValue;
    }

    
    //public bool ExpandSearchHorizontalMin(Tile from, Tile to)
    //{
    //    Debug.Log("asdf " + MapTileManager.Instance.GetDistanceBetweenTiles(to, startSearchTile) + " " + horizontalMin);
    //    return (from.distance + 1) <= horizontalMax && Mathf.Abs(to.height - tileHeight) <= vertical && MapTileManager.Instance.GetDistanceBetweenTiles(to, startSearchTile) >= horizontalMin;
    //}

    //In WalkAroundMode
    //called from PlayerManager 
    //finds targets for the spell and returns a 'lock list' of units to be locked so they can't move or do an action until this action resolves on them
    public List<int> GetWalkAroundTargets(Board board, WalkAroundActionObject waao)
    {
        List<int> retValue = new List<int>();
        List<Tile> tempTiles = GetTilesInRange(board, waao.turn.actor, waao.turn.spellName, waao.turn.actor.Dir);
        foreach( Tile t in tempTiles)
        {
            if( t.UnitId != NameAll.NULL_UNIT_ID)
                retValue.Add(t.UnitId);
        }
        return retValue;
    }

    //Called from PlayerManager GetWalkAroundActionObjectValid
    //sees if the action is valid with the move or not
    public WalkAroundActionObject IsAbilityTargetInRange(Board board, WalkAroundActionObject waao, PlayerUnit actor)
    {
        //some abilities have to search over all directions, others only one direction
        List<Directions> dirList = new List<Directions>();
        if( (waao.turn.spellName.EffectXY >= NameAll.SPELL_EFFECT_CONE_BASE && waao.turn.spellName.EffectXY <= NameAll.SPELL_EFFECT_CONE_MAX) 
            || waao.turn.spellName.RangeXYMax == SPELL_RANGE_LINE)
        {
            dirList.Add(Directions.East);
            dirList.Add(Directions.South);
            dirList.Add(Directions.West);
            dirList.Add(Directions.North);
        }
        else
        {
            dirList.Add(actor.Dir);
        }

        Tile checkTile;
        if (waao.turn.isWalkAroundMoveFirst)
            checkTile = waao.moveTile;
        else
            checkTile = board.GetTile(actor);

        foreach (Directions dir in dirList)
        {
            var tiles = GetTilesInRange(board, waao.turn.actor, waao.turn.spellName, dir, checkTile);
            if (tiles.Contains(waao.turn.targetTile) && waao.turn.targetTile.UnitId == waao.turn.targetUnitId)
            {
                return waao;
            }
        }
        
        return null;
    }
}
