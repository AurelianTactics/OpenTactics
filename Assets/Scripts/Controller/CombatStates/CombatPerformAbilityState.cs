using UnityEngine;
using System.Collections;

//called from CombatConfirmAbilityTargetState
//starts the corouting that performs the ability (or creates the slow action where applicable)

public class CombatPerformAbilityState : CombatState
{
    public override void Enter()
    {
        base.Enter();
        turn.hasUnitActed = true;
        //Debug.Log("perform ability state entered? "  + turn.targets.Count);

        turn.phaseStart = 1;
        StartCoroutine(Animate());

        //not worrying about phaseStart checks here
        //if ( turn.phaseStart == 0)
        //{
        //    turn.phaseStart = 1;
        //    StartCoroutine(Animate());
        //}
        //else
        //{

        //    //turn.phaseStart = 0;
        //    //mime and reaction checked
        //    //EndPhase();
        //}
    }

    IEnumerator Animate()
    {
        //breaking up the active turn
        //path 1 slow action
        //slow action added to queue
        //unit gets charging status
        //precedes to end turn
        //path 2 fast action
        //goes to resolve action
        //passes checks
        //inventory check
        //mp check
        //jumping check
        //silence check (animation on actor)
        //unit turns, does animation, maybe launches particle
        //pre results
        //mp removed
        //mimeQueue added
        //results (iterates over targets)
        //reflect check (animation on target)
        //effect on target 
        //chance to counter
        //animations on target
        //statuses on target
        //possible second swing
        //repates the results section




        //Debug.Log("starting call for fastaction inner from perform ability state");
        owner.battleMessageController.Display(turn.spellName.AbilityName); //owner.calcMono.DoFastAction(board, turn, isActiveTurn: true, isSlowAction: false, isReaction: false, isMime: false);
		owner.calcMono.DoFastAction(board, turn, isActiveTurn: true, isReaction: false, isMime: false, renderMode: owner.renderMode);
		//yield return StartCoroutine(owner.calcMono.DoFastActionInner(board, turn, isActiveTurn: true, isSlowActionPhase: false, isReaction: false, isMime: false));
		//Debug.Log("past call for fastaction inner from perform ability state");
		yield return null;


        if (turn.spellName.CommandSet == NameAll.COMMAND_SET_JUMP)
        {
            PlayerManager.Instance.SetFacingDirectionMidTurn(turn.actor.TurnOrder, board.GetTile(turn.actor), turn.targetTile);
            PlayerManager.Instance.EndCombatTurn(turn);
            turn.phaseStart = 0; //think this is redundant but keeps it from coming back to this unit mid turn
        }
        //Debug.Log("battle is continuing, going to game loop state");
        owner.ChangeState<GameLoopState>(); //gameloopstate will return to performabilitystate using turn.phaseStart

    }

}
