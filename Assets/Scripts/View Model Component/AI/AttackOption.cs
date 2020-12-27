using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackOption 
{
	#region Classes
	class Mark
	{
		public Tile tile;
		public bool isMatch;
		
		public Mark (Tile tile, bool isMatch)
		{
			this.tile = tile;
			this.isMatch = isMatch;
		}
	}
	#endregion

	#region Fields
	public Tile target;
	public Directions direction;
	public List<Tile> areaTargets = new List<Tile>();
	public bool isCasterMatch;
	public Tile bestMoveTile { get; private set; }
	public int bestAngleBasedScore { get; private set; }
	List<Mark> marks = new List<Mark>();
	List<Tile> moveTargets = new List<Tile>();

    public bool isCasterTarget = false; //ability targets the caster, must perform it before moving 
	#endregion

	#region Public
	public void AddMoveTarget (Tile tile)
	{
        //Debug.Log("adding move target " + isCasterMatch + "," + areaTargets.Contains(tile));
        // Dont allow moving to a tile that would negatively affect the caster
        //removes move targets that would involve the caster moving into an area where he would hit himself
        if (!isCasterMatch && areaTargets.Contains(tile))
            return;
        moveTargets.Add(tile);
        //Debug.Log("adding move target success");
    }

	public void AddMark (Tile tile, bool isMatch)
	{
		marks.Add (new Mark(tile, isMatch));
        //Debug.Log("adding mark success " + isMatch + "tile x, y are: " + tile.pos.x +"," + tile.pos.y);
    }

    

    // Scores the option based on how many of the targets are of the desired type
    public int GetScoreSN(PlayerUnit actor, SpellName sn, Tile actorTile)
    {
        GetBestMoveTargetSN(actor,sn, actorTile);
        if (bestMoveTile == null)
        {
            //Debug.Log("best move tile is null");
            return 0;
        }
            

        int score = 0; //Debug.Log("marks count is " + marks.Count);
        for (int i = 0; i < marks.Count; ++i)
        {
            if (marks[i].isMatch)
                score++;
            else
                score--;
        }

        //adds a score point if caster moves into the area of effect (doesn't include self target abilities)
        if (isCasterMatch && areaTargets.Contains(bestMoveTile) && !this.isCasterTarget)
            score++;

        return score;
    }
    #endregion

    #region Private
    // Returns the tile which is the most effective point for the caster to attack from
    //called in GetScoreSN
    void GetBestMoveTargetSN(PlayerUnit actor, SpellName sn, Tile actorTile)
    {
        if (this.isCasterTarget)
        {
            //Debug.Log("setting a new bestMoveTile due to self target ability");
            //abilty must target the caster where the caster stands, CombatComputerPlayer only uses the pos so a new tile with only those two attributes can be created
            bestMoveTile = actorTile;

            return;
        }

        if (moveTargets.Count == 0)
        {
            //Debug.Log("moveTargets count is 0, return null bestmovetarget");
            return;
        }
            

        if (IsAbilityAngleBasedSN(sn))
        {
            bestAngleBasedScore = int.MinValue;
            Tile startTile = actorTile; // caster.tile;
            Directions startDirection = actor.Dir;//caster.dir;
            actor.Dir = direction;//caster.dir = direction;

            List<Tile> bestOptions = new List<Tile>();
            for (int i = 0; i < moveTargets.Count; ++i)
            {
                actor.AISetUnitTile(moveTargets[i]);//caster.Place(moveTargets[i]);
                int score = GetAngleBasedScoreSN(actor);
                if (score > bestAngleBasedScore)
                {
                    bestAngleBasedScore = score;
                    bestOptions.Clear();
                }

                if (score == bestAngleBasedScore)
                {
                    bestOptions.Add(moveTargets[i]);
                }
            }

            actor.AISetUnitTile(startTile);//caster.Place(startTile);
            actor.Dir = startDirection;//caster.dir = startDirection;

            FilterBestMoves(bestOptions);
            bestMoveTile = bestOptions[UnityEngine.Random.Range(0, bestOptions.Count)];
        }
        else
        {
            bestMoveTile = moveTargets[UnityEngine.Random.Range(0, moveTargets.Count)];
        }
    }

    

    // Indicates whether the angle of attack is an important factor in the
    // application of this ability
    bool IsAbilityAngleBasedSN(SpellName sn)
    {
        bool isAngleBased = false;

        if (sn.RemoveStat == NameAll.REMOVE_STAT_HEAL) //postive shit targets allies, doesn't matter the angle
            isAngleBased = false;
        else if (sn.PMType != NameAll.PM_TYPE_MAGICAL)
            isAngleBased = true;

        return isAngleBased;
    }

    

    // Scores the option based on how many of the targets are a match
    // and considers the angle of attack to each mark
    int GetAngleBasedScoreSN(PlayerUnit caster)
    {
        int score = 0;
        for (int i = 0; i < marks.Count; ++i)
        {
            int value = marks[i].isMatch ? 1 : -1;
            int multiplier = MultiplierForAngleSN(caster, marks[i].tile);
            score += value * multiplier;
        }
        return score;
    }

    void FilterBestMoves (List<Tile> list)
	{
		if (!isCasterMatch)
			return;

		bool canTargetSelf = false;
		for (int i = 0; i < list.Count; ++i)
		{
			if (areaTargets.Contains(list[i]))
			{
				canTargetSelf = true;
				break;
			}
		}

		if (canTargetSelf)
		{
			for (int i = list.Count - 1; i >= 0; --i)
			{
				if (!areaTargets.Contains(list[i]))
					list.RemoveAt(i);
			}
		}
	}

	

    int MultiplierForAngleSN(PlayerUnit caster, Tile tile)
    {
        if (tile.UnitId == NameAll.NULL_UNIT_ID)
            return 0;

        PlayerUnit target = PlayerManager.Instance.GetPlayerUnit(tile.UnitId);

        Facings facing = caster.GetFacingPU(target);
        if (facing == Facings.Back)
            return 90;
        if (facing == Facings.Side)
            return 75;
        return 50;
    }
    #endregion

    #region obsolete
    // Scores the option based on how many of the targets are of the desired type
    //public int GetScore (Unit caster, Ability ability)
    //{
    //	GetBestMoveTarget(caster, ability);
    //	if (bestMoveTile == null)
    //		return 0;

    //	int score = 0;
    //	for (int i = 0; i < marks.Count; ++i)
    //	{
    //		if (marks[i].isMatch)
    //			score++;
    //		else
    //			score--;
    //	}

    //	if (isCasterMatch && areaTargets.Contains(bestMoveTile))
    //		score++;

    //	return score;
    //}

    // Returns the tile which is the most effective point for the caster to attack from
    //   void GetBestMoveTarget (Unit caster, Ability ability)
    //{
    //	if (moveTargets.Count == 0)
    //		return;

    //	if (IsAbilityAngleBased(ability))
    //	{
    //		bestAngleBasedScore = int.MinValue;
    //		Tile startTile = caster.tile;
    //		Directions startDirection = caster.dir;
    //		caster.dir = direction;

    //		List<Tile> bestOptions = new List<Tile>();
    //		for (int i = 0; i < moveTargets.Count; ++i)
    //		{
    //			caster.Place(moveTargets[i]);
    //			int score = GetAngleBasedScore(caster);
    //			if (score > bestAngleBasedScore)
    //			{
    //				bestAngleBasedScore = score;
    //				bestOptions.Clear();
    //			}

    //			if (score == bestAngleBasedScore)
    //			{
    //				bestOptions.Add(moveTargets[i]);
    //			}
    //		}

    //		caster.Place(startTile);
    //		caster.dir = startDirection;

    //		FilterBestMoves(bestOptions);
    //		bestMoveTile = bestOptions[ UnityEngine.Random.Range(0, bestOptions.Count) ];
    //	}
    //	else
    //	{
    //		bestMoveTile = moveTargets[ UnityEngine.Random.Range(0, moveTargets.Count) ];
    //	}
    //}

    // Indicates whether the angle of attack is an important factor in the
    // application of this ability
    //bool IsAbilityAngleBased (Ability ability)
    //{
    //	bool isAngleBased = false;
    //	for (int i = 0; i < ability.transform.childCount; ++i)
    //	{
    //		HitRate hr = ability.transform.GetChild(i).GetComponent<HitRate>();
    //		if (hr.IsAngleBased)
    //		{
    //			isAngleBased = true;
    //			break;
    //		}
    //	}
    //	return isAngleBased;
    //}

    // Scores the option based on how many of the targets are a match
    // and considers the angle of attack to each mark
    //   int GetAngleBasedScore (Unit caster)
    //{
    //	int score = 0;
    //	for (int i = 0; i < marks.Count; ++i)
    //	{
    //		int value = marks[i].isMatch ? 1 : -1;
    //		int multiplier = MultiplierForAngle(caster, marks[i].tile);
    //		score += value * multiplier;
    //	}
    //	return score;
    //}

    //int MultiplierForAngle (Unit caster, Tile tile)
    //{
    //	if (tile.content == null)
    //		return 0;

    //	Unit defender = tile.content.GetComponentInChildren<Unit>();
    //	if (defender == null)
    //		return 0;

    //	Facings facing = caster.GetFacing(defender);
    //	if (facing == Facings.Back)
    //		return 90;
    //	if (facing == Facings.Side)
    //		return 75;
    //	return 50;
    //}
    #endregion
}