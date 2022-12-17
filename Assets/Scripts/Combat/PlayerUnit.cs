using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerUnit {

    //tells victory conditions to check to see if a unit is knocked out
    public static string DidChangeNotification()
    {
        return "PlayerUnit.AbleToFightDidChange";
    }

    private bool TwoSwordsEligible { get; set; }
    private bool OnMoveEffect { get; set; }
    private bool SpecialMoveRange { get; set; }

    public int TurnOrder { get; set; }
    public int TeamId { get; set; }
    public string UnitName { get; set; }
    public bool AbleToFight { get; set; }
    public bool QuickFlag { get; set; }

    public bool ReactionFlag { get; set; }
    public Directions Dir { get; set; }
    public int TileX { get; set; }
    public int TileY { get; set; }
    public int TileZ { get; set; }

    public int Level { get; set; }
    public int ClassId { get; set; }
    public string Sex { get; set; }
    public int ZodiacInt { get; set; }
    public int CT { get; set; }

    public int StatTotalMove { get; set; }
    public int StatTotalJump { get; set; }
    public int StatTotalSpeed { get; set; }
    public int StatTotalMaxLife { get; set; }
    public int StatTotalLife { get; set; }
    public int StatTotalMP { get; set; }
    public int StatTotalMaxMP { get; set; }
    public int StatTotalPA { get; set; }
    public int StatTotalAgi { get; set; }
    public int StatTotalMA { get; set; }
    public int StatTotalBrave { get; set; }
    public int StatTotalFaith { get; set; }
    public int StatTotalCunning { get; set; }
    public int StatTotalCEvade { get; set; }
    public int StatTotalPEvade { get; set; }
    public int StatTotalMEvade { get; set; }

    public int StatUnitMove { get; set; }
    public int StatUnitJump { get; set; }
    public int StatUnitSpeed { get; set; }
    public int StatUnitMaxLife { get; set; }
    public int StatUnitLife { get; set; }
    public int StatUnitMP { get; set; }
    public int StatUnitMaxMP { get; set; }
    public int StatUnitPA { get; set; }
    public int StatUnitAgi { get; set; }
    public int StatUnitMA { get; set; }
    public int StatUnitBrave { get; private set; }
    public int StatUnitFaith { get; private set; }
    public int StatUnitCunning { get; private set; }
    public int StatUnitCEvade { get; set; }
    public int StatUnitPEvade { get; set; }
    public int StatUnitMEvade { get; set; }

    public int StatItemMove { get; set; }
    public int StatItemJump { get; set; }
    public int StatItemSpeed { get; set; }
    public int StatItemMaxLife { get; set; }
    public int StatItemLife { get; set; }
    public int StatItemMP { get; set; }
    public int StatItemMaxMP { get; set; }
    public int StatItemPA { get; set; }
    public int StatItemAgi { get; set; }
    public int StatItemMA { get; set; }
    public int StatItemBrave { get; set; }
    public int StatItemFaith { get; set; }
    public int StatItemCunning { get; set; }
    public int StatItemCEvade { get; set; }
    public int StatItemPEvade { get; set; }
    public int StatItemMEvade { get; set; }
    public int StatItemOffhandMEvade { get; set; }
    public int StatItemOffhandPEvade { get; set; }
    public int StatItemAccessoryMEvade { get; set; }
    public int StatItemAccessoryPEvade { get; set; }
    public int StatItemWEvade { get; set; }

    public int ItemSlotWeapon { get; set; }
    public int ItemSlotOffhand { get; set; }
    public int ItemSlotHead { get; set; }
    public int ItemSlotBody { get; set; }
    public int ItemSlotAccessory { get; set; }
    public int AbilitySecondaryCode { get; set; }
    public int AbilityReactionCode { get; set; }
    public int AbilitySupportCode { get; set; }
    public int AbilityMovementCode { get; set; }

    
    public PlayerUnit(int modVersion)
    {
        //turn_order, team_id, unit_name, facing_direction, current_x / y / z, tile
        //class_id, sex, zodiac_int
        this.TurnOrder = 0;
        this.TeamId = 2;

        this.Dir = Directions.North;
        //this.facing_direction = 0;
        //SetUnitTileAndXYZ(zIndex);

        if( modVersion == NameAll.VERSION_AURELIAN)
        {
            this.ClassId = 100;
            this.Level = 5;
            this.Sex = "Male";
            this.UnitName = "Lucas";
        }
        else
        {
            this.ClassId = 1;
            this.Level = 25;
            this.Sex = "Female";
            this.UnitName = "Lena";
        }

        this.ZodiacInt = 1;

        this.ItemSlotWeapon = NameAll.FIST_EQUIP;
        this.ItemSlotOffhand = NameAll.NO_EQUIP;
        this.ItemSlotHead = NameAll.NO_EQUIP;
        this.ItemSlotBody = NameAll.NO_EQUIP;
        this.ItemSlotAccessory = NameAll.NO_EQUIP;

        this.StatUnitBrave = 55;
        this.StatUnitFaith = 55;
        this.StatUnitCunning = 55;
  
        InitializeNewPlayer();
        SetBaseStats();//gets stats based on level and class
        NullEquipment(); //empties equipment
        ClearAbilities(); //ability strings set to null
        CalculateTotalStats(); //called automatically in NullEquipment, added here
    }


	// create a playerUnit for duel mode. mostly random but some stuff hardwired in like equipment
	public PlayerUnit(int modVersion, bool randomize, int classId, bool deactivateSpecial = true, int level = -1919, bool isNullAbilities = false)
	{

		this.TurnOrder = 0;
		this.TeamId = 2;
		this.Dir = Directions.North;
		//this.facing_direction = 0;

		if (UnityEngine.Random.Range(0, 2) == 1)
			this.Sex = "Male";
		else
			this.Sex = "Female";

		this.ZodiacInt = UnityEngine.Random.Range(0, 12);

		if (modVersion == NameAll.VERSION_AURELIAN)
		{
			if (classId == NameAll.NULL_INT)
				if (randomize)
					this.ClassId = UnityEngine.Random.Range(NameAll.CLASS_FIRE_MAGE, NameAll.CLASS_DRUID + 1);
				else
					this.ClassId = NameAll.CLASS_BRAWLER;
			else
				this.ClassId = classId;

			if (level == NameAll.NULL_INT || level < 1 || level > 10)
				this.Level = UnityEngine.Random.Range(1, 11);
			else
				this.Level = level;

			this.StatUnitBrave = UnityEngine.Random.Range(40, 61);
			this.StatUnitFaith = UnityEngine.Random.Range(40, 61);
			this.StatUnitCunning = UnityEngine.Random.Range(40, 61);
		}
		else
		{
			if (classId == NameAll.NULL_INT)
				this.ClassId = UnityEngine.Random.Range(NameAll.CLASS_CHEMIST, NameAll.CLASS_MIME + 1); //this.ClassId = NameAll.CLASS_NINJA;
			else
				this.ClassId = classId;
			if (level == NameAll.NULL_INT || level < 1 || level > 99)
				this.Level = UnityEngine.Random.Range(1, 100);
			else
				this.Level = level;
			this.StatUnitBrave = UnityEngine.Random.Range(40, 71);
			this.StatUnitFaith = UnityEngine.Random.Range(40, 71);
			this.StatUnitCunning = UnityEngine.Random.Range(40, 71);
		}

		this.UnitName = AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY, this.ClassId);

		this.ItemSlotWeapon = NameAll.FIST_EQUIP;
		this.ItemSlotOffhand = NameAll.NO_EQUIP;
		this.ItemSlotHead = NameAll.NO_EQUIP;
		this.ItemSlotBody = NameAll.NO_EQUIP;
		this.ItemSlotAccessory = NameAll.NO_EQUIP;

		InitializeNewPlayer();
		SetBaseStats();
		NullEquipment();
		ClearAbilities();

		//Debug.Log("total move jump are " + this.StatTotalMove + " " + this.StatTotalJump);

		//only do stats
		//if (!isNullAbilities)
		//{
		//	//AbilityManager.Instance.SetIoType(NameAll.ITEM_MANAGER_SIMPLE); //assuming this is done
		//	this.AbilitySecondaryCode = AbilityManager.Instance.GetRandomAbility(this.ClassId, NameAll.ABILITY_SLOT_SECONDARY, deactivateSpecial);
		//	this.AbilityReactionCode = AbilityManager.Instance.GetRandomAbility(this.ClassId, NameAll.ABILITY_SLOT_REACTION, deactivateSpecial);
		//	this.AbilitySupportCode = AbilityManager.Instance.GetRandomAbility(this.ClassId, NameAll.ABILITY_SLOT_SUPPORT, deactivateSpecial);
		//	EquipMovementAbility(AbilityManager.Instance.GetRandomAbility(this.ClassId, NameAll.ABILITY_SLOT_MOVEMENT, deactivateSpecial), true); //keeps the statusmanager from causing something wonky
		//}

		// no items in the duel
		//ItemManager.Instance.SetIoType(NameAll.ITEM_MANAGER_SIMPLE); //assuming this is done
		//EquipItem(ItemManager.Instance.GetRandomItem(this, NameAll.ITEM_SLOT_WEAPON), NameAll.ITEM_SLOT_WEAPON);
		//if (!ItemManager.Instance.IsUsingBothHands(this.ItemSlotWeapon, this.AbilitySupportCode, this.ClassId))
		//{
		//	//equipping the offhand slot to will override the 2h weapon equip. Instead just let the 2H weapon equip stay if applicable
		//	EquipItem(ItemManager.Instance.GetRandomItem(this, NameAll.ITEM_SLOT_OFFHAND), NameAll.ITEM_SLOT_OFFHAND);
		//}
		//EquipItem(ItemManager.Instance.GetRandomItem(this, NameAll.ITEM_SLOT_HEAD), NameAll.ITEM_SLOT_HEAD);
		//EquipItem(ItemManager.Instance.GetRandomItem(this, NameAll.ITEM_SLOT_BODY), NameAll.ITEM_SLOT_BODY);
		//EquipItem(ItemManager.Instance.GetRandomItem(this, NameAll.ITEM_SLOT_ACCESSORY), NameAll.ITEM_SLOT_ACCESSORY);

		//EquipItem(, NameAll.ITEM_SLOT_WEAPON);
		//EquipItem(, NameAll.ITEM_SLOT_HEAD);
		//EquipItem(, NameAll.ITEM_SLOT_BODY);
		CalculateTotalStats();

		//Debug.Log("total move jump are " + this.StatTotalMove + " " + this.StatTotalJump);
	}


	public PlayerUnit( int modVersion, bool randomize, bool deactivateSpecial = true, int level = -1919, bool isNullAbilities = false )
    {

        this.TurnOrder = 0;
        this.TeamId = 2;
        this.Dir = Directions.North;
        //this.facing_direction = 0;
        
        if (UnityEngine.Random.Range(0, 2) == 1)
            this.Sex = "Male";
        else
            this.Sex = "Female";

        this.ZodiacInt = UnityEngine.Random.Range(0, 12);

        if ( modVersion == NameAll.VERSION_AURELIAN)
        {
			if (randomize)
				this.ClassId = UnityEngine.Random.Range(NameAll.CLASS_FIRE_MAGE, NameAll.CLASS_DRUID + 1);
			else
				this.ClassId = NameAll.CLASS_BRAWLER;

			if (level == NameAll.NULL_INT || level < 1 || level > 10)
                this.Level = UnityEngine.Random.Range(1, 11);
            else
                this.Level = level;

            this.StatUnitBrave = UnityEngine.Random.Range(40,61);
            this.StatUnitFaith = UnityEngine.Random.Range(40, 61);
            this.StatUnitCunning = UnityEngine.Random.Range(40, 61);
        }
        else
        {
            this.ClassId = UnityEngine.Random.Range(NameAll.CLASS_CHEMIST, NameAll.CLASS_MIME + 1); //this.ClassId = NameAll.CLASS_NINJA;
            if (level == NameAll.NULL_INT || level < 1 || level > 99)
                this.Level = UnityEngine.Random.Range(1, 100);
            else
                this.Level = level;
            this.StatUnitBrave = UnityEngine.Random.Range(40, 71);
            this.StatUnitFaith = UnityEngine.Random.Range(40, 71);
            this.StatUnitCunning = UnityEngine.Random.Range(40, 71);
        }

        this.UnitName = AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY, this.ClassId);

        this.ItemSlotWeapon = NameAll.FIST_EQUIP;
        this.ItemSlotOffhand = NameAll.NO_EQUIP;
        this.ItemSlotHead = NameAll.NO_EQUIP;
        this.ItemSlotBody = NameAll.NO_EQUIP;
        this.ItemSlotAccessory = NameAll.NO_EQUIP;

        InitializeNewPlayer();
        SetBaseStats();
        NullEquipment();
        ClearAbilities();

        //Debug.Log("total move jump are " + this.StatTotalMove + " " + this.StatTotalJump);

        if( !isNullAbilities)
        {
            //AbilityManager.Instance.SetIoType(NameAll.ITEM_MANAGER_SIMPLE); //assuming this is done
            this.AbilitySecondaryCode = AbilityManager.Instance.GetRandomAbility(this.ClassId, NameAll.ABILITY_SLOT_SECONDARY, deactivateSpecial);
            this.AbilityReactionCode = AbilityManager.Instance.GetRandomAbility(this.ClassId, NameAll.ABILITY_SLOT_REACTION, deactivateSpecial);
            this.AbilitySupportCode = AbilityManager.Instance.GetRandomAbility(this.ClassId, NameAll.ABILITY_SLOT_SUPPORT, deactivateSpecial);
            EquipMovementAbility(AbilityManager.Instance.GetRandomAbility(this.ClassId, NameAll.ABILITY_SLOT_MOVEMENT, deactivateSpecial), true); //keeps the statusmanager from causing something wonky
        }
        
        //ItemManager.Instance.SetIoType(NameAll.ITEM_MANAGER_SIMPLE); //assuming this is done
        EquipItem(ItemManager.Instance.GetRandomItem(this, NameAll.ITEM_SLOT_WEAPON), NameAll.ITEM_SLOT_WEAPON);
        if (!ItemManager.Instance.IsUsingBothHands(this.ItemSlotWeapon, this.AbilitySupportCode, this.ClassId))
        {
            //equipping the offhand slot to will override the 2h weapon equip. Instead just let the 2H weapon equip stay if applicable
            EquipItem(ItemManager.Instance.GetRandomItem(this, NameAll.ITEM_SLOT_OFFHAND), NameAll.ITEM_SLOT_OFFHAND);
        }
        EquipItem(ItemManager.Instance.GetRandomItem(this, NameAll.ITEM_SLOT_HEAD), NameAll.ITEM_SLOT_HEAD);
        EquipItem(ItemManager.Instance.GetRandomItem(this, NameAll.ITEM_SLOT_BODY), NameAll.ITEM_SLOT_BODY);
        EquipItem(ItemManager.Instance.GetRandomItem(this, NameAll.ITEM_SLOT_ACCESSORY), NameAll.ITEM_SLOT_ACCESSORY);
        CalculateTotalStats();

        //Debug.Log("total move jump are " + this.StatTotalMove + " " + this.StatTotalJump);
    }

    //called in ClassEditController for a custome class
    public PlayerUnit(ClassEditObject ce)
    {
        //turn_order, team_id, unit_name, facing_direction, current_x / y / z, tile
        //class_id, sex, zodiac_int
        this.TurnOrder = 0;
        this.TeamId = 2;

        this.Dir = Directions.North;
        //this.facing_direction = 0;
        //SetUnitTileAndXYZ(zIndex);

        this.ClassId = ce.ClassId; //Debug.Log(" class id, move " + ce.ClassId + " " + ce.Move);

        if (ce.Version == NameAll.VERSION_AURELIAN)
        {
            this.Level = 5;
            this.Sex = "Male";
            this.UnitName = "Lucas";
        }
        else
        {
            this.Level = 25;
            this.Sex = "Female";
            this.UnitName = "Lena";
        }

        this.ZodiacInt = 1;

        this.ItemSlotWeapon = NameAll.FIST_EQUIP;
        this.ItemSlotOffhand = NameAll.NO_EQUIP;
        this.ItemSlotHead = NameAll.NO_EQUIP;
        this.ItemSlotBody = NameAll.NO_EQUIP;
        this.ItemSlotAccessory = NameAll.NO_EQUIP;

        this.StatUnitBrave = 55;
        this.StatUnitFaith = 55;
        this.StatUnitCunning = 55;

        InitializeNewPlayer();
        SetBaseStats(ce);//gets stats based on level and class
        NullEquipment(); //empties equipment
        ClearAbilities(); //ability strings set to null
        CalculateTotalStats(); //called automatically in NullEquipment, added here
    }

    //checks that the string can be returned for a value in building
    bool DecipherStringBool(string str, int start, int length)
    {
        string inputString = str.Substring(start, length);
        int numValue;
        if (Int32.TryParse(inputString, out numValue))
        {
            if( numValue != 0)
            {
                return true;
            }
            return false;
        }
        Debug.Log("unable to decipher string, returning false");
        return false;
    }

    //returns int values from the string
    int DecipherString( string str, int start, int length)
    {
        //s.Substring(startIndex, length));
        string inputString = str.Substring(start, length);
        int numValue;
        if( Int32.TryParse(inputString, out numValue))
        {
            return numValue;
        }
        Debug.Log("unable to decipher string, returning 0");
        return 0;
    }

    //called form CalcCode, used in building a PlayerUnit from a string
    public PlayerUnit(string str)
    {
        //find the proper length at some point
        if( str.Length == 105) //99
        {
            //Debug.Log("building player uint from string");
            this.TurnOrder = DecipherString(str, 0, 2);
            this.TeamId = DecipherString(str, 2, 1);
            this.UnitName = str.Substring(3, 5);
            //this.facing_direction = DecipherString(str, 8, 1);
            this.Dir = (Directions)DecipherString(str, 8, 1);
            this.AbleToFight = DecipherStringBool(str, 9, 1);
            this.QuickFlag = DecipherStringBool(str, 10, 1);
            //this.map_tile_index = DecipherString(str, 11, 4);
            this.ReactionFlag = DecipherStringBool(str, 15, 1);

            this.Level = DecipherString(str, 16, 3);
            this.ClassId = DecipherString(str, 19, 3);
            if( str.Substring(22,1).Equals("M"))
            {
                this.Sex = "Male";
            }
            else
            {
                this.Sex = "Female";
            }
            this.ZodiacInt = DecipherString(str, 23, 2);
            this.CT = DecipherString(str, 25, 3);

            //could do a shortcut to build this but for mid game, need to know the actual values
            this.StatUnitMove = DecipherString(str, 28, 2);
            this.StatUnitJump = DecipherString(str, 30, 2);
            this.StatUnitSpeed = DecipherString(str, 32, 2);
            this.StatUnitMaxLife = DecipherString(str, 34, 3);
            this.StatUnitLife = DecipherString(str, 37, 3);
            this.StatUnitMP = DecipherString(str, 40, 3);
            this.StatUnitMaxMP = DecipherString(str, 43, 3);
            this.StatUnitPA = DecipherString(str, 46, 2);
            this.StatUnitAgi = DecipherString(str, 48, 2);
            this.StatUnitMA = DecipherString(str, 50, 2);
            this.StatUnitBrave = DecipherString(str, 52, 3);
            this.StatUnitFaith = DecipherString(str, 55, 3);
            this.StatUnitCunning = DecipherString(str, 58, 3);
            this.StatUnitCEvade = DecipherString(str, 61, 2);
            this.StatUnitPEvade = DecipherString(str, 63, 2);
            this.StatUnitMEvade = DecipherString(str, 65, 2);

            this.AbilitySecondaryCode = DecipherString(str, 67, 3);
            this.AbilityReactionCode = DecipherString(str, 70, 3);
            this.AbilitySupportCode = DecipherString(str, 73, 3);
            this.AbilityMovementCode = DecipherString(str, 76, 3); //have to do it this way or everytime you pass the string around it will change the move/jump stat
            //this.AbilityMovementCode = NameAll.MOVEMENT_NONE;
            //EquipMovementAbility(DecipherString(str, 76, 3),true );

            this.ItemSlotWeapon = NameAll.FIST_EQUIP;
            this.ItemSlotOffhand = NameAll.NO_EQUIP;
            this.ItemSlotHead = NameAll.NO_EQUIP;
            this.ItemSlotBody = NameAll.NO_EQUIP;
            this.ItemSlotAccessory = NameAll.NO_EQUIP;

            EquipItem(DecipherString(str, 79, 4), NameAll.ITEM_SLOT_WEAPON);
            if (!ItemManager.Instance.IsUsingBothHands(this.ItemSlotWeapon, this.AbilitySupportCode, this.ClassId))
            {
                //equipping the offhand slot to will override the 2h weapon equip. Instead just let the 2H weapon equip stay if applicable
                EquipItem(DecipherString(str, 83, 4), NameAll.ITEM_SLOT_OFFHAND);
            }
            EquipItem(DecipherString(str, 87, 4), NameAll.ITEM_SLOT_HEAD);
            EquipItem(DecipherString(str, 91, 4), NameAll.ITEM_SLOT_BODY);
            EquipItem(DecipherString(str, 95, 4), NameAll.ITEM_SLOT_ACCESSORY);

            this.TileX = DecipherString(str, 99, 2);
            this.TileY = DecipherString(str, 101, 2);
            this.TileZ = DecipherString(str, 103, 2);

            CalculateTotalStats();
        }
        else
        {
            Debug.Log("ERROR: unable to build player unit from string" + str.Length);
        }
    }

    string BoolToString(bool zBool)
    {
        if(zBool)
        {
            return "1";
        }
        return "0";
    }

    public string BuildStringFromPlayerUnit()
    {
        //i.ToString("0000"); - explicit form
        //i.ToString("D4"); - short form format specifier
        string str = "";
        str += this.TurnOrder.ToString("00");
        str += this.TeamId.ToString("0");
        int z1 = 5; //Debug.Log("asdf" + this.UnitName);
        if( this.UnitName.Length < 5)
        {
            str += this.UnitName.PadRight(z1);
        }
        else
        {
            str += this.UnitName.Substring(0, z1);
        }
        str += ((int)this.Dir).ToString("0"); //this.facing_direction.ToString("0");
        str += BoolToString(this.AbleToFight);
        str += BoolToString(this.QuickFlag);
        int placeHolder = 0;
        str += placeHolder.ToString("0000");
        str += BoolToString(this.ReactionFlag);
        //this.TurnOrder = DecipherString(str, 0, 2);
        //this.TeamId = DecipherString(str, 2, 1);
        //this.UnitName = DecipherString(str, 3, 5);
        //this.facing_direction = DecipherString(str, 8, 1);
        //this.AbleToFight = DecipherStringBool(str, 9, 1);
        //this.QuickFlag = DecipherStringBool(str, 10, 1);
        //this.map_tile_index = DecipherString(str, 11, 4);
        //this.ReactionFlag = DecipherStringBool(str, 15, 1);

        str += this.Level.ToString("000");
        str += this.ClassId.ToString("000");
        if( this.Sex.Equals("Male"))
        {
            str += "M";
        }
        else
        {
            str += "F";
        }
        str += this.ZodiacInt.ToString("00");
        str += this.CT.ToString("000");
        //this.Level = DecipherString(str, 16, 3);
        //this.ClassId = DecipherString(str, 19, 3);
        //if (str.Substring(22, 1).Equals("M"))
        //{
        //    this.Sex = "Male";
        //}
        //else
        //{
        //    this.Sex = "Female";
        //}
        //this.ZodiacInt = DecipherStringString(str, 23, 1);
        //this.CT = DecipherStringString(str, 24, 3);

        str += this.StatUnitMove.ToString("00");
        str += this.StatUnitJump.ToString("00");
        str += this.StatUnitSpeed.ToString("00");
        str += this.StatUnitMaxLife.ToString("000");
        str += this.StatUnitLife.ToString("000");
        str += this.StatUnitMP.ToString("000");
        str += this.StatUnitMaxMP.ToString("000");
        str += this.StatUnitPA.ToString("00");
        str += this.StatUnitAgi.ToString("00");
        str += this.StatUnitMA.ToString("00");
        str += this.StatUnitBrave.ToString("000");
        str += this.StatUnitFaith.ToString("000");
        str += this.StatUnitCunning.ToString("000");
        str += this.StatUnitCEvade.ToString("00");
        str += this.StatUnitPEvade.ToString("00");
        str += this.StatUnitMEvade.ToString("00");
        //this.StatUnitMove = DecipherStringString(str, 27, 2);
        //this.StatUnitJump = DecipherStringString(str, 29, 2);
        //this.StatUnitSpeed = DecipherStringString(str, 31, 2);
        //this.StatUnitMaxLife = DecipherStringString(str, 33, 3);
        //this.StatUnitLife = DecipherStringString(str, 36, 3);
        //this.StatUnitMP = DecipherStringString(str, 39, 3);
        //this.StatUnitMAxMp = DecipherStringString(str, 42, 3);
        //this.StatUnitPA = DecipherStringString(str, 45, 2);
        //this.StatUnitAgi = DecipherStringString(str, 47, 2);
        //this.StatUnitMA = DecipherStringString(str, 49, 2);
        //this.StatUnitBrave = DecipherStringString(str, 51, 3);
        //this.StatUnitFaith = DecipherStringString(str, 54, 3);
        //this.StatUnitCunning = DecipherStringString(str, 57, 3);
        //this.StatUnitCEvade = DecipherStringString(str, 60, 2);
        //this.StatUnitPEvade = DecipherStringString(str, 62, 2);
        //this.StatUnitMEvade = DecipherStringString(str, 64, 2);

        str += this.AbilitySecondaryCode.ToString("000");
        str += this.AbilityReactionCode.ToString("000");
        str += this.AbilitySupportCode.ToString("000");
        str += this.AbilityMovementCode.ToString("000");
        //this.AbilitySecondaryCode = DecipherStringString(str, 66, 3);
        //this.AbilityReactionCode = DecipherStringString(str, 69, 3);
        //this.AbilitySupportCode = DecipherStringString(str, 72, 3);
        //this.AbilityMovementCode = NameAll.MOVEMENT_NONE;
        //EquipMovementAbility(DecipherStringString(str, 75, 3));

        str += this.ItemSlotWeapon.ToString("0000");
        str += this.ItemSlotOffhand.ToString("0000");
        str += this.ItemSlotHead.ToString("0000");
        str += this.ItemSlotBody.ToString("0000");
        str += this.ItemSlotAccessory.ToString("0000");
        //EquipItem(DecipherStringString(str, 78, 4), NameAll.ITEM_SLOT_WEAPON);
        //EquipItem(DecipherStringString(str, 82, 4), NameAll.ITEM_SLOT_OFFHAND);
        //EquipItem(DecipherStringString(str, 86, 4), NameAll.ITEM_SLOT_HEAD);
        //EquipItem(DecipherStringString(str, 90, 4), NameAll.ITEM_SLOT_BODY);
        //EquipItem(DecipherStringString(str, 94, 4), NameAll.ITEM_SLOT_ACCESSORY);

        str += this.TileX.ToString("00");
        str += this.TileY.ToString("00");
        str += this.TileZ.ToString("00");

        return str;
    }

    public PlayerUnit(PlayerUnit pu)
    {
        this.TurnOrder = pu.TurnOrder;
        this.TeamId = pu.TeamId;
        this.UnitName = pu.UnitName;
        this.Dir = pu.Dir;
        this.TileX = pu.TileX;
        this.TileY = pu.TileY;
        this.TileZ = pu.TileZ;
        //this.facing_direction = pu.GetFacing_direction();
        //this.current_x = pu.GetCurrent_x();
        //this.current_y = pu.GetCurrent_y();
        //this.current_z = pu.GetCurrent_z();
        this.AbleToFight = pu.AbleToFight;
        this.QuickFlag = pu.IsQuickFlag();
        //this.map_tile_index = pu.GetMap_tile_index();
        this.ReactionFlag = pu.ReactionFlag;
        this.Level = pu.Level;
        this.ClassId = pu.ClassId;
        this.Sex = pu.Sex;
        this.ZodiacInt = pu.ZodiacInt;
        this.CT = pu.CT;

        this.StatTotalMove = pu.StatTotalMove;
        this.StatTotalJump = pu.StatTotalJump;
        this.StatTotalSpeed = pu.StatTotalSpeed;
        this.StatTotalMaxLife = pu.StatTotalMaxLife; //add this functionality versus life later
        this.StatTotalLife = pu.StatTotalLife;
        this.StatTotalMP = pu.StatTotalMP;
        this.StatTotalMaxMP = pu.StatTotalMaxMP;
        this.StatTotalPA = pu.StatTotalPA;
        this.StatTotalAgi = pu.StatTotalAgi;
        this.StatTotalMA = pu.StatTotalMA;
        this.StatTotalBrave = pu.StatTotalBrave;
        this.StatTotalFaith = pu.StatTotalFaith;
        this.StatTotalCunning = pu.StatTotalCunning;
        this.StatTotalCEvade = pu.StatTotalCEvade;
        this.StatTotalPEvade = pu.StatTotalPEvade;
        this.StatTotalMEvade = pu.StatTotalMEvade;

        this.StatUnitMove = pu.StatUnitMove;
        this.StatUnitJump = pu.StatUnitJump;
        this.StatUnitSpeed = pu.StatUnitSpeed;
        this.StatUnitMaxLife = pu.StatUnitMaxLife; //add this functionality versus life later
        this.StatUnitLife = pu.StatUnitLife;
        this.StatUnitMP = pu.StatUnitMP;
        this.StatUnitMaxMP = pu.StatUnitMaxMP;
        this.StatUnitPA = pu.StatUnitPA;
        this.StatUnitAgi = pu.StatUnitAgi;
        this.StatUnitMA = pu.StatUnitMA;
        this.StatUnitBrave = pu.StatUnitBrave;
        this.StatUnitFaith = pu.StatUnitFaith;
        this.StatUnitCunning = pu.StatUnitCunning;
        this.StatUnitCEvade = pu.StatUnitCEvade;
        this.StatUnitPEvade = pu.StatUnitPEvade;
        this.StatUnitMEvade = pu.StatUnitMEvade;

        this.StatItemMove = pu.StatItemMove;
        this.StatItemJump = pu.StatItemJump;
        this.StatItemSpeed = pu.StatItemSpeed;
        this.StatItemMaxLife = pu.StatItemMaxLife;  //add this functionality versus life later
        this.StatItemLife = pu.StatItemLife; 
        this.StatItemMP = pu.StatItemMP ;
        this.StatItemMaxMP = pu.StatItemMaxMP;
        this.StatItemPA = pu.StatItemPA;
        this.StatItemAgi = pu.StatItemAgi;
        this.StatItemMA = pu.StatItemMA;
        this.StatItemBrave = pu.StatItemBrave;
        this.StatItemFaith = pu.StatItemFaith;
        this.StatItemCunning = pu.StatItemCunning;
        this.StatItemCEvade = pu.StatItemCEvade;
        this.StatItemPEvade = pu.StatItemPEvade;
        this.StatItemMEvade = pu.StatItemMEvade;
        this.StatItemOffhandMEvade = pu.StatItemOffhandMEvade;
        this.StatItemOffhandPEvade = pu.StatItemOffhandPEvade;
        this.StatItemAccessoryMEvade = pu.StatItemAccessoryMEvade;
        this.StatItemAccessoryPEvade = pu.StatItemAccessoryPEvade;
        this.StatItemWEvade = pu.StatItemWEvade;

        this.ItemSlotWeapon = pu.ItemSlotWeapon;
        this.ItemSlotOffhand = pu.ItemSlotOffhand;
        this.ItemSlotHead = pu.ItemSlotHead;
        this.ItemSlotBody = pu.ItemSlotBody;
        this.ItemSlotAccessory = pu.ItemSlotAccessory;

        this.AbilitySecondaryCode = pu.AbilitySecondaryCode;
        this.AbilityReactionCode = pu.AbilityReactionCode;
        this.AbilitySupportCode = pu.AbilitySupportCode;
        this.AbilityMovementCode = pu.AbilityMovementCode;
    }

    void InitializeNewPlayer()
    {
        this.CT = 0;
        this.AbleToFight = true;
        this.QuickFlag = false;
    }
    

    public void SetBaseStats()
    {
        PlayerUnitLevelStats ls = new PlayerUnitLevelStats();
        List<int> base_stats;

        base_stats = ls.SetBaseStats(this); //Debug.Log("class id is " + class_id);

        this.StatUnitMaxLife = base_stats[0];
        this.StatUnitLife = base_stats[0];
        this.StatUnitMaxMP = base_stats[1];
        this.StatUnitMP = base_stats[1];
        this.StatUnitSpeed = base_stats[2];
        this.StatUnitPA = base_stats[3];
        this.StatUnitMA = base_stats[4];

        this.StatUnitAgi = base_stats[5];
        this.StatUnitMove = base_stats[6];
        this.StatUnitJump = base_stats[7];
        this.StatUnitCEvade = base_stats[8];
        
        CalculateTotalStats();
    }

    void SetBaseStats(ClassEditObject ce)
    {
        PlayerUnitLevelStats ls = new PlayerUnitLevelStats();
        List<int> base_stats;

        base_stats = ls.GetCEBaseStats(ce,this); //Debug.Log("class id is " + class_id);

        this.StatUnitMaxLife = base_stats[0];
        this.StatUnitLife = base_stats[0];
        this.StatUnitMaxMP = base_stats[1];
        this.StatUnitMP = base_stats[1];
        this.StatUnitSpeed = base_stats[2];
        this.StatUnitPA = base_stats[3];
        this.StatUnitMA = base_stats[4];

        this.StatUnitAgi = base_stats[5];
        this.StatUnitMove = base_stats[6];
        this.StatUnitJump = base_stats[7];
        this.StatUnitCEvade = base_stats[8];

        CalculateTotalStats();
    }

    //convenience function when equipping/unequipping
    //probably want an inbattle version and a by stat version at some point
    //isCombat true for when destroying/stealiing equipment
    public void CalculateTotalStats(bool isCombat = false)
    {
        this.StatTotalMove = this.StatUnitMove + this.StatItemMove;
        this.StatTotalJump = this.StatUnitJump + this.StatItemJump;
        this.StatTotalSpeed = this.StatUnitSpeed + this.StatItemSpeed;
        this.StatTotalMaxLife = this.StatUnitMaxLife + this.StatItemMaxLife;
        this.StatTotalMaxMP = this.StatUnitMaxMP + this.StatItemMaxMP;
        if ( isCombat)
        {
            if( this.StatTotalLife > this.StatUnitLife + this.StatItemLife)
            {
                this.StatTotalLife = this.StatUnitLife + this.StatItemLife;
            }
            if (this.StatTotalMP > this.StatUnitMP + this.StatItemMP)
            {
                this.StatTotalMP = this.StatUnitMP + this.StatItemMP;
            }
        }
        else
        {
            this.StatTotalLife = this.StatUnitLife + this.StatItemLife;
            this.StatTotalMP = this.StatUnitMP + this.StatItemMP;
        }
        this.StatTotalPA = this.StatUnitPA + this.StatItemPA;
        this.StatTotalAgi = this.StatUnitAgi + this.StatItemAgi;
        this.StatTotalMA = this.StatUnitMA + this.StatItemMA;
        this.StatTotalBrave = this.StatUnitBrave + this.StatItemBrave;
        this.StatTotalFaith = this.StatUnitFaith + this.StatItemFaith;
        this.StatTotalCunning = this.StatUnitCunning + this.StatItemCunning;
        this.StatTotalCEvade = this.StatUnitCEvade + this.StatItemCEvade;
        this.StatTotalPEvade = this.StatUnitPEvade + this.StatItemPEvade;
        this.StatTotalMEvade = this.StatUnitMEvade + this.StatItemMEvade;
    }

    public void CalculateTotalStat(int statType)
    {
        //Debug.Log("in calculate total stat top " + statType);
        if (statType == NameAll.STAT_TYPE_SPEED)
        {
            this.StatTotalSpeed = this.StatUnitSpeed + this.StatItemSpeed;
        }
        else if (statType == NameAll.STAT_TYPE_MA)
        {
            this.StatTotalMA = this.StatUnitMA + this.StatItemMA;
        }
        else if (statType == NameAll.STAT_TYPE_PA)
        {
            this.StatTotalPA = this.StatUnitPA + this.StatItemPA;
        }
        else if (statType == NameAll.STAT_TYPE_AGI)
        {
            this.StatTotalAgi = this.StatUnitAgi + this.StatItemAgi;
        }
        else if (statType == NameAll.STAT_TYPE_MOVE)
        {
            this.StatTotalMove = this.StatUnitMove + this.StatItemMove;
        }
        else if (statType == NameAll.STAT_TYPE_JUMP)
        {
            this.StatTotalJump = this.StatUnitJump + this.StatItemJump;
        }
        else if (statType == NameAll.STAT_TYPE_BRAVE)
        {
            this.StatTotalBrave = this.StatUnitBrave + this.StatItemBrave;
            if( this.StatTotalBrave > 100)
            {
                this.StatTotalBrave = 100;
            }
        }
        else if (statType == NameAll.STAT_TYPE_FAITH)
        {
            this.StatTotalFaith = this.StatUnitFaith + this.StatItemFaith;
            if (this.StatTotalFaith > 100)
            {
                this.StatTotalFaith = 100;
            }
        }
        else if (statType == NameAll.STAT_TYPE_CUNNING)
        {
            this.StatTotalCunning = this.StatUnitCunning + this.StatItemCunning;
            if (this.StatTotalCunning > 100)
            {
                this.StatTotalCunning = 100;
            }
        }
        else if (statType == NameAll.STAT_TYPE_MP)
        {
            this.StatTotalMP = this.StatUnitMP + this.StatItemMP;
        }
        else if (statType == NameAll.STAT_TYPE_HP)
        {
            this.StatTotalLife = this.StatUnitLife + this.StatItemLife;
        }
        else if (statType == NameAll.STAT_TYPE_MAX_MP)
        {
            this.StatTotalMaxMP = this.StatUnitMaxMP + this.StatItemMaxMP;
            Debug.Log("asdf " + this.StatTotalMaxMP + " " + this.StatTotalMP);
            if (this.StatTotalMaxMP < this.StatTotalMP)
                this.StatTotalMP = this.StatTotalMaxMP;
            Debug.Log("asdf " + this.StatTotalMaxMP + " " + this.StatTotalMP);
        }
        else if (statType == NameAll.STAT_TYPE_MAX_HP)
        {
            this.StatTotalMaxLife = this.StatUnitMaxLife + this.StatItemMaxLife;
            if (this.StatTotalMaxLife < this.StatTotalLife)
                this.StatTotalLife = this.StatTotalMaxLife;
        }
        else if (statType == NameAll.STAT_TYPE_C_EVADE)
        {
            this.StatTotalCEvade = this.StatUnitCEvade + this.StatItemCEvade;
        }
        else if (statType == NameAll.STAT_TYPE_P_EVADE)
        {
            this.StatTotalPEvade = this.StatUnitPEvade + this.StatItemPEvade;
        }
        else if (statType == NameAll.STAT_TYPE_M_EVADE)
        {
            this.StatTotalMEvade = this.StatUnitMEvade + this.StatItemMEvade;
        }
    }

    public void EquipItem(int item_id, int unit_slot)
    {
        int z1 = 0;
        if (unit_slot == NameAll.ITEM_SLOT_WEAPON)
        {
            z1 = NameAll.STAT_TYPE_WEAPON;
            if( ItemManager.Instance.IsUsingBothHands(item_id,this.AbilitySupportCode,this.ClassId))
            {
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
            }
        }
        else if (unit_slot == NameAll.ITEM_SLOT_OFFHAND)
        {
            z1 = NameAll.STAT_TYPE_OFFHAND;
            if (ItemManager.Instance.IsBothHandsWeapon(this.ItemSlotWeapon) )
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
            }
        }
        else if (unit_slot == NameAll.ITEM_SLOT_HEAD)
        {
            z1 = NameAll.STAT_TYPE_HEAD;
        }
        else if (unit_slot == NameAll.ITEM_SLOT_BODY)
        {
            z1 = NameAll.STAT_TYPE_BODY;
        }
        else if (unit_slot == NameAll.ITEM_SLOT_ACCESSORY)
        {
            z1 = NameAll.STAT_TYPE_ACCESSORY;
        }
        UnequipItem(z1);
		//need code here for removing offhand item if weapon uses both hands and removing two handed weapon if offhand uses it
		//if ( )
		//{
		//    UnequipItem(NameAll.ITEM_SLOT_OFFHAND);
		//}
		Debug.Log("trying to get item for item_id " + item_id);
        ItemObject ist = ItemManager.Instance.GetItemObjectById(item_id, unit_slot);
        //Since Equips only happen out of battle, no need to mess with the status manager here
        //StatusManager.Instance.RemoveStatusItems(this.TurnOrder, item_id, unit_slot);
        //StatusManager.Instance.AddStatusLastingById(this.TurnOrder, item_id, unit_slot);
        CalculateIndividualItemStats(1, ist, unit_slot);//slot needed for some of the evades

        if (unit_slot.Equals(NameAll.ITEM_SLOT_WEAPON) )
        {
            this.ItemSlotWeapon = item_id;
        }
        else if (unit_slot.Equals(NameAll.ITEM_SLOT_OFFHAND))
        {
            this.ItemSlotOffhand = item_id;
        }
        else if (unit_slot.Equals(NameAll.ITEM_SLOT_HEAD))
        {
            this.ItemSlotHead = item_id;
        }
        else if (unit_slot.Equals(NameAll.ITEM_SLOT_BODY))
        {
            this.ItemSlotBody = item_id;
        }
        else if (unit_slot.Equals(NameAll.ITEM_SLOT_ACCESSORY))
        {
            this.ItemSlotAccessory = item_id;
        }

    }

    public void UnequipItem( int statType, bool isCombat = false )
    {
        int unit_slot;
        if (statType == NameAll.STAT_TYPE_WEAPON)
        {
            unit_slot = NameAll.ITEM_SLOT_WEAPON;
        }
        else if (statType == NameAll.STAT_TYPE_OFFHAND)
        {
            unit_slot = NameAll.ITEM_SLOT_OFFHAND;
        }
        else if (statType == NameAll.STAT_TYPE_HEAD)
        {
            unit_slot = NameAll.ITEM_SLOT_HEAD;
        }
        else if (statType == NameAll.STAT_TYPE_BODY)
        {
            unit_slot = NameAll.ITEM_SLOT_BODY;
        }
        else 
        {
            unit_slot = NameAll.ITEM_SLOT_ACCESSORY;
        }

        ItemObject ist;
        int item_id;
        if ( unit_slot == NameAll.ITEM_SLOT_WEAPON )
        {
            item_id = this.ItemSlotWeapon;
            ist = ItemManager.Instance.GetItemObjectById(item_id,unit_slot);
            CalculateIndividualItemStats(-1, ist, unit_slot, isCombat);
            StatusManager.Instance.RemoveStatusLastingByUnit(this.TurnOrder, this.ItemSlotWeapon);
            this.ItemSlotWeapon = NameAll.FIST_EQUIP;
        }
        else if (unit_slot == NameAll.ITEM_SLOT_OFFHAND)
        {
            item_id = this.ItemSlotOffhand;
            ist = ItemManager.Instance.GetItemObjectById(item_id, unit_slot);
            CalculateIndividualItemStats(-1, ist, unit_slot, isCombat);
            StatusManager.Instance.RemoveStatusLastingByUnit(this.TurnOrder, this.ItemSlotOffhand);
            if(AbilityManager.Instance.IsInnateAbility(this.ClassId, NameAll.SUPPORT_DUAL_WIELD, NameAll.ABILITY_SLOT_SUPPORT)
                || AbilityManager.Instance.IsInnateAbility(this.ClassId, NameAll.SUPPORT_TWO_SWORDS, NameAll.ABILITY_SLOT_SUPPORT)
                || IsAbilityEquipped(NameAll.SUPPORT_DUAL_WIELD,NameAll.ABILITY_SLOT_SUPPORT)
                || IsAbilityEquipped(NameAll.SUPPORT_TWO_SWORDS, NameAll.ABILITY_SLOT_SUPPORT) )
            {
                this.ItemSlotOffhand = NameAll.FIST_EQUIP;
            }
            else
            {
                this.ItemSlotOffhand = NameAll.NO_EQUIP;
            }
            
        }
        else if (unit_slot == NameAll.ITEM_SLOT_HEAD)
        {
            item_id = this.ItemSlotHead;
            ist = ItemManager.Instance.GetItemObjectById(item_id, unit_slot);
            CalculateIndividualItemStats(-1, ist, unit_slot, isCombat);
            StatusManager.Instance.RemoveStatusLastingByUnit(this.TurnOrder, this.ItemSlotHead);
            this.ItemSlotHead = NameAll.NO_EQUIP;
        }
        else if (unit_slot == NameAll.ITEM_SLOT_BODY)
        {
            item_id = this.ItemSlotBody;
            ist = ItemManager.Instance.GetItemObjectById(item_id, unit_slot);
            CalculateIndividualItemStats(-1, ist, unit_slot, isCombat);
            StatusManager.Instance.RemoveStatusLastingByUnit(this.TurnOrder, this.ItemSlotBody);
            this.ItemSlotBody = NameAll.NO_EQUIP;
        }
        else if (unit_slot == NameAll.ITEM_SLOT_ACCESSORY)
        {
            item_id = this.ItemSlotAccessory;
            ist = ItemManager.Instance.GetItemObjectById(item_id, unit_slot);
            CalculateIndividualItemStats(-1, ist, unit_slot, isCombat);
            StatusManager.Instance.RemoveStatusLastingByUnit(this.TurnOrder, this.ItemSlotAccessory);
            this.ItemSlotAccessory = NameAll.NO_EQUIP;
        }
    }

    //public void clearEquipment(StatusLab statusLab)
    //{
    //    removeAllItemStatuses(statusLab);
    //    this.ItemSlotWeapon = NameAll.NO_EQUIP;
    //    this.ItemSlotOffhand = NameAll.NO_EQUIP;
    //    this.item_slot_both_hands = NameAll.NO_EQUIP;
    //    this.ItemSlotHead = NameAll.NO_EQUIP;
    //    this.ItemSlotBody = NameAll.NO_EQUIP;
    //    this.ItemSlotAccessory = NameAll.NO_EQUIP;
    //    setItemStatsToZero();
    //    CalculateTotalStats();
    //}

    private void NullEquipment()
    {
        UnequipItem(NameAll.STAT_TYPE_WEAPON);
        UnequipItem(NameAll.STAT_TYPE_OFFHAND);
        UnequipItem(NameAll.STAT_TYPE_HEAD);
        UnequipItem(NameAll.STAT_TYPE_BODY);
        UnequipItem(NameAll.STAT_TYPE_ACCESSORY);
        //this.ItemSlotWeapon = NameAll.FIST_EQUIP; //fist
        //this.ItemSlotOffhand = NameAll.NO_EQUIP;
        //this.ItemSlotHead = NameAll.NO_EQUIP;
        //this.ItemSlotBody = NameAll.NO_EQUIP;
        //this.ItemSlotAccessory = NameAll.NO_EQUIP;
        //setItemStatsToZero();
        CalculateTotalStats();
    }

    //public void removeAllItemStatuses(StatusLab statusLab)
    //{
    //    StatusManager.Instance.RemoveStatusLastingByUnit(this.TurnOrder, this.item_slot_both_hands);
    //    StatusManager.Instance.RemoveStatusLastingByUnit(this.TurnOrder, this.ItemSlotOffhand);
    //    StatusManager.Instance.RemoveStatusLastingByUnit(this.TurnOrder, this.ItemSlotWeapon);
    //    StatusManager.Instance.RemoveStatusLastingByUnit(this.TurnOrder, this.ItemSlotHead);
    //    StatusManager.Instance.RemoveStatusLastingByUnit(this.TurnOrder, this.ItemSlotBody);
    //    StatusManager.Instance.RemoveStatusLastingByUnit(this.TurnOrder, this.ItemSlotAccessory);
    //}

    //public void calculateAllItemStats(ItemLab il)
    //{
    //    ItemStats ist;
    //    setItemStatsToZero();
    //    if (!this.ItemSlotWeapon.Equals(NO_EQUIP))
    //    {
    //        ist = il.GetItemStatsById(this.ItemSlotWeapon);
    //        calculateIndividualItemStats(1, ist);
    //    }
    //    if (!this.ItemSlotOffhand.Equals(NO_EQUIP))
    //    {
    //        ist = il.GetItemStatsById(this.ItemSlotOffhand);
    //        calculateIndividualItemStats(1, ist);
    //    }
    //    if (!this.item_slot_both_hands.Equals(NO_EQUIP))
    //    {
    //        ist = il.GetItemStatsById(this.item_slot_both_hands);
    //        calculateIndividualItemStats(1, ist);
    //    }
    //    if (!this.ItemSlotHead.Equals(NO_EQUIP))
    //    {
    //        ist = il.GetItemStatsById(this.ItemSlotHead);
    //        calculateIndividualItemStats(1, ist);
    //    }
    //    if (!this.ItemSlotBody.Equals(NO_EQUIP))
    //    {
    //        ist = il.GetItemStatsById(this.ItemSlotBody);
    //        calculateIndividualItemStats(1, ist);
    //    }
    //    if (!this.ItemSlotAccessory.Equals(NO_EQUIP))
    //    {
    //        ist = il.GetItemStatsById(this.ItemSlotAccessory);
    //        calculateIndividualItemStats(1, ist);
    //    }
    //}

    public void CalculateIndividualItemStats(int add, ItemObject ist, int slot, bool isCombat = false)
    {
        //1 for addition, -1 for subtraction
        if( ist == null)
        {
            Debug.Log("ERROR, null item");
            return;
        }
        if (add == 1)
        {
            //private int stat_brave;
            //private int stat_c_evade;
            //private int stat_cunning;
            //private int stat_faith;
            //private int stat_life;
            //private int stat_jump;
            //private int stat_m_evade;
            //private int stat_ma;
            //private int stat_move;
            //private int stat_mp;
            //private int stat_p_evade;
            //private int stat_pa;
            //private int stat_speed;
            //private int stat_w_evade;
            //private int stat_wp;
            this.StatItemBrave += ist.StatBrave;
            this.StatItemCEvade += ist.StatCEvade;
            this.StatItemCunning += ist.StatCunning;
            this.StatItemFaith += ist.StatFaith;
            this.StatItemLife += ist.StatLife;
            this.StatItemMaxLife += ist.StatLife;
            this.StatItemJump += ist.StatJump;
            this.StatItemMEvade += ist.StatMEvade;
            this.StatItemMA += ist.StatMA;
            this.StatItemMove += ist.StatMove;
            this.StatItemMaxMP += ist.StatMP;
            this.StatItemMP += ist.StatMP;
            this.StatItemPEvade += ist.StatPEvade;
            this.StatItemPA += ist.StatPA;
            this.StatItemAgi += ist.StatAgi;
            this.StatItemSpeed += ist.StatSpeed;
            this.StatItemWEvade += ist.StatWEvade;
            //this.stat_item_wp += ist.GetStatWP();
            //this.stat_item_ = ist.GetStat();

            //item
            //private int stat_item_move;
            //private int stat_item_jump;
            //private int stat_item_speed;
            //private int stat_item_maxLife; //add this functionality versus life later
            //private int stat_item_life;
            //private int stat_item_mp;
            //private int stat_item_maxMP;
            //private int stat_item_pa;
            //private int stat_item_ma;
            //private int stat_item_brave;
            //private int stat_item_faith;
            //private int stat_item_cunning;
            //private int stat_item_class_evade;
            //private int stat_item_p_evade;
            //private int stat_item_m_evade;
            //private int stat_item_offhand_magic_evade;
            //private int stat_item_offhand_physical_evade;
            //private int stat_item_accessory_magic_evade;
            //private int stat_item_accessory_physical_evade;
            //private int stat_item_weapon_evade;

            //accessory and offhands have individual evades
            if ( slot == NameAll.ITEM_SLOT_OFFHAND )
            {
                this.StatItemOffhandMEvade += ist.StatMEvade;
                this.StatItemOffhandPEvade += ist.StatPEvade;
            }
            else if (slot == NameAll.ITEM_SLOT_ACCESSORY)
            {
                this.StatItemAccessoryMEvade += ist.StatMEvade;
                this.StatItemAccessoryPEvade += ist.StatPEvade;
            }
            CalculateTotalStats(isCombat);
        }
        else if (add == -1)
        {
            this.StatItemBrave -= ist.StatBrave;
            this.StatItemCEvade -= ist.StatCEvade;
            this.StatItemCunning -= ist.StatCunning;
            this.StatItemFaith -= ist.StatFaith;
            this.StatItemLife -= ist.StatLife;
            this.StatItemMaxLife -= ist.StatLife;
            this.StatItemJump -= ist.StatJump;
            this.StatItemMEvade -= ist.StatMEvade;
            this.StatItemMA -= ist.StatMA;
            this.StatItemMove -= ist.StatMove;
            this.StatItemMaxMP -= ist.StatMP;
            this.StatItemMP -= ist.StatMP;
            this.StatItemPEvade -= ist.StatPEvade;
            this.StatItemPA -= ist.StatPA;
            this.StatItemAgi -= ist.StatAgi;
            this.StatItemSpeed -= ist.StatSpeed;
            this.StatItemWEvade -= ist.StatWEvade;
            //this.stat_item_wp -= ist.GetStatWP();

            //accessory and offhands have individual evades
            if (slot == NameAll.ITEM_SLOT_OFFHAND)
            {
                this.StatItemOffhandMEvade -= ist.StatMEvade;
                this.StatItemOffhandPEvade -= ist.StatPEvade;
            }
            else if (slot == NameAll.ITEM_SLOT_ACCESSORY)
            {
                this.StatItemAccessoryMEvade -= ist.StatMEvade;
                this.StatItemAccessoryPEvade -= ist.StatPEvade;
            }
            CheckItemStatsForNegative();
            CalculateTotalStats(isCombat);
        }
    }

    public void CheckItemStatsForNegative()
    {
        if (this.StatItemMove < 0)
        {
            this.StatItemMove = 0;
        }
        if (this.StatItemJump < 0)
        {
            this.StatItemJump = 0;
        }
        if (this.StatItemSpeed < 0)
        {
            this.StatItemSpeed = 0;
        }
        if (this.StatItemMaxLife < 0)
        {
            this.StatItemMaxLife = 0;
        }
        if (this.StatItemLife < 0)
        {
            this.StatItemLife = 0;
        }
        if (this.StatItemMP < 0)
        {
            this.StatItemMP = 0;
        }
        if (this.StatItemMaxMP < 0)
        {
            this.StatItemMaxMP = 0;
        }
        if (this.StatItemPA < 0)
        {
            this.StatItemPA = 0;
        }
        if (this.StatItemAgi < 0)
        {
            this.StatItemAgi = 0;
        }
        if (this.StatItemMA < 0)
        {
            this.StatItemMA = 0;
        }
    }

    public void SetItemStatsToZero()
    {
        this.StatItemMove = 0;
        this.StatItemJump = 0;
        this.StatItemSpeed = 0;
        this.StatItemMaxLife = 0;
        this.StatItemLife = 0;
        this.StatItemMP = 0;
        this.StatItemMaxMP = 0;
        this.StatItemPA = 0;
        this.StatItemAgi = 0;
        this.StatItemMA = 0;
    }

    //private void SetClassName()
    //{
    //    //classes, 1 Chemist, 2 Knight, 3 archer, 4 squire, 5 thief, 6 ninja, 7 monk,
    //    //8 priest, 9 wizard, 10 time mage, 11 summoner, 12 mediator, 13 oracle,
    //    //14 geomancer, 15 lancer, 16 samurai, 17 calculator, 18 bard, 19 dancer, 20 mime
    //    ArrayList<string> classNames = new ArrayList<string>();
    //    classNames.add("Chemist");
    //    classNames.add("Knight");
    //    classNames.add("Archer");
    //    classNames.add("Squire");
    //    classNames.add("Thief");

    //    classNames.add("Ninja");
    //    classNames.add("Monk");
    //    classNames.add("Priest");
    //    classNames.add("Wizard");
    //    classNames.add("Time Mage");

    //    classNames.add("Summoner");
    //    classNames.add("Mediator");
    //    classNames.add("Oracle");
    //    classNames.add("Geomancer");
    //    classNames.add("Lancer");

    //    classNames.add("Samurai");
    //    classNames.add("Calculator");
    //    classNames.add("Bard");
    //    classNames.add("Dancer");
    //    classNames.add("Mime");

    //    this.class_name = classNames.Get(this.ClassId - 1);
    //}

    //public void setAbility(int ability_id, string type, int clear)
    //{
    //    if (type.Equals("secondary"))
    //    {
    //        if (clear == 1)
    //        {
    //            this.AbilitySecondaryCode = 0;
    //        }
    //        else {
    //            this.AbilitySecondaryCode = ability_id;
    //        }
    //    }
    //    else if (type.Equals("reaction"))
    //    {
    //        if (clear == 1)
    //        {
    //            this.AbilityReactionCode = 0;
    //        }
    //        else {
    //            this.AbilityReactionCode = ability_id;
    //        }
    //    }
    //    else if (type.Equals("support"))
    //    {
    //        if (clear == 1)
    //        {
    //            this.AbilitySupportCode = 0;
    //        }
    //        else {
    //            this.AbilitySupportCode = ability_id;
    //        }
    //    }
    //    else if (type.Equals("movement"))
    //    {
    //        if (clear == 1)
    //        {
    //            this.AbilityMovementCode = 0;
    //        }
    //        else {
    //            this.AbilityMovementCode = ability_id;
    //        }
    //    }
    //}

    public void ClearAbilities()
    {
        this.AbilitySecondaryCode = 0;
        this.AbilityReactionCode = 0;
        this.AbilitySupportCode = 0;
        this.AbilityMovementCode = 0;
    }

  //  public void SetZodiacName()
  //  {
  //      /*<< " select Zodiac:\n" << "1.) Capricorn\t2.) Aquarius\n"
		//<< "3.) Pisces\t4.) Aries\n"
		//<< "5.) Taurus\t6.) Gemini\n"
		//<< "7.) Cancer\t8.) Leo\n"
		//<< "9.) Virgo\t10.) Libra\n"
		//<< "11.) Scorpio\t12.) Sagittarius\n")*/
  //      ArrayList<string> classNames = new ArrayList<string>();
  //      classNames.add("Capricorn");
  //      classNames.add("Aquarius");
  //      classNames.add("Pisces");
  //      classNames.add("Aries");
  //      classNames.add("Taurus");
  //      classNames.add("Gemini");
  //      classNames.add("Cancer");
  //      classNames.add("Leo");
  //      classNames.add("Virgo");
  //      classNames.add("Libra");
  //      classNames.add("Scorpio");
  //      classNames.add("Sagittarius");

  //      this.zodiac_name = classNames.Get(this.ZodiacInt - 1);
  //  }
  
    //public int GetItemStat(string slot, string stat_type)
    //{
    //    string zstring = "";
    //    if (slot.Equals("armor"))
    //    {
    //        zstring = this.ItemSlotBody;
    //    }
    //    else if (slot.Equals("helm"))
    //    {
    //        zstring = this.ItemSlotHead;
    //    }
    //    if (slot.Equals("accessory"))
    //    {
    //        zstring = this.ItemSlotAccessory;
    //    }
    //    int z1 = ItemLab.Get().GetItemStatById(zstring, slot, stat_type);
    //    return z1;
    //}

    public int GetWeaponPower(bool battleSkill = false, bool twoHandsWP = false) //only set this to true for display and for attacks, 
    {
        //for battleSkill, barehanded attacks don't use pa as wp, everything else (I think) does
        int z1;

        //not sure of slot, starts at both hands
        //For barehanded units, WP = 0 in any equations that use WP (e.g., BATTLE SKILL success rate).
        

        if( battleSkill)
        {
            if (IsFistEquipped() )
            {
                return 0;
            }
            z1 = ItemManager.Instance.GetItemStatById(this.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON, NameAll.ITEM_OBJECT_STAT_WP );
            return z1;
        }

        if( IsFistEquipped())
        {
            z1 = this.StatTotalPA;
        } else
        {
            z1 = ItemManager.Instance.GetItemStatById(this.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON, NameAll.ITEM_OBJECT_STAT_WP);
        }

        if (twoHandsWP && this.ItemSlotOffhand == NameAll.NO_EQUIP && ItemManager.Instance.IsTwoHands(this.ItemSlotWeapon))
        {
            if ( IsAbilityEquipped(NameAll.SUPPORT_MIGHTY_GRIP, NameAll.ABILITY_SLOT_SUPPORT) || IsAbilityEquipped(NameAll.SUPPORT_TWO_HANDS, NameAll.ABILITY_SLOT_SUPPORT)
                || AbilityManager.Instance.IsInnateAbility( this.ClassId,NameAll.SUPPORT_MIGHTY_GRIP,NameAll.ABILITY_SLOT_SUPPORT) ) //innate two hands and empty offhand
            {
                z1 *= 2;
            }
        }

        return z1;
    }

    //public int GetWeaponEvade(string pu_slot)
    //{
    //    //not sure of slot, starts at both hands
    //    //For barehanded units, WP = 0 in any equations that use WP (e.g., BATTLE SKILL success rate).
    //    if (isFistEquipped())
    //    {
    //        return 0;
    //    }
    //    int z1 = 0;
    //    string zstring = GetWeaponstring(pu_slot, 0);
    //    if (!zstring.Equals("") && !zstring.Equals("fist"))
    //    {
    //        z1 = ItemLab.Get().GetItemStatById(zstring, "hand", "w_evade");//weapon is in a hand slot, 1st stat is weapon power
    //    }
    //    return z1;
    //}

    //public string GetWeaponstring(string pu_slot, int two_weapons)
    //{
    //    //default is using 0 so if this changes...
    //    string zstring = "";
    //    if (isFistEquipped())
    //    {
    //        zstring = "fist";
    //        return zstring;
    //    }
    //    if (pu_slot.Equals("") || pu_slot.Equals("unknown"))
    //    {
    //        //checks to see if weapon in both hands
    //        //if not checks to see if weapon in right hand
    //        //if not checks to see if weapon in left hand
    //        //if not assumes none and empty string returned
    //        zstring = this.item_slot_both_hands;
    //        if (!zstring.Equals(NO_EQUIP))
    //        {
    //            return zstring;
    //        }
    //        zstring = this.ItemSlotWeapon;
    //        if (!zstring.Equals(NO_EQUIP) && !zstring.substring(0, 2).Equals("io"))
    //        {
    //            return zstring;
    //        }
    //        zstring = this.ItemSlotOffhand;
    //        if (!zstring.Equals(NO_EQUIP) && !zstring.substring(0, 2).Equals("io"))
    //        {
    //            return zstring;
    //        }

    //    }
    //    else if (pu_slot.Equals("both_hands"))
    //    {
    //        zstring = this.item_slot_both_hands;
    //        if (!zstring.Equals(NO_EQUIP))
    //        {
    //            return zstring;
    //        }
    //    }
    //    else if (pu_slot.Equals("right_hand"))
    //    {
    //        zstring = this.ItemSlotWeapon;
    //        if (!zstring.Equals(NO_EQUIP) && !zstring.substring(0, 2).Equals("io"))
    //        {
    //            return zstring;
    //        }
    //    }
    //    else if (pu_slot.Equals("left_hand"))
    //    {
    //        zstring = this.ItemSlotOffhand;
    //        if (!zstring.Equals(NO_EQUIP) && !zstring.substring(0, 2).Equals("io"))
    //        {
    //            return zstring;
    //        }
    //    }
    //    zstring = "";
    //    return zstring;
    //}

    //public string GetOffhandstring()
    //{
    //    //default is using 0 so if this changes...
    //    string zstring;
    //    zstring = this.ItemSlotOffhand;
    //    if (zstring.Substring(0, 2).Equals("io"))
    //    {
    //        return zstring;
    //    }
    //    return zstring;
    //}

    public bool IsFistEquipped()
    {
        if( this.ItemSlotWeapon == NameAll.FIST_EQUIP)
        {
            return true; 
        }
        return false;
    }

    //public bool IsElementStrengthenedByItem(string element )
    //{

    //    if (StatusManager.Instance.CheckIfElementStrengthenedByItemId(element, this.ItemSlotWeapon))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementStrengthenedByItemId(element, this.ItemSlotOffhand))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementStrengthenedByItemId(element, this.ItemSlotHead))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementStrengthenedByItemId(element, this.ItemSlotBody))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementStrengthenedByItemId(element, this.ItemSlotAccessory))
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    //public bool IsElementWeakByItem(string element )
    //{

    //    if (StatusManager.Instance.CheckIfElementWeakByItemId(element, this.ItemSlotWeapon))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementWeakByItemId(element, this.ItemSlotOffhand))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementWeakByItemId(element, this.ItemSlotHead))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementWeakByItemId(element, this.ItemSlotBody))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementWeakByItemId(element, this.ItemSlotAccessory))
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    //public bool IsElementHalfByItem(string element )
    //{

    //    if (StatusManager.Instance.CheckIfElementHalfByItemId(element, this.ItemSlotWeapon))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementHalfByItemId(element, this.ItemSlotOffhand))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementHalfByItemId(element, this.ItemSlotHead))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementHalfByItemId(element, this.ItemSlotBody))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementHalfByItemId(element, this.ItemSlotAccessory))
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    //public bool IsElementAbsorbByItem(string element )
    //{

    //    if (StatusManager.Instance.CheckIfElementAbsorbByItemId(element, this.ItemSlotWeapon))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementAbsorbByItemId(element, this.ItemSlotOffhand))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementAbsorbByItemId(element, this.ItemSlotHead))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementAbsorbByItemId(element, this.ItemSlotBody))
    //    {
    //        return true;
    //    }
    //    if (StatusManager.Instance.CheckIfElementAbsorbByItemId(element, this.ItemSlotAccessory))
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    public int GetWeaponElementType()
    {
        return ItemManager.Instance.GetWeaponElementById(this.ItemSlotWeapon);
    }

    //public string GetWeaponType(string slot)
    //{
    //    string zstring = "none";
    //    if (slot.Equals("right_hand"))
    //    {
    //        zstring = this.ItemSlotWeapon;
    //    }
    //    else if (slot.Equals("left_hand"))
    //    {
    //        zstring = this.ItemSlotOffhand;
    //    }
    //    else if (slot.Equals("both_hands"))
    //    {
    //        zstring = this.item_slot_both_hands;
    //    }
    //    zstring = ItemLab.Get().GetWeaponTypeById(zstring);
    //    return zstring;
    //}

    public bool IsAbilityEquipped(int abilityId, int abilitySlot)
    {
        if( abilitySlot == NameAll.ABILITY_SLOT_SUPPORT)
        {
            if (abilityId == this.AbilitySupportCode)
                return true;
        }
        else if( abilitySlot == NameAll.ABILITY_SLOT_REACTION)
        {
            if (abilityId == this.AbilityReactionCode)
                return true;
        }
        else if( abilitySlot == NameAll.ABILITY_SLOT_MOVEMENT)
        {
            if (abilityId == this.AbilityMovementCode)
                return true;
        }
        else if( abilitySlot == NameAll.ABILITY_SLOT_SECONDARY)
        {
            if (abilityId == this.AbilitySecondaryCode)
                return true;
        }
        else if (abilitySlot == NameAll.ABILITY_SLOT_PRIMARY)
        {
            if (abilityId == this.ClassId)
                return true;
        }
        return false;
    }

   
    //public void addCT(StatusLab sl)
    public void AddCT()
    {
        //doing simple version for now until statusManager is implemented
        if( this.CT < 100)
        {
            if( !StatusManager.Instance.IsCTHalted(this.TurnOrder))
            {
                int z1 = this.StatTotalSpeed;
                if( IsAbilityEquipped(NameAll.SUPPORT_NATURAL_HIGH, NameAll.ABILITY_SLOT_SUPPORT) && StatusManager.Instance.IsPositiveStatus(this.TurnOrder))
                {
                    z1 += 1;
                }
                else if ( IsAbilityEquipped(NameAll.SUPPORT_UNNATURAL_HIGH, NameAll.ABILITY_SLOT_SUPPORT) && StatusManager.Instance.IsNegativeStatus(this.TurnOrder))
                {
                    z1 += 2;
                }
                z1 = StatusManager.Instance.ModifySpeed(this.TurnOrder, z1);
                this.CT += z1;
            }  
        }
    }

    //only using this for quick for now but maybe for story shit it coudl be used
    public void SetCT(int amount, string statusId = "")
    {
        if( statusId.Equals("quick"))
        {
            if( this.CT < amount)
            {
                this.CT = amount;
            }   
        }
        else
        {
            this.CT = amount;
        }
    }

    //endturn CT
    public void EndTurnCT(CombatTurn turn)
    {
        if( turn.hasUnitActed && turn.hasUnitMoved)
        {
            DecrementCT(100);
        }
        else if( turn.hasUnitActed || turn.hasUnitMoved)
        {
            DecrementCT(80);
        }
        else
        {
            DecrementCT(60);
        }
        ClampCT();
    }

    //endturn CT
    public void EndTurnCT(bool hasUnitActed, bool hasUnitMoved)
    {
        if (hasUnitActed && hasUnitMoved)
        {
            DecrementCT(100);
        }
        else if (hasUnitActed || hasUnitMoved)
        {
            DecrementCT(80);
        }
        else
        {
            DecrementCT(60);
        }
        ClampCT();
    }

    //public void TakeTurn() //based on phaseActor rather than specific
    //{
    //    int z2 = SceneCreate.phaseActor;
    //    int z1 = 60;
    //    if (z2 == 3)
    //    {
    //        z1 = 100;
    //    }
    //    else if (z2 == 1 || z2 == 2)
    //    {
    //        z1 = 80;
    //        if (StatusManager.Instance.IfStatusByUnitAndId(this.TurnOrder, NameAll.STATUS_ID_DONT_MOVE, true) ||
    //            StatusManager.Instance.IfStatusByUnitAndId(this.TurnOrder, NameAll.STATUS_ID_DONT_ACT, true))
    //        {
    //            z1 += 20;
    //        }
    //    }
    //    else
    //    {
    //        if (StatusManager.Instance.IfStatusByUnitAndId(this.TurnOrder, NameAll.STATUS_ID_DONT_MOVE, true) &&
    //            StatusManager.Instance.IfStatusByUnitAndId(this.TurnOrder, NameAll.STATUS_ID_DONT_ACT, true))
    //        {
    //            z1 += 40;
    //        }
    //        else if (StatusManager.Instance.IfStatusByUnitAndId(this.TurnOrder, NameAll.STATUS_ID_DONT_MOVE, true) ||
    //            StatusManager.Instance.IfStatusByUnitAndId(this.TurnOrder, NameAll.STATUS_ID_DONT_ACT, true))
    //        {
    //            z1 += 20;
    //        }
    //    }

    //    this.CT -= z1;
    //    if (this.CT < 0)
    //    {
    //        this.CT = 0;
    //    }
    //    else if (this.CT > 60)
    //    {
    //        this.CT = 60;//ct cannot be over 60 when ending a turn
    //    }
    //    //Debug.Log("post take turn unit id and ct are: " + this.TurnOrder + " " + this.CT);
    //}


    //public void SetDirection(int direction)
    //{
    //    this.facing_direction = direction % 4;
    //    if( direction == 0)
    //    {
    //        this.Dir = Directions.North;
    //    }
    //    else if( direction == 1)
    //    {
    //        this.Dir = Directions.East;
    //    }
    //    else if( direction == 2)
    //    {
    //        this.Dir = Directions.South;
    //    }
    //    else
    //    {
    //        this.Dir = Directions.West;
    //    }
    //}

    //called in playermanager which is called by game loop
    public bool IsTurnActable()
    {
        if (this.CT >= 100)
        {
            if( StatusManager.Instance.IsTurnActable(this.TurnOrder))
            {
                return true;
            }
        }
        return false;
    }

    
    void DecrementCT(int ct1)
    {
        this.CT -= ct1;
        //clamp value elsewhere
        //if (this.CT < 0)
        //{
        //    this.CT = 0;
        //}
    }
    
    void ClampCT()
    {
        Mathf.Clamp(this.CT, 0, 60);
        //if( this.CT < 0)
        //{
        //    this.CT = 0;
        //}
        //else if( this.CT > 60)
        //{
        //    this.CT = 60;
        //}
    }

    public void RemoveMP(int mp)
    {
        this.StatTotalMP -= mp;
        if (this.StatTotalMP < 0)
        {
            this.StatTotalMP = 0;
        }
    }

    //public List<int> GetXYZ()
    //{
    //    List<int> temp = new List<int>();
    //    temp.Add(this.current_x);
    //    temp.Add(this.current_y);
    //    temp.Add(this.current_z); //this is the non float I think
    //    return temp;
    //}

    public void SetAbleToFight(bool able)
    {
        this.AbleToFight = able;
        if(!able)
        {
            this.PostNotification(DidChangeNotification( ));
        }
    }

    public void SetQuickFlag(bool quickFlag)
    {
        this.QuickFlag = quickFlag;
    }

    public bool IsQuickFlag()
    {
        return this.QuickFlag;
    }

    public void DefectFromTeam()
    {
        //Debug.Log("invite hit..., in playerunit");
        if (this.TeamId == 2)
        {
            this.TeamId = 3;
        }
        else if (this.TeamId == 3)
        {
            this.TeamId = 2;
        }
        //updates the marker, not used because the marker needs the unit tile 
        //MapTileManager.Instance.ChangeMarker( this.TeamId,this.TurnOrder);
    }

    //public void moveXYZ(List<int> newLocation)
    //{
    //    this.current_x = newLocation[0];
    //    this.current_y = newLocation[1];
    //    this.current_z = newLocation[2];
    //}

    //called in player manager from status manager when removing dead status
    public void ReturnToLife(int effect)
    {
        this.SetHP(effect, 0, NameAll.ITEM_ELEMENTAL_NONE); Debug.Log("adding life to a unit " + effect);//sets the critical status
        this.SetAbleToFight(true);
        //PlayerManager.Instance.SetPlayerObjectAnimation(this.TurnOrder, "rise", false); //calling this in statusmanager
    }

    public void SetHP(int effect, int remove_stat, int elemental_type = 0, bool removeAll = false, bool isSaveCombatLog = false, SpellName sn = null,
		PlayerUnit actor = null, int rollResult = -1919, int rollChance = -1919, int combatLogSubType = -1919)
    {
		//parameters used for Combat logging: bool isSaveCombatLog = false, SpellName sn = null, PlayerUnit actor = null, int rollValue = -1919, int rollChance = -1919
		//Debug.Log("doing a call to set hp. effect, removeStat, elementalType, removeAll " + effect + ", " + remove_stat + ", " + elemental_type + ", " + removeAll);
		if ( removeAll)
        {
            this.StatTotalLife = 0; //killing the unit like with death sentence
            //DON'T CALL ADD DEAD, ADD DEAD CALLS THIS
			if(isSaveCombatLog)
			{
				if (combatLogSubType == NameAll.NULL_INT)
					combatLogSubType = NameAll.COMBAT_LOG_SUBTYPE_SET_HP_REMOVE_ALL;
				CombatTurn logTurn = new CombatTurn();
				logTurn.actor = actor;
				logTurn.spellName = sn;
				logTurn.targetUnitId = this.TurnOrder;
				PlayerManager.Instance.AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_ACTION, combatLogSubType, logTurn, rollResult,
					rollChance, this.StatTotalLife);
			}
        }
        else
        {
            
            bool critCheckPrior = IsCritical(); //Debug.Log("remove stat is " + remove_stat);
            int hpStart = this.StatTotalLife;
            if (remove_stat == NameAll.REMOVE_STAT_HEAL)
            {
                //handling undead
                //Debug.Log("remove stat is " + remove_stat + " " + StatusManager.Instance.IfStatusByUnitAndId(this.TurnOrder,NameAll.STATUS_ID_UNDEAD));
                if (elemental_type == NameAll.ITEM_ELEMENTAL_UNDEAD && StatusManager.Instance.IsUndead(this.TurnOrder) ) 
                {
                    this.StatTotalLife -= effect;
                    AddDead(sn, actor, rollResult, rollChance, combatLogSubType);
                    PlayerManager.Instance.ShowFloatingText(this.TurnOrder, 1, "" + effect); //damage
                }
                else {

                    this.StatTotalLife += effect; //Debug.Log("adding life effect, statTotalLife, statTotalMaxLife " + effect + "," + this.StatTotalLife + "," + this.StatTotalMaxLife);
                    if (this.StatTotalLife > this.StatTotalMaxLife)
                    {
                        this.StatTotalLife = this.StatTotalMaxLife;
                    }
                    PlayerManager.Instance.ShowFloatingText(this.TurnOrder, 2, "" + effect); //heal
                }

            }
            else if( remove_stat == NameAll.REMOVE_STAT_REMOVE )
            {
                //Debug.Log("remove stat is " + remove_stat);
                if ( effect < 0) //damage type can be healing if the type is absorbed
                {
                    this.StatTotalLife -= effect;
                    if (this.StatTotalLife > this.StatTotalMaxLife)
                    {
                        this.StatTotalLife = this.StatTotalMaxLife;
                    }
                    effect = effect * -1; //for the display
                    PlayerManager.Instance.ShowFloatingText(this.TurnOrder, 2, "" + effect); //heal
                }
                else //does damage
                {
                    this.StatTotalLife -= effect;
					AddDead(sn, actor, rollResult, rollChance, combatLogSubType);
					PlayerManager.Instance.ShowFloatingText(this.TurnOrder, 1, "" + effect); //damage
                }
            }
            else if( remove_stat == NameAll.REMOVE_STAT_ABSORB)
            {
                Debug.Log("remove stat is " + remove_stat);
                //this is the damage done from the damage/absorb part, the typical heal is ordered as a heal elsewhere
                if (elemental_type == NameAll.ITEM_ELEMENTAL_UNDEAD && StatusManager.Instance.IsUndead(this.TurnOrder)) 
                {
                    this.StatTotalLife += effect;
                    if (this.StatTotalLife > this.StatTotalMaxLife)
                    {
                        this.StatTotalLife = this.StatTotalMaxLife;
                    }
                    PlayerManager.Instance.ShowFloatingText(this.TurnOrder, 2, "" + effect); //heal
                }
                else
                {
                    this.StatTotalLife -= effect;
					AddDead(sn, actor, rollResult, rollChance, combatLogSubType);
					PlayerManager.Instance.ShowFloatingText(this.TurnOrder, 1, "" + effect); //damage
                }
            }

            if( critCheckPrior != IsCritical() )
            {
                //critical status changed, modify
                if(critCheckPrior) //was critical before, no longer (includes dead (though that already removes critical)
                {
                    StatusManager.Instance.RemoveStatusTickByUnit(this.TurnOrder, NameAll.STATUS_ID_CRITICAL, isBeingCalledFromPlayerUnit:true); //PlayerManager.Instance.RemoveFromStatusList(this.TurnOrder, "critical");
                } 
                else
                {
                    StatusManager.Instance.AddStatusAndOverrideOthers(0,this.TurnOrder, NameAll.STATUS_ID_CRITICAL); //PlayerManager.Instance.AddToStatusList(this.TurnOrder, "critical");
                }
            }

			if (isSaveCombatLog)
			{
				if (combatLogSubType == NameAll.NULL_INT)
					combatLogSubType = NameAll.COMBAT_LOG_SUBTYPE_SET_HP_REMOVE_ALL;
				CombatTurn logTurn = new CombatTurn();
				logTurn.actor = actor;
				logTurn.spellName = sn;
				logTurn.targetUnitId = this.TurnOrder;
				PlayerManager.Instance.AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_ACTION, combatLogSubType, logTurn, rollResult,
					rollChance, effect, this.StatTotalLife);
			}

			if ( this.StatTotalLife <= hpStart && this.StatTotalLife > 0)
            {
                //no need to do a check since if they don't have them then they won't be removed
                StatusManager.Instance.RemoveStatusTickByUnit(this.TurnOrder, NameAll.STATUS_ID_CHARM, isBeingCalledFromPlayerUnit: true);
                StatusManager.Instance.RemoveStatusTickByUnit(this.TurnOrder, NameAll.STATUS_ID_SLEEP, isBeingCalledFromPlayerUnit: true);
                StatusManager.Instance.RemoveStatusTickByUnit(this.TurnOrder, NameAll.STATUS_ID_CONFUSION, isBeingCalledFromPlayerUnit: true);
            }

        }
    }

    private bool IsCritical()
    {
        if (this.StatTotalLife > 0 && this.StatTotalLife <= ( this.StatTotalMaxLife + 4 ) / 5)
        {
            return true;
        }
        return false;
    }
    

    //called in setHP, adds dead
	//all arguments are for combat logging
    private void AddDead(SpellName sn = null, PlayerUnit actor=null, int rollResult = -1919, int rollChance = -1919, int combatLogSubType = -1919)
    {
        if (this.StatTotalLife <= 0)
        {
            this.StatTotalLife = 0;
            if( !StatusManager.Instance.IfStatusByUnitAndId(this.TurnOrder,NameAll.STATUS_ID_DEAD))
            {
                StatusManager.Instance.AddDead(this.TurnOrder, sn, actor, rollResult, rollChance, combatLogSubType); //StatusManager.Instance.addDead(this.TurnOrder, SpellLab.Get(), PlayerUnitLab.Get()); //this sets status to unable to fight
            }
        }
    }


    public void AlterStat(int alterStat, int effect, int statType, int elementalType, bool isSaveCombatLog = false, SpellName sn = null,
		PlayerUnit actor = null, int rollResult = -1919, int rollChance = -1919, int combatLogSubType = -1919)
    {
		//only used for combat log: bool isSaveCombatLog = false, SpellName sn = null, PlayerUnit actor = null, int rollResult = -1919, int rollChance = -1919

		int max_speed;
        int max_stat;
        int max_stat2;
        int max_move;
        int min_stat;
        int min_stat2;
        //Debug.Log("in PU alterState" + effect);
        if ( this.ClassId < 100)
        {
            max_speed = 99;
            max_stat = 99;
            max_stat2 = 100;
            max_move = 99;
            min_stat = 1;
            min_stat2 = 1;
        }
        else
        {
            max_speed = 20;
            max_stat = 25;
            max_stat2 = 100;
            max_move = 99;
            min_stat = 2;
            min_stat2 = 2;
        }
        
        bool calculate_stats = false;
        if (alterStat == NameAll.REMOVE_STAT_REMOVE || alterStat == NameAll.REMOVE_STAT_ABSORB)
        { //1 removes the stat, 2 is absorb (dmg done here, heal done elsewhere)
            //subtracts
            if ( statType == NameAll.STAT_TYPE_MP )
            {
                this.StatTotalMP -= effect;
                if (this.StatTotalMP <= 0)
                {
                    this.StatTotalMP = 0;
                }
            }
            else if (statType == NameAll.STAT_TYPE_PA)
            {
                this.StatUnitPA -= effect;
                if (this.StatUnitPA < min_stat)
                {
                    this.StatUnitPA = min_stat;
                }
                calculate_stats = true;
            }
            else if (statType == NameAll.STAT_TYPE_AGI)
            {
                this.StatUnitAgi -= effect;
                if (this.StatUnitAgi < min_stat)
                {
                    this.StatUnitAgi = min_stat;
                }
                calculate_stats = true;
            }
            else if (statType == NameAll.STAT_TYPE_MA)
            {
                this.StatUnitMA -= effect;
                if (this.StatUnitMA < min_stat)
                {
                    this.StatUnitMA = min_stat;
                }
                calculate_stats = true;
            }
            else if (statType == NameAll.STAT_TYPE_SPEED)
            {
                this.StatUnitSpeed -= effect;
                if (this.StatUnitSpeed < min_stat)
                {
                    this.StatUnitSpeed = min_stat;
                }
                calculate_stats = true;
            }
            else if (statType == NameAll.STAT_TYPE_FAITH)
            {
                this.StatUnitFaith -= effect;
                if (this.StatUnitFaith < min_stat2)
                {
                    this.StatUnitFaith = min_stat2;
                }
                calculate_stats = true;
            }
            else if (statType == NameAll.STAT_TYPE_BRAVE)
            {
                //Debug.Log("removing brave...");
                this.StatUnitBrave -= effect;
                if (this.StatUnitBrave < min_stat2)
                {
                    this.StatUnitBrave = min_stat2;
                }
                calculate_stats = true;
                if (this.StatUnitBrave + this.StatItemBrave <= 10)
                {
                    //Debug.Log("adding chicken...");
                    StatusManager.Instance.AddStatusAndOverrideOthers(0, this.TurnOrder, NameAll.STATUS_ID_CHICKEN);
                }
            }
            else if (statType == NameAll.STAT_TYPE_CUNNING)
            {
                this.StatUnitCunning -= effect;
                if (this.StatUnitCunning < min_stat2)
                {
                    this.StatUnitCunning = min_stat2;
                }
                calculate_stats = true;
            }
            else if (statType == NameAll.STAT_TYPE_MOVE)
            {
                this.StatUnitMove -= effect;
                if (this.StatUnitMove < max_move)
                {
                    this.StatUnitMove = min_stat;
                }
                calculate_stats = true;
            }
            else if (statType == NameAll.STAT_TYPE_JUMP)
            {
                this.StatUnitJump -= effect;
                if (this.StatUnitJump < max_move)
                {
                    this.StatUnitJump = min_stat;
                }
                calculate_stats = true;
            }
            else if (statType == NameAll.STAT_TYPE_BODY)
            {
                if (IsEquipmentBreakAllowed(statType))
                    UnequipItem(statType, isCombat: true);
                else
                    PlayerManager.Instance.ShowFloatingText(this.TurnOrder,19, "Blocked");
            }
            else if (statType == NameAll.STAT_TYPE_HEAD)
            {
                if (IsEquipmentBreakAllowed(statType))
                    UnequipItem(statType, isCombat: true);
                else
                    PlayerManager.Instance.ShowFloatingText(this.TurnOrder, 19, "Blocked");
            }
            else if (statType == NameAll.STAT_TYPE_WEAPON)
            {
                if (IsEquipmentBreakAllowed(statType))
                {
                    UnequipItem(statType, isCombat: true);
                    PlayerManager.Instance.CheckForChargeRemove(this); //cancels charge when weapon broken
                }
                else
                    PlayerManager.Instance.ShowFloatingText(this.TurnOrder, 19, "Blocked");
            }
            else if (statType == NameAll.STAT_TYPE_OFFHAND)
            {
                if (IsEquipmentBreakAllowed(statType))
                    UnequipItem(statType, isCombat: true);
                else
                    PlayerManager.Instance.ShowFloatingText(this.TurnOrder, 19, "Blocked");
            }
            else if (statType == NameAll.STAT_TYPE_ACCESSORY)
            {
                if (IsEquipmentBreakAllowed(statType))
                    UnequipItem(statType, isCombat: true);
                else
                    PlayerManager.Instance.ShowFloatingText(this.TurnOrder, 19, "Blocked");
            }
            else if (statType == NameAll.STAT_TYPE_CT)
            {
                this.CT -= effect;
                if( this.CT < 0)
                {
                    this.CT = 0;
                }
            }
            else if (statType == NameAll.STAT_TYPE_MAX_MP) //effects MP, not MP max
            {
                AlterStat(NameAll.REMOVE_STAT_REMOVE, effect, NameAll.STAT_TYPE_MP,elementalType);
                //this.StatUnitMaxMP -= effect;
                //if (this.StatUnitMaxMP < min_stat)
                //{
                //    this.StatUnitMaxMP = min_stat;
                //}
                //calculate_stats = true;
            }
            else if (statType == NameAll.STAT_TYPE_MAX_HP)
            {
                SetHP(effect, NameAll.REMOVE_STAT_REMOVE, elementalType, false, isSaveCombatLog:false );
                //this.StatUnitMaxLife -= effect;
                //if (this.StatUnitMaxLife <= 0) //not letting this kill anyone
                //{
                //    this.StatUnitMaxLife = 1;
                //}
                //calculate_stats = true;
            }
            else
            {
                Debug.LogError("ERROR: stat should have been subtraced but nothing happened");
            }

			if(isSaveCombatLog)
			{
				CombatTurn logTurn = new CombatTurn();
				logTurn.actor = actor;
				logTurn.spellName = sn;
				logTurn.targetUnitId = this.TurnOrder;
				PlayerManager.Instance.AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_ALTER_STAT_REMOVE, combatLogSubType, cTurn: logTurn, rollResult: rollResult,
					rollChance: rollChance, effectValue: effect, statusValue: statType);
			}

            if (calculate_stats)
            {
                CalculateTotalStat(statType);
            }
            if( statType != NameAll.STAT_TYPE_MAX_HP && statType != NameAll.STAT_TYPE_MAX_MP)
                PlayerManager.Instance.ShowFloatingText(this.TurnOrder, 6, "-" + effect + " " + NameAll.GetStatTypeString(statType)); //Debug.Log("howing floating text");
        }
        else {
            //adds the stat
            if (statType == NameAll.STAT_TYPE_MP)
            {
                this.StatTotalMP += effect; Debug.Log("restoring MP");
                if (this.StatTotalMP >= this.StatTotalMaxMP)
                {
                    this.StatTotalMP = this.StatTotalMaxMP;
                }
            }
            else if (statType == NameAll.STAT_TYPE_PA)
            {
                //Debug.Log("pa added?" + this.StatUnitPA);
                this.StatUnitPA += effect;
                if (this.StatUnitPA > max_stat)
                {
                    this.StatUnitPA = max_stat;
                }
                //Debug.Log("pa added?" + this.StatUnitPA);
            }
            else if (statType == NameAll.STAT_TYPE_AGI)
            {
                this.StatUnitAgi += effect;
                if (this.StatUnitAgi > max_stat)
                {
                    this.StatUnitAgi = max_stat;
                }
            }
            else if (statType == NameAll.STAT_TYPE_MA)
            {
                this.StatUnitMA += effect;
                if (this.StatUnitMA > max_stat)
                {
                    this.StatUnitMA = max_stat;
                }

            }
            else if (statType == NameAll.STAT_TYPE_SPEED)
            {
                this.StatUnitSpeed += effect;
                if (this.StatUnitSpeed > max_speed)
                {
                    this.StatUnitSpeed = max_speed;
                }
            }
            else if (statType == NameAll.STAT_TYPE_FAITH)
            {
                this.StatUnitFaith += effect;
                if (this.StatUnitFaith > max_stat2)
                {
                    this.StatUnitFaith = max_stat2;
                }
            }
            else if (statType == NameAll.STAT_TYPE_BRAVE)
            {
                //Debug.Log("old brave " + this.StatTotalBrave);
                if( this.StatTotalBrave < 10 && this.StatTotalBrave + effect >= 10)
                {
                    StatusManager.Instance.RemoveStatusTickByUnit(this.TurnOrder, NameAll.STATUS_ID_CHICKEN, isBeingCalledFromPlayerUnit:true);
                }
                this.StatUnitBrave += effect;
                if (this.StatUnitBrave > max_stat2)
                {
                    this.StatUnitBrave = max_stat2;
                }
                //Debug.Log("new brave " + this.StatTotalBrave);
            }
            else if (statType == NameAll.STAT_TYPE_CUNNING)
            {
                this.StatUnitCunning += effect;
                if (this.StatUnitCunning > max_stat2)
                {
                    this.StatUnitCunning = max_stat2;
                }
            }
            else if (statType == NameAll.STAT_TYPE_MOVE)
            {
                this.StatUnitMove += effect;
                if (this.StatUnitMove > max_move)
                {
                    this.StatUnitMove = max_move;
                }
            }
            else if (statType == NameAll.STAT_TYPE_JUMP)
            {
                this.StatUnitJump += effect;
                if (this.StatUnitJump > max_move)
                {
                    this.StatUnitJump = max_move;
                }
            }
            else if (statType == NameAll.STAT_TYPE_CT)
            {
                SetCT(this.CT + 10);
            }
            else if (statType == NameAll.STAT_TYPE_MAX_MP) //effects MP, not MP max
            {
                AlterStat(NameAll.REMOVE_STAT_HEAL, effect, NameAll.STAT_TYPE_MP, elementalType);
                //this.StatUnitMaxMP -= effect;
                //if (this.StatUnitMaxMP < min_stat)
                //{
                //    this.StatUnitMaxMP = min_stat;
                //}
                //calculate_stats = true;
            }
            else if (statType == NameAll.STAT_TYPE_MAX_HP)
            {
                SetHP(effect, NameAll.REMOVE_STAT_HEAL, elementalType);
                //this.StatUnitMaxLife -= effect;
                //if (this.StatUnitMaxLife <= 0) //not letting this kill anyone
                //{
                //    this.StatUnitMaxLife = 1;
                //}
                //calculate_stats = true;
            }
            else if( statType == NameAll.STAT_TYPE_DIRECTION)
            {
                this.Dir = DirectionsExtensions.IntToDirection(effect);
                PlayerManager.Instance.SetPUODirectionMidTurn(this.TurnOrder, this.Dir);
            }
            else
            {
                Debug.LogError("ERROR: stat should have been added but nothing happened");
            }

            if (statType != NameAll.STAT_TYPE_MP)
            {
                //Debug.Log("adding stats? " + this.StatTotalPA);
                CalculateTotalStat(statType);
                //Debug.Log("adding stats? " + this.StatTotalPA);
            }
            if (statType != NameAll.STAT_TYPE_MAX_HP && statType != NameAll.STAT_TYPE_MAX_MP)
                PlayerManager.Instance.ShowFloatingText(this.TurnOrder, 5, "+" + effect + " " + NameAll.GetStatTypeString(statType)); Debug.Log("howing floating text");

			if (isSaveCombatLog)
			{
				CombatTurn logTurn = new CombatTurn();
				logTurn.actor = actor;
				logTurn.spellName = sn;
				logTurn.targetUnitId = this.TurnOrder;
				PlayerManager.Instance.AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_ALTER_STAT_ADD, combatLogSubType, cTurn: logTurn, rollResult: rollResult,
					rollChance: rollChance, effectValue: effect, statusValue: statType);
			}
		}
    }

    //called in player manager, board stuff done there
    public void SetUnitTile(Tile t, bool isStart = false)
    {
        this.TileX = t.pos.x;
        this.TileY = t.pos.y;
        this.TileZ = t.height;
    }

    //used when AI has control over a tile, temporarily moves it to test the outcome of an ability then moves it back adn the end of testing
    public void AISetUnitTile(Tile t)
    {
        this.TileX = t.pos.x;
        this.TileY = t.pos.y;
        this.TileZ = t.height;
    }

    private bool IsEquipmentBreakAllowed(int slot)
    {
        if (NameAll.IsClassicClass(this.ClassId)) //classic classes handle it differently
            return true;
        else
        {
            if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIPMENT_GUARD)
                return false;
            else if (IsSlotEmptyForItemAttack(slot))
                return false;
        }

        return true;
    }

    //used in calculationHitDamage, tells what type of ItemAttack it is and tells if an item is there or not
    public bool IsSlotEmptyForItemAttack(int statType)
    {
        if(statType == NameAll.STAT_TYPE_WEAPON)
        {
            if (this.ItemSlotWeapon == NameAll.FIST_EQUIP)
                return true;
        }
        else if( statType == NameAll.STAT_TYPE_OFFHAND)
        {
            if (this.ItemSlotOffhand == NameAll.NO_EQUIP || this.ItemSlotOffhand == NameAll.FIST_EQUIP)
                return true;
        }
        else if( statType == NameAll.STAT_TYPE_HEAD)
        {
            if (this.ItemSlotHead == NameAll.NO_EQUIP)
                return true;
        }
        else if( statType == NameAll.STAT_TYPE_BODY)
        {
            if (this.ItemSlotBody == NameAll.NO_EQUIP)
                return true;
        }
        else if( statType == NameAll.STAT_TYPE_ACCESSORY)
        {
            if (this.ItemSlotAccessory == NameAll.NO_EQUIP)
                return true;
        }
        return false;
    }

    public bool AbleToResolveSlowAction()
    {
        if (!this.AbleToFight)
        {
            return false;
        }
        if (StatusManager.Instance.UnitCantResolveSlowSpell(this.TurnOrder))
        {
            return false;
        }
        return true; 
    }

    //called in PlayerManager, calls unequip first
    public void EquipMovementAbility(int abilityId, bool isRandomized = false)
    {
        //        movement_map.put("am_jum_1", "Jump +1");
        //        movement_map.put("am_jum_2", "Jump +2");
        //        movement_map.put("am_jum_3", "Jump +3");
        //        movement_map.put("am_mov_1", "Move +1");
        //        movement_map.put("am_mov_2", "Move +2");
        //        movement_map.put("am_mov_3", "Move +3");
        UnequipMovementAbility(isRandomized);
        this.AbilityMovementCode = abilityId;
        if(this.AbilityMovementCode == NameAll.MOVEMENT_MOVE_UP_1)
        {
            this.StatUnitMove += 1;
            CalculateTotalStat(NameAll.STAT_TYPE_MOVE);
        }
        else if (this.AbilityMovementCode == NameAll.MOVEMENT_MOVE_UP_2)
        {
            //Debug.Log("equipping movement_move_up_2");
            this.StatUnitMove += 2;
            this.StatUnitJump -= 2;
            if (this.StatUnitJump <= 0)
                this.StatUnitJump = 1;
            CalculateTotalStat(NameAll.STAT_TYPE_MOVE);
            CalculateTotalStat(NameAll.STAT_TYPE_JUMP);
        }
        else if (this.AbilityMovementCode == NameAll.MOVEMENT_JUMP_UP_2)
        {
            this.StatUnitJump += 2;
            CalculateTotalStat(NameAll.STAT_TYPE_JUMP);
        }
        else if (this.AbilityMovementCode == NameAll.MOVEMENT_MOVE_3)
        {
            this.StatUnitMove += 3;
            CalculateTotalStat(NameAll.STAT_TYPE_MOVE);
        }
        else if (this.AbilityMovementCode == NameAll.MOVEMENT_MOVE_2)
        {
            this.StatUnitMove += 2;
            CalculateTotalStat( NameAll.STAT_TYPE_MOVE);
        }
        else if (this.AbilityMovementCode == NameAll.MOVEMENT_MOVE_1)
        {
            this.StatUnitMove += 1;
            CalculateTotalStat(NameAll.STAT_TYPE_MOVE);
        }
        else if (this.AbilityMovementCode == NameAll.MOVEMENT_JUMP_3)
        {
            this.StatUnitJump += 3;
            CalculateTotalStat(NameAll.STAT_TYPE_JUMP);
        }
        else if (this.AbilityMovementCode == NameAll.MOVEMENT_JUMP_2)
        {
            this.StatUnitJump += 2;
            CalculateTotalStat( NameAll.STAT_TYPE_JUMP);
        }
        else if (this.AbilityMovementCode == NameAll.MOVEMENT_JUMP_1)
        {
            this.StatUnitJump += 1;
            CalculateTotalStat( NameAll.STAT_TYPE_JUMP);
        }
        else if (this.AbilityMovementCode == NameAll.MOVEMENT_FLOAT)
        {
            //Debug.Log("Asdf");
            if (!isRandomized)
            {
                StatusManager.Instance.AddStatusLastingByString(this.TurnOrder, NameAll.STATUS_ID_FLOAT_MOVE);
            }         
        }
    }

    //called in Equip
    private void UnequipMovementAbility(bool isRandomized)
    {
        int abilityId = this.AbilityMovementCode;
        int min_move = 2; int min_jump = 2;

        if (this.AbilityMovementCode == NameAll.MOVEMENT_MOVE_UP_1)
        {
            this.StatUnitMove -= 1;
            if (this.StatUnitMove <= min_move)
            {
                this.StatUnitMove = min_move;
            }
            CalculateTotalStat(NameAll.STAT_TYPE_MOVE);
        }
        else if (this.AbilityMovementCode == NameAll.MOVEMENT_MOVE_UP_2)
        {
            this.StatUnitMove -= 2;
            if (this.StatUnitMove <= min_move)
            {
                this.StatUnitMove = min_move;
            }
            CalculateTotalStat(NameAll.STAT_TYPE_MOVE);
            this.StatUnitJump += 2;
            if (this.StatUnitJump <= min_jump)
            {
                this.StatUnitJump = min_jump;
            }
            CalculateTotalStat(NameAll.STAT_TYPE_JUMP);
        }
        else if (this.AbilityMovementCode == NameAll.MOVEMENT_JUMP_UP_2)
        {
            this.StatUnitJump -= 2;
            if (this.StatUnitJump <= min_jump)
            {
                this.StatUnitJump = min_jump;
            }
            CalculateTotalStat(NameAll.STAT_TYPE_JUMP);
        }
        else if ( abilityId == NameAll.MOVEMENT_MOVE_3 )
        {
            this.StatUnitMove -= 3;
            if (this.StatUnitMove <= min_move)
            {
                this.StatUnitMove = min_move;
            }
            CalculateTotalStat(NameAll.STAT_TYPE_MOVE);
        }
        else if (abilityId == NameAll.MOVEMENT_MOVE_2)
        {
            this.StatUnitMove -= 2;
            if (this.StatUnitMove <= min_move)
            {
                this.StatUnitMove = min_move;
            }
            CalculateTotalStat(NameAll.STAT_TYPE_MOVE);
        }
        else if (abilityId == NameAll.MOVEMENT_MOVE_1)
        {
            this.StatUnitMove -= 1;
            if (this.StatUnitMove <= min_move)
            {
                this.StatUnitMove = min_move;
            }
            CalculateTotalStat(NameAll.STAT_TYPE_MOVE);
        }
        else if (abilityId == NameAll.MOVEMENT_JUMP_3)
        {
            this.StatUnitJump -= 3;
            if (this.StatUnitJump <= min_jump)
            {
                this.StatUnitJump = min_jump;
            }
            CalculateTotalStat(NameAll.STAT_TYPE_JUMP);
        }
        else if (abilityId == NameAll.MOVEMENT_JUMP_2)
        {
            this.StatUnitJump -= 2;
            if (this.StatUnitJump <= min_jump)
            {
                this.StatUnitJump = min_jump;
            }
            CalculateTotalStat(NameAll.STAT_TYPE_JUMP);
        }
        else if (abilityId == NameAll.MOVEMENT_JUMP_1)
        {
            this.StatUnitJump -= 1;
            if (this.StatUnitJump <= min_jump)
            {
                this.StatUnitJump = min_jump;
            }
            CalculateTotalStat(NameAll.STAT_TYPE_JUMP);
        }
        else if (abilityId == NameAll.MOVEMENT_FLOAT)
        {
            if(!isRandomized)
            {
                StatusManager.Instance.RemoveStatusLastingByUnitAndString(this.TurnOrder, NameAll.STATUS_ID_FLOAT_MOVE);
            }
            
        }
        this.AbilityMovementCode = 0;
    }

    public void EquipSupportAbility(int abilityId)
    {
        UnequipSupportAbility();
        this.AbilitySupportCode = abilityId;
    }

    public void UnequipSupportAbility()
    {
        if( this.ClassId >= NameAll.CLASS_FIRE_MAGE)
        {
            if (this.AbilitySupportCode == NameAll.SUPPORT_DUAL_WIELD && ItemManager.Instance.IsOffhandWeaponEquipped(this.ItemSlotOffhand)
                && !AbilityManager.Instance.IsInnateAbility(this.ClassId, NameAll.SUPPORT_DUAL_WIELD, NameAll.ABILITY_SLOT_SUPPORT))
            {
                if (ItemManager.Instance.IsOffhandWeaponEquipped(this.ItemSlotOffhand))
                {
                    EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
                }
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_MAGE_ROBES)
            {
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_BODY);
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_HEAD);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_CLOTHES)
            {
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_BODY);
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_HEAD);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_HEAVY_ARMORS)
            {
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_BODY);
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_HEAD);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_LIGHT_ARMORS)
            {
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_BODY);
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_HEAD);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_SHIELD)
            {
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_WAND)
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_STAFFS)
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_INSTRUMENT_DECK)
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_GUNS)
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_WHIP_MACE)
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_SWORDS)
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_BOWS)
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_SCALES)
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_EQUIP_SPEAR)
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
            }
        }
        else
        {

            //do the equip abilities and two swords
            if ( this.AbilitySupportCode == NameAll.SUPPORT_TWO_SWORDS && ItemManager.Instance.IsOffhandWeaponEquipped(this.ItemSlotOffhand)
                && !AbilityManager.Instance.IsInnateAbility(this.ClassId,NameAll.SUPPORT_TWO_SWORDS, NameAll.ABILITY_SLOT_SUPPORT))
            {
                if (ItemManager.Instance.IsOffhandWeaponEquipped(this.ItemSlotOffhand))
                {
                    EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
                }
            }
            else if( this.AbilitySupportCode == NameAll.SUPPORT_CLASSIC_EQUIP_ARMOR 
                && ItemManager.Instance.GetItemType(this.ItemSlotBody, NameAll.ITEM_SLOT_BODY) == NameAll.ITEM_ITEM_TYPE_CLASSIC_ARMOR)
            {
                EquipItem(NameAll.NO_EQUIP,NameAll.ITEM_SLOT_BODY);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_CLASSIC_EQUIP_SHIELD
                && ItemManager.Instance.GetItemType(this.ItemSlotOffhand, NameAll.ITEM_SLOT_OFFHAND) == NameAll.ITEM_ITEM_TYPE_CLASSIC_SHIELD)
            {
                EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_CLASSIC_EQUIP_SWORD &&
                ( ItemManager.Instance.GetItemType(this.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON) == NameAll.ITEM_ITEM_TYPE_CLASSIC_SWORD
                || ItemManager.Instance.GetItemType(this.ItemSlotOffhand, NameAll.ITEM_SLOT_OFFHAND) == NameAll.ITEM_ITEM_TYPE_CLASSIC_SWORD))
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                if (ItemManager.Instance.IsOffhandWeaponEquipped(this.ItemSlotOffhand))
                {
                    EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
                }
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_CLASSIC_EQUIP_GUN &&
                (ItemManager.Instance.GetItemType(this.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON) == NameAll.ITEM_ITEM_TYPE_CLASSIC_GUN
                || ItemManager.Instance.GetItemType(this.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON) == NameAll.ITEM_ITEM_TYPE_CLASSIC_MAGIC_GUN))

            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                //if (ItemManager.Instance.IsOffhandWeaponEquipped(this.ItemSlotOffhand))
                //{
                //    EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
                //}
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_CLASSIC_EQUIP_SPEAR &&
                ItemManager.Instance.GetItemType(this.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON) == NameAll.ITEM_ITEM_TYPE_CLASSIC_SPEAR)
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                //if (ItemManager.Instance.IsOffhandWeaponEquipped(this.ItemSlotOffhand))
                //{
                //    EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
                //}
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_CLASSIC_EQUIP_CROSSBOW &&
                ItemManager.Instance.GetItemType(this.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON) == NameAll.ITEM_ITEM_TYPE_CLASSIC_CROSSBOW )
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                //if (ItemManager.Instance.IsOffhandWeaponEquipped(this.ItemSlotOffhand))
                //{
                //    EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
                //}
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_CLASSIC_EQUIP_KNIFE && 
                ItemManager.Instance.GetItemType(this.ItemSlotWeapon,NameAll.ITEM_SLOT_WEAPON) == NameAll.ITEM_ITEM_TYPE_CLASSIC_KATANA)
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                //if (ItemManager.Instance.IsOffhandWeaponEquipped(this.ItemSlotOffhand))
                //{
                //    EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
                //}
            }
            else if (this.AbilitySupportCode == NameAll.SUPPORT_CLASSIC_EQUIP_AXE &&
                ItemManager.Instance.GetItemType(this.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON) == NameAll.ITEM_ITEM_TYPE_CLASSIC_AXE)
            {
                EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
                //if (ItemManager.Instance.IsOffhandWeaponEquipped(this.ItemSlotOffhand))
                //{
                //    EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
                //}
            }
        }
    }

    

    public void SetClassIdStatsUnequip(int var)
    {
        this.ClassId = var;
        ClearAbilities();
        //if (this.ClassId == NameAll.CLASS_MIME)
        //{
        //    this.AbilitySecondaryCode = 0;
        //    this.AbilityReactionCode = 0;
        //    this.AbilitySupportCode = 0;
        //    EquipMovementAbility(NameAll.MOVEMENT_NONE);
        //}
        NullEquipment();
        SetBaseStats(); //CalculateTotalStats() called in both the other two things;
        
    }

    //remove all items and abilities
    public void StripClass()
    {
        ClearAbilities();
        NullEquipment();
        SetBaseStats();
    }

    public void SetSex(string var)
    {
        this.Sex = var;
        SetBaseStats();
        CalculateTotalStats();
    }

    public void SetLevel(int var)
    {
        this.Level = var;//Debug.Log("setting new level");
        if( this.ClassId >= NameAll.CLASS_FIRE_MAGE)
        {
            CheckItemLevelsForUnequip();
        }
        SetBaseStats();
        CalculateTotalStats();
    }

    void CheckItemLevelsForUnequip()
    {
        ItemObject io = ItemManager.Instance.GetItemObjectById(this.ItemSlotWeapon);
        if( io.Level > this.Level)
        {
            EquipItem(NameAll.FIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
        }

        io = ItemManager.Instance.GetItemObjectById(this.ItemSlotOffhand);
        if (io.Level > this.Level)
        {
            EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_OFFHAND);
        }

        io = ItemManager.Instance.GetItemObjectById(this.ItemSlotHead);
        if (io.Level > this.Level)
        {
            EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_HEAD);
        }

        io = ItemManager.Instance.GetItemObjectById(this.ItemSlotBody);
        if (io.Level > this.Level)
        {
            EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_BODY);
        }

        io = ItemManager.Instance.GetItemObjectById(this.ItemSlotAccessory);
        if (io.Level > this.Level)
        {
            EquipItem(NameAll.NO_EQUIP, NameAll.ITEM_SLOT_ACCESSORY);
        }

    }

    public int GetCharmTeam()
    {
        //add a charm check here
        int z1 = this.TeamId;
        if( StatusManager.Instance.IfStatusByUnitAndId(this.TurnOrder, NameAll.STATUS_ID_CHARM) )
        {
            if( z1 == 2)
            {
                z1 = 3;
            } else
            {
                z1 = 2;
            }
        }

        return z1;
    }

    public void SetLastingStatuses()
    {
        StatusManager.Instance.AddStatusLastingById(this.TurnOrder,this.ItemSlotWeapon,NameAll.ITEM_SLOT_WEAPON);
        StatusManager.Instance.AddStatusLastingById(this.TurnOrder, this.ItemSlotOffhand, NameAll.ITEM_SLOT_OFFHAND);
        StatusManager.Instance.AddStatusLastingById(this.TurnOrder, this.ItemSlotHead, NameAll.ITEM_SLOT_HEAD);
        StatusManager.Instance.AddStatusLastingById(this.TurnOrder, this.ItemSlotBody, NameAll.ITEM_SLOT_BODY);
        StatusManager.Instance.AddStatusLastingById(this.TurnOrder, this.ItemSlotAccessory, NameAll.ITEM_SLOT_ACCESSORY);

        if( this.AbilityMovementCode == NameAll.MOVEMENT_FLOAT)
            StatusManager.Instance.AddStatusLastingByString(this.TurnOrder, NameAll.STATUS_ID_FLOAT_MOVE);
    }

    public void EnableReactionFlag()
    {
        this.ReactionFlag = true;
    }

    public void DisableReactionFlag()
    {
        this.ReactionFlag = false;
    }
    
    public void InitializeTwoSwordsEligible()
    {
        //Debug.Log("in is eligible for two swords, " + ItemManager.Instance.IsOffhandWeaponEquipped(this.ItemSlotOffhand) + " " 
        //    + AbilityManager.Instance.IsInnateAbility(this.ClassId, NameAll.SUPPORT_TWO_SWORDS, NameAll.ABILITY_SLOT_SUPPORT));
        if ( ItemManager.Instance.IsOffhandWeaponEquipped(this.ItemSlotOffhand) && 
            (IsAbilityEquipped(NameAll.SUPPORT_TWO_SWORDS, NameAll.ABILITY_SLOT_SUPPORT) 
            || IsAbilityEquipped(NameAll.SUPPORT_DUAL_WIELD, NameAll.ABILITY_SLOT_SUPPORT)
            || AbilityManager.Instance.IsInnateAbility(this.ClassId, NameAll.SUPPORT_TWO_SWORDS, NameAll.ABILITY_SLOT_SUPPORT)
            || AbilityManager.Instance.IsInnateAbility(this.ClassId,NameAll.SUPPORT_DUAL_WIELD,NameAll.ABILITY_SLOT_SUPPORT)) )
        {
            //Debug.Log("in is eligible for two swords, returning true ");
            this.TwoSwordsEligible = true;
        }
        else
        {
            this.TwoSwordsEligible = false;
        }
        
    }

    public void InitializeOnMoveEffect()
    {
        if(NameAll.IsClassicClass(this.ClassId))
        {
            if (this.AbilityMovementCode == NameAll.MOVEMENT_MOVE_HP_UP || this.AbilityMovementCode == NameAll.MOVEMENT_MOVE_MP_UP)
                this.OnMoveEffect = true;
            else
                this.OnMoveEffect = false;
        }
        else
        {
            if( this.AbilityMovementCode == NameAll.MOVEMENT_MOVE_WIS_UP ||
                this.AbilityMovementCode == NameAll.MOVEMENT_DRAW_ATTENTION ||
                this.AbilityMovementCode == NameAll.MOVEMENT_RAISE_THE_DEAD ||
                this.AbilityMovementCode == NameAll.MOVEMENT_BLESSED_STEPS ||
                this.AbilityMovementCode == NameAll.MOVEMENT_MP_WALK ||
                this.AbilityMovementCode == NameAll.MOVEMENT_MOVE_CRG_UP ||
                this.AbilityMovementCode == NameAll.MOVEMENT_HP_WALK ||
                this.AbilityMovementCode == NameAll.MOVEMENT_WALK_IT_OFF ||
                this.AbilityMovementCode == NameAll.MOVEMENT_TP_WALK ||
                this.AbilityMovementCode == NameAll.MOVEMENT_MOVE_SKL_UP ||
                this.AbilityMovementCode == NameAll.MOVEMENT_SAINTS_FOOTSTEPS ||
                this.AbilityMovementCode == NameAll.MOVEMENT_WALK_IT_ON ||
                this.AbilityMovementCode == NameAll.MOVEMENT_SILENCE_THE_CROWD ||
                this.AbilityMovementCode == NameAll.MOVEMENT_STRETCH_LEGS || 
                this.AbilityMovementCode == NameAll.MOVEMENT_UNSTABLE_TP)
            {
                this.OnMoveEffect = true;
            }
            else
            {
                this.OnMoveEffect = false;
            }
              
        }
    }

    public void InitializeSpecialMoveRange()
    {
        if( NameAll.IsClassicClass(this.ClassId))
        {
            if( this.AbilityMovementCode == NameAll.MOVEMENT_TELEPORT_1 ||
                this.AbilityMovementCode == NameAll.MOVEMENT_TELEPORT_2 ||
                this.AbilityMovementCode == NameAll.MOVEMENT_FLY ||
                this.AbilityMovementCode == NameAll.MOVEMENT_IGNORE_HEIGHT )
            {
                this.SpecialMoveRange = true;
            }
            else
            {
                this.SpecialMoveRange = false;
            }
        }
        else
        {
            if(this.AbilityMovementCode == NameAll.MOVEMENT_UNSTABLE_TP || 
                this.AbilityMovementCode == NameAll.MOVEMENT_GHOST || 
                this.AbilityMovementCode == NameAll.MOVEMENT_WINDS_OF_FATE ||
                this.AbilityMovementCode == NameAll.MOVEMENT_CRUNCH ||
                this.AbilityMovementCode == NameAll.MOVEMENT_LEAP ||
                this.AbilityMovementCode == NameAll.MOVEMENT_SWAP ||
                this.AbilityMovementCode == NameAll.MOVEMENT_SCALE )
            {
                this.SpecialMoveRange = true;
            }
            else
            {
                this.SpecialMoveRange = false;
            }
        }
    }

    public bool IsSpecialMoveRange()
    {
        return this.SpecialMoveRange;
    }

    public bool IsOnMoveEffect()
    {
        return this.OnMoveEffect;
    }

    public bool IsEligibleForTwoSwords()
    {
        //Debug.Log("is two swords eligible " + TwoSwordsEligible);
        return TwoSwordsEligible;
        //if( ItemManager.Instance.IsOffhandWeaponEquipped(this.ItemSlotOffhand) && 
        //    (IsAbilityEquipped(NameAll.SUPPORT_TWO_SWORDS, NameAll.ABILITY_SLOT_SUPPORT) 
        //    || IsAbilityEquipped(NameAll.SUPPORT_DUAL_WIELD, NameAll.ABILITY_SLOT_SUPPORT)
        //    || AbilityManager.Instance.IsInnateAbility(this.ClassId, NameAll.SUPPORT_TWO_SWORDS, NameAll.ABILITY_SLOT_SUPPORT)
        //    || AbilityManager.Instance.IsInnateAbility(this.ClassId,NameAll.SUPPORT_DUAL_WIELD,NameAll.ABILITY_SLOT_SUPPORT)) )
        //{
        //    //Debug.Log("in is eligible for two swords, returning true " + IsAbilityEquipped(NameAll.SUPPORT_TWO_SWORDS, NameAll.SUPPORT) + " " + HasInnateAbility(NameAll.SUPPORT_TWO_SWORDS, NameAll.SUPPORT));
        //    return true;
        //}
        //return false;
    }

    public int GetReactionNumber()
    {
        int z1;
        if( this.ClassId < NameAll.CLASS_FIRE_MAGE)
        {
            z1 = this.StatTotalBrave;
        }
        else
        {
            z1 = this.StatTotalFaith;
            if (IsAbilityEquipped(NameAll.SUPPORT_ICONOCLAST, NameAll.ABILITY_SLOT_SUPPORT))
            {
                if (this.StatTotalBrave > this.StatTotalFaith)
                {
                    z1 = this.StatTotalBrave;
                }
                if (this.StatTotalCunning > z1)
                {
                    z1 = this.StatTotalCunning;
                }
            }
            else
            {
                if (this.StatTotalBrave < this.StatTotalFaith)
                {
                    z1 = this.StatTotalBrave;
                }
                if (this.StatTotalCunning < z1)
                {
                    z1 = this.StatTotalCunning;
                }
            }
        }
        //Debug.Log("reaction number is " + z1);
        return z1;
    }

    //called from PlayerManager (from puo), knows that the unit has scale
    //scale units basically have infinite height from the first panel
    public int GetJumpScale(Tile t)
    {
        if (t.pos.x == this.TileX && t.pos.y == this.TileY)
            return 1000;
        else
            return this.StatTotalJump;
    }

    public void SetStatUnitBrave(int var)
    {
        this.StatUnitBrave = var;
        CalculateTotalStats();
    }

    public void SetStatUnitFaith(int var)
    {
        this.StatUnitFaith = var;
        CalculateTotalStats();
    }

    public void SetStatUnitCunning(int var)
    {
        this.StatUnitCunning = var;
        CalculateTotalStats();
    }

    //custom classes/secondaries adn units with custom items are not eligible for online
    public bool IsEligibleForOnline()
    {
        if (this.ClassId >= NameAll.CUSTOM_CLASS_ID_START_VALUE || this.AbilitySecondaryCode >= NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE)
            return false;

        if (this.ItemSlotWeapon >= NameAll.CUSTOM_ITEM_ID_START_VALUE || this.ItemSlotOffhand >= NameAll.CUSTOM_ITEM_ID_START_VALUE 
            || this.ItemSlotHead >= NameAll.CUSTOM_ITEM_ID_START_VALUE || this.ItemSlotBody >= NameAll.CUSTOM_ITEM_ID_START_VALUE 
                || this.ItemSlotAccessory >= NameAll.CUSTOM_ITEM_ID_START_VALUE )
            return false;
            

        return true;
    }

	#region AI stuff
	//AI stuff moved from PUO to PU for faster mode
	public Drivers puDriver; //set at beginning of battled and checked to see if if human or AI controlled
	public List<SpellNameAI> aiSpellList; //used to help speed AI ability checks, used in CombatComputerPlayer to help select an action
	public bool isDamageSpell; //used to help speed AI ability checks, used in CombatComputerPlayer to see if player can cast DamageSPell
	public bool isReviveSpell; //used to help speed AI ability checks, used in CombatComputerPlayer to see if player can cast ReviveSpell
	public bool isCureSpell; //used to help speed AI ability checks, used in CombatComputerPlayer to see if player can cast CureSpell
	public List<int> primaryAbilityIdList;
	public List<int> secondaryAbilityIdList;

	//initialize driver at beginning of battle. drivers are used for telling if AI should kick in when entering CombatState for each unit
	public void SetDriver(Drivers d)
	{
		this.puDriver = d;
		//gets the number of primary and secondary abilities for use in random selecting an ability
		SetAbilityList();
	}

	//sets the number of primary and secondary abilities, used for AI in CombatComputerPlayer.cs
	void SetAbilityList()
	{
		int damageType = 0;
		SpellNameAI snai;
		this.aiSpellList = new List<SpellNameAI>();
		this.primaryAbilityIdList = new List<int>();
		this.secondaryAbilityIdList = new List<int>();
		this.isDamageSpell = false;
		this.isCureSpell = false;
		this.isReviveSpell = false;

		PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(this.TurnOrder);
		List<SpellName> primaryList = SpellManager.Instance.GetSpellNamesByCommandSet(pu.ClassId, pu);
		for (int i = 0; i < primaryList.Count; i++)
		{
			primaryAbilityIdList.Add(primaryList[i].SpellId); //Debug.Log(primaryAbilityIdList[i]);

			snai = new SpellNameAI(primaryList[i]);
			if (snai.isReviveType)
			{
				this.aiSpellList.Add(snai);
				this.isReviveSpell = true;
			}
			else if (snai.isCureType)
			{
				this.aiSpellList.Add(snai);
				this.isCureSpell = true;
			}
			else if (snai.isDamageType && damageType < 3)
			{
				damageType += 1;
				this.aiSpellList.Add(snai);
				this.isDamageSpell = true;
			}
		}

		if (pu.AbilitySecondaryCode != NameAll.SECONDARY_NONE)
		{
			List<SpellName> secondaryList = SpellManager.Instance.GetSpellNamesByCommandSet(pu.AbilitySecondaryCode, pu);
			for (int i = 0; i < secondaryList.Count; i++)
			{
				secondaryAbilityIdList.Add(secondaryList[i].SpellId);

				snai = new SpellNameAI(secondaryList[i]);
				if (snai.isReviveType)
				{
					this.aiSpellList.Add(snai);
					this.isReviveSpell = true;
				}
				else if (snai.isCureType)
				{
					this.aiSpellList.Add(snai);
					this.isCureSpell = true;
				}
				else if (snai.isDamageType && damageType < 3)
				{
					damageType += 1;
					this.aiSpellList.Add(snai);
					this.isDamageSpell = true;
				}
			}
		}
	}
	#endregion
}

//public enum ClassNames
//{
//    //classes, 0 mime, 1 Chemist, 2 Knight, 3 archer, 4 squire, 5 thief, 6 ninja, 7 monk,
//    //8 priest, 9 wizard, 10 time mage, 11 summoner, 12 mediator, 13 oracle,
//    //14 geomancer, 15 lancer, 16 samurai, 17 calculator, 18 bard, 19 dancer 

//    Mime = 0,  Chemist = 1, Knight = 2, Archer = 3, Squire = 4, Thief = 5,
//    Ninja = 6, Monk = 7, Priest = 8, Wizard = 9, TimeMage = 10,
//    Summoner = 11, Mediator = 12, Oracle = 13, Geomancer = 14, Lancer = 15,
//    Samurai = 16, Calculator = 17, Bard = 18, Dancer = 19
//}
