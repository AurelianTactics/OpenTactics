using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryPartyController : MonoBehaviour {

    public GameObject itemScrollListPanel;
    public GameObject abilityScrollListPanel;
    public GameObject customizePanel;

    public DialogController dialogueControllerScript;
    [SerializeField]
    private UIUnitInfoPanel statPanel;
    [SerializeField]
    private StoryShopScrollList shopScrollListScript;
    [SerializeField]
    private ScrollListSimple scrollListSimpleScript;

    int SCROLL_LIST_EQUIP = 2;

    PlayerUnit pu;

    StorySave currentSave;
    List<PlayerUnit> puList;
    List<ItemObject> itemList;
    int currentUnitIndex;

    public Text partyXPText;
    public Text partyAPText;
    public Text unitXPText;
    public Text unitAPText;
    public Button levelUpButton;
    public Text levelUpButtonText;

    // Use this for initialization
    void Start () {
        currentSave = CalcCode.LoadStorySave(0, true);
        puList = currentSave.GetPlayerUnitList();
        OpenUnitScrollList();
        itemList = currentSave.GetEquippableItemObjectListFromInventory();
        SetPartyXPAPText();
	}

    void SetPartyXPAPText()
    {
        partyXPText.text = "Party XP: " + currentSave.PartyXP;
        partyAPText.text = "Party AP: " + currentSave.PartyAP;
        unitXPText.text = "Unit XP: " + currentSave.GetUnitXPString(currentUnitIndex,pu.Level);
        unitAPText.text = "Unit AP: ";

    }

    const string SimpleListNotification = "SimpleListNotification";
    const string EquipItemNotification = "EquipItemNotification";
    const string EquipAbilityNotification = "EquipAbilityNotification";
    const string MiscUnitNotification = "MiscUnitNotification";
    const string LearnAbilityNotification = "LearnAbilityNotification";


    void AddObservers()
    {
        this.AddObserver(OnSimpleListNotification, SimpleListNotification);
        this.AddObserver(OnEquipItemNotification, EquipItemNotification);
        this.AddObserver(OnEquipAbilityNotification, EquipAbilityNotification);
        this.AddObserver(OnMiscUnitNotification, MiscUnitNotification);
        this.AddObserver(OnLearnAbilityNotification, LearnAbilityNotification);
    }

    void DisableObservers()
    {
        this.RemoveObserver(OnSimpleListNotification, SimpleListNotification);
        this.RemoveObserver(OnEquipItemNotification, EquipItemNotification);
        this.RemoveObserver(OnEquipAbilityNotification, EquipAbilityNotification);
        this.RemoveObserver(OnMiscUnitNotification, MiscUnitNotification);
        this.RemoveObserver(OnLearnAbilityNotification, LearnAbilityNotification);
    }

    void OnSimpleListNotification(object sender, object args)
    {
        int z1 = (int)args;
        ChangePlayerUnit(puList, z1);
    }

    void OnEquipItemNotification(object sender, object args)
    {
        
        List<int> tempList = args as List<int>;

        currentSave.EquipItem(currentUnitIndex, tempList[0], tempList[1], true);
        itemList = currentSave.GetEquippableItemObjectListFromInventory(); //need to get the new item list due to equip/unequip, shitty way to do this
        shopScrollListScript.Open(pu, itemList, SCROLL_LIST_EQUIP, tempList[0]);
    }

    void OnEquipAbilityNotification(object sender, object args)
    {
        var tempList = args as List<int>;
        int slot = tempList[0];
        int index = tempList[1];
        if (slot == NameAll.ABILITY_SLOT_MOVEMENT)
            pu.EquipMovementAbility(index);
        else if (slot == NameAll.ABILITY_SLOT_SECONDARY)
            pu.AbilitySecondaryCode = index;
        else if (slot == NameAll.ABILITY_SLOT_REACTION)
            pu.AbilityReactionCode = index;
        else if (slot == NameAll.ABILITY_SLOT_SUPPORT)
            pu.EquipSupportAbility(index);
        else if (slot == NameAll.ABILITY_SLOT_PRIMARY)
            pu.SetClassIdStatsUnequip(index);

        statPanel.PopulatePlayerInfo(pu, false);
    }

    int learnAbilitySlot; //better to send these through the action in the dialogueController but uncertain how to do that
    int learnAbilityIndex;
    int learnAbilityClassId;

    void OnLearnAbilityNotification(object sender, object args)
    {
        var tempList = args as List<int>;
        int slot = tempList[0]; //slot is: primary for class, secondary for spellnames, and the rest are their abilities
        int index = tempList[1];
        int classId = tempList[2];

        learnAbilitySlot = slot;
        learnAbilityIndex = index;
        learnAbilityClassId = classId;
        
        int apNeeded = currentSave.GetPartyAPNeeded(currentUnitIndex, classId, slot);
        int apCost = currentSave.GetAPCost(currentUnitIndex, classId, slot);

        if( slot == NameAll.ABILITY_SLOT_PRIMARY)
        {
            if(currentSave.IsClassLearnable(index,pu.Level))
            {
                dialogueControllerScript.Show("Learn New Class?", "One class can be learned every level up. AP cost for abilities in each new class increases.", ConfirmLearnClass, null);
            }
            else
                dialogueControllerScript.Show("Unable to Learn New Class", "Only one class per level can be learned.", null, null);
        }
        else
        {
            if (apNeeded > 0)
            {
                //have to spend partyAP
                if( currentSave.PartyAP >= apNeeded)
                    dialogueControllerScript.Show("Spend Party AP to Learn Ability?", apCost + " total AP cost (" + apNeeded + " Party AP will be spent).", ConfirmLearnAbility, null);
                else
                    dialogueControllerScript.Show("Need More AP to Learn Ability", apCost + " total AP cost ", null, null);
            }
            else
            {
                dialogueControllerScript.Show("Learn Ability?", apCost + "total AP cost", ConfirmLearnAbility, null);
            }
        }
        

    }

    void ConfirmLearnAbility()
    {
        int apNeeded = currentSave.GetPartyAPNeeded(currentUnitIndex, learnAbilityClassId, learnAbilitySlot);
        int apCost = currentSave.GetAPCost(currentUnitIndex,learnAbilityClassId, learnAbilitySlot);
        if (apNeeded > 0)
        {
            currentSave.DecrementPartyAP(apNeeded);
            
        }
        currentSave.DecrementUnitAP(currentUnitIndex, apCost); //can't go lower than 0 so don't need the exact cost
        SetPartyXPAPText();

        currentSave.LearnAbility(currentUnitIndex, learnAbilitySlot, learnAbilityIndex, learnAbilityClassId);
        
        

    }

    void ConfirmLearnClass()
    {
        currentSave.LearnClass(currentUnitIndex, learnAbilityClassId);
    }

    void OnMiscUnitNotification(object sender, object args)
    {
        PlayerUnit newPU = args as PlayerUnit;
        pu = newPU;

        statPanel.PopulatePlayerInfo(pu, false);
    }

    void ChangePlayerUnit(List<PlayerUnit> localList, int index)
    {
        if (index < 0)
            index = 0; //deleting the first unit can cause this to happen

        if (localList.Count > index)
        {
            currentUnitIndex = index;
            pu = localList[index];
            statPanel.PopulatePlayerInfo(pu, false);
            ClosePanels();
            ToggleLevelUpButton();
        }

    }

    void ClosePanels()
    {
        itemScrollListPanel.gameObject.SetActive(false);
        abilityScrollListPanel.gameObject.SetActive(false);
    }

    void OpenUnitScrollList()
    {
        scrollListSimpleScript.Open(puList);
        ChangePlayerUnit(puList, 0); //changes PU and does the call for the items in there
    }

    #region onclick

    public void OnClickAbility()
    {
        ClosePanels();
        if( pu != null)
        {
            Debug.Log("handle ability equip shit here");
        }
    }

    public void OnClickItem()
    {
        ClosePanels();
        if( pu != null)
            shopScrollListScript.Open(pu, itemList, SCROLL_LIST_EQUIP, NameAll.ITEM_SLOT_WEAPON);
    }

    public void OnClickStrip()
    {
        ClosePanels();
        if ( pu != null)
            pu.StripClass();
        statPanel.PopulatePlayerInfo(pu, false);
    }

    public void OnClickLevelUp()
    {
        if(currentSave.IsUnitEligibleForLevelUp(currentUnitIndex))
        {
            currentSave.LevelUpUnit(currentUnitIndex);
        }
        else
        {
            int xpNeeded = currentSave.GetPartyXPNeededForLevelUp(currentUnitIndex);
            if( xpNeeded <= currentSave.PartyXP)
            {
                dialogueControllerScript.Show("Spend Party XP to Level Up?", xpNeeded + " Party XP will be spent.", ConfirmPartyXPLevelUp, null);
            }
        }

        ToggleLevelUpButton();
    }

    void ConfirmPartyXPLevelUp()
    {
        int xpNeeded = currentSave.GetPartyXPNeededForLevelUp(currentUnitIndex);
        currentSave.LevelUpUnit(currentUnitIndex, true);
        currentSave.DecrementPartyXP(xpNeeded);
        SetPartyXPAPText();
    }

    void ToggleLevelUpButton()
    {
        if (currentSave.IsUnitEligibleForLevelUp(currentUnitIndex))
        {
            levelUpButton.gameObject.SetActive(true);
            
        }
        else
        {
            int xpNeeded = currentSave.GetPartyXPNeededForLevelUp(currentUnitIndex);
            if (xpNeeded <= currentSave.PartyXP)
            {
                levelUpButtonText.text = "Level Up (" + xpNeeded +" Party XP)";
            }
            else
            {
                levelUpButton.gameObject.SetActive(false);
            }
            
        }

    }

    public void OnClickDelete()
    {
        dialogueControllerScript.Show("Confirm Remove?", "Unit will be permanently removed from party and can never return.",ConfirmDelete,null);
        

    }

    void ConfirmDelete()
    {
        if(pu != null && puList.Count > 1)
        {
            pu.StripClass();
            currentSave.DeleteUnit(currentUnitIndex);
            puList = currentSave.GetPlayerUnitList();
            ChangePlayerUnit(puList, currentUnitIndex-1);
        }
        
        
    }

    public void OnClickExit()
    {
        DisableObservers();
        //currentStorySave.UnitDictToList(unitDict);
        CalcCode.SaveTempStorySave(currentSave);
        SceneManager.LoadScene(NameAll.SCENE_STORY_MODE);
    }
    #endregion
}
