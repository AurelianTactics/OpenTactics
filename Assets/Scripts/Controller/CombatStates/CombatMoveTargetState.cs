using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatMoveTargetState : CombatState
{

    List<Tile> tiles;
    bool isOffline;
    bool isMasterClient;


    public override void Enter()
    {
        base.Enter(); //Debug.Log("move target state enter");
        isOffline = PlayerManager.Instance.IsOfflineGame();
        isMasterClient = PlayerManager.Instance.isMPMasterClient();

        EnableObservers();

        actorPanel.SetActor(turn.actor);

        var puoComponent = PlayerManager.Instance.GetPlayerUnitObjectComponent(turn.actor.TurnOrder);
        tiles = puoComponent.GetTilesInRange(board, board.GetTile(turn.actor), turn.actor);
        //Movement mover = turn.actor.GetComponent<Movement>();
        //tiles = mover.GetTilesInRange(board);
        board.SelectTiles(tiles);
        
        targetPanel.SetTargetPreview(board,pos);

        if (!isOffline && !isMasterClient)
        { //online game and other's turn. master already did ai checks. waiting on other's input
            return;
        }

        if (driver == Drivers.Computer || StatusManager.Instance.IsAIControlledStatus(turn.actor.TurnOrder) || driver == Drivers.ReinforcementLearning)
            StartCoroutine(ComputerHighlightMoveTarget());
    }

    public override void Exit()
    {
        DisableObservers();
        board.DeSelectTiles(tiles);
        tiles = null;
        targetPanel.Close();
        activeMenu.Close();
        //statPanelController.HidePrimary();
        base.Exit(); //Debug.Log("move target state exit");
    }

    protected override void OnMove(object sender, InfoEventArgs<Point> e)
    {
        SelectTile(e.info + pos);
        //Debug.Log("decide on tile highlight as mover moves");
        //RefreshPrimaryStatPanel(pos);
        targetPanel.SetTargetPreview(board,pos);
        if (tiles.Contains(owner.currentTile))
        {
            activeMenu.SetMenuMove(turn);
        }
        else
            activeMenu.Close();
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
        if( e.info == 0)
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
                        activeMenu.SetMenuMove(turn);
                    }
                }
                
            }
        }
        else if (e.info == 2)
        {
            ConfirmMove();
        }
        else if( e.info == 1)
        {
            owner.ChangeState<CombatCommandSelectionState>();
        }
    }

    IEnumerator ComputerHighlightMoveTarget()
    {
        Point cursorPos = pos;
        while (cursorPos != turn.plan.moveLocation)
        {
            if (cursorPos.x < turn.plan.moveLocation.x) cursorPos.x++;
            if (cursorPos.x > turn.plan.moveLocation.x) cursorPos.x--;
            if (cursorPos.y < turn.plan.moveLocation.y) cursorPos.y++;
            if (cursorPos.y > turn.plan.moveLocation.y) cursorPos.y--;
            SelectTile(cursorPos);
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(0.5f);
        owner.ChangeState<CombatMoveSequenceState>(); //eh confirm move not needed
    }

    #region notifications
    const string ActiveTurnConfirmNotification = "ActiveTurnConfirmNotification";
    const string ActiveTurnBackNotification = "ActiveTurnBackNotification";
    const string DidConfirmClick = "ActiveTurn.ConfirmClick";
    const string DidBackClick = "ActiveTurn.BackClick";

    //void OnEnable()
    //{
    //    this.AddObserver(OnActiveTurnClick, ActiveTurnConfirmNotification);
    //    this.AddObserver(OnActiveTurnClick, ActiveTurnBackNotification);
    //}

    //void OnDisable()
    //{
    //    this.RemoveObserver(OnActiveTurnClick, ActiveTurnConfirmNotification);
    //    this.RemoveObserver(OnActiveTurnClick, ActiveTurnBackNotification);
    //}

    void EnableObservers()
    {
        this.AddObserver(OnActiveTurnClick, ActiveTurnConfirmNotification);
        this.AddObserver(OnActiveTurnClick, ActiveTurnBackNotification);
    }

    void DisableObservers()
    {
        this.RemoveObserver(OnActiveTurnClick, ActiveTurnConfirmNotification);
        this.RemoveObserver(OnActiveTurnClick, ActiveTurnBackNotification);
    }

    void OnActiveTurnClick(object sender, object args)
    {
        string str = (string)args;

        if (str.Equals(DidConfirmClick))
        {
            //if( tiles == null)
            //{
            //    Debug.Log("a");
            //}
            //if( owner.currentTile == null)
            //{
            //    Debug.Log("b");
            //}
            ConfirmMove();
        }
        else if (str.Equals(DidBackClick))
        {
            owner.ChangeState<CombatCommandSelectionState>();
        }
    }

    //called when user inputs move tile
    void ConfirmMove()
    {
        if (tiles.Contains(owner.currentTile))
        {
            if(!isOffline)
            {
                //online game: master send info to other so other can see what master is about to do OR other (p2) sends move command to master
                PlayerManager.Instance.SendMPActiveTurnInput(isMove: true, isAct: false, isWait: false, actorId: turn.actor.TurnOrder, tileX: owner.currentTile.pos.x, tileY: owner.currentTile.pos.y,
                    directionInt: turn.actor.Dir.DirectionToInt(), targetId: NameAll.NULL_INT, spellIndex: NameAll.NULL_INT, spellIndex2: NameAll.NULL_INT);
                if(!isMasterClient)
                {
                    turn.hasUnitMoved = true; //not sure if necessary since this gets updated by master when control passed back here
                    owner.ChangeState<GameLoopState>(); //returns to gameloopstate, master receives input and updates both games, then sees who has the next turn
                }
                else
                {
                    owner.ChangeState<CombatMoveSequenceState>();
                }
            }
            else 
                owner.ChangeState<CombatMoveSequenceState>();
        }
            
    }
    #endregion
}
