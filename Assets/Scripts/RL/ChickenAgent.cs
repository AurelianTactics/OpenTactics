using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine.Serialization;
using Unity.MLAgents.Sensors;

/*

Needs to be able to 
NEED TO TEST handle reset (how do unity examples do it?)
	no reset function in the agent. maybe in the base Agent.cs that can be overriden?
	https://github.com/Unity-Technologies/ml-agents/blob/develop/com.unity.ml-agents/Runtime/Agent.cs
	so I think reset would hit the academy from API which passes it to agent?
read action and send to game loop state
	pretty sure this is OnActionReceived
send obs back out
	pretty sure this is collectobervations
send done back out
	I think this is here:
	https://github.com/Unity-Technologies/ml-agents/blob/develop/com.unity.ml-agents/Runtime/Agent.cs#L45
action mask?
	https://github.com/Unity-Technologies/ml-agents/blob/develop/com.unity.ml-agents/Runtime/Agent.cs#L30
	unclear if can be sent in info or not
	more of a to do
reward
	https://github.com/Unity-Technologies/ml-agents/blob/develop/com.unity.ml-agents/Runtime/Agent.cs#L730
	so this only occurs after the action hits the gameloopstate and is processed
	could alternatively be end of game when done is passed in
info
	unclear how to send this out

so if it was tightly coupled (and not sure it can be? but maybe try?)
	the agent has an instance of gameloopstate
	gameloopstate has an instance of agent
	init or reset
	combatinit happens 
	moves too gameloopstate
		instantiates the agent and passes a gameloopstate instance to it
	gameloopstate does whatever. eventually needs an action
		gives obs, reward, done, info to agent
			reward is tricky since has to be stored side specific since the last time
			guess only need from side 1 for initial thing
		invokes agent to request decision
			agent passes obs, reward, done, info out to gym api
		gameloopstate hangs until receives notification from agent (or if tightly coupled agent hangs? i feel this would cause a crash?)
		gym api takes that info, passes an action in to step
			agent takes the action, passes it to gameloopstate until it done
	on done
		rest call from gym, flows to agent then to gameloopstate (through a notification?)
 */

/// <summary>
/// Test agent for a 1 vs. 1 test script with working thoruhg the python gym api unity offers
/// seeing how things work
/// </summary>
public class ChickenAgent : Agent
{
	

	public DuelRLArea areaScript;
	public float timeBetweenDecisionsAtInference;
	float m_TimeSinceDecision;
	int teamId;
	BehaviorParameters m_BehaviorParameters;

	public override void Initialize()
	{
		m_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();
		teamId = m_BehaviorParameters.TeamId;
		//behaviorType = m_BehaviorParameters.BehaviorType;
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		//collected in area script from PlayerManager function. this seems like a poorly designed way to do this
		var obsArray = areaScript.GetObservations();
		for( int i = 0; i < obsArray.Length; i++){
			sensor.AddObservation(obsArray[i]);
		}
		
	}


	public override void OnActionReceived(float[] vectorAction)
	{
		//3 branches of actions
		//wait, move, attack, primary
		//which primary abilities (8 total)
		//target tile(also used for wait lower left is south, upper right i north, lower right is east, upper left is west)


		//Debug.Log("testing RANDOM ACTION TURN THIS OFF WHEN DONE TESTING ");
		//vectorAction[0] = (float)UnityEngine.Random.Range(1, 3); //testing attack
		//vectorAction[1] = (float)UnityEngine.Random.Range(0, 8);
		//vectorAction[2] = (float)UnityEngine.Random.Range(0, 4);
		//vectorAction[0] = 2.0f;
		//vectorAction[1] = 0.0f;
		//vectorAction[2] = 1.0f;
		//vectorAction[2] = (float)UnityEngine.Random.Range(0, 4);
		//Debug.Log("testing actions" + vectorAction[0] + vectorAction[1] + vectorAction[2]);

		areaScript.TakeAction(vectorAction, teamId);

		//Debug.Log("OnActionReceived");
	}

	public override void Heuristic(float[] actionsOut)
	{
		//can test with this later

		//0 is no action, rest are movements with ASWD
		//if (Input.GetKey(KeyCode.W))
		//{
		//	actionsOut[0] = 1;
		//}
		//else if (Input.GetKey(KeyCode.A))
		//{
		//	actionsOut[0] = 2;
		//}
		//else if (Input.GetKey(KeyCode.S))
		//{
		//	actionsOut[0] = 3;
		//}
		//else if (Input.GetKey(KeyCode.D))
		//{
		//	actionsOut[0] = 4;
		//}
	}

	//// to be implemented by the developer
	public override void OnEpisodeBegin()
	{
		//Debug.Log("TEST OnEpisodeBegin, make sure this lines up with the board reset");
	}

	//ug i'd rather not use these like I did in tictactoe but might need to if i can't figure out wtf I was doing wrong
	public void FixedUpdate()
	{
		WaitTimeInference();
	}

	void WaitTimeInference()
	{
		if (Academy.Instance.IsCommunicatorOn)
		{
			if (areaScript.teamTurn == teamId)
			{
				RequestDecision();
			}
		}
		else
		{
			if (m_TimeSinceDecision >= timeBetweenDecisionsAtInference)
			{
				m_TimeSinceDecision = 0f;
				if (areaScript.teamTurn == teamId)
				{
					//Debug.Log("RequestDecision");
					RequestDecision();
				}
			}
			else
			{
				m_TimeSinceDecision += Time.fixedDeltaTime;
			}
		}
	}

}
