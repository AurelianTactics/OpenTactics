using UnityEngine;
using System.Collections;

public class CombatEndFacingState : BaseCombatAbilityMenuState
{

    Directions startDir;
    bool isOffline;
    bool isMasterClient;

    public override void Enter()
    {
        base.Enter(); //Debug.Log("combatendFacing state");
        isOffline = PlayerManager.Instance.IsOfflineGame();
        isMasterClient = PlayerManager.Instance.isMPMasterClient();

        actorPanel.SetActor(turn.actor);

        EnableObservers();
        startDir = turn.actor.Dir;
        SelectTile(turn.actor);
        //Debug.Log("Add select tile");
        owner.facingIndicator.gameObject.SetActive(true);
        owner.facingIndicator.SetDirection(turn.actor.Dir);
        //Debug.Log("in combatEndFacingState");

        if (!isOffline && !isMasterClient)
        { //online game and other's turn. master already did ai checks. waiting on other's input
            LoadMenu();
            return;
        }

        if (driver == Drivers.Computer || StatusManager.Instance.IsAIControlledStatus(turn.actor.TurnOrder))
            StartCoroutine(ComputerControl());
		else if( driver == Drivers.ReinforcementLearning)
			StartCoroutine(RLControl());
		else
            LoadMenu();
    }

    public override void Exit()
    {
        //Debug.Log("exiting wait function");
        activeMenu.Close();
        owner.facingIndicator.gameObject.SetActive(false);
        DisableObservers();
        base.Exit();
    }

    protected override void LoadMenu()
    {
        //Debug.Log("loading active turn menu");
        activeMenu.SetMenuDirection(turn);
        //activeMenu.SetMenuTop(turn);
    }

    protected override void Confirm( )
    {
        if (!isOffline)
        {
            //online game: master send info to other so other can see what master is about to do OR other (p2) sends move command to master
            PlayerManager.Instance.SendMPActiveTurnInput(isMove: false, isAct: false, isWait: true, actorId: turn.actor.TurnOrder, tileX: NameAll.NULL_INT, tileY: NameAll.NULL_INT,
                    directionInt: turn.endDir.DirectionToInt(), targetId: NameAll.NULL_INT, spellIndex: NameAll.NULL_INT, spellIndex2: NameAll.NULL_INT);
            if (!isMasterClient)
            {
                //Debug.Log("ending turn in ONLINE game, OTHER");
                owner.ChangeState<GameLoopState>(); //returns to gameloopstate, master receives input and updates both games, then sees who has the next turn
            }
            else
            {
                PlayerManager.Instance.EndCombatTurn(turn);
                owner.ChangeState<PostActiveTurnState>(); //need this before gameloopstate
            }
        }
        else
        {
            PlayerManager.Instance.EndCombatTurn(turn);
            owner.ChangeState<PostActiveTurnState>(); //need this before gameloopstate
        }
        
    }

    protected override void Cancel()
    {
        PlayerManager.Instance.SetPUODirectionMidTurn(turn.actor.TurnOrder, turn.actor.Dir);
        owner.ChangeState<CombatCommandSelectionState>();
    }

    protected override void OnMove(object sender, InfoEventArgs<Point> e)
    {
        //Debug.Log("EndFacing move input " + e.info.GetDirection());
        turn.endDir = e.info.GetDirection();
        PlayerManager.Instance.SetPUODirectionMidTurn(turn.actor.TurnOrder, turn.endDir,false);
        //Debug.Log("add Match() equivalent which turns and moves the unit in puo");
        owner.facingIndicator.SetDirection(turn.endDir);
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
        //Debug.Log("End facing on fire " + e.info.ToString());
        switch (e.info)
        {
            case 1:
                if (!turn.hasUnitActed || !turn.hasUnitActed)
                    Cancel();
                break;
            case 2: //confirm
                //Debug.Log("entering confirm");
                Confirm();
                break;
            
        }
    }

    IEnumerator ComputerControl()
    {
        yield return new WaitForSeconds(0.5f);
        turn.endDir = owner.cpu.DetermineEndFacingDirection(); //Debug.Log("end facing dir for computer is " + turn.endDir.ToString());
        //turn.actor.Match();
        owner.facingIndicator.SetDirection(turn.endDir);
        yield return new WaitForSeconds(0.5f);
        //owner.ChangeState<SelectUnitState>();
        PlayerManager.Instance.EndCombatTurn(turn);
        owner.ChangeState<PostActiveTurnState>();
    }

	IEnumerator RLControl()
	{
		yield return new WaitForSeconds(0.1f);
		turn.endDir = turn.plan.attackDirection; //Debug.Log("end facing dir for computer is " + turn.endDir.ToString());
		owner.facingIndicator.SetDirection(turn.endDir);
		yield return new WaitForSeconds(0.1f);
		//owner.ChangeState<SelectUnitState>();
		PlayerManager.Instance.EndCombatTurn(turn);
		owner.ChangeState<PostActiveTurnState>();
	}

	#region Notifications
	const string ActiveTurnWaitNotification = "ActiveTurnWaitNotification";
    const string ActiveTurnBackNotification = "ActiveTurnBackNotification";

    const string DidBackClick = "ActiveTurn.BackClick";
    const string DidNorthClick = "ActiveTurn.NorthClick";
    const string DidEastClick = "ActiveTurn.EastClick";
    const string DidSouthClick = "ActiveTurn.SouthClick";
    const string DidWestClick = "ActiveTurn.WestClick";

    void EnableObservers()
    {
        //Debug.Log("End facing state enabled");
        this.AddObserver(OnBackClick, ActiveTurnBackNotification);
        this.AddObserver(OnDirectionClick, ActiveTurnWaitNotification);
    }

    void DisableObservers()
    {
        this.RemoveObserver(OnBackClick, ActiveTurnBackNotification);
        this.RemoveObserver(OnDirectionClick, ActiveTurnWaitNotification);
    }

    void OnBackClick(object sender, object args)
    {
        Cancel(); //Debug.Log("Cancel performing and charging");
    }

    void OnDirectionClick(object sender, object args)
    {
        //Debug.Log("recieved on direction click from UIActiveTurnMenu");
        Directions dir;
        string str = (string)args;

        if(str.Equals(DidNorthClick))
        {
            dir = Directions.North;
        }
        else if(str.Equals(DidEastClick))
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
        turn.endDir = dir;
        owner.facingIndicator.SetDirection(turn.endDir);
        Confirm();
    }
    
    #endregion
}
