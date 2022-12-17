using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLoopState : CombatState {

    static Phases loopPhase = Phases.StatusTick; //what phase the game loop is in, due to Mime, Counter, QuickFlag the "current" phase can be different
    static Phases lastingPhase = Phases.StatusTick; //so loop phase knows where to return to after jumping to an active turn/counter/mime
    static Phases flagCheckPhase = Phases.SlowAction;
    bool doLoop = false;
    bool isOffline = true;
    bool isMasterClient;

	bool isRLEndNotificationSent;
	const string RLEndEpisode = "ReinforcementLearning.EndEpisode"; //end episode notification sent to RL code
	const string RLResetGame = "ReinforcementLearning.ResetGame"; //reset game, sent from RL code to this state
	// show top menu turns
	const string refreshTurns = "refreshTurns";

	//multiplayer checks for DC from opponent
	private float timeSinceLastCalled;
	private float delay = 30.0f;

	public override void Enter()
    {
        base.Enter();
        //Debug.Log("in game loop state");
        doLoop = true;
        isOffline = PlayerManager.Instance.IsOfflineGame();
        isMasterClient = PlayerManager.Instance.isMPMasterClient();
		isRLEndNotificationSent = false;
        EnableObservers();
        if (!isOffline && !isMasterClient)
        {
            //Other (P2) just waits in the phase until receiving a notification
            //based on the notification phase, does various states
            doLoop = false;
            PlayerManager.Instance.SetMPStandby(); //Other sends this to let master know P2
        }
            
    }


	public override void Exit()
	{
		doLoop = false;
		base.Exit();
		DisableObservers();
	}

    void Update()
    {
        if(doLoop)
        {
            
            //Debug.Log("looping through game loop: " + loopPhase);
            if ( isOffline)
            {
                if (IsBattleOver())
                {
					//Debug.Log("batte is over 0");
					if(owner.combatMode == NameAll.COMBAT_MODE_RL_DUEL)
					{
						//Debug.Log("batte is over 1");
						if (!isRLEndNotificationSent)
						{
							//Debug.Log("batte is over 2");
							this.PostNotification(RLEndEpisode, owner.GetComponent<CombatVictoryCondition>().Victor);
							isRLEndNotificationSent = true;
						}
					}
					else
						owner.ChangeState<CombatCutSceneState>(); //online: call it inside gameloop in MP so that Other is ready

					return;
                }
                DoGameLoop();
            }
            else
            {
                if (isMasterClient)
                    ExecuteMPGameLoop();
            }

        }


		if (!isOffline)
		{
			timeSinceLastCalled += Time.deltaTime;
			if (timeSinceLastCalled > delay)
			{
				timeSinceLastCalled = 0f;
				int z1 = PlayerManager.Instance.GetMPNumberOfPlayers();
				if (z1 != 2)
				{
					owner.battleMessageController.Display("" + z1 + " players remaining in Game! Other player has probably left.", 3.0f);
				}
			}
		}
	}

	#region GameLoop
	//does game loop without using state machine for faster sims for RL mode
	void DoGameLoop()
	{
		//Debug.Log("Doing Game Loop " + loopPhase);
		if (loopPhase == Phases.StatusTick) //some statuses last for a certain number of ticks, decrements statuses by a tick and sees if any trigger
		{
			lastingPhase = Phases.StatusTick;
			loopPhase = Phases.SlowActionTick; //Debug.Log("doing statusTick decrement phase");
			StatusManager.Instance.StatusCheckPhase(); //decrements all lasting statuses, if any expire update combat log and show them expiring.
		}
		else if (loopPhase == Phases.SlowActionTick) //decrements all slowaction ticks. when slow action reaches 0 ticks, it is executed
		{
			lastingPhase = Phases.SlowActionTick;
			SpellManager.Instance.SlowActionTickPhase(); 
			loopPhase = Phases.SlowAction;
		}
		else if (loopPhase == Phases.SlowAction) //executes slow actions
		{
			lastingPhase = Phases.SlowAction;
			//check for quick. mime, and counter flags. These flags signify events that occure before slowactions
			flagCheckPhase = Phases.SlowAction;
			loopPhase = CheckForFlag(loopPhase);
			if (loopPhase == Phases.SlowAction)
			{
				SpellSlow ss = SpellManager.Instance.GetNextSlowAction();
				if (ss != null)
				{
					DoSlowAction(ss);
					RefreshTurnsList();
				}
				else
				{
					loopPhase = Phases.CTIncrement;
				}
			}
		}
		else if (loopPhase == Phases.CTIncrement)
		{
			lastingPhase = Phases.CTIncrement;
			PlayerManager.Instance.IncrementCTPhase();
			loopPhase = Phases.ActiveTurn;
		}
		else if (loopPhase == Phases.ActiveTurn)
		{
			//Debug.Log("Doing Game Loop ActiveTurn top" + loopPhase);
			//turn reached from coming from proper phase
			if (lastingPhase == Phases.CTIncrement || lastingPhase == Phases.ActiveTurn)
			{
				lastingPhase = Phases.ActiveTurn;
				if (CheckForActiveTurn())
				{
					flagCheckPhase = Phases.ActiveTurn; //in case of reaction or mime state, the loop returns to the proper state
														//either quickflag (which is handled below), reaction flag (goes to reaction state then comes back here, mime flag (goes to mime state then returns here), or AT phase which is handled here
					loopPhase = CheckForFlag(loopPhase, turn.phaseStart);
					if (loopPhase == Phases.Quick)
					{
						loopPhase = Phases.ActiveTurn;
						if (owner.renderMode != NameAll.PP_RENDER_NONE)
						{
							RefreshTurnsList();
							owner.ChangeState<ActiveTurnState>();
						}
						else
							ActiveTurnCheck();

					}
					else if (loopPhase == Phases.ActiveTurn)
					{
						//Debug.Log("Doing Game Loop ActiveTurn AT top" + loopPhase + " " + turn.phaseStart);
						//either turn start or mid turn, if turn.phaseStart = 0 then it is turn start
						//need this because after every action, need to check for reactions and mime which can send the game loop to a different stat
						if (turn.phaseStart == 0)
						{
							//Debug.Log("changing state to Active turn state");
							if (owner.renderMode != NameAll.PP_RENDER_NONE)
							{
								//Debug.Log("Doing Game Loop ActiveTurn phaseStart" + loopPhase + " " + turn.phaseStart);
								RefreshTurnsList();
								owner.ChangeState<ActiveTurnState>();
							}
							else
								ActiveTurnCheck();
						}
						else
						{
							
							if (owner.renderMode != NameAll.PP_RENDER_NONE)
							{
								turn.phaseStart = 0;//next turn will be a full turn, this turn already acted and will complete turn below
								owner.ChangeState<CombatCommandSelectionState>();
							}
							else
							{
								//turn.phaseStart and loopPhase going to RLWait (if necessary) handled in function below
								CombatCommandSelection();
							}
						}
					}
				}
				else
				{
					loopPhase = Phases.StatusTick; //returns to the beginning of the loop
				}
			}
			else
			{
				//came here from slow action raising a quick flag, will only do the quick flags or complete the turn that was started due to a quick flag
				if (PlayerManager.Instance.QuickFlagCheckPhase() || turn.phaseStart != 0)
				{
					flagCheckPhase = Phases.ActiveTurn; //in case of reaction or mime state, the loop returns to the proper state
														//either quickflag (which is handled below), reaction flag (goes to reaction state then comes back here, mime flag (goes to mime state then returns here), or AT phase which is handled here
					loopPhase = CheckForFlag(loopPhase, turn.phaseStart);
					if (loopPhase == Phases.Quick)
					{
						loopPhase = Phases.ActiveTurn;
						if (owner.renderMode != NameAll.PP_RENDER_NONE)
						{
							RefreshTurnsList();
							owner.ChangeState<ActiveTurnState>();
						}
						else
							ActiveTurnCheck();
					}
					else if (loopPhase == Phases.ActiveTurn)
					{
						//either turn start or mid turn, if turn.phaseStart = 0 then it is turn start
						//need this because after every action, need to check for reactions and mime which can send the game loop to a different state
						if (turn.phaseStart == 0)
						{
							Debug.Log("Should never reach this part of the gameLoop code");
							if (owner.renderMode != NameAll.PP_RENDER_NONE)
								owner.ChangeState<ActiveTurnState>();
							else
								ActiveTurnCheck();
						}
						else
						{
							if (owner.renderMode != NameAll.PP_RENDER_NONE)
							{
								turn.phaseStart = 0;//next turn will be a full turn, this turn already acted and will complete turn below
								owner.ChangeState<CombatCommandSelectionState>();
							}
							else
							{
								//turn.phaseStart and loopPhase going to RLWait (if necessary) handled in function below
								CombatCommandSelection();
							}
						}
					}
				}
				else
				{
					loopPhase = lastingPhase; //returns to SlowAction
				}
			}

		}
		else if (loopPhase == Phases.Mime)
		{
			if (flagCheckPhase == Phases.ActiveTurn)
				loopPhase = Phases.ActiveTurn; //set back to active turn, if there is more 1 flag it'll come back here again
			else
				loopPhase = Phases.SlowAction;
			DoReaction(false, true, false);
		}
		else if (loopPhase == Phases.Reaction)
		{
			if (flagCheckPhase == Phases.ActiveTurn)
				loopPhase = Phases.ActiveTurn; //set back to active turn, if there is more 1 flag it'll come back here again
			else
				loopPhase = Phases.SlowAction;
			DoReaction(true, false, false);
		}
		else if (loopPhase == Phases.Quick)
		{
			//for now just moves to activeturn, active turn handles the quickflags etc
			loopPhase = Phases.ActiveTurn;
		}
		//else if (loopPhase == Phases.EndActiveTurn) //doing this after a unit ends his turn
	}

	

	//only done in non-render mode
	//in render mode this is choosing actions and targets
	void CombatCommandSelection()
	{
		//handles dead, reraise, chicken etc. if unit is dead and unable to act returns a true
		bool skipTurn = StatusManager.Instance.CheckStatusAtBeginningOfTurn(turn.actor.TurnOrder); //handles dead and reraise, not sure if this is the best place to do it
		if (skipTurn)
		{
			PlayerManager.Instance.EndCombatTurn(turn, true);
			return;
		}

		turn.CheckStatuses(); //allow act/move based on statuses

		if (!UnitHasControl()) //stop, sleep, petrify; Dead midturn (dead at beginning of turn handled in AT)
		{
			//Debug.Log("entering combatcommadnselection unit does not have control, ending turn");
			PlayerManager.Instance.EndCombatTurn(turn);
		}
		else if (StatusManager.Instance.IsAIControlledStatus(turn.actor.TurnOrder) || driver == Drivers.Computer)
		{
			//StartCoroutine(AIStatusTurn());
			ComputerTurn();
		}
		else if( driver == Drivers.BlackBoxRL)
		{
			BlackBoxRLTurn();
		}
		else if (driver == Drivers.ReinforcementLearning)
		{
			// action needed from RL agent
			if (loopPhase != Phases.RLWait) //probably redundant but no harm
			{
				loopPhase = Phases.RLWait;
				this.PostNotification(RLRequestAction, turn);
			}			
			//Debug.Log("CombatComandSelectionState, sending notification to RLRequestAction");
		}
		else
		{
			Debug.Log("BUG: combatcommandselection, should never reach here");
		}
	}


	#region ReinforcementLearning
	/// <summary>
	/// Read action from the Avatar and update the env with that action
	/// </summary>
	void BlackBoxRLTurn()
	{
		// continually hit here until the action is ready. might make sense to change this to a notification style system
		if(this.owner.worldTimeManager.agentSession.avatar.GetNextActionState() == AurelianTactics.BlackBoxRL.NextActionState.Ready)
		{
			List<int> actionList = new List<int>(this.owner.worldTimeManager.agentSession.avatar.actionList);
			this.owner.worldTimeManager.agentSession.avatar.SetNextActionState(AurelianTactics.BlackBoxRL.NextActionState.InProgress);
			ExecuteBlackBoxAction(actionList);
			//to do: after action  executed, move to next thing
		}
	}


	// execute turn or wait if turn is not valid
	void ExecuteBlackBoxAction(List<int> aList)
	{
		//wait
		EndActiveTurn();

	}

	// check that turn is valid
	bool IsValidTurn(CombatTurn tu)
	{
		//other checks done in ExecuteBlackBoxTurn
		if (tu.plan ==  null || (tu.plan.isActFirst && tu.plan.spellName == null))
			return false;

		return true;
	}


	void ComputerTurn()
	{

		//Debug.Log("in computer turn 0");
		if (turn.plan == null)
		{
			if (turn.hasUnitActed == false)
			{
				if (StatusManager.Instance.IfStatusByUnitAndId(turn.actor.TurnOrder, NameAll.STATUS_ID_CHARGING))
				{
					turn.plan = owner.cpu.ContinueChargingPerforming();
					turn.hasUnitActed = true;
				}
				else if (StatusManager.Instance.IfStatusByUnitAndId(turn.actor.TurnOrder, NameAll.STATUS_ID_PERFORMING))
				{
					if (UnityEngine.Random.Range(0, 3) == 0) //2/3 chance to continue performing
					{
						//cancel AI charging
						SpellManager.Instance.RemoveSpellSlowByUnitId(turn.actor.TurnOrder);
						StatusManager.Instance.RemoveStatus(turn.actor.TurnOrder, NameAll.STATUS_ID_CHARGING);
						StatusManager.Instance.RemoveStatus(turn.actor.TurnOrder, NameAll.STATUS_ID_PERFORMING);
					}
					else
					{
						turn.plan = owner.cpu.ContinueChargingPerforming();
						turn.hasUnitActed = true;
					}
				}
			}

			if (turn.plan == null)
				turn.plan = owner.cpu.Evaluate();
			turn.spellName = turn.plan.spellName;
		}

		//Debug.Log("in computer turn 0.5");
		if ((turn.plan.isActFirst && turn.hasUnitActed == false && turn.plan.spellName != null) ||
			(!turn.plan.isActFirst && turn.hasUnitActed == false && turn.plan.spellName != null))
		{
			//Debug.Log("in computer turn 1");
			RLActPhase();
		}
		else if (turn.hasUnitMoved == false && turn.plan.moveLocation != board.GetTile(turn.actor).pos)
		{
			//Debug.Log("in computer turn 2");
			RLMovePhase();
		}
		else
		{
			//Debug.Log("in computer turn 4");
			EndActiveTurn();
		}
		loopPhase = Phases.ActiveTurn;
	}

	// notifications
	const string RLSendAction = "ReinforcementLearning.SendAction"; //get action from RL
	const string RLRequestAction = "ReinforcementLearning.RequestAction";  //send request for action to RL

	//listen for action from RL agent
	void OnRLSendAction(object sender, object args)
	{
		CombatPlanOfAttack plan = (CombatPlanOfAttack)args;
		//convert actionArray into a plan
		turn.plan = plan;
		turn.spellName = plan.spellName;
		turn.targetTile = board.GetTile(plan.fireLocation);

		if (turn.plan.isEndTurn)
		{
			EndActiveTurn();
		}
		else if (turn.hasUnitMoved == false && turn.plan.moveLocation != board.GetTile(turn.actor).pos)
		{
			//Debug.Log("in RL turn 2");
			RLMovePhase();
		}
		else if ( (turn.plan.isActFirst && turn.hasUnitActed == false && turn.plan.spellName != null)
			|| (!turn.plan.isActFirst && turn.hasUnitActed == false && turn.plan.spellName != null) )
		{
			//Debug.Log("in RL turn 1");
			RLActPhase();		
		}
		else
		{
			//Debug.Log("in RL turn 4");
			EndActiveTurn();
		}
		loopPhase = Phases.ActiveTurn;
	}

	void RLActPhase()
	{
		owner.calcMono.DoFastAction(board, turn, isActiveTurn: true, isReaction: false, isMime: false, renderMode: owner.renderMode);
		if (turn.spellName.CommandSet == NameAll.COMMAND_SET_JUMP)
		{
			PlayerManager.Instance.SetFacingDirectionMidTurn(turn.actor.TurnOrder, board.GetTile(turn.actor), turn.targetTile);
			PlayerManager.Instance.EndCombatTurn(turn);
			turn.phaseStart = 0; //think this is redundant but keeps it from coming back to this unit mid turn
		}
		else
		{
			turn.phaseStart = 1;
		}
	}

	void RLMovePhase()
	{
		//Similar to CombatMoveSequenceState
		//in classic has to handle: teleports, crystals, and traps (not implemented)
		//in non-classic not implemented
		loopPhase = Phases.ActiveTurn;
		bool isClassicClass = NameAll.IsClassicClass(turn.actor.ClassId);
		bool confirmMove = true;
		int moveEffect = 0;

		if (isClassicClass)
		{
			if (turn.actor.AbilityMovementCode == NameAll.MOVEMENT_TELEPORT_1)
			{
				int rollChance = 100 + (turn.actor.StatTotalMove - MapTileManager.Instance.GetDistanceBetweenTiles(turn.actor.TileX, turn.actor.TileY, owner.currentTile.pos.x, owner.currentTile.pos.y)) * 10;
				int roll = UnityEngine.Random.Range(1, 101);
				PlayerManager.Instance.AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_MOVE, NameAll.COMBAT_LOG_SUBTYPE_MOVE_TELEPORT_ROLL, rollResult:roll, rollChance:rollChance);
				if (roll > rollChance)
				{
					confirmMove = false;
					//Debug.Log("need to log this somewhere");
					//owner.battleMessageController.Display("Teleport fails!");
					//if (!isOffline)
					//	PlayerManager.Instance.SendMPBattleMessage("Teleport fails!");
				}
			}
		}
		else
		{
			//Not impelmented yet, need to set moveEffect
		}

		if (confirmMove)
		{
			PlayerManager.Instance.SetUnitTile(board, turn.actor.TurnOrder, owner.currentTile, isAddCombatLog:true);
			DoTilePickUp(owner.renderMode);
			if (turn.actor.IsOnMoveEffect())
			{
				//Debug.Log("just prior to CalcResovleAction move effect is " + moveEffect);
				CalculationResolveAction.CreateSpellReactionMove(turn.actor.ClassId, turn.actor.AbilityMovementCode, turn.actor.TurnOrder, moveEffect);
				DoReaction(false, false, true);
				PlayerManager.Instance.AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_MOVE, NameAll.COMBAT_LOG_SUBTYPE_MOVE_EFFECT, cTurn: turn, effectValue: turn.actor.AbilityMovementCode);
			}
		}
		turn.phaseStart = 1;
	}
	#endregion

	void DoTilePickUp(int renderMode)
	{
		Tile t = board.GetTile(turn.actor);
		if (t.PickUpId == 1) //crystal
		{
			if (!isOffline)
				PlayerManager.Instance.SendMPRemoveTilePickUp(t.pos.x, t.pos.y, false, t.PickUpId);

			board.SetTilePickUp(turn.actor.TileX, turn.actor.TileY, false, renderMode, t.PickUpId);
			//fully restores HP and MP
			PlayerManager.Instance.AlterUnitStat(NameAll.REMOVE_STAT_HEAL, turn.actor.StatTotalMaxLife, NameAll.STAT_TYPE_HP, turn.actor.TurnOrder, 
				combatLogSubType:NameAll.COMBAT_LOG_SUBTYPE_CRYSTAL_PICK_UP);
			PlayerManager.Instance.AlterUnitStat(NameAll.REMOVE_STAT_HEAL, turn.actor.StatTotalMaxMP, NameAll.STAT_TYPE_MP, turn.actor.TurnOrder,
				combatLogSubType: NameAll.COMBAT_LOG_SUBTYPE_CRYSTAL_PICK_UP);
		}
	}

	void EndActiveTurn()
	{
		PlayerManager.Instance.EndCombatTurn(turn);
		StatusManager.Instance.CheckStatusAtEndOfTurn(turn.actor.TurnOrder);
		turn.phaseStart = 0;
	}

	bool UnitHasControl()
	{
		return false;
	}

	//only done in non-render mode. a few pre-checks before letting an active turn proceed or not
	void ActiveTurnCheck()
	{
		PlayerUnit pu = PlayerManager.Instance.GetNextActiveTurnPlayerUnit(isSetQuickFlagToFalse: true);//sets quickFlag to false if it was true

		if (pu != null) //probably an unnecessary check
		{
			turn.Change(pu);
			CombatCommandSelection();
		}
	}

	void DoSlowAction(SpellSlow ss)
	{
		if (!StatusManager.Instance.IsTurnActable(ss.UnitId))
		{
			//unit can't go, remove the spell slow
			SpellManager.Instance.RemoveSpellSlowByObject(ss);
			PlayerManager.Instance.AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_SLOW_ACTION, NameAll.COMBAT_LOG_SUBTYPE_SLOW_ACTION_UNABLE_TO_CAST, ss);
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
				string battleMessage = ssTurn.spellName.AbilityName + " is cast!";
				StartCoroutine(HighlightAction(ssTurn, targetTile, battleMessage));
			}

			//do the fast action
			ssTurn.spellName = CalculationAT.ModifySlowActionSpellName(ssTurn.spellName, ssTurn.actor.TurnOrder);
			StatusManager.Instance.CheckChargingJumping(ssTurn.actor.TurnOrder, ssTurn.spellName);

			owner.calcMono.DoFastAction(board, ssTurn, isActiveTurn: false, isReaction: false, isMime: false, renderMode: owner.renderMode);
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

			if (owner.renderMode != NameAll.PP_RENDER_NONE)
			{
				StartCoroutine(UnHighlightAction(ssTurn, true, startDir));
				SpellManager.Instance.RemoveSpellSlowByObject(ss);
			}
			else
			{
				SpellManager.Instance.RemoveSpellSlowByObject(ss); //i'm not sure why I remove the SS here and not earlier
				PlayerManager.Instance.GetPlayerUnit(ss.UnitId).Dir = startDir;
			}
		}

	}

	void DoReaction(bool isReaction, bool isMime, bool isMovement)
	{
		SpellReaction sr;
		if(isMime)
			sr = SpellManager.Instance.GetNextMimeQueue();
		else
			sr = SpellManager.Instance.GetNextSpellReaction();
		if (sr != null)
		{
			SpellName sn = SpellManager.Instance.GetSpellNameByIndex(sr.SpellIndex);
			CombatTurn ssTurn = new CombatTurn(sr, board, isMimeSpellReaction: isMime);
			string battleMessage;
			if (isMime)
				battleMessage = "Mimic: " + ssTurn.spellName.AbilityName;
			else if( isMovement)
				battleMessage = "Move: " + ssTurn.spellName.AbilityName;
			else
				battleMessage = "Counter: " + ssTurn.spellName.AbilityName;

			if (owner.renderMode != NameAll.PP_RENDER_NONE)
				StartCoroutine(HighlightAction(ssTurn, ssTurn.targetTile, battleMessage));

			if (!isOffline)
			{ //online game, sends details to Other for display
				int targetX = sr.TargetX;
				int targetY = sr.TargetY;
				if (sr.TargetX == NameAll.NULL_INT || sr.TargetY == NameAll.NULL_INT)
				{
					PlayerUnit tempUnit = PlayerManager.Instance.GetPlayerUnit(sr.TargetId);
					targetX = tempUnit.TileX;
					targetY = tempUnit.TileY;
				}
				SpellManager.Instance.SendReactionDetails(sr.ActorId, sr.SpellIndex, targetX, targetY, battleMessage);
			}

			//modify the spellName for reaction
			ssTurn.spellName = CalculationAT.ModifyReactionSpellName(sr, ssTurn.actor.TurnOrder);

			owner.calcMono.DoFastAction(board, ssTurn, isActiveTurn: false, isReaction: isReaction, isMime: isMime, renderMode: owner.renderMode);
			if (isMime)
				SpellManager.Instance.RemoveMimeQueueByObject(sr);
			else
				SpellManager.Instance.RemoveSpellReactionByObject(sr);
			if (owner.renderMode != NameAll.PP_RENDER_NONE)
				UnHighlightAction(ssTurn, false, Directions.East);
		}
	}


	IEnumerator HighlightAction(CombatTurn ssTurn, Tile targetTile, string battleMessage)
	{
		if (targetTile != null)
			cameraMain.MoveToMapTile(targetTile);
		else
			cameraMain.MoveToMapTile(PlayerManager.Instance.GetPlayerUnitTile(board, ssTurn.actor.TurnOrder));
		HighlightActorTile(ssTurn.actor, true);
		owner.battleMessageController.Display(battleMessage); //this.PostNotification(DidSlowActionResolve); //Debug.Log("Send message to spell title panel");
		board.SelectTiles(ssTurn.targets); //Debug.Log("number of targets is " + ssTurn.targets.Count);//highlight the target tiles
		yield return new WaitForSeconds(0.75f);
	}

	IEnumerator UnHighlightAction(CombatTurn ssTurn, bool isTurnPU, Directions startDir)
	{
		HighlightActorTile(ssTurn.actor, false); //unhighlight
		board.DeSelectTiles(ssTurn.targets); //deselect the tiles
		yield return new WaitForSeconds(0.5f);
		if (isTurnPU)
		{
			PlayerManager.Instance.SetPUODirectionMidTurn(ssTurn.actor.TurnOrder, startDir);
			PlayerManager.Instance.GetPlayerUnit(ssTurn.actor.TurnOrder).Dir = startDir;
		}
		
		yield return null;
	}

	#endregion

	public bool IsOpponentReady()
    {
        return PlayerManager.Instance.IsOpponentInStandbyAndReady();
    }

    public void ExecuteMPGameLoop()
    {
        if (!IsOpponentReady())
        {
            //Debug.Log("opponent isn't ready");
            return;
        }

        if (IsBattleOver())
        {
            owner.ChangeState<CombatCutSceneState>();
            return;
        }
            

        if (loopPhase == Phases.StatusTick)
        {
            PlayerManager.Instance.SendMPPhase(loopPhase);
            lastingPhase = Phases.StatusTick;
            loopPhase = Phases.SlowActionTick; //Debug.Log("doing statusTick decrement phase");
			//owner.ChangeState<StatusCheckState>(); //decrements all lasting statuses, if any expire update combat log and show them expiring
			StatusManager.Instance.StatusCheckPhase(); //decrements all lasting statuses, if any expire update combat log and show them expiring.
		}
        else if (loopPhase == Phases.SlowActionTick)
        {
            //no seperate phase or game state, decrements Slow Action spells and sends an RPC to Other to do the same
            lastingPhase = Phases.SlowActionTick;
            SpellManager.Instance.SlowActionTickPhase();
            loopPhase = Phases.SlowAction;
        }
        else if (loopPhase == Phases.SlowAction)
        {
            lastingPhase = Phases.SlowAction;
            //check for quick. mime, and counter flags
            flagCheckPhase = Phases.SlowAction;
            loopPhase = CheckForFlag(loopPhase);
            if (loopPhase == Phases.SlowAction)
            {
                if (SpellManager.Instance.GetNextSlowAction() != null)
                {
                    owner.ChangeState<SlowActionState>();
                }
                else
                {
                    loopPhase = Phases.CTIncrement;
                }
            }
        }
        else if (loopPhase == Phases.CTIncrement)
        {
            lastingPhase = Phases.CTIncrement;
            PlayerManager.Instance.IncrementCTPhase();
            loopPhase = Phases.ActiveTurn;

            //loopPhase = CheckForFlag(loopPhase); //not sure if check is needed
            //if( loopPhase == Phases.CTIncrement)
            //{
            //PlayerManager.Instance.IncrementCTPhase();
            //loopPhase = Phases.ActiveTurn;
            //lastingPhase = Phases.StatusTick; //starts the loop over after all activeturns are done, is a check in ActiveTurn to make sure they are all done before going to lasting phase
            //} 
        }
        else if (loopPhase == Phases.ActiveTurn)
        {
            //turn reached from coming from proper phase
            if (lastingPhase == Phases.CTIncrement || lastingPhase == Phases.ActiveTurn)
            {
                lastingPhase = Phases.ActiveTurn;
                if (CheckForActiveTurn())
                {
                    flagCheckPhase = Phases.ActiveTurn; //in case of reaction or mime state, the loop returns to the proper state
                    //either quickflag (which is handled below), reaction flag (goes to reaction state then comes back here, mime flag (goes to mime state then returns here), or AT phase which is handled here
                    loopPhase = CheckForFlag(loopPhase, turn.phaseStart);
                    if (loopPhase == Phases.Quick)
                    {
                        loopPhase = Phases.ActiveTurn;
                        owner.ChangeState<ActiveTurnState>();
                    }
                    else if (loopPhase == Phases.ActiveTurn)
                    {
                        //either turn start or mid turn, if turn.phaseStart = 0 then it is turn start
                        //need this because after every action, need to check for reactions and mime which can send the game loop to a different stat
                        if (turn.phaseStart == 0)
                        {
                            //Debug.Log("changing state to Active turn state");
                            owner.ChangeState<ActiveTurnState>();
                        }
                        else
                        {
                            turn.phaseStart = 0;//next turn will be a full turn, this turn already acted and will complete turn below
                            owner.ChangeState<CombatCommandSelectionState>();
                        }
                    }
                }
                else
                {
                    loopPhase = Phases.StatusTick; //returns to the beginning of the loop
                }
            }
            else
            {
                //came here from slow action raising a quick flag, will only do the quick flags or complete the turn that was started due to a quick flag
                if (PlayerManager.Instance.QuickFlagCheckPhase() || turn.phaseStart != 0)
                {
                    flagCheckPhase = Phases.ActiveTurn; //in case of reaction or mime state, the loop returns to the proper state
                    //either quickflag (which is handled below), reaction flag (goes to reaction state then comes back here, mime flag (goes to mime state then returns here), or AT phase which is handled here
                    loopPhase = CheckForFlag(loopPhase, turn.phaseStart);
                    if (loopPhase == Phases.Quick)
                    {
                        loopPhase = Phases.ActiveTurn;
                        owner.ChangeState<ActiveTurnState>();
                    }
                    else if (loopPhase == Phases.ActiveTurn)
                    {
                        //either turn start or mid turn, if turn.phaseStart = 0 then it is turn start
                        //need this because after every action, need to check for reactions and mime which can send the game loop to a different state
                        if (turn.phaseStart == 0)
                        {
                            owner.ChangeState<ActiveTurnState>(); Debug.Log("Should never reach this part of the gameLoop code");
                        }
                        else
                        {
                            turn.phaseStart = 0;//next turn will be a full turn, this turn already acted and will complete turn below
                            owner.ChangeState<CombatCommandSelectionState>();
                        }
                    }
                }
                else
                {
                    loopPhase = lastingPhase; //returns to SlowAction
                }
            }

        }
        else if (loopPhase == Phases.Mime)
        {
            if (flagCheckPhase == Phases.ActiveTurn)
                loopPhase = Phases.ActiveTurn; //set back to active turn, if there is more 1 flag it'll come back here again
            else
                loopPhase = Phases.SlowAction;
            owner.ChangeState<MimeState>();
        }
        else if (loopPhase == Phases.Reaction)
        {
            if (flagCheckPhase == Phases.ActiveTurn)
                loopPhase = Phases.ActiveTurn; //set back to active turn, if there is more 1 flag it'll come back here again
            else
                loopPhase = Phases.SlowAction;
            owner.ChangeState<ReactionState>();
        }
        else if (loopPhase == Phases.Quick)
        {
            //for now just moves to activeturn, active turn handles the quickflags etc
            loopPhase = Phases.ActiveTurn;
        }
        //else if (loopPhase == Phases.EndActiveTurn) //doing this after a unit ends his turn
        //{

        //}

    }

 
    //counterflag goes first, then mimeflag, then quickflag
    Phases CheckForFlag(Phases currentPhase, int midActiveTurn = 0)
    {
        //Debug.Log("checking for flag");
        if (SpellManager.Instance.IsSpellReaction() )
        {
            //Debug.Log("checking for flag is reaction");
            return Phases.Reaction;
        }
        else if (SpellManager.Instance.GetNextMimeQueue() != null)
        {
            return Phases.Mime;
        }
        else if (PlayerManager.Instance.QuickFlagCheckPhase() && midActiveTurn == 0)
        {
            //mid turn indicator is for ActiveTurns, can't jump from a midActiveTurn active turn into a Quick turn  
            return Phases.Quick;
        }
        return currentPhase;
    }

    bool CheckForActiveTurn()
    {
        if( PlayerManager.Instance.GetNextActiveTurnPlayerUnit(isSetQuickFlagToFalse:false) != null) //don't set quickflag to false, only do that when getting the active unit
        {
            return true;
        }
        return false;
    }


	#region notifications
	//listeners that need to be able to be called from anywhere like StatusManager and crystalize
	const string DidStatusManager = "StatusManager.Did";
    const string CombatMenuAdd = "CombatMenu.AddItem";
    //const string MultiplayerGameLoop = "Multiplayer.GameLoop"; //called from PlayerManager for Other (p2)
    //const string MultiplayerSpellSlow = "Multiplayer.SpellSlow";
    //const string MultiplayerReaction = "Multiplayer.Reaction";//mime and reaction
    //const string MultiplayerActiveTurnPreTurn = "Multiplayer.ActiveTurnPreTurn";//active turn start, show whose turn it is
    //const string MultiplayerActiveTurnMidTurn = "Multiplayer.ActiveTurnMidTurn";//active turn mid-turn. Master raises it after other has told results of input to Master and vice versa
    //const string MultiplayerCommandTurn = "Multiplayer.CommandTurn";//other gets to input an action
    //const string MultiplayerMove = "Multiplayer.Move";//other gets the actual move that happens and starts the move
    //const string MultiplayerDisableUnit = "Multiplayer.DisableUnit";//does the call to board to disable the unit
    //const string MultiplayerTilePickUp = "Multiplayer.RemoveTilePickUp";
    //const string MultiplayerGameOver = "Multiplayer.GameOver";
    //const string MultiplayerMessageNotification = "Multiplayer.Message";

    void OnEnable()
    {
        this.AddObserver(OnStatusManagerNotification, DidStatusManager);
        this.AddObserver(OnQuitNotification, NameAll.NOTIFICATION_EXIT_GAME);
		this.AddObserver(OnResetNotification, NameAll.NOTIFICATION_RESET_GAME);
		this.AddObserver(OnRLResetGame, RLResetGame);
	}

    void OnDisable()
    {
        DirectRemoveObservers();
    }

    void DirectRemoveObservers()
    {
        this.RemoveObserver(OnStatusManagerNotification, DidStatusManager);
        this.RemoveObserver(OnQuitNotification, NameAll.NOTIFICATION_EXIT_GAME);
		this.RemoveObserver(OnResetNotification, NameAll.NOTIFICATION_RESET_GAME);
		this.RemoveObserver(OnRLResetGame, RLResetGame);
	}

    void EnableObservers()
    {
		//multiplayer deprecated for now
		//if (!PlayerManager.Instance.IsOfflineGame() && !PlayerManager.Instance.isMPMasterClient())
		//{
		//    this.AddObserver(OnMultiplayerPhaseNotification, MultiplayerGameLoop);
		//    this.AddObserver(OnMultiplayerSpellSlowNotification, MultiplayerSpellSlow);
		//    this.AddObserver(OnMultiplayerReactionNotification, MultiplayerReaction);
		//    this.AddObserver(OnMultiplayerActiveTurnPreTurnNotification, MultiplayerActiveTurnPreTurn);
		//    this.AddObserver(OnMultiplayerCommandTurnNotification, MultiplayerCommandTurn);
		//    this.AddObserver(OnMultiplayerTurnInputNotification, MultiplayerActiveTurnMidTurn);
		//    this.AddObserver(OnMultiplayerMoveNotification, MultiplayerMove);
		//    this.AddObserver(OnMultiplayerDisableUnitNotification, MultiplayerDisableUnit);
		//    this.AddObserver(OnMultiplayerRemoveTilePickUpNotification, MultiplayerTilePickUp);
		//    this.AddObserver(OnMultiplayerGameOver, MultiplayerGameOver);
		//    this.AddObserver(OnMultiplayerBattleMessageControllerNotification, MultiplayerMessageNotification);
		//}
	}

	void DisableObservers()
    {
		//multiplayer deprecated for now
        //if (!PlayerManager.Instance.IsOfflineGame() && !PlayerManager.Instance.isMPMasterClient())
        //{
        //    this.RemoveObserver(OnMultiplayerPhaseNotification, MultiplayerGameLoop);
        //    this.RemoveObserver(OnMultiplayerSpellSlowNotification, MultiplayerSpellSlow);
        //    this.RemoveObserver(OnMultiplayerReactionNotification, MultiplayerReaction);
        //    this.RemoveObserver(OnMultiplayerActiveTurnPreTurnNotification, MultiplayerActiveTurnPreTurn);
        //    this.RemoveObserver(OnMultiplayerCommandTurnNotification, MultiplayerCommandTurn);
        //    this.RemoveObserver(OnMultiplayerTurnInputNotification, MultiplayerActiveTurnMidTurn);
        //    this.RemoveObserver(OnMultiplayerMoveNotification, MultiplayerMove);
        //    this.RemoveObserver(OnMultiplayerDisableUnitNotification, MultiplayerDisableUnit);
        //    this.RemoveObserver(OnMultiplayerRemoveTilePickUpNotification, MultiplayerTilePickUp);
        //    this.RemoveObserver(OnMultiplayerGameOver, MultiplayerGameOver);
        //    this.RemoveObserver(OnMultiplayerBattleMessageControllerNotification, MultiplayerMessageNotification);
        //}
    }

    void OnStatusManagerNotification(object sender, object args)
    {
        //string str = (string)args;
        //object sent is a list of ints first is the statusId, 2nd is the playerId
        List<int> tempList = args as List<int>;
        if (tempList[0] == NameAll.STATUS_ID_CRYSTAL) //str.Equals(NameAll.STATUS_NAME_CRYSTAL)
        {
       
            if ( !isOffline && isMasterClient) //sends result to other
                PlayerManager.Instance.SendMPCrystalOutcome(tempList);

            PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(tempList[1]);

			
            
            if (tempList[2] == 1)
            {//roll for crystal occurs in StatusManager
                //turn object holds the player shit
                board.SetTilePickUp(pu.TileX, pu.TileY, true, owner.renderMode, 1);
                CombatLogClass cll2 = new CombatLogClass("time runs to 0 and turns to crystal", pu.TurnOrder, PlayerManager.Instance.GetRenderMode());
                cll2.SendNotification();
            }
			else
			{
				CombatLogClass cll = new CombatLogClass("time runs to 0 and disappears", pu.TurnOrder, PlayerManager.Instance.GetRenderMode());
				cll.SendNotification();
			}
            board.DisableUnit(pu);
            PlayerManager.Instance.DisableUnit(pu.TurnOrder);
        }
    }

    void OnQuitNotification(object sender, object args)
    {
        //Debug.Log("quitting game due to notification 0");
        DirectRemoveObservers(); //want to remove observers
        if (!isOffline)
        {
            //Debug.Log("quitting game due to notification 1");
            bool isSelfQuit = (bool)args;
            if(isSelfQuit) //this player quit. let other player know and end game
            {
                //Debug.Log("quitting game by self quit");
                PlayerManager.Instance.SendMPQuitGame( isSelfQuit);
                owner.ChangeState<CombatEndState>();
            }
            else //other playerquit, move to CombatCutSceneState
            {
                //Debug.Log("quitting game because other player quit");

                if ( isMasterClient)
                    owner.GetComponent<CombatVictoryCondition>().Victor = Teams.Team1;
                else
                    owner.GetComponent<CombatVictoryCondition>().Victor = Teams.Team2;

                owner.ChangeState<CombatCutSceneState>();
            }
        }
        else
        {
            owner.ChangeState<CombatEndState>();
        }     
    }


	void OnResetNotification(object sender, object args)
	{
		//Debug.Log("resetting game due to notification 0");
		if (isOffline)
		{
			//reset the victory conditions
			owner.GetComponent<CombatVictoryCondition>().Victor = Teams.None;
			//reset the board, units etc
			calcMono.ResetLevel(board, PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_GREEN), PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_RED),
				owner.markerTeam1Prefab, owner.markerTeam2Prefab, isOffline, owner.isRLMode, false, owner.renderMode);
		}
	}


	void OnRLResetGame(object sender, object args)
	{
		//Debug.Log("batte is over 5");
		owner.ChangeState<CombatStateInit>();
	}

	//posting notifications

	//refresh the turns menu after certain game loop things. basically after a slow action resolves or before an active turn
	void RefreshTurnsList()
	{
		if( owner.renderMode != NameAll.PP_RENDER_NONE)
		{
			//Debug.Log("sending refresh turns notification");
			this.PostNotification(refreshTurns);
		}
			
	}


	#endregion

	/*
	 * multiplayer deprecated for now
	 * 
	   //activeturn: show whose turn it is
    void OnMultiplayerActiveTurnPreTurnNotification(object sender, object args)
    {
        int unitId = (int)args;
        PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(unitId);
        if( pu != null)
        {
            turn.Change(pu); //resets the turn. when P2 acts out a full turn, it remembers that it moved/acted before for menuupdates 
            SelectTile(pu);
            HighlightActorTile(pu, true);
            actorPanel.SetActor(turn.actor);
            Tile targetTile = board.GetTile(pu);
            cameraMain.MoveToMapTile(targetTile);
            
            if (pu.TeamId == NameAll.TEAM_ID_GREEN)
            {
                owner.battleMessageController.Display("Opponent Turn for: " + pu.UnitName);
                StartCoroutine(UnhighlightSpellSlowCoroutine(pu, null, 1.5f));
            }
            else
                owner.battleMessageController.Display("Your Turn for: " + pu.UnitName);
        }   
    }

	
    IEnumerator UnhighlightSpellSlowCoroutine(PlayerUnit actor, List<Tile> targetTiles, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        HighlightActorTile(actor, false); //unhighlight
        if( targetTiles != null)
            board.DeSelectTiles(targetTiles); //deselect the tiles
    }
	
	void OnMultiplayerPhaseNotification(object sender, object args)
    {
        Phases phase = (Phases)args;
        if (phase == Phases.StatusTick)
        {
			//Master already knows not ready, don't update readiness to Master until coming back from this state
			//PlayerManager.Instance.SetMPSelfStatusAndPhase(false,phase);
			//owner.ChangeState<StatusCheckState>(); //decrements all lasting statuses, if any expire update combat log and show them expiring
			StatusManager.Instance.StatusCheckPhase(); //decrements all lasting statuses, if any expire update combat log and show them expiring.
		}
        //else if( phase == Phases.SlowActionTick) //Phases.CTIncrement
        //{
        //    //not needed, Master sends SpellManager RPC for Other to decrement SlowActionTIck
        //    //for CT increment, Master sends PlayerManager RPC to other for increment CT
        //}
        //else if (phase == Phases.SlowAction) //Phases.MimeAction //Phases.MimeReaction //Phases.Quick
        //{
            //Master does all the calculations, sends RPC for Other to followe the action
            //rather than have Other follow Master through all the states that can be bounced around (SlowAction->Reaction->Mime->Quick-> etc), instead mimics the results/displays
            //Other takes RPCs for:
            //Name of Spell and what tiles it hits
            //any outputs that effect PlayerUnits (dmg, statuses, etc) or PlayerUnitObjects (turning, dieing, statuses, etc)
        //}
        //else if(phase == Phases.
    }

    //pre reaction, mime cast, and on movement effect
    void OnMultiplayerReactionNotification(object sender, object args)
    {
        //        Other gets camera move
        //Other gets battlemessenger display
        //Other gets tiles that it hits display (caster and target)

        ReactionDetails rd = (ReactionDetails)args;
        if (rd != null)
        {
            owner.battleMessageController.Display(rd.DisplayName);

            Tile targetTile = board.GetTile(rd.TargetX, rd.TargetY);
            cameraMain.MoveToMapTile(targetTile);
            PlayerUnit actor = PlayerManager.Instance.GetPlayerUnit(rd.ActorId);
            HighlightActorTile(actor, true);
            
            CombatAbilityArea caa = new CombatAbilityArea();
            List<Tile> targetList = caa.GetTilesInArea(board, actor, SpellManager.Instance.GetSpellNameByIndex(rd.SpellIndex), targetTile, actor.Dir);
            board.SelectTiles(targetList);
            StartCoroutine(UnhighlightSpellSlowCoroutine(actor,targetList,0.5f));
        }

    }

	//pre spellslow cast
    void OnMultiplayerSpellSlowNotification(object sender, object args)
    {
        //        Other gets camera move
        //Other gets battlemessenger display
        //Other gets tiles that it hits display(caster and target)
        
        SpellSlow ss = (SpellSlow)args; 
        if( ss!= null)
        {
            Tile targetTile = owner.calcMono.GetCameraTargetTile(board,ss);
            cameraMain.MoveToMapTile(targetTile);
            CombatTurn ssTurn = new CombatTurn(ss, board);
            HighlightActorTile(ssTurn.actor, true);
            owner.battleMessageController.Display(ssTurn.spellName.AbilityName + " is cast!"); //this.PostNotification(DidSlowActionResolve); //Debug.Log("Send message to spell title panel");
            board.SelectTiles(ssTurn.targets);

            StartCoroutine(UnhighlightSpellSlowCoroutine(ssTurn.actor,ssTurn.targets,1.1f));
        }
        
    }

    //Other now gets to input a command. moves to CombatCommandSelectionState (after updating what the actor can do)
    void OnMultiplayerCommandTurnNotification(object sender, object args)
    {
        CombatTurn tempTurn = (CombatTurn)args;
        if( tempTurn != null)
        {
            turn.hasUnitActed = tempTurn.hasUnitActed;
            turn.hasUnitMoved = tempTurn.hasUnitMoved;
            owner.ChangeState<CombatCommandSelectionState>();
        }
    }

		//Master has sent other an RPC in PlayerManager with master's turn. display update here, effect update elsewhere
    void OnMultiplayerTurnInputNotification(object sender, object args)
    {
        CombatMultiplayerTurn cmt = (CombatMultiplayerTurn)args;
        if (cmt != null)
        {
            if (cmt.IsWait)
            {
                turn.endDir = DirectionsExtensions.IntToDirection(cmt.DirectionInt);
                StartCoroutine(DoMPWait());
            }
            else if (cmt.IsMove)
            {
                StartCoroutine(DoMPMove(cmt.TileX, cmt.TileY));
            }
            else if (cmt.IsAct)
            {
                StartCoroutine(DoMPAct(cmt));
            }
        }
    }
	
	IEnumerator DoMPWait()
    {
        owner.facingIndicator.gameObject.SetActive(true);
        owner.facingIndicator.SetDirection(turn.actor.Dir);
        yield return new WaitForSeconds(0.5f);
        owner.facingIndicator.gameObject.SetActive(false);
    }

    IEnumerator DoMPMove(int moveX, int moveY)
    {
        //move: show selected tile (actual move done by RPC)
        Point cursorPos = new Point(moveX, moveY);
        SelectTile(cursorPos); //sets pos to this tile
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator DoMPAct(CombatMultiplayerTurn cmt)
    {
        //show what tile was targetted
        turn.targetUnitId = cmt.TargetId;
        if (cmt.TargetId != NameAll.NULL_UNIT_ID)
        {
            turn.targetTile = board.GetTile(PlayerManager.Instance.GetPlayerUnit(cmt.TargetId));
            SelectTile(PlayerManager.Instance.GetPlayerUnit(turn.targetUnitId));
        }
        else
        {
            Point cursorPos = new Point(cmt.TileX, cmt.TileY);
            SelectTile(cursorPos);
            turn.targetTile = board.GetTile(cursorPos);
        }

        //show AOE
        CombatAbilityArea aa = new CombatAbilityArea();
        PlayerUnit actor = PlayerManager.Instance.GetPlayerUnit(cmt.ActorId);
        SpellName sn = SpellManager.Instance.GetSpellNameByIndex(cmt.SpellIndex); //this has to be a spellIndex or something has gone wrong
        Directions dir = DirectionsExtensions.IntToDirection(cmt.DirectionInt);
        List<Tile> tiles = aa.GetTilesInArea(board, actor, sn, turn.targetTile, dir);
        board.SelectTiles(tiles);

        turn.actor = actor;
        turn.spellName = sn;

        if (cmt.SpellIndex2 != NameAll.NULL_INT)
            turn.spellName2 = SpellManager.Instance.GetSpellNameByIndex(cmt.SpellIndex2);
        else
            turn.spellName2 = null;

        //show targetPanel pop up
        targetPanel.SetTargetPreview(turn.targetTile);
        //show hit rate
        List<string> strList = CalculationAT.GetHitPreview(board, turn, turn.targetTile);
        previewPanel.SetHitPreview(strList[0], strList[1], strList[2], strList[3], strList[4]);

        yield return new WaitForSeconds(1.0f);
        targetPanel.Close();
        previewPanel.Close();
        board.DeSelectTiles(tiles);
    }

		
	//master has sent an RPC to other with the actual move that was performed
	//void OnMultiplayerMoveNotification(object sender, object args)
	//{
	//    CombatMultiplayerMove cmm = (CombatMultiplayerMove)args;
	//    if (cmm != null)
	//    {
	//        //Debug.Log("other has received the move notification");
	//        PlayerUnit actor = PlayerManager.Instance.GetPlayerUnit(cmm.ActorId);
	//        Tile targetTile = board.GetTile(cmm.TileX, cmm.TileY);

	//        if (cmm.IsKnockback)
	//        {
	//            PlayerManager.Instance.KnockbackPlayer(board, cmm.ActorId, targetTile);
	//        }
	//        else
	//        {
	//            PlayerManager.Instance.ConfirmMove(board, actor, targetTile, cmm.IsClassicClass, cmm.SwapUnitId);
	//        }
	//    }
	//}

	//unit gets crunched
	void OnMultiplayerDisableUnitNotification(object sender, object args)
    {
        int unitId = (int)args;
        PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(unitId);
        board.DisableUnit(pu);
        CombatLogClass cll2 = new CombatLogClass("turns to crystal", pu.TurnOrder, PlayerManager.Instance.GetRenderMode());
        cll2.SendNotification();
    }

    void OnMultiplayerRemoveTilePickUpNotification(object sender, object args)
    {
        List<int> tempList = args as List<int>;
        board.SetTilePickUp(tempList[0], tempList[1], false, owner.renderMode, tempList[2]);
    }

    void OnMultiplayerGameOver(object sender, object args)
    {
        owner.ChangeState<CombatCutSceneState>();
    }

    //Master sending a string to Other to display. Examples: Winds of Fate or Teleport fail
    void OnMultiplayerBattleMessageControllerNotification(object sender, object args)
    {
        string zString = (string)args;
        owner.battleMessageController.Display(zString);
    }


	*/





	#region DEPRECATED
	/*
	public void ExecuteGameLoop()
	{
		if (loopPhase == Phases.StatusTick)
		{
			lastingPhase = Phases.StatusTick;
			loopPhase = Phases.SlowActionTick; //Debug.Log("doing statusTick decrement phase");
											   //owner.ChangeState<StatusCheckState>(); //decrements all lasting statuses, if any expire update combat log and show them expiring
			StatusManager.Instance.StatusCheckPhase(); //decrements all lasting statuses, if any expire update combat log and show them expiring.
		}
		else if (loopPhase == Phases.SlowActionTick)
		{
			lastingPhase = Phases.SlowActionTick;
			SpellManager.Instance.SlowActionTickPhase();
			loopPhase = Phases.SlowAction;
		}
		else if (loopPhase == Phases.SlowAction)
		{
			lastingPhase = Phases.SlowAction;
			//check for quick. mime, and counter flags
			flagCheckPhase = Phases.SlowAction;
			loopPhase = CheckForFlag(loopPhase);
			if (loopPhase == Phases.SlowAction)
			{
				if (SpellManager.Instance.GetNextSlowAction() != null)
				{
					owner.ChangeState<SlowActionState>();
				}
				else
				{
					loopPhase = Phases.CTIncrement;
				}
			}
		}
		else if (loopPhase == Phases.CTIncrement)
		{
			lastingPhase = Phases.CTIncrement;
			PlayerManager.Instance.IncrementCTPhase();
			loopPhase = Phases.ActiveTurn;

			//loopPhase = CheckForFlag(loopPhase); //not sure if check is needed
			//if( loopPhase == Phases.CTIncrement)
			//{
			//PlayerManager.Instance.IncrementCTPhase();
			//loopPhase = Phases.ActiveTurn;
			//lastingPhase = Phases.StatusTick; //starts the loop over after all activeturns are done, is a check in ActiveTurn to make sure they are all done before going to lasting phase
			//} 
		}
		else if (loopPhase == Phases.ActiveTurn)
		{
			//turn reached from coming from proper phase
			if (lastingPhase == Phases.CTIncrement || lastingPhase == Phases.ActiveTurn)
			{
				lastingPhase = Phases.ActiveTurn;
				if (CheckForActiveTurn())
				{
					flagCheckPhase = Phases.ActiveTurn; //in case of reaction or mime state, the loop returns to the proper state
														//either quickflag (which is handled below), reaction flag (goes to reaction state then comes back here, mime flag (goes to mime state then returns here), or AT phase which is handled here
					loopPhase = CheckForFlag(loopPhase, turn.phaseStart);
					if (loopPhase == Phases.Quick)
					{
						loopPhase = Phases.ActiveTurn;
						owner.ChangeState<ActiveTurnState>();
					}
					else if (loopPhase == Phases.ActiveTurn)
					{
						//either turn start or mid turn, if turn.phaseStart = 0 then it is turn start
						//need this because after every action, need to check for reactions and mime which can send the game loop to a different stat
						if (turn.phaseStart == 0)
						{
							//Debug.Log("changing state to Active turn state");
							owner.ChangeState<ActiveTurnState>();
						}
						else
						{
							turn.phaseStart = 0;//next turn will be a full turn, this turn already acted and will complete turn below
							owner.ChangeState<CombatCommandSelectionState>();
						}
					}
				}
				else
				{
					loopPhase = Phases.StatusTick; //returns to the beginning of the loop
				}
			}
			else
			{
				//came here from slow action raising a quick flag, will only do the quick flags or complete the turn that was started due to a quick flag
				if (PlayerManager.Instance.QuickFlagCheckPhase() || turn.phaseStart != 0)
				{
					flagCheckPhase = Phases.ActiveTurn; //in case of reaction or mime state, the loop returns to the proper state
														//either quickflag (which is handled below), reaction flag (goes to reaction state then comes back here, mime flag (goes to mime state then returns here), or AT phase which is handled here
					loopPhase = CheckForFlag(loopPhase, turn.phaseStart);
					if (loopPhase == Phases.Quick)
					{
						loopPhase = Phases.ActiveTurn;
						owner.ChangeState<ActiveTurnState>();
					}
					else if (loopPhase == Phases.ActiveTurn)
					{
						//either turn start or mid turn, if turn.phaseStart = 0 then it is turn start
						//need this because after every action, need to check for reactions and mime which can send the game loop to a different state
						if (turn.phaseStart == 0)
						{
							owner.ChangeState<ActiveTurnState>(); Debug.Log("Should never reach this part of the gameLoop code");
						}
						else
						{
							turn.phaseStart = 0;//next turn will be a full turn, this turn already acted and will complete turn below
							owner.ChangeState<CombatCommandSelectionState>();
						}
					}
				}
				else
				{
					loopPhase = lastingPhase; //returns to SlowAction
				}
			}

		}
		else if (loopPhase == Phases.Mime)
		{
			if (flagCheckPhase == Phases.ActiveTurn)
				loopPhase = Phases.ActiveTurn; //set back to active turn, if there is more 1 flag it'll come back here again
			else
				loopPhase = Phases.SlowAction;
			owner.ChangeState<MimeState>();
		}
		else if (loopPhase == Phases.Reaction)
		{
			if (flagCheckPhase == Phases.ActiveTurn)
				loopPhase = Phases.ActiveTurn; //set back to active turn, if there is more 1 flag it'll come back here again
			else
				loopPhase = Phases.SlowAction;
			owner.ChangeState<ReactionState>();
		}
		else if (loopPhase == Phases.Quick)
		{
			//for now just moves to activeturn, active turn handles the quickflags etc
			loopPhase = Phases.ActiveTurn;
		}
		//else if (loopPhase == Phases.EndActiveTurn) //doing this after a unit ends his turn
		//{

		//}

	}
	*/
	#endregion
}
