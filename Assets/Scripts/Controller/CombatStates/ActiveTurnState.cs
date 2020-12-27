using UnityEngine;
using System.Collections;

//entered from GameLoopState
	//seems kind of unnecessary. could probably put most of this in CombatCommandSelectionState or maybe part of it in GameLoopState
public class ActiveTurnState : CombatState
{

    bool isOffline;

    public override void Enter()
    {
        base.Enter();
        isOffline = PlayerManager.Instance.IsOfflineGame();
        //EnableObservers();
        StartCoroutine(DoPhase());
    }

    public override void Exit()
    {
        base.Exit();
        //DisableObservers();
        //actorPanel.Close();
        //statPanelController.HidePrimary();
    }

    IEnumerator DoPhase()
    {
        PlayerUnit pu = PlayerManager.Instance.GetNextActiveTurnPlayerUnit(isSetQuickFlagToFalse: true);//sets quickFlag to false if it was true
        
        if (pu != null) //probably an unnecessary check
        {
            turn.Change(pu);

            actorPanel.SetActor(turn.actor); //stat panel showing unit and details
            cameraMain.Open(board.GetTile(turn.actor));
            cameraMain.FollowUnit(turn.actor.TurnOrder);
            if (!isOffline)
            {
                PlayerManager.Instance.SendMPActiveTurnPreTurn(pu.TurnOrder); //so Other can highlight the proper unit and know whose turn it is
                if ( pu.TeamId == NameAll.TEAM_ID_GREEN)
                    owner.battleMessageController.Display("Your Turn for: " + turn.actor.UnitName);
                else
                    owner.battleMessageController.Display("Opponent Turn for: " + turn.actor.UnitName);

            }
                

            //handles dead, reraise, chicken etc. if unit is dead and unable to act returns a true
            bool skipTurn = StatusManager.Instance.CheckStatusAtBeginningOfTurn(turn.actor.TurnOrder); //handles dead and reraise, not sure if this is the best place to do it
            yield return null;
            SelectTile(turn.actor);
            
            yield return null;
            if( skipTurn)
            {
                yield return new WaitForSeconds(1.0f);
                PlayerManager.Instance.EndCombatTurn(turn,true);
                actorPanel.Close();
                owner.ChangeState<GameLoopState>();
            }
            else
            {
                owner.ChangeState<CombatCommandSelectionState>();
            }
            
        }
        else
        {
            actorPanel.Close();
            owner.ChangeState<GameLoopState>();
        }
    }

    //#region notifications //moved to GameLoopState

    //const string DidStatusManager = "StatusManager.Did";

    //void EnableObservers()
    //{
    //    this.AddObserver(OnStatusManagerNotification, DidStatusManager);
    //}

    //void DisableObservers()
    //{
    //    this.RemoveObserver(OnStatusManagerNotification, DidStatusManager);
    //}

    //void OnStatusManagerNotification(object sender, object args)
    //{
    //    string str = (string)args;
    //    if( str.Equals(NameAll.STATUS_NAME_CRYSTAL))
    //    {
    //        //roll for crystal
    //        if (CalculationResolveAction.RollForSuccess(50))
    //        {
    //            //turn object holds the player shit
    //            board.SetTilePickUp(turn.actor, true, 1);
    //        }
    //        board.DisableUnit(turn.actor);
    //        PlayerManager.Instance.DisableUnit(turn.actor.TurnOrder);
    //    }
    //}
    //    #endregion
}
