using UnityEngine;
using System.Collections;

public class StatusCheckState : CombatState {
	/// <summary>
	/// DEPRECATED FOR NOW
	/// just doing the status managercheck from gameloop state to speed things up a bit
	/// </summary>

    //decrement the current statuses
        //create an array that shows the expired but not shown
        //use that array with this state to do whatever you need, empty that array at the end
    //update the combat log to say which expired
    //move the camera around to show which expired
    //flash a messaging saying which expire (or maybe a general message if more than x)

    public override void Enter()
    {
        base.Enter();
        StartCoroutine(DoPhase());

    }

    public override void Exit()
    {
        base.Exit();
    }

    IEnumerator DoPhase()
    {
        StatusManager.Instance.StatusCheckPhase();
        yield return null;
        owner.ChangeState<GameLoopState>();
    }
}
