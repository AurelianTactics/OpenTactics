using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//called in various class like:
//CombatPerformAbilityState
//SlowActionState,MimeState,ReactionState
//launches an action or creates a slow action

public class CalculationMono : MonoBehaviour {

    //convenience functions called in other parts, inherits form MonoBehaviour to take advantage of
       //breaking up the active turn
        //path 1 slow action
            //slow action added to queue
            //unit gets charging status
            //precedes to end turn
        //path 2 fast action
            //goes to resolve action
            //passes checks
                //inventory check
                //mp check
                //jumping check
                //silence check (animation on actor)
            //unit turns, does animation, maybe launches particle
            //pre results
                //mp removed
                //mimeQueue added
            //results (iterates over targets)
                //reflect check (animation on target)
                //effect on target 
                //chance to counter
                //animations on target
                //statuses on target
            //possible second swing
                //repates the results section

	public void DoFastAction(Board board, CombatTurn turn, bool isActiveTurn, bool isReaction, bool isMime, int renderMode, bool isWAMode=false)
    {
        //Debug.Log("reached fast action inner here");
        if( isActiveTurn && SlowActionCheck(turn) )
        {
            //Debug.Log("something is fucked up about the targetting. unitId: " + turn.targetUnitId + " x,y " + turn.targetTile.pos.x + "," + turn.targetTile.pos.y);
            //slow action, adds it to the queue, apply ability is done, moves onto proper state
            SpellManager.Instance.CreateSpellSlow(turn, isWAMode); 
            if (turn.spellName.CommandSet == NameAll.COMMAND_SET_CHARGE)
            {
                turn.hasUnitMoved = true;
            }
            else if (turn.spellName.CommandSet == NameAll.COMMAND_SET_JUMP)
            {
                turn.hasUnitMoved = true;
            }
            //Debug.Log("going through do fast action inner");
        }
        else
        {
            //non slow action
            //get all the relevant data first, then passed on that set the proper order
            //Debug.Log("reached here, non active turn && slow action");
            bool isTwoSwordsEligible = PlayerManager.Instance.IsEligibleForTwoSwords(turn.actor.TurnOrder, turn.spellName.CommandSet);
            bool isPassesChecks = CalculationResolveAction.FastActionChecks(turn, true); //suppress messages for the first time
            if (isPassesChecks)
            {
                //decrement from inventory/chance of breaking
                //can't select ability if not enough items
                CalculationResolveAction.CheckInventory(turn.spellName, turn.actor.TeamId, turn.actor.TurnOrder, renderMode);

                ApplyPreresults(board,turn,isReaction,isMime);//mimequeue and subtract MP

				if( renderMode != NameAll.PP_RENDER_NONE)
				{
					//do animation (turn, animation)
					//may want to rearrange this due to double swings
					PlayerManager.Instance.SetFacingDirectionAttack(board, turn.actor, turn.targetTile); //Debug.Log("reached here");
					PlayerManager.Instance.SetPlayerObjectAnimation(turn.actor.TurnOrder, "attacking", false);
					PlayerManager.Instance.SendPlayerObjectAnimation(turn.actor.TurnOrder, "attacking", false); //in online games, sends out the RPC
					//yield return new WaitForSeconds(0.3f);
					//PlayerManager.Instance.SetPlayerObjectAnimation(turn.actor.TurnOrder, "setIdle",true);
					//does the checks, if any check fails proper message is shown
					//no particles for now
					//ParticleManager.Instance.LaunchCastParticle(turn.actor, turn.targetTile, turn.spellName);
				}
                
                //roll for on attack effect (if applicable)
                bool isOnAttackEffect = IsOnAttackEffect(turn.actor, turn.spellName);

                //results (iterates over targets)
                //reflect check (animation on target)
                //effect on target 
                //chance to counter (does not show the counter, simply adds it to the counter queue)
                //animations on target (just launches it, not worked about sync)
                //statuses on target (just launches it, not worried about sync)
                CalculationResolveAction.FastActionResults(board, turn, isReaction:isReaction, isFirstSwing:true, isForceFacing:false, 
                    isAllowBladeGrasp:true, isOnAttackEffect:isOnAttackEffect, renderMode:renderMode);
                if (isOnAttackEffect) //chance that a spell can be cast on an effect
                {
                    SpellName snAttackEffect = CheckForOnAttackEffect(turn.actor, turn.spellName);
                    if (snAttackEffect != null)
                    {
                        CalculationResolveAction.FastActionAttackEffect(board, turn, isReaction, isForceFacing:false, isAllowBladeGrasp:false, isOnAttackEffect:false, renderMode: renderMode);
                    }
                }
                //Debug.Log("in calc mono, is two swords eligible: " + isTwoSwordsEligible);
                if (isTwoSwordsEligible)
                {
					if (renderMode != NameAll.PP_RENDER_NONE)
					{
						StartCoroutine(TwoSwordsWait(turn));
					}
                    //does the second swing, may need some data from the first swing
                    bool isForceFacing = false; bool isAllowBladeGrasp = false;
                    isOnAttackEffect = IsOnAttackEffect(turn.actor, turn.spellName);
                    CalculationResolveAction.FastActionSecondSwing(board, turn, isReaction, isForceFacing, isAllowBladeGrasp, isOnAttackEffect, renderMode);
                    if (isOnAttackEffect)
                    {
                        SpellName snAttackEffect = CheckForOnAttackEffect(turn.actor, turn.spellName);
                        if (snAttackEffect != null)
                        {
                            CalculationResolveAction.FastActionAttackEffect(board, turn, isReaction, isForceFacing: false, isAllowBladeGrasp: false, isOnAttackEffect: false, renderMode: renderMode);
                        }
                    }
                }

				//if it's a reaction, set the actor back to how it was facing before
				if (renderMode != NameAll.PP_RENDER_NONE)
					ReactionMimeCheck(turn, isReaction, isMime);
            }
            else
            {
                CalculationResolveAction.FastActionChecks(turn, false);
            }
        }
		if(renderMode != NameAll.PP_RENDER_NONE)
			StartCoroutine(CameraWait()); //just a visual thing so the camera doesn't change too quickly

		//Debug.Log("finished calc mono, DoFastAction");
	}

	IEnumerator TwoSwordsWait(CombatTurn turn)
	{
		yield return new WaitForSeconds(1.5f);
		PlayerManager.Instance.SetPlayerObjectAnimation(turn.actor.TurnOrder, "attacking", false); //Debug.Log("in calc mono, is two swords eligible: " + isTwoSwordsEligible);
		PlayerManager.Instance.SendPlayerObjectAnimation(turn.actor.TurnOrder, "attacking", false); //in online games, sends out the RPC
	}

	IEnumerator CameraWait()
	{
		yield return new WaitForSeconds(0.3f); 
	}

	IEnumerator ReactionMimeCheck(CombatTurn turn, bool isReaction, bool isMime)
	{
		if (isReaction || isMime) //mime never turns permanently and animated shouldn't turn at all but after the turn sometimes ends up facing in a wonky direction
		{
			yield return new WaitForSeconds(1.0f); //not sure on this timing
			if (!StatusManager.Instance.IfStatusByUnitAndId(turn.actor.TurnOrder, NameAll.STATUS_ID_DEAD))
			//&& turn.actor.Dir != PlayerManager.Instance.GetPlayerUnitObjectComponent(turn.actor.TurnOrder).di)
			{
				PlayerManager.Instance.SetPUODirectionMidTurn(turn.actor.TurnOrder, turn.actor.Dir);
			}
		}
	}


	bool SlowActionCheck(CombatTurn turn)
    {
        //Debug.Log("Doing slow action check " + CalculationAT.CalculateCTR(turn.actor, turn.spellName));
        //if ( turn.actor == null)
        //{
        //    //Debug.Log("actor is null");
        //}
        //if( turn.spellName == null)
        //{
        //    //Debug.Log("sn is null");
        //}
        int ctr = CalculationAT.CalculateCTR(turn.actor, turn.spellName);
        if (ctr == 0)
        {
            //Debug.Log("not a slow action");
            return false;
        }
        return true;
    }

    void ApplyPreresults(Board board, CombatTurn turn, bool isReaction, bool isMime)
    {
        //subtract MP
        if( !isMime)
        {
            CalculationResolveAction.SubtractMP(turn.actor, turn.spellName);
        }
        
        //add to mime queue
        if( !isMime && !isReaction)
        {
            CalculationResolveAction.AddToMimeQueue(board, turn.actor, turn.spellName, turn.targetTile);
        }
    }

    bool IsOnAttackEffect(PlayerUnit actor, SpellName sn)
    {
        if (sn.CommandSet == NameAll.COMMAND_SET_ATTACK_AURELIAN || sn.CommandSet == NameAll.COMMAND_SET_ATTACK)
        {
            ItemObject io = ItemManager.Instance.GetItemObjectById(actor.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON);
            if (io.OnHitChance > 0 )
            {
				if( PlayerManager.Instance.IsRollSuccess(io.OnHitChance,1,100,NameAll.NULL_INT, NameAll.COMBAT_LOG_SUBTYPE_ITEM_ON_HIT_CHANCE, sn, actor, null, io.OnHitEffect))
					return true;
            }
        }
        return false;
    }

    SpellName CheckForOnAttackEffect(PlayerUnit actor, SpellName sn)
    {
        //Debug.Log("testing weapon add effect, set hit chance for 100 so change that back");
        ItemObject io = ItemManager.Instance.GetItemObjectById(actor.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON);
        //check if spell type or not, spell types are over 1000 (formerly spell types are coded in by starting with 0_
        if (io.OnHitEffect > 1000) //.Length > 2 && io.OnHitEffect.Substring(0, 1).Equals("0") )
        {
            int newSpellIndex = io.OnHitEffect - 1000;
            SpellName newSpellName = SpellManager.Instance.GetSpellNameByIndex(newSpellIndex);
            return newSpellName;
        }
        else
        {
            return null;
        }
    }

    //slow action pre slow action shit
    public Tile GetCameraTargetTile(Board board, SpellSlow ss)
    {
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

        return targetTile;
    }


	#region init and reset level

	public void ResetLevel(Board board, List<PlayerUnit> greenList, List<PlayerUnit> redList, GameObject markerTeam1Prefab,
		GameObject markerTeam2Prefab, bool isOffline, bool isRLDriver, bool isSetDriver, int renderMode)
	{
		//reset the board
		board.ResetBoard(renderMode);
		if(renderMode != NameAll.PP_RENDER_NONE)
		{
			MapTileManager.Instance.ClearNewBoard();
			GameObject playerHolder = GameObject.Find("PlayerHolder"); //rather than find could store these game objects in CombatController and destroy by accessing that
			GameObject markerHolder = GameObject.Find("MarkerHolder");
			if(playerHolder != null)
				Destroy(playerHolder);//destroy game objects, reset with new units
			if(markerHolder != null)
				Destroy(markerHolder);//destroy game objects, reset with new marker holders
		}

		PlayerManager.Instance.ClearForReset();
		StatusManager.Instance.ClearLists(); //clear static lists

		//spawn units
		SpawnUnits(board, greenList, redList, markerTeam1Prefab, markerTeam2Prefab, isOffline, isRLDriver, isSetDriver, renderMode);
	}

	//called from combatstateinit to set up level or gameloopstate to reset the level
	public void SpawnUnits(Board board, List<PlayerUnit> green, List<PlayerUnit> red,  GameObject markerTeam1Prefab,
		GameObject markerTeam2Prefab, bool isOffline, bool isRLDriver, bool isSetDriver, int renderMode)
	{

		List<PlayerUnit> temp = new List<PlayerUnit>();
		PlayerUnit pu;
		PlayerManager.Instance.SetLocalTeamId(NameAll.TEAM_ID_GREEN);

		int zBreak = 0; //temp
		int turn = NameAll.TEAM_ID_GREEN; //green goes first, then snake draft
		int unitsToAdd = 1; //first time only adds one unit, then 2 (snake draft)
		int currentIndex = 0;
		int greenIndex = 0;
		int redIndex = 0;
		int greenCount = green.Count;
		int redCount = red.Count;


		//set up turn order for PlayerUnits
		while (greenCount > 0 || redCount > 0)
		{
			if (turn == NameAll.TEAM_ID_GREEN)
			{
				turn = NameAll.TEAM_ID_RED;
				if (greenCount > 0)
				{
					while (unitsToAdd > 0 && greenCount > 0)
					{
						//for some modes need to only let player cast learned abilities. abilities loaded but have to modify the uniqueId with the turnOrder
						//so change the oldTurnOrder with the newly assigned turnOrder (the old turn order is a place holder set when the spells are added to the spellmanager)
						if (SpellManager.Instance.spellLearnedType == NameAll.SPELL_LEARNED_TYPE_PLAYER_1)
							SpellManager.Instance.AlterSpellLearnedList(currentIndex, green[greenIndex].TurnOrder);

						unitsToAdd -= 1;
						pu = new PlayerUnit(green[greenIndex]);
						greenIndex += 1;
						greenCount -= 1;
						pu.TurnOrder = currentIndex; // SetTurn_order(currentIndex);
						pu.TeamId = NameAll.TEAM_ID_GREEN; // SetTeam_id(NameAll.TEAM_ID_GREEN);
						temp.Add(pu);
						temp[currentIndex] = pu; //redundant
						currentIndex += 1;
					}
				}
				else
				{
					continue;
				}
			}
			else
			{
				turn = NameAll.TEAM_ID_GREEN;
				if (redCount > 0)
				{
					while (unitsToAdd > 0 && redCount > 0)
					{
						unitsToAdd -= 1;
						pu = new PlayerUnit(red[redIndex]);
						redCount -= 1;
						redIndex += 1;
						pu.TurnOrder = currentIndex;// SetTurn_order(currentIndex);
						pu.TeamId = NameAll.TEAM_ID_RED;// SetTeam_id(NameAll.TEAM_ID_RED);
						temp.Add(pu);
						temp[currentIndex] = pu;
						currentIndex += 1;
					}
				}
				else
				{
					continue;
				}
			}
			unitsToAdd = 2;
			zBreak += 1;
			if (zBreak > 100)
			{
				break;
			}
			//Debug.Log("in while loop " + greenCount + " " + redCount);
		}

		foreach (PlayerUnit playerUnit in temp)
		{
			PlayerManager.Instance.AddPlayerUnit(playerUnit);
			PlayerManager.Instance.AssignTeamIdToPlayerUnit(playerUnit.TurnOrder);
			if (isSetDriver)
				playerUnit.SetDriver(SetDrivers(playerUnit.TeamId, isOffline, isRLDriver));
		}

		if (renderMode != NameAll.PP_RENDER_NONE)
		{
			//physically place the units on the map
			RenderUnits(board, temp, board.spawnList, markerTeam1Prefab, markerTeam2Prefab);
		}
		else
		{
			PlacePU(board, temp);	
		}

		//sets the teams alliances
		PlayerManager.Instance.AssignAlliances();
		//sets the mime shit
		PlayerManager.Instance.SetMimeOnTeam();
		//adds the lasting statuses
		PlayerManager.Instance.SetLastingStatuses();
		//sets the twosword eligible flag, onmoveeffect flag
		PlayerManager.Instance.InitializePlayerUnits();
		//Debug.Log("" + MapTileMan);
	}

	void PlacePU(Board board, List<PlayerUnit> puList)
	{
		List<SerializableVector3> tempSpawnList = new List<SerializableVector3>(board.spawnList);
		//place units based on spawn points
		foreach (PlayerUnit playerUnit in puList)
		{
			foreach (Vector3 vec in tempSpawnList.ToList())
			{
				if (playerUnit.TeamId == (int)vec.z)
				{
					Point p = new Point((int)vec.x, (int)vec.y);
					Tile startTile = board.GetTile(p);
					playerUnit.SetUnitTile(startTile, true);
					//tells the tiles that someone is on them
					board.GetTile(playerUnit).UnitId = playerUnit.TurnOrder;
					tempSpawnList.Remove(vec);
					break;
				}
			}
			PlayerManager.Instance.SetInitialFacingDirection(playerUnit.TurnOrder);
		}
	}

	//physically place the units on the map
	//spList is x, y, and teamId
	public void RenderUnits(Board board, List<PlayerUnit> puList, List<SerializableVector3> spawnList, GameObject markerTeam1Prefab,
		GameObject markerTeam2Prefab)
	{
		GameObject playerUnitObject;
		Transform playerHolder = new GameObject("PlayerHolder").transform;
		Transform markerHolder = new GameObject("MarkerHolder").transform;

		foreach (PlayerUnit playerUnit in puList)
		{
			GameObject marker;
			if (playerUnit.TeamId == NameAll.TEAM_ID_GREEN)
			{
				marker = Instantiate(markerTeam1Prefab) as GameObject;
			}
			else
			{
				marker = Instantiate(markerTeam2Prefab) as GameObject;
			}
			MapTileManager.Instance.AddMarker(marker); //Debug.Log("adding marker to list...");
			marker.transform.SetParent(markerHolder);

			string puoString = NameAll.GetPUOString(playerUnit.ClassId); //Debug.Log("trying to instantiate puo object" + puoString);
			playerUnitObject = Instantiate(Resources.Load(puoString)) as GameObject; //Debug.Log("is puo active" + playerUnitObject.GetActive());

			foreach (Vector3 vec in spawnList.ToList())
			{
				if (playerUnit.TeamId == (int)vec.z)
				{
					Point p = new Point((int)vec.x, (int)vec.y);
					Tile startTile = board.GetTile(p);
					playerUnit.SetUnitTile(startTile, true);
					//tells the tiles that someone is on them
					board.GetTile(playerUnit).UnitId = playerUnit.TurnOrder;

					spawnList.Remove(vec);
					Vector3 vecTemp = startTile.transform.position;
					vecTemp.y = 0.7f; //adjust starting height of unit
					playerUnitObject.transform.position = vecTemp;
					MapTileManager.Instance.MoveMarker(playerUnit.TurnOrder, startTile);
					break;
				}
			}
			PlayerUnitObject puo = playerUnitObject.GetComponent<PlayerUnitObject>();
			puo.UnitId = playerUnit.TurnOrder;
			PlayerManager.Instance.AddPlayerObject(playerUnitObject);
			playerUnitObject.transform.SetParent(playerHolder);
			PlayerManager.Instance.SetInitialFacingDirection(playerUnit.TurnOrder);
		}
	}

	//offline: takes into account player preferences
	//online: both sides human
	Drivers SetDrivers(int teamId, bool isOffline, bool isRLDriver)
	{
		//Debug.Log("setting RL driver " + isRLDriver);
		//AI is trainable by Machine Learning, set drivers accordingly
		if (isRLDriver)
			return Drivers.ReinforcementLearning;

		//Debug.Log("setting RL driver after " + isRLDriver);

		if (isOffline)
		{
			int aiType = PlayerPrefs.GetInt(NameAll.PP_AI_TYPE, NameAll.AI_TYPE_HUMAN_VS_AI);
			int zEntry = PlayerPrefs.GetInt(NameAll.PP_COMBAT_ENTRY, NameAll.SCENE_CUSTOM_GAME);
			if (aiType == NameAll.AI_TYPE_HUMAN_VS_AI || zEntry == NameAll.SCENE_STORY_MODE || zEntry == NameAll.SCENE_CAMPAIGNS)
			{
				if (teamId == NameAll.TEAM_ID_GREEN)
					return Drivers.Human;
				else
					return Drivers.Computer;
			}
			else if (aiType == NameAll.AI_TYPE_HUMAN_VS_HUMAN)
			{
				return Drivers.Human;
			}
			else if (aiType == NameAll.AI_TYPE_AI_VS_AI)
			{
				return Drivers.Computer;
			}
			else
			{
				if (teamId == NameAll.TEAM_ID_GREEN)
					return Drivers.Human;
				else
					return Drivers.Computer;
			}
		}
		else
		{
			return Drivers.Human;
		}
	}

	#endregion


	/*
	 * DEPRECATED, now put the coroutiens in an inner thing
	public IEnumerator DoFastActionInner(Board board, CombatTurn turn, bool isActiveTurn, bool isSlowActionPhase, bool isReaction, bool isMime, bool isWAMode=false)
    {
        //Debug.Log("reached fast action inner here");
        if( isActiveTurn && SlowActionCheck(turn) )
        {
            //Debug.Log("something is fucked up about the targetting. unitId: " + turn.targetUnitId + " x,y " + turn.targetTile.pos.x + "," + turn.targetTile.pos.y);
            //slow action, adds it to the queue, apply ability is done, moves onto proper state
            SpellManager.Instance.CreateSpellSlow(turn, isWAMode); 
            if (turn.spellName.CommandSet == NameAll.COMMAND_SET_CHARGE)
            {
                turn.hasUnitMoved = true;
            }
            else if (turn.spellName.CommandSet == NameAll.COMMAND_SET_JUMP)
            {
                turn.hasUnitMoved = true;
            }
            //Debug.Log("going through do fast action inner");
        }
        else
        {
            //non slow action
            //get all the relevant data first, then passed on that set the proper order
            //Debug.Log("reached here, non active turn && slow action");
            bool isTwoSwordsEligible = PlayerManager.Instance.IsEligibleForTwoSwords(turn.actor.TurnOrder, turn.spellName.CommandSet);
            bool isPassesChecks = CalculationResolveAction.FastActionChecks(turn, true); //suppress messages for the first time
            if (isPassesChecks)
            {
                //decrement from inventory/chance of breaking
                //can't select ability if not enough items
                CalculationResolveAction.CheckInventory(turn.spellName, turn.actor.TeamId, turn.actor.TurnOrder);

                ApplyPreresults(board,turn,isReaction,isMime);//mimequeue and subtract MP

                //do animation (turn, animation)
                //may want to rearrange this due to double swings
                PlayerManager.Instance.SetFacingDirectionAttack(board, turn.actor, turn.targetTile); //Debug.Log("reached here");
                PlayerManager.Instance.SetPlayerObjectAnimation(turn.actor.TurnOrder, "attacking", false);
                PlayerManager.Instance.SendPlayerObjectAnimation(turn.actor.TurnOrder, "attacking", false); //in online games, sends out the RPC
                //yield return new WaitForSeconds(0.3f);
                
                //PlayerManager.Instance.SetPlayerObjectAnimation(turn.actor.TurnOrder, "setIdle",true);

                
                //does the checks, if any check fails proper message is shown
                ParticleManager.Instance.LaunchCastParticle(turn.actor, turn.targetTile, turn.spellName);

                //roll for on attack effect (if applicable)
                bool isOnAttackEffect = IsOnAttackEffect(turn.actor, turn.spellName);

                //results (iterates over targets)
                //reflect check (animation on target)
                //effect on target 
                //chance to counter (does not show the counter, simply adds it to the counter queue)
                //animations on target (just launches it, not worked about sync)
                //statuses on target (just launches it, not worried about sync)
                CalculationResolveAction.FastActionResults(board, turn, isReaction:isReaction, isFirstSwing:true, isForceFacing:false, 
                    isAllowBladeGrasp:true, isOnAttackEffect:isOnAttackEffect);
                if (isOnAttackEffect)
                {
                    SpellName snAttackEffect = CheckForOnAttackEffect(turn.actor, turn.spellName);
                    if (snAttackEffect != null)
                    {
                        CalculationResolveAction.FastActionAttackEffect(board, turn, isReaction, isForceFacing:false, isAllowBladeGrasp:false, isOnAttackEffect:false);
                    }
                }
                //Debug.Log("in calc mono, is two swords eligible: " + isTwoSwordsEligible);
                if (isTwoSwordsEligible)
                {
                    yield return new WaitForSeconds(1.5f);
                    PlayerManager.Instance.SetPlayerObjectAnimation(turn.actor.TurnOrder, "attacking", false); //Debug.Log("in calc mono, is two swords eligible: " + isTwoSwordsEligible);
                    PlayerManager.Instance.SendPlayerObjectAnimation(turn.actor.TurnOrder, "attacking", false); //in online games, sends out the RPC
                    //does the second swing, may need some data from the first swing
                    bool isForceFacing = false; bool isAllowBladeGrasp = false;
                    isOnAttackEffect = IsOnAttackEffect(turn.actor, turn.spellName);
                    CalculationResolveAction.FastActionSecondSwing(board, turn, isReaction, isForceFacing, isAllowBladeGrasp, isOnAttackEffect);
                    if (isOnAttackEffect)
                    {
                        SpellName snAttackEffect = CheckForOnAttackEffect(turn.actor, turn.spellName);
                        if (snAttackEffect != null)
                        {
                            CalculationResolveAction.FastActionAttackEffect(board, turn, isReaction, isForceFacing: false, isAllowBladeGrasp: false, isOnAttackEffect: false);
                        }
                    }
                }

                //if it's a reaction, set the actor back to how it was facing before
                if( isReaction || isMime) //mime never turns permanently and animated shouldn't turn at all but after the turn sometimes ends up facing in a wonky direction
                {
                    yield return new WaitForSeconds(1.0f); //not sure on this timing
                    if (!StatusManager.Instance.IfStatusByUnitAndId(turn.actor.TurnOrder, NameAll.STATUS_ID_DEAD) )
                        //&& turn.actor.Dir != PlayerManager.Instance.GetPlayerUnitObjectComponent(turn.actor.TurnOrder).di)
                    {
                        PlayerManager.Instance.SetPUODirectionMidTurn(turn.actor.TurnOrder, turn.actor.Dir);
                    }
                }
            }
            else
            {
                CalculationResolveAction.FastActionChecks(turn, false);
            }
        }
        //yield return null;
        yield return new WaitForSeconds(0.3f); //just a visual thing so the camera doesn't change too quickly
        //Debug.Log("finished calc mono, fast action innner");
    }
	*/
}
