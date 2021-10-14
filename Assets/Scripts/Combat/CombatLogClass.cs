using UnityEngine;
using System.Collections.Generic;

public class CombatLogClass : MonoBehaviour {
	/// <summary>
	/// Saves actions and rolls to a combatlog menu option so players can see what happened in the past
	/// </summary>
    const string CombatMenuAdd = "CombatMenu.AddItem";

    //holds combatlog objects for the combatLog
    public string Message { get; set; }
    public int UnitId { get; set; }
	public int RenderMode { get; set; }

	public CombatLogClass(string message, int unitId, int renderMode)
    {
        Message = message;
        UnitId = unitId;
		RenderMode = renderMode;
    }

    public void SendNotification()
    {

		//List<CombatLogClass> tempList = new List<CombatLogClass>();
		//tempList.Add(this);
		//Debug.Log("posting notification");
		if (this.RenderMode != NameAll.PP_RENDER_NONE)
			this.PostNotification(CombatMenuAdd, this);
    }

}
