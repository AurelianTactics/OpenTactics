using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class CalculationZodiac
{

    public static int GetZodiacMain(int xa, PlayerUnit actor, PlayerUnit target)
    {
        if (xa <= 0)
        {
            xa = 0;
            return xa;
        }

        if (Math.Abs(actor.ZodiacInt - target.ZodiacInt) == 6)
        {
            //best/worst zodiac
            if (!actor.Sex.Equals(target.Sex))
            {
                xa += xa / 2;
                return xa;
            }
            else {
                xa -= xa / 2;
                return xa;
            }
        }
        else if (Math.Abs(actor.ZodiacInt - target.ZodiacInt) == 8 ||
              Math.Abs(actor.ZodiacInt - target.ZodiacInt) == 4)
        {
            //good compat
            xa += xa / 4;
            return xa;
        }
        else if (Math.Abs(actor.ZodiacInt - target.ZodiacInt) == 3 ||
              Math.Abs(actor.ZodiacInt - target.ZodiacInt) == 9)
        {
            //bad compat
            xa -= xa / 4;
            return xa;
        }
        else
        {
            return xa; //neutral
        }

    }

    //typically add this to the base_hit outside the function to get the success number
    public static int GetZodiacMod3(int xa, int baseHit, PlayerUnit actor, PlayerUnit target, bool uses_wp = false)
    {
        int weaponPower = 0;
        if (uses_wp)
        {
            weaponPower = PlayerManager.Instance.GetWeaponPower(actor.TurnOrder, uses_wp); //Battle Skill uses wp (but not barehanded pa)
        }

        if (xa <= 0)
        {
            xa = 0;
            return xa;
        }
        if (Math.Abs(actor.ZodiacInt - target.ZodiacInt) == 6)
        {
            //best/worst zodiac
            if (!actor.Sex.Equals(target.Sex))
            {
                xa = xa + xa / 2 + baseHit / 2 + weaponPower / 2;
                return xa;
            }
            else {
                xa = xa - xa / 2 - baseHit / 2 - weaponPower / 2;
                if (xa <= 0)
                {
                    xa = 0;
                }
                return xa;
            }
        }
        else if (Math.Abs(actor.ZodiacInt - target.ZodiacInt) == 8 ||
              Math.Abs(actor.ZodiacInt - target.ZodiacInt) == 4)
        {
            //good compat
            xa = xa + xa / 4 + baseHit / 4 + weaponPower / 4;
            return xa;
        }
        else if (Math.Abs(actor.ZodiacInt - target.ZodiacInt) == 3 ||
              Math.Abs(actor.ZodiacInt - target.ZodiacInt) == 9)
        {
            //bad compat
            xa = xa - xa / 4 - baseHit / 4 - weaponPower / 4;
            if (xa <= 0)
            {
                xa = 0;
            }
            return xa;
        }
        else {
            return xa; //neutral
        }
    }

}
