﻿using System;


namespace EckTechGames.FloatingCombatText
{
	/// <summary>
	/// This enumerates the various Combat Text Types that we support.
	/// See the ReadMe.txt file for more detailed instructions on how to make 
	/// your own CombatTextType and associated animations.
	/// </summary>
	public enum CombatTextType
	{
		UnsetInvalid = 0, // Default enumeration value. Don't use this
		Miss = 1,
		Hit = 2,
		CriticalHit = 3,
		Heal = 4,
        HealMP = 5, //mp and other stats
        HitMP = 6, //mp and other stats
        AddStatus = 7, //all statuses
    }
}
