using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Guides the ItemBuilder scene. Creates custom items that are saved in Assets/Custom/Items and can be equipped by various characters in for custom game and story mode
/// </summary>
public class ItemBuilderController : MonoBehaviour {

    //item_id version slot item_type   level item_name   blocks status_name stat_brave stat_c_evade    stat_cunning stat_faith  stat_life stat_jump   stat_m_evade stat_ma stat_move stat_mp stat_p_evade stat_pa stat_speed stat_w_evade    stat_wp elemental_type  on_hit_effect on_hit_chance   stat_agi description
    //maybe add gold too

    //display image
    GameObject playerUnitObject;
    public UIUnitInfoPanel puInfo;

    //input object
    public GameObject userInputPanel;
    public Text userInputTitle;
    public Text userInputDetails;
    public InputField userInputField;

    //info object (not used)
    //public GameObject infoPanel;
    //public Text infoTitle;
    //public Text infoDetails;

    //scrollObject
    public GameObject sampleButton;
    public Transform contentPanel;

    ItemObject currentItemObject;
    int currentInputType;

    public Text leftText;

    // Use this for initialization
    void Start () {
        currentItemObject = null;
        currentInputType = NameAll.NULL_INT;
        SetLeftText();
        AddInputFieldListeners();
	}

    void SetLeftText()
    {
        if(currentItemObject == null)
        {
            leftText.text = "No Item Selected";
        }
        else
        {
            leftText.text = "Item: " + currentItemObject.ItemName;
        }
    }
	
    #region OnClickButtons
    //scrollist is populated with loadable objects
    public void OnClickLoad()
    {
        PopulatePanelList(SCROLL_LIST_LOAD);
    }

    //scrolllist is populated with current class, available to edit
    public void OnClickEdit()
    {
        if( currentItemObject != null)
        {
            PopulatePanelList(SCROLL_LIST_EDIT);
        }
    }
    //new class is created and ready to edit
    public void OnClickNew()
    {
        int z1 = CalcCode.GetNewCustomItem();
        currentItemObject = new ItemObject(z1);
        SetLeftText();
        OnClickEdit();
    }

    public void OnClickExit()
    {
        SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
    }

    //saves the current class
    public void OnClickSave()
    {
        if (currentItemObject != null)
            CalcCode.SaveCustomItemObject(currentItemObject);
    }
    #endregion

    #region populatescrolllist

    readonly int SCROLL_LIST_LOAD = 1;
    readonly int SCROLL_LIST_EDIT = 2;

    ////further input leads to another scroll list
    //readonly int INPUT_ITEM_ELEMENTAL_TYPE = 110;
    //readonly int INPUT_ITEM_BLOCKS = 113;
    //readonly int INPUT_ITEM_TYPE = 125;
    //readonly int INPUT_ITEM_VERSION = 126;
    //readonly int INPUT_ITEM_SLOT = 129;
    //readonly int INPUT_ITEM_STATUS_NAME = 131;

    ////further input asks for user to input value into input field
    //readonly int INPUT_ITEM_DESCRIPTION = 11;
    //readonly int INPUT_ITEM_CUNNING = 12;
    //readonly int INPUT_ITEM_AGI = 14;
    //readonly int INPUT_ITEM_BRAVE = 15;
    //readonly int INPUT_ITEM_C_EVADE = 16;
    //readonly int INPUT_ITEM_FAITH = 17;
    //readonly int INPUT_ITEM_JUMP = 19;
    //readonly int INPUT_ITEM_MA = 20;
    //readonly int INPUT_ITEM_MP = 21;
    //readonly int INPUT_ITEM_ON_HIT_CHANCE = 22;
    //readonly int INPUT_ITEM_ON_HIT_EFFECT = 23;
    //readonly int INPUT_ITEM_PA = 24;
    //readonly int INPUT_ITEM_WP = 27;
    //readonly int INPUT_ITEM_W_EVADE = 28;
    //readonly int INPUT_ITEM_LEVEL = 30;
    //readonly int INPUT_ITEM_SPEED = 32;
    //readonly int INPUT_ITEM_P_EVADE = 33;
    //readonly int INPUT_ITEM_NAME = 34;
    //readonly int INPUT_ITEM_M_EVADE = 35;
    //readonly int INPUT_ITEM_MOVE = 36;
    //readonly int INPUT_ITEM_LIFE = 37;
    //readonly int INPUT_ITEM_GOLD = 38;

    void PopulatePanelList(int panelType)
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildPanelList(panelType);

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonPanelClicked(panelType, tempInt));
        }
    }

    List<AbilityBuilderObject> BuildPanelList(int panelType)
    {
        List<AbilityBuilderObject> retValue = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;

        if( panelType == SCROLL_LIST_LOAD)
        {
            var tempList = CalcCode.LoadCustomItemObjectList();
            foreach( ItemObject io in tempList)
            {
                abo = new AbilityBuilderObject("Load Item: " + io.ItemName, "Type: " +  ItemConstants.GetItemTypeString(io.ItemType), io.ItemId);
                retValue.Add(abo);
            }
        }
        else if( panelType == SCROLL_LIST_EDIT)
        {
            //this.ItemId = zId;
            //this.Version = NameAll.VERSION_AURELIAN;
            //this.Slot = NameAll.ITEM_SLOT_ACCESSORY;
            //this.ItemType = NameAll.ITEM_ITEM_TYPE_BRACELET;
            //this.Level = 1;
            //abo = new AbilityBuilderObject("Edit Item ID", "Current Value: " + currentItemObject.ItemId, INPUT_ITEM_ID);
            //retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Version", "Current Value: " + CalcCode.GetVersionFromInt(currentItemObject.Version) , ItemConstants.INPUT_ITEM_VERSION);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Slot", "Current Value: " + currentItemObject.GetSlotName(), ItemConstants.INPUT_ITEM_SLOT);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Type", "Current Value: " + ItemConstants.GetItemTypeString(currentItemObject.ItemType) , ItemConstants.INPUT_ITEM_TYPE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Level", "Current Value: " + currentItemObject.Level , ItemConstants.INPUT_ITEM_LEVEL);
            retValue.Add(abo);
            //this.ItemName = "Custom Item " + zId;
            //this.Blocks = 0;
            //this.StatusName = 0;
            //this.StatBrave = 0;
            //this.StatCEvade = 0;
            abo = new AbilityBuilderObject("Edit Item Name", "Current Value: " + currentItemObject.ItemName, ItemConstants.INPUT_ITEM_NAME);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Blocks Status", "Current Value: " + NameAll.GetStatusString(currentItemObject.Blocks), ItemConstants.INPUT_ITEM_BLOCKS);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Adds Status", "Current Value: " + NameAll.GetStatusString(currentItemObject.StatusName), ItemConstants.INPUT_ITEM_STATUS_NAME);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Courage", "Current Value: " + currentItemObject.StatBrave, ItemConstants.INPUT_ITEM_BRAVE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Class Evade", "Current Value: " + currentItemObject.StatCEvade, ItemConstants.INPUT_ITEM_C_EVADE);
            retValue.Add(abo);
            //this.StatCunning = 0;
            //this.StatFaith = 0;
            //this.StatLife = 0;
            //this.StatJump = 0;
            //this.StatMEvade = 0;
            abo = new AbilityBuilderObject("Edit Item Cunning", "Current Value: " + currentItemObject.StatCunning, ItemConstants.INPUT_ITEM_CUNNING);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Faith", "Current Value: " + currentItemObject.StatFaith, ItemConstants.INPUT_ITEM_FAITH);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item HP", "Current Value: " + currentItemObject.StatLife, ItemConstants.INPUT_ITEM_LIFE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Jump", "Current Value: " + currentItemObject.StatJump, ItemConstants.INPUT_ITEM_JUMP);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Magic Evade", "Current Value: " + currentItemObject.StatMEvade, ItemConstants.INPUT_ITEM_M_EVADE);
            retValue.Add(abo);
            //this.StatMA = 0;
            //this.StatMove = 0;
            //this.StatMP = 0;
            //this.StatPEvade = 0;
            //this.StatPA = 0;
            abo = new AbilityBuilderObject("Edit Item INT", "Current Value: " + currentItemObject.StatMA, ItemConstants.INPUT_ITEM_MA);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Move", "Current Value: " + currentItemObject.StatMove, ItemConstants.INPUT_ITEM_MOVE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item MP", "Current Value: " + currentItemObject.StatMP, ItemConstants.INPUT_ITEM_MP);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Physical Evade", "Current Value: " + currentItemObject.StatPEvade, ItemConstants.INPUT_ITEM_P_EVADE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item STR", "Current Value: " + currentItemObject.StatPA, ItemConstants.INPUT_ITEM_PA);
            retValue.Add(abo);
            //this.StatSpeed = 0;
            //this.StatWEvade = 0;
            //this.StatWP = 0;
            //this.ElementalType = 0;
            //this.OnHitEffect = 0;
            abo = new AbilityBuilderObject("Edit Item Speed", "Current Value: " + currentItemObject.StatSpeed, ItemConstants.INPUT_ITEM_SPEED);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Weapon Evade", "Current Value: " + currentItemObject.StatWEvade, ItemConstants.INPUT_ITEM_W_EVADE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Weapon Power", "Current Value: " + currentItemObject.StatWP, ItemConstants.INPUT_ITEM_WP);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Elemental Type", "Current Value: " + NameAll.GetElementalString(currentItemObject.ElementalType), ItemConstants.INPUT_ITEM_ELEMENTAL_TYPE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item On Hit Effect", "Current Value: " + NameAll.GetStatusString(currentItemObject.OnHitEffect), ItemConstants.INPUT_ITEM_ON_HIT_EFFECT);
            retValue.Add(abo);
            //this.OnHitChance = 0;
            //this.StatAgi = 0;
            //this.Description = "";
            abo = new AbilityBuilderObject("Edit Item On Hit Chance", "Current Value: " + currentItemObject.OnHitChance, ItemConstants.INPUT_ITEM_ON_HIT_CHANCE);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item AGI", "Current Value: " + currentItemObject.StatAgi, ItemConstants.INPUT_ITEM_AGI);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Description", "Current Value: " + currentItemObject.Description, ItemConstants.INPUT_ITEM_DESCRIPTION);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Edit Item Gold", "Current Value: " + currentItemObject.Gold, ItemConstants.INPUT_ITEM_GOLD);
            retValue.Add(abo);
        }
        else if( panelType == ItemConstants.INPUT_ITEM_SLOT)
        {
            abo = new AbilityBuilderObject("Item Slot: Weapon", "", NameAll.ITEM_SLOT_WEAPON);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Item Slot: Offhand", "", NameAll.ITEM_SLOT_OFFHAND);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Item Slot: Head", "", NameAll.ITEM_SLOT_HEAD);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Item Slot: Body", "", NameAll.ITEM_SLOT_BODY);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Item Slot: Accessory", "", NameAll.ITEM_SLOT_ACCESSORY);
            retValue.Add(abo);
        }
        else if (panelType == ItemConstants.INPUT_ITEM_VERSION)
        {
            abo = new AbilityBuilderObject("Version: " + CalcCode.GetVersionFromInt(NameAll.VERSION_AURELIAN), "", NameAll.VERSION_AURELIAN);
            retValue.Add(abo);
            abo = new AbilityBuilderObject("Version: " + CalcCode.GetVersionFromInt(NameAll.VERSION_CLASSIC), "", NameAll.VERSION_CLASSIC);
            retValue.Add(abo);
        }
        else if (panelType == ItemConstants.INPUT_ITEM_TYPE)
        {
            var tempDict = ItemConstants.GetItemTypeDict(currentItemObject.Version,currentItemObject.Slot);
            foreach( KeyValuePair<int,string> kvp in tempDict)
            {
                abo = new AbilityBuilderObject("Item Type: " + kvp.Value, "", kvp.Key);
                retValue.Add(abo);
            }

        }
        else if (panelType == ItemConstants.INPUT_ITEM_ELEMENTAL_TYPE)
        {
            var tempDict = ItemConstants.GetItemElementalDict();
            foreach (KeyValuePair<int, string> kvp in tempDict)
            {
                abo = new AbilityBuilderObject("Item Elemental: " + kvp.Value, "", kvp.Key);
                retValue.Add(abo);
            }
        }
        else if (panelType == ItemConstants.INPUT_ITEM_BLOCKS)
        {
            var tempDict = NameAll.GetItemBlockDictionary();
            foreach (KeyValuePair<int, string> kvp in tempDict)
            {
                abo = new AbilityBuilderObject("Block Status: " + kvp.Value, "", kvp.Key);
                retValue.Add(abo);
            }
        }
        else if (panelType == ItemConstants.INPUT_ITEM_STATUS_NAME)
        {
            var tempDict = NameAll.GetItemAutoStatusDictionary();
            foreach (KeyValuePair<int, string> kvp in tempDict)
            {
                abo = new AbilityBuilderObject("Gain Status: " + kvp.Value, "", kvp.Key);
                retValue.Add(abo);
            }
        }
        else if (panelType == ItemConstants.INPUT_ITEM_ON_HIT_EFFECT)
        {
            var tempDict = NameAll.GetItemOnHitDictionary();
            foreach (KeyValuePair<int, string> kvp in tempDict)
            {
                abo = new AbilityBuilderObject("On Hit Status: " + kvp.Value, "", kvp.Key);
                retValue.Add(abo);
            }
        }

        return retValue;
    }


    void ButtonPanelClicked(int panelType, int clickId)
    {
        if( panelType == SCROLL_LIST_LOAD)
        {
            currentItemObject = CalcCode.LoadCustomItemObject(clickId);
            SetLeftText();
            return;
        }
        else if( panelType == SCROLL_LIST_EDIT)
        {
            if( clickId >= 100)
            {
                //repopulates panel with a specific sub list
                PopulatePanelList(clickId);
                return;
            }
            else
            {
                //asks for user input
                OpenInputField(clickId);
                return;
            }
        }
        else if( panelType >= 100)
        {
            currentItemObject.ChangeValueFromScrollList(panelType, clickId);
            PopulatePanelList(SCROLL_LIST_EDIT);
            return;
        }


    }


    void OpenInputField( int panelType)
    {
        currentInputType = panelType;

        userInputPanel.SetActive(true);
        userInputTitle.text = "Enter New Value";
        userInputField.text = "";

        if (  panelType == ItemConstants.INPUT_ITEM_DESCRIPTION || panelType == ItemConstants.INPUT_ITEM_NAME)
        {
            userInputField.characterLimit = 100;
            userInputField.characterValidation = InputField.CharacterValidation.None;
        }
        else
        {
            userInputField.characterValidation = InputField.CharacterValidation.Integer;
            userInputField.characterLimit = 3;
            if( panelType == ItemConstants.INPUT_ITEM_GOLD)
                userInputField.characterLimit = 6;
        } 
    }

    public void OnSubmitUserInput(string zString)
    {
        //Debug.Log("submitting user input " + zString);
        if( currentItemObject != null && currentInputType != NameAll.NULL_INT)
        {
            //Debug.Log("submitting user input " + zString);
            if ( currentInputType == ItemConstants.INPUT_ITEM_NAME)
            {
                currentItemObject.ChangeValueFromUserInput(currentInputType, zString);
                SetLeftText();
            }
            else if( currentInputType == ItemConstants.INPUT_ITEM_DESCRIPTION)
            {
                currentItemObject.ChangeValueFromUserInput(currentInputType, zString);
            }
            else
            {
                //Debug.Log("submitting user input " + zString);
                int z1;
                if (currentInputType == ItemConstants.INPUT_ITEM_GOLD)
                    z1 = GetIntFromString(zString, 0, 0, 999999);
                else if (currentInputType == ItemConstants.INPUT_ITEM_LIFE || currentInputType == ItemConstants.INPUT_ITEM_MP)
                    z1 = GetIntFromString(zString, 0, 0, 999);
                else
                    z1 = GetIntFromString(zString, 0, 0, 99);

                currentItemObject.ChangeValueFromUserInput(currentInputType, z1);
            }
            
        }
        CloseUserInput();
        OnClickEdit(); //Debug.Log("submitting user input " + zString);
    }

    void AddInputFieldListeners()
    {
        InputField.SubmitEvent userInputEvent = new InputField.SubmitEvent();
        userInputEvent.AddListener(OnSubmitUserInput);
        userInputField.onEndEdit = userInputEvent;
    }

    void CloseUserInput()
    {
        userInputPanel.SetActive(false);
    }

    int GetIntFromString(string zString, int defaultInt = 0, int minInt = 0, int maxInt = 10000)
    {
        int z1 = defaultInt;
        if (Int32.TryParse(zString, out z1))
        {
            if (z1 < minInt)
            {
                z1 = minInt;
            }
            else if (z1 > maxInt)
            {
                z1 = maxInt;
            }
        }

        return z1;
    }

    
    #endregion
}
