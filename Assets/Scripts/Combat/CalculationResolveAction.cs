using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
    OLD NOTES
    //assuming reflect spells if the resolve over an area, the single panel reflects
        //ie doesn't reflect a spell's entire area of effect
	to do list:
	the two swings code is super messy and complicated
	the resolve action code is super messy and complicated
	can probably split some functionionality of this class into different things
	reflect not really implemented fully mechanically
 */

	/// <summary>
	/// Mess of a class. Performs many calculations related to game logic. Mostly dealing with resolving actions.
	/// Most important thing is that through CalculationMono, calculates (and starts the application of) action resolving.
	/// Not just fast actions but slow actions, reactions, etc getting resolved runs through here.
	/// Also a mess of convenience functions. May make sense to split this class up.
	/// </summary>
public static class CalculationResolveAction {

    static int bugTest = 0;

    //called in CalculationMono for the various attack checks
    public static bool FastActionChecks(CombatTurn turn, bool isSuppressText)
    {
        
        //check if spellMP > actor's available MP
        //if no MP, shows a message
        if (!IsMP(turn.spellName, turn.actor, isSuppressText))
            return false;
        //spell targets a jumping unit, does not resolve; if target is jumping shows a message
        if (turn.targetUnitId != NameAll.NULL_UNIT_ID && IsJumping(turn.actor.TurnOrder, isSuppressText))
            return false;
        //ie silenced and trying to cast spell, showas a message
        if (IsUnableToPerformAction(turn.actor.TurnOrder, turn.spellName, isSuppressText)) 
            return false;

        return true;
    }

    //called in CalculationMono
    public static void CheckInventory(SpellName sn, int teamId, int actorId, int renderMode)
    {
        if (sn.Version == NameAll.VERSION_CLASSIC)
        {
            if (sn.CommandSet == NameAll.COMMAND_SET_DRAW_OUT)
            {
                if (PlayerManager.Instance.IsRollSuccess(15, 1, 100, NameAll.NULL_INT, NameAll.COMBAT_LOG_SUBTYPE_KATANA_BREAK, sn, 
					PlayerManager.Instance.GetPlayerUnit(actorId) ))
                {
					if(renderMode != NameAll.PP_RENDER_NONE)
						PlayerManager.Instance.ShowFloatingText(actorId, NameAll.FLOATING_TEXT_CUSTOM, "Katana Broken");
                    SpellManager.Instance.DecrementInventory(sn, teamId);
                }
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_ELIXIR)
            {
				if (renderMode != NameAll.PP_RENDER_NONE)
				{
					if (teamId == NameAll.TEAM_ID_GREEN)
					{
						PlayerManager.Instance.ShowFloatingText(actorId, NameAll.FLOATING_TEXT_CUSTOM, "Elixirs Remaining " + (SpellManager.Instance.greenElixir - 1));
					}
					else
					{
						PlayerManager.Instance.ShowFloatingText(actorId, NameAll.FLOATING_TEXT_CUSTOM, "Elixirs Remaining " + (SpellManager.Instance.redElixir - 1));
					}
				}
				SpellManager.Instance.DecrementInventory(sn, teamId);
            }
        }
    }


    //Called in CalculationMono, allows fastactions and things that become fast actions (slow actions, reactions etc) to proceed)
    public static void FastActionResults(Board board, CombatTurn turn, bool isReaction = false, bool isFirstSwing = true, bool isForceFacing = false,
        bool isAllowBladeGrasp = false, bool isOnAttackEffect = false, int renderMode = 0)
    {
        //Debug.Log("launching fast action results " + turn.targets.Count);

        List<int> outEffect; PlayerUnit target;

        //some spells have no target tile due to intended effect being projectile collision. 
        //In this case, adds the targetTile as a tile to potentially target (but not if it's a self target ability, or if the targetTile is where the actor is standing)
        if( turn.targets.Count == 0 && turn.spellName.RangeXYMin != NameAll.SPELL_RANGE_MIN_SELF_TARGET
            && turn.targetTile != null && turn.targetTile != board.GetTile(turn.actor) )
        {
            //Debug.Log("Fast action result: adding the target tile ");
            turn.targets.Add(turn.targetTile); //so you can check for projectile collision
        }

        SpellName sn;
        if (turn.spellName.CommandSet == NameAll.COMMAND_SET_MATH_SKILL)
            sn = turn.spellName2; //change it here as the reflect calculations 
        else
            sn = turn.spellName;

        //Debug.Log("doing the fast action results " + turn.targets.Count);
        for (int i = 0; i < turn.targets.Count; i++)
        {
            //Debug.Log(" " + turn.targets[i].GetTileSummary());
            //check for projectile collision on current tile
            Tile targetTile = GetProjectileCollision(board, turn.actor, turn.targets[i],turn.spellName); //Debug.Log("cycling through tiles " + targetTile.pos.x + ", " + targetTile.pos.y);
            int targetId = targetTile.UnitId;
            if (targetId != NameAll.NULL_UNIT_ID)
            {
                target = PlayerManager.Instance.GetPlayerUnit(targetId);
                target = ReflectCalculation(board, turn.actor, turn.spellName, target);
                //target = FloatCalculation(board, turn, target);
                if (target == null)
                    return; //spell reflected off the map or somewhere inconsequential
                //Debug.Log("fast action results prior to outEffect");
                //only use sn here, math skill can't be reflected
                outEffect = GenerateHitDamageList(board, turn.actor, target, sn, allowBladeGrasp: isAllowBladeGrasp, forceFacing: isForceFacing); 
                ResolveSpellAction(outEffect, board, sn, turn.actor, target, isReaction: isReaction, isFirstSwing: isFirstSwing, isOnEffectAttack: isOnAttackEffect, renderMode: renderMode);
            }
            else
            {
                continue;
            }
        }
    }

    //two swords second swing version of fastaction results
    public static void FastActionSecondSwing(Board board, CombatTurn turn, bool isReaction, bool isForceFacing = false,
        bool isAllowBladeGrasp = false, bool isOnAttackEffect = false, int renderMode = 0)
    {
        if (turn.targetTile.UnitId == NameAll.NULL_UNIT_ID)
            return;

        PlayerUnit target = PlayerManager.Instance.GetPlayerUnit( turn.targetTile.UnitId ); 
        if( target.StatTotalLife > 0)
        {
            List<int> outEffect = GenerateHitDamageList(board, turn.actor, target, turn.spellName, allowBladeGrasp: isAllowBladeGrasp, forceFacing: isForceFacing);
            ResolveSpellAction(outEffect, board, turn.spellName, turn.actor, target, isReaction: isReaction, isFirstSwing: false, isOnEffectAttack: isOnAttackEffect, renderMode: renderMode);
        }
        
    }

    //on attack effect of an attack version of fastaction results
	//ie a weapon that has a chance to cast a spell after the weapon attack
    public static void FastActionAttackEffect(Board board, CombatTurn turn, bool isReaction, bool isForceFacing = false,
       bool isAllowBladeGrasp = false, bool isOnAttackEffect = false, int renderMode = 0)
    {
        PlayerUnit target = PlayerManager.Instance.GetPlayerUnit(turn.targetTile.UnitId);
        if (target.StatTotalLife > 0)
        {
            List<int> outEffect = GenerateHitDamageList(board, turn.actor, target, turn.spellName, allowBladeGrasp: isAllowBladeGrasp, forceFacing: isForceFacing);
            ResolveSpellAction(outEffect, board, turn.spellName, turn.actor, target, isReaction: isReaction, isFirstSwing: false, isOnEffectAttack: isOnAttackEffect, renderMode: renderMode);
        }

    }

    //check for reflect on target and if reflected gets new target (if there is one)
    static PlayerUnit ReflectCalculation(Board board, PlayerUnit actor, SpellName sn, PlayerUnit target, bool isShowFloatingText = true)
    {
        if (StatusManager.Instance.CheckIfReflect(target.TurnOrder))
        {
            if (sn.EvasionReflect == 1 || sn.EvasionReflect == 3)
            {
                if( isShowFloatingText)
                    PlayerManager.Instance.ShowFloatingText(target.TurnOrder, NameAll.FLOATING_TEXT_CUSTOM, "REFLECTED");

                Tile t = board.GetReflectTile(actor,target);
                if (t != null && t.UnitId != NameAll.NULL_UNIT_ID)
                {
                    return PlayerManager.Instance.GetPlayerUnit(t.UnitId);
                }
                return null;
            }
        }
        return target;
    }

    //check to see if unit is floating and if so whether the spell still hits (height is changed), called in loop through target panel
    //currently not implemented, need to learn more about float
    static PlayerUnit FloatCalculation(Board board, CombatTurn turn, PlayerUnit target)
    {
        if (target == null)
            return null;

        if (StatusManager.Instance.CheckIfFloat(target.TurnOrder))
        {
            //redo the height check
            int heightDifference = Math.Abs(board.GetTile(turn.actor).height - ( board.GetTile(target).height + 1));
            //Debug.Log("in floatCalculation checking the height difference " + heightDifference);
            if( heightDifference <= turn.spellName.EffectZ)
            {
                //Debug.Log("passed float height difference check");
                return target;
            }
            else
            {
                //Debug.Log("failed float height difference check");
                return null;
            }
            
        }
        else
            return target;
    }

    //sees if actor can perform an action, 
    static bool IsUnableToPerformAction(int actorId, SpellName sn, bool isSuppressText = false)
    {
        //check that a status doesn't prevent this type of spell//for now it's just silence
        if (StatusManager.Instance.IfStatusByUnitAndId(actorId, NameAll.STATUS_ID_SILENCE, true) && sn.StatusCancel == 1) //StatusManager.Instance.Get().ifStatusByUnitAndId(actor.TurnOrder, "silence", true)
        {
            if (sn.StatusCancel == 1)
            {
                if (!isSuppressText)
                    PlayerManager.Instance.ShowFloatingText(actorId, 19, "Silenced");
            }
            return true; //ends this
        }
        return false;
    }

    //checks if unit is jumping, jumping units cannot have anythign resolve on them
    static bool IsJumping(int unitId, bool isSuppressText)
    {
        if (StatusManager.Instance.IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_JUMPING, false))
        {
            if (!isSuppressText)
                PlayerManager.Instance.ShowFloatingText(unitId, 19, "Target is Jumping");
            return true; //unit is jumping
        }
        return false;
    }

    //checks if MP is needed and unit does not have enough MP
    static bool IsMP(SpellName sn, PlayerUnit actor, bool isSuppressText)
    {
        if (GetMPCost(actor, sn) > actor.StatTotalMP)
        {
            if( !isSuppressText)
                PlayerManager.Instance.ShowFloatingText(actor.TurnOrder, 19, "No MP!");
            return false;
        }
        return true;
    }

    //called in CalculationMono, subtracts the MP
    public static void SubtractMP(PlayerUnit actor, SpellName sn)
    {
        int spellMP = GetMPCost(actor, sn);
        if (spellMP > 0)
        {
            PlayerManager.Instance.RemoveMPById(actor.TurnOrder, spellMP);
        }
    }

    //called here and in UIAbilityScrollList to see the MP cost of the SN
    public static int GetMPCost(PlayerUnit actor, SpellName sn)
    {
        if (actor.ClassId == NameAll.CLASS_MIME) //mimic'd spells use no mp
        {
            return 0;
        }

        int spellMP;

        int z1 = sn.MP;
        if (z1 > 1000)
        {
            z1 = (actor.StatTotalMaxMP * (z1 - 1000)) / 100;
        }

        if (actor.IsAbilityEquipped(NameAll.SUPPORT_HALF_MP, NameAll.ABILITY_SLOT_SUPPORT) || actor.IsAbilityEquipped(NameAll.SUPPORT_MP_CUT, NameAll.ABILITY_SLOT_SUPPORT))// half mp
        {
            spellMP = (z1 + 1) / 2; //rounds up
        }
        else {
            spellMP = z1;
        }
        return spellMP;
    }

    
    //called in CalculationMono after certain FastActions, checks if item should be added to MimeQUeue
    public static void AddToMimeQueue(Board board, PlayerUnit actor, SpellName sn, Tile targetTile)
    {
        
        if (bugTest == 1)
        {
            Debug.Log("In add to mime queue" + sn.CalculateMimic+ " asdf " + actor.ClassId + " asdf " + PlayerManager.Instance.IsMimeOnTeam(actor.GetCharmTeam()));
        }
        if ((sn.CalculateMimic== 1 || sn.CalculateMimic == 3) && PlayerManager.Instance.IsMimeOnTeam(actor.GetCharmTeam())
            && actor.ClassId != NameAll.CLASS_MIME)
        {
            if (bugTest == 1)
            {
                Debug.Log("In add to mime queue, passed first check " + targetTile.GetTileSummary() + " " + board.GetTile(actor).GetTileSummary() );
            }
            List<PlayerUnit> mimeList = PlayerManager.Instance.GetMimeList(actor.GetCharmTeam());
            foreach (PlayerUnit mimePU in mimeList)
            {
                //Debug.Log("Add mime test using a board function");
                Tile mimeTargetTile = board.GetMimeTargetTile(actor, mimePU, targetTile);
                if( mimeTargetTile != null)
                {
                    if (bugTest == 1)
                    {
                        Debug.Log("In add to mime queue, passed last check, adding to mime queue " + mimeTargetTile.GetTileSummary() );
                    }
                    SpellReaction sr = new SpellReaction(mimePU.TurnOrder, sn.Index, mimeTargetTile);
                    SpellManager.Instance.AddMimeQueue(sr);
                }
            }
        }
    }

    //gets the fastaction preview details, called in CombatConfirmAbilityTargetState
    public static List<int> GetFastActionPreview(Board board, PlayerUnit actor, SpellName sn, Tile targetTile)
    {
        List<int> outEffect = new List<int>();
        outEffect.Add(0); outEffect.Add(0); outEffect.Add(0); outEffect.Add(0); outEffect.Add(0);

        //Status notes
        //can still cast spells with no MP or silence, gets correct % back anyway
        //believe that statuses shouldn't effect this (only one i can think of is don't act which blocks out the menu
        //sees if there is a target on that tile
        //in future if caring about more targets in the effect, can get a list of all targetable tiles then do the calculation
        //projectiles can change the map tile index

        Tile newTargetTile = GetProjectileCollision(board, actor, targetTile, sn);
        if( newTargetTile.UnitId != NameAll.NULL_UNIT_ID) //hits some unit
        {
            if( newTargetTile.UnitId != targetTile.UnitId) //hits unit other than who was targetted, don't show anything
            {

            }
            else
            {
                if(!StatusManager.Instance.IfStatusByUnitAndId(targetTile.UnitId, NameAll.STATUS_ID_JUMPING))
                {
                    PlayerUnit target = PlayerManager.Instance.GetPlayerUnit(targetTile.UnitId);
                    PlayerUnit reflectTarget = ReflectCalculation(board, actor, sn, target, false);

                    //reflect on different unit or reflected to no unit at all, shit stays at 0
                    if ( reflectTarget == null || reflectTarget.TurnOrder != target.TurnOrder)
                    {

                    }
                    else
                    {
                        //spell will actually hit the targeted unit, generates a preview
                        sn = CalculationAT.ModifySlowActionSpellName(sn, actor.TurnOrder); //in case of charge, not used for anything else currently
                        outEffect = GenerateHitDamageList(board, actor, target, sn); //doesn't need to worry about Two Swords nonsens
                    }
                }
            }
        }
        else
        {
            //no unit on tile
        }
        
        if (bugTest == 1)
        {
            Debug.Log("In get FastActionPreview " + outEffect[0] + " " + outEffect[1] + " " + outEffect[2] + " " + outEffect[3] + " ");
        }
        return outEffect;
    }

    //called in fastActionPreview
    //called in FastActionResults
    //sees if another unit gets in the way of the target unt
    static Tile GetProjectileCollision(Board board, PlayerUnit actor, Tile targetTile, SpellName sn)
    {
        //Debug.Log("in Get ProjectileCollission start");
        //check if sn is a project and what type
        int projectileType = GetProjectileType(sn);
        if (projectileType > NameAll.PROJECTILE_NONE)
        {
            //Debug.Log("in get ProjectileCollission about to do linecast test");
            //projectiles that have a min range of 1 and the target one panel away, don't need to do the projectile
            if (projectileType == NameAll.PROJECTILE_LIMITED)
            {
                if (MapTileManager.Instance.GetDistanceBetweenTiles(actor.TileX, actor.TileY, targetTile.pos.x, targetTile.pos.y) == 1)
                {
                    return targetTile;
                }
            }

            //new way: collisions based on math
            //CombatAbilityRange ar = new CombatAbilityRange();
            CalculationProjectile cp = new CalculationProjectile();
            targetTile = cp.GetProjectileHit(board, sn, actor, targetTile);

            //old way: do ray and collision
            //Debug.Log("in get ProjectileCollission about to do linecast test");

            //get the actor height (for now just use actor tile plus some fudge factor)
            //get target height (0.5f above the tile/player)
            //if actor and target right next to each other, still rely on collider
            //could do a ray between the two targets I guess and assume everything is straight line

            //GameObject goTarget;
            //int targetId = targetTile.UnitId;
            //if (targetId != NameAll.NULL_UNIT_ID)
            //{
            //    goTarget = PlayerManager.Instance.GetPlayerUnitObject(targetId);
            //}
            //else
            //{
            //    goTarget = targetTile.gameObject;
            //}

            ////if any collision by any object between the start and end returns
            //RaycastHit hit;
            //GameObject goActor = PlayerManager.Instance.GetPlayerUnitObject(actor.TurnOrder); //PlayerUnitObject puActor = PlayerManager.Instance.GetPlayerUnitObjectComponent(actor.TurnOrder);
            //Vector3 offset = new Vector3(0.0f, 0.5f, 0.0f); //assuming it starts from the feet, want to start a little higher
            //if (Physics.Linecast(goActor.transform.position + offset, goTarget.transform.position + offset, out hit))
            //{
            //    //is a collision, need to see what the collision is
            //    //Debug.Log("was a collision ");
            //    GameObject hitObject = hit.transform.gameObject;
            //    PlayerUnitObject puTarget = hitObject.GetComponent<PlayerUnitObject>();
            //    if (puTarget != null)
            //    {
            //        PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(puTarget.UnitId);
            //        return board.GetTile(pu);
            //    }
            //}
            //Debug.Log("no collission between units");
        }
        return targetTile;
    }


    //called in the FastAction functions, gets the actual effect of the spells on the target
    static List<int> GenerateHitDamageList(Board board, PlayerUnit actor, PlayerUnit target, SpellName sn, bool allowBladeGrasp = true, bool forceFacing = false)
    {
        //get the mod calculation
        //List<int> outEffect = new List<int>();
        //if (sn.CommandSet == NameAll.COMMAND_SET_BATTLE_SKILL) //battle skill becomming attack in some cases
        //{
        //    if ((sn.SpellId == NameAll.SPELL_INDEX_WEAPON_BREAK && target.ItemSlotWeapon == NameAll.FIST_EQUIP)
        //        || (sn.SpellId == NameAll.SPELL_INDEX_SHIELD_BREAK && target.ItemSlotOffhand == NameAll.NO_EQUIP)
        //        || (sn.SpellId == NameAll.SPELL_INDEX_HEAD_BREAK && target.ItemSlotHead == NameAll.NO_EQUIP)
        //        || (sn.SpellId == NameAll.SPELL_INDEX_ARMOR_BREAK && target.ItemSlotBody == NameAll.NO_EQUIP))
        //    {
        //        SpellName snAttack = SpellManager.Instance.GetSpellNameByIndex(CalculationAT.GetAttackSpellIndex(actor.TurnOrder));
        //        ResolveFastAction(actor,snAttack);
        //        //outEffect = CalculationMod.GetModCalculation(snAttack, actor, target);
        //    }
        //}
        //else
        //{
        //    outEffect = CalculationMod.GetModCalculation(sn, actor, target);
        //}

        List<int> outEffect = CalculationMod.GetModCalculation(sn, actor, target);
        if (bugTest == 1)
        {
            Debug.Log("In calculation mod " + outEffect[0] + " " + outEffect[1] + " " + outEffect[2] + " " + outEffect[3] + " ");
        }
        //check to see if blocked by lasting status
        outEffect.Add(0); //change to 1 if main status blocked, 2 if additional status blocked, 3 if both blocked (doubt I'll use 3)
        if (sn.HitsStat == 0 && sn.AddsStatus == 1) //spell type is the type that just casts a status
        {
            if (StatusManager.Instance.IsStatusBlockByUnit(target.TurnOrder, sn.StatusType))
            {
                //outEffect[0] = 0; //effectively 0 but would rather show the block in preview and in the spell resolving
                outEffect[4] = 1;
            }
        }
        else if (sn.AddsStatus == 2) //spell type is the type that has status infliction as an additional type
        {
            if (StatusManager.Instance.IsStatusBlockByUnit(target.TurnOrder, sn.StatusType))
            {
                outEffect[4] = 2; //main spell goes through but additional component blocked
            }
        }

        List<int> tempList;
        //get the evasion calculation
        if (sn.Version != NameAll.VERSION_CLASSIC)
        {
            tempList = CalculationEvasion.GetAurelianEvasion(outEffect[0], board, sn, actor, target, forceFacing);
        }
        else
        {
            tempList = CalculationEvasion.GetEvasionHitChance(outEffect[0], board, sn, actor, target, allowBladeGrasp, forceFacing);
        }

        //first variable is the hit chance override outEffect
        outEffect[0] = tempList[0];
        outEffect.Add(tempList[1]); outEffect.Add(tempList[2]); outEffect.Add(tempList[3]); //adds the three evasion components

        if (bugTest == 1)
        {
            Debug.Log("In calculation mod " + outEffect[0] + " " + outEffect[1] + " " + outEffect[2] + " " + outEffect[3] + " ");
        }
        return outEffect;
    }


    #region ResolveSpellAction and related functions
    //called in FastActionResults and FastActionSecondSwing
    //applies the spellName effect and creates the reaction
    static void ResolveSpellAction(List<int> modEffectArray, Board board, SpellName sn, PlayerUnit actor, PlayerUnit target, bool isReaction = false, 
        bool isFirstSwing = false, bool isOnEffectAttack = false, int renderMode = 0)
    {
        PlayerManager.Instance.WalkAroundCombatStartCheck(actor, target, sn);
        //modEffectArray has
        //it rate at 0, effect at 1, if crit at 2 (run knockback check from this), 3 is special effect (like chakra)
        //4 tells if blocked by item block (ie block_darkness: 1 if main status blocked, 2 if additional status blocked, 3 if both blocked (doubt I'll use 3)
        //5 is evasion due to dodge, 6 is evasion due to block (0 for now), 7 is evasion due to counter ability
        int effect = modEffectArray[1];
        bool targetReceivedDamage = false;
        int targetStartLife = target.StatTotalLife; //checks down below tosee if damage done, needed for reaction abilities
        int reactionEffect = 0; //spells like distribute and

        //hamedo code here
        if (sn.CommandSet == NameAll.COMMAND_SET_ATTACK || sn.CommandSet == NameAll.COMMAND_SET_ATTACK_AURELIAN) //hamedo only interrupts an attack
        {
            if (PlayerManager.Instance.IsAbilityEquipped(target.TurnOrder, NameAll.REACTION_HAMEDO, NameAll.ABILITY_SLOT_REACTION) && isFirstSwing)
            {
                //checks to see if range
                int tempSpellIndex = CalculationAT.GetAttackSpellIndex(target.TurnOrder);
                SpellName tempSN = SpellManager.Instance.GetSpellNameByIndex(tempSpellIndex);
                if (MapTileManager.Instance.IsTileInAttackRange(actor,target, tempSN)) //TARGET AND ACTOR SWAPPED
                {
					//roll to see if reaction is successful
					if (PlayerManager.Instance.IsRollSuccess(target.StatTotalBrave, 1, 100, NameAll.NULL_INT, NameAll.COMBAT_LOG_SUBTYPE_REACTION_BRAVE_ROLL, 
						sn, actor, target, target.AbilityReactionCode) )
                    {
                        //creates the reaction
                        CreateSpellReaction(NameAll.REACTION_HAMEDO, target, actor, sn, reactionEffect); //TARGET AND ACTOR SWAPPED
                        //Debug.Log("Hamedo reaction and two swords issue in CalculationResolveActoin");
                        return;//attack no longer goes into effect, interrupts the current resolution
                    }
                }
            }
        }

		//does it hit
		int rollResult = PlayerManager.Instance.GetRollResult(modEffectArray[0], 1, 100, NameAll.NULL_INT, NameAll.COMBAT_LOG_SUBTYPE_ROLL_RESOLVE_ACTION, sn, actor, target);
        string combatLogString = "";
        if (isReaction)
        {
			if(renderMode != NameAll.PP_RENDER_NONE)
				combatLogString = "reaction type: " + BuildCombatLogString(actor, target, sn, 1, renderMode);
        }
        else
        {
            combatLogString = BuildCombatLogString(actor, target, sn, 1, renderMode);
        }
        combatLogString = AppendCombatLogString(combatLogString, modEffectArray, rollResult, sn, 1, renderMode);

        if (rollResult <= modEffectArray[0]) //spell hits
        {
			if(renderMode != NameAll.PP_RENDER_NONE)
			{
				SoundManager.Instance.PlaySoundClip(1);
				//actor cast particles over head
				AddParticleHit(target.TurnOrder, sn);
			}
            combatLogString = AppendCombatLogString(combatLogString, " hits ", renderMode);

            //unit can't be dead for this part to hit
            if( !StatusManager.Instance.IfStatusByUnitAndId(target.TurnOrder,NameAll.STATUS_ID_DEAD) )
            {
                //do the knockback check
                if (modEffectArray[2] >= 1)
                {
                    //Debug.Log("doing crit knockback check 1");
                    if (modEffectArray[2] == NameAll.CRIT_KNOCKBACK_SUCCESS) //dash, throw stone, already passed the knockback check
                    {
                        Tile moveTile = CalculationMod.KnockbackCheck(board, sn, actor, target, 1, 1, true); //Debug.Log("doing crit knockback check 2");
                        if (moveTile != null)
                        {
                            combatLogString = AppendCombatLogString(combatLogString, " target is moved back by ability ", renderMode); //Debug.Log("doing crit knockback check 3");
                                                                                    //moves the target to a new location
                            //Debug.Log("using knockback a player in CalcResolve");
                            PlayerManager.Instance.KnockbackPlayer(board, target.TurnOrder, moveTile, sn, actor); //50 50 roll happened elsewhere
                        }
                    }
                    else //test for crit knock back
                    {
						PlayerManager.Instance.ShowFloatingText(target.TurnOrder, NameAll.FLOATING_TEXT_CUSTOM, "CRITICAL HIT");
						combatLogString = AppendCombatLogString(combatLogString, " critical hit ", renderMode);
                        Tile moveTile = CalculationMod.KnockbackCheck(board, sn, actor, target, 1, 1, true);
                        if (moveTile != null)
                        {
							combatLogString = AppendCombatLogString(combatLogString, " target is moved back by crit ", renderMode);
                            //moves the target to a new location
                            //Debug.Log("using knockback a player in CalcResolve");
                            PlayerManager.Instance.KnockbackPlayer(board, target.TurnOrder, moveTile, sn, actor);
                            //PlayerManager.Instance.MovePlayer(target.TurnOrder, moveLocation, true);
                        }
                    }
                }
                combatLogString = AppendCombatLogString(combatLogString, modEffectArray, rollResult, sn, 2, renderMode);

                if (sn.HitsStat >= 1 && sn.HitsStat < 4) //hits a stat, if 0 then it only adds a status
                {
                    effect = CheckForGolem(sn, effect, target.TeamId, target.TurnOrder); //Debug.Log("doing golem check");
                    AlterUnitStatMPSwitch(effect, actor, target, sn, isReaction);

                    int snAbsorb = CheckForSpellNameAbsorb(actor, sn);

                    if (snAbsorb == 2) //absorbs the stat too (1 is damage 0 is heal, might be modified by undead reversal
                    {
                        if (sn.ElementType == NameAll.ITEM_ELEMENTAL_UNDEAD && StatusManager.Instance.IsUndead(target.TurnOrder))
                        {
                            PlayerManager.Instance.AlterUnitStat(1, effect, sn.StatType, actor.TurnOrder, sn.ElementType, sn, actor); //undead absorb, does damage
                        }
                        else
                        {
                            PlayerManager.Instance.AlterUnitStat(0, effect, sn.StatType, actor.TurnOrder, sn.ElementType, sn, actor); //absorb the stat
                        }
                    }

                    //roll for success and io check, check for is this an attack command set done outside of the function
                    if (isOnEffectAttack)
                    {
                        ItemObject io = ItemManager.Instance.GetItemObjectById(actor.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON);
                        //check if spell type or not, spell types are over 1000 (formerly spell types are coded in by starting with 0_
                        if (io.OnHitEffect > 1000) //.Length > 2 && io.OnHitEffect.Substring(0, 1).Equals("0") )
                        {
                            return; //no counter here, counter chance in next one
                        }
                        else
                        {
                            //string zString = NameAll.GetStatusString(io.OnHitEffect);
                            //check if status is blocked
                            if (!StatusManager.Instance.IsStatusBlockByUnit(target.TurnOrder, io.OnHitEffect))
                            {
								combatLogString = AppendCombatLogString(combatLogString, " additional status roll hits " + io.OnHitChance + " " + io.OnHitEffect, renderMode);
                                StatusManager.Instance.AddStatusAndOverrideOthers(effect, target.TurnOrder, io.OnHitEffect);
								PlayerManager.Instance.ShowFloatingText(target.TurnOrder, 7, NameAll.GetStatusString(io.OnHitEffect)); //add status
                            }
                        }
                    }


                }
                else if (sn.HitsStat == 4)
                {
                    //chakra, restores hp and mp, mp stored in out effect 3
                    int effect2 = modEffectArray[3];
                    //hp
                    PlayerManager.Instance.AlterUnitStat(sn.RemoveStat, effect, NameAll.STAT_TYPE_HP, target.TurnOrder, sn.ElementType, sn, actor);
                    //mp
                    //Debug.Log("restoring Mp " + effect2);
                    PlayerManager.Instance.AlterUnitStat(sn.RemoveStat, effect2, NameAll.STAT_TYPE_MP, target.TurnOrder, sn.ElementType, sn, actor);
                }
                //if the spell uses a statuseffect, checks and does the necessary work
                combatLogString += OnHitAddStatus(actor, target,sn, effect, renderMode);
            }
            else
            {
                //Debug.Log("unit is dead, going to add life if life is the status added");
                //unit is dead, only works if status is adding life
                combatLogString += OnHitAddStatus(actor, target, sn, effect, renderMode, isDead : true);
            }
 
        }
        else
        {
			if (renderMode != NameAll.PP_RENDER_NONE)
				SoundManager.Instance.PlaySoundClip(2);
			combatLogString = AppendCombatLogString(combatLogString, " misses ", renderMode);
            //find out why it missed
            if (isFirstSwing) //dodged a two swords attack
            {
                if (rollResult <= modEffectArray[7]) //missed due to bladegrasp
                {
					//may need to account for bladegrasp being INACTIVE here
					combatLogString = AppendCombatLogString(combatLogString, " misses due to bladegrasp", renderMode);
                }
                else
                {
                    //may need to account for bladegrasp being ACTIVE here
                }
            }
			PlayerManager.Instance.ShowFloatingText(target.TurnOrder, 0, "MISS");
        }
        CombatLogClass clObject = new CombatLogClass(combatLogString, actor.TurnOrder, renderMode);
        clObject.SendNotification();
        
        if (!isReaction)
        {
            //Debug.Log("in isReaction " + isFirstSwing);
            if (isFirstSwing && PlayerManager.Instance.IsEligibleForTwoSwords(actor.TurnOrder,sn.CommandSet) ) //StatusManager.Instance.IsAbleToSecondSwing(actor.TurnOrder)
            {
                //Debug.Log(" is first swing and is able to second swing " + isFirstSwing);
                return; //only creating reactions on the 2nd swing
            }

            if (StatusManager.Instance.IsAbleToReact(target.TurnOrder))
            {
                //Debug.Log("in is able to react");
                if (target.StatTotalLife < targetStartLife && target.StatTotalLife != target.StatTotalMaxLife) //2nd inequality for equipment steals/breaks
                {
                    targetReceivedDamage = true; 
                    if (target.AbilityReactionCode == NameAll.REACTION_DAMAGE_SPLIT || target.AbilityReactionCode == NameAll.REACTION_RETURN_DAMAGE)
                    {
                        //finds out how much damage the actor took for the damage split effect
                        reactionEffect = (targetStartLife + 1 - target.StatTotalLife) / 2;
                    }
                }
                else if ((target.AbilityReactionCode == NameAll.REACTION_DISTRIBUTE || target.AbilityReactionCode == NameAll.REACTION_THRIFTY_HEAL)
                    && targetStartLife < target.StatTotalLife && target.StatTotalLife == target.StatTotalMaxLife)
                {
                    //Debug.Log("in distribute " + reactionEffect);
                    reactionEffect = effect - Math.Abs(targetStartLife - target.StatTotalMaxLife);
                    //Debug.Log("in distribute " + reactionEffect);
                    if (reactionEffect < 0)
                    {
                        reactionEffect = 0;
                    }
                }
                //Debug.Log("reached this part of reaction ability " + targetStartLife + " " + target.StatTotalLife);
                CheckForReaction(sn, target, actor, targetReceivedDamage, renderMode, reactionEffect); //TARGET AND ACTOR ARE SWAPPED
            }
            //else
            //{
            //    Debug.Log("NOT able to react");
            //}

        }
    }

    //called in resolve spellaction
    static string OnHitAddStatus(PlayerUnit actor, PlayerUnit target, SpellName sn, int effect, int renderMode, bool isDead = false)
    {
        string combatLogString = "";
    
        if (sn.AddsStatus >= 1)//adds a status, 1 means added on the roll, over 100 means subtract 100 and roll again
        {
            //unit is dead, can't be hit by any status except life and zombie life
            if( isDead && ( sn.StatusType != NameAll.STATUS_ID_LIFE && sn.StatusType != NameAll.STATUS_ID_ZOMBIE_LIFE && sn.StatusType != NameAll.STATUS_ID_CRYSTAL) )
            {
                return combatLogString;
            }

            if (sn.AddsStatus == 1)
            {
                if (StatusManager.Instance.IsStatusBlockByUnit(target.TurnOrder, sn.StatusType))//status blocked by item
                {
                    combatLogString += " status blocked by item immunity";
                    PlayerManager.Instance.ShowFloatingText(target.TurnOrder, 19, "Blocked"); //miss
                }
                else
                {
                    combatLogString += " status hits " + sn.StatusType; //Debug.Log("adding status " + sn.StatusType);
                    StatusManager.Instance.AddStatusAndOverrideOthers(effect, target.TurnOrder, sn.StatusType);
                    PlayerManager.Instance.ShowFloatingText(target.TurnOrder, 7, NameAll.GetStatusString(sn.StatusType)); //add status
                    PlayerManager.Instance.TallyCombatStats(actor.TurnOrder, NameAll.STATS_STATUSES_DONE, 1);
                }
            }
            else if (sn.AddsStatus > 100)
            {
                if (StatusManager.Instance.IsStatusBlockByUnit(target.TurnOrder, sn.StatusType)) //status blocked by item
                {
                    combatLogString += " additional status blocked by item immunity";
                    PlayerManager.Instance.ShowFloatingText(target.TurnOrder, 0, "Blocked"); //miss
                }
                else
                {
					//25% chance to hit, do a roll
					int roll = PlayerManager.Instance.GetRollResult(sn.AddsStatus - 100, 1, 100, NameAll.NULL_INT, NameAll.COMBAT_LOG_SUBTYPE_STATUS_ADD_ROLL,
						sn, actor, target, NameAll.NULL_INT, sn.StatusType);
                    if (roll <= sn.AddsStatus - 100)
                    {
                        combatLogString += " additional status roll hits 25 (" + roll + ") " + sn.StatusType;
                        StatusManager.Instance.AddStatusAndOverrideOthers(effect, target.TurnOrder, sn.StatusType);
						if (renderMode != NameAll.PP_RENDER_NONE)
							PlayerManager.Instance.ShowFloatingText(target.TurnOrder, 7, NameAll.GetStatusString(sn.StatusType)); //add status
                        PlayerManager.Instance.TallyCombatStats(actor.TurnOrder, NameAll.STATS_STATUSES_DONE, 1);
                    }
                    else
                    {
                        combatLogString += " additional status roll misses 25 (" + roll + ")";
                    }
                }
            }
        }
		if (renderMode != NameAll.PP_RENDER_NONE)
			return combatLogString;
		else
			return "";
    }

    //can be called in calcAT for preview string and in ResolveSpellAction
    public static int CheckForSpellNameAbsorb(PlayerUnit actor, SpellName sn)
    {
        int z1 = sn.RemoveStat;
        //chance this changes to 2 and effect is absorbed
        if (sn.CommandSet == NameAll.COMMAND_SET_ATTACK_AURELIAN && PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.SUPPORT_LEACH, NameAll.ABILITY_SLOT_SUPPORT) ) //absorbs the stat too (1 is damage 0 is heal, might be modified by undead reversal
        {
            z1 = NameAll.REMOVE_STAT_ABSORB;
        }
        else if( sn.CommandSet == NameAll.COMMAND_SET_ATTACK && actor.GetWeaponElementType() == NameAll.ITEM_ELEMENTAL_HP_DRAIN)
        {
            z1 = NameAll.REMOVE_STAT_ABSORB;
        }
        return z1;   
    }

    //called in ResolveSpellAction
    static void AlterUnitStatMPSwitch(int effect, PlayerUnit actor, PlayerUnit target, SpellName sn, bool isReaction ) //checks for mp switch on altering unit stat
    {
        List<int> tempList = CheckForSpellNameHPRestore(actor, sn); //checks if the weapon heals
		if (!isReaction && MPSwitchCheck(target) && sn.StatType == NameAll.STAT_TYPE_HP )
        {
            PlayerManager.Instance.AlterUnitStat(tempList[0], effect, NameAll.STAT_TYPE_MP, target.TurnOrder, tempList[1], sn, actor); //alters mp instead
        }
        else
        {
            PlayerManager.Instance.AlterUnitStat(tempList[0], effect, sn.StatType, target.TurnOrder, tempList[1], sn, actor);
        }
    }

    //can be called in calcAT for preview string and in ResolveSpellAction
    public static List<int> CheckForSpellNameHPRestore(PlayerUnit actor, SpellName sn)
    {
        List<int> tempList = new List<int>();
        tempList.Add(sn.RemoveStat);
        tempList.Add(sn.ElementType);
        if ((sn.CommandSet == NameAll.COMMAND_SET_ATTACK_AURELIAN || sn.CommandSet == NameAll.COMMAND_SET_ATTACK)
            && actor.GetWeaponElementType() == NameAll.ITEM_ELEMENTAL_HP_RESTORE)
        {
            tempList[0] = NameAll.REMOVE_STAT_HEAL;
            tempList[1] = NameAll.ITEM_ELEMENTAL_UNDEAD;
        }
        return tempList;
    }

    //can also be called in playermanager for poision (and probably for fall in the future)
    //also called in ResolveSpellAction
    public static bool MPSwitchCheck(PlayerUnit reactor)
    {
        if ((PlayerManager.Instance.IsAbilityEquipped(reactor.TurnOrder, NameAll.REACTION_MP_SWITCH, NameAll.ABILITY_SLOT_REACTION)
            || PlayerManager.Instance.IsAbilityEquipped(reactor.TurnOrder, NameAll.REACTION_HP_TO_MP, NameAll.ABILITY_SLOT_REACTION))
            && reactor.StatTotalMP > 0)
        {
            if (PlayerManager.Instance.IsRollSuccess(reactor.StatTotalBrave,1,100,NameAll.NULL_INT, NameAll.COMBAT_LOG_SUBTYPE_REACTION_BRAVE_ROLL, null,
				reactor, null, reactor.AbilityReactionCode))
            {
                return true;
            }
        }
        return false;
    }

    //called in ResolveSpellAction
    static void AddParticleHit(int targetId, SpellName sn)
    {
        int particleType = 0; //default type
        if (sn.PMType == NameAll.PM_TYPE_PHYSICAL && sn.RemoveStat != NameAll.HITS_STAT_NONE) //physical, blood effect
        {
            particleType = 1;
        }
        if (sn.RemoveStat == NameAll.REMOVE_STAT_REMOVE && sn.HitsStat != NameAll.HITS_STAT_NONE)
        {
            particleType = 4; //debuff
        }
        else if (sn.RemoveStat == NameAll.REMOVE_STAT_HEAL && sn.StatType == NameAll.STAT_TYPE_HP)
        {
            particleType = 2; //heal
        }
        else if (sn.RemoveStat == NameAll.REMOVE_STAT_HEAL)
        {
            particleType = 3; //buff
        } 

		//no particles for now
        //ParticleManager.Instance.SetSpellHit(targetId, particleType);
    }

    //called in ResolveSpellAction
    private static int GetProjectileType(SpellName sn)
    {
        //    public static readonly int PROJECTILE_NONE = 0;
        //public static readonly int PROJECTILE_LIMITED = 2;
        //public static readonly int PROJECTILE_ATTACK = 1;
        if (sn.RangeXYMax > 1 && (sn.CommandSet == NameAll.COMMAND_SET_ATTACK_AURELIAN || sn.CommandSet == NameAll.COMMAND_SET_ATTACK || sn.CommandSet == NameAll.COMMAND_SET_BATTLE_SKILL))
        {
            if (sn.RangeXYMin == 1)
            {
                return NameAll.PROJECTILE_LIMITED;
            }
            else
            {
                return NameAll.PROJECTILE_ATTACK;
            }
        }
        return NameAll.PROJECTILE_NONE;
    }

    //called in ResolveSpellAction
    static int CheckForGolem(SpellName sn, int effect, int targetTeamId, int targetId)
    {
        if ( StatusManager.Instance.IsGolem(targetTeamId) &&  IsGolemAttack(sn))
        {
            effect = StatusManager.Instance.DecrementGolem(effect, targetTeamId, targetId);
        }
        return effect;
    }

    //called in resolveSpellAction, attack (no spell gun) jump and throw, if it is the effect is run through SM's golem modification
    static bool IsGolemAttack(SpellName sn)
    {
        //Debug.Log("checking for golem 1");
        if (sn.Version != NameAll.VERSION_CLASSIC)
        {
            //Debug.Log("checking for golem 2");
            if (sn.PMType == NameAll.SPELL_TYPE_PHYSICAL && sn.StatType == NameAll.STAT_TYPE_HP && sn.RemoveStat == NameAll.REMOVE_STAT_REMOVE)
            {
                //Debug.Log("checking for golem 3");
                return true;
            }
        }
        else
        {
            if ((sn.CommandSet == NameAll.COMMAND_SET_ATTACK && sn.SpellId != NameAll.SPELL_INDEX_ATTACK_MAGIC_GUN)
            || sn.CommandSet == NameAll.COMMAND_SET_JUMP || sn.CommandSet == NameAll.COMMAND_SET_THROW)
            {
                return true;
            }

        }
        return false;
    }

    #endregion

    #region CheckForReaction and related functions
    //called in resolvespellaction, also adds to combat log
    static void CheckForReaction(SpellName sn, PlayerUnit actor, PlayerUnit target, bool receivedDamage, int renderMode, int effect = 0)
    {
        
        int abilityId = actor.AbilityReactionCode;
        int actorId = actor.TurnOrder;
        int reactChance = actor.GetReactionNumber();
        int reactionType = GetReactionType(abilityId, sn); //Debug.Log("reaction type is " + reactionType);
        bool isCreateReaction = false;

		bool isRollSuccessful = PlayerManager.Instance.IsRollSuccess(reactChance, 1, 100, NameAll.NULL_INT, NameAll.COMBAT_LOG_SUBTYPE_REACTION_BRAVE_ROLL,
			sn, actor, target);

        string combatLogString = "";
        if (!isRollSuccessful) //pre roll it, not really the proper way but saves a lot of checks
        {
            combatLogString = "reaction roll failed " + reactChance;
        }
        else
        {
            //damage triggered
            //can't be self damage
            if (reactionType == NameAll.REACTION_TYPE_DAMAGE)
            {
                if (receivedDamage && actorId != target.TurnOrder)
                {
                    isCreateReaction = true;
                }
            }
            else if (reactionType == NameAll.REACTION_TYPE_CRITICAL)
            {
                //Debug.Log("in critical status reaction ability");
                //critical and received damage
                if (StatusManager.Instance.IfStatusByUnitAndId(actorId, NameAll.STATUS_ID_CRITICAL, true) && actorId != target.TurnOrder)
                {
                    isCreateReaction = true;
                }
            }
            else if (reactionType == NameAll.REACTION_TYPE_ENEMY_TARGET)
            {
                if (actor.TeamId != target.TeamId)
                {
                    isCreateReaction = true;
                }

            }
            else if (reactionType == NameAll.REACTION_TYPE_ALLY_TARGET)
            {
                if (actor.TeamId == target.TeamId)
                {
                    isCreateReaction = true;
                }
            }
            else if (reactionType == NameAll.REACTION_TYPE_ANY_TARGET)
            {
                isCreateReaction = true;
            }
            else if (reactionType == NameAll.REACTION_TYPE_MAGIC_SPELL)
            {
                if (sn.PMType == NameAll.SPELL_TYPE_MAGICAL && sn.MP > 0)
                {
                    isCreateReaction = true;
                }
            }
            else if (reactionType == NameAll.REACTION_TYPE_DISTRIBUTE && !receivedDamage) //distribute, guess it could be applied to other types in the future
            {
                //Debug.Log(" reaction type is distribute"); effect += 1;
                if (effect > 0)
                {
                    //Debug.Log(" reaction type is distribute, effect is " + effect);
                    isCreateReaction = true;
                }

            }
            else if (reactionType == 2)
            {
                //countergrasp
                if (sn.CounterType == 2 || sn.CounterType == 16) //2 for countergrasp, 16 for counter grasp and coutnerflood
                {
                    isCreateReaction = true;
                }
            }
            else if (reactionType == 4 || reactionType == 5) //doing counter magic here for now, not sure if i want to get into this
            {
                //Debug.Log("in reaction type 4");
                //ability uses mp, counter magic, face up, mp absorb
                if (sn.MP > 0 && actorId != target.TurnOrder)
                {
                    if (reactionType == 5)
                    {
                        if (sn.CommandSet == NameAll.COMMAND_SET_SUMMON_MAGIC || (sn.IgnoresDefense == 1 && sn.Index != NameAll.SPELL_INDEX_QUICK)
                            || sn.Index == NameAll.SPELL_INDEX_BOLT_4 || sn.Index == NameAll.SPELL_INDEX_FIRE_4 || sn.Index == NameAll.SPELL_INDEX_ICE_4)
                        {

                        }
                        else
                        {
                            isCreateReaction = true;
                        }
                    }
                    else
                    {
                        isCreateReaction = true;
                    }

                }
            }
            else if (reactionType == 6)
            {
                //counterflood
                if (sn.CounterType == 6 || sn.CounterType == 16) //2 for countergrasp, 16 for counter grasp and coutnerflood
                {

                    isCreateReaction = true;
                }
            }


            if (isCreateReaction)
            {
                CreateSpellReaction(abilityId, actor, target, sn, effect);
                //CheckForReactionRoll(reactChance, abilityId, actor, target, sn, effect, combatLogString); //don't need out I think
            }
        }

        if( combatLogString.Length > 1)
        {
            CombatLogClass clObject = new CombatLogClass(combatLogString, actor.TurnOrder, renderMode);
            clObject.SendNotification();
        }
        

        if (bugTest == 1)
        {
            Debug.Log("reaction check: " + combatLogString);
        }
    }

    //used in CheckForReaction
    static int GetReactionType(int abilityId, SpellName sn)
    {
        int version = sn.Version; //Debug.Log(" ability Id is " + abilityId);
        if (abilityId == 0)
        {
            return 0;
        }

        if (version != NameAll.VERSION_CLASSIC)
        {
            //public static readonly int REACTION_RETURN_DAMAGE = 129;
            //public static readonly int REACTION_AGI_BOOST = 133;
            //public static readonly int REACTION_LEG_SALVE = 121;
            //public static readonly int REACTION_HEALING_SALVE = 120;
            //public static readonly int REACTION_STRIKE_BACK = 124;
            //public static readonly int REACTION_SKL_BOOST = 123;
            //public static readonly int REACTION_STR_BOOST = 125;
            //public static readonly int REACTION_CRG_BOOST = 127;
            //public static readonly int REACTION_SLOW_HEAL = 134;
            //public static readonly int REACTION_WIS_BOOST = 201;
            //public static readonly int REACTION_INT_BOOST = 117;
            if (abilityId == NameAll.REACTION_INT_BOOST || abilityId == NameAll.REACTION_WIS_BOOST
                || abilityId == NameAll.REACTION_SLOW_HEAL || abilityId == NameAll.REACTION_CRG_BOOST
                || abilityId == NameAll.REACTION_STR_BOOST || abilityId == NameAll.REACTION_SKL_BOOST
                || abilityId == NameAll.REACTION_STRIKE_BACK || abilityId == NameAll.REACTION_HEALING_SALVE
                || abilityId == NameAll.REACTION_LEG_SALVE || abilityId == NameAll.REACTION_AGI_BOOST
                || abilityId == NameAll.REACTION_RETURN_DAMAGE || abilityId == NameAll.REACTION_ENCORE
                )
            {
                return NameAll.REACTION_TYPE_DAMAGE;
            }
            else if (abilityId == NameAll.REACTION_DOOMSAYER || abilityId == NameAll.REACTION_BREAK_A_LEG ||
                abilityId == NameAll.REACTION_GUARD || abilityId == NameAll.REACTION_REBUTTAL )
            {
                //public static readonly int REACTION_DOOMSAYER = 116;
                //public static readonly int REACTION_BREAK_A_LEG = 119;
                //public static readonly int REACTION_GUARD = 131;
                //public static readonly int REACTION_REBUTTAL = 122;
                //public static readonly int REACTION_ENCORE = 118;
                return NameAll.REACTION_TYPE_ENEMY_TARGET;
            }
            else if (abilityId == NameAll.REACTION_LAST_STAND || abilityId == NameAll.REACTION_RESTORE_MANA ||
                abilityId == NameAll.REACTION_RESTORE_HP || abilityId == NameAll.REACTION_SHORTCUT)
            {
                //public static readonly int REACTION_LAST_STAND = 126;
                //        public static readonly int REACTION_RESTORE_MANA = 112;
                //public static readonly int REACTION_RESTORE_HP = 114;
                //public static readonly int REACTION_SHORTCUT = 135;
                return NameAll.REACTION_TYPE_CRITICAL;
            }
            else if (abilityId == NameAll.REACTION_RETURN_SPELL)
            {
                //public static readonly int REACTION_RETURN_SPELL = 113;
                return NameAll.REACTION_TYPE_MAGIC_SPELL;
            }
            else if (abilityId == NameAll.REACTION_THRIFTY_HEAL)
            {
                return NameAll.REACTION_TYPE_DISTRIBUTE;
                //public static readonly int REACTION_THRIFTY_HEAL = 115;
            }
        }
        else
        {
            if (abilityId == NameAll.REACTION_COUNTER || abilityId == NameAll.REACTION_SPEED_SAVE || abilityId == NameAll.REACTION_REGENERATOR ||
                            abilityId == NameAll.REACTION_MA_SAVE || abilityId == NameAll.REACTION_CAUTION || abilityId == NameAll.REACTION_AUTO_POTION ||
                            abilityId == NameAll.REACTION_DAMAGE_SPLIT || abilityId == NameAll.REACTION_A_SAVE)
            {
                return 1;
            }
            else if (abilityId == NameAll.REACTION_MP_RESTORE || abilityId == NameAll.REACTION_CRITICAL_QUICK ||
                   abilityId == NameAll.REACTION_HP_RESTORE || abilityId == NameAll.REACTION_MEATBONE_SLASH)
            {
                return 3;
            }
            else if (abilityId == NameAll.REACTION_COUNTER_TACKLE || abilityId == NameAll.REACTION_BRAVE_UP
                || abilityId == NameAll.REACTION_DRAGON_SPIRIT)
            {
                return 2;
            }
            else if (abilityId == NameAll.REACTION_FACE_UP || abilityId == NameAll.REACTION_ABSORB_USED_MP)
            {
                return 4;
            }
            else if (abilityId == NameAll.REACTION_COUNTER_MAGIC)
            {
                return 5;
            }
            else if (abilityId == NameAll.REACTION_COUNTER_FLOOD)
            {
                return 6;
            }
            else if( abilityId == NameAll.REACTION_DISTRIBUTE)
            {
                //Debug.Log(" ability Id is " + abilityId);
                return NameAll.REACTION_TYPE_DISTRIBUTE;
            }
        }

        return 0;
    }

    //used in CheckForReaction
    static void CreateSpellReaction(int abilityId, PlayerUnit actor, PlayerUnit target, SpellName sn, int zEffect)
    {
        SpellName tempSN;
        int spellIndex = NameAll.NULL_INT;
        int effect = zEffect; //defaults to 0 in the ResolveSpellAction code
        bool isHitsActor = false; //does the reaction target the actor (doing hte reaction) or the target (unit who caused the reaction

        //get the spellIndex based on the abilityId
        if (sn.Version != NameAll.VERSION_CLASSIC)
        {
            //public static readonly int REACTION_RESTORE_MANA = 112;
            //public static readonly int REACTION_RETURN_SPELL = 113;
            //public static readonly int REACTION_RESTORE_HP = 114;
            //public static readonly int REACTION_THRIFTY_HEAL = 115;
            //public static readonly int REACTION_DOOMSAYER = 116;
            //public static readonly int REACTION_INT_BOOST = 117;
            //public static readonly int REACTION_ENCORE = 118;
            //public static readonly int REACTION_BREAK_A_LEG = 119;
            //public static readonly int REACTION_HEALING_SALVE = 120;
            //public static readonly int REACTION_LEG_SALVE = 121;
            //public static readonly int REACTION_REBUTTAL = 122;
            //public static readonly int REACTION_SKL_BOOST = 123;
            //public static readonly int REACTION_STRIKE_BACK = 124;
            //public static readonly int REACTION_STR_BOOST = 125;
            //public static readonly int REACTION_LAST_STAND = 126;
            //public static readonly int REACTION_CRG_BOOST = 127;
            //public static readonly int REACTION_DEFLECT_FAR = 128;
            //public static readonly int REACTION_RETURN_DAMAGE = 129;
            //public static readonly int REACTION_DEFLECT_CLOSE = 130;
            //public static readonly int REACTION_GUARD = 131;
            //public static readonly int REACTION_HP_TO_MP = 132;
            //public static readonly int REACTION_AGI_BOOST = 133;
            //public static readonly int REACTION_SLOW_HEAL = 134;
            //public static readonly int REACTION_SHORTCUT = 135;
            //public static readonly int REACTION_WIS_BOOST = 201;
            if (abilityId == NameAll.REACTION_RESTORE_MANA)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_RESTORE_MANA;
            }
            else if (abilityId == NameAll.REACTION_WIS_BOOST)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_WIS_BOOST;
            }
            else if (abilityId == NameAll.REACTION_SHORTCUT)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_SHORTCUT;
            }
            else if (abilityId == NameAll.REACTION_SLOW_HEAL)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_SLOW_HEAL;
            }
            else if (abilityId == NameAll.REACTION_AGI_BOOST)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_AGI_BOOST;
            }
            else if (abilityId == NameAll.REACTION_GUARD)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_GUARD;
            }
            else if (abilityId == NameAll.REACTION_CRG_BOOST)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_CRG_BOOST;
            }
            else if (abilityId == NameAll.REACTION_LAST_STAND)
            {
                //isHitsActor = true; //intentional, not srue why
                spellIndex = NameAll.SPELL_INDEX_REACTION_LAST_STAND;
            }
            else if (abilityId == NameAll.REACTION_STR_BOOST)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_STR_BOOST;
            }
            else if (abilityId == NameAll.REACTION_STRIKE_BACK)
            {
                //Debug.Log("Add new code for reaction strike back");
                spellIndex = GetCounterSpellIndex(actor, target); //returns NULL INT
            }
            else if (abilityId == NameAll.REACTION_SKL_BOOST)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_SKL_BOOST;
            }
            else if (abilityId == NameAll.REACTION_REBUTTAL)
            {
                if (sn.PMType == NameAll.SPELL_TYPE_PHYSICAL)
                {
                    spellIndex = NameAll.SPELL_INDEX_REACTION_REBUTTAL_CRG;
                }
                else if (sn.PMType == NameAll.SPELL_TYPE_MAGICAL)
                {
                    spellIndex = NameAll.SPELL_INDEX_REACTION_REBUTTAL_WIS;
                }
                else if (sn.PMType == NameAll.SPELL_TYPE_NEUTRAL)
                {
                    spellIndex = NameAll.SPELL_INDEX_REACTION_REBUTTAL_SKL;
                }
            }
            else if (abilityId == NameAll.REACTION_LEG_SALVE)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_LEG_SALVE;
            }
            else if (abilityId == NameAll.REACTION_HEALING_SALVE)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_HEALING_SALVE;
            }
            else if (abilityId == NameAll.REACTION_BREAK_A_LEG)
            {
                spellIndex = NameAll.SPELL_INDEX_REACTION_BREAK_A_LEG;
            }
            else if (abilityId == NameAll.REACTION_ENCORE)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_ENCORE;
            }
            else if (abilityId == NameAll.REACTION_INT_BOOST)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_INT_BOOST;
            }
            else if (abilityId == NameAll.REACTION_DOOMSAYER)
            {
                //isHitsActor = true; //not sure why this is the opposite
                spellIndex = NameAll.SPELL_INDEX_REACTION_DOOMSAYER;
            }
            else if (abilityId == NameAll.REACTION_THRIFTY_HEAL)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_THRIFTY_HEAL;
            }
            else if (abilityId == NameAll.REACTION_RESTORE_HP)
            {
                isHitsActor = true;
                spellIndex = NameAll.SPELL_INDEX_REACTION_RESTORE_HP;
            }
            else if (abilityId == NameAll.REACTION_RETURN_SPELL)
            {
                //use this equality for unit or team if you want to turn off return spell on a unit casting it on itself
                //if( actor.TurnOrder != target.TurnOrder)
                //{

                //}
                //for direction based spell, having them start at the actor
                if (!sn.IsDirectionIndependent())
                    isHitsActor = true;

                spellIndex = sn.Index; //assuming they all work
            }
            else if (abilityId == NameAll.REACTION_RETURN_DAMAGE)
            {
                spellIndex = NameAll.SPELL_INDEX_REACTION_RETURN_DAMAGE; //assuming they all work
            }
        }
        else
        {
            
            if (abilityId == NameAll.REACTION_ABSORB_USED_MP)
            {
                //basically ether except the baseQ value gets modified when the SN is created
                spellIndex = 196;
                effect = GetMPCost(actor, sn);
                isHitsActor = true;
            }
            else if (abilityId == NameAll.REACTION_A_SAVE)
            {
                spellIndex = 0; //accumulate
                isHitsActor = true;
                return;
            }
            else if (abilityId == NameAll.REACTION_AUTO_POTION)
            {
                if (actor.Level <= 15)
                {
                    spellIndex = 146;
                }
                else if (actor.Level <= 30)
                {
                    spellIndex = 147;
                }
                else
                {
                    spellIndex = 148;
                }
                isHitsActor = true;
            }
            else if (abilityId == NameAll.REACTION_BRAVE_UP)
            {
                spellIndex = 197;
                isHitsActor = true;
            }
            else if (abilityId == NameAll.REACTION_CAUTION)
            {
                spellIndex = 198;
                isHitsActor = true;
            }
            else if (abilityId == NameAll.REACTION_COUNTER)
            {
                //Debug.Log("Add new code for reaction counter");
                spellIndex = GetCounterSpellIndex(actor, target); //returns NULL INT on fail
            }
            else if (abilityId == NameAll.REACTION_COUNTER_FLOOD)
            {
                //come back to this when elemental is implemented
                //basically uses actor's maptileindex to determine which spell index to use
                //HOWEVER NO RANGE CONSIDERATION SO SLIGHTLY DIFFERENT THAN COUNTER FLOOD
                spellIndex = NameAll.SPELL_INDEX_HELL_IVY;
            }
            else if (abilityId == NameAll.REACTION_COUNTER_MAGIC)
            {
                spellIndex = sn.Index; //assuming they all work
            }
            else if (abilityId == NameAll.REACTION_COUNTER_TACKLE)
            {
                //sees if in range of dash (spellIndex 1)
                tempSN = SpellManager.Instance.GetSpellNameByIndex(1);
                //Debug.Log("Add new code for dash reaction");
                if (MapTileManager.Instance.IsTileInAttackRange(actor, target, tempSN))
                {
                    spellIndex = 1; //dash
                }
            }
            else if (abilityId == NameAll.REACTION_CRITICAL_QUICK)
            {
                spellIndex = 199;
                isHitsActor = true;
            }
            else if (abilityId == NameAll.REACTION_DAMAGE_SPLIT)
            {
                spellIndex = 200; //effect adds the damage
            }
            else if (abilityId == NameAll.REACTION_DISTRIBUTE)
            {
                //effect is the amount healed, targetting all units (even though the unit itself is already healed)
                //unsure about undead reversal
                //effect is modified: is divided between all units not at full hp
                //Debug.Log("effect is" + effect);
                List<PlayerUnit> tempList = PlayerManager.Instance.GetAllUnitsByTeamId(target.TeamId);
                int zCount = 0;
                foreach (PlayerUnit pu in tempList)
                {
                    if (pu.StatTotalLife != pu.StatTotalMaxLife)
                    {
                        zCount += 1;
                    }
                }
                if (zCount > 0)
                {
                    effect = (effect + zCount - 1) / zCount;//not sure why but the effectg is 1 too high, could subtract one
                    //Debug.Log("effect is" + effect + " " + zCount);
                    spellIndex = 201;
                }

            }
            else if (abilityId == NameAll.REACTION_DRAGON_SPIRIT)
            {
                spellIndex = 202;
                isHitsActor = true;
            }
            else if (abilityId == NameAll.REACTION_FACE_UP)
            {
                spellIndex = 203;
                isHitsActor = true;
            }
            else if (abilityId == NameAll.REACTION_HAMEDO)
            {
                spellIndex = GetCounterSpellIndex(actor, target); //returns NULL_INT on fail

            }
            else if (abilityId == NameAll.REACTION_HP_RESTORE)
            {
                spellIndex = 204;
                isHitsActor = true;
            }
            else if (abilityId == NameAll.REACTION_MA_SAVE)
            {
                spellIndex = 205;
                isHitsActor = true;
            }
            else if (abilityId == NameAll.REACTION_MEATBONE_SLASH)
            {
                //enemy within weapon range
                int tempSpellIndex = CalculationAT.GetAttackSpellIndex(actor.TurnOrder);
                tempSN = SpellManager.Instance.GetSpellNameByIndex(tempSpellIndex);
                //Debug.Log("Add new code for meatboneslash");
                if (MapTileManager.Instance.IsTileInAttackRange(actor, target, tempSN))
                {
                    spellIndex = 207;
                    effect = actor.StatTotalMaxLife;
                }
            }
            else if (abilityId == NameAll.REACTION_MP_RESTORE)
            {
                spellIndex = 208;
                isHitsActor = true;
                //effect = actor.StatTotalMaxMP;
            }
            else if (abilityId == NameAll.REACTION_REGENERATOR)
            {
                spellIndex = 209;
                isHitsActor = true;
            }
            else if (abilityId == NameAll.REACTION_SPEED_SAVE)
            {
                spellIndex = 206;
                isHitsActor = true;
            }
        }


        if (spellIndex != NameAll.NULL_INT)
        {
            SpellReaction sr;
            if (isHitsActor)
                sr = new SpellReaction(actor.TurnOrder, spellIndex, actor.TurnOrder, effect);
            else
                sr = new SpellReaction(actor.TurnOrder, spellIndex, target.TurnOrder, effect);
            SpellManager.Instance.AddSpellReaction(sr);
        }

    }

    //used in Reaction Counter creator, returns null int to tell not to make a SpellReaction
    private static int GetCounterSpellIndex(PlayerUnit actor, PlayerUnit target)
    {
        int tempSpellIndex = CalculationAT.GetAttackSpellIndex(actor.TurnOrder);
        SpellName tempSN = SpellManager.Instance.GetSpellNameByIndex(tempSpellIndex);
        if (MapTileManager.Instance.IsTileInAttackRange(actor, target, tempSN))
        {
            return tempSpellIndex;
        }
        return NameAll.NULL_INT;
    }


    //some on move effect spells are similar to SpellReactions, those are created here
    public static void CreateSpellReactionMove(int classId, int abilityId, int actorId, int effect = 0)
    {
        int spellIndex = NameAll.NULL_INT;
        //Move WIS Up
        //Saint's Footsteps
        //Blessed Steps
        //Raise the Dead
        //Draw Attention
        //Walk it On
        //Silence the Crowd
        //MP Walk
        //Move CRG Up
        //HP Walk
        //Move SKL Up
        //TP Walk
        //Walk it Off

        if (NameAll.IsClassicClass(classId))
        {
            if (abilityId == NameAll.MOVEMENT_MOVE_HP_UP)
            {
                spellIndex = 210;
            }
            else if (abilityId == NameAll.MOVEMENT_MOVE_MP_UP)
            {
                spellIndex = 211;
            }
        }
        else
        {
            if (abilityId == NameAll.MOVEMENT_MOVE_WIS_UP)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_MOVE_WIS_UP;
            }
            else if (abilityId == NameAll.MOVEMENT_SAINTS_FOOTSTEPS)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_SAINTS_FOOTSTEPS;
            }
            else if (abilityId == NameAll.MOVEMENT_MOVE_SKL_UP)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_MOVE_SKL_UP;
            }
            else if (abilityId == NameAll.MOVEMENT_TP_WALK)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_TP_WALK;
            }
            else if (abilityId == NameAll.MOVEMENT_WALK_IT_OFF)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_WALK_IT_OFF;
            }
            else if (abilityId == NameAll.MOVEMENT_HP_WALK)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_HP_WALK;
            }
            else if (abilityId == NameAll.MOVEMENT_MOVE_CRG_UP)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_MOVE_CRG_UP;
            }
            else if (abilityId == NameAll.MOVEMENT_MP_WALK)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_MP_WALK;
            }
            else if (abilityId == NameAll.MOVEMENT_BLESSED_STEPS)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_BLESSED_STEPS;
            }
            else if (abilityId == NameAll.MOVEMENT_RAISE_THE_DEAD)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_RAISE_THE_DEAD;
            }
            else if (abilityId == NameAll.MOVEMENT_DRAW_ATTENTION)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_DRAW_ATTENTION;
            }
            else if (abilityId == NameAll.MOVEMENT_WALK_IT_ON)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_WALK_IT_ON;
            }
            else if (abilityId == NameAll.MOVEMENT_SILENCE_THE_CROWD)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_SILENCE_THE_CROWD;
            }
            else if (abilityId == NameAll.MOVEMENT_STRETCH_LEGS && effect == 1)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_STRETCH_LEGS;
                effect = 0; //effect used in sr for other things, just using it here to trigger this ability
            }
            else if (abilityId == NameAll.MOVEMENT_UNSTABLE_TP && effect > 0)
            {
                spellIndex = NameAll.SPELL_INDEX_MOVE_UNSTABLE_TP;
                //effect is multiplied by the baseQ to determine the dmg
            }
        }


        if (spellIndex != NameAll.NULL_INT)
        {
            SpellReaction sr = new SpellReaction(actorId, spellIndex, actorId, effect);
            SpellManager.Instance.AddSpellReaction(sr);
        }
    }

    #endregion

    #region misc
    static string BuildCombatLogString(PlayerUnit actor, PlayerUnit target, SpellName sn, int phase, int renderMode)
    {
		if (renderMode == NameAll.PP_RENDER_NONE)
			return "";

		string zString = "";
        if (phase == 1)
        {
            zString += actor.UnitName + " casts " + sn.AbilityName + " on " + target.UnitName;
        }

        return zString;
    }

    static string AppendCombatLogString(string clString, List<int> outEffect, int roll, SpellName sn, int phase, int renderMode)
    {
		if (renderMode == NameAll.PP_RENDER_NONE)
			return clString;

        if (phase == 1)
        {
            clString += " roll is " + outEffect[0] + "(" + roll + ")";
        }
        else if (phase == 2)
        {
            string statHit = "";
            int hitsStat = sn.HitsStat;
            if (hitsStat > 0)
            {
                statHit = NameAll.GetStatTypeString(sn.StatType);

                string statSign;
                int removeStat = sn.RemoveStat;
                if (removeStat == 1) //does damage
                {
                    statSign = "-";
                }
                else if (removeStat == 0) //adds the stat
                {
                    statSign = "+";
                }
                else //absorbs the stat
                {
                    statSign = "+/-";
                }
                clString += "Effect: " + statSign + outEffect[1] + " " + statHit + ",";
            }
            else if (hitsStat == 0)
            {
                //zString += "Status Hit %: " + outEffect[0] + ",";
                clString += "Add Status: " + sn.StatusType + ",";
            }
        }
        else if (phase == 3)
        {

        }
        return clString;
    }

	static string AppendCombatLogString(string clString, string stringToAppend, int renderMode)
	{
		if(renderMode != NameAll.PP_RENDER_NONE)
		{
			clString = clString += stringToAppend;
		}
		return clString;
	}

    #endregion

}




