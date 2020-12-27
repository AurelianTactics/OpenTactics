using UnityEngine;
using System.Collections;

//instance is found in PlayerManager
//allows Master and Other to communicate readiness status
//function in PlayerManager allows CombatState to take necessary MP statuses
public class CombatMultiplayerObject {

    public bool IsOffline { get; set; }
    public bool IsMasterClient { get; set; }
    public bool IsReady { get; set; }
    public bool IsOpponentReady { get; set; }
    public Phases SelfCurrentPhase { get; set; }
    public Phases OpponentCurrentPhase { get; set; }
    public int NumberOfPlayers { get; set; }

    public CombatMultiplayerObject()
    {
        this.IsOffline = true;
        this.IsReady = true;
        this.IsOpponentReady = true;
        this.SelfCurrentPhase = Phases.Prephase;
        this.OpponentCurrentPhase = Phases.Prephase;
        this.IsMasterClient = false;
        this.NumberOfPlayers = 2;
    }

    public CombatMultiplayerObject( bool zIsOffline)
    {
        this.IsOffline = zIsOffline;
        this.IsReady = true;
        this.IsOpponentReady = true;
        this.SelfCurrentPhase = Phases.Prephase;
        this.OpponentCurrentPhase = Phases.Prephase;
        this.IsMasterClient = false;
        this.NumberOfPlayers = 2;
    }

}
