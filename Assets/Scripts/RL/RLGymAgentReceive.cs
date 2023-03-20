using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

/// <summary>
/// When an agent has decided upon an action returns through to do
/// This class is created and return to gameLoopstate
/// </summary>
public class RLGymAgentReceive
{
	/// <summary>
	/// ID to match requests with actions received
	/// </summary>
	public int id;

	/// <summary>
	/// action returned from the agent
	/// </summary>
	public int action;

	RLGymAgentReceive(int id, int action)
	{
		this.id = id;
		this.action = action;

	}
	
} 
