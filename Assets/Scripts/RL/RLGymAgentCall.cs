using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

/// <summary>
/// Sends message to agent that an action is needed
/// Stores items from the env that the agent needs ot make a decision
/// Called from GameLoopState
/// </summary>
public class RLGymAgentCall
{

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

	RLGymAgentCall(float[] obsArray, int id, int gameTick, int numRequests)
	{
		this.obsArray = obsArray;
		this.id = id;
		this.gameTick = gameTick;
		this numRequests = numRequests;

	}
	
} 
