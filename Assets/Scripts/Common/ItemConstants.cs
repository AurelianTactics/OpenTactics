using System;
using System.Collections.Generic;
using System.Linq;

//constants related to items used across multiple scripts

public class ItemConstants
{

    #region ItemObject constants for building custom itemobjects
    public static readonly int INPUT_ITEM_ELEMENTAL_TYPE = 110;
    public static readonly int INPUT_ITEM_BLOCKS = 113;
    public static readonly int INPUT_ITEM_TYPE = 125;
    public static readonly int INPUT_ITEM_VERSION = 126;
    public static readonly int INPUT_ITEM_SLOT = 129;
    public static readonly int INPUT_ITEM_STATUS_NAME = 131;
    public static readonly int INPUT_ITEM_ON_HIT_EFFECT = 123;

    public static readonly int INPUT_ITEM_DESCRIPTION = 11;
    public static readonly int INPUT_ITEM_NAME = 34;

    public static readonly int INPUT_ITEM_CUNNING = 12;
    public static readonly int INPUT_ITEM_AGI = 14;
    public static readonly int INPUT_ITEM_BRAVE = 15;
    public static readonly int INPUT_ITEM_C_EVADE = 16;
    public static readonly int INPUT_ITEM_FAITH = 17;
    public static readonly int INPUT_ITEM_JUMP = 19;
    public static readonly int INPUT_ITEM_MA = 20;
    public static readonly int INPUT_ITEM_MP = 21;
    public static readonly int INPUT_ITEM_ON_HIT_CHANCE = 22;
    public static readonly int INPUT_ITEM_PA = 24;
    public static readonly int INPUT_ITEM_WP = 27;
    public static readonly int INPUT_ITEM_W_EVADE = 28;
    public static readonly int INPUT_ITEM_LEVEL = 30;
    public static readonly int INPUT_ITEM_SPEED = 32;
    public static readonly int INPUT_ITEM_P_EVADE = 33;
    public static readonly int INPUT_ITEM_M_EVADE = 35;
    public static readonly int INPUT_ITEM_MOVE = 36;
    public static readonly int INPUT_ITEM_LIFE = 37;

    public static readonly int INPUT_ITEM_GOLD = 38;
    #endregion

    public static readonly int ITEM_ITEM_TYPE_NONE = 0;
    public static readonly int ITEM_ITEM_TYPE_HAT = 1;
    public static readonly int ITEM_ITEM_TYPE_HELMET = 2;
    public static readonly int ITEM_ITEM_TYPE_MAGE_HAT = 3;
    public static readonly int ITEM_ITEM_TYPE_BANDANA = 4;
    public static readonly int ITEM_ITEM_TYPE_HEAVY_ARMOR = 5;
    public static readonly int ITEM_ITEM_TYPE_CLOTHES = 6;
    public static readonly int ITEM_ITEM_TYPE_ROBES = 7;
    public static readonly int ITEM_ITEM_TYPE_LIGHT_ARMOR = 8;
    public static readonly int ITEM_ITEM_TYPE_SHIELD = 9;
    public static readonly int ITEM_ITEM_TYPE_ORB = 10;
    public static readonly int ITEM_ITEM_TYPE_BOOK = 11;
    public static readonly int ITEM_ITEM_TYPE_CHAIN = 12;
    public static readonly int ITEM_ITEM_TYPE_RING = 13;
    public static readonly int ITEM_ITEM_TYPE_BOOTS = 14;
    public static readonly int ITEM_ITEM_TYPE_BRACELET = 15;
    public static readonly int ITEM_ITEM_TYPE_GLOVES = 16;
    public static readonly int ITEM_ITEM_TYPE_CLOAK = 17;
    public static readonly int ITEM_ITEM_TYPE_NECKLACE = 18;
    public static readonly int ITEM_ITEM_TYPE_FIST = 234;
    public static readonly int ITEM_ITEM_TYPE_SWORD = 19;
    public static readonly int ITEM_ITEM_TYPE_DAGGER = 20;
    public static readonly int ITEM_ITEM_TYPE_BOW = 21;
    public static readonly int ITEM_ITEM_TYPE_WAND = 22;
    public static readonly int ITEM_ITEM_TYPE_CROSSBOW = 23;
    public static readonly int ITEM_ITEM_TYPE_GREATSWORD = 24;
    public static readonly int ITEM_ITEM_TYPE_STICK = 25;
    public static readonly int ITEM_ITEM_TYPE_GUN = 26;
    public static readonly int ITEM_ITEM_TYPE_SCALES = 27;
    public static readonly int ITEM_ITEM_TYPE_INSTRUMENT = 28;
    public static readonly int ITEM_ITEM_TYPE_DECK = 29;
    public static readonly int ITEM_ITEM_TYPE_SPEAR = 30;
    public static readonly int ITEM_ITEM_TYPE_WHIP = 31;
    public static readonly int ITEM_ITEM_TYPE_MACE = 32;
    public static readonly int ITEM_ITEM_TYPE_KATANA = 33;
    public static readonly int ITEM_ITEM_TYPE_PISTOL = 34;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_ARMLET = 100;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_GAUNTLET = 101;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_MANTLE = 102;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_PERFUME = 103;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_RING = 104;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_SHOES = 105;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_ARMOR = 106;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_CLOTHES = 107;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_ROBES = 108;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_AXE = 109;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_LONGBOW = 110;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_BAG = 111;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_CLOTH = 112;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_CROSSBOW = 113;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_DAGGER = 114;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_DICTIONARY = 115;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_GUN = 116;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_HAMMER = 117;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_HARP = 118;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_KATANA = 119;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_KNIGHT = 120;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_MAGIC_GUN = 121;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_NINJA = 122;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_ROD = 123;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_SHIELD = 124;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_SPEAR = 125;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_STAFF = 126;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_STICK = 127;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_SWORD = 128;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_HAT = 129;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_HELMET = 130;
    public static readonly int ITEM_ITEM_TYPE_CLASSIC_RIBBON = 131;


    static Dictionary<int, string> itemTypeDict;

    public static Dictionary<int,string> GetItemTypeDict(int version, int slot)
    {
        if (itemTypeDict == null)
            BuildItemTypeDict();

        var tempDict = new Dictionary<int, string>();
        if( version == NameAll.VERSION_CLASSIC)
        {
            if( slot == NameAll.ITEM_SLOT_WEAPON)
            {
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_AXE, "Classic Axe");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_LONGBOW, "Classic Longbow");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_BAG, "Classic Bag");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_CLOTH, "Classic Cloth");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_CROSSBOW, "Classic Crossbow");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_DAGGER, "Classic Dagger");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_DICTIONARY, "Classic Dictionary");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_GUN, "Classic Gun");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_HAMMER, "Classic Hammer");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_HARP, "Classic Harp");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_KATANA, "Classic Katana");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_KNIGHT, "Classic Knight Sword");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_MAGIC_GUN, "Classic Magic Gun");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_NINJA, "Classic Ninja Sword");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_ROD, "Classic Rod");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_SPEAR, "Classic Spear");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_STAFF, "Classic Staff");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_STICK, "Classic Stick");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_SWORD, "Classic Sword"); 
            }
            else if( slot == NameAll.ITEM_SLOT_OFFHAND)
            {
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_SHIELD, "Classic Shield");
            }
            else if (slot == NameAll.ITEM_SLOT_HEAD)
            {
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_HAT, "Classic Hat");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_HELMET, "Classic Helmet");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_RIBBON, "Classic Ribbon");
            }
            else if (slot == NameAll.ITEM_SLOT_BODY)
            {
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_ARMOR, "Classic Armor");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_CLOTHES, "Classic Clothes");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_ROBES, "Classic Robes");
            }
            else if (slot == NameAll.ITEM_SLOT_ACCESSORY)
            {
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_ARMLET, "Classic Armlet");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_GAUNTLET, "Classic Gauntlet");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_MANTLE, "Classic Mantle");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_PERFUME, "Classic Perfume");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_RING, "Classic Ring");
                tempDict.Add(ITEM_ITEM_TYPE_CLASSIC_SHOES, "Classic Shoes");
            }
        }
        else
        {
            if (slot == NameAll.ITEM_SLOT_WEAPON)
            {
                tempDict.Add(ITEM_ITEM_TYPE_SWORD, "Sword");
                tempDict.Add(ITEM_ITEM_TYPE_DAGGER, "Dagger");
                tempDict.Add(ITEM_ITEM_TYPE_BOW, "Bow");
                tempDict.Add(ITEM_ITEM_TYPE_WAND, "Wand");
                tempDict.Add(ITEM_ITEM_TYPE_CROSSBOW, "Crossbow");
                tempDict.Add(ITEM_ITEM_TYPE_GREATSWORD, "Greatsword");
                tempDict.Add(ITEM_ITEM_TYPE_STICK, "Stick");
                tempDict.Add(ITEM_ITEM_TYPE_GUN, "Gun");
                tempDict.Add(ITEM_ITEM_TYPE_SCALES, "Scales");
                tempDict.Add(ITEM_ITEM_TYPE_INSTRUMENT, "Instrument");
                tempDict.Add(ITEM_ITEM_TYPE_DECK, "Deck");
                tempDict.Add(ITEM_ITEM_TYPE_SPEAR, "Spear");
                tempDict.Add(ITEM_ITEM_TYPE_WHIP, "Whip");
                tempDict.Add(ITEM_ITEM_TYPE_MACE, "Mace");
                tempDict.Add(ITEM_ITEM_TYPE_KATANA, "Katana");
                tempDict.Add(ITEM_ITEM_TYPE_PISTOL, "Pistol");
            }
            else if (slot == NameAll.ITEM_SLOT_OFFHAND)
            {
                tempDict.Add(ITEM_ITEM_TYPE_SHIELD, "Shield");
                tempDict.Add(ITEM_ITEM_TYPE_ORB, "Orb");
                tempDict.Add(ITEM_ITEM_TYPE_BOOK, "Book");
                tempDict.Add(ITEM_ITEM_TYPE_CHAIN, "Chain");
            }
            else if (slot == NameAll.ITEM_SLOT_HEAD)
            {
                tempDict.Add(ITEM_ITEM_TYPE_HAT, "Hat");
                tempDict.Add(ITEM_ITEM_TYPE_HELMET, "Helmet");
                tempDict.Add(ITEM_ITEM_TYPE_MAGE_HAT, "Mage Hat");
                tempDict.Add(ITEM_ITEM_TYPE_BANDANA, "Bandana");
            }
            else if (slot == NameAll.ITEM_SLOT_BODY)
            {
                tempDict.Add(ITEM_ITEM_TYPE_HEAVY_ARMOR, "Heavy Armor");
                tempDict.Add(ITEM_ITEM_TYPE_CLOTHES, "Clothes");

                tempDict.Add(ITEM_ITEM_TYPE_ROBES, "Robes");
                tempDict.Add(ITEM_ITEM_TYPE_LIGHT_ARMOR, "Light Armor");
            }
            else if (slot == NameAll.ITEM_SLOT_ACCESSORY)
            {
                tempDict.Add(ITEM_ITEM_TYPE_RING, "Ring");
                tempDict.Add(ITEM_ITEM_TYPE_BOOTS, "Boots");
                tempDict.Add(ITEM_ITEM_TYPE_BRACELET, "Bracelet");
                tempDict.Add(ITEM_ITEM_TYPE_GLOVES, "Gloves");
                tempDict.Add(ITEM_ITEM_TYPE_CLOAK, "Cloak");
                tempDict.Add(ITEM_ITEM_TYPE_NECKLACE, "Necklace");
            }
        }

        //foreach( KeyValuePair<int,string> kvp in itemTypeDict)
        //{
        //    if( kvp.Key > 100 && version == NameAll.VERSION_CLASSIC)
        //    {

        //    }
        //    else
        //    {

        //    }
        //}
        return tempDict;
    }

    public static string GetItemTypeString( int itemType)
    {
        if (itemTypeDict == null)
            BuildItemTypeDict();

        try
        {
            string zString = itemTypeDict[itemType];
            return zString;
        }
        catch (Exception e )
        {
            return "unknown";
        }
    }

    static void BuildItemTypeDict()
    {
        itemTypeDict = new Dictionary<int, string>();
        itemTypeDict.Add(ITEM_ITEM_TYPE_NONE,"None");
        itemTypeDict.Add(ITEM_ITEM_TYPE_HAT, "Hat");
        itemTypeDict.Add(ITEM_ITEM_TYPE_HELMET, "Helmet");
        itemTypeDict.Add(ITEM_ITEM_TYPE_MAGE_HAT, "Mage Hat");
        itemTypeDict.Add(ITEM_ITEM_TYPE_BANDANA, "Bandana");
        itemTypeDict.Add(ITEM_ITEM_TYPE_HEAVY_ARMOR, "Heavy Armor");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLOTHES, "Clothes");

        itemTypeDict.Add(ITEM_ITEM_TYPE_ROBES, "Robes");
        itemTypeDict.Add(ITEM_ITEM_TYPE_LIGHT_ARMOR, "Light Armor");
        itemTypeDict.Add(ITEM_ITEM_TYPE_SHIELD, "Shield");
        itemTypeDict.Add(ITEM_ITEM_TYPE_ORB, "Orb");
        itemTypeDict.Add(ITEM_ITEM_TYPE_BOOK, "Book");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CHAIN, "Chain");
        itemTypeDict.Add(ITEM_ITEM_TYPE_RING, "Ring");
        itemTypeDict.Add(ITEM_ITEM_TYPE_BOOTS, "Boots");
        itemTypeDict.Add(ITEM_ITEM_TYPE_BRACELET, "Bracelet");
        itemTypeDict.Add(ITEM_ITEM_TYPE_GLOVES, "Gloves");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLOAK, "Cloak");
        itemTypeDict.Add(ITEM_ITEM_TYPE_NECKLACE, "Necklace");
        itemTypeDict.Add(ITEM_ITEM_TYPE_FIST, "Fist");
        itemTypeDict.Add(ITEM_ITEM_TYPE_SWORD, "Sword");
        itemTypeDict.Add(ITEM_ITEM_TYPE_DAGGER, "Dagger");
        itemTypeDict.Add(ITEM_ITEM_TYPE_BOW, "Bow");
        itemTypeDict.Add(ITEM_ITEM_TYPE_WAND, "Wand");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CROSSBOW, "Crossbow");
        itemTypeDict.Add(ITEM_ITEM_TYPE_GREATSWORD, "Greatsword");
        itemTypeDict.Add(ITEM_ITEM_TYPE_STICK, "Stick");
        itemTypeDict.Add(ITEM_ITEM_TYPE_GUN, "Gun");
        itemTypeDict.Add(ITEM_ITEM_TYPE_SCALES, "Scales");
        itemTypeDict.Add(ITEM_ITEM_TYPE_INSTRUMENT, "Instrument");
        itemTypeDict.Add(ITEM_ITEM_TYPE_DECK, "Deck");
        itemTypeDict.Add(ITEM_ITEM_TYPE_SPEAR, "Spear");
        itemTypeDict.Add(ITEM_ITEM_TYPE_WHIP, "Whip");
        itemTypeDict.Add(ITEM_ITEM_TYPE_MACE, "Mace");
        itemTypeDict.Add(ITEM_ITEM_TYPE_KATANA, "Katana");
        itemTypeDict.Add(ITEM_ITEM_TYPE_PISTOL, "Pistol");

        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_ARMLET, "Classic Armlet");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_GAUNTLET, "Classic Gauntlet");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_MANTLE, "Classic Mantle");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_PERFUME, "Classic Perfume");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_RING, "Classic Ring");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_SHOES, "Classic Shoes");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_ARMOR, "Classic Armor");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_CLOTHES, "Classic Clothes");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_ROBES, "Classic Robes");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_AXE, "Classic Axe");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_LONGBOW, "Classic Longbow");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_BAG, "Classic Bag");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_CLOTH, "Classic Cloth");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_CROSSBOW, "Classic Crossbow");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_DAGGER, "Classic Dagger");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_DICTIONARY, "Classic Dictionary");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_GUN, "Classic Gun");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_HAMMER, "Classic Hammer");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_HARP, "Classic Harp");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_KATANA, "Classic Katana");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_KNIGHT, "Classic Knight Sword");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_MAGIC_GUN, "Classic Magic Gun");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_NINJA, "Classic Ninja Sword");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_ROD, "Classic Rod");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_SHIELD, "Classic Shield");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_SPEAR, "Classic Spear");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_STAFF, "Classic Staff");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_STICK, "Classic Stick");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_SWORD, "Classic Sword");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_HAT, "Classic Hat");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_HELMET, "Classic Helmet");
        itemTypeDict.Add(ITEM_ITEM_TYPE_CLASSIC_RIBBON, "Classic Ribbon");

    }

    public static readonly int ITEM_ELEMENTAL_NONE = 0;
    public static readonly int ITEM_ELEMENTAL_AIR = 1;
    public static readonly int ITEM_ELEMENTAL_DARK = 2;
    public static readonly int ITEM_ELEMENTAL_EARTH = 3;
    public static readonly int ITEM_ELEMENTAL_FIRE = 4;
    public static readonly int ITEM_ELEMENTAL_LIGHT = 5;
    public static readonly int ITEM_ELEMENTAL_LIGHTNING = 6;
    public static readonly int ITEM_ELEMENTAL_UNDEAD = 7;
    public static readonly int ITEM_ELEMENTAL_WATER = 8;
    public static readonly int ITEM_ELEMENTAL_WEAPON = 9;
    public static readonly int ITEM_ELEMENTAL_WIND = 10;
    public static readonly int ITEM_ELEMENTAL_ICE = 11;
    public static readonly int ITEM_ELEMENTAL_HP_DRAIN = 12;
    public static readonly int ITEM_ELEMENTAL_HP_RESTORE = 13;

    static Dictionary<int, string> itemElementalDict;

    public static Dictionary<int, string> GetItemElementalDict()
    {
        if (itemElementalDict == null)
            BuildItemElementalDict();

        return itemElementalDict;
    }

    public static string GetItemElementalString(int itemType)
    {
        if (itemElementalDict == null)
            BuildItemElementalDict();

        try
        {
            string zString = itemTypeDict[itemType];
            return zString;
        }
        catch (Exception e)
        {
            return "unknown";
        }
    }

    static void BuildItemElementalDict()
    {
        itemElementalDict = new Dictionary<int, string>();
        itemElementalDict.Add(ITEM_ELEMENTAL_NONE, "None");
        itemElementalDict.Add(ITEM_ELEMENTAL_AIR, "Air");
        itemElementalDict.Add(ITEM_ELEMENTAL_DARK, "Dark");
        itemElementalDict.Add(ITEM_ELEMENTAL_EARTH, "Earth");
        itemElementalDict.Add(ITEM_ELEMENTAL_FIRE, "Fire");
        itemElementalDict.Add(ITEM_ELEMENTAL_LIGHT, "Light");
        itemElementalDict.Add(ITEM_ELEMENTAL_LIGHTNING, "Lightning");
        itemElementalDict.Add(ITEM_ELEMENTAL_UNDEAD, "Undead");
        itemElementalDict.Add(ITEM_ELEMENTAL_WATER, "Water");
        itemElementalDict.Add(ITEM_ELEMENTAL_WEAPON, "Weapon");
        itemElementalDict.Add(ITEM_ELEMENTAL_WIND, "Wind");
        itemElementalDict.Add(ITEM_ELEMENTAL_ICE, "Ice");
        itemElementalDict.Add(ITEM_ELEMENTAL_HP_DRAIN, "HP Drain");
        itemElementalDict.Add(ITEM_ELEMENTAL_HP_RESTORE, "HP Restore");

    }

}
