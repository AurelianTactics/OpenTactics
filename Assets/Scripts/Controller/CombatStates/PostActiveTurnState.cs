using UnityEngine;
using System.Collections;

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
