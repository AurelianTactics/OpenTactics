//entered after WalkAroundInitState
//WalkAround game loop occurs here
//user entries done by players in WalkAroundPlayerState
//actions inputted by users are done
//status ticks, slow and fast actions done here

//to do:
//check for combat done here, if conditions met launches combat phase
//no multiplayer support: thinking each player gets its own menus and localturns but needs to be built out
//AI logic
//What menu support to have (thinking now Turns menu update but could be done)


/*
How turns are done by user input
	WalkAroundInput enums track what stage of entering a turn the unit is end
		At the beginning of user input, if able to move map or continue ability move into WalkAroundInput.UnitMoveMap or .UnitContinue status and pull up confirm/cancel buttons
			if confirmed/cancelled move to proper WalkAroundInput and take necessary actions
	When End is clicked, the CombatTurn is turned into a WalkAroundActionObject and sent to PlayerManager to be added to a static list
			fast actions go to one queue for immediate execution, slow actions to another for execution when their CTR = 0
		Update checks for those WAAO and executes them here if any are entered (PlayerManager.Instance.ConsumeNextWalkAroundActionObject)
	Every fixed amount of seconds of time (paused when a spell/ability/movement is being done) a status tick is done and a slow action tick is done
		Status ticks remove statuses when the status is over
		Slow action ticks see which WAAO are in the sWalkAroundSpellSlowQueue and move them to slowspells with 0 CTR for resolution
			//PlayerManager.Instance.IncrementWalkAroundTick() increments the walk around tick in a loop from 0 to MAX_CTR_TICK 

Combat Start Trigger:
	Execute non combat loop until PlayerManager (SendCombatStart) throws a combatstart notification
		current ways to start combat: cast harmful ability on someone not on your team or move within x tiles of someone not on your team
			WalkAroundCombatStartCheck in PlayerManager is called from CalculationResolveAction to check for harmful ability
			CheckCombatProximity in PlayerManager is called from PUO of moving unit after every tile move
	On CombatStart notification
		Save following information:
			XXX
		Switch scene to Combat
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;

public class WalkAroundMainState : CombatState
{
	static WalkAroundInput unitCommand = WalkAroundInput.UnitNone;
    bool doLoop = false;
    bool isCombat = false;
    bool isOffline = false;
	//bool isMasterClient;
	bool isSlowActionLock = false; //locks gameloop check until the slow action resolves
    float statusTickTime = 5.0f; //how often a status tick ticks down
    float statusTickRunningTime = 0.0f;
	float slowActionTickTime = 3.0f; //how often a slow action tick goes
	float slowActionTickRunningTime = 0.0f;
    PlayerUnit puActor = null; //select unit to see available actions
	List<Tile> tiles; //tiles available for targeting, moving etc
	CombatAbilityArea aa; //area available for ability targeting
	int targetIndex = 0; //targetIndex for seeing hit preview when multiple units are targeted
	//things needed to target an ability
	CombatAbilityRange ar;
	const int ABILITY_ID_NULL = -19;
	bool directionTarget;
	Directions currentDir; //unit can change directio mid turn
	readonly int SPELL_RANGE_LINE = 102;

	public override void Enter()
    {
        base.Enter();
        //isMasterClient = true; //change for mp
        doLoop = true;
        isCombat = false;
		tiles = new List<Tile>();
		PlayerManager.Instance.SetWalkAroundMoveAllowed(true);
        EnableObservers();
        //actorPanel.Close();
        //previewPanel.Close();
        //targetPanel.Close();
        Debug.Log("entered WalkAroundMainState");
        //StartCoroutine(Init());
    }

    public override void Exit()
    {
        Debug.Log("leaving WalkAroundMainState");
        doLoop = false;
        base.Exit();
        DisableObservers();
    }

	//main gameplay loop of WAAO
	//checks for combat flag enabled
		//if so change to combat state
		//if not executes WAAO mode
	//lock for when slow actions are being casted
    void Update()
    {
        if (doLoop)
        {
			if (isCombat)
			{
				Debug.Log("entering combat, need to change to combat");
				AddVictoryCondition();
				owner.ChangeState<CombatCutSceneState>();
			}
            else if (isSlowActionLock)
            {
				Debug.Log("slowaction lock is " + isSlowActionLock);
			}
            else
            {
				ExecuteNonCombatLoop();
			}
            
        }

    }

	//called in Update, consumes fast and slow actions if WAAO are in the queue
	void ExecuteNonCombatLoop()
	{
		//check for slow actions in the queue
		if (SpellManager.Instance.isWalkAroundSpellSlow())
		{
			Debug.Log("in ExecuteNonCombatLoop 0");
			SpellSlow ss = SpellManager.Instance.GetNextSlowAction();
			if (ss != null)
			{
				Debug.Log("in ExecuteNonCombatLoop 1");
				isSlowActionLock = true;
				StartCoroutine(ResolveSlowAction(ss));
				//SpellManager.Instance.RemoveSpellSlowByObject(ss);
				//asdf
			}	
		}
		else
		{
			//check to see if WalkAroundActionObjects are in the queue
			//do fast actions in Consume, if no fast action then add the slowActionTickRunningTime
			if (!PlayerManager.Instance.ConsumeNextWalkAroundActionObject(board, calcMono)){

				statusTickRunningTime += Time.deltaTime;
				if (statusTickRunningTime >= statusTickTime)
				{
					statusTickRunningTime = 0.0f;
					StatusManager.Instance.StatusCheckPhase();
				}
				
				slowActionTickRunningTime += Time.deltaTime;
				if( slowActionTickRunningTime >= slowActionTickTime)
				{
					slowActionTickRunningTime = 0.0f;
					PlayerManager.Instance.IncrementWalkAroundTick(); //increment walk around tick and maybe create slow actions if needed
					//Debug.Log("in ExecuteNonCombatLoop 2");
				}
			}
		}

	}

	//Not implemented, concept idea to limit actions user inputs/actions
	//private float timeBetweenTicks = 3.0f; //give this much tim between ticks, allows time for users to input actions
	//private float timeSinceLastTurn = 3.0f;
	//bool CheckIfTickAllowed()
	//{
	//	if (timeSinceLastTurn >= timeBetweenTicks)
	//	{
	//		timeSinceLastTurn = 0.0f;
	//		return true;
	//	}
	//	else
	//	{
	//		timeSinceLastTurn += Time.deltaTime;
	//		return false;
	//	}
	//}



	//called in ExecuteNonCombatLoop to resolve any slow action WAAO
	IEnumerator ResolveSlowAction(SpellSlow ss)
    {
        if (!StatusManager.Instance.IsTurnActable(ss.UnitId))
        {
            //unit can't go, remove the spell slow, will change state down below
            SpellManager.Instance.RemoveSpellSlowByObject(ss);
			PlayerManager.Instance.RemoveWAAOFlag(ss.UnitId);
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

            cameraMain.MoveToMapTile(targetTile);

            CombatTurn ssTurn = new CombatTurn(ss, board);
            HighlightActorTile(ssTurn.actor, true);
            owner.battleMessageController.Display(ssTurn.spellName.AbilityName + " is cast!"); //this.PostNotification(DidSlowActionResolve); //Debug.Log("Send message to spell title panel");
            board.SelectTiles(ssTurn.targets); //Debug.Log("number of targets is " + ssTurn.targets.Count);//highlight the target tiles
            yield return new WaitForSeconds(1.25f);

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
            HighlightActorTile(ssTurn.actor, false); //unhighlight
            board.DeSelectTiles(ssTurn.targets); //deselect the tiles
			Debug.Log("removing slowaction pre");
            SpellManager.Instance.RemoveSpellSlowByObject(ss);
			Debug.Log("removing slowaction post");
			yield return new WaitForSeconds(0.75f);
            PlayerManager.Instance.SetPUODirectionMidTurn(ss.UnitId, startDir);
            PlayerManager.Instance.GetPlayerUnit(ss.UnitId).Dir = startDir;
			PlayerManager.Instance.RemoveWAAOFlag(ss.UnitId);
		}
		isSlowActionLock = false;
        yield return null;

    }

    WalkAroundActionObject GetAITurn()
    {
        Debug.Log("need AI turn logic");
        //need argument for figure out full turn, 
        //figure out action only turn (already moved, this should be straightforward if move mathces move) if not re find an action
        //if action first, then do the move after
        return null;
    }

	//if trigger is met, combat is entered
    void EnterCombat()
    {
        isCombat = true;
        //if walkaround allowed/not sets it in player manager so units stop moving, then combat is launched
        PlayerManager.Instance.SetWalkAroundMoveAllowed(false);
        owner.battleMessageController.Display("Entering Combat...");
        //change scene is called in Update
    }

    #region notifications and listeners
    //enabled and disabling observers in entering and exiting state
    void OnEnable()
    {
        EnableObservers();
    }

    void OnDisable()
    {
        DisableObservers();
    }

    //notifications
    const string CombatStart = "PlayerManager.CombatStart";
    const string CombatEnd = "PlayerManager.CombatEnd";
    const string TeamDefeated = "PlayerManager.TeamDefeated";
    const string refreshTurns = "refreshTurns";

	//user input notifications
	const string ActiveTurnTopNotification = "ActiveTurnTopNotification";
	const string ActiveTurnWaitNotification = "ActiveTurnWaitNotification";
	const string ActiveTurnTargetUnitMapNotification = "ActiveTurnTargetUnitMapNotification";
	const string DidWaitClick = "ActiveTurn.WaitClick";
	const string DidMoveClick = "ActiveTurn.MoveClick";
	const string DidAttackClick = "ActiveTurn.AttackClick";
	const string DidPrimaryClick = "ActiveTurn.PrimaryClick";
	const string DidSecondaryClick = "ActiveTurn.SecondaryClick";
	const string DidConfirmClick = "ActiveTurn.ConfirmClick";
	const string DidBackClick = "ActiveTurn.BackClick";
	const string DidTargetUnitClick = "ActiveTurn.TargetUnitClick";
	const string DidTargetMapClick = "ActiveTurn.TargetMapClick";
	const string DidClose = "ClosePanel.Did";

	//wait menu
	const string DirectionNotification = "ActiveTurnDirectionNotification";
	const string DidNorthClick = "ActiveTurn.NorthClick";
	const string DidEastClick = "ActiveTurn.EastClick";
	const string DidSouthClick = "ActiveTurn.SouthClick";
	const string DidWestClick = "ActiveTurn.WestClick";
	//ability menu
	public const string DidAbilityClick = "AbilitySelect.AbilityClick";
	//public const string DidMathSkillClick = "AbilitySelect.MathSkillClick"; //purposely not allowing nested abilities in WA mode

	void RefreshTurnsList()
    {
        this.PostNotification(refreshTurns);
    }
    //combat triggered by:
    //a unitmoves within x tiles of a hostile target, checked in PlayerUnitObject
    //spell targets unit on other team, checked in 


    //players send notifications, WalkAroundMainState handles various inputs
    void EnableObservers()
    {
        this.AddObserver(OnCombatStart, CombatStart);
		//top of active turn menu
		this.AddObserver(OnActiveTurnClick, ActiveTurnTopNotification);
		this.AddObserver(OnDirectionClick, ActiveTurnWaitNotification);
		this.AddObserver(OnAbilityClick, DidAbilityClick);
		//confirm/back
		this.AddObserver(OnActiveTurnClick, DidConfirmClick);
		this.AddObserver(OnActiveTurnClick, DidBackClick);
		//target unit/map
		this.AddObserver(OnActiveTurnClick, ActiveTurnTargetUnitMapNotification);
	}

    void DisableObservers()
    {
        this.RemoveObserver(OnCombatStart, CombatStart);
		this.RemoveObserver(OnActiveTurnClick, ActiveTurnTopNotification);
		this.RemoveObserver(OnDirectionClick, ActiveTurnWaitNotification);
		this.RemoveObserver(OnAbilityClick, DidAbilityClick);
		this.RemoveObserver(OnActiveTurnClick, DidConfirmClick);
		this.RemoveObserver(OnActiveTurnClick, DidBackClick);
		this.RemoveObserver(OnActiveTurnClick, ActiveTurnTargetUnitMapNotification);
	}

	//to do: implement combat start. some potential ideas:
    //sent from Board if hostile units within proximity
    //sent from PlayerManager if not same teams target each other
    void OnCombatStart(object sender, object args)
    {
        EnterCombat();
    }
	#endregion


	#region UserInputs
	//moveevent and fireevent listeners inherited from CombatState from InputController
	//OnMove is generally moving the cursor with WASD (some target/direction navigation as well)
	//depending on WalkAroundInput.X move keys do different things
	protected override void OnMove(object sender, InfoEventArgs<Point> e)
	{
		if( unitCommand != WalkAroundInput.UnitWait && unitCommand != WalkAroundInput.UnitAbilityConfirm)
			SelectTile(e.info + pos);
		//Debug.Log("in onMove unit command: " + unitCommand);
		//RefreshPrimaryStatPanel(pos);
		targetPanel.SetTargetPreview(board, pos);

		if( unitCommand == WalkAroundInput.UnitNone) //no unit is selected
		{
			//Debug.Log("in onMove 1a unit command: " + unitCommand );
			puActor = targetPanel.SetWalkAroundTargetPreview(board, pos);
			if (puActor != null)
			{
				//Debug.Log("in onMove 1b unit command: " + unitCommand);
				if (IsEligiblePlayerUnit(puActor))
				{
					if (StatusManager.Instance.IsContinueAbility(puActor))
						ConfirmContinue(puActor);
					else if( owner.board.IsMapMove(puActor) && StatusManager.Instance.IsTurnActable(puActor.TurnOrder)) 
						ConfirmMoveMap(puActor); //sets WalkAroundInput to UnitMoveMap, queries to see if user wants to move
					else
						ConfirmSelect(puActor);
					//Debug.Log("in onMove 1c unit command: " + unitCommand);
				}					
			}
		}
		//else if( unitCommand == WalkAroundInput.UnitSelected)
		//{
		//	//don't need to do anything here at this point
		//}
		else if (unitCommand == WalkAroundInput.UnitEligible) //can give a unit a menu command
		{
			//Debug.Log("in onMove 3a unit command: " + unitCommand);
			puActor = targetPanel.SetWalkAroundTargetPreview(board, pos);
			if (localTurn.walkAroundMoveTile == null && localTurn.spellName == null)
				ConfirmUnselect(); //no turn/move selected can navigate around
			//else
			//	ConfirmCancel(); //go back to top menu showing
		}
		else if (unitCommand == WalkAroundInput.UnitMove ) //inputted move command, now selecting target
		{
			//Debug.Log("decide on tile highlight as mover moves");
			//RefreshPrimaryStatPanel(pos);
			targetPanel.SetTargetPreview(board, pos);
			activeMenu.SetMenuMoveWalkAround(localTurn, tiles.Contains(owner.currentTile));
		}
		//else if (unitCommand == WalkAroundInput.UnitAct)
		//{

		//}
		//else if(unitCommand == WalkAroundInput.UnitAbilitySelect)
		//{
		//	//nothing done here
		//}
		else if (unitCommand == WalkAroundInput.UnitAbilityTarget) //ability selected and now targetting the ability
		{
			if (directionTarget)
			{
				//Debug.Log("testing onmove in combattargetabilitystate" + e.info + " " + pos.x + " " + pos.y);
				ChangeDirection(e.info);
			}
			else
			{
				//Debug.Log("b");
				//move cursor around, if valid move can confirm if not can only cancel
				targetPanel.SetTargetPreview(board, pos);
				activeMenu.SetMenuMoveWalkAround(localTurn, tiles.Contains(owner.currentTile));
				//this option only way to move to next unitCommand is with space/click on the target. other option moves to next unitCommand on move command
				//targetPanel.SetTargetPreview(board, pos);
				//if (tiles.Contains(board.GetTile(pos)))
				//	AbilityTargetSelected();
				//else
				//	targetPanel.SetTargetPreview(board, pos);
			}
		}
		else if (unitCommand == WalkAroundInput.UnitAbilityConfirm) //confirming the ability target
		{
			//cursor doesn't move, only switches between targets
			if (e.info.y > 0 || e.info.x > 0)
				targetIndex = SetTarget(targetIndex + 1);
			else
				targetIndex = SetTarget(targetIndex - 1);
		}
		else if (unitCommand == WalkAroundInput.UnitWait) //ending unit turn
		{
			//nothing here, should only change the direction menu
			localTurn.endDir = e.info.GetDirection();
			//PlayerManager.Instance.SetPUODirectionMidTurn(localTurn.actor.TurnOrder, localTurn.endDir, false);
			//Debug.Log("Ending turn with onWait click");
			//owner.facingIndicator.SetDirection(localTurn.endDir);
			ConfirmWait(localTurn.endDir);
		}
		else if (unitCommand == WalkAroundInput.UnitContinue) //unit has an action from prior turn that hasn't resolved
		{
			puActor = targetPanel.SetWalkAroundTargetPreview(board, pos);
			if (puActor != null)
			{
				//Debug.Log("in onMove 1b unit command: " + unitCommand);
				if (IsEligiblePlayerUnit(puActor))
				{
					if (StatusManager.Instance.IsContinueAbility(puActor))
						ConfirmContinue(puActor);
					else
						ConfirmSelect(puActor);
					//Debug.Log("in onMove 1c unit command: " + unitCommand);
				}
			}
			else
			{
				unitCommand = WalkAroundInput.UnitNone;
				activeMenu.Close(); //close the confirm/continue menu
			}
		}
		else if( unitCommand == WalkAroundInput.UnitMoveMap) //unir 
		{
			puActor = targetPanel.SetWalkAroundTargetPreview(board, pos);
			if (puActor != null)
			{
				//Debug.Log("in onMove 1b unit command: " + unitCommand);
				if (IsEligiblePlayerUnit(puActor))
				{
					if (owner.board.IsMapMove(puActor) && StatusManager.Instance.IsTurnActable(puActor.TurnOrder))
						ConfirmMoveMap(puActor); //see if unit wants to move
					else
						ConfirmSelect(puActor);
					//Debug.Log("in onMove 1c unit command: " + unitCommand);
				}
			}
			else
			{
				unitCommand = WalkAroundInput.UnitNone;
				activeMenu.Close(); //close the confirm/continue menu
			}
		}
	}

	//handles user input in form of non-directional button presses (left click, right click, spacebar)
		//left click is 0 and is confirm/select; right click is 1 and is cancel, spacebar is 2
		//different functionality depending on what WalkAroundInput.X and what is clicked on
	protected override void OnFire(object sender, InfoEventArgs<int> e)
	{
		//Debug.Log("fire event regsitered " + e.info);
		if (unitCommand == WalkAroundInput.UnitNone)
		{
			if (e.info == 1 || e.info == 2)
			{
				//bring up the menu if a pointer is on a unit
				if (puActor != null)
				{
					if (IsEligiblePlayerUnit(puActor))
					{
						if (StatusManager.Instance.IsContinueAbility(puActor))
							ConfirmContinue(puActor);
						else if (owner.board.IsMapMove(puActor) && StatusManager.Instance.IsTurnActable(puActor.TurnOrder)) //see if unit eligible to move map
							ConfirmMoveMap(puActor); //pull up menu to see if unit wants to move, changes to WalkAroundInput.UnitMoveMap 
						else
							ConfirmSelect(puActor);
					}
				}
			}
			else if (e.info == 0)
			{
				if (EventSystem.current.IsPointerOverGameObject())
				{
					return;
				}

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))
				{
					GameObject hitObject = hit.transform.gameObject;
					Tile tile = hitObject.GetComponent<Tile>();
					if (tile != null)
					{
						SelectTile(tile.pos);
						targetPanel.SetTargetPreview(tile);
					}
					else
					{
						PlayerUnitObject puo = hitObject.GetComponent<PlayerUnitObject>();
						if (puo != null)
						{
							puActor = PlayerManager.Instance.GetPlayerUnit(puo.UnitId);
							SelectTile(puActor);
							targetPanel.SetTargetPreview(puActor);
							if (IsEligiblePlayerUnit(puActor))
							{
								ConfirmSelect(puActor);
							}
								
						}
					}
				}
			}
		}
		//else if (unitCommand == WalkAroundInput.UnitSelected)
		//{
		//	//don't need to do anything here at this point
		//}
		else if (unitCommand == WalkAroundInput.UnitEligible)
		{
			//can left click on tiles/units to show them in target panel
			//can right click to cancel and exit top menu
			if (e.info == 0)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))
				{
					GameObject hitObject = hit.transform.gameObject;
					Tile tile = hitObject.GetComponent<Tile>();
					if (tile != null)
					{
						SelectTile(tile.pos);
						targetPanel.SetTargetPreview(board, pos);
					}
					else
					{
						PlayerUnitObject puo = hitObject.GetComponent<PlayerUnitObject>();
						if (puo != null)
						{
							puActor = PlayerManager.Instance.GetPlayerUnit(puo.UnitId);
							SelectTile(puActor);
							targetPanel.SetTargetPreview(puActor);
						}
					}
				}
			}
			else if (e.info == 1)
			{
				//cancel command go back to no unit selected
				ConfirmUnselect();
			}
		}
		else if (unitCommand == WalkAroundInput.UnitMove)
		{
			//unit has selected the move command and is now choosing which tile to move to
			if (e.info == 0)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))
				{
					GameObject hitObject = hit.transform.gameObject;
					Tile tile = hitObject.GetComponent<Tile>();
					if (tile != null)
					{
						SelectTile(tile.pos);
						targetPanel.SetTargetPreview(board, pos);
						if (tiles.Contains(owner.currentTile))
						{
							activeMenu.SetMenuMove(localTurn);
						}
					}

				}
			}
			else if (e.info == 2)
			{
				//move confirmed through menu confirm button being highlighted, if not causes issues where active turn menu also consumes the space bar click
					//has to be a better way to fucking do this
				Debug.Log("move confirmed with space bar ");
				//ConfirmMove();
			}
			else if (e.info == 1)
			{
				//cancel command go back to top menu
				ConfirmCancel();
			}
		}
		//else if (unitCommand == WalkAroundInput.UnitAct)
		//{

		//}
		else if (unitCommand == WalkAroundInput.UnitAbilitySelect)
		{
			//fire not done here, have to select ability from ability menu
			if (e.info == 1)
			{
				abilityMenu.Close();
				//cancel command go back to top menu
				ConfirmCancel();
			}
		}
		else if (unitCommand == WalkAroundInput.UnitAbilityTarget)
		{
			//cancel button is hit go back a menu. no confirm here, confirm is done by selecting the ability
			if (e.info == 1)
			{
				//cancel command go back to top menu
				ConfirmCancel();
			}
			else if (e.info == 2)
			{
				//ability target selected
				Debug.Log("targeting ability with space bar");
				//AbilityTargetSelected();
			}
			else if (e.info == 0)
			{
				if (EventSystem.current.IsPointerOverGameObject())
				{ //clicking on an ability in scrolllist can cause this to be triggered
				  // Debug.Log("point is over game object");
					return;
				}


				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))
				{
					GameObject hitObject = hit.transform.gameObject;
					Tile tile = hitObject.GetComponent<Tile>();
					if (tile != null)
					{
						targetPanel.SetTargetPreview(tile);
						SelectTile(tile.pos);
						if (tiles.Contains(board.GetTile(pos)))
							AbilityTargetSelected();
						else
							targetPanel.SetTargetPreview(board, pos);
					}
					else
					{
						PlayerUnitObject puo = hitObject.GetComponent<PlayerUnitObject>();
						if (puo != null)
						{
							PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(puo.UnitId);
							targetPanel.SetTargetPreview(pu);
							SelectTile(pu);
							if (tiles.Contains(board.GetTile(pos)))
								AbilityTargetSelected();
							else
								targetPanel.SetTargetPreview(board, pos);
						}
					}

				}
			}
		}
		else if (unitCommand == WalkAroundInput.UnitAbilityConfirm)
		{
			//ability and target ahve been selected, confirm them both
			if (e.info == 2)
			{
				Debug.Log("testing confirm on fire 2 in confirm ability target state ");
				localTurn.targetUnitId = NameAll.NULL_UNIT_ID;
				localTurn.targetTile = currentTile; //have to do this because the targetTile can change based on what is being shown in the preview box 
				//ConfirmAbility(); //don't call it here, do it through highlighting confirm button in menu and using space bar click here
				//having issues confirming button clicks so that's an alternative way of doin git
			}
			else if (e.info == 1) //cancel
			{
				//cancel command go back to top menu
				ConfirmCancel();
			}
			else if (e.info == 0)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))
				{
					GameObject hitObject = hit.transform.gameObject;
					Tile t = hitObject.GetComponent<Tile>();
					if (localTurn.targets.Contains(t))
					{
						SelectTile(t.pos);
						targetIndex = SetTarget(t);
					}
				}
			}
		}
		else if (unitCommand == WalkAroundInput.UnitWait)
		{
			//can cancel to move back
			if (e.info == 1) //cancel
			{
				//cancel command go back to top menu
				ConfirmCancel();
			}
		}
		else if(unitCommand == WalkAroundInput.UnitContinue)
		{
			if (e.info == 1) //cancel
			{
				//stop casting the ability
				ConfirmContinueCancel(localTurn.actor);
			}
			else if(e.info == 2)
			{
				//continue casting the ability
				ConfirmContinueAbility();
			}
			else if (e.info == 0)
			{
				if (EventSystem.current.IsPointerOverGameObject())
				{ //clicking on an ability in scrolllist can cause this to be triggered
				  // Debug.Log("point is over game object");
					return;
				}

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))
				{
					GameObject hitObject = hit.transform.gameObject;
					Tile tile = hitObject.GetComponent<Tile>();
					if (tile != null)
					{
						targetPanel.SetTargetPreview(tile);
						SelectTile(tile.pos);
						unitCommand = WalkAroundInput.UnitNone;
					}
					else
					{
						PlayerUnitObject puo = hitObject.GetComponent<PlayerUnitObject>();
						if (puo != null)
						{
							PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(puo.UnitId);
							targetPanel.SetTargetPreview(pu);
							SelectTile(pu);
						}
						unitCommand = WalkAroundInput.UnitNone;
					}

				}
			}
		}
		else if (unitCommand == WalkAroundInput.UnitMoveMap)
		{
			if (e.info == 1) //cancel
			{
				//don't move the map
				//ConfirmContinueCancel(localTurn.actor);
				ConfirmMoveMapCancel(localTurn.actor);
			}
			else if (e.info == 2)
			{
				//move map
				DoMoveMap(localTurn.actor, owner.board);
			}
			else if (e.info == 0)
			{
				if (EventSystem.current.IsPointerOverGameObject())
				{ //clicking on an ability in scrolllist can cause this to be triggered
				  // Debug.Log("point is over game object");
					return;
				}

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))
				{
					GameObject hitObject = hit.transform.gameObject;
					Tile tile = hitObject.GetComponent<Tile>();
					if (tile != null)
					{
						targetPanel.SetTargetPreview(tile);
						SelectTile(tile.pos);
						unitCommand = WalkAroundInput.UnitNone;
					}
					else
					{
						PlayerUnitObject puo = hitObject.GetComponent<PlayerUnitObject>();
						if (puo != null)
						{
							PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(puo.UnitId);
							targetPanel.SetTargetPreview(pu);
							SelectTile(pu);
						}
						unitCommand = WalkAroundInput.UnitNone;
					}

				}
			}
		}

	}

	//eligible unit is selected by player: open appropriate menus (actor panel and active turn)
	void OpenActorPanel(PlayerUnit pu, bool followUnit = false, bool isMoveMap = false)
	{
		actorPanel.SetActor(pu);
		HighlightActorTile(pu, true);
		if (followUnit)
		{
			cameraMain.Open(board.GetTile(pu));
			cameraMain.FollowUnit(pu.TurnOrder);
		}
		
		if( isMoveMap)
			activeMenu.SetMenuMoveMap(localTurn, owner.battleMessageController);
		else
			activeMenu.SetMenuTop(localTurn, owner.battleMessageController);
	}

	//check if PU eligible to have menu open
		//team check, not locked, TurnActable
	bool IsEligiblePlayerUnit(PlayerUnit pu)
	{
		//Debug.Log("reached here");
		return PlayerManager.Instance.IsWalkAroundPlayerUnitEligible(pu);
	}

	//click on top menu by user. after unit is selected and eligible unitCommand == WalkAroundInput.UnitEligible
	//also handles confirm/cancel clicks by user
	void OnActiveTurnClick(object sender, object args)
	{
		string str = (string)args; //Debug.Log("in onactiveturnclick " + str);

		if (str.Equals(DidWaitClick)) //submits turn to be processed by queue and ends teh action
		{

			//Debug.Log("Did wait Click about to exit");
			//activeMenu.Close();
			//PlayerManager.Instance.AddWalkAroundActionObject(localTurn);
			//PlayerManager.Instance.WalkAroundInputTurn(localTurn);
			board.UnhighlightTile(localTurn.actor);
			unitCommand = WalkAroundInput.UnitWait;
			activeMenu.SetMenuDirection(localTurn);
			//owner.ChangeState<WalkAroundPlayerState>();
		}
		else if (str.Equals(DidMoveClick))
		{
			//Debug.Log("Did move click");
			//owner.ChangeState<WalkAroundMoveTargetState>();
			//owner.ChangeState<CombatMoveTargetState>();
			board.UnhighlightTile(localTurn.actor);
			
			unitCommand = WalkAroundInput.UnitMove;
			//show eligible tiles
			var puoComponent = PlayerManager.Instance.GetPlayerUnitObjectComponent(localTurn.actor.TurnOrder);
			tiles = puoComponent.GetWalkAroundTilesInRange(board, board.GetTile(localTurn.actor), localTurn.actor);
			board.SelectTiles(tiles);
			targetPanel.SetTargetPreview(board, pos);
			activeMenu.SetMenuMoveWalkAround(localTurn, tiles.Contains(owner.currentTile));

		}
		else if (str.Equals(DidAttackClick))
		{
			//Debug.Log("Did attack click unit command is " + unitCommand );
			board.UnhighlightTile(localTurn.actor);
			localTurn.spellName = SpellManager.Instance.GetSpellNameByIndex(CalculationAT.GetAttackSpellIndex(localTurn.actor.TurnOrder));
			AbilitySelected();
		}
		else if (str.Equals(DidPrimaryClick))
		{
			//Not doing anything with Math Skill/nested abilities
			board.UnhighlightTile(localTurn.actor);
			localTurn.isPrimary = true;
			activeMenu.Close();
			abilityMenu.Open(localTurn, isMathSkill: false);
			unitCommand = WalkAroundInput.UnitAbilitySelect;
			//owner.ChangeState<WalkAroundAbilitySelectState>();
		}
		else if (str.Equals(DidSecondaryClick))
		{
			//Not doing anything with Math Skill/nested abilities
			board.UnhighlightTile(localTurn.actor);
			localTurn.isPrimary = false;
			activeMenu.Close();
			abilityMenu.Open(localTurn, isMathSkill: false);
			unitCommand = WalkAroundInput.UnitAbilitySelect;
			//owner.ChangeState<WalkAroundAbilitySelectState>();
		}
		else if (str.Equals(DidConfirmClick))
		{
			//Debug.Log("testing didconfirmclick");
			if (unitCommand == WalkAroundInput.UnitMove)
				ConfirmMove();
			else if (unitCommand == WalkAroundInput.UnitAbilityConfirm)
				ConfirmAbility();
			else if (unitCommand == WalkAroundInput.UnitAbilityTarget)
				AbilityTargetSelected();
			else if (unitCommand == WalkAroundInput.UnitContinue)
				ConfirmContinueAbility();
			else if (unitCommand == WalkAroundInput.UnitMoveMap)
				DoMoveMap(localTurn.actor, owner.board);
		}
		else if (str.Equals(DidBackClick))
		{
			//Debug.Log("testing cancelclick 0");
			if (unitCommand == WalkAroundInput.UnitContinue)
			{
				//Debug.Log("testing cancelclick 1");
				ConfirmContinueCancel(localTurn.actor);
			}
			else if (unitCommand == WalkAroundInput.UnitMoveMap)
				ConfirmMoveMapCancel(localTurn.actor);
			else
				ConfirmCancel();
		}
		else if (args.Equals(DidTargetUnitClick))
		{
			localTurn.targetUnitId = localTurn.targetTile.UnitId;
			localTurn.targetTile = currentTile;
			ConfirmAbility();
		}
		else if (args.Equals(DidTargetMapClick))
		{
			localTurn.targetUnitId = NameAll.NULL_UNIT_ID;
			localTurn.targetTile = currentTile;
			ConfirmAbility();
		}
	}

	//end is clicked or an ability requires a direction, now select a direction
	void OnDirectionClick(object sender, object args)
	{
		Directions dir;
		string str = (string)args;
		//Debug.Log("recieved on direction click from UIActiveTurnMenu 0 " + str);

		if (unitCommand == WalkAroundInput.UnitWait)
		{
			//Debug.Log("recieved on direction click from UIActiveTurnMenu 1 " + str);
			if (str.Equals(DidNorthClick))
			{
				dir = Directions.North;
				ConfirmWait(dir);
			}
			else if (str.Equals(DidEastClick))
			{
				dir = Directions.East;
				ConfirmWait(dir);
			}
			else if (str.Equals(DidSouthClick))
			{
				dir = Directions.South;
				ConfirmWait(dir);
			}
			else if(str.Equals(DidWestClick))
			{
				dir = Directions.West;
				ConfirmWait(dir);
			}
			
		}
		else if(unitCommand == WalkAroundInput.UnitAbilityTarget)
		{
			if (str.Equals(DidNorthClick))
			{
				dir = Directions.North;
			}
			else if (str.Equals(DidEastClick))
			{
				dir = Directions.East;
			}
			else if (str.Equals(DidSouthClick))
			{
				dir = Directions.South;
			}
			else
			{
				dir = Directions.West;
			}
			ChangeDirection(dir);
			AbilityTargetSelected();
		}
	}

	
	//ability from ability menu is clicked. opens up targetting panels
	void OnAbilityClick(object sender, object args)
	{
		int abilityId = (int)args;
		localTurn.spellName = SpellManager.Instance.GetSpellNameByIndex(abilityId); //Debug.Log("on ability click " + localTurn.spellName.CommandSet);
		if (CalculationAT.IsAutoTargetAbility(localTurn.spellName)) //auto target abilities move to confirm target, else move to ability select
		{
			AbilityTargetSelected();
		}
		else
		{
			AbilitySelected();
		}	
	}

	#region ability targetting functions
	//can move these and access the functions for here and other combat states from that file

	//ability is selected, now move to phase where ability target is selected
	void AbilitySelected()
	{
		unitCommand = WalkAroundInput.UnitAbilityTarget;
		SelectTile(localTurn.actor); //think this is needed as juggling between this and CombatConfirmAbilityTargetState can cause currentTile to shift and move around self target abilities
		ar = new CombatAbilityRange();

		actorPanel.SetActor(localTurn.actor);
		currentDir = localTurn.actor.Dir;
		localTurn.endDir = currentDir;
		SelectAbilityTiles(currentDir);

		//attack can be targetted automatically due to AI or the attack ability
		directionTarget = GetDirectionTarget();
		if (directionTarget)
		{
			activeMenu.SetMenuTargetDirectionWalkAround(localTurn);
		}
		else
		{
			activeMenu.SetMenuMoveWalkAround(localTurn, tiles.Contains(owner.currentTile)); //yes MOVE is intentional
		}
	}

	//ability selected but no target selected, function to help with targetting
	bool GetDirectionTarget()
	{
		SpellName sn = localTurn.spellName;
		if (sn.CommandSet == NameAll.COMMAND_SET_MATH_SKILL && localTurn.spellName2 != null)
		{
			sn = localTurn.spellName2;
		}

		if (sn.EffectXY >= NameAll.SPELL_EFFECT_CONE_BASE && sn.EffectXY <= NameAll.SPELL_EFFECT_CONE_MAX)
		{
			return true;
		}
		else if (sn.RangeXYMax == SPELL_RANGE_LINE)
		{
			return true;
		}

		return false;
	}

	//ability selected but no target selected, function to help with targetting
	void ChangeDirection(Point p)
	{
		Directions dir = p.GetDirection(); //Debug.Log("dir is " + dir.ToString());
		if (currentDir != dir)
		{
			PlayerManager.Instance.SetPUODirectionMidTurn(localTurn.actor.TurnOrder, dir, false);
			currentDir = dir;
			localTurn.endDir = currentDir; //used in CombatConfirmAbilityTargetState
			board.DeSelectTiles(tiles);
			SelectAbilityTiles(currentDir);
		}
	}

	//ability selected but no target selected, function to help with targetting
	void ChangeDirection(Directions dir)
	{
		//Debug.Log("dir is " + dir.ToString());
		if (currentDir != dir)
		{
			PlayerManager.Instance.SetPUODirectionMidTurn(localTurn.actor.TurnOrder, dir, false);
			currentDir = dir;
			localTurn.endDir = currentDir;
			board.DeSelectTiles(tiles);
			SelectAbilityTiles(currentDir);
		}
	}

	//ability selected but no target selected, function to help with targetting
	void SelectAbilityTiles(Directions dir)
	{
		SpellName sn = localTurn.spellName;
		if (sn.CommandSet == NameAll.COMMAND_SET_MATH_SKILL && localTurn.spellName2 != null)
		{
			sn = localTurn.spellName2;
		}
		//Debug.Log("am I here?");
		tiles = ar.GetTilesInRange(board, localTurn.actor, sn, dir);
		board.SelectTiles(tiles);

	}

	//ability selected and target selected
	void AbilityTargetSelected()
	{
		board.DeSelectTiles(tiles);
		unitCommand = WalkAroundInput.UnitAbilityConfirm;
		SetConfirmTargetButtons(localTurn, currentTile); //sets menu for either confirm/cancel or target unit/map
		aa = new CombatAbilityArea();
		tiles = aa.GetTilesInArea(board, localTurn.actor, localTurn.spellName, currentTile, localTurn.endDir);
		localTurn.targetTile = currentTile;
		board.SelectTiles(tiles);

		//targetting panels opened, now showing hit chance preview
		targetIndex = 0;
		FindTargets();
		if (localTurn.targets.Count > 0)
		{
			if (driver == Drivers.Human)
				UpdateHitSuccessIndicator(targetIndex);//hitSuccessIndicator.Show();
			targetIndex = SetTarget(targetIndex);
		}
		else
		{
			SetHitPreview(currentTile);
		}
	}

	//when an ability is selected, do a preview by finding targets
	void FindTargets()
	{
		localTurn.targets = new List<Tile>();
		for (int i = 0; i < tiles.Count; ++i)
		{
			if (tiles[i].UnitId != NameAll.NULL_UNIT_ID)
			{
				localTurn.targets.Add(tiles[i]); //check for casterimmune and enemies/allies occurs in combatabilityarea
			}

		}
	}

	//show previewing of hit chance when ability is targetting but not confirmed
	void SetHitPreview(Tile targetTile)
	{
		List<string> strList = CalculationAT.GetHitPreview(board, localTurn, targetTile);
		previewPanel.SetHitPreview(strList[0], strList[1], strList[2], strList[3], strList[4]);
	}

	//show previwing of hit chance when ability is targetting but not confirmed
	void UpdateHitSuccessIndicator(int tgtIndex)
	{
		Tile targetTile = localTurn.targets[tgtIndex];
		SetHitPreview(targetTile);
		//Debug.Log("asdf 1");
	}

	//show previwing of hit chance when ability is targetting but not confirmed
	int SetTarget(Tile t)
	{
		int tempIndex = 0;
		for (int i = 0; i < localTurn.targets.Count; i++)
		{
			if (localTurn.targets[i] == t)
			{
				tempIndex = i;
				targetPanel.SetTargetPreview(localTurn.targets[tempIndex]);
				UpdateHitSuccessIndicator(tempIndex);
				localTurn.targetTile = t;
				break;
			}
		}
		return tempIndex;
	}

	//show previwing of hit chance when ability is targetting but not confirmed
	int SetTarget(int target)
	{
		if (target < 0)
			target = localTurn.targets.Count - 1;
		if (target >= localTurn.targets.Count)
			target = 0;

		if (localTurn.targets.Count > 0)
		{

			targetPanel.SetTargetPreview(localTurn.targets[target]);
			UpdateHitSuccessIndicator(target);
			localTurn.targetTile = localTurn.targets[target];
		}
		else
		{
			targetPanel.SetTargetPreview(currentTile);
			localTurn.targetTile = currentTile;
		}
		return target;
	}
	#endregion


	//when targeting, either confirm/cancel buttons show up or target unit/map
	void SetConfirmTargetButtons(CombatTurn tu, Tile t)
	{
		if (t.UnitId != NameAll.NULL_UNIT_ID && SpellManager.Instance.IsTargetUnitMap(tu.spellName, t.UnitId))
		{
			activeMenu.SetMenuAbilityConfirm(tu, true);
			//Debug.Log("can target unit");
		}
		else
		{
			activeMenu.SetMenuAbilityConfirmWalkAround(tu, false);
			//Debug.Log("cannot target unit");
		}

	}


	//confirming various transitions after user input/turn
	//confirm that the unit has been selected. reset the turn
	void ConfirmSelect(PlayerUnit pu)
	{
		localTurn.Change(pu);
		//currentDir can change sometimes, reset it here
		currentDir = pu.Dir;
		OpenActorPanel(puActor, true);
		unitCommand = WalkAroundInput.UnitEligible;
	}

	//unit is selected for new turn but still performing ability from last turn
	void ConfirmContinue(PlayerUnit pu)
	{
		localTurn.Change(pu);
		unitCommand = WalkAroundInput.UnitContinue;
		OpenActorPanel(puActor, true);
	}

	//ability from last turn is cancelled
	void ConfirmContinueCancel(PlayerUnit pu)
	{
		//remove the WAAO from PlayerManager
		PlayerManager.Instance.RemoveWAAO(pu.TurnOrder);
		//remove charing/performing status from the unit
		StatusManager.Instance.RemoveStatus(pu.TurnOrder, NameAll.STATUS_ID_CHARGING);
		StatusManager.Instance.RemoveStatus(pu.TurnOrder, NameAll.STATUS_ID_PERFORMING);
		//start a new turn
		ConfirmSelect(pu);
	}

	//canceling move map
	void ConfirmMoveMapCancel(PlayerUnit pu)
	{
		ConfirmSelect(pu);
	}

	//ability from last turn is continued
	void ConfirmContinueAbility()
	{
		//keep casting the ability, close the confirm menu
		activeMenu.Close();
	}

	//confirm ability: user has inputted an ability
	void ConfirmAbility()
	{
		board.DeSelectTiles(tiles);
		localTurn.hasUnitActed = true;
		if (localTurn.spellName.CommandSet == NameAll.COMMAND_SET_JUMP || localTurn.spellName.CommandSet == NameAll.COMMAND_SET_CHARGE)
		{
			localTurn.hasUnitMoved = true;
		}
		previewPanel.Close();
		OpenActorPanel(puActor);
		unitCommand = WalkAroundInput.UnitEligible;
	}

	//confirmed move after move selected and then confirmed with menu click or spacebar
	void ConfirmMove()
	{
		//Debug.Log("confirmMove 0");
		if(unitCommand == WalkAroundInput.UnitMove)
		{
			//Debug.Log("confirmMove 1");
			if (tiles.Contains(owner.currentTile))
			{
				//Debug.Log("confirmMove 2");
				localTurn.walkAroundMoveTile = owner.currentTile;
				localTurn.hasUnitMoved = true;
				if (!localTurn.hasUnitActed)
					localTurn.isWalkAroundMoveFirst = true;
				//goes back to top menu where player can act or wait to enter the WAAO for process
				OpenActorPanel(puActor);
				unitCommand = WalkAroundInput.UnitEligible;
				//Debug.Log("move confirmed, has unit moved: " + localTurn.hasUnitMoved);
			}
			//Debug.Log("confirmMove 3");
		}
		board.DeSelectTiles(tiles);
	}

	//unit is selected but cancel button hit so unselect unit. Close actor menu and panel and reset UnitCommand status
	void ConfirmUnselect()
	{
		unitCommand = WalkAroundInput.UnitNone;
		activeMenu.Close();
		actorPanel.Close();
		board.DeSelectTiles(tiles);
	}

	//cancel button clicked, go back to unit eligible menu part, keeps current turn
	void ConfirmCancel()
	{
		if (puActor != null)
		{
			if (IsEligiblePlayerUnit(puActor))
			{
				//turn direction back if current direction different from end direction
				if(currentDir != puActor.Dir)
				{
					ChangeDirection(puActor.Dir); //currentDir changed in here
				}
				OpenActorPanel(puActor);
				if (unitCommand == WalkAroundInput.UnitAbilityConfirm || unitCommand == WalkAroundInput.UnitAbilityTarget)
				{
					localTurn.spellName = null;
					previewPanel.Close();
				}
				unitCommand = WalkAroundInput.UnitEligible;
			}
		}
		else
		{
			unitCommand = WalkAroundInput.UnitNone;
		}
		board.DeSelectTiles(tiles);
	}

	//end turn is clicked
	void ConfirmWait(Directions dir)
	{
		if(unitCommand == WalkAroundInput.UnitWait)
		{
			localTurn.endDir = dir;
			PlayerManager.Instance.AddWalkAroundActionObject(localTurn);
			unitCommand = WalkAroundInput.UnitNone;
			activeMenu.Close();
			actorPanel.Close();
			board.DeSelectTiles(tiles);
		}
	}

	//confirm movemap
	void ConfirmMoveMap(PlayerUnit pu)
	{
		localTurn.Change(pu);
		unitCommand = WalkAroundInput.UnitMoveMap;
		OpenActorPanel(puActor, true, true);
	}

	void DoMoveMap(PlayerUnit pu, Board board)
	{
		PlayerManager.Instance.SaveWalkAroundPlayerUnits(NameAll.WA_UNIT_SAVE_MAP_LIST); //save any map units still alive
		PlayerManager.Instance.SetMapXYOnMapExit(pu, board.max, board.min); //set new current map so know what to load	
		PlayerManager.Instance.SetTeamList(PlayerManager.Instance.GetAllUnitsByTeamId(NameAll.TEAM_ID_WALK_AROUND_GREEN), NameAll.TEAM_ID_WALK_AROUND_GREEN); //load player's units from this temp list
		//delete objects and restart PM in WalkAroundInitState
		owner.ChangeState<WalkAroundInitState>(); //change t

	}
	#endregion

	//this should be abstracted out and accessible to combat as well
	void AddVictoryCondition()
	{
		int victoryType;
		//if (isOffline)
		//{
		//	victoryType = PlayerPrefs.GetInt(NameAll.PP_VICTORY_TYPE, NameAll.VICTORY_TYPE_DEFEAT_PARTY); //Debug.Log("victory type is " + victoryType );
		//	if (PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_GREEN).Count == 0 || PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_RED).Count == 0)
		//	{
		//		victoryType = NameAll.VICTORY_TYPE_NONE; //Debug.Log("victoryType set to none");
		//	}
		//}
		//else
		//{
		//	victoryType = NameAll.VICTORY_TYPE_DEFEAT_PARTY;
		//}
		victoryType = NameAll.VICTORY_TYPE_DEFEAT_PARTY;
		CombatVictoryCondition vc = owner.gameObject.AddComponent<CombatVictoryCondition>();
		vc.VictoryType = victoryType;

	}

}
