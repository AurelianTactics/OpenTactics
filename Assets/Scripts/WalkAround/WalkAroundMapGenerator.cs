//Generates a map based on a string
//called from WalkAroundManager
//in WalkAroundInitState WalkAroundManager BuildLevel is called which builds a level through WalkAroundMapGenerator which calls LevelData

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAroundMapGenerator {

	public LevelData BuildLevel(string level_string, int seed, int timeInt, int map_x, int map_y, bool isFirstMapVisit = false)
	{
		LevelData ld = new LevelData(level_string, seed, timeInt, map_x, map_y, isFirstMapVisit);
		//generates a random level

		return ld;
	}

	//generate new units for the map
		//nothing fancy for now, one player controlled, one hostile and one neuntral per map
	public List<PlayerUnit> GenerateRandomUnits(int teamId, int numUnits){
		var retList = new List<PlayerUnit>();
		PlayerUnit pu;
		for( int i = 0; i < numUnits; i++)
		{
			pu = new PlayerUnit(NameAll.VERSION_AURELIAN, randomize:true);
			pu.TeamId = teamId;
			retList.Add(pu);
		}
		

		//pu = new PlayerUnit(NameAll.VERSION_AURELIAN);
		//pu.TeamId = NameAll.TEAM_ID_WALK_AROUND_RED;
		//retList.Add(pu);

		//pu = new PlayerUnit(NameAll.VERSION_AURELIAN);
		//pu.TeamId = NameAll.TEAM_ID_WALK_AROUND_NEUTRAL;
		//retList.Add(pu);

		return retList;
	}

	//can create code here to load the level
	//public LevelData LoadLevel()
	//{
	//    LevelData ld = new LevelData();

	//    return ld;
	//}
}
