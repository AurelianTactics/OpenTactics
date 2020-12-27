using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CombatAbilityArea {

    //uses sn.EffectZ (vertical)
    //uses sn.EffectXY (horizontal)

    public int horizontal;
    public int vertical;
    public int tileHeight;

    readonly int SPELL_LINE_MIN = 102;
    readonly int SPELL_LINE_MAX = 108;
    readonly int SPELL_CONE_MIN = 122;
    readonly int SPELL_CONE_MAX = 129;
    readonly int SPELL_EFFECT_ALLIES = 109;
    readonly int SPELL_EFFECT_ENEMIES = 110;
    readonly int SPELL_EFFECT_MATH_SKILL = 111;

    public List<Tile> GetTilesInArea(Board board, PlayerUnit actor, SpellName sn, Tile targetTile, Directions dir)
    {
        List<Tile> retValue = new List<Tile>();
        int spellEffect = sn.EffectXY;
        horizontal = GetSpellEffect(sn.EffectXY, actor, sn);
        vertical = sn.EffectZ; //1919 for unlimited range
        tileHeight = targetTile.height;

        if (spellEffect < 100)
        {
            retValue = GetSearchTilesInRange(board, targetTile); //Debug.Log("retValue size is " + retValue.Count);
        }
        else if (spellEffect >= SPELL_LINE_MIN && spellEffect <= SPELL_LINE_MAX) //straightLine
        {
            retValue = GetLineTilesInRange(board, board.GetTile(actor), dir, horizontal, vertical); //Debug.Log("dir is: " + dir.ToString());
        }
        else if (spellEffect >= SPELL_CONE_MIN && spellEffect <= SPELL_CONE_MAX) //cone
        {
            retValue = GetConeTilesInRange(board, board.GetTile(actor), dir, horizontal, vertical);
        }
        else if (spellEffect == SPELL_EFFECT_ALLIES)
        {
            retValue = GetAlliesTiles(board,actor,sn);
        }
        else if (spellEffect == SPELL_EFFECT_ENEMIES)
        {
            retValue = GetEnemiesTiles(board, actor, sn);
        }
        else if (spellEffect == SPELL_EFFECT_MATH_SKILL)
        {
            retValue = GetMathSkillTiles(board, sn);
        }

        retValue = GetValidTiles(retValue,actor,sn); //if their is a unit on the tile does it pass the test (casterImmune, allies/enemies)
        //Debug.Log("retValue size is " + retValue.Count);
        return retValue;

    }

    //his function
    List<Tile> GetSearchTilesInRange(Board board, Tile startTile)
    {
        return board.Search(startTile, ExpandSearch);
    }

    public bool ExpandSearch(Tile from, Tile to)
    {
        return (from.distance + 1) < horizontal && Mathf.Abs(to.height - tileHeight) <= vertical;
    }

    List<Tile> GetMathSkillTiles(Board board, SpellName sn)
    {
        List<Tile> retValue = new List<Tile>();
        var pul = PlayerManager.Instance.GetPlayerUnitList();
        foreach (PlayerUnit pu in pul)
        {
            if (IsHitByMathSkill(board, pu.TurnOrder, sn.SpellId))
            {
                Point p = new Point(pu.TileX, pu.TileY);
                retValue.Add(board.GetTile(p));
            }
        }
        return retValue;
    }

    bool IsHitByMathSkill(Board board, int unitId, int spellIndex)
    {
        int z1 = 0; int z2 = 3; Tile targetTile;

        if (spellIndex == NameAll.SPELL_INDEX_CT_3)
        {
            z1 = PlayerManager.Instance.GetPlayerUnit(unitId).CT;
            z2 = 3;
        }
        else if (spellIndex == NameAll.SPELL_INDEX_CT_4)
        {
            z1 = PlayerManager.Instance.GetPlayerUnit(unitId).CT;
            z2 = 4;
        }
        else if (spellIndex == NameAll.SPELL_INDEX_CT_5)
        {
            z1 = PlayerManager.Instance.GetPlayerUnit(unitId).CT;
            z2 = 5;
        }
        else if (spellIndex == NameAll.SPELL_INDEX_CT_PRIME)
        {
            z1 = PlayerManager.Instance.GetPlayerUnit(unitId).CT;
            z2 = 19;
        }
        else if (spellIndex == NameAll.SPELL_INDEX_LEVEL_3)
        {
            z1 = PlayerManager.Instance.GetPlayerUnit(unitId).Level;
            z2 = 3;
        }
        else if (spellIndex == NameAll.SPELL_INDEX_LEVEL_4)
        {
            z1 = PlayerManager.Instance.GetPlayerUnit(unitId).Level;
            z2 = 4;
        }
        else if (spellIndex == NameAll.SPELL_INDEX_LEVEL_5)
        {
            z1 = PlayerManager.Instance.GetPlayerUnit(unitId).Level;
            z2 = 5;
        }
        else if (spellIndex == NameAll.SPELL_INDEX_LEVEL_PRIME)
        {
            z1 = PlayerManager.Instance.GetPlayerUnit(unitId).Level;
            z2 = 19;
        }
        else if (spellIndex == NameAll.SPELL_INDEX_HEIGHT_3)
        {
            targetTile = PlayerManager.Instance.GetPlayerUnitTile(board, unitId);
            z2 = 3;
        }
        else if (spellIndex == NameAll.SPELL_INDEX_HEIGHT_4)
        {
            targetTile = PlayerManager.Instance.GetPlayerUnitTile(board, unitId);
            z2 = 4;
        }
        else if (spellIndex == NameAll.SPELL_INDEX_HEIGHT_5)
        {
            targetTile = PlayerManager.Instance.GetPlayerUnitTile(board, unitId);
            z2 = 5;
        }
        else if (spellIndex == NameAll.SPELL_INDEX_HEIGHT_PRIME)
        {
            targetTile = PlayerManager.Instance.GetPlayerUnitTile(board, unitId);
            z2 = 19;
        }

        if (z1 == 0)
        {
            return false;
        }

        if (z2 == 19)
        {
            return IsPrime(z1);
        }
        else
        {
            if (z1 % z2 == 0)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPrime(int number)
    {

        int boundary = (int)Math.Floor(Math.Sqrt(number));

        //check is elsewhere if (number == 0) return false;
        if (number == 1) return false;
        if (number == 2) return true;

        for (int i = 2; i <= boundary; ++i)
        {
            if (number % i == 0) return false;
        }

        return true;
    }

    List<Tile> GetAlliesTiles(Board board, PlayerUnit actor, SpellName sn)
    {
        List<Tile> retValue = new List<Tile>();
        int teamId = actor.GetCharmTeam();
        int actorId = actor.TurnOrder;

        List<PlayerUnit> zPuList = PlayerManager.Instance.GetAllUnitsByTeamId(teamId, true);
        foreach (PlayerUnit pu in zPuList)
        {
            Point p = new Point(pu.TileX, pu.TileY);
            retValue.Add(board.GetTile(p));
        }

        return retValue;
    }

    List<Tile> GetEnemiesTiles(Board board, PlayerUnit actor, SpellName sn)
    {
        List<Tile> retValue = new List<Tile>();
        int teamId = actor.GetCharmTeam();
        int actorId = actor.TurnOrder;

        List<PlayerUnit> zPuList = PlayerManager.Instance.GetAllUnitsByTeamId(teamId, false);
        foreach (PlayerUnit pu in zPuList)
        {
            Point p = new Point(pu.TileX, pu.TileY);
            retValue.Add(board.GetTile(p));
        }

        return retValue;
    }

    //checks if spell is caster immune and the caster is a target

    List<Tile> GetValidTiles( List<Tile> tileList, PlayerUnit actor, SpellName sn)
    {
        List<Tile> retValue = new List<Tile>();
        int targetTileId;
        int casterImmune = sn.CasterImmune;
        int actorId = actor.TurnOrder;
        int allyEnemy = sn.AlliesType; //Debug.Log("allies type is " + allyEnemy);
        int actorTeamId = actor.GetCharmTeam(); //Debug.Log("team Id is " + actorTeamId);

        for( int i = 0; i < tileList.Count; i++)
        {
            targetTileId = tileList[i].UnitId; //Debug.Log("cycling through valid tiles " + tileList[i].pos.x + "," + tileList[i].pos.y);
            if( targetTileId != NameAll.NULL_UNIT_ID)
            {
                if( IsValidTileCasterImmune(casterImmune,actorId,targetTileId)
                    && IsValidTileAllyEnemy(allyEnemy, actorTeamId, targetTileId) )
                {
                    retValue.Add(tileList[i]);
                }
            }
            else
            {
                retValue.Add(tileList[i]); //empty tile, can do other valid check down the line if you'd like
            }
            
        }
        return retValue;
    }

    bool IsValidTileCasterImmune(int casterImmune, int actorId, int targetId)
    {
        if(actorId == targetId && casterImmune == 1 )
        {
            return false;
        }
        return true;
    }

    bool IsValidTileAllyEnemy(int allyEnemy, int actorTeamId, int targetId)
    {
        if (allyEnemy == NameAll.ALLIES_TYPE_ANY) //hits everyone
        {
            return true;
        }
        else if( allyEnemy == NameAll.ALLIES_TYPE_ALLIES) //allies only
        {
            int targetTeamId = PlayerManager.Instance.GetPlayerUnit(targetId).TeamId; //Debug.Log("teamId is" + targetTeamId);
            if( actorTeamId == targetTeamId)
            {
                return true;
            }
            return false;
        }
        else if (allyEnemy == NameAll.ALLIES_TYPE_ENEMIES) //enemies only
        {
            int targetTeamId = PlayerManager.Instance.GetPlayerUnit(targetId).TeamId; //Debug.Log("teamId is" + targetTeamId);
            if (actorTeamId != targetTeamId)
            {
                return true;
            }
            return false;
        }
        return true;
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
                    if (ValidTile(tile, startTile, verticalRange))
                        retValue.Add(tile);
                }
                lateral += 2;
            }
        }

        return retValue;
    }

    bool ValidTile(Tile t, Tile startTile, int verticalRange)
    {
        return t != null && Mathf.Abs(t.height - startTile.height) <= verticalRange;
    }


    int GetSpellEffect(int spellEffect, PlayerUnit actor, SpellName sn)
    {
        //109 for allies, 110 for enemies, 111 for math skill handled in GetTiles
        int retValue = spellEffect;
        if( spellEffect < 100)
        {
            //no change
        }
        else if( spellEffect >= SPELL_LINE_MIN && spellEffect <= SPELL_LINE_MAX) //straightLine
        {
            retValue = sn.EffectXY - 100;
            retValue = Mathf.Clamp(retValue, 2, 8);
        }
        else if (spellEffect >= SPELL_CONE_MIN && spellEffect <= SPELL_CONE_MAX)
        {
            retValue = sn.EffectXY - NameAll.SPELL_EFFECT_CONE_BASE;
            retValue = Mathf.Clamp(retValue, 1, 9);
        }
        
        return retValue;
    }
}
