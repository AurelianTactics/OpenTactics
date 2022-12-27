using UnityEngine;
using System.Collections;

/// <summary>
/// WA mode, different types of inputs that can be done for a PlayerUnit turn
/// </summary>
public enum WalkAroundInput
{
    UnitNone = 0, //no unit highlighted
    UnitSelected, //unit highlighted but eligible?
    UnitEligible, //unit eligible for input
    UnitMove, //move selected
	UnitMoveConfirm,
	UnitAct,
	UnitAbilitySelect, //ability selected but target not selected
	UnitAbilityTarget, //ability and target selected
	UnitAbilityConfirm, //confirming the ability and target
	UnitWait, //end turn
	UnitWaitTarget,
	UnitWaitConfirm,
	UnitMoveMap, //unit is eligible to move map but hasn't decided to move or not
	UnitContinue //continue charging/performing/already inputted turn
}

