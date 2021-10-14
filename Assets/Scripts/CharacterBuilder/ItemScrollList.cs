using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ItemScrollList : MonoBehaviour
{
    //public Button backButton;
    public GameObject sampleButton;
    public Transform contentPanel;

    //List<string> combatLogList = new List<string>();
    //UIBackButton backButtonUI;
    List<ItemObject> itemList;
    // Use this for initialization

    int slot = 0;
    PlayerUnit pu;

    void Awake()
    {
        List<ItemObject> itemList = new List<ItemObject>();
    }


    public void Open(PlayerUnit puIn)
    {
        //Debug.Log("wtf who opened");
        gameObject.SetActive(true);
        itemList = new List<ItemObject>();
        itemList.Clear();
        pu = puIn;
        PopulateNames(pu);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void PopulateNames(PlayerUnit pu)
    {
        //Debug.Log("populating turns names neu");
        itemList = ItemManager.Instance.GetItemsBySlotAndUnit(slot, pu);
        PopulateInner();
        
    }

    void PopulateInner()
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (ItemObject i in itemList)
        {
            //Debug.Log("item size" + itemList.Count);
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            ItemScrollListButton tb = newButton.GetComponent<ItemScrollListButton>();
            int tempInt = i.ItemId;
            //Debug.Log("asdf" + i.ItemName + i.Level);
            //tb.title.text = "asdf";
            tb.title.text = " " + i.ItemName + " (" + i.Level + ")";
            tb.mainStat.text = ItemManager.Instance.GetMainStat(i) + " ";
            tb.details.text = " " + ItemManager.Instance.GetDetails(i);
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonClicked(tempInt));
        }
    }

    const string CharacterBuilderNotification = "CharacterBuilderNotification";
    void ButtonClicked(int index)
    {
        //ADD CODE HERE TO SHOW WHERE ON THE MAP THE CLICK IS
        //Debug.Log("Yatta, unit list button pressed..." + index);
        CharacterUIController.pu.EquipItem(index, slot);
        this.PostNotification(CharacterBuilderNotification);
        //CharacterUIController.itemUpdate = 1;
    }

    //change the slot and what items are shown
    void SetSlot( int z1)
    {
        if( z1 >= 0 && z1 <= 4)
        {
            slot = z1;
            PopulateNames(pu);
        }
    }

    public void OnClickWeapon()
    {
        SetSlot(NameAll.ITEM_SLOT_WEAPON);
    }

    public void OnClickOffhand()
    {
        SetSlot(NameAll.ITEM_SLOT_OFFHAND);
    }

    public void OnClickHead()
    {
        SetSlot(NameAll.ITEM_SLOT_HEAD);
    }

    public void OnClickBody()
    {
        SetSlot(NameAll.ITEM_SLOT_BODY);
    }

    public void OnClickOther()
    {
        SetSlot(NameAll.ITEM_SLOT_ACCESSORY);
    }

    public void SortName()
    {
        itemList.Sort(delegate (ItemObject x, ItemObject y)
       {
           return x.ItemName.CompareTo(y.ItemName);
       });
        PopulateInner();
    }

    public void SortLevel()
    {
        itemList.Sort(delegate (ItemObject x, ItemObject y)
        {
            int c = y.Level.CompareTo(x.Level);
            if (c != 0)
                return c;
            return x.ItemName.CompareTo(y.ItemName);
        });
        PopulateInner();
    }

    public void SortType()
    {
        itemList.Sort(delegate (ItemObject x, ItemObject y)
        {
            int c = x.ItemType.CompareTo(y.ItemType);
            if (c != 0)
                return c;
            return x.Level.CompareTo(y.Level);
        });
        PopulateInner();
    }

}

