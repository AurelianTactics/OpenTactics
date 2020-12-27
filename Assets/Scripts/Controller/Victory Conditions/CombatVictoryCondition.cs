using UnityEngine;
using System.Collections;

public class CombatVictoryCondition : MonoBehaviour
{
    #region Fields & Properties
    //public Alliances Victor
    //{
    //    get { return victor; }
    //    protected set { victor = value; }
    //}
    //Alliances victor = Alliances.None;

    public Teams Victor
    {
        get { return victor; }
        set { victor = value; }
    }
    Teams victor = Teams.None;

    //int victoryType = NameAll.VICTORY_TYPE_DEFEAT_PARTY;

    public int VictoryType
    {
        get { return victoryType; }
        set { victoryType = value; }
    }
    int victoryType = NameAll.VICTORY_TYPE_DEFEAT_PARTY;

    string teamId;
    public const string DidAbleToFightChangeNotification = "PlayerUnit.AbleToFightDidChange";
    //protected BattleController bc;
    #endregion

    #region MonoBehaviour
    //don't need a reference to the BattleController, accessing the PlayerManager Singleton directly
    //protected virtual void Awake()
    //{
    //    bc = GetComponent<BattleController>();
    //}

    void OnEnable()
    {
        //this.AddObserver(OnHPDidChangeNotification, Stats.DidChangeNotification(StatTypes.HP));
        this.AddObserver(OnAbleToFightChangeNotification, DidAbleToFightChangeNotification);
    }

    void OnDisable()
    {
        //this.RemoveObserver(OnHPDidChangeNotification, Stats.DidChangeNotification(StatTypes.HP));
        this.RemoveObserver(OnAbleToFightChangeNotification, DidAbleToFightChangeNotification);
    }
    #endregion

    #region Notification Handlers
    void OnAbleToFightChangeNotification(object sender, object args)
    {
        //uncertain how to get the teamId from the args as a parameter
        CheckForGameOver();
    }

    //in future maybe implement something for defeat a unit down to x% of health
    //protected virtual void OnHPDidChangeNotification(object sender, object args)
    //{
    //    CheckForGameOver();
    //}
    #endregion

    #region other

    void CheckForGameOver()
    {
        //for now just doing the default, in the future allow different arguments
        if( victoryType == NameAll.VICTORY_TYPE_DEFEAT_PARTY || victoryType == NameAll.VICTORY_TYPE_RL_RESET_EPISODE)
        {
            Victor = PlayerManager.Instance.CheckForDefeat();
        }

    }

    //protected virtual void CheckForGameOver()
    //{
    //    if (PartyDefeated(Alliances.Hero))
    //        Victor = Alliances.Enemy;
    //}

    //protected virtual bool PartyDefeated(Alliances type)
    //{
    //    for (int i = 0; i < bc.units.Count; ++i)
    //    {
    //        Alliance a = bc.units[i].GetComponent<Alliance>();
    //        if (a == null)
    //            continue;

    //        if (a.type == type && !IsDefeated(bc.units[i]))
    //            return false;
    //    }
    //    return true;
    //}

    //protected virtual bool IsDefeated(Unit unit)
    //{
    //    Health health = unit.GetComponent<Health>();
    //    if (health)
    //        return health.MinHP == health.HP;

    //    Stats stats = unit.GetComponent<Stats>();
    //    return stats[StatTypes.HP] == 0;
    //}
    #endregion
}