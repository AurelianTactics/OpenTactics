using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class CharacterClassPopup : MonoBehaviour {

    //[SerializeField]
    //private Dropdown dropdownClass;
    [SerializeField]
    private Dropdown dropdownSex;
    [SerializeField]
    private Dropdown dropdownColor;
    [SerializeField]
    private InputField inputName;
    [SerializeField]
    private InputField inputLevel;
    [SerializeField]
    private InputField inputBrave;
    [SerializeField]
    private InputField inputFaith;
    [SerializeField]
    private InputField inputCunning;

    //Dictionary<int, string> classDict;
    //List<string> classList;
    Dictionary<int, string> colorDict;
    List<string> colorList;

    const string CharacterBuilderNotification = "CharacterBuilderNotification";
    //const string MiscUnitNotification = "MiscUnitNotification";

    void Awake()
    {
        SetDropdowns();
    }

    void Start()
    {
        //SetDropdowns();
        dropdownSex.onValueChanged.AddListener(delegate {
            myDropdownSexValueChangedHandler(dropdownSex);
        });
        dropdownColor.onValueChanged.AddListener(delegate {
            myDropdownColorValueChangedHandler(dropdownColor);
        });
        //dropdownClass.onValueChanged.AddListener(delegate {
        //    myDropdownClassValueChangedHandler(dropdownClass);
        //});

        // Add listener to catch the submit
        InputField.SubmitEvent submitNameEvent = new InputField.SubmitEvent();
        submitNameEvent.AddListener(OnSubmitName);
        inputName.onEndEdit = submitNameEvent;
        // Add validation
        inputName.characterValidation = InputField.CharacterValidation.Alphanumeric;

        InputField.SubmitEvent submitLevelEvent = new InputField.SubmitEvent();
        submitLevelEvent.AddListener(OnSubmitLevel);
        inputLevel.onEndEdit = submitLevelEvent;
        inputLevel.characterValidation = InputField.CharacterValidation.Alphanumeric;

        InputField.SubmitEvent submitBraveEvent = new InputField.SubmitEvent();
        submitBraveEvent.AddListener(OnSubmitBrave);
        inputBrave.onEndEdit = submitBraveEvent;
        inputBrave.characterValidation = InputField.CharacterValidation.Alphanumeric;

        InputField.SubmitEvent submitFaithEvent = new InputField.SubmitEvent();
        submitFaithEvent.AddListener(OnSubmitFaith);
        inputFaith.onEndEdit = submitFaithEvent;
        inputFaith.characterValidation = InputField.CharacterValidation.Alphanumeric;

        InputField.SubmitEvent submitCunningEvent = new InputField.SubmitEvent();
        submitCunningEvent.AddListener(OnSubmitCunning);
        inputCunning.onEndEdit = submitCunningEvent;
        inputCunning.characterValidation = InputField.CharacterValidation.Alphanumeric;

    }

    public void Open()
    {
        gameObject.SetActive(true);
        UpdateAllFields();
    }

    void SetDropdowns()
    {
        dropdownSex.options.Clear();
        List<string> myList = new List<string>();
        myList.Add("Male");
        myList.Add("Female");
        dropdownSex.AddOptions(myList);
        //dropdownSex.options.Add(new Dropdown.OptionData("Male"));
        //dropdownSex.options.Add(new Dropdown.OptionData("Female"));
        //dropdownSex.value = 1; //this shouldn't be necessary..

        //classDict = AbilityManager.Instance.GetPrimaryDict();
        //classList = new List<string>();
        //classList.AddRange(classDict.Values);
        //dropdownClass.options.Clear();
        //dropdownClass.AddOptions(classList);

        colorDict = AbilityManager.Instance.GetColorDict();
        colorList = new List<string>();
        colorList.AddRange(colorDict.Values);
        dropdownColor.options.Clear();
        dropdownColor.AddOptions(colorList);
        //myList.Clear();
        //for (int i = 0; i < 12; i++)
        //{
        //    myList.Add(i.ToString());
        //}


    }
    

    private void myDropdownSexValueChangedHandler(Dropdown target)
    {
        //Debug.Log("selected: " + target.value);
        if( target.value == 0)
        {
            CharacterUIController.pu.SetSex("Male");
        } else
        {
            CharacterUIController.pu.SetSex("Female");
        }
        this.PostNotification(CharacterBuilderNotification);
    }

    //private void myDropdownClassValueChangedHandler(Dropdown target)
    //{
    //    string zString = classList[target.value];
    //    int z1 = 1;
    //    foreach (KeyValuePair<int, string> pair in classDict)
    //    {
    //        if (pair.Value == zString)
    //        {
    //            z1 = pair.Key; // Found
    //            break;
    //        }
    //    }
    //    CharacterUIController.pu.SetClassIdStatsUnequip(z1); 
    //}

    private void myDropdownColorValueChangedHandler(Dropdown target)
    {
        //Debug.Log("selected: " + target.value);
        string zString = colorList[target.value];
        int z1 = 0;
        foreach (KeyValuePair<int, string> pair in colorDict)
        {
            if (pair.Value == zString)
            {
                z1 = pair.Key; // Found
                break;
            }
        }
        CharacterUIController.pu.ZodiacInt = target.value;
        this.PostNotification(CharacterBuilderNotification);
    }

    
    public void Close()
    {
        //CharacterUIController.classUpdate = 1;
        this.PostNotification(CharacterBuilderNotification);
        gameObject.SetActive(false);
    }

    public void OnSubmitName(string name)
    {
        CharacterUIController.pu.UnitName = name; 
        inputName.text = name;//CharacterUIController.pu.UnitName;
        //this.PostNotification(MiscUnitNotification);
		this.PostNotification(CharacterBuilderNotification);
	}

    public void OnSubmitLevel(string var)
    {
        int z1 = CharacterUIController.pu.Level;
        if (Int32.TryParse(var, out z1))
        {
            if (z1 < 1)
            {
                z1 = 1;
            }
            else if (z1 > 99)
            {
                z1 = 99;
            }
        }
        else
        {
            z1 = CharacterUIController.pu.Level;
        }

        CharacterUIController.pu.SetLevel(z1);
        inputLevel.text = CharacterUIController.pu.Level.ToString();
        this.PostNotification(CharacterBuilderNotification);
    }

    public void OnSubmitBrave(string var)
    {
        bool isClassicClass = NameAll.IsClassicClass(CharacterUIController.pu.ClassId);
        int z1 = CharacterUIController.pu.StatUnitBrave;
        if (Int32.TryParse(var, out z1))
        {
            if (z1 < 40)
            {
                z1 = 40;
            }

            if (isClassicClass)
            {
                if (z1 > 70)
                {

                    z1 = 70;
                }
            }
            else
            {
                if (z1 > 60)
                {

                    z1 = 60;
                }
            } 

            if ( z1 == 1919)
            {
                z1 = 100;
            }

        } else
        {
            z1 = CharacterUIController.pu.StatUnitBrave;
        }
        CharacterUIController.pu.SetStatUnitBrave(z1);
        inputBrave.text = CharacterUIController.pu.StatUnitBrave.ToString();
        this.PostNotification(CharacterBuilderNotification);
    }

    public void OnSubmitFaith(string var)
    {
        bool isClassicClass = NameAll.IsClassicClass(CharacterUIController.pu.ClassId);
        int z1 = CharacterUIController.pu.StatUnitFaith;
        if (Int32.TryParse(var, out z1))
        {
            if (z1 < 40)
            {
                z1 = 40;
            }

            if(isClassicClass)
            {
                if (z1 > 70)
                {

                    z1 = 70;
                }
            }
            else
            {
                if (z1 > 60)
                {

                    z1 = 60;
                }
            } 
        }
        else
        {
            z1 = CharacterUIController.pu.StatUnitFaith;
        }
        CharacterUIController.pu.SetStatUnitFaith(z1); // SetStat_unit_faith(z1);
        inputFaith.text = CharacterUIController.pu.StatUnitFaith.ToString();
        this.PostNotification(CharacterBuilderNotification);
    }

    public void OnSubmitCunning(string var)
    {
        bool isClassicClass = NameAll.IsClassicClass(CharacterUIController.pu.ClassId);
        int z1 = CharacterUIController.pu.StatUnitCunning;
        if (Int32.TryParse(var, out z1))
        {
            if (z1 < 40)
            {
                z1 = 40;
            }
            if (isClassicClass)
            {
                if (z1 > 70)
                {

                    z1 = 70;
                }
            }
            else
            {
                if (z1 > 60)
                {

                    z1 = 60;
                }
            }
        }
        else
        {
            z1 = CharacterUIController.pu.StatUnitCunning;
        }
        CharacterUIController.pu.SetStatUnitCunning(z1);
        inputCunning.text = CharacterUIController.pu.StatUnitCunning.ToString();
        this.PostNotification(CharacterBuilderNotification);
    }

    //int GetIntFromString(string zString, int min, int max)
    //{
    //    int z1 = 10;
    //    bool result = Int32.TryParse(zString, out z1);
    //    if (result)
    //    {
    //        if (z1 < 4)
    //        {
    //            z1 = 4;
    //        }
    //        else if (z1 > 20)
    //        {
    //            z1 = 20;
    //        }
    //    }
    //    else
    //    {
    //        z1 = 10;
    //    }
    //    return z1;
    //}

    public void UpdateAllFields()
    {
        inputName.text = CharacterUIController.pu.UnitName;
        inputLevel.text = CharacterUIController.pu.Level.ToString();
        inputBrave.text = CharacterUIController.pu.StatUnitBrave.ToString();
        inputFaith.text = CharacterUIController.pu.StatUnitFaith.ToString();
        inputCunning.text = CharacterUIController.pu.StatUnitCunning.ToString();

        if (CharacterUIController.pu.Sex.Equals("Male"))
        {
            dropdownSex.value = 0;
        }
        else
        {
            dropdownSex.value = 1;
        }

        //dropdownSex.value = 0;
        int z1;
        z1 = colorList.IndexOf(colorDict[CharacterUIController.pu.ZodiacInt]);
        dropdownColor.value = z1;//CharacterUIController.pu.ZodiacInt;
        //z1 = classList.IndexOf(classDict[CharacterUIController.pu.ClassId]);
        //dropdownClass.value = z1; //Debug.Log("in updateallfields " + z1);
        //dropdownClass.value = CharacterUIController.pu.ClassId;
        //Debug.Log("class value is " + CharacterUIController.pu.ClassId);
    }
}
