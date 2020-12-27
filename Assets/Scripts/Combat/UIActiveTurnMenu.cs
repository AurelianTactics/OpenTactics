using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

public class UIActiveTurnMenu : MonoBehaviour {

    #region Constants
    [SerializeField]
    private Text puPrimary;
    [SerializeField]
    private Text puSecondary;
    [SerializeField]
    private Button moveButton;
    [SerializeField]
    private Button attackButton;
    [SerializeField]
    private Button waitButton;
    [SerializeField]
    private Button primaryButton;
    [SerializeField]
    private Button secondaryButton;
    [SerializeField]
    private Button confirmButton;
    [SerializeField]
    private Button backButton;
    public Button targetUnitButton;
    public Button targetMapButton;
    public Button waitNorth;
    public Button waitEast;
    public Button waitSouth;
    public Button waitWest;

    UIGenericButton moveButtonUI;
    UIGenericButton attackButtonUI;
    UIGenericButton primaryButtonUI;
    UIGenericButton secondaryButtonUI;
    UIGenericButton waitButtonUI;
    UIGenericButton confirmButtonUI;
    UIGenericButton targetUnitButtonUI;
    UIGenericButton targetMapButtonUI;
    UIGenericButton backButtonUI;
    UIGenericButton waitNorthUI;
    UIGenericButton waitSouthUI;
    UIGenericButton waitEastUI;
    UIGenericButton waitWestUI;

    List<Button> buttonList = new List<Button>(); //handles the highlighting of buttons and selection of buttons
    int selection { get; set; } //which button in the list is selected

    private bool confirmCharge = false;

    const string ActiveTurnTopNotification = "ActiveTurnTopNotification";
    const string ActiveTurnWaitNotification = "ActiveTurnWaitNotification";
    const string ActiveTurnConfirmNotification = "ActiveTurnConfirmNotification";
    const string ActiveTurnTargetUnitMapNotification = "ActiveTurnTargetUnitMapNotification";
    const string ActiveTurnBackNotification = "ActiveTurnBackNotification";

    public const string DidWaitClick = "ActiveTurn.WaitClick";
    public const string DidMoveClick = "ActiveTurn.MoveClick";
    public const string DidAttackClick = "ActiveTurn.AttackClick";
    public const string DidPrimaryClick = "ActiveTurn.PrimaryClick";
    public const string DidSecondaryClick = "ActiveTurn.SecondaryClick";

    public const string DidConfirmClick = "ActiveTurn.ConfirmClick";
    public const string DidBackClick = "ActiveTurn.BackClick";
    public const string DidTargetUnitClick = "ActiveTurn.TargetUnitClick";
    public const string DidTargetMapClick = "ActiveTurn.TargetMapClick";

    public const string DidNorthClick = "ActiveTurn.NorthClick";
    public const string DidEastClick = "ActiveTurn.EastClick";
    public const string DidSouthClick = "ActiveTurn.SouthClick";
    public const string DidWestClick = "ActiveTurn.WestClick";

    //colors for window based on alliances
    Color allianceNoneColor = new Color32(203, 216, 220, 255);
    Color allianceNeutralColor = new Color32(126, 209, 232, 255);
    Color allianceHeroColor = new Color32(236, 142, 47, 255);
    Color allianceAllyColor = new Color32(252, 202, 152, 255);
    Color allianceEnemyColor = new Color32(129, 77, 197, 255);
    #endregion

    #region input
    //void OnEnable()
    //{
    //    InputController.moveEvent += OnMoveEvent;
    //    InputController.fireEvent += OnFireEvent;
    //}

    void OnDisable()
    {
        DisableInputs();
    }

    void DisableInputs()
    {
        //Debug.Log("disabling inputs");
        InputController.moveEvent -= OnMoveEvent;
        InputController.fireEvent -= OnFireEvent;
    }

    void EnableInputs()
    {
        //Debug.Log("enabling inputs");
        InputController.moveEvent += OnMoveEvent;
        InputController.fireEvent += OnFireEvent;
    }

    void OnFixedUpdate()
    {
        if( buttonList.Count > 0)
        {
            int index = Mathf.Abs(selection % buttonList.Count);
            buttonList[index].Select();
        }
    }

    void OnMoveEvent(object sender, InfoEventArgs<Point> e)
    {
        //Debug.Log("Move " + e.info.ToString());
        //SelectTile(e.info + pos);

        //highlights the next button in the buttonList
        if( buttonList.Count > 0)
        {
            if (e.info.x > 0 || e.info.y < 0)
            {
                selection += 1;

            }
            else
            {
                selection -= 1;
            }
            if (selection < 0)
            {
                selection = buttonList.Count - 1;
            }
            else if (selection >= buttonList.Count)
            {
                selection = selection % buttonList.Count;
            }
            int index = selection; //Debug.Log("asdf" + index + " " + selection + " " + buttonList.Count); //Mathf.Abs(selection % buttonList.Count); 
            buttonList[index].Select();
            selection = index;
        }
        
    }

    void OnFireEvent(object sender, InfoEventArgs<int> e)
    {
        //Debug.Log("1 in on fire event " + e.info.ToString());
        //click 0 handled by buttons, click 1 (cancel) handled by state, click 2 takes the confirm as an input on the highlighted button
        if (buttonList.Count > 0)
        {
            if (e.info == 2)
            {
                
                int index = selection % buttonList.Count; //Debug.Log("about to trigger button OnClick " + selection);
                Button btn = buttonList[index];
                btn.onClick.Invoke();
            }
        }
        
 
    }
    #endregion

    #region Notifications
    //CombatCommandSelectionState, ..., listen but only the active state listens and proceeds with it
    public void DidActiveTurnMenuNotification(string notificationType, string clickType)
    {
        //Debug.Log("posting notification " + notificationType + " " + clickType);
        this.PostNotification(notificationType,clickType);
    }
    #endregion

    // Use this for initialization
    void Awake () {
        //Debug.Log("shit initialized?"); //for some reason have to do awake, calling it in start doesn't let it be called until after SetActor...
	    moveButtonUI = moveButton.GetComponent<UIGenericButton>();
        attackButtonUI = attackButton.GetComponent<UIGenericButton>();
        primaryButtonUI = primaryButton.GetComponent<UIGenericButton>();
        secondaryButtonUI = secondaryButton.GetComponent<UIGenericButton>();
        waitButtonUI = waitButton.GetComponent<UIGenericButton>();
        confirmButtonUI = confirmButton.GetComponent<UIGenericButton>();
        backButtonUI = backButton.GetComponent<UIGenericButton>();
        targetUnitButtonUI = targetUnitButton.GetComponent<UIGenericButton>();
        targetMapButtonUI = targetMapButton.GetComponent<UIGenericButton>();
        waitNorthUI = waitNorth.GetComponent<UIGenericButton>();
        waitEastUI = waitEast.GetComponent<UIGenericButton>();
        waitSouthUI = waitSouth.GetComponent<UIGenericButton>();
        waitWestUI = waitWest.GetComponent<UIGenericButton>();
        selection = 0;
        //backButtonUI.Close();
        //targetUnitButtonUI.Close();
        //targetMapButtonUI.Close();
        //previewTextUI = previewImage.GetComponent<UIPreviewText>();
        //ShowTopMenuPhase();
    }

    #region SetMenu functions
    //called in CombatCommandSelectionState, base options
    public void SetMenuTop(CombatTurn turn, BattleMessageController bmc)
    {
        //Debug.Log("set menu top");
        Open(turn); //Debug.Log("Opening AT Menu ");
        ButtonsHideAll();
        if ( !turn.hasUnitActed && StatusManager.Instance.IsContinueAbility(turn.actor) )
        {
            //Debug.Log("ADD CODE TO Handle charging/performing menu options");
            //send message to spelltitle panel saying how to handle
            bmc.Display("Continue ability from prior turn?");
            ButtonsShowConfirmCancel();
        }
        else
        {
            ButtonsShowTop(turn); //wait/direction only menu is its own state
        }
    }

	public void SetMenuMoveMap(CombatTurn turn, BattleMessageController bmc)
	{
		Open(turn); //Debug.Log("Opening AT Menu ");
		ButtonsHideAll();
		bmc.Display("Leave area?");
		ButtonsShowConfirmCancel();
	}

    //called in CombatEndFacingState
    public void SetMenuDirection(CombatTurn turn)
    {
        //Debug.Log("In set menu direction");
        Open(turn);
        ButtonsHideAll();
        bool allowBack = true;
        if (turn.hasUnitActed && turn.hasUnitMoved)
            allowBack = false;
        ButtonsShowWait(allowBack);
    }

    public void SetMenuTarget(CombatTurn turn)
    {
        Open(turn);
        ButtonsHideAll();
        ButtonsShowTarget();
    }

    public void SetMenuTargetDirection(CombatTurn turn)
    {
        Open(turn);
        ButtonsHideAll();
        ButtonsShowDirection();
    }

	//not sure why I don't allow confirm and cancel button in combat mode
	public void SetMenuTargetDirectionWalkAround(CombatTurn turn)
	{
		Open(turn);
		ButtonsHideAll();
		ButtonsShowDirection();
		ButtonsShowConfirmCancelWalkAround();
	}

	public void SetMenuAttack(CombatTurn turn)
	{
		//target unit/panel to have confirm button show
		Open(turn);
		ButtonsHideAll();
		ButtonsShowCancel();
	}

	public void SetMenuAbilityConfirm(CombatTurn turn, bool canTargetUnit = false)
    {
        //either confirm/cancel OR target unit/target map/cancel
        Open(turn);
        ButtonsHideAll();
        if ( canTargetUnit )
        {
            ButtonsShowTargetUnitOrMap();
        } 
        else
        {
            ButtonsShowConfirmCancel();
        }
    }

	//walkaround mode wants the buttons highlighted, stupid fucking workaround
	public void SetMenuAbilityConfirmWalkAround(CombatTurn turn, bool canTargetUnit = false)
	{
		//either confirm/cancel OR target unit/target map/cancel
		Open(turn);
		ButtonsHideAll();
		if (canTargetUnit)
		{
			ButtonsShowTargetUnitOrMap();
		}
		else
		{
			ButtonsShowConfirmCancelWalkAround();
		}
	}

	public void SetMenuMove(CombatTurn turn)
    {
        //either confirm/cancel OR target unit/target map/cancel
        Open(turn);
        ButtonsHideAll();
        ButtonsShowConfirmCancel();
    }

	public void SetMenuMoveWalkAround(CombatTurn turn, bool isValidMoveTile)
	{
		//either confirm/cancel OR target unit/target map/cancel
		Open(turn);
		ButtonsHideAll();
		if (isValidMoveTile)
			ButtonsShowConfirmCancelWalkAround();
		else
			ButtonsShowCancel();
	}
	#endregion

	#region functions buttons to show/hide
	void ButtonsShowTop(CombatTurn turn)
    {
        //Debug.Log("showing buttons");
        if( turn.hasUnitMoved) //statuses applied elsewhere || StatusManager.Instance.IfStatusByUnitAndId(turn.puId, NameAll.STATUS_ID_DONT_MOVE, true)
        {
            moveButtonUI.Close();
        }
        else
        {
            moveButtonUI.Open();
            buttonList.Add(moveButton);
        }

        if (turn.hasUnitActed) //if don't act on unit, turn takes that into account|| StatusManager.Instance.IfStatusByUnitAndId(turn.puId, NameAll.STATUS_ID_DONT_ACT, true)
        {
            attackButtonUI.Close();
            primaryButtonUI.Close();
            secondaryButtonUI.Close();
        }
        else
        {
            attackButtonUI.Open();
            buttonList.Add(attackButton);
            if ( IsPrimaryEligible(turn))
            {
                primaryButtonUI.Open();
                puPrimary.text = AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY, turn.actor.ClassId );
                buttonList.Add(primaryButton);
            }
            if (IsSecondaryEligible(turn))
            {
                secondaryButtonUI.Open();
                puSecondary.text = AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_SECONDARY, turn.actor.AbilitySecondaryCode );
                buttonList.Add(secondaryButton);
            }
        }

        waitButtonUI.Open();
        buttonList.Add(waitButton);
		selection = 0;
		buttonList[selection].Select();

	}

    void ButtonsShowWait(bool allowBack = true)
    {
        //DO NOT POPULATE THE buttonList on this, move buttons cycle throug the directions and not the menu
        if(allowBack)
        {
            backButtonUI.Open();
        }
        ButtonsShowDirection();
    }

    void ButtonsHideAll()
    {
        buttonList.Clear();
        moveButtonUI.Close();// = moveButton.GetComponent<UIGenericButton>();
        attackButtonUI.Close();// = attackButton.GetComponent<UIGenericButton>();
        primaryButtonUI.Close();// = primaryButton.GetComponent<UIGenericButton>();
        secondaryButtonUI.Close();// = secondaryButton.GetComponent<UIGenericButton>();
        waitButtonUI.Close();// = waitButton.GetComponent<UIGenericButton>();
        confirmButtonUI.Close();// = confirmButton.GetComponent<UIGenericButton>();
        backButtonUI.Close();// = backButton.GetComponent<UIGenericButton>();
        targetUnitButtonUI.Close();// = targetUnitButton.GetComponent<UIGenericButton>();
        targetMapButtonUI.Close();// = targetMapButton.GetComponent<UIGenericButton>();
        waitNorthUI.Close();// = waitNorth.GetComponent<UIGenericButton>();
        waitEastUI.Close();// = waitEast.GetComponent<UIGenericButton>();
        waitSouthUI.Close();// = waitSouth.GetComponent<UIGenericButton>();
        waitWestUI.Close();// = waitWest.GetComponent<UIGenericButton>();
    }

    void ButtonsHideTop()
    {
        moveButtonUI.Close();
        attackButtonUI.Close();
        primaryButtonUI.Close();
        secondaryButtonUI.Close();
        waitButtonUI.Close();
    }

    void ButtonsShowConfirmCancel()
    {
        confirmButtonUI.Open();
        backButtonUI.Open();

		//don't want cancel in button list for this. spacebar is confirm rightclick is cancel
		//don't want this for CombatMoveTargetState, if wanted for another state make a new function

		////buttonList.Add(backButton);
		//buttonList.Clear();
		//buttonList.Add(confirmButton);
		//selection = 0;
		//buttonList[selection].Select();
    }

	void ButtonsShowConfirmCancelWalkAround()
	{
		confirmButtonUI.Open();
		backButtonUI.Open();
		//confirm button is always highlighted
		buttonList.Clear();
		buttonList.Add(confirmButton);
		selection = 0;
		buttonList[selection].Select();
	}

	void ButtonsShowCancel()
	{
		//confirmButtonUI.Open();
		backButtonUI.Open();
	}

	void ButtonsShowTargetUnitOrMap()
    {
        targetMapButtonUI.Open();
        targetUnitButtonUI.Open();
        backButtonUI.Open();

        //only only does the targetMap
        //selection = 0;
        ////buttonList.Add(targetUnitButton);
        //buttonList.Add(targetMapButton);
        ////buttonList.Add(backButton);
        //buttonList[selection].Select();

    }

    void ButtonsHideConfirmCancel()
    {
        confirmButtonUI.Close();
        backButtonUI.Close();
    }

    void ButtonsShowTarget() //not allowing back from here, in future maybe add it
    {
        targetUnitButtonUI.Open();
        targetMapButtonUI.Open();
    }

    void ButtonsHideTarget()
    {
        targetUnitButtonUI.Close();
        targetMapButtonUI.Close();
    }

    void ButtonsShowDirection()
    {
        waitNorthUI.Open();
        waitSouthUI.Open();
        waitEastUI.Open();
        waitWestUI.Open();
    }

    void ButtonsHideDirection()
    {
        waitNorthUI.Close();
        waitSouthUI.Close();
        waitEastUI.Close();
        waitWestUI.Close();
    }

    bool IsPrimaryEligible(CombatTurn turn)
    {
        if (StatusManager.Instance.IfStatusByUnitAndId(turn.actor.TurnOrder, NameAll.STATUS_ID_FROG, true) || turn.actor.ClassId == NameAll.CLASS_MIME )
        {
            return false;
        }
        return true;
    }

    bool IsSecondaryEligible(CombatTurn turn)
    {
        if (StatusManager.Instance.IfStatusByUnitAndId(turn.actor.TurnOrder, NameAll.STATUS_ID_FROG, true))
        {
            return false;
        }
        else if (PlayerManager.Instance.GetPlayerUnit(turn.actor.TurnOrder).AbilitySecondaryCode == NameAll.SECONDARY_NONE)
        {
            return false;
        }
        return true;
    }

    public void Open(CombatTurn turn)
    {
        if( !gameObject.activeSelf)
        {
            gameObject.SetActive(true); 
            EnableInputs();
        }
        
        buttonList.Clear();
        //Debug.Log("opening menu " );
        Alliances alliancesCheck = PlayerManager.Instance.GetAlliances(turn.actor.TeamId);
        if( alliancesCheck == Alliances.Hero || alliancesCheck == Alliances.Allied)
        {
            //gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_team_1");
            gameObject.GetComponent<Image>().color = allianceHeroColor;
        }
        else if (alliancesCheck == Alliances.Enemy )
        {
            //gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_team_2");
            gameObject.GetComponent<Image>().color = allianceEnemyColor;
        }
        else
        {
            //gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_neutral");
            gameObject.GetComponent<Image>().color = allianceNeutralColor;
        }        
    }

    //public void Open()
    //{
    //    gameObject.SetActive(true);
    //}

    public void Close()
    {

        DisableInputs();
        gameObject.SetActive(false);
        //Debug.Log("closing menu");
    }
    #endregion

    #region OnButtonClicked
    public void ActWait()
    {
        DidActiveTurnMenuNotification(ActiveTurnTopNotification, DidWaitClick);
        //Close(); //don't need to close I don't think;
    }

    public void ActMove()
    {
        DidActiveTurnMenuNotification(ActiveTurnTopNotification, DidMoveClick);
        //Close();

    }

    public void ActAttack()
    {
        DidActiveTurnMenuNotification(ActiveTurnTopNotification, DidAttackClick);
        //Close();
        
    }

    public void ActPrimary()
    {
        DidActiveTurnMenuNotification(ActiveTurnTopNotification, DidPrimaryClick);
        //Close();
    }

    public void ActSecondary()
    {
        DidActiveTurnMenuNotification(ActiveTurnTopNotification, DidSecondaryClick);
        //Close();
    }

    public void ActConfirm()
    {
        DidActiveTurnMenuNotification(ActiveTurnConfirmNotification, DidConfirmClick); //Debug.Log("act confirm");
        DidActiveTurnMenuNotification(ActiveTurnTopNotification, DidConfirmClick); //Debug.Log("act confirm");
        //Close();
    }

    public void ActTargetUnit()
    {
        DidActiveTurnMenuNotification(ActiveTurnTargetUnitMapNotification, DidTargetUnitClick);
        //Close();
      
    }

    public void ActTargetMap()
    {
        DidActiveTurnMenuNotification(ActiveTurnTargetUnitMapNotification, DidTargetMapClick);
        //Close();
      
    }

    public void ActBack()
    {
        DidActiveTurnMenuNotification(ActiveTurnBackNotification, DidBackClick); //Debug.Log("act cancel");
        DidActiveTurnMenuNotification(ActiveTurnTopNotification, DidBackClick);
        //Close();
    }

    public void ActNorth()
    {
        DidActiveTurnMenuNotification(ActiveTurnWaitNotification, DidNorthClick);
        //used in CombatEndFacingState and picking direction in CombatTargetAbilityState
    }

    public void ActEast()
    {
        DidActiveTurnMenuNotification(ActiveTurnWaitNotification, DidEastClick);
        //Close();
    }

    public void ActSouth()
    {
        DidActiveTurnMenuNotification(ActiveTurnWaitNotification, DidSouthClick);
        //Close();
    }

    public void ActWest()
    {
        DidActiveTurnMenuNotification(ActiveTurnWaitNotification, DidWestClick);
        //Close();
    }
    #endregion

  
}
