using UnityEngine;
using System.Collections;

public class CombatAbilitySelectState : BaseCombatAbilityMenuState
{

    public override void Enter()
    {
        base.Enter();
        EnableObservers();
        turn.spellName = null;
        turn.spellName2 = null;
        LoadMenu();
    }

    public override void Exit()
    {
        base.Exit();
        DisableObservers();
    }

    public const string DidBackClick = "AbilitySelect.BackClick";
    public const string DidAbilityClick = "AbilitySelect.AbilityClick";
    public const string DidMathSkillClick = "AbilitySelect.MathSkillClick";

    void EnableObservers()
    {
        this.AddObserver(OnBackClick, DidBackClick);
        this.AddObserver(OnAbilityClick, DidAbilityClick);
        this.AddObserver(OnMathSkillClick, DidMathSkillClick);
    }

    void DisableObservers()
    {
        this.RemoveObserver(OnBackClick, DidBackClick);
        this.RemoveObserver(OnAbilityClick, DidAbilityClick);
        this.RemoveObserver(OnMathSkillClick, DidMathSkillClick);
    }

    void OnBackClick(object sender, object args)
    {
        Cancel();
        
    }

    void OnAbilityClick(object sender, object args)
    {
        int abilityId = (int)args; 
        turn.spellName = SpellManager.Instance.GetSpellNameByIndex(abilityId); //Debug.Log("on ability click " + turn.spellName.CommandSet);

        if( turn.spellName.CommandSet == NameAll.COMMAND_SET_MATH_SKILL)
        {
            abilityMenu.Open(turn, isMathSkill:true); //is math skill
        }
        else
        {
            //Debug.Log("in combatabilityselectstate changing state ");
            owner.ChangeState<CombatTargetAbilityState>();
        }
    }

    void OnMathSkillClick(object sender, object args)
    {
        int abilityId2 = (int)args;
        turn.spellName2 = SpellManager.Instance.GetSpellNameByIndex(abilityId2);
        Debug.Log("in combatabilityselectstate changing state ");
        owner.ChangeState<CombatTargetAbilityState>();
    }

    protected override void LoadMenu()
    {
        abilityMenu.Open(turn,isMathSkill:false);
    }

    protected override void Confirm()
    {
        Debug.Log("in confirm, ignoring I think, activeturn menu handles the input");
    }

    protected override void Cancel()
    {
        abilityMenu.Close();
        owner.ChangeState<CombatCommandSelectionState>();
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
        //Debug.Log("2 in on fire event " + e.info.ToString());
        //UIAbilityScrollListHandles the clicks
        //if (e.info == 0)
        //    Confirm();
        //else
        //    Cancel();
        if (e.info == 1)
        {
            Cancel();
        }
    }

    protected override void OnMove(object sender, InfoEventArgs<Point> e)
    {
        //Debug.Log("Move 2" + e.info.ToString());
        //ActiveTurn menu handles the move
        //if (e.info.x > 0 || e.info.y < 0)
        //    activeMenu.
        //else
        //    abilityMenuPanelController.Previous();
    }

}
