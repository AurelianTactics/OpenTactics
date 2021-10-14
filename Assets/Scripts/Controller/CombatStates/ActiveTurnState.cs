using UnityEngine;
using System.Collections;

//entered from GameLoopState
	//seems kind of unnecessary. could probably put most of this in CombatCommandSelectionState or maybe part of it in GameLoopState
	//assuming that never come here if rendermode is none or get some game breaking change state things
public class ActiveTurnState : CombatState
{

    bool isOffline;

    public override void Enter()
    {
        base.Enter();
        isOffline = PlayerManager.Instance.IsOfflineGame();
		//EnableObservers();
		DoPhase();
    }

    public override void Exit()
    {
        base.Exit();
        //DisableObservers();
        //actorPanel.Close();
        //statPanelController.HidePrimary();
    }

	void DoPhase()
	{
		PlayerUnit pu = PlayerManager.Instance.GetNextActiveTurnPlayerUnit(isSetQuickFlagToFalse: true);//sets quickFlag to false if it was true
		//Debug.Log("in do phase top");
		
		if (pu != null) //probably an unnecessary check
		{
			turn.Change(pu);

			actorPanel.SetActor(turn.actor); //stat panel showing unit and details
			cameraMain.Open(board.GetTile(turn.actor));
			cameraMain.FollowUnit(turn.actor.TurnOrder);

			if (!isOffline)
			{
				PlayerManager.Instance.SendMPActiveTurnPreTurn(pu.TurnOrder); //so Other can highlight the proper unit and know whose turn it is
				if (pu.TeamId == NameAll.TEAM_ID_GREEN)
					owner.battleMessageController.Display("Your Turn for: " + turn.actor.UnitName);
				else
					owner.battleMessageController.Display("Opponent Turn for: " + turn.actor.UnitName);

			}


			//handles dead, reraise, chicken etc. if unit is dead and unable to act returns a true
			bool skipTurn = StatusManager.Instance.CheckStatusAtBeginningOfTurn(turn.actor.TurnOrder); //handles dead and reraise, not sure if this is the best place to do it
			if (skipTurn)
			{
				StartCoroutine(SkipTurnCoroutine());
			}
			else
			{
				StartCoroutine(GoToCombatCommandSelectionState());
			}
			//Debug.Log("in do phase near end");
		}
		else
		{
			if (owner.renderMode != NameAll.PP_RENDER_NONE)
				actorPanel.Close();
			owner.ChangeState<GameLoopState>();
		}
	}

	IEnumerator GoToCombatCommandSelectionState()
	{
		yield return null;
		owner.ChangeState<CombatCommandSelectionState>();
	}

	IEnumerator SkipTurnCoroutine()
	{
		yield return null;
		SelectTile(turn.actor);
		yield return new WaitForSeconds(1.0f);
		PlayerManager.Instance.EndCombatTurn(turn, true);
		actorPanel.Close();
		owner.ChangeState<GameLoopState>();
	}

}
