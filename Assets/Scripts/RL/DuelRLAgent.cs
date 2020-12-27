using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine.Serialization;
using Unity.MLAgents.Sensors;

public class DuelRLAgent : Agent
{
	/// <summary>
	/// Handle agent logic for DuelRL, works with DuelRLArea.cs script
	/// </summary>

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
