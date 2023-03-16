using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

/// <summary>
/// Sends message to env that an action is needed
/// Stores items needed by the env
/// Called from GameLoopState
/// </summary>
public class RLMessageClass
{
	//so probably want a rquest, time requested, number requested
	//when it requests sends over the obs as well
	//flexible enough to handle later
	//maybe matching ids so won't consume the same thing twice

	/// <summary>
	/// Stores obs for the current state
	/// </summary>
	public float[] obsArray;

	/// <summary>
	/// ID to match requests with actions received
	/// </summary>
	public int id;

	/// <summary>
	/// game tick sent
	/// </summary>
	public int gameTick;

	/// <summary>
	/// Number of requests sent this tick. Can be used to handle too many requests sent
	/// </summary>
	public int numRequestsThisTick;

	RLMessageClass(float[] obsArray, int id, int gameTick, int numRequests)
	{
		this.obsArray = obsArray;
		this.id = id;
		this.gameTick = gameTick;
		this numRequests = numRequests;

	}
	
} 
