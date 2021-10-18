using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class SpellManager : Singleton<SpellManager>
{

    private static List<SpellSlow> sSpellSlowList;
    //private static List<SpellName> sSpellNameList;
    //private static List<SpellName> sSpellAttackList;
    private static List<SpellReaction> sSpellReactionList;
    private static List<SpellReaction> sSpellMimeList;
    private static List<SpellCommandSet> sSpellCommandSetList;
    private static List<SpellLearnedSet> sSpellLearnedList;

    public int greenElixir = 1;
    public int redElixir = 1;

    public int greenAsura = 1;
    public int greenBizen = 1;
    public int greenChiri = 1;
    public int greenHeaven = 1;
    public int greenKiku = 1;
    public int greenKiyo = 1;
    public int greenKoutetsu = 1;
    public int greenMasamune = 1;
    public int greenMuramasa = 1;
    public int greenMurasame = 1;

    public int redAsura = 1;
    public int redBizen = 1;
    public int redChiri = 1;
    public int redHeaven = 1;
    public int redKiku = 1;
    public int redKiyo = 1;
    public int redKoutetsu = 1;
    public int redMasamune = 1;
    public int redMuramasa = 1;
    public int redMurasame = 1;

    public int spellLearnedType;

    //PhotonView photonView;

    void Awake()
    {
        //photonView = PhotonView.Get(this);
        string filePath = Application.dataPath + "/Custom/CSV/SpellNameDataCSV.csv"; //Debug.Log(filePath);
        sSpellCommandSetList = GenerateSpellCommandSetList(filePath);
    }

    protected SpellManager()
    { // guarantee this will be always a singleton only - can't use the constructor!
        sSpellSlowList = new List<SpellSlow>();
        SpellName sn = new SpellName();
        //sSpellNameList = sn.GetAllSpellTypes();
        //sSpellAttackList = new List<SpellName>();
        sSpellReactionList = new List<SpellReaction>();
        sSpellMimeList = new List<SpellReaction>();
        sSpellCommandSetList = new List<SpellCommandSet>();
        sSpellLearnedList = new List<SpellLearnedSet>();
        spellLearnedType = NameAll.SPELL_LEARNED_TYPE_NONE;
        //CreateSpellAttackList();//the attack spells in sSpellAttackList
    }

    public SpellName GetSpellAttackByWeaponType(int weaponType, int classId)
    {
        int index = NameAll.GetSpellIndexByWeaponType(weaponType, classId); //Debug.Log(" " + index + " " + weaponType);
        //SpellName sn = GetSpellNameByIndex(index);
        //Debug.Log(" sn info is " + sn.Index + sn.GetSpellName());
        return GetSpellNameByIndex(index);
        //string zString = "attack_" + weaponType;
        //foreach ( SpellName sn in sSpellAttackList)
        //{
        //    if ( sn.GetSpellName().Equals(zString) )
        //    {
        //        return sn;
        //    }
        //}
        //Debug.Log("ERROR: did not return a spell name object for an attack");
        //return null;
    }

    public SpellName GetSpellAttackByWeaponId(int weaponId, int classId)
    {
        int weaponType = ItemManager.Instance.GetItemType(weaponId, NameAll.ITEM_SLOT_WEAPON);
        int index = NameAll.GetSpellIndexByWeaponType(weaponType, classId); //Debug.Log(" " + index + " " + weaponType);

        return GetSpellNameByIndex(index);
    }

    public List<SpellName> GetSpellNamesByCommandSet(int commandSet, PlayerUnit pu, int mathSkillOnly = 0)
    {
        Debug.Log("get spellname list from scriptable objects");

        int teamId = pu.TeamId;
        List<SpellName> retList = new List<SpellName>();

        //in certain modes, load the spell from list of available spells. else grab the entire command set
        if (spellLearnedType == NameAll.SPELL_LEARNED_TYPE_PLAYER_1 && teamId == NameAll.TEAM_ID_GREEN)
        {
            IEnumerable<SpellLearnedSet> enumerable = sSpellLearnedList.Where(s => s.TurnOrder == pu.TurnOrder && s.ClassId == commandSet );
            List<SpellLearnedSet> asList = enumerable.ToList(); //Debug.Log("asdf " + asList.Count);
            for (int i = 0; i < asList.Count; i++)
            {
                retList.Add(GetSpellNameByIndex(asList[i].SpellIndex)); //Debug.Log( "SpellName, CommandSet, Id " + retList[i].AbilityName + "," + retList[i].CommandSet + "," + retList[i].SpellId);
            }
        }
        else
        {
            IEnumerable<SpellCommandSet> enumerable = sSpellCommandSetList.Where(scs => scs.CommandSet == commandSet);
            List<SpellCommandSet> asList = enumerable.ToList(); //Debug.Log("asdf " + asList.Count);
            for (int i = 0; i < asList.Count; i++)
            {
                retList.Add(GetSpellNameByIndex(asList[i].SpellIndex)); //Debug.Log( "SpellName, CommandSet, Id " + retList[i].AbilityName + "," + retList[i].CommandSet + "," + retList[i].SpellId);
            }
        }


        if (NameAll.IsClassicClass(pu.ClassId))
        {
            //if frog and command set is black_magic edit
            if (commandSet == NameAll.COMMAND_SET_ELEMENTAL)
            {
                retList.Clear();
                retList.Add(GetSpellNameByIndex(NameAll.SPELL_INDEX_HELL_IVY));
                
            }
            else if (commandSet == NameAll.COMMAND_SET_DRAW_OUT)
            {
                if (teamId == NameAll.TEAM_ID_GREEN)
                {
                    RemoveDrawOut(retList, greenAsura, NameAll.SPELL_INDEX_ASURA);
                    RemoveDrawOut(retList, greenBizen, NameAll.SPELL_INDEX_BIZEN_BOAT);
                    RemoveDrawOut(retList, greenChiri, NameAll.SPELL_INDEX_CHIRIJIRADEN);
                    RemoveDrawOut(retList, greenHeaven, NameAll.SPELL_INDEX_HEAVENS_CLOUD);
                    RemoveDrawOut(retList, greenKiku, NameAll.SPELL_INDEX_KIKUICHIMOJI);
                    RemoveDrawOut(retList, greenKiyo, NameAll.SPELL_INDEX_KIYOMORI);
                    RemoveDrawOut(retList, greenKoutetsu, NameAll.SPELL_INDEX_KOUTETSU);
                    RemoveDrawOut(retList, greenMasamune, NameAll.SPELL_INDEX_MASAMUNE);
                    RemoveDrawOut(retList, greenMuramasa, NameAll.SPELL_INDEX_MURAMASA);
                    RemoveDrawOut(retList, greenMurasame, NameAll.SPELL_INDEX_MURASAME);
                }
                else
                {
                    RemoveDrawOut(retList, redAsura, NameAll.SPELL_INDEX_ASURA);
                    RemoveDrawOut(retList, redBizen, NameAll.SPELL_INDEX_BIZEN_BOAT);
                    RemoveDrawOut(retList, redChiri, NameAll.SPELL_INDEX_CHIRIJIRADEN);
                    RemoveDrawOut(retList, redHeaven, NameAll.SPELL_INDEX_HEAVENS_CLOUD);
                    RemoveDrawOut(retList, redKiku, NameAll.SPELL_INDEX_KIKUICHIMOJI);
                    RemoveDrawOut(retList, redKiyo, NameAll.SPELL_INDEX_KIYOMORI);
                    RemoveDrawOut(retList, redKoutetsu, NameAll.SPELL_INDEX_KOUTETSU);
                    RemoveDrawOut(retList, redMasamune, NameAll.SPELL_INDEX_MASAMUNE);
                    RemoveDrawOut(retList, redMuramasa, NameAll.SPELL_INDEX_MURAMASA);
                    RemoveDrawOut(retList, redMurasame, NameAll.SPELL_INDEX_MURASAME);
                }
            }
            else if (commandSet == NameAll.COMMAND_SET_ITEM)
            {
                //check for elixirs
                if ((teamId == NameAll.TEAM_ID_GREEN && greenElixir <= 0) || (teamId == NameAll.TEAM_ID_RED && redElixir <= 0))
                {
                    retList.Remove(GetSpellNameByIndex(NameAll.SPELL_INDEX_ELIXIR));
                }
            }
            else if (StatusManager.Instance.IfStatusByUnitAndId(pu.TurnOrder, NameAll.STATUS_ID_FROG, true))
            {
                retList.Clear();
                if (commandSet == NameAll.COMMAND_SET_BLACK_MAGIC)
                {
                    retList.Add(GetSpellNameByIndex(NameAll.FROG_SPELL_SPELL_INDEX));
                }
                return retList;
            }

            if (mathSkillOnly == 1)
            {

                foreach (SpellName sn in retList.ToList())
                {
                    if (!sn.IsCalculate())
                    {
                        retList.Remove(sn);
                    }
                }
            }
        }
        return retList;
        
    }

    void RemoveDrawOut( List<SpellName> snList, int number, int spellIndex)
    {
        if( number <= 0)
        {
            snList.Remove(GetSpellNameByIndex(spellIndex));
        }
    }

	//just getting the spellNames for the command set
	public List<SpellName> GetSpellNamesByCommandSet(int commandSet)
	{
		//Debug.Log("get spellname list from scriptable objects");
		List<SpellName> retList = new List<SpellName>();
		IEnumerable<SpellCommandSet> enumerable = sSpellCommandSetList.Where(scs => scs.CommandSet == commandSet);
		List<SpellCommandSet> asList = enumerable.ToList(); //Debug.Log("asdf " + asList.Count);
		for (int i = 0; i < asList.Count; i++)
		{
			//Debug.Log("CommandSet, Id " + asList[i].CommandSet + "," + asList[i].SpellIndex);
			retList.Add(GetSpellNameByIndex(asList[i].SpellIndex)); //Debug.Log( "SpellName, CommandSet, Id " + retList[i].AbilityName + "," + retList[i].CommandSet + "," + retList[i].SpellId);
		}

		return retList;
	}

	//used for testing
	//public List<SpellName> GetSpellNamesAttack()
	//{
	//    return sSpellAttackList;
	//}

	//private void CreateSpellAttackList()
	//{
	//    //Debug.Log("need to create new spellAttackList");
	//    IEnumerable<SpellCommandSet> enumerable = sSpellCommandSetList.Where(scs => scs.CommandSet == 0);   //sSpellNameList.Where(sn => sn.CommandSet == commandSet);
	//    List<SpellCommandSet> asList = enumerable.ToList();
	//    foreach( SpellCommandSet scs in asList)
	//    {
	//        SpellName sn = GetSpellNameByIndex(scs.SpellIndex);
	//        sSpellAttackList.Add(sn);
	//    }

	//    //foreach (SpellName sn in sSpellNameList.ToList())
	//    //{
	//    //    if (sn.CommandSet == 0)
	//    //    {
	//    //        //Debug.Log("Creating spellAttackList, item added" + sn.GetSpellName());
	//    //        sSpellAttackList.Add(sn);
	//    //        //sSpellNameList.Remove(sn);
	//    //    }
	//    //}
	//    //Debug.Log("Finished Creating spellAttackList ");
	//}

	#region SpellSlow
	//used for creating turns menu
	public List<SpellSlow> GetSpellSlowList()
    {
        return sSpellSlowList;
    }

   // [PunRPC]
    public void SlowActionTickPhase()
    {
        foreach (SpellSlow s in sSpellSlowList)
        {
            s.DecrementCtrOnly();
        }
        //if( !PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("SlowActionTickPhase",PhotonTargets.Others);
        //}
    }

	//gets next slow action but doesn't remove it, remove it with RemoveSpellSlowByObject
	public SpellSlow GetNextSlowAction()
    {
        int z1 = 9999;
        int zUniqueId = 999999; //each spell has a unique id, in case of ties by same unit use this
        SpellSlow s1 = null;
        foreach (SpellSlow ss in sSpellSlowList)
        {
			//Debug.Log("ss CTR is " + ss.CTR);
            if (ss.CTR == 0)
            {
                if (ss.UnitId <= z1) //get the one with the lowest Turn_order
                {
                    if( ss.UnitId == z1)
                    {
                        if( ss.UniqueId < zUniqueId ) //same unit, break the tiebreakder
                        {
                            z1 = ss.UnitId;
                            zUniqueId = ss.UniqueId;
                            s1 = ss;
                        }
                    }
                    else
                    {
                        z1 = ss.UnitId;
                        zUniqueId = ss.UniqueId;
                        s1 = ss;
                    }
                }
            }
        }
        return s1;
    }

    //called in SlowActionState, lets P2 know what spellslow to queue up so prespell shit can be shown
    //also has P2 remove the spellSlow from its queue
    public void SendSpellSlowPreCast(int uniqueId)
    {
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //photonView.RPC("ReceiveSpellSlowPreCast", PhotonTargets.Others, new object[] { uniqueId });
    }

    const string MultiplayerSpellSlow = "Multiplayer.SpellSlow";

    //[PunRPC]
    public void ReceiveSpellSlowPreCast(int uniqueId)
    {
        foreach (SpellSlow s in sSpellSlowList.ToList())
        {
            if (s.UniqueId == uniqueId)
            {
                this.PostNotification(MultiplayerSpellSlow, s); //Other receives this in GameLoopState
                sSpellSlowList.Remove(s);
                break;
            }
        }
    }

    
    //offline: removes spellslow after it's cast
    //DEPRECATED: online: sends RPC from Master to Other to remove Other's spellslow (other never casts the spell, but has it for TurnsManager)
    public void RemoveSpellSlowByObject(SpellSlow ss)
    {
		Debug.Log("spellslow list count pre" + sSpellSlowList.Count);
        sSpellSlowList.Remove(ss);
		Debug.Log("spellslow list count post" + sSpellSlowList.Count);
		//if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
		//{
		//    photonView.RPC("RemoveSpellSlowByObjectRPC", PhotonTargets.Others, new object[] { ss.GetUniqueId() });
		//}
	}

    //now removes spellslow on precast info being sent
    //received by Other, removes the spell (other never casts the spell, but has it for TurnsManager)
    //[PunRPC]
    //public void RemoveSpellSlowByObjectRPC(int uniqueId)
    //{
    //    foreach (SpellSlow s in sSpellSlowList.ToList())
    //    {
    //        if (s.GetUniqueId() == uniqueId)
    //        {
    //            sSpellSlowList.Remove(s);
    //            break;
    //        }
    //    }
    //}

    //[PunRPC]
    public void RemoveSpellSlowByUnitId(int unitId)
    {
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("RemoveSpellSlowByUnitId", PhotonTargets.Others, new object[] { unitId });
        //}

        foreach (SpellSlow s in sSpellSlowList.ToList())
        {
            if (s.UnitId == unitId)
            {
                sSpellSlowList.Remove(s);
            }
        }
    }



    //adds spell slow to list
    //online: sends message to P2 to add spellslow. can't serialize spell slow so send the info over then add it
    private void AddSpellSlow(SpellSlow ss)
    {
		//Debug.Log("new spell CTR is " + ss.CTR);
        sSpellSlowList.Add(ss);
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("AddSpellSlowRPC", PhotonTargets.Others,
        //        new object[] { ss.CTR, ss.UnitId, ss.SpellIndex, ss.TargetUnitId, ss.TargetX, ss.TargetY, ss.UniqueId });
        //}
    }

    //[PunRPC]
    private void AddSpellSlowRPC(int zCtr, int zUnitId, int zSpellIndex, int zTargetUnitId, int zTargetX, int zTargetY, int zUniqueId)
    {
        SpellSlow ss = new SpellSlow(zCtr, zUnitId, zSpellIndex, zTargetUnitId, zTargetX, zTargetY, zUniqueId);
        sSpellSlowList.Add(ss);
    }

    //[PunRPC] //don't think I need to pun it since it's only called from inside a PM pun ablity
    public bool RemoveArcherCharge(int unitId)
    {
        bool isRemoved = false;
        foreach (SpellSlow s in sSpellSlowList.ToList())
        { 
            if (s.UnitId == unitId )
            {
                SpellName sn = GetSpellNameByIndex(s.SpellIndex);
                if( sn.CommandSet == NameAll.COMMAND_SET_CHARGE)
                {
                    sSpellSlowList.Remove(s);
                    return true;
                }
            }
        }
        //if( isRemoved)
        //{

        //}
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("RemoveArcherCharge", PhotonTargets.Others, new object[] { unitId });
        //}
        return isRemoved;
    }


    //public void RemoveSpellSlowSafety()
    //{
    //    List<SpellSlow> temp = new List<SpellSlow>();
    //    foreach (SpellSlow ss in sSpellSlowList.ToList())
    //    {
    //        if (ss.GetCtr() <= 0)
    //        {
    //            //temp.Add(ss);
    //            sSpellSlowList.Remove(ss);
    //        }
    //    }
    //}

    #endregion

    #region SpellReaction
    //online: no need to RPC SpellReations. Don't show up on turns, don't do anything locally for Other

    //called in game loop, gets the next spell reaction for casting
    public SpellReaction GetNextSpellReaction()
    {
        int z1 = 9999; //tie breaker
        SpellReaction s1 = null;
        foreach (SpellReaction sr in sSpellReactionList)
        {
            if (sr.ActorId < z1) //get the one with the lowest Turn_order
            {
                z1 = sr.ActorId;
                s1 = sr;
            }
        }
        return s1;
    }
    

    //called in game loop
    public void RemoveSpellReactionByObject(SpellReaction sr)
    {
        PlayerManager.Instance.AlterReactionFlag(sr.ActorId, false);
        RemoveSpellReaction(sr);
    }

    //called when a unit dies (and other status). Called locally so don't need to RPC (other should never have anything in this queue)
    public void RemoveSpellReactionByUnitId(int unitId)
    {
        foreach (SpellReaction sr in sSpellReactionList.ToList())
        {
            if (sr.ActorId == unitId)
            {
                RemoveSpellReaction(sr);
            }
        }
    }

    //called after two swords in case two reactions are created, removes the 2nd one created
    //public void RemoveDoubleSpellReaction(int unitId)
    //{
    //    int z1 = 0;
    //    foreach (SpellReaction sr in sSpellReactionList.ToList())
    //    {
    //        if (sr.GetActorId() == unitId)
    //        {
    //            z1 += 1;
    //            if (z1 == 2)
    //            {
    //                RemoveSpellReaction(sr);
    //                break;
    //            }
    //        }
    //    }
    //}


    public void AddSpellReaction(SpellReaction sr)
    {
        sSpellReactionList.Add(sr);
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("AddSpellReactionRPC", PhotonTargets.Others, new object[] { sr.ActorId, sr.SpellIndex, sr.TargetId, sr.Effect });
        //}
    }

    //no need to have it for Other in online game
    //[PunRPC]
    //public void AddSpellReactionRPC(int unitId, int spellIndex, int targetUnitId, int effect) //SpellReaction(int zUnitId, int zSpellIndex, int zTargetUnitId, int zEffect = 0)
    //{
    //    SpellReaction sr = new SpellReaction(unitId, spellIndex, targetUnitId, effect);
    //    sSpellReactionList.Add(sr);
    //}

    public void RemoveSpellReaction(SpellReaction sr)
    {
        sSpellReactionList.Remove(sr); //Debug.Log("removing spell reaction " + sSpellReactionList.Count);
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("RemoveSpellReactionRPC", PhotonTargets.Others, new object[] { sr.ActorId, sr.SpellIndex, sr.TargetId, sr.Effect });
        //}
    }

    //[PunRPC]
    //public void RemoveSpellReactionRPC(int unitId, int spellIndex, int targetUnitId, int effect) //SpellReaction(int zUnitId, int zSpellIndex, int zTargetUnitId, int zEffect = 0)
    //{
    //    SpellReaction sr = new SpellReaction(unitId, spellIndex, targetUnitId, effect);
    //    sSpellReactionList.Remove(sr);
    //}

    public bool IsSpellReaction()
    {
        //Debug.Log("checking for spellreaction " + sSpellReactionList.Count);
        if( sSpellReactionList.Count > 0)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region MimeQueue
    //online: no need to add to queue for P2. Not in turns, not interactions done (though if Quick behaves in a funky way might want to reconsider this)

    //called in game loop, gets the next mime queue for casting
    public SpellReaction GetNextMimeQueue()
    {
        int z1 = 9999; //tie breaker
        SpellReaction s1 = null; 
        foreach (SpellReaction sr in sSpellMimeList)
        {
            if (sr.ActorId < z1) //get the one with the lowest Turn_order
            {
                z1 = sr.ActorId;
                s1 = sr;
            }
        }
        return s1;
    }

    //called in game loop
    public void RemoveMimeQueueByObject(SpellReaction sr)
    {
        RemoveMimeQueue(sr);
    }

    //called when a unit dies
    public void RemoveMimeQueueByUnit(int unitId)
    {
        foreach (SpellReaction sr in sSpellMimeList.ToList())
        {
            if (sr.ActorId == unitId)
            {
                RemoveMimeQueue(sr);
            }
        }
    }

    public void AddMimeQueue(SpellReaction sr)
    {
        sSpellMimeList.Add(sr);
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("AddMimeQueueRPC", PhotonTargets.Others, new object[] { sr.ActorId, sr.SpellIndex, sr.TargetId, sr.Effect });
        //}
    }

    //[PunRPC]
    //public void AddMimeQueueRPC(int unitId, int spellIndex, int targetUnitId, int effect) //SpellReaction(int zUnitId, int zSpellIndex, int zTargetUnitId, int zEffect = 0)
    //{
    //    SpellReaction sr = new SpellReaction(unitId, spellIndex, targetUnitId, effect);
    //    sSpellMimeList.Add(sr);
    //}

    public void RemoveMimeQueue(SpellReaction sr)
    {
        sSpellMimeList.Remove(sr);
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("RemoveMimeQueueRPC", PhotonTargets.Others, new object[] { sr.ActorId, sr.SpellIndex, sr.TargetId, sr.Effect });
        //}
    }

    //[PunRPC]
    //public void RemoveMimeQueueRPC(int unitId, int spellIndex, int targetUnitId, int effect) //SpellReaction(int zUnitId, int zSpellIndex, int zTargetUnitId, int zEffect = 0)
    //{
    //    SpellReaction sr = new SpellReaction(unitId, spellIndex, targetUnitId, effect);
    //    sSpellMimeList.Remove(sr);
    //}

    public bool IsMimeQueue()
    {
        if (sSpellMimeList.Count > 0)
        {
            return true;
        }
        return false;
    }

    //called when a unit dies (and other status). Called locally so don't need to RPC (other should never have anything in this queue)
    public void RemoveSpellMimeByUnitId(int unitId)
    {
        foreach (SpellReaction sr in sSpellMimeList.ToList())
        {
            if (sr.ActorId == unitId)
                sSpellMimeList.Remove(sr);
        }
    }
    #endregion

    public int GetSpellElementalByIndex(int index)
    {
        return GetSpellNameByIndex(index).ElementType;
    }

    public SpellName GetSpellNameByIndex(int index)
    {
        //Debug.Log("index is " + index);
        if( index < 10000) 
        {
            //Debug.Log("testing, loading this now...");
            SpellNameData snd = Resources.Load<SpellNameData>("SpellNames/sn_" + index);
			if (snd == null)
			{
				Debug.Log("ERROR snd is null...");
			}
			SpellName sn = new SpellName(snd); //Debug.Log("loading sn " + snd.Index + "," + sn.Index +"," + index);
            snd = null;
            Resources.UnloadAsset(snd);
            return sn;
        }
        else //custom created spellNames
        {
            SpellName sn;
            string fileName = Application.dataPath + "/Custom/Spells/" + index + "_sn.dat";
            if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
            {
                sn = Serializer.Load<SpellName>(fileName);
            }
            else
            {
                Debug.Log("Error, spellName not found");
                return null;
            }

            return sn;
        }
        
    //    ItemObject GetItemDataObject(int itemId)
    //{

    //        ItemData id = Resources.Load<ItemData>("Items/item_" + itemId);
    //        //Debug.Log("in item data id object is" + id.item_name + "asdf" + itemId);
    //        ItemObject io = new ItemObject(id);
    //        //Debug.Log("in item data is object is" + io.GetItemName() + "asdf" + io.GetStatusName());
    //        id = null;
    //        Resources.UnloadAsset(id);// Resources.UnloadUnusedAssets(); //not sure which of these to call
    //        return io;
    //    }

        //Debug.Log("get spellNames from Scriptable objects");
        //return null;
        //return sSpellNameList[index];
        //SpellName sn = new SpellName();
        //try
        //{
        //    sn = sSpellNameList[index];
        //    return sn;
        //}
        //catch (ArgumentOutOfRangeException e)
        //{
        //    Debug.Log("SpellManager " + e.ToString());
        //}
        //return null;
    }

    //creates the spellslow and adds it to the queue
    public void CreateSpellSlow(CombatTurn turn, bool isWAMode = false)
    {
		PlayerManager.Instance.AddCombatLogSaveObject(NameAll.COMBAT_LOG_TYPE_ACTION, NameAll.COMBAT_LOG_SUBTYPE_SPELL_SLOW_ADD, turn);
        //SpellSlow(int zCtr, int zUnitId, int zSpellIndex, int zTargetUnitId, int zMapTileId)
        SpellSlow ss = new SpellSlow(turn);
		if (isWAMode)
			PlayerManager.Instance.AddWalkAroundSpellSlow(ss);
		else
			AddSpellSlow(ss);
        //Debug.Log("something is fucked up about the targetting. unitId: " + turn.targetUnitId + " x,y " + turn.targetTile.pos.x + "," + turn.targetTile.pos.y);
        //Debug.Log("something is fucked up about the targetting. unitId: " + ss.TargetUnitId + " x,y " + ss.TargetX + "," + ss.TargetY);

        if (turn.spellName.CommandSet == NameAll.COMMAND_SET_SING || turn.spellName.CommandSet == NameAll.COMMAND_SET_DANCE
            || turn.spellName.CommandSet == NameAll.COMMAND_SET_ARTS && (turn.spellName.CTR % 10) == 1)
        {
            StatusManager.Instance.AddStatusAndOverrideOthers(0, turn.actor.TurnOrder, NameAll.STATUS_ID_PERFORMING);
        }
        else if (turn.spellName.CommandSet == NameAll.COMMAND_SET_JUMP)
        {
            //add jump status
            StatusManager.Instance.AddStatusAndOverrideOthers(0, turn.actor.TurnOrder, NameAll.STATUS_ID_JUMPING);
            //PlayerManager.Instance.SetFacingDirectionMidTurn(SceneCreate.active_unit, pu.GetMap_tile_index(), mapTileIndex);
        }
        else
        {
            StatusManager.Instance.AddStatusAndOverrideOthers(0, turn.actor.TurnOrder, NameAll.STATUS_ID_CHARGING);
        }
    }

    //only called for performing spells, no need to redo the performing status
    public void CreateSpellSlow(SpellSlow ss)
    {
        //SpellSlow(int zCtr, int zUnitId, int zSpellIndex, int zTargetUnitId, int zMapTileId)
        SpellSlow ss2 = new SpellSlow(ss);
        AddSpellSlow(ss2);
    }

    //called in UIActiveTurnMenu to tell if it's the targetting should be confirm/cancel or target unit/map/cancel
    public bool IsTargetUnitMap(SpellName sn, int targetId)
    {
        //turn.spellName.RangeXYMin != NameAll.SPELL_RANGE_MIN_SELF_TARGET
        //&& !StatusManager.Instance.IfStatusByUnitAndId(currentTile.UnitId, NameAll.STATUS_ID_JUMPING )) {
        //can't unit target a jumping unit

        if ( sn.CTR > 0)
        {
            if (sn.RangeXYMin == NameAll.SPELL_RANGE_MIN_SELF_TARGET || StatusManager.Instance.IfStatusByUnitAndId(targetId, NameAll.STATUS_ID_JUMPING))
                return false;

            if( sn.Version == NameAll.VERSION_CLASSIC)
            {
                if (sn.CommandSet != NameAll.COMMAND_SET_JUMP && sn.CommandSet != NameAll.COMMAND_SET_CHARGE)
                    return true;
                else
                    return false;
            }
            else
            {
                //Debug.Log(" testing " + sn.RangeXYMin + " " + sn.EffectXY);
                if( sn.EffectXY < 100 && sn.RangeXYMin < 100 ) //effect: cones, line attacks etc are not target unit; range: weapon based attacks cannot target unit, only the map
                    return true;
            }
        }
        return false;
    }

	//called from PlayerManager during WAAO, tells if spell can still target that square
	public bool IsSpellTargetable(WalkAroundActionObject waao)
	{
		//in WAAO if unit does ability then moves
			//all slow actions are fine
			//target all allies/enemies are fine
			//instants with range min/max is not fine if out of that range
			//direction stuff should still just go in that direction
		if( waao.turn.spellName.CTR == 0)
		{
			//either no move or no targetTile (don't target unit's on instant) then true
			if(waao.turn.walkAroundMoveTile == null || waao.turn.targetTile == null)
				return true;

			return IsAbilityInRange(waao.turn.spellName, waao.turn.walkAroundMoveTile, waao.turn.targetTile);
		}

		return true;
	}

	bool IsAbilityInRange(SpellName sn, Tile t1, Tile t2)
	{
		int range = Math.Abs(t1.pos.x - t2.pos.x) + Math.Abs(t1.pos.y - t2.pos.y);
		int height = Math.Abs(t1.height - t2.height);
		if( range >= sn.RangeXYMin && range <= sn.RangeXYMax)
		{
			if( height <= sn.RangeZ)
			{
				return true;
			}
		}
		return false;
	}

    //creates the spellslow and adds it to the queue
    //public void CreateSpellSlow(int ctr, int actorId, int spellIndex, int targetId, int mapTileIndex)
    //{
    //    //SpellSlow(int zCtr, int zUnitId, int zSpellIndex, int zTargetUnitId, int zMapTileId)
    //    SpellSlow ss = new SpellSlow(ctr, actorId, spellIndex, targetId, mapTileIndex);
    //    AddSpellSlow(ss);
    //}

    //called in game loop off of a perfoming spell
    //public void CreateSpellSlow(int ctr, SpellSlow ssOld)
    //{
    //    //SpellSlow(int zCtr, int zUnitId, int zSpellIndex, int zTargetUnitId, int zMapTileId)
    //    SpellSlow ss = new SpellSlow(ctr, ssOld);
    //    AddSpellSlow(ss);
    //}

    //public void SetBaseQForAttack(int spellIndex, int z1)
    //{
    //    sSpellNameList[spellIndex].SetBaseQForAttack(z1);
    //}

    //public void SetRangeXYMaxForThrow(int spellIndex, int z1)
    //{
    //    sSpellNameList[spellIndex].SetRangeXYMaxForThrow(z1);
    //}

    //public void SetRangeXYMinForBattleSkill(int spellIndex, int z1)
    //{
    //    sSpellNameList[spellIndex].SetRangeXYMinForBattleSkill(z1);
    //}

    //public void SetCtrForJump(int spellIndex, int z1)
    //{
    //    sSpellNameList[spellIndex].SetCtrForJump(z1);
    //}

    //called in calculation AT for math skill
    //public void ModifySpellForMathSkill(int mathSpellIndex, int otherSpellIndex)
    //{
    //    GetSpellNameByIndex(mathSpellIndex).ModifySpellForMathSkill(GetSpellNameByIndex(otherSpellIndex));
    //}

    //public SpellName GetRandomSpellName(int unitId)
    //{
    //    List<SpellName> tempList = new List<SpellName>();
    //    List<int> tempInts = new List<int>();
    //    tempInts.Add(CalculationAT.GetAttackSpellIndex(unitId));
    //    //frog should be handled in here
    //    tempList = GetSpellNamesByCommandSet(PlayerManager.Instance.GetPlayerUnit(unitId).ClassId, PlayerManager.Instance.GetPlayerUnit(unitId));
    //    foreach( SpellName sn in tempList)
    //    {
    //        tempInts.Add(sn.Index);
    //    }
    //    tempList = GetSpellNamesByCommandSet(PlayerManager.Instance.GetPlayerUnit(unitId).AbilitySecondaryCode, PlayerManager.Instance.GetPlayerUnit(unitId));
    //    foreach (SpellName sn in tempList)
    //    {
    //        tempInts.Add(sn.Index);
    //    }

    //    int z1 = UnityEngine.Random.Range(0, tempInts.Count);
    //    return GetSpellNameByIndex(tempInts[z1]);
        
    //}

    public void DecrementInventory(SpellName sn, int teamId)
    {
        if(sn.CommandSet == NameAll.COMMAND_SET_DRAW_OUT)
        {
            DecrementDrawOut(sn, teamId);
        }
        else if( sn.SpellId == NameAll.SPELL_INDEX_ELIXIR)
        {
            if( teamId == NameAll.TEAM_ID_GREEN)
            {
                greenElixir -= 1;
            }
            else
            {
                redElixir -= 1;
            }
        }
    }

    void DecrementDrawOut(SpellName sn, int teamId)
    {
        if( teamId == NameAll.TEAM_ID_GREEN)
        {
            if( sn.SpellId == NameAll.SPELL_INDEX_ASURA)
            {
                greenAsura -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_BIZEN_BOAT )
            {
                greenBizen -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_CHIRIJIRADEN)
            {
                greenChiri -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_HEAVENS_CLOUD)
            {
                greenHeaven -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_KIKUICHIMOJI)
            {
                greenKiku -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_KIYOMORI)
            {
                greenKiyo -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_KOUTETSU)
            {
                greenKoutetsu -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_MASAMUNE)
            {
                greenMasamune -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_MURAMASA)
            {
                greenMuramasa -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_MURASAME)
            {
                greenMurasame -= 1;
            }
        }
        else
        {
            if (sn.SpellId == NameAll.SPELL_INDEX_ASURA)
            {
                redAsura -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_BIZEN_BOAT)
            {
                redBizen -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_CHIRIJIRADEN)
            {
                redChiri -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_HEAVENS_CLOUD)
            {
                redHeaven -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_KIKUICHIMOJI)
            {
                redKiku -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_KIYOMORI)
            {
                redKiyo -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_KOUTETSU)
            {
                redKoutetsu -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_MASAMUNE)
            {
                redMasamune -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_MURAMASA)
            {
                redMuramasa -= 1;
            }
            else if (sn.SpellId == NameAll.SPELL_INDEX_MURASAME)
            {
                redMurasame -= 1;
            }
        }
    }

    //reads the SpellNameDataCSV to create shorthand list of abilities and their command sets, used with LINQ to quickly get the abilities for ActiveTurns
    List<SpellCommandSet> GenerateSpellCommandSetList( string filePath)
    {
        string[] readText = File.ReadAllLines(filePath);
        //string[] readText = File.ReadAllLines("CSV/SpellNameDataCSV.csv"); //File.ReadAllLines("Assets/Settings/SpellNameDataCSV.csv");
        //filePath = "Assets/Resources/Abilities/";
        var retValue = new List<SpellCommandSet>();
        for (int i = 1; i < readText.Length; ++i)
        {
            //not sure whether it's faster to just do the scriptable object or to read the csv
            var line = readText[i];
            var values = line.Split(',');
            SpellCommandSet sc = new SpellCommandSet(Int32.Parse(values[0]), Int32.Parse(values[4]));
            retValue.Add(sc); //Debug.Log("SC data " + sc.SpellIndex + "," + sc.CommandSet);

            //AbilityData abilityData = ScriptableObject.CreateInstance<AbilityData>();
            //abilityData.Load(readText[i]);
            //string fileName = string.Format("{0}{1}.asset", filePath, "ability_" + abilityData.slot + "_" + abilityData.slotId);
            //AssetDatabase.CreateAsset(abilityData, fileName);
        }
        return retValue;
    }

    public void GenerateSpellNamesByCustomCommandSet(int commandSet)
    {
        IEnumerable<SpellCommandSet> enumerable = sSpellCommandSetList.Where(scs => scs.CommandSet == commandSet);
        if (enumerable.Count() > 0) //already been loaded
            return;
        else
        {
            var snList = CalcCode.LoadCustomSpellNameList();
            foreach(SpellName sn in snList)
            {
                if( sn.CommandSet == commandSet)
                {
                    SpellCommandSet scs = new SpellCommandSet(sn.Index, commandSet);
                    sSpellCommandSetList.Add(scs);
                }

            }
        }
    }

    //used in ai, checks if spell is helpful/harmful (in general)
    public bool IsSpellPositive(SpellName sn)
    {
        //short hand, just using it for now
        if(sn.RemoveStat == NameAll.REMOVE_STAT_HEAL)
        {
            return true;
        }

        return false;
    }

    //online: sends reaction details from Master to Other. Other receives and does notification to display reaction details info
    public void SendReactionDetails(int actorId, int spellIndex, int targetX, int targetY, string displayName)
    {
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //    photonView.RPC("ReceiveReactionDetails",PhotonTargets.Others,new object[] { actorId, spellIndex, targetX, targetY, displayName });
    }

    const string MultiplayerReaction = "Multiplayer.Reaction";

    //[PunRPC]
    public void ReceiveReactionDetails(int actorId, int spellIndex, int targetX, int targetY, string displayName)
    {
        ReactionDetails rd = new ReactionDetails(actorId,spellIndex,targetX,targetY,displayName);
        this.PostNotification(MultiplayerReaction,rd);
    }

    public void SetSpellLearnedType(int type)
    {
        spellLearnedType = type;
    }

    //spells added here are done before the turnOrder is set
    public void AddToSpellLearnedList(int unitId, List<AbilityLearnedListObject> tempList)
    {
        foreach( AbilityLearnedListObject a in tempList)
        {
            sSpellLearnedList.Add(new SpellLearnedSet(unitId + 100, a.ClassId, a.AbilityId));
        }
    }

    //alters all spells after the turn order is set
    public void AlterSpellLearnedList( int turnOrder, int listOrder)
    {
        //collection.Select(c => { c.PropertyToSet = value; return c; }).ToList();
        sSpellLearnedList.Where(s => s.TurnOrder == listOrder + 100).ToList().ForEach(s2 => s2.TurnOrder = turnOrder);
        //sSpellLearnedList.Select(s => { s.TurnOrder = turnOrder; return s; }).ToList();
    }

    public List<AbilityObject> GetSpellNamesToAbilityObject(int commandSetId, bool isCustomClass)
    {
        List<AbilityObject> retValue = new List<AbilityObject>();

        if (isCustomClass)
            GenerateSpellNamesByCustomCommandSet(commandSetId); //adds custom abilities if not already in

        var tempList = sSpellCommandSetList.Where(s => s.CommandSet == commandSetId);
        foreach( SpellCommandSet scs in tempList)
        {
            SpellName sn = GetSpellNameByIndex(scs.SpellIndex);
            if( sn != null)
                retValue.Add(new AbilityObject(sn));
        }
        return retValue;
    }

    #region WalkAround Functions
    //WalkAroundMode
    //called from PlayerManager in WalkAround mode when doing spellSlows
    public void AddWalkAroundSpellSlow(SpellSlow ss, bool isCTRZero = true)
    {
		if(isCTRZero)
			ss.CTR = 0; //in WA mode only add spell slow when CTR is 0
		//Debug.Log("current CTR is " + ss.CTR);
        AddSpellSlow(ss);
    }

    //called from walkAroundMainState, not decrementing ticks, if something is in the queue then it has priority
    public bool isWalkAroundSpellSlow()
    {
        return sSpellSlowList.Any();
    }

    
    #endregion
}

//simple class that stores the spellIndex and the command set, used for quickly loading command sets on Active Turns
//class instead of dictionary for LINQ usage
public class SpellCommandSet
{
    public int SpellIndex { get; set; }
    public int CommandSet { get; set; }

    public SpellCommandSet(int zIndex, int zCS)
    {
        this.SpellIndex = zIndex;
        this.CommandSet = zCS;
    }
}

//stores all spells known by active units in story/campaign modes. used for populating ability lists
public class SpellLearnedSet
{
    public int TurnOrder { get; set; }
    public int ClassId { get; set; }
    public int SpellIndex { get; set; }

    public SpellLearnedSet(int zOrder, int zClass, int zSpellIndex)
    {
        this.TurnOrder = zOrder;
        this.ClassId = zClass;
        this.SpellIndex = zSpellIndex;
    }
}


