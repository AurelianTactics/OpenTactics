using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseCombatAbilityMenuState : CombatState
{
    protected string menuTitle;
    protected List<string> menuOptions;

    public override void Enter()
    {
        base.Enter();
        //SelectTile(turn.actor.tile.pos);
        SelectMenuUnitTile();
        //if (driver.Current == Drivers.Human)
        //    LoadMenu();
        //if (driver == Drivers.Human)
        //    LoadMenu();
    }

    public override void Exit()
    {
        base.Exit();
        //abilityMenuPanelController.Hide();
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
        if (e.info == 0)
            Confirm();
        else
            Cancel();
    }

    protected override void OnMove(object sender, InfoEventArgs<Point> e)
    {
        //if (e.info.x > 0 || e.info.y < 0)
        //    abilityMenuPanelController.Next();
        //else
        //    abilityMenuPanelController.Previous();
    }

    public void SelectMenuUnitTile()
    {
        //not doing from now, called at beginnin of pu
        //int puId = PlayerManager.Instance.GetNextTurnPlayerUnitId();
        //if( puId != NameAll.NULL_INT)
        //{
        //    PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(puId)
        //    SelectTile(pu);
        //    Debug.Log("Add info to select tile");
        //}
    }

    protected abstract void LoadMenu();
    protected abstract void Confirm();
    protected abstract void Cancel();
}