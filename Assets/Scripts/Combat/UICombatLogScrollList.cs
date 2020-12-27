using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UICombatLogScrollList : MonoBehaviour
{
    public Button backButton;
    public GameObject sampleButton;
    public Transform contentPanel;

    //List<CombatLogClass> combatLogList = new List<CombatLogClass>(); 
    //UIBackButton backButtonUI;
    //const string DidStatusEnd = "Status.DidEnd";
    //const string DidCombatLog = "CombatLog.DidMessage";

    //// Use this for initialization
    //void Start()
    //{
    //    backButtonUI = backButton.GetComponent<UIBackButton>();
    //}

    //void OnEnable()
    //{
    //    this.AddObserver(OnStatusEnd, DidStatusEnd);
    //    this.AddObserver(OnMessage, DidCombatLog);
    //}

    //void OnDisable()
    //{
    //    this.RemoveObserver(OnStatusEnd, DidStatusEnd);
    //    this.RemoveObserver(OnMessage, DidCombatLog);
    //}

    //void OnStatusEnd(object sender, object args)
    //{
    //    var statusList = StatusManager.Instance.GetStatusTickTemp();
    //    foreach( StatusObject ss in statusList)
    //    {
    //        AddToLog(string.Format("Status {0} ends on {1}", 
    //            NameAll.GetStatusString( ss.GetStatusId() ), 
    //            PlayerManager.Instance.GetPlayerUnit( ss.GetUnitId() ).UnitName ),
    //            ss.GetUnitId() );
    //    }
    //}

    //void OnMessage(object sender, object args)
    //{
    //    CombatLogClass clObject = (CombatLogClass) args;
    //    AddToLog(clObject);
    //}

    //void AddToLog(string message, int unitId)
    //{
    //    var clObject = new CombatLogClass(message, unitId);
    //    AddToLog(clObject);
        
    //}

    //void AddToLog(CombatLogClass clObject)
    //{
    //    combatLogList.Insert(0, clObject);
    //    if (combatLogList.Count > 50)
    //    {
    //        combatLogList.RemoveRange(49, combatLogList.Count - 49);
    //    }
    //}

    public void Open(List<CombatLogClass> clcList)
    {
        gameObject.SetActive(true);
        PopulateCombatLogNames(clcList); 
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ActBack()
    {
        //closes this menu and reopens the other menu
        Close();
    }

    void PopulateCombatLogNames(List<CombatLogClass> clcList)
    {
        //Debug.Log("populating turns names neu");
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
        int z1 = 0;
        foreach( CombatLogClass cl in clcList )
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            UICombatLogButton tb = newButton.GetComponent<UICombatLogButton>();
            string unitName = PlayerManager.Instance.GetPlayerUnit(cl.UnitId).UnitName;
            tb.title.text = "" + z1 + " - " + unitName +": " + cl.Message;
            tb.transform.SetParent(contentPanel);
            tb.index = z1;
            int tempInt = tb.index;
            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonClicked(tempInt, cl.UnitId));
            z1++;
        }
    }

    void ButtonClicked(int index, int unitId)
    {
        //ADD CODE HERE TO SHOW WHERE ON THE MAP THE CLICK IS
        Debug.Log("Yatta, combat log button pressed, add functionality for highlighting/moving camera"+ unitId);
    }
}

