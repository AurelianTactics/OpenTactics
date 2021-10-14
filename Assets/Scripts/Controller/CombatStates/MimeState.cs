using UnityEngine;
using System.Collections;

public class MimeState : CombatState
{
    bool isOffline;

    public override void Enter()
    {
        base.Enter();
        isOffline = PlayerManager.Instance.IsOfflineGame();
        StartCoroutine(DoPhase());
    }

    public override void Exit()
    {
        base.Exit();
    }

    IEnumerator DoPhase()
    {
        SpellReaction sr = SpellManager.Instance.GetNextMimeQueue();
        if (sr != null)
        {
            yield return new WaitForSeconds(0.5f); //on fast action like attacks, reach this state really quickly

            SpellName sn = SpellManager.Instance.GetSpellNameByIndex(sr.SpellIndex);
            CombatTurn ssTurn = new CombatTurn(sr, board,isMimeSpellReaction:true);
            HighlightActorTile(ssTurn.actor, true);
            owner.battleMessageController.Display("Mimic: " + ssTurn.spellName.AbilityName);
            board.SelectTiles(ssTurn.targets);

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
                SpellManager.Instance.SendReactionDetails(sr.ActorId, sr.SpellIndex, targetX, targetY, "Mimic: " + sn.AbilityName);
            }

            yield return new WaitForSeconds(0.5f);

           
            //modify the spellName for reaction
            ssTurn.spellName = CalculationAT.ModifyReactionSpellName(sr, ssTurn.actor.TurnOrder);

			owner.calcMono.DoFastAction(board, ssTurn, isActiveTurn: false, isReaction: false, isMime: true, renderMode: owner.renderMode);
			//yield return StartCoroutine( owner.calcMono.DoFastActionInner(board, ssTurn, isActiveTurn: false, isSlowActionPhase: false, isReaction: false, isMime: true) );
			SpellManager.Instance.RemoveMimeQueueByObject(sr);
            HighlightActorTile(ssTurn.actor, false); //unhighlight
            board.DeSelectTiles(ssTurn.targets); //deselect the tiles
        }

        yield return null;
        //if (IsBattleOver())
        //{
        //    owner.ChangeState<CombatCutSceneState>();
        //}
        //else
        //{
        //    owner.ChangeState<GameLoopState>();
        //}
        owner.ChangeState<GameLoopState>();
    }

  
}
