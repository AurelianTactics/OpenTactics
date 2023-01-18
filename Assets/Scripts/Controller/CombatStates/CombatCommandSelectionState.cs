using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// PlayerUnit begins deciding on CombatTurn here
/// entered from GameLoopState or ActiveTurnState
/// can also be entered midway through turn by cancelling menu actions or selecting menu actions
/// Different tracks for human and AI controlled based on drivers
/// </summary>
public class CombatCommandSelectionState : BaseCombatAbilityMenuState
{
	/// <summary>
	/// is offline game or online gamee
	/// </summary>
	bool isOffline;

	/// <summary>
	/// in deprecated online mode, one player was the master client and the other was not
	/// </summary>
	bool isMasterClient;

	// notifications
	/// <summary>
	///
	/// </summary>
	const string RLSendAction = "ReinforcementLearning.SendAction"; //get action from RL
	/// <summary>
	///
	/// </summary>
	const string RLRequestAction = "ReinforcementLearning.RequestAction";  //send request for action to RL

	/// <summary>
	/// enter the state
	/// to do document this and why you do the various parts
	/// </summary>
	public override void Enter()
	{
		base.Enter(); //Debug.Log("entered CombatComandSelectionState successfully driver is " + driver);

		isOffline = PlayerManager.Instance.IsOfflineGame();
		isMasterClient = PlayerManager.Instance.isMPMasterClient();

		EnableObservers();

		actorPanel.SetActor(turn.actor);

		turn.CheckStatuses(); //act/move based on statuses

		if (!isOffline && !isMasterClient)
		{ //online game and other's turn. master already did ai checks. waiting on other's input
			LoadMenu();
			return;
		}

		SelectTile(board.GetTile(turn.actor).pos);
		HighlightActorTile(turn.actor, true);
		//active turn state follows the proper unit, this allows the 'snap back' for the current unit
		cameraMain.Open(board.GetTile(turn.actor));
		cameraMain.FollowUnit(turn.actor.TurnOrder);

		//Debug.Log("entering combatcommadnselectionstate");
		//options: status prevents action, AI status, computer, or player controlled.
		if (!UnitHasControl()) //stop, sleep, petrify; Dead midturn (dead at beginning of turn handled in AT)
		{
			//Debug.Log("entering combatcommadnselectionstate unit does not have control");
			StartCoroutine(EndTurnMidTurn());
		}
		else if (StatusManager.Instance.IsAIControlledStatus(turn.actor.TurnOrder))
		{
			//StartCoroutine(AIStatusTurn());
			StartCoroutine(ComputerTurn());
		}
		else if (driver == Drivers.Computer)
		{
			StartCoroutine(ComputerTurn());
		}
		else if (driver == Drivers.ReinforcementLearning)
		{
			// action needed from RL agent
			this.PostNotification(RLRequestAction, turn);
			//Debug.Log("CombatComandSelectionState, sending notification to RLRequestAction");
		}
		else
		{
			//Debug.Log("combatcommadnselectionstate: loading menu");
			if (isOffline || turn.actor.TeamId == NameAll.TEAM_ID_GREEN)
				LoadMenu();
			else
			{
				//online game and other team's turn. masterClient moves to wait state and lets other know that other gets to move
				StartCoroutine(GoToMultiplayerWaitState());
			}
		}
	}

	/// <summary>
	/// exit the state
	/// </summary>
	public override void Exit()
	{
		base.Exit();
		DisableObservers();
		activeMenu.Close();
		HighlightActorTile(turn.actor, false);
		//abilityMenuPanelController.Hide();
	}

	#region Notifications
	public const string ActiveTurnTopNotification = "ActiveTurnTopNotification";
	public const string DidWaitClick = "ActiveTurn.WaitClick";
	public const string DidMoveClick = "ActiveTurn.MoveClick";
	public const string DidAttackClick = "ActiveTurn.AttackClick";
	public const string DidPrimaryClick = "ActiveTurn.PrimaryClick";
	public const string DidSecondaryClick = "ActiveTurn.SecondaryClick";
	public const string DidConfirmClick = "ActiveTurn.ConfirmClick";
	public const string DidBackClick = "ActiveTurn.BackClick";
	public const string DidTargetUnitClick = "ActiveTurn.TargetUnitClick";
	public const string DidTargetMapClick = "ActiveTurn.TargetMapClick";

	public const string DidClose = "ClosePanel.Did";

	/// <summary>
	///
	/// </summary>
	public string CloseSpellPanelNotification()
	{
		return DidClose;
	}

	/// <summary>
	///
	/// </summary>
	void EnableObservers()
	{
		this.AddObserver(OnActiveTurnClick, ActiveTurnTopNotification);
		this.AddObserver(OnRLSendAction, RLSendAction);
		//this.AddObserver(OnWaitClick, DidWaitClick);
		//this.AddObserver(OnMoveClick, DidMoveClick);
		//this.AddObserver(OnAttackClick, DidAttackClick);
		//this.AddObserver(OnPrimaryClick, DidPrimaryClick);
		//this.AddObserver(OnSecondaryClick, DidSecondaryClick);
		//this.AddObserver(OnConfirmClick, DidConfirmClick);
		//this.AddObserver(OnBackClick, DidBackClick);
		//this.AddObserver(OnTargetUnitClick, DidTargetUnitClick);
		//this.AddObserver(OnTargetMapClick, DidTargetMapClick);
	}

	/// <summary>
	///
	/// </summary>
	void DisableObservers()
	{
		//Debug.Log("disabling observers");
		this.RemoveObserver(OnActiveTurnClick, ActiveTurnTopNotification);
		this.RemoveObserver(OnRLSendAction, RLSendAction);
		//this.RemoveObserver(OnWaitClick, DidWaitClick);
		//this.RemoveObserver(OnMoveClick, DidMoveClick);
		//this.RemoveObserver(OnAttackClick, DidAttackClick);
		//this.RemoveObserver(OnPrimaryClick, DidPrimaryClick);
		//this.RemoveObserver(OnSecondaryClick, DidSecondaryClick);
		//this.RemoveObserver(OnConfirmClick, DidConfirmClick);
		//this.RemoveObserver(OnBackClick, DidBackClick);
		//this.RemoveObserver(OnTargetUnitClick, DidTargetUnitClick);
		//this.RemoveObserver(OnTargetMapClick, DidTargetMapClick);
	}

	/// <summary>
	/// handle user selecting a turn component from a menu
	/// </summary>
	void OnActiveTurnClick(object sender, object args)
	{
		string str = (string)args; //Debug.Log("in onactiveturnclick " + str);

		if (str.Equals(DidWaitClick))
		{
			//Debug.Log("about to enter CombatEndFacingState");
			//activeMenu.Close();
			owner.ChangeState<CombatEndFacingState>();
		}
		else if (str.Equals(DidMoveClick))
		{
			owner.ChangeState<CombatMoveTargetState>();
		}
		else if (str.Equals(DidAttackClick))
		{
			turn.spellName = SpellManager.Instance.GetSpellNameByIndex(CalculationAT.GetAttackSpellIndex(turn.actor.TurnOrder));
			owner.ChangeState<CombatTargetAbilityState>();
		}
		else if (str.Equals(DidPrimaryClick))
		{
			turn.isPrimary = true;
			owner.ChangeState<CombatAbilitySelectState>();
		}
		else if (str.Equals(DidSecondaryClick))
		{
			turn.isPrimary = false;
			owner.ChangeState<CombatAbilitySelectState>();
		}
		else if (str.Equals(DidConfirmClick))
		{
			ConfirmCharging(); //Debug.Log("confirm performing and charging");
		}
		else if (str.Equals(DidBackClick))
		{
			CancelCharging(); //Debug.Log("Cancel performing and charging");
		}
		else if (str.Equals(DidTargetUnitClick))
		{

		}
		else if (str.Equals(DidTargetMapClick))
		{

		}
	}


	#endregion

	/// <summary>
	/// is a PU has a slow action from last turn, ask player whether action should continue
	/// this function responds to "yes continue action"
	/// </summary>
	void ConfirmCharging()
	{

		turn.hasUnitActed = true; Debug.Log("in confirm charging");
		activeMenu.SetMenuTop(turn, owner.battleMessageController);
		//CloseSpellPanelNotification();// 
	}

	/// <summary>
	/// is a PU has a slow action from last turn, ask player whether action should continue
	/// this function responds to "no, cancel action"
	/// </summary>
	void CancelCharging()
	{
		turn.hasUnitActed = false; Debug.Log("in cancel charging");
		SpellManager.Instance.RemoveSpellSlowByUnitId(turn.actor.TurnOrder);
		StatusManager.Instance.RemoveStatus(turn.actor.TurnOrder, NameAll.STATUS_ID_CHARGING);
		StatusManager.Instance.RemoveStatus(turn.actor.TurnOrder, NameAll.STATUS_ID_PERFORMING);
		activeMenu.SetMenuTop(turn, owner.battleMessageController);
		//CloseSpellPanelNotification();//Debug.Log("Close spell title panel");
	}

	/// <summary>
	/// cancel AI slow action that began on prior turn
	/// </summary>
	void CancelAICharging()
	{
		SpellManager.Instance.RemoveSpellSlowByUnitId(turn.actor.TurnOrder);
		StatusManager.Instance.RemoveStatus(turn.actor.TurnOrder, NameAll.STATUS_ID_CHARGING);
		StatusManager.Instance.RemoveStatus(turn.actor.TurnOrder, NameAll.STATUS_ID_PERFORMING);
	}

	/// <summary>
	/// load the PU menu for turm selection
	/// </summary>
	protected override void LoadMenu()
	{

		//read the turn to decide which parts of the active menu to show

		if (!turn.hasUnitActed || !turn.hasUnitMoved)
		{
			activeMenu.SetMenuTop(turn, owner.battleMessageController); //Debug.Log("reached here a");
		}
		else
		{
			//owner.ChangeState<CombatEndFacingState>(); //apparently can't switch states in the middle of an enter call
			//Debug.Log("reached here b");
			StartCoroutine(GoToEndFacingState());
		}

		//his way
		//if (menuOptions == null)
		//{
		//    menuTitle = "Commands";
		//    menuOptions = new List<string>(3);
		//    menuOptions.Add("Move");
		//    menuOptions.Add("Action");
		//    menuOptions.Add("Wait");
		//}

		//abilityMenuPanelController.Show(menuTitle, menuOptions);
		//abilityMenuPanelController.SetLocked(0, turn.hasUnitMoved);
		//abilityMenuPanelController.SetLocked(1, turn.hasUnitActed);

	}

	/// <summary>
	/// mouse button clicked
	/// </summary>
	protected override void OnFire(object sender, InfoEventArgs<int> e)
	{
		//Debug.Log("2 in on fire event " + e.info.ToString());
		//ActiveTurn Menu handles the OnFire
		//if (e.info == 0)
		//    Confirm();
		//else
		//    Cancel();
		if (e.info == 1)
		{
			Cancel();
		}
	}

	/// <summary>
	///
	/// </summary>
	protected override void OnMove(object sender, InfoEventArgs<Point> e)
	{
		//Debug.Log("Move 2" + e.info.ToString());
		//ActiveTurn menu handles the move
		//if (e.info.x > 0 || e.info.y < 0)
		//    activeMenu.
		//else
		//    abilityMenuPanelController.Previous();
	}

	/// <summary>
	///
	/// </summary>
	protected override void Confirm()
	{
		//ATMenu handles the confirm
		//Debug.Log("need to get which button is highlighted");
		//switch (abilityMenuPanelController.selection)
		//{
		//    case 0: // Move
		//        owner.ChangeState<MoveTargetState>();
		//        break;
		//    case 1: // Action
		//        owner.ChangeState<CategorySelectionState>();
		//        break;
		//    case 2: // Wait
		//        owner.ChangeState<EndFacingState>();
		//        break;
		//}
	}

	/// <summary>
	/// cancel button clicked, exit state
	/// </summary>
	protected override void Cancel()
	{
		owner.ChangeState<CombatExploreState>();

		//if (turn.hasUnitMoved && !turn.lockMove)
		//{
		//    turn.UndoMove();
		//    abilityMenuPanelController.SetLocked(0, false);
		//    //SelectTile(turn.actor.tile.pos);
		//}
		//else
		//{
		//    owner.ChangeState<ExploreState>();
		//}
	}

	//can't change states on an enter call
	/// <summary>
	/// player cant move or act so go to combat end facing state
	/// need ienumerator cuz cant go directly from one state to another apparently
	/// </summary>
	IEnumerator GoToEndFacingState()
	{
		yield return null;
		owner.ChangeState<CombatEndFacingState>();
	}

	/// <summary>
	/// 
	/// </summary>
	IEnumerator GoToMultiplayerWaitState()
	{
		yield return null;
		owner.ChangeState<MultiplayerWaitState>();
	}

	/// <summary>
	/// need ienumerator cuz cant go directly from one state to another apparently
	/// </summary>
	IEnumerator DoPhase()
	{
		yield return null;
		//int puId = PlayerManager.Instance.GetNextTurnPlayerUnitId();

		//if (puId != NameAll.NULL_INT)
		//{
		//    PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(puId);
		//    Debug.Log("Add info to select tile");
		//    //SelectTile(pu.GetMap_tile_index()); //change this to point when applicable
		//    cameraMain.FollowUnit(puId);
		//    yield return null;
		//    owner.ChangeState<CombatCommandSelectionState>();
		//}
		//else
		//{
		//    owner.ChangeState<GameLoopState>();
		//}
	}

	//IEnumerator AIStatusTurn()
	//{
	//    yield return new WaitForSeconds(0f);
	//}

	/// <summary>
	/// somerging caused PU to lose turn
	/// need ienumerator cuz cant go directly from one state to another apparently
	/// </summary>
	IEnumerator EndTurnMidTurn()
	{
		yield return null;
		PlayerManager.Instance.EndCombatTurn(turn);
		owner.ChangeState<GameLoopState>();
	}

	/// <summary>
	/// AI turn
	/// </summary>
	IEnumerator ComputerTurn()
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
						CancelAICharging();
					}
					else
					{
						turn.plan = owner.cpu.ContinueChargingPerforming();
						turn.hasUnitActed = true;
					}
				}
			}

			//if ( owner.cpu == null)
			//{
			//    Debug.Log("testing null");
			//}
			if (turn.plan == null)
				turn.plan = owner.cpu.Evaluate();
			turn.spellName = turn.plan.spellName;
		}

		yield return new WaitForSeconds(1f);
		//Debug.Log("in computer turn 0.5");
		if (turn.plan.isActFirst && turn.hasUnitActed == false && turn.plan.spellName != null)
		{
			//Debug.Log("in computer turn 1");
			owner.ChangeState<CombatTargetAbilityState>();
		}
		else if (turn.hasUnitMoved == false && turn.plan.moveLocation != board.GetTile(turn.actor).pos)
		{
			//Debug.Log("in computer turn 2");
			owner.ChangeState<CombatMoveTargetState>();
		}
		else if (!turn.plan.isActFirst && turn.hasUnitActed == false && turn.plan.spellName != null)
		{
			//Debug.Log("in computer turn 3");
			owner.ChangeState<CombatTargetAbilityState>();
		}
		else
		{
			//Debug.Log("in computer turn 4");
			owner.ChangeState<CombatEndFacingState>();
		}

	}

	/// <summary>
	///
	/// </summary>
	bool UnitHasControl()
	{
		//Debug.Log("in UnitHasControl " + StatusManager.Instance.IfStatusByUnitAndId(turn.actor.TurnOrder, NameAll.STATUS_ID_DEAD));
		//dead mid turn check, dead check at beginning of turn in AT
		if (StatusManager.Instance.IfStatusByUnitAndId(turn.actor.TurnOrder, NameAll.STATUS_ID_DEAD))
			return false;

		return StatusManager.Instance.IsTurnActable(turn.actor.TurnOrder); //sleep, petrify, stop
	}

	#region ReinforcementLearning
	//listen for action from RL agent
	/// <summary>
	///
	/// </summary>
	void OnRLSendAction(object sender, object args)
	{
		CombatPlanOfAttack plan = (CombatPlanOfAttack)args;
		//convert actionArray into a plan
		turn.plan = plan;
		turn.spellName = plan.spellName;
		turn.targetTile = board.GetTile(plan.fireLocation);

		if (turn.plan.isEndTurn)
		{
			owner.ChangeState<CombatEndFacingState>();
		}
		else if (turn.plan.isActFirst && turn.hasUnitActed == false && turn.plan.spellName != null)
		{
			//Debug.Log("in computer turn 1");
			owner.ChangeState<CombatTargetAbilityState>();
		}
		else if (turn.hasUnitMoved == false && turn.plan.moveLocation != board.GetTile(turn.actor).pos)
		{
			//Debug.Log("in computer turn 2");
			owner.ChangeState<CombatMoveTargetState>();
		}
		else if (!turn.plan.isActFirst && turn.hasUnitActed == false && turn.plan.spellName != null)
		{
			//Debug.Log("in computer turn 3");
			owner.ChangeState<CombatTargetAbilityState>();
		}
		else
		{
			//Debug.Log("in computer turn 4");
			owner.ChangeState<CombatEndFacingState>();
		}
	}
	#endregion
}
