using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerUnitObjectText.PUOText;

public class PlayerManager : Singleton<PlayerManager>
{

	private static List<PlayerUnit> sPlayerUnitList;
	private static List<GameObject> sPlayerObjectList;
	private static List<PlayerUnit> sGreenList;
	private static List<PlayerUnit> sRedList;
	private static List<WalkAroundActionObject> sWalkAroundActionList;
	private static Dictionary<int, bool> sWalkAroundLockDict;
	private static Dictionary<int, int> sWalkAroundPlayerWaao; //keeps track of which players have active waao's in queue. in case player enters new waao, look here to see if replace needed
	private static Dictionary<int, List<SpellSlow>> sWalkAroundSpellSlowQueue;

	private static bool isMimeOnGreen;// = false;
	private static bool isMimeOnRed;// = false;

	private static bool isWalkAroundMoveAllowed;
	private static int walkAroundTick; //goes from 0 to MAX_WALK_AROUND_CTR then back to 0
	private const int HAS_WAAO_IN_QUEUE = 0;
	private const int HAS_SS_IN_QUEUE = 1;
	private const int HAS_SS_IN_SPELL_MANAGER = 2; //SS is in spell manager but not resolved
	private const int MAX_WALK_AROUND_CTR = 11; //any slow action with CTR > this will equal this - 1

	static int currentTick;
	const string TickMenuAdd = "TickMenu.AddItem"; //notification picked up to displayer current tick (UIMenuMenu)
	static readonly string TickKey = "roomTick";
	public static CombatMultiplayerObject sMPObject;
	public static int sGameMode; //combat or walkaround
	public static int sRenderMode; //normal or no render. not really no render mode more like don't do game object or stuff that slows us down

	static int sUniqueTeamId;
	Dictionary<Point, Alliances> sAllianceDict; //during battle this dictionary is called to check alliances. No sets, no tuples so using point for the two team Ids. fucking stupid
	Dictionary<int, int> sTempIdToTeamIdDict; //used when assigning TeamIds so that units with the same tempId end up on the same team
	Dictionary<int, int> sTeamIdToType; //used to assign alliances for teams at start of battle and when more teams join
	Dictionary<int, bool> sTeamIdToIsAbleToFight; //used to tell if/which teams are able to fight and if combat should end
	Dictionary<int, List<int>> sTeamIdToPU; //easier access to playerUnit lists for faster searches/easier to tell if combat should end or not
	Dictionary<Point, bool> sTeamEnemyDict; //stores which teams are enemies, used for telling if combat should end or not
	static int sLocalTeamId; //teamId of the player, used for telling alliances 

	//PhotonView photonView;

	private static Dictionary<Tuple<int, int>, Tuple<int, int>> sWalkAroundMapDictionary; //stores map data like seed, current maps, and maps visited. x,y coordinates are the tuples where applicable

	//saving combatLog stuff
	static List<CombatLogSaveObject> sCombatLogList; //stores CombatLogSaveObject for saving to file
	static int COMBAT_LOG_SAVE_EVERY = 100;
	int combatLogId;  //actual idea of the combat log save
	int timeInt; //used for saving things like the CombatLog
	bool isSaveCombatLog;


	void Awake()
	{
		//photonView = PhotonView.Get(this);
		currentTick = 0;
		sUniqueTeamId = 999;
		sLocalTeamId = NameAll.TEAM_ID_GREEN;
		isWalkAroundMoveAllowed = false;
		walkAroundTick = 0;

		isSaveCombatLog = true;
		timeInt = (int)Time.time;
		combatLogId = -1;
		sCombatLogList = new List<CombatLogSaveObject>();
	}


	protected PlayerManager()
	{ // guarantee this will be always a singleton only - can't use the constructor!
	  //myGlobalVar = "asdf";
		sPlayerUnitList = new List<PlayerUnit>();
		sPlayerObjectList = new List<GameObject>();
		sGreenList = new List<PlayerUnit>();
		sRedList = new List<PlayerUnit>();
		sMPObject = new CombatMultiplayerObject();

		sWalkAroundActionList = new List<WalkAroundActionObject>();
		sWalkAroundLockDict = new Dictionary<int, bool>();
		sWalkAroundPlayerWaao = new Dictionary<int, int>();
		sWalkAroundSpellSlowQueue = new Dictionary<int, List<SpellSlow>>();
		//populate the dictionary so don't have to check if key exists each time
		for (int i = 0; i < MAX_WALK_AROUND_CTR; i++)
		{
			sWalkAroundSpellSlowQueue[i] = new List<SpellSlow>();
		}

		sAllianceDict = new Dictionary<Point, Alliances>();
		sTempIdToTeamIdDict = new Dictionary<int, int>();
		sTeamIdToType = new Dictionary<int, int>();
		sTeamIdToIsAbleToFight = new Dictionary<int, bool>();
		sTeamIdToPU = new Dictionary<int, List<int>>();
		sTeamEnemyDict = new Dictionary<Point, bool>();

		sWalkAroundMapDictionary = new Dictionary<Tuple<int, int>, Tuple<int, int>>();

	}


	//called in scene transitions, need to clear the lists
	public void ClearLists()
	{
		sPlayerUnitList = new List<PlayerUnit>();
		sPlayerObjectList = new List<GameObject>();
		sGreenList = new List<PlayerUnit>();
		sRedList = new List<PlayerUnit>();

	}

	//called in WA mode when switching boards
	public void ClearNewBoard()
	{
		sPlayerUnitList = new List<PlayerUnit>();
		sPlayerObjectList = new List<GameObject>();
		//sGreenList = new List<PlayerUnit>(); //player's units are sent to sGreenList when exiting hte map
		sRedList = new List<PlayerUnit>();

		sWalkAroundActionList = new List<WalkAroundActionObject>();
		sWalkAroundLockDict = new Dictionary<int, bool>();
		sWalkAroundPlayerWaao = new Dictionary<int, int>();
		sWalkAroundSpellSlowQueue = new Dictionary<int, List<SpellSlow>>();
		//populate the dictionary so don't have to check if key exists each time
		for (int i = 0; i < MAX_WALK_AROUND_CTR; i++)
		{
			sWalkAroundSpellSlowQueue[i] = new List<SpellSlow>();
		}

		sAllianceDict = new Dictionary<Point, Alliances>();
		sTempIdToTeamIdDict = new Dictionary<int, int>();
		sTeamIdToType = new Dictionary<int, int>();
		sTeamIdToIsAbleToFight = new Dictionary<int, bool>();
		sTeamIdToPU = new Dictionary<int, List<int>>();
		sTeamEnemyDict = new Dictionary<Point, bool>();
		walkAroundTick = 0;
	}

	public void ClearDuelRLBoard()
	{
		sPlayerUnitList = new List<PlayerUnit>();
		sPlayerObjectList = new List<GameObject>();
		sGreenList = new List<PlayerUnit>(); 
		sRedList = new List<PlayerUnit>();

		//sWalkAroundActionList = new List<WalkAroundActionObject>();
		//sWalkAroundLockDict = new Dictionary<int, bool>();
		//sWalkAroundPlayerWaao = new Dictionary<int, int>();
		//sWalkAroundSpellSlowQueue = new Dictionary<int, List<SpellSlow>>();
		////populate the dictionary so don't have to check if key exists each time
		//for (int i = 0; i < MAX_WALK_AROUND_CTR; i++)
		//{
		//	sWalkAroundSpellSlowQueue[i] = new List<SpellSlow>();
		//}

		sAllianceDict = new Dictionary<Point, Alliances>();
		sTempIdToTeamIdDict = new Dictionary<int, int>();
		sTeamIdToType = new Dictionary<int, int>();
		sTeamIdToIsAbleToFight = new Dictionary<int, bool>();
		sTeamIdToPU = new Dictionary<int, List<int>>();
		sTeamEnemyDict = new Dictionary<Point, bool>();
		walkAroundTick = 0;
	}

	public void ClearForReset()
	{
		sPlayerUnitList = new List<PlayerUnit>();
		sPlayerObjectList = new List<GameObject>();
		sAllianceDict = new Dictionary<Point, Alliances>();
		sTempIdToTeamIdDict = new Dictionary<int, int>();
		sTeamIdToType = new Dictionary<int, int>();
		sTeamIdToIsAbleToFight = new Dictionary<int, bool>();
		sTeamIdToPU = new Dictionary<int, List<int>>();
		sTeamEnemyDict = new Dictionary<Point, bool>();
		walkAroundTick = 0;
	}

	#region PreCombat
	//PlayerManager is created in the pre-game (MPGameController) and persists through the scene load into combat

	//[PunRPC]
	public void AddToListRPC(string puCode, int teamId, int type)
	{
		//Debug.Log("why is this beign called in Combat and from where?");
		PlayerUnit pu = CalcCode.BuildPlayerUnit(puCode);
		if (teamId == 2)
		{
			sGreenList.Add(pu);
		}
		else
		{
			sRedList.Add(pu);
		}
	}

	//[PunRPC]
	public void RemoveFromListRPC(int teamId)
	{
		if (teamId == 2)
		{
			if (sGreenList.Count > 0)
			{
				sGreenList.RemoveAt(sGreenList.Count - 1);
			}
		}
		else
		{
			if (sRedList.Count > 0)
			{
				sRedList.RemoveAt(sRedList.Count - 1);
			}
		}
	}

	//called in WA mode when loading PU lists from save or from map transition
	public void SetTeamList(List<PlayerUnit> puList, int teamId)
	{
		if (teamId == NameAll.TEAM_ID_GREEN)
		{
			sGreenList = puList;
		}
		else if (teamId == NameAll.TEAM_ID_RED)
		{
			sRedList = puList;
		}
		else if (teamId == NameAll.TEAM_ID_WALK_AROUND_GREEN)
		{
			Debug.Log("setting team list");
			sGreenList = puList;
			var tempUnitList = PlayerManager.Instance.GetTeamList(NameAll.TEAM_ID_WALK_AROUND_GREEN);
			Debug.Log("setting team list " + tempUnitList.Count);
		}
	}

	public void EditTeamLists(PlayerUnit pu, int teamId, int type)
	{
		if (type == NameAll.TEAM_LIST_CLEAR)
		{
			sRedList.Clear();
			sGreenList.Clear();
			return;
		}

		if (type == NameAll.TEAM_LIST_ADD)
		{
			if (false)//if(!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
			{
				//string puCode = CalcCode.BuildStringFromPlayerUnit(pu);
				//photonView.RPC("AddToListRPC", PhotonTargets.All, new object[] { puCode, teamId, NameAll.TEAM_LIST_ADD });
			}
			else
			{
				if (teamId == NameAll.TEAM_ID_GREEN)
				{
					sGreenList.Add(pu);
				}
				else
				{
					sRedList.Add(pu);
				}
			}
		}
		else if (type == NameAll.TEAM_LIST_REMOVE)
		{
			if (false)//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
			{
				//photonView.RPC("RemoveFromListRPC", PhotonTargets.All, new object[] { teamId });
			}
			else
			{
				if (teamId == NameAll.TEAM_ID_GREEN)
				{
					if (sGreenList.Count > 0)
					{
						sGreenList.RemoveAt(sGreenList.Count - 1);
					}
				}
				else
				{
					if (sRedList.Count > 0)
					{
						sRedList.RemoveAt(sRedList.Count - 1);
					}
				}
			}
		}
	}

	public List<PlayerUnit> GetTeamList(int teamId)
	{
		if (teamId == NameAll.TEAM_ID_GREEN)
		{
			return sGreenList;
		}
		else if (teamId == NameAll.TEAM_ID_WALK_AROUND_GREEN) //walkaround init state, get green list units
		{
			return sGreenList;
		}
		else
		{
			return sRedList;
		}
	}

	public void ClearTeamLists()
	{
		sGreenList.Clear();
		sRedList.Clear();
	}
	#endregion

	//units added in SetUpMap (to masterClient and P2)
	public void AddPlayerUnit(PlayerUnit pu)
	{
		sPlayerUnitList.Add(pu);
	}

	public void AddPlayerObject(GameObject po)
	{
		sPlayerObjectList.Add(po);
	}

	//doing it by index
	public PlayerUnit GetPlayerUnit(int unit_id)
	{
		//try
		//{
		//    return sPlayerUnitList[unit_id];
		//}
		//catch (ArgumentOutOfRangeException)
		//{
		//    Debug.Log("ERROR: GetPlayerUnit returns null " + unit_id);
		//    return null;
		//}
		//catch (IndexOutOfRangeException)
		//{
		//    Debug.Log("ERROR: GetPlayerUnit returns null " + unit_id);
		//    return null;
		//}
		return sPlayerUnitList[unit_id];
		//return null;
	}

	//doing it by index
	public GameObject GetPlayerUnitObject(int unit_id)
	{
		return sPlayerObjectList[unit_id];
		//try
		//{
		//    return sPlayerObjectList[unit_id];
		//}
		//catch (IndexOutOfRangeException)
		//{
		//    return null;
		//}

		//return null;
	}

	public PlayerUnitObject GetPlayerUnitObjectComponent(int unitId)
	{
		GameObject p = GetPlayerUnitObject(unitId);
		return p.GetComponent<PlayerUnitObject>();
	}

	public List<PlayerUnit> GetPlayerUnitList()
	{
		//List<PlayerUnit> puList = new List<PlayerUnit>();
		//foreach( PlayerUnit pu in sPlayerUnitList)
		//{
		//    if( !StatusManager.Instance.IfStatusByUnitAndId(pu.TurnOrder,NameAll.STATUS_ID_CRYSTAL))
		//    {
		//        puList.Add(pu);
		//    }
		//}
		//return puList;
		return sPlayerUnitList;
	}

	//increments the CT. 
	//non-essential part: updates current tick for testing and for MP display to test if host and client are in sync and on the same tick
	//[PunRPC]
	public void IncrementCTPhase()
	{
		currentTick += 1;
		Dictionary<string, int> tempDict = new Dictionary<string, int>();
		tempDict.Add("currentTick", currentTick);

		foreach (PlayerUnit p in sPlayerUnitList)
		{
			p.AddCT(); //checks for statuses
		}

		//if (!PhotonNetwork.offlineMode )
		//{
		//    Room room = PhotonNetwork.room;
		//    if (PhotonNetwork.isMasterClient)
		//    {
		//        ExitGames.Client.Photon.Hashtable turnProps = new ExitGames.Client.Photon.Hashtable();
		//        turnProps[TickKey] = currentTick;
		//        room.SetCustomProperties(turnProps);

		//        photonView.RPC("IncrementCTPhase", PhotonTargets.Others, new object[] { });
		//    }

		//    if (room == null || room.customProperties == null || !room.customProperties.ContainsKey(TickKey))
		//    {
		//        tempDict.Add("roomTick", 0);
		//    }
		//    else
		//    {
		//        tempDict.Add("roomTick", (int)room.customProperties[TickKey]); 
		//    }


		//}
		if( sRenderMode != NameAll.PP_RENDER_NONE)
			this.PostNotification(TickMenuAdd, tempDict);
	}

	public PlayerUnit GetNextActiveTurnPlayerUnit(bool isSetQuickFlagToFalse)
	{
		//loops through looking for quick flag and if CT over 100
		//units are ordered by turn order so just looking for the first one
		PlayerUnit retValue = null;
		int count = sPlayerUnitList.Count;
		for (int i = 0; i < count; i++)
		{
			//don't override the returnPU with a lower turn order unit, only override it with a quick flag unit
			PlayerUnit tempPU = GetPlayerUnit(i);
			if (tempPU.IsQuickFlag() && StatusManager.Instance.IsTurnActable(tempPU.TurnOrder))
			{
				if (isSetQuickFlagToFalse)
					SetQuickFlag(tempPU.TurnOrder, false);
				return tempPU;
			}
			else if (retValue == null && tempPU.IsTurnActable())
			{
				retValue = tempPU;
			}
		}
		return retValue;
	}

	//gets next player turn, assumes players added to array in turn order
	//likely need add in status lab
	//public int GetNextTurnPlayerUnitId()
	//{
	//    foreach( PlayerUnit p in sPlayerUnitList)
	//    {
	//        if( p.IsTurnActable()) //vs. eligible. eligible ends the current player turn, actable never lets it happen
	//        {
	//            return p.TurnOrder;
	//        }
	//    }
	//    return NameAll.NULL_INT;
	//}

	//called in various calculations (like calculationresove
	//[PunRPC] //don't think I need this to be a RPC as both sides should be getting to this locally
	public void SetPlayerObjectAnimation(int unitId, string animation, bool isIdle)
	{
		//Debug.Log("receiving an animation for playerObject " + animation);
		PlayerUnitObject puo = GetPlayerUnitObject(unitId).GetComponent<PlayerUnitObject>();
		puo.SetAnimation(animation, isIdle);
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    photonView.RPC("SetPlayerObjectAnimation", PhotonTargets.Others, new object[] { unitId, animation, isIdle });
		//}
	}

	//online: life and death sent through PunRPC
	//[PunRPC]
	public void SendPlayerObjectAnimation(int unitId, string animation, bool isIdle)
	{
		//Debug.Log("receiving an animation for playerObject " + animation);
		//if (!PhotonNetwork.offlineMode)
		//{
		//    if (PhotonNetwork.isMasterClient)
		//    {
		//        photonView.RPC("SendPlayerObjectAnimation", PhotonTargets.Others, new object[] { unitId, animation, isIdle });
		//    }
		//    else
		//    {
		//        PlayerUnitObject puo = GetPlayerUnitObject(unitId).GetComponent<PlayerUnitObject>();
		//        puo.SetAnimation(animation, isIdle);
		//        if( animation == NameAll.ANIMATION_DEAD )
		//            AddToStatusList(unitId, NameAll.STATUS_ID_DEAD_3);
		//        else if( animation == NameAll.ANIMATION_LIFE)
		//        {
		//            RemoveFromStatusList(unitId, NameAll.STATUS_ID_DEAD_0);
		//            RemoveFromStatusList(unitId, NameAll.STATUS_ID_DEAD_1);
		//            RemoveFromStatusList(unitId, NameAll.STATUS_ID_DEAD_2);
		//            RemoveFromStatusList(unitId, NameAll.STATUS_ID_DEAD_3);
		//        }
		//    }
		//}
	}

	public void KnockbackPlayer(Board board, int unitId, Tile moveTile, SpellName sn = null, PlayerUnit actor = null, int rollResult = -1919, 
		int rollChance = -1919, int combatLogSubType = -1919)
	{
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    int tileX = moveTile.pos.x;
		//    int tileY = moveTile.pos.y;
		//    photonView.RPC("KnockbackPlayerRPC", PhotonTargets.Others, new object[] { unitId, tileX, tileY });
		//}
		//Debug.Log("knocking back player ");
		if( sRenderMode != NameAll.PP_RENDER_NONE)
		{
			MapTileManager.Instance.MoveMarker(unitId, moveTile);
			PlayerUnitObject puo = GetPlayerUnitObject(unitId).GetComponent<PlayerUnitObject>(); ;
			puo.InitializeKnockback(moveTile);
		}

		PlayerUnit pu = GetPlayerUnit(unitId);

		//if (true)//if(PhotonNetwork.offlineMode || PhotonNetwork.isMasterClient) //Other doesn't do falldamage, only master
		int fallDamage = DoFallDamage(pu, pu.TileZ, moveTile.height, sn, actor, rollResult, rollChance, NameAll.COMBAT_LOG_SUBTYPE_KNOCKBACK_DAMAGE);
		SetUnitTile(board, unitId, moveTile);


		if (isSaveCombatLog)
		{
			CombatTurn logTurn = new CombatTurn();
			logTurn.actor = GetPlayerUnit(unitId);
			logTurn.targetTile = moveTile;
			//roll chances captured through do fallDamage. this captures the movement efect
			AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_MISC, NameAll.COMBAT_LOG_SUBTYPE_KNOCKBACK_MOVE, cTurn: logTurn, effectValue: fallDamage);
		}

	}

	//[PunRPC]
	//public void KnockbackPlayerRPC(int unitId, int tileX, int tileY)
	//{
	//	//only arguments that matter are first 3 and last 1
	//	CombatMultiplayerMove cmm = new CombatMultiplayerMove(unitId, tileX, tileY, false, NameAll.NULL_INT, isKnockback: true);
	//	this.PostNotification(MultiplayerMove, cmm);
	//}

	int DoFallDamage(PlayerUnit target, int startHeight, int endHeight, SpellName sn= null, PlayerUnit actor = null, 
		int rollResult = -1919, int rollChance = -1919, int combatLogSubType=-1919)
	{
		if (target.AbilityMovementCode == NameAll.MOVEMENT_FLY)
		{
			return 0;
		}
		int fallDistance = startHeight - endHeight;
		if (fallDistance > target.StatTotalJump)
		{
			int fallDamage = (int)Math.Ceiling((fallDistance - (double)target.StatTotalJump) * (double)target.StatTotalMaxLife / 10);
			AlterUnitStat(NameAll.ALTER_STAT_DAMAGE, fallDamage, NameAll.STAT_TYPE_HP, target.TurnOrder, element_type: 0, sn:sn, actor:actor, rollResult:rollResult,
				rollChance:rollChance, combatLogSubType: combatLogSubType);
			return fallDamage;
		}
		return 0;
	}

	//called if a weapon is broken, charge attack is broken
	public void CheckForChargeRemove(PlayerUnit actor)
	{
		int actorId = actor.TurnOrder;
		if (NameAll.IsClassicClass(actor.ClassId) && StatusManager.Instance.IfStatusByUnitAndId(actorId, NameAll.STATUS_ID_CHARGING)
				&& SpellManager.Instance.RemoveArcherCharge(actorId))
		{
			//spellslow removed in the spellmanager check, status removed here
			StatusManager.Instance.RemoveStatus(actorId, NameAll.STATUS_ID_CHARGING);
		}
	}


	//called in CombatMoveSequenceState in online game. DEPRECATED FOR NOW
	//if master, tells other what movement to mirror
	//if other receives an RPC, raises a notification, notification then starts the movement action
	//master has to do the move in CombatMoveSequenceState AND other has to be in gameeloop state or the timing will be fucked up
	//public void ConfirmMove(Board board, PlayerUnit actor, Tile targetTile, bool isClassicClass, int swapUnitId)
	//{

	//	//if( !PhotonNetwork.offlineMode )
	//	//{
	//	//    if( PhotonNetwork.isMasterClient)
	//	//    {
	//	//        int actorId = actor.TurnOrder;
	//	//        int tileX = targetTile.pos.x;
	//	//        int tileY = targetTile.pos.y;
	//	//        Debug.Log("sending confirm move to other");
	//	//        photonView.RPC("ConfirmMoveRPC", PhotonTargets.Others, new object[] { actorId, tileX, tileY, isClassicClass, swapUnitId });
	//	//    }
	//	//    else
	//	//    {
	//	//        StartCoroutine(ConfirmMoveInner(board, actor, targetTile, isClassicClass, swapUnitId));
	//	//    }

	//	//}

	//}

	//const string MultiplayerMove = "Multiplayer.Move";

	//was an online thing, deprecated
	//Other is doing the actual move, sends the coordinates P2 in the notification so P2 can do the move on its side
	//[PunRPC]
	//public void ConfirmMoveRPC(int actorId, int tileX, int tileY, bool isClassicClass, int swapUnitId)
	//{
	//	//Debug.Log("other has received the move RPC, sending a notification so other can start the move");
	//	//sends a notification to P2, P2 gets it then calls ConfirmMove
	//	CombatMultiplayerMove cmm = new CombatMultiplayerMove(actorId, tileX, tileY, isClassicClass, swapUnitId, false);
	//	this.PostNotification(MultiplayerMove, cmm);
	//}

	//was an online thing, deprecated
	//IEnumerator ConfirmMoveInner(Board board, PlayerUnit actor, Tile targetTile, bool isClassicClass, int swapUnitId)
	//{


	//	//Debug.Log("other is moving a unit in confirmMoveInner 0");
	//	Tile actorStartTile = board.GetTile(actor); //used in case of movement ability swap

	//	MapTileManager.Instance.MoveMarker(actor.TurnOrder, targetTile);
	//	PlayerUnitObject puo = GetPlayerUnitObjectComponent(actor.TurnOrder);
	//	puo.GetTilesInRange(board, actorStartTile, actor); //movement relies on knowing link to previous tile (t.prev); this sets the links)
	//	puo.SetAnimation("moving", false);

	//	if (actor.IsSpecialMoveRange())
	//	{
	//		//Debug.Log("other is moving a unit in confirmMoveInner 1");
	//		if (isClassicClass)
	//		{
	//			if (actor.AbilityMovementCode == NameAll.MOVEMENT_FLY)
	//				yield return StartCoroutine(puo.TraverseFly(targetTile));
	//			else if (actor.AbilityMovementCode == NameAll.MOVEMENT_TELEPORT_1)
	//				yield return StartCoroutine(puo.TraverseTeleport(targetTile)); //Debug.Log("Decide on movement details and remove this yield return null");
	//			else
	//				yield return StartCoroutine(puo.Traverse(targetTile));
	//		}
	//		else
	//		{
	//			if (actor.AbilityMovementCode == NameAll.MOVEMENT_UNSTABLE_TP
	//				|| actor.AbilityMovementCode == NameAll.MOVEMENT_WINDS_OF_FATE)
	//				yield return StartCoroutine(puo.TraverseTeleport(targetTile));
	//			else if (actor.AbilityMovementCode == NameAll.MOVEMENT_LEAP)
	//				yield return StartCoroutine(puo.TraverseFly(targetTile));
	//			else
	//				yield return StartCoroutine(puo.Traverse(targetTile));
	//		}
	//	}
	//	else
	//	{
	//		//Debug.Log("other is moving a unit in confirmMoveInner 2");
	//		yield return StartCoroutine(puo.Traverse(targetTile));
	//	}


	//	puo.SetAnimation("idle", true);
	//	SetUnitTile(board, actor.TurnOrder, targetTile);

	//	yield return new WaitForFixedUpdate(); //not sure if needed but want at least a slight delay for the swap
	//	if (swapUnitId != NameAll.NULL_UNIT_ID)
	//		SetUnitTileSwap(board, swapUnitId, actorStartTile);

	//}

	//called in CombatmoveSequenceState, sets the correct unit tile
	//called in PlayerUnitObject WalkAroundTraverse
	public void SetUnitTile(Board board, int unitId, Tile t, bool isSetMarker = false, bool isAddCombatLog = false)
	{
		//GetPlayerUnitObject(unitId); //set as the animation occurs
		//Debug.Log("setting unit tile " + t.GetTileSummary());
		board.UpdatePlayerUnitTile(GetPlayerUnit(unitId), t); //takes the old tile, sets it to null unit id, sets the new tile to current player Id
		GetPlayerUnit(unitId).SetUnitTile(t); //updates the PlayerUnit with the correct x,y,z
		if (isSetMarker)
			MapTileManager.Instance.MoveMarker(unitId, t);

		if (isAddCombatLog && isSaveCombatLog)
		{
			CombatTurn logTurn = new CombatTurn();
			logTurn.actor = GetPlayerUnit(unitId);
			logTurn.targetTile = t;
			AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_MOVE, cTurn: logTurn);
		}
	}

	//UnitId is unit to be swapped, tile t is starting position of the the actor that does the swapping
	public void SetUnitTileSwap(Board board, int unitId, Tile t)
	{

		board.UpdatePlayerUnitTileSwap(GetPlayerUnit(unitId), t); //takes the old tile, sets it to null unit id, sets the new tile to current player Id
		GetPlayerUnit(unitId).SetUnitTile(t); //updates the PlayerUnit with the correct x,y,z
		MapTileManager.Instance.MoveMarker(unitId, t);
		StartCoroutine(GetPlayerUnitObjectComponent(unitId).TraverseTeleport(t)); //moves the swapped player
		if(isSaveCombatLog)
		{
			//swapper already updated in CombatMoveSequenceState, swappee is occuring now
			CombatTurn logSwappeeTurn = new CombatTurn();
			logSwappeeTurn.targetTile = t;
			logSwappeeTurn.actor = GetPlayerUnit(unitId);
			AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_MOVE, NameAll.COMBAT_LOG_SUBTYPE_MOVE_SWAP, logSwappeeTurn);
		}
	}

	//Called in CombatEndFacingState if alive, ActiveTurnState if dead
	public void EndCombatTurn(CombatTurn turn, bool isDead = false)
	{
		//if( !PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    photonView.RPC("EndCombatTurnRPC", PhotonTargets.Others, new object[] { turn.actor.TurnOrder, turn.endDir.DirectionToInt(), turn.hasUnitActed,
		//        turn.hasUnitMoved, isDead });
		//}

		PlayerUnit actor = turn.actor;
		if (isDead)
		{
			turn.hasUnitActed = true; //turning this to true so full turn is decremented
			turn.hasUnitMoved = true;
			actor.EndTurnCT(turn);
		}
		else
		{
			actor.Dir = turn.endDir;
			if( sRenderMode != NameAll.PP_RENDER_NONE)
				SetPUODirectionEndTurn(actor.TurnOrder);
			//Debug.Log("do the playerunitobject turn shit here or in combatEndFacingState");
			actor.EndTurnCT(turn);
		}

		AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_END_TURN, cTurn: turn, effectValue: actor.CT);		
	}

	//[PunRPC]
	public void EndCombatTurnRPC(int unitId, int directionInt, bool hasUnitActed, bool hasUnitMoved, bool isDead)
	{
		PlayerUnit actor = GetPlayerUnit(unitId);
		if (isDead)
		{
			actor.EndTurnCT(hasUnitActed, hasUnitMoved);
		}
		else
		{
			actor.Dir = DirectionsExtensions.IntToDirection(directionInt);
			SetPUODirectionEndTurn(actor.TurnOrder);
			actor.EndTurnCT(hasUnitActed, hasUnitMoved);
		}
	}

	#region Calls to PlayerUnitObjects to cause turning

	//dir is in the actor. turn to the direction actor should be facing
	//NO RPC NEEDED, called only in EndCombatTurn and EndCombatTurnRPC
	public void SetPUODirectionEndTurn(int unitId)
	{
		GetPlayerUnitObjectComponent(unitId).SetFacingDirectionEndTurn(GetPlayerUnit(unitId));
	}

	//called in InitCombatState & WalkAroundInitState to set the initial facing direction of the unit
	//no RPC needed as both sides call it locally (forcing an RPC call would result in errors
	//to do: make facing direction based on where you spawn on the map
	public void SetInitialFacingDirection(int unitId, Directions dir = Directions.North, bool isDefault = true)
	{
		if (isDefault)
		{
			if (GetPlayerUnit(unitId).TeamId == NameAll.TEAM_ID_GREEN)
			{
				GetPlayerUnit(unitId).Dir = Directions.East;

			}
			else
			{
				GetPlayerUnit(unitId).Dir = Directions.West;
			}
		}
		else
		{
			GetPlayerUnit(unitId).Dir = dir;
		}
		if(sRenderMode != NameAll.PP_RENDER_NONE)
			GetPlayerUnitObjectComponent(unitId).SetAttackDirection(GetPlayerUnit(unitId).Dir);
	}

	public void SetFacingDirectionMidTurn(int unitId, Tile startTile, Tile endTile) //based on actor and target's tiles, like jumping
	{
		Directions dir = startTile.GetDirectionAttack(endTile);
		GetPlayerUnit(unitId).Dir = dir;
		if (endTile.pos.x != startTile.pos.x || endTile.pos.y != startTile.pos.y)
		{
			SetPUODirectionMidTurn(unitId, dir);
		}
	}

	//called in combatperformabilitystate and calcMono
	public void SetFacingDirectionAttack(Board board, PlayerUnit actor, Tile targetTile)
	{
		Tile actorTile = board.GetTile(actor);
		Directions dir = actorTile.GetDirectionAttack(targetTile);
		if (targetTile.pos.x != actor.TileX || targetTile.pos.y != actor.TileY)
		{
			SetPUODirectionMidTurn(actor.TurnOrder, dir);
		}
	}

	//for turning mid-turn. forces a PUO to turn but does nothing permanent to the PU (so unit can turn back to its correct direction later)
	public void SetPUODirectionMidTurn(int unitId, Directions dir, bool isSend = true)
	{
		GetPlayerUnitObjectComponent(unitId).SetAttackDirection(dir);
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient && isSend) //some cases don't want to send since it's a mid turn action
		//    photonView.RPC("SetPUODirectionMidTurnRPC", PhotonTargets.Others, new object[] { unitId, dir.DirectionToInt() });
	}

	//other receives this from master from a slow action
	//[PunRPC]
	public void SetPUODirectionMidTurnRPC(int unitId, int dirInt)
	{
		GetPlayerUnitObjectComponent(unitId).SetAttackDirection(DirectionsExtensions.IntToDirection(dirInt));
	}

	//online: master calls this on direction attacks so that other can see the turn
	//only called form combatconfirmabilitytargetstate when master is doing the input
	public void SetMPPUODirectionMidTurn(int unitId, Directions dir)
	{
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient )
		//    photonView.RPC("SetPUODirectionMidTurnRPC", PhotonTargets.Others, new object[] { unitId, dir.DirectionToInt() });
	}
	#endregion

	//Movement Ability Scale has a special jump value. called in PUO to get special jump value for scale (which only works for certain tiles)
	//could speed this up by doing a call directly to the playerunit
	public int GetJumpScale(int unitId, Tile t)
	{
		return GetPlayerUnit(unitId).GetJumpScale(t);
	}


	public List<PlayerUnit> GetAllUnitsByTeamId(int teamId, bool same_team = true)
	{
		List<PlayerUnit> temp = new List<PlayerUnit>();
		if (same_team)
		{
			//teamId = teamId;
		}
		else {
			if (teamId == 2)
			{
				teamId = 3;
			}
			else {
				teamId = 2;
			}
		}

		foreach (PlayerUnit p in sPlayerUnitList)
		{
			if (p.TeamId == teamId && !StatusManager.Instance.IfStatusByUnitAndId(p.TurnOrder, NameAll.STATUS_ID_CRYSTAL))
			{
				temp.Add(p);
			}
		}
		return temp;
	}

	//set able to fight: battle ends when all units on a side unable to fight (or other Victory Conditions)
	//online mode: master send RPC to other to toggle status
	//[PunRPC]
	public void SetAbleToFight(int unitId, bool able)
	{
		GetPlayerUnit(unitId).SetAbleToFight(able);
		CheckForEndCombat(GetPlayerUnit(unitId), able); //checks for end of combat
														//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
														//{
														//    photonView.RPC("SetAbleToFight", PhotonTargets.Others, new object[] { unitId, able });
														//}
	}

	//[PunRPC]
	public void AddLife(int effect, int unitId)
	{
		GetPlayerUnit(unitId).ReturnToLife(effect); //Debug.Log("adding life to a unit" + effect);
		TallyCombatStats(unitId, NameAll.STATS_KILLS_HEALED, 1);
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    photonView.RPC("AddLife", PhotonTargets.Others, new object[] { effect, unitId });
		//}
	}

	//[PunRPC]
	public void RemoveMPById(int unitId, int mp)
	{
		GetPlayerUnit(unitId).RemoveMP(mp);
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    photonView.RPC("RemoveMPById", PhotonTargets.Others, new object[] { unitId, mp});
		//}
	}

	//called in calculationResolveaction
	//[PunRPC]
	public void AlterUnitStat(int alter_stat, int effect, int statType, int unitId, int element_type = 0, SpellName sn=null, PlayerUnit actor = null, 
		int rollResult = -1919, int rollChance = -1919, int combatLogSubType = -1919)
	{
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    photonView.RPC("AlterUnitStat", PhotonTargets.Others, new object[] { alter_stat, effect, statType, unitId, element_type });
		//}
		//Debug.Log("alter stat is " + alter_stat + " statType, elementType " + statType + ", " + element_type );
		if( sRenderMode != NameAll.PP_RENDER_NONE)
			SetPlayerObjectAnimation(unitId, "damage", false);
		if (statType == NameAll.STAT_TYPE_HP)
		{
			GetPlayerUnit(unitId).SetHP(effect, alter_stat, element_type, false, isSaveCombatLog: isSaveCombatLog, sn:sn, actor:actor, rollResult:rollResult,
				rollChance:rollChance, combatLogSubType: combatLogSubType);
		}
		else
		{
			//Debug.Log("altering unit stat " + effect);
			GetPlayerUnit(unitId).AlterStat(alter_stat, effect, statType, element_type, isSaveCombatLog: isSaveCombatLog, sn: sn, actor: actor, rollResult: rollResult,
				rollChance: rollChance, combatLogSubType: combatLogSubType);
		}
		//could make the alter stat be worth more than HP dmg at some point
		int z1 = NameAll.STATS_DAMAGE_DONE;
		if (alter_stat != NameAll.ALTER_STAT_DAMAGE)
			z1 = NameAll.STATS_DAMAGE_HEALED;
		TallyCombatStats(unitId, z1, effect);
		//this.PostNotification(ActorStatChangeNotification, GetPlayerUnit(unitId)); //causes errors in multiplayer
	}

	const string ActorStatChangeNotification = "CombatUITarget.ActorStatChangeNotification"; //updates the actor panel

	//[PunRPC]
	public void RemoveLife(int effect, int remove_stat, int unitId, int elemental_type, bool removeAll = false, SpellName sn = null, PlayerUnit actor = null,
		int rollResult = -1919, int rollChance = -1919, int combatLogSubType = -1919)
	{
		GetPlayerUnit(unitId).SetHP(effect, remove_stat, elemental_type, removeAll, isSaveCombatLog:isSaveCombatLog, sn:sn, actor:actor, rollResult: rollResult,
			rollChance:rollChance, combatLogSubType:combatLogSubType);
		TallyCombatStats(unitId, NameAll.STATS_KILLS_DONE, 1);
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    photonView.RPC("RemoveLife", PhotonTargets.Others, new object[] { effect, remove_stat, unitId, elemental_type, removeAll });
		//}
	}

	public bool IsMimeOnTeam(int team_id)
	{
		//Debug.Log(" is mime on team " + isMimeOnGreen + isMimeOnRed);
		if (team_id == NameAll.TEAM_ID_GREEN)
		{
			return isMimeOnGreen;
		}
		else if (team_id == NameAll.TEAM_ID_RED)
		{
			return isMimeOnRed;
		}
		return false;
	}

	//called in InitCombatState, allows faster checking for if mime on team
	public void SetMimeOnTeam()
	{
		isMimeOnGreen = false;
		isMimeOnRed = false;
		foreach (PlayerUnit p in sPlayerUnitList)
		{
			if (p.ClassId == NameAll.CLASS_MIME)
			{
				if (p.TeamId == NameAll.TEAM_ID_GREEN)
				{
					isMimeOnGreen = true;
				}
				else if (p.TeamId == NameAll.TEAM_ID_RED)
				{
					isMimeOnRed = true;
				}
			}
		}
	}

	//called in InitCombatState, adds the lasting statuses (item based statuses and blocks)
	public void SetLastingStatuses()
	{
		foreach (PlayerUnit pu in sPlayerUnitList)
		{
			pu.SetLastingStatuses();
		}
	}

	//called in calculation resolve action, gets list of mimes
	public List<PlayerUnit> GetMimeList(int actorCharmTeam)
	{
		List<PlayerUnit> temp = new List<PlayerUnit>();
		foreach (PlayerUnit p in sPlayerUnitList)
		{
			if (p.ClassId == NameAll.CLASS_MIME && StatusManager.Instance.IsAbleToReact(p.TurnOrder) && p.GetCharmTeam() == actorCharmTeam)
			{
				temp.Add(p);
			}
		}
		return temp;
	}

	public bool QuickFlagCheckPhase()
	{
		//tells the game loop, there's someone (anyone) with quick, thus it is their turn to act
		//might want a status check here at some point (or count on status's properly removing quick flags and not adding them to ineligible units
		foreach (PlayerUnit p in sPlayerUnitList)
		{
			if (p.IsQuickFlag())
			{
				Debug.Log("Found a unit with quick");
				return true;
			}
		}
		return false;
	}

	public int GetQuickFlagUnitId()
	{
		int z1 = NameAll.NULL_INT; //Debug.Log("getting a quick flag unit");
		foreach (PlayerUnit p in sPlayerUnitList)
		{
			if (p.IsQuickFlag() && p.TurnOrder < z1)
			{
				z1 = p.TurnOrder;
			}
		}
		if (z1 != NameAll.NULL_INT)
		{
			//Debug.Log("ending quick flag units turn");
			//this unit is getting a turn now, disabling the quick flag
			SetQuickFlag(z1, false);
			StatusManager.Instance.RemoveStatus(z1, NameAll.STATUS_ID_QUICK, true); //RemoveFromStatusList(z1, "quick"); handled in there

		}
		return z1;
	}

	//RPC not needed. When Quick added in statusmanager quick on both sides is called
	public void SetQuickFlag(int unitId, bool isQuick)
	{
		GetPlayerUnit(unitId).SetQuickFlag(isQuick);
		if (isQuick)
		{
			GetPlayerUnit(unitId).SetCT(100, "quick");
		}
		else
		{
			StatusManager.Instance.RemoveStatus(unitId, NameAll.STATUS_ID_QUICK);
		}
	}

	public bool IsAbilityEquipped(int unitId, int abilityId, int abilitySlot)
	{
		if (GetPlayerUnit(unitId).IsAbilityEquipped(abilityId, abilitySlot))
		{
			return true;
		}
		return false;
	}

	public int GetWeaponPower(int unitId, bool battleSkill = false, bool twoHandsTypeAllowed = false)
	{
		return GetPlayerUnit(unitId).GetWeaponPower(battleSkill, twoHandsTypeAllowed);
	}

	//not using this, using on the size of the SpellManager SpellReaction Queue
	//public bool IsReactionFlagActive() 
	//{
	//    foreach (PlayerUnit p in sPlayerUnitList)
	//    {
	//        if (p.IsReactionFlag() )
	//        {
	//            return true;
	//        }
	//    }
	//    return false;
	//}

	//set to inactive in SpellManager, set to active in one of the Calculations
	public void AlterReactionFlag(int unitId, bool reactionFlag)
	{
		if (reactionFlag)
		{
			GetPlayerUnit(unitId).EnableReactionFlag();
		}
		else
		{
			GetPlayerUnit(unitId).DisableReactionFlag();
		}
	}

	public void EquipMovementAbility(int unitId, int abilityId) //calls unequip first in playerunit
	{
		GetPlayerUnit(unitId).EquipMovementAbility(abilityId);
	}

	public Tile GetPlayerUnitTile(Board board, int unitId)
	{
		var pu = GetPlayerUnit(unitId);
		Point p = new Point(pu.TileX, pu.TileY);
		return board.GetTile(p);
	}

	//called in initcombatsatte
	public void InitializePlayerUnits()
	{
		foreach (PlayerUnit pu in sPlayerUnitList)
		{
			pu.InitializeTwoSwordsEligible();
			pu.InitializeOnMoveEffect();
			pu.InitializeSpecialMoveRange();
			if (pu.ClassId >= NameAll.CUSTOM_CAMPAIGN_ID_START_VALUE)
			{
				ClassEditObject ce = CalcCode.LoadCustomClass(pu.ClassId);
				if (ce != null)
					SpellManager.Instance.GenerateSpellNamesByCustomCommandSet(ce.CommandSet);
			}
			else if (pu.AbilitySecondaryCode >= NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE)
			{
				SpellManager.Instance.GenerateSpellNamesByCustomCommandSet(pu.AbilitySecondaryCode);
			}
		}
	}

	public bool IsEligibleForTwoSwords(int unitId, int commandSet)
	{
		if (commandSet == NameAll.COMMAND_SET_ATTACK_AURELIAN || commandSet == NameAll.COMMAND_SET_ATTACK || commandSet == NameAll.COMMAND_SET_BATTLE_SKILL)
		{
			//Debug.Log("in is pm is eligible for two swords");
			return GetPlayerUnit(unitId).IsEligibleForTwoSwords();
		}
		return false;
	}

	//show text over a playerUnit
	//[PunRPC]
	public void ShowFloatingText(int unitId, int type, string value, bool isCrit = false)
	{
		if (sRenderMode == NameAll.PP_RENDER_NONE)
			return;
		//Debug.Log("showing floating text " + type + " " + value );
		//if( !PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    //showfloatingtext for 0, 7, 19 need notifications so other shows them
		//    if (type == 0 || type == 19 || type == 7)
		//        photonView.RPC("ShowFloatingText", PhotonTargets.Others, new object[] { unitId, type, value, isCrit });
		//}

		if (type == 0) //miss
		{
			PUOTextOverlayCanvasController.instance.ShowPUOText(GetPlayerUnitObject(unitId), PUOTextType.Miss, "MISS!");
		}
		else if (type == 1) //damage, not doing crit for now
		{
			PUOTextOverlayCanvasController.instance.ShowPUOText(GetPlayerUnitObject(unitId), PUOTextType.Hit, value);
			//if(isCrit)
			//{
			//    PUOTextOverlayCanvasController.instance.ShowPUOText(GetPlayerUnitObject(unitId), PUOTextType.CriticalHit, value);
			//}
			//else
			//{
			//    PUOTextOverlayCanvasController.instance.ShowPUOText(GetPlayerUnitObject(unitId), PUOTextType.Hit, value);
			//}
		}
		else if (type == 2) //heal
		{
			Debug.Log(" testing what is null 0 ");
			GetPlayerUnitObject(unitId);
			Debug.Log(" testing what is null 1 " + value + " " + PUOTextType.Heal);
			Debug.Log(" testing what is null 2 " );
			PUOTextOverlayCanvasController.instance.ShowPUOText(GetPlayerUnitObject(unitId), PUOTextType.Heal, value);
		}
		else if (type == 7) //status
		{
			PUOTextOverlayCanvasController.instance.ShowPUOText(GetPlayerUnitObject(unitId), PUOTextType.AddStatus, value);
		}
		else if (type == 5) //heal mp or other stat
		{
			//Debug.Log("showing heal mp?");
			PUOTextOverlayCanvasController.instance.ShowPUOText(GetPlayerUnitObject(unitId), PUOTextType.HealMP, value);
		}
		else if (type == 6) //damage mp or other stat
		{
			PUOTextOverlayCanvasController.instance.ShowPUOText(GetPlayerUnitObject(unitId), PUOTextType.HitMP, value);
		}
		else if (type == 19) //custome message
		{
			PUOTextOverlayCanvasController.instance.ShowPUOText(GetPlayerUnitObject(unitId), PUOTextType.Miss, value);
		}

		//PUOTextOverlayCanvasController.instance.ShowPUOText(hitObject, PUOTextType.Hit, value);
		//Debug.Log("asdf2");
	}


	//should update playerunit teamId with enums at some point
	public Teams CheckForDefeat()
	{
		Teams victor = Teams.None;
		bool team1Dead = true;
		bool team2Dead = true;

		foreach (PlayerUnit p in sPlayerUnitList)
		{
			if (team1Dead || team2Dead)
			{
				if (p.TeamId == NameAll.TEAM_ID_GREEN)
				{
					if (p.AbleToFight)
					{
						team1Dead = false;
					}
				}
				else
				{
					if (p.AbleToFight)
					{
						team2Dead = false;
					}
				}

			}
			else
			{
				break;
			}
		}

		if (team1Dead)
		{
			victor = Teams.Team2;
		}
		if (team2Dead)
		{
			victor = Teams.Team1;
		}

		return victor;
	}

	//called in game loop, returns 2 for team 2 win return 3 for team 3 win
	//should be obsolete at some point
	public int CheckEndGameConditions()
	{
		int z1 = 0;
		bool team2Dead = true;
		bool team3Dead = true;
		foreach (PlayerUnit p in sPlayerUnitList)
		{
			if (team2Dead || team3Dead)
			{
				if (p.TeamId == 2)
				{
					if (p.AbleToFight)
					{
						team2Dead = false;
					}
				}
				else
				{
					if (p.AbleToFight)
					{
						team3Dead = false;
					}
				}

			}
			else
			{
				break;
			}
		}
		if (team2Dead)
		{
			return 3; //team 3 won
		}
		if (team3Dead)
		{
			return 2; //team 2 won
		}
		return z1;
	}


	//non lasting statuses created in status amanger
	//LASTING STATUSES FOR PLAYERUNITOBJECTS CREATED IN STATUS OBJECT CREATOR
	//[PunRPC] //calling PunRPC in statusManager to call this
	//Online: generic call done here, locally on each side. For specific call use SendMPStatusList
	public void AddToStatusList(int unitId, int statusId)
	{
		//Debug.Log("adding status to status list in player manager " + statusId + " " + unitId);
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    photonView.RPC("AddToStatusList", PhotonTargets.Others, new object[] { unitId, statusId });
		//}
		PlayerUnitObject puo = GetPlayerUnitObject(unitId).GetComponent<PlayerUnitObject>();
		puo.AddToStatusList(NameAll.GetStatusString(statusId));
	}

	//called in status manager where applicable
	public void RemoveFromStatusList(int unitId, int statusId)
	{
		//Debug.Log("removing status from status list in player manager " + statusId + " " + unitId);
		PlayerUnitObject puo = GetPlayerUnitObject(unitId).GetComponent<PlayerUnitObject>();
		puo.RemoveFromStatusList(NameAll.GetStatusString(statusId));
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    photonView.RPC("RemoveFromStatusListRPC", PhotonTargets.Others, new object[] { unitId, statusId });
		//}
	}

	//for Other, this is going to duplicate some calls (as the PunRPC is called after it is called locally) but won't result in an error or an issue in puo
	//[PunRPC]
	//public void RemoveFromStatusListRPC(int unitId, int statusId)
	//{
	//    PlayerUnitObject puo = GetPlayerUnitObject(unitId).GetComponent<PlayerUnitObject>();
	//    puo.RemoveFromStatusList(NameAll.GetStatusString(statusId));
	//}

	//offline: not used
	//online: called in StatusManager when Master wants to send Other a message to manipulate its status list
	//[PunRPC]
	public void SendMPStatusList(bool isAdd, int unitId, int statusId)
	{
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    photonView.RPC("SendMPStatusList", PhotonTargets.Others, new object[] { isAdd, unitId, statusId });
		//    return;
		//}
		//other receing the rpc
		if (isAdd)
			AddToStatusList(unitId, statusId);
		else
			RemoveFromStatusList(unitId, statusId);
	}


	public void EndOfTurnTick(int unitId, int type) //0 for poison, 1 for regen
	{
		int combatLogSubType = NameAll.COMBAT_LOG_SUBTYPE_END_TURN_TICK_REGEN;
		if (type == 0)
			combatLogSubType = NameAll.COMBAT_LOG_SUBTYPE_END_TURN_TICK_POISON;

		PlayerUnit pu = GetPlayerUnit(unitId);
		int z1 = pu.StatTotalMaxLife / 8;
		if (type == 0)
		{
			if (CalculationResolveAction.MPSwitchCheck(GetPlayerUnit(unitId)))
			{
				AlterUnitStat(1, z1, NameAll.STAT_TYPE_MP, unitId, combatLogSubType: combatLogSubType);
			}
			else
			{
				//pu.SetHP(z1, 1); //1 for the type that removes a stat
				RemoveLife(z1, 1, unitId, NameAll.ITEM_ELEMENTAL_NONE, combatLogSubType: combatLogSubType);
			}

		}
		else if (type == 1)
		{
			//pu.SetHP(z1, 0, "undead");
			RemoveLife(z1, 0, unitId, NameAll.ITEM_ELEMENTAL_UNDEAD, combatLogSubType: combatLogSubType);
		}

	}


	//called in character builder scene
	public void ClearPlayerLists()
	{
		sPlayerUnitList.Clear();
		sPlayerObjectList.Clear();
	}

	//called in status manager after invite hits
	public void DefectFromTeam(int unitId)
	{
		GetPlayerUnit(unitId).DefectFromTeam();
	}


	//List<PlayerUnit> RandomizePUList()
	//{
	//    List<PlayerUnit> puList = sPlayerUnitList.ToList();
	//    //var shuffledcards = cards.OrderBy(a => rng.Next());
	//    System.Random rng = new System.Random();
	//    puList = puList.OrderBy(a => rng.Next()).ToList();
	//    return puList;
	//}

	//List<int> RandomizeIntList(List<int> tempList)
	//{
	//    List<int> intList = tempList.ToList();
	//    //var shuffledcards = cards.OrderBy(a => rng.Next());
	//    System.Random rng = new System.Random();
	//    intList = intList.OrderBy(a => rng.Next()).ToList();
	//    return intList;
	//}

	//void RandomizeList<T>( List<T> shuffleList)
	//{
	//    List<T> tempList = shuffleList.ToList();
	//    //var shuffledcards = cards.OrderBy(a => rng.Next());
	//    System.Random rng = new System.Random();
	//    tempList = tempList.OrderBy(a => rng.Next()).ToList();
	//}

	const string MultiplayerDisableUnit = "Multiplayer.DisableUnit";
	//called from CombatStateActiveTurn when unit is crystallized and in CombatMoveSequence state
	//[PunRPC]
	public void DisableUnit(int unitId)
	{
		//if (!PhotonNetwork.offlineMode )
		//{
		//    if (PhotonNetwork.isMasterClient)
		//        photonView.RPC("DisableUnit", PhotonTargets.Others, new object[] { unitId });
		//    else //other needs to make a call to board to disable the unit
		//        this.PostNotification(MultiplayerDisableUnit, unitId);
		//}

		//PlayerUnit keeps the same tilex,tiley but is no longer physically there
		//moves the unit off the map
		Vector3 vec = new Vector3(-5, -5, -5);
		GetPlayerUnitObject(unitId).transform.position = vec;
		GetPlayerUnitObject(unitId).SetActive(false);
		GetPlayerUnit(unitId).SetCT(0); //crystal keeps it from going again, should be no other way for it to get a turn
		MapTileManager.Instance.DisableMarker(unitId); //moves marker off the map
	}


	//[PunRPC]
	public void ToggleJumping(int unitId, bool isJumping)
	{
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//    photonView.RPC("ToggleJumping", PhotonTargets.Others, new object[] { unitId, isJumping });

		Vector3 mtoPos = MapTileManager.Instance.GetMarkerByIndex(unitId).transform.position; //MapTileManager.Instance.GetMapTileObjectByIndex(GetPlayerUnit(unitId).GetMap_tile_index()).transform.position;
		Vector3 vec;
		if (isJumping)
		{
			vec = new Vector3(0, 40, 0);
		}
		else
		{
			vec = new Vector3(0, mtoPos.y + NameAll.TILE_CENTER_HEIGHT, 0);
		}

		vec += mtoPos;
		GetPlayerUnitObject(unitId).transform.position = vec;
	}


	public bool IsOnTeam(int actorId, int targetId)
	{
		if (GetPlayerUnit(actorId).TeamId == GetPlayerUnit(targetId).TeamId)
			return true;

		return false;
	}

	//called in CombatComputerPlayer
	//he uses alliances, I'm using team Id
	public bool IsMatch(int actorId, int targetId, Targets targets)
	{
		//Debug.Log("is testing for targetId " + targetId);
		if (targetId == NameAll.NULL_UNIT_ID) //targetId fed in is tile.UnitId
			return false;

		bool isMatch = false;
		//Debug.Log("testing for target " + targets.ToString());
		switch (targets)
		{
			case Targets.Self:
				isMatch = actorId == targetId;
				break;
			case Targets.Ally:
				isMatch = IsOnTeam(actorId, targetId);
				break;
			case Targets.Foe:
				isMatch = !IsOnTeam(actorId, targetId);
				break;
		}
		//can add status effects down the line
		return isMatch;
	}

	//called in CombatComputerPlayer. Get lists of pu for actor to try to target first (ie target dead units with revive, hurt enemies with dmg etc)
	public List<PlayerUnit> GetAIList(int actorCharmTeam, int aiType)
	{
		List<PlayerUnit> retValue = new List<PlayerUnit>();
		if (aiType == NameAll.AI_LIST_HURT_ENEMY)
		{
			foreach (PlayerUnit pu in sPlayerUnitList)
			{
				if (pu.TeamId != actorCharmTeam)
				{
					int z1 = pu.StatTotalLife * 2; //Debug.Log("testing for hurt enemy, hp is " + z1);
					if (z1 > 0 && z1 < pu.StatTotalMaxLife)
					{
						retValue.Add(pu); //Debug.Log("adding hurt enemy to list");
					}
				}
			}
		}
		else if (aiType == NameAll.AI_LIST_DEAD_ALLY)
		{
			foreach (PlayerUnit pu in sPlayerUnitList)
			{
				if (pu.TeamId == actorCharmTeam)
				{
					if (StatusManager.Instance.IfStatusByUnitAndId(pu.TurnOrder, NameAll.STATUS_ID_DEAD))
					{
						retValue.Add(pu);
					}
				}
			}
		}
		else if (aiType == NameAll.AI_LIST_HURT_ALLY)
		{
			foreach (PlayerUnit pu in sPlayerUnitList)
			{
				if (pu.TeamId == actorCharmTeam)
				{
					if (pu.StatTotalLife * 2 > 0 && pu.StatTotalLife * 2 < pu.StatTotalMaxLife)
					{
						retValue.Add(pu);
					}
				}
			}
		}
		return retValue;
	}

	#region multiplayer and multiplayer notifications
	const string MultiplayerActiveTurnPreTurn = "Multiplayer.ActiveTurnPreTurn";//pre active turn start, show whose turn it is. Other raises it to show PreTurn stuff
	const string MultiplayerCommandTurn = "Multiplayer.CommandTurn";//other gets to input an action
	const string MultiplayerActiveTurnMidTurn = "Multiplayer.ActiveTurnMidTurn";//active turn mid-turn. Master raises it after other has told results of input to Master and vice versa

	//Master calls this
	public void SendMPActiveTurnPreTurn(int unitId)
	{
		//photonView.RPC("ReceiveMPActiveTurnPreTurn", PhotonTargets.Others, new object[] { unitId });
	}

	//Other receives this
	//[PunRPC]
	public void ReceiveMPActiveTurnPreTurn(int unitId)
	{
		this.PostNotification(MultiplayerActiveTurnPreTurn, unitId);
	}

	//Master calls this
	public void SendMPActiveTurnStartTurn(int unitId, bool hasUnitActed, bool hasUnitMoved)
	{
		//photonView.RPC("ReceiveMPActiveTurnStartTurn", PhotonTargets.Others, new object[] { unitId, hasUnitActed, hasUnitMoved });
	}

	//Other receives this
	//[PunRPC]
	public void ReceiveMPActiveTurnStartTurn(int unitId, bool hasUnitActed, bool hasUnitMoved)
	{
		CombatTurn tempTurn = new CombatTurn();
		tempTurn.hasUnitActed = hasUnitActed;
		tempTurn.hasUnitMoved = hasUnitMoved;
		this.PostNotification(MultiplayerCommandTurn, tempTurn);
	}

	//online: master calls this to other, let's other show master's inputs (not the results but what was selected)
	//online: other calls this to master. tells results of input
	public void SendMPActiveTurnInput(bool isMove, bool isAct, bool isWait, int actorId, int tileX, int tileY, int directionInt,
		int targetId, int spellIndex, int spellIndex2)
	{
		//Debug.Log("calling send MPActiveTurnInput: " + actorId + " x,y " + tileX + "," + tileY + " " + directionInt );
		//Debug.Log("calling send MPActiveTurnInput: " + directionInt + ", " + targetId + "," + spellIndex + " " + spellIndex2);
		//photonView.RPC("ReceiveMPActiveTurnInput", PhotonTargets.Others, new object[] { isMove, isAct, isWait, actorId, tileX, tileY, directionInt, targetId, spellIndex, spellIndex2 });
	}

	//[PunRPC]
	public void ReceiveMPActiveTurnInput(bool isMove, bool isAct, bool isWait, int actorId, int tileX, int tileY, int directionInt,
		int targetId, int spellIndex, int spellIndex2)
	{
		//Debug.Log("receiving MPActiveTurnInput: " + actorId + " x,y " + tileX + "," + tileY + " " + directionInt);
		CombatMultiplayerTurn cmt = new CombatMultiplayerTurn(isMove, isAct, isWait, actorId, tileX, tileY, directionInt, targetId, spellIndex, spellIndex2);
		this.PostNotification(MultiplayerActiveTurnMidTurn, cmt);
	}

	//Called in MPGameController to set game to online and who is MasterClient
	public void SetMPOnline(bool isMasterClient)
	{
		sMPObject.IsOffline = false;
		sMPObject.IsReady = false;
		sMPObject.IsOpponentReady = false;
		sMPObject.IsMasterClient = isMasterClient;
	}

	//called by Other in GameLoopState after a notification,
	//currently not needed but if moved from Master checking update constantly to a notification based system, this would be needed (in case Master queries Other)
	public void SetMPSelfStatusAndPhase(bool isReady, Phases phase)
	{
		sMPObject.IsReady = isReady;
		sMPObject.SelfCurrentPhase = phase;
	}

	//called by Master in GameLoopState and MultiplayerWaitPhase
	//master realizes other is not ready
	//other realizes that master is now waiting for it and does the necessary phase
	public void SendMPPhase(Phases phase)
	{
		sMPObject.IsOpponentReady = false;
		sMPObject.OpponentCurrentPhase = phase;
		//if (PhotonNetwork.isMasterClient)
		//    photonView.RPC("ReceiveMPPhase", PhotonTargets.Others, (byte)phase);
	}

	const string MultiplayerGameLoop = "Multiplayer.GameLoop";

	//[PunRPC]
	public void ReceiveMPPhase(byte phaseType)
	{
		sMPObject.SelfCurrentPhase = (Phases)phaseType;
		this.PostNotification(MultiplayerGameLoop, sMPObject.SelfCurrentPhase);
	}

	//Called by master in MultiplayerWaitState so that master knows that other is not ready and not in standby phase
	public void SetMPOpponentReadyAndPhase(bool isReady, Phases phase)
	{
		sMPObject.IsOpponentReady = isReady;
		sMPObject.OpponentCurrentPhase = phase;
	}

	//Called by Master in Online game. Checks to make sure game can move to next phase.
	public bool IsOpponentInStandbyAndReady()
	{
		if (sMPObject.IsOpponentReady && sMPObject.OpponentCurrentPhase == Phases.Standby)
			return true;

		return false;
	}

	//called by Other in GameLoopState in Online game. Lets Master know that Other is ready for the next phase
	//Other self update currently not needed but if moved from Master checking update constantly to a notification based system, this would be needed (in case Master queries Other)
	public void SetMPStandby()
	{
		//Debug.Log("letting master know that ready now");
		sMPObject.IsReady = true;
		sMPObject.SelfCurrentPhase = Phases.Standby;
		//photonView.RPC("SendMPStandby", PhotonTargets.Others, new object[] { });
	}

	//received by Master in Online game, lets Master know that other is ready for the next phase
	//[PunRPC]
	public void SendMPStandby()
	{
		sMPObject.IsOpponentReady = true;
		sMPObject.OpponentCurrentPhase = Phases.Standby;
		//can send notification here but already checking for it in GameLoopState
	}

	//called at beginning of GameLoopState to know if MP game or not
	public bool IsOfflineGame()
	{
		return sMPObject.IsOffline;
	}

	//called at beginning of GameLoopState to know if MC
	public bool isMPMasterClient()
	{
		return sMPObject.IsMasterClient;
	}

	//for debugging in multiplayer, goes on turns button
	public string GetMPSelfPhase()
	{
		return sMPObject.SelfCurrentPhase.ToString();
	}

	//online: called by master to tell P2 what to do with crystalized unit
	public void SendMPCrystalOutcome(List<int> tempList)
	{
		//photonView.RPC("ReceiveMPCrystalOutcome", PhotonTargets.Others, new object[] { tempList[0], tempList[1], tempList[2] });
	}

	const string DidStatusManager = "StatusManager.Did";

	//[PunRPC]
	public void ReceiveMPCrystalOutcome(int statusId, int unitId, int willCrystalize)
	{
		List<int> tempList = new List<int>();
		tempList.Add(statusId);
		tempList.Add(unitId);
		tempList.Add(willCrystalize);

		this.PostNotification(DidStatusManager, tempList);
	}

	//online: called by master to tell P2 to remove an item on the board
	public void SendMPRemoveTilePickUp(int tileX, int tileY, bool isEnable, int tilePickUpType)
	{
		//for now just removing but in future can use isEnable to add
		//photonView.RPC("ReceiveMPRemoveTilePickUp", PhotonTargets.Others, new object[] { tileX, tileY, tilePickUpType });
	}

	const string MultiplayerTilePickUp = "Multiplayer.RemoveTilePickUp";

	//[PunRPC]
	public void ReceiveMPRemoveTilePickUp(int tileX, int tileY, int tilePickUpType)
	{
		List<int> tempList = new List<int>();
		tempList.Add(tileX);
		tempList.Add(tileY);
		tempList.Add(tilePickUpType);

		this.PostNotification(MultiplayerTilePickUp, tempList);
	}

	//[PunRPC]
	public void SendMPQuitGame(bool isSelfQuit)
	{
		//if(!PhotonNetwork.offlineMode)
		//{
		//    photonView.RPC("SendMPQuitGame", PhotonTargets.Others, new object[] { !isSelfQuit });
		//}
		this.PostNotification(NameAll.NOTIFICATION_EXIT_GAME, isSelfQuit);

	}

	const string MultiplayerGameOver = "Multiplayer.GameOver";

	//online: called by Master in CombatCutSceneState to tell Other game is over
	//[PunRPC]
	public void SendMPGameOver()
	{
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    photonView.RPC("SendMPGameOver", PhotonTargets.Others, new object[] { });
		//}
		//else
		//{
		//    this.PostNotification(MultiplayerGameOver);
		//}
	}

	const string MultiplayerMessageNotification = "Multiplayer.Message";

	//[PunRPC]
	public void SendMPBattleMessage(string zString)
	{
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    photonView.RPC("SendMPBattleMessage", PhotonTargets.Others, new object[] { zString });
		//}
		//else
		//{
		//    this.PostNotification(MultiplayerMessageNotification, zString);
		//}
	}

	//obviously better to use Photon.PunBehavour OnPhotonPlayerDisconnected but that requires the main class inheriting from Photon.PunBehaviour
	public int GetMPNumberOfPlayers()
	{
		//sMPObject.NumberOfPlayers = PhotonNetwork.playerList.Length;
		return sMPObject.NumberOfPlayers;
	}

	//used by master at start of certain states to set all to false, only turned to true when both sides ready or master override (which shouldn't be needed)
	//[PunRPC]
	//public void SetMPReadyStatusAll(bool isReady, bool isOpponentReady)
	//{
	//    sMPObject.IsReady = isReady;
	//    sMPObject.IsOpponentReady = isOpponentReady;
	//    if( !PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
	//        photonView.RPC("SetMPReadyStatusAll", PhotonTargets.Others, new object[] { isReady,isOpponentReady });

	//}

	//public void SetMPSelfReadyStatus(bool isReady)
	//{
	//    sMPObject.IsReady = isReady;
	//    if (!PhotonNetwork.offlineMode)
	//        photonView.RPC("SendMPReadyStatus", PhotonTargets.Others, new object[] { isReady});
	//}

	//[PunRPC]
	//public void SendMPReadyStatus(bool readyStatus)
	//{
	//    sMPObject.IsOpponentReady = readyStatus;
	//}
	#endregion

	public void SetPhotonNetworkOfflineMode(bool isOffline)
	{
		//PhotonNetwork.offlineMode = true;
	}

	#region combatstats
	private static CombatStats sCombatStats;

	public void InitializeCombatStats(bool isNull, int battleXP, int battleAP)
	{
		if (isNull)
			sCombatStats = null;
		else
			sCombatStats = new CombatStats(sPlayerUnitList, battleAP, battleXP);
	}

	//for now just post combat AP and XP but can add more shit in the future for teams, mid combat stats etc
	const string CombatStatsShow = "CombatStats.Show";

	public void ShowCombatStats()
	{
		if (sCombatStats != null)
			this.PostNotification(CombatStatsShow);
	}

	public CombatStats GetCombatStats()
	{
		return sCombatStats;
	}

	public void TallyCombatStats(int unitId, int statType, int statValue)
	{
		if (sCombatStats != null)
		{
			int teamId = GetPlayerUnit(unitId).TeamId;
			sCombatStats.AddStatValue(unitId, teamId, statType, statValue);
		}
	}

	#endregion

	#region TeamID and Alliances
	//assigns team IDs and alliances between teams

	//assigns unique team ids
	int AssignUniqueTeamId()
	{
		sUniqueTeamId += 1;
		return sUniqueTeamId;
	}

	//assigns teamIds for combat based on the temporary ids
	//see descriptions of predefined ones in NameAll
	//called in WalkAroundInitState and CombatInitState
	//populates dicts that store player units by team id and isAbletoFightDict (for end of combat)
	public void AssignTeamIdToPlayerUnit(int unitId)
	{
		PlayerUnit pu = GetPlayerUnit(unitId);
		if (pu.TeamId == NameAll.TEAM_ID_GREEN || pu.TeamId == NameAll.TEAM_ID_RED || pu.TeamId == NameAll.TEAM_ID_NEUTRAL || pu.TeamId == NameAll.TEAM_ID_HOSTILE_TEAM)
		{
			//remain on current team, predefined rules
			if (!sTeamIdToType.ContainsKey(pu.TeamId))
			{
				sTeamIdToType.Add(pu.TeamId, pu.TeamId);
			}
		}
		else if (pu.TeamId == NameAll.TEAM_ID_NONE || pu.TeamId == NameAll.TEAM_ID_HOSTILE_NONE)
		{
			//each unit assigned to its own team
			int z1 = AssignUniqueTeamId();
			if (!sTeamIdToType.ContainsKey(pu.TeamId))
				sTeamIdToType.Add(pu.TeamId, pu.TeamId);
			pu.TeamId = z1;
		}
		else if (pu.TeamId == sLocalTeamId)
		{
			if (!sTeamIdToType.ContainsKey(pu.TeamId))
				sTeamIdToType.Add(pu.TeamId, NameAll.TEAM_ID_LOCAL);
		}
		else
		{
			//other teams default to friendly to same team number and neutral to all others
			if (sTempIdToTeamIdDict.ContainsKey(pu.TeamId))
			{ //teamId already assigned, grab same teamId to be on ally team
				pu.TeamId = sTempIdToTeamIdDict[pu.TeamId];
			}
			else
			{
				int z1 = AssignUniqueTeamId();
				if (!sTeamIdToType.ContainsKey(pu.TeamId))
					sTeamIdToType.Add(pu.TeamId, NameAll.TEAM_ID_NEUTRAL);
				sTempIdToTeamIdDict.Add(pu.TeamId, z1); //teammates will get the same team Id
				pu.TeamId = z1;
			}
		}
		//set up teamId dicts
		sTeamIdToIsAbleToFight[pu.TeamId] = true;
		//if teamId does not exist in sTeamIdToPU, need to create a new list for that dictionary
		if (!sTeamIdToPU.ContainsKey(pu.TeamId))
		{
			sTeamIdToPU[pu.TeamId] = new List<int>();
		}
		//team id exists, add new unit to the teamId list
		sTeamIdToPU[pu.TeamId].Add(pu.TurnOrder);

	}

	//assigns alliances between all team combos
	//called in WalkAroundInitState after all units assigned teams
	public void AssignAlliances()
	{
		List<int> tempList = new List<int>(sTeamIdToType.Keys);
		int z1 = tempList.Count;

		for (int i = 0; i < z1; i++)
		{
			int alliance1 = tempList[i];
			for (int j = i; j < z1; j++)
			{
				int alliance2 = tempList[j];
				if (!sAllianceDict.ContainsKey(new Point(alliance1, alliance2)))
				{
					Alliances tempAlliances = ComputeAlliances(alliance1, alliance2);
					sAllianceDict.Add(new Point(alliance1, alliance2), tempAlliances);

					if (alliance1 != alliance2)
					{
						sAllianceDict.Add(new Point(alliance2, alliance1), tempAlliances);
						//populate sTeamEnemyDict that tells of current hostilities so we can tell when combat ends
						if (tempAlliances == Alliances.Enemy)
						{
							if (alliance1 < alliance2)
								sTeamEnemyDict.Add(new Point(alliance1, alliance2), true);
							else
								sTeamEnemyDict.Add(new Point(alliance2, alliance1), true);
						}

					}
				}

			}
		}
	}

	Alliances ComputeAlliances(int teamId1, int teamId2)
	{
		int team1Type = sTeamIdToType[teamId1];
		int team2Type = sTeamIdToType[teamId2];

		if (team1Type == NameAll.TEAM_ID_HOSTILE_NONE || team1Type == NameAll.TEAM_ID_HOSTILE_TEAM
			|| team2Type == NameAll.TEAM_ID_HOSTILE_NONE || team2Type == NameAll.TEAM_ID_HOSTILE_TEAM)
		{
			return Alliances.Enemy;
		}
		else if (team1Type == NameAll.TEAM_ID_NEUTRAL || team2Type == NameAll.TEAM_ID_NEUTRAL)
		{
			return Alliances.Neutral;
		}
		else if (team1Type == NameAll.TEAM_ID_GREEN && team2Type == NameAll.TEAM_ID_RED)
		{
			return Alliances.Enemy;
		}
		else if (team1Type == NameAll.TEAM_ID_RED && team2Type == NameAll.TEAM_ID_GREEN)
		{
			return Alliances.Enemy;
		}
		else if (team1Type == NameAll.TEAM_ID_GREEN && team2Type == NameAll.TEAM_ID_GREEN)
		{
			return Alliances.Allied;
		}
		else if (team1Type == NameAll.TEAM_ID_RED && team2Type == NameAll.TEAM_ID_RED)
		{
			return Alliances.Allied;
		}
		else if (team1Type == NameAll.TEAM_ID_NONE && team2Type == NameAll.TEAM_ID_NONE)
		{
			return Alliances.None;
		}
		else if (team1Type == NameAll.TEAM_ID_NONE && team2Type == NameAll.TEAM_ID_NONE)
		{
			return Alliances.None;
		}
		return Alliances.Neutral;
	}

	//when a new unit shows up
	void AssignAlliancesMidBattle(int newTeamId)
	{
		List<int> tempList = new List<int>(sTeamIdToType.Keys);
		int z1 = tempList.Count;
		for (int i = 0; i < z1; i++)
		{
			if (i == newTeamId)
				continue;

			if (!sAllianceDict.ContainsKey(new Point(i, newTeamId)))
			{
				Alliances tempAlliances = ComputeAlliances(i, newTeamId);
				sAllianceDict.Add(new Point(i, newTeamId), tempAlliances);
				sAllianceDict.Add(new Point(newTeamId, i), tempAlliances);
			}
		}
	}
	//certain actions can change how alliances are (ie attacking a neutral unit can make it angry), reassign them here
	public void ReassignAlliance(int teamId1, int teamId2, Alliances a)
	{
		sAllianceDict[new Point(teamId1, teamId2)] = a;
		sAllianceDict[new Point(teamId2, teamId1)] = a;
	}

	//check alliance for two different teams
	public Alliances GetAlliances(int teamId1, int teamId2)
	{
		//Debug.Log("alliances for team1,team" + teamId1 + ", " + teamId2);
		return sAllianceDict[new Point(teamId1, teamId2)];
	}

	//check alliance for local player vs. a teamId
	public Alliances GetAlliances(int teamId1)
	{
		//Debug.Log("alliances for team1,team " + teamId1 + ", " + sLocalTeamId);
		//Debug.Log("outputting contents of sAllianceDict");
		//foreach (KeyValuePair<Point, Alliances> entry in sAllianceDict)
		//{
		//	Debug.Log("outputting contents of sAllianceDict " + entry.Key + "--" + entry.Value);
		//}
		return sAllianceDict[new Point(teamId1, sLocalTeamId)];
	}

	//placeholder
	public void SetLocalTeamId(int tempTeamId)
	{
		sLocalTeamId = tempTeamId;
	}

	public int GetLocalTeamId()
	{
		return sLocalTeamId;
	}

	//functions for telling if combat is over
	//combat over if team is unableToFight or flees
	//everytime unableToFight status is updated (from StatusManager) call to CheckForEndCombat
	//if team becomes able to fight, update sTeamIdToAbleToFight = true
	//if unit unable to fight, check if team is defeated (UpdateTeamStatus)
	//if team is defeated, check alliances dict to see if still hostility between remaining alliances
	//if combat is over PlayerManager tells WalkAround mainState
	//called in StatusManager any time a unit receives a unable to fight condition
	public void CheckForEndCombat(PlayerUnit pu, bool isAbleToFight)
	{
		bool isTeamUnable = UpdateTeamStatus(pu, isAbleToFight);
		if (!isTeamUnable)
		{
			SendTeamDefeated(pu.TeamId);
			if (!IsTeamEnemyAbleToFight())
			{
				//no more active enemies, combat is over
				SendCombatEnd();
			}
		}

	}

	//updates sTeamIdToIsAbleToFight after a PU ableToFight status has changed
	//used in CheckForEndCombat to see if combat is over
	bool UpdateTeamStatus(PlayerUnit pu, bool isAbleToFight)
	{
		if (isAbleToFight)
		{
			//if team is able to right again, update sTeamEnemyDict so that active hostilities are known
			if (!sTeamIdToIsAbleToFight[pu.TeamId])
			{
				UpdateTeamEnemyDict(pu.TeamId, isAbleToFight);
			}
			sTeamIdToIsAbleToFight[pu.TeamId] = true;

			return true;

		}
		else
		{
			foreach (int i in sTeamIdToPU[pu.TeamId])
			{
				if (GetPlayerUnit(i).AbleToFight)
				{
					return true;
				}
			}
			UpdateTeamEnemyDict(pu.TeamId, isAbleToFight);
			sTeamIdToIsAbleToFight[pu.TeamId] = false;

			return false;
		}
	}

	//update if team enemy pairs are still active based on PU status changing
	void UpdateTeamEnemyDict(int teamId, bool isAbleToFight)
	{
		if (isAbleToFight)
		{
			List<Point> keys = new List<Point>(sTeamEnemyDict.Keys);
			foreach(Point p in keys)
			{
				if( p.x == teamId)
				{
					if (sTeamIdToIsAbleToFight[p.y])
						sTeamEnemyDict[p] = true;
				}
				else if (p.y == teamId)
				{
					if (sTeamIdToIsAbleToFight[p.x])
						sTeamEnemyDict[p] = true;
				}
			}
		}
		else
		{
			List<Point> keys = new List<Point>(sTeamEnemyDict.Keys);
			foreach (Point p in keys)
			{
				if (p.x == teamId || p.y == teamId)
				{
					sTeamEnemyDict[p] = false;

				}
			}
		}
	}

	bool IsTeamEnemyAbleToFight()
	{
		foreach (KeyValuePair<Point, bool> entry in sTeamEnemyDict)
		{
			if (entry.Value)
				return true;
		}
		return false;
	}

	//when a player quits, notification sent from the quit menu
	//listened in WalkAroundMainState.OnQuitNotification, sends message here
	public void TeamQuit(int teamId)
	{
		UpdateTeamEnemyDict(teamId, false);
		if (!IsTeamEnemyAbleToFight())
		{
			//no more active enemies, combat is over
			SendCombatEnd();
		}
	}

	#endregion

	#region WalkAround

	//combat start check
	//called in CalculationResolveAction
	public void WalkAroundCombatStartCheck(PlayerUnit actor, PlayerUnit target, SpellName sn)
	{
		if (isWalkAroundMoveAllowed)
		{
			Alliances allianceType = GetAlliances(actor.TeamId, target.TeamId);
			if (sn.RemoveStat != NameAll.REMOVE_STAT_HEAL && allianceType != Alliances.Allied)
			{
				SendCombatStart();
				//isWalkAroundMoveAllowed = false; //setting this in WalkAroundMainState
			}
		}

	}

	//In WalkAround mode, If unit is within x tiles of someone not on team, send CombatStart notification
	//called from PUO of the unit doing the moving after every tile move
	public void CheckCombatProximity(Tile t, int actorId)
	{
		//Debug.Log("PlayerManagerProximityCombatTrigger 1...");
		//Tile is within proximity range of moving character actor
		if (t.UnitId != NameAll.NULL_UNIT_ID)
		{
			//Debug.Log("PlayerManagerProximityCombatTrigger 2...");
			if (GetPlayerUnit(t.UnitId).TeamId != GetPlayerUnit(actorId).TeamId)
			{
				//Debug.Log("PlayerManagerProximityCombatTrigger 3...");
				SendCombatStart();
			}
			//Dictionary<int, int> teamIdDict = new Dictionary<int, int>();
			//teamIdDict[PlayerManager.Instance.GetPlayerUnit(actorId).TeamId] = 1;
			//if (IsTeamIdHostile(teamIdDict))
			//{
			//	Debug.Log("PlayerManagerProximityCombatTrigger 3...");
			//	SendCombatStart();
			//}
		}
	}

	//used to check if units that are close to each other are in fact hostile
	//for now simply if on different teams then they are hostile
	bool IsTeamIdHostile(Dictionary<int, int> teamIdDict)
	{

		if (teamIdDict.Count() >= 1)
			return true;

		return false;
	}

	public bool GetWalkAroundMoveAllowed()
	{
		return isWalkAroundMoveAllowed;
	}

	//called from WalkAroundMainState
	public void SetWalkAroundMoveAllowed(bool zBool)
	{
		isWalkAroundMoveAllowed = zBool;
	}

	public void ClearWalkAroundLists()
	{
		sWalkAroundActionList = new List<WalkAroundActionObject>();
		sWalkAroundPlayerWaao = new Dictionary<int, int>();
		sWalkAroundLockDict = new Dictionary<int, bool>();
		sWalkAroundSpellSlowQueue = new Dictionary<int, List<SpellSlow>>();
		for (int i = 0; i < MAX_WALK_AROUND_CTR; i++)
		{
			sWalkAroundSpellSlowQueue[i] = new List<SpellSlow>();
		}
	}

	//PlayerUnit enters an action, goes into queue and/or rewrites existing one
	public void AddWalkAroundActionObject(CombatTurn tu)
	{
		//chance that a unit has had something inflicted to him during the turn that prevents the waao from being added
		if (!StatusManager.Instance.IsTurnActable(tu.actor.TurnOrder))
			return;

		if (!IsCheckAndRemoveQueue(tu.actor.TurnOrder)) //checks that WAAO can be added
			return;

		sWalkAroundPlayerWaao[tu.actor.TurnOrder] = HAS_WAAO_IN_QUEUE;
		sWalkAroundActionList.Add(new WalkAroundActionObject(tu));
		SendTurnInQueue(tu.actor.TurnOrder);
	}


	//sees if the entered action can still be executed
	//if targetting a unit, is the unit still in range
	//if moving to a tile, is the tile still a valid move
	public WalkAroundActionObject GetWalkAroundActionObjectValid(Board board, int unitId)
	{
		if (sWalkAroundPlayerWaao[unitId] == HAS_SS_IN_QUEUE)
		{
			WalkAroundActionObject waao = GetWalkAroundActionObject(unitId);
			//check if move is valid
			//check if action is valid (move/act order checked in CombatAbilityRange)
			PlayerUnit actor = GetPlayerUnit(unitId);
			var tilesInRange = GetPlayerUnitObjectComponent(unitId).GetTilesInRange(board, board.GetTile(actor), actor);
			if (tilesInRange.Contains(waao.moveTile))
			{
				CombatAbilityRange ar = new CombatAbilityRange();
				waao = ar.IsAbilityTargetInRange(board, waao, actor);
				return waao;//set to null in CombatAbilityRange if not in range
			}
		}

		return null;
	}

	public void AddWalkAroundSpellSlow(SpellSlow ss)
	{
		//chance that a unit has had something inflicted to him during the turn that prevents the waao from being added
		if (!StatusManager.Instance.IsTurnActable(ss.UnitId))
			return;

		if (!IsCheckAndRemoveQueue(ss.UnitId)) //can't replace the old WAAO in the queue
			return;

		sWalkAroundPlayerWaao[ss.UnitId] = HAS_SS_IN_QUEUE;
		//what tick to store it in
		int z1 = ss.CTR;
		if (z1 > MAX_WALK_AROUND_CTR - 1)
			z1 = MAX_WALK_AROUND_CTR - 1;
		z1 = (walkAroundTick + z1) % MAX_WALK_AROUND_CTR;
		sWalkAroundSpellSlowQueue[z1].Add(ss);
	}

	//called when adding a WAAO or SS to queue
	//checks if WAAO currently exists
	//if one does then removes it
	//called from places where WAAO and SS are added to queue, lets them handle the sWalkAroundPlayerWaao
	//return true if cancheck and remove, false if can't (already a spellslow for that unit that hasn't resolved)
	private bool IsCheckAndRemoveQueue(int unitId)
	{
		if (sWalkAroundPlayerWaao.ContainsKey(unitId))
		{
			if (sWalkAroundPlayerWaao[unitId] == HAS_WAAO_IN_QUEUE)
			{
				foreach (WalkAroundActionObject o in sWalkAroundActionList.ToList())
				{
					if (unitId == o.turn.actor.TurnOrder)
					{
						sWalkAroundActionList.Remove(o);
						RemoveWAAOFlag(unitId);
						return true;
					}
				}
			}
			else if (sWalkAroundPlayerWaao[unitId] == HAS_SS_IN_QUEUE) {
				//ug this is bad code. should change it from O(N) to O(1)
				for (int i = 0; i < MAX_WALK_AROUND_CTR; i++)
				{
					var tempList = sWalkAroundSpellSlowQueue[i];
					foreach (SpellSlow ss in tempList.ToList())
					{
						if (ss.UnitId == unitId)
						{
							tempList.Remove(ss);
							RemoveWAAOFlag(unitId);
							return true;
						}
					}
				}
			}
			else if (sWalkAroundPlayerWaao[unitId] == HAS_SS_IN_SPELL_MANAGER)
			{
				//cant do anything until this item is removed from slow action being cast
				return false;
			}
		}
		return true;
	}

	//removes all WAAO for UnitId
	//from sWalkAroundActionList and sWalkAroundSpellSlowQueue and changes flag back
	public void RemoveWAAO(int unitId)
	{
		if (sWalkAroundPlayerWaao.ContainsKey(unitId))
		{
			if (sWalkAroundPlayerWaao[unitId] == HAS_WAAO_IN_QUEUE)
			{
				foreach (WalkAroundActionObject o in sWalkAroundActionList.ToList())
				{
					if (unitId == o.turn.actor.TurnOrder)
					{
						sWalkAroundActionList.Remove(o);
					}
				}
			}
			else if (sWalkAroundPlayerWaao[unitId] == HAS_SS_IN_QUEUE)
			{
				//ug this is bad code. should change it from O(N) to O(1)
				for (int i = 0; i < MAX_WALK_AROUND_CTR; i++)
				{
					var tempList = sWalkAroundSpellSlowQueue[i];
					foreach (SpellSlow ss in tempList.ToList())
					{
						if (ss.UnitId == unitId)
						{
							tempList.Remove(ss);
						}
					}
				}
			}
		}
		RemoveWAAOFlag(unitId);
	}

	//Function for manipulating sWalkAroundPlayerWaao, should be more widely used
	public void RemoveWAAOFlag(int unitId)
	{
		if (sWalkAroundPlayerWaao.ContainsKey(unitId))
		{
			sWalkAroundPlayerWaao.Remove(unitId);
		}
	}

	private void ChangeWAAOFlag(int unitId, int waaoType)
	{
		sWalkAroundPlayerWaao[unitId] = waaoType;
	}

	//called in ConsumeNextWalkAroundActionObject (if unitId = -1919
	//removes the next WAAO for processing
	//during combat can be called by unitId to get a specific unit's WAAO
	//can also remove WAAO if player cancels a previously entered action
	public WalkAroundActionObject GetWalkAroundActionObject(int unitId = -19, bool removeWAAO = true)
	{

		if (sWalkAroundActionList.Count > 0)
		{
			//Debug.Log("is a WAAO in queue, grabbing it 0");
			if (unitId == NameAll.NULL_UNIT_ID)
			{
				//Debug.Log("is a WAAO in queue, grabbing it 1");
				var retValue = sWalkAroundActionList[0];
				if (removeWAAO)
					sWalkAroundActionList.RemoveAt(0);
				return retValue;
			}
			else
			{
				//Debug.Log("is a WAAO in queue, grabbing it 2");
				foreach (WalkAroundActionObject waao in sWalkAroundActionList.ToList())
				{
					if (unitId == waao.turn.actor.TurnOrder)
					{
						if (removeWAAO)
							sWalkAroundActionList.Remove(waao);
						return waao;
					}
				}
			}
		}
		return null;
	}

	//checks if is a WAAO for a unit, called in turns menu
	public bool IsWalkAroundActionObjectInQueue(int unitId)
	{
		if (sWalkAroundPlayerWaao.ContainsKey(unitId) && sWalkAroundPlayerWaao[unitId] == HAS_WAAO_IN_QUEUE)
		{
			return true;
		}
		return false;
	}

	//increment the walkAroundTick then add the spellslows
	public void IncrementWalkAroundTick()
	{
		walkAroundTick = (walkAroundTick + 1) % MAX_WALK_AROUND_CTR;

		//check spellslow first, if no spell slow check for WalkAroundActionObject
		if (sWalkAroundSpellSlowQueue[walkAroundTick].Count() > 0)
		{
			//check for user able to do the ss in the slow action state (since this will change as slow actions go off)
			foreach (SpellSlow ss in sWalkAroundSpellSlowQueue[walkAroundTick])
			{
				SpellManager.Instance.AddWalkAroundSpellSlow(ss);
				ChangeWAAOFlag(ss.UnitId, HAS_SS_IN_SPELL_MANAGER);
			}
			sWalkAroundSpellSlowQueue[walkAroundTick].Clear();
			return; //Goes to WalkAroundMainState goes to proper state in order to resolve
		}
	}

	//Consumes next WalkAroundActionObject, performs locking, executes the waao
	//called in WalkAroundMainState
	//spell slows handled in IncrementWalkAroundTick, only come here if no spellslows
	public bool ConsumeNextWalkAroundActionObject(Board board, CalculationMono calcMono)
	{

		WalkAroundActionObject waao = GetWalkAroundActionObject();
		if (waao == null)
			return false;

		//chance that a unit has had something inflicted to him to prevent action from going
		if (!StatusManager.Instance.IsTurnActable(waao.turn.actor.TurnOrder))
			return false;

		//is a waao to do, start the coroutine stuff for the inner consme
		AddWalkAroundLock(waao.GetTurnOrder());
		//Debug.Log("before start coroutine");
		StartCoroutine(ConsumeWAAOInner(board, calcMono, waao));
		//Debug.Log("after start coroutine");
		RemoveWalkAroundLock(waao.GetTurnOrder());
		return true;
	}

	IEnumerator ConsumeWAAOInner(Board board, CalculationMono calcMono, WalkAroundActionObject waao)
	{
		//add unit to locked list
		//check if move or act first
		//execute move/act in proper order
		//end facing direction
		//remove unit from locked list

		board.UnhighlightTile(waao.turn.actor); //unhighlight tile unit is on
		if (waao.turn.isWalkAroundMoveFirst)
		{
			//Debug.Log("in ConsumeWAAOInner 0");
			yield return StartCoroutine(DoWalkAroundMove(board, waao));
			if (waao.turn.spellName != null)
			{
				if (SpellManager.Instance.IsSpellTargetable(waao))
					yield return StartCoroutine(DoWalkAroundAction(board, calcMono, waao));
			}
		}
		else
		{
			//Debug.Log("in ConsumeWAAOInner 1");
			yield return StartCoroutine(DoWalkAroundAction(board, calcMono, waao));
			yield return StartCoroutine(DoWalkAroundMove(board, waao));
		}
		//Debug.Log("in ConsumeWAAOInner 2");
		//yield return new WaitForSeconds(0.5f); //looks better to have the attack turn then the wait but why slow shit down?
		yield return DoWalkAroundEndTurn(waao);
		yield return null;
	}

	//in case unit dies, remove the waao from queue
	//also called in UITurnsScrollList when queue'd turn is cancelled
	public void RemoveWalkAroundObjectByUnitId(int unitId)
	{
		foreach (WalkAroundActionObject waao in sWalkAroundActionList.ToList())
		{
			if (unitId == waao.turn.actor.TurnOrder)
			{
				sWalkAroundActionList.Remove(waao);
			}
		}
	}

	IEnumerator DoWalkAroundAction(Board board, CalculationMono calcMono, WalkAroundActionObject waao)
	{
		if (waao.turn.spellName != null)
		{
			List<int> lockList = LockWalkAroundTargets(board, waao);
			//if slow action with CTR > 0 add to queue
			calcMono.DoFastAction(board, waao.turn, isActiveTurn: true, isReaction: false, isMime: false, isWAMode: true, renderMode: sRenderMode);
			//yield return StartCoroutine(calcMono.DoFastActionInner(board, waao.turn, isActiveTurn: true, isSlowActionPhase: false,isReaction: false, isMime: false, isWAMode: true));
			foreach (int i in lockList)
			{
				RemoveWalkAroundLock(i);
			}
		}

		yield return null;
	}

	IEnumerator DoWalkAroundMove(Board board, WalkAroundActionObject waao)
	{
		if (waao.turn.walkAroundMoveTile != null)
		{
			//Debug.Log("In do walk around move, I should be moving here...");
			PlayerUnitObject puo = GetPlayerUnitObjectComponent(waao.turn.actor.TurnOrder);
			yield return StartCoroutine(puo.TraverseWalkAround(board, waao.turn.walkAroundMoveTile));
			DoWalkAroundEndTurn(waao); //change turning direction
									   //unit tile set in PlayerUnitObject as in WalkAround mode you might not end up on the tile you clicked on
									   //SetUnitTile(board, waao.turn.actor.TurnOrder, puo.walkAroundEndTile);
		}
		yield return null;
	}

	//sets facing direction for unit
	IEnumerator DoWalkAroundEndTurn(WalkAroundActionObject waao)
	{
		GetPlayerUnit(waao.turn.actor.TurnOrder).Dir = waao.turn.endDir;
		PlayerUnitObject puo = GetPlayerUnitObjectComponent(waao.turn.actor.TurnOrder);
		puo.SetFacingDirectionEndTurn(waao.turn.actor);
		yield return null;
		//yield return puo.SetFacingDirectionEndTurn(waao.turn.actor);
		//yield return StartCoroutine(puo.Turn(waao.turn.endDir));
		//yield return null;
	}

	//get details of queued WAAO for turns list
	public string GetWalkAroundActionObjectDescription(int unitId)
	{
		string zString;
		if (sWalkAroundPlayerWaao[unitId] == HAS_SS_IN_QUEUE)
		{
			WalkAroundActionObject waao = GetWalkAroundActionObject(unitId, false);
			zString = "" + waao.turn.actor.UnitName + " ";
			if (waao.turn.isWalkAroundMoveFirst)
			{
				zString = GetWAAOMoveDescription(waao, zString);
				zString = " " + waao.turn.actor.UnitName + " ";
				zString = GetWAAOActionDescription(waao, zString);
			}
			else
			{
				zString = GetWAAOActionDescription(waao, zString);
				zString = " " + waao.turn.actor.UnitName + " ";
				zString = GetWAAOMoveDescription(waao, zString);
			}
		}
		else
		{
			zString = "No turn found.";
		}

		return zString;
	}

	//called in GetWalkAroundActionObjectDescription for description for turns list
	private string GetWAAOActionDescription(WalkAroundActionObject waao, string zString)
	{
		zString += "casts " + waao.turn.spellName.AbilityName + " on ";
		if (waao.turn.targetUnitId == NameAll.NULL_UNIT_ID)
		{
			zString += "tile " + waao.turn.targetTile.pos.x + ", " + waao.turn.targetTile.pos.y + ". ";
		}
		else
		{
			zString += " " + GetPlayerUnit(waao.turn.targetUnitId).UnitName + ". ";
		}
		return zString;
	}

	private string GetWAAOMoveDescription(WalkAroundActionObject waao, string zString)
	{

		if (waao.moveTile != null)
		{
			zString += "moves to ";
			zString += "tile " + waao.turn.targetTile.pos.x + ", " + waao.turn.targetTile.pos.y;
		}
		else
		{
			zString += " doesn't move. ";
		}
		return zString;
	}

	#endregion

	#region WalkAround PlayerUnit Eligibility
	//called in WalkAroundMainState
	//concept: players can move and control their units unless one of two things:
	//that unit currently has 'focus' and is being moved
	//that unit is the target of an action
	//else actions pulled from the sWalkAroundActionList (ConsumeNextWalkAroundActionObject)

	//check if unit is able to be controlled (ie not locked) and on the proper team)
	public bool IsWalkAroundPlayerUnitEligible(PlayerUnit pu)
	{
		//Debug.Log("reached here");
		int unitId = pu.TurnOrder;
		//Debug.Log("isWalkAroundPlayerUnitEligible " + IsWalkAroundPlayerUnitLocked(unitId) + IsWalkAroundPlayerUnitControllable(pu));
		if (IsWalkAroundPlayerUnitLocked(unitId))
		{
			return false;
		}
		else if (IsWalkAroundPlayerUnitControllable(pu)) {
			return true;
		}
		return false;
	}

	bool IsWalkAroundPlayerUnitLocked(int id)
	{
		if (sWalkAroundLockDict.ContainsKey(id))
		{
			return true;
		}
		return false;
	}

	//check that unit can move and is on the proper team
	bool IsWalkAroundPlayerUnitControllable(PlayerUnit pu)
	{
		//Debug.Log("isWalkAroundPlayerUnitControllable " + StatusManager.Instance.IsTurnActable(pu.TurnOrder) + pu.TeamId + PlayerManager.Instance.GetLocalTeamId());
		if (StatusManager.Instance.IsTurnActable(pu.TurnOrder))
		{
			if (pu.TeamId == PlayerManager.Instance.GetLocalTeamId())
			{
				return true;
			}
		}

		return false;
	}

	void AddWalkAroundLock(int id)
	{
		if (!sWalkAroundLockDict.ContainsKey(id))
		{
			sWalkAroundLockDict.Add(id, true);
		}
	}

	void RemoveWalkAroundLock(int id)
	{
		sWalkAroundLockDict.Remove(id);
	}

	//locks potential targets prior to walkAround action being resolved
	//chance targets can be locked but still move out of range prior to spell resolving on them but not a big deal if that happens
	List<int> LockWalkAroundTargets(Board board, WalkAroundActionObject waao)
	{
		CombatAbilityRange car = new CombatAbilityRange();
		var lockList = car.GetWalkAroundTargets(board, waao);
		foreach (int i in lockList)
		{
			AddWalkAroundLock(i);
		}
		return lockList;
	}

	public void SetGameMode(int mode)
	{
		sGameMode = mode;
	}

	public void SetRenderMode(int mode)
	{
		sRenderMode = mode;
	}

	public int GetRenderMode()
	{
		return sRenderMode;
	}

	public int GetGameMode()
	{
		return sGameMode;
	}

	const string InputtedTurn = "InputtedTurn";

	//[PunRPC]
	public void WalkAroundInputTurn(CombatTurn tempTurn)
	{
		this.PostNotification(InputtedTurn, tempTurn);
	}



	#endregion

	#region WA map dictionary
	//functions for helping create new  maps, load existing ones, and saving map data
	//sWalkAroundMapDictionary saves info like seed, time_int and current map coordinates
	//other keys are the map coordinates themselves and whether the map has been visited
	//maps are generated based on seed and coordinates
	public void InitializeMapDictionary(int seed, int timeInt)
	{
		sWalkAroundMapDictionary = new Dictionary<Tuple<int, int>, Tuple<int, int>>();
		var seedKey = new Tuple<int, int>(NameAll.MAP_DICT_SEED, NameAll.MAP_DICT_SEED);
		var timeIntKey = new Tuple<int, int>(NameAll.MAP_DICT_TIME_INT, NameAll.MAP_DICT_TIME_INT);
		var currentMapKey = new Tuple<int, int>(NameAll.MAP_DICT_CURRENT_MAP, NameAll.MAP_DICT_CURRENT_MAP);

		sWalkAroundMapDictionary[seedKey] = new Tuple<int, int>(seed, seed);
		sWalkAroundMapDictionary[timeIntKey] = new Tuple<int, int>(timeInt, timeInt);
		sWalkAroundMapDictionary[currentMapKey] = new Tuple<int, int>(0, 0);
	}

	public int GetWalkAroundSeed()
	{
		//if(sWalkAroundSeed == NameAll.NULL_INT) //have to set the seed and time int
		//{
		//	sWalkAroundSeed = System.Environment.TickCount;
		//	sWalkAroundTimeInt = (int)Time.time;

		//}
		var seedKey = new Tuple<int, int>(NameAll.MAP_DICT_SEED, NameAll.MAP_DICT_SEED);
		return sWalkAroundMapDictionary[seedKey].Item1;
	}

	public int GetWalkAroundTimeInt()
	{
		var timeIntKey = new Tuple<int, int>(NameAll.MAP_DICT_TIME_INT, NameAll.MAP_DICT_TIME_INT);
		return sWalkAroundMapDictionary[timeIntKey].Item1;
	}

	//int GetNextMapInt()
	//{
	//	var currentMapKey = new Tuple<int, int>(NameAll.MAP_DICT_CURRENT_MAP, NameAll.MAP_DICT_CURRENT_MAP);
	//	var createdMapsKey = new Tuple<int, int>(NameAll.MAP_DICT_CREATED_MAPS, NameAll.MAP_DICT_CREATED_MAPS);
	//	//created maps number is always +1 over the max current mapId so grab created maps number THEN incremen it
	//	sWalkAroundMapDictionary[currentMapKey] = sWalkAroundMapDictionary[createdMapsKey]; //set current map to the new mapId since after load this will be new map
	//	sWalkAroundMapDictionary[createdMapsKey] = sWalkAroundMapDictionary[createdMapsKey] + 1;
	//	return sWalkAroundMapDictionary[currentMapKey];
	//}

	public void SetMapXY(int map_x, int map_y)
	{
		var currentMapKey = new Tuple<int, int>(NameAll.MAP_DICT_CURRENT_MAP, NameAll.MAP_DICT_CURRENT_MAP);
		sWalkAroundMapDictionary[currentMapKey] = new Tuple<int, int>(map_x, map_y);
	}

	//WA: when leaving a map, set this so on loading next map have the correct coordinates
	public void SetMapXYOnMapExit(PlayerUnit pu, Point max, Point min)
	{
		int x_offset = 0;
		int y_offset = 0;
		var currentMapKey = new Tuple<int, int>(NameAll.MAP_DICT_CURRENT_MAP, NameAll.MAP_DICT_CURRENT_MAP);
		var current_xy = sWalkAroundMapDictionary[currentMapKey];

		int current_x = current_xy.Item1;
		int current_y = current_xy.Item2;
		//for now based on edge of map
		if (pu.TileX == min.x)
		{
			//move west, ie x -=1
			x_offset += -1;
		}
		else if (pu.TileY == min.y)
		{
			//move south, ie y -=1
			y_offset += -1;
		}
		else if (pu.TileX == max.x)
		{
			//move east, ie x +=1
			x_offset += 1;
		}
		else
		{
			y_offset += 1; //move north
		}
		sWalkAroundMapDictionary[currentMapKey] = new Tuple<int, int>(current_x + x_offset, current_y + y_offset);
	}

	//set variable so when loading new map knows which map to load based on currentMapKey and direction leaving map from
	//this is set when leaving a map, sWalkAroundMapDirection is used when loading a new map
	//public void SetMoveMapDirection(PlayerUnit pu, Point max, Point min)
	//{
	//	//0 N, 1 E, 2 S, 3 W
	//	//for now based on edge of map
	//	if (pu.TileX == min.x)
	//		sWalkAroundMapDirection = 3;
	//	else if (pu.TileY == min.y)
	//		sWalkAroundMapDirection = 2;
	//	else if (pu.TileX == max.x)
	//		sWalkAroundMapDirection = 1;
	//	else
	//		sWalkAroundMapDirection = 0;
	//}

	//pring contents of mapDict for debugging purposes
	public void PrintMapDict()
	{
		Debug.Log("MapDict contents are:");
		foreach (KeyValuePair<Tuple<int, int>, Tuple<int, int>> kvp in sWalkAroundMapDictionary)
		{
			Debug.Log("Key " + kvp.Key + ", Value " + kvp.Value);
		}
	}

	public Tuple<int, int> GetMapXY()
	{
		var currentMapKey = new Tuple<int, int>(NameAll.MAP_DICT_CURRENT_MAP, NameAll.MAP_DICT_CURRENT_MAP);
		return sWalkAroundMapDictionary[currentMapKey];
	}

	public bool IsFirstMapVisit(Tuple<int, int> mapXY)
	{
		if (sWalkAroundMapDictionary.ContainsKey(mapXY))
		{
			return false;
		}
		return true;
	}

	//DEFUNCT when moving to a new map, use current map and directon to get which new map is being moved to
	//public int GetLoadMapId()
	//{
	//	var currentMapKey = new Tuple<int, int>(NameAll.MAP_DICT_CURRENT_MAP, NameAll.MAP_DICT_CURRENT_MAP);
	//	int currentMapInt = sWalkAroundMapDictionary[currentMapKey];
	//	//Debug.Log("Testing map loading stuff 0, currentMapInt is " + currentMapInt);
	//	//PrintMapDict();

	//	if (sWalkAroundMapDirection == NO_MAP_DIRECTION)
	//	{
	//		//Debug.Log("Testing map loading stuff 1: no direction set, loading current map");
	//		return currentMapInt;
	//	}

	//	var mapKey = new Tuple<int, int>(currentMapInt, sWalkAroundMapDirection);
	//	if (sWalkAroundMapDictionary.ContainsKey(mapKey)) //mapId and direction leads to existing map
	//	{
	//		//Debug.Log("Testing map loading stuff 2: loading already created map for map Id " + sWalkAroundMapDictionary[mapKey] + " dir: " + sWalkAroundMapDirection);
	//		//before returning id of map to be loaded, set currentMapKey to mapId of map to be loaded (because once the map is loaded it becomes the current map)
	//		sWalkAroundMapDictionary[currentMapKey] = sWalkAroundMapDictionary[mapKey];
	//		return sWalkAroundMapDictionary[mapKey];
	//	}
	//	else //new map needs to be created
	//	{
	//		int newMapId = GetNextMapInt(); //get new map id and set currentMap to this newMapId
	//		sWalkAroundMapDictionary[mapKey] = newMapId; //link old map and direction to new map
	//													 //also need to create return path
	//		var reverseKey = new Tuple<int, int>(newMapId, (sWalkAroundMapDirection + 2) % 4);
	//		sWalkAroundMapDictionary[reverseKey] = currentMapInt; //link new map with opposite direction to return to old map
	//		//Debug.Log("Testing map loading stuff 3: testing new map creation for map Id " + sWalkAroundMapDictionary[mapKey] + " dir: " + sWalkAroundMapDirection);
	//		//PrintMapDict();
	//		return newMapId;
	//	}
	//}

	//for saving the Map dictionary
	public Dictionary<Tuple<int, int>, Tuple<int, int>> GetMapDictionary()
	{
		return sWalkAroundMapDictionary;
	}

	public void SetMapDictionary(Dictionary<Tuple<int, int>, Tuple<int, int>> mapDict) //for loading the map dictionar
	{
		sWalkAroundMapDictionary = mapDict;
	}

	//for now just storing in the dictionary if that map has been visited with a 1
	//useful for checking whether to generate enemey units from scratch
	//in future can be used for other things
	public void AddXYToMapDict(int map_x, int map_y)
	{
		var mapKey = new Tuple<int, int>(map_x, map_y);
		sWalkAroundMapDictionary[mapKey] = new Tuple<int, int>(1, 1);
	}
	public void AddXYToMapDict(Tuple<int, int> map_xy)
	{
		sWalkAroundMapDictionary[map_xy] = new Tuple<int, int>(1, 1);
	}

	//save playerUnits for a WA map
	public void SaveWalkAroundPlayerUnits(string puType)
	{
		List<PlayerUnit> puList = new List<PlayerUnit>();
		if (puType == NameAll.WA_UNIT_SAVE_MAP_LIST)
		{
			puList = GetMapPlayerUnits();
		}
		else if (puType == NameAll.WA_UNIT_SAVE_PLAYER_LIST)
		{
			puList = GetAllUnitsByTeamId(NameAll.TEAM_ID_WALK_AROUND_GREEN);
		}

		if (puList.Count > 0)
		{
			int seed = GetWalkAroundSeed();
			int timeInt = GetWalkAroundTimeInt();
			var mapXY = PlayerManager.Instance.GetMapXY();
			CalcCode.SaveWalkAroundPlayerUnitList(puList, seed, timeInt, mapXY.Item1, mapXY.Item2, puType);
		}

	}

	//get walk around maps units: not in players's party, not dead or crystalized
	//temporary for now
	List<PlayerUnit> GetMapPlayerUnits()
	{
		List<PlayerUnit> retList = new List<PlayerUnit>();
		foreach (PlayerUnit p in sPlayerUnitList)
		{
			if (p.TeamId != NameAll.TEAM_ID_WALK_AROUND_GREEN && !StatusManager.Instance.IfStatusByUnitAndId(p.TurnOrder, NameAll.STATUS_ID_DEAD)
				&& !StatusManager.Instance.IfStatusByUnitAndId(p.TurnOrder, NameAll.STATUS_ID_PETRIFY)
				&& !StatusManager.Instance.IfStatusByUnitAndId(p.TurnOrder, NameAll.STATUS_ID_CRYSTAL))
			{
				retList.Add(p);
			}
		}
		return retList;
	}
	#endregion

	#region RL Gridworld

	//returns tile index of gridworld agent
	public int GetGridworldAgentIndex(int maxX)
	{
		PlayerUnit pu = GetPlayerUnit(0);
		return pu.TileX + pu.TileY * (maxX + 1);
	}

	public int MoveGridworldAgent(int action, Board board)
	{
		PlayerUnit pu = GetPlayerUnit(0);
		int puX = pu.TileX;
		int puY = pu.TileY;
		int minX = 0;
		int minY = 0;
		int maxX = board.max.x;
		int maxY = board.max.y;

		//W is 1 (up), A is 2 (left), s is 3 (down), D is 4 (right)
		if (action == 1)
		{
			if (puY + 1 > maxY) //can't move further up
				return GetGridworldAgentIndex(maxX);
			pu.TileY += 1;
		}
		else if (action == 2)
		{
			if (puX - 1 < minX) //can't move further left
				return GetGridworldAgentIndex(maxX);
			pu.TileX -= 1;
		}
		else if (action == 3)
		{
			if (puY - 1 < minY) //can't move further down
				return GetGridworldAgentIndex(maxX);
			pu.TileY -= 1;
		}
		else if (action == 4)
		{
			if (puX + 1 > maxX) //can't move further right
				return GetGridworldAgentIndex(maxX);
			pu.TileX += 1;
		}
		Tile t = board.GetTile(pu.TileX, pu.TileY);
		PlayerUnitObject puo = GetPlayerUnitObjectComponent(0);
		puo.MoveGridworld(t);
		return GetGridworldAgentIndex(maxX);
	}

	//end up just resetting the crystal, do that in board
	//void ResetGridworld(Board board)
	//{

	//	//move crystal
	//	int x = UnityEngine.Random.Range(0, board.max.x);
	//	int y = UnityEngine.Random.Range(0, board.max.y);
	//	while (board.IsCrystalOnTile(x,y))
	//	{
	//		x = UnityEngine.Random.Range(0, board.max.x);
	//		y = UnityEngine.Random.Range(0, board.max.y);
	//	}
	//	board.MoveCrystal(x, y);

	//	//only moving crystal, not the player

	//	board.SetTilePickUp(x, y, true);
	//	//replace unit (for now just first in PU list
	//	//initialize the physical representation of the player unit
	//	PlayerUnit pu = GetPlayerUnit(0);
	//	PlayerUnitObject puo = GetPlayerUnitObjectComponent(0);
	//	board.DisableUnit(pu); //sets board where PU used to be back to 0

	//	//randomize unit starting spot
	//	while (true)
	//	{
	//		x = UnityEngine.Random.Range(0, board.max.x);
	//		y = UnityEngine.Random.Range(0, board.max.x);
	//		//check that starting spot is empty
	//		if (!board.IsCrystalOnTile(x, y))
	//		{
	//			//tile is empty, place unit
	//			Tile startTile = board.GetTile(x, y);
	//			pu.SetUnitTile(startTile, true);
	//			//tells the tiles that someone is on them
	//			board.GetTile(pu).UnitId = pu.TurnOrder;
	//			Vector3 vecTemp = startTile.transform.position;
	//			vecTemp.y = vecTemp.y * 2.0f;
	//			puo.transform.position = vecTemp;
	//			MapTileManager.Instance.MoveMarker(pu.TurnOrder, startTile);
	//			break;
	//		}
	//	}

	//}
	#endregion

	#region RL Duel

	//get obs from the two players in the PU list
	public float[] GetDuelObservation()
	{
		//teamId, x, y, currentHP, maxHP, PA, MA, Speed, MP, brave, CT
		//divide to scale values in 0 to 1 range
		float[] retValue = new float[23];
		retValue[0] = 0f; //placeholder
		int zIndex = 1;
		foreach(PlayerUnit pu in sPlayerUnitList)
		{
			retValue[zIndex] = pu.TeamId / 10f;
			zIndex += 1;
			retValue[zIndex] = pu.TileX / 10f;
			zIndex += 1;
			retValue[zIndex] = pu.TileY / 10f;
			zIndex += 1;
			retValue[zIndex] = pu.StatTotalLife / 100f; //should be 1000 if actual equipment
			zIndex += 1;
			retValue[zIndex] = pu.StatTotalMaxLife / 100f; //should be 1000 if actual equipment
			zIndex += 1;
			retValue[zIndex] = pu.StatTotalPA / 10f; //should be 100 if actual equipment
			zIndex += 1;
			retValue[zIndex] = pu.StatTotalMA / 10f; //should be 100 if actual equipment
			zIndex += 1;
			retValue[zIndex] = pu.StatTotalSpeed / 20f; //20 is fine, capped that way in AT
			zIndex += 1;
			retValue[zIndex] = pu.StatTotalMP / 100f; //should be 1000 if actual equipment
			zIndex += 1;
			retValue[zIndex] = pu.StatTotalBrave / 100f;
			zIndex += 1;
			retValue[zIndex] = pu.CT / 100f;
			zIndex += 1;
		}

		return retValue;
	}

	//called from DuelRLAgent, checks if the action plan is valid. bunch of shortcuts here, rewrite this if using again
	public bool IsRLPlanValid(float[] actionArray, int teamId, Board board, int game_state)
	{
		int game_state_all = 0; 
		int game_state_move = 1; //can only move or wait
		int game_state_act = 2; //can only act or wait
		int game_state_wait = 3; //can only wait
								 //Debug.Log("testing moving to tile, isRLPLanValid top " + actionArray[0] + actionArray[1] + actionArray[2]);
								 //3 branches of actions
								 //index 0: wait 0, move 1, attack 2, primary 3
								 //index 1: which primary abilities (8 total)
								 //index 2: target tile (0 is 0,0; 1 is 1,0; 2 is 0,1; 3 is 1,1) or wait direction (the direction enums)

		//Debug.Log("START unit locations on each tile " );
		//Tile t1 = board.GetTile(new Point(0, 0));
		//Debug.Log("testing moving to tile, checking unit id on tile " + t1.UnitId + " " + 0 + " " + 0 );
		//t1 = board.GetTile(new Point(1, 0));
		//Debug.Log("testing moving to tile, checking unit id on tile " + t1.UnitId + " " + 1 + " " + 0);
		//t1 = board.GetTile(new Point(0, 1));
		//Debug.Log("testing moving to tile, checking unit id on tile " + t1.UnitId + " " + 0 + " " + 1);
		//t1 = board.GetTile(new Point(1, 1));
		//Debug.Log("testing moving to tile, checking unit id on tile " + t1.UnitId + " " + 1 + " " + 1);

		int index0 = (int)actionArray[0];
		if (index0 == 0)
		{ //wait
			return true;
		}
		else
		{
			//check can only move/act if valid
			if (game_state == game_state_wait)
				return false;
			else if (game_state == game_state_act && index0 == 1) //can only act but selected move
				return false;
			else if (game_state == game_state_move && index0 == 2) //can only move but selected act
				return false;

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

			Tile t = board.GetTile(p);
			if (index0 == 1 && (game_state == game_state_all || game_state == game_state_move)) //move. check if tile is occupied
			{
				if (t.UnitId == NameAll.NULL_UNIT_ID)
					return true;
			}
			else if(game_state == game_state_all || game_state == game_state_act) //attack / primary
			{
				//get acting unit's position. target tile has to be within one square
				int pu_x;
				int pu_y;
				if (teamId == NameAll.TEAM_ID_GREEN)
				{
					pu_x = this.GetPlayerUnit(0).TileX;
					pu_y = this.GetPlayerUnit(0).TileY;
				}
				else
				{
					pu_x = this.GetPlayerUnit(1).TileX;
					pu_y = this.GetPlayerUnit(1).TileY;
				}
				//Debug.Log("valid action 0, team id is " + teamId + " target tile is " + t.pos.x + t.pos.y + " actor tile is " + pu_x + pu_y);
				int target_x = t.pos.x;
				int target_y = t.pos.y;
				if( Math.Abs(pu_x-target_x) + Math.Abs(pu_y-target_y) == 1)
				{
					//Debug.Log("valid action 1, team id is " + teamId + " target tile is " + t.pos.x + t.pos.y + " actor tile is " + pu_x + pu_y);
					return true;
				}
			}
		}

		return false;
	}

	#endregion

	#region general notifications
	const string CombatStart = "PlayerManager.CombatStart";
    const string CombatEnd = "PlayerManager.CombatEnd";
    const string TeamDefeated = "PlayerManager.TeamDefeated";
    const string TurnInQueue = "PlayerManager.TurnInQueue";

    //listener is in WalkAroundMainState, if triggered lets WalkAround know that combat is starting
    //sent from ???
    public void SendCombatStart()
    {
        this.PostNotification(CombatStart, null);
    }

    //listener in WalkAroundMainState, if triggered lets WalkAround know that combat is over
    void SendCombatEnd()
    {
        this.PostNotification(CombatEnd);
    }

    void SendTeamDefeated(int teamId)
    {
        this.PostNotification(TeamDefeated, teamId);
    }

    //unit has turnInQueue, sent for display purposes
    void SendTurnInQueue(int unitId)
    {
        this.PostNotification(TurnInQueue, unitId);
    }
	#endregion

	#region Handle persistent stuff when going from one map to another in WA mode
	//get saved units
	public List<PlayerUnit> GetPersistentUnits()
	{
		var retList = new List<PlayerUnit>();

		
		return retList;
	}

	#endregion

	#region CombatLog
	/// <summary>
	/// Handles all true/false rolls. Done here for easier logging if logging is enabled.
	/// </summary>
	/// <param name="rollChance"></param>
	/// <param name="rollLow">Min is inclusive</param>
	/// <param name="rollSpread">Max is exclusive</param>
	/// <param name="logType"></param>
	/// <param name="logSubType"></param>
	/// <param name="sName"></param>
	/// <param name="puActor"></param>
	/// <param name="puTarget"></param>
	/// <param name="effectValue"></param>
	/// <param name="statusValue"></param>
	/// <returns>bool: true if roll result <= rollChance </returns>
	public bool IsRollSuccess(int rollChance, int rollLow = 1, int rollSpread = 100, int logType = -1919, int logSubType = -1919, 
		SpellName sName = null, PlayerUnit puActor = null, PlayerUnit puTarget = null, int effectValue = -1919, int statusValue = -1919)
	{
		bool retValue;
		int rollResult = GetRollResult(rollChance, rollLow, rollSpread, logType, logSubType, sName, puActor, puTarget, effectValue, statusValue);
		if ( rollResult <= rollChance) //min is inclusive, max is exclusive
		{
			retValue = true;
		}
		else
		{
			retValue = false;
		}

		return retValue;
	}


	/// <summary>
	/// Returns result of a roll with a number. Some functions prefer the number rather than the actual true/false result
	/// </summary>
	/// <param name="rollChance"></param>
	/// <param name="rollLow"></param>
	/// <param name="rollSpread"></param>
	/// <param name="logType"></param>
	/// <param name="logSubType"></param>
	/// <param name="sName"></param>
	/// <param name="puActor"></param>
	/// <param name="puTarget"></param>
	/// <param name="effectValue"></param>
	/// <param name="statusValue"></param>
	/// <returns></returns>
	public int GetRollResult(int rollChance, int rollLow = 1, int rollSpread = 100, int logType = -1919, int logSubType = -1919,
		SpellName sName = null, PlayerUnit puActor = null, PlayerUnit puTarget = null, int effectValue = -1919, int statusValue = -1919)
	{
		int rollHigh = rollLow + rollSpread;
		int rollResult = UnityEngine.Random.Range(rollLow, rollHigh);

		if (isSaveCombatLog)
		{
			if (logType == NameAll.NULL_INT)
				logType = NameAll.COMBAT_LOG_TYPE_ROLL;
			CombatTurn logTurn = new CombatTurn();
			logTurn.spellName = sName;
			logTurn.actor = puActor;
			if (puTarget == null)
				logTurn.targetUnitId = NameAll.NULL_UNIT_ID;
			AddCombatLogSaveObject(logType, logSubType, logTurn, rollResult, rollChance, effectValue, statusValue);
		}

		return rollResult;
	}



	//int logId;
	//int tick;
	//int logType;
	//int logSubType
	//int turnsNumber;
	//int rollResult;
	//int rollChance;
	//int spellNameId;
	//int actorId;
	//int targetId
	//int targetTileX;
	//int targetTileY;
	//int targetTileZ;

	public void AddCombatLogSaveObject(int logType, int logSubType=-1919, CombatTurn cTurn = null, int rollResult=-1919, int rollChance=-1919, int effectValue=-1919, int statusValue=-1919)
	{
		CombatLogSaveObject clso = null;
		if (isSaveCombatLog)
		{
			clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
			StoreCombatLogSaveObject(clso);
			combatLogId += 1;
			/*
			//rest of this function is so I have notes on what all the shit means when it comes time to decode the combatLog
			
			if (logType == NameAll.COMBAT_LOG_TYPE_END_TURN)
			{
				//actorID is unit who ended turn (stored through turn), effectValue is actor's new CT after ending turn
				clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
			}
			else if (logType == NameAll.COMBAT_LOG_TYPE_MOVE)
			{
				if(logSubType == NameAll.NULL_INT)
				{
					//default move type, override of SetUnitTile. stores unit moving and destination in logTurn
					clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
				}
				else if(logSubType == NameAll.COMBAT_LOG_SUBTYPE_MOVE_TELEPORT_ROLL)
				{
					//if roll > rollChance, succeeds if not fails. rest of values null
					clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
				}
				else if (logSubType == NameAll.COMBAT_LOG_SUBTYPE_MOVE_EFFECT)
				{
					//turn has the actorId of the unit doing the on move effect ability and effectValue is the movement
					//called after the movement reaction is created in GameLoopState or CombatMoveSequenceState
					clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
				}
				else if(logSubType == NameAll.COMBAT_LOG_SUBTYPE_MOVE_SWAP)
				{
					//for swap, a unit swaps places with another unit with the other unit going to the swapper's start position
					//the swapper has already been updated in setunittile, this updates the swapee
					//actor in turn is the swappee, move tile is the original tile the swapper started at
					clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
				}
			}
			else if( logType == NameAll.COMBAT_LOG_TYPE_ACTION)
			{
				if(logType == NameAll.COMBAT_LOG_SUBTYPE_SET_HP)
				{
					//ability causes HP healing/dmg. actorId is the caster, targetId is the unit that took the hp change. effectValue is final HP, status is how much HP changed
					clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
				}
				else if (logType == NameAll.COMBAT_LOG_SUBTYPE_SET_HP_REMOVE_ALL)
				{
					//ability causes unit HP to go to 0. actorId is the caster, targetId is the unit that took the hp loss. effectValue is hp (in this case 0)
					clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
				}
			}
			else if (logType == NameAll.COMBAT_LOG_TYPE_REACTION)
			{

			}
			else if (logType == NameAll.COMBAT_LOG_TYPE_SLOW_ACTION)
			{
				if( logSubType == NameAll.COMBAT_LOG_SUBTYPE_SLOW_ACTION_UNABLE_TO_CAST)
				{
					//spell unable to resolve due to some status ailment. handled in own function before being rerouted to this one to create the CLSO
					clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
				}
			}
			else if(logType == NameAll.COMBAT_LOG_TYPE_STATUS_MANAGER)
			{
				//for now just removing status from status manager. logType, subLogType, and statusValue for status removed
				clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
			}
			else if (logType == NameAll.COMBAT_LOG_TYPE_MISC)
			{
				//AddCombatLogSaveObject(COMBAT_LOG_TYPE_MISC, NameAll.COMBAT_LOG_SUBTYPE_KNOCKBACK, cTurn: logTurn, effectValue: fallDamage, rollResult: rollResult, rollChance: rollChance);
				if(logSubType == NameAll.COMBAT_LOG_SUBTYPE_KNOCKBACK_MOVE)
				{
					//knockback eligible and roll passed (roll logged elsewhere). here storing tile went to through turn and the falldamage in effectValue
					clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
				}
				else if (logSubType == NameAll.COMBAT_LOG_SUBTYPE_KNOCKBACK_DAMAGE)
				{
					//shouldn't reach here, handled through AlterUnitStat. Basic AlterUnitState except unique subtype
					clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
				}
			}
			else if(logType == NameAll.COMBAT_LOG_TYPE_ALTER_STAT_ADD || logType == NameAll.COMBAT_LOG_TYPE_ALTER_STAT_REMOVE)
			{
				//statusValue is the stat that is altered. effect value is how much stat is altered.
				//actor is the caster of the ability, target is the unit having the stat altered
				//logSubType can be for COMBAT_LOG_SUBTYPE_END_TURN_TICK_REGEN or COMBAT_LOG_SUBTYPE_END_TURN_TICK_POISON or null
				clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
			}
			else if(logType == NameALl.COMBAT_LOG_TYPE_ROLL){
				clso = new CombatLogSaveObject(combatLogId, currentTick, logType, logSubType, effectValue, statusValue, rollResult, rollChance, cTurn);
				//COMBAT_LOG_SUBTYPE_STATUS_ADD_ROLL
				//some abilities have a chance to add a status. this is a roll for this. statusValue is the status to add
				//COMBAT_LOG_SUBTYPE_ITEM_ON_HIT_CHANCE
				//attacks from some items have a chance to have an on hit effect. this is the roll for this. EffectValue is the items effect. can later be dodged/miss I think
				//COMBAT_LOG_SUBTYPE_KATANA_BREAK
				//15% chance the katana breaks
				//COMBAT_LOG_SUBTYPE_REACTION_BRAVE_ROLL
				//roll to see if reaction goes off
				//COMBAT_LOG_SUBTYPE_UNDEAD_REVIVE_ROLL: 50% chance undead revive when counter reaches 0. effect value is unit id
				//COMBAT_LOG_SUBTYPE_CRYSTAL_ROLL: 50% chance to turn into crystal. effect value is unit id
			}

			if (clso != null)
			{
				StoreCombatLogSaveObject(clso);
				combatLogId += 1;
			}
			else
			{
				Debug.Log("Possible error, CombatLogSaveObject not added " + logType + " " + logSubType);
			}
			*/
		}
	}

	/// <summary>
	/// Handles adding spellslow objects to combat log. For now just ones that cannot be resolved.
	/// </summary>
	/// <param name="logType"></param>
	/// <param name="logSubType"></param>
	/// <param name="ss"></param>
	public void AddCombatLogSaveObject(int logType, int logSubType, SpellSlow ss)
	{
		if (isSaveCombatLog)
		{
			CombatTurn logTurn = new CombatTurn();
			logTurn.spellName = SpellManager.Instance.GetSpellNameByIndex(ss.SpellIndex);
			logTurn.actor = GetPlayerUnit(ss.UnitId);
			logTurn.targetUnitId = ss.TargetUnitId;
			AddCombatLogSaveObject(logType, logSubType, logTurn);
		}
	}

	//collect CombatLogSaveObjects. Periodically write to file
	public void StoreCombatLogSaveObject(CombatLogSaveObject clso)
	{
		sCombatLogList.Add(clso);
		if(sCombatLogList.Count > COMBAT_LOG_SAVE_EVERY)
		{
			int combatLogSaveCount = combatLogId / COMBAT_LOG_SAVE_EVERY;
			string fileName = Application.dataPath + "/CombatLevels/combat_log_save_" + timeInt + "_"+ combatLogSaveCount + ".dat";
			Serializer.Save<List<CombatLogSaveObject>>(fileName, sCombatLogList);
			sCombatLogList = new List<CombatLogSaveObject>();
		}
	}
	#endregion
}

