using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIUnitListScrollList : MonoBehaviour
{
    public Button backButton;
    public GameObject sampleButton;
    public Transform contentPanel;
    public UIUnitInfoPanel unitInfoMenu;

    //List<string> combatLogList = new List<string>();
    List<PlayerUnit> puList;
    // Use this for initialization

    Color neutralColor = new Color32(126, 209, 232, 255);
    Color team1Color = new Color32(236, 142, 47, 255);
    Color team2Color = new Color32(129, 77, 197, 255);

    void Awake()
    {
        List<PlayerUnit> puList = new List<PlayerUnit>();
    }


    public void Open()
    {
        //Debug.Log("wtf who opened");
        gameObject.SetActive(true);
        puList = new List<PlayerUnit>();
        puList.Clear();
        PopulateNames();
    }

    public void Close()
    {
        
        gameObject.SetActive(false);
    }

    public void ActBack()
    {
        //closes this menu and reopens the other menu
        Close();
        //MapTileManager.Instance.UnhighlightAllTiles();
    }

    void PopulateNames()
    {
        //Debug.Log("populating turns names neu");
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        List<PlayerUnit> puList = PlayerManager.Instance.GetPlayerUnitList();
        
        foreach (PlayerUnit pu in puList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            UIUnitListButton tb = newButton.GetComponent<UIUnitListButton>();
            int tempInt = pu.TurnOrder; //Debug.Log("creating unit list, turn order is " + pu.TurnOrder);
            tb.title.text = " " + tempInt + ": " + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY, pu.ClassId) + " " + pu.UnitName;
            tb.transform.SetParent(contentPanel);
            tb.index = tempInt;

            Button tempButton = tb.GetComponent<Button>();
            SetButtonColor(newButton, pu.TeamId);
            tempButton.onClick.AddListener(() => ButtonClicked(tempInt));
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

    void ButtonClicked(int index)
    {
        //ADD CODE HERE TO SHOW WHERE ON THE MAP THE CLICK IS
        //Debug.Log("Yatta, unit list button pressed..." + index);
        PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(index);
        unitInfoMenu.PopulatePlayerInfo(pu, true);
    }

    public void CloseUnitInfoMenu()
    {
        unitInfoMenu.Close();
    }
}

