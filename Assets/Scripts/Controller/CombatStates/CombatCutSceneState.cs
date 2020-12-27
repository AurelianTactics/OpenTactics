using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatCutSceneState : CombatState
{
    ConversationController conversationController;
    ConversationData data;
    bool isOfflineGame;
    bool isStatsShown;

    protected override void Awake()
    {
        base.Awake();
        conversationController = owner.GetComponentInChildren<ConversationController>();
    }

    public override void Enter()
    {
        base.Enter(); 
        isOfflineGame = PlayerManager.Instance.IsOfflineGame(); //Debug.Log("entering cut scene state " + isOfflineGame + " " );
        isStatsShown = false;
        if (IsBattleOver())
        {
            if (!isOfflineGame && PlayerManager.Instance.isMPMasterClient())
                PlayerManager.Instance.SendMPGameOver();

            data = GetConversation(NameAll.DIALOGUE_PHASE_END);
            ConversationData cd = GetVictoryMessage();
            if( data != null)
            {
                data.list.Insert(0, cd.list[0]);
            }
            else
            {
                data = cd;
                //Resources.UnloadAsset(cd);
            }
        }
        else
        {
            //online game just gets the victory conditions, can change that in future
            if ( isOfflineGame)
                data = GetConversation(NameAll.DIALOGUE_PHASE_START);
            
            data = GetVictoryConditions(data);
        }

        if( data != null)
        {
            conversationController.Show(data); //Debug.Log("showing conversation data");
        }
           

        if (!isOfflineGame && !IsBattleOver()) //beginning of online games trigger this way
            StartCoroutine(BeginGameLoopState());
        //rest of games (and end game for offline and online games. still have the OnCompleteConversation trigger
    }

    

    public override void Exit()
    {
        //Debug.Log("wtf is calling exit?");
        base.Exit();
        //if (data)
        //    Resources.UnloadAsset(data); //unloading from elsewhere now
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        if( driver == Drivers.Computer)
        {
            InputController.fireEvent += OnFire; //this isn't added in the base implementation but needed here on defeat
        }
        ConversationController.completeEvent += OnCompleteConversation;
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        ConversationController.completeEvent -= OnCompleteConversation;
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
        base.OnFire(sender, e);
        conversationController.Next();
        if(isStatsShown)
        {
            isStatsShown = false;
            owner.ChangeState<CombatEndState>();
        }
    }

    void OnCompleteConversation(object sender, System.EventArgs e)
    {
        //in single player story mode, enable these two lines to test entering battle, watching initial converstion then leaving battle
        //StartCoroutine(TestGameEnd());
        //return;

        //Debug.Log("changing state?" + IsBattleOver());
        if (IsBattleOver())
        {
            int z1 = PlayerPrefs.GetInt(NameAll.PP_COMBAT_ENTRY, NameAll.SCENE_MAIN_MENU);
            if (z1 == NameAll.SCENE_STORY_MODE && owner.GetComponent<CombatVictoryCondition>().Victor == Teams.Team1)
            {
                isStatsShown = true;
                PlayerManager.Instance.ShowCombatStats();
            }
            else
                owner.ChangeState<CombatEndState>();
        }
        else
        {
            if( isOfflineGame)
                owner.ChangeState<GameLoopState>();
            //online game not complete conversation trigger, coroutine goes to GameLoopState
        }                        
    }

    ConversationData GetVictoryConditions(ConversationData conData)
    {
        ConversationData cd;
        CombatVictoryCondition vc = owner.gameObject.GetComponent<CombatVictoryCondition>(); //Debug.Log("vc victory type is " + vc.VictoryType);
        if (vc.VictoryType == NameAll.VICTORY_TYPE_DEFEAT_PARTY)
            cd = Resources.Load<ConversationData>("Conversations/ConditionDefeatParty");
        else
            cd = Resources.Load<ConversationData>("Conversations/ConditionNone");

        if (conData == null)
            return cd;

        foreach( SpeakerData sd in cd.list)
        {
            conData.list.Add(sd);
        }

        Resources.UnloadAsset(cd);

        return conData;
    }

    ConversationData GetConversation(int dialoguePhase)
    {
        string levelLoadType = PlayerPrefs.GetString(NameAll.PP_LEVEL_DIRECTORY, NameAll.LEVEL_DIRECTORY_AURELIAN); 
        
        if(levelLoadType == NameAll.LEVEL_DIRECTORY_CAMPAIGN_AURELIAN || levelLoadType == NameAll.LEVEL_DIRECTORY_CAMPAIGN_CUSTOM)
        {
            int levelKey = PlayerPrefs.GetInt(NameAll.PP_COMBAT_LEVEL, 0); //Debug.Log("dialogue Phase is " + dialoguePhase);
            if (levelKey >= 1000)
                levelKey -= 1000;

            CampaignDialogue cd = CalcCode.LoadCampaignDialogue(PlayerPrefs.GetInt(NameAll.PP_COMBAT_CAMPAIGN_LOAD, 0));
            if( cd != null)
            {
                return cd.GetConversationData(levelKey, dialoguePhase);
            }
        }
        
        return null;
    }
    
    ConversationData GetVictoryMessage()
    {
        //Debug.Log("battle is over, displaying end game message");
        if (owner.GetComponent<CombatVictoryCondition>().Victor == Teams.Team2)
        {
            if( isOfflineGame)
                owner.battleMessageController.Display("Defeat!");
            else
                owner.battleMessageController.Display("Team 2 Wins!");
        } 
        else
        {
            if(isOfflineGame)
                owner.battleMessageController.Display("Victory!");
            else
                owner.battleMessageController.Display("Team 1 Wins!");
        }
            
        //Debug.Log("Add message saying which team won");

        ConversationData cd;
        if (owner.GetComponent<CombatVictoryCondition>().Victor != Teams.Team2)
            cd = Resources.Load<ConversationData>("Conversations/Team1Victory");
        else
            cd = Resources.Load<ConversationData>("Conversations/Team2Victory");

        return cd;
    }

    //for online games, go to GameLoopState after a set time, not based on completing the conversation
    IEnumerator BeginGameLoopState()
    {
        yield return new WaitForSeconds(2.0f);
        conversationController.Hide();
        //Debug.Log("about to enter gameLoopState");
        owner.ChangeState<GameLoopState>();
    }

    //IEnumerator TestGameEnd()
    //{
    //    Debug.Log("TESTING ending game after initial conversation");
    //    yield return new WaitForSeconds(0.5f);
    //    conversationController.Hide();
    //    owner.GetComponent<CombatVictoryCondition>().Victor = Teams.Team1;
    //    owner.ChangeState<CombatEndState>();
    //}
}
