using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 Scrap code as I clean some stuff up
 */

/// <summary>
/// Orchestrates Combat and handles the transitions between states for most of the gameloop
/// to do: document and clean up
/// removed all multiplayer code for now, can look at old versions of GameLoopState for how MP used to work
/// </summary>
public class GameLoopState : CombatState
{

	/// <summary>
	/// what phase the game loop is in, due to Mime, Counter, QuickFlag the "current" phase can be different
	/// </summary>
	static Phases loopPhase = Phases.StatusTick;

	/// <summary>
	/// so loop phase knows where to return to after jumping to an active turn/counter/mime
	/// </summary>
	static Phases lastingPhase = Phases.StatusTick;

	/// <summary>
	/// Reaction, Mime and Quick flags can cause order of gameloop to shift. Need a flagCheckPhase to enure the proper order
	/// </summary>
	static Phases flagCheckPhase = Phases.SlowAction;

	/// <summary>
	/// If true, continue doing the game loop. Turned on when entering the state and off when leaving the state 
	/// or not the master client in multiplayer
	/// </summary>
	bool doLoop = false;

	// to do, write these notification when fleshed out a bit more. idea is agent will send a notification after receiving one from teh python API
	bool isRLEndNotificationSent;
	const string RLEndEpisode = "ReinforcementLearning.EndEpisode"; //end episode notification sent to RL code
	const string RLResetGame = "ReinforcementLearning.ResetGame"; //reset game, sent from RL code to this state
																  // show top menu turns
	const string refreshTurns = "refreshTurns";

	/// <summary>
	/// Agent class for RL agent when training an RL
	/// to do: abstract the class
	/// </summary>
	public AgentForGymWrapper RLAgent;


	public override void Enter()
	{
		base.Enter();
		//Debug.Log("in game loop state");
		doLoop = true;
		// to do, add this to owner
		var combatGameType = owner.combatGameType;
		// to do abstract this; look up the syntax on this
		RLAgent = new ChickenAgent(gameLoopState=this);
	}


	public override void Exit()
	{
		doLoop = false;
		base.Exit();
		DisableObservers();
	}

	void Update()
	{
		if (doLoop)
		{

			if (IsBattleOver())
			{
				//Debug.Log("batte is over 0");
				if (owner.combatMode == NameAll.COMBAT_MODE_RL_DUEL)
				{
					// to do: how to handle RL over. some sort of reset here probably
				}
				else
					owner.ChangeState<CombatCutSceneState>(); //online: call it inside gameloop in MP so that Other is ready

				return;
			}
			DoGameLoop();
		}

	}

	#region GameLoop
	//does game loop without using state machine for faster sims for RL mode
	void DoGameLoop()
	{
		//Debug.Log("Doing Game Loop " + loopPhase);
		if (loopPhase == Phases.StatusTick) 
		{
			//some statuses last for a certain number of ticks, decrements statuses by a tick and sees if any trigger
			lastingPhase = Phases.StatusTick;
			loopPhase = Phases.SlowActionTick; //Debug.Log("doing statusTick decrement phase");
			StatusManager.Instance.StatusCheckPhase(); //decrements all lasting statuses, if any expire update combat log and show them expiring.
		}
		else if (loopPhase == Phases.SlowActionTick) 
		{
			//decrements all slowaction ticks. when slow action reaches 0 ticks, it is executed
			lastingPhase = Phases.SlowActionTick;
			SpellManager.Instance.SlowActionTickPhase();
			loopPhase = Phases.SlowAction;
		}
		else if (loopPhase == Phases.SlowAction) 
		{
			// executes slow actions
			lastingPhase = Phases.SlowAction;
			// check for quick. mime, and counter flags. These flags signify events that occur before slowactions
			// potential for those flags to change the loopPhase. example: first slowaction goes and triggers a reaction, then reaction goes,
			// then next slowaction goes
			flagCheckPhase = Phases.SlowAction;
			loopPhase = CheckForFlag(loopPhase);

			if (loopPhase == Phases.SlowAction)
			{
				SpellSlow ss = SpellManager.Instance.GetNextSlowAction();
				if (ss != null)
				{
					// Slow action found. Resolve it.
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
			if (lastingPhase == Phases.CTIncrement || lastingPhase == Phases.ActiveTurn)
			{
				// turn reached from coming from proper phase and not a quick flag
				lastingPhase = Phases.ActiveTurn;
				if (CheckForActiveTurn())
				{
					// in case of reaction or mime state, the loop returns to the proper state
					// either quickflag (which is handled below), reaction flag (goes to reaction state then comes back here,
					// mime flag (goes to mime state then returns here), or AT phase which is handled here
					flagCheckPhase = Phases.ActiveTurn; 									
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
							ActiveTurnCheckAndStart();

					}
					else if (loopPhase == Phases.ActiveTurn)
					{
						//Debug.Log("Doing Game Loop ActiveTurn AT top" + loopPhase + " " + turn.phaseStart);
						// either turn start or mid turn, if turn.phaseStart = 0 then it is turn start
						// need this because after every action, need to check for reactions and mime which can send the game loop to a different state
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
								ActiveTurnCheckAndStart();
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
					// no active turns. return to top of the game loop.
					loopPhase = Phases.StatusTick; 
				}
			}
			else
			{
				//came here from slow action raising a quick flag, will only do the quick flags or complete the turn that was started due to a quick flag
				if (PlayerManager.Instance.QuickFlagCheckPhase() || turn.phaseStart != 0)
				{
					// in case of reaction or mime state, the loop returns to the proper state
					// either quickflag (which is handled below), reaction flag (goes to reaction state then comes back here,
					// mime flag (goes to mime state then returns here), or AT phase which is handled here
					flagCheckPhase = Phases.ActiveTurn; 
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
							ActiveTurnCheckAndStart();
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
								ActiveTurnCheckAndStart();
						}
						else
						{
							if (owner.renderMode != NameAll.PP_RENDER_NONE)
							{
								// next turn will be a full turn, this turn already acted and will complete turn below
								turn.phaseStart = 0;
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
					//returns to SlowAction
					loopPhase = lastingPhase; 
				}
			}

		}
		else if (loopPhase == Phases.Mime)
		{
			// Do Mime phase. Mime Phase happens after eligible fast or slow action resolves
			if (flagCheckPhase == Phases.ActiveTurn)
			{
				// set back to active turn, if there is more 1 flag it'll come back here again
				loopPhase = Phases.ActiveTurn;
			}
			else
				loopPhase = Phases.SlowAction;
			DoReaction(false, true, false);
		}
		else if (loopPhase == Phases.Reaction)
		{
			// Do reaction phase. Mime Phase happens after eligible fast or slow action resolves
			if (flagCheckPhase == Phases.ActiveTurn)
			{
				//set back to active turn, if there is more 1 flag it'll come back here again
				loopPhase = Phases.ActiveTurn; 
			}
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



	/// <summary>
	/// Called from DoGameLoop in non-render mode
	/// Decides how the action for the active turn is generated
	/// </summary>
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
		else if (driver == Drivers.BlackBoxRL)
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
		else if(driver == Drivers.ReinforcementLearningGym)
		{
			// action is supplied throuhg the python Gym API. waiting on an action from that source before proceeding
			if(loopPhase != Phases.RLWait)
			{
				loopPhase = Phases.RLWait;
				// to do, not sure how to handle getting an action back from the gym. maybe it is a notification? maybe return from a function?
			}
		}
		else
		{
			Debug.Log("BUG: combatcommandselection, should never reach here");
		}
	}


	#region ReinforcementLearning

	/// <summary>
	/// Get observations for the GymWrapper
	/// </summary>
	/// <returns></returns>
	public float[] GetGymWrapperObservations()
	{
		var obsArray = PlayerManager.Instance.GetGymWrapperObservationTest();

		//obsArray[0] = game_state / 10f;
		//Debug.Log("testing observations");
		//for( int i = 0; i < obsArray.Length; i++)
		//{
		//	print("testing observations " + obsArray[i]);
		//}
		return obsArray;
	}

	/// <summary>
	/// Read action from the Avatar and update the env with that action
	/// </summary>
	void BlackBoxRLTurn()
	{
		// continually hit here until the action is ready. might make sense to change this to a notification style system
		if (this.owner.worldTimeManager.agentSession.avatar.GetNextActionState() == AurelianTactics.BlackBoxRL.NextActionState.Ready)
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
		if (tu.plan == null || (tu.plan.isActFirst && tu.plan.spellName == null))
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
		else if ((turn.plan.isActFirst && turn.hasUnitActed == false && turn.plan.spellName != null)
			|| (!turn.plan.isActFirst && turn.hasUnitActed == false && turn.plan.spellName != null))
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
				PlayerManager.Instance.AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_MOVE, NameAll.COMBAT_LOG_SUBTYPE_MOVE_TELEPORT_ROLL, rollResult: roll, rollChance: rollChance);
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
			PlayerManager.Instance.SetUnitTile(board, turn.actor.TurnOrder, owner.currentTile, isAddCombatLog: true);
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
				combatLogSubType: NameAll.COMBAT_LOG_SUBTYPE_CRYSTAL_PICK_UP);
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

	/// <summary>
	/// Start the ActiveTurn 
	/// only done in non-render mode. Called from DoGameLoop.
	/// </summary>
	void ActiveTurnCheckAndStart()
	{
		// sets quickFlag to false if it was true
		PlayerUnit pu = PlayerManager.Instance.GetNextActiveTurnPlayerUnit(isSetQuickFlagToFalse: true);

		if (pu != null) //probably an unnecessary check
		{
			turn.Change(pu);
			CombatCommandSelection();
		}
	}

	/// <summary>
	/// Execute a slow action
	/// </summary>
	/// <param name="ss"></param>
	void DoSlowAction(SpellSlow ss)
	{
		if (!StatusManager.Instance.IsTurnActable(ss.UnitId))
		{
			// unit can't go, remove the spell slow
			SpellManager.Instance.RemoveSpellSlowByObject(ss);
			PlayerManager.Instance.AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_SLOW_ACTION, NameAll.COMBAT_LOG_SUBTYPE_SLOW_ACTION_UNABLE_TO_CAST, ss);
		}
		else
		{
			// unit may to cast the slow action so save startDir to turn the unit back if applicable
			Directions startDir = PlayerManager.Instance.GetPlayerUnit(ss.UnitId).Dir;
			Tile targetTile; //Debug.Log("spell slow is is targetting " + ss.GetTargetUnitId());
			if (ss.TargetUnitId != NameAll.NULL_UNIT_ID)
			{
				// target a unit
				targetTile = board.GetTile(PlayerManager.Instance.GetPlayerUnit(ss.UnitId)); //Debug.Log("spell slow is targeting a unit");
			}
			else
			{
				// target a panel
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

			// get the SpellName to see if the a new slow action needs to be created
			SpellName sn = SpellManager.Instance.GetSpellNameByIndex(ss.SpellIndex);
			
			if (sn.CommandSet == NameAll.COMMAND_SET_SING || sn.CommandSet == NameAll.COMMAND_SET_DANCE
				|| (sn.CommandSet == NameAll.COMMAND_SET_ARTS && sn.CTR % 10 == 1))
			{
				//if performing, add a new instance of it CTR into the future
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
				// remove the SpellSlow and turn the unit back to original direction
				SpellManager.Instance.RemoveSpellSlowByObject(ss);
				PlayerManager.Instance.GetPlayerUnit(ss.UnitId).Dir = startDir;
			}
		}

	}

	/// <summary>
	/// Execute the reaction, mime action, or after move action
	/// Called from DoGameLoop
	/// I think I did move actions here since these reaction resolve types happen instantly
	/// </summary>
	/// <param name="isReaction">Is the phase a reaction phase</param>
	/// <param name="isMime">Is the phase a mime phase</param>
	/// <param name="isMovement">IS the phase a movement</param>
	void DoReaction(bool isReaction, bool isMime, bool isMovement)
	{
		SpellReaction sr;
		if (isMime)
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
			else if (isMovement)
				battleMessage = "Move: " + ssTurn.spellName.AbilityName;
			else
				battleMessage = "Counter: " + ssTurn.spellName.AbilityName;

			if (owner.renderMode != NameAll.PP_RENDER_NONE)
				StartCoroutine(HighlightAction(ssTurn, ssTurn.targetTile, battleMessage));


			// modify the spellName for reaction
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



	/// <summary>
	/// Check for flags to see if DoGameLoop should change which game phase it is in
	/// counterflag goes first, then mimeflag, then quickflag
	/// </summary>
	/// <param name="currentPhase"></param>
	/// <param name="midActiveTurn"></param>
	/// <returns></returns>
	Phases CheckForFlag(Phases currentPhase, int midActiveTurn = 0)
	{
		//Debug.Log("checking for flag");
		if (SpellManager.Instance.IsSpellReaction())
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

	/// <summary>
	/// Check if player has the next active turn
	/// </summary>
	/// <returns>true if player has next active turn, false if not</returns>
	bool CheckForActiveTurn()
	{
		//don't set quickflag to false, only do that when getting the active unit
		if (PlayerManager.Instance.GetNextActiveTurnPlayerUnit(isSetQuickFlagToFalse: false) != null) 
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

	void OnStatusManagerNotification(object sender, object args)
	{
		//string str = (string)args;
		//object sent is a list of ints first is the statusId, 2nd is the playerId
		List<int> tempList = args as List<int>;
		if (tempList[0] == NameAll.STATUS_ID_CRYSTAL) //str.Equals(NameAll.STATUS_NAME_CRYSTAL)
		{

			if (!isOffline && isMasterClient) //sends result to other
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
			if (isSelfQuit) //this player quit. let other player know and end game
			{
				//Debug.Log("quitting game by self quit");
				PlayerManager.Instance.SendMPQuitGame(isSelfQuit);
				owner.ChangeState<CombatEndState>();
			}
			else //other playerquit, move to CombatCutSceneState
			{
				//Debug.Log("quitting game because other player quit");

				if (isMasterClient)
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

	/// <summary>
	/// refresh the turns menu after certain game loop things. basically after a slow action resolves or before an active turn
	/// Non headless mode, sends a notification to refresh turns
	/// possible to do: if using the turns as an obs in RL mode would have to reconfigure the refresh and how RL mode gets the turns
	/// </summary>
	void RefreshTurnsList()
	{
		if (owner.renderMode != NameAll.PP_RENDER_NONE)
		{
			//Debug.Log("sending refresh turns notification");
			this.PostNotification(refreshTurns);
		}

	}


	#endregion

}
