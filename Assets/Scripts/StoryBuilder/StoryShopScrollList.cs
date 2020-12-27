using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//used in story mode for shopping and in party mode for equipping items

public class StoryShopScrollList : MonoBehaviour
{
    //public Button backButton;
    public GameObject sampleButton;
    public Transform contentPanel;

    int SCROLL_LIST_BUY = 0;
    int SCROLL_LIST_BUY_EQUIP = 1;
    int SCROLL_LIST_EQUIP = 2;

    int populateListType;
    PlayerUnit pu;
    List<ItemObject> itemList;
    int slot = 0;


    void Awake()
    {
        List<ItemObject> itemList = new List<ItemObject>();
    }

    void Start()
    {
        pu = null;

    }

    public void Open(PlayerUnit puIn, List<ItemObject> listIn, int scrollListType, int zSlot)
    {
        //Debug.Log("wtf who opened");
        gameObject.SetActive(true);
        PopulateNames(pu, itemList, populateListType, slot);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void PopulateNames(PlayerUnit puIn, List<ItemObject> listIn, int scrollListType, int zSlot)
    {
        //Debug.Log("populating turns names neu");
        //itemList = ItemManager.Instance.GetItemsBySlotAndUnit(slot, pu);
        if (puIn != null)
            pu = puIn;
        itemList = listIn;
        populateListType = scrollListType;
        slot = zSlot;
        PopulateInner(pu, itemList, populateListType, slot);
    }

    void PopulateInner(PlayerUnit puIn, List<ItemObject> listIn, int scrollListType, int zSlot)
    {
        var innerList = new List<ItemObject>();
        if (scrollListType == SCROLL_LIST_BUY)
            innerList = itemList.Where(io => io.Slot == zSlot).ToList();
        else if (scrollListType == SCROLL_LIST_BUY_EQUIP)
            innerList = ItemManager.Instance.GetEquippableItemsBySlotUnit(listIn, puIn, zSlot);
        else if( scrollListType == SCROLL_LIST_EQUIP)
            innerList = ItemManager.Instance.GetEquippableItemsBySlotUnit(listIn, puIn, zSlot);

        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (ItemObject i in innerList)
        {
            //Debug.Log("item size" + itemList.Count);
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            ItemScrollListButton tb = newButton.GetComponent<ItemScrollListButton>();
            int tempInt = i.ItemId;
            //Debug.Log("asdf" + i.ItemName + i.Level);
            //tb.title.text = "asdf";

            Button tempButton = tb.GetComponent<Button>();

            if ( scrollListType == SCROLL_LIST_EQUIP)
            {
                tb.title.text =  i.ItemName + " (" + i.Level + ")";
                tb.mainStat.text = ItemManager.Instance.GetMainStat(i) + " ";
                tb.details.text = " " + ItemManager.Instance.GetDetails(i);
                tb.transform.SetParent(contentPanel);

                tempButton.onClick.AddListener(() => ButtonEquipClicked(tempInt));
            }
            else
            {
                int goldCost = ItemManager.Instance.GetItemCost(i);
                tb.title.text = "Gold: " + goldCost + " -- " + i.ItemName + " (" + i.Level + ")";
                tb.mainStat.text = ItemManager.Instance.GetMainStat(i) + " ";
                tb.details.text = " " + ItemManager.Instance.GetDetails(i);
                tb.transform.SetParent(contentPanel);

                if (scrollListType == SCROLL_LIST_BUY)
                    tempButton.onClick.AddListener(() => ButtonBuyClicked(tempInt, goldCost, 0));
                else if (scrollListType == SCROLL_LIST_BUY_EQUIP)
                    tempButton.onClick.AddListener(() => ButtonBuyClicked(tempInt, goldCost, 1));
            }
            
                

        }
    }

    const string ItemBuyNotification = "ItemBuyNotification";
    const string GoldSpentNotification = "GoldSpentNotification";
    const string ItemEquipNotification = "ItemEquipNotification";

    void ButtonBuyClicked(int itemId, int goldCost, int equipInt)
    {
        //Debug.Log("Yatta, unit list button pressed..." + index);
        var tempList = new List<int>();
        tempList.Add(itemId);
        tempList.Add(equipInt);
        tempList.Add(goldCost);
        tempList.Add(slot);
        this.PostNotification(ItemBuyNotification, tempList);
        this.PostNotification(GoldSpentNotification, goldCost);
    }

    void ButtonEquipClicked(int itemId)
    {
        var tempList = new List<int>();
        tempList.Add(itemId);
        tempList.Add(slot);
        this.PostNotification(ItemEquipNotification, itemId);
    }

    //change the slot and what items are shown
    void SetSlot( int z1)
    {
        if( z1 >= 0 && z1 <= 4)
        {
            slot = z1;
            PopulateNames(pu, itemList, populateListType, slot);
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
        PopulateInner(pu,itemList,populateListType,slot);
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
        PopulateInner(pu, itemList, populateListType, slot);
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
        PopulateInner(pu, itemList, populateListType, slot);
    }

}

