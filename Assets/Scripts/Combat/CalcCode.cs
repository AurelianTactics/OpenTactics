using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

//takes a string, returns a PU
public static class CalcCode {

	public static PlayerUnit BuildPlayerUnit(string str)
    {
        PlayerUnit pu = new PlayerUnit(str);
        return pu;
    }

    public static string BuildStringFromPlayerUnit(PlayerUnit pu)
    {
        //i.ToString("0000"); - explicit form
        //i.ToString("D4"); - short form format specifier
        return pu.BuildStringFromPlayerUnit();
    }

    public static string SpellNameValueToString(SpellName sn, string snCategory)
    {
        string retValue = "";
        if(snCategory.Equals(NameAll.SN_ABILITY_NAME))
        {
            retValue = sn.AbilityName;
        }
        else if (snCategory.Equals(NameAll.SN_ABILITY_TYPE))
        {
            retValue = NameAll.GetPmTypeString(sn.PMType);
        }
        else if (snCategory.Equals(NameAll.SN_ADD_STATUS))
        {
            retValue = NameAll.GetHitsStatString(sn.AddsStatus);
        }
        else if (snCategory.Equals(NameAll.SN_ALLIES_TYPE))
        {
            if( sn.AlliesType == NameAll.ALLIES_TYPE_ALLIES )
            {
                retValue = "allies only";
            }
            else if( sn.AlliesType == NameAll.ALLIES_TYPE_ENEMIES)
            {
                retValue = "enemies only";
            }
            else
            {
                retValue = "any unit";
            }
        }
        else if (snCategory.Equals(NameAll.SN_BASE_HIT))
        {
            retValue = sn.BaseHit.ToString() + "%";
        }
        else if (snCategory.Equals(NameAll.SN_BASE_Q))
        {
            retValue = sn.BaseQ.ToString();
        }
        else if (snCategory.Equals(NameAll.SN_CALCULATE))
        {
            if( sn.CalculateMimic == NameAll.SPELL_CALC || sn.CalculateMimic == NameAll.SPELL_CALC_MIMIC )
            {
                retValue = "yes";
            }
            else
            {
                retValue = "no";
            }
        }
        else if (snCategory.Equals(NameAll.SN_CASTER_IMMUNE))
        {
            if (sn.CasterImmune == 1)
            {
                retValue = "yes";
            }
            else
            {
                retValue = "no";
            }
        }
        else if (snCategory.Equals(NameAll.SN_COMMAND_SET))
        {
            if( sn.CommandSet >= NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE) //custom command set
            {
                retValue = GetCommandSetName(sn.CommandSet);
                //Debug.Log("in calcCode testing command set " + NameAll.PP_CUSTOM_COMMAND_SET_BASE + sn.CommandSet);
                //Debug.Log("in calc code, checking custom command set name " + sn.CommandSet + " " + PlayerPrefs.GetString(NameAll.PP_CUSTOM_COMMAND_SET_BASE + sn.CommandSet, "default"));
                //retValue = PlayerPrefs.GetString(NameAll.PP_CUSTOM_COMMAND_SET_BASE + sn.CommandSet, "Custom Set: " + (sn.CommandSet - NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE));
            }
            else
            {
                retValue = AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_SECONDARY,sn.CommandSet);
            }
        }
        else if (snCategory.Equals(NameAll.SN_COUNTER_TYPE))
        {
            retValue = sn.CounterType.ToString(); //Debug.Log("convert counterType int into name");
        }
        else if (snCategory.Equals(NameAll.SN_CTR))
        {
            retValue = GetCTRString(sn.CTR);
            
        }
        else if (snCategory.Equals(NameAll.SN_DAMAGE_FORMULA))
        {
            var tempList = GetDFTString(sn.Version,sn.DamageFormulaType);
            retValue = tempList[0] + " -- " + tempList[1];
            //retValue = sn.DamageFormulaType.ToString(); //Debug.Log("convert dft int into name");
        }
        else if (snCategory.Equals(NameAll.SN_EFFECT_XY))
        {
            retValue = GetEffectXYString(sn.EffectXY);
            //retValue = sn.EffectXY.ToString(); //Debug.Log("convert command set int into name");
        }
        else if (snCategory.Equals(NameAll.SN_EFFECT_Z))
        {
            retValue = NameAll.GetEffectZString(sn.EffectZ);
        }
        else if (snCategory.Equals(NameAll.SN_ELEMENT_TYPE))
        {
            retValue = NameAll.GetElementalString(sn.ElementType, true);
        }
        else if (snCategory.Equals(NameAll.SN_EVADABLE))
        {
            if( sn.EvasionReflect == NameAll.SPELL_EVASION || sn.EvasionReflect == NameAll.SPELL_EVASION_REFLECT)
            {
                retValue = "yes";
            }
            else
            {
                retValue = "no";
            }
        }
        else if (snCategory.Equals(NameAll.SN_HITS_STAT))
        {
            retValue = NameAll.GetHitsStatString(sn.HitsStat);
        }
        else if (snCategory.Equals(NameAll.SN_IGNORES_DEFENSE))
        {
            if( sn.IgnoresDefense == 0)
            {
                retValue = "no";
            }
            else
            {
                retValue = "yes";
            }
        }
        else if (snCategory.Equals(NameAll.SN_MIMIC))
        {
            if (sn.CalculateMimic == NameAll.SPELL_MIMIC || sn.CalculateMimic == NameAll.SPELL_CALC_MIMIC)
            {
                retValue = "yes";
            }
            else
            {
                retValue = "no";
            }
        }
        else if (snCategory.Equals(NameAll.SN_MOD))
        {
            retValue = NameAll.GetModName(sn.Mod);
        }
        else if (snCategory.Equals(NameAll.SN_MP))
        {
            retValue = GetMPString(sn.MP);
        }
        else if (snCategory.Equals(NameAll.SN_RANGE_XY_MAX))
        {
            retValue = NameAll.GetRangeTypeMax(sn.RangeXYMin, sn.RangeXYMax);
        }
        else if (snCategory.Equals(NameAll.SN_RANGE_XY_MIN))
        {
            retValue = NameAll.GetRangeTypeMin(sn.RangeXYMin);
        }
        else if (snCategory.Equals(NameAll.SN_RANGE_Z))
        {
            retValue = NameAll.GetEffectZString(sn.RangeZ);
        }
        else if (snCategory.Equals(NameAll.SN_REFLECTABLE))
        {
            if (sn.EvasionReflect == NameAll.SPELL_REFLECT || sn.EvasionReflect == NameAll.SPELL_EVASION_REFLECT)
            {
                retValue = "yes";
            }
            else
            {
                retValue = "no";
            }
        }
        else if (snCategory.Equals(NameAll.SN_REMOVE_STAT))
        {
            //retValue = sn.GetRemoveStat().ToString(); Debug.Log("convert command set int into name");
            retValue = NameAll.GetRemoveStatString(sn.RemoveStat, sn.HitsStat);
        }
        else if (snCategory.Equals(NameAll.SN_STATUS_CANCEL))
        {
            if( sn.StatusCancel == 1)
            {
                retValue = "silence";
            }
            else
            {
                retValue = "no";
            }
        }
        else if (snCategory.Equals(NameAll.SN_STATUS_TYPE))
        {
            retValue = NameAll.GetStatusString(sn.StatusType);
        }
        else if (snCategory.Equals(NameAll.SN_STAT_TYPE))
        {
            retValue = NameAll.GetStatTypeString(sn.StatType);
        }
        else if (snCategory.Equals(NameAll.SN_VERSION))
        {
            if(sn.Version.Equals( NameAll.VERSION_CLASSIC))
            {
                retValue = "classic";
            }
            else if (sn.Version.Equals( NameAll.VERSION_AURELIAN))
            {
                retValue = "Aurelian";
            }
            else
            {
                retValue = "any";
            }
        }
       
        return retValue;
    }

    public static string GetMPString(int mp)
    {
        string retValue = "";

        int z1 = mp;
        if (z1 < 1000)
        {
            retValue = z1.ToString();
        }
        else
        {
            z1 = z1 - 1000;
            retValue = z1 + "% of max MP";
        }
        return retValue;
    }

    public static string GetCTRString(int ctr)
    {
        string retValue = "";

        
        if (ctr == 0)
        {
            retValue = "instant";
        }
        else if (ctr < 100)
        {
            retValue = ctr + " ticks";
        }
        else if (ctr < 200)
        {
            int z1 = ctr - 100;
            retValue = z1 + "% of CT";
        }

        return retValue;
    }

    public static string GetEffectXYString(int effect)
    {
        string retValue = "";

        if( effect < 100)
        {
            retValue = effect.ToString();
        }
        else if( effect == NameAll.SPELL_EFFECT_ALLIES)
        {
            retValue = "all allies";
        }
        else if (effect == NameAll.SPELL_EFFECT_ENEMIES)
        {
            retValue = "all enemies";
        }
        else if (effect >= NameAll.SPELL_EFFECT_CONE_BASE && effect <= NameAll.SPELL_EFFECT_CONE_MAX)
        {
            retValue = "cone (level " + (effect - NameAll.SPELL_EFFECT_CONE_BASE) +")";
        }
        else if (effect >= NameAll.SPELL_EFFECT_LINE_2 && effect <= NameAll.SPELL_EFFECT_LINE_8)
        {
            retValue = "line (" + (effect - NameAll.SPELL_EFFECT_LINE_2 + 2) + ")";
        }

        return retValue;
    }

    public static List<string> GetDFTString(int version, int dft)
    {
        List<string> retValue = new List<string>();
        if( version == NameAll.VERSION_CLASSIC)
        {
            if( dft == 0)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = Ability Strength");
            }
            else if( dft == 1)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = Ability Strength + INT");
            }
            else if (dft == 2)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = STR + (STR * COURAGE)/100");
            }
            else if (dft == 3)
            {
                retValue.Add("Hit % = Base Hit + Zodiac (Opposite Sex Only)");
                retValue.Add("Effect = Ability Strength");
            }
            else if (dft == 4)
            {
                retValue.Add("Hit % = Base Hit + Zodiac (Talk Skill)");
                retValue.Add("Effect = Ability Strength");
            }
            else if (dft == 6)
            {
                retValue.Add("Hit % = Base Hit + XA");
                retValue.Add("Effect = Ability Strength");
            }
            else if (dft == 8)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = XA * (STR/2)");
            }
            else if (dft == 10)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = XA * (STR/2 + 1)");
            }
            else if (dft == 15)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = (Target Max HP) * (Ability Strength)/100");
            }
            else if (dft == 18)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = (Target Max MP) * (Ability Strength)/100");
            }
            else if (dft == 20)
            {
                retValue.Add("Hit % = Base Hit + XA (Prevented by Maintenance)");
                retValue.Add("Effect = Ability Strength");
            }
            else if (dft == 21)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = XA * Ability Strength (Range is Actor's Move Range)");
            }
            else if (dft == 25)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = XA * Ability Strength");
            }
            else if (dft == 30)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("INT * (STR + 2)/2");
            }
            else if (dft == 31)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = Ability Strength * INT");
            }
            else
            {
                retValue.Add("");
                retValue.Add("");
            }

        }
        else
        {
            if (dft == 0)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = Ability Strength");
            }
            else if (dft == 1)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = Actor WIS * Target WIS * XA * Ability Strength / 10000");
            }
            else if (dft == 2)
            {
                retValue.Add("Hit % = Actor WIS * Target WIS * (Base Hit + XA) /10000");
                retValue.Add("Effect = Ability Strength");
            }
            else if (dft == 3)
            {
                retValue.Add("Hit % = Base Hit + XA");
                retValue.Add("Effect = Ability Strength");
            }
            else if (dft == 9)
            {
                retValue.Add("Hit % = Base Hit + XA");
                retValue.Add("Effect = Ability Strength + XA");
            }
            else if (dft == 4)
            {
                retValue.Add("Hit % = Base Hit + XA");
                retValue.Add("Effect = Ability Strength * XA");
            }
            else if (dft == 17)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = Ability Strength * XA");
            }
            else if (dft == 5)
            {
                retValue.Add("Hit % = Actor WIS * Target WIS * (Base Hit + XA) /10000");
                retValue.Add("Effect = Ability Strength + XA");
            }
            else if (dft == 6)
            {
                retValue.Add("Hit % = Base Hit + Color Modification");
                retValue.Add("Effect = Ability Strength + Color Modification");
            }
            else if (dft == 8)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = XA * (Actor STR + 1)");
            }
            else if (dft == 10)
            {
                retValue.Add("Hit % = Base Hit + XA (100% Knockback)");
                retValue.Add("Effect = Ability Strength * XA");
            }
            else if (dft == 11)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = XA * ( (Actor STR or AGI or INT) / 2 + Ability Strength)");
            }
            else if (dft == 15)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = XA * ( (Actor STR or AGI or INT) / 4 + Ability Strength)");
            }
            else if (dft == 16)
            {
                retValue.Add("Hit % = Base Hit");
                retValue.Add("Effect = XA * ( (Actor STR + Actor AGI + Ability Strength) / 4 )");
            }
            else if (dft == 21)
            {
                retValue.Add("Hit % = Base Hit + Color Modification");
                retValue.Add("Effect = Ability Strength * Color Modification * Color Modification");
            }
            else
            {
                retValue.Add("");
                retValue.Add("");
            }
        }
        return retValue;
    }

    //public static Dictionary<int,string> GetCommandSetDict()
    //{
    //    //Debug.Log("testting command set dict in calcCode " + PlayerPrefs.GetInt(NameAll.PP_COMMAND_SET_MAX_INDEX, NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE)
    //        //+ " " + NameAll.CUSTOM_CLASS_ID_START_VALUE);
    //    var retDict = new Dictionary<int, string>();
    //    int zMax = PlayerPrefs.GetInt(NameAll.PP_COMMAND_SET_MAX_INDEX,NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE);
    //    for( int i = NameAll.CUSTOM_CLASS_ID_START_VALUE; i <= zMax; i++)
    //    {
    //        //Debug.Log(" " + i);
    //        retDict.Add(i, PlayerPrefs.GetString(NameAll.PP_CUSTOM_COMMAND_SET_BASE + i, "Custom Set: " + (i - NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE)));
    //    }

    //    return retDict;
    //}

    public static string GetVersionFromInt(int z1)
    {
        if( z1 == NameAll.VERSION_AURELIAN)
        {
            return "Aurelian";
        }
        else if( z1 == NameAll.VERSION_CLASSIC)
        {
            return "Classic";
        }
        else if( z1 == NameAll.VERSION_ALL)
        {
            return "All";
        }
        else
        {
            return "";
        }
    }

	public static void SaveCombatLogSaveObjectList(List<CombatLogSaveObject> clsoList, string fileName)
	{
		Debug.Log("probably a wrong save path here, all the others ones save to Custom directory ");
		fileName = Application.dataPath + "/" + fileName;
		Serializer.Save<List<CombatLogSaveObject>>(fileName, clsoList);
	}

	#region WalkAround Mode
	//save and load Dictionary that stores map data (seed, time_int, which maps have been linked)
	public static void LoadWalkAroundMapData(int seed, int time_int)
	{
		Dictionary<Tuple<int, int>, Tuple<int, int>> mapDict;
		string fileName = Application.dataPath + "/Custom/WA/Maps/mapseed_" + seed + "_" + time_int + ".dat";
		mapDict = Serializer.Load<Dictionary<Tuple<int, int>, Tuple<int, int>>>(fileName); //Serializer.Load<ClassEditObject>(fileName);
		PlayerManager.Instance.SetMapDictionary(mapDict);
	}

	public static void SaveWalkAroundMapData()
	{
		var mapDict = PlayerManager.Instance.GetMapDictionary();
		var seedKey = new Tuple<int, int>(NameAll.MAP_DICT_SEED, NameAll.MAP_DICT_SEED);
		var timeIntKey = new Tuple<int, int>(NameAll.MAP_DICT_TIME_INT, NameAll.MAP_DICT_TIME_INT);
		string fileName = Application.dataPath + "/Custom/WA/Maps/mapseed_" + mapDict[seedKey].Item1 + "_" + mapDict[timeIntKey].Item1 + ".dat";
		Serializer.Save<Dictionary<Tuple<int, int>, Tuple<int, int>>>(fileName, mapDict);
	}

	//save units to file. currently either playerUnits or mapUnits
	//for now just saving as list of strings, in future can make it list List<string, List<int>> or whatever for saving statuses etc
	public static void SaveWalkAroundPlayerUnitList(List<PlayerUnit> puList, int seed, int timeInt, int mapX, int mapY, string listType)
	{
		var saveList = new List<string>();
		foreach( PlayerUnit pu in puList)
		{
			saveList.Add(BuildStringFromPlayerUnit(pu));
		}
		string fileName = Application.dataPath + "/Custom/WA/Units/" + listType + "mapseed_" + seed + "_" + timeInt + "_" + mapX + "_" + mapY + ".dat";
		Serializer.Save<List<string>>(fileName, saveList);
	}

	public static List<PlayerUnit> LoadWalkAroundPlayerUnitList(int seed, int timeInt, int mapX, int mapY, string listType)
	{
		List<string> puStringList;
		List<PlayerUnit> retList = new List<PlayerUnit>();
		string fileName = Application.dataPath + "/Custom/WA/Units/" + listType + "mapseed_" + seed + "_" + timeInt + "_" + mapX + "_" + mapY + ".dat";
		puStringList = Serializer.Load<List<string>>(fileName);
		foreach (string str in puStringList)
		{
			retList.Add(BuildPlayerUnit(str));
		}
		return retList;
	}

	#endregion

	//To do: wow this is poorly designed. remove the hardcoded in numbers. break when doesn't find stuff not when it misses 10. wtf was I even thinking here
	public static List<ClassEditObject> LoadCustomClassList(int version)
    {
        List<ClassEditObject> retValue = new List<ClassEditObject>();
        int zBreak = 0;
        for (int i = 1000; i <= 2000; i++)
        {
            if (zBreak > 10)
                break;

            ClassEditObject ce = CalcCode.LoadCustomClass(i, version); //Debug.Log("loading ceObject");
            if( ce != null)
                retValue.Add(ce);
            else
                zBreak += 1;
        }
        return retValue;
    }

    public static ClassEditObject LoadCustomClass(int ceIndex, int version)
    {
        ClassEditObject ce;
        string fileName = Application.dataPath + "/Custom/Jobs/" + ceIndex + "_class.dat"; Debug.Log("filename: " + fileName);
        if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
        {
            ce = Serializer.Load<ClassEditObject>(fileName); //Debug.Log("fileName found");
            if (ce != null && ce.Version == version)
                return ce;
        }
        
        return null;
    }

    public static ClassEditObject LoadCustomClass(int ceIndex)
    {
        ClassEditObject ce;
        string fileName = Application.dataPath + "/Custom/Jobs/" + ceIndex + "_class.dat"; //Debug.Log("filename: " + fileName);
        if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
        {
            ce = Serializer.Load<ClassEditObject>(fileName); //Debug.Log("fileName found");
            if (ce != null)
                return ce;
        }

        return null;
    }

    //loads the ceObject, used in ClassEdit, abilityManager,etc
    public static ClassEditObject LoadCEObject(int ceIndex)
    {
        ClassEditObject ce;
        string fileName = Application.dataPath + "/Custom/Jobs/" + ceIndex + "_class.dat";// Debug.Log(" trying to load filename: " + fileName);
        if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
        {
            ce = Serializer.Load<ClassEditObject>(fileName); //Debug.Log("fileName found");
        }
        else
        {
            //Debug.Log("Error, ceObject not found");
            ce = new ClassEditObject(ceIndex, NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE, NameAll.VERSION_AURELIAN);
        }

        return ce;
    }

    public static int GetNewCustomSpellNameIndex()
    {
        int zMin = NameAll.CUSTOM_SPELL_NAME_ID_START_VALUE;
        int zMax = NameAll.CUSTOM_SPELL_NAME_ID_START_VALUE + 1000;
        for ( int i = zMin; i <= zMax; i++)
        {
            SpellName sn = LoadCustomSpellName(i);
            if (sn == null)
                return i;
        }

        return zMax;
    }

    public static int GetNewCommandSet(int version, string zName)
    {
        for (int i = 1000; i <= 2000; i++)
        {
            CommandSet cs = LoadCustomCommandSet(i);
            if (cs == null)
            {
                cs = new CommandSet(i, zName, version);
                cs.Save();                
                return cs.CommandSetId;
            }
        }

        return 2000;

        //int z1 = PlayerPrefs.GetInt(NameAll.PP_COMMAND_SET_MAX_INDEX, NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE);
        //z1 += 1;
        //PlayerPrefs.SetInt(NameAll.PP_COMMAND_SET_MAX_INDEX, z1);
        //return z1;
        //string defaultString = "asdfasdfasdfasdfasdf";
        //for ( int i = NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE; i < 2000; i++)
        //{
        //    //finds the first number that doesn't have a saved command set
        //    string zTest = PlayerPrefs.GetString(NameAll.PP_CUSTOM_COMMAND_SET_BASE + i, defaultString);
        //    if( zTest == defaultString)
        //    {
        //        return i;
        //    }
        //}
        //return NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE;


    }

    public static void RenameCommandSet(int csId, string zName)
    {
        CommandSet cs = LoadCustomCommandSet(csId);
        if (cs != null)
        {
            cs.RenameAndSave(zName);
        }
    }

    public static string GetVCString(int vc)
    {
        if( vc == NameAll.VICTORY_TYPE_DEFEAT_PARTY)
        {
            return "Defeat All Enemies";
        }
        else if( vc == NameAll.VICTORY_TYPE_NONE)
        {
            return "None";
        }
        return "unknown";
    }

    public static Dictionary<int, LevelData> GetCustomLevelDict()
    {
        var retValue = new Dictionary<int, LevelData>();
        int zBreak = 0;
        LevelData ld;
        for (int i = 0; i < 100; i++)
        {
            string filePath = Application.dataPath + "/Custom/Levels/Custom/custom_" + i + ".dat";
            if (File.Exists(filePath))
            {
                ld = Serializer.Load<LevelData>(filePath); //Debug.Log("map loading 1");
                if (ld != null)
                {
                    retValue.Add(i, ld);
                }
            }
            else
            {
                zBreak += 1;
            }

            if (zBreak >= 5)
                break;
        }
        return retValue;
    }

    public static string GetMapString(int mapId)
    {
        LevelData ld;
        string retValue = "no map found";
        string filePath = Application.dataPath + "/Custom/Levels/Custom/custom_" + mapId + ".dat"; //Debug.Log(filePath);
        if (!File.Exists(filePath))
        {
            retValue = "no map found"; //Debug.Log("map not found");
        }
        else
        {
            ld = Serializer.Load<LevelData>(filePath); //Debug.Log("map loading 1");
            if (ld != null)
            {
                retValue = ld.levelName; //Debug.Log("map loading 2");
            }
        }
        return retValue;
    }

    public static string GetDialoguePhase(int phase)
    {
        if( phase == NameAll.DIALOGUE_PHASE_START)
        {
            return "Battle Start";
        }
        else if( phase == NameAll.DIALOGUE_PHASE_END)
        {
            return "Battle End";
        }
        return "unknown";
    }

    public static string GetDialogueSide(int side)
    {
        if (side == NameAll.DIALOGUE_SIDE_LEFT)
        {
            return "Left";
        }
        else if (side == NameAll.DIALOGUE_SIDE_RIGHT)
        {
            return "Right";
        }
        return "unknown";
    }

    public static string GetSpawnTeamString(int spawnTeam)
    {
        if(spawnTeam == NameAll.TEAM_ID_GREEN)
        {
            return "Spawn Type: Player Team (Team 1)";
        }
        else if(spawnTeam == NameAll.TEAM_ID_RED)
        {
            return "Spawn Type: Enemy Team (Team 2)";
        }
        Debug.Log("no team found is " + spawnTeam);
        return "";
    }

    public static string GetSpawnTypeString(int spawnType)
    {
        if (spawnType == NameAll.SPAWN_TYPE_RANDOM)
        {
            return "Spawn Type: Random";
        }

        return "";
    }

    public static string GetSpawnUnit(int unit)
    {
        if( unit == NameAll.CAMPAIGN_SPAWN_UNIT_NONE)
        {
            return "none";
        }
        else if( unit == NameAll.CAMPAIGN_SPAWN_UNIT_NULL)
        {
            return "null";
        }
        else if( unit >= 0)
        {
            return "" + unit;
        }
        return "";
    }

    public static string GetPickType(int pickType)
    {
        if( pickType == NameAll.DRAFT_TYPE_FREE_PICK)
        {
            return "Free Pick";
        }
        else if( pickType == NameAll.DRAFT_TYPE_RANDOM_DRAFT)
        {
            return "Random Draft";
        }
        else if( pickType == NameAll.DRAFT_TYPE_TIMED_PICK)
        {
            return "Timed Pick";
        }
        return "unknown";
    }

    public static string GetAIType(int aiType)
    {
        if( aiType == NameAll.AI_TYPE_HUMAN_VS_AI)
        {
            return "Human Player (Player 1) vs. Computer Player (Player 2)";
        }
        else if( aiType == NameAll.AI_TYPE_HUMAN_VS_HUMAN)
        {
            return "Human Player (Player 1) vs. Human Player (Player 2)";
        }
        else if( aiType == NameAll.AI_TYPE_AI_VS_AI)
        {
            return "Computer Player (Player 1) vs. Computer Player (Player 2)";
        }

        return "unknown";
    }

    public static List<CampaignCampaign> LoadCampaignList()
    {
        List<CampaignCampaign> retValue = new List<CampaignCampaign>();
        int zBreak = 0;
        for (int i = NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE; i <= (NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE + 1000); i++)
        {
            CampaignCampaign cc = CalcCode.LoadCampaignCampaign(i); //Debug.Log("reached create new campaign " + i);
            if (cc != null)
            {
                retValue.Add(cc);
            }
            else
            {
                zBreak += 1;
            }
            if (zBreak >= 5)
                break;
        }
        return retValue;
    }

    public static CampaignCampaign LoadCampaignCampaign(int campaignId)
    {
        string fileName;
        if (campaignId < NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE)
        {
            fileName = Application.dataPath + "/Custom/Campaigns/Aurelian/" + campaignId + "_campaign.dat";
        }
        else
        {
            fileName = Application.dataPath + "/Custom/Campaigns/Custom/" + campaignId + "_campaign.dat";
        }
        //Debug.Log(fileName);
        if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
        {
            return Serializer.Load<CampaignCampaign>(fileName);
        }
        return null;
    }

    public static CampaignDialogue LoadCampaignDialogue(int campaignId)
    {
        string fileName;
        if (campaignId < NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE)
        {
            fileName = Application.dataPath + "/Custom/Campaigns/Aurelian/" + campaignId + "_dialogue.dat";
        }
        else
        {
            fileName = Application.dataPath + "/Custom/Campaigns/Custom/" + campaignId + "_dialogue.dat";
        }
        //Debug.Log(fileName);
        if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
        {
            return Serializer.Load<CampaignDialogue>(fileName);
        }
        return null;
    }

    public static Dictionary<int,PlayerUnit> LoadPlayerUnitDict(int version, bool isCampaign, int campaignId, bool isOffline = true)
    {
        Dictionary<int, PlayerUnit> retValue = new Dictionary<int, PlayerUnit>();
        string filePathBase = Application.dataPath ;
        if(isCampaign)
        {
            filePathBase += "/Custom/Units/Campaign_" + campaignId + "/unit_";
        }
        else
        {
            if( version == NameAll.VERSION_CLASSIC)
            {
                filePathBase += "/Custom/Units/Classic/unit_";
            }
            else
            {
                filePathBase += "/Custom/Units/Aurelian/unit_";
            }
        }
        int zBreak = 0;
        for( int i = 0; i <= 100; i++)
        {
            if (zBreak >= 10)
                break;
            string filePath = filePathBase + i + ".dat";
            if (File.Exists(filePath))
            {
                PlayerUnit pu = Serializer.Load<PlayerUnit>(filePath);
                if( isOffline)
                    retValue.Add(i, pu);
                else //in online no custom classes or secondaries yet
                {
                    if (pu.IsEligibleForOnline())
                        retValue.Add(i, pu);
                    
                }
            }
            else
            {
                zBreak += 1;
            }
        }
        return retValue;
    }
    

    public static List<SpellName> LoadCustomSpellNameList()
    {
        List<SpellName> retValue = new List<SpellName>();
        int zBreak = 0;
        //int maxIndex = PlayerPrefs.GetInt(NameAll.PP_ABILITY_EDIT_MAX_INDEX, 10000); //Debug.Log("max index is " + maxIndex);
        for (int i = 10000; i <= 11000; i++) //starts at 10,000
        {
            if (zBreak >= 10)
                break;
            SpellName sn = LoadCustomSpellName(i);
            if (sn != null)
                retValue.Add(sn);
            else
                zBreak += 1;
        }
        return retValue;
    }

    public static SpellName LoadCustomSpellName(int snIndex)
    {
        string fileName = Application.dataPath + "/Custom/Spells/" + snIndex + "_sn.dat";
        if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
        {
            return Serializer.Load<SpellName>(fileName);
        }
        else
        {
            return null;
            //Debug.Log("Error, spellName not found");
            //sn = new SpellName(PlayerPrefs.GetInt(NameAll.PP_ABILITY_EDIT_MAX_INDEX, 10000), currentCommandSet);
        }
    }

    public static List<CommandSet> LoadCustomCommandSetList(int version)
    {
        List<CommandSet> retValue = new List<CommandSet>();
        int zBreak = 0;
        for (int i = 1000; i <= 2000; i++) //starts at 10,000
        {
            if (zBreak >= 10)
                break;
            CommandSet cs = LoadCustomCommandSet(i);
            if (cs != null)
            {
                if( cs.Version == version)
                    retValue.Add(cs);
            }
            else
                zBreak += 1;
        }
        return retValue;
    }

    public static CommandSet LoadCustomCommandSet(int id)
    {
        string fileName = Application.dataPath + "/Custom/CommandSets/" + id+ "_command_set.dat";
        if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
        {
            return Serializer.Load<CommandSet>(fileName);
        }
        else
        {
            return null;
            //Debug.Log("Error, spellName not found");
        }
    }

    public static string GetCommandSetName(int id)
    {
        string fileName = Application.dataPath + "/Custom/CommandSets/" + id + "_command_set.dat";
        if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
        {
            return Serializer.Load<CommandSet>(fileName).CommandSetName;
        }
        else
        {
            return "";
            //Debug.Log("Error, spellName not found");
        }
    }

    //loads levels for campaign and aurelian
    public static Dictionary<int, LevelData> LoadLevelDict(bool isCampaign, int campaignId)
    {
        Dictionary<int, LevelData> retValue = new Dictionary<int, LevelData>();
        string filePathBase = Application.dataPath;
        if (isCampaign)
        {
            filePathBase += "/Custom/Levels/Campaign_" + campaignId + "/custom_";
        }
        else
        {
            filePathBase += "/Custom/Levels/Aurelian/custom_";
        }
       
        int zBreak = 0;
        for (int i = 0; i <= 100; i++)
        {
            if (zBreak >= 5)
                break;
            string filePath = filePathBase + i + ".dat";
            if (File.Exists(filePath))
            {
                LevelData ld = Serializer.Load<LevelData>(filePath);
                retValue.Add(i, ld);
            }
            else
            {
                zBreak += 1;
            }
        }
        return retValue;
    }

    public static string GetTitleFromPlayerUnit(PlayerUnit pu)
    {
        string retValue = "";
        retValue = "Unit: " + pu.UnitName;
        return retValue;
    }

    public static string GetDetailsFromPlayerUnit(PlayerUnit pu)
    {
        string retValue = "";
        retValue = "Level: " + pu.Level + " " + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY, pu.ClassId)
                        + "/" + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_SECONDARY, pu.AbilitySecondaryCode);
        return retValue;
    }

    public static CampaignLevel LoadCampaignLevel(int campaignId)
    {
        string fileName;
        if (campaignId < NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE)
        {
            fileName = Application.dataPath + "/Custom/Campaigns/Aurelian/" + campaignId + "_level.dat";
        }
        else
        {
            fileName = Application.dataPath + "/Custom/Campaigns/Custom/" + campaignId + "_level.dat";
        }
       
        if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
        {
            return Serializer.Load<CampaignLevel>(fileName);
        }
        return null;
        //return new CampaignLevel(campaignId);
    }

    public static CampaignSpawn LoadCampaignSpawn(int campaignId)
    {

        string fileName;
        if (campaignId < NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE)
        {
            fileName = Application.dataPath + "/Custom/Campaigns/Aurelian/" + campaignId + "_spawn.dat";
        }
        else
        {
            fileName = Application.dataPath + "/Custom/Campaigns/Custom/" + campaignId + "_spawn.dat";
        }

        if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
        {
            return Serializer.Load<CampaignSpawn>(fileName);
        }
        return null;
    }

    //called in CampaignLauncher, converts Spawn Objects into Player Units
    public static PlayerUnit GetPlayerUnitFromSpawnObject(SpawnObject so, List<PlayerUnit> puList)
    {
        PlayerUnit retValue = null;
        //return null, user selects in campaign launcher
        if (so.SpawnType == NameAll.SPAWN_TYPE_USER_SELECT)
            return null;

        //Debug.Log("FINISH THE CODE HERE");
        //need to find which spawn types have a unit on them (are not null
        //if it is random, then randomly select between them
        //if not then take the first unit
        //int spawnType = so.SpawnType;
        int unitId = NameAll.CAMPAIGN_SPAWN_UNIT_NULL;
        List<int> randomList = new List<int>();

        if (so.Unit1 != NameAll.CAMPAIGN_SPAWN_UNIT_NULL)
            randomList.Add(so.Unit1);
        if (so.Unit2 != NameAll.CAMPAIGN_SPAWN_UNIT_NULL)
            randomList.Add(so.Unit2);
        if (so.Unit3 != NameAll.CAMPAIGN_SPAWN_UNIT_NULL)
            randomList.Add(so.Unit3);
        if (so.Unit4 != NameAll.CAMPAIGN_SPAWN_UNIT_NULL)
            randomList.Add(so.Unit4);
        if (so.Unit5 != NameAll.CAMPAIGN_SPAWN_UNIT_NULL)
            randomList.Add(so.Unit5);

        if ( so.SpawnType == NameAll.SPAWN_TYPE_RANDOM)
        {
            int r = UnityEngine.Random.Range(0, randomList.Count); //Debug.Log("random variable is " + r);
            unitId = randomList[r];
        }
        else
        {
            unitId = so.Unit1;
        }

        if( unitId < 0)
        {
            retValue = null;
        }
        else
        {
            try
            {
                retValue = puList[unitId];
            }
            catch(Exception e)
            {
                retValue = null;
            }
        }
        return retValue;
    }

    public static StoryPoint LoadStoryPoint(int storyId)
    {
        string fileName;
        if (storyId < 1000)
        {
            fileName = Application.dataPath + "/Custom/Stories/Aurelian/" + storyId + "_story_point.dat";
        }
        else
        {
            fileName = Application.dataPath + "/Custom/Stories/Custom/" + storyId + "_story_point.dat";
        }

        if (File.Exists(fileName))
        {
            return Serializer.Load<StoryPoint>(fileName);
        }
        return null;
    }

    public static StoryCutScene LoadStoryCutScene(int storyId)
    {
        string fileName;
        if (storyId < 1000)
        {
            fileName = Application.dataPath + "/Custom/Stories/Aurelian/" + storyId + "_story_cut_scene.dat";
        }
        else
        {
            fileName = Application.dataPath + "/Custom/Stories/Custom/" + storyId + "_story_cut_scene.dat";
        }

        if (File.Exists(fileName))
        {
            return Serializer.Load<StoryCutScene>(fileName);
        }
        return null;
    }

    public static StoryItem LoadStoryItem(int storyId)
    {
        string fileName;
        if (storyId < 1000)
        {
            fileName = Application.dataPath + "/Custom/Stories/Aurelian/" + storyId + "_story_item.dat";
        }
        else
        {
            fileName = Application.dataPath + "/Custom/Stories/Custom/" + storyId + "_story_item.dat";
        }

        if (File.Exists(fileName))
        {
            return Serializer.Load<StoryItem>(fileName);
        }
        return null;
    }

    //get a list for a specific shop
    public static List<ItemObject> LoadStoryItemItemObjectList(int storyId, int pointId, int storyInt)
    {
        StoryItem si = LoadStoryItem(storyId);
        if (si != null)
            return si.GetStoryItemListForShop(pointId, storyInt);

        return null;
    }

    public static void DeleteStory(int storyId)
    {
        string fileName;
        string fileName2;
        string fileName3;
        string fileName4;
        if (storyId < 1000)
            return;
        else
        {
            fileName = Application.dataPath + "/Custom/Stories/Custom/" + storyId + "_story_object.dat";
            fileName2 = Application.dataPath + "/Custom/Stories/Custom/" + storyId + "_story_point.dat";
            fileName3 = Application.dataPath + "/Custom/Stories/Custom/" + storyId + "_story_cut_scene.dat";
            fileName4 = Application.dataPath + "/Custom/Stories/Custom/" + storyId + "_story_item.dat";
        }

        if (File.Exists(fileName))
            File.Delete(fileName);

        if (File.Exists(fileName2))
            File.Delete(fileName2);

        if (File.Exists(fileName3))
            File.Delete(fileName3);

        if (File.Exists(fileName4))
            File.Delete(fileName4);

    }
    public static StoryObject LoadStoryObject(int storyId)
    {
        string fileName;
        if (storyId < 1000)
        {
            fileName = Application.dataPath + "/Custom/Stories/Aurelian/" + storyId + "_story_object.dat";
        }
        else
        {
            fileName = Application.dataPath + "/Custom/Stories/Custom/" + storyId + "_story_object.dat";
        }

        if (File.Exists(fileName)) 
        {
            return Serializer.Load<StoryObject>(fileName);
        }
        return null;
    }

    public static List<StoryObject> LoadStoryObjectList()
    {
        List<StoryObject> retValue = new List<StoryObject>();
        int zBreak = 0;
        for (int i = 1000; i <= 2000; i++) //custom start at 1000
        {
            if (zBreak >= 10)
                break;
            StoryObject so = LoadStoryObject(i);
            if (so != null)
            {
                    retValue.Add(so);
            }
            else
                zBreak += 1;
        }
        return retValue;
    } 

    public static string GetStoryMapImageString(int mapId)
    {
        //loaded from resources folder
        return "MapImages/story_map_" + mapId;
    }

    public static StoryObject GetNewStoryObject()
    {
        int zMin = 1000;
        int zMax = 1000 + 1000;
        for (int i = zMin; i <= zMax; i++)
        {
            StoryObject so = LoadStoryObject(i);
            if (so == null)
                return new StoryObject(i, NameAll.STORY_MAP_0, "Custom Story " + i);
        }

        return new StoryObject(2000, NameAll.STORY_MAP_0, "Custom Story " + 2000);
    }

    //need to return int so the storysave object is updated with the saveId
    public static int SaveStorySave(bool isNewSave, StorySave ss)
    {
        string fileName;
        if ( ss.StorySaveId == NameAll.NULL_INT || isNewSave)
        {
            //Debug.Log("testing story save " + isNewSave + " " + ss.StorySaveId);
            for( int i = 0; i < 1000; i++)
            {
                fileName = Application.dataPath + "/Custom/Stories/Saves/" + i + "_story_save.dat"; //Debug.Log(fileName);
                if (File.Exists(fileName))
                {
                    //Debug.Log("testing");
                    continue;
                }
                else
                {
                    ss.StorySaveId = i; //Debug.Log("testing 2");
                    fileName = Application.dataPath + "/Custom/Stories/Saves/" + i + "_story_save.dat";
                    Serializer.Save<StorySave>(fileName, ss);
                    return ss.StorySaveId;
                }
            }
        }
        else
        {
            fileName = Application.dataPath + "/Custom/Stories/Saves/" + ss.StorySaveId + "_story_save.dat";
            Serializer.Save<StorySave>(fileName, ss);
            return ss.StorySaveId;
        }
        Debug.Log("ERROR: story save failed to save");
        return ss.StorySaveId;
    }

    //temp save, used for going between cutscenes/battles and storymode
    public static void SaveTempStorySave(StorySave ss)
    {
        string fileName = Application.dataPath + "/Custom/Stories/Saves/TempSave/temp_story_save.dat";
        Serializer.Save<StorySave>(fileName, ss);
    }

    public static List<StorySave> LoadStorySaveList()
    {
        List<StorySave> retValue = new List<StorySave>();
        int zBreak = 0;
        for ( int i = 0; i < 1000; i++)
        {
            StorySave ss = LoadStorySave(i, false);
            if( ss != null)
            {
                retValue.Add(ss);
            }
            else
            {
                zBreak += 1;
                if (zBreak >= 10)
                    break;
            }
        }
        return retValue;
    }
    public static StorySave LoadStorySave(int saveId, bool isTempSave)
    {
        if(isTempSave)
        {
            string fileName = Application.dataPath + "/Custom/Stories/Saves/TempSave/temp_story_save.dat";
            if (File.Exists(fileName))
                return Serializer.Load<StorySave>(fileName);

            return null;
        }
        else
        {
            string fileName = Application.dataPath + "/Custom/Stories/Saves/" + saveId + "_story_save.dat";
            if (File.Exists(fileName))
            {
                return Serializer.Load<StorySave>(fileName);
            }
        }
        return null;
    }

    //loads the custom player uints for the campaign
    public static List<PlayerUnit> GetPlayerUnitListForCampaign(int version, int campaignId)
    {
        List<PlayerUnit> retValue = new List<PlayerUnit>();
        string filePathBase;

        if (campaignId == NameAll.NULL_UNIT_ID || campaignId < 0)
        {
            if (version == NameAll.VERSION_CLASSIC)
            {
                filePathBase = Application.dataPath + "/Custom/Units/Classic/";
            }
            else
            {
                filePathBase = Application.dataPath + "/Custom/Units/Aurelian/";
            }
        }
        else
        {
            filePathBase = Application.dataPath + "/Custom/Units/Campaign_" + campaignId + "/";
        }


        int z1 = 0;
        for (int i = 0; i < 1000; i++)
        {
            if (z1 >= 5)
            {
                break;
            }

            string puFilePath = filePathBase + "unit_" + i + ".dat"; //Debug.Log("pu filepath" + puFilePath);
            if (File.Exists(puFilePath))
            {
                PlayerUnit pu = Serializer.Load<PlayerUnit>(puFilePath);
                if (pu != null)
                {
                    pu.TurnOrder = i;// SetTurn_order(i); //used in campaign edit to know wher to load from
                    retValue.Add(pu); //Debug.Log("player exists " + pu.TurnOrder);
                }
                else
                {
                    z1 += 1;
                }

            }
            else
            {
                z1 += 1;
            }
        }

        return retValue;
    }

    public static List<ItemObject> LoadCustomItemObjectList()
    {
        List<ItemObject> retValue = new List<ItemObject>();
        int zBreak = 0;
        for (int i = 10000; i < 11000; i++)
        {
            ItemObject io = LoadCustomItemObject(i);
            if (io != null)
            {
                retValue.Add(io);
            }
            else
            {
                zBreak += 1;
                if (zBreak >= 10)
                    break;
            }
        }
        return retValue;
    }

    public static ItemObject LoadCustomItemObject(int id)
    {
        string fileName = "";

        if (id >= 10000)
        {
            fileName = Application.dataPath + "/Custom/Items/" + id + "_item_object.dat";
        }

        if (File.Exists(fileName))
        {
            return Serializer.Load<ItemObject>(fileName);
        }
        return null;
    }

    public static int GetNewCustomItem()
    {
        int zMin = 10000;
        int zMax = 10000 + 1000;
        for (int i = zMin; i <= zMax; i++)
        {
            ItemObject io = LoadCustomItemObject(i);
            if (io == null)
                return i;
        }

        return zMax;
    }

    public static void SaveCustomItemObject(ItemObject io)
    {
        string fileName = Application.dataPath + "/Custom/Items/" + io.ItemId + "_item_object.dat";
        Serializer.Save<ItemObject>(fileName, io);
    }

	public static void SaveCustomClassEditObject(ClassEditObject ceo)
	{
		string filePath = Application.dataPath + "/Custom/Jobs/";
		string filename = filePath + ceo.ClassId + "_class.dat"; Debug.Log("trying to save new job filepath: " + filename);
		//Debug.Log("trying to save ceObject  is: " + ceo.ToString());
		//Debug.Log("trying to save ceObject  is: " + ceo.GetCEObjectAsString());
		Serializer.Save<ClassEditObject>(filename, ceo);

		//Debug.Log("current ceObject  is: " + ceObject.GetCEObjectAsString());
		//ClassEditObject testCEObject = CalcCode.LoadCEObject(ceo.ClassId);
		//Debug.Log("saved ceObject  is: " + testCEObject.GetCEObjectAsString());
	}

}
