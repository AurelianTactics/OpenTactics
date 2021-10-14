using UnityEngine;
using System.Collections;

public class CombatMoveSequenceState : CombatState
{
    int moveEffect;
    bool isOffline;

    public override void Enter()
    {
        base.Enter();
        isOffline = PlayerManager.Instance.IsOfflineGame();
        turn.hasUnitMoved = true;
        moveEffect = 0;
        StartCoroutine("Sequence");
    }

    IEnumerator Sequence()
    {
        bool confirmMove = true;
        int swapUnitId = NameAll.NULL_UNIT_ID;
        Tile swapTile = null;
        bool isClassicClass = NameAll.IsClassicClass(turn.actor.ClassId);
        if ( isClassicClass)
        {
            if (turn.actor.AbilityMovementCode == NameAll.MOVEMENT_TELEPORT_1)
            {
                int rollChance = 100 + (turn.actor.StatTotalMove - MapTileManager.Instance.GetDistanceBetweenTiles(turn.actor.TileX, turn.actor.TileY, owner.currentTile.pos.x, owner.currentTile.pos.y)) * 10;
				int roll = PlayerManager.Instance.GetRollResult(rollChance, 1, 100, NameAll.COMBAT_LOG_TYPE_MOVE, NameAll.COMBAT_LOG_SUBTYPE_MOVE_TELEPORT_ROLL, null, turn.actor);
				if (roll > rollChance)
                {
                    confirmMove = false;
                    owner.battleMessageController.Display("Teleport fails!");
                    if (!isOffline)
                        PlayerManager.Instance.SendMPBattleMessage("Teleport fails!");
                }
            }
        }
        else
        {
            if( turn.actor.IsOnMoveEffect())
            {
                if (turn.actor.AbilityMovementCode == NameAll.MOVEMENT_UNSTABLE_TP)
                {
                    moveEffect = MapTileManager.Instance.GetDistanceBetweenTiles(turn.actor.TileX, turn.actor.TileY, owner.currentTile.pos.x, owner.currentTile.pos.y) - turn.actor.StatTotalMove;
                    if (moveEffect < 0) //if teleporting more than move, increase move effect
                        moveEffect = 0;

                    int z1 = Mathf.Abs(owner.currentTile.height - turn.actor.TileZ);
                    if (z1 > turn.actor.StatTotalJump)
                        moveEffect += z1 - turn.actor.StatTotalJump; //if height more than jump, increase move effect

                    moveEffect *= 10; //ability does % of HP dmg
                    //Debug.Log("is unstable TP moveEffect is " + moveEffect);
                }
                else if( turn.actor.AbilityMovementCode == NameAll.MOVEMENT_STRETCH_LEGS)
                {
                    if(MapTileManager.Instance.GetDistanceBetweenTiles(turn.actor.TileX, turn.actor.TileY, owner.currentTile.pos.x, owner.currentTile.pos.y) == turn.actor.StatTotalMove)
                    {
                        moveEffect = 1; //will increase jump by 1, passed
                    } 
                } 
            }
            else if( turn.actor.IsSpecialMoveRange())
            {
                if (turn.actor.AbilityMovementCode == NameAll.MOVEMENT_WINDS_OF_FATE)
                {
                    //if outside move and jump range, then decreasing chance to move there
                    int distance = MapTileManager.Instance.GetDistanceBetweenTiles(turn.actor.TileX, turn.actor.TileY, owner.currentTile.pos.x, owner.currentTile.pos.y) - turn.actor.StatTotalMove;
                    int height = Mathf.Abs(owner.currentTile.height - turn.actor.TileZ) - turn.actor.StatTotalJump;
                    if (distance > 0 || height > 0)
                    {
                        if (distance < 0)
                        {
                            distance = 0;
                        }
                        if (height < 0)
                        {
                            height = 0;
                        }
                        int rollMax = 100 - distance * 20 - height * 10; //10% loss per tile beyond max move range
                        if (rollMax > 0 && rollMax > UnityEngine.Random.Range(1, 101)) //!CalculationResolveAction.RollForSuccess(rollMax)
                        {
                            //roll succeeded, still move to that tile as normal
                            //Debug.Log("winds of fate succesful, move to that tile");
                        }
                        else
                        {
                            //Debug.Log("winds of fate: moving to random tile");
                            owner.battleMessageController.Display("Winds of Fate fails! Moving to random tile.");
                            if (!isOffline)
                                PlayerManager.Instance.SendMPBattleMessage("Winds of Fate fails! Moving to random tile.");

                            Point p = new Point(turn.actor.TileX, turn.actor.TileY);//if can't find a random one, uses this as default
                            SelectTile(board.GetRandomPoint(p)); //changes pos which changes owner.currentTile;
                        }
                    }
                }
                else if( turn.actor.AbilityMovementCode == NameAll.MOVEMENT_CRUNCH)
                {
                    Debug.Log("using crunch on dead unit");
                    int crunchUnitId = owner.currentTile.UnitId;
                    if( crunchUnitId != NameAll.NULL_UNIT_ID)
                    {
                        StatusManager.Instance.AddStatusAndOverrideOthers(NameAll.MOVEMENT_CRUNCH, crunchUnitId, NameAll.STATUS_ID_CRYSTAL);
                        PlayerManager.Instance.DisableUnit(crunchUnitId); //online: RPC call to other; also sends notification if other is calling this
                        board.DisableUnit(PlayerManager.Instance.GetPlayerUnit(crunchUnitId));
						if( PlayerManager.Instance.GetRenderMode() != NameAll.PP_RENDER_NONE)
						{
							CombatLogClass cll = new CombatLogClass("is hit by Crunch and is ground to dust...", crunchUnitId, PlayerManager.Instance.GetRenderMode());
							cll.SendNotification();
						}
                        
                    }
                }
                else if (turn.actor.AbilityMovementCode == NameAll.MOVEMENT_SWAP) //know who to swap right away but can't do it due to tile updating sequence til after actor moves
                {
                    Debug.Log("using swap on unit");
                    swapUnitId = owner.currentTile.UnitId;
                    swapTile = board.GetTile(turn.actor);
                }
            }
            
        }
        
        if(confirmMove)
        {
            //online: this lets Other know that a move is coming and to do the move
            if(!isOffline)
            {
                //Debug.Log("sending confirm move to other");
                //PlayerManager.Instance.ConfirmMove(board, turn.actor, owner.currentTile, isClassicClass, swapUnitId);
            }
                

            Tile actorStartTile = board.GetTile(turn.actor); //used in case of movement ability swap

            MapTileManager.Instance.MoveMarker(turn.actor.TurnOrder, owner.currentTile);
            PlayerUnitObject puo = PlayerManager.Instance.GetPlayerUnitObjectComponent(turn.actor.TurnOrder);
            puo.SetAnimation("moving", false);

            if (turn.actor.IsSpecialMoveRange())
            {
                if (isClassicClass)
                {
                    if (turn.actor.AbilityMovementCode == NameAll.MOVEMENT_FLY)
                        yield return StartCoroutine(puo.TraverseFly(owner.currentTile));
                    else if (turn.actor.AbilityMovementCode == NameAll.MOVEMENT_TELEPORT_1)
                        yield return StartCoroutine(puo.TraverseTeleport(owner.currentTile)); //Debug.Log("Decide on movement details and remove this yield return null");
                    else
                        yield return StartCoroutine(puo.Traverse(owner.currentTile));
                }
                else
                {
                    if (turn.actor.AbilityMovementCode == NameAll.MOVEMENT_UNSTABLE_TP
                        || turn.actor.AbilityMovementCode == NameAll.MOVEMENT_WINDS_OF_FATE)
                        yield return StartCoroutine(puo.TraverseTeleport(owner.currentTile));
                    else if (turn.actor.AbilityMovementCode == NameAll.MOVEMENT_LEAP)
                        yield return StartCoroutine(puo.TraverseFly(owner.currentTile));
                    else
                        yield return StartCoroutine(puo.Traverse(owner.currentTile));
                }
            }
            else
                yield return StartCoroutine(puo.Traverse(owner.currentTile));

            puo.SetAnimation("idle", true);
            PlayerManager.Instance.SetUnitTile(board, turn.actor.TurnOrder, owner.currentTile, isAddCombatLog:true);

            yield return new WaitForFixedUpdate(); //not sure if needed but want at least a slight delay for the swap
            if (swapUnitId != NameAll.NULL_UNIT_ID)
                PlayerManager.Instance.SetUnitTileSwap(board, swapUnitId, actorStartTile);
   
        }
        
        
        DoTilePickUp(owner.renderMode); //Debug.Log("calling coroutine move effect is " + moveEffect);
        StartCoroutine(OnMoveEffect()); //Phase change happens in here after the result of the OnMoveEffect inner function, could move out here and do yield return StartCoroutine()

        //yield return new WaitForFixedUpdate();//in case confirmMove fails, need to wait a frame before changing the state
        //owner.ChangeState<CombatCommandSelectionState>();

    }

    void ChangeState()
    {
        owner.ChangeState<CombatCommandSelectionState>();
    }

    void DoTilePickUp(int renderMode)
    {
        Tile t = board.GetTile(turn.actor);
        if (t.PickUpId == 1) //crystal
        {
            if (!isOffline)
                PlayerManager.Instance.SendMPRemoveTilePickUp( t.pos.x, t.pos.y, false, t.PickUpId);

            board.SetTilePickUp(turn.actor.TileX,turn.actor.TileY, false, renderMode, t.PickUpId);
            //fully restores HP and MP
            PlayerManager.Instance.AlterUnitStat(NameAll.REMOVE_STAT_HEAL, turn.actor.StatTotalMaxLife, NameAll.STAT_TYPE_HP, turn.actor.TurnOrder, 
				combatLogSubType:NameAll.COMBAT_LOG_SUBTYPE_CRYSTAL_PICK_UP);
            PlayerManager.Instance.AlterUnitStat(NameAll.REMOVE_STAT_HEAL, turn.actor.StatTotalMaxMP, NameAll.STAT_TYPE_MP, turn.actor.TurnOrder,
				combatLogSubType: NameAll.COMBAT_LOG_SUBTYPE_CRYSTAL_PICK_UP);
        }
    }

    IEnumerator OnMoveEffect()
    {
        if( turn.actor.IsOnMoveEffect())
        {
            //Debug.Log("just prior to CalcResovleAction move effect is " + moveEffect);
            CalculationResolveAction.CreateSpellReactionMove( turn.actor.ClassId, turn.actor.AbilityMovementCode, turn.actor.TurnOrder, moveEffect);
			PlayerManager.Instance.AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_MOVE, NameAll.COMBAT_LOG_SUBTYPE_MOVE_EFFECT, cTurn: turn, effectValue: turn.actor.AbilityMovementCode);
			yield return null;
            StartCoroutine(DoPhase());
        }
        else
        {
            yield return null;
            ChangeState();
        }
    }

    IEnumerator DoPhase()
    {
        //should always be the movement that just happend, if there is an issue with this creath a new spellReaction queue for movements
        SpellReaction sr = SpellManager.Instance.GetNextSpellReaction();
        if (sr != null)
        {
            yield return new WaitForSeconds(0.1f); //not sure if necessary

            CombatTurn ssTurn = new CombatTurn(sr, board, isMimeSpellReaction: false);

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
                SpellManager.Instance.SendReactionDetails(sr.ActorId, sr.SpellIndex, targetX, targetY, "Movement: " + ssTurn.spellName.AbilityName);
            }

            SpellName sn = SpellManager.Instance.GetSpellNameByIndex(sr.SpellIndex); //Debug.Log("about to do movement: " + sn.AbilityName);            
            owner.battleMessageController.Display("Movement: " + ssTurn.spellName.AbilityName);
            yield return new WaitForSeconds(0.5f);

            //modify the spellName for reaction
            ssTurn.spellName = CalculationAT.ModifyReactionSpellName(sr, ssTurn.actor.TurnOrder);

			owner.calcMono.DoFastAction(board, ssTurn, isActiveTurn: false, isReaction: true, isMime: false, renderMode: owner.renderMode);
			//yield return StartCoroutine(owner.calcMono.DoFastActionInner(board, ssTurn, isActiveTurn: false, isSlowActionPhase: false, isReaction: true, isMime: false));
			//owner.calcMono.DoFastAction(board, ssTurn, isActiveTurn: false, isSlowAction: false, isReaction: true, isMime: false);
			SpellManager.Instance.RemoveSpellReactionByObject(sr); //disables player reaction flag in here
        }
        ChangeState();
    }
}