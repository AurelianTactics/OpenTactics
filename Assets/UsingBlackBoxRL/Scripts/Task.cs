using System;
using System.Collections;
using System.Collections.Generic;

namespace AurelianTactics.BlackBoxRL
{
	/// <summary>
	/// Sends and receives information to and from AgentSession
	/// </summary>
	
	//To do:
	//i think design docs say each agentsession has its own task
	//design doc helps grab available avatar or create a new avatar for the agent. for now just having both in AgentSession
	//tasks define and return the reward. i'm not sure how the reward manipulation logic should be done. In the task or in the scene?
	//define done
	//start a new episode
	//LATER control time: many examples of this like speeding up/slowing down perception; starting new episode by letting x number of seconds go etc
	//maybe add a reward clip
	//add functions for getting the reward
		//probably abstract this and then inherit and set up reward functions
	//add function for telling done from the state



	public class Task
	{
		private float reward;
		private bool done;

		public Task(string config)
		{
			this.reward = 0.0f;
			this.done = false;
		}


		public float GetReward()
		{
			return this.reward;
		}

		public bool GetDone()
		{
			return this.done;
		}

		public void SetDone(bool d)
		{
			this.done = d;
		}

		public void SetReward(float r)
		{
			this.reward = r;
		}

	}

	

	
	
}
