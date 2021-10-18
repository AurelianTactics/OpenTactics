using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Holds statics/constants/functions that are called from a variety of scripts
/// I'm sure there's a better way to do this
/// </summary>
/// <remarks>
/// Seems pretty fucking stupid to do it this way but alternative is copying a bunch of constants to each script that calls it
/// </remarks>
public class NameAll
{
    //public const float TILE_PLACEMENT_HEIGHT = 0.125f;
    public const float TILE_STEP_HEIGHT = 0.25f;
    public static readonly int ROTATION_OFFSET = -25; //prefabs kind of aligned funnily

    public static readonly string NOTIFICATION_EXIT_GAME = "NotificationExitGame";
	public static readonly string NOTIFICATION_RESET_GAME = "NotificationResetGame";

	public static readonly int CRIT_KNOCKBACK_SUCCESS = 2;

    public static readonly int FLOATING_TEXT_CUSTOM = 19;

    public static readonly int ALTER_STAT_DAMAGE = 1;

    public static readonly int TWO_SWORDS_PHASE_START = 0;
    public static readonly int TWO_SWORDS_PHASE_FORCE_FACING = 1;
    public static readonly int TWO_SWORDS_PHASE_BLADEGRASP = 2;
    public static readonly int TWO_SWORDS_PHASE_SECOND_SWING = 3;

    public static readonly int MOD_NULL = 0; //and zodiac
    public static readonly int MOD_PHYSICAL = 1;
    public static readonly int MOD_MAGICAL = 2;
    public static readonly int MOD_NEUTRAL = 3;
    public static readonly int MOD_ATTACK = 4;
    public static readonly int MOD_PHYSICAL_MAGICAL = 11;
    public static readonly int MOD_PHYSICAL_NEUTRAL = 21;
    public static readonly int MOD_MAGICAL_PHYSICAL = 12;
    public static readonly int MOD_MAGICAL_NEUTRAL = 22;
    public static readonly int MOD_NEUTRAL_PHYSICAL  = 13;
    public static readonly int MOD_NEUTRAL_MAGICAL = 23;
    public static readonly int MOD_PHYSICAL_AGI = 31;

    public static readonly int VERSION_AURELIAN = 2;
    public static readonly int VERSION_CLASSIC = 1;
    public static readonly int VERSION_ALL = 0;
    public static readonly string VERSION_NAME_AURELIAN = "Aurelian";
    public static readonly string VERSION_NAME_CLASSIC = "Classic";

    public static readonly int NORTH = 0;
    public static readonly int EAST = 1;
    public static readonly int SOUTH = 2;
    public static readonly int WEST = 3;

    public static readonly int PROJECTILE_NONE = 0;
    public static readonly int PROJECTILE_LIMITED = 2;
    public static readonly int PROJECTILE_ATTACK = 1;

    public static readonly int SCENE_MAIN_MENU = 0;
    public static readonly int SCENE_EDIT_UNIT = 1;
    public static readonly int SCENE_CUSTOM_GAME = 2;
    public static readonly int SCENE_LEVEL_BUILDER = 3;
    public static readonly int SCENE_COMBAT = 4;
    
    public static readonly int SCENE_ABILITY_BUILDER = 5;
    public static readonly int SCENE_CAMPAIGN_BUILDER = 6;
    public static readonly int SCENE_CLASS_BUILDER = 7;
    public static readonly int SCENE_CAMPAIGNS = 8;

    public static readonly int SCENE_MP_MENU = 9;
    public static readonly int SCENE_STORY_BUILDER = 10;
    public static readonly int SCENE_CUT_SCENE = 11;
    public static readonly int SCENE_STORY_MODE = 12;

    public static readonly int SCENE_STORY_SHOP = 13;
    public static readonly int SCENE_ITEM_BUILDER = 14;
    public static readonly int SCENE_STORY_PARTY = 15;
    public static readonly int SCENE_WALK_AROUND = 16;

    public static readonly string PP_EDIT_UNIT_ENTRY = "editUnitEntry";
    public static readonly string PP_CUSTOM_GAME_ENTRY = "customGameEntry";
    public static readonly string PP_STORY_MODE_ENTRY = "storyModeEntry";
    //public static readonly string PP_CUSTOM_GAME_LEVEL = "customGameLevel"; //if in the future you want to have different maps load from differnt modes
    public static readonly string PP_COMBAT_ENTRY = "combatEntry";
    public static readonly string PP_CUT_SCENE_ENTRY = "cutSceneEntry";
    //public static readonly string PP_MP_MENU_ENTRY = "multiplayerMenuEntry";
    public static readonly string PP_COMBAT_LEVEL = "combatMap"; //index number of the level to load
    public static readonly string PP_LEVEL_DIRECTORY = "Aurelian"; //4 ways to load levels: level Aurelian, level custom, campaign level aurelian, campaign level custom
    public static readonly string PP_COMBAT_CAMPAIGN_LOAD = "campaignId"; //when loading a campaign level, what is the id to load?
	public static readonly string PP_RENDER_MODE = "renderMode";
	public static readonly int PP_RENDER_NORMAL = 0;
	public static readonly int PP_RENDER_NONE = 1;

	public static readonly string PP_RL_MODE = "reinforcementLearningMode";
	public static readonly int PP_RL_MODE_FALSE = 0;
	public static readonly int PP_RL_MODE_TRUE = 1;


	//public static readonly string PP_LEVEL_LOAD = "level_0"; //level to load, used in all modes for now

	public static readonly string PP_CUSTOM_GAME_TYPE = "customGameType";
    public static readonly int CUSTOM_GAME_OFFLINE = 0;
    public static readonly int CUSTOM_GAME_ONLINE = 1;

    public static readonly string PP_MOD_VERSION = "modVersion";
    public static readonly string PP_MP_OPTIONS_VERSION = "mpOptionsVersion";
    public static readonly string PP_MP_OPTIONS_DRAFT = "mpOptionsDraft";
    public static readonly string PP_MP_OPTIONS_NAME = "mpOptionsName";
    public static readonly string PP_MP_OPTIONS_PRIVATE_GAME = "mpOptionsPrivateGame";
    public static readonly string PP_VICTORY_TYPE = "victoryType";
    //public static readonly string PP_ABILITY_EDIT_MAX_INDEX = "abilityEditMaxIndex";
    public static readonly string PP_CLASS_EDIT_MAX_INDEX = "classEditMaxIndex";
    //public static readonly string PP_COMMAND_SET_MAX_INDEX = "commandSetMaxIndex";
    public static readonly string PP_CUSTOM_COMMAND_SET_BASE = "customCommandSet";
    public static readonly string PP_CUSTOM_CAMPAIGN_MAX_INDEX = "campaignMaxIndex";
    public static readonly string PP_AI_TYPE = "aiType";
    public static readonly string PP_CUT_SCENE_STORY_ID = "cutSceneStoryId"; //on cut scene load, which story Id to load
    public static readonly string PP_CUT_SCENE_CUT_SCENE_ID = "cutSceneCutSceneId";//on cut scene load, which cutSceneId to load
    public static readonly string PP_PROGRESSION_INT = "progressionInt"; //saved to file when going from story mode to cut scene or battle. 
																		 //after completion of either of those, this is loaded and used along with story save to progress the story

	public static readonly string PP_WA_MAP_SEED = "walkAroundMapSeed"; //PlayerPrefs for loading a WA map or creating own
	public static readonly string PP_WA_MAP_TIME_INT = "walkAroundTimeInt";

	public static readonly string PP_INIT_STATE = "initState"; //decides which state to initialize like Combat, WalkAround etc for CombatController
    public static readonly int INIT_STATE_COMBAT = 0;
    public static readonly int INIT_STATE_WALK_AROUND = 1;

    public static readonly string LEVEL_DIRECTORY_AURELIAN = "level_aurelian"; //aurelian levels in mp game controller
    public static readonly string LEVEL_DIRECTORY_CUSTOM = "level_custom"; //custom levels in mp game controller
    public static readonly string LEVEL_DIRECTORY_CAMPAIGN_AURELIAN = "campaign_aurelian";
    public static readonly string LEVEL_DIRECTORY_CAMPAIGN_CUSTOM = "campaign_custom";

    public static readonly int DRAFT_TYPE_FREE_PICK = 0;
    public static readonly int DRAFT_TYPE_TIMED_PICK = 1;
    public static readonly int DRAFT_TYPE_RANDOM_DRAFT = 2;

    public static readonly int VICTORY_TYPE_DEFEAT_PARTY = 0;
    public static readonly int VICTORY_TYPE_NONE = 1;
	public static readonly int VICTORY_TYPE_RL_RESET_EPISODE = 2;

	public static readonly int AI_TYPE_HUMAN_VS_HUMAN = 0;
    public static readonly int AI_TYPE_HUMAN_VS_AI = 1;
    public static readonly int AI_TYPE_AI_VS_AI = 2;

    public static readonly int SPELL_LEARNED_TYPE_NONE = 0;
    public static readonly int SPELL_LEARNED_TYPE_PLAYER_1 = 1;

    public static readonly string MAP_SAVE_CUSTOM_DEFAULT = "customDefault";

    public static readonly string LEVEL_ERROR_MESSAGE = "errorLevel19";

    public static readonly string LEVEL_BUILDER_RANDOM = "random";
    

    public static readonly float TILE_CENTER_HEIGHT = 0.05f;

    public static readonly int CUSTOM_COMMAND_SET_ID_START_VALUE = 1000;
    public static readonly int CUSTOM_SPELL_NAME_ID_START_VALUE = 10000;
    public static readonly int CUSTOM_ITEM_ID_START_VALUE = 10000;
    public static readonly int CUSTOM_CLASS_ID_START_VALUE = 1000;
    public static readonly int CUSTOM_CAMPAIGN_ID_START_VALUE = 1000;
    public static readonly int CUSTOM_STORY_ID_START_VALUE = 1000;
    //public static readonly int CUSTOM_CAMPAIGN_ID_MAX_VALUE = 1000;

    public static readonly int DIALOGUE_PHASE_START = 0;
    public static readonly int DIALOGUE_PHASE_END = 1;
    public static readonly int DIALOGUE_ICON_0 = 0;
    public static readonly int DIALOGUE_ICON_1 = 1;
    public static readonly int DIALOGUE_SIDE_LEFT = 0;
    public static readonly int DIALOGUE_SIDE_RIGHT = 1;
    public static readonly int SPAWN_TYPE_RANDOM = 0;
    public static readonly int SPAWN_TYPE_USER_SELECT = 1;
    public static readonly int CAMPAIGN_SPAWN_UNIT_NONE = -1;
    public static readonly int CAMPAIGN_SPAWN_UNIT_NULL = -2;

	public static readonly int COMBAT_LOG_TYPE_END_TURN = 0;
	public static readonly int COMBAT_LOG_TYPE_MOVE = 1;
	public static readonly int COMBAT_LOG_TYPE_ACTION = 2;
	public static readonly int COMBAT_LOG_TYPE_CONTINUED_ACTION = 3;
	public static readonly int COMBAT_LOG_TYPE_REACTION = 4;
	public static readonly int COMBAT_LOG_TYPE_MIME_ACTION = 5;
	public static readonly int COMBAT_LOG_TYPE_SLOW_ACTION = 6;
	public static readonly int COMBAT_LOG_TYPE_STATUS_MANAGER = 7;
	public static readonly int COMBAT_LOG_TYPE_MISC = 8;
	public static readonly int COMBAT_LOG_TYPE_ALTER_STAT_ADD = 9;
	public static readonly int COMBAT_LOG_TYPE_ALTER_STAT_REMOVE = 10;
	public static readonly int COMBAT_LOG_TYPE_ROLL = 11;

	public static readonly int COMBAT_LOG_SUBTYPE_MOVE_TELEPORT_ROLL = 1000;
	public static readonly int COMBAT_LOG_SUBTYPE_MOVE_SWAP = 1001;
	public static readonly int COMBAT_LOG_SUBTYPE_MOVE_EFFECT = 1002;
	public static readonly int COMBAT_LOG_SUBTYPE_SET_HP = 2000;
	public static readonly int COMBAT_LOG_SUBTYPE_SET_HP_REMOVE_ALL = 2001;
	public static readonly int COMBAT_LOG_SUBTYPE_ITEM_ON_HIT_CHANCE = 2002;
	public static readonly int COMBAT_LOG_SUBTYPE_KATANA_BREAK = 2003;
	public static readonly int COMBAT_LOG_SUBTYPE_ROLL_RESOLVE_ACTION = 2004;
	public static readonly int COMBAT_LOB_SUBTYPE_CRIT_ROLL = 2005;
	public static readonly int COMBAT_LOG_SUBTYPE_SPELL_SLOW_ADD = 2006;
	public static readonly int COMBAT_LOG_SUBTYPE_REACTION_BRAVE_ROLL = 4000;
	public static readonly int COMBAT_LOG_SUBTYPE_SLOW_ACTION_UNABLE_TO_CAST = 6000;
	public static readonly int COMBAT_LOG_SUBTYPE_STATUS_REMOVE = 7000;
	public static readonly int COMBAT_LOG_SUBTYPE_END_TURN_TICK_REGEN = 7001;
	public static readonly int COMBAT_LOG_SUBTYPE_END_TURN_TICK_POISON = 7002;
	public static readonly int COMBAT_LOG_SUBTYPE_STATUS_ADD_ROLL = 7003;
	public static readonly int COMBAT_LOG_SUBTYPE_STATUS_ADD = 7004;
	public static readonly int COMBAT_LOG_SUBTYPE_KNOCKBACK_MOVE = 8000;
	public static readonly int COMBAT_LOG_SUBTYPE_KNOCKBACK_DAMAGE = 8001;
	public static readonly int COMBAT_LOG_SUBTYPE_ADD_BRAVE_DEFAULT = 8002;
	public static readonly int COMBAT_LOG_SUBTYPE_CRYSTAL_PICK_UP = 8003;
	public static readonly int COMBAT_LOG_SUBTYPE_UNDEAD_REVIVE_ROLL = 8004;
	public static readonly int COMBAT_LOG_SUBTYPE_KNOCKBACK_SUCCESS = 8005;
	public static readonly int COMBAT_LOG_SUBTYPE_CRYSTAL_ROLL = 8006;

	public static readonly int STORY_MAP_0 = 0;
    public static readonly int STORY_MAP_1 = 1;

    public static readonly int NULL_INT = -1919;
    public static readonly int NULL_UNIT_ID = -19;

	public static readonly int MAP_DICT_SEED = -1919190; //in WA mode, save map dictionary including seed and time_int
	public static readonly int MAP_DICT_TIME_INT = -1919191;
	public static readonly int MAP_DICT_CURRENT_MAP = -1919192;
	public static readonly string WA_UNIT_SAVE_PLAYER_LIST = "player_units_";
	public static readonly string WA_UNIT_SAVE_MAP_LIST = "map_units_";

	public static readonly int TILE_TYPE_DEFAULT = 0; //standard tile
	public static readonly int TILE_TYPE_EXIT_MAP = 1; //can switch maps

	public static readonly int MAX_CTR = 12;

    public static readonly int ITEM_MANAGER_SIMPLE = 0; //
    public static readonly int ITEM_MANAGER_SO = 1; //load and unload itemobjects as scriptable objects

    public static readonly int STATS_DAMAGE_DONE = 0;
    public static readonly int STATS_DAMAGE_HEALED = 1;
    public static readonly int STATS_STATUSES_DONE = 2;
    public static readonly int STATS_STATUSES_HEALED = 3;
    public static readonly int STATS_KILLS_DONE = 4;
    public static readonly int STATS_KILLS_HEALED = 5;

    public static readonly int SPELL_EFFECT_LINE_2 = 102;
    public static readonly int SPELL_EFFECT_LINE_3 = 103;
    public static readonly int SPELL_EFFECT_LINE_4 = 104;
    public static readonly int SPELL_EFFECT_LINE_5 = 105;
    public static readonly int SPELL_EFFECT_LINE_6 = 106;
    public static readonly int SPELL_EFFECT_LINE_7 = 107;
    public static readonly int SPELL_EFFECT_LINE_8 = 108;
    public static readonly int SPELL_EFFECT_ALLIES = 109;
    public static readonly int SPELL_EFFECT_ENEMIES = 110;
    public static readonly int SPELL_EFFECT_MATH_SKILL = 111;
    public static readonly int SPELL_EFFECT_CONE_BASE = 120;
    public static readonly int SPELL_EFFECT_CONE_2 = 122; //subtract by base to get the level of the cone
    public static readonly int SPELL_EFFECT_CONE_3 = 123; //subtract by base to get the level of the cone
    public static readonly int SPELL_EFFECT_CONE_4 = 124; //subtract by base to get the level of the cone
    public static readonly int SPELL_EFFECT_CONE_5 = 125; //subtract by base to get the level of the cone
    public static readonly int SPELL_EFFECT_CONE_6 = 126; //subtract by base to get the level of the cone
    public static readonly int SPELL_EFFECT_CONE_7 = 127; //subtract by base to get the level of the cone
    public static readonly int SPELL_EFFECT_CONE_8 = 128; //subtract by base to get the level of the cone
    public static readonly int SPELL_EFFECT_CONE_MAX = 129; //subtract by base to get the level of the cone

    public static readonly int SPELL_RANGE_MIN_SELF_TARGET = -1;
    public static readonly int SPELL_RANGE_MIN_WEAPON = 100;
    public static readonly int SPELL_RANGE_MAX_WEAPON = 101;
    public static readonly int SPELL_RANGE_MAX_LINE = 102;
    public static readonly int SPELL_RANGE_MAX_MOVE = 103;
    public static readonly int SPELL_RANGE_MAX_THROW_ITEM = 104;
    public static readonly int SPELL_RANGE_Z_WEAPON = 105;
    public static readonly int SPELL_RANGE_SPEAR = 106;

    public static readonly int SPELL_RANGE_Z_HEIGHT_BASE = 110;
    public static readonly int SPELL_RANGE_Z_HEIGHT_1 = 111; 
    public static readonly int SPELL_RANGE_Z_HEIGHT_2 = 112; //bow height +/- 1 panel for every 2H difference
    public static readonly int SPELL_RANGE_Z_HEIGHT_3 = 113;
    public static readonly int SPELL_RANGE_Z_HEIGHT_4 = 114;
    public static readonly int SPELL_RANGE_Z_HEIGHT_5 = 115;
    public static readonly int SPELL_RANGE_Z_HEIGHT_MAX = 115;

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

    public static readonly int ELEMENT_TYPE_STRENGTHEN = 0;
    public static readonly int ELEMENT_TYPE_WEAK = 1;
    public static readonly int ELEMENT_TYPE_HALF = 2;
    public static readonly int ELEMENT_TYPE_ABSORB = 3;

    public static int GetStatusIdFromElementType( int element, int elementType)
    {
        int z1 = 0;
        if( element == NameAll.ITEM_ELEMENTAL_FIRE)
        {
            if( elementType == ELEMENT_TYPE_STRENGTHEN)
            {
                z1 = NameAll.STATUS_ID_STRENGTHEN_FIRE;
            }
            else if (elementType == ELEMENT_TYPE_WEAK)
            {
                z1 = NameAll.STATUS_ID_WEAK_FIRE;
            }
            else if( elementType == ELEMENT_TYPE_HALF)
            {
                z1 = NameAll.STATUS_ID_HALF_FIRE;
            }
            else if (elementType == ELEMENT_TYPE_ABSORB)
            {
                z1 = NameAll.STATUS_ID_ABSORB_FIRE;
            }
        }
        else if (element == NameAll.ITEM_ELEMENTAL_UNDEAD)
        {
            if (elementType == ELEMENT_TYPE_STRENGTHEN)
            {
                z1 = NameAll.STATUS_ID_STRENGTHEN_UNDEAD;
            }
            else if (elementType == ELEMENT_TYPE_WEAK)
            {
                z1 = NameAll.STATUS_ID_WEAK_UNDEAD;
            }
            else if (elementType == ELEMENT_TYPE_HALF)
            {
                z1 = NameAll.STATUS_ID_HALF_UNDEAD;
            }
            else if (elementType == ELEMENT_TYPE_ABSORB)
            {
                z1 = NameAll.STATUS_ID_ABSORB_UNDEAD;
            }
        }
        else if (element == NameAll.ITEM_ELEMENTAL_DARK)
        {
            if (elementType == ELEMENT_TYPE_STRENGTHEN)
            {
                z1 = NameAll.STATUS_ID_STRENGTHEN_DARK;
            }
            else if (elementType == ELEMENT_TYPE_WEAK)
            {
                z1 = NameAll.STATUS_ID_WEAK_DARK;
            }
            else if (elementType == ELEMENT_TYPE_HALF)
            {
                z1 = NameAll.STATUS_ID_HALF_DARK;
            }
            else if (elementType == ELEMENT_TYPE_ABSORB)
            {
                z1 = NameAll.STATUS_ID_ABSORB_DARK;
            }
        }
        else if (element == NameAll.ITEM_ELEMENTAL_LIGHT)
        {
            if (elementType == ELEMENT_TYPE_STRENGTHEN)
            {
                z1 = NameAll.STATUS_ID_STRENGTHEN_LIGHT;
            }
            else if (elementType == ELEMENT_TYPE_WEAK)
            {
                z1 = NameAll.STATUS_ID_WEAK_LIGHT;
            }
            else if (elementType == ELEMENT_TYPE_HALF)
            {
                z1 = NameAll.STATUS_ID_HALF_LIGHT;
            }
            else if (elementType == ELEMENT_TYPE_ABSORB)
            {
                z1 = NameAll.STATUS_ID_ABSORB_LIGHT;
            }
        }
        else if (element == NameAll.ITEM_ELEMENTAL_EARTH)
        {
            if (elementType == ELEMENT_TYPE_STRENGTHEN)
            {
                z1 = NameAll.STATUS_ID_STRENGTHEN_EARTH;
            }
            else if (elementType == ELEMENT_TYPE_WEAK)
            {
                z1 = NameAll.STATUS_ID_WEAK_EARTH;
            }
            else if (elementType == ELEMENT_TYPE_HALF)
            {
                z1 = NameAll.STATUS_ID_HALF_EARTH;
            }
            else if (elementType == ELEMENT_TYPE_ABSORB)
            {
                z1 = NameAll.STATUS_ID_ABSORB_EARTH;
            }
        }
        else if (element == NameAll.ITEM_ELEMENTAL_WEAPON)
        {
            if (elementType == ELEMENT_TYPE_STRENGTHEN)
            {
                z1 = NameAll.STATUS_ID_STRENGTHEN_WEAPON;
            }
            else if (elementType == ELEMENT_TYPE_WEAK)
            {
                z1 = NameAll.STATUS_ID_WEAK_WEAPON;
            }
            else if (elementType == ELEMENT_TYPE_HALF)
            {
                z1 = NameAll.STATUS_ID_HALF_WEAPON;
            }
            else if (elementType == ELEMENT_TYPE_ABSORB)
            {
                z1 = NameAll.STATUS_ID_ABSORB_WEAPON;
            }
        }
        else if (element == NameAll.ITEM_ELEMENTAL_LIGHTNING)
        {
            if (elementType == ELEMENT_TYPE_STRENGTHEN)
            {
                z1 = NameAll.STATUS_ID_STRENGTHEN_LIGHTNING;
            }
            else if (elementType == ELEMENT_TYPE_WEAK)
            {
                z1 = NameAll.STATUS_ID_WEAK_LIGHTNING;
            }
            else if (elementType == ELEMENT_TYPE_HALF)
            {
                z1 = NameAll.STATUS_ID_HALF_LIGHTNING;
            }
            else if (elementType == ELEMENT_TYPE_ABSORB)
            {
                z1 = NameAll.STATUS_ID_ABSORB_LIGHTNING;
            }
        }
        else if (element == NameAll.ITEM_ELEMENTAL_ICE)
        {
            if (elementType == ELEMENT_TYPE_STRENGTHEN)
            {
                z1 = NameAll.STATUS_ID_STRENGTHEN_ICE;
            }
            else if (elementType == ELEMENT_TYPE_WEAK)
            {
                z1 = NameAll.STATUS_ID_WEAK_ICE;
            }
            else if (elementType == ELEMENT_TYPE_HALF)
            {
                z1 = NameAll.STATUS_ID_HALF_ICE;
            }
            else if (elementType == ELEMENT_TYPE_ABSORB)
            {
                z1 = NameAll.STATUS_ID_ABSORB_ICE;
            }
        }
        else if (element == NameAll.ITEM_ELEMENTAL_AIR)
        {
            if (elementType == ELEMENT_TYPE_STRENGTHEN)
            {
                z1 = NameAll.STATUS_ID_STRENGTHEN_AIR;
            }
            else if (elementType == ELEMENT_TYPE_WEAK)
            {
                z1 = NameAll.STATUS_ID_WEAK_AIR;
            }
            else if (elementType == ELEMENT_TYPE_HALF)
            {
                z1 = NameAll.STATUS_ID_HALF_AIR;
            }
            else if (elementType == ELEMENT_TYPE_ABSORB)
            {
                z1 = NameAll.STATUS_ID_ABSORB_AIR;
            }
        }
        else if (element == NameAll.ITEM_ELEMENTAL_WATER)
        {
            if (elementType == ELEMENT_TYPE_STRENGTHEN)
            {
                z1 = NameAll.STATUS_ID_STRENGTHEN_WATER;
            }
            else if (elementType == ELEMENT_TYPE_WEAK)
            {
                z1 = NameAll.STATUS_ID_WEAK_WATER;
            }
            else if (elementType == ELEMENT_TYPE_HALF)
            {
                z1 = NameAll.STATUS_ID_HALF_WATER;
            }
            else if (elementType == ELEMENT_TYPE_ABSORB)
            {
                z1 = NameAll.STATUS_ID_ABSORB_WATER;
            }
        }
        else if (element == NameAll.ITEM_ELEMENTAL_WIND)
        {
            if (elementType == ELEMENT_TYPE_STRENGTHEN)
            {
                z1 = NameAll.STATUS_ID_STRENGTHEN_WIND;
            }
            else if (elementType == ELEMENT_TYPE_WEAK)
            {
                z1 = NameAll.STATUS_ID_WEAK_WIND;
            }
            else if (elementType == ELEMENT_TYPE_HALF)
            {
                z1 = NameAll.STATUS_ID_HALF_WIND;
            }
            else if (elementType == ELEMENT_TYPE_ABSORB)
            {
                z1 = NameAll.STATUS_ID_ABSORB_WIND;
            }
        }
        else if (element == NameAll.ITEM_ELEMENTAL_HP_DRAIN)
        {
            //Debug.Log("ERROR: no elemental type for hp drain");
            //if (elementType == ELEMENT_TYPE_STRENGTHEN)
            //{
            //    z1 = NameAll.STATUS_ID_STRENGTHEN_HP_DRAIN;
            //}
            //else if (elementType == ELEMENT_TYPE_WEAK)
            //{
            //    z1 = NameAll.STATUS_ID_WEAK_HP_DRAIN;
            //}
            //else if (elementType == ELEMENT_TYPE_HALF)
            //{
            //    z1 = NameAll.STATUS_ID_HALF_HP_DRAIN;
            //}
            //else if (elementType == ELEMENT_TYPE_ABSORB)
            //{
            //    z1 = NameAll.STATUS_ID_ABSORB_HP_DRAIN;
            //}
        }
        else if (element == NameAll.ITEM_ELEMENTAL_HP_RESTORE)
        {
            //Debug.Log("ERROR: no elemental type for hp restore");
            //if (elementType == ELEMENT_TYPE_STRENGTHEN)
            //{
            //    z1 = NameAll.STATUS_ID_STRENGTHEN_WIND;
            //}
            //else if (elementType == ELEMENT_TYPE_WEAK)
            //{
            //    z1 = NameAll.STATUS_ID_WEAK_WIND;
            //}
            //else if (elementType == ELEMENT_TYPE_HALF)
            //{
            //    z1 = NameAll.STATUS_ID_HALF_WIND;
            //}
            //else if (elementType == ELEMENT_TYPE_ABSORB)
            //{
            //    z1 = NameAll.STATUS_ID_ABSORB_WIND;
            //}
        }
        return z1;
    }

    public static string GetElementalString(int elementalType, bool isAbilityEdit = false)
    {
        string zString;
        if( elementalType == ITEM_ELEMENTAL_AIR)
        {
            if(isAbilityEdit)
                zString = "Air";
            else
                zString = "e:air";
        }
        else if (elementalType == ITEM_ELEMENTAL_DARK)
        {
            if (isAbilityEdit)
                zString = "Dark";
            else
                zString = "e:dark";
        }
        else if (elementalType == ITEM_ELEMENTAL_EARTH)
        {
            if (isAbilityEdit)
                zString = "Earth";
            else
                zString = "e:earth";
        }
        else if (elementalType == ITEM_ELEMENTAL_FIRE)
        {
            if (isAbilityEdit)
                zString = "Fire";
            else
                zString = "e:fire";
        }
        else if (elementalType == ITEM_ELEMENTAL_LIGHT)
        {
            if (isAbilityEdit)
                zString = "Holy";
            else
                zString = "e:holy";
        }
        else if (elementalType == ITEM_ELEMENTAL_LIGHTNING)
        {
            if (isAbilityEdit)
                zString = "Lightning";
            else
                zString = "e:lightning";
        }
        else if (elementalType == ITEM_ELEMENTAL_UNDEAD)
        {
            if (isAbilityEdit)
                zString = "Undead";
            else
                zString = "e:undead";
        }
        else if (elementalType == ITEM_ELEMENTAL_WATER)
        {
            if (isAbilityEdit)
                zString = "Water";
            else
                zString = "e:water";
        }
        else if (elementalType == ITEM_ELEMENTAL_WEAPON)
        {
            if (isAbilityEdit)
                zString = "Weapon";
            else
                zString = "e:weapon";
        }
        else if (elementalType == ITEM_ELEMENTAL_HP_DRAIN)
        {
            if (isAbilityEdit)
                zString = "Life Drain";
            else
                zString = "e:life drain";
        }
        else if (elementalType == ITEM_ELEMENTAL_HP_RESTORE)
        {
            if (isAbilityEdit)
                zString = "HP Restore";
            else
                zString = "e:hp restore";
        }
        else if (elementalType == ITEM_ELEMENTAL_ICE)
        {
            if (isAbilityEdit)
                zString = "Ice";
            else
                zString = "e:ice";
        }
        else if (elementalType == ITEM_ELEMENTAL_WIND)
        {
            if (isAbilityEdit)
                zString = "Wind";
            else
                zString = "e:wind";
        }
        else
        {
            zString = "none";
        }
        return zString;
    }

    public static readonly int ITEM_OBJECT_ITEM_ID = 0;
    public static readonly int ITEM_OBJECT_VERSION = 1;
    public static readonly int ITEM_OBJECT_SLOT = 2;
    public static readonly int ITEM_OBJECT_ITEM_TYPE = 3;
    public static readonly int ITEM_OBJECT_LEVEL = 4;

    public static readonly int ITEM_OBJECT_ITEM_NAME = 5;
    public static readonly int ITEM_OBJECT_BLOCKS = 6;
    public static readonly int ITEM_OBJECT_STATUS_NAME = 7;
    public static readonly int ITEM_OBJECT_STAT_BRAVE = 8;
    public static readonly int ITEM_OBJECT_STAT_C_EVADE = 9;

    public static readonly int ITEM_OBJECT_STAT_CUNNING = 10;
    public static readonly int ITEM_OBJECT_STAT_FAITH = 11;
    public static readonly int ITEM_OBJECT_STAT_LIFE = 12;
    public static readonly int ITEM_OBJECT_STAT_JUMP = 13;
    public static readonly int ITEM_OBJECT_STAT_M_EVADE = 14;
 
    public static readonly int ITEM_OBJECT_STAT_MA = 15;
    public static readonly int ITEM_OBJECT_STAT_MOVE = 16;
    public static readonly int ITEM_OBJECT_STAT_MP = 17;
    public static readonly int ITEM_OBJECT_STAT_P_EVADE = 18;
    public static readonly int ITEM_OBJECT_STAT_PA = 19;

    public static readonly int ITEM_OBJECT_STAT_SPEED = 20;
    public static readonly int ITEM_OBJECT_STAT_W_EVADE = 21;
    public static readonly int ITEM_OBJECT_STAT_WP = 22;
    public static readonly int ITEM_OBJECT_ELEMENTAL_TYPE = 23;
    public static readonly int ITEM_OBJECT_ON_HIT_EFFECT = 24;

    public static readonly int ITEM_OBJECT_ON_HIT_CHANCE = 25;
    public static readonly int ITEM_OBJECT_STAT_AGI = 26;
    public static readonly int ITEM_OBJECT_DESCRIPTION = 27;

    public static readonly int ITEM_SLOT_WEAPON = 0;
    public static readonly int ITEM_SLOT_OFFHAND = 1;
    public static readonly int ITEM_SLOT_HEAD = 2;
    public static readonly int ITEM_SLOT_BODY = 3;
    public static readonly int ITEM_SLOT_ACCESSORY = 4;
    public static readonly int ITEM_SLOT_ANY = 5;

    public static readonly int ABILITY_SLOT_PRIMARY = 1;
    public static readonly int ABILITY_SLOT_SECONDARY = 2;
    public static readonly int ABILITY_SLOT_REACTION = 3;
    public static readonly int ABILITY_SLOT_SUPPORT = 4;
    public static readonly int ABILITY_SLOT_MOVEMENT = 5;

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

    public static readonly string SEX_FEMALE = "Female";
    public static readonly string SEX_MALE = "Male";

    public static readonly int MAP_TILE_HIGHLIGHT_TARGET = 101;
    public static readonly int MAP_TILE_HIGHLIGHT_EFFECT = 102;

    //SpellName Keys. used for titles  (AbilityEditScrollList) and for deciphering SN values in CalcCode
    //calculateMimic counterType statusCancel elementType ignoresDefense
    public static readonly string SN_CALCULATE = "Can Use With Math Skill";
    public static readonly string SN_MIMIC = "Can Be Mimicked";
    public static readonly string SN_COUNTER_TYPE = "Counter Reaction Type";
    public static readonly string SN_STATUS_CANCEL = "Canceled by a Status";
    public static readonly string SN_ELEMENT_TYPE = "Elemental Type";
    public static readonly string SN_IGNORES_DEFENSE = "Ignores Defenses";
    //rangeXYMin  rangeXYMax rangeZ  effectXY effectZ alliesType casterImmune
    public static readonly string SN_RANGE_XY_MIN = "Minimum Horizontal Range";
    public static readonly string SN_RANGE_XY_MAX = "Maximum Horizontal Range";
    public static readonly string SN_RANGE_Z = "Vertical Range";
    public static readonly string SN_EFFECT_XY = "Area of Effect Horizontal";
    public static readonly string SN_EFFECT_Z = "Area of Effect Vertical";
    public static readonly string SN_ALLIES_TYPE = "Hits Allies/Enemies Only";
    public static readonly string SN_CASTER_IMMUNE = "Caster Immune";

    public static readonly string SN_MOD = "Modification of Primary Stat (Mod)";
    public static readonly string SN_DAMAGE_FORMULA = "Damage Formula";
    public static readonly string SN_BASE_HIT = "Base Hit %";
    public static readonly string SN_ABILITY_TYPE = "Type";
    public static readonly string SN_EVADABLE = "Evadable";
    public static readonly string SN_REFLECTABLE = "Reflectable";
    public static readonly string SN_BASE_Q = "Ability Strength";
    public static readonly string SN_HITS_STAT = "Effects a Stat?";
    public static readonly string SN_REMOVE_STAT = "Heal/Damage Type";
    public static readonly string SN_STAT_TYPE = "Stat Effect";
    public static readonly string SN_ADD_STATUS = "Inflicts Status?";
    public static readonly string SN_STATUS_TYPE = "Status Name";

    public static readonly string SN_COMMAND_SET = "Command Set";
    public static readonly string SN_VERSION = "Version";
    public static readonly string SN_ABILITY_NAME = "Ability Name";
    public static readonly string SN_CTR = "CTR (Clock Ticks to Resolution)";
    public static readonly string SN_MP = "Magic Points";
    public static readonly string SN_DELETE = "Delete Ability";

    public static readonly string STATUS_CANCEL_NAME_NONE = "None";
    public static readonly string STATUS_CANCEL_NAME_SILENCE = "Cancelled by Silence";
    public static readonly int STATUS_CANCEL_NONE = 0;
    public static readonly int STATUS_CANCEL_SILENCE = 1;

    public static readonly int FROG_ATTACK_SPELL_INDEX = 212;
    public static readonly int FROG_SPELL_SPELL_INDEX = 22;
    public static readonly int LONGBOW_ATTACK_SPELL_INDEX = 181;
    public static readonly int SPELL_INDEX_CT_3 = 213;
    public static readonly int SPELL_INDEX_CT_4 = 214;
    public static readonly int SPELL_INDEX_CT_5 = 215;
    public static readonly int SPELL_INDEX_CT_PRIME = 216;
    public static readonly int SPELL_INDEX_LEVEL_3 = 221;
    public static readonly int SPELL_INDEX_LEVEL_4 = 222;
    public static readonly int SPELL_INDEX_LEVEL_5 = 223;
    public static readonly int SPELL_INDEX_LEVEL_PRIME = 224;
    public static readonly int SPELL_INDEX_HEIGHT_3 = 217;
    public static readonly int SPELL_INDEX_HEIGHT_4 = 218;
    public static readonly int SPELL_INDEX_HEIGHT_5 = 219;
    public static readonly int SPELL_INDEX_HEIGHT_PRIME = 220;
    public static readonly int SPELL_INDEX_QUICK = 112;
    public static readonly int SPELL_INDEX_FIRE_4 = 20;
    public static readonly int SPELL_INDEX_ICE_4 = 26;
    public static readonly int SPELL_INDEX_BOLT_4 = 15;
    public static readonly int SPELL_INDEX_ELIXIR = 151;
    public static readonly int SPELL_INDEX_DEFEND = 198;

    public static readonly int SPELL_INDEX_ARMOR_BREAK = 4;
    public static readonly int SPELL_INDEX_HEAD_BREAK = 5;
    public static readonly int SPELL_INDEX_MIND_BREAK = 6;
    public static readonly int SPELL_INDEX_MAGIC_BREAK = 7;
    public static readonly int SPELL_INDEX_POWER_BREAK = 8;
    public static readonly int SPELL_INDEX_SHIELD_BREAK = 9;
    public static readonly int SPELL_INDEX_SPEED_BREAK = 10;
    public static readonly int SPELL_INDEX_WEAPON_BREAK = 11;

    public static readonly int SPELL_INDEX_ASURA = 35;
    public static readonly int SPELL_INDEX_BIZEN_BOAT = 36;
    public static readonly int SPELL_INDEX_CHIRIJIRADEN = 37;
    public static readonly int SPELL_INDEX_HEAVENS_CLOUD = 38;
    public static readonly int SPELL_INDEX_KIKUICHIMOJI = 39;
    public static readonly int SPELL_INDEX_KIYOMORI = 40;
    public static readonly int SPELL_INDEX_KOUTETSU = 41;
    public static readonly int SPELL_INDEX_MASAMUNE = 42;
    public static readonly int SPELL_INDEX_MURAMASA = 43;
    public static readonly int SPELL_INDEX_MURASAME = 44;

    public static readonly int SPELL_INDEX_ATTACK_FIST = 401;
    public static readonly int SPELL_INDEX_ATTACK_KATANA = 402;
    public static readonly int SPELL_INDEX_ATTACK_GREATSWORD = 403;
    public static readonly int SPELL_INDEX_ATTACK_DAGGER = 404;
    public static readonly int SPELL_INDEX_ATTACK_MACE = 405;
    public static readonly int SPELL_INDEX_ATTACK_BOW = 406;
    public static readonly int SPELL_INDEX_ATTACK_SWORD = 407;
    public static readonly int SPELL_INDEX_ATTACK_ROD = 408;
    public static readonly int SPELL_INDEX_ATTACK_SPEAR = 409;
    public static readonly int SPELL_INDEX_ATTACK_CROSSBOW = 410;
    public static readonly int SPELL_INDEX_ATTACK_AXE = 411;
    public static readonly int SPELL_INDEX_ATTACK_BAG = 412;
    public static readonly int SPELL_INDEX_ATTACK_HAMMER = 413;
    public static readonly int SPELL_INDEX_ATTACK_STAFF = 414;
    public static readonly int SPELL_INDEX_ATTACK_STICK = 415;
    public static readonly int SPELL_INDEX_ATTACK_GUN = 416;
    public static readonly int SPELL_INDEX_ATTACK_MAGIC_GUN = 417;
    public static readonly int SPELL_INDEX_ATTACK_CLOTH = 418;
    public static readonly int SPELL_INDEX_ATTACK_DECK = 419;
    public static readonly int SPELL_INDEX_ATTACK_INSTRUMENT = 420;
    public static readonly int SPELL_INDEX_ATTACK_WAND = 421;
    public static readonly int SPELL_INDEX_ATTACK_SCALES = 422;
    public static readonly int SPELL_INDEX_ATTACK_WHIP = 423;
    public static readonly int SPELL_INDEX_ATTACK_PISTOL = 424;


    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_FIST = 176;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_KATANA = 177;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_KNIGHT = 178;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_DAGGER = 179;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_NINJA = 180;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_LONGBOW = 181;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_SWORD = 182;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_ROD = 183;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_SPEAR = 184;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_CROSSBOW = 185;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_AXE = 186;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_BAG = 187;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_HAMMER = 188;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_STAFF = 189;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_STICK = 190;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_GUN = 191;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_MAGIC_GUN = 192;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_CLOTH = 193;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_DICTIONARY = 194;
    public static readonly int SPELL_INDEX_ATTACK_CLASSIC_HARP = 195;

    public static readonly int SPELL_INDEX_WEAK_ATTACK = 261;

    public static readonly int SPELL_INDEX_REACTION_RESTORE_MANA = 239;
    public static readonly int SPELL_INDEX_REACTION_RESTORE_HP = 240;
    public static readonly int SPELL_INDEX_REACTION_THRIFTY_HEAL = 241;
    public static readonly int SPELL_INDEX_REACTION_DOOMSAYER = 242;
    public static readonly int SPELL_INDEX_REACTION_INT_BOOST = 243;
    public static readonly int SPELL_INDEX_REACTION_ENCORE = 244;
    public static readonly int SPELL_INDEX_REACTION_BREAK_A_LEG = 245;
    public static readonly int SPELL_INDEX_REACTION_HEALING_SALVE = 246;
    public static readonly int SPELL_INDEX_REACTION_LEG_SALVE = 247;
    public static readonly int SPELL_INDEX_REACTION_REBUTTAL_CRG = 248;
    public static readonly int SPELL_INDEX_REACTION_REBUTTAL_WIS = 249;
    public static readonly int SPELL_INDEX_REACTION_REBUTTAL_SKL = 250;
    public static readonly int SPELL_INDEX_REACTION_SKL_BOOST = 251;
    public static readonly int SPELL_INDEX_REACTION_STR_BOOST = 252;
    public static readonly int SPELL_INDEX_REACTION_LAST_STAND = 253;
    public static readonly int SPELL_INDEX_REACTION_CRG_BOOST = 254;
    public static readonly int SPELL_INDEX_REACTION_RETURN_DAMAGE = 255;
    public static readonly int SPELL_INDEX_REACTION_GUARD = 256;
    public static readonly int SPELL_INDEX_REACTION_AGI_BOOST = 257;
    public static readonly int SPELL_INDEX_REACTION_SLOW_HEAL = 258;
    public static readonly int SPELL_INDEX_REACTION_SHORTCUT = 259;
    public static readonly int SPELL_INDEX_REACTION_WIS_BOOST = 260;

    public static readonly int SPELL_INDEX_MOVE_MOVE_WIS_UP = 225;
    public static readonly int SPELL_INDEX_MOVE_SAINTS_FOOTSTEPS = 226;
    public static readonly int SPELL_INDEX_MOVE_BLESSED_STEPS = 227;
    public static readonly int SPELL_INDEX_MOVE_RAISE_THE_DEAD = 228;
    public static readonly int SPELL_INDEX_MOVE_DRAW_ATTENTION = 229;
    public static readonly int SPELL_INDEX_MOVE_WALK_IT_ON = 230;
    public static readonly int SPELL_INDEX_MOVE_SILENCE_THE_CROWD = 231;
    public static readonly int SPELL_INDEX_MOVE_MP_WALK = 232;
    public static readonly int SPELL_INDEX_MOVE_MOVE_CRG_UP = 233;
    public static readonly int SPELL_INDEX_MOVE_HP_WALK = 234;
    public static readonly int SPELL_INDEX_MOVE_MOVE_SKL_UP = 235;
    public static readonly int SPELL_INDEX_MOVE_TP_WALK = 236;
    public static readonly int SPELL_INDEX_MOVE_WALK_IT_OFF = 237;
    public static readonly int SPELL_INDEX_MOVE_STRETCH_LEGS = 238;
    public static readonly int SPELL_INDEX_MOVE_UNSTABLE_TP = 434;

    public static readonly int SPELL_INDEX_HIDDEN_TRACK = 400;

    public static readonly int SPELL_TYPE_PHYSICAL = 0;
    public static readonly int SPELL_TYPE_MAGICAL = 1;
    public static readonly int SPELL_TYPE_NEUTRAL = 2;
    public static readonly int SPELL_TYPE_NULL = 3;

    public static readonly int SPELL_NO_EVASION_REFLECT = 0;
    public static readonly int SPELL_EVASION_REFLECT = 1;
    public static readonly int SPELL_EVASION = 2;
    public static readonly int SPELL_REFLECT = 3;

    public static readonly string NO = "No";
    public static readonly string YES = "Yes";

    public static readonly int SPELL_NO_CALC_MIMIC = 0;
    public static readonly int SPELL_CALC_MIMIC = 1;
    public static readonly int SPELL_CALC = 2;
    public static readonly int SPELL_MIMIC = 3;

    public static readonly int SPELL_Z_INFINITE = 1919;

    //    213	CT 3
    //214	CT 4
    //215	CT 5
    //216	CT Prime
    //217	Height 3
    //218	Height 4
    //219	Height 5
    //220	Height Prime
    //221	Level 3
    //222	Level 4
    //223	Level 5
    //224	Level Prime

    public static readonly int SPELL_INDEX_HELL_IVY = 49;
    public static readonly int SPELL_INDEX_PITFALL = 53;
    //public static readonly int MAP_TERRAIN_PITFALL = 0;
    public static readonly int MAP_TERRAIN_HELL_IVY = 0;
    //        45	45	Blizzard
    //46	46	Carve Model
    //47	47	Demon Fire
    //48	48	Gusty Wind
    //49	49	Hell Ivy
    //50	50	Kamaitachi
    //51	51	Lava Ball
    //52	52	Local Quake
    //53	53	Pitfall
    //54	54	Quicksand
    //55	55	Water Ball

    //public static readonly string STATUS_NAME_NAMELESS_DANCE = "nameless_dance";
    //public static readonly string STATUS_NAME_NAMELESS_SONG = "nameless_song";

    public static readonly int STATUS_ID_NONE = 0;
    public static readonly int STATUS_ID_BERSERK = 1;
    public static readonly int STATUS_ID_BLOOD_SUCK = 2;
    public static readonly int STATUS_ID_CHARGING = 3;
    public static readonly int STATUS_ID_CHARM = 4;
    public static readonly int STATUS_ID_CHICKEN = 5;
    public static readonly int STATUS_ID_CONFUSION = 6;
    public static readonly int STATUS_ID_CRITICAL = 7;
    public static readonly int STATUS_ID_CRYSTAL = 8;
    public static readonly int STATUS_ID_DARKNESS = 9;
    public static readonly int STATUS_ID_DEAD = 10;
    public static readonly int STATUS_ID_DEATH_SENTENCE = 11;
    public static readonly int STATUS_ID_DEFENDING = 12;
    public static readonly int STATUS_ID_DONT_ACT = 13;
    public static readonly int STATUS_ID_DONT_MOVE = 14;
    public static readonly int STATUS_ID_FAITH = 15;
    public static readonly int STATUS_ID_FLOAT = 16;
    public static readonly int STATUS_ID_FROG = 17;
    public static readonly int STATUS_ID_HASTE = 18;
    public static readonly int STATUS_ID_INNOCENT = 19;
    public static readonly int STATUS_ID_OIL = 20;
    public static readonly int STATUS_ID_PERFORMING = 21;
    public static readonly int STATUS_ID_PETRIFY = 22;
    public static readonly int STATUS_ID_PROTECT = 23;
    public static readonly int STATUS_ID_QUICK = 24;
    public static readonly int STATUS_ID_REFLECT = 25;
    public static readonly int STATUS_ID_REGEN = 26;
    public static readonly int STATUS_ID_RERAISE = 27;
    public static readonly int STATUS_ID_SHELL = 28;
    public static readonly int STATUS_ID_SILENCE = 29;
    public static readonly int STATUS_ID_SLEEP = 30;
    public static readonly int STATUS_ID_SLOW = 31;
    public static readonly int STATUS_ID_STOP = 32;
    public static readonly int STATUS_ID_UNDEAD = 33;
    public static readonly int STATUS_ID_APATHY = 34;
    public static readonly int STATUS_ID_ABSORB_AIR = 35;
    public static readonly int STATUS_ID_ABSORB_DARK = 36;
    public static readonly int STATUS_ID_ABSORB_EARTH = 37;
    public static readonly int STATUS_ID_ABSORB_FIRE = 38;
    public static readonly int STATUS_ID_ABSORB_LIGHT = 39;
    public static readonly int STATUS_ID_ABSORB_LIGHTNING = 40;
    public static readonly int STATUS_ID_ABSORB_UNDEAD = 41;
    public static readonly int STATUS_ID_ABSORB_WATER = 42;
    public static readonly int STATUS_ID_ABSORB_WEAPON = 43;
    public static readonly int STATUS_ID_ABSORB_WIND = 44;
    public static readonly int STATUS_ID_HALF_AIR = 45;
    public static readonly int STATUS_ID_HALF_DARK = 46;
    public static readonly int STATUS_ID_HALF_EARTH = 47;
    public static readonly int STATUS_ID_HALF_FIRE = 48;
    public static readonly int STATUS_ID_HALF_LIGHT = 49;
    public static readonly int STATUS_ID_HALF_LIGHTNING = 50;
    public static readonly int STATUS_ID_HALF_UNDEAD = 51;
    public static readonly int STATUS_ID_HALF_WATER = 52;
    public static readonly int STATUS_ID_HALF_WEAPON = 53;
    public static readonly int STATUS_ID_HALF_WIND = 54;
    public static readonly int STATUS_ID_WEAK_AIR = 55;
    public static readonly int STATUS_ID_WEAK_DARK = 56;
    public static readonly int STATUS_ID_WEAK_EARTH = 57;
    public static readonly int STATUS_ID_WEAK_FIRE = 58;
    public static readonly int STATUS_ID_WEAK_LIGHT = 59;
    public static readonly int STATUS_ID_WEAK_LIGHTNING = 60;
    public static readonly int STATUS_ID_WEAK_UNDEAD = 61;
    public static readonly int STATUS_ID_WEAK_WATER = 62;
    public static readonly int STATUS_ID_WEAK_WEAPON = 63;
    public static readonly int STATUS_ID_WEAK_WIND = 64;
    public static readonly int STATUS_ID_STRENGTHEN_AIR = 65;
    public static readonly int STATUS_ID_STRENGTHEN_DARK = 66;
    public static readonly int STATUS_ID_STRENGTHEN_EARTH = 67;
    public static readonly int STATUS_ID_STRENGTHEN_FIRE = 68;
    public static readonly int STATUS_ID_STRENGTHEN_LIGHT = 69;
    public static readonly int STATUS_ID_STRENGTHEN_LIGHTNING = 70;
    public static readonly int STATUS_ID_STRENGTHEN_UNDEAD = 71;
    public static readonly int STATUS_ID_STRENGTHEN_WATER = 72;
    public static readonly int STATUS_ID_STRENGTHEN_WEAPON = 73;
    public static readonly int STATUS_ID_STRENGTHEN_WIND = 74;
    public static readonly int STATUS_ID_STRENGTHEN_ALL = 75;
    public static readonly int STATUS_ID_AUTO_FLOAT_REFLECT = 76;
    public static readonly int STATUS_ID_AUTO_PROTECT_SHELL = 77;
    public static readonly int STATUS_ID_AUTO_REGEN_RERAISE = 78;
    public static readonly int STATUS_ID_AUTO_HASTE = 79;
    //public static readonly int STATUS_ID_REFLECT_ITEM = 80;
    public static readonly int STATUS_ID_RERAISE_START = 81;
    public static readonly int STATUS_ID_AUTO_UNDEAD = 82;
    public static readonly int STATUS_ID_AUTO_FLOAT = 83;
    public static readonly int STATUS_ID_AUTO_REFLECT = 84;
    public static readonly int STATUS_ID_ABSORB_EARTH_STRENGTHEN_EARTH = 85;
    //public static readonly int STATUS_ID_ABSORB_HOLY = 86;
    public static readonly int STATUS_ID_HALF_FIRE_LIGHTNING_ICE = 87;
    public static readonly int STATUS_ID_STRENGTHEN_FIRE_LIGHTNING_ICE = 88;
    public static readonly int STATUS_ID_AUTO_REGEN = 89;
    public static readonly int STATUS_ID_START_PETRIFY = 90;
    public static readonly int STATUS_ID_EXCALIBUR = 91;
    public static readonly int STATUS_ID_AUTO_PROTECT = 92;
    public static readonly int STATUS_ID_AUTO_SHELL = 93;
    public static readonly int STATUS_ID_AUTO_FAITH = 94;
    public static readonly int STATUS_ID_ABSORB_FIRE_HALF_ICE_WEAK_WATER = 95;
    public static readonly int STATUS_ID_ABSORB_ICE_HALF_FIRE_WEAK_LIGHTNING = 96;
    public static readonly int STATUS_ID_STRENGTHEN_ICE = 97;
    public static readonly int STATUS_ID_ABSORB_ICE = 98;
    public static readonly int STATUS_ID_HALF_ICE = 99;
    public static readonly int STATUS_ID_WEAK_ICE = 100;
    public static readonly int STATUS_ID_DEAD_3 = 101;
    public static readonly int STATUS_ID_DEAD_2 = 102;
    public static readonly int STATUS_ID_DEAD_1 = 103;
    public static readonly int STATUS_ID_DEAD_0 = 104;
    public static readonly int STATUS_ID_DEATH_SENTENCE_3 = 105;
    public static readonly int STATUS_ID_DEATH_SENTENCE_2 = 106;
    public static readonly int STATUS_ID_DEATH_SENTENCE_1 = 107;
    public static readonly int STATUS_ID_DEATH_SENTENCE_0 = 108;
    public static readonly int STATUS_ID_ALL = 109;
    public static readonly int STATUS_ID_JUMPING = 110;
    public static readonly int STATUS_ID_LIFE = 111;
    public static readonly int STATUS_ID_CANCEL_ESUNA = 112;
    public static readonly int STATUS_ID_CANCEL_PETRIFY = 113;
    public static readonly int STATUS_ID_CANCEL_FROG = 114;
    public static readonly int STATUS_ID_CANCEL_STIGMA_MAGIC = 115;
    public static readonly int STATUS_ID_FLOAT_MOVE = 116;
    public static readonly int STATUS_ID_INVITE = 117;
    public static readonly int STATUS_ID_GOLEM = 118;
    public static readonly int STATUS_ID_POISON = 119;
    public static readonly int STATUS_ID_AUTO_SLOW = 120;
    public static readonly int STATUS_ID_AUTO_INNOCENT = 121;
    public static readonly int STATUS_ID_CANCEL_HEAL = 122;
    public static readonly int STATUS_ID_CANCEL_DISPEL = 123;
    public static readonly int STATUS_ID_BLOCK_SLOW = 124;
    public static readonly int STATUS_ID_BLOCK_CONFUSION_CHARM = 125;
    public static readonly int STATUS_ID_BLOCK_DONT_MOVE_DONT_ACT = 126;
    public static readonly int STATUS_ID_BLOCK_PETRIFY_STOP = 127;
    public static readonly int STATUS_ID_BLOCK_UNDEAD_BLOOD_SUCK = 128;
    public static readonly int STATUS_ID_BLOCK_SILENCE_BERSERK = 129;
    public static readonly int STATUS_ID_BLOCK_SLEEP_DEATH_SENTENCE = 130;
    public static readonly int STATUS_ID_BLOCK_DEAD_DARKNESS = 131;
    public static readonly int STATUS_ID_BLOCK_INVITE = 132;
    public static readonly int STATUS_ID_BLOCK_DONT_MOVE_LIGHTNING = 133;
    public static readonly int STATUS_ID_BLOCK_LIGHTNING = 134;
    public static readonly int STATUS_ID_BLOCK_STOP = 135;
    public static readonly int STATUS_ID_BLOCK_DEAD = 136;
    public static readonly int STATUS_ID_BLOCK_SILENCE = 137;
    public static readonly int STATUS_ID_BLOCK_DARKNESS_SLEEP = 138;
    public static readonly int STATUS_ID_BLOCK_CACHUSA = 139;
    public static readonly int STATUS_ID_BLOCK_RIBBON = 140;
    public static readonly int STATUS_ID_BLOCK_BARETTE = 141;
    public static readonly int STATUS_ID_BLOCK_BERSERK = 142;
    public static readonly int STATUS_ID_BLOCK_BLOOD_SUCK = 143;
    public static readonly int STATUS_ID_BLOCK_CHARGING = 144;
    public static readonly int STATUS_ID_BLOCK_CHARM = 145;
    public static readonly int STATUS_ID_BLOCK_CHICKEN = 146;
    public static readonly int STATUS_ID_BLOCK_CONFUSION = 147;
    public static readonly int STATUS_ID_BLOCK_CRITICAL = 148;
    public static readonly int STATUS_ID_BLOCK_CRYSTAL = 149;
    public static readonly int STATUS_ID_BLOCK_DARKNESS = 150;
    public static readonly int STATUS_ID_BLOCK_DEATH_SENTENCE = 151;
    public static readonly int STATUS_ID_BLOCK_DEFENDING = 152;
    public static readonly int STATUS_ID_BLOCK_DONT_ACT = 153;
    public static readonly int STATUS_ID_BLOCK_DONT_MOVE = 154;
    public static readonly int STATUS_ID_BLOCK_FAITH = 155;
    public static readonly int STATUS_ID_BLOCK_FLOAT = 156;
    public static readonly int STATUS_ID_BLOCK_FROG = 157;
    public static readonly int STATUS_ID_BLOCK_HASTE = 158;
    public static readonly int STATUS_ID_BLOCK_INNOCENT = 159;
    public static readonly int STATUS_ID_BLOCK_OIL = 160;
    public static readonly int STATUS_ID_BLOCK_PERFORMING = 161;
    public static readonly int STATUS_ID_BLOCK_PETRIFY = 162;
    public static readonly int STATUS_ID_BLOCK_PROTECT = 163;
    public static readonly int STATUS_ID_BLOCK_QUICK = 164;
    public static readonly int STATUS_ID_BLOCK_REFLECT = 165;
    public static readonly int STATUS_ID_BLOCK_REGEN = 166;
    public static readonly int STATUS_ID_BLOCK_RERAISE = 167;
    public static readonly int STATUS_ID_BLOCK_SHELL = 168;
    public static readonly int STATUS_ID_BLOCK_SLEEP = 169;
    public static readonly int STATUS_ID_BLOCK_UNDEAD = 170;
    public static readonly int STATUS_ID_BLOCK_APATHY = 171;
    public static readonly int STATUS_ID_BLOCK_POISON = 172;
    public static readonly int STATUS_ID_BLOCK_NECKLACE_1 = 173;
    public static readonly int STATUS_ID_BLOCK_NECKLACE_2 = 174;
    public static readonly int STATUS_ID_BLOCK_NECKLACE_3 = 175;
    public static readonly int STATUS_ID_BLOCK_NECKLACE_4 = 176;
    public static readonly int STATUS_ID_BLOCK_NECKLACE_5 = 177;
    public static readonly int STATUS_ID_BLOCK_NECKLACE_6 = 178;
    public static readonly int STATUS_ID_CANCEL_OCTAGON = 179;
    public static readonly int STATUS_ID_CANCEL_DEATH_SENTENCE = 180;
    public static readonly int STATUS_ID_NAMELESS_DANCE = 181;
    public static readonly int STATUS_ID_PROTECT_SHELL = 182;
    public static readonly int STATUS_ID_REGEN_HASTE = 183;
    public static readonly int STATUS_ID_NAMELESS_SONG = 184;
    public static readonly int STATUS_ID_CANCEL_POISON = 185;
    public static readonly int STATUS_ID_CANCEL_DARKNESS = 186;
    public static readonly int STATUS_ID_CANCEL_SILENCE = 187;
    public static readonly int STATUS_ID_CANCEL_HOLY_WATER = 188;
    public static readonly int STATUS_ID_CANCEL_REMEDY = 189;
    public static readonly int STATUS_ID_CANCEL_NEGATIVE = 190;
    public static readonly int STATUS_ID_ZOMBIE_LIFE = 191;
    public static readonly int STATUS_ID_ADD_POSITIVE = 192;
    public static readonly int STATUS_ID_OIL_DEAD = 193;
    public static readonly int STATUS_ID_OIL_FAITH = 194;
    public static readonly int STATUS_ID_CURE_STATUS_1 = 195;
    public static readonly int STATUS_ID_HASTE_DS = 196;
    public static readonly int STATUS_ID_CURE_STATUS_2 = 197;
    public static readonly int STATUS_ID_BERSERK_START = 198;
    public static readonly int STATUS_ID_UNCONSCIOUS = 199; //death except no crystal or permadeath

    public static readonly string STATUS_NAME_NONE = "none";
    public static readonly string STATUS_NAME_BERSERK = "berserk";
    public static readonly string STATUS_NAME_BLOOD_SUCK = "blood suck";
    public static readonly string STATUS_NAME_CHARGING = "charging";
    public static readonly string STATUS_NAME_CHARM = "charm";
    public static readonly string STATUS_NAME_CHICKEN = "chicken";
    public static readonly string STATUS_NAME_CONFUSION = "confusion";
    public static readonly string STATUS_NAME_CRITICAL = "critical";
    public static readonly string STATUS_NAME_CRYSTAL = "crystal";
    public static readonly string STATUS_NAME_DARKNESS = "darkness";
    public static readonly string STATUS_NAME_DEAD = "dead";
    public static readonly string STATUS_NAME_DEATH_SENTENCE = "death sentence";
    public static readonly string STATUS_NAME_DEFENDING = "defending";
    public static readonly string STATUS_NAME_DONT_ACT = "can't act";
    public static readonly string STATUS_NAME_DONT_MOVE = "can't move";
    public static readonly string STATUS_NAME_FAITH = "faith";
    public static readonly string STATUS_NAME_FLOAT = "float";
    public static readonly string STATUS_NAME_FROG = "frog";
    public static readonly string STATUS_NAME_HASTE = "haste";
    public static readonly string STATUS_NAME_INNOCENT = "innocent";
    public static readonly string STATUS_NAME_OIL = "oil";
    public static readonly string STATUS_NAME_PERFORMING = "performing";
    public static readonly string STATUS_NAME_PETRIFY = "petrify";
    public static readonly string STATUS_NAME_PROTECT = "protect";
    public static readonly string STATUS_NAME_QUICK = "quick";
    public static readonly string STATUS_NAME_REFLECT = "reflect";
    public static readonly string STATUS_NAME_REGEN = "regen";
    public static readonly string STATUS_NAME_RERAISE = "reraise";
    public static readonly string STATUS_NAME_SHELL = "shell";
    public static readonly string STATUS_NAME_SILENCE = "silence";
    public static readonly string STATUS_NAME_SLEEP = "sleep";
    public static readonly string STATUS_NAME_SLOW = "slow";
    public static readonly string STATUS_NAME_STOP = "stop";
    public static readonly string STATUS_NAME_UNDEAD = "undead";
    public static readonly string STATUS_NAME_APATHY = "apathy";
    public static readonly string STATUS_NAME_ABSORB_AIR = "absorb air";
    public static readonly string STATUS_NAME_ABSORB_DARK = "absorb dark";
    public static readonly string STATUS_NAME_ABSORB_EARTH = "absorb earth";
    public static readonly string STATUS_NAME_ABSORB_FIRE = "absorb fire";
    public static readonly string STATUS_NAME_ABSORB_LIGHT = "absorb holy";
    public static readonly string STATUS_NAME_ABSORB_LIGHTNING = "absorb lightning";
    public static readonly string STATUS_NAME_ABSORB_UNDEAD = "absorb undead";
    public static readonly string STATUS_NAME_ABSORB_WATER = "absorb water";
    public static readonly string STATUS_NAME_ABSORB_WEAPON = "absorb weapon";
    public static readonly string STATUS_NAME_ABSORB_WIND = "absorb wind";
    public static readonly string STATUS_NAME_HALF_AIR = "half dmg air";
    public static readonly string STATUS_NAME_HALF_DARK = "half dmg dark";
    public static readonly string STATUS_NAME_HALF_EARTH = "half dmg earth";
    public static readonly string STATUS_NAME_HALF_FIRE = "half dmg fire";
    public static readonly string STATUS_NAME_HALF_LIGHT = "half dmg holy";
    public static readonly string STATUS_NAME_HALF_LIGHTNING = "half dmg lightning";
    public static readonly string STATUS_NAME_HALF_UNDEAD = "half dmg undead";
    public static readonly string STATUS_NAME_HALF_WATER = "half dmg water";
    public static readonly string STATUS_NAME_HALF_WEAPON = "half dmg weapon";
    public static readonly string STATUS_NAME_HALF_WIND = "half dmg wind";
    public static readonly string STATUS_NAME_WEAK_AIR = "weak to air";
    public static readonly string STATUS_NAME_WEAK_DARK = "weak to dark";
    public static readonly string STATUS_NAME_WEAK_EARTH = "weak to earth";
    public static readonly string STATUS_NAME_WEAK_FIRE = "weak to fire";
    public static readonly string STATUS_NAME_WEAK_LIGHT = "weak to light";
    public static readonly string STATUS_NAME_WEAK_LIGHTNING = "weak to lightning";
    public static readonly string STATUS_NAME_WEAK_UNDEAD = "weak to undead";
    public static readonly string STATUS_NAME_WEAK_WATER = "weak to water";
    public static readonly string STATUS_NAME_WEAK_WEAPON = "weak to weapon";
    public static readonly string STATUS_NAME_WEAK_WIND = "weak to wind";
    public static readonly string STATUS_NAME_STRENGTHEN_AIR = "strengthen air";
    public static readonly string STATUS_NAME_STRENGTHEN_DARK = "strengthen dark";
    public static readonly string STATUS_NAME_STRENGTHEN_EARTH = "strengthen earth";
    public static readonly string STATUS_NAME_STRENGTHEN_FIRE = "strengthen fire";
    public static readonly string STATUS_NAME_STRENGTHEN_LIGHT = "strengthen holy";
    public static readonly string STATUS_NAME_STRENGTHEN_LIGHTNING = "strengthen lightning";
    public static readonly string STATUS_NAME_STRENGTHEN_UNDEAD = "strengthen undead";
    public static readonly string STATUS_NAME_STRENGTHEN_WATER = "strengthen water";
    public static readonly string STATUS_NAME_STRENGTHEN_WEAPON = "strengthen weapon";
    public static readonly string STATUS_NAME_STRENGTHEN_WIND = "strengthen wind";
    public static readonly string STATUS_NAME_STRENGTHEN_ALL = "strengthen all";
    public static readonly string STATUS_NAME_AUTO_FLOAT_REFLECT = "auto float reflect";
    public static readonly string STATUS_NAME_AUTO_PROTECT_SHELL = "auto protect shell";
    public static readonly string STATUS_NAME_AUTO_REGEN_RERAISE = "auto regen reraise";
    public static readonly string STATUS_NAME_AUTO_HASTE = "auto haste";
    //public static readonly string STATUS_NAME_REFLECT_ITEM = "";
    public static readonly string STATUS_NAME_RERAISE_START = "reraise start";
    public static readonly string STATUS_NAME_AUTO_UNDEAD = "auto undead";
    public static readonly string STATUS_NAME_AUTO_FLOAT = "auto float";
    public static readonly string STATUS_NAME_AUTO_REFLECT = "auto reflect";
    public static readonly string STATUS_NAME_ABSORB_EARTH_STRENGTHEN_EARTH = "absorb earth, strengthen earth";
    //public static readonly string STATUS_NAME_ABSORB_HOLY = "";
    public static readonly string STATUS_NAME_HALF_FIRE_LIGHTNING_ICE = "half dmg fire, lightning, ice";
    public static readonly string STATUS_NAME_STRENGTHEN_FIRE_LIGHTNING_ICE = "strengthen fire, lightning, ice";
    public static readonly string STATUS_NAME_AUTO_REGEN = "auto regen";
    public static readonly string STATUS_NAME_START_PETRIFY = "petrify start";
    public static readonly string STATUS_NAME_EXCALIBUR = "auto haste, absorb holy";
    public static readonly string STATUS_NAME_AUTO_PROTECT = "auto protect";
    public static readonly string STATUS_NAME_AUTO_SHELL = "auto shell";
    public static readonly string STATUS_NAME_AUTO_FAITH = "auto faith";
    public static readonly string STATUS_NAME_ABSORB_FIRE_HALF_ICE_WEAK_WATER = "absorb fire, half ice, weak to water";
    public static readonly string STATUS_NAME_ABSORB_ICE_HALF_FIRE_WEAK_LIGHTNING = "absorb ice, half fire, weak to lightning";
    public static readonly string STATUS_NAME_STRENGTHEN_ICE = "strengthen ice";
    public static readonly string STATUS_NAME_ABSORB_ICE = "absorb ice";
    public static readonly string STATUS_NAME_HALF_ICE = "half ice";
    public static readonly string STATUS_NAME_WEAK_ICE = "weak to ice";
    public static readonly string STATUS_NAME_DEAD_3 = "dead 3";
    public static readonly string STATUS_NAME_DEAD_2 = "dead 2";
    public static readonly string STATUS_NAME_DEAD_1 = "dead 1";
    public static readonly string STATUS_NAME_DEAD_0 = "dead 0";
    public static readonly string STATUS_NAME_DEATH_SENTENCE_3 = "death sentence 3";
    public static readonly string STATUS_NAME_DEATH_SENTENCE_2 = "death sentence 2";
    public static readonly string STATUS_NAME_DEATH_SENTENCE_1 = "death sentence 1";
    public static readonly string STATUS_NAME_DEATH_SENTENCE_0 = "death sentence 0";
    //public static readonly string STATUS_NAME_ALL = "all";
    public static readonly string STATUS_NAME_JUMPING = "jumping";
    public static readonly string STATUS_NAME_LIFE = "life";
    public static readonly string STATUS_NAME_CANCEL_ESUNA = "cancel esuna";
    public static readonly string STATUS_NAME_CANCEL_PETRIFY = "cancel petrify";
    public static readonly string STATUS_NAME_CANCEL_FROG = "cancel frog";
    public static readonly string STATUS_NAME_CANCEL_STIGMA_MAGIC = "cancel stigma magic";
    public static readonly string STATUS_NAME_FLOAT_MOVE = "float move";
    public static readonly string STATUS_NAME_INVITE = "invite";
    public static readonly string STATUS_NAME_GOLEM = "golem";
    public static readonly string STATUS_NAME_POISON = "poison";
    public static readonly string STATUS_NAME_AUTO_SLOW = "auto slow";
    public static readonly string STATUS_NAME_AUTO_INNOCENT = "auto innocent";
    public static readonly string STATUS_NAME_CANCEL_HEAL = "cancel heal";
    public static readonly string STATUS_NAME_CANCEL_DISPEL = "cancel dispel";
    public static readonly string STATUS_NAME_BLOCK_SLOW = "block slow";
    public static readonly string STATUS_NAME_BLOCK_CONFUSION_CHARM = "block confusion, charm";
    public static readonly string STATUS_NAME_BLOCK_DONT_MOVE_DONT_ACT = "block can't move, can't act";
    public static readonly string STATUS_NAME_BLOCK_PETRIFY_STOP = "block petrify, stop";
    public static readonly string STATUS_NAME_BLOCK_UNDEAD_BLOOD_SUCK = "block undead, blood suck";
    public static readonly string STATUS_NAME_BLOCK_SILENCE_BERSERK = "block berserk";
    public static readonly string STATUS_NAME_BLOCK_SLEEP_DEATH_SENTENCE = "block sleep, death sentence";
    public static readonly string STATUS_NAME_BLOCK_DEAD_DARKNESS = "block dead, darkness";
    public static readonly string STATUS_NAME_BLOCK_INVITE = "block invite";
    public static readonly string STATUS_NAME_BLOCK_DONT_MOVE_LIGHTNING = "block can't move, lightning";
    public static readonly string STATUS_NAME_BLOCK_LIGHTNING = "block lightning";
    public static readonly string STATUS_NAME_BLOCK_STOP = "block stop";
    public static readonly string STATUS_NAME_BLOCK_DEAD = "block dead";
    public static readonly string STATUS_NAME_BLOCK_SILENCE = "block silence";
    public static readonly string STATUS_NAME_BLOCK_DARKNESS_SLEEP = "block darkness, slee";
    public static readonly string STATUS_NAME_BLOCK_CACHUSA = "block cachusa";
    public static readonly string STATUS_NAME_BLOCK_RIBBON = "block ribbon";
    public static readonly string STATUS_NAME_BLOCK_BARETTE = "block barette";
    public static readonly string STATUS_NAME_BLOCK_BERSERK = "block berserk";
    public static readonly string STATUS_NAME_BLOCK_BLOOD_SUCK = "block blood suck";
    public static readonly string STATUS_NAME_BLOCK_CHARGING = "block charging";
    public static readonly string STATUS_NAME_BLOCK_CHARM = "block charm";
    public static readonly string STATUS_NAME_BLOCK_CHICKEN = "block chicken";
    public static readonly string STATUS_NAME_BLOCK_CONFUSION = "block confusion";
    public static readonly string STATUS_NAME_BLOCK_CRITICAL = "block critical";
    public static readonly string STATUS_NAME_BLOCK_CRYSTAL = "block crystal";
    public static readonly string STATUS_NAME_BLOCK_DARKNESS = "block darkness";
    public static readonly string STATUS_NAME_BLOCK_DEATH_SENTENCE = "block death sentence";
    public static readonly string STATUS_NAME_BLOCK_DEFENDING = "block defending";
    public static readonly string STATUS_NAME_BLOCK_DONT_ACT = "block can't act";
    public static readonly string STATUS_NAME_BLOCK_DONT_MOVE = "block can't move";
    public static readonly string STATUS_NAME_BLOCK_FAITH = "block faith";
    public static readonly string STATUS_NAME_BLOCK_FLOAT = "block float";
    public static readonly string STATUS_NAME_BLOCK_FROG = "block frog";
    public static readonly string STATUS_NAME_BLOCK_HASTE = "block haste";
    public static readonly string STATUS_NAME_BLOCK_INNOCENT = "block innocent";
    public static readonly string STATUS_NAME_BLOCK_OIL = "block oil";
    public static readonly string STATUS_NAME_BLOCK_PERFORMING = "block performing";
    public static readonly string STATUS_NAME_BLOCK_PETRIFY = "block petrify";
    public static readonly string STATUS_NAME_BLOCK_PROTECT = "block protect";
    public static readonly string STATUS_NAME_BLOCK_QUICK = "block quick";
    public static readonly string STATUS_NAME_BLOCK_REFLECT = "block reflect";
    public static readonly string STATUS_NAME_BLOCK_REGEN = "block regen";
    public static readonly string STATUS_NAME_BLOCK_RERAISE = "block reraise";
    public static readonly string STATUS_NAME_BLOCK_SHELL = "block shell";
    public static readonly string STATUS_NAME_BLOCK_SLEEP = "block sleep";
    public static readonly string STATUS_NAME_BLOCK_UNDEAD = "block undead";
    public static readonly string STATUS_NAME_BLOCK_APATHY = "block apathy";
    public static readonly string STATUS_NAME_BLOCK_POISON = "block poison";
    public static readonly string STATUS_NAME_BLOCK_NECKLACE_1 = "block necklace 1";
    public static readonly string STATUS_NAME_BLOCK_NECKLACE_2 = "block necklace 2";
    public static readonly string STATUS_NAME_BLOCK_NECKLACE_3 = "block necklace 3";
    public static readonly string STATUS_NAME_BLOCK_NECKLACE_4 = "block necklace 4";
    public static readonly string STATUS_NAME_BLOCK_NECKLACE_5 = "block necklace 5";
    public static readonly string STATUS_NAME_BLOCK_NECKLACE_6 = "block necklace 6";
    public static readonly string STATUS_NAME_CANCEL_OCTAGON = "cancel octagon";
    public static readonly string STATUS_NAME_CANCEL_DEATH_SENTENCE = "cancel death sentence";
    public static readonly string STATUS_NAME_NAMELESS_DANCE = "nameless dance";
    public static readonly string STATUS_NAME_PROTECT_SHELL = "protect, shell";
    public static readonly string STATUS_NAME_REGEN_HASTE = "regen, haste";
    public static readonly string STATUS_NAME_NAMELESS_SONG = "nameless song";
    public static readonly string STATUS_NAME_CANCEL_POISON = "cancel poison";
    public static readonly string STATUS_NAME_CANCEL_DARKNESS = "cancel darkness";
    public static readonly string STATUS_NAME_CANCEL_SILENCE = "cancel silence";
    public static readonly string STATUS_NAME_CANCEL_HOLY_WATER = "cancel holy water";
    public static readonly string STATUS_NAME_CANCEL_REMEDY = "cancel remedy";
    public static readonly string STATUS_NAME_CANCEL_NEGATIVE = "cancel negative";
    public static readonly string STATUS_NAME_ZOMBIE_LIFE = "zombie life";
    public static readonly string STATUS_NAME_ADD_POSITIVE = "add positive";
    public static readonly string STATUS_NAME_OIL_DEAD = "oil, dead";
    public static readonly string STATUS_NAME_OIL_FAITH = "oil, faith";
    public static readonly string STATUS_NAME_CURE_STATUS_1 = "cure status 1";
    public static readonly string STATUS_NAME_HASTE_DS = "haste, death sentence";
    public static readonly string STATUS_NAME_CURE_STATUS_2 = "cure status 2";
    public static readonly string STATUS_NAME_BERSERK_START = "berserk start";
    public static readonly string STATUS_NAME_UNCONSCIOUS = "unconscious";

    public static Dictionary<int, string> GetStatusDict()
    {
        Dictionary<int, string> retValue = new Dictionary<int, string>();
        retValue.Add(NameAll.STATUS_ID_NONE, NameAll.STATUS_NAME_NONE);
        retValue.Add(NameAll.STATUS_ID_ADD_POSITIVE, NameAll.STATUS_NAME_ADD_POSITIVE);
        retValue.Add(NameAll.STATUS_ID_APATHY, NameAll.STATUS_NAME_APATHY);
        retValue.Add(NameAll.STATUS_ID_BERSERK, NameAll.STATUS_NAME_BERSERK);
        retValue.Add(NameAll.STATUS_ID_BLOOD_SUCK, NameAll.STATUS_NAME_BLOOD_SUCK);
        retValue.Add(NameAll.STATUS_ID_CHARGING, NameAll.STATUS_NAME_CHARGING);
        retValue.Add(NameAll.STATUS_ID_CHARM, NameAll.STATUS_NAME_CHARM);
        retValue.Add(NameAll.STATUS_ID_CHICKEN, NameAll.STATUS_NAME_CHICKEN);
        retValue.Add(NameAll.STATUS_ID_CONFUSION, NameAll.STATUS_NAME_CONFUSION);
        retValue.Add(NameAll.STATUS_ID_CRITICAL, NameAll.STATUS_NAME_CRITICAL);
        retValue.Add(NameAll.STATUS_ID_CRYSTAL, NameAll.STATUS_NAME_CRYSTAL);
        retValue.Add(NameAll.STATUS_ID_DARKNESS, NameAll.STATUS_NAME_DARKNESS);
        retValue.Add(NameAll.STATUS_ID_DEAD, NameAll.STATUS_NAME_DEAD);
        retValue.Add(NameAll.STATUS_ID_DEATH_SENTENCE, NameAll.STATUS_NAME_DEATH_SENTENCE);
        retValue.Add(NameAll.STATUS_ID_DEFENDING, NameAll.STATUS_NAME_DEFENDING);
        retValue.Add(NameAll.STATUS_ID_DONT_ACT, NameAll.STATUS_NAME_DONT_ACT);
        retValue.Add(NameAll.STATUS_ID_DONT_MOVE, NameAll.STATUS_NAME_DONT_MOVE);
        retValue.Add(NameAll.STATUS_ID_FAITH, NameAll.STATUS_NAME_FAITH);
        retValue.Add(NameAll.STATUS_ID_FLOAT, NameAll.STATUS_NAME_FLOAT);
        retValue.Add(NameAll.STATUS_ID_FROG, NameAll.STATUS_NAME_FROG);
        retValue.Add(NameAll.STATUS_ID_HASTE, NameAll.STATUS_NAME_HASTE);
        retValue.Add(NameAll.STATUS_ID_HASTE_DS, NameAll.STATUS_NAME_HASTE_DS);
        retValue.Add(NameAll.STATUS_ID_INNOCENT, NameAll.STATUS_NAME_INNOCENT);
        retValue.Add(NameAll.STATUS_ID_INVITE, NameAll.STATUS_NAME_INVITE);
        retValue.Add(NameAll.STATUS_ID_LIFE, NameAll.STATUS_NAME_LIFE);
        retValue.Add(NameAll.STATUS_ID_NAMELESS_DANCE, NameAll.STATUS_NAME_NAMELESS_DANCE);
        retValue.Add(NameAll.STATUS_ID_NAMELESS_SONG, NameAll.STATUS_NAME_NAMELESS_SONG);
        retValue.Add(NameAll.STATUS_ID_OIL, NameAll.STATUS_NAME_OIL);
        retValue.Add(NameAll.STATUS_ID_OIL_DEAD, NameAll.STATUS_NAME_OIL_DEAD);
        retValue.Add(NameAll.STATUS_ID_OIL_FAITH, NameAll.STATUS_NAME_OIL_FAITH);
        retValue.Add(NameAll.STATUS_ID_PERFORMING, NameAll.STATUS_NAME_PERFORMING);
        retValue.Add(NameAll.STATUS_ID_PETRIFY, NameAll.STATUS_NAME_PETRIFY);
        retValue.Add(NameAll.STATUS_ID_POISON, NameAll.STATUS_NAME_POISON);
        retValue.Add(NameAll.STATUS_ID_PROTECT, NameAll.STATUS_NAME_PROTECT);
        retValue.Add(NameAll.STATUS_ID_PROTECT_SHELL, NameAll.STATUS_NAME_PROTECT_SHELL);
        retValue.Add(NameAll.STATUS_ID_QUICK, NameAll.STATUS_NAME_QUICK);
        retValue.Add(NameAll.STATUS_ID_REFLECT, NameAll.STATUS_NAME_REFLECT);
        retValue.Add(NameAll.STATUS_ID_REGEN, NameAll.STATUS_NAME_REGEN);
        retValue.Add(NameAll.STATUS_ID_REGEN_HASTE, NameAll.STATUS_NAME_REGEN_HASTE);
        retValue.Add(NameAll.STATUS_ID_RERAISE, NameAll.STATUS_NAME_RERAISE);
        retValue.Add(NameAll.STATUS_ID_SHELL, NameAll.STATUS_NAME_SHELL);
        retValue.Add(NameAll.STATUS_ID_SILENCE, NameAll.STATUS_NAME_SILENCE);
        retValue.Add(NameAll.STATUS_ID_SLEEP, NameAll.STATUS_NAME_SLEEP);
        retValue.Add(NameAll.STATUS_ID_SLOW, NameAll.STATUS_NAME_SLOW);
        retValue.Add(NameAll.STATUS_ID_STOP, NameAll.STATUS_NAME_STOP);
        retValue.Add(NameAll.STATUS_ID_UNDEAD, NameAll.STATUS_NAME_UNDEAD);
        retValue.Add(NameAll.STATUS_ID_ZOMBIE_LIFE, NameAll.STATUS_NAME_ZOMBIE_LIFE);
        retValue.Add(NameAll.STATUS_ID_UNCONSCIOUS, NameAll.STATUS_NAME_UNCONSCIOUS);
        return retValue;
    }

    public static readonly int AI_LIST_HURT_ENEMY = 0;
    public static readonly int AI_LIST_DEAD_ALLY = 1;
    public static readonly int AI_LIST_HURT_ALLY = 2;

    public static readonly int LOOP_PHASE_END_ACTIVE_TURN = 47;

    public static readonly int NO_EQUIP = 0;
    public static readonly int FIST_EQUIP = 234;

    public static readonly int CLASS_CHEMIST = 1;
    public static readonly int CLASS_KNIGHT = 2;
    public static readonly int CLASS_ARCHER = 3;
    public static readonly int CLASS_SQUIRE = 4;
    public static readonly int CLASS_THIEF = 5;
    public static readonly int CLASS_NINJA = 6;
    public static readonly int CLASS_MONK = 7;
    public static readonly int CLASS_PRIEST = 8;
    public static readonly int CLASS_WIZARD = 9;
    public static readonly int CLASS_TIME_MAGE = 10;
    public static readonly int CLASS_SUMMONER = 11;
    public static readonly int CLASS_MEDIATOR = 12;
    public static readonly int CLASS_ORACLE = 13;
    public static readonly int CLASS_GEOMANCER = 14;
    public static readonly int CLASS_LANCER = 15;
    public static readonly int CLASS_SAMURAI = 16;
    public static readonly int CLASS_CALCULATOR = 17;
    public static readonly int CLASS_BARD = 18;
    public static readonly int CLASS_DANCER = 19;
    public static readonly int CLASS_MIME = 20;

    public static readonly int CLASS_FIRE_MAGE = 100;
    public static readonly int CLASS_HEALER = 101;
    public static readonly int CLASS_NECROMANCER = 102;
    public static readonly int CLASS_ARTIST = 103;
    public static readonly int CLASS_APOTHECARY = 104;
    public static readonly int CLASS_DEMAGOGUE = 105;
    public static readonly int CLASS_BRAWLER = 106;
    public static readonly int CLASS_WARRIOR = 107;
    public static readonly int CLASS_CENTURION = 108;
    public static readonly int CLASS_ROGUE = 109;
    public static readonly int CLASS_RANGER = 110;
    public static readonly int CLASS_DRUID = 111;

	/// <summary>
	/// returns the string needed to instantiate the GameObject for the PlayerUnitObject
	/// for now just returning a default thing but when new stuff is created can actually custom load
	/// </summary>
	/// <param name="classId"></param>
	/// <returns></returns>
    public static string GetPUOString( int classId)
    {

        if( classId >= NameAll.CUSTOM_CLASS_ID_START_VALUE)
        {
			//return "Heroes/default_puo";
			return GetIconStringFromClass(classId);
		}
		return "Heroes/default_puo";
        //string zString = "Heroes/box_man";
        //if (classId >= CLASS_FIRE_MAGE)
        //{
        //    if( classId == CLASS_FIRE_MAGE)
        //    {
        //        zString = "Heroes/wizard";
        //    }
        //    else if (classId == CLASS_HEALER)
        //    {
        //        zString = "Heroes/magic_plant";
        //    }
        //    else if (classId == CLASS_NECROMANCER)
        //    {
        //        zString = "Heroes/undeath_2";
        //    }
        //    else if (classId == CLASS_ARTIST)
        //    {
        //        zString = "Heroes/box_man";
        //    }
        //    else if (classId == CLASS_APOTHECARY)
        //    {
        //        zString = "Heroes/surgeon_zombie";
        //    }
        //    else if (classId == CLASS_DEMAGOGUE)
        //    {
        //        zString = "Heroes/woopa";
        //    }
        //    else if (classId == CLASS_BRAWLER)
        //    {
        //        zString = "Heroes/war_bear";
        //    }
        //    else if (classId == CLASS_WARRIOR)
        //    {
        //        zString = "Heroes/greenwar";
        //    }
        //    else if (classId == CLASS_CENTURION)
        //    {
        //        zString = "Heroes/warrior";
        //    }
        //    else if (classId == CLASS_ROGUE)
        //    {
        //        zString = "Heroes/vam";
        //    }
        //    else if (classId == CLASS_RANGER)
        //    {
        //        zString = "Heroes/sparcher";
        //    }
        //    else if (classId == CLASS_DRUID)
        //    {
        //        zString = "Heroes/fogaman";
        //    }

        //}
        //else
        //{
            
            
        //    if (classId == CLASS_ARCHER)
        //    {
        //        zString = "Heroes/sparcher";
        //    }
        //    else if (classId == CLASS_BARD)
        //    {
        //        zString = "Heroes/surgeon";
        //    }
        //    if (classId == CLASS_CHEMIST)
        //    {
        //        zString = "Heroes/surgeon_zombie";
        //    }
        //    else if (classId == CLASS_CALCULATOR)
        //    {
        //        zString = "Heroes/nurse";
        //    }
        //    else if (classId == CLASS_DANCER)
        //    {
        //        zString = "Heroes/surgeon";
        //    }
        //    else if (classId == CLASS_GEOMANCER)
        //    {
        //        zString = "Heroes/greenwar";
        //    }
        //    else if (classId == CLASS_KNIGHT)
        //    {
        //        zString = "Heroes/knight";
        //    }
        //    else if (classId == CLASS_LANCER)
        //    {
        //        zString = "Heroes/warrior";
        //    }
        //    else if (classId == CLASS_MEDIATOR)
        //    {
        //        zString = "Heroes/surgeon_zombie";
        //    }
        //    else if (classId == CLASS_MIME)
        //    {
        //        zString = "Heroes/box_man";
        //    }
        //    else if (classId == CLASS_MONK)
        //    {
        //        zString = "Heroes/war_bear";
        //    }
        //    else if (classId == CLASS_NINJA)
        //    {
        //        zString = "Heroes/undeath_2";
        //    }
        //    else if (classId == CLASS_ORACLE)
        //    {
        //        zString = "Heroes/woopa";
        //    }
        //    else if (classId == CLASS_PRIEST)
        //    {
        //        zString = "Heroes/magic_plant";
        //    }
        //    else if (classId == CLASS_SAMURAI)
        //    {
        //        zString = "Heroes/vam";
        //    }
        //    else if (classId == CLASS_SQUIRE)
        //    {
        //        zString = "Heroes/pirate";
        //    }
        //    else if (classId == CLASS_SUMMONER)
        //    {
        //        zString = "Heroes/nurse";
        //    }
        //    else if (classId == CLASS_THIEF)
        //    {
        //        zString = "Heroes/fogaman";
        //    }
        //    else if (classId == CLASS_TIME_MAGE)
        //    {
        //        zString = "Heroes/king";
        //    }
        //    else if (classId == CLASS_WIZARD)
        //    {
        //        zString = "Heroes/wizard";
        //    }
        //}
        //return zString;
    }

    //public static readonly string WEAPON_SLOT = "weapon";
    //public static readonly string OFFHAND_SLOT = "offhand";
    //public static readonly string HEAD_SLOT = "head";
    //public static readonly string BODY_SLOT = "body";
    //public static readonly string ACCESSORY_SLOT = "accessory";

    public static readonly string PRIMARY = "primary";
    public static readonly string SECONDARY = "secondary";
    public static readonly string REACTION = "reaction";
    public static readonly string SUPPORT = "support";
    public static readonly string MOVEMENT = "movement";

    public static readonly int COMMAND_SET_ATTACK = 0;
    public static readonly int COMMAND_SET_ITEM = 1;
    public static readonly int COMMAND_SET_BATTLE_SKILL = 2;
    public static readonly int COMMAND_SET_CHARGE = 3;
    public static readonly int COMMAND_SET_THROW = 6;
    public static readonly int COMMAND_SET_BLACK_MAGIC = 9;
    public static readonly int COMMAND_SET_SUMMON_MAGIC = 11;
    public static readonly int COMMAND_SET_ELEMENTAL = 14;
    public static readonly int COMMAND_SET_JUMP = 15;
    public static readonly int COMMAND_SET_DRAW_OUT = 16;
    public static readonly int COMMAND_SET_MATH_SKILL = 17;
    public static readonly int COMMAND_SET_SING = 18;
    public static readonly int COMMAND_SET_DANCE = 19;
    public static readonly int COMMAND_SET_REACTION = 20;
    public static readonly int COMMAND_SET_MOVE = 21;

    public static readonly int SECONDARY_NONE = 0;
    //Mime = 20,  Chemist = 1, Knight = 2, Archer = 3, Squire = 4, Thief = 5,
    //Ninja = 6, Monk = 7, Priest = 8, Wizard = 9, TimeMage = 10,
    //Summoner = 11, Mediator = 12, Oracle = 13, Geomancer = 14, Lancer = 15,
    //Samurai = 16, Calculator = 17, Bard = 18, Dancer = 19

    public static readonly int COMMAND_SET_INFERNO = 100;
    public static readonly int COMMAND_SET_LIGHT_GRIMOIRE = 101;
    public static readonly int COMMAND_SET_DARK_GRIMOIRE = 102;
    public static readonly int COMMAND_SET_ARTS = 103;
    public static readonly int COMMAND_SET_POTION = 104;
    public static readonly int COMMAND_SET_ORATORY = 105;
    public static readonly int COMMAND_SET_UNARMED_SKILLS = 106;
    public static readonly int COMMAND_SET_SOLDIER_SKILLS = 107;
    public static readonly int COMMAND_SET_LEGION_KIT = 108;
    public static readonly int COMMAND_SET_DIRTY_TRICKS = 109;
    public static readonly int COMMAND_SET_HAWKEYE = 110;
    public static readonly int COMMAND_SET_BALANCE = 111;

    public static readonly int COMMAND_SET_ATTACK_AURELIAN = 200;
    public static readonly int COMMAND_SET_MOVE_AURELIAN = 201;
    public static readonly int COMMAND_SET_REACTION_AURELIAN = 202;



    public static readonly int REACTION_ABANDON = 1;
    public static readonly int REACTION_ABSORB_USED_MP = 2;
    public static readonly int REACTION_ARROW_GUARD = 3;
    public static readonly int REACTION_A_SAVE = 4;
    public static readonly int REACTION_AUTO_POTION = 5;
    public static readonly int REACTION_BLADE_GRASP = 6;
    public static readonly int REACTION_BRAVE_UP = 7;
    public static readonly int REACTION_CAUTION = 8;
    public static readonly int REACTION_COUNTER = 9;
    public static readonly int REACTION_COUNTER_FLOOD = 10;
    public static readonly int REACTION_COUNTER_MAGIC = 11;
    public static readonly int REACTION_COUNTER_TACKLE = 12;
    public static readonly int REACTION_CRITICAL_QUICK = 13;
    public static readonly int REACTION_DAMAGE_SPLIT = 14;
    public static readonly int REACTION_DISTRIBUTE = 15;
    public static readonly int REACTION_DRAGON_SPIRIT = 16;
    public static readonly int REACTION_FACE_UP = 17;
    public static readonly int REACTION_FINGER_GUARD = 18;
    public static readonly int REACTION_HAMEDO = 20;//myDict.Add(20, "Hamedo");
    public static readonly int REACTION_HP_RESTORE = 21;
    public static readonly int REACTION_MA_SAVE = 22;
    public static readonly int REACTION_MEATBONE_SLASH = 23;
    public static readonly int REACTION_MP_RESTORE = 24;
    public static readonly int REACTION_MP_SWITCH = 25;
    public static readonly int REACTION_CATCH = 26;
    public static readonly int REACTION_REGENERATOR = 27;
    public static readonly int REACTION_SPEED_SAVE = 28;
    public static readonly int REACTION_WEAPON_GUARD = 30;

    public static readonly int REACTION_RESTORE_MANA = 112;
    public static readonly int REACTION_RETURN_SPELL = 113;
    public static readonly int REACTION_RESTORE_HP = 114;
    public static readonly int REACTION_THRIFTY_HEAL = 115;
    public static readonly int REACTION_DOOMSAYER = 116;
    public static readonly int REACTION_INT_BOOST = 117;
    public static readonly int REACTION_ENCORE = 118;
    public static readonly int REACTION_BREAK_A_LEG = 119;
    public static readonly int REACTION_HEALING_SALVE = 120;
    public static readonly int REACTION_LEG_SALVE = 121;
    public static readonly int REACTION_REBUTTAL = 122;
    public static readonly int REACTION_SKL_BOOST = 123;
    public static readonly int REACTION_STRIKE_BACK = 124;
    public static readonly int REACTION_STR_BOOST = 125;
    public static readonly int REACTION_LAST_STAND = 126;
    public static readonly int REACTION_CRG_BOOST = 127;
    public static readonly int REACTION_DEFLECT_FAR = 128;
    public static readonly int REACTION_RETURN_DAMAGE = 129;
    public static readonly int REACTION_DEFLECT_CLOSE = 130;
    public static readonly int REACTION_GUARD = 131;
    public static readonly int REACTION_HP_TO_MP = 132;
    public static readonly int REACTION_AGI_BOOST = 133;
    public static readonly int REACTION_SLOW_HEAL = 134;
    public static readonly int REACTION_SHORTCUT = 135;
    public static readonly int REACTION_WIS_BOOST = 201;


    public static readonly int REACTION_TYPE_DAMAGE = 1;
    public static readonly int REACTION_TYPE_CRITICAL = 3;
    public static readonly int REACTION_TYPE_ANY_TARGET = 10; //targeted by anyone or anything
    public static readonly int REACTION_TYPE_ALLY_TARGET = 11; //targetted by an ally
    public static readonly int REACTION_TYPE_ENEMY_TARGET = 12; //targetted by an enemy
    public static readonly int REACTION_TYPE_MAGIC_SPELL = 13; //targetd by pmType == magic and has mp > 0
    public static readonly int REACTION_TYPE_DISTRIBUTE = 14;

    public static readonly int SUPPORT_HALF_MP = 14;
    public static readonly int SUPPORT_MAINTENANCE = 17;
    public static readonly int SUPPORT_MARTIAL_ARTS = 18;
    public static readonly int SUPPORT_THROW_ITEM = 24;
    public static readonly int SUPPORT_TWO_HANDS = 26;
    public static readonly int SUPPORT_TWO_SWORDS = 27;
    public static readonly int SUPPORT_ATTACK_UP = 1;
    public static readonly int SUPPORT_MAGIC_ATTACK_UP = 15;
    public static readonly int SUPPORT_DEFENSE_UP = 4;
    public static readonly int SUPPORT_MAGIC_DEFEND_UP = 16;
    public static readonly int SUPPORT_CONCENTRATE = 2;
    public static readonly int SUPPORT_SHORT_CHARGE = 23; //CalculationAT.cs for CTR modification
    public static readonly int SUPPORT_NO_CHARGE = 21; //CalculationAT.cs for CTR modification
	public static readonly int SUPPORT_DEFEND = 3;

    public static readonly int SUPPORT_CLASSIC_EQUIP_ARMOR = 5;
    public static readonly int SUPPORT_CLASSIC_EQUIP_AXE = 6;
    public static readonly int SUPPORT_CLASSIC_EQUIP_CHANGE = 7;
    public static readonly int SUPPORT_CLASSIC_EQUIP_CROSSBOW = 8;
    public static readonly int SUPPORT_CLASSIC_EQUIP_GUN = 9;
    public static readonly int SUPPORT_CLASSIC_EQUIP_KNIFE = 10;
    public static readonly int SUPPORT_CLASSIC_EQUIP_SHIELD = 11;
    public static readonly int SUPPORT_CLASSIC_EQUIP_SPEAR = 12;
    public static readonly int SUPPORT_CLASSIC_EQUIP_SWORD = 13;

    //Aurelian
    public static readonly int SUPPORT_MAGIC_ATK_UP = 136;
    public static readonly int SUPPORT_FLAME_TOUCHED = 137;
    public static readonly int SUPPORT_QUICK_CAST = 138;
    public static readonly int SUPPORT_LIGHTS_BLESSING = 139;
    public static readonly int SUPPORT_MP_CUT = 140;
    public static readonly int SUPPORT_LEACH = 141;
    public static readonly int SUPPORT_SADIST = 142;
    public static readonly int SUPPORT_SHOW_MUST_GO_ON = 143;
    public static readonly int SUPPORT_ICONOCLAST = 144;
    public static readonly int SUPPORT_UNNATURAL_HIGH = 145;
    public static readonly int SUPPORT_MAGIC_DEF_UP = 146;
    public static readonly int SUPPORT_NEUTRAL_ATK_UP = 147;
    public static readonly int SUPPORT_EQUIPMENT_GUARD = 148;
    public static readonly int SUPPORT_PHYSICAL_ATK_UP = 149;
    public static readonly int SUPPORT_ENVY = 150;
    public static readonly int SUPPORT_MIGHTY_GRIP = 151;
    public static readonly int SUPPORT_STAND_GROUND = 152;
    public static readonly int SUPPORT_FOR_THE_IMPERIUM = 153;
    public static readonly int SUPPORT_PHYSICAL_DEF_UP = 154;
    public static readonly int SUPPORT_DUAL_WIELD = 155;
    public static readonly int SUPPORT_CRIT_HIT_UP = 156;
    public static readonly int SUPPORT_FOCUS = 157;
    public static readonly int SUPPORT_KNOCKBACK = 158;
    public static readonly int SUPPORT_NATURAL_HIGH = 159;
    public static readonly int SUPPORT_NEUTRAL_DEF_UP = 160;
    public static readonly int SUPPORT_CHANNEL = 161;
    public static readonly int SUPPORT_HONE = 162;
    
    public static readonly int SUPPORT_EQUIP_WAND = 187;
    public static readonly int SUPPORT_EQUIP_MAGE_ROBES = 188;
    public static readonly int SUPPORT_EQUIP_STAFFS = 189;
    public static readonly int SUPPORT_EQUIP_INSTRUMENT_DECK = 190;
    public static readonly int SUPPORT_EQUIP_GUNS = 191;
    public static readonly int SUPPORT_EQUIP_CLOTHES = 192;
    public static readonly int SUPPORT_EQUIP_WHIP_MACE = 193;
    public static readonly int SUPPORT_EQUIP_SWORDS = 194;
    public static readonly int SUPPORT_EQUIP_HEAVY_ARMORS = 195;
    public static readonly int SUPPORT_EQUIP_SHIELD = 196;
    public static readonly int SUPPORT_EQUIP_LIGHT_ARMORS = 197;
    public static readonly int SUPPORT_EQUIP_BOWS = 198;
    public static readonly int SUPPORT_EQUIP_SCALES = 199;
    public static readonly int SUPPORT_EQUIP_SPEAR = 200;

    

    public static readonly int MOVEMENT_UNSTABLE_TP = 163;
    public static readonly int MOVEMENT_MOVE_WIS_UP = 164;
    public static readonly int MOVEMENT_SAINTS_FOOTSTEPS = 165;
    public static readonly int MOVEMENT_BLESSED_STEPS = 166;
    public static readonly int MOVEMENT_RAISE_THE_DEAD = 167;
    public static readonly int MOVEMENT_GHOST = 168;
    public static readonly int MOVEMENT_WINDS_OF_FATE = 169;
    public static readonly int MOVEMENT_DRAW_ATTENTION = 170;
    public static readonly int MOVEMENT_SWAP = 171;
    public static readonly int MOVEMENT_WALK_IT_ON = 172;
    public static readonly int MOVEMENT_SILENCE_THE_CROWD = 173;
    public static readonly int MOVEMENT_MP_WALK = 174;
    public static readonly int MOVEMENT_JUMP_UP_2 = 175;
    public static readonly int MOVEMENT_MOVE_UP_2 = 176;
    public static readonly int MOVEMENT_MOVE_CRG_UP = 177;
    public static readonly int MOVEMENT_CRUNCH = 178;
    public static readonly int MOVEMENT_HP_WALK = 179;
    public static readonly int MOVEMENT_SCALE = 180;
    public static readonly int MOVEMENT_MOVE_UP_1 = 181;
    public static readonly int MOVEMENT_MOVE_SKL_UP = 182;
    public static readonly int MOVEMENT_LEAP = 183;
    public static readonly int MOVEMENT_TP_WALK = 184;
    public static readonly int MOVEMENT_STRETCH_LEGS = 185;
    public static readonly int MOVEMENT_WALK_IT_OFF = 186;

    public static readonly int MOVEMENT_NONE = 0;
    public static readonly int MOVEMENT_FLOAT = 3;
    public static readonly int MOVEMENT_FLY = 4;
    public static readonly int MOVEMENT_IGNORE_HEIGHT = 5;
    public static readonly int MOVEMENT_JUMP_1 = 6;
    public static readonly int MOVEMENT_JUMP_2 = 7;
    public static readonly int MOVEMENT_JUMP_3 = 8;
    public static readonly int MOVEMENT_MOVE_1 = 9;
    public static readonly int MOVEMENT_MOVE_2 = 10;
    public static readonly int MOVEMENT_MOVE_3 = 11;
    public static readonly int MOVEMENT_MOVE_HP_UP = 12;
    public static readonly int MOVEMENT_MOVE_MP_UP = 15;
    public static readonly int MOVEMENT_TELEPORT_1 = 16;
    public static readonly int MOVEMENT_TELEPORT_2 = 17;

    public static readonly string ANIMATION_DEAD = "dead";
    public static readonly string ANIMATION_LIFE = "life";

    public static readonly int TILE_STANDABLE = 1;
    public static readonly int UNDEAD_OVERRIDE = 1919;//called in addstatusandoverrideothers for undead possibly revivig

    //used for outputs for RL for board 
    public static readonly int GROUND_CRYSTAL_ID = -1;

    public static readonly int PHASE_ACTOR_CHARGING = -1;
    public static readonly int PHASE_ACTOR_ALL = 0;
    public static readonly int PHASE_ACTOR_ACT_COMPLETED = 1;
    public static readonly int PHASE_ACTOR_MOVE_COMPLETED = 2;
    public static readonly int PHASE_ACTOR_END = 3;

    public static readonly int HITS_STAT_NONE = 0;
    public static readonly int HITS_STAT= 1;
    //public static readonly int HITS_STAT_ABSORB = 2;
    public static readonly int HITS_STAT_PERCENTAGE = 3;

    public static readonly int IGNORES_DEFENSE_NO = 0;
    public static readonly int IGNORES_DEFENSE_YES = 1;

    public static readonly int REMOVE_STAT_REMOVE = 1;
    public static readonly int REMOVE_STAT_HEAL = 0;
    public static readonly int REMOVE_STAT_ABSORB = 2;

    public static readonly string REMOVE_STAT_NAME_REMOVE = "Damage";
    public static readonly string REMOVE_STAT_NAME_HEAL = "Heal";
    public static readonly string REMOVE_STAT_NAME_ABSORB = "Absorb";

    public static readonly int STAT_TYPE_NONE = 0;
    public static readonly int STAT_TYPE_HP = 1;
    public static readonly int STAT_TYPE_MP = 2;
    public static readonly int STAT_TYPE_SPEED = 3;
    public static readonly int STAT_TYPE_PA = 4;

    public static readonly int STAT_TYPE_AGI = 5;
    public static readonly int STAT_TYPE_MA = 6;
    public static readonly int STAT_TYPE_BRAVE = 7;
    public static readonly int STAT_TYPE_FAITH = 8;
    public static readonly int STAT_TYPE_CUNNING = 9;

    public static readonly int STAT_TYPE_WEAPON = 10;
    public static readonly int STAT_TYPE_OFFHAND = 11;
    public static readonly int STAT_TYPE_HEAD = 12;
    public static readonly int STAT_TYPE_BODY = 13;
    public static readonly int STAT_TYPE_ACCESSORY = 14;

    public static readonly int STAT_TYPE_CT = 15;
    public static readonly int STAT_TYPE_MAX_MP = 16;
    public static readonly int STAT_TYPE_MAX_HP = 17;
    public static readonly int STAT_TYPE_C_EVADE = 18;
    public static readonly int STAT_TYPE_P_EVADE = 19;

    public static readonly int STAT_TYPE_M_EVADE = 20;
    public static readonly int STAT_TYPE_MOVE = 21;
    public static readonly int STAT_TYPE_JUMP = 22;
    public static readonly int STAT_TYPE_DIRECTION = 23;

    public static readonly int BASE_Q_DIRECTION_FACE_TOWARD = 0;
    public static readonly int BASE_Q_DIRECTION_FACE_AWAY = 1;

    public static readonly string STAT_TYPE_NAME_NONE = "None";
    public static readonly string STAT_TYPE_NAME_HP = "HP";
    public static readonly string STAT_TYPE_NAME_MP = "MP";
    public static readonly string STAT_TYPE_NAME_SPEED = "Speed";
    public static readonly string STAT_TYPE_NAME_PA = "Strength";

    public static readonly string STAT_TYPE_NAME_AGI = "Agility";
    public static readonly string STAT_TYPE_NAME_MA = "Intelligence";
    public static readonly string STAT_TYPE_NAME_BRAVE = "Courage";
    public static readonly string STAT_TYPE_NAME_FAITH = "Wisdom";
    public static readonly string STAT_TYPE_NAME_CUNNING = "Cunning";

    public static readonly string STAT_TYPE_NAME_WEAPON = "Break Weapon";
    public static readonly string STAT_TYPE_NAME_OFFHAND = "Break Offhand";
    public static readonly string STAT_TYPE_NAME_HEAD = "Break Helm";
    public static readonly string STAT_TYPE_NAME_BODY = "Break Armor";
    public static readonly string STAT_TYPE_NAME_ACCESSORY = "Break Accessory";

    public static readonly string STAT_TYPE_NAME_CT = "CT";
    public static readonly string STAT_TYPE_NAME_MAX_MP = "Max MP";
    public static readonly string STAT_TYPE_NAME_MAX_HP = "Max HP";
    public static readonly string STAT_TYPE_NAME_C_EVADE = "Class Evade";
    public static readonly string STAT_TYPE_NAME_P_EVADE = "Physical Evade";

    public static readonly string STAT_TYPE_NAME_M_EVADE = "Magic Evade";
    public static readonly string STAT_TYPE_NAME_MOVE = "Move";
    public static readonly string STAT_TYPE_NAME_JUMP = "Jump";
    public static readonly string STAT_TYPE_NAME_DIRECTION = "Direction";

    static public string GetStatTypeString(int statType)
    {
        string zString = "";
        if( statType == 1)
        {
            zString = "hp";
        }
        else if (statType == 2)
        {
            zString = "mp";
        }
        else if (statType == 3)
        {
            zString = "speed";
        }
        else if (statType == 4)
        {
            zString = "STR";
        }
        else if (statType == 5)
        {
            zString = "AGI";
        }
        else if (statType == 6)
        {
            zString = "INT";
        }
        else if (statType == 7)
        {
            zString = "CRG";
        }
        else if (statType == 8)
        {
            zString = "WIS";
        }
        else if (statType == 9)
        {
            zString = "SKL";
        }
        else if (statType == 10)
        {
            zString = "broken weapon";
        }
        else if (statType == 11)
        {
            zString = "broken offhand";
        }
        else if (statType == 12)
        {
            zString = "broken helm";
        }
        else if (statType == 13)
        {
            zString = "broken armor";
        }
        else if (statType == 14)
        {
            zString = "broken accessory";
        }
        else if (statType == 15)
        {
            zString = "TP";
        }
        else if (statType == 16)
        {
            zString = "max MP";
        }
        else if (statType == 17)
        {
            zString = "max HP";
        }
        else if (statType == 18)
        {
            zString = "c evade";
        }
        else if (statType == 19)
        {
            zString = "p evade";
        }
        else if (statType == 20)
        {
            zString = "m evade";
        }
        else if (statType == 21)
        {
            zString = "move";
        }
        else if (statType == 22)
        {
            zString = "jump";
        }
        
        return zString;
    }

    //    0	none
    //1	    private int stat_total_life;
    //2	    private int stat_total_mp;
    //3	    private int stat_total_speed;
    //4	    private int stat_total_pa;
    //5	    private int stat_total_agi;
    //6	    private int stat_total_ma;
    //7	    private int stat_total_brave;
    //8	    private int stat_total_faith;
    //9	    private int stat_total_cunning;
    //10	weapon
    //11	offhand
    //12	head
    //13	body
    //14	accessory
    //15	ct
    //15	    private int stat_total_maxMP;
    //16	    private int stat_total_maxLife; //add this functionality versus life later
    //17	    private int stat_total_class_evade;
    //18	    private int stat_total_p_evade;
    //19	    private int stat_total_m_evade;
    //20	 private int stat_total_move;
    //21	    private int stat_total_jump;

    public static bool IsClassicClass(int classId)
    {
        if (classId <= CLASS_MIME)
            return true;
        else if (classId < CUSTOM_CLASS_ID_START_VALUE)
            return false;
        else
        {
            ClassEditObject ce = CalcCode.LoadCEObject(classId);
            if (ce.Version == NameAll.VERSION_CLASSIC)
                return true;
            else
                return false;
        }
    }

    public static int GetSpellIndexByWeaponType(int weaponType, int classId)
    {
        bool isAurelianCustom = false;

        if( classId >= CUSTOM_CLASS_ID_START_VALUE)
        {
            ClassEditObject ce = CalcCode.LoadCEObject(classId);
            if (ce.Version != NameAll.VERSION_CLASSIC)
                isAurelianCustom = true;
        }


        if( (classId >= NameAll.CLASS_FIRE_MAGE && classId < CUSTOM_CLASS_ID_START_VALUE)
            || isAurelianCustom == true)
        {
            if (weaponType == NameAll.ITEM_ITEM_TYPE_FIST)
            {
                return NameAll.SPELL_INDEX_ATTACK_FIST;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_KATANA)
            {
                return NameAll.SPELL_INDEX_ATTACK_KATANA;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_GREATSWORD)
            {
                return NameAll.SPELL_INDEX_ATTACK_GREATSWORD;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_DAGGER)
            {
                return NameAll.SPELL_INDEX_ATTACK_DAGGER;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_MACE)
            {
                return NameAll.SPELL_INDEX_ATTACK_MACE;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_BOW)
            {
                return NameAll.SPELL_INDEX_ATTACK_BOW;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_SWORD)
            {
                return NameAll.SPELL_INDEX_ATTACK_SWORD;
            }
            //else if (weaponType == NameAll.ITEM_ITEM_TYPE_ROD)
            //{
            //    return NameAll.SPELL_INDEX_ATTACK_ROD;
            //}
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_SPEAR)
            {
                return NameAll.SPELL_INDEX_ATTACK_SPEAR;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CROSSBOW)
            {
                return NameAll.SPELL_INDEX_ATTACK_CROSSBOW;
            }
            //else if (weaponType == NameAll.ITEM_ITEM_TYPE_AXE)
            //{
            //    return NameAll.SPELL_INDEX_ATTACK_AXE;
            //}
            //else if (weaponType == NameAll.ITEM_ITEM_TYPE_STAFF)
            //{
            //    return NameAll.SPELL_INDEX_ATTACK_STAFF;
            //}
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_STICK)
            {
                return NameAll.SPELL_INDEX_ATTACK_STICK;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_GUN)
            {
                return NameAll.SPELL_INDEX_ATTACK_GUN;
            }
            //else if (weaponType == NameAll.ITEM_ITEM_TYPE_MAGIC_GUN)
            //{
            //    return NameAll.SPELL_INDEX_ATTACK_MAGIC_GUN;
            //}
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_DECK)
            {
                return NameAll.SPELL_INDEX_ATTACK_DECK;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_INSTRUMENT)
            {
                return NameAll.SPELL_INDEX_ATTACK_INSTRUMENT;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_WAND)
            {
                return NameAll.SPELL_INDEX_ATTACK_WAND;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_SCALES)
            {
                return NameAll.SPELL_INDEX_ATTACK_SCALES;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_WHIP)
            {
                return NameAll.SPELL_INDEX_ATTACK_WHIP;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_PISTOL)
            {
                return NameAll.SPELL_INDEX_ATTACK_PISTOL;
            }
            else
            {
                Debug.Log("ERROR: unable to find the spell name index for attack (classId,weaponType)" + classId + ", " +weaponType );
                return NameAll.SPELL_INDEX_ATTACK_FIST;
            }
        }
        else
        {
            if (weaponType == NameAll.ITEM_ITEM_TYPE_FIST)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_FIST;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_KATANA)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_KATANA;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_KNIGHT)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_KNIGHT;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_DAGGER)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_DAGGER;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_NINJA)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_NINJA;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_LONGBOW)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_LONGBOW;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_SWORD)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_SWORD;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_ROD)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_ROD;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_SPEAR)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_SPEAR;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_CROSSBOW)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_CROSSBOW;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_AXE)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_AXE;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_BAG)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_BAG;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_HAMMER)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_HAMMER;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_STAFF)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_STAFF;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_STICK)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_STICK;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_GUN)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_GUN;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_MAGIC_GUN)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_MAGIC_GUN;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_CLOTH)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_CLOTH;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_DICTIONARY)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_DICTIONARY;
            }
            else if (weaponType == NameAll.ITEM_ITEM_TYPE_CLASSIC_HARP)
            {
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_HARP;
            }
            else
            {
                Debug.Log("ERROR: unable to find the spell name index for attack");
                return NameAll.SPELL_INDEX_ATTACK_CLASSIC_FIST;
            }
        }
        
    }

    public static string GetModName(int modType)
    {
        string zString = "";
        if (modType == MOD_NULL)
        {
            zString = "null";
        }
        else if (modType == MOD_PHYSICAL)
        {
            zString = "physical";
        }
        else if (modType == MOD_MAGICAL)
        {
            zString = "magical";
        }
        else if (modType == MOD_NEUTRAL)
        {
            zString = "neutral";
        }
        else if (modType == MOD_ATTACK)
        {
            zString = "attack";
        }
        else if (modType == MOD_PHYSICAL_MAGICAL)
        {
            zString = "phy-mag";
        }
        else if (modType == MOD_PHYSICAL_NEUTRAL)
        {
            zString = "phy-neu";
        }
        else if (modType == MOD_MAGICAL_PHYSICAL)
        {
            zString = "mag-phy";
        }
        else if (modType == MOD_MAGICAL_NEUTRAL)
        {
            zString = "mag-neu";
        }
        else if (modType == MOD_NEUTRAL_PHYSICAL)
        {
            zString = "neu-phy";
        }
        else if (modType == MOD_NEUTRAL_MAGICAL)
        {
            zString = "neu-mag";
        }
        else if (modType == MOD_PHYSICAL_AGI)
        {
            zString = "phy-agi";
        }

        return zString;
    }

    public static string GetHitsStatString(int hitsStat)
    {
        if( hitsStat == 0)
        {
            return "no";
        }
        else if( hitsStat == 1)
        {
            return "yes";
        }
        else if(hitsStat > 100)
        {
            return "yes (" + (hitsStat - 100) + "%)";
        }
        return "yes";
    }

    public static string GetRemoveStatString(int removesStat, int hitsStat)
    {
        string zString = "";
        if( hitsStat == 0)
        {
            zString = "NA";
        }
        else
        {
            if (removesStat == REMOVE_STAT_HEAL)
            {
                zString = "heals";
            }
            else if (removesStat == REMOVE_STAT_REMOVE)
            {
                zString = "dmg";
            }
            else if (removesStat == REMOVE_STAT_ABSORB)
            {
                zString = "absorb";
            }
        }
        
        return zString;
    }

    public static readonly int PM_TYPE_PHYSICAL = 0;
    public static readonly int PM_TYPE_MAGICAL = 1;
    public static readonly int PM_TYPE_NEUTRAL = 2;
    public static readonly int PM_TYPE_NULL = 3;

    public static readonly string PM_TYPE_NAME_PHYSICAL = "physical";
    public static readonly string PM_TYPE_NAME_MAGICAL = "magical";
    public static readonly string PM_TYPE_NAME_NEUTRAL = "neutral";
    public static readonly string PM_TYPE_NAME_NULL = "null";

    public static string GetPmTypeString(int pmType)
    {
        string zString = "";
        if( pmType == PM_TYPE_PHYSICAL)
        {
            zString = "physical";
        }
        else if (pmType == PM_TYPE_MAGICAL)
        {
            zString = "magical";
        }
        else if (pmType == PM_TYPE_NEUTRAL)
        {
            zString = "neutral";
        }
        else
        {
            zString = "null";
        }
        return zString;
    }

    public static readonly int COUNTER_TYPE_NONE = 0;
    public static readonly int COUNTER_TYPE_BLADE_GRASP = 2;
    public static readonly int COUNTER_TYPE_COUNTER_FLOOD = 6;
    public static readonly int COUNTER_TYPE_BOTH = 16;

    public static readonly int ALLIES_TYPE_ANY = 0;
    public static readonly int ALLIES_TYPE_ALLIES = 1;
    public static readonly int ALLIES_TYPE_ENEMIES = 2;

    public static string GetAlliesTypeString(int type)
    {
        string zString = "";
        if (type == ALLIES_TYPE_ALLIES)
        {
            zString = "allies";
        }
        else if (type == ALLIES_TYPE_ENEMIES)
        {
            zString = "enemies";
        }
        else
        {
            zString = "all";
        }
        return zString;
    }

    public static string GetEffectTypeString(int type)
    {
        string zString = "";
        if ( type >= 1 || type <= 4)
        {
            zString = type.ToString();
        }
        else if( type >= 102 && type <= 108)
        {
            zString = "line " + (type - 100) + " panels";
        }
        else if( type == 109)
        {
            zString = "allies";
        }
        else if (type == 110)
        {
            zString = "enemies";
        }
        else if (type == 111)
        {
            zString = "math skill";
        }
        else if (type >= SPELL_EFFECT_CONE_2 && type <= SPELL_EFFECT_CONE_MAX)
        {
            zString = "cone size " + (type-SPELL_EFFECT_CONE_BASE);
        }
        else
        {
            zString = type.ToString();
        }
        return zString;
    }
    //called in CalcCode, used for building abilities
    public static string GetRangeTypeMin(int type)
    {
        string zString = "";
        if (type == -1)
        {
            zString = "self-target";
        }
        else if (type == 100)
        {
            zString = "weapon";
        }
        else if (type == 1919)
        {
            zString = "NA";
        }
        else
        {
            zString = type.ToString();
        }
        return zString;
    }
    //called in CalcCode, used for building abilities
    public static string GetRangeTypeMax(int type, int typeMax)
    {
        string zString = "";
        if (type == -1)
        {
            zString = "self-target";
        }
        else if( typeMax < 100)
        {
            zString = typeMax.ToString();
        }
        //else if (type >= 0 && typeMax < 100)
        //{
        //    zString = typeMax.ToString();
        //}
        else if (typeMax == NameAll.SPELL_RANGE_MAX_WEAPON)
        {
            zString = "weapon";
        }
        //else if (type == 1919)
        //{
        //    zString = "NA";
        //}
        else if (typeMax == SPELL_RANGE_MAX_LINE)
        {
            zString = "line";
        }
        else if (typeMax == SPELL_RANGE_MAX_MOVE)
        {
            zString = "actor's move";
        }
        else if (typeMax == SPELL_RANGE_SPEAR)
        {
            zString = "2 (spear)";
        }
        else
        {
            zString = typeMax.ToString();
        }

        return zString;
    }

    //called in UISpellNameDetails
    public static string GetRangeTypeString(int type, int typeMax)
    {
        string zString = "";
        if( type == -1)
        {
            zString = "self-target";
        }
        else if( type >= 0 && typeMax < 100)
        {
            zString = type + " - " + typeMax;
        }
        else if( type == 100)
        {
            zString = "weapon";
        }
        else if( type == 1919)
        {
            zString = "";
        }
        else if( typeMax == SPELL_RANGE_MAX_LINE)
        {
            zString = "line";
        }
        else if( typeMax == SPELL_RANGE_MAX_MOVE)
        {
            zString = type + " - " + " move";
        }
        else if (typeMax == SPELL_RANGE_SPEAR)
        {
            zString = type + " - " + " 2";
        }
        else
        {
            zString = type + " - " + typeMax;
        }

        return zString;
    }

    public static string GetEffectZString(int type)
    {
        string zString = "";
        if (type == 1919)
        {
            zString = "NA";
        }
        else
        {
            zString = type.ToString();
        }

        return zString;
    }


    

    //changing the team lists in playermanager
    public static readonly int TEAM_LIST_CLEAR = 3;
    public static readonly int TEAM_LIST_ADD = 1;
    public static readonly int TEAM_LIST_REMOVE = 0;
    //team ids
    //these pre set ones tell the alliances, others are generated on the fly in PlayerManager and have the alliances generated there too
    public static readonly int TEAM_ID_NONE = 0; //always on its own team. if aggro'd will become enemy to that team
    public static readonly int TEAM_ID_NEUTRAL = 1; //on neutral team. neutral to other units unless neutral team is aggro'd then enemy to that team too
    public static readonly int TEAM_ID_GREEN = 2; //allied with green, enemy of red, neutral to others
    public static readonly int TEAM_ID_RED = 3; //allied with red, enemy of green, neutral to others
    public static readonly int TEAM_ID_HOSTILE_TEAM = 4; //hostile to units except on the hostile team
    public static readonly int TEAM_ID_HOSTILE_NONE = 5; //hostile to all other units
    public static readonly int TEAM_ID_LOCAL = 50; //locally assigned team id
    public static readonly int TEAM_ID_WALK_AROUND_DEFAULT = 1000; //default for walk around
	public static readonly int TEAM_ID_WALK_AROUND_NEUTRAL = 1;//1001; //might want it's own unique number later
	public static readonly int TEAM_ID_WALK_AROUND_GREEN = 2;//1002; //might want it's own unique number later
	public static readonly int TEAM_ID_WALK_AROUND_RED = 3;//1003; //might want it's own unique number later

	//takes a statusId and returns the statusId of the block status, called in statusmanager
	public static int GetStatusBlockId(int statusId)
    {
        //Debug.Log("need to write this function");
        int z1 = 0;
        if( statusId == NameAll.STATUS_ID_NONE)
        {
            z1 = 0;
        }
        else if( statusId == NameAll.STATUS_ID_POISON)
        {
            z1 = NameAll.STATUS_ID_BLOCK_POISON;
        }
        else if (statusId == NameAll.STATUS_ID_BERSERK)
        {
            z1 = NameAll.STATUS_ID_BLOCK_BERSERK;
        }
        else if (statusId == NameAll.STATUS_ID_CHARM)
        {
            z1 = NameAll.STATUS_ID_BLOCK_CHARM;
        }
        else if (statusId == NameAll.STATUS_ID_CHICKEN)
        {
            z1 = NameAll.STATUS_ID_BLOCK_CHICKEN;
        }
        else if (statusId == NameAll.STATUS_ID_CONFUSION)
        {
            z1 = NameAll.STATUS_ID_BLOCK_CONFUSION;
        }
        else if (statusId == NameAll.STATUS_ID_DARKNESS)
        {
            z1 = NameAll.STATUS_ID_BLOCK_DARKNESS;
        }
        else if (statusId == NameAll.STATUS_ID_DEAD)
        {
            z1 = NameAll.STATUS_ID_BLOCK_DEAD;
        }
        else if (statusId == NameAll.STATUS_ID_DEATH_SENTENCE)
        {
            z1 = NameAll.STATUS_ID_BLOCK_DEATH_SENTENCE;
        }
        else if (statusId == NameAll.STATUS_ID_DONT_ACT)
        {
            z1 = NameAll.STATUS_ID_BLOCK_DONT_ACT;
        }
        else if (statusId == NameAll.STATUS_ID_DONT_MOVE)
        {
            z1 = NameAll.STATUS_ID_BLOCK_DONT_MOVE;
        }
        else if (statusId == NameAll.STATUS_ID_FAITH)
        {
            z1 = NameAll.STATUS_ID_BLOCK_FAITH;
        }
        else if (statusId == NameAll.STATUS_ID_FLOAT)
        {
            z1 = NameAll.STATUS_ID_BLOCK_FLOAT;
        }
        else if (statusId == NameAll.STATUS_ID_FROG)
        {
            z1 = NameAll.STATUS_ID_BLOCK_FROG;
        }
        else if (statusId == NameAll.STATUS_ID_HASTE)
        {
            z1 = NameAll.STATUS_ID_BLOCK_HASTE;
        }
        else if (statusId == NameAll.STATUS_ID_INNOCENT)
        {
            z1 = NameAll.STATUS_ID_BLOCK_INNOCENT;
        }
        else if (statusId == NameAll.STATUS_ID_OIL)
        {
            z1 = NameAll.STATUS_ID_BLOCK_OIL;
        }
        else if (statusId == NameAll.STATUS_ID_PETRIFY)
        {
            z1 = NameAll.STATUS_ID_BLOCK_PETRIFY;
        }
        else if (statusId == NameAll.STATUS_ID_REFLECT)
        {
            z1 = NameAll.STATUS_ID_BLOCK_REFLECT;
        }
        else if (statusId == NameAll.STATUS_ID_REGEN)
        {
            z1 = NameAll.STATUS_ID_BLOCK_REGEN;
        }
        else if (statusId == NameAll.STATUS_ID_RERAISE)
        {
            z1 = NameAll.STATUS_ID_BLOCK_RERAISE;
        }
        else if (statusId == NameAll.STATUS_ID_SILENCE)
        {
            z1 = NameAll.STATUS_ID_BLOCK_SILENCE;
        }
        else if (statusId == NameAll.STATUS_ID_SLEEP)
        {
            z1 = NameAll.STATUS_ID_BLOCK_SLEEP;
        }
        else if (statusId == NameAll.STATUS_ID_SLOW)
        {
            z1 = NameAll.STATUS_ID_BLOCK_SLOW;
        }
        else if (statusId == NameAll.STATUS_ID_STOP)
        {
            z1 = NameAll.STATUS_ID_BLOCK_STOP;
        }
        else if (statusId == NameAll.STATUS_ID_UNDEAD)
        {
            z1 = NameAll.STATUS_ID_BLOCK_UNDEAD;
        }
        else if (statusId == NameAll.STATUS_ID_APATHY)
        {
            z1 = NameAll.STATUS_ID_BLOCK_APATHY;
        }
        //else
        //{
        //    Debug.Log("ERROR: could not find the block for this status");
        //}
        return z1;
    }

    //I have no idea why this isn't a dictionary
    public static string GetStatusString(int statusId)
    {
        //Debug.Log("need to write this function");
        string zString = "";
        if (statusId == NameAll.STATUS_ID_NONE)
        {
            zString = "";
        }
        else if (statusId == NameAll.STATUS_ID_BERSERK)
        {
            zString = "berserk";
        }
        else if (statusId == NameAll.STATUS_ID_CHARGING)
        {
            zString = "charging";
        }
        else if (statusId == NameAll.STATUS_ID_CHARM)
        {
            zString = "charm";
        }
        else if (statusId == NameAll.STATUS_ID_CHICKEN)
        {
            zString = "fear";
        }
        else if (statusId == NameAll.STATUS_ID_CONFUSION)
        {
            zString = "confusion";
        }
        else if (statusId == NameAll.STATUS_ID_CRITICAL)
        {
            zString = "critical";
        }
        else if (statusId == NameAll.STATUS_ID_CRYSTAL)
        {
            zString = "crystal";
        }
        else if (statusId == NameAll.STATUS_ID_DARKNESS)
        {
            zString = "blind";
        }
        else if (statusId == NameAll.STATUS_ID_DEAD)
        {
            zString = "dead";
        }
        else if (statusId == NameAll.STATUS_ID_DEATH_SENTENCE)
        {
            zString = "death sentence";
        }
        else if (statusId == NameAll.STATUS_ID_DONT_ACT)
        {
            zString = "cant act";
        }
        else if (statusId == NameAll.STATUS_ID_DONT_MOVE)
        {
            zString = "cant move";
        }
        else if (statusId == NameAll.STATUS_ID_FAITH)
        {
            zString = "faith";
        }
        else if (statusId == NameAll.STATUS_ID_INNOCENT)
        {
            zString = "innocent";
        }
        else if (statusId == NameAll.STATUS_ID_OIL)
        {
            zString = "burn";
        }
        else if (statusId == NameAll.STATUS_ID_FLOAT)
        {
            zString = "float";
        }
        else if (statusId == NameAll.STATUS_ID_FROG)
        {
            zString = "frog";
        }
        else if (statusId == NameAll.STATUS_ID_HASTE)
        {
            zString = "haste";
        }
        else if (statusId == NameAll.STATUS_ID_PERFORMING)
        {
            zString = "performing";
        }
        else if (statusId == NameAll.STATUS_ID_PETRIFY)
        {
            zString = "petrify";
        }
        else if (statusId == NameAll.STATUS_ID_PROTECT)
        {
            zString = "protect";
        }
        else if (statusId == NameAll.STATUS_ID_QUICK)
        {
            zString = "quick";
        }
        else if (statusId == NameAll.STATUS_ID_REFLECT)
        {
            zString = "reflect";
        }
        else if (statusId == NameAll.STATUS_ID_REGEN)
        {
            zString = "regen";
        }
        else if (statusId == NameAll.STATUS_ID_RERAISE)
        {
            zString = "reraise";
        }
        else if (statusId == NameAll.STATUS_ID_SHELL)
        {
            zString = "shell";
        }
        else if (statusId == NameAll.STATUS_ID_SILENCE)
        {
            zString = "silence";
        }
        else if (statusId == NameAll.STATUS_ID_SLEEP)
        {
            zString = "sleep";
        }
        else if (statusId == NameAll.STATUS_ID_SLOW)
        {
            zString = "slow";
        }
        else if (statusId == NameAll.STATUS_ID_STOP)
        {
            zString = "stop";
        }
        else if (statusId == NameAll.STATUS_ID_UNDEAD)
        {
            zString = "undead";
        }
        else if (statusId == NameAll.STATUS_ID_APATHY)
        {
            zString = "apathy";
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_AIR)
        {
            zString = "absorb air";
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_DARK)
        {
            zString = "absorb dark";
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_EARTH)
        {
            zString = "absorb earth";
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_LIGHT)
        {
            zString = "absorb light";
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_LIGHTNING)
        {
            zString = "absorb lightning";
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_UNDEAD)
        {
            zString = "absorb undead";
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_WATER)
        {
            zString = "absorb water";
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_WEAPON)
        {
            zString = "absorb weapon";
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_WIND)
        {
            zString = "absorb wind";
        }
        else if (statusId == NameAll.STATUS_ID_HALF_AIR)
        {
            zString = "half air";
        }
        else if (statusId == NameAll.STATUS_ID_HALF_DARK)
        {
            zString = "half dark";
        }
        else if (statusId == NameAll.STATUS_ID_HALF_EARTH)
        {
            zString = "half earth";
        }
        else if (statusId == NameAll.STATUS_ID_HALF_FIRE)
        {
            zString = "half fire";
        }
        else if (statusId == NameAll.STATUS_ID_HALF_LIGHT)
        {
            zString = "half light";
        }
        else if (statusId == NameAll.STATUS_ID_HALF_LIGHTNING)
        {
            zString = "half lightning";
        }
        else if (statusId == NameAll.STATUS_ID_HALF_UNDEAD)
        {
            zString = "half undead";
        }
        else if (statusId == NameAll.STATUS_ID_HALF_WATER)
        {
            zString = "half water";
        }
        else if (statusId == NameAll.STATUS_ID_HALF_WEAPON)
        {
            zString = "half weapon";
        }
        else if (statusId == NameAll.STATUS_ID_HALF_WIND)
        {
            zString = "half wind";
        }
        else if (statusId == NameAll.STATUS_ID_WEAK_AIR)
        {
            zString = "weak air";
        }
        else if (statusId == NameAll.STATUS_ID_WEAK_DARK)
        {
            zString = "weak dark";
        }
        else if (statusId == NameAll.STATUS_ID_WEAK_EARTH)
        {
            zString = "weak earth";
        }
        else if (statusId == NameAll.STATUS_ID_WEAK_FIRE)
        {
            zString = "weak fire";
        }
        else if (statusId == NameAll.STATUS_ID_WEAK_LIGHT)
        {
            zString = "weak light";
        }
        else if (statusId == NameAll.STATUS_ID_WEAK_LIGHTNING)
        {
            zString = "weak lightning";
        }
        else if (statusId == NameAll.STATUS_ID_WEAK_UNDEAD)
        {
            zString = "weak undead";
        }
        else if (statusId == NameAll.STATUS_ID_WEAK_WATER)
        {
            zString = "weak water";
        }
        else if (statusId == NameAll.STATUS_ID_WEAK_WEAPON)
        {
            zString = "weak weapon";
        }
        else if (statusId == NameAll.STATUS_ID_WEAK_WIND)
        {
            zString = "weak wind";
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_AIR)
        {
            zString = "air up";
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_DARK)
        {
            zString = "dark up";
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_EARTH)
        {
            zString = "earth up";
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_FIRE)
        {
            zString = "fire up";
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_LIGHT)
        {
            zString = "light up";
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_LIGHTNING)
        {
            zString = "lightning up";
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_UNDEAD)
        {
            zString = "undead up";
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_WATER)
        {
            zString = "water up";
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_WEAPON)
        {
            zString = "weapon up";
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_WIND)
        {
            zString = "wind up";
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_ALL)
        {
            zString = "elements up";
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_FLOAT_REFLECT)
        {
            zString = "float, reflect";
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_REGEN_RERAISE)
        {
            zString = "regen, reraise";
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_HASTE)
        {
            zString = "haste";
        }
        //else if (statusId == NameAll.STATUS_ID_REFLECT_ITEM)
        //{
        //    zString = "reflect";
        //}
        else if (statusId == NameAll.STATUS_ID_RERAISE_START)
        {
            zString = "reraise";
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_UNDEAD)
        {
            zString = "undead";
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_FLOAT)
        {
            zString = "float";
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_REFLECT)
        {
            zString = "reflect";
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_EARTH_STRENGTHEN_EARTH)
        {
            zString = "absorb earth, earth up";
        }
        //else if (statusId == NameAll.STATUS_ID_ABSORB_HOLY)
        //{
        //    zString = "absorb holy";
        //}
        else if (statusId == NameAll.STATUS_ID_HALF_FIRE_LIGHTNING_ICE)
        {
            zString = "half fire, half lightning, half ice";
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_FIRE_LIGHTNING_ICE)
        {
            zString = "fire up, lightning up, ice up";
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_REGEN)
        {
            zString = "regen";
        }
        else if (statusId == NameAll.STATUS_ID_START_PETRIFY)
        {
            zString = "petrify";
        }
        else if (statusId == NameAll.STATUS_ID_EXCALIBUR)
        {
            zString = "haste, holy up, absorb holy";
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_PROTECT)
        {
            zString = "protect";
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_SHELL)
        {
            zString = "shell";
        }
        else if( statusId == NameAll.STATUS_ID_AUTO_PROTECT_SHELL)
        {
            zString = "protect, shell";
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_FAITH)
        {
            zString = "faith";
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_FIRE_HALF_ICE_WEAK_WATER)
        {
            zString = "absorb fire, half ice, weak water";
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_ICE_HALF_FIRE_WEAK_LIGHTNING)
        {
            zString = "absorb ice, half fire, weak lightning";
        }
        else if (statusId == NameAll.STATUS_ID_STRENGTHEN_ICE)
        {
            zString = "ice up";
        }
        else if (statusId == NameAll.STATUS_ID_ABSORB_ICE)
        {
            zString = "absorb ice";
        }
        else if (statusId == NameAll.STATUS_ID_HALF_ICE)
        {
            zString = "half ice";
        }
        else if (statusId == NameAll.STATUS_ID_WEAK_ICE)
        {
            zString = "weak ice";
        }
        else if (statusId == NameAll.STATUS_ID_DEAD_3)
        {
            zString = "dead_3";
        }
        else if (statusId == NameAll.STATUS_ID_DEAD_2)
        {
            zString = "dead_2";
        }
        else if (statusId == NameAll.STATUS_ID_DEAD_1)
        {
            zString = "dead_1";
        }
        else if (statusId == NameAll.STATUS_ID_DEAD_0)
        {
            zString = "dead_0";
        }
        else if (statusId == NameAll.STATUS_ID_ALL)
        {
            zString = "";
        }
        else if (statusId == NameAll.STATUS_ID_JUMPING)
        {
            zString = "jumping";
        }
        else if (statusId == NameAll.STATUS_ID_LIFE)
        {
            zString = "life";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_ESUNA)
        {
            zString = "esuna";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_PETRIFY)
        {
            zString = "c: petrify";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_FROG)
        {
            zString = "c: weak";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_STIGMA_MAGIC)
        {
            zString = "stigma magic";
        }
        else if (statusId == NameAll.STATUS_ID_FLOAT_MOVE)
        {
            zString = "float";
        }
        else if (statusId == NameAll.STATUS_ID_INVITE)
        {
            zString = "invite";
        }
        else if (statusId == NameAll.STATUS_ID_GOLEM)
        {
            zString = "living wall";
        }
        else if (statusId == NameAll.STATUS_ID_POISON)
        {
            zString = "poison";
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_SLOW)
        {
            zString = "slow";
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_INNOCENT)
        {
            zString = "innocent";
        }
        else if (statusId == NameAll.STATUS_ID_DEATH_SENTENCE_3)
        {
            zString = "death_sentence_3";
        }
        else if (statusId == NameAll.STATUS_ID_DEATH_SENTENCE_2)
        {
            zString = "death_sentence_2";
        }
        else if (statusId == NameAll.STATUS_ID_DEATH_SENTENCE_1)
        {
            zString = "death_sentence_1";
        }
        else if (statusId == NameAll.STATUS_ID_DEATH_SENTENCE_0)
        {
            zString = "death_sentence_0";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_HEAL)
        {
            zString = "heal";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_DISPEL)
        {
            zString = "dispel";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_POISON)
        {
            zString = "cure antidote";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_DARKNESS)
        {
            zString = "cure blind";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_SILENCE)
        {
            zString = "cure silence";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_HOLY_WATER)
        {
            zString = "cure undead";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_REMEDY)
        {
            zString = "remedy";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_SLOW)
        {
            zString = "b: slow";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_CONFUSION_CHARM)
        {
            zString = "b: confusion, charm";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_DONT_MOVE_DONT_ACT)
        {
            zString = "b: cant move, cant act";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_PETRIFY_STOP)
        {
            zString = "b: petrify, stop";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_UNDEAD_BLOOD_SUCK)
        {
            zString = "b: undead";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_SILENCE_BERSERK)
        {
            zString = "b: silence, berserk";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_SLEEP_DEATH_SENTENCE)
        {
            zString = "b: sleep, death sentence";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_DEAD_DARKNESS)
        {
            zString = "b: dead, blind";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_INVITE)
        {
            zString = "b: invite";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_DONT_MOVE_LIGHTNING)
        {
            zString = "b: cant move, lightning";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_LIGHTNING)
        {
            zString = "b: lightning";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_STOP)
        {
            zString = "b: stop";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_DEAD)
        {
            zString = "b: dead";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_SILENCE)
        {
            zString = "b: silence";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_DARKNESS_SLEEP)
        {
            zString = "b: blind, sleep";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_CACHUSA)
        {
            zString = "b: cachusa";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_RIBBON)
        {
            zString = "b: ribbon";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_BARETTE)
        {
            zString = "b: barette";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_BERSERK)
        {
            zString = "b: berserk";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_BLOOD_SUCK)
        {
            zString = "";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_CHARGING)
        {
            zString = "";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_CHARM)
        {
            zString = "b: charm";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_CHICKEN)
        {
            zString = "b: fear";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_CONFUSION)
        {
            zString = "b: confusion";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_CRITICAL)
        {
            zString = "";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_CRYSTAL)
        {
            zString = "b: crystal";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_DARKNESS)
        {
            zString = "b: blind";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_DEATH_SENTENCE)
        {
            zString = "b: death sentence";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_DEFENDING)
        {
            zString = "";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_DONT_ACT)
        {
            zString = "b: cant act";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_DONT_MOVE)
        {
            zString = "b: cant move";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_FAITH)
        {
            zString = "b: faith";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_FLOAT)
        {
            zString = "b: float";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_FROG)
        {
            zString = "b: weak";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_HASTE)
        {
            zString = "b: haste";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_INNOCENT)
        {
            zString = "b: innocent";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_OIL)
        {
            zString = "b: burn";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_PERFORMING)
        {
            zString = "";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_PETRIFY)
        {
            zString = "b: petrify";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_PROTECT)
        {
            zString = "b: protect";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_QUICK)
        {
            zString = "b: quick";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_REFLECT)
        {
            zString = "b: reflect";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_REGEN)
        {
            zString = "b: regen";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_RERAISE)
        {
            zString = "b: reraise";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_SHELL)
        {
            zString = "b: shell";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_SLEEP)
        {
            zString = "b: sleep";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_UNDEAD)
        {
            zString = "b: undead";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_APATHY)
        {
            zString = "b: apathy";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_POISON)
        {
            zString = "b: poison";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_NECKLACE_1)
        {
//            1   2   3   4   5   6
//dead petrify darkness silence don’t act   weak
//death sentence chicken berserk charm   confusion apathy
//don’t move  innocent faith   undead oil slow
//stop                poison

        zString = "b: dead, death sentence, cant move, stop";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_NECKLACE_2)
        {
            zString = "b: petrify, fear, innocent";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_NECKLACE_3)
        {
            zString = "b: blind, berserk, faith";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_NECKLACE_4)
        {
            zString = "b: silence, charm, undead";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_NECKLACE_5)
        {
            zString = "b: cant act, confusion, burn";
        }
        else if (statusId == NameAll.STATUS_ID_BLOCK_NECKLACE_6)
        {
            zString = "b: weak, apathy, slow, poison";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_OCTAGON)
        {
            zString = "c: octagon";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_DEATH_SENTENCE)
        {
            zString = "c: death sentence";
        }
        else if (statusId == NameAll.STATUS_ID_NAMELESS_SONG)
        {
            zString = "Nameless Song";
        }
        else if (statusId == NameAll.STATUS_ID_NAMELESS_DANCE)
        {
            zString = "Nameless Dance";
        }
        else if (statusId == NameAll.STATUS_ID_PROTECT_SHELL)
        {
            zString = "protect shell";
        }
        else if (statusId == NameAll.STATUS_ID_REGEN_HASTE)
        {
            zString = "regen haste";
        }
        else if (statusId == NameAll.STATUS_ID_DEFENDING)
        {
            zString = "guard";
        }
        else if (statusId == NameAll.STATUS_ID_OIL_FAITH)
        {
            zString = "burn, faith";
        }
        else if (statusId == NameAll.STATUS_ID_ZOMBIE_LIFE)
        {
            zString = "zombie, life";
        }
        else if (statusId == NameAll.STATUS_ID_CURE_STATUS_2)
        {
            //undead, charm, faith, innocent, reflect, stop, slow

            zString = "cure some statuses";//undead, charm, max/min WIS, reflect, stop, slow
        }
        else if (statusId == NameAll.STATUS_ID_HASTE_DS)
        {
            zString = "haste, doom";
        }
        else if (statusId == NameAll.STATUS_ID_ADD_POSITIVE)
        {
            zString = "";
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_NEGATIVE)
        {
            zString = "panacea";
        }
        else if (statusId == NameAll.STATUS_ID_BERSERK_START)
        {
            zString = "berserk";
        }
        else if (statusId == NameAll.STATUS_ID_UNCONSCIOUS)
        {
            zString = "unconscious";
        }
        else if( statusId > 1000) //spell effects on weapon
        {
            zString = " cast:" + SpellManager.Instance.GetSpellNameByIndex(statusId - 1000).AbilityName;
        }
        else
        {
            zString = "Unknown"; Debug.Log("unknown status, status id is " + statusId);
        }
        return zString;
    }

    //builds dictionaries for scroll lists for custom item
    #region ItemBuilder dicts
    static Dictionary<int, string> itemBlockDictionary;

    public static Dictionary<int,string> GetItemBlockDictionary()
    {
        if (itemBlockDictionary == null)
            BuildItemBlockDictionary();

        return itemBlockDictionary;
    }

    static void BuildItemBlockDictionary()
    {
        itemBlockDictionary = new Dictionary<int, string>();
        itemBlockDictionary.Add(STATUS_ID_NONE, STATUS_NAME_NONE);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_SLOW, STATUS_NAME_BLOCK_SLOW);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_CONFUSION_CHARM, STATUS_NAME_BLOCK_CONFUSION_CHARM);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_DONT_MOVE_DONT_ACT, STATUS_NAME_BLOCK_DONT_MOVE_DONT_ACT);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_PETRIFY_STOP, STATUS_NAME_BLOCK_PETRIFY_STOP);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_UNDEAD_BLOOD_SUCK, STATUS_NAME_BLOCK_UNDEAD_BLOOD_SUCK);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_BERSERK, STATUS_NAME_BLOCK_BERSERK);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_DEATH_SENTENCE, STATUS_NAME_BLOCK_DEATH_SENTENCE);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_DEAD_DARKNESS, STATUS_NAME_BLOCK_DEAD_DARKNESS);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_INVITE, STATUS_NAME_BLOCK_INVITE);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_DONT_MOVE_LIGHTNING, STATUS_NAME_BLOCK_DONT_MOVE_LIGHTNING);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_LIGHTNING, STATUS_NAME_BLOCK_LIGHTNING);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_STOP, STATUS_NAME_BLOCK_STOP);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_DEAD, STATUS_NAME_BLOCK_DEAD);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_SILENCE, STATUS_NAME_BLOCK_SILENCE);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_DARKNESS_SLEEP, STATUS_NAME_BLOCK_DARKNESS_SLEEP);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_CACHUSA, STATUS_NAME_BLOCK_CACHUSA);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_RIBBON, STATUS_NAME_BLOCK_RIBBON);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_BARETTE, STATUS_NAME_BLOCK_BARETTE);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_BLOOD_SUCK, STATUS_NAME_BLOCK_BLOOD_SUCK);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_CHARGING, STATUS_NAME_BLOCK_CHARGING);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_CHARM, STATUS_NAME_BLOCK_CHARM);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_CHICKEN, STATUS_NAME_BLOCK_CHICKEN);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_CONFUSION, STATUS_NAME_BLOCK_CONFUSION);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_CRITICAL, STATUS_NAME_BLOCK_CRITICAL);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_CRYSTAL, STATUS_NAME_BLOCK_CRYSTAL);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_DARKNESS, STATUS_NAME_BLOCK_DARKNESS);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_DEFENDING, STATUS_NAME_BLOCK_DEFENDING);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_DONT_ACT, STATUS_NAME_BLOCK_DONT_ACT);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_DONT_MOVE, STATUS_NAME_BLOCK_DONT_MOVE);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_FAITH, STATUS_NAME_BLOCK_FAITH);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_FLOAT, STATUS_NAME_BLOCK_FLOAT);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_FROG, STATUS_NAME_BLOCK_FROG);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_HASTE, STATUS_NAME_BLOCK_HASTE);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_INNOCENT, STATUS_NAME_BLOCK_INNOCENT);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_OIL, STATUS_NAME_BLOCK_OIL);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_PERFORMING, STATUS_NAME_BLOCK_PERFORMING);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_PETRIFY, STATUS_NAME_BLOCK_PETRIFY);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_PROTECT, STATUS_NAME_BLOCK_PROTECT);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_QUICK, STATUS_NAME_BLOCK_QUICK);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_REFLECT, STATUS_NAME_BLOCK_REFLECT);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_REGEN, STATUS_NAME_BLOCK_REGEN);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_RERAISE, STATUS_NAME_BLOCK_RERAISE);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_SHELL, STATUS_NAME_BLOCK_SHELL);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_SLEEP, STATUS_NAME_BLOCK_SLEEP);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_UNDEAD, STATUS_NAME_BLOCK_UNDEAD);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_APATHY, STATUS_NAME_BLOCK_APATHY);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_POISON, STATUS_NAME_BLOCK_POISON);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_NECKLACE_1, STATUS_NAME_BLOCK_NECKLACE_1);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_NECKLACE_2, STATUS_NAME_BLOCK_NECKLACE_2);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_NECKLACE_3, STATUS_NAME_BLOCK_NECKLACE_3);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_NECKLACE_4, STATUS_NAME_BLOCK_NECKLACE_4);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_NECKLACE_5, STATUS_NAME_BLOCK_NECKLACE_5);
        itemBlockDictionary.Add(STATUS_ID_BLOCK_NECKLACE_6, STATUS_NAME_BLOCK_NECKLACE_6);
    }

    static Dictionary<int, string> itemAutoStatusDictionary;

    public static Dictionary<int, string> GetItemAutoStatusDictionary()
    {
        if (itemAutoStatusDictionary == null)
            BuildItemAutoStatusDictionary();

        return itemAutoStatusDictionary;
    }

    static void BuildItemAutoStatusDictionary()
    {
        itemAutoStatusDictionary = new Dictionary<int, string>();
        itemAutoStatusDictionary.Add(STATUS_ID_NONE, STATUS_NAME_NONE);
        itemAutoStatusDictionary.Add(STATUS_ID_AUTO_FLOAT_REFLECT, STATUS_NAME_AUTO_FLOAT_REFLECT);
        itemAutoStatusDictionary.Add(STATUS_ID_AUTO_PROTECT_SHELL, STATUS_NAME_AUTO_PROTECT_SHELL);
        itemAutoStatusDictionary.Add(STATUS_ID_AUTO_REGEN_RERAISE, STATUS_NAME_AUTO_REGEN_RERAISE);
        itemAutoStatusDictionary.Add(STATUS_ID_AUTO_HASTE, STATUS_NAME_AUTO_HASTE);
        itemAutoStatusDictionary.Add(STATUS_ID_AUTO_UNDEAD, STATUS_NAME_AUTO_UNDEAD);
        itemAutoStatusDictionary.Add(STATUS_ID_AUTO_FLOAT, STATUS_NAME_AUTO_FLOAT);
        itemAutoStatusDictionary.Add(STATUS_ID_AUTO_REFLECT, STATUS_NAME_AUTO_REFLECT);
        itemAutoStatusDictionary.Add(STATUS_ID_AUTO_REGEN, STATUS_NAME_AUTO_REGEN);
        itemAutoStatusDictionary.Add(STATUS_ID_AUTO_PROTECT, STATUS_NAME_AUTO_PROTECT);
        itemAutoStatusDictionary.Add(STATUS_ID_AUTO_SHELL, STATUS_NAME_AUTO_SHELL);
        itemAutoStatusDictionary.Add(STATUS_ID_AUTO_FAITH, STATUS_NAME_AUTO_FAITH);
        itemAutoStatusDictionary.Add(STATUS_ID_AUTO_SLOW, STATUS_NAME_AUTO_SLOW);
        itemAutoStatusDictionary.Add(STATUS_ID_AUTO_INNOCENT, STATUS_NAME_AUTO_INNOCENT);
    }

    static Dictionary<int, string> itemOnHitDictionary;

    public static Dictionary<int, string> GetItemOnHitDictionary()
    {
        if (itemOnHitDictionary == null)
            BuildItemOnHitDictionary();

        return itemOnHitDictionary;
    }

    static void BuildItemOnHitDictionary()
    {
        itemOnHitDictionary = new Dictionary<int, string>();
        itemOnHitDictionary.Add(STATUS_ID_NONE, STATUS_NAME_NONE);
        itemOnHitDictionary.Add(STATUS_ID_BERSERK, STATUS_NAME_BERSERK);
        //itemOnHitDictionary.Add(STATUS_ID_BLOOD_SUCK, STATUS_NAME_);
        //itemOnHitDictionary.Add(STATUS_ID_CHARGING, STATUS_NAME_);
        itemOnHitDictionary.Add(STATUS_ID_CHARM, STATUS_NAME_CHARM);
        //itemOnHitDictionary.Add(STATUS_ID_CHICKEN, STATUS_NAME_);
        itemOnHitDictionary.Add(STATUS_ID_CONFUSION, STATUS_NAME_CONFUSION);
        //itemOnHitDictionary.Add(STATUS_ID_CRITICAL, STATUS_NAME_);
        //itemOnHitDictionary.Add(STATUS_ID_CRYSTAL, STATUS_NAME_);
        itemOnHitDictionary.Add(STATUS_ID_DARKNESS, STATUS_NAME_DARKNESS);
        itemOnHitDictionary.Add(STATUS_ID_DEAD, STATUS_NAME_DEAD);
        itemOnHitDictionary.Add(STATUS_ID_DEATH_SENTENCE, STATUS_NAME_DEATH_SENTENCE);
        itemOnHitDictionary.Add(STATUS_ID_DEFENDING, STATUS_NAME_DEFENDING);
        itemOnHitDictionary.Add(STATUS_ID_DONT_ACT, STATUS_NAME_DONT_ACT);
        itemOnHitDictionary.Add(STATUS_ID_DONT_MOVE, STATUS_NAME_DONT_MOVE);
        itemOnHitDictionary.Add(STATUS_ID_FAITH, STATUS_NAME_FAITH);
        itemOnHitDictionary.Add(STATUS_ID_FLOAT, STATUS_NAME_FLOAT);
        itemOnHitDictionary.Add(STATUS_ID_FROG, STATUS_NAME_FROG);
        itemOnHitDictionary.Add(STATUS_ID_HASTE, STATUS_NAME_HASTE);
        itemOnHitDictionary.Add(STATUS_ID_INNOCENT, STATUS_NAME_INNOCENT);
        itemOnHitDictionary.Add(STATUS_ID_OIL, STATUS_NAME_OIL);
        //itemOnHitDictionary.Add(STATUS_ID_PERFORMING, STATUS_NAME_);
        itemOnHitDictionary.Add(STATUS_ID_PETRIFY, STATUS_NAME_PETRIFY);
        itemOnHitDictionary.Add(STATUS_ID_PROTECT, STATUS_NAME_PROTECT);
        itemOnHitDictionary.Add(STATUS_ID_QUICK, STATUS_NAME_QUICK);
        itemOnHitDictionary.Add(STATUS_ID_REFLECT, STATUS_NAME_REFLECT);
        itemOnHitDictionary.Add(STATUS_ID_REGEN, STATUS_NAME_REGEN);
        itemOnHitDictionary.Add(STATUS_ID_RERAISE, STATUS_NAME_RERAISE);
        itemOnHitDictionary.Add(STATUS_ID_SHELL, STATUS_NAME_SHELL);
        itemOnHitDictionary.Add(STATUS_ID_SILENCE, STATUS_NAME_SILENCE);
        itemOnHitDictionary.Add(STATUS_ID_SLEEP, STATUS_NAME_SLEEP);
        itemOnHitDictionary.Add(STATUS_ID_SLOW, STATUS_NAME_SLOW);
        itemOnHitDictionary.Add(STATUS_ID_STOP, STATUS_NAME_STOP);
        itemOnHitDictionary.Add(STATUS_ID_UNDEAD, STATUS_NAME_UNDEAD);
        //itemOnHitDictionary.Add(STATUS_ID_APATHY, STATUS_NAME_);
        itemOnHitDictionary.Add(STATUS_ID_POISON, STATUS_NAME_POISON);;
    }
    #endregion



    #region Icons for CustomClasses
    public static readonly string ICON_NAME_ARCHER = "Archer";
    public static readonly string ICON_NAME_BOX_MAN = "Box Man";
    public static readonly string ICON_NAME_FLOWER_GIRL = "Flower Girl";
    public static readonly string ICON_NAME_FROG_MAN = "Frog Man";
    public static readonly string ICON_NAME_GREEN_WARRIOR = "Green Warrior";
    public static readonly string ICON_NAME_KING = "King";
    public static readonly string ICON_NAME_KNIGHT = "Knight";
    public static readonly string ICON_NAME_NURSE = "Nurse";
    public static readonly string ICON_NAME_PIRATE = "Pirate";
    public static readonly string ICON_NAME_SURGEON = "Surgeon";
    public static readonly string ICON_NAME_SURGEON_ZOMBIE = "Surgeon Zombie";
    public static readonly string ICON_NAME_UNDEAD = "Undead";
    public static readonly string ICON_NAME_VAMPIRE = "Vampire";
    public static readonly string ICON_NAME_WAR_BEAR = "War Bear";
    public static readonly string ICON_NAME_WARRIOR = "Warrior";
    public static readonly string ICON_NAME_WIZARD = "Wizard";
    public static readonly string ICON_NAME_WOOPA = "Woopa";

    public static readonly int ICON_ID_ARCHER = 0;
    public static readonly int ICON_ID_BOX_MAN = 1;
    public static readonly int ICON_ID_FLOWER_GIRL = 2;
    public static readonly int ICON_ID_FROG_MAN = 3;
    public static readonly int ICON_ID_GREEN_WARRIOR = 4;
    public static readonly int ICON_ID_KING = 5;
    public static readonly int ICON_ID_KNIGHT = 6;
    public static readonly int ICON_ID_NURSE = 7;
    public static readonly int ICON_ID_PIRATE = 8;
    public static readonly int ICON_ID_SURGEON = 9;
    public static readonly int ICON_ID_SURGEON_ZOMBIE = 10;
    public static readonly int ICON_ID_UNDEAD = 11;
    public static readonly int ICON_ID_VAMPIRE = 12;
    public static readonly int ICON_ID_WAR_BEAR = 13;
    public static readonly int ICON_ID_WARRIOR = 14;
    public static readonly int ICON_ID_WIZARD = 15;
    public static readonly int ICON_ID_WOOPA = 16;

    public static Dictionary<int,string> GetIconDict()
    {
        Dictionary<int, string> retValue = new Dictionary<int, string>();
        retValue.Add(ICON_ID_ARCHER, ICON_NAME_ARCHER);
        retValue.Add(ICON_ID_BOX_MAN, ICON_NAME_BOX_MAN);
        retValue.Add(ICON_ID_FLOWER_GIRL,ICON_NAME_FLOWER_GIRL);
        retValue.Add(ICON_ID_FROG_MAN, ICON_NAME_FROG_MAN);
        retValue.Add(ICON_ID_GREEN_WARRIOR,ICON_NAME_GREEN_WARRIOR);
        retValue.Add(ICON_ID_KING,ICON_NAME_KING);
        retValue.Add(ICON_ID_KNIGHT,ICON_NAME_KNIGHT);
        retValue.Add(ICON_ID_NURSE,ICON_NAME_NURSE);
        retValue.Add(ICON_ID_PIRATE,ICON_NAME_PIRATE);
        retValue.Add(ICON_ID_SURGEON,ICON_NAME_SURGEON);
        retValue.Add(ICON_ID_SURGEON_ZOMBIE,ICON_NAME_SURGEON_ZOMBIE);
        retValue.Add(ICON_ID_UNDEAD,ICON_NAME_UNDEAD);
        retValue.Add(ICON_ID_VAMPIRE,ICON_NAME_VAMPIRE);
        retValue.Add(ICON_ID_WARRIOR,ICON_NAME_WARRIOR);
        retValue.Add(ICON_ID_WAR_BEAR,ICON_NAME_WAR_BEAR);
        retValue.Add(ICON_ID_WIZARD,ICON_NAME_WIZARD);
        retValue.Add(ICON_ID_WOOPA,ICON_NAME_WOOPA);
        return retValue;
    }

    public static string GetIconString(int iconId)
    {
		return "Heroes/default_puo"; //restore functionality at some point for this
        string zString = "Heroes/box_man";
       
        if (iconId == ICON_ID_ARCHER)
        {
            zString = "Heroes/sparcher";
        }
        else if(iconId == ICON_ID_BOX_MAN )
        {
            zString = "Heroes/box_man";
        }
        else if (iconId == ICON_ID_FLOWER_GIRL)
        {
            zString = "Heroes/magic_plant";
        }
        else if (iconId == ICON_ID_FROG_MAN)
        {
            zString = "Heroes/fogaman";
        }
        else if (iconId == ICON_ID_GREEN_WARRIOR)
        {
            zString = "Heroes/greenwar";
        }
        else if (iconId == ICON_ID_KING)
        {
            zString = "Heroes/king";
        }
        else if (iconId == ICON_ID_KNIGHT)
        {
            zString = "Heroes/knight";
        }
        else if (iconId == ICON_ID_NURSE)
        {
            zString = "Heroes/nurse";
        }
        else if (iconId == ICON_ID_PIRATE)
        {
            zString = "Heroes/pirate";
        }
        else if (iconId == ICON_ID_SURGEON)
        {
            zString = "Heroes/surgeon";
        }
        else if (iconId == ICON_ID_SURGEON_ZOMBIE)
        {
            zString = "Heroes/surgeon_zombie";
        }
        else if (iconId == ICON_ID_UNDEAD)
        {
            zString = "Heroes/undeath_2";
        }
        else if (iconId == ICON_ID_VAMPIRE)
        {
            zString = "Heroes/vam";
        }
        else if (iconId == ICON_ID_WARRIOR)
        {
            zString = "Heroes/warrior";
        }
        else if (iconId == ICON_ID_WAR_BEAR)
        {
            zString = "Heroes/war_bear";
        }
        else if (iconId == ICON_ID_WIZARD)
        {
            zString = "Heroes/wizard";
        }
        else if (iconId == ICON_ID_WOOPA)
        {
            zString = "Heroes/woopa";
        }
        return zString;
    }

    public static string GetIconStringFromClass(int classId)
    {
        ClassEditObject ce = CalcCode.LoadCEObject(classId);
        return GetIconString(ce.Icon);
    }
    #endregion

    //death mode: dead units either go unconscious and stay dead or go through crystal phase
    public static readonly int DEATH_MODE_DEATH = 0;
    public static readonly int DEATH_MODE_UNCONSCIOUS = 1;

	//combat mode, set in CombatStateInit. logic checks used in various places like deciding how to end combat etc
	public static readonly int COMBAT_MODE_DEFAULT = 0;
	public static readonly int COMBAT_MODE_RL_DUEL = 1;
}