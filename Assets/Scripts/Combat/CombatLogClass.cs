using UnityEngine;
using System.Collections.Generic;

public class CombatLogClass : MonoBehaviour {

    const string CombatMenuAdd = "CombatMenu.AddItem";

    //holds combatlog objects for the combatLog
    public string Message { get; set; }
    public int UnitId { get; set; }

    public CombatLogClass(string message, int unitId)
    {
        Message = message;
        UnitId = unitId;
    }

    public void SendNotification()
    {
        //List<CombatLogClass> tempList = new List<CombatLogClass>();
        //tempList.Add(this);
        //Debug.Log("posting notification");
        this.PostNotification(CombatMenuAdd, this);
    }

}
