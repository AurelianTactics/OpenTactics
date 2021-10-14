using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine.Serialization;
using Unity.MLAgents.Sensors;

public class GridworldTacticsAgent : Agent
{
	/// <summary>
	/// Handles agent logic for Gridworld game. Works tightly with GridworldTacticsArea.cs
	/// receives info from Area when turn starts, gets obs, produces action, relays action to Area
	/// </summary>
	
	public GridworldTacticsArea gridworldArea;
	public float timeBetweenDecisionsAtInference;
	float m_TimeSinceDecision;

	int NO_ACTION = 0;

	public override void Initialize()
	{
		//m_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();
		//teamId = m_BehaviorParameters.TeamId;
		//behaviorType = m_BehaviorParameters.BehaviorType;
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		//need state of each grid square
			//for now just the 16 squares but obviously better ways of doing it (feature layers, in relation to user etc)
			//0 for empty, 1 for reward, -1 for unit
		var gridBoard = gridworldArea.GetObservations();
		for (int i = 0; i < gridBoard.Length; i++)
		{
			//Debug.Log("testings observation " + i + " " + gridBoard[i]);
			sensor.AddObservation(gridBoard[i]);
		}
		//Debug.Log("collecting observations");
		
		
	}

	//// to be implemented by the developer
	public override void OnActionReceived(float[] vectorAction)
	{
		//noop, move up/down/left/right
		var action = Mathf.FloorToInt(vectorAction[0]);
		gridworldArea.TakeAction(action);
		//Debug.Log("OnActionReceived");
	}

	public override void Heuristic(float[] actionsOut)
	{
		//0 is no action, rest are movements with ASWD
		if (Input.GetKey(KeyCode.W))
		{
			actionsOut[0] = 1;
		}
		else if (Input.GetKey(KeyCode.A))
		{
			actionsOut[0] = 2;
		}
		else if (Input.GetKey(KeyCode.S))
		{
			actionsOut[0] = 3;
		}
		else if (Input.GetKey(KeyCode.D))
		{
			actionsOut[0] = 4;
		}
	}

	//// to be implemented by the developer
	public override void OnEpisodeBegin()
	{
		//don't think it's needed in this example because I'm resetting the board in GridworldTacticsArea
		//Debug.Log("OnEpisodeBegin");
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
			if (gridworldArea.isActReady)
			{
				RequestDecision();
			}
		}
		else
		{
			if (m_TimeSinceDecision >= timeBetweenDecisionsAtInference)
			{
				m_TimeSinceDecision = 0f;
				if (gridworldArea.isActReady)
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
