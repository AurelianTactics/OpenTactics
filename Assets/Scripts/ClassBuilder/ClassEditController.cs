using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.SceneManagement;

/// <summary>
/// This script drives the ClassBuilder scene
/// Build classes here. Classes can be given abilities, commandsets and different base stats.
/// </summary>

public class ClassEditController : MonoBehaviour {

    //display image
    GameObject playerUnitObject;
    public UIUnitInfoPanel puInfo;

    //input object
    public GameObject abilityInput;
    public Text aiTitle;
    public Text aiDetails;
    public InputField aiInputField;
    //public Dropdown aiDropdown;
    //List<string> dropdownList = new List<string>();

    //info object (not used)
    public GameObject infoPanel;
    public Text infoTitle;
    public Text infoDetails;

    //scrollObject
    public GameObject sampleButton;
    public Transform contentPanel;

    ClassEditObject ceObject; //object that is edited, saved, and loaded throughout this
    List<ClassEditObject> ceList;
    int currentCEIndex;// = NameAll.CUSTOM_CLASS_ID_START_VALUE; //which classId is selected for saving/loading
    string ceTitle = ""; //lets input field listener know which title has been selected

    //titles for scroll list and used to tell which scroll list button has been selected
    static readonly string CE_CLASS_NAME = "Class Name";
    static readonly string CE_VERSION = "Version";
    static readonly string CE_ICON = "Icon";
    static readonly string CE_COMMAND_SET = "Command Set";
    static readonly string CE_MOVE = "Move";
    static readonly string CE_JUMP = "Jump";
    static readonly string CE_CLASS_EVADE = "Class Evade";
    static readonly string CE_HP_BASE = "HP Base";
    static readonly string CE_MP_BASE = "MP Base";
    static readonly string CE_SPEED_BASE = "Speed Base";
    static readonly string CE_PA_BASE = "Strength Base";
    static readonly string CE_MA_BASE = "Intelligence Base";
    static readonly string CE_AGI_BASE = "Agility Base";
    static readonly string CE_HP_GROWTH = "HP Growth";
    static readonly string CE_MP_GROWTH = "MP Growth";
    static readonly string CE_SPEED_GROWTH = "Speed Growth";
    static readonly string CE_PA_GROWTH = "Strength Growth";
    static readonly string CE_MA_GROWTH = "Intelligence Growth";
    static readonly string CE_AGI_GROWTH = "Agility Growth";
    static readonly string CE_DELETE = "Delete Current Class";

    //scroll list input variables
    static readonly string CE_INPUT_AURELIAN = "Aurelian";
    static readonly string CE_INPUT_CLASSIC = "Classic";

    Dictionary<int, string> iconDict;
    List<CommandSet> commandSetList;
    //List<int> baseStats;

    void Awake()
    {
        
        //ceObject = //load or build a new one here
        ceList = new List<ClassEditObject>();
        
        //AddDropdownListener();
        AddInputFieldListener();
        
    }

    void Start()
    {
        //Debug.Log("starting");
        iconDict = NameAll.GetIconDict();
        

        //sets the initial ceObject
        currentCEIndex = NameAll.CUSTOM_CLASS_ID_START_VALUE;
        ceObject = CalcCode.LoadCEObject(currentCEIndex);
        LoadPUO();

		//testing this with following two lines off
		PlayerUnit pu = new PlayerUnit(ceObject);
		puInfo.PopulatePlayerInfo(pu);

		//var puls = new PlayerUnitLevelStats();
		//baseStats = puls.GetCEBaseStats(ceObject);

		commandSetList = CalcCode.LoadCustomCommandSetList(ceObject.Version);
    }

    #region populating scroll lists, building the lists, and handling input

    //main scroll list, lets user click on a category to edit the value
    void PopulateEditScrollList(ClassEditObject ce)
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<AbilityBuilderObject> aboList = BuildABOList(ceObject);

        foreach (AbilityBuilderObject i in aboList)
        {
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            string tempString = i.Title;
            tb.title.text = i.Title;
            tb.details.text = i.Value;
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonCEEditClicked(tempString));
        }
    }

    //build the scroll list for PopulateEditScrollList
    List<AbilityBuilderObject> BuildABOList(ClassEditObject ce)
    {
        List<AbilityBuilderObject> aboList = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo = new AbilityBuilderObject(CE_CLASS_NAME, ce.ClassName);
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_VERSION, CalcCode.GetVersionFromInt(ce.Version));
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_ICON, CheckDict(iconDict, ce.Icon));
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_COMMAND_SET, CalcCode.GetCommandSetName(ce.CommandSet));
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_MOVE, ce.Move);
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_JUMP, ce.Jump);
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_HP_BASE, ce.HPBase);
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_MP_BASE, ce.MPBase);
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_SPEED_BASE, ce.SpeedBase);
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_PA_BASE, ce.PABase);
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_MA_BASE, ce.MABase);
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_AGI_BASE, ce.AgiBase);
        aboList.Add(abo);

        abo = new AbilityBuilderObject(CE_HP_GROWTH, ce.HPGrowth);
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_MP_GROWTH, ce.MPGrowth);
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_SPEED_GROWTH, ce.SpeedGrowth);
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_PA_GROWTH, ce.PAGrowth);
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_MA_GROWTH, ce.MAGrowth);
        aboList.Add(abo);
        abo = new AbilityBuilderObject(CE_AGI_GROWTH, ce.AgiGrowth);
        aboList.Add(abo);

        abo = new AbilityBuilderObject(CE_DELETE, "");
        aboList.Add(abo);
        return aboList;
    }

    //button on PopulateEditScrollList Clicked
    void ButtonCEEditClicked(string zTitle)
    {
        if( zTitle == CE_DELETE)
        {
            DeleteClassEditObject();
            return;
        }
        ceTitle = zTitle;
        OpenInput(zTitle);
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
    //build the scroll list for PopulateInputScrollList (ie user is selecting an input)
    List<AbilityBuilderObject> BuildInputList(string zTitle)
    {
        List<AbilityBuilderObject> aboList = new List<AbilityBuilderObject>();
        AbilityBuilderObject abo;

        if (zTitle == CE_VERSION)
        {
            abo = new AbilityBuilderObject(CE_INPUT_AURELIAN, "", NameAll.VERSION_AURELIAN);
            aboList.Add(abo);
            abo = new AbilityBuilderObject(CE_INPUT_CLASSIC, "", NameAll.VERSION_CLASSIC);
            aboList.Add(abo);
        }
        else if (zTitle == CE_COMMAND_SET)
        {
            foreach (CommandSet cs in commandSetList)
            {
                abo = new AbilityBuilderObject(cs.CommandSetName, "", cs.CommandSetId);
                aboList.Add(abo);
            }
        }
        else if (zTitle == CE_ICON)
        {
            foreach (KeyValuePair<int, string> kvp in iconDict)
            {
                abo = new AbilityBuilderObject(kvp.Value, "", kvp.Key);
                aboList.Add(abo);
            }
        }

        return aboList;
    }
    //button on PopulateInputScrollList clicked, update value and reopen the ceObject in the Edit list
    void ButtonInputClicked(int z1)
    {
        if (ceTitle == CE_VERSION)
        {
            ceObject.Version = z1;
        }
        else if (ceTitle == CE_COMMAND_SET)
        {
            ceObject.CommandSet = z1;
        }
        else if (ceTitle == CE_ICON)
        {
            ceObject.Icon = z1;
            LoadPUO();
        }
        OnClickEdit();
    }

    //opens the InputPanel, set the current inputfield/dropdown objects
    void OpenInput(string zTitle)
    {


        if (zTitle == CE_CLASS_NAME)
        {
            abilityInput.SetActive(true);
            aiTitle.text = zTitle;
            aiInputField.text = "";
            aiInputField.gameObject.SetActive(true);
            aiInputField.characterLimit = 15;
            aiInputField.characterValidation = InputField.CharacterValidation.Alphanumeric;
        }
        else if (zTitle == CE_COMMAND_SET || zTitle == CE_ICON || zTitle == CE_VERSION)
        {
            PopulateInputScrollList(ceTitle);
        }
        else if (zTitle == CE_CLASS_EVADE)
        {
            abilityInput.SetActive(true);
            aiTitle.text = zTitle;
            aiInputField.text = "";
            aiInputField.gameObject.SetActive(true);
            aiInputField.characterLimit = 2;
            aiInputField.characterValidation = InputField.CharacterValidation.Integer;
        }
        else
        {
            abilityInput.SetActive(true);
            aiTitle.text = zTitle;
            aiInputField.text = "";
            aiInputField.gameObject.SetActive(true);
            aiInputField.characterLimit = 4;
            aiInputField.characterValidation = InputField.CharacterValidation.Integer;
        }
    }


    //load button clicked, shows classes that have been saved
    void PopulateLoadScrollList(List<ClassEditObject> myList)
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < myList.Count; i++)
        {
            ClassEditObject ce = myList[i];
            GameObject newButton = Instantiate(sampleButton) as GameObject;
            AbilityEditScrollListButton tb = newButton.GetComponent<AbilityEditScrollListButton>();
            int tempInt = ce.ClassId;
            tb.title.text = "Class Name: " + ce.ClassName;
            tb.details.text = "Command Set: " + CalcCode.GetCommandSetName(ce.CommandSet);
            tb.transform.SetParent(contentPanel);

            Button tempButton = tb.GetComponent<Button>();
            tempButton.onClick.AddListener(() => ButtonCELoadClicked(tempInt));
        }
    }

    //when load is clicked, builds the list for the scrollView
    List<ClassEditObject> BuildCEList()
    {
        List<ClassEditObject> retList = new List<ClassEditObject>();
        int maxIndex = PlayerPrefs.GetInt(NameAll.PP_CLASS_EDIT_MAX_INDEX, 1000);
        for (int i = 1000; i <= maxIndex; i++)
        {
            ClassEditObject ce = CalcCode.LoadCEObject(i); Debug.Log("loading ceObject");
            retList.Add(ce);
        }

        return retList;
    }

    //click on the list, sets the current to the one that was clicked so it can be edited
    void ButtonCELoadClicked(int ceClassId)
    {
        int z1 = ceList.Count;
        for (int i = 0; i < z1; i++)
        {
            if (ceList[i].ClassId == ceClassId)
            {
                ceObject = ceList[i];
                OnClickEdit();
                break;
            }
        }

        //currentSNIndex = snIndex;
        //spellName = snList[snIndex];
        //OnClickType();
    }

    #endregion



    #region OnClickButtons
    //scrollist is populated with loadable objects
    public void OnClickLoad()
    {
		Debug.Log("clicking load ");
        ceList = BuildCEList();
        PopulateLoadScrollList(ceList);
    }
    //scrolllist is populated with current class, available to edit
    public void OnClickEdit()
    {
        PopulateEditScrollList(ceObject);

		//trying to test this with this off
		PlayerUnit pu = new PlayerUnit(ceObject); //Debug.Log(" pu stats " + pu.StatTotalMove + " " + ceObject.Move);
		puInfo.PopulatePlayerInfo(pu);
	}

    //new class is created and ready to edit
    public void OnClickNew()
    {
        int ceIndex = PlayerPrefs.GetInt(NameAll.PP_CLASS_EDIT_MAX_INDEX, NameAll.CUSTOM_CLASS_ID_START_VALUE);
        string fileName = Application.dataPath + "/Custom/Jobs/" + ceIndex + "_class.dat";
        if (File.Exists(fileName)) //saves sn exists at this place, update the snIndex and the PP
        {
            ceIndex += 1;
            PlayerPrefs.SetInt(NameAll.PP_CLASS_EDIT_MAX_INDEX, ceIndex);
            currentCEIndex = ceIndex;
        }
        else
        {
            currentCEIndex = ceIndex;
        }
        ceObject = new ClassEditObject(ceIndex, NameAll.CUSTOM_COMMAND_SET_ID_START_VALUE, NameAll.VERSION_AURELIAN);
        OnClickEdit();
    }

    public void OnClickExit()
    {
        SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
    }
    //saves the current class
    public void OnClickSave()
    {
		CalcCode.SaveCustomClassEditObject(ceObject);
		//      string filePath = Application.dataPath + "/Custom/Jobs/";
		//      string filename = filePath + ceObject.ClassId + "_class.dat"; Debug.Log("trying to save new job filepath: " + filename);
		//Debug.Log("trying to save ceObject  is: " + ceObject.ToString());
		//Debug.Log("trying to save ceObject  is: " + ceObject.GetCEObjectAsString());
		//Serializer.Save<ClassEditObject>(filename, ceObject);
		//Debug.Log("current ceObject  is: " + ceObject.GetCEObjectAsString());
		//ClassEditObject testCEObject = CalcCode.LoadCEObject(ceObject.ClassId);
		//Debug.Log("saved ceObject  is: " + testCEObject.GetCEObjectAsString());

		//trying to test save again
		//filename = filePath + 1019 + "_class.dat"; Debug.Log("trying to save new job filepath: " + filename);
		//ceObject.ClassName = "WTF IS HAPPENING";
		//Serializer.Save<ClassEditObject>(filename, ceObject);
		//testCEObject = CalcCode.LoadCEObject(1019);
		//Debug.Log("test saved ceObject  is: " + testCEObject.GetCEObjectAsString());

		//if (!Directory.Exists(filePath))
		//    Directory.CreateDirectory(filePath);
	}
    #endregion


    #region input fields
    void AddInputFieldListener()
    {
        InputField.SubmitEvent aiInputEvent = new InputField.SubmitEvent();
        aiInputEvent.AddListener(OnSubmitAIInput);
        aiInputField.onEndEdit = aiInputEvent;
        //aiInputField.characterValidation = InputField.CharacterValidation.Alphanumeric;
    }

    //input field has been changed, updates the current value and the value stored in the current ceObject
    void OnSubmitAIInput(string zString)
    {
        //PlayerUnitLevelStats puls = new PlayerUnitLevelStats();
        //baseStats = puls.GetCEBaseStats(ceObject); //for the new OnClickEdit

        if( ceTitle == "")
        {

        }
        else if (ceTitle == CE_CLASS_NAME)
        {
            ceObject.ClassName = zString;
            aiInputField.text = zString;
        }
        else if (ceTitle == CE_CLASS_EVADE)
        {
            int z1 = 10;
            if (Int32.TryParse(zString, out z1))
            {
                if (z1 <= 0)
                {
                    z1 = 1;
                }
                else if (z1 >= 100)
                {
                    z1 = 99;
                }
            }

            ceObject.ClassEvade = z1;
            aiInputField.text = z1.ToString();
        }
        else if( ceTitle == CE_AGI_BASE )
        {
            ceObject.AgiBase = GetValidGrowthStat(zString);
        }
        else if (ceTitle == CE_AGI_GROWTH)
        {
            ceObject.AgiGrowth = GetValidGrowthStat(zString);
        }
        else if (ceTitle == CE_HP_BASE)
        {
            ceObject.HPBase = GetValidGrowthStat(zString);
        }
        else if (ceTitle == CE_HP_GROWTH)
        {
            ceObject.HPGrowth = GetValidGrowthStat(zString);
        }
        else if (ceTitle == CE_JUMP)
        {
            int z1 = 3;
            if (Int32.TryParse(zString, out z1))
            {
                if (z1 <= 0)
                {
                    z1 = 1;
                }
                else if (z1 >= 100)
                {
                    z1 = 99;
                }
            }
            ceObject.Jump = z1;
            aiInputField.text = z1.ToString();
        }
        else if (ceTitle == CE_MOVE)
        {
            int z1 = 3;
            if (Int32.TryParse(zString, out z1))
            {
                if (z1 <= 0)
                {
                    z1 = 1;
                }
                else if (z1 >= 100)
                {
                    z1 = 99;
                }
            }
            ceObject.Move = z1;
            aiInputField.text = z1.ToString();
        }
        else if (ceTitle == CE_MA_BASE)
        {
            ceObject.MABase = GetValidGrowthStat(zString);
        }
        else if (ceTitle == CE_MA_GROWTH)
        {
            ceObject.MAGrowth = GetValidGrowthStat(zString);
        }
        else if (ceTitle == CE_MP_BASE)
        {
            ceObject.MPBase = GetValidGrowthStat(zString);
        }
        else if (ceTitle == CE_MP_GROWTH)
        {
            ceObject.MPGrowth = GetValidGrowthStat(zString);
        }
        else if (ceTitle == CE_PA_BASE)
        {
            ceObject.PABase = GetValidGrowthStat(zString);
        }
        else if (ceTitle == CE_PA_GROWTH)
        {
            ceObject.PAGrowth = GetValidGrowthStat(zString);
        }
        else if (ceTitle == CE_SPEED_BASE)
        {
            ceObject.SpeedBase = GetValidGrowthStat(zString);
        }
        else if (ceTitle == CE_SPEED_GROWTH)
        {
            ceObject.SpeedGrowth = GetValidGrowthStat(zString);
        }

    }

    //called in OnSubmitAIInput, makes sure the values are in the correct range
    int GetValidGrowthStat(string zString)
    {
        int z1 = 100;
        if (Int32.TryParse(zString, out z1))
        {
            if (z1 <= 0)
            {
                z1 = 1;
            }
            else if (z1 > 9999)
            {
                z1 = 9999;
            }
        }
        aiInputField.text = z1.ToString();
        return z1;
    }

    //closes the panel, reloads the OnClickEdit list (could simplify it so only the new field changes)
    public void OnInputPanelClose()
    {
        abilityInput.SetActive(false);
        OnClickEdit();
    }



    #endregion

    void LoadPUO()
    {
        Destroy(playerUnitObject);
        string puoString = NameAll.GetIconString(ceObject.Icon); //Debug.Log("asdf " + puoString);
        //string puoString = NameAll.GetPUOString(pu.ClassId); 
        playerUnitObject = Instantiate(Resources.Load(puoString), new Vector3(2.5f,0.0f,-1.0f), transform.rotation) as GameObject;
        playerUnitObject.SetActive(true);
        //SetUpPlayerManager();
        //StatusManager.Instance.GenerateAllStatusLasting(); //creates the lastign statuses from items and abilities at the beginning of the round
    }

    string CheckDict(Dictionary<int,string>myDict, int key)
    {
        if(myDict.ContainsKey(key))
        {
            return myDict[key];
        }
        return "";
    }

    void DeleteClassEditObject()
    {
        string filePath = Application.dataPath + "/Custom/Jobs/" + ceObject.ClassId + "_class.dat";
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
