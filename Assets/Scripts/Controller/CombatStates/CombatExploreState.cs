using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CombatExploreState : CombatState
{
    public override void Enter()
    {
        base.Enter();
        targetPanel.SetTargetPreview(board,pos);
        
    }

    public override void Exit()
    {
        base.Exit();
        targetPanel.Close();
    }

    protected override void OnMove(object sender, InfoEventArgs<Point> e)
    {
        SelectTile(e.info + pos);
        targetPanel.SetTargetPreview(board,pos);
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
        if (e.info == 1 || e.info == 2)
            owner.ChangeState<CombatCommandSelectionState>();
        else if( e.info == 0)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                Tile tile = hitObject.GetComponent<Tile>();
                if (tile != null)
                {
                    SelectTile(tile.pos);
                    targetPanel.SetTargetPreview(tile);
                }
                else
                {
                    PlayerUnitObject puo = hitObject.GetComponent<PlayerUnitObject>();
                    if( puo != null)
                    {
                        PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(puo.UnitId);
                        SelectTile(pu);
                        targetPanel.SetTargetPreview(pu);
                    }
                }
                
            }
        }
    }
}
