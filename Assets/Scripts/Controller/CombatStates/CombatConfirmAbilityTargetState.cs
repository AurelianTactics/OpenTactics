using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatConfirmAbilityTargetState : CombatState
{
    List<Tile> tiles;
    CombatAbilityArea aa;
    int index;
    bool isOffline;
    bool isMasterClient;

    public override void Enter()
    {
        base.Enter();

        index = 0;
        isOffline = PlayerManager.Instance.IsOfflineGame();
        isMasterClient = PlayerManager.Instance.isMPMasterClient();
        EnableObservers();
        //Debug.Log("entering confirmabilitytargetState");
        
        aa = new CombatAbilityArea();
        tiles = aa.GetTilesInArea(board, turn.actor, turn.spellName,currentTile,turn.endDir); 
        turn.targetTile = currentTile; 
        board.SelectTiles(tiles); 
        FindTargets(); 
        //RefreshPrimaryStatPanel(turn.actor.tile.pos);
        //Debug.Log("Decide on ability preview targetting");
        if (turn.targets.Count > 0)
        {
            if (driver == Drivers.Human)
                UpdateHitSuccessIndicator();//hitSuccessIndicator.Show();
            SetTarget(0);
   
        }
        else
        {
            SetHitPreview(currentTile);
        
        }
        
        if ( !isOffline)
        {
            if( isMasterClient && turn.actor.TeamId == NameAll.TEAM_ID_RED)
            {
                //master client and displaying and doing other team's input
                turn.targetTile = currentTile; //have to do this because the targetTile can change based on what is being shown in the preview box 
                StartCoroutine(GoToCombatPerformAbilityState());
                return;
            }

        }

        if (driver == Drivers.Computer || StatusManager.Instance.IsAIControlledStatus(turn.actor.TurnOrder) || driver == Drivers.ReinforcementLearning)
            StartCoroutine(ComputerDisplayAbilitySelection());
        else
        {
            turn.targetUnitId = NameAll.NULL_UNIT_ID; //needs to be reset as juggling between menus can cause an old version to be saved and weird targetting issues
            SetActiveTurnButtons();
        }
            
    }

    public override void Exit()
    {
        DisableObservers();
        board.DeSelectTiles(tiles);
        activeMenu.Close();
        previewPanel.Close();
        targetPanel.Close();
        base.Exit();
        //statPanelController.HidePrimary();
        //statPanelController.HideSecondary();
        //hitSuccessIndicator.Hide();
    }

    protected override void OnMove(object sender, InfoEventArgs<Point> e)
    {
        if (e.info.y > 0 || e.info.x > 0)
            SetTarget(index + 1);
        else
            SetTarget(index - 1);
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
        //Debug.Log("fired?");
        if (e.info == 2)
        {
            //Debug.Log("testing confirm on fire in confirm ability target state");
            //defaults to targetting unit if over a unit, map if not
            //if( currentTile.UnitId != NameAll.NULL_UNIT_ID && turn.targets.Contains(currentTile))
            //{
            //    turn.targetUnitId = currentTile.UnitId; Debug.Log("testing confirm on fire in confirm ability target state");
            //    turn.targetTile = currentTile;
            //}

            turn.targetUnitId = NameAll.NULL_UNIT_ID;
            turn.targetTile = currentTile; //have to do this because the targetTile can change based on what is being shown in the preview box 
            ConfirmAbility(); 
            //UIActiveTurnMenu can't handle the fire because it's reading fires from previous states and confirms

        }
        else if( e.info == 1) //cancel
        {
            if( CalculationAT.IsAutoTargetAbility(turn.spellName))
            {
                owner.ChangeState<CombatAbilitySelectState>();
            }
            else
            {
                Cancel();
            }
            
        }
        else if( e.info == 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                Tile t = hitObject.GetComponent<Tile>();
                if(turn.targets.Contains(t))
                {
                    SelectTile(t.pos);
                    SetTarget(t);
                }
            }
        }
            
    }

    void FindTargets()
    {
        turn.targets = new List<Tile>();
        for (int i = 0; i < tiles.Count; ++i)
        {
            if(tiles[i].UnitId != NameAll.NULL_UNIT_ID)
            {
                turn.targets.Add(tiles[i]); //check for casterimmune and enemies/allies occurs in combatabilityarea
            }
            
        }
    }

    void SetTarget(int target)
    {
        index = target;
        if (index < 0)
            index = turn.targets.Count - 1;
        if (index >= turn.targets.Count)
            index = 0;

        if (turn.targets.Count > 0)
        {
            
            targetPanel.SetTargetPreview(turn.targets[index]);
            UpdateHitSuccessIndicator();
            turn.targetTile = turn.targets[index];
        }
        else
        {
            targetPanel.SetTargetPreview(currentTile);
            turn.targetTile = currentTile;
        }
    }

    void SetTarget(Tile t)
    {
        for (int i = 0; i < turn.targets.Count; i++)
        {
            if (turn.targets[i] == t)
            {
                index = i;
                targetPanel.SetTargetPreview(turn.targets[index]);
                UpdateHitSuccessIndicator();
                turn.targetTile = t;
                break;
            }
        }
    }

    //called when nothing on tile
    void SetHitPreview(Tile targetTile)
    {
        List<string> strList = CalculationAT.GetHitPreview(board, turn, targetTile);
        previewPanel.SetHitPreview(strList[0], strList[1], strList[2], strList[3], strList[4]);
    }

    void UpdateHitSuccessIndicator()
    {
        Tile targetTile = turn.targets[index];
        SetHitPreview(targetTile);
        //Debug.Log("asdf 1");
    }

    //calls the buttons to be displayed (ie back/confirm or targetUnit/Map/confirm
    void SetActiveTurnButtons()
    {
        if (currentTile.UnitId != NameAll.NULL_UNIT_ID && SpellManager.Instance.IsTargetUnitMap(turn.spellName, currentTile.UnitId))
        { 
            activeMenu.SetMenuAbilityConfirm(turn, true);
            //Debug.Log("can target unit");
        }
        else
        {
            activeMenu.SetMenuAbilityConfirm(turn, false);
            //Debug.Log("cannot target unit");
        }
            
    }

    IEnumerator ComputerDisplayAbilitySelection()
    {
        //owner.battleMessageController.Display(turn.spellName.AbilityName); //doing this in performabilitystate
        yield return new WaitForSeconds(0.5f);
        ConfirmAbility();
    }

    //online: master waits here for a bit to see what ability AoE and hit rate is
    IEnumerator GoToCombatPerformAbilityState()
    {
        yield return new WaitForSeconds(0.5f);
        owner.ChangeState<CombatPerformAbilityState>();
    }

    #region Notifications
    const string ActiveTurnConfirmNotification = "ActiveTurnConfirmNotification";
    const string ActiveTurnTargetUnitMapNotification = "ActiveTurnTargetUnitMapNotification";
    const string ActiveTurnBackNotification = "ActiveTurnBackNotification";
    public const string DidConfirmClick = "ActiveTurn.ConfirmClick";
    public const string DidBackClick = "ActiveTurn.BackClick";
    public const string DidTargetUnitClick = "ActiveTurn.TargetUnitClick";
    public const string DidTargetMapClick = "ActiveTurn.TargetMapClick";

    void EnableObservers()
    {
        this.AddObserver(OnActiveTurnClick, ActiveTurnConfirmNotification);
        this.AddObserver(OnActiveTurnClick, ActiveTurnBackNotification);
        this.AddObserver(OnActiveTurnClick, ActiveTurnTargetUnitMapNotification);
    }

    void DisableObservers()
    {
        this.RemoveObserver(OnActiveTurnClick, ActiveTurnConfirmNotification);
        this.RemoveObserver(OnActiveTurnClick, ActiveTurnBackNotification);
        this.RemoveObserver(OnActiveTurnClick, ActiveTurnTargetUnitMapNotification);
    }

    void OnActiveTurnClick(object sender, object args)
    {
        string str = (string)args;
        if(args.Equals(DidBackClick))
        {
            //owner.ChangeState<CombatTargetAbilityState>();
            if (CalculationAT.IsAutoTargetAbility(turn.spellName))
            {
                owner.ChangeState<CombatAbilitySelectState>(); //don't need cancel as it's skipping back two states
            }
            else
            {
                Cancel();
            }
        }
        else if (args.Equals(DidConfirmClick))
        {
            //Debug.Log("changing state to perform");
            turn.targetUnitId = NameAll.NULL_UNIT_ID;
            turn.targetTile = currentTile; //have to do this because the targetTile can change based on what is being shown in the preview box 
            ConfirmAbility();
        }
        else if (args.Equals(DidTargetUnitClick))
        {
            turn.targetUnitId = turn.targetTile.UnitId;
            turn.targetTile = currentTile;
            ConfirmAbility();
        }
        else if (args.Equals(DidTargetMapClick))
        {
            turn.targetUnitId = NameAll.NULL_UNIT_ID;
            turn.targetTile = currentTile;
            ConfirmAbility();
        }
    }

    void ConfirmAbility()
    {
        if (!isOffline)
        {
            
            //online game: master send info to other so other can see what master is about to do OR other (p2) sends move command to master
            //some of these can be null
            int zSpellIndex;
            int zSpellIndex2;
            if (turn.spellName == null)
                zSpellIndex = NameAll.NULL_INT;
            else
                zSpellIndex = turn.spellName.Index;

            if (turn.spellName2 == null)
                zSpellIndex2 = NameAll.NULL_INT;
            else
                zSpellIndex2 = turn.spellName2.Index;

            
            PlayerManager.Instance.SendMPActiveTurnInput(isMove: false, isAct: true, isWait: false, actorId: turn.actor.TurnOrder, tileX: turn.targetTile.pos.x, tileY: turn.targetTile.pos.y,
                    directionInt: turn.endDir.DirectionToInt(), targetId: turn.targetUnitId, spellIndex: zSpellIndex, spellIndex2: zSpellIndex2);

            if (!isMasterClient)
            {
               
                turn.hasUnitActed = true; //not sure if necessary since this gets updated by master when control passed back here
                owner.ChangeState<GameLoopState>(); //returns to gameloopstate, master receives input and updates both games, then sees who has the next turn
            }
            else
            {
                //in online games, master has to send this so other turns its unit (master is already turned)
                if (!isOffline && turn.actor.Dir != turn.endDir)
                    PlayerManager.Instance.SetMPPUODirectionMidTurn(turn.actor.TurnOrder, turn.endDir);
                owner.ChangeState<CombatPerformAbilityState>();
            }
                
        }
        else
            owner.ChangeState<CombatPerformAbilityState>();
            
    }

    //on canceling a direction attack, need to undo the turning
    void ChangeDirection(Directions dir)
    {
        //Debug.Log("dir is " + dir.ToString());
        if (turn.endDir != dir)
        {
            PlayerManager.Instance.SetPUODirectionMidTurn(turn.actor.TurnOrder, dir, false);
            turn.endDir = dir;
        }
    }

    void Cancel()
    {
        ChangeDirection(turn.actor.Dir); //turns the actor back to correct position if necessary
        owner.ChangeState<CombatTargetAbilityState>();
    }
    #endregion
}
