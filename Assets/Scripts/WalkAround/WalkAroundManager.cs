//DEPRECATED

//catch all for various functions in WalkAround mode
//any WalkAround only class should be called through this manager rather than calls to the class itself
//usage:
//in WalkAroundInitState WalkAroundManager BuildLevel is called which builds a level through WalkAroundMapGenerator which calls LevelData

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class WalkAroundManager {

//    public LevelData BuildLevel(string level_string)
//    {
//        WalkAroundMapGenerator wamg = new WalkAroundMapGenerator();
//        var ld = wamg.BuildLevel(level_string);
        
//        return ld;
//        //if (ld == null)
//        //{
//        //    Debug.Log("couldn't find level to load");
//        //    //SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
//        //    return;
//        //}
//    }

//}
