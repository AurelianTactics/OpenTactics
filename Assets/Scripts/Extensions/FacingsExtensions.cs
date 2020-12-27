using UnityEngine;
using System.Collections;

public static class FacingsExtensions
{
	//public static Facings GetFacing (this Unit attacker, Unit target)
	//{
	//	Vector2 targetDirection = target.dir.GetNormal();
	//	Vector2 approachDirection = ((Vector2)(target.tile.pos - attacker.tile.pos)).normalized;
	//	float dot = Vector2.Dot( approachDirection, targetDirection );
	//	if (dot >= 0.45f)
	//		return Facings.Back;
	//	if (dot <= -0.45f)
	//		return Facings.Front;
	//	return Facings.Side;
	//}

    public static Facings GetFacingPU(this PlayerUnit actor, PlayerUnit target)
    {
        //Vector2 targetDirection = target.dir.GetNormal();
        //Vector2 approachDirection = ((Vector2)(target.tile.pos - attacker.tile.pos)).normalized;
        //float dot = Vector2.Dot(approachDirection, targetDirection);
        //if (dot >= 0.45f)
        //    return Facings.Back;
        //if (dot <= -0.45f)
        //    return Facings.Front;
        //return Facings.Side;
        int z1 = actor.TileX - target.TileX;
        int z2 = actor.TileY - target.TileY;
        int z3 = Mathf.Abs(z1);
        int z4 = Mathf.Abs(z2);

        if (target.Dir == Directions.North)
        { //n
            if (z3 > z4)
            {
                return Facings.Side;
            }
            else if (z2 < 0)
            {
                return Facings.Back;
            }
            else {
                return Facings.Front;
            }
        }
        else if (target.Dir == Directions.East)
        { //east
            if (z3 < z4)
            {
                return Facings.Side;
            }
            else if (z1 < 0)
            {
                return Facings.Back;
            }
            else {
                return Facings.Front;
            }
        }
        else if (target.Dir == Directions.South)
        { //s
            if (z3 > z4)
            {
                return Facings.Side;
            }
            else if (z2 < 0)
            {
                return Facings.Front;
            }
            else {
                return Facings.Back;
            }
        }
        else { //west
            if (z3 < z4)
            {
                return Facings.Side;
            }
            else if (z1 < 0)
            {
                return Facings.Front;
            }
            else {
                return Facings.Back;
            }
        }
        
    }
}
