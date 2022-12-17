using System;
using System.Collections.Generic;

namespace AurelianTactics.BlackBoxRL
{
	/// <summary>
	/// Receives information from AgentSession to Actuators (sort of like actions)
	/// Sends information to AgentSession from Sensors (sort of like observations)
	/// DeepMind paper way is to attach an an Avatar to a game object and then have Acuators and Sensors be C# components
	/// Example for now is just a generic 
	/// </summary>
	

	//to do:
	//how to format the actions? Needs to be done on creation? 
		//I'm thinking define number of actions, min and max, continuous or discrete for each available action
			//can flow from some config set up for the env
			//return gym and dm_env convenient functions
		//brainstorming: discrete vs. continuous. array min/maxed vs selecting one. action size for multiple actions
		//action size static or increasing
		//which data type to store it in: list, dictionary, array, 
		//can probably format it based on some dm_env or gym prototype
	//how to handle invalid actions (maybe a default do nothing command that can be set upon creation)
	//how to describe the observation
		//where am I storing the global observation (maybe global avatar)
		//where am I storing and defining the local observation
	//do i need some sort of confirmation that the action is done so that things can progress?
	//have to set up the actual spec file for things to be loaded
	//need something that converst C# style action spec to the DM version
		//either grpc or json, see how the actual connect work


	public class Avatar 
	{
		ActionSpec actionSpec;
		ObservationSpec observationSpec;
		Dictionary<string, string> observation;
		NextActionState nextActionState;
		public List<int> actionList;

		//in paper, described as being on the game object so can send actions and what not through that attachment
		//here faking it and have a combat controller attached then sending actions through that
		//I need to attach the avatar to the scene and put on the combatController

		public Avatar(string config)
		{
			//to do: code to load the spec
			//to do: code to turn spec into various variables
			this.actionSpec = new ActionSpec();
			this.observationSpec = new ObservationSpec();
			this.nextActionState = NextActionState.Waiting;
			this.actionList = new List<int>(new int[4]);
			
		}

		//I'm imagining something like the AgentSession sends in an action
		//not
		public void TakeAction(List<int> actionList)
		{
			this.actionList = actionList;
			this.nextActionState = NextActionState.Ready;
		}

		//I'm imagining something like the AgentSession wants to know available actions
		//to do: add mask
		public List<int> GetAvailableActions()
		{
			return new List<int>(new int[this.actionSpec.numValues]);
		}

		void SendSensorInformation()
		{

		}

		//imagining something like sending over the number of actions and the observation size
		void SendActionObservationSpecs()
		{

		}

		public ObservationSpec GetObservationSpec()
		{
			return this.observationSpec;
		}

		public ActionSpec GetActionSpec()
		{
			return this.actionSpec;
		}

		public Dictionary<string, string> GetObservation()
		{
			return this.observation;
		}

		public NextActionState GetNextActionState()
		{
			return this.nextActionState;
		}

		public void SetNextActionState(NextActionState nas)
		{
			this.nextActionState = nas;
		}
	}

	/// <summary>
	/// Idea is to do something similar to gym/dm_env envs where you send back and observation broad details
	/// can then use these details to set up agent value function and what not
	/// however on many complicated envs I have seen the obs are large and jagged
	/// and part of the challenge is turning this huge amount of obs into something workable
	/// I'm leaning towards just sending back a dictionary, and the agent side can decide how to interpret
	/// maybe build in a default way as well but idk
	/// </summary>
	public class ObservationSpec
	{
		//dm_env busuite catch 0 example
		//print(env.observation_spec())
		//BoundedArray(shape= (10, 5), dtype= dtype('float32'), name= 'board', minimum= 0.0, maximum= 1.0)

		string dtype; //int, float, double
		List<int> obsShape; // depending on how I do this, I might need a list of lists with tons of varying size
		float min;
		float max;

		//not sure if i want static obs or some sort of dictionary return thing
		//kind of leaning towards a dictionary and then some code on the other side to treat it
		//idk maybe both?
		//double[,] ServicePoint = new double[10,9];//<-ok (2)
		public ObservationSpec()
		{

		}
	}

	public class ActionSpec
	{
		//dm_env bsuite catch 0 example
		//print(env.action_spec())
		//DiscreteArray(shape= (), dtype= int64, name= action, minimum= 0, maximum= 2, num_values= 3)
		//just doing a default one for now

		string dtype; //int, float, double
		bool isDiscrete; //continuous or discrete, not sure if needed
		List<int> actionShape; //list can be dynamic but arrays are set in C# as far as I know. can return an this as an array
		List<Tuple<int,int>> actionMinMax; //min and max for each part of the action shape
		public int numValues; // total number of actions to enter. basically a prod of actionShape

		//initial default
		//index 0:
			//continue action from last turn
			//wait
			//move
			//attack
			//primary
			//secondary

		//index 1: 
			//ability to select
			//if wait: 0 to 3 for N to W going clockwise

		//index 2:
			//x coordinate relative to unit
			//	ie can be negative

		//index 3:
			//y coordinate relative to unit

		//if invalid then wait or do negative reward

		public ActionSpec()
		{
			this.dtype = "int";
			this.isDiscrete = true;
			this.numValues = 4;
			this.actionShape = new List<int>(new int[this.numValues]);
			this.actionMinMax = new List<Tuple<int, int>>();
			this.actionMinMax.Add(new Tuple<int, int>(0, 5)); //type of action
			this.actionMinMax.Add(new Tuple<int, int>(0, 100)); //index of action
			this.actionMinMax.Add(new Tuple<int, int>(-100, 100)); //relative x coord
			this.actionMinMax.Add(new Tuple<int, int>(-100, 100)); //relative y coord
			

		}

		/*
		 add function to send to agentsesson then to communication layer
		 */
	}

	//enums to hold status of action 
	public enum NextActionState
	{
		Waiting, //awaiting next action input from Avatar
		Ready, //action has been inputted, env can now read it and use it
		InProgress, //env is inputing the action and updating the env 
		Finished //action has been finished, can clear the action and set to waiting
	}

}
