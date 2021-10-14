using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles logic for UI interactions with team lists of units
/// Scroll list appears in CustomGame scene so players can select units for combat
/// </summary>
/// <remarks>
/// Additional logic for various draft modes used for selecting units
/// </remarks>
public class CustomGameTeamScrollList : MonoBehaviour {

    [SerializeField]
    private UIUnitInfoPanel statPanel;

    //public GameObject sampleButton;
    public Transform contentPanel;

    public GameObject pickBanButton;

    [SerializeField]
    MPDraftInfo mpDraft;
//    List<PlayerUnit> puList;

    //void Awake()
    //{
    //    List<PlayerUnit> puList = new List<PlayerUnit>();
    //}

    //public void Open()
    //{
    //    //gameObject.SetActive(true);
    //    puList = new List<PlayerUnit>();
    //    puList.Clear();
    //    PopulateNames();
    //}

    //public void Close()
    //{
    //    SceneCreate.menuMenu.Open();
    //    gameObject.SetActive(false);
    //}

    //populate with red or green depending
    public void PopulateNames(List<PlayerUnit> puList)
    {
        //Debug.Log("populating turns names neu");
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        //List<PlayerUnit> puList = PlayerManager.Instance.GetPlayerUnitList();

        foreach (PlayerUnit pu in puList)
        {
            GameObject newButton = Instantiate(pickBanButton) as GameObject;
            MPPickBanButton tb = newButton.GetComponent<MPPickBanButton>();

            tb.title.text = " " + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY, pu.ClassId) + " / "
               + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_SECONDARY, pu.AbilitySecondaryCode);
            tb.details.text = " " + pu.Level + " | " + pu.StatTotalPA + "/" + pu.StatTotalMA + "/" + pu.StatTotalAgi + " | " + pu.StatTotalBrave + "/" + pu.StatTotalFaith + "/" + pu.StatTotalCunning;
            tb.transform.SetParent(contentPanel);
            //int tempInt = pu.TurnOrder; //Debug.Log("creating unit list, turn order is " + pu.TurnOrder);
            //tb.title.text = " " + tempInt + ": " + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY, pu.ClassId) + " " + pu.UnitName;
            tb.pickBanButton.gameObject.SetActive(false);
            //tb.index = tempInt;
            string str = CalcCode.BuildStringFromPlayerUnit(pu);
            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonClicked(str));
        }
    }

    //void ButtonClicked(int index)
    //{
    //    //ADD CODE HERE TO SHOW WHERE ON THE MAP THE CLICK IS
    //    Debug.Log("Yatta, unit list button pressed..." + index);
    //    PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(index);
    //    SceneCreate.unitInfoMenu.PopulatePlayerInfo(pu,false);
    //}

    void ButtonClicked(string str)
    {
        //ADD CODE HERE TO SHOW WHERE ON THE MAP THE CLICK IS
        //Debug.Log("Yatta, unit list button pressed..." + str);// + index);
        PlayerUnit pu = CalcCode.BuildPlayerUnit(str);
        statPanel.PopulatePlayerInfo(pu, false);
        //SceneCreate.unitInfoMenu.PopulatePlayerInfo(pu, false);
    }


    public void PopulatePickBanNames(List<PlayerUnit> puList, int pickOrder, int lastPick, int pickingTeam, bool offlineMode, bool isMasterClient)
    {
        //Debug.Log("testing populate");
        int phase;
        if( (pickOrder >= 1 && pickOrder <= 4) || ( pickOrder >= 9 && pickOrder <= 12 ) ) //bans
        {
            phase = 0;
        }
        else if(pickOrder < 1 || pickOrder > lastPick ) //pre picks
        {
            phase = 2;
        }
        else //pre picks/bans
        {
            phase = 1;
        }
        //Debug.Log("populating turns names neu");
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        //List<PlayerUnit> puList = PlayerManager.Instance.GetPlayerUnitList();
        int i = 0;
        foreach (PlayerUnit pu in puList)
        {
            GameObject newButton = Instantiate(pickBanButton) as GameObject;
            MPPickBanButton tb = newButton.GetComponent<MPPickBanButton>();
            int tempInt = i; //Debug.Log("creating unit list, turn order is " + pu.TurnOrder);
            tb.title.text = " " + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY, pu.ClassId) + " / "
                + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_SECONDARY, pu.AbilitySecondaryCode);
            tb.details.text = " " + pu.Level + " | " + pu.StatTotalPA + "/" + pu.StatTotalMA + "/" + pu.StatTotalAgi + " | " + pu.StatTotalBrave + "/" + pu.StatTotalFaith + "/" + pu.StatTotalCunning;
            tb.transform.SetParent(contentPanel);
            //tb.index = tempInt;
            string str = CalcCode.BuildStringFromPlayerUnit(pu);
            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonClicked(str));

            //Debug.Log("offline mode is " + offlineMode);
            if (phase == 0) //bans
            {
                //tb.pickbanbutton is one thing
                //sb.infoButton.onClick.AddListener(() => InfoButtonClicked(tempInt)); //gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_team_1");
                if (offlineMode || (isMasterClient && pickingTeam == NameAll.TEAM_ID_GREEN) || (!isMasterClient && pickingTeam == NameAll.TEAM_ID_RED))
                {
                    tb.pickBanButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("ban"); Debug.Log("is turn, setting pick ban button ON");
                    tb.pickBanButton.onClick.AddListener(() => PickBanButtonClicked(str, phase, tempInt, pickingTeam));
                }
                else
                {
                    Debug.Log("not turn, setting pick ban button off");
                    tb.pickBanButton.gameObject.SetActive(false); //disable pickban button when not your turn
                }
            }
            else if( phase == 1) //picks
            {
                //tb.pickbanbutton is something else
                //sb.infoButton.onClick.AddListener(() => InfoButtonClicked(tempInt));
                if (offlineMode || (isMasterClient && pickingTeam == NameAll.TEAM_ID_GREEN) || (!isMasterClient && pickingTeam == NameAll.TEAM_ID_RED))
                {
                    tb.pickBanButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("pick"); Debug.Log("is turn, setting pick ban button ON");
                    tb.pickBanButton.onClick.AddListener(() => PickBanButtonClicked(str, phase, tempInt, pickingTeam));
                }
                else
                {
                    Debug.Log("not turn, setting pick ban button off");
                    tb.pickBanButton.gameObject.SetActive(false); //disable pickban button when not your turn
                } 
            }
            else if( phase == 2) //pre or after draft
            {
                //pre draft phase
                tb.pickBanButton.gameObject.SetActive(false);
            }
            i++;
        }
    }

    void PickBanButtonClicked(string str, int phase, int index, int zPickingTeam)
    {
        PlayerUnit pu = CalcCode.BuildPlayerUnit(str);
        statPanel.PopulatePlayerInfo(pu, false); //Debug.Log("asdf");
        if(phase == 0)
        {
            mpDraft.UpdateBanList(index);
            //mpDraft.UpdatePickList()
        }
        else if( phase == 1)
        {
            mpDraft.UpdatePickList(index, zPickingTeam);
        }
        //do something with MPDraftInfo

    }
}
