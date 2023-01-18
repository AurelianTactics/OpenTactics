using UnityEngine;
using System.Collections;

/// <summary>
/// At the end of ActiveTurns, handles any remaining things that may occure
/// Currently, that is just some statuses
/// </summary>
public class PostActiveTurnState : CombatState
{

	public override void Enter()
	{
		base.Enter();
		StartCoroutine(DoPhase());
	}

	IEnumerator DoPhase()
	{
		StatusManager.Instance.CheckStatusAtEndOfTurn(turn.actor.TurnOrder);
		yield return null;
		actorPanel.Close();
		owner.ChangeState<GameLoopState>();
	}
}
