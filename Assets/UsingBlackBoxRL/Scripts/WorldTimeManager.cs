using System;

namespace AurelianTactics.BlackBoxRL
{
	/// <summary>
	/// Manages the RequestQueue with process requests
	/// Sets up an internal agent session
	/// </summary>

	//to do 
	//do I want to spin this and check next requests or have some sort of push from process requests?
		//i'm leaning towards spinning here
	//(not sure if here or AgentSession, kind of leaning towards here:
		//create game settings
		//join game settings (stub)
	//I can take an action here into the agentsession but returning obs/reward/etc from a different channel
	//actual arguments

	/* Misc notes
	process requests						
	on create game settings					
		stub for choosing different maps (maybe just alternate between two)				
			down the line: generate a map from a config string			
		units and full unit info				
		start game from scratch or not				
		if not from scratch then need info on these things				
			loop phase, game phase, status lasting, status temporary, slow action			
		fucking hell, how should this be passed? i guess need to know how grpc does protobuffs?				
	on join game settings (stub for now, assuming join on create)					
i need some sort of trigger that process the actions, then returns saying like (need an action)						
also some sort of return saying here's the result						
me: how do i make sure things are matched up and not too much spam on one side or the other? that things are synced						
	 */

	public class WorldTimeManager
	{
		private RequestQueue requestQueue;
		public AgentSession agentSession;

		public WorldTimeManager(string config = "default")
		{
			this.requestQueue = new RequestQueue();
			this.agentSession = new AgentSession(config);
		}

		
		public void ProcessRequests()
		{
			var rqo = this.requestQueue.GetNextRequest();
			if( rqo != null)
			{
				//logic for handling new env
				//logic for handling env reset
				//logic for sending action to agent
				if(rqo.isNewEnv)
				{

				}
				else if (rqo.isNewEpisode)
				{

				}
				else
				{
					this.agentSession.TakeAction(rqo.actionInt);
				}
				
			}
		}
	}
}
