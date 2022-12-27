using UnityEngine;
using System.Collections;

/// <summary>
/// When using an ability, the target types (self, ally, target etc)
/// Used for seeing if ability can target
/// </summary>
public enum Targets
{
	None,
	Self,
	Ally,
	Foe,
	Tile
}