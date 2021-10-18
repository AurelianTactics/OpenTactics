using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class AbilityScrollList : MonoBehaviour
{
    //public Button backButton;
    public GameObject sampleButton;
    public Transform contentPanel;

    //public Button primaryButton;

    //List<string> combatLogList = new List<string>();
    //UIBackButton backButtonUI;
    List<AbilityObject> itemList;
    // Use this for initialization

    int slot = 1;
    PlayerUnit pu;

    int scrollListType;
    UnitAbilityObject unitAbilityObject;
    const int SCROLL_LIST_DEFAULT = 0;
    const int SCROLL_LIST_STORY_MODE_EQUIP = 1;
    const int SCROLL_LIST_STORY_MODE_LEARN = 2;
    int classId;

    public Text primaryButtonText;
    public Text secondaryButtonText;

    void Awake()
    {
        List<AbilityObject> itemList = new List<AbilityObject>();
        //scrollListType = SCROLL_LIST_DEFAULT;
        unitAbilityObject = null;
    }

    public void Open(PlayerUnit puIn )
    {
        //Debug.Log("wtf who opened");
        gameObject.SetActive(true);
        itemList = new List<AbilityObject>();
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
        Debug.Log("populating ability names " + pu.ClassId + " " + slot);
        itemList = AbilityManager.Instance.GetAbilityList(pu.ClassId, slot, true);
        //Debug.Log("asdf" + itemList.Count);
        //PopulateInner();
        SortName();

    }

    void PopulateInner()
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (AbilityObject a in itemList)
        {
            //Debug.Log("item size" + itemList.Count);
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            ItemScrollListButton tb = newButton.GetComponent<ItemScrollListButton>();
            int tempInt = a.SlotId;
            //Debug.Log("asdf" + i.GetItemName() + i.GetLevel());
            //tb.title.text = "asdf";
            tb.title.text = " " + a.AbilityName;
            tb.mainStat.text = " ";
            tb.details.text = " " + a.Description;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonClicked(tempInt, a.Slot));
        }
    }

    const string EquipAbilityNotification = "EquipAbilityNotification";
    void ButtonClicked(int index, int slot)
    {
        //ADD CODE HERE TO SHOW WHERE ON THE MAP THE CLICK IS
        //Debug.Log("Yatta, unit list button pressed..." + index);
        //if( slot == NameAll.ABILITY_SLOT_MOVEMENT)
        //{
        //    CharacterUIController.pu.EquipMovementAbility(index);
        //}
        //else if( slot == NameAll.ABILITY_SLOT_SECONDARY)
        //{
        //    CharacterUIController.pu.AbilitySecondaryCode = index;
        //}
        //else if (slot == NameAll.ABILITY_SLOT_REACTION)
        //{
        //    CharacterUIController.pu.AbilityReactionCode = index;
        //}
        //else if (slot == NameAll.ABILITY_SLOT_SUPPORT)
        //{
        //    CharacterUIController.pu.EquipSupportAbility(index);
        //}
        //else if( slot == NameAll.ABILITY_SLOT_PRIMARY)
        //{
        //    CharacterUIController.pu.SetClassIdStatsUnequip(index);
        //}
        //CharacterUIController.abilityUpdate = 1;
        var tempList = new List<int>();
        tempList.Add(slot);
        tempList.Add(index);
        this.PostNotification(EquipAbilityNotification, tempList);
    }



    //change the slot and what items are shown
    void SetSlot(int z1)
    {
        if (z1 >= 1 && z1 <= 5)
        {
            slot = z1;
            PopulateNames(pu);
        }
    }

    public void OnClickWeapon()
    {
        SetSlot(NameAll.ABILITY_SLOT_PRIMARY);
    }

    public void OnClickOffhand()
    {
        SetSlot(NameAll.ABILITY_SLOT_SECONDARY);
    }

    public void OnClickHead()
    {
        SetSlot(NameAll.ABILITY_SLOT_REACTION);
    }

    public void OnClickBody()
    {
        SetSlot(NameAll.ABILITY_SLOT_SUPPORT);
    }

    public void OnClickOther()
    {
        SetSlot(NameAll.ABILITY_SLOT_MOVEMENT);
    }

    public void SortName()
    {
        itemList.Sort(delegate (AbilityObject x, AbilityObject y)
        {
            return x.AbilityName.CompareTo(y.AbilityName);
        });
        PopulateInner();
    }

    //public void SortLevel()
    //{
    //    itemList.Sort(delegate (ItemObject x, ItemObject y)
    //    {
    //        int c = y.GetLevel().CompareTo(x.GetLevel());
    //        if (c != 0)
    //            return c;
    //        return x.GetItemName().CompareTo(y.GetItemName());
    //    });
    //    PopulateInner();
    //}

    //public void SortType()
    //{
    //    itemList.Sort(delegate (ItemObject x, ItemObject y)
    //    {
    //        int c = x.GetItemType().CompareTo(y.GetItemType());
    //        if (c != 0)
    //            return c;
    //        return x.GetLevel().CompareTo(y.GetLevel());
    //    });
    //    PopulateInner();
    //}

    #region StoryParty

    void OpenStoryMode(PlayerUnit puIn, int listType, UnitAbilityObject uao)
    {
        //Debug.Log("wtf who opened");

        gameObject.SetActive(true);
        unitAbilityObject = uao;
        pu = puIn;
        scrollListType = listType;
        classId = pu.ClassId;

        PopulateAbilityScrollList(unitAbilityObject, classId, slot, scrollListType);
    }

    public void OnClickEquip()
    {
        primaryButtonText.text = "Primary";
        secondaryButtonText.text = "Secondary";
        scrollListType = SCROLL_LIST_STORY_MODE_EQUIP;
        PopulateAbilityScrollList(unitAbilityObject, classId, slot, scrollListType);
    }

    public void OnClickLearn()
    {
        primaryButtonText.text = "Class";
        secondaryButtonText.text = "Spells";
        scrollListType = SCROLL_LIST_STORY_MODE_LEARN;
        PopulateAbilityScrollList(unitAbilityObject, classId, slot, scrollListType);
    }
    
    

    void PopulateAbilityScrollList(UnitAbilityObject uao, int zClassId, int zSlot, int listType)
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityObject> abilityList = new List<AbilityObject>(); ;

        if( listType == SCROLL_LIST_STORY_MODE_LEARN)
        {
            abilityList = uao.GetUnknownAbilities(zClassId, zSlot);
        }
        else if( listType == SCROLL_LIST_STORY_MODE_EQUIP)
        {
            //classId used here only so that you don't equip a secondary same as primary
            abilityList = uao.GetEquippableAbilities(zClassId, zSlot); 
        }
        else
        {
            return;
        }

        int cost = uao.GetAPCost(zClassId, zSlot);

        foreach (AbilityObject a in abilityList)
        {
            //Debug.Log("item size" + itemList.Count);
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            ItemScrollListButton tb = newButton.GetComponent<ItemScrollListButton>();
            int tempInt = a.SlotId;
            //Debug.Log("asdf" + i.GetItemName() + i.GetLevel());
            //tb.title.text = "asdf";
            tb.title.text = " " + a.AbilityName +  " (Cost: " + cost + " AP)";
            tb.mainStat.text = " ";
            tb.details.text = " " + a.Description;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonStoryModeClicked(tempInt, a.Slot));
        }
    }

    const string LearnAbilityNotification = "LearnAbilityNotification";

    void ButtonStoryModeClicked(int index, int slot)
    {
        if( scrollListType == SCROLL_LIST_STORY_MODE_EQUIP)
        {
            ButtonClicked(index, slot);
        }
        else if( scrollListType == SCROLL_LIST_STORY_MODE_LEARN)
        {
            if( slot == NameAll.ABILITY_SLOT_PRIMARY)
            {
                if (unitAbilityObject.IsClassLearned(index) )
                    classId = index;
                else
                {
                    List<int> tempList = new List<int>();
                    tempList.Add(slot);
                    tempList.Add(index);
                    tempList.Add(classId);
                    this.PostNotification(LearnAbilityNotification, tempList);
                }
                    
            }
            else
            {
                List<int> tempList = new List<int>();
                tempList.Add(slot);
                tempList.Add(index);
                tempList.Add(classId);
                this.PostNotification(LearnAbilityNotification, tempList);
            }
            
        }
    }

    void SetSlotStoryMode(int z1)
    {
        if (z1 >= 1 && z1 <= 5)
        {
            slot = z1;
            PopulateAbilityScrollList(unitAbilityObject, classId, slot, scrollListType);
        }
    }

    public void OnClickPrimary()
    {
        SetSlotStoryMode(NameAll.ABILITY_SLOT_PRIMARY);
    }

    public void OnClickSecondary()
    {
        SetSlotStoryMode(NameAll.ABILITY_SLOT_SECONDARY);
    }

    public void OnClickCounter()
    {
        SetSlotStoryMode(NameAll.ABILITY_SLOT_REACTION);
    }

    public void OnClickEnhance()
    {
        SetSlotStoryMode(NameAll.ABILITY_SLOT_SUPPORT);
    }

    public void OnClickMovement()
    {
        SetSlotStoryMode(NameAll.ABILITY_SLOT_MOVEMENT);
    }
    #endregion

}

