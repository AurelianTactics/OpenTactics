using System;
using System.Collections;
using System.Collections.Generic;

namespace AurelianTactics.BlackBoxRL
{
	/// <summary>
	/// Created by WorldTimeManager
	/// Sends information to Communication layer
	/// Not sure how and what to balance between WorldTimeManager and this
	/// </summary>

	//to do:
	//i'm thinking i instantiate/look for the avatars and tasks and attach them to agentsession in an iterable object
	//then can do shit on them and be able to do stuff on them
	//i'm assuming possible multiples of avatars but only one task per scene.
	//task is supposed to specify the avatar(s) (maybe plural, not sure on this) the agent controls
	//a single scalar reward
	//i think i want action masking here
	// make obs, action, reward dynamic
	//some sort of sanity check to confirm action actually went through?
	//reward penalty for choosing non available action

		// from deepmind env (bsuite env catch example)
	//print(env.action_spec())
	//DiscreteArray(shape= (), dtype= int64, name= action, minimum= 0, maximum= 2, num_values= 3)
	//print(env.observation_spec())
	//BoundedArray(shape= (10, 5), dtype= dtype('float32'), name= 'board', minimum= 0.0, maximum= 1.0)


	public class AgentSession
	{
		private Task task;
		public Avatar avatar;

		private List<int> availableActions; //change based on shape
		private int defaultAction;

		private int action; //change based on action shape
		private Dictionary<string, string> observation; //change based on obs shape
		private float reward; 
		private bool done;

		private Dictionary<string, string> priorObs; //change based on obs shape
		private float priorReward;
		private int priorAction; //change based on action shape

		public AgentSession(string config = "default")
		{
			this.task = new Task(config);
			this.avatar = new Avatar(config);
		}

		

		void SendInfoToCommunicationLayer()
		{
			//to do actually hook up and send
			Console.WriteLine(
				"statement Obs :{0}\t reward B :{1}\t action C :{2}\t done D :{3}",
				this.observation, this.reward, this.action, this.done);
		}

		// get all available actions agent can take
		ActionSpec GetActionSpec()
		{
			return this.avatar.GetActionSpec();
		}

		// get observation spec
		ObservationSpec GetObservationSpec()
		{
			return this.avatar.GetObservationSpec();
		}


		public void GetAvailableActions()
		{
			this.availableActions = this.avatar.GetAvailableActions();
		}
		// take an action
		//get available actions
		//select the one setn from the manager
		//take it in the avatar
		//get result
		public void TakeAction(int wmAction)
		{
			if(this.availableActions.Contains(action))
				this.action = this.availableActions[wmAction];
			else
			{
				Console.WriteLine("action not found, using default action ");
				this.action = this.defaultAction;
			}

			UpdatePrior();

			// get obs and info back from avatar
			//var ObsInfoList = this.avatar.TakeAction(action);
			// get reward and done back from task
			//var RewardDoneList = this.task.TakeAction(action);
			//send this info to communication layer
			this.done = this.task.GetDone();
			this.reward = this.task.GetReward();
			this.observation = this.avatar.GetObservation();
		}

		// update prior information
		void UpdatePrior()
		{
			this.priorObs = new Dictionary<string, string>(this.observation);
			this.priorReward = this.reward;
			this.priorAction = this.action;
		}

	}
}
