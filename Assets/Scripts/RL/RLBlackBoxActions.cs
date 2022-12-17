using System.Collections;
using System.Collections.Generic;

/// <summary>
/// class for handling actions
/// </summary>
public class RLBlackBoxActions
{
	public RLBlackBoxActionPhase actionPhase;
	public RLBlackBoxActionChosen actionChosen;
	public int targetX;
	public int targetY;

	public RLBlackBoxActions()
	{
		this.actionPhase = RLBlackBoxActionPhase.None;
		this.actionChosen = RLBlackBoxActionChosen.None;
		this.targetX = NameAll.NULL_INT;
		this.targetY = NameAll.NULL_INT;
	}
}

public enum RLBlackBoxActionChosen 
{
	None,
	WaitSelected,
	MoveSelected,
	AttackSelected,
	PrimarySelected,
	SecondarySelected
}

public enum RLBlackBoxActionPhase
{
	None,
	continueTurn,
	turnStart,
	chooseEndDirection,
	choosePrimaryAbility,
	chooseSecondaryAbility,
	targetUnit,
	chooseTargetX,
	chooseTargetY
	//chooseMoveX,
	//chooseMoveY,
	//chooseAttackX,
	//chooseAttackY,
	//chooseAbilityX,
	//chooseAbilityY,
	
}
