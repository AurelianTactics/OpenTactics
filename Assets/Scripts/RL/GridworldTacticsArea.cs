using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class GridworldTacticsArea : MonoBehaviour
{
	/// <summary>
	/// Implements Gridworld RL env with game
	/// Agent gets reward for reaching the goal, then the goal resets
	/// </summary>
	[SerializeField] Board board; //script for holding information about the game board and updating positions
	public GridworldTacticsAgent m_Agent; //agent script

	int TILE_EMPTY = 0;
	int TILE_AGENT = -1;
	int TILE_GOAL = 1;
	int NO_ACTION = 0;
	float REWARD_MOVE = -0.1f; //reward for moving
	float REWARD_GOAL = 1.0f; //reward for reaching goal
	public bool isBoardSet = false; //don't let agent move until board is set up
	public bool isActReady = false; //don't let agent move again until action is ready
	int NOT_SET = NameAll.NULL_INT; //

	int goal_index; //stores index of tile reward is on
	int agent_index; //stores tile index of tile player is on
	int board_size; //used for size of observation array
	//int[] observationArray; //stores observations. all 0's except for TILE_AGENT index position and TILE_GOAL position. 0,0 is index 0. +1 x moves index up 1, +1 y moves index up maxX


	void Start()
    {
		goal_index = NOT_SET;
		agent_index = NOT_SET;
		//InitializeBoard();
		//goal_index = board.GetGridworldGoal();
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		if (!isBoardSet)
		{
			isBoardSet = board.isGridworldBoardSet;
			if (isBoardSet)
			{
				goal_index = board.GetGridworldGoal();
				agent_index = PlayerManager.Instance.GetGridworldAgentIndex(board.max.x);
				isBoardSet = true;
				isActReady = true;
				board_size = (board.max.x + 1) * (board.max.y + 1);
			}
			
		}
    }

	// on end of episode. reset goal, initial position, board size
	void BoardReset()
	{
		m_Agent.SetReward(REWARD_GOAL);
		m_Agent.EndEpisode();
		//Debug.Log("resetting episode, reward was " + REWARD_GOAL);
		goal_index = board.ResetGridworldGoal();
		isActReady = true;
	}

	public void TakeAction(int action)
	{
		isActReady = false; //don't allow more actions until this one completes
		//move unit to that place on board and update player coordinates
		if(action != NO_ACTION)
			agent_index = PlayerManager.Instance.MoveGridworldAgent(action, board);

		if ( agent_index == goal_index) //player is on same tile as goal, end episode
		{
			BoardReset();
			return;
		}
		m_Agent.SetReward(REWARD_MOVE);
		//Debug.Log("testing reward was " + REWARD_MOVE);
		isActReady = true;
	}

	public int[] GetObservations()
	{
		//Debug.Log("getting board size " + board.max.x + " " + board.max.y);
		int[] observationArray = new int[board_size]; //all values default to 0
		observationArray[agent_index] = TILE_AGENT;
		observationArray[goal_index] = TILE_GOAL;
		//Debug.Log("testing index places of agent " + agent_index + " and goal " + goal_index );
		return observationArray;
	}

	//int GetBoardSize(int min = 4, int max=8)
	//{
	//	return UnityEngine.Random.Range(min, max+1);
	//}

	//void InitializeBoard()
	//{
	//	//get board size
	//	xSize = GetBoardSize(sizeMin, sizeMax);
	//	ySize = GetBoardSize(sizeMin, sizeMax);
	//	//reset the boardArray
	//}

}
