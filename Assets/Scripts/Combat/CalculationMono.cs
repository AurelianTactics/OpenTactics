using UnityEngine;
using System.Collections;

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

    //public void DoFastAction(Board board, CombatTurn turn, bool isActiveTurn, bool isSlowAction, bool isReaction, bool isMime)
    //{
    //    //Debug.Log("reached here");
    //    //if( board == null)
    //    //{
    //    //    Debug.Log("board is null");
    //    //}
    //    //if( turn == null)
    //    //{
    //    //    Debug.Log("turn is null");
    //    //}
    //    StartCoroutine(DoFastActionInner(board,turn,isActiveTurn,isSlowAction,isReaction,isMime));
    //}

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
            if (io.OnHitChance > 0 && CalculationResolveAction.RollForSuccess(io.OnHitChance))
            {
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

}
