using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Does AI for units. If unit's driver is set to AI or has an AI status, CombatTurn.plan is created using the logic here

public class CombatComputerPlayer : MonoBehaviour 
{
	#region Fields
	CombatController cc;
    PlayerUnit actor;
    PlayerUnit nearestFoe;
    Board board;
    SpellNameAI snai; //helps the AI know what type of spell it is (ie target enemies, allies etc)
    List<PlayerUnit> puList; //list filled with high priority targets like enemies less than 50% and allies that need to be cured/revived

    //BattleController bc;
	//Unit actor { get { return bc.turn.actor; }}
	//Alliance alliance { get { return actor.GetComponent<Alliance>(); }}
	//Unit nearestFoe;
	#endregion
	
	#region MonoBehaviour
	void Start ()
	{
        //bc = GetComponent<BattleController>();
        cc = GetComponent<CombatController>();
        board = cc.board;
	}
	#endregion
	
	#region Public
	public CombatPlanOfAttack Evaluate ()
	{
        //Debug.Log("testing, reached here 0");
        actor = cc.turn.actor;
        CombatPlanOfAttack poa = new CombatPlanOfAttack();

        bool isChicken = StatusManager.Instance.IfStatusByUnitAndId(actor.TurnOrder, NameAll.STATUS_ID_CHICKEN);
        if(isChicken || cc.turn.hasUnitActed)
        {
            poa.spellName = null;
            MoveAwayOpponent(poa);
            return poa;
        }

        CombatRandomAbilityPicker rap = new CombatRandomAbilityPicker();

        bool isBerserk = StatusManager.Instance.IfStatusByUnitAndId(actor.TurnOrder, NameAll.STATUS_ID_BERSERK);
        bool isConfusion = StatusManager.Instance.IfStatusByUnitAndId(actor.TurnOrder, NameAll.STATUS_ID_CONFUSION);
        bool isCharm = StatusManager.Instance.IfStatusByUnitAndId(actor.TurnOrder, NameAll.STATUS_ID_CHARM);

        if( isBerserk || isConfusion || isCharm)
        {
            poa = rap.PickAIStatus(poa, actor, isBerserk,isConfusion,isCharm); //Debug.Log("is berserk, doing a berserk turn");
        }
        else
        {
            int actorCharmTeam = actor.GetCharmTeam();

            //try to kill a hurt enemy
            puList = PlayerManager.Instance.GetAIList(actorCharmTeam, NameAll.AI_LIST_HURT_ENEMY);// Debug.Log("testing whether to target hurt enemy: list count is " + puList.Count);
            if( puList.Count > 0)
            {
                int zCount = 0; //Debug.Log("targetting a hurt enemy " + puList.Count);
                do
                {
                    //Debug.Log("targetting a hurt enemy " + puList.Count);
                    poa = rap.PickEnemyHurt(poa, actor, zCount); //attack, first dmg ability, 2nd dmg ability
                    poa = EvaluateDirectionMovePUList(poa, puList);
                    zCount += 1;
                } while (poa.spellName == null && zCount < 3 );

                if (poa.spellName != null)
                    return poa;
                else
                {
                    //Debug.Log("failed to choose an ability that hurts enemies");
                }
                    
            }
            //try to revive a fallen ally
            if (actor.isReviveSpell)
            {
                puList = PlayerManager.Instance.GetAIList(actorCharmTeam, NameAll.AI_LIST_DEAD_ALLY);
                if (puList.Count > 0)
                {
                    //Debug.Log("targetting a dead player " + puList.Count);
                    poa = rap.PickAllyDead(poa, actor);
                    poa = EvaluateDirectionMovePUList(poa, puList);
                }
                if (poa.spellName != null)
                    return poa;
            }
            //try to cure a hurt ally
            if (actor.isCureSpell)
            {
                puList = PlayerManager.Instance.GetAIList(actorCharmTeam, NameAll.AI_LIST_HURT_ALLY);
                if (puList.Count > 0)
                {
                    //Debug.Log("targetting a hurt ally " + puList.Count);
                    poa = rap.PickAllyHurt(poa, actor);
                    poa = EvaluateDirectionMovePUList(poa, puList);
                }
                if (poa.spellName != null)
                    return poa;
            }

            //none of the above conditions apply, pick a random ability
            poa = rap.PickRandom(poa, actor);

        }

        poa = EvaluateDirectionMove(poa);
        return poa;
        //snai = new SpellNameAI(poa.spellName);
        ////AttackPattern pattern = actor.GetComponentInChildren<AttackPattern>();
        ////if (pattern)
        ////	pattern.Pick(poa);
        ////else
        ////	DefaultAttackPattern(poa);

        //poa.POASummary();

        //if (IsPositionIndependent(poa))
        //{
        //    PlanPositionIndependent(poa); Debug.Log("here a");
        //}   
        //else if (IsDirectionIndependent(poa))
        //{
        //    PlanDirectionIndependent(poa); Debug.Log("here b");
        //}
        //else
        //{
        //    PlanDirectionDependent(poa); Debug.Log("here c");
        //}

        //poa.POASummary();

        //if (poa.spellName == null)
        //    MoveTowardOpponent(poa);
        //else if (poa.moveLocation == board.GetTile(actor).pos) //if actor's moveLocation is the same as the current tile, actor moves second
        //{
        //    poa.isActFirst = true; 
        //    MoveAwayOpponent(poa);
        //}

        //return poa;
	}

    private CombatPlanOfAttack EvaluateDirectionMove(CombatPlanOfAttack poa)
    {
        snai = new SpellNameAI(poa.spellName);

        //AttackPattern pattern = actor.GetComponentInChildren<AttackPattern>();
        //if (pattern)
        //	pattern.Pick(poa);
        //else
        //	DefaultAttackPattern(poa);

        //poa.POASummary();

        if (IsPositionIndependent(poa))
        {
            //Debug.Log("ability IsPosition Independent");
            PlanPositionIndependent(poa);
            MoveAwayOpponent(poa);
            return poa;
        }
        else if (IsDirectionIndependent(poa))
        {
            //Debug.Log("ability isDirection Independent");
            PlanDirectionIndependent(poa, null);
        }
        else
        {
            //Debug.Log("ability isDirection Dependent");
            PlanDirectionDependent(poa, null);
        }

        //poa.POASummary();

        if (poa.spellName == null)
            MoveTowardOpponent(poa);
        else if (poa.moveLocation == board.GetTile(actor).pos) //if actor's moveLocation is the same as the current tile, actor moves second
        {
            poa.isActFirst = true;
            MoveAwayOpponent(poa);
        }

        return poa;
    }

    private CombatPlanOfAttack EvaluateDirectionMovePUList(CombatPlanOfAttack poa, List<PlayerUnit> puList)
    {
        snai = new SpellNameAI(poa.spellName);
        //Debug.Log("in PU List, evaluating direction move");
        //AttackPattern pattern = actor.GetComponentInChildren<AttackPattern>();
        //if (pattern)
        //	pattern.Pick(poa);
        //else
        //	DefaultAttackPattern(poa);

        //poa.POASummary();

        if (IsPositionIndependent(poa))
        {
            //Debug.Log("ability IsPosition Independent");
            PlanPositionIndependent(poa);
            MoveAwayOpponent(poa);
            return poa;
        }
        else if (IsDirectionIndependent(poa))
        {
            //Debug.Log("ability isDirection Independent");
            PlanDirectionIndependent(poa, puList); 
        }
        else
        {
            //Debug.Log("ability isDirection Dependent");
            PlanDirectionDependent(poa, puList); 
        }

        //poa.POASummary();

        if (poa.spellName == null)
            MoveTowardOpponent(poa);
        else if (poa.moveLocation == board.GetTile(actor).pos) //if actor's moveLocation is the same as the current tile, actor moves second
        {
            poa.isActFirst = true;
            MoveAwayOpponent(poa);
        }

        return poa;
    }

    public CombatPlanOfAttack ContinueChargingPerforming()
    {
        CombatPlanOfAttack poa = new CombatPlanOfAttack();
        poa.spellName = null; //just continues the prior action
        MoveAwayOpponent(poa); //Debug.Log("continuing to charge/perform");
        return poa;
    }
    #endregion

    #region Private
    //void DefaultAttackPattern (CombatPlanOfAttack poa)
    //{
    // Just get the first "Attack" ability
    //poa.ability = actor.GetComponentInChildren<Ability>();
    //poa.target = Targets.Foe;
    //}

    bool IsPositionIndependent(CombatPlanOfAttack poa)
    {
        //AbilityRange range = poa.ability.GetComponent<AbilityRange>();
        //return range.positionOriented == false;
        //need to do a check on poa.spellName
        //Debug.Log("testing position independint " + poa.spellName.EffectXY);
        if (poa.spellName.EffectXY >= NameAll.SPELL_EFFECT_ALLIES && poa.spellName.EffectXY <= NameAll.SPELL_EFFECT_MATH_SKILL)
        {
            return true;
        }
        

        return false;
	}
	
	bool IsDirectionIndependent (CombatPlanOfAttack poa)
	{
        //AbilityRange range = poa.ability.GetComponent<AbilityRange>();
        //return !range.directionOriented;

        if ( (poa.spellName.EffectXY >= NameAll.SPELL_EFFECT_CONE_BASE && poa.spellName.EffectXY <= NameAll.SPELL_EFFECT_CONE_MAX)
            || (poa.spellName.EffectXY >= NameAll.SPELL_EFFECT_LINE_2 && poa.spellName.EffectXY <= NameAll.SPELL_EFFECT_LINE_8) )
            return false;

        return true;
    }
	
	void PlanPositionIndependent (CombatPlanOfAttack poa)
	{
		List<Tile> moveOptions = GetMoveOptions();
		Tile tile = moveOptions[Random.Range(0, moveOptions.Count)];
		poa.moveLocation = poa.fireLocation = tile.pos;
	}
	
	void PlanDirectionIndependent (CombatPlanOfAttack poa, List<PlayerUnit> puList = null)
	{
        Tile startTile = board.GetTile(actor);//actor.tile;
        Dictionary<Tile, AttackOption> map = new Dictionary<Tile, AttackOption>();
        //AbilityRange ar = poa.ability.GetComponent<AbilityRange>();
        CombatAbilityRange car = new CombatAbilityRange();
        List<Tile> moveOptions = GetMoveOptions();

        for (int i = 0; i < moveOptions.Count; ++i)
        {
            Tile moveTile = moveOptions[i];
            //actor.Place(moveTile);
            actor.AISetUnitTile(moveTile);
            List<Tile> fireOptions = car.GetTilesInRange(board,actor,poa.spellName,actor.Dir);
            //Debug.Log("testing size of map " + map.Count);
            //Debug.Log(" tile is " + moveTile.pos.x + "," + moveTile.pos.y);
            for (int j = 0; j < fireOptions.Count; ++j)
            {
                Tile fireTile = fireOptions[j];
                AttackOption ao = null;
                if (map.ContainsKey(fireTile))
                {
                    ao = map[fireTile]; //Debug.Log("map contains fire tile");
                }
                else
                {
                    ao = new AttackOption(); //Debug.Log("map doesn't contain fire tile");
                    map[fireTile] = ao;
                    ao.target = fireTile;
                    ao.direction = actor.Dir;
                    RateFireLocation(poa, ao, puList);
                }

                ao.AddMoveTarget(moveTile);
            }
        }

        //actor.Place(startTile);
        actor.AISetUnitTile(startTile);
        List<AttackOption> list = new List<AttackOption>(map.Values);
        PickBestOption(poa, list);
    }
	
	void PlanDirectionDependent (CombatPlanOfAttack poa, List<PlayerUnit> puList = null)
	{
        Tile startTile = board.GetTile(actor);//actor.tile;
        Directions startDirection = actor.Dir;//actor.dir;
        List<AttackOption> list = new List<AttackOption>();
        List<Tile> moveOptions = GetMoveOptions(); //Debug.Log("move options count is " + moveOptions.Count);

        for (int i = 0; i < moveOptions.Count; ++i)
        {
            Tile moveTile = moveOptions[i];
            //actor.Place(moveTile);
            actor.AISetUnitTile(moveTile);

            for (int j = 0; j < 4; ++j)
            {
                actor.Dir = (Directions)j;
                AttackOption ao = new AttackOption();
                ao.target = moveTile;
                ao.direction = actor.Dir;
                RateFireLocation(poa, ao, puList);
                ao.AddMoveTarget(moveTile);
                list.Add(ao);
            }
        }

        //actor.Place(startTile);
        actor.AISetUnitTile(startTile);
        actor.Dir = startDirection;
        //poa.POASummary();
        PickBestOption(poa, list); //poa.POASummary();
    }

	bool IsAbilityTargetMatch (CombatPlanOfAttack poa, Tile tile, SpellNameAI spellNameAI)
	{
		bool isMatch = false;
		if (poa.target == Targets.Tile)
        {
            isMatch = true; //Debug.Log("is match should be true");
        }	
		else if (poa.target != Targets.None)
		{
            //Alliance other = tile.content.GetComponentInChildren<Alliance>();
            //if (other != null && alliance.IsMatch(other, poa.target))
            //	isMatch = true;
            
            isMatch = PlayerManager.Instance.IsMatch(actor.TurnOrder,tile.UnitId,poa.target); //Debug.Log("in isabilitytargetmatch isMatch is " + isMatch + " " + actor.TurnOrder + " " + tile.UnitId + " " + poa.target.ToString());
            if(isMatch) //null pu check done here
            {

                PlayerUnit target = PlayerManager.Instance.GetPlayerUnit(tile.UnitId); //Debug.Log("in isabilitytargetmatch is ableToFightCheck " + spellNameAI.isAbleToFight + " " + target.AbleToFight);
                if (spellNameAI.isAbleToFight == target.AbleToFight)
                {
                    if (spellNameAI.isAbleToFight)
                    {
                        isMatch = true; //Debug.Log("setting ismatch to true " + isMatch);
                    }
                    else
                    {
                        //check that the reason for not matching is what the ability cures (ie revive for dead, cure petrify for petrify etc
                        if (StatusManager.Instance.IfStatusCuredBySpell(spellNameAI.isAbleToFightStatusId, poa.spellName))
                            isMatch = true;
                        else
                            isMatch = false;
                    }
                }
                else
                {
                    isMatch = false;
                }
                    
            }
            //Debug.Log("testing if is match is true, is match is " + isMatch + " tile x,y are:" + tile.pos.x +"," + tile.pos.y);
        }
        //Debug.Log("returning isMatch: " + isMatch);
		return isMatch;
	}

    //does the ability target one of the PlayerUnits on a short list (list of hurt enemies, wounded allies, etc)
    bool IsAbilityTargetMatchForPlayerUnit(CombatPlanOfAttack poa, Tile tile, SpellNameAI spellNameAI, List<PlayerUnit> targetList)
    {
        bool isMatch = false;

        if (poa.target == Targets.Tile)
        {
            isMatch = true; //Debug.Log("is match should be true");
        }
        else if (poa.target != Targets.None)
        {
            foreach(PlayerUnit pu in targetList)
            {
                if( tile.UnitId == pu.TurnOrder)
                {
                    isMatch = true;
                    break;
                }
            }
        }
        return isMatch;
    }
	
	List<Tile> GetMoveOptions ()
	{
        //return actor.GetComponent<Movement>().GetTilesInRange(bc.board);
        PlayerUnitObject puo = PlayerManager.Instance.GetPlayerUnitObjectComponent(actor.TurnOrder);
        return puo.GetAITilesToMove(board, board.GetTile(actor), actor); //lets the AI select the tile it is currently on
        //return puo.GetTilesInRange(board, board.GetTile(actor), actor); //deprecated way
    }
	
	void RateFireLocation (CombatPlanOfAttack poa, AttackOption option, List<PlayerUnit> puList = null)
	{
        //AbilityArea area = poa.ability.GetComponent<AbilityArea>();
        CombatAbilityArea area = new CombatAbilityArea();
        //List<Tile> tiles = area.GetTilesInArea(bc.board, option.target.pos);
        List<Tile> tiles = area.GetTilesInArea(board, actor, poa.spellName, board.GetTile(option.target.pos), actor.Dir);
        option.areaTargets = tiles;
        //basically, is it cool or not if the ability hits the caster? 
        //later the caster location and this are used to see if allow the ability to cast (ie if the ability will hit the caster, this isCasterMatch says, no this is a bad spell don't let it cast on me or yes
        //this is a good spell, let it cast on me
        option.isCasterMatch = IsAbilityTargetMatch(poa, board.GetTile(actor),snai); 

        bool isActorMoved = true; //is the actor in a hypothetical new location or standing on starting position? (matters for hitting self with an ability
        Tile currentTile = board.GetTile(actor);
        if (currentTile.UnitId == actor.TurnOrder)
            isActorMoved = false;

        //Debug.Log("tiles count is " + tiles.Count);
        for (int i = 0; i < tiles.Count; ++i)
        {
            Tile tile = tiles[i];
            //Debug.Log("evaluating tile: " + tile.GetTileSummary());
            if( tile.UnitId == actor.TurnOrder )
            {
                option.isCasterTarget = true; //ability targets caster in current location, used later to tell unit to move after
                if(isActorMoved || !snai.isHitSelf)
                {
                    //Debug.Log("ability targetting actor but actor has moved or the ability doesn't allow self hits " + tile.GetTileSummary());
                    option.AddMark(tile, false); //can't target self after hypothetically moving or if ability doesn't allow it
                    continue;
                }
                
            }
            else if (board.GetTile(actor) == tiles[i]) //|| !poa.ability.IsTarget(tile)
            {
                //Ignore where the caster is standing as this isn't the true place the caster is standing but a temporary thing depending on the move
                //caster position figured in later
                //Debug.Log("only tile found in tiles count is the tile the actor is on, moving to the enxt one " + tile.GetTileSummary());
                continue;
            }

            //Debug.Log("rating fire location, inside tile loop");
            bool isMatch;
            if (puList == null)
            {
                //Debug.Log("adding mark a x,y, isMatch " + tile.pos.x + "," + tile.pos.y + " " + tile.UnitId);
                isMatch = IsAbilityTargetMatch(poa, tile, snai); 
            }
            else
            {
                //Debug.Log("adding mark b x,y, isMatch " + tile.pos.x + "," + tile.pos.y);
                isMatch = IsAbilityTargetMatchForPlayerUnit(poa, tile, snai, puList); 
            }
            //Debug.Log("adding mark x,y, isMatch " + tile.pos.x + "," + tile.pos.y + " " + isMatch);
            option.AddMark(tile, isMatch); 
        }
    }
	
	void PickBestOption (CombatPlanOfAttack poa, List<AttackOption> list)
	{
        int bestScore = 1;
        List<AttackOption> bestOptions = new List<AttackOption>();
        for (int i = 0; i < list.Count; ++i)
        {
            AttackOption option = list[i];
            int score = option.GetScoreSN(actor, poa.spellName, board.GetTile(actor)); //Debug.Log("score is " + score + " x,y: " + option.target.pos.x +"," + option.target.pos.y);
            if (score > bestScore)
            {
                bestScore = score;
                bestOptions.Clear();
                bestOptions.Add(option);
            }
            else if (score == bestScore)
            {
                bestOptions.Add(option);
            }
        }

        if (bestOptions.Count == 0)
        {
            poa.spellName = null; //Debug.Log("setting spellName to null, no good option found"); // Clear ability as a sign not to perform it
            return;
        }

        List<AttackOption> finalPicks = new List<AttackOption>();
        bestScore = 0;
        for (int i = 0; i < bestOptions.Count; ++i)
        {
            AttackOption option = bestOptions[i];
            int score = option.bestAngleBasedScore; //Debug.Log("angle based score is " + score + " x,y: " + option.target.pos.x + "," + option.target.pos.y);
            if (score > bestScore)
            {
                bestScore = score;
                finalPicks.Clear();
                finalPicks.Add(option);
            }
            else if (score == bestScore)
            {
                finalPicks.Add(option);
            }
        }

        AttackOption choice = finalPicks[UnityEngine.Random.Range(0, finalPicks.Count)];
        poa.fireLocation = choice.target.pos;
        poa.attackDirection = choice.direction;
        poa.moveLocation = choice.bestMoveTile.pos;
    }

	void FindNearestFoe ()
	{
        nearestFoe = null;
        board.Search(board.GetTile(actor), delegate (Tile arg1, Tile arg2)
        {
            //Debug.Log("searching for nearest foe 1");
            if (nearestFoe == null && arg2.UnitId != NameAll.NULL_INT)
            {
                //Debug.Log("searching for nearest foe 2");
                int targetId = arg2.UnitId;
                if( PlayerManager.Instance.IsMatch(actor.TurnOrder,targetId,Targets.Foe))
                {
                    //Debug.Log("searching for nearest foe 3");
                    if (PlayerManager.Instance.GetPlayerUnit(targetId).AbleToFight)
                    {
                        //Debug.Log("searching for nearest foe 4 " + targetId + " isAbleToFight " + PlayerManager.Instance.GetPlayerUnit(targetId).AbleToFight);
                        nearestFoe = PlayerManager.Instance.GetPlayerUnit(targetId);
                        return true;
                    }
                }
            }
            return nearestFoe == null;
        });

        //nearestFoe = null;
        //bc.board.Search(actor.tile, delegate(Tile arg1, Tile arg2) {
        //	if (nearestFoe == null && arg2.content != null)
        //	{
        //		Alliance other = arg2.content.GetComponentInChildren<Alliance>();
        //		if (other != null && alliance.IsMatch(other, Targets.Foe))
        //		{
        //			Unit unit = other.GetComponent<Unit>();
        //			Stats stats = unit.GetComponent<Stats>();
        //			if (stats[StatTypes.HP] > 0)
        //			{
        //				nearestFoe = unit;
        //				return true;
        //			}
        //		}
        //	}
        //	return nearestFoe == null;
        //});
    }

    void MoveTowardOpponent (CombatPlanOfAttack poa)
	{
        if (cc.turn.hasUnitMoved)
        {//ie can't move
            poa.moveLocation = board.GetTile(actor).pos;
            return;
        }

        List<Tile> moveOptions = GetMoveOptions(); //Debug.Log("moving towards opponent");
        FindNearestFoe();
        if (nearestFoe != null)
        {
            Tile toCheck = board.GetTile(nearestFoe);// nearestFoe.tile;
            while (toCheck != null)
            {
                if (moveOptions.Contains(toCheck))
                {
                    poa.moveLocation = toCheck.pos;
                    return;
                }
                toCheck = toCheck.prev;
            }
        }

        poa.moveLocation = board.GetTile(actor).pos; //actor.tile.pos;

        //List<Tile> moveOptions = GetMoveOptions();
        //FindNearestFoe();
        //if (nearestFoe != null)
        //{
        //	Tile toCheck = nearestFoe.tile;
        //	while (toCheck != null)
        //	{
        //		if (moveOptions.Contains(toCheck))
        //		{
        //			poa.moveLocation = toCheck.pos;
        //			return;
        //		}
        //		toCheck = toCheck.prev;
        //	}
        //}

        //poa.moveLocation = actor.tile.pos;
    }

    void MoveAwayOpponent(CombatPlanOfAttack poa)
    {
        if (cc.turn.hasUnitMoved)
        {//ie can't move
            poa.moveLocation = board.GetTile(actor).pos;
            return;
        }

        List<Tile> moveOptions = GetMoveOptions(); //Debug.Log("moving away from opponent");
        FindNearestFoe();
        if (nearestFoe != null)
        {
            poa.moveLocation = board.GetFarthestPoint(moveOptions, board.GetTile(nearestFoe), board.GetTile(actor));
            return;
        }

        poa.moveLocation = board.GetTile(actor).pos; //actor.tile.pos;
    }

    public Directions DetermineEndFacingDirection ()
	{
		Directions dir = (Directions)UnityEngine.Random.Range(0, 4);
        FindNearestFoe();
        if (nearestFoe != null)
        {
            //Debug.Log("nearest foe is " + nearestFoe.TurnOrder + "," + nearestFoe.TeamId);
            Directions start = actor.Dir;
            for (int i = 0; i < 4; ++i)
            {
                actor.Dir = (Directions)i; 
                if (nearestFoe.GetFacingPU(actor) == Facings.Front)
                {
                    dir = actor.Dir;
                    break;
                }
            }
            actor.Dir = start;
        }
        else
        {
            //Debug.Log("CANT FIND NEAREST FOE");
        }
        return dir;
	}
	#endregion
}