using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For RL from black box, load env creation from a JSON
/// Once the communcation layer is activated, unclear how much of this will remain
/// https://docs.unity3d.com/Manual/JSONSerialization.html
/// </summary>
[Serializable]
public class RLEnvConfig
{
	public int board;
	public int turn;
	public int team_0_unit_0;
	public int team_1_unit_0;
}

/*
{
    "board": 0,
    "turn": 0,
    "team_0_unit_0": 0,
    "team_1_unit_0": 0
}
 */
