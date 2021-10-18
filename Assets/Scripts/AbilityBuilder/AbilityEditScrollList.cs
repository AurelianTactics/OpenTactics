using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.SceneManagement;

/// <summary>
/// I thin kthis script guides teh AbilityBuilder scene. I don't think it is used elsewhere
/// Create new abilities (spells) in the AbilityBuider scene. Set various options like what it does, who it hits etc. Can assign to a command set. The command set can be equipped by a class.
/// A class can be put on a unit or the command set can be used as a secondary ability set to allow units to cast the spells in the game.
/// </summary>

public class AbilityEditScrollList : MonoBehaviour {

    #region variables, constants, initializations
    //input object
    public GameObject abilityInput;
    public Text aiTitle;
    public Text aiDetails;
    public InputField aiInputField;
    //public Dropdown aiDropdown;

    //info object
    public GameObject infoPanel;
    public Text infoTitle;
    public Text infoDetails;

    //scrollList
    public GameObject sampleButton;
    public Transform contentPanel;

    //text panel
    public Text leftText;

    static readonly int TYPE = 0;
    static readonly int EFFECT = 1;
    static readonly int RANGE = 2;
    static readonly int MISC = 3;

    static readonly int MP_EXACT = 1;
    static readonly int MP_PERCENTAGE = 2;
    static readonly int CTR_EXACT = 1;
    static readonly int CTR_PERCENTAGE = 2;
    static readonly int BASE_HIT_ZODIAC = 1;
    static readonly int BASE_HIT_DIRECT = 0;
    static readonly int BASE_Q_RANDOM = 1;
    static readonly int BASE_Q_DIRECT = 0;
    static readonly int COMMAND_SET_NEW = 0;
    static readonly int COMMAND_SET_RENAME = 1;
    static readonly int COMMAND_SET_DELETE = 2;
    static readonly int SPELL_NAME_DELETE = -1;

    SpellName spellName;
    string snTitle = ""; //current snTitle when the title is selected for AbilityEditInput (ie if "name" is selected, the inputField will update the name category
    int snSubCategory = 0; //some inputs have multiple typs of categories, OnSubmitInputField relies on this for updating some variables

    List<AbilityBuilderObject> aboList; //list displayed in the scrollList
    int aboCurrent; //so when a value is updated, knows which list to refresh

    int currentSNIndex;//; = PlayerPrefs.GetInt(NameAll.PP_ABILITY_EDIT_MAX_INDEX,10000);
    int currentCommandSet = NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE;
    List<SpellName> snList;

    Dictionary<int, string> statusDict;
    
    void Awake()
    {
        aboList = new List<AbilityBuilderObject>();
        snList = new List<SpellName>();
        spellName = new SpellName();
        aboCurrent = TYPE;

        InputField.SubmitEvent aiInputEvent = new InputField.SubmitEvent();
        aiInputEvent.AddListener(OnSubmitAIInput);
        aiInputField.onEndEdit = aiInputEvent;
        //aiInputField.characterValidation = InputField.CharacterValidation.Alphanumeric;
    }

    void Start()
    {
        statusDict = NameAll.GetStatusDict();
        OnClickNew(); //clears the SN, sets the proper snIndex
        OnClickEffect(); 
        
    }

    #endregion

    #region onclick side menu and edit menu (top layer)
    //after an ability has been updated, refreshes that category
    void OnRefresh(int category)
    {
        if (category == TYPE)
        {
            OnClickType();
        }
        else if (category == EFFECT)
        {
            OnClickEffect();
        }
        else if (category == RANGE)
        {
            OnClickRange();
        }
        else if (category == MISC)
        {
            OnClickMisc();
        }
    }

    public void OnClickType()
    {
        abilityInput.SetActive(false);
        BuildABOList(TYPE, spellName);
        PopulateEdit(TYPE);
    }

    public void OnClickEffect()
    {
        abilityInput.SetActive(false);
        BuildABOList(EFFECT, spellName);
        PopulateEdit(EFFECT);
    }

    public void OnClickRange()
    {
        abilityInput.SetActive(false);
        BuildABOList(RANGE, spellName);
        PopulateEdit(RANGE);
    }

    public void OnClickMisc()
    {
        abilityInput.SetActive(false);
        BuildABOList(MISC, spellName);
        PopulateEdit(MISC);
    }

    public void OnClickSave()
    {
        string filePath = Application.dataPath + "/Custom/Spells";
        string filename = filePath + "/" + currentSNIndex + "_sn.dat";
        Serializer.Save<SpellName>(filename, spellName);
        infoPanel.SetActive(false);
        //if (!Directory.Exists(filePath))
        //    Directory.CreateDirectory(filePath);
    }

    public void OnClickNew()
    {
        currentSNIndex = CalcCode.GetNewCustomSpellNameIndex();
        spellName = new SpellName(currentSNIndex, currentCommandSet);
        OnClickType();
        infoPanel.SetActive(false);
        SetLeftText(spellName.AbilityName);
    }

    public void OnClickLoad()
    {
        abilityInput.SetActive(false);
        infoPanel.SetActive(false);
        BuildSNList();
        PopulateLoadList(snList);
        infoPanel.SetActive(false);
    }

    public void OnClickExit()
    {
        SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
    }

    //public void OnClickEdit()
    //{
    //    //CloseAll();
    //    //editPanel.SetActive(true);
    //    OnClickType();
    //}

    //SpellName LoadSpellName(int snIndex)
    //{
    //    SpellName sn;
    //    string fileName = Application.dataPath + "/Spells/" + snIndex + "_sn.dat";
    //    if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
    //    {
    //        sn = Serializer.Load<SpellName>(fileName);
    //    }
    //    else
    //    {
    //        return null;
    //        //Debug.Log("Error, spellName not found");
    //        //sn = new SpellName(PlayerPrefs.GetInt(NameAll.PP_ABILITY_EDIT_MAX_INDEX, 10000), currentCommandSet);
    //    }

    //    return sn;
    //}

    #endregion

    #region populate scroll list, build scroll list, handle scroll list clicks

    void PopulateEdit(int type)
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            string tempString = i.Title;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonPopulateEditClicked(tempString));
        }

        //foreach (ItemObject i in itemList)
        //{
        //    //Debug.Log("item size" + itemList.Count);
        //    GameObject newButton = Instantiate(sampleButton) as GameObject;
        //    ItemScrollListButton tb = newButton.GetComponent<ItemScrollListButton>();
        //    int tempInt = i.GetItemId();
        //    //Debug.Log("asdf" + i.GetItemName() + i.GetLevel());
        //    //tb.title.text = "asdf";
        //    tb.title.text = " " + i.GetItemName() + " (" + i.GetLevel() + ")";
        //    tb.mainStat.text = ItemManager.Instance.GetMainStat(i) + " ";
        //    tb.details.text = " " + ItemManager.Instance.GetDetails(i);
        //    tb.transform.SetParent(contentPanel);

        //    Button tempButton = tb.GetComponent<Button>();
        //    tempButton.onClick.AddListener(() => ButtonClicked(tempInt));
        //}
    }

    //on populateInner button clicked
    void ButtonPopulateEditClicked(string aboTitle)
    {
        //abilityInput.SetActive(true);
        if( aboTitle == NameAll.SN_DELETE)
        {
            DeleteSpellName();
            return;
        }

        OpenAbilityEditInput(aboTitle);
        OpenInfoPanel(snTitle);
        //Debug.Log("Yatta, aboButton is pressed..." + index);
        //launch the info on what type of button
        //launch the edit/spinner options
    }

    //builds list for PopulateInner scrollList
    void BuildABOList(int category, SpellName sn)
    {
        //refresh on every click, not caring if the same (this is called on a refresh too)
        //if (category != aboCurrent)
        //{
            aboList.Clear();
            aboCurrent = category;
            AbilityBuilderObject abo;
            if (aboCurrent == TYPE)
            {
                //commandSet version spellName	ctr	mp	
                abo = new AbilityBuilderObject(NameAll.SN_ABILITY_NAME, CalcCode.SpellNameValueToString(sn, NameAll.SN_ABILITY_NAME));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_COMMAND_SET, CalcCode.SpellNameValueToString(sn, NameAll.SN_COMMAND_SET));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_VERSION, CalcCode.SpellNameValueToString(sn, NameAll.SN_VERSION));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_CTR, CalcCode.SpellNameValueToString(sn, NameAll.SN_CTR));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_MP, CalcCode.SpellNameValueToString(sn, NameAll.SN_MP));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_DELETE,"",SPELL_NAME_DELETE);
                aboList.Add(abo);
            }
            else if (aboCurrent == EFFECT)
            {
                //mod damageFormulaType baseHit pmType t baseQ removeStat hitsStat statType addsStatus    statusType
                abo = new AbilityBuilderObject(NameAll.SN_MOD, CalcCode.SpellNameValueToString(sn, NameAll.SN_MOD));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_DAMAGE_FORMULA, CalcCode.SpellNameValueToString(sn, NameAll.SN_DAMAGE_FORMULA));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_BASE_HIT, CalcCode.SpellNameValueToString(sn, NameAll.SN_BASE_HIT));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_BASE_Q, CalcCode.SpellNameValueToString(sn, NameAll.SN_BASE_Q));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_ABILITY_TYPE, CalcCode.SpellNameValueToString(sn, NameAll.SN_ABILITY_TYPE));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_HITS_STAT, CalcCode.SpellNameValueToString(sn, NameAll.SN_HITS_STAT));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_STAT_TYPE, CalcCode.SpellNameValueToString(sn, NameAll.SN_STAT_TYPE));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_ADD_STATUS, CalcCode.SpellNameValueToString(sn, NameAll.SN_ADD_STATUS));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_STATUS_TYPE, CalcCode.SpellNameValueToString(sn, NameAll.SN_STATUS_TYPE));
                aboList.Add(abo);


            }
            else if (aboCurrent == RANGE)
            {
                //rangeXYMin rangeXYMax  rangeZ effectXY    effectZ alliesType casterImmune
                abo = new AbilityBuilderObject(NameAll.SN_RANGE_XY_MIN, CalcCode.SpellNameValueToString(sn, NameAll.SN_RANGE_XY_MIN));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_RANGE_XY_MAX, CalcCode.SpellNameValueToString(sn, NameAll.SN_RANGE_XY_MAX));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_RANGE_Z, CalcCode.SpellNameValueToString(sn, NameAll.SN_RANGE_Z));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_EFFECT_XY, CalcCode.SpellNameValueToString(sn, NameAll.SN_EFFECT_XY));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_EFFECT_Z, CalcCode.SpellNameValueToString(sn, NameAll.SN_EFFECT_Z));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_ALLIES_TYPE, CalcCode.SpellNameValueToString(sn, NameAll.SN_ALLIES_TYPE));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_CASTER_IMMUNE, CalcCode.SpellNameValueToString(sn, NameAll.SN_CASTER_IMMUNE));
                aboList.Add(abo);
            }
            else if (aboCurrent == MISC)
            {
                //evasionReflec calculateMimic  counterType statusCancel elementType ignoresDefense

                abo = new AbilityBuilderObject(NameAll.SN_EVADABLE, CalcCode.SpellNameValueToString(sn, NameAll.SN_EVADABLE));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_REFLECTABLE, CalcCode.SpellNameValueToString(sn, NameAll.SN_REFLECTABLE));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_CALCULATE, CalcCode.SpellNameValueToString(sn, NameAll.SN_CALCULATE));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_MIMIC, CalcCode.SpellNameValueToString(sn, NameAll.SN_MIMIC));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_COUNTER_TYPE, CalcCode.SpellNameValueToString(sn, NameAll.SN_COUNTER_TYPE));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_STATUS_CANCEL, CalcCode.SpellNameValueToString(sn, NameAll.SN_STATUS_CANCEL));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_ELEMENT_TYPE, CalcCode.SpellNameValueToString(sn, NameAll.SN_ELEMENT_TYPE));
                aboList.Add(abo);
                abo = new AbilityBuilderObject(NameAll.SN_IGNORES_DEFENSE, CalcCode.SpellNameValueToString(sn, NameAll.SN_IGNORES_DEFENSE));
                aboList.Add(abo);
            }
        //}
    }


    //populates the scrollList with the loaded sn's
    void PopulateLoadList(List<SpellName> myList)
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < myList.Count; i++)
        {
            SpellName sn = myList[i];
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = sn.Index;
            tb.title.text = sn.AbilityName;
            tb.details.text = CalcCode.SpellNameValueToString(sn,NameAll.SN_COMMAND_SET) + ": " + sn.MP + " MP";
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonSNClicked(tempInt, sn));
        }
    }

    //click on the list of spellNames, sets the current spellName to the one that was clicked so it can be edited
    void ButtonSNClicked(int snIndex, SpellName sn)
    {
        currentSNIndex = snIndex;
        spellName = sn;
        SetLeftText(spellName.AbilityName);
        OnClickType();
    }

    void SetLeftText( string zString)
    {
        leftText.text = "Current Ability: " + zString;
    }

    //when load is clicked, goes through the list of already created spell names
    void BuildSNList()
    {
        snList.Clear();
        snList = CalcCode.LoadCustomSpellNameList();
        
    }

    void DeleteSpellName()
    {
        string filePath = Application.dataPath + "/Custom/Spells/" + currentSNIndex + "_sn.dat";
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
    

    
    //public void OnInputCancel()
    //{
    //    allowSubmit = false;
    //    OnRefresh(aboCurrent);
    //}

    public void OnInputConfirm()
    {
        //allowSubmit = true;
        OnRefresh(aboCurrent);
    }
    
    //after an scrollList has been clicked and needs an input of a certain type
    void OpenAbilityInput(string snCategory, int subType)
    {
        abilityInput.SetActive(true);
        snSubCategory = subType;

        aiTitle.text = snCategory;
        aiDetails.text = "";

        if ( snCategory == NameAll.SN_BASE_HIT)
        {
            aiInputField.gameObject.SetActive(true);
            aiInputField.text = spellName.BaseHit + "%";
            aiInputField.characterValidation = InputField.CharacterValidation.Integer;
            aiInputField.characterLimit = 3;
        }
        else if( snCategory == NameAll.SN_MP)
        {
            aiInputField.gameObject.SetActive(true);
            aiInputField.characterValidation = InputField.CharacterValidation.Integer;
            aiInputField.characterLimit = 3; 
        }
        else if( snCategory == NameAll.SN_CTR)
        {
            aiInputField.gameObject.SetActive(true);
            aiInputField.characterValidation = InputField.CharacterValidation.Integer;
            aiInputField.characterLimit = 3;
        }
        else if( snCategory == NameAll.SN_COMMAND_SET)
        {
            aiInputField.gameObject.SetActive(true);
            aiInputField.characterValidation = InputField.CharacterValidation.Alphanumeric;
            aiInputField.characterLimit = 15;
        }
        //else if (snCategory.Equals(NameAll.SN_BASE_Q))
        //{
        //    if( subType == BASE_Q_RANDOM)
        //    {

        //    }
        //    else
        //    {
        //        aiInputField.gameObject.SetActive(true);
        //        aiInputField.text = spellName.BaseQ.ToString();
        //        aiInputField.characterValidation = InputField.CharacterValidation.Integer;
        //        aiInputField.characterLimit = 3;
        //    }
        //}
    }

    //has an X box, has a title, has a description on the left, has a spinner/input text on the right
    void OpenAbilityEditInput(string snCategory)
    {
        //Debug.Log(" in openability edit input " + snCategory);
        aiTitle.text = snCategory;
        aiDetails.text = "";
        //aiInputField.gameObject.SetActive(false);
        //aiDropdown.gameObject.SetActive(false);

        snTitle = snCategory;
        
        if(snCategory.Equals(NameAll.SN_ABILITY_NAME))
        {
            abilityInput.SetActive(true);
            aiInputField.gameObject.SetActive(true);
            aiInputField.text = spellName.AbilityName;
            aiInputField.characterValidation = InputField.CharacterValidation.Alphanumeric;
            aiInputField.characterLimit = 15;
        }
        else if (snCategory.Equals(NameAll.SN_BASE_Q))
        {
            abilityInput.SetActive(true);
            aiInputField.gameObject.SetActive(true);
            aiInputField.text = spellName.BaseQ.ToString();
            aiInputField.characterValidation = InputField.CharacterValidation.Integer;
            aiInputField.characterLimit = 3;
        }
        //else if (snCategory.Equals(NameAll.SN_BASE_HIT))
        //{
        //    aiInputField.gameObject.SetActive(true);
        //    aiInputField.text = spellName.BaseHit + "%";
        //    aiInputField.characterValidation = InputField.CharacterValidation.Integer;
        //    aiInputField.characterLimit = 3;
        //}
        //else if (snCategory.Equals(NameAll.SN_COMMAND_SET))
        //{
        //    aiDropdown.gameObject.SetActive(true); //allow user to create a new CommandSet
        //    aiInputField.gameObject.SetActive(true);
        //    aiInputField.text = CalcCode.SpellNameValueToString(spellName, NameAll.SN_COMMAND_SET);
        //    aiInputField.characterValidation = InputField.CharacterValidation.Alphanumeric;
        //    aiInputField.characterLimit = 15;
        //}
        else
        {
            PopulateInputScrollList(snCategory);
        }
        //else if(snCategory.Equals(NameAll.SN_ABILITY_TYPE))
        //{
        //    PopulateInputScrollList(snCategory);
        //}
        //else if (snCategory.Equals(NameAll.SN_ADD_STATUS))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_ALLIES_TYPE))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        
        //else if (snCategory.Equals(NameAll.SN_CALCULATE))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_CASTER_IMMUNE))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        
        //else if (snCategory.Equals(NameAll.SN_COUNTER_TYPE))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_CTR))
        //{
        //    aiDropdown.gameObject.SetActive(true);

        //}
        //else if (snCategory.Equals(NameAll.SN_DAMAGE_FORMULA))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_EFFECT_XY))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_EFFECT_Z))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_ELEMENT_TYPE))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_EVADABLE))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_HITS_STAT))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_IGNORES_DEFENSE))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_MIMIC))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_MOD))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_MP))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_RANGE_XY_MAX))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_RANGE_XY_MIN))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_RANGE_Z))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_REFLECTABLE))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_REMOVE_STAT))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_STATUS_CANCEL))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_STATUS_TYPE))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_STAT_TYPE))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}
        //else if (snCategory.Equals(NameAll.SN_VERSION))
        //{
        //    aiDropdown.gameObject.SetActive(true);
        //}

    }

    //called if certain editscroll list buttons are pressed (ie subcategory)
    void PopulateInputScrollList(string zTitle)
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildInputList(zTitle);

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempId = i.Id;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonInputClicked(tempId));
        }

    }

    //button on InputScrollList clicked, update value and reopen the ceObject in the Edit list
    void ButtonInputClicked(int z1)
    {
        //Debug.Log("spellName is " + spellName.ElementType + " " + z1 + " " + snTitle );
        if (snTitle.Equals(NameAll.SN_ABILITY_TYPE))
        {
            spellName.PMType = z1;
        }
        else if (snTitle.Equals(NameAll.SN_ADD_STATUS))
        {
            spellName.AddsStatus = z1;
        }
        else if (snTitle.Equals(NameAll.SN_ALLIES_TYPE))
        {
            spellName.AlliesType = z1;
        }
        else if (snTitle.Equals(NameAll.SN_BASE_HIT))
        {
            OpenAbilityInput(NameAll.SN_BASE_HIT, z1);
            return;
        }
        //else if (snTitle.Equals(NameAll.SN_BASE_Q))
        //{
        //    OpenAbilityInput(NameAll.SN_BASE_Q, z1);
        //}
        else if (snTitle.Equals(NameAll.SN_CALCULATE))
        {
            //calculate and mimic grouped, check on other
            if (spellName.CalculateMimic == NameAll.SPELL_NO_CALC_MIMIC)
            {
                if (z1 == NameAll.SPELL_CALC)
                {
                    spellName.CalculateMimic = NameAll.SPELL_CALC;
                }
            }
            else if (spellName.CalculateMimic == NameAll.SPELL_CALC)
            {
                if (z1 == NameAll.SPELL_NO_CALC_MIMIC)
                {
                    spellName.CalculateMimic = NameAll.SPELL_NO_CALC_MIMIC;
                }
            }
            else if (spellName.CalculateMimic == NameAll.SPELL_MIMIC)
            {
                if (z1 == NameAll.SPELL_CALC)
                {
                    spellName.CalculateMimic = NameAll.SPELL_CALC_MIMIC;
                }
            }
            else if (spellName.CalculateMimic == NameAll.SPELL_CALC_MIMIC)
            {
                if (z1 == NameAll.SPELL_NO_CALC_MIMIC)
                {
                    spellName.CalculateMimic = NameAll.SPELL_MIMIC;
                }
            }
        }
        else if (snTitle.Equals(NameAll.SN_CASTER_IMMUNE))
        {
            spellName.CasterImmune = z1;
        }
        else if (snTitle.Equals(NameAll.SN_COUNTER_TYPE))
        {
            spellName.CounterType = z1;
        }
        else if (snTitle.Equals(NameAll.SN_CTR))
        {
            if (z1 == CTR_PERCENTAGE)
            {
                OpenAbilityInput(NameAll.SN_CTR, CTR_PERCENTAGE);
                return;
            }
            else if( z1 == CTR_EXACT)
            {
                OpenAbilityInput(NameAll.SN_CTR, CTR_EXACT);
                return;
            }
            else
            {
                spellName.CTR = 0;
                OnClickType();
                return;
            }

        }
        else if (snTitle == NameAll.SN_COMMAND_SET)
        {
            //can click on list of command set or create new
            if (z1 == COMMAND_SET_NEW)
            {
                OpenAbilityInput(NameAll.SN_COMMAND_SET, COMMAND_SET_NEW);
                return;
            }
            else if( z1 == COMMAND_SET_RENAME)
            {
                OpenAbilityInput(NameAll.SN_COMMAND_SET, COMMAND_SET_RENAME);
                return;
            }
            else if( z1 == COMMAND_SET_DELETE)
            {
                DeleteCommandSet();
                return;
            }
            else
            {
                currentCommandSet = z1;
                spellName.CommandSet = z1;
            }
        }
        else if (snTitle.Equals(NameAll.SN_DAMAGE_FORMULA))
        {
            spellName.DamageFormulaType = z1;
        }
        else if (snTitle.Equals(NameAll.SN_EFFECT_XY))
        {
            spellName.EffectXY = z1;
        }
        else if (snTitle.Equals(NameAll.SN_EFFECT_Z))
        {
            spellName.EffectZ = z1;
        }
        else if (snTitle.Equals(NameAll.SN_ELEMENT_TYPE))
        {
            spellName.ElementType = z1; //Debug.Log("spellName is " + spellName.ElementType + " " + z1);
        }
        else if (snTitle.Equals(NameAll.SN_EVADABLE))
        {
            //evasion and reflect grouped, check on other
            if (spellName.EvasionReflect == NameAll.SPELL_NO_EVASION_REFLECT)
            {
                if (z1 == NameAll.SPELL_EVASION)
                {
                    spellName.EvasionReflect = NameAll.SPELL_EVASION;
                }
            }
            else if (spellName.EvasionReflect == NameAll.SPELL_EVASION)
            {
                if (z1 == NameAll.SPELL_NO_EVASION_REFLECT)
                {
                    spellName.EvasionReflect = NameAll.SPELL_NO_EVASION_REFLECT;
                }
            }
            else if (spellName.EvasionReflect == NameAll.SPELL_REFLECT)
            {
                if (z1 == NameAll.SPELL_EVASION)
                {
                    spellName.EvasionReflect = NameAll.SPELL_EVASION_REFLECT;
                }
            }
            else if (spellName.EvasionReflect == NameAll.SPELL_EVASION_REFLECT)
            {
                if (z1 == NameAll.SPELL_NO_EVASION_REFLECT)
                {
                    spellName.EvasionReflect = NameAll.SPELL_REFLECT;
                }
            }
        }
        else if (snTitle.Equals(NameAll.SN_HITS_STAT))
        {
            spellName.HitsStat = z1;
        }
        else if (snTitle.Equals(NameAll.SN_IGNORES_DEFENSE))
        {
            spellName.IgnoresDefense = z1;
        }
        else if (snTitle.Equals(NameAll.SN_MIMIC))
        {
            //calculate and mimic grouped, check on other
            if (spellName.CalculateMimic == NameAll.SPELL_NO_CALC_MIMIC)
            {
                if (z1 == NameAll.SPELL_MIMIC)
                {
                    spellName.CalculateMimic = NameAll.SPELL_MIMIC;
                }
            }
            else if (spellName.CalculateMimic == NameAll.SPELL_CALC)
            {
                if (z1 == NameAll.SPELL_MIMIC)
                {
                    spellName.CalculateMimic = NameAll.SPELL_CALC_MIMIC;
                }
            }
            else if (spellName.CalculateMimic == NameAll.SPELL_MIMIC)
            {
                if (z1 == NameAll.SPELL_NO_CALC_MIMIC)
                {
                    spellName.CalculateMimic = NameAll.SPELL_NO_CALC_MIMIC;
                }
            }
            else if (spellName.CalculateMimic == NameAll.SPELL_CALC_MIMIC)
            {
                if (z1 == NameAll.SPELL_NO_CALC_MIMIC)
                {
                    spellName.CalculateMimic = NameAll.SPELL_CALC;
                }
            }
        }
        else if (snTitle.Equals(NameAll.SN_MOD))
        {
            spellName.Mod = z1;
        }
        else if (snTitle.Equals(NameAll.SN_MP))
        {
            //Debug.Log("testing MP input, " + snTitle + " " + z1);
            if (z1 == 0)
            {
                spellName.MP = z1;
            }
            else if (z1 == MP_EXACT)
            {
                //Debug.Log("testing MP input, " + snTitle + " " + z1);
                OpenAbilityInput(NameAll.SN_MP, MP_EXACT);
                return;
            }
            else if (z1 == MP_PERCENTAGE)
            {
                OpenAbilityInput(NameAll.SN_MP, MP_PERCENTAGE);
                return;
            }

        }
        else if (snTitle.Equals(NameAll.SN_RANGE_XY_MAX))
        {
            spellName.RangeXYMax = z1;
        }
        else if (snTitle.Equals(NameAll.SN_RANGE_XY_MIN))
        {
            spellName.RangeXYMin = z1;
        }
        else if (snTitle.Equals(NameAll.SN_RANGE_Z))
        {
            spellName.RangeZ = z1;
        }
        else if (snTitle.Equals(NameAll.SN_REFLECTABLE))
        {
            //evasion and reflect grouped, check on other
            if (spellName.EvasionReflect == NameAll.SPELL_NO_EVASION_REFLECT)
            {
                if (z1 == NameAll.SPELL_REFLECT)
                {
                    spellName.EvasionReflect = NameAll.SPELL_REFLECT;
                }
            }
            else if (spellName.EvasionReflect == NameAll.SPELL_EVASION)
            {
                if (z1 == NameAll.SPELL_REFLECT)
                {
                    spellName.EvasionReflect = NameAll.SPELL_EVASION_REFLECT;
                }
            }
            else if (spellName.EvasionReflect == NameAll.SPELL_REFLECT)
            {
                if (z1 == NameAll.SPELL_NO_EVASION_REFLECT)
                {
                    spellName.EvasionReflect = NameAll.SPELL_NO_EVASION_REFLECT;
                }
            }
            else if (spellName.EvasionReflect == NameAll.SPELL_EVASION_REFLECT)
            {
                if (z1 == NameAll.SPELL_NO_EVASION_REFLECT)
                {
                    spellName.EvasionReflect = NameAll.SPELL_EVASION;
                }
            }
        }
        else if (snTitle.Equals(NameAll.SN_REMOVE_STAT))
        {
            spellName.RemoveStat = z1;
        }
        else if (snTitle.Equals(NameAll.SN_STATUS_CANCEL))
        {
            spellName.StatusCancel = z1;
        }
        else if (snTitle.Equals(NameAll.SN_STATUS_TYPE))
        {
            spellName.StatusType = z1;
        }
        else if (snTitle.Equals(NameAll.SN_STAT_TYPE))
        {
            spellName.StatType = z1;
        }
        else if (snTitle.Equals(NameAll.SN_VERSION))
        {
            spellName.Version = z1;
        }
        OnRefresh(aboCurrent);
    }

    //builds scrollList for PopulateInputScrollList (ie user is choosing to input a new value)
    List<AbilityBuilderObject> BuildInputList(string zTitle)
    {
        List<AbilityBuilderObject> aboList = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;

        if (zTitle.Equals(NameAll.SN_ABILITY_TYPE))
        {
            abo = new AbilityBuilderObject(NameAll.PM_TYPE_NAME_MAGICAL, "", NameAll.PM_TYPE_MAGICAL);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.PM_TYPE_NAME_NEUTRAL, "", NameAll.PM_TYPE_NEUTRAL);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.PM_TYPE_NAME_PHYSICAL, "", NameAll.PM_TYPE_PHYSICAL);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.PM_TYPE_NAME_NULL, "", NameAll.PM_TYPE_NULL);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_ADD_STATUS))
        {
            abo = new AbilityBuilderObject(NameAll.NO, "", 0);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Yes (100% of hits)", "", 1);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Yes (75% of hits)", "", 175);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Yes (50% of hits)", "", 150);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Yes (25% of hits)", "", 125);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Yes (10% of hits)", "", 110);
            aboList.Add(abo);

        }
        else if (zTitle.Equals(NameAll.SN_ALLIES_TYPE))
        {
            abo = new AbilityBuilderObject(NameAll.GetAlliesTypeString(NameAll.ALLIES_TYPE_ANY), "", NameAll.ALLIES_TYPE_ANY);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetAlliesTypeString(NameAll.ALLIES_TYPE_ALLIES), "", NameAll.ALLIES_TYPE_ALLIES);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetAlliesTypeString(NameAll.ALLIES_TYPE_ENEMIES), "", NameAll.ALLIES_TYPE_ENEMIES);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_BASE_HIT))
        {
            if (spellName.Version == NameAll.VERSION_CLASSIC)
            {
                OpenAbilityInput(zTitle, BASE_HIT_DIRECT);
            }
            else
            {
                abo = new AbilityBuilderObject("Base Hit (Color Modification)", "", BASE_HIT_ZODIAC);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Base Hit (No Color Modification)", "", BASE_HIT_DIRECT);
                aboList.Add(abo);
            }


        }
        else if (zTitle.Equals(NameAll.SN_CALCULATE))
        {
            abo = new AbilityBuilderObject(NameAll.YES, "", NameAll.SPELL_CALC);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.NO, "", NameAll.SPELL_NO_CALC_MIMIC);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_CASTER_IMMUNE))
        {
            abo = new AbilityBuilderObject(NameAll.YES, "", 1);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.NO, "", 0);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_COUNTER_TYPE))
        {
            if (spellName.Version == NameAll.VERSION_AURELIAN)
            {
                abo = new AbilityBuilderObject("NA. This is only used in Classic Version", "", 0);
                aboList.Add(abo);
            }
            else
            {
                abo = new AbilityBuilderObject("None", "", NameAll.COUNTER_TYPE_NONE);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Blase Grasp", "", NameAll.COUNTER_TYPE_BLADE_GRASP);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Counter Flood", "", NameAll.COUNTER_TYPE_COUNTER_FLOOD);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Both", "", NameAll.COUNTER_TYPE_BOTH);
                aboList.Add(abo);
            }
        }
        else if (zTitle.Equals(NameAll.SN_CTR))
        {
            abo = new AbilityBuilderObject("Instant Cast", "", 0);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Ticks (enter exact number)", "", CTR_EXACT);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("% of CT (cast time based on speed)", "", CTR_PERCENTAGE);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_COMMAND_SET))
        {
            abo = new AbilityBuilderObject("New Command Set", "", COMMAND_SET_NEW);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Delete Current Command Set", "", COMMAND_SET_DELETE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Rename Current Command Set", "", COMMAND_SET_RENAME);
            aboList.Add(abo);

            List<CommandSet> csList = CalcCode.LoadCustomCommandSetList(spellName.Version);
            foreach (CommandSet cs in csList)
            {
                abo = new AbilityBuilderObject(cs.CommandSetName, "", cs.CommandSetId);
                aboList.Add(abo);
            }
        }
        else if (zTitle.Equals(NameAll.SN_DAMAGE_FORMULA))
        {
            if (spellName.Version == NameAll.VERSION_CLASSIC)
            {
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = Ability Strength", 0);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = Ability Strength + INT", 1);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = STR + (STR * COURAGE)/100", 2);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit + Zodiac (Opposite Sex Only) ", "Effect = Ability Strength", 3);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit + Zodiac (Talk Skill)", "Effect = Ability Strength", 4);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit + XA ", "Effect = Ability Strength", 6);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = XA * (STR/2)", 8);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = XA * (STR/2 + 1)", 10);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = (Target Max HP) * (Ability Strength)/100", 15);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = (Target Max MP) * (Ability Strength)/100", 18);
                aboList.Add(abo);
                //abo = new AbilityBuilderObject("Hit % = Base Hit + XA (Prevented by Maintenance)", "Effect = Ability Strength", 20);
                //aboList.Add(abo);
                //abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = XA * Ability Strength (Range is Actor's Move Range)", 21);
                //aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = XA * Ability Strength", 25);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "INT * (STR + 2)/2", 30);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = Ability Strength * INT", 31);
                aboList.Add(abo);
            }
            else
            {
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = Ability Strength", 0);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = Actor WIS * Target WIS * XA * Ability Strength / 10000", 1);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Actor WIS * Target WIS * (Base Hit + XA) /10000", "Effect = Ability Strength", 2);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit + XA", "Effect = Ability Strength", 3);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit + XA", "Effect = Ability Strength + XA", 9);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit + XA", "Effect = Ability Strength * XA", 4);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = Ability Strength * XA", 17);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Actor WIS * Target WIS * (Base Hit + XA) /10000", "Effect = Ability Strength + XA", 5);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit + Color Modification", "Effect = Ability Strength + Color Modification", 6);
                aboList.Add(abo);
                //SKIP 7
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = XA * (Actor STR + 1)", 8);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit + XA (100% Knockback)", "Effect = Ability Strength * XA", 10);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = XA * ( (Actor STR or AGI or INT) / 2 + Ability Strength)", 11);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = XA * ( (Actor STR or AGI or INT) / 4 + Ability Strength)", 15);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit", "Effect = XA * ( (Actor STR + Actor AGI + Ability Strength) / 4 )", 16);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Hit % = Base Hit + Color Modification", "Effect = Ability Strength * Color Modification * Color Modification", 21);
                aboList.Add(abo);
                //abo = new AbilityBuilderObject("Hit % = ", "Effect =", );
                //aboList.Add(abo);
                //abo = new AbilityBuilderObject("Hit % = ", "Effect =", );
                //aboList.Add(abo);
            }
        }
        else if (zTitle.Equals(NameAll.SN_EFFECT_XY))
        {
            abo = new AbilityBuilderObject("Effect Tiles (Width): 1", "", 1);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Width): 2", "", 2);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Width): 3", "", 3);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Width): 4", "", 4);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Width): 5", "", 5);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Width): 6", "", 6);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Width): 7", "", 7);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Width): 8", "", 8);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Allies", "", NameAll.SPELL_EFFECT_ALLIES);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Enemies", "", NameAll.SPELL_EFFECT_ENEMIES);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Line (2 Tiles)", "", NameAll.SPELL_EFFECT_LINE_2);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Line (3 Tiles)", "", NameAll.SPELL_EFFECT_LINE_3);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Line (4 Tiles)", "", NameAll.SPELL_EFFECT_LINE_4);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Line (5 Tiles)", "", NameAll.SPELL_EFFECT_LINE_5);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Line (6 Tiles)", "", NameAll.SPELL_EFFECT_LINE_6);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Line (7 Tiles)", "", NameAll.SPELL_EFFECT_LINE_7);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Line (8 Tiles)", "", NameAll.SPELL_EFFECT_LINE_8);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Cone (2)", "", NameAll.SPELL_EFFECT_CONE_2);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Cone (3)", "", NameAll.SPELL_EFFECT_CONE_3);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Cone (4)", "", NameAll.SPELL_EFFECT_CONE_4);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Cone (5)", "", NameAll.SPELL_EFFECT_CONE_5);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Cone (6)", "", NameAll.SPELL_EFFECT_CONE_6);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Cone (7)", "", NameAll.SPELL_EFFECT_CONE_7);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Cone (8)", "", NameAll.SPELL_EFFECT_CONE_8);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles: Cone (9)", "", NameAll.SPELL_EFFECT_CONE_MAX);
            aboList.Add(abo);

        }
        else if (zTitle.Equals(NameAll.SN_EFFECT_Z))
        {
            abo = new AbilityBuilderObject("Effect Tiles (Height): Infinite", "", NameAll.SPELL_Z_INFINITE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Height): 0", "", 0);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Height): 1", "", 1);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Height): 2", "", 2);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Height): 3", "", 3);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Height): 4", "", 4);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Height): 5", "", 5);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Height): 6", "", 6);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Height): 7", "", 7);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Height): 8", "", 8);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Height): 9", "", 9);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Effect Tiles (Height): 10", "", 10);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_ELEMENT_TYPE))
        {
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_NONE,true), "", NameAll.ITEM_ELEMENTAL_NONE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_AIR, true), "", NameAll.ITEM_ELEMENTAL_AIR);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_DARK, true), "", NameAll.ITEM_ELEMENTAL_DARK);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_EARTH, true), "", NameAll.ITEM_ELEMENTAL_EARTH);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_FIRE, true), "", NameAll.ITEM_ELEMENTAL_FIRE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_HP_DRAIN, true), "", NameAll.ITEM_ELEMENTAL_HP_DRAIN);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_HP_RESTORE, true), "", NameAll.ITEM_ELEMENTAL_HP_RESTORE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_ICE, true), "", NameAll.ITEM_ELEMENTAL_ICE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_LIGHT, true), "", NameAll.ITEM_ELEMENTAL_LIGHT);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_LIGHTNING, true), "", NameAll.ITEM_ELEMENTAL_LIGHTNING);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_UNDEAD, true), "", NameAll.ITEM_ELEMENTAL_UNDEAD);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_WATER, true), "", NameAll.ITEM_ELEMENTAL_WATER);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_WEAPON, true), "", NameAll.ITEM_ELEMENTAL_WEAPON);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.GetElementalString(NameAll.ITEM_ELEMENTAL_WIND, true), "", NameAll.ITEM_ELEMENTAL_WIND);
            aboList.Add(abo);

        }
        else if (zTitle.Equals(NameAll.SN_EVADABLE))
        {
            abo = new AbilityBuilderObject(NameAll.YES, "", NameAll.SPELL_EVASION);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.NO, "", NameAll.SPELL_NO_EVASION_REFLECT);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_HITS_STAT))
        {
            abo = new AbilityBuilderObject(NameAll.NO, "", NameAll.HITS_STAT_NONE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Yes", "", NameAll.HITS_STAT);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Yes (% of total)", "", NameAll.HITS_STAT_PERCENTAGE);
            aboList.Add(abo);

        }
        else if (zTitle.Equals(NameAll.SN_IGNORES_DEFENSE))
        {
            abo = new AbilityBuilderObject(NameAll.YES, "", NameAll.IGNORES_DEFENSE_YES);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.NO, "", NameAll.IGNORES_DEFENSE_NO);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_MIMIC))
        {
            abo = new AbilityBuilderObject(NameAll.YES, "", NameAll.SPELL_MIMIC);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.NO, "", NameAll.SPELL_NO_CALC_MIMIC);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_MOD))
        {
            if (spellName.Version == NameAll.VERSION_CLASSIC)
            {
                abo = new AbilityBuilderObject("Mod 0", "No modification, no primary stat.", 0);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod 1", "No modification, no primary stat.", 1);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod 2", "Physical attacks (STR), effect variable.", 2);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod 3", "Physical attacks (STR), hit % variable.", 3);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod 4", "Speed based attacks.", 4);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod 5", "Magical attacks (INT), effect variable.", 5);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod 6", "Magical attacks (INT), hit % variable.", 6);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod 7", "Color Modification only.", 7);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod 8", "Speed based attacks.", 8);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod 9", "Physical, spear equipped attacks.", 9);
                aboList.Add(abo);
            }
            else
            {
                abo = new AbilityBuilderObject("Mod Null", "STR, INT, and AGI based", NameAll.MOD_NULL);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod Strength", "STR based", NameAll.MOD_PHYSICAL);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod Intelligence", "INT based", NameAll.MOD_MAGICAL);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod Agility", "AGI based", NameAll.MOD_NEUTRAL);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod Attack", "", NameAll.MOD_ATTACK);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod STR INT", "STR (primary) and INT based", NameAll.MOD_PHYSICAL_MAGICAL);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod STR AGI", "STR (primary) and AGI based", NameAll.MOD_PHYSICAL_NEUTRAL);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod INT STR", "INT (primary) and STR based", NameAll.MOD_MAGICAL_PHYSICAL);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod INT AGI", "INT (primary) and AGI based", NameAll.MOD_MAGICAL_NEUTRAL);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod AGI STR", "AGI (primary) and STR based", NameAll.MOD_NEUTRAL_PHYSICAL);
                aboList.Add(abo);
                abo = new AbilityBuilderObject("Mod AGI INT", "AGI (primary) and INT based", NameAll.MOD_NEUTRAL_MAGICAL);
                aboList.Add(abo);
                //abo = new AbilityBuilderObject("Mod ", "STR (primary) and AGI based", NameAll.MOD_PHYSICAL_AGI);
                //aboList.Add(abo);
                //            public static readonly int MOD_NULL = 0; //and zodiac
                //public static readonly int MOD_PHYSICAL = 1;
                //public static readonly int MOD_MAGICAL = 2;
                //public static readonly int MOD_NEUTRAL = 3;
                //public static readonly int MOD_ATTACK = 4;
                //public static readonly int MOD_PHYSICAL_MAGICAL = 11;
                //public static readonly int MOD_PHYSICAL_NEUTRAL = 21;
                //public static readonly int MOD_MAGICAL_PHYSICAL = 12;
                //public static readonly int MOD_MAGICAL_NEUTRAL = 22;
                //public static readonly int MOD_NEUTRAL_PHYSICAL  = 13;
                //public static readonly int MOD_NEUTRAL_MAGICAL = 23;
                //public static readonly int MOD_PHYSICAL_AGI = 31;
            }
        }
        else if (zTitle.Equals(NameAll.SN_MP))
        {
            abo = new AbilityBuilderObject("MP Cost: 0", "", 0);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("MP Cost: Enter Exact Value", "", MP_EXACT);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("MP Cost: Enter % of Max MP", "", MP_PERCENTAGE);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_RANGE_XY_MAX))
        {
            abo = new AbilityBuilderObject("Range Tiles: 0", "", 0);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 1", "", 1);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 2", "", 2);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 3", "", 3);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 4", "", 4);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 5", "", 5);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 6", "", 6);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 7", "", 7);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 8", "", 8);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: Weapon", "", NameAll.SPELL_RANGE_MAX_WEAPON);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: Line", "", NameAll.SPELL_RANGE_MAX_LINE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: Actor Move", "", NameAll.SPELL_RANGE_MAX_MOVE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: Spear", "", NameAll.SPELL_RANGE_SPEAR);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_RANGE_XY_MIN))
        {
            abo = new AbilityBuilderObject("Range Tiles: Target Actor", "", NameAll.SPELL_RANGE_MIN_SELF_TARGET);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 0", "", 0);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 1", "", 1);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 2", "", 2);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 3", "", 3);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 4", "", 4);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 5", "", 5);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 6", "", 6);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 7", "", 7);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 8", "", 8);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: 9", "", 9);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Range Tiles: Weapon Range", "", NameAll.SPELL_RANGE_MIN_WEAPON);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_RANGE_Z))
        {
            abo = new AbilityBuilderObject("Target Range (Height): Infinite", "", NameAll.SPELL_Z_INFINITE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Target Range (Height): 0", "", 0);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Target Range (Height): 1", "", 1);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Target Range (Height): 2", "", 2);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Target Range (Height): 3", "", 3);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Target Range (Height): 4", "", 4);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Target Range (Height): 5", "", 5);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Target Range (Height): 6", "", 6);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Target Range (Height): 7", "", 7);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Target Range (Height): 8", "", 8);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Target Range (Height): 9", "", 9);
            aboList.Add(abo);
            abo = new AbilityBuilderObject("Target Range (Height): 10", "", 10);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_REFLECTABLE))
        {
            abo = new AbilityBuilderObject(NameAll.YES, "", NameAll.SPELL_REFLECT);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.NO, "", NameAll.SPELL_NO_EVASION_REFLECT);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_REMOVE_STAT))
        {
            abo = new AbilityBuilderObject(NameAll.REMOVE_STAT_NAME_REMOVE, "", NameAll.REMOVE_STAT_REMOVE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.REMOVE_STAT_NAME_HEAL, "", NameAll.REMOVE_STAT_HEAL);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.REMOVE_STAT_NAME_ABSORB, "", NameAll.REMOVE_STAT_ABSORB);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_STATUS_CANCEL))
        {
            abo = new AbilityBuilderObject(NameAll.STATUS_CANCEL_NAME_NONE, "", NameAll.STATUS_CANCEL_NONE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STATUS_CANCEL_NAME_SILENCE, "", NameAll.STATUS_CANCEL_SILENCE);
            aboList.Add(abo);
        }
        else if (zTitle.Equals(NameAll.SN_STATUS_TYPE))
        {
            foreach (KeyValuePair<int, string> kvp in statusDict)
            {
                abo = new AbilityBuilderObject(kvp.Value, "", kvp.Key);
                aboList.Add(abo);
            }
        }
        else if (zTitle.Equals(NameAll.SN_STAT_TYPE))
        {
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_NONE, "", NameAll.STAT_TYPE_NONE);
            aboList.Add(abo);

            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_AGI, "", NameAll.STAT_TYPE_AGI);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_CT, "", NameAll.STAT_TYPE_CT);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_DIRECTION, "", NameAll.STAT_TYPE_DIRECTION);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_HP, "", NameAll.STAT_TYPE_HP);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_MAX_HP, "", NameAll.STAT_TYPE_MAX_HP);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_JUMP, "", NameAll.STAT_TYPE_JUMP);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_MA, "", NameAll.STAT_TYPE_MA);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_MP, "", NameAll.STAT_TYPE_MP);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_MAX_MP, "", NameAll.STAT_TYPE_MAX_MP);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_MOVE, "", NameAll.STAT_TYPE_MOVE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_PA, "", NameAll.STAT_TYPE_PA);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_SPEED, "", NameAll.STAT_TYPE_SPEED);
            aboList.Add(abo);

            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_BRAVE, "", NameAll.STAT_TYPE_BRAVE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_CUNNING, "", NameAll.STAT_TYPE_CUNNING);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_FAITH, "", NameAll.STAT_TYPE_FAITH);
            aboList.Add(abo);

            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_ACCESSORY, "", NameAll.STAT_TYPE_ACCESSORY);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_BODY, "", NameAll.STAT_TYPE_BODY);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_HEAD, "", NameAll.STAT_TYPE_HEAD);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_OFFHAND, "", NameAll.STAT_TYPE_OFFHAND);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_WEAPON, "", NameAll.STAT_TYPE_WEAPON);
            aboList.Add(abo);

            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_C_EVADE, "", NameAll.STAT_TYPE_C_EVADE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_M_EVADE, "", NameAll.STAT_TYPE_M_EVADE);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.STAT_TYPE_NAME_P_EVADE, "", NameAll.STAT_TYPE_P_EVADE);
            aboList.Add(abo);

        }
        else if (zTitle.Equals(NameAll.SN_VERSION))
        {
            abo = new AbilityBuilderObject(NameAll.VERSION_NAME_AURELIAN, "", NameAll.VERSION_AURELIAN);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(NameAll.VERSION_NAME_CLASSIC, "", NameAll.VERSION_CLASSIC);
            aboList.Add(abo);
        }

        //else if (zTitle == CE_COMMAND_SET)
        //{
        //    foreach (KeyValuePair<int, string> kvp in commandSetDict)
        //    {
        //        abo = new AbilityBuilderObject(kvp.Value, "", kvp.Key);
        //        aboList.Add(abo);
        //    }
        //}

        return aboList;
    }


    //inputField updated, changes spellName field
    void OnSubmitAIInput(string zString)
    {
        //Debug.Log(" in on submit AI input, " + snTitle + " " + zString);

        int z1 = 10;
        if (snTitle.Equals(""))
        {

        }
        else if( snTitle.Equals(NameAll.SN_ABILITY_NAME))
        {
            spellName.AbilityName = zString;
            aiInputField.text = zString;
            SetLeftText(spellName.AbilityName);
        }
        else if (snTitle.Equals(NameAll.SN_BASE_HIT))
        {
            if (Int32.TryParse(zString, out z1))
            {
                if( z1 < 0)
                {
                    z1 = 0;
                }
                else if( z1 >= 1000)
                {
                    z1 = 80;
                }
            }
            
            if( snSubCategory == BASE_HIT_ZODIAC)
            {
                spellName.BaseHit = z1 + 1000;
            }
            else
            {
                spellName.BaseHit = z1;
            }
            

            aiInputField.text = z1+"%";
        }
        else if (snTitle.Equals(NameAll.SN_BASE_Q))
        {
            if (Int32.TryParse(zString, out z1))
            {
                if (z1 < 0)
                {
                    z1 = 0;
                }
                else if (z1 >= 100)
                {
                    z1 = 10;
                }
            }
            spellName.BaseQ = z1;
            aiInputField.text = z1.ToString();
        }
        else if(snTitle.Equals(NameAll.SN_COMMAND_SET))
        {
            //Debug.Log(" in here  " + snSubCategory);
            if (snSubCategory == COMMAND_SET_RENAME)
            {
                CalcCode.RenameCommandSet(currentCommandSet, zString);
                //PlayerPrefs.SetString(NameAll.PP_CUSTOM_COMMAND_SET_BASE + currentCommandSet, zString);
                //Debug.Log("asdf " + NameAll.PP_CUSTOM_COMMAND_SET_BASE + currentCommandSet);
                //Debug.Log("asdf " + PlayerPrefs.GetString(NameAll.PP_CUSTOM_COMMAND_SET_BASE + currentCommandSet, "default") ); ;
            }
            else if( snSubCategory == COMMAND_SET_NEW)
            {
                //create the new command set and give it a name
                currentCommandSet = CalcCode.GetNewCommandSet(spellName.Version,zString);
                spellName.CommandSet = currentCommandSet;
            }
            
        }
        else if( snTitle == NameAll.SN_CTR)
        {
            if (Int32.TryParse(zString, out z1))
            {
                if (z1 <= 1)
                {
                    z1 = 1;
                }
                else if (z1 >= 100)
                {
                    z1 = 99;
                }
            }
            if ( snSubCategory == CTR_PERCENTAGE)
            {
                z1 += 100;
            }

            spellName.CTR = z1;
            aiInputField.text = z1.ToString();
        }
        else if( snTitle == NameAll.SN_MP)
        {
            if (Int32.TryParse(zString, out z1))
            {
                if (z1 <= 1)
                {
                    z1 = 1;
                }
                else if (z1 >= 1000)
                {
                    z1 = 999;
                }
            }

            if (snSubCategory == MP_PERCENTAGE)
            {
                spellName.MP = z1 + 1000;
            }
            else
                spellName.MP = z1;

            aiInputField.text = z1.ToString();
        }
        //CharacterUIController.pu.SetUnit_name(name);
        //inputName.text = CharacterUIController.pu.UnitName;
        
    }



    #endregion

    //open info panel and provide some help
    void OpenInfoPanel(string snCategory)
    {
        infoPanel.SetActive(true);
        infoTitle.text = snCategory;
        if (snCategory.Equals(NameAll.SN_ABILITY_NAME))
        {
            infoDetails.text = "Select ability name.";
        }
        else if (snCategory.Equals(NameAll.SN_ABILITY_TYPE))
        {
            infoDetails.text = "Abilities can be of a physical, magical, neutral or null type. Different ability types are subject to different evasion and damage calculations.";
        }
        else if (snCategory.Equals(NameAll.SN_ADD_STATUS))
        {
            infoDetails.text = "Does the ability add a status (or a chance of adding a status) on a successful hit?";
        }
        else if (snCategory.Equals(NameAll.SN_ALLIES_TYPE))
        {
            infoDetails.text = "Allows ability to hit allies only, enemies only, or either type of unit.";
        }
        else if (snCategory.Equals(NameAll.SN_BASE_HIT))
        {
            infoDetails.text = "Chance to hit of the ability before evasion or other calculations.";
        }
        else if (snCategory.Equals(NameAll.SN_BASE_Q))
        {
            infoDetails.text = "Strength of the ability. Higher numbers generally lead to stronger abilities.";
        }
        else if (snCategory.Equals(NameAll.SN_CALCULATE))
        {
            infoDetails.text = "Allows the ability to be used with Math Skill.";
        }
        else if (snCategory.Equals(NameAll.SN_CASTER_IMMUNE))
        {
            infoDetails.text = "Can the caster be hit by the ability?";
        }
        else if (snCategory.Equals(NameAll.SN_COMMAND_SET))
        {
            infoDetails.text = "Command set that the ability is grouped under. Ie, the 'Inferno' command set contains all the Fire Mage abilities.";
        }
        else if (snCategory.Equals(NameAll.SN_COUNTER_TYPE))
        {
            infoDetails.text = "The reaction abilities does the ability trigger.";
        }
        else if (snCategory.Equals(NameAll.SN_CTR))
        {
            infoDetails.text = "'Clock Ticks to Resolution.' How many ticks it takes the ability to be performed.";

        }
        else if (snCategory.Equals(NameAll.SN_DAMAGE_FORMULA))
        {
            infoDetails.text = "Determines what damage calculations the ability is subject to. Typically the Mod selection" +
                " determines which stat is selected to be modified (XA), while this category adds further adjustments to that XA and Base Hit.";
        }
        else if (snCategory.Equals(NameAll.SN_EFFECT_XY))
        {
            infoDetails.text = "Area of Effect for the spell.";
        }
        else if (snCategory.Equals(NameAll.SN_EFFECT_Z))
        {
            infoDetails.text = "Vertical Area of Effect for the spell. Higher numbers allow the spell to effect more tiles on uneven terrain.";
        }
        else if (snCategory.Equals(NameAll.SN_ELEMENT_TYPE))
        {
            infoDetails.text = "Element type of the ability.";
        }
        else if (snCategory.Equals(NameAll.SN_EVADABLE))
        {
            infoDetails.text = "Can the ability be evaded? Ie is the ability subject to evasion calculations.";
        }
        else if (snCategory.Equals(NameAll.SN_HITS_STAT))
        {
            infoDetails.text = "What stat does the ability influence (if any)?";
        }
        else if (snCategory.Equals(NameAll.SN_IGNORES_DEFENSE))
        {
            infoDetails.text = "Ability ignores Protect, Shell, Defense Up, etc. Set to 'Yes' for stronger healing spells or to increase power of offensive spells.";
        }
        else if (snCategory.Equals(NameAll.SN_MIMIC))
        {
            infoDetails.text = "Can the ability be mimicked?";
        }
        else if (snCategory.Equals(NameAll.SN_MOD))
        {
            infoDetails.text = "Decides which stats the ability relies on and the calculations that the ability is subject to.";
        }
        else if (snCategory.Equals(NameAll.SN_MP))
        {
            infoDetails.text = "Magic Points cost of the ability.";
        }
        else if (snCategory.Equals(NameAll.SN_RANGE_XY_MAX))
        {
            infoDetails.text = "Maximum target range of ability.";
        }
        else if (snCategory.Equals(NameAll.SN_RANGE_XY_MIN))
        {
            infoDetails.text = "Minimum target range of ability.";
        }
        else if (snCategory.Equals(NameAll.SN_RANGE_Z))
        {
            infoDetails.text = "Minimum and maximum vertical (height) target range of ability.";
        }
        else if (snCategory.Equals(NameAll.SN_REFLECTABLE))
        {
            infoDetails.text = "Can the spell be reflected?";
        }
        else if (snCategory.Equals(NameAll.SN_REMOVE_STAT))
        {
            infoDetails.text = "Which stat does the ability remove? Ie if the ability causes HP damage, choose the HP stat.";
        }
        else if (snCategory.Equals(NameAll.SN_STATUS_CANCEL))
        {
            infoDetails.text = "Area of Effect for the spell.";
        }
        else if (snCategory.Equals(NameAll.SN_STATUS_TYPE))
        {
            infoDetails.text = "Can the ability not be cast due to a certain status? Ie 'Silence' prevents some magic spells from being cast.";
        }
        else if (snCategory.Equals(NameAll.SN_STAT_TYPE))
        {
            infoDetails.text = "Does the ability damage, heal, or absorb the selected stat?";
        }
        else if (snCategory.Equals(NameAll.SN_VERSION))
        {
            infoDetails.text = "Which game version will this ability be played on?";
        }
    }

    void DeleteCommandSet()
    {
        string filePath = Application.dataPath + "/Custom/CommandSets/" + currentCommandSet + "_command_set.dat";//currentUnitDirectoryPath + "unit_" + currentSaveUnitId + ".dat";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    //void SetSpellName(int snIndex)
    //{
    //    currentSNIndex = snIndex;
    //    string fileName = Application.dataPath + "/Spells" + snIndex + "_sn.dat";
    //    if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
    //    {
    //        spellName = Serializer.Load<SpellName>(fileName);
    //    }
    //    else
    //    {
    //        Debug.Log("Error, spellName not found");
    //        spellName = new SpellName("abilityEdit");
    //    }
    //}
}
