using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CombatExploreState : CombatState
{
	//when clicking on a turns list ability, highlight the tiles indicated by tiles
	List<Tile> tiles;

	public override void Enter()
    {
        base.Enter();
        targetPanel.SetTargetPreview(board,pos);
		tiles = new List<Tile>();
        
    }

    public override void Exit()
    {
		board.DeSelectTiles(tiles);
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

	// notifications
	// when turns list is clicked, highlight the board/aoe effect
	const string TurnsListClickPU = "TurnsListClick.PlayerUnit";
	const string TurnsListClickTurnObject = "TurnsListClick.TurnObject";

	void OnEnable()
	{
		this.AddObserver(OnTurnsListClickPU, TurnsListClickPU);
		this.AddObserver(OnTurnsListClickTurnObject, TurnsListClickTurnObject);
	}

	void OnDisable()
	{
		this.RemoveObserver(OnTurnsListClickPU, TurnsListClickPU);
		this.RemoveObserver(OnTurnsListClickTurnObject, TurnsListClickTurnObject);
	}

	void OnTurnsListClickPU(object sender, object args)
	{
		//this.PostNotification(TurnsListClickPU, unitId);
		Debug.Log("Recievign args: " + args);
		int unitID = (int)args;
		if (unitID != NameAll.NULL_UNIT_ID)
		{
			PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(unitID);
			SelectTile(pu);
			targetPanel.SetTargetPreview(pu);
		}
	}

	void OnTurnsListClickTurnObject(object sender, object args)
	{
		TurnObject to = (TurnObject)args;
		if( to != null)
		{
			if( to.targetX != NameAll.NULL_INT && to.targetY != NameAll.NULL_INT)
			{

				Tile t = owner.board.GetTile(to.targetX, to.targetY);
				targetPanel.SetTargetPreview(t);
				if (to.spellName != null)
				{
					//select tiles for the spell
					PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(to.GetActorId());
					CombatAbilityArea aa = new CombatAbilityArea();
					tiles = aa.GetTilesInArea(owner.board, pu, to.spellName, t, pu.Dir);
					turn.targetTile = currentTile;
					board.SelectTiles(tiles);
				}
				else
				{
					SelectTile(t.pos);
				}
			}
			else if(to.GetActorId() != NameAll.NULL_UNIT_ID)
			{
				PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(to.GetActorId());
				SelectTile(pu);
				targetPanel.SetTargetPreview(pu);
			}
		}
	}

}
