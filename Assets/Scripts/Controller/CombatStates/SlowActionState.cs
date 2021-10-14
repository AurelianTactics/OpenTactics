//enter here from GameLoopState (combat) or WalkAroundMainState (walkAroundMode)

using UnityEngine;
using System.Collections;

public class SlowActionState : CombatState {

    public const string DidSlowActionResolve = "SlowAction.DidResolve";
    bool isOffline;

    public override void Enter()
    {
        base.Enter();
        isOffline = PlayerManager.Instance.IsOfflineGame();
		//StartCoroutine(DoPhase());
		DoPhase(owner.renderMode);
    }

    public override void Exit()
    {
        base.Exit();
    }

	void DoPhase(int renderMode)
	{
		SpellSlow ss = SpellManager.Instance.GetNextSlowAction();
		if (ss != null) //checked in GameLoopState too, needed here for WalkAround
		{
			if (!StatusManager.Instance.IsTurnActable(ss.UnitId))
			{
				//unit can't go, remove the spell slow, will change state down below
				SpellManager.Instance.RemoveSpellSlowByObject(ss);
			}
			else
			{
				if (!isOffline) //online game, sends details to Other for display
					SpellManager.Instance.SendSpellSlowPreCast(ss.UniqueId);

				Directions startDir = PlayerManager.Instance.GetPlayerUnit(ss.UnitId).Dir;
				Tile targetTile; //Debug.Log("spell slow is is targetting " + ss.GetTargetUnitId());
				if (ss.TargetUnitId != NameAll.NULL_UNIT_ID)
				{
					targetTile = board.GetTile(PlayerManager.Instance.GetPlayerUnit(ss.UnitId)); //Debug.Log("spell slow is targeting a unit");
				}
				else
				{
					targetTile = board.GetTile(ss.TargetX, ss.TargetY); //Debug.Log("spell slow is targeting a panel");
				}

				CombatTurn ssTurn = new CombatTurn(ss, board);

				if (renderMode != NameAll.PP_RENDER_NONE)
				{
					StartCoroutine(HighlightAction(ssTurn, targetTile));
				}

				//do the fast action
				ssTurn.spellName = CalculationAT.ModifySlowActionSpellName(ssTurn.spellName, ssTurn.actor.TurnOrder);
				StatusManager.Instance.CheckChargingJumping(ssTurn.actor.TurnOrder, ssTurn.spellName);

				owner.calcMono.DoFastAction(board, ssTurn, isActiveTurn: false, isReaction: false, isMime: false, renderMode: renderMode);
				//yield return StartCoroutine(owner.calcMono.DoFastActionInner(board, ssTurn, isActiveTurn: false, isSlowActionPhase: true, isReaction: false, isMime: false));

				SpellName sn = SpellManager.Instance.GetSpellNameByIndex(ss.SpellIndex);
				//if performing, add a new instance of it Ctr into the future
				if (sn.CommandSet == NameAll.COMMAND_SET_SING || sn.CommandSet == NameAll.COMMAND_SET_DANCE
					|| (sn.CommandSet == NameAll.COMMAND_SET_ARTS && sn.CTR % 10 == 1))
				{
					SpellManager.Instance.CreateSpellSlow(ss);
				}
				else
				{
					StatusManager.Instance.RemoveStatus(ss.UnitId, NameAll.STATUS_ID_CHARGING, true);
				}

				if (renderMode != NameAll.PP_RENDER_NONE)
				{
					StartCoroutine(UnHighlightAction(ssTurn, ss, startDir));
				}
				else
				{
					SpellManager.Instance.RemoveSpellSlowByObject(ss); //i'm not sure why I remove the SS here and not earlier
					PlayerManager.Instance.GetPlayerUnit(ss.UnitId).Dir = startDir;
				}				
			}
		}

		if (PlayerManager.Instance.GetGameMode() == NameAll.INIT_STATE_WALK_AROUND)
			owner.ChangeState<WalkAroundMainState>();
		else
			owner.ChangeState<GameLoopState>();
	}

	IEnumerator HighlightAction(CombatTurn ssTurn, Tile targetTile)
	{
		cameraMain.MoveToMapTile(targetTile);
		HighlightActorTile(ssTurn.actor, true);
		owner.battleMessageController.Display(ssTurn.spellName.AbilityName + " is cast!"); //this.PostNotification(DidSlowActionResolve); //Debug.Log("Send message to spell title panel");
		board.SelectTiles(ssTurn.targets); //Debug.Log("number of targets is " + ssTurn.targets.Count);//highlight the target tiles
		yield return new WaitForSeconds(1.25f);
	}

	IEnumerator UnHighlightAction(CombatTurn ssTurn, SpellSlow ss, Directions startDir)
	{
		HighlightActorTile(ssTurn.actor, false); //unhighlight
		board.DeSelectTiles(ssTurn.targets); //deselect the tiles
		SpellManager.Instance.RemoveSpellSlowByObject(ss); //i'm not sure why I remove the SS here and not earlier
		yield return new WaitForSeconds(0.75f);
		PlayerManager.Instance.SetPUODirectionMidTurn(ss.UnitId, startDir);
		PlayerManager.Instance.GetPlayerUnit(ss.UnitId).Dir = startDir;
		yield return null;
	}

	//deprecated. doing coroutines on inner phase
	/*
	IEnumerator DoPhase()
	{
		SpellSlow ss = SpellManager.Instance.GetNextSlowAction();
		if (ss != null) //checked in GameLoopState too, needed here for WalkAround
		{
			if (!StatusManager.Instance.IsTurnActable(ss.UnitId))
			{
				//unit can't go, remove the spell slow, will change state down below
				SpellManager.Instance.RemoveSpellSlowByObject(ss);
			}
			else
			{
				if (!isOffline) //online game, sends details to Other for display
					SpellManager.Instance.SendSpellSlowPreCast(ss.UniqueId);

				Directions startDir = PlayerManager.Instance.GetPlayerUnit(ss.UnitId).Dir;
				Tile targetTile; //Debug.Log("spell slow is is targetting " + ss.GetTargetUnitId());
				if (ss.TargetUnitId != NameAll.NULL_UNIT_ID)
				{
					targetTile = board.GetTile(PlayerManager.Instance.GetPlayerUnit(ss.UnitId)); //Debug.Log("spell slow is targeting a unit");
				}
				else
				{
					targetTile = board.GetTile(ss.TargetX, ss.TargetY); //Debug.Log("spell slow is targeting a panel");
				}

				CombatTurn ssTurn = new CombatTurn(ss, board);

				if (owner.renderMode != NameAll.PP_RENDER_NONE)
				{
					cameraMain.MoveToMapTile(targetTile);
					HighlightActorTile(ssTurn.actor, true);
					owner.battleMessageController.Display(ssTurn.spellName.AbilityName + " is cast!"); //this.PostNotification(DidSlowActionResolve); //Debug.Log("Send message to spell title panel");
					board.SelectTiles(ssTurn.targets); //Debug.Log("number of targets is " + ssTurn.targets.Count);//highlight the target tiles
					yield return new WaitForSeconds(1.25f);
				}

				//do the fast action
				ssTurn.spellName = CalculationAT.ModifySlowActionSpellName(ssTurn.spellName, ssTurn.actor.TurnOrder);
				StatusManager.Instance.CheckChargingJumping(ssTurn.actor.TurnOrder, ssTurn.spellName);

				yield return StartCoroutine(owner.calcMono.DoFastActionInner(board, ssTurn, isActiveTurn: false, isSlowActionPhase: true, isReaction: false, isMime: false));

				SpellName sn = SpellManager.Instance.GetSpellNameByIndex(ss.SpellIndex);
				//if performing, add a new instance of it Ctr into the future
				if (sn.CommandSet == NameAll.COMMAND_SET_SING || sn.CommandSet == NameAll.COMMAND_SET_DANCE
					|| (sn.CommandSet == NameAll.COMMAND_SET_ARTS && sn.CTR % 10 == 1))
				{
					SpellManager.Instance.CreateSpellSlow(ss);
				}
				else
				{
					StatusManager.Instance.RemoveStatus(ss.UnitId, NameAll.STATUS_ID_CHARGING, true);
				}

				if (owner.renderMode != NameAll.PP_RENDER_NONE)
				{
					HighlightActorTile(ssTurn.actor, false); //unhighlight
					board.DeSelectTiles(ssTurn.targets); //deselect the tiles
					SpellManager.Instance.RemoveSpellSlowByObject(ss); //i'm not sure why I remove the SS here and not earlier
					yield return new WaitForSeconds(0.75f);
					PlayerManager.Instance.SetPUODirectionMidTurn(ss.UnitId, startDir);
					PlayerManager.Instance.GetPlayerUnit(ss.UnitId).Dir = startDir;
				}
				else
				{
					SpellManager.Instance.RemoveSpellSlowByObject(ss); //i'm not sure why I remove the SS here and not earlier
					PlayerManager.Instance.GetPlayerUnit(ss.UnitId).Dir = startDir;
				}
			}
		}

		yield return null;

		if (PlayerManager.Instance.GetGameMode() == NameAll.INIT_STATE_WALK_AROUND)
			owner.ChangeState<WalkAroundMainState>();
		else
			owner.ChangeState<GameLoopState>();
	}
	*/


	//similar to CombatPerformAbilityState
	//IEnumerator Animate(SpellSlow ss)
	//{
	////perform the fast action
	//    //path 2 fast action
	//        //goes to resolve action
	//        //passes checks
	//            //inventory check
	//            //mp check
	//            //jumping check
	//            //silence check (animation on actor)
	//        //unit turns, does animation, maybe launches particle
	//        //pre results
	//            //mp removed
	//            //mimeQueue added
	//        //results (iterates over targets)
	//            //reflect check (animation on target)
	//            //effect on target 
	//            //chance to counter
	//            //animations on target
	//            //statuses on target
	//        //possible second swing
	//            //repates the results section

	//    //non slow action
	//            //get all the relevant data first, then passed on that set the proper order

	//    CombatTurn ssTurn = new CombatTurn(ss, board);
	//    ssTurn.spellName = CalculationAT.ModifySlowActionSpellName(ssTurn.spellName, ssTurn.actor.TurnOrder);
	//    CheckChargingJumping(ss);

	//    bool isTwoSwordsEligible = PlayerManager.Instance.IsEligibleForTwoSwords(ssTurn.puId, ssTurn.spellName.CommandSet);
	//    bool isPassesChecks = CalculationResolveAction.FastActionChecks(turn,true); //suppress messages for the first time
	//    if( isPassesChecks)
	//    {
	//        //decrement from inventory/chance of breaking
	//            //can't select ability if not enough items
	//        CalculationResolveAction.CheckInventory(ssTurn.spellName, ssTurn.actor.TeamId, ssTurn.actor.TurnOrder);

	//        ApplyPreresults(ssTurn);//mimequeue and subtract MP

	//        //do animation (turn, animation)
	//        //may want to rearrange this due to double swings
	//        PlayerManager.Instance.SetPlayerObjectAnimation(ssTurn.actor.TurnOrder, "attacking", false);
	//        //does the checks, if any check fails proper message is shown
	//        ParticleManager.Instance.LaunchCastParticle(ssTurn.actor, ssTurn.targetTile, ssTurn.spellName);

	//        //results (iterates over targets)
	//            //reflect check (animation on target)
	//            //effect on target 
	//            //chance to counter (does not show the counter, simply adds it to the counter queue)
	//            //animations on target (just launches it, not worked about sync)
	//            //statuses on target (just launches it, not worried about sync)
	//        CalculationResolveAction.FastActionResults(board, turn);
	//        if( isTwoSwordsEligible)
	//        {
	//            yield return new WaitForSeconds(1.5f);
	//            PlayerManager.Instance.SetPlayerObjectAnimation(turn.actor.TurnOrder, "attacking", false);
	//            //does the second swing, may need some data from the first swing
	//            bool isForceFacing = false; bool isAllowBladeGrasp = false;
	//            CalculationResolveAction.FastActionSecondSwing(board, turn, false, isForceFacing, isAllowBladeGrasp );
	//        }
	//    }
	//    else
	//    {
	//        CalculationResolveAction.FastActionChecks(turn, false);
	//    }

	//    yield return null;

	//    if (IsBattleOver())
	//    {
	//        owner.ChangeState<CombatCutSceneState>();
	//    } 
	//}

	//void ApplyPreresults(CombatTurn ssTurn)
	//{
	//    //subtract MP
	//    CalculationResolveAction.SubtractMP(ssTurn.actor,ssTurn.spellName);
	//    //add to mime queue
	//    CalculationResolveAction.AddToMimeQueue(board, turn.actor, turn.spellName, turn.targetTile); Debug.Log("just did mime queu check");
	//}

	//void CheckChargingJumping(SpellSlow ss)
	//{
	//    SpellName sn = SpellManager.Instance.GetSpellNameByIndex(ss.GetSpellIndex());
	//    if( sn.CommandSet == NameAll.COMMAND_SET_JUMP)
	//    {
	//        StatusManager.Instance.RemoveStatus( ss.GetUnitId(), NameAll.STATUS_ID_JUMPING);
	//        PlayerManager.Instance.ToggleJumping(ss.GetUnitId(), false);
	//    }
	//    else if(sn.CommandSet == NameAll.COMMAND_SET_CHARGE)
	//    {
	//        StatusManager.Instance.RemoveStatus(ss.GetUnitId(),NameAll.STATUS_ID_CHARGING);
	//    }
	//}

}
