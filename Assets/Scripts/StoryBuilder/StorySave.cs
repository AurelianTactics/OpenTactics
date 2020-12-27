using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

//object for saving the user's current story progress
[Serializable]
public class StorySave {

    public int StoryId { get; set; } //story that this save is associated with
    public int StoryInt { get; set; } //progress in that story
    public int PointId { get; set; } //current point the party is at. for now just used for shops but can be used for party travel distance etc
    public int Day { get; set; } //current day of the story, not implemented
    public int Gold { get; set; } //gold party has
    public int PartyXP { get; set; }
    public int PartyAP { get; set; }
    public List<StoryPointObject> storyPointObjectList; //need it here to know what cutScenes and battles have been consumed
    public int StorySaveId { get; set; } //assigned when the story is actually saved to prevent over-writing
    private List<PlayerUnit> unitList;
    public List<ItemInventoryObject> itemList; //for managing item inventory

    public List<UnitXPObject> unitXPList;
    public List<UnitAbilityObject> abilityLearnedList;

    private int uniqueUnitId;


    //unit list
    //item list
    //experience/job points

    //item list: how to story when saved, how to juggle equipped/not equipped items
//experience: have free XP(can be used on anyone) and unit specfic XP
//job points? how to spend points on unit to purchase abilities(for now maybe just combine//with XP)

    public StorySave( int zStoryId, int zStoryInt, int zGold, List<StoryPointObject> zList, List<PlayerUnit> puList )
    {
        this.StoryId = zStoryId;
        this.StoryInt = zStoryInt;
        this.Gold = zGold;
        this.storyPointObjectList = zList;
        this.StorySaveId = NameAll.NULL_INT;
        this.unitList = puList;
        this.PointId = NameAll.NULL_INT;
        this.PartyAP = 0;
        this.PartyXP = 0;

        this.uniqueUnitId = 0;
        AssignUnitId(puList);

        this.unitXPList = CreateUnitXPList(puList);
        this.abilityLearnedList = CreateAblityLearnedList(puList);
    }

    void AssignUnitId(List<PlayerUnit> puList)
    {
        for( int i = 0; i < puList.Count; i++)
        {
            puList[i].TurnOrder = GetUniqueUnitId();
        }
    }

    int GetUniqueUnitId()
    {
        this.uniqueUnitId += 1;
        return this.uniqueUnitId;
    }

    //turns battle and/or cutscene off
    public void ConsumeStoryPointInt(StoryPointInt spi, bool isConsumeBattle, bool isConsumeCutScene)
    {
        foreach (StoryPointObject spo in this.storyPointObjectList)
        {
            if (spi.StoryPointId == spo.PointId)
            {
                if(spo.storyIntList.Contains(spi))
                {
                    if (isConsumeBattle)
                        spi.IsHasStoryBattle = false;
                    if (isConsumeCutScene)
                        spi.IsHasCutScene = false;
                    return;
                }
                return;
            }
        }
    }
    
    //public Dictionary<int,PlayerUnit> GetPlayerUnitDict()
    //{
    //    Dictionary<int,PlayerUnit> unitDict = new Dictionary<int, PlayerUnit>();
    //    for (int i = 0; i < unitList.Count; i++)
    //    {
    //        unitDict.Add(i, unitList[i]);
    //    }

    //    return unitDict;
    //}

    //public void UnitDictToList(Dictionary<int,PlayerUnit> unitDict)
    //{
    //    this.unitList.Clear();
    //    foreach(KeyValuePair<int,PlayerUnit> kvp in unitDict)
    //    {
    //        this.unitList.Add(kvp.Value);
    //    }
    //}


    public void AddItem(int itemId)
    {
        foreach (ItemInventoryObject i in this.itemList)
        {
            if (i.ItemId == itemId)
            {
                i.AddExistingItem();
                return;
            }
        }

        CreateItem(itemId);
    }

    //the true/false return tells if underlieing item list has changed
    public void EquipItem(int unitListIndex, int itemSlot, int itemId, bool isEquip)
    {
        if (this.unitList.ElementAt(unitListIndex) != null)
        {
            if (isEquip)
            {
                this.unitList[unitListIndex].EquipItem(itemId, itemSlot);
            }
            else
            {
                this.unitList[unitListIndex].UnequipItem(itemSlot);
            }

            EquipItemToItemList(itemId, isEquip);
        }
    }

    void EquipItemToItemList(int itemId, bool isEquip)
    {
        foreach (ItemInventoryObject i in this.itemList)
        {
            if (i.ItemId == itemId)
            {
                if (isEquip)
                    i.EquipItem();
                else
                    i.UnequipItem();
                return;
            }
        }
    }
    
    public void DeleteItem(int itemId)
    {
        foreach (ItemInventoryObject i in this.itemList)
        {
            if (i.ItemId == itemId)
            {
                if( i.IsDeletable)
                {
                    if (i.CountOwned > i.CountEquipped)
                    {
                        i.CountOwned -= 1;
                        if (i.CountOwned <= 0)
                        {
                            this.itemList.Remove(i);
                            return;
                        }
                    }     
                }
            }
        }
    }

    bool IsHasItem(int itemId)
    {
        foreach(ItemInventoryObject i in this.itemList)
        {
            if (i.ItemId == itemId)
                return true;
        }
        return false;
    }

    void CreateItem(int itemId)
    {
        ItemInventoryObject i = new ItemInventoryObject(itemId);
        this.itemList.Add(i);
    }

    public void EquipAndBuyItem( int unitListIndex, int itemGold, int itemId, int itemSlot)
    {
        AddItem(itemId);
        EquipItem(unitListIndex, itemSlot, itemId, true);
        ChangeGold(itemGold);
    }

    void ChangeGold(int goldChange)
    {
        this.Gold += goldChange;
        Mathf.Clamp(this.Gold, 0, 9999999);
    }

    
    public List<ItemObject> GetEquippableItemObjectListFromInventory()
    {
        var retValue = new List<ItemObject>();
        foreach( ItemInventoryObject i in this.itemList)
        {
            if (i.CountOwned > 0 && i.IsEquippable && i.CountOwned > i.CountEquipped)
                retValue.Add(ItemManager.Instance.GetItemObjectById(i.ItemId));
        }
        return retValue;
    }

    public void DeleteUnit(int index)
    {
        Debug.LogError("Need to add a function that deletes the ability from the list on a unit delete");
        
        if( index < this.unitXPList.Count && index < this.unitXPList.Count)
        {
            this.unitXPList.RemoveAt(index);
            this.unitList.RemoveAt(index);
        }
        else
            Debug.LogError("can't properly delete unit"); 
    }

    public List<PlayerUnit> GetPlayerUnitList()
    {
        return this.unitList;
    }

    public void AddPlayerUnit(PlayerUnit pu)
    {
        pu.TurnOrder = GetUniqueUnitId();
        this.unitList.Add(pu);
        AddToUnitXPList(pu);
        AddToAbilityList(pu);
    }

    #region unitxp 
    List<UnitXPObject> CreateUnitXPList(List<PlayerUnit> tempList)
    {
        var retValue = new List<UnitXPObject>();
        foreach (PlayerUnit pu in tempList)
            retValue.Add(new UnitXPObject(pu.TurnOrder));
        return retValue;
    }

    public void AddToUnitXPList(PlayerUnit pu)
    {
        this.unitXPList.Add(new UnitXPObject(pu.TurnOrder));
    }

    private int GetUnitLevel( int unitId)
    {
        foreach (PlayerUnit pu in this.unitList)
        {
            if (pu.TurnOrder == unitId)
            {
                return pu.Level;
            }
        }
        return NameAll.NULL_INT;
    }

    public void LevelUpUnit(int unitId, bool isForce = false)
    {
        foreach (PlayerUnit pu in unitList)
        {
            if (pu.TurnOrder == unitId)
            {
                int oldLevel = pu.Level;
                foreach (UnitXPObject uxo in this.unitXPList)
                {
                    if (uxo.UnitId == unitId)
                    {
                        if(isForce)
                        {
                            uxo.ForceLevelUp();
                            pu.SetLevel(oldLevel + 1);
                        }
                        else
                        {
                            int newLevel = uxo.LevelUpUnit(oldLevel);
                            pu.SetLevel(newLevel);
                        }
                        break;
                    }
                }
                break;
            }
        }

        //if( index < this.unitList.Count && index < this.unitXPList.Count)
        //{
        //    int oldLevel = this.unitList[index].Level;
        //    if ( isForce)
        //    {
        //        this.unitXPList[index].ForceLevelUp(); //clears the current XP
        //        this.unitList[index].SetLevel(oldLevel + 1);
        //    }
        //    else
        //    {
                
        //        int newLevel = this.unitXPList[index].LevelUpUnit(oldLevel);
        //        if (oldLevel != newLevel)
        //            this.unitList[index].SetLevel(newLevel);
        //    }
        //}
            
    }

    public void DecrementPartyXP(int xp)
    {
        this.PartyXP -= xp;
        if (this.PartyXP < 0)
            this.PartyXP = 0;
    }

    public void DecrementPartyAP(int ap)
    {
        this.PartyAP -= ap;
        if (this.PartyAP < 0)
            this.PartyAP = 0;
    }

    public int GetPartyXPNeededForLevelUp(int unitId)
    {
        int level = GetUnitLevel(unitId);
        if (  level != NameAll.NULL_INT)
        {
            foreach (UnitXPObject uxo in this.unitXPList)
            {
                if (uxo.UnitId == unitId)
                {
                    return uxo.GetXPNeededForLevelUp(level);
                }
            }
        }

        //if (index < this.unitList.Count && index < this.unitXPList.Count)
        //{
        //    int level = this.unitList[index].Level;
        //    return this.unitXPList[index].GetXPNeededForLevelUp(level);
        //}

        return 999999; //will never have this amount of party XP so error shouldn't be an issue
    }

    public bool IsUnitEligibleForLevelUp(int unitId)
    {
        int level = GetUnitLevel(unitId);
        if (level != NameAll.NULL_INT)
        {
            foreach (UnitXPObject uxo in this.unitXPList)
            {
                if (uxo.UnitId == unitId)
                {
                    return uxo.IsEligibleForLevelUp(level);
                }
            }
        }
        //if (index < this.unitList.Count && index < this.unitXPList.Count)
        //{
        //    int level = this.unitList[index].Level;
        //    return this.unitXPList[index].IsEligibleForLevelUp(level);
        //}

        return false;
    }
   
    public string GetUnitXPString(int unitId, int level)
    {
        string zString = "";
        foreach (UnitXPObject uxo in this.unitXPList)
        {
            if (uxo.UnitId == unitId)
            {
                return  uxo.CurrentXP + "/" + LevelXP.GetNextLevelXP(level);
            }
        }
        //if (index < this.unitXPList.Count)
        //{
        //    zString = this.unitXPList[index].CurrentXP + "/" + LevelXP.GetNextLevelXP(level);
        //}
        return zString;
    }
    #endregion

    #region abilities
    List<UnitAbilityObject> CreateAblityLearnedList(List<PlayerUnit> tempList)
    {
        var retValue = new List<UnitAbilityObject>();
        foreach (PlayerUnit pu in tempList)
            retValue.Add(new UnitAbilityObject(pu.TurnOrder, pu.ClassId));
        return retValue;
    }

    public void AddToAbilityList(PlayerUnit pu)
    {
        this.abilityLearnedList.Add(new UnitAbilityObject(pu.TurnOrder, pu.ClassId));
    }

    public List<AbilityLearnedListObject> GetPrimaryAbilityLearnedList(int index, int classId, int secondaryId)
    {
        if( index < this.abilityLearnedList.Count)
        {
            return this.abilityLearnedList[index].GetAbilityPrimaryList(classId, secondaryId);
        }
        return new List<AbilityLearnedListObject>();

    }

    public int GetAPCost(int index, int classId, int abilitySlot)
    {
        if (index < this.unitList.Count && index < this.abilityLearnedList.Count)
        {
            return this.abilityLearnedList[index].GetAPCost(classId,abilitySlot);
            //int level = this.unitList[index].Level;
            //return this.unitXPList[index].GetXPNeededForLevelUp(level);
        }
        return 999999;
    }

    //returns positive number if partyAP is needed
    public int GetPartyAPNeeded(int index, int classId, int abilitySlot)
    {
        if (index < this.unitList.Count && index < this.abilityLearnedList.Count)
        {
            return (this.abilityLearnedList[index].GetAPCost(classId, abilitySlot) - this.abilityLearnedList[index].CurrentAP );
        }

        return 999999; //will never have this amount of party XP so error shouldn't be an issue
    }

    public bool IsClassLearnable(int index, int level)
    {
        if (index < this.unitList.Count && index < this.abilityLearnedList.Count)
        {
            if (level > this.abilityLearnedList[index].classLearnedOrderList.Count)
                return true;
        }
        return false;
    }

    public void LearnClass(int index, int classId)
    {
        if (index < this.unitList.Count && index < this.abilityLearnedList.Count)
        {
            this.abilityLearnedList[index].LearnNewClass(classId);
        }
    }

    public void LearnAbility(int index, int abilitySlot, int abilityId, int classId)
    {
        if (index < this.unitList.Count && index < this.abilityLearnedList.Count)
        {
            if (abilitySlot == NameAll.ABILITY_SLOT_SECONDARY)
                this.abilityLearnedList[index].LearnNewSpell(classId,abilitySlot,abilityId);
            else
                this.abilityLearnedList[index].LearnNewAbility(classId, abilitySlot, abilityId);
        }
    }

    public void DecrementUnitAP(int index, int apCost)
    {
        if (index < this.unitList.Count && index < this.abilityLearnedList.Count)
        {
            this.abilityLearnedList[index].DecrementAP(apCost);
        }
    }
    #endregion
}

//for managing a player's inventory
[Serializable]
public class ItemInventoryObject
{
    public int ItemId { get; set; } //for loading the itemObject
    //public int UniqueId { get; set; } //assigned when created, unique to each inventory
    public int CountOwned { get; set; } //number of items of this type owned
    public int CountEquipped { get; set; } //number of this type of items equipped
    public bool IsDeletable { get; set; }
    public bool IsEquippable { get; set; }

    public ItemInventoryObject( int zItemId)
    {
        this.ItemId = zItemId;
        this.CountOwned = 1;
        this.CountEquipped = 0;
        this.IsDeletable = true;
        this.IsEquippable = true;
    }

    public void EquipItem()
    {
        this.CountEquipped += 1;
        if (this.CountEquipped > this.CountOwned)
            this.CountEquipped = this.CountOwned;
    }

    public void UnequipItem()
    {
        this.CountEquipped -= 1;
        if (this.CountEquipped < 0)
            this.CountEquipped = 0;
    }

    public void AddExistingItem()
    {
        this.CountOwned += 1;
    }

}

public static class LevelXP
{
    static int Level1XP = 0;
    static int Level2XP = 100;
    static int Level3XP = 300;
    static int Level4XP = 700;
    static int Level5XP = 1500;
    static int Level6XP = 3100;
    static int Level7XP = 6300;
    static int Level8XP = 12700;
    static int Level9XP = 25500;
    static int Level10XP = 51100;
    static int LevelMaxXP = 99999;

    public static int GetNextLevelXP(int currentLevel)
    {
        currentLevel += 1;

        if (currentLevel <= 2)
            return Level2XP;
        else if (currentLevel == 3)
            return Level3XP;
        else if (currentLevel == 4)
            return Level4XP;
        else if (currentLevel == 5)
            return Level5XP;
        else if (currentLevel == 6)
            return Level6XP;
        else if (currentLevel == 7)
            return Level7XP;
        else if (currentLevel == 8)
            return Level8XP;
        else if (currentLevel == 9)
            return Level9XP;
        else if (currentLevel == 10)
            return Level10XP;
        else
            return LevelMaxXP;
    }
}

[Serializable]
public class UnitXPObject
{
    

    //public int TotalXP { get; set; } //not needed. can figure it out by level and current XP
    public int CurrentXP { get; set; }
    public int UnitId { get; set; }

    public UnitXPObject(int zUnitId)
    {
        this.UnitId = zUnitId;
        this.CurrentXP = 0;
        //if (currentLevel <= 1)
        //    this.TotalXP = 0;
        //else
        //    this.TotalXP = GetNextLevelXP(currentLevel - 1);
    }

    

    //returns <= 0 if able to levelUp
    public int GetXPNeededForLevelUp(int currentLevel)
    {
        int xpNeeded = LevelXP.GetNextLevelXP(currentLevel);
        return (xpNeeded - this.CurrentXP);
    }

    public int LevelUpUnit(int currentLevel)
    {
        int nextLevelXP = LevelXP.GetNextLevelXP(currentLevel);
        if (this.CurrentXP >= nextLevelXP)
        {
            this.CurrentXP -= nextLevelXP;
            return currentLevel + 1;
        }
            
        return currentLevel;
    }

    public bool IsEligibleForLevelUp( int currentLevel)
    {
        int nextLevelXP = LevelXP.GetNextLevelXP(currentLevel);
        if (this.CurrentXP >= nextLevelXP)
            return true;

        return false;
    }

    //called when partyXP is used for leveling up
    public void ForceLevelUp()
    {
        this.CurrentXP = 0;
    }

}

[Serializable]
public class UnitAbilityObject
{
    //not using unitId, storing them in same order as unit list
    public List<AbilityLearnedListObject> abilityPrimaryList; //abilities cast in primary/secondary
    public List<AbilityLearnedListObject> abilityOtherList; //reaction, support and movement abilities
    public List<int> classLearnedOrderList; //stores the order in which classes were learned
    public int CurrentAP { get; set; }
    public int UnitId { get; set; }

    public UnitAbilityObject(int zUnitId, int classId)
    {
        this.UnitId = zUnitId;
        this.abilityPrimaryList = new List<AbilityLearnedListObject>();
        this.abilityOtherList = new List<AbilityLearnedListObject>();

        this.CurrentAP = 100;
        this.classLearnedOrderList = new List<int>();
        this.classLearnedOrderList.Add(classId);
    }

    public void DecrementAP(int apCost)
    {
        this.CurrentAP -= apCost;
        if (this.CurrentAP < 0)
            this.CurrentAP = 0;
    }

    public int GetAPCost(int classId, int abilitySlot)
    {
        int classLearnedOrder = NameAll.NULL_INT;
        for( int i = 0; i < this.classLearnedOrderList.Count; i++)
        {
            if( classId == this.classLearnedOrderList[i])
            {
                classLearnedOrder = i + 1;
                break;
            }
        }

        if (classLearnedOrder == NameAll.NULL_INT)
            return NameAll.NULL_INT;
        else
        {
            if (abilitySlot == NameAll.ABILITY_SLOT_PRIMARY || abilitySlot == NameAll.ABILITY_SLOT_SECONDARY)
                return 100 * classLearnedOrder;
            else
                return 300 * classLearnedOrder;
        }
    }

    public void LearnNewClass(int classId)
    {
        this.classLearnedOrderList.Add(classId);
        this.CurrentAP += 100;
    }

    public void LearnNewSpell(int classId, int abilitySlot, int abilityId)
    {
        this.abilityPrimaryList.Add(new AbilityLearnedListObject(classId, abilitySlot, abilityId));
    }

    public void LearnNewAbility(int classId, int abilitySlot, int abilityId)
    {
        this.abilityOtherList.Add(new AbilityLearnedListObject(classId, abilitySlot, abilityId));
    }

    public List<AbilityLearnedListObject> GetAbilityPrimaryList(int classId, int secondaryId)
    {
        return this.abilityPrimaryList.Where(a => a.ClassId == classId || a.ClassId == secondaryId).ToList();
    }

    public List<AbilityLearnedListObject> GetAbilityOtherList()
    {
        return this.abilityOtherList;
    }

    public List<AbilityObject> GetUnknownAbilities(int classId, int abilitySlot)
    {
        List<AbilityObject> retValue = new List<AbilityObject>();
        List<AbilityObject> fullAbilityList;

        bool isCustomClass = false;
        if (classId >= NameAll.CUSTOM_CLASS_ID_START_VALUE)
            isCustomClass = true;

        if ( abilitySlot == NameAll.ABILITY_SLOT_PRIMARY)
        {
            fullAbilityList = AbilityManager.Instance.GetAbilityList(classId, NameAll.ABILITY_SLOT_PRIMARY, isCustomClass);
            foreach( AbilityObject ao in fullAbilityList)
            {
                bool isAlreadyAdded = false;
                foreach( int i in this.classLearnedOrderList)
                {
                    if (ao.ClassId == i)
                    {
                        ao.Description = "LEARNED. Click to learn abilities from this class.";
                        retValue.Add(ao);
                        isAlreadyAdded = true;
                        break;
                    } 
                } 

                if( !isAlreadyAdded)
                    retValue.Add( ao);
            }
        }
        else if( abilitySlot == NameAll.ABILITY_SLOT_SECONDARY)
        {
            //NEED TO HANDLE SPELL NAMES DIFFERENTLY
            ////this is actually active abilities
            fullAbilityList = SpellManager.Instance.GetSpellNamesToAbilityObject(classId, isCustomClass);
            var tempList = GetKnownAbilities(classId, abilitySlot); //THIS STAYS SECONDARY
            retValue = fullAbilityList.Where(a => !tempList.Any(a2 => a2.OverallId != a.OverallId)).ToList();
            return retValue; 
        }
        else
        {
            fullAbilityList = AbilityManager.Instance.GetAbilityList(classId, abilitySlot, isCustomClass);
            var tempList = GetKnownAbilities(classId, abilitySlot);
            retValue = fullAbilityList.Where(a => !tempList.Any(a2 => a2.OverallId != a.OverallId)).ToList();
            return retValue;
        }
        

        

        //var result = peopleList2.Where(p => !peopleList1.Any(p2 => p2.ID == p.ID));
        

        return retValue;

    }


    List<AbilityObject> GetKnownAbilities(int classId, int abilitySlot)
    {
        List<AbilityObject> retValue = new List<AbilityObject>();
        if ( abilitySlot == NameAll.ABILITY_SLOT_PRIMARY )
        {
            //should never reach here
        }
        else if( abilitySlot == NameAll.ABILITY_SLOT_SECONDARY)
        {
            //THESE ARE THE ACTIVE ABILITIES
            foreach( AbilityLearnedListObject al in this.abilityPrimaryList)
            {
                //NEED TO HANDLE SPELL NAMES DIFFERENTLY
                if (al.ClassId == classId)
                {
                    SpellName sn = SpellManager.Instance.GetSpellNameByIndex(al.AbilityId);
                    AbilityObject ao = new AbilityObject(sn);
                    retValue.Add(ao); //IS CHANGED TO PRIMARY ON PURPOSE
                }
                    
            }
        }
        else
        {
            foreach (AbilityLearnedListObject al in this.abilityOtherList)
            {
                if (al.ClassId == classId)
                    retValue.Add(AbilityManager.Instance.GetAbilityObject(abilitySlot, al.AbilityId));
            }
        }

        return retValue;
    }

    public List<AbilityObject> GetEquippableAbilities(int classId, int abilitySlot)
    {
        List<AbilityObject> retValue = new List<AbilityObject>();

        if( abilitySlot == NameAll.ABILITY_SLOT_PRIMARY)
        {
            foreach (int i in classLearnedOrderList)
            {
                retValue.Add(AbilityManager.Instance.GetAbilityObject(abilitySlot, i));
            }
        }
        else if( abilitySlot == NameAll.ABILITY_SLOT_SECONDARY)
        {
            foreach (int i in classLearnedOrderList)
            {
                if (classId == i) //can't equip secondary of current active ability
                    continue;

                retValue.Add(AbilityManager.Instance.GetAbilityObject(abilitySlot, i));
            }
        }
        else
        {
            foreach( AbilityLearnedListObject al in abilityOtherList)
            {
                retValue.Add(AbilityManager.Instance.GetAbilityObject(abilitySlot, al.AbilityId));
            }
        }
        

        return retValue;
    }

    public bool IsClassLearned(int classId)
    {
        foreach( int i in classLearnedOrderList)
        {
            if (i == classId)
                return true;
        }
        return false;
    }

}

[Serializable]
public class AbilityLearnedListObject
{
    public int ClassId;
    public int AbilitySlot;
    public int AbilityId; //overallId, not slot specific ID

    public AbilityLearnedListObject(int zClassId, int zAbilitySlot, int zAbilityId)
    {
        this.ClassId = zClassId;
        this.AbilitySlot = zAbilitySlot;
        this.AbilityId = zAbilityId; 
    }
}
