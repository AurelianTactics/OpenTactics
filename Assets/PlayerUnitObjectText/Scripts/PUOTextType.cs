using System;


namespace PlayerUnitObjectText.PUOText
{
	/// <summary>
	/// Can add or remove various combat texts here
	/// </summary>
	public enum PUOTextType
	{
		UnsetInvalid = 0, // don't change or use
		Miss = 1,
		Hit = 2,
		CriticalHit = 3,
		Heal = 4,
        HealMP = 5, //mp and other stats
        HitMP = 6, //mp and other stats
        AddStatus = 7, //all statuses
    }
}
