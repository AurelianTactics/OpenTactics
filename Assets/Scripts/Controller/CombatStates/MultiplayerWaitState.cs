using UnityEngine;
using System.Collections;

//in online games, master waits here for input from other (player 2)
//called from CombatCommandSelectionState when master reaches point that it's other turn
//sends RPC to other to trigger menu opening and allow other to send back the user input 
//internally sets variables so that gameLoop cannot proceed until user input received back

public class MultiplayerWaitState : CombatState
{

    public override void Enter()
    {
        base.Enter();
        EnableObservers();
        StartCoroutine(DoPhase());
    }

    public override void Exit()
    {
        base.Exit();
        DisableObservers();
    }

    IEnumerator DoPhase()
    {
        PlayerManager.Instance.SetMPOpponentReadyAndPhase(false, Phases.ActiveTurn);
        yield return null;
        PlayerManager.Instance.SendMPActiveTurnStartTurn(turn.actor.TurnOrder, turn.hasUnitActed, turn.hasUnitMoved);
    }

    const string MultiplayerActiveTurnMidTurn = "Multiplayer.ActiveTurnMidTurn";

    void EnableObservers()
    {
        this.AddObserver(OnMultiplayerMidTurn, MultiplayerActiveTurnMidTurn);
    }

    void DisableObservers()
    {
        this.RemoveObserver(OnMultiplayerMidTurn, MultiplayerActiveTurnMidTurn);
    }

    //receives a notification from PlayerManager after PM receives a notification from other telling what the results of the input were
    void OnMultiplayerMidTurn(object sender, object args)
    {
        //Debug.Log("receiving notification of a turn input 0");
        CombatMultiplayerTurn cmt = (CombatMultiplayerTurn)args; 
        if ( cmt != null)
        {
            //Debug.Log("receiving notification of a turn input 1 " + cmt.IsWait);
            //do some logic to change between the various states
            if (cmt.IsWait)
            {
                //wait doesn't need to wait for Other to be ready, just sends the RPC to other and other handles
                turn.endDir = DirectionsExtensions.IntToDirection(cmt.DirectionInt); 
                StartCoroutine(DoWait());
            }
            else if(cmt.IsAct)
            {
                //Debug.Log("cmt is " + cmt.TargetId + " " + cmt.TileX +" " + cmt.TileY);
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

                if ( cmt.SpellIndex != NameAll.NULL_INT)
                    turn.spellName = SpellManager.Instance.GetSpellNameByIndex(cmt.SpellIndex);
                if (cmt.SpellIndex2 != NameAll.NULL_INT)
                    turn.spellName2 = SpellManager.Instance.GetSpellNameByIndex(cmt.SpellIndex2);

                turn.endDir = DirectionsExtensions.IntToDirection(cmt.DirectionInt);

                //Debug.Log("something is fucked up about the targetting. unitId: " + turn.targetUnitId + " x,y " + turn.targetTile.pos.x + "," + turn.targetTile.pos.y + " " + turn.endDir.ToString() );

                StartCoroutine(DoAct());
            }
            else if (cmt.IsMove)
            {
                //Debug.Log("is move " + cmt.IsMove);
                //the way finding tiles works (tile.prev), master needs to know the range of the actor. call it here before it goes into CombatMoveSequenceState
                PlayerUnit actor = PlayerManager.Instance.GetPlayerUnit(cmt.ActorId);
                PlayerManager.Instance.GetPlayerUnitObjectComponent(cmt.ActorId).GetTilesInRange(board, board.GetTile(actor), actor);
                StartCoroutine(DoMove(cmt.TileX,cmt.TileY));
            }
        }
    }

    IEnumerator DoWait()
    {
        //Debug.Log("receiving notification of a turn input 3");
        owner.facingIndicator.gameObject.SetActive(true);
        owner.facingIndicator.SetDirection(turn.actor.Dir);
        PlayerManager.Instance.EndCombatTurn(turn);
        yield return new WaitForSeconds(0.5f);
        owner.facingIndicator.gameObject.SetActive(false);
        owner.ChangeState<PostActiveTurnState>();
    }

    IEnumerator DoMove(int moveX, int moveY)
    {
        //move: show selected tile (actual move done by RPC)
        Point cursorPos = new Point(moveX, moveY);
        SelectTile(cursorPos); //sets pos to this tile
        yield return new WaitForSeconds(0.5f);
        //turn.hasUnitMoved = true; //not needed, set to true in CombatMoveSeqenceState
        owner.ChangeState<CombatMoveSequenceState>();
    }

    IEnumerator DoAct()
    {
        if (turn.actor.Dir != turn.endDir)
            PlayerManager.Instance.SetPUODirectionMidTurn(turn.actor.TurnOrder, turn.endDir); //turns the unit

        yield return null; //waits for .5 seconds in CombatConfirmAbilityTargetState
        //turn.hasUnitActed = true; //not needed set to true in CombatPerformAbilityState

        owner.ChangeState<CombatConfirmAbilityTargetState>();
    }

    //obviously stupid way to do this. alternative is to inherit from a Photon.PunBehavour on another script and to the OnPlayerDisconnect
    private float timeSinceLastCalled;
    private float delay = 30.0f;

    void Update()
    {
        //if (!isOffline)
        //{
            timeSinceLastCalled += Time.deltaTime;
            if (timeSinceLastCalled > delay)
            {
                timeSinceLastCalled = 0f;
                int z1 = PlayerManager.Instance.GetMPNumberOfPlayers();
                if (z1 != 2)
                {
                    owner.battleMessageController.Display("" + z1 + " players remaining in Game! Other player has probably left.",3.0f);
                }
            }
        //}

    }


}
