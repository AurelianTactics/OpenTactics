using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ItemManager : Singleton<ItemManager>
{
    //for battles
    //assumign just create an array of items that already exist

    //for character creator
    //use ItemObjects to create master arrays that populate by slot here
    //create functions that take in character class and support abilities and output lists of objects to CharacterItemPopup
    //with lists, modify the dropdowns (for abilities used dictionaries but due to complexities like two handed weapons, weapon in each hand etc, need to have more flexibility)
    //public static myGlobalVar = "whatever";

    private int modVersion = NameAll.VERSION_CLASSIC;
    int ioType = NameAll.ITEM_MANAGER_SO;
    
    private static List<ItemObject> sItemWeaponList;
    private static List<ItemObject> sItemOffhandList;
    private static List<ItemObject> sItemHeadList;
    private static List<ItemObject> sItemBodyList;
    private static List<ItemObject> sItemAccessoryList;

    void Start()
    {
        modVersion = PlayerPrefs.GetInt(NameAll.PP_MOD_VERSION, NameAll.VERSION_CLASSIC);
    }

    protected ItemManager( )
    { // guarantee this will be always a singleton only - can't use the constructor!
        //myGlobalVar = "asdf";
        
        if( ioType == NameAll.ITEM_MANAGER_SIMPLE) //gets all the items by version, used in character create and non combat
        {
            ItemObject io = new ItemObject();
            sItemWeaponList = GetItemList(NameAll.ITEM_SLOT_WEAPON,modVersion);//io.GetWeaponList(modVersion); //Debug.Log("in itemmanager creator size of list is " + sItemWeaponList.Count);
            sItemOffhandList = GetItemList(NameAll.ITEM_SLOT_OFFHAND, modVersion);//io.GetOffhandList(modVersion);
            sItemHeadList = GetItemList(NameAll.ITEM_SLOT_HEAD, modVersion);//io.GetHeadList(modVersion);
            sItemBodyList = GetItemList(NameAll.ITEM_SLOT_BODY, modVersion);//io.GetBodyList(modVersion);
            sItemAccessoryList = GetItemList(NameAll.ITEM_SLOT_ACCESSORY, modVersion);//io.GetAccessoryList(modVersion);
        }
        else //just empty lists, loads itemobjects on demand (third alternative would be just to make a simple list/dict of only the items in the scene
        {
            sItemWeaponList = new List<ItemObject>();
            sItemOffhandList = new List<ItemObject>();
            sItemHeadList = new List<ItemObject>();
            sItemBodyList = new List<ItemObject>();
            sItemAccessoryList = new List<ItemObject>();
        }
        
    }

    List<ItemObject> GetItemList(int slot, int version)
    {
        var retValue = new List<ItemObject>();
        if( slot == NameAll.ITEM_SLOT_WEAPON )
            retValue.Add(GetItemObjectById(NameAll.FIST_EQUIP));
        else
            retValue.Add(GetItemObjectById(NameAll.NO_EQUIP));

        for ( int i = 0; i < NameAll.CUSTOM_ITEM_ID_START_VALUE; i++)
        {
            //Debug.Log("i is " + i);
            ItemObject io = GetItemObjectById(i);
            if (io == null)
                break;

            if (io.Slot == slot && version == io.Version)
                retValue.Add(io);
        }

        //loads the custom items
        var tempList = CalcCode.LoadCustomItemObjectList();
        foreach( ItemObject io in tempList)
        {
            if (io.Slot == slot && version == io.Version)
                retValue.Add(io);
        }

        return retValue;
    }

    //get item list for storys
    //for now type is just the level
    public List<ItemObject> GetStoryItemList(int version, int type)
    {
        var retValue = new List<ItemObject>();

        sItemWeaponList = GetItemList(NameAll.ITEM_SLOT_WEAPON, version);//Debug.Log("in itemmanager creator size of list is " + sItemWeaponList.Count);
        retValue.AddRange(sItemWeaponList.Where(io => io.Level <= type));

        sItemOffhandList = GetItemList(NameAll.ITEM_SLOT_OFFHAND, version);
        retValue.AddRange(sItemOffhandList.Where(io => io.Level <= type));

        sItemHeadList = GetItemList(NameAll.ITEM_SLOT_HEAD, version);
        retValue.AddRange(sItemHeadList.Where(io => io.Level <= type));

        sItemBodyList = GetItemList(NameAll.ITEM_SLOT_BODY, version);
        retValue.AddRange(sItemBodyList.Where(io => io.Level <= type));

        sItemAccessoryList = GetItemList(NameAll.ITEM_SLOT_ACCESSORY, version);
        retValue.AddRange(sItemAccessoryList.Where(io => io.Level <= type));

        return retValue;
    }

    public void SetIoType(int type)
    {
        ioType = type;
        if (ioType == NameAll.ITEM_MANAGER_SIMPLE) //gets all the items by version, used in character create and non combat
        {
            modVersion = PlayerPrefs.GetInt(NameAll.PP_MOD_VERSION, NameAll.VERSION_CLASSIC);
            ItemObject io = new ItemObject();
            sItemWeaponList = GetItemList(NameAll.ITEM_SLOT_WEAPON, modVersion);//io.GetWeaponList(modVersion); //Debug.Log("in itemmanager creator size of list is " + sItemWeaponList.Count);
            sItemOffhandList = GetItemList(NameAll.ITEM_SLOT_OFFHAND, modVersion);//io.GetOffhandList(modVersion);
            sItemHeadList = GetItemList(NameAll.ITEM_SLOT_HEAD, modVersion);//io.GetHeadList(modVersion);
            sItemBodyList = GetItemList(NameAll.ITEM_SLOT_BODY, modVersion);//io.GetBodyList(modVersion);
            sItemAccessoryList = GetItemList(NameAll.ITEM_SLOT_ACCESSORY, modVersion);//io.GetAccessoryList(modVersion);
        }
        else //just empty lists, loads itemobjects on demand (third alternative would be just to make a simple list/dict of only the items in the scene
        {
            sItemWeaponList = new List<ItemObject>();
            sItemOffhandList = new List<ItemObject>();
            sItemHeadList = new List<ItemObject>();
            sItemBodyList = new List<ItemObject>();
            sItemAccessoryList = new List<ItemObject>();
        }
    }

    //called for player2 as there can be some issues when loading this
    //public void Initialize()
    //{
    //    ItemObject io = new ItemObject();
    //    sItemWeaponList = io.GetWeaponList(); //Debug.Log("in itemmanager creator size of list is " + sItemWeaponList.Count);
    //    sItemOffhandList = io.GetOffhandList();
    //    sItemHeadList = io.GetHeadList();
    //    sItemBodyList = io.GetBodyList();
    //    sItemAccessoryList = io.GetAccessoryList();
    //}

    //public void CallMessage()
    //{
    //    Debug.Log("why doesn't this initialize like others?");
    //}

        //loads the ItemData, gets the field
    int GetItemDataInt(int itemId, int itemStatType) 
    {
        //ItemData test = Resources.Load<ItemData>("Items/item_1");
        //ItemData test = Resources.Load<ItemData>("Items/item_1");
        //////ItemData test = Resources.Load<ItemData>("Data/235");
        //Debug.Log("Asdf " + test.item_name + test.item_id );
        //if (test == null)
        //{
        //    Debug.Log("fucking null somehow");
        //}
        //test = null;
        int z1 = 0;
        
        ItemData id = Resources.Load<ItemData>("Items/item_"+itemId);
        //some sort of thing to get the correct part of id
        if (itemStatType == NameAll.ITEM_OBJECT_ITEM_TYPE)
        {
            z1 = id.item_type;
        }
        else if( itemStatType == NameAll.ITEM_OBJECT_SLOT)
        {
            z1 = id.slot;
        }
        else if( itemStatType == NameAll.ITEM_OBJECT_ELEMENTAL_TYPE)
        {
            z1 = id.elemental_type;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_BRAVE)
        {
            z1 = id.stat_brave; 
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_C_EVADE)
        {
            z1 = id.stat_c_evade;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_CUNNING)
        {
            z1 = id.stat_cunning;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_FAITH)
        {
            z1 = id.stat_faith;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_LIFE)
        {
            z1 = id.stat_life;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_JUMP)
        {
            z1 = id.stat_jump;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_M_EVADE)
        {
            z1 = id.stat_m_evade;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_MA)
        {
            z1 = id.stat_ma;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_MOVE)
        {
            z1 = id.stat_move;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_MP)
        {
            z1 = id.stat_mp;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_P_EVADE)
        {
            z1 = id.stat_p_evade;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_PA)
        {
            z1 = id.stat_pa;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_SPEED)
        {
            z1 = id.stat_speed;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_W_EVADE)
        {
            z1 = id.stat_w_evade;
        }
        else if (itemStatType == NameAll.ITEM_OBJECT_STAT_WP)
        {
            z1 = id.stat_wp;
        }
        else
        {
            Debug.Log("ERROR: unable to find the requested item type");
        }

        id = null;
        Resources.UnloadAsset(id);// Resources.UnloadUnusedAssets(); //not sure which of these to call
        return z1;
    }

    string GetItemDataString(int itemId, int field)
    {
		if (itemId < NameAll.CUSTOM_ITEM_ID_START_VALUE)
		{
			ItemData id = Resources.Load<ItemData>("Items/item_" + itemId);

			string zString;
			if (field == NameAll.ITEM_OBJECT_ITEM_NAME)
			{
				zString = id.item_name;
			}
			else
			{
				zString = id.description;
			}


			id = null;
			Resources.UnloadAsset(id);// Resources.UnloadUnusedAssets(); //not sure which of these to call
			return zString;
		}
		else
		{
			ItemObject io = CalcCode.LoadCustomItemObject(itemId);
			string itemDataString = "";
			if (field == NameAll.ITEM_OBJECT_ITEM_NAME)
				itemDataString = io.ItemName;
			else
				itemDataString = io.Description;
			return itemDataString;
		}
        
    }

    ItemObject GetItemDataObject(int itemId)
    {
        if( itemId < NameAll.CUSTOM_ITEM_ID_START_VALUE)
		{
			ItemData id = Resources.Load<ItemData>("Items/item_" + itemId);
			//Debug.Log("in item data id object is" + id.item_name + "asdf" + itemId);
			ItemObject io = null;
			if (id != null)
			{
				io = new ItemObject(id);
				id = null;
			}
			//Debug.Log("in item data is object is" + io.GetItemName() + "asdf" + io.StatusName);
			Resources.UnloadAsset(id);// Resources.UnloadUnusedAssets(); //not sure which of these to call
			return io;
		}
		else
		{
			//loading custom objects
			return CalcCode.LoadCustomItemObject(itemId);
		}
    }

    //called in getitemsbyslotandunit, need this because offhands can be weapons
    void AddWeaponsByClass(List<ItemObject> ina, int class_id, int support_id, int level = 1919, bool isOffhand = false, string sex = "Male" )
    {
        ina = AddFist(ina); //Debug.Log("weapon list size is " + sItemWeaponList.Count); Debug.Log(" unit id is " + class_id);
        int slot = NameAll.ITEM_SLOT_WEAPON;
        if( class_id >= NameAll.CLASS_FIRE_MAGE)
        {
            if( !isOffhand)
            {
                if (class_id == NameAll.CLASS_RANGER || support_id == NameAll.SUPPORT_EQUIP_BOWS)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_BOW, slot, true, level);
                }
                if (class_id == NameAll.CLASS_WARRIOR || class_id == NameAll.CLASS_CENTURION || support_id == NameAll.SUPPORT_EQUIP_SWORDS)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_GREATSWORD, slot, true, level);
                }
                if (class_id == NameAll.CLASS_HEALER || class_id == NameAll.CLASS_NECROMANCER
                || support_id == NameAll.SUPPORT_EQUIP_STAFFS)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_STICK, slot, true, level);
                }
                if (class_id == NameAll.CLASS_APOTHECARY || class_id == NameAll.CLASS_DEMAGOGUE || support_id == NameAll.SUPPORT_EQUIP_GUNS)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_GUN, slot, true, level);
                }
                if (class_id == NameAll.CLASS_WARRIOR || class_id == NameAll.CLASS_ROGUE || support_id == NameAll.SUPPORT_EQUIP_SWORDS)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_KATANA, slot, true, level);
                }
            }
            //sword	dagger	bow	wand	crossbow	greatsword	stick	gun	
            if (class_id == NameAll.CLASS_ROGUE || class_id == NameAll.CLASS_WARRIOR || class_id == NameAll.CLASS_CENTURION
                || support_id == NameAll.SUPPORT_EQUIP_SWORDS)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_SWORD, slot, true, level);
            }
            if (class_id >= NameAll.CLASS_FIRE_MAGE)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_DAGGER, slot, true, level);
            }
            
            if (class_id == NameAll.CLASS_FIRE_MAGE || class_id == NameAll.CLASS_HEALER || class_id == NameAll.CLASS_NECROMANCER
                || support_id == NameAll.SUPPORT_EQUIP_WAND)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_WAND, slot, true, level);
            }
            if (class_id == NameAll.CLASS_RANGER || support_id == NameAll.SUPPORT_EQUIP_BOWS)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CROSSBOW, slot, true, level);
            }
            
            if (class_id == NameAll.CLASS_APOTHECARY || class_id == NameAll.CLASS_DEMAGOGUE || support_id == NameAll.SUPPORT_EQUIP_GUNS)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_PISTOL, slot, true, level);
            }
            //scales	instrument	deck	spear	whip	mace	katana	pistol
            if (class_id == NameAll.CLASS_ARTIST || class_id == NameAll.CLASS_DRUID || support_id == NameAll.SUPPORT_EQUIP_SCALES)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_SCALES, slot, true, level);
            }
            if (class_id == NameAll.CLASS_ARTIST || support_id == NameAll.SUPPORT_EQUIP_INSTRUMENT_DECK)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_INSTRUMENT, slot, true, level);
            }
            if (class_id == NameAll.CLASS_ARTIST || class_id == NameAll.CLASS_DEMAGOGUE || class_id == NameAll.CLASS_APOTHECARY
                || support_id == NameAll.SUPPORT_EQUIP_INSTRUMENT_DECK)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_DECK, slot, true, level);
            }
            if (class_id == NameAll.CLASS_WARRIOR || class_id == NameAll.CLASS_CENTURION || class_id == NameAll.CLASS_ROGUE
                || support_id == NameAll.SUPPORT_EQUIP_SPEAR)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_SPEAR, slot, true, level);
            }
            if (class_id == NameAll.CLASS_BRAWLER || class_id == NameAll.CLASS_DEMAGOGUE || class_id == NameAll.CLASS_NECROMANCER
                 || class_id == NameAll.CLASS_RANGER || support_id == NameAll.SUPPORT_EQUIP_WHIP_MACE)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_WHIP, slot, true, level);
            }
            if (class_id == NameAll.CLASS_BRAWLER || class_id == NameAll.CLASS_HEALER || class_id == NameAll.CLASS_WARRIOR || class_id == NameAll.CLASS_ROGUE
                 || class_id == NameAll.CLASS_DRUID || support_id == NameAll.SUPPORT_EQUIP_WHIP_MACE)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_MACE, slot, true, level);
            }
            
        }
        else
        {
            //Debug.Log("getting offhand weapons");
            if (!isOffhand)
            {
                //gun
                if (class_id == 1 || class_id == 12 || support_id == 9)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_GUN, slot);
                }
                //magic gun
                if (class_id == 1 || class_id == 12 || support_id == 9)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_MAGIC_GUN, slot);
                }
                //axe
                if (class_id == 4 || class_id == 14 || support_id == 6)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_AXE, slot);
                }

                //longbow
                if (class_id == 3)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_LONGBOW, slot);
                }
                //harp
                if (class_id == 18)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_HARP, slot);
                }

                //crossbow
                if (class_id == 3 || support_id == 8)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_CROSSBOW, slot);
                }

                //dictionary
                if (class_id == 12 || class_id == 17 || class_id == 13)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_DICTIONARY, slot);
                }
                //spear
                if (class_id == 15 || support_id == 12)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_SPEAR, slot);
                }
                //stick
                if (class_id == 13 || class_id == 17)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_STICK, slot);
                }
                //cloth
                if (class_id == 19)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_CLOTH, slot);
                }
                if (sex.Equals("Female"))
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_BAG, slot);
                }
            }

            if (class_id == 1 || class_id == NameAll.CLASS_SQUIRE || class_id == 5
                    || class_id == 6 || class_id == 12 || class_id == 19 )
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_DAGGER, slot); //Debug.Log(" unit id is " + class_id);
            }
            //ninja swords
            if (class_id == 6)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_NINJA, slot);
            }
            //knight swords
            if (class_id == 2)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_KNIGHT, slot);
            }
            //swords
            if (class_id == 2 || class_id == NameAll.CLASS_SQUIRE || class_id == 14 || support_id == 13)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_SWORD, slot);
            }
            //katana
            if (class_id == 16 || support_id == NameAll.SUPPORT_CLASSIC_EQUIP_KNIFE)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_KATANA, slot);
            }
            
            //hammer
            if (class_id == NameAll.CLASS_SQUIRE || class_id == 14 || class_id == 6)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_HAMMER, slot);
            }
            //rod: oracle summoner wizard
            if (class_id == 13 || class_id == 11 || class_id == 9)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_ROD, slot);
            }
            //staff: priest, tm, ora, summ
            if (class_id == 8 || class_id == 11 || class_id == 10 || class_id == 13)
            {
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_STAFF, slot);
            }
            
        }
    }

    public List<ItemObject> GetItemsBySlotAndUnit(int slot, PlayerUnit unit)
    {
        List<ItemObject> ina = new List<ItemObject>();
        //List<ItemObject> tempIna = new List<ItemObject>();
        string sex = unit.Sex;
        int class_id = unit.ClassId;
        int support_id = unit.AbilitySupportCode;
        int level = unit.Level;// GetLevel();
        //myDict.Add(5, "Equip Armor");
        //myDict.Add(6, "Equip Axe");
        //myDict.Add(7, "Equip Change");
        //myDict.Add(8, "Equip Crossbow");
        //myDict.Add(9, "Equip Gun");
        //myDict.Add(10, "Equip Knife");
        //myDict.Add(11, "Equip Shield");
        //myDict.Add(12, "Equip Spear");
        //myDict.Add(13, "Equip Sword");

        if (class_id == NameAll.CLASS_MIME)
        {
            if ( slot == NameAll.ITEM_SLOT_WEAPON )
            {
                ina = AddFist(ina);
            }
            else
            {
                ina = AddEmpty(ina);
            }
            return ina;
        }
        //classes, 20 mime, 1 Chemist, 2 Knight, 3 archer, 4 squire, 5 thief, 6 ninja, 7 monk,
        //8 priest, 9 wizard, 10 time mage, 11 summoner, 12 mediator, 13 oracle,
        //14 geomancer, 15 lancer, 16 samurai, 17 calculator, 18 bard, 19 dancer

        if (slot == NameAll.ITEM_SLOT_WEAPON)
        {
            
            //daggers: thief, squire, chemist, mediator, ninja,dancer
            if( NameAll.IsClassicClass(class_id) )
            {
                AddWeaponsByClass(ina, class_id, support_id, level, false, sex); 
            }
            else
            {
                AddWeaponsByClass(ina, class_id, support_id, level);
            }


        }
        else if(slot == NameAll.ITEM_SLOT_OFFHAND)
        {
            //Debug.Log("in offhand" + unit.AbilitySupportCode + unit.IsAbilityEquipped(NameAll.SUPPORT_TWO_SWORDS, NameAll.SUPPORT));
            ina = AddEmpty(ina);
            if (NameAll.IsClassicClass(class_id))
            {
                //shield
                if (class_id == 2 || class_id == 15 || class_id == 14 || class_id == NameAll.CLASS_ARCHER || support_id == 11 )
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_SHIELD, slot);
                }
                if (AbilityManager.Instance.IsInnateAbility(class_id, NameAll.SUPPORT_TWO_SWORDS, NameAll.ABILITY_SLOT_SUPPORT)
                    || unit.IsAbilityEquipped(NameAll.SUPPORT_TWO_SWORDS, NameAll.ABILITY_SLOT_SUPPORT) )
                {
                    AddWeaponsByClass(ina, class_id, support_id, level, true, sex); 
                } 
            }
            else
            {

                if( class_id == NameAll.CLASS_WARRIOR || class_id == NameAll.CLASS_CENTURION 
                    || support_id == NameAll.SUPPORT_EQUIP_SHIELD)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_SHIELD, slot, true, level);
                }

                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_ORB, slot, true,level);
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CHAIN, slot, true, level);
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_DECK, slot, true, level);

                if (AbilityManager.Instance.IsInnateAbility(class_id, NameAll.SUPPORT_DUAL_WIELD, NameAll.ABILITY_SLOT_SUPPORT)
                    || unit.IsAbilityEquipped(NameAll.SUPPORT_DUAL_WIELD, NameAll.ABILITY_SLOT_SUPPORT) )
                {
                    AddWeaponsByClass(ina, class_id, support_id, level, true);
                }
            }
                
        }
        else if (slot == NameAll.ITEM_SLOT_HEAD)
        {
            ina = AddEmpty(ina);

            if (NameAll.IsClassicClass(class_id))
            {
                if (class_id == 2 || class_id == 15 || class_id == 16)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_HELMET, slot);
                }
                else if (class_id == 7)
                {
                    //monk ets nothing
                }
                else {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_HAT, slot);
                }
                if (sex.Equals("Female"))
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_RIBBON, slot);
                }
            }
            else
            {
                if (class_id == NameAll.CLASS_FIRE_MAGE || class_id == NameAll.CLASS_HEALER || class_id == NameAll.CLASS_NECROMANCER
                    || class_id == NameAll.CLASS_DEMAGOGUE || support_id == NameAll.SUPPORT_EQUIP_MAGE_ROBES)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_MAGE_HAT, slot, true, level);
                }
                if (class_id == NameAll.CLASS_ARTIST || class_id == NameAll.CLASS_APOTHECARY || class_id == NameAll.CLASS_DEMAGOGUE
                    || class_id == NameAll.CLASS_NECROMANCER || class_id == NameAll.CLASS_DRUID || support_id == NameAll.SUPPORT_EQUIP_CLOTHES)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_HAT, slot, true, level);
                }
                if (class_id == NameAll.CLASS_BRAWLER || class_id == NameAll.CLASS_WARRIOR || class_id == NameAll.CLASS_CENTURION
                    || class_id == NameAll.CLASS_ARTIST || support_id == NameAll.SUPPORT_EQUIP_HEAVY_ARMORS)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_HELMET, slot, true, level);
                }
                if (class_id == NameAll.CLASS_ROGUE || class_id == NameAll.CLASS_RANGER || class_id == NameAll.CLASS_DRUID
                    || class_id == NameAll.CLASS_BRAWLER || support_id == NameAll.SUPPORT_EQUIP_LIGHT_ARMORS)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_BANDANA, slot, true, level);
                }
            }
            
        }
        else if (slot == NameAll.ITEM_SLOT_BODY)
        {
            ina = AddEmpty(ina);

            if (NameAll.IsClassicClass(class_id))
            {
                //think armor is knight, lancer, samurai, and Equip Armor ability
                if (class_id == 2 || class_id == 15 || class_id == 16
                        || support_id == 5)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_ARMOR, slot);
                }
                //clothes is everyone else I think
                if (class_id != 2 && class_id != 15 && class_id != 16)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_CLOTHES, slot);
                }
                //robes knight, priest, wizard, time mage, summoner, mediator, oracle, geomancer, lancer, calculator
                if (class_id == 2 || class_id == 15 || class_id == 16
                        || class_id == 8 || class_id == 9 || class_id == 10 || class_id == 11
                        || class_id == 12 || class_id == 13 || class_id == 14 || class_id == 17)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_ROBES, slot);
                }
            }
            else
            {
                if( class_id == NameAll.CLASS_FIRE_MAGE || class_id == NameAll.CLASS_HEALER || class_id == NameAll.CLASS_NECROMANCER
                    || class_id == NameAll.CLASS_DEMAGOGUE || support_id == NameAll.SUPPORT_EQUIP_MAGE_ROBES)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_ROBES, slot, true, level);
                }
                if (class_id == NameAll.CLASS_ARTIST || class_id == NameAll.CLASS_APOTHECARY || class_id == NameAll.CLASS_DEMAGOGUE
                    || class_id == NameAll.CLASS_NECROMANCER || class_id == NameAll.CLASS_DRUID || support_id == NameAll.SUPPORT_EQUIP_CLOTHES)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLOTHES, slot, true, level);
                }
                if (class_id == NameAll.CLASS_BRAWLER || class_id == NameAll.CLASS_WARRIOR || class_id == NameAll.CLASS_CENTURION
                    || class_id == NameAll.CLASS_ARTIST || support_id == NameAll.SUPPORT_EQUIP_HEAVY_ARMORS)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_HEAVY_ARMOR, slot, true, level);
                }
                if (class_id == NameAll.CLASS_ROGUE || class_id == NameAll.CLASS_RANGER || class_id == NameAll.CLASS_DRUID
                    || class_id == NameAll.CLASS_BRAWLER || support_id == NameAll.SUPPORT_EQUIP_LIGHT_ARMORS)
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_LIGHT_ARMOR, slot, true, level);
                }
            }
            
        }
        else if (slot == NameAll.ITEM_SLOT_ACCESSORY)
        {

            
            if (NameAll.IsClassicClass(class_id))
            {
                ina = AddEmpty(ina);
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_SHOES, slot);
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_MANTLE, slot);
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_GAUNTLET, slot);
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_RING, slot);
                ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_ARMLET, slot);
                if (sex.Equals("Female"))
                {
                    ina = AddItemsByType(ina, NameAll.ITEM_ITEM_TYPE_CLASSIC_PERFUME, slot);
                }
            }
            else
            {
                ina = AddItemsBySlot(ina, NameAll.ITEM_SLOT_ACCESSORY, true, level);
            }

            
        }
        
        return ina;
    }

    public List<ItemObject> AddItemsByType(List<ItemObject> item_names, int type, int slot, bool isLevelCheck = false, int level = 1919)
    {
        List<ItemObject> ina = item_names;

        if(isLevelCheck)
        {
            if (slot == NameAll.ITEM_SLOT_WEAPON)
            {
                //Debug.Log("adding to ina 1" + type + slot);
                foreach (ItemObject i in sItemWeaponList)
                {
                    //Debug.Log("adding to ina 2");
                    if (i.ItemType == type && i.Level <= level )
                    {
                        ina.Add(i); //Debug.Log("adding to ina 3");
                    }
                }
            }
            else if (slot == NameAll.ITEM_SLOT_OFFHAND)
            {
                foreach (ItemObject i in sItemOffhandList)
                {
                    if (i.ItemType == type && i.Level <= level)
                    {
                        ina.Add(i);
                    }
                }
            }
            else if (slot == NameAll.ITEM_SLOT_HEAD)
            {
                foreach (ItemObject i in sItemHeadList)
                {
                    if (i.ItemType == type && i.Level <= level)
                    {
                        ina.Add(i);
                    }
                }
            }
            else if (slot == NameAll.ITEM_SLOT_BODY)
            {
                foreach (ItemObject i in sItemBodyList)
                {
                    if (i.ItemType == type && i.Level <= level)
                    {
                        ina.Add(i);
                    }
                }
            }
            else if (slot == NameAll.ITEM_SLOT_ACCESSORY)
            {
                foreach (ItemObject i in sItemAccessoryList)
                {
                    if (i.ItemType == type && i.Level <= level)
                    {
                        ina.Add(i);
                    }
                }
            }
        }
        else
        {
            if (slot == NameAll.ITEM_SLOT_WEAPON)
            {
                //Debug.Log("adding to ina 1" + type + slot);
                foreach (ItemObject i in sItemWeaponList)
                {
                    //Debug.Log("adding to ina 2");
                    if (i.ItemType == type)
                    {
                        ina.Add(i); //Debug.Log("adding to ina 3");
                    }
                }
            }
            else if (slot == NameAll.ITEM_SLOT_OFFHAND)
            {
                foreach (ItemObject i in sItemOffhandList)
                {
                    if (i.ItemType == type)
                    {
                        ina.Add(i);
                    }
                }
            }
            else if (slot == NameAll.ITEM_SLOT_HEAD)
            {
                foreach (ItemObject i in sItemHeadList)
                {
                    if (i.ItemType == type)
                    {
                        ina.Add(i);
                    }
                }
            }
            else if (slot == NameAll.ITEM_SLOT_BODY)
            {
                foreach (ItemObject i in sItemBodyList)
                {
                    if (i.ItemType == type)
                    {
                        ina.Add(i);
                    }
                }
            }
            else if (slot == NameAll.ITEM_SLOT_ACCESSORY)
            {
                foreach (ItemObject i in sItemAccessoryList)
                {
                    if (i.ItemType == type)
                    {
                        ina.Add(i);
                    }
                }
            }
        }
        

        return ina;
    }

    public List<ItemObject> AddItemsBySlot(List<ItemObject> item_names, int slot, bool isLevel = false, int level = 1919)
    {
        List<ItemObject> ina = item_names;

        if (slot == NameAll.ITEM_SLOT_WEAPON)
        {
            //Debug.Log("adding to ina 1" + type + slot);
            ina.AddRange(sItemWeaponList);
        }
        else if (slot == NameAll.ITEM_SLOT_OFFHAND)
        {
            ina.AddRange(sItemOffhandList);
        }
        else if (slot == NameAll.ITEM_SLOT_HEAD)
        {
            ina.AddRange(sItemHeadList);
        }
        else if (slot == NameAll.ITEM_SLOT_BODY)
        {
            ina.AddRange(sItemBodyList);
        }
        else if (slot == NameAll.ITEM_SLOT_ACCESSORY)
        { 
            if (isLevel)
            {
                foreach( ItemObject i in sItemAccessoryList)
                {
                    if( i.Level <= level)
                    {
                        ina.Add(i);
                    }
                }
            }
            else
            {
                ina.AddRange(sItemAccessoryList);
            }
            
        }

        return ina;
    }

    public List<ItemObject> AddFist(List<ItemObject> item_names)
    {
        List<ItemObject> ina = item_names;
        //ina.Add(sItemWeaponList[0]);
        ina.Add(GetItemObjectById(NameAll.FIST_EQUIP));
        return ina;
    }

    public List<ItemObject> AddEmpty(List<ItemObject> item_names)
    {
        List<ItemObject> ina = item_names;
        //ina.Add(sItemOffhandList[0]);
        ina.Add(GetItemObjectById(NameAll.NO_EQUIP));
        return ina;
    }

    public int GetItemType(int itemId, int slot)
    {
        if( ioType == NameAll.ITEM_MANAGER_SIMPLE)
        {
            List<ItemObject> ina = GetINA(slot);

            foreach (ItemObject i in ina)
            {
                if (i.ItemId == itemId)
                {
                    return i.ItemType;
                }
            }

            Debug.Log("ERROR: Unable to return item type");
            return 0;
        }
        else
        {
            return GetItemDataInt(itemId, NameAll.ITEM_OBJECT_ITEM_TYPE);
        }
        
    }

    public string GetItemName(int itemId, int slot)
    {
        return GetItemDataString(itemId, NameAll.ITEM_OBJECT_ITEM_NAME);
        //if (ioType == NameAll.ITEM_MANAGER_SIMPLE)
        //{
        //    List<ItemObject> ina = GetINA(slot);

        //    foreach (ItemObject i in ina)
        //    {
        //        if (i.ItemId == itemId)
        //        {
        //            return i.GetItemName();
        //        }
        //    }

        //    return "ERROR";
        //}
        //else
        //{
        //    //Debug.Log("working?");
        //    return GetItemDataString(itemId, NameAll.ITEM_OBJECT_ITEM_NAME);
        //}
    }

    public int GetItemStatById(int itemId, int slot, int statType)
    {

        if (ioType == NameAll.ITEM_MANAGER_SIMPLE)
        {

            int z1 = 0; //probably need to do some error handling
            List<ItemObject> ina = GetINA(slot);

            foreach (ItemObject i in ina)
            {
                if (i.ItemId == itemId)
                {

                    if (statType == NameAll.ITEM_OBJECT_STAT_BRAVE )
                    {
                        return i.StatBrave;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_C_EVADE)
                    {
                        return i.StatCEvade;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_CUNNING)
                    {
                        return i.StatCunning;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_FAITH)
                    {
                        return i.StatFaith;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_LIFE)
                    {
                        return i.StatLife;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_JUMP)
                    {
                        return i.StatJump;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_M_EVADE)
                    {
                        return i.StatMEvade;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_MA)
                    {
                        return i.StatMA;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_MOVE)
                    {
                        return i.StatMove;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_MP)
                    {
                        return i.StatMP;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_P_EVADE)
                    {
                        return i.StatPEvade;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_PA)
                    {
                        return i.StatPA;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_SPEED)
                    {
                        return i.StatSpeed;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_W_EVADE)
                    {
                        return i.StatWEvade;
                    }
                    else if (statType == NameAll.ITEM_OBJECT_STAT_WP)
                    {
                        return i.StatWP;
                    }
                    return 0;
                }
            }

            //        private int stat_brave;
            //        private int stat_c_evade;
            //        private int stat_faith;
            //        private int stat_hp;
            //        private int stat_jump;
            //        private int stat_m_evade;
            //        private int stat_ma;
            //        private int stat_move;
            //        private int stat_mp;
            //        private int stat_p_evade;
            //        private int stat_pa;
            //        private int stat_speed;
            //        private int stat_w_evade;
            //        private int stat_wp;
            return z1;
        }
        else
        {
            return GetItemDataInt(itemId, statType);
        }
    }

    List<ItemObject> GetINA(int slot)
    {
        List<ItemObject> ina = new List<ItemObject>();

        if (slot == NameAll.ITEM_SLOT_WEAPON)
        {
            ina = sItemWeaponList;
        }
        else if (slot == NameAll.ITEM_SLOT_OFFHAND)
        {
            ina = sItemOffhandList;
        }
        else if (slot == NameAll.ITEM_SLOT_HEAD)
        {
            ina = sItemHeadList;
        }
        else if (slot == NameAll.ITEM_SLOT_BODY)
        {
            ina = sItemBodyList;
        }
        else 
        {
            ina = sItemAccessoryList;
        }

        return ina;
    }

    public ItemObject GetItemObjectById(int itemId, int slot = 1 )
    {
        return GetItemDataObject(itemId);
        //if (ioType == NameAll.ITEM_MANAGER_SIMPLE)
        //{
        //    List<ItemObject> ina = GetINA(slot);

        //    foreach (ItemObject i in ina)
        //    {
        //        if (i.ItemId == itemId)
        //        {
        //            return i;
        //        }
        //    }
        //    Debug.Log("ERROR: couldn't find item to remove on item unequip (slot, itemId) " + slot + " " + itemId + "loading the item object from asset");
        //    return GetItemDataObject(itemId);
        //    //return null;
        //}
        //else
        //{
        //    //Debug.Log("returning itemobject from item data");
        //    return GetItemDataObject(itemId);
        //}

        
    }

    public int GetWeaponElementById(int itemId)
    {
        if (ioType == NameAll.ITEM_MANAGER_SIMPLE)
        {
            int z1 = 0;
            foreach (ItemObject i in sItemWeaponList)
            {
                if (i.ItemId == itemId)
                {
                    return i.ElementalType;
                }
            }
            return z1;
        }
        else
        {
            return GetItemDataInt(itemId, NameAll.ITEM_OBJECT_ELEMENTAL_TYPE);
        }

        
    }

    public bool IsUsingBothHands(int weaponId, int supportId, int classId)
    {
        if( classId >= NameAll.CLASS_FIRE_MAGE)
        {
            if( (supportId == NameAll.SUPPORT_MIGHTY_GRIP || AbilityManager.Instance.IsInnateAbility(classId,NameAll.SUPPORT_MIGHTY_GRIP,NameAll.ABILITY_SLOT_SUPPORT))
                && IsTwoHands(weaponId))
            {
                return true;
            }
            else if( IsBothHandsWeapon(weaponId) )
            {
                return true;
            }
        }
        else
        {
            if (supportId == NameAll.SUPPORT_TWO_HANDS && IsTwoHands(weaponId))
            {
                return true;
            }
            else if (IsBothHandsWeapon(weaponId))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsBothHandsWeapon(int weaponId)
    {
        int type = GetItemType(weaponId, NameAll.ITEM_SLOT_WEAPON);
        if (modVersion == NameAll.VERSION_AURELIAN)
        {
            if ( type == NameAll.ITEM_ITEM_TYPE_GREATSWORD || type == NameAll.ITEM_ITEM_TYPE_BOW
                || type == NameAll.ITEM_ITEM_TYPE_KATANA || type == NameAll.ITEM_ITEM_TYPE_STICK || type == NameAll.ITEM_ITEM_TYPE_GUN)
            {
                return true;
            }
        }
        else
        {

            if (type == NameAll.ITEM_ITEM_TYPE_CLASSIC_LONGBOW || type == NameAll.ITEM_ITEM_TYPE_CLASSIC_GUN
                || type == NameAll.ITEM_ITEM_TYPE_CLASSIC_MAGIC_GUN || type == NameAll.ITEM_ITEM_TYPE_CLASSIC_AXE)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsTwoHands(int weaponId)
    {
        int type = GetItemType(weaponId, NameAll.ITEM_SLOT_WEAPON);
        
        if( modVersion == NameAll.VERSION_AURELIAN)
        {
            //able to double wp with two hands
            //sword, scales, spear, mace
            if ( type == NameAll.ITEM_ITEM_TYPE_SWORD || type == NameAll.ITEM_ITEM_TYPE_SCALES
                 || type == NameAll.ITEM_ITEM_TYPE_SPEAR || type == NameAll.ITEM_ITEM_TYPE_MACE )
            {
                return true;
            }
        }
        else
        {
            //able to double wp with two hands
            //ninja, swords, knight, katana, rod, stave, hammer, spear, stick 
            if ( type == NameAll.ITEM_ITEM_TYPE_CLASSIC_SWORD || type == NameAll.ITEM_ITEM_TYPE_CLASSIC_NINJA
                || type == NameAll.ITEM_ITEM_TYPE_CLASSIC_KNIGHT || type == NameAll.ITEM_ITEM_TYPE_CLASSIC_KATANA
                || type == NameAll.ITEM_ITEM_TYPE_CLASSIC_ROD || type == NameAll.ITEM_ITEM_TYPE_CLASSIC_STAFF
                || type == NameAll.ITEM_ITEM_TYPE_CLASSIC_HAMMER || type == NameAll.ITEM_ITEM_TYPE_CLASSIC_SPEAR
                || type == NameAll.ITEM_ITEM_TYPE_CLASSIC_STICK )
            {
                return true;
            }
        }       
        return false;
    }

    public bool IsOffhandWeaponEquipped(int offhandId)
    {
        if (ioType == NameAll.ITEM_MANAGER_SIMPLE)
        {
            int type = GetItemType(offhandId, NameAll.ITEM_SLOT_WEAPON ); //searches for offhandId in the array that only has weapons
            if (type == NameAll.ITEM_SLOT_WEAPON)
            {
                return true;
            }
            return true;
        }
        else
        {
            int type = GetItemDataInt(offhandId, NameAll.ITEM_OBJECT_SLOT);
            if( type == NameAll.ITEM_SLOT_WEAPON )
            {
                return true;
            }
            return false;
        }

        
    }

    string AddComma( string zString)
    {
        //Debug.Log("asdf" + zString.Length);
        if( zString.Length > 0)
        {
            zString += ", ";
        }
        //Debug.Log("asdf" + zString.Length);
        return zString;
    }
    //can be called in simple or not simple mode
    public string GetItemStatusNames(PlayerUnit pu)
    {
        string zString = "";
        ItemObject io = GetItemObjectById(pu.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON);
        if (io.StatusName != 0)
        {
            zString += NameAll.GetStatusString(io.StatusName);
        }

        io = GetItemObjectById(pu.ItemSlotOffhand, NameAll.ITEM_SLOT_OFFHAND);
        if (io.StatusName != 0)
        {
            zString = AddComma(zString);
            zString += NameAll.GetStatusString(io.StatusName);
        }

        io = GetItemObjectById(pu.ItemSlotHead, NameAll.ITEM_SLOT_HEAD);
        if (io.StatusName != 0)
        {
            zString = AddComma(zString);
            zString += NameAll.GetStatusString(io.StatusName);
        }

        io = GetItemObjectById(pu.ItemSlotBody, NameAll.ITEM_SLOT_BODY);
        if (io.StatusName != 0)
        {
            zString = AddComma(zString);
            zString += NameAll.GetStatusString(io.StatusName);
        }

        io = GetItemObjectById(pu.ItemSlotAccessory, NameAll.ITEM_SLOT_ACCESSORY);
        if (io.StatusName != 0)
        {
            zString = AddComma(zString);
            zString += NameAll.GetStatusString(io.StatusName);
        }
        return zString;

        //weaponText.text = "" + ItemManager.Instance.GetItemName(pu.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON);
        //offhandText.text = "" + ItemManager.Instance.GetItemName(pu.ItemSlotOffhand, NameAll.ITEM_SLOT_OFFHAND);
        //headText.text = "" + ItemManager.Instance.GetItemName(pu.ItemSlotHead, NameAll.ITEM_SLOT_HEAD);
        //bodyText.text = "" + ItemManager.Instance.GetItemName(pu.ItemSlotBody, NameAll.ITEM_SLOT_BODY);
        //accessoryText.text = "" + ItemManager.Instance.GetItemName(pu.ItemSlotAccessory, NameAll.ITEM_SLOT_ACCESSORY);
    }

    //returns a string of the main stat for an item button in an item scroll list
    public string GetMainStat(ItemObject i)
    {
        string zString = "";
        if (i.Slot == NameAll.ITEM_SLOT_WEAPON)
        {
            zString = "WP:" + i.StatWP + " WEv:" + i.StatWEvade;
        }
        else if (i.Slot == NameAll.ITEM_SLOT_OFFHAND)
        {
            zString = ItemConstants.GetItemTypeString(i.ItemType);
        }
        else if (i.Slot == NameAll.ITEM_SLOT_HEAD)
        {
            zString = "HP:" + i.StatLife + " MP:" + i.StatMP;
        }
        else if (i.Slot == NameAll.ITEM_SLOT_BODY)
        {
            zString = "HP:" + i.StatLife + " MP:" + i.StatMP;
        }
        else if (i.Slot == NameAll.ITEM_SLOT_ACCESSORY)
        {
            zString = ItemConstants.GetItemTypeString(i.ItemType);
        }

        return zString;
    }

    //returns a random item id, called in player unit
    public int GetRandomItem(PlayerUnit pu, int itemSlot)
    {
        int version;
        if( pu.ClassId < NameAll.CLASS_FIRE_MAGE)
        {
            version = NameAll.VERSION_CLASSIC;
        }
        else
        {
            version = NameAll.VERSION_AURELIAN;
        }

        int z1 = NameAll.NO_EQUIP;
        int supportId = pu.AbilitySupportCode;
        int level = pu.Level;
        List<ItemObject> tempItems = new List<ItemObject>();
     
        //170	2	4	151	Mighty Grip	107	Hold weapon in both hands to increase damage

        if ( itemSlot == NameAll.ITEM_SLOT_WEAPON)
        {
            z1 = NameAll.FIST_EQUIP;
            if (version == NameAll.VERSION_AURELIAN)
            {
                //        206 2   4   187 Equip Wand
                //208 2   4   189 Equip Staffs
                //209 2   4   190 Equip Instrument/ Deck
                //210 2   4   191 Equip Guns
                //212 2   4   193 Equip Whip/ Mace
                //213 2   4   194 Equip Swords
                //217 2   4   198 Equip Bows
                //218 2   4   199 Equip Scales
                //219 2   4   200 Equip Spear
                if (supportId == NameAll.SUPPORT_EQUIP_SPEAR)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_SPEAR, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_SCALES)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_SCALES, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_BOWS)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_BOW, itemSlot);
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_CROSSBOW, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_SWORDS)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_SWORD, itemSlot);
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_GREATSWORD, itemSlot);
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_KATANA, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_WHIP_MACE)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_WHIP, itemSlot);
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_MACE, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_GUNS)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_GUN, itemSlot);
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_PISTOL, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_INSTRUMENT_DECK)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_INSTRUMENT, itemSlot);
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_DECK, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_STAFFS)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_STICK, itemSlot);
                    //tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_STAFF, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_WAND)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_WAND, itemSlot);
                }
                else
                {
                    tempItems = GetItemsBySlotAndUnit(itemSlot, pu);
                }
                z1 = GetRandomItemId(tempItems, NameAll.FIST_EQUIP, level, version);
            }
            else
            {
                if (supportId == NameAll.SUPPORT_MARTIAL_ARTS)
                {
                    return NameAll.FIST_EQUIP;
                }
                else if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_AXE)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_CLASSIC_AXE, NameAll.ITEM_SLOT_WEAPON);
                }
                else if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_CROSSBOW)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_CLASSIC_CROSSBOW, NameAll.ITEM_SLOT_WEAPON);
                }
                else if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_GUN)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_CLASSIC_GUN, NameAll.ITEM_SLOT_WEAPON);
                }
                else if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_KNIFE)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_CLASSIC_KATANA, NameAll.ITEM_SLOT_WEAPON);
                }
                else if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_SPEAR)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_CLASSIC_SPEAR, NameAll.ITEM_SLOT_WEAPON);
                }
                else if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_SWORD)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_CLASSIC_SWORD, NameAll.ITEM_SLOT_WEAPON);
                }
                else
                {
                    tempItems = GetItemsBySlotAndUnit(itemSlot, pu);
                }
                //maybe two hands maybe not
                z1 = GetRandomItemId(tempItems, NameAll.FIST_EQUIP,level,version);
            }
        }
        else if( itemSlot == NameAll.ITEM_SLOT_OFFHAND)
        {
            if (version == NameAll.VERSION_AURELIAN)
            {
                if (supportId == NameAll.SUPPORT_EQUIP_SHIELD)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_SHIELD, NameAll.ITEM_SLOT_OFFHAND);
                }
                else if (supportId == NameAll.SUPPORT_DUAL_WIELD)
                {
                    tempItems = GetItemsBySlotAndUnit(NameAll.ITEM_SLOT_WEAPON, pu);
                }
                else if ( AbilityManager.Instance.IsInnateAbility(pu.ClassId,NameAll.SUPPORT_DUAL_WIELD,itemSlot) )
                {
                    tempItems = GetItemsBySlotAndUnit(NameAll.ITEM_SLOT_WEAPON, pu);
                }
                else
                {
                    tempItems = GetItemsBySlotAndUnit(itemSlot, pu);
                }
            }
            else
            {
                if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_SHIELD)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_CLASSIC_SHIELD, NameAll.ITEM_SLOT_OFFHAND);
                }
                else if (supportId == NameAll.SUPPORT_TWO_SWORDS)
                {
                    tempItems = GetItemsBySlotAndUnit(NameAll.ITEM_SLOT_OFFHAND, pu); //gets some shields in there for some classes I guess
                }
                else if (AbilityManager.Instance.IsInnateAbility(pu.ClassId, NameAll.SUPPORT_TWO_SWORDS, NameAll.ABILITY_SLOT_SUPPORT))
                {
                    tempItems = GetItemsBySlotAndUnit(NameAll.ITEM_SLOT_OFFHAND, pu);
                    //Debug.Log("is ninja, innate, tempitems list " + tempItems.Count);
                }
                else
                {
                    tempItems = GetItemsBySlotAndUnit(itemSlot, pu);
                }
            }

            z1 = GetRandomItemId(tempItems, NameAll.NO_EQUIP, level, version);
        }
        else if( itemSlot == NameAll.ITEM_SLOT_HEAD)
        {
            if (version == NameAll.VERSION_AURELIAN)
            {
                //216 2   4   197 Equip Light Armors
                //214 2   4   195 Equip Heavy Armors
                //207 2   4   188 Equip Mage Robes
                //211 2   4   192 Equip Clothes
                if (supportId == NameAll.SUPPORT_EQUIP_LIGHT_ARMORS)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_BANDANA, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_HEAVY_ARMORS)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_HELMET, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_MAGE_ROBES)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_MAGE_HAT, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_CLOTHES)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_HAT, itemSlot);
                }
                else
                {
                    tempItems = GetItemsBySlotAndUnit(itemSlot, pu);
                }
            }
            else
            {
                tempItems = GetItemsBySlotAndUnit(itemSlot, pu);
            }
            z1 = GetRandomItemId(tempItems, NameAll.NO_EQUIP, level, version);
        }
        else if (itemSlot == NameAll.ITEM_SLOT_BODY)
        {
            if (version == NameAll.VERSION_AURELIAN)
            {
                //216 2   4   197 Equip Light Armors
                //214 2   4   195 Equip Heavy Armors
                //207 2   4   188 Equip Mage Robes
                //211 2   4   192 Equip Clothes
                if( supportId == NameAll.SUPPORT_EQUIP_LIGHT_ARMORS)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_LIGHT_ARMOR, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_HEAVY_ARMORS)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_HEAVY_ARMOR, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_MAGE_ROBES)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_ROBES, itemSlot);
                }
                else if (supportId == NameAll.SUPPORT_EQUIP_CLOTHES)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_CLOTHES, itemSlot);
                }
                else
                {
                    tempItems = GetItemsBySlotAndUnit(itemSlot, pu);
                }
            }
            else
            {
                if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_ARMOR)
                {
                    tempItems = AddItemsByType(tempItems, NameAll.ITEM_ITEM_TYPE_CLASSIC_ARMOR, itemSlot);
                }
                else
                {
                    tempItems = GetItemsBySlotAndUnit(itemSlot, pu);
                }
            }
            z1 = GetRandomItemId(tempItems, NameAll.NO_EQUIP, level, version);
        }
        else if (itemSlot == NameAll.ITEM_SLOT_ACCESSORY)
        {
            tempItems = GetItemsBySlotAndUnit(itemSlot, pu);
            z1 = GetRandomItemId(tempItems, NameAll.NO_EQUIP, level, version);
        }

        return z1;
    }

    private int GetRandomItemId(List<ItemObject> tempItems, int defaultEquip, int level, int version)
    {
        //Debug.Log("random item count is " + tempItems.Count);
        if( tempItems.Count == 0)
        {
            return defaultEquip;
        }
        else
        {
            //foreach(ItemObject i in tempItems)
            //{
            //    Debug.Log(i.GetItemName());
            //}
            int minLevel;
            if( version == NameAll.VERSION_AURELIAN)
            {
                minLevel = level - 2;
            }
            else
            {
                if( level > 35)
                {
                    minLevel = 30;
                }
                else
                {
                    minLevel = level - 10;
                }
            }
            IEnumerable<int> filteringQuery = from io in tempItems where io.Level <= level && io.Level >= minLevel && io.ItemId != defaultEquip select io.ItemId;
            //= from ao in sAbilityList where ao.GetSlot() == abilitySlot && ao.GetSlotId() != classId select ao.GetSlotId();
            //Debug.Log("random item count filtering is " + filteringQuery.Count());
            if ( filteringQuery.Count() == 0)
            {
                return defaultEquip;
            }
            int z1 = UnityEngine.Random.Range(0, filteringQuery.Count()); //Debug.Log("item id is " + z1);
            return filteringQuery.ElementAt(z1);
        }
    }

    //returns a string of details for an item button in an item scroll list
    public string GetDetails(ItemObject i)
    {
        string zString = "";
        //blocks status_name stat_brave stat_c_evade    stat_cunning stat_faith  stat_life stat_jump   
        //stat_m_evade stat_ma stat_move stat_mp stat_p_evade stat_pa stat_speed stat_w_evade    
        //stat_wp elemental_type  on_hit_effect on_hit_chance   stat_agi

        if (i.StatSpeed != 0)
        {
            zString += i.StatSpeed + "sp ";
        }
        if ( i.StatPA != 0)
        {
            zString += i.StatPA + "str ";
        }
        if (i.StatMA != 0)
        {
            zString += i.StatMA + "int ";
        }
        if (i.StatAgi != 0)
        {
            zString += i.StatAgi + "agi ";
        }
        if (i.StatPEvade != 0)
        {
            zString += i.StatPEvade + "p.ev ";
        }
        if (i.StatMEvade != 0)
        {
            zString += i.StatMEvade + "m.ev ";
        }
        if (i.StatMove != 0)
        {
            zString += i.StatMove + "move ";
        }
        if (i.StatJump != 0)
        {
            zString += i.StatJump + "jump ";
        }
        if (i.StatBrave != 0)
        {
            zString += i.StatBrave + "crg ";
        }
        if (i.StatFaith != 0)
        {
            zString += i.StatFaith + "wis ";
        }
        if (i.StatCunning != 0)
        {
            zString += i.StatCunning + "skl ";
        }
        if (i.Blocks != 0)
        {
            zString += " " + NameAll.GetStatusString(i.Blocks);
        }
        if (i.StatusName != 0)
        {
            zString += " " + NameAll.GetStatusString(i.StatusName);
        }
        if (i.ElementalType != 0)
        {
            zString += " " + NameAll.GetElementalString(i.ElementalType);
        }
        if (i.OnHitEffect != 0) //on_hit_effect on_hit_chance
        {
            zString += " " + NameAll.GetStatusString(i.OnHitEffect) + "(" + i.OnHitChance  + "%)";
        }


        return zString;
    }

    public List<ItemObject> GetEquippableItemsBySlotUnit(List<ItemObject> listIn, PlayerUnit pu, int slot)
    {
        List<ItemObject> retValue = new List<ItemObject>();
        var tempList = listIn.Where(io => io.Slot == slot).ToList();
        var equippableTypeList = GetEquippableTypesByUnitSlot(pu, slot);

        foreach(int i in equippableTypeList)
        {
            retValue.AddRange(tempList.Where(io => io.ItemType == i));
        }

        return retValue;
    }

    public List<int> GetEquippableTypesByUnitSlot(PlayerUnit pu, int slot)
    {
        var retValue = new List<int>();
        bool isClassicClass = NameAll.IsClassicClass(pu.ClassId);
        //int supportId = pu.AbilitySupportCode;

        if( slot == NameAll.ITEM_SLOT_WEAPON)
        {
            if(isClassicClass)
            {
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_AXE = 109;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_LONGBOW = 110;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_BAG = 111;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_CLOTH = 112;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_CROSSBOW = 113;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_DAGGER = 114;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_DICTIONARY = 115;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_GUN = 116;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_HAMMER = 117;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_HARP = 118;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_KATANA = 119;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_KNIGHT = 120;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_MAGIC_GUN = 121;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_NINJA = 122;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_ROD = 123;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_SPEAR = 125;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_STAFF = 126;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_STICK = 127;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_SWORD = 128;
                if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_AXE))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_AXE);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_LONGBOW))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_LONGBOW);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_BAG))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_BAG);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_CLOTH))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_CLOTH);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_CROSSBOW))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_CROSSBOW);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_DAGGER))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_DAGGER);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_DICTIONARY))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_DICTIONARY);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_GUN))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_GUN);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_HAMMER))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_HAMMER);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_HARP))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_HARP);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_KATANA))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_KATANA);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_KNIGHT))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_KNIGHT);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_MAGIC_GUN))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_MAGIC_GUN);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_NINJA))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_NINJA);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_ROD))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_ROD);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_SPEAR))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_SPEAR);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_STAFF))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_STAFF);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_STICK))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_STICK);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_SWORD))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_SWORD);
            }
            else
            {
                //public static readonly int ITEM_ITEM_TYPE_SWORD = 19;
                //public static readonly int ITEM_ITEM_TYPE_DAGGER = 20;
                //public static readonly int ITEM_ITEM_TYPE_BOW = 21;
                //public static readonly int ITEM_ITEM_TYPE_WAND = 22;
                //public static readonly int ITEM_ITEM_TYPE_CROSSBOW = 23;
                //public static readonly int ITEM_ITEM_TYPE_GREATSWORD = 24;
                //public static readonly int ITEM_ITEM_TYPE_STICK = 25;
                //public static readonly int ITEM_ITEM_TYPE_GUN = 26;
                //public static readonly int ITEM_ITEM_TYPE_SCALES = 27;
                //public static readonly int ITEM_ITEM_TYPE_INSTRUMENT = 28;
                //public static readonly int ITEM_ITEM_TYPE_DECK = 29;
                //public static readonly int ITEM_ITEM_TYPE_SPEAR = 30;
                //public static readonly int ITEM_ITEM_TYPE_WHIP = 31;
                //public static readonly int ITEM_ITEM_TYPE_MACE = 32;
                //public static readonly int ITEM_ITEM_TYPE_KATANA = 33;
                //public static readonly int ITEM_ITEM_TYPE_PISTOL = 34;
                if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_SWORD))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_SWORD);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_DAGGER))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_DAGGER);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_BOW))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_BOW);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_WAND))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_WAND);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CROSSBOW))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CROSSBOW);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_GREATSWORD))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_GREATSWORD);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_STICK))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_STICK);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_GUN))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_GUN);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_SCALES))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_SCALES);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_INSTRUMENT))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_INSTRUMENT);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_DECK))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_DECK);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_SPEAR))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_SPEAR);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_WHIP))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_WHIP);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_MACE))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_MACE);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_KATANA))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_KATANA);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_PISTOL))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_PISTOL);
            }
        }
        else if( slot == NameAll.ITEM_SLOT_OFFHAND)
        {
            if(isClassicClass)
            {
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_SWORD = 128;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_NINJA = 122;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_KNIGHT = 120;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_HAMMER = 117;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_DAGGER = 114;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_SHIELD = 124;
                if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_SHIELD))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_SHIELD);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_DAGGER))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_DAGGER);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_HAMMER))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_HAMMER);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_KNIGHT))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_KNIGHT);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_NINJA))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_NINJA);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_SWORD))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_SWORD);
            }
            else
            {
                //public static readonly int ITEM_ITEM_TYPE_SHIELD = 9;
                //public static readonly int ITEM_ITEM_TYPE_ORB = 10;
                //public static readonly int ITEM_ITEM_TYPE_BOOK = 11;
                //public static readonly int ITEM_ITEM_TYPE_CHAIN = 12;
                //NameAll.ITEM_ITEM_TYPE_SWORD
                //NameAll.ITEM_ITEM_TYPE_DAGGER
                //NameAll.ITEM_ITEM_TYPE_PISTOL
                //NameAll.ITEM_ITEM_TYPE_MACE
                if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_SHIELD))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_SHIELD);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_ORB))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_ORB);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_BOOK))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_BOOK);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CHAIN))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CHAIN);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_SWORD))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_SWORD);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_DAGGER))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_DAGGER);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_PISTOL))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_PISTOL);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_MACE))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_MACE);


            }
        }
        else if( slot == NameAll.ITEM_SLOT_HEAD)
        {
            if(isClassicClass)
            {
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_HAT = 129;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_HELMET = 130;
                ///ribbon
                if (IsEquippable(pu,slot,NameAll.ITEM_ITEM_TYPE_CLASSIC_HAT))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_HAT);
                else if(IsEquippable(pu,slot,NameAll.ITEM_ITEM_TYPE_CLASSIC_HELMET))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_HELMET);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_RIBBON))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_RIBBON);
            }
            else
            {
                //public static readonly int ITEM_ITEM_TYPE_HAT = 1;
                //public static readonly int ITEM_ITEM_TYPE_HELMET = 2;
                //public static readonly int ITEM_ITEM_TYPE_MAGE_HAT = 3;
                //public static readonly int ITEM_ITEM_TYPE_BANDANA = 4;
                if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_HAT))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_HAT);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_HELMET))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_HELMET);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_MAGE_HAT))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_MAGE_HAT);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_BANDANA))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_BANDANA);
            }
        }
        else if( slot == NameAll.ITEM_SLOT_BODY)
        {
            if (isClassicClass)
            {
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_ARMOR = 106;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_CLOTHES = 107;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_ROBES = 108;
                if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_ARMOR))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_ARMOR);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_CLOTHES))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_CLOTHES);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLASSIC_ROBES))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_ROBES);
            }
            else
            {
                //public static readonly int ITEM_ITEM_TYPE_HEAVY_ARMOR = 5;
                //public static readonly int ITEM_ITEM_TYPE_CLOTHES = 6;
                //public static readonly int ITEM_ITEM_TYPE_ROBES = 7;
                //public static readonly int ITEM_ITEM_TYPE_LIGHT_ARMOR = 8;
                if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_HEAVY_ARMOR))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_HEAVY_ARMOR);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_CLOTHES))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLOTHES);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_ROBES))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_ROBES);
                else if (IsEquippable(pu, slot, NameAll.ITEM_ITEM_TYPE_LIGHT_ARMOR))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_LIGHT_ARMOR);
            }
        }
        else if( slot == NameAll.ITEM_SLOT_ACCESSORY)
        {
            if(isClassicClass)
            {
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_PERFUME = 103;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_ARMLET = 100;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_GAUNTLET = 101;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_MANTLE = 102;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_RING = 104;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_SHOES = 105;

                if ( IsEquippable(pu,slot,NameAll.ITEM_ITEM_TYPE_CLASSIC_PERFUME))
                    retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_PERFUME);

                retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_ARMLET);
                retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_GAUNTLET);
                retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_MANTLE);
                retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_RING);
                retValue.Add(NameAll.ITEM_ITEM_TYPE_CLASSIC_SHOES);
            }
            else
            {
                //public static readonly int ITEM_ITEM_TYPE_BOOTS = 14;
                //public static readonly int ITEM_ITEM_TYPE_BRACELET = 15;
                //public static readonly int ITEM_ITEM_TYPE_GLOVES = 16;
                //public static readonly int ITEM_ITEM_TYPE_CLOAK = 17;
                //public static readonly int ITEM_ITEM_TYPE_NECKLACE = 18;
                //public static readonly int ITEM_ITEM_TYPE_RING = 13;
                //everyone gets all accessories, not doing checks for now
                retValue.Add(NameAll.ITEM_ITEM_TYPE_BOOTS);
                retValue.Add(NameAll.ITEM_ITEM_TYPE_BRACELET);
                retValue.Add(NameAll.ITEM_ITEM_TYPE_GLOVES);
                retValue.Add(NameAll.ITEM_ITEM_TYPE_CLOAK);
                retValue.Add(NameAll.ITEM_ITEM_TYPE_NECKLACE);
                retValue.Add(NameAll.ITEM_ITEM_TYPE_RING);
            }
        }
        return retValue;
    }

    bool IsEquippable(PlayerUnit pu, int slot, int itemType)
    {
        bool isClassicClass = NameAll.IsClassicClass(pu.ClassId);
        int supportId = pu.AbilitySupportCode;
        int classId = pu.ClassId;

        if(slot == NameAll.ITEM_SLOT_WEAPON)
        {
            if(isClassicClass)
            {
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_AXE = 109;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_LONGBOW = 110;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_BAG = 111;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_CLOTH = 112;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_CROSSBOW = 113;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_DAGGER = 114;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_DICTIONARY = 115;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_GUN = 116;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_HAMMER = 117;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_HARP = 118;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_KATANA = 119;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_KNIGHT = 120;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_MAGIC_GUN = 121;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_NINJA = 122;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_ROD = 123;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_SPEAR = 125;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_STAFF = 126;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_STICK = 127;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_SWORD = 128;
                if( itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_AXE)
                {
                    if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_AXE || classId == NameAll.CLASS_GEOMANCER || classId == NameAll.CLASS_SQUIRE)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_LONGBOW)
                {
                    if (classId == NameAll.CLASS_ARCHER)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_BAG)
                {
                    if (pu.Sex == NameAll.SEX_FEMALE)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_CLOTH)
                {
                    if (classId == NameAll.CLASS_DANCER)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_CROSSBOW)
                {
                    if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_CROSSBOW || classId == NameAll.CLASS_ARCHER)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_DAGGER)
                {
                    if (classId == NameAll.CLASS_CHEMIST || classId == NameAll.CLASS_SQUIRE || classId == NameAll.CLASS_MEDIATOR || classId == NameAll.CLASS_THIEF
                        || classId == NameAll.CLASS_NINJA || classId == NameAll.CLASS_DANCER)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_DICTIONARY)
                {
                    if (classId == NameAll.CLASS_ORACLE || classId == NameAll.CLASS_MEDIATOR || classId == NameAll.CLASS_CALCULATOR)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_GUN)
                {
                    if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_GUN || classId == NameAll.CLASS_CHEMIST || classId == NameAll.CLASS_MEDIATOR)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_HAMMER)
                {
                    if (classId == NameAll.CLASS_NINJA || classId == NameAll.CLASS_SQUIRE || classId == NameAll.CLASS_GEOMANCER)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_HARP)
                {
                    if (classId == NameAll.CLASS_BARD)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_KATANA)
                {
                    if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_KNIFE || classId == NameAll.CLASS_SAMURAI)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_KNIGHT)
                {
                    if (classId == NameAll.CLASS_KNIGHT)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_MAGIC_GUN)
                {
                    if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_GUN || classId == NameAll.CLASS_CHEMIST || classId == NameAll.CLASS_MEDIATOR)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_NINJA)
                {
                    if (classId == NameAll.CLASS_NINJA)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_ROD)
                {
                    if (classId == NameAll.CLASS_TIME_MAGE || classId == NameAll.CLASS_WIZARD || classId == NameAll.CLASS_SUMMONER)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_SPEAR)
                {
                    if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_SPEAR || classId == NameAll.CLASS_LANCER)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_STAFF)
                {
                    if (classId == NameAll.CLASS_TIME_MAGE || classId == NameAll.CLASS_PRIEST || classId == NameAll.CLASS_SUMMONER || classId == NameAll.CLASS_ORACLE)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_STICK)
                {
                    if (classId == NameAll.CLASS_ORACLE || classId == NameAll.CLASS_CALCULATOR)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_SWORD)
                {
                    if ( supportId == NameAll.SUPPORT_CLASSIC_EQUIP_SWORD || classId == NameAll.CLASS_SQUIRE || classId == NameAll.CLASS_KNIGHT || classId == NameAll.CLASS_GEOMANCER)
                        return true;
                }
            }
            else
            {
                //public static readonly int ITEM_ITEM_TYPE_SWORD = 19;
                //public static readonly int ITEM_ITEM_TYPE_DAGGER = 20;
                //public static readonly int ITEM_ITEM_TYPE_BOW = 21;
                //public static readonly int ITEM_ITEM_TYPE_WAND = 22;
                //public static readonly int ITEM_ITEM_TYPE_CROSSBOW = 23;
                //public static readonly int ITEM_ITEM_TYPE_GREATSWORD = 24;
                //public static readonly int ITEM_ITEM_TYPE_STICK = 25;
                //public static readonly int ITEM_ITEM_TYPE_GUN = 26;
                //public static readonly int ITEM_ITEM_TYPE_SCALES = 27;
                //public static readonly int ITEM_ITEM_TYPE_INSTRUMENT = 28;
                //public static readonly int ITEM_ITEM_TYPE_DECK = 29;
                //public static readonly int ITEM_ITEM_TYPE_SPEAR = 30;
                //public static readonly int ITEM_ITEM_TYPE_WHIP = 31;
                //public static readonly int ITEM_ITEM_TYPE_MACE = 32;
                //public static readonly int ITEM_ITEM_TYPE_KATANA = 33;
                //public static readonly int ITEM_ITEM_TYPE_PISTOL = 34;

                if( itemType == NameAll.ITEM_ITEM_TYPE_SWORD)
                {
                    if (classId == NameAll.CLASS_WARRIOR || classId == NameAll.CLASS_CENTURION || classId == NameAll.CLASS_ROGUE || supportId == NameAll.SUPPORT_EQUIP_SWORDS)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_DAGGER)
                {
                    return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_BOW)
                {
                    if (classId == NameAll.CLASS_RANGER || supportId == NameAll.SUPPORT_EQUIP_BOWS)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_WAND)
                {
                    if (classId == NameAll.CLASS_FIRE_MAGE || classId == NameAll.CLASS_HEALER || classId == NameAll.CLASS_NECROMANCER || supportId == NameAll.SUPPORT_EQUIP_WAND)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CROSSBOW)
                {
                    if (classId == NameAll.CLASS_RANGER || supportId == NameAll.SUPPORT_EQUIP_BOWS)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_GREATSWORD)
                {
                    if (classId == NameAll.CLASS_WARRIOR || classId == NameAll.CLASS_CENTURION || supportId == NameAll.SUPPORT_EQUIP_SWORDS)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_STICK)
                {
                    if (classId == NameAll.CLASS_HEALER || classId == NameAll.CLASS_NECROMANCER || supportId == NameAll.SUPPORT_EQUIP_STAFFS)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_GUN)
                {
                    if (classId == NameAll.CLASS_APOTHECARY || classId == NameAll.CLASS_DEMAGOGUE || supportId == NameAll.SUPPORT_EQUIP_GUNS)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_SCALES)
                {
                    if (classId == NameAll.CLASS_ARTIST || classId == NameAll.CLASS_DRUID || supportId == NameAll.SUPPORT_EQUIP_SCALES)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_INSTRUMENT)
                {
                    if (classId == NameAll.CLASS_ARTIST || supportId == NameAll.SUPPORT_EQUIP_INSTRUMENT_DECK)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_DECK)
                {
                    if (classId == NameAll.CLASS_ARTIST || classId == NameAll.CLASS_DEMAGOGUE || classId == NameAll.CLASS_APOTHECARY
                || supportId == NameAll.SUPPORT_EQUIP_INSTRUMENT_DECK)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_SPEAR)
                {
                    if (classId == NameAll.CLASS_WARRIOR || classId == NameAll.CLASS_CENTURION || classId == NameAll.CLASS_ROGUE
                            || supportId == NameAll.SUPPORT_EQUIP_SPEAR)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_WHIP)
                {
                    if (classId == NameAll.CLASS_BRAWLER || classId == NameAll.CLASS_DEMAGOGUE || classId == NameAll.CLASS_NECROMANCER
                        || classId == NameAll.CLASS_RANGER || supportId == NameAll.SUPPORT_EQUIP_WHIP_MACE)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_MACE)
                {
                    if (classId == NameAll.CLASS_BRAWLER || classId == NameAll.CLASS_HEALER || classId == NameAll.CLASS_WARRIOR || classId == NameAll.CLASS_ROGUE
                        || classId == NameAll.CLASS_DRUID || supportId == NameAll.SUPPORT_EQUIP_WHIP_MACE)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_PISTOL)
                {
                    if (classId == NameAll.CLASS_APOTHECARY || classId == NameAll.CLASS_DEMAGOGUE || supportId == NameAll.SUPPORT_EQUIP_GUNS)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_KATANA)
                {
                    if (classId == NameAll.CLASS_WARRIOR || classId == NameAll.CLASS_ROGUE || supportId == NameAll.SUPPORT_EQUIP_SWORDS)
                        return true;
                }

            }
        }
        else if (slot == NameAll.ITEM_SLOT_OFFHAND)
        {
            //public static readonly int ITEM_ITEM_TYPE_CLASSIC_SHIELD = 124;
            if(isClassicClass)
            {
                if( itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_SHIELD)
                {
                    if (supportId == NameAll.SUPPORT_CLASSIC_EQUIP_SHIELD || classId == NameAll.CLASS_KNIGHT || classId == NameAll.CLASS_LANCER || classId == NameAll.CLASS_GEOMANCER
                        || classId == NameAll.CLASS_ARCHER)
                        return true;
                }
                else if( supportId == NameAll.SUPPORT_TWO_SWORDS || AbilityManager.Instance.IsInnateAbility(pu.ClassId,NameAll.SUPPORT_TWO_SWORDS,NameAll.ABILITY_SLOT_SUPPORT))
                {
                    //public static readonly int ITEM_ITEM_TYPE_CLASSIC_SWORD = 128;
                    //public static readonly int ITEM_ITEM_TYPE_CLASSIC_NINJA = 122;
                    //public static readonly int ITEM_ITEM_TYPE_CLASSIC_KNIGHT = 120;
                    //public static readonly int ITEM_ITEM_TYPE_CLASSIC_HAMMER = 117;
                    //public static readonly int ITEM_ITEM_TYPE_CLASSIC_DAGGER = 114;
                    if( itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_SWORD || itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_NINJA || itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_KNIGHT
                        || itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_HAMMER || itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_DAGGER)
                    {
                        return IsEquippable(pu, NameAll.ITEM_SLOT_WEAPON, itemType);
                    }
                }

            }
            else
            {
                //public static readonly int ITEM_ITEM_TYPE_SHIELD = 9;
                //public static readonly int ITEM_ITEM_TYPE_ORB = 10;
                //public static readonly int ITEM_ITEM_TYPE_BOOK = 11;
                //public static readonly int ITEM_ITEM_TYPE_CHAIN = 12;
                if ( itemType == NameAll.ITEM_ITEM_TYPE_SHIELD)
                {
                    if (classId == NameAll.CLASS_WARRIOR || classId == NameAll.CLASS_CENTURION || supportId == NameAll.SUPPORT_EQUIP_SHIELD)
                        return true;
                }
                else if( itemType == NameAll.ITEM_ITEM_TYPE_ORB)
                {
                    return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_BOOK)
                {
                    return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CHAIN)
                {
                    return true;
                }
                else if (AbilityManager.Instance.IsInnateAbility(classId,NameAll.SUPPORT_DUAL_WIELD, NameAll.ABILITY_SLOT_SUPPORT)
                    || pu.IsAbilityEquipped(NameAll.SUPPORT_DUAL_WIELD, NameAll.ABILITY_SLOT_SUPPORT))
                {
                    if( itemType == NameAll.ITEM_ITEM_TYPE_SWORD || itemType == NameAll.ITEM_ITEM_TYPE_DAGGER || itemType == NameAll.ITEM_ITEM_TYPE_MACE
                        || itemType == NameAll.ITEM_ITEM_TYPE_PISTOL)
                    return IsEquippable(pu, NameAll.ITEM_SLOT_WEAPON, itemType);
                }

            }
        }
        else if (slot == NameAll.ITEM_SLOT_HEAD)
        {
            if (isClassicClass)
            {
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_HAT = 129;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_HELMET = 130;
                if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_HAT)
                {
                    if (classId != NameAll.CLASS_KNIGHT && classId != NameAll.CLASS_LANCER && classId != NameAll.CLASS_SAMURAI
                        && classId != NameAll.CLASS_MONK && classId != NameAll.CLASS_MIME)
                        return true;
                }
                else if( itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_HELMET)
                {
                    //class_id == 2 || class_id == 15 || class_id == 16
                    if (classId == NameAll.CLASS_KNIGHT || classId == NameAll.CLASS_LANCER || classId == NameAll.CLASS_SAMURAI)
                        return true;
                }
                else if( itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_RIBBON)
                {
                    if (pu.Sex == NameAll.SEX_FEMALE && classId != NameAll.CLASS_MONK)
                        return true;
                }
                return false;
                   
            }
            else
            {
                //public static readonly int ITEM_ITEM_TYPE_HAT = 1;
                //public static readonly int ITEM_ITEM_TYPE_HELMET = 2;
                //public static readonly int ITEM_ITEM_TYPE_MAGE_HAT = 3;
                //public static readonly int ITEM_ITEM_TYPE_BANDANA = 4;
                if( itemType == NameAll.ITEM_ITEM_TYPE_HAT)
                {
                    if (classId == NameAll.CLASS_ARTIST || classId == NameAll.CLASS_APOTHECARY || classId == NameAll.CLASS_DEMAGOGUE
                    || classId == NameAll.CLASS_NECROMANCER || classId == NameAll.CLASS_DRUID || supportId == NameAll.SUPPORT_EQUIP_CLOTHES)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_HELMET)
                {
                    if (classId == NameAll.CLASS_BRAWLER || classId == NameAll.CLASS_WARRIOR || classId == NameAll.CLASS_CENTURION
                    || classId == NameAll.CLASS_ARTIST || supportId == NameAll.SUPPORT_EQUIP_HEAVY_ARMORS)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_MAGE_HAT)
                {
                    if (classId == NameAll.CLASS_FIRE_MAGE || classId == NameAll.CLASS_HEALER || classId == NameAll.CLASS_NECROMANCER || classId == NameAll.CLASS_DEMAGOGUE
                        || supportId == NameAll.SUPPORT_EQUIP_MAGE_ROBES)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_BANDANA)
                {
                    if (classId == NameAll.CLASS_ROGUE || classId == NameAll.CLASS_RANGER || classId == NameAll.CLASS_DRUID
                    || classId == NameAll.CLASS_BRAWLER || supportId == NameAll.SUPPORT_EQUIP_LIGHT_ARMORS)
                        return true;
                }
            }
        }
        else if (slot == NameAll.ITEM_SLOT_BODY)
        {
            if(isClassicClass)
            {
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_ARMOR = 106;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_CLOTHES = 107;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_ROBES = 108;
                if( itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_ARMOR)
                {
                    if ( supportId == NameAll.SUPPORT_CLASSIC_EQUIP_ARMOR || classId == NameAll.CLASS_KNIGHT || classId == NameAll.CLASS_LANCER || classId == NameAll.CLASS_SAMURAI)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_CLOTHES)
                {
                    if (classId != NameAll.CLASS_KNIGHT && classId != NameAll.CLASS_LANCER && classId != NameAll.CLASS_SAMURAI
                        && classId != NameAll.CLASS_MIME)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_ROBES)
                {
                    //robes: knight, priest, wizard, time mage, summoner, mediator, oracle, geomancer, lancer, calculator, samurai
                    if (classId == NameAll.CLASS_KNIGHT || classId == NameAll.CLASS_LANCER || classId == NameAll.CLASS_SAMURAI
                            || classId == NameAll.CLASS_PRIEST || classId == NameAll.CLASS_WIZARD || classId == NameAll.CLASS_TIME_MAGE || classId == NameAll.CLASS_SUMMONER
                            || classId == NameAll.CLASS_MEDIATOR || classId == NameAll.CLASS_ORACLE || classId == NameAll.CLASS_GEOMANCER || classId == NameAll.CLASS_CALCULATOR)
                        return true;
                }
                return false;
            }
            else
            {
                //public static readonly int ITEM_ITEM_TYPE_HEAVY_ARMOR = 5;
                //public static readonly int ITEM_ITEM_TYPE_CLOTHES = 6;
                //public static readonly int ITEM_ITEM_TYPE_ROBES = 7;
                //public static readonly int ITEM_ITEM_TYPE_LIGHT_ARMOR = 8;
                if (itemType == NameAll.ITEM_ITEM_TYPE_HEAVY_ARMOR)
                {
                    if (classId == NameAll.CLASS_BRAWLER || classId == NameAll.CLASS_WARRIOR || classId == NameAll.CLASS_CENTURION
                    || classId == NameAll.CLASS_ARTIST || supportId == NameAll.SUPPORT_EQUIP_HEAVY_ARMORS)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_CLOTHES)
                {
                    if (classId == NameAll.CLASS_ARTIST || classId == NameAll.CLASS_APOTHECARY || classId == NameAll.CLASS_DEMAGOGUE
                    || classId == NameAll.CLASS_NECROMANCER || classId == NameAll.CLASS_DRUID || supportId == NameAll.SUPPORT_EQUIP_CLOTHES)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_ROBES)
                {
                    if (classId == NameAll.CLASS_FIRE_MAGE || classId == NameAll.CLASS_HEALER || classId == NameAll.CLASS_NECROMANCER || classId == NameAll.CLASS_DEMAGOGUE
                        || supportId == NameAll.SUPPORT_EQUIP_MAGE_ROBES)
                        return true;
                }
                else if (itemType == NameAll.ITEM_ITEM_TYPE_LIGHT_ARMOR)
                {
                    if (classId == NameAll.CLASS_ROGUE || classId == NameAll.CLASS_RANGER || classId == NameAll.CLASS_DRUID
                    || classId == NameAll.CLASS_BRAWLER || supportId == NameAll.SUPPORT_EQUIP_LIGHT_ARMORS)
                        return true;
                }
            }
        }
        else if (slot == NameAll.ITEM_SLOT_ACCESSORY)
        {
            if(isClassicClass)
            {
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_ARMLET = 100;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_GAUNTLET = 101;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_MANTLE = 102;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_PERFUME = 103;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_RING = 104;
                //public static readonly int ITEM_ITEM_TYPE_CLASSIC_SHOES = 105;
                if( itemType == NameAll.ITEM_ITEM_TYPE_CLASSIC_PERFUME)
                {
                    if (pu.Sex == NameAll.SEX_FEMALE)
                        return true;
                }
                else
                {
                    return true;
                }
                return false;
                
            }
            else
            {
                //public static readonly int ITEM_ITEM_TYPE_BOOTS = 14;
                //public static readonly int ITEM_ITEM_TYPE_BRACELET = 15;
                //public static readonly int ITEM_ITEM_TYPE_GLOVES = 16;
                //public static readonly int ITEM_ITEM_TYPE_CLOAK = 17;
                //public static readonly int ITEM_ITEM_TYPE_NECKLACE = 18;
                //public static readonly int ITEM_ITEM_TYPE_RING = 13;
                return true;
            }
        }

        return false;
    }

    public int GetItemCost(ItemObject io)
    {
        float zFloat = Mathf.Pow((float)io.Level, 1.5f);
        int z1 = (int)zFloat * 100;
        return z1;

    }


}