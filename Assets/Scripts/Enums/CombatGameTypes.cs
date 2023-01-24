using UnityEngine;
using System.Collections;

/// <summary>
/// Game types in combat. Managers the flow of states
/// </summary>
public enum Alliances
{
    /// <summary>
    /// One player controls one side. AI controls the rest
    /// </summary>
    OnePlayerVsAI = 0,

    /// <summary>
    /// Reinforcement Learning Training. Both sides seek inputs from RL agent
    /// </summary>
    ReinforcementLearningTraining,

}