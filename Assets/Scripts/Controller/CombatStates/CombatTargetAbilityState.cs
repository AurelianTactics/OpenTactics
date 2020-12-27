using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CombatTargetAbilityState : CombatState
{
    List<Tile> tiles;
    CombatAbilityRange ar;
    const int ABILITY_ID_NULL = -19;
    bool directionTarget;
    Directions currentDir;
    readonly int SPELL_RANGE_LINE = 102;
    

    
    public override void Enter()
    {
        base.Enter();
        EnableObservers();

        SelectTile(turn.actor); //think this is needed as juggling between this and CombatConfirmAbilityTargetState can cause currentTile to shift and move around self target abilities
        ar = new CombatAbilityRange();
        
        actorPanel.SetActor(turn.actor);
        currentDir = turn.actor.Dir;
        turn.endDir = currentDir;
        SelectAbilityTiles(currentDir);

        directionTarget = GetDirectionTarget();
        if (driver == Drivers.Computer || StatusManager.Instance.IsAIControlledStatus(turn.actor.TurnOrder) || driver == Drivers.ReinforcementLearning)
            StartCoroutine(ComputerHighlightTarget());
        else
        {
            if(CalculationAT.IsAutoTargetAbility(turn.spellName))
            {
                StartCoroutine(MoveToConfirmState());
            }
            else if (directionTarget)
            {
                activeMenu.SetMenuTargetDirection(turn);
            }
        }

        //if (ar.directionOriented)
        //RefreshSecondaryStatPanel(pos);
        
    }

    

    bool GetDirectionTarget()
    {
        SpellName sn = turn.spellName;
        if (sn.CommandSet == NameAll.COMMAND_SET_MATH_SKILL && turn.spellName2 != null)
        {
            sn = turn.spellName2;
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

    public override void Exit()
    {
        DisableObservers();
        base.Exit();
        board.DeSelectTiles(tiles);
        activeMenu.Close();
        //statPanelController.HidePrimary();
        //statPanelController.HideSecondary();
    }

    protected override void OnMove(object sender, InfoEventArgs<Point> e)
    {
        //Debug.Log("testing onmove in combattargetabilitystate" + e.info + " " + pos.x + " " + pos.y);
        if(directionTarget)
        {
            //Debug.Log("testing onmove in combattargetabilitystate" + e.info + " " + pos.x + " " + pos.y);
            ChangeDirection(e.info);
        }
        else
        {
            //Debug.Log("b");
            SelectTile(e.info + pos);
            targetPanel.SetTargetPreview(board,pos);
        }

    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
        //Debug.Log("testing how many fires");
        if (e.info == 2)
        {
            if (directionTarget || tiles.Contains(board.GetTile(pos)))
            {
                owner.ChangeState<CombatConfirmAbilityTargetState>();
            }
                
        }
        else if( e.info == 1) //cancels
        {
            ChangeDirection(turn.actor.Dir); //in case actor has changed dir during this due to ability direciton state
            owner.ChangeState<CombatCommandSelectionState>();
            //owner.ChangeState<CategorySelectionState>();
        }
        else if( e.info == 0)
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
                        owner.ChangeState<CombatConfirmAbilityTargetState>();
                    else
                        targetPanel.SetTargetPreview(board,pos);
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
                            owner.ChangeState<CombatConfirmAbilityTargetState>();
                        else
                            targetPanel.SetTargetPreview(board, pos);
                    }
                }
                
            }
        }
    }

    void ChangeDirection(Point p)
    {
        Directions dir = p.GetDirection(); //Debug.Log("dir is " + dir.ToString());
        if (currentDir != dir)
        {
            PlayerManager.Instance.SetPUODirectionMidTurn(turn.actor.TurnOrder, dir,false);
            currentDir = dir;
            turn.endDir = currentDir; //used in CombatConfirmAbilityTargetState
            board.DeSelectTiles(tiles);
            SelectAbilityTiles(currentDir);
        }
    }

    void ChangeDirection(Directions dir)
    {
        //Debug.Log("dir is " + dir.ToString());
        if (currentDir != dir)
        {
            PlayerManager.Instance.SetPUODirectionMidTurn(turn.actor.TurnOrder, dir,false);
            currentDir = dir;
            turn.endDir = currentDir;
            board.DeSelectTiles(tiles);
            SelectAbilityTiles(currentDir);
        }
    }

    void SelectAbilityTiles(Directions dir)
    {
        SpellName sn = turn.spellName;
        if( sn.CommandSet == NameAll.COMMAND_SET_MATH_SKILL && turn.spellName2 != null)
        {
            sn = turn.spellName2;
        }
        //Debug.Log("am I here?");
        tiles = ar.GetTilesInRange(board,turn.actor,sn, dir);
        board.SelectTiles(tiles);

    }

    

    //done on auto target abilities
    IEnumerator MoveToConfirmState()
    {
        //Debug.Log("move to confirm state 1");
        yield return null;
        //Debug.Log("move to confirm state 2");
        owner.ChangeState<CombatConfirmAbilityTargetState>();
    }

    IEnumerator ComputerHighlightTarget()
    {
        if (ar.directionOriented)
        {
            ChangeDirection(turn.plan.attackDirection.GetNormal());
            yield return new WaitForSeconds(0.25f);
        }
        else
        {
            Point cursorPos = pos;
            while (cursorPos != turn.plan.fireLocation)
            {
                if (cursorPos.x < turn.plan.fireLocation.x) cursorPos.x++;
                if (cursorPos.x > turn.plan.fireLocation.x) cursorPos.x--;
                if (cursorPos.y < turn.plan.fireLocation.y) cursorPos.y++;
                if (cursorPos.y > turn.plan.fireLocation.y) cursorPos.y--;
                SelectTile(cursorPos);
                yield return new WaitForSeconds(0.25f);
            }
        }
        yield return new WaitForSeconds(0.5f);
        owner.ChangeState<CombatConfirmAbilityTargetState>();
    }

    #region notifications
    const string ActiveTurnWaitNotification = "ActiveTurnWaitNotification";

    public const string DidNorthClick = "ActiveTurn.NorthClick";
    public const string DidEastClick = "ActiveTurn.EastClick";
    public const string DidSouthClick = "ActiveTurn.SouthClick";
    public const string DidWestClick = "ActiveTurn.WestClick";

    void EnableObservers()
    {
        this.AddObserver(OnDirectionClick, ActiveTurnWaitNotification);
    }

    void DisableObservers()
    {
        this.RemoveObserver(OnDirectionClick, ActiveTurnWaitNotification);
    }

    void OnDirectionClick(object sender, object args)
    {
        string str = (string)args;
        Directions dir;
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
        owner.ChangeState<CombatConfirmAbilityTargetState>();
    }

    #endregion
}
