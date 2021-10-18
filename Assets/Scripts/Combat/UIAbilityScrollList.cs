using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIAbilityScrollList : MonoBehaviour
{
    public Button backButton;
    public GameObject abilityButton;
    public Transform contentPanel;
    List<SpellName> spellNameList = new List<SpellName>();
    List<GameObject> buttonList = new List<GameObject>();
    int selection = 0; //where it is in the list
    public GameObject spellNameDetails;

    UIBackButton backButtonUI;

    void Start()
    {
        backButtonUI = backButton.GetComponent<UIBackButton>();
    }

    public void Open(CombatTurn turn, bool isMathSkill = false)
    {
        gameObject.SetActive(true);
        int commandSet;
        if( turn.isPrimary)
        {
            if( turn.actor.ClassId >= NameAll.CUSTOM_CLASS_ID_START_VALUE)
            {
                ClassEditObject ce = CalcCode.LoadCustomClass(turn.actor.ClassId);
				Debug.Log("Testing loading custom command set. classId is " + turn.actor.ClassId);
				if (ce != null)
				{
					commandSet = ce.CommandSet;
				}
                else
                    commandSet = NameAll.NULL_INT;
				Debug.Log("Testing loading custom command set. comandset number is " + commandSet);
            }
            else
                commandSet = turn.actor.ClassId;
        }
        else
        {
            commandSet = turn.actor.AbilitySecondaryCode;
        }
        
        spellNameList.Clear();
        buttonList.Clear();
        //Debug.Log("list size is " + spellNameList.Count);
        if (!isMathSkill)
            PopulateSpellNames(turn.actor, commandSet, isMathSkill:false);
        else
            PopulateSpellNames(turn.actor, commandSet, isMathSkill:true, abilityId:turn.spellName.Index);
    }

    public void Close()
    {
        //Debug.Log("closing ability scroll list?");
        gameObject.SetActive(false);
    }

    const string DidBackClick = "AbilitySelect.BackClick";
    const string DidAbilityClick = "AbilitySelect.AbilityClick";
    const string DidMathSkillClick = "AbilitySelect.MathSkillClick";
    const string DidInfoButtonClick = "AbilitySelect.InfoButtonClick";
    const string TurnsMenuAdd = "TurnsMenu.AddItem";
    public void ActBack()
    {
        //Debug.Log("act back clicked");
        this.PostNotification(DidBackClick, 0);
        Close();
    }

    public static string DidAbilityMenuNotification(string clickType)
    {
        return clickType;
    }

    void PopulateSpellNames(PlayerUnit actor, int commandSet, bool isMathSkill = false, int abilityId = -19)
    {
        foreach (Transform child in contentPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
        //Debug.Log("populating scroll list " + isMathSkill);
        if (!isMathSkill) //is not a math skill ability, just grab the nomral commandset 
        {
            spellNameList = SpellManager.Instance.GetSpellNamesByCommandSet(commandSet, actor); Debug.Log("getting command set " + commandSet);
            if (PlayerManager.Instance.IsAbilityEquipped(actor.TurnOrder, NameAll.SUPPORT_DEFEND, NameAll.ABILITY_SLOT_SUPPORT))
            {
                spellNameList.Add(SpellManager.Instance.GetSpellNameByIndex(NameAll.SPELL_INDEX_DEFEND));
            }
        }
        else //is  a mathSkill ability, have to grab the second list of spell
        {
            int classId = actor.ClassId;
            if (classId != NameAll.COMMAND_SET_MATH_SKILL) //ie not a calculator
            {
                spellNameList = SpellManager.Instance.GetSpellNamesByCommandSet(classId, actor, 1);
            }
            else //is a calculator, grabbing the secondary
            {
                classId = actor.AbilitySecondaryCode;
                spellNameList = SpellManager.Instance.GetSpellNamesByCommandSet(classId, actor, 1);
            }

        }

        foreach (SpellName sn in spellNameList)
        {
            GameObject newButton = Instantiate(abilityButton) as GameObject;
            buttonList.Add(newButton);
            UISampleButton sb = newButton.GetComponent<UISampleButton>();
            sb.spellName.text = sn.AbilityName;// + "(" + sn.GetMp() + ")";
            sb.spellDetails.text = " MP: " + CalculationResolveAction.GetMPCost(actor, sn) + " CTR: " + CalculationAT.CalculateCTR(actor, sn);
            sb.transform.SetParent(contentPanel);
            sb.spellId = sn.SpellId;
            //sb.button.onClick = myClick;
            int tempInt = sn.Index;

            //sb.button.onClick.AddListener(() => ButtonClicked(tempInt));
            Button tempButton = sb.GetComponent<Button>();
            //int tempInt = i;

            if (isMathSkill) //first spell already gotten, getting second spell
            {
                //Debug.Log("isMathSkill is true, sending the ButtonClickedMathSkill to CombatAbilitySelectState");
                tempButton.onClick.AddListener(() => ButtonClickedMathSkill(actor, commandSet, abilityId, tempInt));
            }
            else //sends spell to handler which passies it to CombatAbilitySelectState
            {
                //Debug.Log("isMathSkill is false, sending normal ability to CombatAbilitySelectState");
                tempButton.onClick.AddListener(() => ButtonClicked(tempInt));
            }

            sb.infoButton.onClick.AddListener(() => InfoButtonClicked(tempInt,actor.TurnOrder));
        }

        selection = 0;
        if( buttonList.Count > 0)
        {
            buttonList[selection].GetComponent<UISampleButton>().button.Select();
        }
    }
    
    void ButtonClicked(int spellIndex)
    {
        Close();
        this.PostNotification(DidAbilityClick, spellIndex);
    }

    void ButtonClickedMathSkill(PlayerUnit actor, int commandSet, int spellIndex, int spellIndex2)
    {
        Close();
        Debug.Log("did buttomClickedMathSkill notification sent");
        this.PostNotification(DidMathSkillClick, spellIndex2); //first spellIndex is already in turn
    }

    void InfoButtonClicked(int spellIndex, int actorId)
    {
        //Debug.Log("info button clicked, about to post notification " + spellIndex);
        spellNameDetails.SetActive(true);
        this.PostNotification(DidInfoButtonClick, spellIndex);
        Dictionary<int, SpellName> tempDict = new Dictionary<int, SpellName>();
        tempDict.Add( actorId, SpellManager.Instance.GetSpellNameByIndex(spellIndex));
        this.PostNotification(TurnsMenuAdd, tempDict);
        //GameObject go = GameObject.FindWithTag("SpellNameDetails");
        //go.SetActive(true);

        //var goList = GameObject.FindGameObjectsWithTag("spellDetails");
        //Debug.Log("Asdf " + goList.Length);
        //if( goList.Length > 0)
        //{
        //    Debug.Log("info button clicked, about to post notification " + spellIndex);
        //    GameObject go = goList[0];
        //    go.SetActive(true);
        //    this.PostNotification(DidInfoButtonClick, spellIndex);
        //}

    }

    #region input
    void OnEnable()
    {
        InputController.moveEvent += OnMoveEvent;
        InputController.fireEvent += OnFireEvent;
    }

    void OnDisable()
    {
        InputController.moveEvent -= OnMoveEvent;
        InputController.fireEvent -= OnFireEvent;
    }

    void OnFixedUpdate()
    {
        if (buttonList.Count > 0)
        {
            int index = Mathf.Abs(selection % buttonList.Count);
            buttonList[index].GetComponent<UISampleButton>().button.Select();
        }
    }

    void OnMoveEvent(object sender, InfoEventArgs<Point> e)
    {
        //Debug.Log("Move " + e.info.ToString());
        //SelectTile(e.info + pos);

        //highlights the next button in the buttonList
        if (buttonList.Count > 0)
        {
            if (e.info.x > 0 || e.info.y < 0)
            {
                selection += 1;

            }
            else
            {
                selection -= 1;
            }
            if (selection < 0)
            {
                selection = buttonList.Count - 1;
            }
            else if (selection >= buttonList.Count)
            {
                selection = selection % buttonList.Count;
            }
            int index = selection; //Debug.Log("asdf" + index + " " + selection + " " + buttonList.Count); //Mathf.Abs(selection % buttonList.Count); 
            buttonList[index].GetComponent<UISampleButton>().button.Select();
            selection = index;
        }

    }

    void OnFireEvent(object sender, InfoEventArgs<int> e)
    {
        //Debug.Log("1 in on fire event " + e.info.ToString());
        //click 0 handled by buttons, click 1 (cancel) handled by state, click 2 takes the confirm as an input on the highlighted button
        if (buttonList.Count > 0)
        {
            if (e.info == 2)
            {

                int index = selection % buttonList.Count; //Debug.Log("about to trigger button OnClick " + selection);
                buttonList[index].GetComponent<UISampleButton>().button.onClick.Invoke();
            }
        }
    }
    #endregion
}
