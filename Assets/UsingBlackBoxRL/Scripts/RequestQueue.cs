using System;
using System.Collections;
using System.Collections.Generic;

namespace AurelianTactics.BlackBoxRL
{
	/// <summary>
	/// Handles information from the communication layer
	/// still unknown how to implement it until I get a better sense of what info is going in and how this information is being served to WorldTimeManager
	/// </summary>

	//for now jsut a simple list that has actions. can add to end or pop from beginning

	//to do
	//once I know more from teh communications layer, expand RQO
	//probably need to implement some sort of locking on this
	//not sure whether i want to push from here to WorldTimeManager or pull from WorldTimeManager
	//do I want to do some sort of check so that dm_env_rpc (or whatever teh outer thing) is synced with this?

	public class RequestQueue
	{

		LinkedList<RequestQueueObject> rqoLinkedList; //for now just actions. will flesh it
		LinkedList<int> tempList;
		public RequestQueue()
		{
			this.rqoLinkedList = new LinkedList<RequestQueueObject>();
		}

		//then just 

		public RequestQueueObject GetNextRequest()
		{
			if (this.rqoLinkedList.Count > 0)
			{
				var rqo = this.rqoLinkedList.First.Value;
				this.rqoLinkedList.RemoveFirst();
				return rqo;
			}
			return null;	
			
		}

	}

	
	public class RequestQueueObject
	{
		public bool isNewEnv;
		public bool isNewEpisode;
		public int actionInt;

		public RequestQueueObject(int action, bool newEnv, bool newEpisode)
		{
			this.actionInt = action;
			this.isNewEnv = newEnv;
			this.isNewEpisode = newEpisode;
		}


	}

}

