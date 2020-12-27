using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryShopController : MonoBehaviour {

    #region variables
    public GameObject itemScrollListPanel;
    [SerializeField]
    private UIUnitInfoPanel statPanel;
    [SerializeField]
    private StoryShopScrollList shopScrollListScript;
    [SerializeField]
    private ScrollListSimple scrollListSimpleScript;
    [SerializeField]
    private ScrollListSimple recruitScrollListSimpleScript;

    int SCROLL_LIST_BUY = 0;
    int SCROLL_LIST_BUY_EQUIP = 1;

    public Text goldText;

    PlayerUnit pu;

    StorySave currentSave;
    List<PlayerUnit> puList;
    List<ItemObject> itemList;
    List<PlayerUnit> recruitList;
    int currentUnitIndex;
    #endregion


    // Use this for initialization
    void Start () {
        pu = null;
        currentSave = CalcCode.LoadStorySave(0, true);
        puList = currentSave.GetPlayerUnitList();
        itemList = CalcCode.LoadStoryItemItemObjectList(currentSave.StoryId, currentSave.PointId, currentSave.StoryInt); //Debug.Log("ADD CODE TO GET ITEM LIST FROM A SAVE");
        goldText.text = "Gold: " + currentSave.Gold;
        recruitList = new List<PlayerUnit>();
	}
	
    //onclick button actions
    public void OnClickBuy()
    {
        recruitScrollListSimpleScript.Close();
        scrollListSimpleScript.Close();
        shopScrollListScript.Open(pu, itemList, SCROLL_LIST_BUY, NameAll.ITEM_SLOT_WEAPON);
    }

    int UNIT_LIST_NOTIFICATION_CHANGE = 1;
    int UNIT_LIST_NOTIFICATION_RECRUIT = 2;

    public void OnClickEquip()
    {
        recruitScrollListSimpleScript.Close();
        scrollListSimpleScript.Open(puList, UNIT_LIST_NOTIFICATION_CHANGE);
        ChangePlayerUnit(puList, 0); //changes PU and does the call for the items in there
    }

    public void OnClickRecruit()
    {
        scrollListSimpleScript.Close();
        recruitList.Clear();
        int modVersion = NameAll.VERSION_AURELIAN;
        if (NameAll.IsClassicClass(puList[0].ClassId))
            modVersion = NameAll.VERSION_CLASSIC;
        
        recruitList.Add(new PlayerUnit(modVersion, true, true, 1, true));
        recruitList.Add(new PlayerUnit(modVersion, true, true, 1, true));
        recruitList.Add(new PlayerUnit(modVersion, true, true, 1, true));
        recruitList.Add(new PlayerUnit(modVersion, true, true, 1, true));
        recruitList.Add(new PlayerUnit(modVersion, true, true, 1, true));

        recruitScrollListSimpleScript.Open(recruitList, UNIT_LIST_NOTIFICATION_RECRUIT);
    }

    void ChangePlayerUnit(List<PlayerUnit> localList, int index)
    {
        if( localList.Count > index)
        {
            currentUnitIndex = index;
            pu = localList[index];
            statPanel.PopulatePlayerInfo(pu, false);
            shopScrollListScript.Open(pu, itemList, SCROLL_LIST_BUY_EQUIP, NameAll.ITEM_SLOT_WEAPON);
        }

    }

    public void OnClickExit()
    {
        DisableObservers();
        //currentStorySave.UnitDictToList(unitDict);
        CalcCode.SaveTempStorySave(currentSave);
        SceneManager.LoadScene(NameAll.SCENE_STORY_MODE);
    }

    const string SimpleListNotification = "SimpleListNotification";
    const string ItemBuyNotification = "ItemBuyNotification";
    const string GoldSpentNotification = "GoldSpentNotification";

    void AddObservers()
    {
        this.AddObserver(OnSimpleListNotification, SimpleListNotification);
        this.AddObserver(OnItemBuyNotification, ItemBuyNotification);
        this.AddObserver(OnGoldSpentNotification, GoldSpentNotification);
    }

    void DisableObservers()
    {
        this.RemoveObserver(OnSimpleListNotification, SimpleListNotification);
        this.RemoveObserver(OnItemBuyNotification, ItemBuyNotification);
        this.RemoveObserver(OnGoldSpentNotification, GoldSpentNotification);
    }

    //different player unit clicked, update it
    void OnSimpleListNotification(object sender, object args)
    {
        List<int> tempList = args as List<int>;
        if( tempList[0] == UNIT_LIST_NOTIFICATION_CHANGE)
        {
            ChangePlayerUnit(puList, tempList[1]);
        }
        else if( tempList[0] == UNIT_LIST_NOTIFICATION_RECRUIT)
        {
            recruitScrollListSimpleScript.Close();
            if( puList.Count < 50)
            {
                var pu2 = recruitList[tempList[1]];
                currentSave.AddPlayerUnit(pu2);
                puList = currentSave.GetPlayerUnitList();
                scrollListSimpleScript.Open(puList, UNIT_LIST_NOTIFICATION_CHANGE); //refreshes the list  
            }
             
        }
        
    }

    void OnItemBuyNotification(object sender, object args)
    {
        //items are: itemId, is equip/not, goldcost, slot
        List<int> tempList = args as List<int>;
        
        if ( tempList[1] == 1) //item is bought and equipped
        {
            currentSave.EquipAndBuyItem(currentUnitIndex, tempList[2], tempList[0], tempList[3]);
            statPanel.PopulatePlayerInfo(pu, false); //show the new item on the unit
        }
        else
        {
            currentSave.AddItem(tempList[0]); //adds the item
        }
        goldText.text = "Gold: " + currentSave.Gold;
    }

    //if you want to do something fancy with the gold at some point
    //he has a script with an easing transition
    void OnGoldSpentNotification(object sender, object args)
    {

    }
}
