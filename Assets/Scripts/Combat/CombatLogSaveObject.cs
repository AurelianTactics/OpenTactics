using System.Collections;
using System.Collections.Generic;


public class CombatLogSaveObject
{
	/// <summary>
	/// Combat Log can be saved to file using these objects
	/// storing a list and saving in PlayerManager for now
	/// </summary>



	int logId;
	int tick;
	int logType;
	int logSubType;
	int value;
	int statusValue;
	int rollResult;
	int rollChance;
	int spellNameId;
	int actorId;
	int targetId;
	int targetTileX;
	int targetTileY;
	int targetTileZ;


	public CombatLogSaveObject(int clsId, int log_tick, int type, int subType, int effectValue, int sValue, int rollR, int rollC, CombatTurn turn)
	{
		this.logId = clsId;
		this.tick = log_tick;
		this.logType = type;
		this.logSubType = subType;
		this.value = effectValue;
		this.statusValue = sValue;
		this.rollResult = rollR;
		this.rollChance = rollC;
		if( turn != null)
		{
			if(turn.spellName != null)
				this.spellNameId = turn.spellName.SpellId;
			if( turn.actor != null)
				this.actorId = turn.actor.TurnOrder;
			this.targetId = turn.targetUnitId;
			if(turn.targetTile != null)
			{
				this.targetTileX = turn.targetTile.pos.x;
				this.targetTileY = turn.targetTile.pos.y;
				this.targetTileZ = turn.targetTile.height;
			}
			
		}
		else
		{
			this.spellNameId = NameAll.NULL_INT;
			this.actorId = NameAll.NULL_INT;
			this.targetId = NameAll.NULL_INT;
			this.targetTileX = NameAll.NULL_INT;
			this.targetTileY = NameAll.NULL_INT;
			this.targetTileZ = NameAll.NULL_INT;
		}
	}

	public CombatLogSaveObject(int clsId, int log_tick, int type, int subType, int effectValue, int sValue, int rollR, int rollC, int spellId,
		int actor, int tgtId, int tileX, int tileY, int tileZ)
	{
		this.logId = clsId;
		this.tick = log_tick;
		this.logType = type;
		this.logSubType = subType;
		this.value = effectValue;
		this.statusValue = sValue;
		this.rollResult = rollR;
		this.rollChance = rollC;
		this.spellNameId = spellId;
		this.actorId = actor;
		this.targetId = tgtId;
		this.targetTileX = tileX;
		this.targetTileY = tileY;
		this.targetTileZ = tileZ;
	}



}
