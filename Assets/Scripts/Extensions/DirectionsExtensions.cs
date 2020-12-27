using UnityEngine;
using System.Collections;

public static class DirectionsExtensions
{
    //called when moving so unit can get the direction to turn to
    public static Directions GetDirection(this Tile t1, Tile t2)
    {
        if (t1.pos.y < t2.pos.y)
            return Directions.North;
        if (t1.pos.x < t2.pos.x)
            return Directions.East;
        if (t1.pos.y > t2.pos.y)
            return Directions.South;
        return Directions.West;
    }

    //called when attacking. unit faces east/west unless north/south is a higher magnitude
    public static Directions GetDirectionAttack(this Tile t1, Tile t2)
    {
        int xOffset = Mathf.Abs(t1.pos.x - t2.pos.x);
        int yOffset = Mathf.Abs(t1.pos.y - t2.pos.y);

        if( yOffset > xOffset)
        {
            if (t2.pos.y > t1.pos.y)
                return Directions.North;
            else
                return Directions.South;
        }
        else
        {
            if (t2.pos.x > t1.pos.x)
                return Directions.East;
            else
                return Directions.West;
        }
    }

    public static Vector3 ToEuler (this Directions d)
	{
		return new Vector3(0, ((int)d * 90) + NameAll.ROTATION_OFFSET , 0); //rotation offset since they line up funny
	}

	public static Directions GetDirection (this Point p)
	{
		if (p.y > 0)
			return Directions.North;
		if (p.x > 0)
			return Directions.East;
		if (p.y < 0)
			return Directions.South;
		return Directions.West;
	}

	public static Point GetNormal (this Directions dir)
	{
		switch (dir)
		{
		case Directions.North:
			return new Point(0, 1);
		case Directions.East:
			return new Point(1, 0);
		case Directions.South:
			return new Point(0, -1);
		default: // Directions.West:
			return new Point(-1, 0);
		}
	}

    public static int GetDirectionPU(PlayerUnit lookee, PlayerUnit looker, int baseQDirection)
    {
        int z1 = lookee.TileX - looker.TileX;
        int z2 = lookee.TileY - looker.TileY;
        int z3 = Mathf.Abs(z1);
        int z4 = Mathf.Abs(z2); //Debug.Log("in get direction pu " + z1 + ", " + z2 + "," + z3 + "," + z4);

        if ( z3 > z4) //more X than Y
        {
            if( z1 > 0) //lookee to the east
            {
                if (baseQDirection == NameAll.BASE_Q_DIRECTION_FACE_TOWARD)
                    return NameAll.EAST;
                else if (baseQDirection == NameAll.BASE_Q_DIRECTION_FACE_AWAY)
                    return NameAll.WEST;
            }
            else //lookee to the west
            {
                if (baseQDirection == NameAll.BASE_Q_DIRECTION_FACE_TOWARD)
                    return NameAll.WEST;
                else if (baseQDirection == NameAll.BASE_Q_DIRECTION_FACE_AWAY)
                    return NameAll.EAST;
            }
        }
        else
        {
            if( z2 > 0) //lookee to the north
            {
                if (baseQDirection == NameAll.BASE_Q_DIRECTION_FACE_TOWARD)
                    return NameAll.NORTH;
                else if (baseQDirection == NameAll.BASE_Q_DIRECTION_FACE_AWAY)
                    return NameAll.SOUTH;
            }
            else //lookee to the south
            {
                if (baseQDirection == NameAll.BASE_Q_DIRECTION_FACE_TOWARD)
                    return NameAll.SOUTH;
                else if (baseQDirection == NameAll.BASE_Q_DIRECTION_FACE_AWAY)
                    return NameAll.NORTH;
            }
        }

        Debug.Log("ERROR: couldn't find proper facing direction");
        return NameAll.NORTH;

        //if (t1.pos.y < t2.pos.y)
        //    return Directions.North;
        //if (t1.pos.x < t2.pos.x)
        //    return Directions.East;
        //if (t1.pos.y > t2.pos.y)
        //    return Directions.South;
        //return Directions.West;
    }

    public static Directions IntToDirection(int directionInt)
    {
        int z1 = directionInt % 4;

        if (z1 == NameAll.NORTH)
            return Directions.North;
        else if (z1 == NameAll.EAST)
            return Directions.East;
        else if (z1 == NameAll.SOUTH)
            return Directions.South;
        else
            return Directions.West;
    }

    public static int DirectionToInt(this Directions d)
    {
        if (d == Directions.North)
            return NameAll.NORTH;
        else if (d == Directions.East)
            return NameAll.EAST;
        else if (d == Directions.South)
            return NameAll.SOUTH;
        else
            return NameAll.WEST;
    }
}