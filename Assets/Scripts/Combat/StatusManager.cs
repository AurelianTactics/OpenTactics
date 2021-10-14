using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StatusManager : Singleton<StatusManager>
{

    private static List<StatusObject> sStatusTickList; //tickable and curable statuses
    private static List<StatusObject> sStatusLastingList; //statuses given items/ability/innate
    //private static List<StatusObject> sStatusTickTemp; //temp list, used for showing expiring statuses

    private int greenGolemHP;
    private int redGolemHP;
    private int deathMode;

	const int COMBAT_LOG_TYPE_STATUS_MANAGER = 7;
	const int COMBAT_LOG_STATUS_REMOVE = 0;
	//const int COMBAT_LOG_STATUS_ADD = 1;

	void Awake()
    {
        //photonView = PhotonView.Get(this);
    }

    protected StatusManager()
    { // guarantee this will be always a singleton only - can't use the constructor!
        //myGlobalVar = "asdf";
        //sPlayerUnitList = new List<PlayerUnit>();
        //sPlayerObjectList = new List<GameObject>();

        sStatusTickList = new List<StatusObject>();
        sStatusLastingList = new List<StatusObject>();
        //sStatusTickTemp = new List<StatusObject>();
        //StatusName sn = new StatusName();
        //sStatusNameList = sn.GetAllStatusTypes();
        greenGolemHP = 0;
        redGolemHP = 0;
        deathMode = NameAll.DEATH_MODE_DEATH;
    }

    //called in WA mode when switching maps. could keep some statuses at some point
    public void ClearLists()
    {
        sStatusTickList = new List<StatusObject>();
        sStatusLastingList = new List<StatusObject>();
        //sStatusTickTemp = new List<StatusObject>();
        //StatusName sn = new StatusName();
        //sStatusNameList = sn.GetAllStatusTypes();
    }

    //decreases all statuses by 1
    //for statuses that reach 0, Removes them
    //removes them from the playerunitobject status list too
    //assumes that when they hit 0 they are gone
    //[PunRPC]
    public void StatusCheckPhase()
    {
        foreach (StatusObject s in sStatusTickList.ToList()) //believe this allows iterate while removing
        {
            //Debug.Log("about to decrement status, " + s.GetStatusId() + " " + s.GetTicksLeft());
            if (s.GetTickType() == 1) //death sentence and dead have special ticks but because .getticktype == 0 they aren't decremented here
            {
                s.SubtractTick();
                if (s.GetTicksLeft() <= 0)
                {
                    PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(),s.GetStatusId());
                    sStatusTickList.Remove(s); //Debug.Log("removing status from tick list " + s.GetStatusId() );
					if( PlayerManager.Instance.GetRenderMode() != NameAll.PP_RENDER_NONE)
					{
						CombatLogClass cll = new CombatLogClass("status " + NameAll.GetStatusString(s.GetStatusId()) + " ends", s.GetUnitId(), PlayerManager.Instance.GetRenderMode());
						cll.SendNotification();
					}

					PlayerManager.Instance.AddCombatLogSaveObject(COMBAT_LOG_TYPE_STATUS_MANAGER, NameAll.COMBAT_LOG_SUBTYPE_STATUS_REMOVE, statusValue:s.GetStatusId());
					//sStatusTickTemp.Add(s);
				}
            }
            //Debug.Log("decremented status id, " + s.GetStatusId() + " " + s.GetTicksLeft());
        }

        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("StatusCheckPhase", PhotonTargets.Others);
        //}      
    }
    
    

    //can streamline
    //called in ActiveTurnState, returns true if unit is dead and should skip turn
    public bool CheckStatusAtBeginningOfTurn(int unitId) //death sentence and dead, decrements the getTicksleft, when it reached 0 special things happen
    {
        bool retValue = false;
        foreach (StatusObject s in sStatusTickList.ToList()) 
        {
            if( s.GetUnitId() == unitId)
            {
                if (s.GetStatusId() == NameAll.STATUS_ID_DEAD)
                {
                    //Debug.Log("a unit is dead, doing the work inside to check statuses and alter the overhead messages");
                    
                    //removing the status indicator image that indicates number of turns left until perma death
                    int z2 = GetStatusIdFromTicks(s.GetStatusId(), s.GetTicksLeft());
                    PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), z2 );
                    //if (!PhotonNetwork.offlineMode)
                    //    PlayerManager.Instance.SendMPStatusList(isAdd: false, unitId: unitId, statusId: z2);

                    if ( IfStatusByUnitAndId(unitId,NameAll.STATUS_ID_RERAISE,true))
                    {
                        PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), NameAll.STATUS_ID_RERAISE);
                        RemoveStatusTickByUnit(unitId, NameAll.STATUS_ID_RERAISE); //Debug.Log("unit is dead, about to get hit by reraise");
                        PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(unitId);
                        int z1 = (pu.StatTotalMaxLife + 9) / 10;
                        AddStatusAndOverrideOthers(z1, unitId, NameAll.STATUS_ID_LIFE);
                        return false;
                    }

                    s.SubtractTick();
                    if (s.GetTicksLeft() <= -1)
                    {
                        if(deathMode == NameAll.DEATH_MODE_UNCONSCIOUS)
                        {
                            AddStatusAndOverrideOthers(0, unitId, NameAll.STATUS_ID_UNCONSCIOUS);
                        }
                        else
                        {
                            Debug.Log("Add crystalize status");
                            AddStatusAndOverrideOthers(0, unitId, NameAll.STATUS_ID_CRYSTAL);
                        }
                    }
                    else
                    {
                        //adding the status indicator image that indicates number of turns left until perma death
                        int z3 = GetStatusIdFromTicks(s.GetStatusId(), s.GetTicksLeft());
                        PlayerManager.Instance.AddToStatusList(s.GetUnitId(),  z3 );
                        //if (!PhotonNetwork.offlineMode)
                        //    PlayerManager.Instance.SendMPStatusList(isAdd: true, unitId: unitId, statusId: z3);
                    }
                    return true;
                }
                else if (s.GetStatusId() == NameAll.STATUS_ID_DEATH_SENTENCE)
                {
                    //removing the status indicator image that indicates number of turns until death sentence hits
                    int z2 = GetStatusIdFromTicks(s.GetStatusId(), s.GetTicksLeft());
                    PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), z2);
                    //if (!PhotonNetwork.offlineMode)
                    //    PlayerManager.Instance.SendMPStatusList(isAdd: false, unitId: unitId, statusId: z2);

                    s.SubtractTick();
                    if (s.GetTicksLeft() <= -1)
                    {
                        //removes the base death_sentence
                        RemoveStatusTickByUnit(unitId, s.GetStatusId());
                        //sStatusTickList.Remove(s);
                        //PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), s.GetStatusId()); 

                        AddStatusAndOverrideOthers(0, unitId, NameAll.STATUS_ID_DEAD);
                        return true;
                    }
                    else
                    {
                        //adding the status indicator image that indicates number of turns left until perma death
                        int z3 = GetStatusIdFromTicks(s.GetStatusId(), s.GetTicksLeft());
                        PlayerManager.Instance.AddToStatusList(s.GetUnitId(),  z3 );
                        //if (!PhotonNetwork.offlineMode)
                        //    PlayerManager.Instance.SendMPStatusList(isAdd: true, unitId: unitId, statusId: z3);
                    }
                }
                else if (s.GetStatusId() == NameAll.STATUS_ID_CHICKEN)
                {
                    //Debug.Log("adding brave for chicken");
                    PlayerManager.Instance.AlterUnitStat(0, 1, NameAll.STAT_TYPE_BRAVE, unitId);
                }
                else if (s.GetStatusId() == NameAll.STATUS_ID_DEFENDING)//removed at beginning of turn
                {
                    RemoveStatusTickByUnit(unitId, s.GetStatusId());
                    //sStatusTickList.Remove(s);
                    //PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), s.GetStatusId());
                }
            }  
        }
        return retValue;
    }

    //called in check status at beginning of turn. used to get dead_3, dead_2 etc
    int GetStatusIdFromTicks(int statusId, int ticksLeft)
    {
        int z1 = 0;
        if (statusId == NameAll.STATUS_ID_DEAD)
        {
            if (ticksLeft == 0)
            {
                z1 = NameAll.STATUS_ID_DEAD_0;
            }
            else if (ticksLeft == 1)
            {
                z1 = NameAll.STATUS_ID_DEAD_1;
            }
            else if (ticksLeft == 2)
            {
                z1 = NameAll.STATUS_ID_DEAD_2;
            }
            else
            {
                z1 = NameAll.STATUS_ID_DEAD_3;
            }
        }
        else
        {
            if (ticksLeft == 0)
            {
                z1 = NameAll.STATUS_ID_DEATH_SENTENCE_0;
            }
            else if (ticksLeft == 1)
            {
                z1 = NameAll.STATUS_ID_DEATH_SENTENCE_1;
            }
            else if (ticksLeft == 2)
            {
                z1 = NameAll.STATUS_ID_DEATH_SENTENCE_2;
            }
            else
            {
                z1 = NameAll.STATUS_ID_DEATH_SENTENCE_3;
            }
        }
        return z1;
    }

    public void CheckStatusAtEndOfTurn(int unitId) //called in game loop at end of player turn, No PunRPC (dmg handled in playermanager)
    {
        //Debug.Log("in loop phase 47, heading to 50 a");
        if ( IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_JUMPING, true) )
        {
            return;
        }
        //Debug.Log("in loop phase 47, heading to 50 a");
        foreach (StatusObject s in sStatusTickList.ToList())
        {
            if (s.GetUnitId() == unitId)
            {
                //Debug.Log("in loop phase 47, heading to 50 b");
                if (s.GetStatusId() == NameAll.STATUS_ID_POISON)
                {
                    PlayerManager.Instance.EndOfTurnTick(unitId, 0); //0 for poison, 1 for regen
                }
                else if (s.GetStatusId() == NameAll.STATUS_ID_REGEN)
                {
                    PlayerManager.Instance.EndOfTurnTick(unitId, 1); //0 for poison, 1 for regen 
                    //Debug.Log("in loop phase 47, heading to 50 c");
                }
            }

        }
    }

    //on death etc
    //online: additional code due to fucntion being called in PlayerUnit by master and other. if being called in PlayerUnit, only want Other to respond to the RPC call
    //online: could just have other do it by its internal call (and not send the RPC but want to be consistent
    //[PunRPC]
    public void RemoveStatusTickByUnit(int unit_id, int statusId, bool isBeingCalledFromPlayerUnit = false)
    {
        //if (!PhotonNetwork.offlineMode )
        //{
        //    if( PhotonNetwork.isMasterClient)
        //        photonView.RPC("RemoveStatusTickByUnit", PhotonTargets.Others, new object[] { unit_id, statusId, false });
        //    else
        //    {
        //        //Other only responds to the RPC call, not its internal call for this. can argue that it should do the opposite
        //        if (isBeingCalledFromPlayerUnit)
        //            return;
        //    }
        //}
        
        if (statusId == NameAll.STATUS_ID_ALL)
        {
            foreach (StatusObject s in sStatusTickList.ToList())
            {
                if (s.GetUnitId() == unit_id)
                {
                    PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), s.GetStatusId());
                    sStatusTickList.Remove(s);
                }
            }
        }
        else if (statusId == NameAll.STATUS_ID_DEAD)
        {
            foreach (StatusObject s in sStatusTickList.ToList())
            {
                if (s.GetUnitId() == unit_id && s.GetStatusId() != NameAll.STATUS_ID_RERAISE && s.GetStatusId() != NameAll.STATUS_ID_UNDEAD )
                {
                    PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), s.GetStatusId());
                    sStatusTickList.Remove(s);
                }
            }
        }
        else {
            foreach (StatusObject s in sStatusTickList.ToList())
            {
                if (s.GetUnitId() == unit_id && s.GetStatusId() == statusId)
                {
                    PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), s.GetStatusId());
                    sStatusTickList.Remove(s);
                    break;
                }
            }
        }
    }

   
    //item destroyed, equip change etc
    //[PunRPC]
    public void RemoveStatusLastingByUnit(int unit_id, int item_id)
    {
        if (item_id == 0)
        {
            return;
        }
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("RemoveStatusLastingByUnit", PhotonTargets.Others, new object[] { unit_id, item_id });
        //}
        
        foreach (StatusObject s in sStatusTickList.ToList())
        {
            if (s.GetUnitId() == unit_id && s.GetItemId() == item_id)
            {
                PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), s.GetStatusId()); //will remove some blocked statuses but won't really matter
                sStatusLastingList.Remove(s);
            }
        }
    }

    //LASTING STATUSES FOR PLAYERUNITOBJECTS CREATED IN STATUS OBJECT CREATOR
    //pun not needed, each side adds the lasting statuses individually at the beginning of the map

    //called from equip in PlayerUnit, used in displaying stats from charactercreate
    //full list should be generated at start of match
    public void AddStatusLastingById(int unitId, int itemId, int slot)
    {
        StatusObjectCreator.CreateStatusObjectListFromItem(unitId, itemId, slot);
    }

    //create statusObject in static StatusObjectCreator class (bottom of StatusObject file
    public void AddStatusLastingByStatusObject(StatusObject so)
    {
        sStatusLastingList.Add(so);
        //Debug.Log("testing lasting so " + so.GetUnitId() + " " + so.GetItemId());
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("RPC", PhotonTargets.Others, new object[] { so.GetUnitId(), so.GetItemId(), so.GetStatusId(), so.GetItemSlot() });
        //}
    }

    //used to add float_move in PlayerUnit, might be some other ones. these are perma statuses
    public void AddStatusLastingByString(int unitId, int statusId)
    {
        StatusObject so = new StatusObject(unitId, statusId);
        sStatusLastingList.Add(so);
        PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId()); //Debug.Log("adding status lasting");
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("AddStatusLastingByStringRPC", PhotonTargets.Others, new object[] { unitId, statusName });
        //}
    }

    //generate all the status lasting
    //should only be done once, sometime after the playerunit list is created
    public void GenerateAllStatusLasting()
    {
        List<PlayerUnit> tempList = PlayerManager.Instance.GetPlayerUnitList();
        foreach (PlayerUnit pu in tempList)
        {
            StatusObjectCreator.CreateStatusObjectListFromItem(pu.TurnOrder, pu.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON);
            StatusObjectCreator.CreateStatusObjectListFromItem(pu.TurnOrder, pu.ItemSlotOffhand, NameAll.ITEM_SLOT_OFFHAND);
            StatusObjectCreator.CreateStatusObjectListFromItem(pu.TurnOrder, pu.ItemSlotHead, NameAll.ITEM_SLOT_HEAD);
            StatusObjectCreator.CreateStatusObjectListFromItem(pu.TurnOrder, pu.ItemSlotBody, NameAll.ITEM_SLOT_BODY);
            StatusObjectCreator.CreateStatusObjectListFromItem(pu.TurnOrder, pu.ItemSlotAccessory, NameAll.ITEM_SLOT_ACCESSORY);

            AddPUStatusLasting(pu.TurnOrder, pu.ClassId, NameAll.ABILITY_SLOT_PRIMARY);
            AddPUStatusLasting(pu.TurnOrder, pu.AbilitySupportCode, NameAll.ABILITY_SLOT_SUPPORT);
            AddPUStatusLasting(pu.TurnOrder, pu.AbilityMovementCode, NameAll.ABILITY_SLOT_MOVEMENT);
        }
    }

    //remove float_move (for now); these are perma statuses
    //[PunRPC]
    public void RemoveStatusLastingByUnitAndString(int unitId, int statusId)
    {
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("RemoveStatusLastingByUnitAndString", PhotonTargets.Others, new object[] { unitId, statusId });
        //}

        foreach (StatusObject s in sStatusTickList.ToList())
        {
            if (s.GetUnitId() == unitId && s.GetStatusId() == statusId )
            {
                PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), s.GetStatusId());
                sStatusLastingList.Remove(s);
                //break; //guess there could be multiples
            }
        }
    }

	//sends message to PlayerManager on the result of a crystalize status roll
    const string DidStatusManager = "StatusManager.Did";

    //effect for life, golem etc
    //PunRPC called where the actual writes are done and not here
    public void AddStatusAndOverrideOthers(int effect, int unitId, int statusId)
    {
		//if (!PhotonNetwork.offlineMode && !PhotonNetwork.isMasterClient)
		//    return; //only want master to run this. other gets the effects. for example called in PU but don't want other calling it internally
		PlayerManager.Instance.AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_STATUS_MANAGER, NameAll.COMBAT_LOG_SUBTYPE_STATUS_ADD, statusValue: statusId);
        if (statusId == NameAll.STATUS_ID_LIFE)
        {
            Debug.Log("hitting unit with life 1");
            //life only works on undead units if they are alive (thus killing them)
            //OR if the effect has UNDEAD_OVERRIDE, in which case it forces the dead unit back to life
            //effect is the amount of life back
            if (IsUndead(unitId))
            {
                Debug.Log("in adding life to undead 1");
                if(!IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_DEAD))
                {
                    Debug.Log("in adding life to undead 2");
                    AddDead(unitId); 
                }
                else if( effect == NameAll.UNDEAD_OVERRIDE)
                {
                    int z2 = UnityEngine.Random.Range(1, 1 + PlayerManager.Instance.GetPlayerUnit(unitId).StatItemMaxLife);
                    AddLife( z2, unitId);
                }
            }
            else
            {
                Debug.Log("hitting unit with life 2");
                AddLife(effect, unitId);
            }
            return;
        }
        else if (statusId == NameAll.STATUS_ID_ZOMBIE_LIFE && IfStatusByUnitAndId(unitId,NameAll.STATUS_ID_DEAD))
        {
            Debug.Log("adding zombie life");
            if( IsUndead(unitId))
                AddStatusAndOverrideOthers(NameAll.UNDEAD_OVERRIDE, unitId, NameAll.STATUS_ID_LIFE);
            else
                AddStatusAndOverrideOthers(effect, unitId, NameAll.STATUS_ID_LIFE);
            AddStatusAndOverrideOthers(effect, unitId, NameAll.STATUS_ID_UNDEAD);
        }
        else if (statusId == NameAll.STATUS_ID_CRYSTAL)
        {
            Debug.Log("adding crystal ");
            if (IsUndead(unitId) && effect != NameAll.MOVEMENT_CRUNCH) //zombies get CRUNCHED too, stupid zombies
            {
                if( PlayerManager.Instance.IsRollSuccess(50, 1, 100, NameAll.NULL_INT, NameAll.COMBAT_LOG_SUBTYPE_UNDEAD_REVIVE_ROLL, null, null, null, unitId))
                {
                    Debug.Log("undead revival");
                    AddStatusAndOverrideOthers(NameAll.UNDEAD_OVERRIDE, unitId, NameAll.STATUS_ID_LIFE);
                    return;
                }
            }

            //Debug.Log("crystalized for good revival");
            //RemoveStatus(unitId, NameAll.STATUS_ID_DEAD);
            //PlayerManager.Instance.RemoveFromStatusList(unitId, "dead");
            //PlayerManager.Instance.RemoveFromStatusList(unitId, NameAll.STATUS_ID_DEAD_0);
            //PlayerManager.Instance.RemoveFromStatusList(unitId, NameAll.STATUS_ID_DEAD_1);
            //PlayerManager.Instance.RemoveFromStatusList(unitId, NameAll.STATUS_ID_DEAD_2);
            //PlayerManager.Instance.RemoveFromStatusList(unitId, NameAll.STATUS_ID_DEAD_3);
            Debug.Log("adding crystal 2 ");
            AddStatusToTickList(unitId, NameAll.STATUS_ID_CRYSTAL); //guess I could disable all the other statuses but meh
            //PlayerManager.Instance.DisableUnit(unitId);

            if( effect != NameAll.MOVEMENT_CRUNCH) //handled in CombatMoveSequenceState, 
            {
                List<int> tempList = new List<int>();
                tempList.Add(NameAll.STATUS_ID_CRYSTAL);
                tempList.Add(unitId);
				if (PlayerManager.Instance.IsRollSuccess(50, 1, 100, NameAll.NULL_INT, NameAll.COMBAT_LOG_SUBTYPE_CRYSTAL_ROLL, null, null, null, unitId))
                    tempList.Add(1);
                else
                    tempList.Add(0);
                this.PostNotification(DidStatusManager, tempList); //sends this message to GameLoopState and PlayerManager. I believe former handles most of the crystal stuff
            }

            //Debug.Log("crystalize status added, do something with the map");
            return;
        }
        else if(statusId == NameAll.DEATH_MODE_UNCONSCIOUS)
        {
            AddStatusToTickList(unitId, statusId);
            return;
        }

        if ( IfStatusByUnitAndId(unitId,NameAll.STATUS_ID_DEAD))
        {
            return;
        }

        if (statusId == NameAll.STATUS_ID_CANCEL_REMEDY)
        {
            //Cancel: Petrify, Darkness, Confusion, Silence,  Oil, Frog, Poison, Sleep
            RemoveStatus(unitId, NameAll.STATUS_ID_PETRIFY);
            RemoveStatus(unitId, NameAll.STATUS_ID_DARKNESS);
            RemoveStatus(unitId, NameAll.STATUS_ID_CONFUSION);
            RemoveStatus(unitId, NameAll.STATUS_ID_SILENCE);
            RemoveStatus(unitId, NameAll.STATUS_ID_FROG);
            RemoveStatus(unitId, NameAll.STATUS_ID_POISON);
            RemoveStatus(unitId, NameAll.STATUS_ID_SLEEP);
            RemoveStatus(unitId, NameAll.STATUS_ID_OIL);
            return;
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_STIGMA_MAGIC)
        {
//        Cancel: Petrify, Darkness, Confusion,      |
//| REFL: -  | CM: - | CTR:   0 | Frog, Silence, Berserk, Poison,    |
//| CALC: -  | CF: - | JP: 200 | Sleep, Don't Move, Don't Act
            RemoveStatus(unitId, NameAll.STATUS_ID_PETRIFY);
            RemoveStatus(unitId, NameAll.STATUS_ID_CONFUSION);
            RemoveStatus(unitId, NameAll.STATUS_ID_DARKNESS);
            RemoveStatus(unitId, NameAll.STATUS_ID_FROG);
            RemoveStatus(unitId, NameAll.STATUS_ID_SILENCE);
            RemoveStatus(unitId, NameAll.STATUS_ID_BERSERK);
            RemoveStatus(unitId, NameAll.STATUS_ID_POISON);
            RemoveStatus(unitId, NameAll.STATUS_ID_SLEEP);
            RemoveStatus(unitId, NameAll.STATUS_ID_DONT_MOVE);
            RemoveStatus(unitId, NameAll.STATUS_ID_DONT_ACT);
            return;
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_ESUNA)
        {
            //        Cancel: Petrify, Darkness, Confusion,      |
            //| REFL: -  | CM: - | CTR:   0 | Frog, Silence, Berserk, Poison,    |
            //| CALC: -  | CF: - | JP: 200 | Sleep, Don't Move, Don't Act
            RemoveStatus(unitId, NameAll.STATUS_ID_PETRIFY);
            RemoveStatus(unitId, NameAll.STATUS_ID_CONFUSION);
            RemoveStatus(unitId, NameAll.STATUS_ID_DARKNESS);
            RemoveStatus(unitId, NameAll.STATUS_ID_FROG);
            RemoveStatus(unitId, NameAll.STATUS_ID_SILENCE);
            RemoveStatus(unitId, NameAll.STATUS_ID_BERSERK);
            RemoveStatus(unitId, NameAll.STATUS_ID_POISON);
            RemoveStatus(unitId, NameAll.STATUS_ID_SLEEP);
            RemoveStatus(unitId, NameAll.STATUS_ID_DONT_MOVE);
            RemoveStatus(unitId, NameAll.STATUS_ID_DONT_ACT);
            return;
        }
        else if( statusId == NameAll.STATUS_ID_CANCEL_NEGATIVE)
        {
            //Debug.Log("removing negative status, cancel_negative");
            RemoveStatusNegative(unitId, 1); //for now just remove now in the future maybe modify the effect to remove multiple bad statuses
            return;
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_PETRIFY)
        {
            RemoveStatus(unitId, NameAll.STATUS_ID_PETRIFY);
            return;
        }

        if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_PETRIFY))
        {
            return;
        }

        if (statusId == NameAll.STATUS_ID_CHARGING)
        {
            RemoveForOverride(unitId, statusId);
            AddStatusToTickList(unitId, statusId);
            RemoveStatus(unitId, NameAll.STATUS_ID_PERFORMING, true);
            RemoveStatus(unitId, NameAll.STATUS_ID_DEFENDING, true);
        }
        else if (statusId == NameAll.STATUS_ID_PERFORMING)
        {
            RemoveForOverride(unitId, statusId);
            AddStatusToTickList(unitId, statusId);
            RemoveStatus(unitId, NameAll.STATUS_ID_CHARGING, true);
            RemoveStatus(unitId, NameAll.STATUS_ID_DEFENDING, true);
        }
        else if (statusId == NameAll.STATUS_ID_DEFENDING)
        {
            RemoveForOverride(unitId, statusId);
            AddStatusToTickList(unitId, statusId);
            RemoveStatus(unitId, NameAll.STATUS_ID_CHARGING, true);
            RemoveStatus(unitId, NameAll.STATUS_ID_PERFORMING, true);
        }
        else if (statusId == NameAll.STATUS_ID_JUMPING)
        {
            RemoveForOverride(unitId, statusId);
            AddStatusToTickList(unitId, statusId);
            RemoveStatus(unitId, NameAll.STATUS_ID_CHARGING, true);
            RemoveStatus(unitId, NameAll.STATUS_ID_PERFORMING, true);
            PlayerManager.Instance.ToggleJumping(unitId, true);
        }
        else if (statusId == NameAll.STATUS_ID_CRITICAL)
        {
            RemoveForOverride(unitId, statusId);
            AddStatusToTickList(unitId, statusId);
        }
        else if (statusId == NameAll.STATUS_ID_FLOAT)
        {
            //checks that float is not innate by move or item
            if (!IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_FLOAT, false) &&
                    !IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_FLOAT_MOVE, false))
            {
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_HASTE)
        {
            //if slow, slow is overriden; if haste_item nothing happens; if haste haste is overriden
            RemoveStatus(unitId, NameAll.STATUS_ID_SLOW, true);
            if (!IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_HASTE, false))
            {
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_REGEN_HASTE) //doing it below
        {
            AddStatusAndOverrideOthers(0, unitId, NameAll.STATUS_ID_REGEN);
            AddStatusAndOverrideOthers(0, unitId, NameAll.STATUS_ID_HASTE);
            ////if slow, slow is overriden; if haste_item nothing happens; if haste haste is overriden
            //RemoveStatus(unitId, NameAll.STATUS_ID_SLOW, true);
            //if (!IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_HASTE, false))
            //{
            //    //if already heaste removes the old one
            //    RemoveForOverride(unitId, NameAll.STATUS_ID_HASTE);
            //    AddStatusToTickList(unitId, statusId);
            //}
            //RemoveStatus(unitId, NameAll.STATUS_ID_POISON, true);
            //if (!IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_REGEN, false))
            //{
            //    RemoveForOverride(unitId, NameAll.STATUS_ID_REGEN);
            //    AddStatusToTickList(unitId, statusId);
            //}
        }
        else if (statusId == NameAll.STATUS_ID_PROTECT)
        {
            if (!IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_PROTECT, false))
            {
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_REGEN)
        { //similar to slow/haste
            RemoveStatus(unitId, NameAll.STATUS_ID_POISON, true);
            if (!IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_REGEN, false))
            {
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_RERAISE)
        {
            //cannot go on undead unless the source is auto-reraise (ie item)
            if (!IsUndead(unitId))
            {
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_SHELL)
        {
            if (!IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_SHELL, false))
            {
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_AUTO_PROTECT_SHELL)
        {
            if (!IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_PROTECT, false))
            {
                RemoveForOverride(unitId, NameAll.STATUS_ID_PROTECT);
                AddStatusToTickList(unitId, statusId);
            }
            if (!IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_SHELL, false))
            {
                RemoveForOverride(unitId, NameAll.STATUS_ID_SHELL);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if( statusId == NameAll.STATUS_ID_PROTECT_SHELL)
        {
            AddStatusAndOverrideOthers(0, unitId, NameAll.STATUS_ID_PROTECT);
            AddStatusAndOverrideOthers(0, unitId, NameAll.STATUS_ID_SHELL);
        }
        //else if (status.Equals("blood_suck"))
        //{
        //    RemoveForOverride(unitId, status);
        //    CheckStatusAndRemoveSlowAction(unitId, status, true);
        //}
        else if (statusId == NameAll.STATUS_ID_CHICKEN)
        {
            RemoveForOverride(unitId, statusId);
            CheckStatusAndRemoveSlowAction(unitId, statusId, false); //status added here
        }
        else if (statusId == NameAll.STATUS_ID_CHARM)
        {
            if (!IfStatusLastingBlocksStatus(unitId, statusId))
            {
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_CONFUSION)
        {
            RemoveForOverride(unitId, statusId);
            CheckStatusAndRemoveSlowAction(unitId, statusId, false);
        }
        else if (statusId == NameAll.STATUS_ID_DARKNESS)
        {
            if (!IfStatusLastingBlocksStatus(unitId, statusId))
            {
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_DEAD)
        { //this is the dead status, not dead from hp loss
            if (!IfStatusLastingBlocksStatus(unitId, statusId))
            {
                AddDead(unitId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_DEATH_SENTENCE)
        {
            if (!IfStatusLastingBlocksStatus(unitId, statusId))
            {
                //assuming death sentence doesn't override it
                if( !IfStatusByUnitAndId(unitId,statusId,true))
                {
                    AddStatusToTickList(unitId, statusId);
                    PlayerManager.Instance.AddToStatusList(unitId, NameAll.STATUS_ID_DEATH_SENTENCE_3); //special removeal
                    //if (!PhotonNetwork.offlineMode) //online: need to send a special call so Other shows the status image
                    //    PlayerManager.Instance.SendMPStatusList(isAdd: true, unitId: unitId, statusId: NameAll.STATUS_ID_DEATH_SENTENCE_3);
                }    
            }
        }
        else if (statusId == NameAll.STATUS_ID_DONT_ACT)
        {
            RemoveForOverride(unitId, statusId);
            CheckStatusAndRemoveSlowAction(unitId, statusId, false);
        }
        else if (statusId == NameAll.STATUS_ID_DONT_MOVE)
        {
            if (!IfStatusLastingBlocksStatus(unitId, statusId))
            {
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_FROG)
        {
            //Debug.Log("addings status " + statusId);
            if (!IfStatusLastingBlocksStatus(unitId, statusId))
            {
                if (IfStatusByUnitAndId(unitId, statusId, true))
                { //Removes frog
                    RemoveStatus(unitId, statusId, true);
                }
                else { //adds frog
                    //Debug.Log("addings status " + statusId);
                    CheckStatusAndRemoveSlowAction(unitId, statusId, false); 
                }
            }
        }
        //else if (status.Equals("morbol"))
        //{
        //    CheckStatusAndRemoveSlowAction(unitId, status, false);
        //}
        else if (statusId == NameAll.STATUS_ID_OIL)
        {
            //Debug.Log("adding oil 1");
            if (!IfStatusLastingBlocksStatus(unitId, statusId))
            {
                //Debug.Log("adding oil 2");
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_PETRIFY)
        {
            RemoveForOverride(unitId, statusId);
            //special code in this for petrify to Remove death_sentence and transparent
            CheckStatusAndRemoveSlowAction(unitId, statusId, true);
        }
        else if (statusId == NameAll.STATUS_ID_POISON)
        {
            if (!IfStatusLastingBlocksStatus(unitId, statusId))
            {
                RemoveStatus(unitId, NameAll.STATUS_ID_REGEN, true); //Remove regen
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_SILENCE)
        {
            if (!IfStatusLastingBlocksStatus(unitId, statusId))
            {
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_SLEEP)
        {
            RemoveForOverride(unitId, statusId);
            CheckStatusAndRemoveSlowAction(unitId, statusId, false);
        }
        else if (statusId == NameAll.STATUS_ID_SLOW)
        {
            //Debug.Log("adding slow");
            RemoveStatus(unitId, NameAll.STATUS_ID_HASTE, true); //Remove haste
            RemoveForOverride(unitId, statusId);
            AddStatusToTickList(unitId, statusId);
        }
        else if (statusId == NameAll.STATUS_ID_STOP)
        {
            Debug.Log("adding stop");
            RemoveForOverride(unitId, statusId);
            CheckStatusAndRemoveSlowAction(unitId, statusId, false);
        }
        else if (statusId == NameAll.STATUS_ID_UNDEAD)
        {
            if (!IfStatusLastingBlocksStatus(unitId, statusId) && !IsUndead(unitId) )
            {
                //RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_BERSERK)
        {
            RemoveForOverride(unitId, statusId);
            CheckStatusAndRemoveSlowAction(unitId, statusId, false);
        }
        else if (statusId == NameAll.STATUS_ID_FAITH)
        {
            if (!IfStatusLastingBlocksStatus(unitId, statusId))
            {
                RemoveStatus(unitId, NameAll.STATUS_ID_INNOCENT, true);
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_INNOCENT)
        {
            if (!IfStatusLastingBlocksStatus(unitId, statusId))
            {
                RemoveStatus(unitId, NameAll.STATUS_ID_FAITH, true);
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_REFLECT)
        {
            //Debug.Log("outer reflect");
            if (!IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_REFLECT, false))
            {
                //Debug.Log("inner reflect");
                RemoveForOverride(unitId, statusId);
                AddStatusToTickList(unitId, statusId);
            }
        }
        else if (statusId == NameAll.STATUS_ID_QUICK)
        {
            RemoveForOverride(unitId, statusId);
            AddStatusToTickList(unitId, statusId);
            PlayerManager.Instance.SetQuickFlag(unitId, true); //toggles the quick flag on
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_POISON)
        {
            RemoveStatus(unitId, NameAll.STATUS_ID_POISON);
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_DARKNESS)
        {
            RemoveStatus(unitId, NameAll.STATUS_ID_DARKNESS);
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_SILENCE)
        {
            RemoveStatus(unitId, NameAll.STATUS_ID_SILENCE);
        }   
        else if (statusId == NameAll.STATUS_ID_CANCEL_HEAL)
        {
            RemoveStatus(unitId, NameAll.STATUS_ID_DARKNESS);
            RemoveStatus(unitId, NameAll.STATUS_ID_POISON);
            RemoveStatus(unitId, NameAll.STATUS_ID_SILENCE);
        }  
        else if (statusId == NameAll.STATUS_ID_CANCEL_FROG)
        {
            RemoveStatus(unitId, NameAll.STATUS_ID_FROG);
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_HOLY_WATER)
        {
            RemoveStatus(unitId, NameAll.STATUS_ID_UNDEAD);
        }
        else if (statusId == NameAll.STATUS_ID_INVITE)
        {
            //pul.defectFromTeam(unitId);
            //Debug.Log("invite hit...");
            PlayerManager.Instance.DefectFromTeam(unitId); //not worrying about charm here
        }
        else if (statusId == NameAll.STATUS_ID_GOLEM)
        {
            FuckingGolem(effect, unitId);
            //AddStatusToTickList(unitId, statusId);
            AddGolemToTeam(unitId);
        }
        else if(statusId == NameAll.STATUS_ID_NAMELESS_DANCE)
        {
            int newStatusId = GetStatusFromNameless(statusId);
            AddStatusAndOverrideOthers(0, unitId, newStatusId);
            //RemoveForOverride(unitId, newStatusId);
            //AddStatusToTickList(unitId, newStatusId);
        }
        else if (statusId == NameAll.STATUS_ID_NAMELESS_SONG)
        {
            int newStatusId = GetStatusFromNameless(statusId);
            AddStatusAndOverrideOthers(0, unitId, newStatusId);
            //RemoveForOverride(unitId, newStatusId);
            //AddStatusToTickList(unitId, newStatusId);
        }
        else if( statusId == NameAll.STATUS_ID_ADD_POSITIVE)
        {
            int newStatusId = GetStatusFromNameless(statusId);
            AddStatusAndOverrideOthers(0, unitId, newStatusId);
            //RemoveForOverride(unitId, newStatusId);
            //AddStatusToTickList(unitId, newStatusId);
        }
        else if (statusId == NameAll.STATUS_ID_CURE_STATUS_2)
        {
            //Debug.Log("adding zombie life");
            //zString = "cure undead, charm, faith, innocent, reflect, stop, slow";
            RemoveStatus(unitId,NameAll.STATUS_ID_UNDEAD);
            RemoveStatus(unitId, NameAll.STATUS_ID_CHARM);
            RemoveStatus(unitId, NameAll.STATUS_ID_FAITH);
            RemoveStatus(unitId, NameAll.STATUS_ID_INNOCENT);
            RemoveStatus(unitId, NameAll.STATUS_ID_REFLECT);
            RemoveStatus(unitId, NameAll.STATUS_ID_STOP);
            RemoveStatus(unitId, NameAll.STATUS_ID_SLOW);
        }
        else if (statusId == NameAll.STATUS_ID_OIL_FAITH)
        {
            //Debug.Log("adding oil faith");
            AddStatusAndOverrideOthers(effect, unitId, NameAll.STATUS_ID_OIL);
            AddStatusAndOverrideOthers(effect, unitId, NameAll.STATUS_ID_FAITH);
        }
        else if (statusId == NameAll.STATUS_ID_HASTE_DS)
        {
            //Debug.Log("adding oil faith");
            AddStatusAndOverrideOthers(effect, unitId, NameAll.STATUS_ID_HASTE);
            AddStatusAndOverrideOthers(effect, unitId, NameAll.STATUS_ID_DEATH_SENTENCE);
        }
        else if (statusId == NameAll.STATUS_ID_CANCEL_DISPEL)
        {
//            Protect, Shell, Haste, Float,      |
//| REFL: -  | CM: + | CTR:   3 | Regen, Reraise, Transparent, Faith,|
//| CALC: +  | CF: - | JP: 700 | Reflect
            RemoveStatus(unitId, NameAll.STATUS_ID_PROTECT);
            RemoveStatus(unitId, NameAll.STATUS_ID_SHELL);
            RemoveStatus(unitId, NameAll.STATUS_ID_HASTE);
            RemoveStatus(unitId, NameAll.STATUS_ID_FLOAT);
            RemoveStatus(unitId, NameAll.STATUS_ID_REGEN);
            RemoveStatus(unitId, NameAll.STATUS_ID_RERAISE);
            RemoveStatus(unitId, NameAll.STATUS_ID_FAITH);
            RemoveStatus(unitId, NameAll.STATUS_ID_REFLECT);
        }
        else
        {
            Debug.Log("added a status but no effect..." + statusId);
        }
    }

    
    //called in override other, if the status already exists, remove the current version and override it
    private void RemoveForOverride(int unitId, int statusId)
    {
        if( IfStatusByUnitAndId(unitId, statusId, true))
        {
            RemoveStatus(unitId, statusId, true);
        }
    }

    //weird exceptions like slow removing haste from haste_item but haste from haste_item returning when slow returns
    //called in SlowActionState to remove charging from casting unit
    //[PunRPC]
    public void RemoveStatus(int unitId, int statusId, bool isTickList = true)
    {
        
        if (isTickList)
        {
            //Probably should move this into the foreach loop and see if status needs to be removed rather than call it preemptively
            //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
            //{
            //    photonView.RPC("RemoveStatus", PhotonTargets.Others, new object[] { unitId, statusId, true });
            //}

            foreach (StatusObject s in sStatusTickList.ToList())
            {
                if (s.RemoveStatusObject(unitId, statusId))
                {
                    PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), s.GetStatusId());
                    sStatusTickList.Remove(s);

                    
                    break;
                }
            }
        }
    }

    //[PunRPC]
    void RemoveStatusNegative(int unitId, int numberToRemove, bool isTickList = true)
    {
        if (isTickList)
        {
            //could move it into the foreach loop and see if status is removed rather than call it preemptively
            //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
            //{
            //    photonView.RPC("RemoveStatusNegative", PhotonTargets.Others, new object[] { unitId, numberToRemove, true });
            //}

            foreach (StatusObject s in sStatusTickList.ToList())
            {
                //Debug.Log("removing negative statuses ");
                if( s.GetUnitId() == unitId && IsNegativeStatusId(s.GetStatusId() ))
                {
                    PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), s.GetStatusId());
                    sStatusTickList.Remove(s);
                    numberToRemove -= 1;
                    if (numberToRemove <= 0)
                        return;
                }
            }
        }
    }

    //remove all the statuses in a list (like curing a bunch of statuses)
    //not a punRPC since I have to turn the statusIdList into an array, PunRPC follows below
    public void RemoveStatusList(int unitId, List<int> statusIdList)
    {
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("RemoveStatusListArray", PhotonTargets.Others, new object[] { unitId, statusIdList.ToArray() });
        //}
        
        foreach (StatusObject s in sStatusTickList.ToList())
        {
            if (s.GetUnitId() == unitId)
            {
                foreach (int statusId in statusIdList)
                {
                    if (s.GetStatusId() == statusId)
                    {
                        PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), s.GetStatusId());
                        sStatusTickList.Remove(s);
                    }
                }
            }
        }
    }

    //[PunRPC]
    public void RemoveStatusListArray(int unitId, int[] statusIdArray)
    {

        foreach (StatusObject s in sStatusTickList.ToList())
        {
            if (s.GetUnitId() == unitId)
            {
                foreach (int statusId in statusIdArray)
                {
                    if (s.GetStatusId() == statusId)
                    {
                        PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), s.GetStatusId());
                        sStatusTickList.Remove(s);
                    }
                }
            }
        }
    }


    //need the slot in case double items are equipped
    //[PunRPC]
    public void RemoveStatusItems(int unitId, int itemId, int slot)
    {
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("RemoveStatusItems", PhotonTargets.Others, new object[] { unitId, itemId, slot });
        //}
        foreach (StatusObject s in sStatusLastingList.ToList())
        {
            if (s.GetUnitId() == unitId && s.GetItemId() == itemId && s.GetItemSlot() == slot)
            {
                PlayerManager.Instance.RemoveFromStatusList(s.GetUnitId(), s.GetStatusId());
                sStatusLastingList.Remove(s);
            }
        }
    }

    //called in PlayerUnit and TurnsManager to increment CT, check to see if CT is actually incremented is elsewhere
    //slow/haste override their item equivalents, assuming having both with items cancel each other out
    public int ModifySpeed(int unitId, int tempSpeed)
    {
        int z1 = tempSpeed;
        if( IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_DEAD,true))
        {
            //does nothing, returns the inputted speed
        } else if ( IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_SLOW, true ) )
        {
            z1 = z1 / 2;
        }
        else if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_HASTE, true))
        {
            z1 = z1 * 3 / 2;
        }
        else if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_HASTE, false) 
            && !IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_SLOW, false)) //assumign the two cancel each other out
        {
            z1 = z1 * 3 / 2;
        }
        else if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_SLOW, false)
            && !IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_HASTE, false))
        {
            z1 = z1 / 2;
        }
        return z1;
    }

    //called in the mod spells
    public long GetModFaith(int unitId, int unitFaith)
    {
        long z1 = unitFaith;
        if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_INNOCENT, true))
        {
            z1 = 0;
        }
        else if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_FAITH, true))
        {
            z1 = 100;
        }
        else if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_FAITH, false)
            && !IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_INNOCENT, false)) //assumign the two cancel each other out
        {
            z1 = 100;
        }
        else if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_INNOCENT, false)
            && !IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_AUTO_FAITH, false))
        {
            z1 = 0;
        }
        return z1;
    }

    public bool IfStatusByUnitAndId(int unit_id, int statusId, bool is_tick_type = true)
    {
        if (is_tick_type)
        {
            foreach (StatusObject s in sStatusTickList.ToList())
            {
                if (s.GetUnitId() == unit_id)
                {
                    if (s.GetStatusId() == statusId)
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            //Debug.Log("in inner check sStatusLastingList check" + sStatusLastingList.Count);
            foreach (StatusObject s in sStatusLastingList)
            {
                //Debug.Log("in inner check sStatusLastingList chieck");
                if (s.GetUnitId() == unit_id)
                {
                    if (s.GetStatusId() == statusId )
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool IfNoEvade(int unit_id, int which_array = 1)
    {
        List<int> tempStatus = new List<int>(); //these statuses mean the target cannot evade
        tempStatus.Add(NameAll.STATUS_ID_CHARGING);
        tempStatus.Add(NameAll.STATUS_ID_PERFORMING);
        tempStatus.Add(NameAll.STATUS_ID_DONT_ACT);
        tempStatus.Add(NameAll.STATUS_ID_CHICKEN);
        tempStatus.Add(NameAll.STATUS_ID_FROG);

        tempStatus.Add(NameAll.STATUS_ID_SLEEP);
        tempStatus.Add(NameAll.STATUS_ID_STOP);
        tempStatus.Add(NameAll.STATUS_ID_CONFUSION);
        tempStatus.Add(NameAll.STATUS_ID_BERSERK);
        //meh blood suck

        if (which_array == 1)
        {
            foreach (StatusObject s in sStatusTickList.ToList())
            {
                if (s.GetUnitId() == unit_id)
                {
                    foreach (int i in tempStatus)
                    {
                        if (s.GetStatusId() == i )
                        {
                            if(!IsShowMust(unit_id, NameAll.STATUS_ID_CHARGING ) )//anything at this point is good for IsShow
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool IfNoHit(int unit_id)
    {
        List<int> tempStatus = new List<int>();
        tempStatus.Add(NameAll.STATUS_ID_DEAD);
        tempStatus.Add(NameAll.STATUS_ID_PETRIFY);
        //tempStatus.Add("petrify");
        //tempStatus.Add("dead");

        foreach (StatusObject s in sStatusTickList.ToList())
        {
            if (s.GetUnitId() == unit_id)
            {
                foreach (int i in tempStatus)
                {
                    if (s.GetStatusId() == i)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    //called in SpellNameAI to help classify a spellNames
    public bool IfStatusCuredBySpell(int statusId, SpellName sn)
    {
        if (sn.AddsStatus == 1)
        { //adds a status of some type

            if (statusId == NameAll.STATUS_ID_DEAD )
            {
                if (sn.StatusType == NameAll.STATUS_ID_LIFE || sn.StatusType == NameAll.STATUS_ID_ZOMBIE_LIFE)
                {
                    return true;
                }
            }
            else if (statusId == NameAll.STATUS_ID_PETRIFY )
            {
                if (sn.StatusType == NameAll.STATUS_ID_CANCEL_ESUNA|| sn.StatType == NameAll.STATUS_ID_CANCEL_PETRIFY
                        || sn.StatusType == NameAll.STATUS_ID_CANCEL_STIGMA_MAGIC)
                {
                    return true;
                }
            }
            else if (statusId == NameAll.STATUS_ID_FROG)
            {
                if (sn.StatusType == NameAll.STATUS_ID_CANCEL_ESUNA
                        || sn.StatusType == NameAll.STATUS_ID_FROG || sn.StatusType == NameAll.STATUS_ID_CANCEL_FROG )
                { //frog on frog equals anti frog
                    return true;
                }
            }
            else if (statusId == NameAll.STATUS_ID_BLOOD_SUCK)
            {
                if (sn.StatusType == NameAll.STATUS_ID_CANCEL_HOLY_WATER )
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IfSpellCuresAnyStatus(SpellName sn, int unitId)
    {
        List<int> temp = new List<int>();
        if (sn.AddsStatus == 1)
        {
            if (sn.StatusType == NameAll.STATUS_ID_CANCEL_ESUNA || sn.StatusType == NameAll.STATUS_ID_CANCEL_STIGMA_MAGIC)
            {
                //                Cancel: Petrify, Darkness, Confusion,      |
                //                | REFL: +  |  CM: - | CTR:   3   |         Silence, Berserk, Frog, Poison,    |
                //                | CALC: +  |  CF: - |  JP: 280   |         Sleep, Don't Move, Don't Act
                temp.Add(NameAll.STATUS_ID_PETRIFY); temp.Add(NameAll.STATUS_ID_DARKNESS); temp.Add(NameAll.STATUS_ID_CONFUSION);
                temp.Add(NameAll.STATUS_ID_SILENCE); temp.Add(NameAll.STATUS_ID_BERSERK); temp.Add(NameAll.STATUS_ID_FROG); temp.Add(NameAll.STATUS_ID_POISON);
                temp.Add(NameAll.STATUS_ID_SLEEP); temp.Add(NameAll.STATUS_ID_DONT_MOVE); temp.Add(NameAll.STATUS_ID_DONT_ACT);
            }
            else if (sn.StatusType == NameAll.STATUS_ID_CANCEL_HEAL)
            {
                //Cancel: Darkness, Silence, Poison
                temp.Add(NameAll.STATUS_ID_DARKNESS); temp.Add(NameAll.STATUS_ID_SILENCE); temp.Add(NameAll.STATUS_ID_POISON);
            }
            else if (sn.StatusType == NameAll.STATUS_ID_CANCEL_DISPEL)
            {
                //                magical  | CBG: - |  MP:  34   | Cancel: Protect, Shell, Haste, Float,      |
                //                | REFL: -  |  CM: + | CTR:   3   |         Regen, Reraise, Transparent, Faith,|
                //                | CALC: +  |  CF: - |  JP: 700   |         Reflect
                temp.Add(NameAll.STATUS_ID_PROTECT); temp.Add(NameAll.STATUS_ID_SHELL); temp.Add(NameAll.STATUS_ID_HASTE); temp.Add(NameAll.STATUS_ID_FLOAT);
                temp.Add(NameAll.STATUS_ID_REGEN); temp.Add(NameAll.STATUS_ID_RERAISE); temp.Add(NameAll.STATUS_ID_FAITH);
                temp.Add(NameAll.STATUS_ID_REFLECT);
            }
        }
        foreach (int s in temp)
        {
            if (IfStatusByUnitAndId(unitId, s, true))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsCTHalted(int unit_id)
    {
        foreach (StatusObject s in sStatusTickList.ToList())
        {
            if (s.GetUnitId() == unit_id)
            {
                if (s.GetStatusId() == NameAll.STATUS_ID_STOP || s.GetStatusId() == NameAll.STATUS_ID_SLEEP
                        || s.GetStatusId() == NameAll.STATUS_ID_PETRIFY || s.GetStatusId() == NameAll.STATUS_ID_CRYSTAL)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //OBSOLETE
    //public bool IsTurnEligible(int unit_id)
    //{
    //    //Debug.Log("turn eligible check " + unit_id);
    //    foreach (StatusObject s in sStatusTickList.ToList())
    //    {
    //        //Debug.Log("turn eligible check " + unit_id + s.GetStatusId());
    //        if (s.GetUnitId() == unit_id)
    //        {
    //            if(s.GetStatusId() == NameAll.STATUS_ID_DEAD )
    //            {
    //                //Debug.Log("unit is dead " + unit_id);
    //                if( SceneCreate.phaseActor == 0) //dead at start of turn
    //                {
    //                    PlayerManager.Instance.TakeTurn(unit_id, 0);
    //                }
    //                else
    //                {
    //                    PlayerManager.Instance.TakeTurn(unit_id); //mid turn death
    //                }
                    
    //                return false;
    //            }
    //            else if (s.GetStatusId() == NameAll.STATUS_ID_STOP|| s.GetStatusId() == NameAll.STATUS_ID_SLEEP
    //                    || s.GetStatusId() == NameAll.STATUS_ID_PETRIFY)
    //            {
    //                //Debug.Log("unit is not turn eligible " + unit_id);
    //                return false;
    //            }
    //        }
    //    }
    //    //Debug.Log("unit is turn eligible " + unit_id);
    //    return true;
    //}

    //petrify, sleep, stop, these units don't get an active turn
    //dead units do get an active turn (they just don't get to do anything with it), their CT decrements  
    public bool IsTurnActable(int unitId)
    {
        foreach (StatusObject s in sStatusTickList.ToList())
        {
            if (s.GetUnitId() == unitId)
            {
                if (s.GetStatusId() == NameAll.STATUS_ID_STOP || s.GetStatusId() == NameAll.STATUS_ID_SLEEP
                        || s.GetStatusId() == NameAll.STATUS_ID_PETRIFY )
                {
                    //Debug.Log("unit is not turn eligible " + unit_id);
                    return false;
                }
            }
        }
        return true;
    }

    //called in calculationresolveAction getHitdamage, if true the hit chance goes to 0
    public bool IsStatusBlockByUnit(int unitId, int statusId)
    {
        //string zString = "block_" + statusName;
        int z1 = NameAll.GetStatusBlockId(statusId);
        foreach (StatusObject s in sStatusLastingList)
        {
            if (s.GetUnitId() == unitId && s.GetStatusId() == z1)
            {
                return true;
            }
        }
        return false;
    }

    //called in mods, goes through status lasting list
    public bool IsUnitStrengthenByElement(PlayerUnit unit, int element)
    {
        int elementId = NameAll.GetStatusIdFromElementType(element,NameAll.ELEMENT_TYPE_STRENGTHEN);
        int unitId = unit.TurnOrder;
        if(AbilityManager.Instance.IsInnateAbility(unit.ClassId, elementId, NameAll.ABILITY_SLOT_PRIMARY)
            || (element == NameAll.ITEM_ELEMENTAL_FIRE && PlayerManager.Instance.IsAbilityEquipped(unitId, NameAll.SUPPORT_FLAME_TOUCHED, NameAll.ABILITY_SLOT_SUPPORT))
            || ((element == NameAll.ITEM_ELEMENTAL_LIGHT || element == NameAll.ITEM_ELEMENTAL_UNDEAD) && PlayerManager.Instance.IsAbilityEquipped(unitId, NameAll.SUPPORT_LIGHTS_BLESSING, NameAll.ABILITY_SLOT_SUPPORT)))
        {
            return true;
        }

        foreach (StatusObject s in sStatusLastingList)
        {
            if( s.GetUnitId() == unitId && s.GetStatusId() == elementId)
            {
                return true;
            }
        }
        return false;
    }

    //called in mods, goes through status lasting list
    public bool IsUnitWeakByElement(int unitId, int element)
    {
        int elementId = NameAll.GetStatusIdFromElementType(element, NameAll.ELEMENT_TYPE_WEAK);

        foreach (StatusObject s in sStatusLastingList)
        {
            if (s.GetUnitId() == unitId && s.GetStatusId() == elementId)
            {
                return true;
            }
        }
        //Debug.Log(" testing for burn");
        if ( element == NameAll.ITEM_ELEMENTAL_FIRE) //checks target for burn
        {
            foreach (StatusObject s in sStatusTickList.ToList())
            {
                //Debug.Log(" " + s.GetUnitId() + " asdf " + s.GetStatusId());
                if (s.GetUnitId() == unitId && s.GetStatusId() == NameAll.STATUS_ID_OIL)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //called in mods, goes through status lasting list
    public bool IsUnitHalfByElement(PlayerUnit unit, int element)
    {
        int elementId = NameAll.GetStatusIdFromElementType(element, NameAll.ELEMENT_TYPE_HALF);
        int unitId = unit.TurnOrder;
        if (AbilityManager.Instance.IsInnateAbility(unit.ClassId, elementId, NameAll.ABILITY_SLOT_PRIMARY)
            || (element == NameAll.ITEM_ELEMENTAL_FIRE && PlayerManager.Instance.IsAbilityEquipped(unitId,NameAll.SUPPORT_FLAME_TOUCHED, NameAll.ABILITY_SLOT_SUPPORT)) )
        {
            return true;
        }

        foreach (StatusObject s in sStatusLastingList)
        {
            if (s.GetUnitId() == unitId && s.GetStatusId() == elementId)
            {
                return true;
            }
        }
        return false;
    }

    //called in mods, goes through status lasting list
    public bool IsUnitAbsorbByElement(int unitId, int element)
    {
        int elementId = NameAll.GetStatusIdFromElementType(element, NameAll.ELEMENT_TYPE_ABSORB);
        
        foreach (StatusObject s in sStatusLastingList)
        {
            if (s.GetUnitId() == unitId && s.GetStatusId() == elementId)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckIfReflect(int unitId)
    {
        foreach (StatusObject s in sStatusTickList.ToList())
        {
            if (s.GetUnitId() == unitId && s.GetStatusId() == NameAll.STATUS_ID_REFLECT )
            {
                return true;
            }
        }
        foreach (StatusObject s in sStatusLastingList)
        { //goes through dead
            if (s.GetUnitId() == unitId && s.GetStatusId() == NameAll.STATUS_ID_AUTO_REFLECT )
            {
                return true;
            }
        }
        return false;
    }

    public bool IsUndead(int unitId)
    {
        foreach (StatusObject s in sStatusTickList.ToList())
        {
            if (s.GetUnitId() == unitId && s.GetStatusId() == NameAll.STATUS_ID_UNDEAD)
            {
                return true;
            }
        }
        foreach (StatusObject s in sStatusLastingList)
        { //goes through dead
            if (s.GetUnitId() == unitId
                    && (s.GetStatusId() == NameAll.STATUS_ID_AUTO_UNDEAD ))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckIfFloat(int unitId)
    {
        foreach (StatusObject s in sStatusTickList.ToList())
        {
            if (s.GetUnitId() == unitId && s.GetStatusId() == NameAll.STATUS_ID_FLOAT)
            {
                return true;
            }
        }
        foreach (StatusObject s in sStatusLastingList)
        { //goes through dead
            if (s.GetUnitId() == unitId
                    && (s.GetStatusId() == NameAll.STATUS_ID_AUTO_FLOAT || s.GetStatusId() == NameAll.STATUS_ID_FLOAT_MOVE))
            {
                return true;
            }
        }
        return false;
    }

    //Berserk, Blood Suck, Don't Act, Sleep, Confusion, Stop
    public bool IsAbleToReact(int unitId)
    {
        //unable to fight ones (dead, petrify, blood suck already handled)
        if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_DEAD, true)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_BERSERK, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_SLEEP, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_DONT_ACT, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_FROG, true) //uncertain about this
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_STOP, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_CONFUSION, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_PETRIFY, true)
                //|| IfStatusByUnitAndId(unitId, "blood_suck", true)
                )
        {
            //Debug.Log("not able to react, return false");
            return false;
        }
        //Debug.Log("able to react, returning true");
        return true;
    }

    public bool IsAbleToSecondSwing(int unitId)
    {
        if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_DEAD, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_SLEEP, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_DONT_ACT, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_STOP, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_PETRIFY, true)
                )
        {
            return false;
        }
        return true;
    }

    public bool IsAIControlledStatus(int unitId)
    {
        //unable to fight ones (dead, petrify, blood suck already handled)
        if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_CHICKEN, true)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_BERSERK, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_CONFUSION, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_CHARM, true)
                )
        {
            return true;
        }
        return false;
    }

    public bool UnitCantResolveSlowSpell(int unitId)
    {
        //unable to fight ones (dead, petrify, blood suck already handled)
        if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_DONT_ACT, true)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_BERSERK, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_SLEEP, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_CHICKEN, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_FROG, true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_STOP, true)
                //|| IfStatusByUnitAndId(unitId, "morbol", true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_CONFUSION, true)
                )
        {
            return true;
        }
        return false;
    }

    public bool UnitCantResolveMovement(int unitId)
    {
        //unable to fight ones (dead, petrify, blood suck already handled)
        if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_BERSERK, true)
                //|| IfStatusByUnitAndId(unitId, "blood_suck", true)
                || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_CONFUSION, true)
                )
        {
            return true;
        }
        return false;
    }

    private void FuckingGolem(int effect, int unitId )
    {
        PlayerUnit actor = PlayerManager.Instance.GetPlayerUnit(unitId);
        int z1 = actor.StatTotalMaxLife; //effect;
        Debug.Log("z1 is " + z1);
        int z2 = actor.TeamId;
        //List<PlayerUnit> golemTargets;
        if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_CHARM,true)) //is charmed cast on other team
        {
            if (z2 == NameAll.TEAM_ID_GREEN)
            {
                this.redGolemHP = z1;
            }
            else
            {
                this.greenGolemHP = z1;
            }
        }
        else {
            //golem on opposite team
            //golemTargets = pul.getAllUnitsByTeamId(actor.getTeam_id(), false);
            //PlayerManager.Instance.GetAllUnitsByTeamId(actor.TeamId);
            if (z2 == NameAll.TEAM_ID_GREEN)
            {
                this.greenGolemHP = z1;
            }
            else
            {
                this.redGolemHP = z1;
            }
        }
        Debug.Log("golem green hp is" + this.greenGolemHP);
    }

    //called in calcresolveaction, tests if there is a golem active or not
    public bool IsGolem(int targetTeamId)
    {
        if (targetTeamId == NameAll.TEAM_ID_GREEN && greenGolemHP > 0)
        {
            return true;
        }
        else if( targetTeamId == NameAll.TEAM_ID_RED && redGolemHP > 0)
        {
            return true;
        }
        return false;
    }
    //called in calcresolveaction
    public int DecrementGolem(int effect, int targetTeamId, int targetId)
    {
        int z1 = 0;
        if (targetTeamId == NameAll.TEAM_ID_GREEN && greenGolemHP > 0)
        {
            if (effect >= greenGolemHP)
            {
                z1 = greenGolemHP;
                effect -= greenGolemHP;
                greenGolemHP = 0;
                RemoveGolemFromTeam(targetTeamId);
            }
            else
            {
                z1 = effect;
                greenGolemHP -= effect;
                effect = 0;
            }
        }
        else
        {
            if (effect >= redGolemHP)
            {
                z1 = redGolemHP;
                effect -= redGolemHP;
                redGolemHP = 0;
                RemoveGolemFromTeam(targetTeamId);
            }
            else
            {
                z1 = effect;
                redGolemHP -= effect;
                effect = 0;
            }
        }
        if (z1 != 0)
        {
            PlayerManager.Instance.ShowFloatingText(targetId, NameAll.FLOATING_TEXT_CUSTOM, "Golem Absorbs " + z1);
        }
        return effect;
    }

    private void AddGolemToTeam(int unitId)
    {
        int teamId = PlayerManager.Instance.GetPlayerUnit(unitId).GetCharmTeam();
        List<PlayerUnit> puList = PlayerManager.Instance.GetAllUnitsByTeamId(teamId);
        foreach (PlayerUnit pu in puList)
        {
            AddStatusToTickList(pu.TurnOrder, NameAll.STATUS_ID_GOLEM);
        }
    }

    private void RemoveGolemFromTeam(int teamId)
    {
        List<PlayerUnit> puList = PlayerManager.Instance.GetAllUnitsByTeamId(teamId);
        foreach( PlayerUnit pu in puList)
        {
            RemoveStatus(pu.TurnOrder, NameAll.STATUS_ID_GOLEM);
        }
    }

    private void CheckStatusAndRemoveSlowAction(int unitId, int statusId, bool add_unable_to_fight)
    {
        if (!IfStatusLastingBlocksStatus(unitId, statusId))
        {
            List<int> statusIdList = new List<int>(); //Debug.Log("addign status " + statusId);
            statusIdList.Add(NameAll.STATUS_ID_PERFORMING); statusIdList.Add(NameAll.STATUS_ID_CHARGING); statusIdList.Add(NameAll.STATUS_ID_DEFENDING);
            RemoveStatusList(unitId, statusIdList);
            AddStatusToTickList(unitId, statusId); 

            SpellManager.Instance.RemoveSpellSlowByUnitId(unitId);
            SpellManager.Instance.RemoveSpellReactionByUnitId(unitId);
            SpellManager.Instance.RemoveSpellMimeByUnitId(unitId);

            if (add_unable_to_fight)
            {
                PlayerManager.Instance.SetAbleToFight(unitId, false);
				if( PlayerManager.Instance.GetRenderMode() != NameAll.PP_RENDER_NONE)
				{
					CombatLogClass cll = new CombatLogClass("is unable to fight due to " + NameAll.GetStatusString(statusId), unitId, PlayerManager.Instance.GetRenderMode());
					cll.SendNotification();
				}
                //pul.get().setAbleToFight(unitId, false); //able to fight set to false ie unable
            }
            if (statusId == NameAll.STATUS_ID_PETRIFY)
            {
                RemoveStatus(unitId, NameAll.STATUS_ID_DEATH_SENTENCE, true);
                //RemoveStatus(unitId, "transparent", true);
            }
        }
    }

    //called in Should only be called from RemoveStatusAndOverrideOthers and its derivative CheckStatusAndRemoveSlowAction
    //PlayerManager part adds to status to the PlayerUnitObject so status can be visible (RPC called there too)
    //[PunRPC] 
    void AddStatusToTickList( int unitId, int statusId)
    {
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("AddStatusToTickList", PhotonTargets.Others, new object[] { unitId, statusId });
        //}

        StatusObject so = new StatusObject(unitId, statusId);
        sStatusTickList.Add(so); //Debug.Log("adding status " + statusId + " " + unitId);
        PlayerManager.Instance.AddToStatusList(so.GetUnitId(), so.GetStatusId()); //removal handled in RemoveStatus

    }

    public bool IfStatusLastingBlocksStatus(int unitId, int statusId)
    {
        //string zString = "block_" + status;
        int z1 = NameAll.GetStatusBlockId(statusId);
        foreach (StatusObject s in sStatusLastingList)
        {
            if (s.GetUnitId() == unitId)
            {
                if (s.GetStatusId() == z1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //adds dead to a unit
    //online: Other can call this in online (through PlayerUnit, though it shouldn't as long as the timing isn't shit). don't duplicate the RPC's that are sent
    public void AddDead(int unitId, SpellName sn=null, PlayerUnit actor=null, int rollResult = -1919, int rollChance = -1919, int combatLogSubType = -1919)
    {
        if (true) //PhotonNetwork.offlineMode || PhotonNetwork.isMasterClient
		{
            
            RemoveStatusTickByUnit(unitId, NameAll.STATUS_ID_DEAD); //doesn't Remove reraise; is an RPC
            SpellManager.Instance.RemoveSpellSlowByUnitId(unitId); //is an rpc
            SpellManager.Instance.RemoveSpellReactionByUnitId(unitId); //NOT an rpc but Other never sees these
            if (PlayerManager.Instance.GetPlayerUnit(unitId).ClassId == NameAll.CLASS_MIME)
                SpellManager.Instance.RemoveSpellMimeByUnitId(unitId); //NOT an rpc but Other never sees these

            PlayerManager.Instance.RemoveLife(0, 0, unitId, NameAll.ITEM_ELEMENTAL_NONE, true, sn: sn, actor:actor, rollResult:rollResult, rollChance:rollChance,
				combatLogSubType:combatLogSubType);//is RPC; removes remaining HP
            PlayerManager.Instance.AddToStatusList(unitId, NameAll.STATUS_ID_DEAD_3); //NOT RPC; special, normal dead done elsewhere
            PlayerManager.Instance.SetAbleToFight(unitId, false); //is an RPC

            AddStatusToTickList(unitId, NameAll.STATUS_ID_DEAD); //is an rpc, sent to other player. only have master call this

			//if (!PhotonNetwork.offlineMode)
			//{
			//    //tells P2 to not only do the dead animation but to do the 'Dead 3' icon above unit head
			//    PlayerManager.Instance.SendPlayerObjectAnimation(unitId, "dead", false);
			//}
			if (PlayerManager.Instance.GetRenderMode() != NameAll.PP_RENDER_NONE)
			{
				SoundManager.Instance.PlaySoundClip(3);
				PlayerManager.Instance.SetPlayerObjectAnimation(unitId, "dead", false); //NO LONGER AN RPC EACH UNIT CALLS THIS ITSELF
				CombatLogClass cll = new CombatLogClass("dies", unitId, PlayerManager.Instance.GetRenderMode());
				cll.SendNotification();
			}
        }
        
    }

    public void AddLife(int effect, int unitId)
    {
        RemoveStatus(unitId, NameAll.STATUS_ID_DEAD, true); //handles PlayerManager.Instance.RemoveFromStatusList(unitId, "dead");
        PlayerManager.Instance.AddLife(effect, unitId); //this handles critical status
        //Debug.Log("adding life to a unit " + effect);
        PlayerManager.Instance.RemoveFromStatusList(unitId, NameAll.STATUS_ID_DEAD_0);
        PlayerManager.Instance.RemoveFromStatusList(unitId, NameAll.STATUS_ID_DEAD_1);
        PlayerManager.Instance.RemoveFromStatusList(unitId, NameAll.STATUS_ID_DEAD_2);
        PlayerManager.Instance.RemoveFromStatusList(unitId, NameAll.STATUS_ID_DEAD_3);
        if(deathMode == NameAll.DEATH_MODE_UNCONSCIOUS)
            PlayerManager.Instance.RemoveFromStatusList(unitId, NameAll.STATUS_ID_UNCONSCIOUS);
        PlayerManager.Instance.SetPlayerObjectAnimation(unitId, "life", false);

        //if (!PhotonNetwork.offlineMode)
        //    PlayerManager.Instance.SendPlayerObjectAnimation(unitId, "life", false); //ALSO REMOVES TEH STATUS_ID_DEAD_X
    }

    //tells if turn should end (ie dead, petrified etc) or if AI should take over
    public int GetActiveTurnStatus(int actorId)
    {
        if( IfStatusByUnitAndId( actorId, NameAll.STATUS_ID_DEAD, false) || IfStatusByUnitAndId(actorId, NameAll.STATUS_ID_PETRIFY, false) )
        {
            return 1;
        }
        //add ai takes over: charm, confusion, berserk, chicken, blood suck
        return 0;
    }

    //called in playerunitlist, shows the statuses active in string form
    public void GetUnitStatusList(int unitId, List<string> statusList){
        foreach (StatusObject s in sStatusLastingList)
        {
            //Debug.Log("statuses to grab?");
            if (s.GetUnitId() == unitId)
            {
                statusList.Add( NameAll.GetStatusString(s.GetStatusId()) );
            }
        }
        foreach ( StatusObject s in sStatusTickList.ToList())
        {
            //Debug.Log("statuses to grab?");
            if( s.GetUnitId() == unitId)
            {
                statusList.Add(NameAll.GetStatusString(s.GetStatusId()));
            }  
        }
    }

    //called from playermanager where it is called at start
    //adds statuslasting that are a result of starting class or ability
    public void AddPUStatusLasting(int unitId, int abilityId, int abilitySlot)
    {
        List<StatusObject> soList = new List<StatusObject>();
        StatusObject so;
        if ( abilitySlot == NameAll.ABILITY_SLOT_PRIMARY)
        {
            if (abilityId == NameAll.CLASS_FIRE_MAGE) //block burn, half fire, fire up
            {
                so = new StatusObject(unitId, NameAll.STATUS_ID_STRENGTHEN_FIRE);
                soList.Add(so);
                so = new StatusObject(unitId, NameAll.STATUS_ID_HALF_FIRE);
                soList.Add(so);
                so = new StatusObject(unitId, NameAll.STATUS_ID_BLOCK_OIL);
                soList.Add(so);
            }
            else if( abilityId == NameAll.CLASS_HEALER)
            {
                so = new StatusObject(unitId, NameAll.STATUS_ID_STRENGTHEN_LIGHT);
                soList.Add(so);
                so = new StatusObject(unitId, NameAll.STATUS_ID_STRENGTHEN_UNDEAD);
                soList.Add(so);
            }
            
        }
        else if( abilitySlot == NameAll.ABILITY_SLOT_SUPPORT)
        {
            if (abilityId == NameAll.SUPPORT_FLAME_TOUCHED) //block burn, half fire, fire up
            {
                so = new StatusObject(unitId, NameAll.STATUS_ID_STRENGTHEN_FIRE);
                soList.Add(so);
                so = new StatusObject(unitId, NameAll.STATUS_ID_HALF_FIRE);
                soList.Add(so);
                so = new StatusObject(unitId, NameAll.STATUS_ID_BLOCK_OIL);
                soList.Add(so);
            }
            else if (abilityId == NameAll.SUPPORT_LIGHTS_BLESSING)
            {
                so = new StatusObject(unitId, NameAll.STATUS_ID_STRENGTHEN_LIGHT);
                soList.Add(so);
                so = new StatusObject(unitId, NameAll.STATUS_ID_STRENGTHEN_UNDEAD);
                soList.Add(so);
            }
        }
        else if (abilitySlot == NameAll.ABILITY_SLOT_MOVEMENT)
        {
            if (abilityId == NameAll.MOVEMENT_FLOAT) //block burn, half fire, fire up
            {
                so = new StatusObject(unitId, NameAll.STATUS_ID_FLOAT_MOVE);
                soList.Add(so);
            }
        }

        foreach ( StatusObject s in soList)
        {
            AddStatusLastingByStatusObject(s);
        }
    }

    //called in CalculationMod sees if target is under sadist boost effects (critical, undead, death sentence
    public bool IsSadist(int unitId)
    {
        if( IfStatusByUnitAndId(unitId,NameAll.STATUS_ID_CRITICAL) || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_DEATH_SENTENCE)
            || IsUndead(unitId) )
        {
            return true;
        }
        return false;
    }

    //called in calcmod and calc evasion
    public bool IsShowMust(int targetId, int statusId)
    {
        if(PlayerManager.Instance.IsAbilityEquipped(targetId, NameAll.SUPPORT_SHOW_MUST_GO_ON, NameAll.ABILITY_SLOT_SUPPORT))
        {
            if( statusId == NameAll.STATUS_ID_CHARGING || statusId == NameAll.STATUS_ID_PERFORMING)
            {
                return true;
            }
        }
        return false;
    }

	//does unit have active turn but already has an ability in action from last turn (ie performing/charging)
	public bool IsContinueAbility(PlayerUnit pu)
	{
		if (StatusManager.Instance.IfStatusByUnitAndId(pu.TurnOrder, NameAll.STATUS_ID_CHARGING, true)
			|| StatusManager.Instance.IfStatusByUnitAndId(pu.TurnOrder, NameAll.STATUS_ID_PERFORMING, true))
			return true;
		return false;
	}

    //called from playerunit to see if speed is bumped
    public bool IsPositiveStatus(int unitId)
    {
        if (IfStatusByUnitAndId(unitId,NameAll.STATUS_ID_HASTE) 
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_PROTECT)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_SHELL)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_REGEN)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_RERAISE)
            )
        {
            return true;
        }
        return false;
    }

    //is the unitId inflicted by a negative status
    public bool IsNegativeStatus(int unitId)
    {
        if (IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_POISON)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_SILENCE)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_DARKNESS)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_SLOW)
            || IsUndead(unitId)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_CONFUSION)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_SLEEP)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_STOP)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_CHICKEN)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_APATHY)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_PETRIFY)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_BERSERK)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_CHARM)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_FROG)
            //|| IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_CRITICAL) //eh not really what I want here
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_DEATH_SENTENCE)
            || IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_OIL) )
        {
            return true;
        }
        return false;
    }

    //is the status itself negative, if so return true
    public bool IsNegativeStatusId(int statusId)
    {
        if ( statusId == NameAll.STATUS_ID_POISON
            || statusId == NameAll.STATUS_ID_SILENCE
            || statusId == NameAll.STATUS_ID_DARKNESS
            || statusId == NameAll.STATUS_ID_SLOW
            || statusId == NameAll.STATUS_ID_UNDEAD
            || statusId == NameAll.STATUS_ID_CONFUSION
            || statusId == NameAll.STATUS_ID_SLEEP
            || statusId == NameAll.STATUS_ID_STOP
            || statusId == NameAll.STATUS_ID_PETRIFY
            || statusId == NameAll.STATUS_ID_BERSERK
            || statusId == NameAll.STATUS_ID_CHARM
            || statusId == NameAll.STATUS_ID_FROG
            || statusId == NameAll.STATUS_ID_DEATH_SENTENCE
            || statusId == NameAll.STATUS_ID_OIL)
        {
            return true;
        }
        return false;
    }

    public bool IsUnitPassable(int unitId)
    {
        //assumign just dead for now
        if(IfStatusByUnitAndId(unitId, NameAll.STATUS_ID_DEAD))
        {
            return true;
        }
        return false;
    }

    //on a status hit, interprets the added effect
    static int GetStatusFromNameless(int statusId)
    {
        int statusOut = 0;
        if (statusId == NameAll.STATUS_ID_NAMELESS_DANCE)
        {
            //one of the following is returned: Confusion, Silence, Frog, Poison, Slow, Stop, Sleep
            int z1 = UnityEngine.Random.Range(0, 7);
            if (z1 == 0)
            {
                //zString = "confusion";
                statusOut = NameAll.STATUS_ID_CONFUSION;
            }
            else if (z1 == 1)
            {
                //zString = "silence";
                statusOut = NameAll.STATUS_ID_SILENCE;
            }
            else if (z1 == 2)
            {
                //zString = "frog";
                statusOut = NameAll.STATUS_ID_FROG;
            }
            else if (z1 == 3)
            {
                //zString = "poison";
                statusOut = NameAll.STATUS_ID_POISON;
            }
            else if (z1 == 4)
            {
                //zString = "slow";
                statusOut = NameAll.STATUS_ID_SLOW;
            }
            else if (z1 == 5)
            {
                //zString = "stop";
                statusOut = NameAll.STATUS_ID_STOP;
            }
            else
            {
                //zString = "sleep";
                statusOut = NameAll.STATUS_ID_SLEEP;
            }
		}
        else if (statusId == NameAll.STATUS_ID_NAMELESS_SONG)
        {
            //reraise, Regen, Protect, Shell, Reflect
            int z1 = UnityEngine.Random.Range(0, 5);
            if (z1 == 0)
            {
                //zString = "reraise";
                statusOut = NameAll.STATUS_ID_RERAISE;
            }
            else if (z1 == 1)
            {
                //zString = "regen";
                statusOut = NameAll.STATUS_ID_REGEN;
            }
            else if (z1 == 2)
            {
                //zString = "protect";
                statusOut = NameAll.STATUS_ID_PROTECT;
            }
            else if (z1 == 3)
            {
                //zString = "shell";
                statusOut = NameAll.STATUS_ID_SHELL;
            }
            else
            {
                statusOut = NameAll.STATUS_ID_REFLECT;
                //zString = "reflect";
            }
        }
        else if (statusId == NameAll.STATUS_ID_ADD_POSITIVE)
        {
            int z1 = UnityEngine.Random.Range(0, 4);
            if (z1 == 0)
            {
                //zString = "reraise";
                statusOut = NameAll.STATUS_ID_RERAISE;
            }
            else if (z1 == 1)
            {
                //zString = "regen";
                statusOut = NameAll.STATUS_ID_REGEN;
            }
            else if (z1 == 2)
            {
                //zString = "protect";
                statusOut = NameAll.STATUS_ID_PROTECT;
            }
            else //(z1 == 3)
            {
                //zString = "shell";
                statusOut = NameAll.STATUS_ID_SHELL;
            }
            //else
            //{
            //    //zString = "shell";
            //    statusOut = NameAll.STATUS_ID_FAITH;
            //}

        }
        return statusOut;
    }


    public int GetDeathMode()
    {
        return this.deathMode;
    }

    public void SetDeathMode(int dMode)
    {
        this.deathMode = dMode;
    }

	//called in ResolveSlowAction. charge and jump do unique things to the PlayerUnits that cast them, thus remove the statuses here
	public void CheckChargingJumping(int actorId, SpellName sn)
	{
		if (sn.CommandSet == NameAll.COMMAND_SET_JUMP)
		{
			StatusManager.Instance.RemoveStatus(actorId, NameAll.STATUS_ID_JUMPING);
			PlayerManager.Instance.ToggleJumping(actorId, false);
		}
		else if (sn.CommandSet == NameAll.COMMAND_SET_CHARGE)
		{
			StatusManager.Instance.RemoveStatus(actorId, NameAll.STATUS_ID_CHARGING);
		}
	}

}