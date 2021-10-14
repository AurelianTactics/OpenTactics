using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIMenuMenu : MonoBehaviour {

    public Button menuButton;
    public Button atButton;
    public Button logButton;
    public Button statsButton;
    public Button wheelButton;
	public Button resetButton;
	public Button quitButton;

    UIGenericButton menuButtonUI;
    UIGenericButton atButtonUI;
    UIGenericButton wheelButtonUI;
    UIGenericButton logButtonUI;
    UIGenericButton statsButtonUI;
	UIGenericButton resetButtonUI;
	UIGenericButton quitButtonUI;

    [SerializeField]
    GameObject combatLogMenuField;
    UICombatLogScrollList combatLogMenu;

    [SerializeField]
    GameObject turnsMenuField;
    UITurnsScrollList turnsMenu;

    [SerializeField]
    GameObject unitListMenuField;
    UIUnitListScrollList unitListMenu;

    [SerializeField]
    UICombatStats uiCombatStatsScript;

    //stores the combatLogMessages for display
    List<CombatLogClass> combatLogList = new List<CombatLogClass>();

    private static bool toggleMaximize;

    void Awake()
    {
        menuButtonUI = menuButton.GetComponent<UIGenericButton>();
        atButtonUI = atButton.GetComponent<UIGenericButton>();
        wheelButtonUI = wheelButton.GetComponent<UIGenericButton>();
        logButtonUI = logButton.GetComponent<UIGenericButton>();
        statsButtonUI = statsButton.GetComponent<UIGenericButton>();
		resetButtonUI = resetButton.GetComponent<UIGenericButton>();
		quitButtonUI = quitButton.GetComponent<UIGenericButton>();

        menuButtonUI.SetText("Menu");
        atButtonUI.SetText("Turns");
        wheelButtonUI.Close();
        //wheelButtonUI.SetText("Wheel");
        logButtonUI.SetText("Log");
        statsButtonUI.SetText("Stats");
		resetButtonUI.SetText("Reset");
		quitButtonUI.SetText("Quit");
        toggleMaximize = true;
        Minimize();

        combatLogMenu = combatLogMenuField.GetComponent<UICombatLogScrollList>();
        turnsMenu = turnsMenuField.GetComponent<UITurnsScrollList>();
        unitListMenu = unitListMenuField.GetComponent<UIUnitListScrollList>();
    }

    // Use this for initialization
    public void Open () {
        gameObject.SetActive(true);
        toggleMaximize = true;
        Minimize();
	}

    public void Close()
    {
        gameObject.SetActive(false);
    }
	
    public void ToggleMenu()
    {
        if( toggleMaximize)
        {
            OpenMenu(); 
        }
        else
        {
            CloseMenu();
        }
        toggleMaximize = !toggleMaximize;
    }

    public void ShowTurnsMenu() 
    {
        if (!turnsMenuField.activeSelf)
        {
            turnsMenu.Open();
            HideStatsMenu();
            HideCombatMenu();
        }
        else
            HideTurnsMenu();
        //Close();
    }

    public void HideTurnsMenu()
    {
        turnsMenu.Close();
        //Open();
    }

    public void ShowWheelMenu()
    {
		//to do: so people can use color/number wheel for compat at some point
    }

    public void ShowCombatMenu()
    {
        if (!combatLogMenuField.activeSelf)
        {
            combatLogMenu.Open(combatLogList);
            HideTurnsMenu();
            HideStatsMenu();
        }
        else
            HideCombatMenu();
        //Close();
    }

    public void ShowStatsMenu()
    {
        if (!unitListMenuField.activeSelf)
        {
            unitListMenu.Open();
            HideTurnsMenu();
            HideCombatMenu();
        }
        else
        {
            HideStatsMenu();
        }
            
        //Close();
    }

    public void HideStatsMenu()
    {
        unitListMenu.CloseUnitInfoMenu();
        unitListMenu.Close();
        //Open();
    }

    public void HideCombatMenu()
    {
        combatLogMenu.Close();
        //Open();
    }

    public void ShowQuitMenu()
    {
        GetComponent<DialogController>().Show("Quit Game", "Are you sure you want to quit?", ConfirmExit, null);
    }

	public void ShowResetMenu()
	{
		GetComponent<DialogController>().Show("Reset Level", "Are you sure you want to reset?", ConfirmReset, null);
	}

	public void ShowCombatStats(CombatStats cs = null)
    {
        if (cs == null)
            cs = PlayerManager.Instance.GetCombatStats();

        if( cs != null)
            uiCombatStatsScript.Open(cs);
    }

    void ConfirmExit()
    {
        //Debug.Log("confirming exit");
        bool isSelfQuit = true;
        this.PostNotification(NameAll.NOTIFICATION_EXIT_GAME, isSelfQuit);

        //GameObject go = GameObject.Find("PlayerManagerObject(Clone)");
        //Destroy(go);
        //if (!PhotonNetwork.offlineMode)
        //{
        //    //PhotonNetwork.Disconnect();
        //    go = GameObject.Find("ChatGameObject");
        //    Destroy(go);
        //    PhotonNetwork.LeaveRoom();
        //    SceneManager.LoadScene(NameAll.SCENE_MP_MENU);
        //}
        //else
        //{
        //    this.PostNotification(NameAll.NOTIFICATION_EXIT_GAME);
        //    //SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
        //}
        
    }

	void ConfirmReset()
	{
		//Debug.Log("confirming reset");
		bool isSelfReset = true;
		this.PostNotification(NameAll.NOTIFICATION_RESET_GAME, isSelfReset);

	}

	void OpenMenu()
    {
        Maximize();
    }

    void CloseMenu()
    {
        Minimize();
    }

    void Minimize()
    {
        //just show menu and AT
        menuButtonUI.SetText("Menu");
        //wheelButtonUI.Close();
        logButtonUI.Close();
        statsButtonUI.Close();
		resetButtonUI.Close();
		quitButtonUI.Close();
    }

    void Maximize()
    {
        //show all fields
        menuButtonUI.SetText("Hide");
        //wheelButtonUI.Open();
        logButtonUI.Open();
        statsButtonUI.Open();
		resetButtonUI.Open();
		quitButtonUI.Open();
    }

    #region notifications
    const string TurnsMenuAdd = "TurnsMenu.AddItem";
    const string CombatMenuAdd = "CombatMenu.AddItem";
    const string TickMenuAdd = "TickMenu.AddItem"; //shows the current game tick
    const string CombatStatsShow = "CombatStats.Show";


    void OnEnable()
    {
        this.AddObserver(OnMenuMenuNotification, TurnsMenuAdd);
        this.AddObserver(OnMenuCombatNotification, CombatMenuAdd);
        this.AddObserver(OnMenuTickNotification, TickMenuAdd);
        this.AddObserver(OnCombatStatsShowNotification, CombatStatsShow);
    }

    void OnDisable()
    {
        this.RemoveObserver(OnMenuMenuNotification, TurnsMenuAdd);
        this.RemoveObserver(OnMenuCombatNotification, CombatMenuAdd);
        this.RemoveObserver(OnMenuTickNotification, TickMenuAdd);
        this.RemoveObserver(OnCombatStatsShowNotification, CombatStatsShow);
    }

    void OnMenuMenuNotification(object sender, object args)
    {
        //object sent is a dictionary with the pu.TurnOrder and the SpellName
        Dictionary<int, SpellName> tempDict = args as Dictionary<int, SpellName>;
        SpellName sn = null;
        PlayerUnit pu = null;
        foreach(KeyValuePair<int,SpellName> kvp in tempDict)
        {
            sn = kvp.Value;
            pu = PlayerManager.Instance.GetPlayerUnit(kvp.Key);
        }

        
        if( pu != null && sn != null)
        {
            if (!turnsMenuField.activeSelf)
            {
                turnsMenu.Open();
            }
            turnsMenu.AddSpellName(pu, sn);
        }
            

        
    }

    //adds string to the combatLog, combatLog shown when someone opens the menu
    void OnMenuCombatNotification(object sender, object args)
    {
        //List<CombatLogClass> tempList = args as List<CombatLogClass>;
        //foreach( CombatLogClass cll in tempList)
        //{
        //    combatLogList.Insert(0, cll);
        //}
        //Debug.Log("handling menu combat notification");
        try
        {
            CombatLogClass cll = args as CombatLogClass; //Debug.Log("handling menu combat notification");
            combatLogList.Insert(0, cll); //Debug.Log("handling menu combat notification");

            if (combatLogList.Count > 50)
            {
                //Debug.Log("testing combatLog remove feature, change from 10 to 100 after being tested");
                combatLogList.RemoveRange(50, combatLogList.Count);
            }
        }
        catch( Exception e)
        {
            //Debug.Log("failed combatLog notification " + e.ToString());
        }
        
    }

    void OnMenuTickNotification(object sender, object args)
    {
        //object sent is a dictionary with the pu.TurnOrder and the SpellName
        //int tempInt = (int)args;
        Dictionary<string, int> tempDict = args as Dictionary<string, int>;
        string zString = "Turns";
        if (tempDict.ContainsKey("currentTick"))
            zString += " " + tempDict["currentTick"];
        if (tempDict.ContainsKey("roomTick"))
        {
            zString += " " + tempDict["roomTick"] + " " + PlayerManager.Instance.GetMPSelfPhase();
        }
            

        atButtonUI.SetText(zString);
    }

    void OnCombatStatsShowNotification(object sender, object args)
    {
        //guess assume that the arg is CombatStats object. could just grab it from PlayerManager but whatever
        CombatStats cs = args as CombatStats;
        if (cs != null)
        {
            ShowCombatStats();
        }
    }
    #endregion
}
