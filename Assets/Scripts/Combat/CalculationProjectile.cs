using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**
 Detects if a projectile reaches its target or hits a different unit/obstacle along the way

//possible things to add:
    different units/map objects different height/width
    map objects have different things passible
 */

public class CalculationProjectile
//public class CalculationProjectile : MonoBehaviour
{

    //public static
    public Tile GetProjectileHit(Board board, SpellName sn, PlayerUnit actor, Tile targetTile)
    {
        Tile startTile = board.GetTile(actor.TileX, actor.TileY);
        Tile retTile = targetTile;
        float startX = actor.TileX;
        float endX = targetTile.pos.x;
        float xDiff = endX - startX;
        float startY = actor.TileY;
        float endY = targetTile.pos.y;
        float yDiff = endY - startY;
        float zConstant = 2.0f;
        float startZ = actor.TileZ + zConstant;
        float endHeight = targetTile.height + zConstant;
        float zDiff = endHeight - startZ;

        int numSteps = (int)Math.Ceiling(Math.Sqrt(Math.Pow(startX - endX, 2) + Math.Pow(startY - endY, 2)));
        float xPerStep = xDiff / (float)numSteps;
        float yPerStep = yDiff / (float)numSteps;
        float zPerStep = zDiff / (float)numSteps;

        float currentX = startX;
        float currentY = startY;
        float currentZ = startZ;
        for (int i = 1; i < numSteps; i++)
        {
            currentX = startX + (float)i * xPerStep;
            currentY = startY + (float)i * yPerStep;
            currentZ = startZ + (float)i * zPerStep;

            Tile currentTile = board.GetTileCollision(currentX, currentY, currentZ, startTile);
            if (currentTile != null)
                return currentTile;
        }

        return retTile;
    }
    

}
