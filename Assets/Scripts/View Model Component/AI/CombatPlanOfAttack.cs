using UnityEngine;
using System.Collections;

//used for AI turn and for RL turn stuff. see below for RL Duel modifications of these
public class CombatPlanOfAttack
{
    public SpellName spellName;
    public Targets target;
    public Point moveLocation;
    public Point fireLocation;
    public Directions attackDirection;
    public bool isActFirst;
	public bool isEndTurn;

    public CombatPlanOfAttack()
    {
        this.spellName = null;
        //this.target = null;
        //this.moveLocation = null;
        //this.fireLocation = null;
        //this.attackDirection = null;
        this.isActFirst = false;
		this.isEndTurn = false;
    }

	//constructor for RL duel mode, given a tightly coupled array, construct this plan
	public CombatPlanOfAttack(float[] actionArray, Board board)
	{
		this.spellName = null;
		//this.moveLocation = null; //move location
		//this.target = null;
		//this.fireLocation = null; //ability usage location
		//this.attackdirection = null; //used for wait direction in wait, target tile all other cases
		this.isActFirst = false;

		//3 branches of actions
		//index 0: wait 0, move 1, attack 2, primary 3
		//index 1: which primary abilities (8 total)
		//index 2: target tile (0 is 0,0; 1 is 0,1; 2 is 1,0; 3 is 1,1) or wait direction (the direction enums)

		int actionType = (int)actionArray[0];
		if( actionType == 0) //wait
		{
			this.attackDirection = (Directions)actionArray[2];
			this.isEndTurn = true;
		}
		else
		{
			this.isEndTurn = false;
			int index2 = (int)actionArray[2];
			Point p;
			if (index2 == 0)
				p = new Point(0, 0);
			else if (index2 == 1)
				p = new Point(1, 0);
			else if (index2 == 2)
				p = new Point(0, 1);
			else
				p = new Point(1, 1);

			if (actionType == 1)
			{
				this.moveLocation = p;
			}
			else
			{
				this.isActFirst = true;
				this.fireLocation = p;
				//Debug.Log("making attack, fireLocation is " + p.x + p.y);
				this.target = Targets.Tile;
				if (actionType == 2)
				{
					//is attack, only attacking in duel
					this.spellName = SpellManager.Instance.GetSpellNameByIndex(NameAll.SPELL_INDEX_ATTACK_FIST);
				}
				else
				{
					//get ability
					int abilityInt = (int)actionArray[1];
					var spellNameList = SpellManager.Instance.GetSpellNamesByCommandSet(NameAll.COMMAND_SET_BATTLE_SKILL);
					this.spellName = spellNameList[abilityInt];
				}
			}
			
		}
	}


	public void POASummary()
    {
        if( this.spellName == null)
        {
            Debug.Log("poa spellName is null");
        }
        else if( this.moveLocation == null)
        {
            Debug.Log("poa moveLocation is null");
        }
        else if (this.fireLocation == null)
        {
            Debug.Log("poa fireLocation is null");
        }
        else
        {
            Debug.Log("" + spellName.AbilityName + ", " + target.ToString() + ", "
                        + moveLocation.ToString() + ", " + fireLocation.ToString() + ", " + attackDirection.ToString() + ", " + isActFirst);
        }
        
    }
}

