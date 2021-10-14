using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]

//controls the physical PlayerUnit Object representation
	//for example, moving the unit on the gameboard calls a function here, showing unit statuses calls a function here
//some other misc stuff is randomly thrown in here like some AI and ability stuff which may be better moved elsewhere
//animation is disabled for now. see parts labelled '//Animation Disabled' to turn it back on

public class PlayerUnitObject : MonoBehaviour {

    private int unitId;
    public int UnitId
    {
        get { return unitId; }
        set { unitId = value; }
    }

	#region Const
	// for teleporting, turn scale of PUO back to original scal. I don't get why it's done this way
	public const float teleportScale = 0.7f;
	#endregion

	float placeY = 0.1f;
    bool isDead = false;

    public GameObject statusImageObject; //set in the prefab, keep this public
    private Animator _animator;
    bool statusImageState;
    private List<string> statusList;
    private int statusIndex;
    private float updateStatusImage;
    private Image statusImageImage;



	//movement related functions
	Directions dir;
    //Tile tile; meh not holding this here though i could
    Transform jumper; 
    float walkSpeed = 0.2f;
    bool isAttackingCheck = false;
    bool isDamageCheck = false;
    int safetyCheck = 0;
    //public int primaryAbilityCount { get; set; }
    //public int secondaryAbilityCount { get; set; }

    //ai related things moved to PU
	//public Drivers puoDriver; //set at beginning of battled and checked to see if if human or AI controlled
	//public List<SpellNameAI> aiSpellList; //used to help speed AI ability checks, used in CombatComputerPlayer to help select an action
	//public bool isDamageSpell; //used to help speed AI ability checks, used in CombatComputerPlayer to see if player can cast DamageSPell
	//public bool isReviveSpell; //used to help speed AI ability checks, used in CombatComputerPlayer to see if player can cast ReviveSpell
	//public bool isCureSpell; //used to help speed AI ability checks, used in CombatComputerPlayer to see if player can cast CureSpell

	//walk around
	public Tile walkAroundEndTile; //don't always end up in the tile that you targetted in walkAround

    void Awake()
    {
        statusImageState = false; //toggle to true to test statuses
        statusImageObject.SetActive(statusImageState);
        statusIndex = 0;
        statusList = new List<string>();
        updateStatusImage = 0.0f;
        statusImageImage = transform.Find("Unit Canvas").Find("Image").GetComponent<Image>();
        statusImageImage.enabled = false;

        //jumper = transform.FindChild("Jumper"); //not implemented yet
        jumper = this.transform;
    }

    void Start()
    {
        
        _animator = GetComponent<Animator>();

        isDead = false;
    }

    #region StatusImages

    
    void FixedUpdate()
    {
        //Debug.Log("status image state is " + statusImageState + " " + statusImageImage.enabled);
        //Debug.Log("statusList size is " + statusList.Count );
        //displays the status icons on units
        if (statusImageState)
        {
            updateStatusImage += Time.deltaTime; //might want to change the toggle time depending on number of statuses Debug.Log(" " + updateStatusImage);
            if( updateStatusImage >= 1.0f)
            {
                UpdateStatusImage();
                updateStatusImage = 0.0f;
            }
        }

        if (isAttackingCheck)
        {
			//Animation Disabled
			isAttackingCheck = false;

			//Debug.Log("update check " + _animator.GetCurrentAnimatorStateInfo(0).IsName("idle2"));
			//Debug.Log("update check " + _animator.GetCurrentAnimatorStateInfo(0).IsName("atk2"));
			//if (_animator.GetCurrentAnimatorStateInfo(0).IsName("atk2"))
			//{
			//    isAttackingCheck = false;
			//    SetAnimation("setIdle", true);
			//}

		}

		if ( isDamageCheck)
        {
			//Animation Disabled
			isDamageCheck = false;

			//Debug.Log("in is dmgCheck");
			//if (_animator.GetCurrentAnimatorStateInfo(0).IsName("damage"))
   //         {
   //             //Debug.Log("in is dmgCheck");
   //             isDamageCheck = false;
   //             SetAnimation("setIdle", true);
   //         }
        }

    }

    //switches to the next status image, called in FixedUpdate
    void UpdateStatusImage()
    {
        statusIndex += 1; //Debug.Log("asdf "+statusIndex);
        if (statusIndex > statusList.Count - 1)
        {
            statusIndex = 0;
        }
        if( statusList.Count <= 0)
        {
            return; //should never happen
        }
        //sets status image based on statusList[statusIndex]
        string statusString = statusList[statusIndex];
        Sprite statusSprite = Resources.Load<Sprite>("Sprites/" + statusString);
        
        //Debug.Log("updating status image " + statusString);
        if (statusSprite == null)
        {
            Debug.Log("ERROR: status failed to load " + statusString + " asdf " + statusIndex );
        }
        
        statusImageImage.sprite = statusSprite;
    }

    //adds a status to be displayed, called in PlayerManager
    public void AddToStatusList(string statusId)
    {
        if(statusId.Equals(""))
        {
            return;
        }
        statusList.Add(statusId); //Debug.Log("statusId is " + statusId);
        ToggleStatusImageState();
    }

    //removes a status from the list, called in PlayerManager
    public void RemoveFromStatusList(string statusId)
    {
        //List<string> tempList = new List<string>();
        //if( statusId.Equals("dead"))
        //{
        //    tempList.Add("dead");
        //    tempList.Add("dead_3");
        //    tempList.Add("dead_2");
        //    tempList.Add("dead_1");
        //    tempList.Add("dead_0");
        //}
        //else if (statusId.Equals("death_sentence") )
        //{
        //    tempList.Add("death_sentence");
        //    tempList.Add("death_sentence_3");
        //    tempList.Add("death_sentence_2");
        //    tempList.Add("death_sentence_1");
        //    tempList.Add("death_sentence_0");
        //}
        //else
        //{
        //    tempList.Add("statusId");
        //}

        //foreach( string s in statusList.ToList())
        //{
        //    foreach( string s2 in tempList)
        //    {
        //        if (s.Equals(s2))
        //        {
        //            statusList.Remove(s);
        //        }
        //    } 
        //}
        foreach (string s in statusList.ToList())
        {
            if (s.Equals(statusId))
            {
                statusList.Remove(s);
            }
        }
        ToggleStatusImageState();
    }

    //deactives the statusImage
    void ToggleStatusImageState()
    {
        if( statusList.Count > 0)
        {
            statusImageState = true;
            statusImageObject.SetActive(true);
            statusImageImage.enabled = true;
        }
        else
        {
            statusImageState = false;
            statusImageObject.SetActive(false);
            statusImageImage.enabled = false;
        }
    }

    #endregion

    //called in playermanager, knocks the playerUnitObject back
    public void InitializeKnockback( Tile t)
    {
        Vector3 pos = t.transform.position; 
        pos.y += pos.y + placeY;
        this.transform.position =  pos;
        
    }


        //called in CombatMoveSequenceState for move
    public void SetAnimation(string animation, bool setIdle = true)
    {
        if (setIdle)
        {
            if (!isDead)
            {
				//Animation Disabled// _animator.SetInteger("animation", 0);
				//Debug.Log("asdf " + _animator.GetCurrentAnimatorStateInfo(0).IsName("idle2"));
			}

		}
        else
        {
            if (animation.Equals("moving"))
            {
				//Animation Disabled// _animator.SetInteger("animation", 13); //Debug.Log("is moving?");
				//Debug.Log("asdf " + _animator.GetCurrentAnimatorStateInfo(0).IsName("run"));
			}
			//else if(animation.Equals("OnAttack"))
			//{
			//    _animator.SetTrigger("OnAttack"); Debug.Log("is attacking?");
			//}
			else if (animation.Equals("attacking"))
            {
                //attacking = true;
                //_animator.SetBool("attacking", attacking);
                isAttackingCheck = true;
                safetyCheck = 0;
				//Animation Disabled// _animator.SetInteger("animation", 1); //Debug.Log("is attacking?");
				//Debug.Log("asdf " + _animator.GetCurrentAnimatorStateInfo(0).IsName("idle2"));
				//Debug.Log("asdf " + _animator.GetCurrentAnimatorStateInfo(0).IsName("atk2") );
				//StartCoroutine(SetBackToIdle());
				//_animator.SetInteger("animation", 0); Debug.Log("not attacking");
			}
			else if (animation.Equals("damage"))
            {
                isDamageCheck = true;
				//_animator.SetBool("attacking", attacking);
				//Animation Disabled// _animator.SetInteger("animation", 7);//Debug.Log("is taking damage?" + isDamageCheck);
			}
			else if (animation.Equals("dead"))
            {
				//isDead = true;
				//isDeadAnimation = true;
				//Animation Disabled// _animator.SetInteger("animation", 11);//Debug.Log("Adding dead animation");
			}
			else if (animation.Equals("life"))
            {
                //isDead = false;
                //isRise = true;
                StartCoroutine(BringBackToLife()); Debug.Log("Adding life animation");
                
            }
        }
    }

    IEnumerator BringBackToLife()
    {
		//Animation Disabled// _animator.SetInteger("animation", 6);
		yield return new WaitForSeconds(0.1f);
        Turn(PlayerManager.Instance.GetPlayerUnit(this.UnitId).Dir);
        SetAnimation("idle",true);
    }

    //IEnumerator SetBackToIdle()
    //{
    //    while(_animator.GetCurrentAnimatorStateInfo(0).IsName("idle2") && safetyCheck < 25)
    //    {
    //        ++safetyCheck;
    //        Debug.Log("asdf " + safetyCheck);
    //        yield return new WaitForFixedUpdate();
    //    }
    //    Debug.Log("after while loop");
    //    yield return new WaitForFixedUpdate();
    //    SetAnimation("setIdle", true);
    //}

    #region TacticsRPG move functions. modified from his code his walking

    int moveRange = 0;
    int jumpHeight = 0;
    int teamId;
	int combatProximityRange = 1; //range to check for if combat triggered, can be set here to

    //PlayerUnit puUnit;

    public List<Tile> GetTilesInRange(Board board, Tile startTile, PlayerUnit pu)
    {
        moveRange = pu.StatTotalMove;
        jumpHeight = pu.StatTotalJump;
        teamId = pu.TeamId;

        List<Tile> retValue = GetMoveRange(board, startTile, pu);

        Filter(retValue);
        return retValue;
    }

    public List<Tile> GetWalkAroundTilesInRange(Board board, Tile startTile, PlayerUnit pu)
    {
        moveRange = pu.StatTotalMove;
        jumpHeight = pu.StatTotalJump;
        teamId = pu.TeamId;

        List<Tile> retValue = GetWalkAroundMoveRange(board, startTile, pu);

        Filter(retValue);
        return retValue;
    }

    //basically GetTilesInRange but lets the AI choose the tile it started on
    public List<Tile> GetAITilesToMove(Board board, Tile startTile, PlayerUnit pu)
    {
        moveRange = pu.StatTotalMove;
        jumpHeight = pu.StatTotalJump;
        teamId = pu.TeamId;

        List<Tile> retValue = GetMoveRange(board, startTile, pu);

        FilterAITilesToMove(retValue,pu.TurnOrder);
        return retValue;
    }

    List<Tile> GetMoveRange(Board board, Tile startTile, PlayerUnit pu)
    {
        //List<Tile> retValue;

        if(pu.IsSpecialMoveRange())
        {
            if (NameAll.IsClassicClass(pu.ClassId))
            {
                if( pu.AbilityMovementCode == NameAll.MOVEMENT_TELEPORT_1 || pu.AbilityMovementCode == NameAll.MOVEMENT_TELEPORT_2)
                {
                    return board.Search(startTile, ExpandSearchTeleport);
                }
                else if(pu.AbilityMovementCode == NameAll.MOVEMENT_FLY)
                {
                    return board.Search(startTile, ExpandSearchWalkFly);
                }
                else if( pu.AbilityMovementCode == NameAll.MOVEMENT_IGNORE_HEIGHT)
                {
                    return board.Search(startTile, ExpandSearchWalkIgnoreHeight);
                }
            }
            else
            {
                if( pu.AbilityMovementCode == NameAll.MOVEMENT_UNSTABLE_TP)
                {
                    return board.Search(startTile, ExpandSearchTeleport);
                }
                else if( pu.AbilityMovementCode == NameAll.MOVEMENT_WINDS_OF_FATE)
                {
                    return board.Search(startTile, ExpandSearchTeleport);
                }
                else if( pu.AbilityMovementCode == NameAll.MOVEMENT_GHOST)
                {
                    List<Tile> tempList = board.Search(startTile, ExpandSearchWalkFly); //get all tiles in move range
                    return FilterGhostTiles(tempList,startTile, pu.StatTotalJump); //Ghost can only move to tiles within move range and height range (board search doesn't work because you can sometimes reach a tile 2 tiles away but not one tile away)
                }
                else if( pu.AbilityMovementCode == NameAll.MOVEMENT_SCALE)
                {
                    //two ways of doing this (using 1 for now)
                    //1 to modify board.search or playerunit.getjumpscale to realize hey, this is the first tile, look for a chance to scale and then build from there
                    //2 way after you get the board.search tiles, look for all adjacent tiles and see if they can be added
                    return board.Search(startTile, ExpandSearchScale);
                }
                else if( pu.AbilityMovementCode == NameAll.MOVEMENT_LEAP)
                {
                    List<Tile> tempList = board.Search(startTile, ExpandSearchWalk);
                    //adds tiles at the diagnols
                    int z1 = pu.StatTotalMove / 2 + 2;
                    Tile t;
                    t = board.GetTile(startTile.pos.x + z1, startTile.pos.y + z1);
                    if (t != null && t.UnitId == NameAll.NULL_UNIT_ID && Mathf.Abs(t.height - startTile.height) <= pu.StatTotalJump)
                        tempList.Add(board.GetTile(startTile.pos.x + z1, startTile.pos.y + z1));
                    t = board.GetTile(startTile.pos.x + z1, startTile.pos.y - z1);
                    if (t != null && t.UnitId == NameAll.NULL_UNIT_ID && Mathf.Abs(t.height - startTile.height) <= pu.StatTotalJump)
                        tempList.Add(board.GetTile(startTile.pos.x + z1, startTile.pos.y - z1));
                    t = board.GetTile(startTile.pos.x - z1, startTile.pos.y + z1);
                    if (t != null && t.UnitId == NameAll.NULL_UNIT_ID && Mathf.Abs(t.height - startTile.height) <= pu.StatTotalJump)
                        tempList.Add(board.GetTile(startTile.pos.x - z1, startTile.pos.y + z1));
                    t = board.GetTile(startTile.pos.x - z1, startTile.pos.y - z1);
                    if (t != null && t.UnitId == NameAll.NULL_UNIT_ID && Mathf.Abs(t.height - startTile.height) <= pu.StatTotalJump)
                        tempList.Add(board.GetTile(startTile.pos.x - z1, startTile.pos.y - z1));
                    return tempList;
                }
                //else if( pu.AbilityMovementCode == NameAll.MOVEMENT_CRUNCH)
                //{
                //    //still does expandsearchwalk, difference is doesn't filter out dead units
                //}
                //else if (pu.AbilityMovementCode == NameAll.MOVEMENT_SWAP)
                //{
                //    return board.Search(startTile, ExpandSearchWalk); //difference is doesn't filter out allies
                //}
            }
        }
        return board.Search(startTile, ExpandSearchWalk);
    }

    //called above in GetWalkAroundTilesInRange
    //calls a special board.SearchWalkAround since Search in combat relies on only one unit at a time going
    List<Tile> GetWalkAroundMoveRange(Board board, Tile startTile, PlayerUnit pu)
    {
        //List<Tile> retValue;

        if (pu.IsSpecialMoveRange())
        {
            if (NameAll.IsClassicClass(pu.ClassId))
            {
                if (pu.AbilityMovementCode == NameAll.MOVEMENT_TELEPORT_1 || pu.AbilityMovementCode == NameAll.MOVEMENT_TELEPORT_2)
                {
                    return board.SearchWalkAround(startTile, this.UnitId, ExpandSearchTeleport);
                }
                else if (pu.AbilityMovementCode == NameAll.MOVEMENT_FLY)
                {
                    return board.SearchWalkAround(startTile, this.UnitId, ExpandSearchWalkFly);
                }
                else if (pu.AbilityMovementCode == NameAll.MOVEMENT_IGNORE_HEIGHT)
                {
                    return board.SearchWalkAround(startTile, this.UnitId, ExpandSearchWalkIgnoreHeight);
                }
            }
            else
            {
                if (pu.AbilityMovementCode == NameAll.MOVEMENT_UNSTABLE_TP)
                {
                    return board.SearchWalkAround(startTile, this.UnitId, ExpandSearchTeleport);
                }
                else if (pu.AbilityMovementCode == NameAll.MOVEMENT_WINDS_OF_FATE)
                {
                    return board.SearchWalkAround(startTile, this.UnitId, ExpandSearchTeleport);
                }
                else if (pu.AbilityMovementCode == NameAll.MOVEMENT_GHOST)
                {
                    List<Tile> tempList = board.SearchWalkAround(startTile, this.UnitId, ExpandSearchWalkFly); //get all tiles in move range
                    return FilterGhostTiles(tempList, startTile, pu.StatTotalJump); //Ghost can only move to tiles within move range and height range (board search doesn't work because you can sometimes reach a tile 2 tiles away but not one tile away)
                }
                else if (pu.AbilityMovementCode == NameAll.MOVEMENT_SCALE)
                {
                    //two ways of doing this (using 1 for now)
                    //1 to modify board.search or playerunit.getjumpscale to realize hey, this is the first tile, look for a chance to scale and then build from there
                    //2 way after you get the board.search tiles, look for all adjacent tiles and see if they can be added
                    return board.SearchWalkAround(startTile, this.UnitId, ExpandSearchScale);
                }
                else if (pu.AbilityMovementCode == NameAll.MOVEMENT_LEAP)
                {
                    List<Tile> tempList = board.SearchWalkAround(startTile, this.UnitId, ExpandSearchWalk);
                    //adds tiles at the diagnols
                    int z1 = pu.StatTotalMove / 2 + 2;
                    Tile t;
                    t = board.GetTile(startTile.pos.x + z1, startTile.pos.y + z1);
                    if (t != null && t.UnitId == NameAll.NULL_UNIT_ID && Mathf.Abs(t.height - startTile.height) <= pu.StatTotalJump)
                        tempList.Add(board.GetTile(startTile.pos.x + z1, startTile.pos.y + z1));
                    t = board.GetTile(startTile.pos.x + z1, startTile.pos.y - z1);
                    if (t != null && t.UnitId == NameAll.NULL_UNIT_ID && Mathf.Abs(t.height - startTile.height) <= pu.StatTotalJump)
                        tempList.Add(board.GetTile(startTile.pos.x + z1, startTile.pos.y - z1));
                    t = board.GetTile(startTile.pos.x - z1, startTile.pos.y + z1);
                    if (t != null && t.UnitId == NameAll.NULL_UNIT_ID && Mathf.Abs(t.height - startTile.height) <= pu.StatTotalJump)
                        tempList.Add(board.GetTile(startTile.pos.x - z1, startTile.pos.y + z1));
                    t = board.GetTile(startTile.pos.x - z1, startTile.pos.y - z1);
                    if (t != null && t.UnitId == NameAll.NULL_UNIT_ID && Mathf.Abs(t.height - startTile.height) <= pu.StatTotalJump)
                        tempList.Add(board.GetTile(startTile.pos.x - z1, startTile.pos.y - z1));
                    return tempList;
                }
                //else if( pu.AbilityMovementCode == NameAll.MOVEMENT_CRUNCH)
                //{
                //    //still does expandsearchwalk, difference is doesn't filter out dead units
                //}
                //else if (pu.AbilityMovementCode == NameAll.MOVEMENT_SWAP)
                //{
                //    return board.Search(startTile, ExpandSearchWalk); //difference is doesn't filter out allies
                //}
            }
        }
        return board.SearchWalkAround(startTile, this.UnitId, ExpandSearchWalk);
    }

    protected bool ExpandSearchWalk(Tile from, Tile to)
    {

        // Skip if the distance in height between the two tiles is more than the unit can jump
        if ((Mathf.Abs(from.height - to.height) > jumpHeight))
            return false;

        // Skip if the tile is occupied by an enemy
        if ( to.UnitId != NameAll.NULL_UNIT_ID &&
            PlayerManager.Instance.GetPlayerUnit(to.UnitId).GetCharmTeam() != teamId
            && !StatusManager.Instance.IfStatusByUnitAndId(to.UnitId,NameAll.STATUS_ID_DEAD) )
        {
            return false;
        }

        return ExpandSearchBase(from, to);
    }

    protected bool ExpandSearchTeleport(Tile from, Tile to)
    {
        //no height check
        // Skip if the tile is occupied by an enemy
        //PlayerManager.Instance.GetPlayerUnit(to.UnitId).GetCharmTeam() != teamId)
        //{
        //    return false;
        //}
        //no range check
        return true;
    }

    protected bool ExpandSearchWalkFly(Tile from, Tile to)
    {
        //doesn't do a height check

        //fly doesn't skip enemy occupied tiles

        return ExpandSearchBase(from, to);
    }

    protected bool ExpandSearchWalkIgnoreHeight(Tile from, Tile to)
    {
        //doesn't do a height check

        // Skip if the tile is occupied by an enemy
        if (to.UnitId != NameAll.NULL_UNIT_ID &&
            PlayerManager.Instance.GetPlayerUnit(to.UnitId).GetCharmTeam() != teamId)
        {
            return false;
        }

        return ExpandSearchBase(from, to);
    }

    protected bool ExpandSearchScale(Tile from, Tile to)
    {

        // Skip if the distance in height between the two tiles is more than the unit can jump
        if (Mathf.Abs(from.height - to.height) > PlayerManager.Instance.GetJumpScale(this.UnitId, from) )
            return false;

        // Skip if the tile is occupied by an enemy
        if (to.UnitId != NameAll.NULL_UNIT_ID &&
            PlayerManager.Instance.GetPlayerUnit(to.UnitId).GetCharmTeam() != teamId
            && !StatusManager.Instance.IfStatusByUnitAndId(to.UnitId, NameAll.STATUS_ID_DEAD))
        {
            return false;
        }

        return ExpandSearchBase(from, to);
    }

    protected bool ExpandSearchBase(Tile from, Tile to)
    {
        return (from.distance + 1) <= moveRange;
    }

	//checks to see if units are within combatProximityRange and then a later check sees if combat should start
	protected bool ExpandSearchWalkAroundProximity(Tile from, Tile to)
	{
		//Debug.Log("testings ExpandSearchWalkAroundProximity " + from.GetTileSummary() + " " + to.GetTileSummary());
		//bool zBool = (from.distance + 1) <= combatProximityRange;
		//Debug.Log("testings ExpandSearchWalkAroundProximity " + (from.distance + 1) + " " + zBool + " " + combatProximityRange);
		return (from.distance + 1) <= combatProximityRange;
	}

	//can expand upon this in future in many interesting ways, for now just 1
	void SetCombatProximityRange()
	{
		combatProximityRange = 1; //to do: expand upon this
	}

	protected void Filter(List<Tile> tiles)
    {
        if( PlayerManager.Instance.GetPlayerUnit(this.UnitId).AbilityMovementCode == NameAll.MOVEMENT_CRUNCH)
        {
            for (int i = tiles.Count - 1; i >= 0; --i)
            {
                if (tiles[i].UnitId != NameAll.NULL_UNIT_ID && !StatusManager.Instance.IfStatusByUnitAndId(tiles[i].UnitId,NameAll.STATUS_ID_DEAD) )
                {
                    tiles.RemoveAt(i);
                }  
            }     
        }
        else if( PlayerManager.Instance.GetPlayerUnit(this.UnitId).AbilityMovementCode == NameAll.MOVEMENT_SWAP)
        {
            for (int i = tiles.Count - 1; i >= 0; --i)
            {
                if (tiles[i].UnitId != NameAll.NULL_UNIT_ID && !PlayerManager.Instance.IsOnTeam(this.UnitId,tiles[i].UnitId) )
                {
                    tiles.RemoveAt(i);
                }
            }
        }
        else
        {
            for (int i = tiles.Count - 1; i >= 0; --i)
                if (tiles[i].UnitId != NameAll.NULL_UNIT_ID)
                    tiles.RemoveAt(i);
        }   
    }

    protected void FilterAITilesToMove(List<Tile> tiles, int actorId)
    {
        for (int i = tiles.Count - 1; i >= 0; --i)
            if (tiles[i].UnitId != NameAll.NULL_UNIT_ID && tiles[i].UnitId != actorId)
                tiles.RemoveAt(i);
    }

    protected List<Tile> FilterGhostTiles(List<Tile> tiles, Tile startTile, int jumpHeight)
    {
        for (int i = tiles.Count - 1; i >= 0; --i)
        {
            if (Mathf.Abs(startTile.height - tiles[i].height) > jumpHeight)
                tiles.RemoveAt(i);
        }
        return tiles;
    }

    protected IEnumerator Turn(Directions newDir)
    {
        TransformLocalEulerTweener t = (TransformLocalEulerTweener)transform.RotateToLocal(newDir.ToEuler(), 0.25f, EasingEquations.EaseInOutQuad);
		t.endTweenValue.y += 25.0f; //yeah I don't know why i need this
		// When rotating between North and West, we must make an exception so it looks like the unit
		// rotates the most efficient way (since 0 and 360 are treated the same)
		if (Mathf.Approximately(t.startTweenValue.y, 0f) && Mathf.Approximately(t.endTweenValue.y, 270f))
            t.startTweenValue = new Vector3(t.startTweenValue.x, 360f, t.startTweenValue.z);
        else if (Mathf.Approximately(t.startTweenValue.y, 270) && Mathf.Approximately(t.endTweenValue.y, 0))
            t.endTweenValue = new Vector3(t.startTweenValue.x, 360f, t.startTweenValue.z);

        //unit.dir = dir; //I update the PlayerUnit elsewhere
        this.dir = newDir;
        //Debug.Log("in turn function turning? " + t.startTweenValue + " " + t.endTweenValue);
        while (t != null)
            yield return null;
    }

    //called to start the movement
    //when the move path is set, each tile is given a tile.prev to link the tile movement order, 
        //traverse takes that movement order and calls appropriate movement coroutine
    public IEnumerator Traverse(Tile t)
    {
        //unit.Place(tile); //unit position updated elsewhere

        // Build a list of way points from the unit's 
        // starting tile to the destination tile
        List<Tile> targets = new List<Tile>();
        while (t != null)
        {
            targets.Insert(0, t);
            t = t.prev;
        }

        // Move to each way point in succession
        for (int i = 1; i < targets.Count; ++i)
        {
            Tile from = targets[i - 1];
            Tile to = targets[i];

            Directions dir = from.GetDirection(to);
            if (this.dir != dir)
                yield return StartCoroutine(Turn(dir));

            if (from.height == to.height)
                yield return StartCoroutine(Walk(to));
            else
                yield return StartCoroutine(Jump(to));
        }

        yield return null;
    }

    //called from PlayerManager
    //allows unit to walk from one tile to another in WalkAroundMode
    public IEnumerator TraverseWalkAround(Board board, Tile t)
    {
        //Debug.Log("in TraverseWalkAround I should be moving here... " + t);
        // Build a list of way points from the unit's 
        // starting tile to the destination tile
        List<Tile> targets = new List<Tile>();
        while (t != null)
        {
            targets.Insert(0, t);
            Tile tempTile;
            if (!t.prevDict.TryGetValue(this.UnitId, out tempTile))
                break;
            t.prevDict.Remove(this.UnitId);
            t.distanceDict.Remove(this.UnitId);
            t = tempTile;
        }

        // Move to each way point in succession
        for (int i = 1; i < targets.Count; ++i)
        {
            //walk around is halted stop moving the unit
            if (!PlayerManager.Instance.GetWalkAroundMoveAllowed())
                break;
            Tile from = targets[i - 1];
            Tile to = targets[i];

            //Debug.Log("TraverseWalkAround testing testing testing");
            //if tile is not occupied, claim it for this uni
            if( to.UnitId == NameAll.NULL_UNIT_ID)
            {
                //Debug.Log("moving through tile, why am I not claiming it? " + to.GetTileSummary());
                PlayerManager.Instance.SetUnitTile(board, this.UnitId, to, true);
                this.walkAroundEndTile = t;

				//check all tiles within proximity range to see if the move triggers combat starting
				SetCombatProximityRange(); //placeholder for now, can expand upon this
				var proximityTiles = board.Search(to, ExpandSearchWalkAroundProximity); //eh this works the one below it does not, not srue why
				//var proximityTiles = board.SearchWalkAround(to, this.UnitId + 1000, ExpandSearchWalkAroundProximity); //not sure why this doesn't work
				//Debug.Log("prox tile number " + proximityTiles.Count);
				foreach( Tile pt in proximityTiles)
				{
					//Debug.Log("prox tile details " + pt.GetTileSummary());
					PlayerManager.Instance.CheckCombatProximity(pt, this.UnitId);
				}
            }

            Directions dir = from.GetDirection(to);
            if (this.dir != dir)
                yield return StartCoroutine(Turn(dir));

            if (from.height == to.height)
                yield return StartCoroutine(Walk(to));
            else
                yield return StartCoroutine(Jump(to));
        }
		//this.walkAroundEndTile = 
		//need to check if tile in front is going to open up
		//or if unit can pass through the unit on it
		//at end need to set walkAroundEndTile

		yield return null;
    }

    public IEnumerator TraverseFly(Tile tile)
    {
        // Store the distance between the start tile and target tile
        PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(this.UnitId);
        float dist = Mathf.Sqrt(Mathf.Pow(tile.pos.x - pu.TileX, 2) + Mathf.Pow(tile.pos.y - pu.TileY, 2));
        //unit.Place(tile);

        // Fly high enough not to clip through any ground tiles
        float y = Tile.stepHeight * 10;
        float duration = (y - jumper.position.y) * 0.3f;
        //Tweener tweener = jumper.MoveToLocal(new Vector3(0, y, 0), duration, EasingEquations.EaseInOutQuad);
        Tweener tweener = jumper.MoveToLocal(new Vector3(pu.TileX, y, pu.TileY), duration, EasingEquations.EaseInOutQuad);
        while (tweener != null)
            yield return null;
       
        // Turn to face the general direction
        Directions dir;
        Vector3 toTile = (tile.center - transform.position);
        if (Mathf.Abs(toTile.x) > Mathf.Abs(toTile.z))
            dir = toTile.x > 0 ? Directions.East : Directions.West;
        else
            dir = toTile.z > 0 ? Directions.North : Directions.South;
        yield return StartCoroutine(Turn(dir));

        // Move to the correct position
        duration = dist * 0.5f;
        tweener = transform.MoveTo(tile.center, duration, EasingEquations.EaseInOutQuad);
        while (tweener != null)
            yield return null;

        // Land DOESN"T WORK NEED TO CHANGE MOVE TO LOCAL TO SOMETHING ELSE
        //duration = (y - tile.center.y) * 0.5f;
        //tweener = jumper.MoveToLocal(Vector3.zero, 0.5f, EasingEquations.EaseInOutQuad); //jumper.MoveToLocal(new Vector3(0, Tile.stepHeight * 2f, 0), tweener.duration / 2f, EasingEquations.EaseOutQuad);
        //while (tweener != null)
        //    yield return null;
    }

    public IEnumerator TraverseTeleport(Tile tile)
    {
        //unit.Place(tile);

        Tweener spin = jumper.RotateToLocal(new Vector3(0, 360, 0), 0.5f, EasingEquations.EaseInOutQuad);
        spin.loopCount = 1;
        spin.loopType = EasingControl.LoopType.PingPong;

        Tweener shrink = transform.ScaleTo(Vector3.zero, 0.5f, EasingEquations.EaseInBack); //new Vector3(0.5f,0.5f,0.5f)

        while (shrink != null)
            yield return null;
        //yield return new WaitForSeconds(0.25f);
        transform.position = tile.center;
        //yield return new WaitForSeconds(0.25f);
        Tweener grow = transform.ScaleTo(new Vector3(teleportScale, teleportScale, teleportScale), 0.5f, EasingEquations.EaseOutBack);
        while (grow != null)
            yield return null;
    }

    IEnumerator Walk(Tile target)
    {
        Tweener tweener = transform.MoveTo(target.center, walkSpeed, EasingEquations.Linear);
        while (tweener != null)
            yield return null;
    }

    IEnumerator Jump(Tile to)
    {
        Tweener tweener = transform.MoveTo(to.center, walkSpeed*1.5f, EasingEquations.Linear);
        //Debug.Log("testing tile to " + to.pos.x + ", " + to.pos.y);
        //Debug.Log("testing tile to " + jumper.transform.position);
        //Debug.Log("testing tile to " + this.transform.position);
        //Debug.Log("testing tile to " + transform.position);
        //Tweener t2 = jumper.MoveToLocal(new Vector3(0, Tile.stepHeight * 2f, 0), tweener.duration / 2f, EasingEquations.EaseOutQuad);
        //Tweener t2 = jumper.MoveToLocal(to.center, tweener.duration / 2f, EasingEquations.EaseOutQuad); //new Vector3(to.pos.x, Tile.stepHeight * 2f, to.pos.y)
        //Tweener t2 = transform.MoveToLocal(new Vector3(0, Tile.stepHeight * 2f, 0), tweener.duration / 2f, EasingEquations.EaseOutQuad);
        //t2.loopCount = 1;
        //t2.loopType = EasingControl.LoopType.PingPong;

        //tweener.loopCount = 1;
        //tweener.loopType = EasingControl.LoopType.PingPong;

        while (tweener != null)
            yield return null;

        //transform.position = to.center;
    }

    

    //called from playermanager at end of turn, turns direction to end turn
    public void SetFacingDirectionEndTurn(PlayerUnit pu)
    {
        StartCoroutine(Turn(pu.Dir)); //dir already updated
    }

    //move direction mid turn
    public void SetAttackDirection(Directions dir)
    {
        //Debug.Log("fucking turning?");
        StartCoroutine(Turn(dir)); //dir already updated
    }
	#endregion

	//move in gridworld, simply moves the PUO object to the tile
	public void MoveGridworld(Tile t)
	{
		Vector3 pos = t.transform.position;
		pos.y += pos.y + placeY;
		this.transform.position = pos;

	}
}
