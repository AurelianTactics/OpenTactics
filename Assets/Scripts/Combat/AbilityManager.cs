using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AbilityManager : Singleton<AbilityManager>
{

    private int modVersion = NameAll.VERSION_CLASSIC;
    int ioType = NameAll.ITEM_MANAGER_SO;

    private static List<AbilityObject> sAbilityList;

    //private static Dictionary<int, string> sPrimaryDict;
    //private static Dictionary<int, string> sSecondaryDict;
    //private static Dictionary<int, string> sReactionDict;
    //private static Dictionary<int, string> sSupportDict;
    //private static Dictionary<int, string> sMovementDict;
    private static Dictionary<int, string> sColorDict;

    private static string PRIMARY = "primary";
    private static string SECONDARY = "secondary";
    private static string REACTION = "reaction";
    private static string SUPPORT = "support";
    private static string MOVEMENT = "movement";

    void Start()
    {
        modVersion = PlayerPrefs.GetInt(NameAll.PP_MOD_VERSION, NameAll.VERSION_CLASSIC);
    }

    protected AbilityManager()
    { // guarantee this will be always a singleton only - can't use the constructor!
        //myGlobalVar = "asdf";
        
        if (ioType == NameAll.ITEM_MANAGER_SIMPLE) //gets all the items by version, used in character create and non combat
        {
            NameAbility na = new NameAbility();
            //sPrimaryDict = na.GetPrimaryDict();
            //sSecondaryDict = na.GetSecondaryDict();
            //sReactionDict = na.GetReactionDict();
            //sSupportDict = na.GetSupportDict();
            //sMovementDict = na.GetMovementDict();
            sColorDict = na.GetColorDict();
            AbilityObject ao = new AbilityObject();
            sAbilityList = ao.GetAbilityList(modVersion);
        }
        else
        {
            sAbilityList = new List<AbilityObject>();
        } 
    }

    public void SetIoType(int type)
    {
        ioType = type;
        if (ioType == NameAll.ITEM_MANAGER_SIMPLE) //gets all the items by version, used in character create and non combat
        {
            modVersion = PlayerPrefs.GetInt(NameAll.PP_MOD_VERSION, NameAll.VERSION_CLASSIC);
            NameAbility na = new NameAbility();
            //sPrimaryDict = na.GetPrimaryDict();
            //sSecondaryDict = na.GetSecondaryDict();
            //sReactionDict = na.GetReactionDict();
            //sSupportDict = na.GetSupportDict();
            //sMovementDict = na.GetMovementDict();
            sColorDict = na.GetColorDict();
            AbilityObject ao = new AbilityObject();
            sAbilityList = ao.GetAbilityList(modVersion);
        }
        else //just empty lists, loads itemobjects on demand (third alternative would be just to make a simple list/dict of only the items in the scene
        {
            ClearAll();
        }
    }

    //called in 2player game as player 2 may have some initialization errors
    //public void Initialize()
    //{
    //    NameAbility na = new NameAbility();
    //    sPrimaryDict = na.GetPrimaryDict();
    //    sSecondaryDict = na.GetSecondaryDict();
    //    sReactionDict = na.GetReactionDict();
    //    sSupportDict = na.GetSupportDict();
    //    sMovementDict = na.GetMovementDict();
    //    sColorDict = na.GetColorDict();
    //}

    public void ClearAll()
    {
        //sPrimaryDict.Clear();
        //sSecondaryDict.Clear();
        //sReactionDict.Clear();
        //sSupportDict.Clear();
        //sMovementDict.Clear();
        sColorDict.Clear();
        sAbilityList.Clear();
    }

    //if i'm lazy and just want to build it mid game
    public void BuildAbilityList()
    {
        AbilityObject ao = new AbilityObject();
        sAbilityList = ao.GetAbilityList(modVersion);
    }

    public string GetAbilityName(int slot, int abilityId)
    {
        if( slot == NameAll.ABILITY_SLOT_PRIMARY && abilityId >= NameAll.CUSTOM_CLASS_ID_START_VALUE)
        {
            ClassEditObject ce = CalcCode.LoadCEObject(abilityId);
            return ce.ClassName;
        }

        //Debug.Log("slot" + slot + " " + abilityId);
        if( ioType == NameAll.ITEM_MANAGER_SIMPLE)
        {
            foreach (AbilityObject a in sAbilityList)
            {
                if (a.Slot == slot && a.SlotId == abilityId)
                {
                    return a.AbilityName;
                }
            }
        }
        else
        {
            AbilityObject ao = GetAbilityObject(slot, abilityId);
            return ao.AbilityName;
        }
        return "";
        //Dictionary<int, string> tempDict = new Dictionary<int, string>();
        //if( slot.Equals(SECONDARY) )
        //{
        //    tempDict = sSecondaryDict;
        //}
        //else if (slot.Equals(REACTION))
        //{
        //    tempDict = sReactionDict;
        //}
        //else if (slot.Equals(SUPPORT))
        //{
        //    tempDict = sSupportDict;
        //}
        //else if (slot.Equals(MOVEMENT))
        //{
        //    tempDict = sMovementDict;
        //}
        //else if (slot.Equals(PRIMARY))
        //{
        //    tempDict = sPrimaryDict;
        //} else
        //{
        //    Debug.Log("ERROR: returning no ability name");
        //    return "";
        //}

        //return tempDict[abilityId];
    }

    public AbilityObject GetAbilityObject(int slot, int abilityId)
    {
        //if( slot == NameAll.ABILITY_SLOT_PRIMARY && abilityId )
        AbilityData ad = Resources.Load<AbilityData>("Abilities/ability_" + slot + "_"+abilityId);
        //Debug.Log("in item data id object is" + id.item_name + "asdf" + itemId);
        AbilityObject ao = new AbilityObject(ad);
        //Debug.Log("in item data is object is" + io.GetItemName() + "asdf" + io.GetStatusName());
        ad = null;
        Resources.UnloadAsset(ad);// Resources.UnloadUnusedAssets(); //not sure which of these to call
        return ao;
    }

    //public Dictionary<int, string> GetPrimaryDict()
    //{
    //    //Debug.Log("returning sPrimaryDict");
    //    return sPrimaryDict;
    //}

    public Dictionary<int, string> GetColorDict()
    {
        //Debug.Log("returning sPrimaryDict");
        return sColorDict;
    }

    public bool IsInnateAbility(int classId, int abilityId, int abilitySlot)
    {
        if( abilitySlot == NameAll.ABILITY_SLOT_SUPPORT)
        {
            if (abilityId == NameAll.SUPPORT_MARTIAL_ARTS) //martial arts
            {
                if (classId == NameAll.CLASS_MONK || classId == NameAll.CLASS_MIME)
                {
                    return true;
                }
            }
            else if (abilityId == NameAll.SUPPORT_DUAL_WIELD)
            {
                if (classId == NameAll.CLASS_ROGUE)
                {
                    return true;
                }
            }
            //else if (abilityId == NameAll.SUPPORT_TWO_HANDS) //martial arts
            //{
            //    if (classId == NameAll.CLASS_SAMURAI)
            //    {
            //        return true;
            //    }
            //}
            else if (abilityId == NameAll.SUPPORT_TWO_SWORDS) 
            {
                if (classId == NameAll.CLASS_NINJA)
                {
                    return true;
                }
            }
            else if (abilityId == NameAll.SUPPORT_THROW_ITEM) 
            {
                if (classId == NameAll.CLASS_CHEMIST)
                {
                    return true;
                }
            }
            else if (abilityId == NameAll.SUPPORT_CONCENTRATE) 
            {
                if (classId == NameAll.CLASS_MIME)
                {
                    return true;
                }
            }
        }
        else if (abilitySlot == NameAll.ABILITY_SLOT_PRIMARY)
        {
            //storing the elemental ups in the primary, the abilityId is the item elementa
            if (classId == NameAll.CLASS_FIRE_MAGE &&
                (abilityId == NameAll.STATUS_ID_STRENGTHEN_FIRE || abilityId == NameAll.STATUS_ID_HALF_FIRE))
            {
                return true;
            }
            else if (classId == NameAll.CLASS_HEALER &&
                (abilityId == NameAll.STATUS_ID_STRENGTHEN_LIGHT || abilityId == NameAll.STATUS_ID_STRENGTHEN_UNDEAD))
            {
                return true;
            }
            else if (classId == NameAll.CLASS_WARRIOR && abilityId == NameAll.STATUS_ID_STRENGTHEN_WEAPON)
            {
                return true;
            }
        }
        return false;
    }

    

    //called in UIUnitInfoPanel for display purposes
    public string GetInnateString( int classId)
    {
        string zString = "";
        if( classId >= 100)
        {
            if (classId == NameAll.CLASS_FIRE_MAGE)
            {
                zString = "Flame Touched";
            }
            else if (classId == NameAll.CLASS_HEALER)
            {
                zString = "Light's Blessing";
            }
            else if (classId == NameAll.CLASS_NECROMANCER)
            {
                zString = "Channel";
            }
            else if (classId == NameAll.CLASS_ARTIST)
            {
                zString = "Iconoclast";
            }
            else if (classId == NameAll.CLASS_APOTHECARY)
            {
                zString = "Magic Def. Up";
            }
            else if (classId == NameAll.CLASS_DEMAGOGUE)
            {
                zString = "Hone";
            }
            else if (classId == NameAll.CLASS_BRAWLER)
            {
                zString = "Bare Handed";
            }
            else if (classId == NameAll.CLASS_WARRIOR)
            {
                zString = "Strong Grip, Weapon Element Up";
            }
            else if (classId == NameAll.CLASS_CENTURION)
            {
                zString = "Phys. Def. Up";
            }
            else if (classId == NameAll.CLASS_ROGUE)
            {
                zString = "Dual Wield";
            }
            else if (classId == NameAll.CLASS_RANGER)
            {
                zString = "Focus";
            }
            else if (classId == NameAll.CLASS_DRUID)
            {
                zString = "Neutral Def. Up";
            }
        }
        else
        {
            if( classId == NameAll.CLASS_NINJA)
            {
                zString = "Two Swords";
            }
            else if( classId == NameAll.CLASS_MONK )
            {
                zString = "Martial Arts";
            }
            else if (classId == NameAll.CLASS_MIME)
            {
                zString = "Martial Arts, Concentrate";
            }
            else if (classId == NameAll.CLASS_CHEMIST)
            {
                zString = "Throw Item";
            }
        }
        return zString;
    }

    public List<AbilityObject> GetAbilityList(int classId, int slot, bool isGetCustomClass = false)
    {
        List<AbilityObject> tempList = new List<AbilityObject>();
        
        if (classId == NameAll.CLASS_MIME && slot != NameAll.ABILITY_SLOT_PRIMARY)
        {
            return tempList;
        }
        Debug.Log("asdf " + slot + " asdf " + sAbilityList.Count);
        foreach (AbilityObject a in sAbilityList)
        {
            if (a.Slot == slot)
            {
                tempList.Add(a);
            }
        }

        if( slot == NameAll.ABILITY_SLOT_PRIMARY && isGetCustomClass)
        {
			Debug.Log(" getting customclass list " + modVersion);
			var customList = CalcCode.LoadCustomClassList(modVersion);
            foreach(ClassEditObject ce in customList)
            {
                AbilityObject a = new AbilityObject(ce);
                tempList.Add(a);
            }
        }

        return tempList;
    }

    
    public int GetRandomAbility(int classId, int abilitySlot, bool deactivateSpecial)
    {
        if( classId == NameAll.CLASS_MIME)
        {
            return 0;
        }

        IEnumerable<int> filteringQuery;
        if ( abilitySlot == NameAll.ABILITY_SLOT_SECONDARY)
        {
            filteringQuery = from ao in sAbilityList where ao.Slot == abilitySlot && ao.SlotId != classId && ao.SlotId != 0 select ao.SlotId;
        }
        else
        {
            if(deactivateSpecial)
            {
                filteringQuery = from ao in sAbilityList where ao.Slot == abilitySlot && ao.SlotId != 0 && ao.ClassId != 1919 select ao.SlotId;
            }
            else
            {
                filteringQuery = from ao in sAbilityList where ao.Slot == abilitySlot && ao.SlotId != 0 select ao.SlotId;
            }
        }

        
        if( filteringQuery.Count() > 0)
        {
            int z1 = UnityEngine.Random.Range(0, filteringQuery.Count());
            return filteringQuery.ElementAt(z1);
        }
        return 0;
    }

    //called in puInfo list in onCombat situations
    public string GetAbilityStatusNames(PlayerUnit pu)
    {
        string zString = "";
        AbilityObject ao = GetAbilityObject(NameAll.ABILITY_SLOT_MOVEMENT, pu.AbilityMovementCode);
        if( ao.SlotId == NameAll.MOVEMENT_FLOAT)
        {
            zString += " float";
        }
        return zString;
        
    }

    //called from CombatMoveSequenceState in case of an after move effect like Move HP up etc
    //public bool IsOnMoveEffect(PlayerUnit pu)
    //{
    //    if( NameAll.IsClassicClass(pu.ClassId))
    //    {
    //        if (pu.AbilityMovementCode == NameAll.MOVEMENT_MOVE_HP_UP || pu.AbilityMovementCode == NameAll.MOVEMENT_MOVE_MP_UP)
    //            return true;
    //    }
    //    else
    //    {
    //        if( pu.AbilityMovementCode == )
    //    }
    //    return false;
    //}

}

//public static enum
