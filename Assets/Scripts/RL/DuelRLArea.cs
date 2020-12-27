using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class DuelRLArea : MonoBehaviour
{
	/// <summary>
	/// script handles coordination between main game loop and ML parts
	/// this script sets up observer to wait until game loop needs an ML action in CombatCommandSelectionState (known because of PU driver check),
	/// this script then sets teamTurn variable which causes agent to RequestDecision(), agent calls get obs, and makes an action, action gets sent here, 
	/// here queries that action is valid or not, if action is invalid small negative reward and 
	/// rewrite action to wait(direction stays the same, sends a notification which CombatCommandSelectionState observes and uses the ability 
	/// ends episodes for agents and resets turn to no turn through a notification. this isn't synced with other parts of game but should complete faster than game resets the board and calls for an action
	/// </summary>

	[SerializeField] Board board; //script for holding information about the game board and updating positions
	public DuelRLAgent m_Agent_Green; //agent script
	public DuelRLAgent m_Agent_Red; //agent script

	float REWARD_INVALID = -0.01f; //reward for selecting invalid action
	float REWARD_WIN = 1.0f; //reward for reaching goal
	float REWARD_LOSS = -1.0f; //reward for reaching goal
	float REWARD_DRAW = 0.0f; //reward for drawing game
	int NOT_SET = NameAll.NULL_INT; //

	int game_state; //what part of the player turn it is (can move/act/wait, can move only, can act only, can wait only
	int GAME_STATE_ALL = 0; //unit can move and act
	int GAME_STATE_MOVE = 1; //unit can only move or wait
	int GAME_STATE_ACT = 2; //unit can only act or wait
	int GAME_STATE_WAIT = 3; //unit can only wait
	int TEAM_ID_GREEN = 2; //green team as id 2 in this game
	int TEAM_ID_RED = 3; //red team as id 2 in this game
	int TEAM_ID_OFFSET = 2; //agents have team id 0 (green team) and 1 (red team) in the agent scripts

	//which team currently has the turn, lets agent know when its time to request a decision
	public int teamTurn;
	int TEAM_TURN_NONE = -1919;

	// notifications
	const string RLSendAction = "ReinforcementLearning.SendAction"; // send notification along with actions
	const string RLRequestAction = "ReinforcementLearning.RequestAction";  // recieve notification that action is needed from agent
	const string RLEndEpisode = "ReinforcementLearning.EndEpisode"; //end episode
	const string RLResetGame = "ReinforcementLearning.ResetGame"; //reset game, sent to GameLoopState

	void Start()
    {
		game_state = NOT_SET;
		teamTurn = TEAM_TURN_NONE;
	}

	// take action from agent, try to move it through the player manager
	public void TakeAction(float[] actionArray, int teamId )
	{
		//3 branches of actions
		//index 0: wait 0, move 1, attack 2, primary 3
		//index 1: which primary abilities (8 total)
		//index 2: target tile (0 is 0,0; 1 is 0,1; 2 is 1,0; 3 is 1,1) or wait direction (the direction enums)

		teamTurn = TEAM_TURN_NONE; //agent inputted its turn, keeps agent from acting again until its next turn
		//Debug.Log("testing moving to tile, top of take action " + actionArray[0] + actionArray[1] + actionArray[2]);
		//query action is valid or not
		//Debug.Log("in DuelRL area, taking action, teamId is " + teamId);
		bool isPlanValid = PlayerManager.Instance.IsRLPlanValid(actionArray, teamId + TEAM_ID_OFFSET, board, game_state);
		//if invalid ovewrite it and do small negative reward
		if (!isPlanValid)
		{
			if(teamId == TEAM_ID_GREEN)
				m_Agent_Green.SetReward(REWARD_INVALID);
			else
				m_Agent_Red.SetReward(REWARD_INVALID);
			//overwrite action to be wait
			actionArray[0] = 0.0f;
			//Debug.Log("testing reward for invalid action " + teamId);
		}
		//Debug.Log("testing moving to tile, after isPlan Valid " + actionArray[0] + actionArray[1] + actionArray[2]);
		//turn action into plan (game logic will convert plan into necessary commands
		CombatPlanOfAttack rlPlan = new CombatPlanOfAttack(actionArray, board);
		//send plan through notification, CombatCommandSelectionState is waiting for this
		this.PostNotification(RLSendAction, rlPlan);
		//Debug.Log("testing reward was " + REWARD_MOVE);
	}


	public float[] GetObservations()
	{
		var obsArray = PlayerManager.Instance.GetDuelObservation();
		
		obsArray[0] = game_state / 10f;
		//Debug.Log("testing observations");
		//for( int i = 0; i < obsArray.Length; i++)
		//{
		//	print("testing observations " + obsArray[i]);
		//}
		return obsArray;
	}

	#region notifications
	//set up observers
	void OnEnable()
	{
		this.AddObserver(OnRLRequestAction, RLRequestAction);
		this.AddObserver(OnRLEndEpisode, RLEndEpisode);
	}

	void OnDisable()
	{
		this.RemoveObserver(OnRLRequestAction, RLRequestAction);
		this.RemoveObserver(OnRLEndEpisode, RLEndEpisode);
	}

	//notification sent from CombatCommandSelectionState when an action is needed triggers ML part
	void OnRLRequestAction(object sender, object args)
	{
		//Debug.Log("in DuelRL area, recieving notification");
		//send over the entire
		CombatTurn turn = (CombatTurn)args;
		int teamId = turn.actor.TeamId;
		teamTurn = teamId - TEAM_ID_OFFSET;

		if(turn.hasUnitActed)
		{
			if (turn.hasUnitMoved)
				game_state = GAME_STATE_WAIT;
			else
				game_state = GAME_STATE_MOVE;
		}
		else
		{
			if (turn.hasUnitMoved)
				game_state = GAME_STATE_ACT;
			else
				game_state = GAME_STATE_ALL;
		}
	}

	// on end of episode. Give out reward, end episode, wait until next turn is set to start next episode. Unit/board reset happens elsewhere
	void OnRLEndEpisode(object sender, object args)
	{
		//Debug.Log("batte is over, resetting episode ");
		Teams victor = (Teams)args;
		int teamWinner;
		if (victor == Teams.Team1)
			teamWinner = TEAM_ID_GREEN;
		else
			teamWinner = TEAM_ID_RED;

		if (teamWinner == TEAM_ID_GREEN)
		{
			m_Agent_Green.SetReward(REWARD_WIN); //Debug.Log("resetting episode " + REWARD_WIN);
			m_Agent_Red.SetReward(REWARD_LOSS); //Debug.Log("resetting episode " + REWARD_LOSS);
		}
		else if (teamWinner == TEAM_ID_RED)
		{
			m_Agent_Green.SetReward(REWARD_LOSS); //Debug.Log("resetting episode " + REWARD_LOSS);
			m_Agent_Red.SetReward(REWARD_WIN); //Debug.Log("resetting episode " + REWARD_WIN);
		}
		else
		{
			m_Agent_Green.SetReward(REWARD_DRAW); //Debug.Log("resetting episode " + REWARD_DRAW);
			m_Agent_Red.SetReward(REWARD_DRAW); //Debug.Log("resetting episode " + REWARD_DRAW);
		}
		m_Agent_Red.EndEpisode();
		m_Agent_Green.EndEpisode();
		teamTurn = TEAM_TURN_NONE;
		//Debug.Log("batte is over 4");
		this.PostNotification(RLResetGame);
	}
	#endregion
}
