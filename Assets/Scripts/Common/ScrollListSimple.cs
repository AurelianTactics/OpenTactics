using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScrollListSimple : MonoBehaviour {

    //public Button backButton;
    public GameObject sampleButton;
    public Transform contentPanel;

    int notificationType = NameAll.NULL_INT;
    
    public void Open<T>(List<T> genericList, int notificationInt = -1919)
    {
        gameObject.SetActive(true);
        notificationType = notificationInt;
        PopulateList(genericList);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }


    void PopulateList<T>(List<T> genericList)
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        
        for (int i = 0; i < genericList.Count; i++)
        {
            //Debug.Log("item size" + itemList.Count);
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i;
            var listItem = genericList[i];

            tb.title.text = GetListString(listItem, 0);
            tb.details.text = GetListString(listItem, 1);
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonClicked(tempInt));
        }
    }

    const string SimpleListNotification = "SimpleListNotification";
    void ButtonClicked(int listIndex)
    {
        List<int> tempList = new List<int>();
        tempList.Add(notificationType);
        tempList.Add(listIndex);
        this.PostNotification(SimpleListNotification, tempList);
    }


    string GetListString<T>(T listItem, int type)
    {
        //if (typeof(T) == typeof(PlayerUnit))
        //{
        //    PlayerUnit pu = listItem;
        //    if (type == 0)
        //        return CalcCode.GetTitleFromPlayerUnit(listItem);
        //    else if (type == 1)
        //        return CalcCode.GetDetailsFromPlayerUnit(listItem);
        //}


        return "";
    }

    string GetListString(PlayerUnit pu, int type)
    {
        //PlayerUnit pu = (PlayerUnit)listItem;
        if (type == 0)
            return CalcCode.GetTitleFromPlayerUnit(pu);
        else if (type == 1)
            return CalcCode.GetDetailsFromPlayerUnit(pu);


        return "";
    }

}
