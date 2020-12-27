using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatEndState : CombatState
{

    public override void Enter()
    {
        base.Enter();
        GameObject go = GameObject.Find("PlayerManagerObject(Clone)");
        Destroy(go);
        if ( !PlayerManager.Instance.IsOfflineGame() )
        {
            //go = GameObject.Find("ChatGameObject");
            //if (go != null)
            //{
            //    go.SetActive(false);
            //    //Debug.Log("why the fuck can't I leave the fucking chat channel?");
            //    //go.GetComponent<ChatHandler>().LeaveRoomChannels();
            //}
            //else
            //Debug.Log("ERROR: couldn't find chat game object before leaving scene");
            //PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(NameAll.SCENE_MP_MENU);
        }
        else
        {
            int z1 = PlayerPrefs.GetInt(NameAll.PP_COMBAT_ENTRY, NameAll.SCENE_MAIN_MENU);
            if ( z1 == NameAll.SCENE_STORY_MODE && owner.GetComponent<CombatVictoryCondition>().Victor == Teams.Team1)
            {
                SceneManager.LoadScene(NameAll.SCENE_STORY_MODE);
            }
            else if( z1 == NameAll.SCENE_CAMPAIGNS)
                SceneManager.LoadScene(NameAll.SCENE_CAMPAIGNS);
            else 
                SceneManager.LoadScene(NameAll.SCENE_MAIN_MENU);
        }
        
    }


}
