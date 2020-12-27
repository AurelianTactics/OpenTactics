using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UITurnsScrollList : MonoBehaviour {

    public Button backButton;
    public GameObject turnListButton;
    public Transform contentPanel;
    List<TurnObject> spellNameList = new List<TurnObject>();
    UIBackButton backButtonUI;

    Color neutralColor = new Color32(126, 209, 232, 255);
    Color team1Color = new Color32(236, 142, 47, 255);
    Color team2Color = new Color32(129, 77, 197, 255);

    // Use this for initialization
    void Start()
    {
        backButtonUI = backButton.GetComponent<UIBackButton>();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        //Debug.Log("turns menu opened");
        PopulateTurnsNames();
        
        //int commandSet;
        //if (SceneCreate.phaseMenu == 4) //secondary
        //{
        //    commandSet = PlayerManager.Instance.GetPlayerUnit(SceneCreate.active_unit).AbilitySecondaryCode;
        //}
        //else
        //{
        //    commandSet = PlayerManager.Instance.GetPlayerUnit(SceneCreate.active_unit).ClassId;
        //}
        //gameObject.SetActive(true);
        //spellNameList.Clear();
        ////Debug.Log("list size is " + spellNameList.Count);
        //PopulateSpellNames(commandSet);
    }

    public void Close()
    {
        //SceneCreate.menuMenu.Open();
        gameObject.SetActive(false);
    }

    public void ActBack()
    {
        //closes this menu and reopens the other menu
        Close();
        //MapTileManager.Instance.UnhighlightAllTiles();
    }

    void PopulateTurnsNames()
    {
        //Debug.Log("populating turns names neu");
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        TurnsManager.Instance.RecreateTurnsList();
        foreach (TurnObject t in TurnsManager.Instance.GetTurnsList())
        {
            GameObject newButton = Instantiate(turnListButton) as GameObject;
            UITurnsListButton tb = newButton.GetComponent<UITurnsListButton>();

            int tempInt = t.GetTurnId();
            tb.title.text = " "  + tempInt + ": " + t.GetTitle();
            tb.transform.SetParent(contentPanel);
            tb.turnsIndex = tempInt;
            
            Button tempButton = tb.GetComponent<Button>();
            SetButtonColor(newButton, t.GetTeamId());
            tempButton.onClick.AddListener(() => PlayerUnitButtonClicked(t.actorId));

            //add the button to add a queued up object
            tb.addQueueButton.onClick.AddListener(() => AddQueueButtonClicked(t.actorId));

            if(PlayerManager.Instance.IsWalkAroundActionObjectInQueue(t.actorId))
            {
                tb.displayQueueButton.enabled = true;
                tb.displayQueueButton.onClick.AddListener(() => DisplayQueueButtonClicked(t.actorId,tb.queueInfoButton));
                tb.cancelQueueButton.enabled = true;
                tb.cancelQueueButton.onClick.AddListener(() => CancelQueueButtonClicked(t.actorId, tb.cancelQueueButton));
            }
        }
    }

    void SetButtonColor(GameObject go, int teamId)
    {
        if (teamId == NameAll.TEAM_ID_GREEN)
        {
            //go.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_team_1");
            go.GetComponent<Image>().color = team1Color;
        }
        else if (teamId == NameAll.TEAM_ID_RED)
        {
            //go.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_team_2");
            go.GetComponent<Image>().color = team2Color;
        }
        else
        {
            //go.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_neutral");
            go.GetComponent<Image>().color = neutralColor;
        }
    }

    //void PopulateSpellNames(int commandSet)
    //{
    //    foreach (Transform child in contentPanel)
    //    {
    //        GameObject.Destroy(child.gameObject);
    //    }
    //    spellNameList = SpellManager.Instance.GetSpellNamesByCommandSet(commandSet);
    //    foreach (SpellName sn in spellNameList)
    //    {
    //        GameObject newButton = Instantiate(sampleButton) as GameObject;
    //        UISampleButton sb = newButton.GetComponent<UISampleButton>();
    //        sb.spellName.text = sn.GetSpellName();
    //        sb.transform.SetParent(contentPanel);
    //        sb.spellId = sn.GetSpellId();
    //        //sb.button.onClick = myClick;
    //        int tempInt = sn.GetIndex();

    //        //sb.button.onClick.AddListener(() => ButtonClicked(tempInt));
    //        Button tempButton = sb.GetComponent<Button>();
    //        //int tempInt = i;

    //        tempButton.onClick.AddListener(() => ButtonClicked(tempInt));
    //    }
    //}

    void PlayerUnitButtonClicked(int unitId)
    {
        //ADD CODE HERE TO SHOW WHERE ON THE MAP THE CLICK IS
        if( unitId == NameAll.NULL_UNIT_ID) //hypothetical turn, can't click on it
        {
            return;
        }
        SendTurnsListClickPU(unitId);
    }

    void SpellNameButtonClicked(SpellName sn)
    {
        //ADD CODE HERE TO SHOW WHERE ON THE MAP THE CLICK IS
        SendTurnsListClickSlowAction(sn);
    }

    void AddQueueButtonClicked(int unitId)
    {
        //ADD CODE HERE TO SHOW WHERE ON THE MAP THE CLICK IS
        if (unitId == NameAll.NULL_UNIT_ID) //hypothetical turn, can't click on it
        {
            return;
        }
        SendTurnsListClickPU(unitId);
    }

    void DisplayQueueButtonClicked(int unitId, Button queueInfoButton)
    {
        if( queueInfoButton.enabled)
        {
            queueInfoButton.enabled = false;
        }
        else
        {
            //GET TEXT FROM A WAAO OBJECT
            queueInfoButton.enabled = true;
            queueInfoButton.GetComponent<Text>().text = PlayerManager.Instance.GetWalkAroundActionObjectDescription(unitId);
        }
    }

    void CancelQueueButtonClicked(int turnsIndex, Button cancelButton)
    {
        if (turnsIndex == NameAll.NULL_UNIT_ID) //hypothetical turn, can't click on it
        {
            return;
        }
        PlayerManager.Instance.RemoveWalkAroundObjectByUnitId(turnsIndex);
        cancelButton.enabled = false;
    }

    //called from UIMenuMenu after receiving a notification from an ability list click
    public void AddSpellName(PlayerUnit pu, SpellName sn)
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        TurnsManager.Instance.InsertTurn(pu, sn);
        //don't need to recreate, already created
        //TurnsManager.Instance.RecreateTurnsList();
        foreach (TurnObject t in TurnsManager.Instance.GetTurnsList())
        {
            GameObject newButton = Instantiate(turnListButton) as GameObject;
            UITurnsListButton tb = newButton.GetComponent<UITurnsListButton>();

            int tempInt = t.GetTurnId();
            if( t.GetTurnId() == NameAll.NULL_UNIT_ID) //hypothetical ability
                tb.title.text = " " + t.GetTitle();
            else
                tb.title.text = " " + tempInt + ": " + t.GetTitle();

            tb.transform.SetParent(contentPanel);
            tb.turnsIndex = tempInt;

            Button tempButton = tb.GetComponent<Button>();
            SetButtonColor(newButton, t.GetTeamId());
            tempButton.onClick.AddListener(() => SpellNameButtonClicked(sn));
        }
    }

    //notifications
    const string TurnsListClickPU = "TurnsListClick.PlayerUnit";
    const string TurnsListClickSlowAction = "TurnsListClick.SlowAction";

    public void SendTurnsListClickPU(int unitId)
    {
        this.PostNotification(TurnsListClickPU, unitId);
    }

    public void SendTurnsListClickSlowAction(SpellName sn)
    {
        this.PostNotification(TurnsListClickSlowAction, sn);
    }

    //listeners
    //if this menu is open, listens to things that cause the list to need to be redone and redoes the list
    const string refreshTurns = "refreshTurns";

    void EnableObservers()
    {
        this.AddObserver(OnRefreshTurns, refreshTurns);
    }

    void DisableObservers()
    {
        this.RemoveObserver(OnRefreshTurns, refreshTurns);
    }

    void OnRefreshTurns(object sender, object args)
    {
        PopulateTurnsNames();
    }
}
