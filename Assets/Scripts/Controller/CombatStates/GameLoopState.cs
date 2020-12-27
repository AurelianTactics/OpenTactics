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


    //obviously stupid way to do this. alternative is to inherit from a Photon.PunBehavour on another script and to the OnPlayerDisconnect
    private float timeSinceLastCalled;
    private float delay = 30.0f;

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
                
                    

                ExecuteGameLoop();
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

    

    public override void Exit()
    {
        doLoop = false;
        base.Exit();
        DisableObservers();
    }

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
            owner.ChangeState<StatusCheckState>(); //decrements all lasting statuses, if any expire update combat log and show them expiring
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

    public void ExecuteGameLoop()
    {
        if( loopPhase == Phases.StatusTick)
        {
            lastingPhase = Phases.StatusTick;
            loopPhase = Phases.SlowActionTick; //Debug.Log("doing statusTick decrement phase");
            owner.ChangeState<StatusCheckState>(); //decrements all lasting statuses, if any expire update combat log and show them expiring

            //StatusManager.Instance.StatusCheckPhase();//called in statuscheckstate decrements all ticks on statuses
        }
        else if( loopPhase == Phases.SlowActionTick)
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
            if( loopPhase == Phases.SlowAction)
            {
                if(SpellManager.Instance.GetNextSlowAction() != null)
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
            if( lastingPhase == Phases.CTIncrement || lastingPhase == Phases.ActiveTurn )
            {
                lastingPhase = Phases.ActiveTurn;
                if(CheckForActiveTurn()) 
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
                if( PlayerManager.Instance.QuickFlagCheckPhase() || turn.phaseStart != 0)
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
    const string MultiplayerGameLoop = "Multiplayer.GameLoop"; //called from PlayerManager for Other (p2)
    const string MultiplayerSpellSlow = "Multiplayer.SpellSlow";
    const string MultiplayerReaction = "Multiplayer.Reaction";//mime and reaction
    const string MultiplayerActiveTurnPreTurn = "Multiplayer.ActiveTurnPreTurn";//active turn start, show whose turn it is
    const string MultiplayerActiveTurnMidTurn = "Multiplayer.ActiveTurnMidTurn";//active turn mid-turn. Master raises it after other has told results of input to Master and vice versa
    const string MultiplayerCommandTurn = "Multiplayer.CommandTurn";//other gets to input an action
    const string MultiplayerMove = "Multiplayer.Move";//other gets the actual move that happens and starts the move
    const string MultiplayerDisableUnit = "Multiplayer.DisableUnit";//does the call to board to disable the unit
    const string MultiplayerTilePickUp = "Multiplayer.RemoveTilePickUp";
    const string MultiplayerGameOver = "Multiplayer.GameOver";
    const string MultiplayerMessageNotification = "Multiplayer.Message";

    void OnEnable()
    {
        this.AddObserver(OnStatusManagerNotification, DidStatusManager);
        this.AddObserver(OnQuitNotification, NameAll.NOTIFICATION_EXIT_GAME);
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
		this.RemoveObserver(OnRLResetGame, RLResetGame);
	}

    void EnableObservers()
    {
        if (!PlayerManager.Instance.IsOfflineGame() && !PlayerManager.Instance.isMPMasterClient())
        {
            this.AddObserver(OnMultiplayerPhaseNotification, MultiplayerGameLoop);
            this.AddObserver(OnMultiplayerSpellSlowNotification, MultiplayerSpellSlow);
            this.AddObserver(OnMultiplayerReactionNotification, MultiplayerReaction);
            this.AddObserver(OnMultiplayerActiveTurnPreTurnNotification, MultiplayerActiveTurnPreTurn);
            this.AddObserver(OnMultiplayerCommandTurnNotification, MultiplayerCommandTurn);
            this.AddObserver(OnMultiplayerTurnInputNotification, MultiplayerActiveTurnMidTurn);
            this.AddObserver(OnMultiplayerMoveNotification, MultiplayerMove);
            this.AddObserver(OnMultiplayerDisableUnitNotification, MultiplayerDisableUnit);
            this.AddObserver(OnMultiplayerRemoveTilePickUpNotification, MultiplayerTilePickUp);
            this.AddObserver(OnMultiplayerGameOver, MultiplayerGameOver);
            this.AddObserver(OnMultiplayerBattleMessageControllerNotification, MultiplayerMessageNotification);
        }
    }

    void DisableObservers()
    {
        if (!PlayerManager.Instance.IsOfflineGame() && !PlayerManager.Instance.isMPMasterClient())
        {
            this.RemoveObserver(OnMultiplayerPhaseNotification, MultiplayerGameLoop);
            this.RemoveObserver(OnMultiplayerSpellSlowNotification, MultiplayerSpellSlow);
            this.RemoveObserver(OnMultiplayerReactionNotification, MultiplayerReaction);
            this.RemoveObserver(OnMultiplayerActiveTurnPreTurnNotification, MultiplayerActiveTurnPreTurn);
            this.RemoveObserver(OnMultiplayerCommandTurnNotification, MultiplayerCommandTurn);
            this.RemoveObserver(OnMultiplayerTurnInputNotification, MultiplayerActiveTurnMidTurn);
            this.RemoveObserver(OnMultiplayerMoveNotification, MultiplayerMove);
            this.RemoveObserver(OnMultiplayerDisableUnitNotification, MultiplayerDisableUnit);
            this.RemoveObserver(OnMultiplayerRemoveTilePickUpNotification, MultiplayerTilePickUp);
            this.RemoveObserver(OnMultiplayerGameOver, MultiplayerGameOver);
            this.RemoveObserver(OnMultiplayerBattleMessageControllerNotification, MultiplayerMessageNotification);
        }
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
            CombatLogClass cll = new CombatLogClass("time runs to 0", pu.TurnOrder);
            cll.SendNotification();
            
            if (tempList[2] == 1)
            {//roll for crystal occurs in StatusManager
                //turn object holds the player shit
                board.SetTilePickUp(pu.TileX, pu.TileY, true, 1);
                CombatLogClass cll2 = new CombatLogClass("turns to crystal", pu.TurnOrder);
                cll2.SendNotification();
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

    void OnMultiplayerPhaseNotification(object sender, object args)
    {
        Phases phase = (Phases)args;
        if (phase == Phases.StatusTick)
        {
            //Master already knows not ready, don't update readiness to Master until coming back from this state
            //PlayerManager.Instance.SetMPSelfStatusAndPhase(false,phase);
            owner.ChangeState<StatusCheckState>(); //decrements all lasting statuses, if any expire update combat log and show them expiring
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

    IEnumerator UnhighlightSpellSlowCoroutine(PlayerUnit actor, List<Tile> targetTiles, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        HighlightActorTile(actor, false); //unhighlight
        if( targetTiles != null)
            board.DeSelectTiles(targetTiles); //deselect the tiles
    }

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
    void OnMultiplayerMoveNotification(object sender, object args)
    {
        CombatMultiplayerMove cmm = (CombatMultiplayerMove)args;
        if (cmm != null)
        {
            //Debug.Log("other has received the move notification");
            PlayerUnit actor = PlayerManager.Instance.GetPlayerUnit(cmm.ActorId);
            Tile targetTile = board.GetTile(cmm.TileX, cmm.TileY);

            if (cmm.IsKnockback)
            {
                PlayerManager.Instance.KnockbackPlayer(board, cmm.ActorId, targetTile);
            }
            else
            {
                PlayerManager.Instance.ConfirmMove(board, actor, targetTile, cmm.IsClassicClass, cmm.SwapUnitId);
            }
        }
    }

    //unit gets crunched
    void OnMultiplayerDisableUnitNotification(object sender, object args)
    {
        int unitId = (int)args;
        PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(unitId);
        board.DisableUnit(pu);
        CombatLogClass cll2 = new CombatLogClass("turns to crystal", pu.TurnOrder);
        cll2.SendNotification();
    }

    void OnMultiplayerRemoveTilePickUpNotification(object sender, object args)
    {
        List<int> tempList = args as List<int>;
        board.SetTilePickUp(tempList[0], tempList[1], false, tempList[2]);
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

	void OnRLResetGame(object sender, object args)
	{
		//Debug.Log("batte is over 5");
		owner.ChangeState<CombatStateInit>();
	}

	#endregion

}
