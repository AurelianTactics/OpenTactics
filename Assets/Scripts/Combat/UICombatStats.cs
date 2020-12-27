using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//shows the stats
//for now assumign only shows post combat. in the future can modify this

public class UICombatStats : MonoBehaviour {

    public GameObject sampleButton;
    public Transform contentPanel;

    CombatStats combatStats;

    

    public void Open(CombatStats cs)
    {
        combatStats = cs;
        PopulateScrollList();
    }

    #region scrolllist display
    void PopulateScrollList()
    {
        

        //Debug.Log("populating turns names neu");
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildStatList();

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            //Button tempButton = tb.GetComponent<Button>();
            //tempButton.onClick.AddListener(() => ButtonLevelClicked(tempInt));
        }
    }

    List<AbilityBuilderObject> BuildStatList()
    {
        //var retValue = new List<AbilityBuilderObject>();
        return combatStats.GetDisplayList(NameAll.TEAM_ID_GREEN); //combat stats turns the data into an abilitybuilder object
    }

    //no button click so no function needed at this time

    public void Close()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
