using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//controls the cutscene
public class CutSceneController : MonoBehaviour {

    public ConversationController conversationController;
    public Text titleText;
    public Text mainText;

    int storyId;
    StoryCutSceneObject csObject;
    ConversationData data;
    bool isTitleShown;

    //need a dialogue controller

    // Use this for initialization
    void Start () {
        this.AddListeners();
        storyId = PlayerPrefs.GetInt(NameAll.PP_CUT_SCENE_STORY_ID, NameAll.NULL_INT);
        int cutSceneId = PlayerPrefs.GetInt(NameAll.PP_CUT_SCENE_CUT_SCENE_ID, NameAll.NULL_INT);
        //PlayerPrefs.SetInt(NameAll.PP_STORY_MODE_ENTRY, NameAll.SCENE_CUT_SCENE); //set it when leaving storymode, not needed here
        SetCutSceneObject(storyId, cutSceneId); //sets the csObject and the ConservationData (if any)
        
	}
	
	void SetCutSceneObject(int soId, int cutSceneId)
    {
        isTitleShown = false;
        data = null;

        StoryObject so = CalcCode.LoadStoryObject(soId);
        StoryCutScene scs = CalcCode.LoadStoryCutScene(soId);

        if( so != null && scs != null)
        {
            csObject = scs.GetCutSceneObject(cutSceneId);
            if( csObject != null && csObject.DialogueLevel != NameAll.NULL_INT && so.CampaignId != NameAll.NULL_INT)
            {
                CampaignDialogue cd = CalcCode.LoadCampaignDialogue(so.CampaignId);
                if (cd != null)
                {
                    data = cd.GetConversationData(csObject.DialogueLevel, NameAll.DIALOGUE_PHASE_START);
                }
            }
        }

        DisplayCutSceneTitle();
    }

    void DisplayCutSceneTitle()
    {
        if( csObject != null)
        {
            titleText.text = csObject.BackgroundTitle;
            mainText.text = csObject.BackgroundText;
        }
    }

    void AddListeners()
    {
        InputController.fireEvent += OnFire;
        ConversationController.completeEvent += OnCompleteConversation;
    }

    void RemoveListeners()
    {
        InputController.fireEvent -= OnFire;
        ConversationController.completeEvent -= OnCompleteConversation;
    }

    void OnFire(object sender, InfoEventArgs<int> e)
    {
        //Debug.Log("fire event in cut scene controller ");
        if (!isTitleShown)
        {
            isTitleShown = true;
            if( data != null)
                conversationController.Show(data);
            return;
        }
       
        if( data != null)
        {
            //start showing the conversation only after the title has been shown
            conversationController.Next();
        }   
        else
        {
            ExitScene();
        }
        
    }

    void OnCompleteConversation(object sender, System.EventArgs e)
    {
        if (csObject.NextCutSceneId == NameAll.NULL_INT)
            ExitScene();
        else
        {
            SetCutSceneObject(storyId, csObject.NextCutSceneId);
        }
    }

    void ExitScene()
    {
        this.RemoveListeners();
        SceneManager.LoadScene(NameAll.SCENE_STORY_MODE);
    }
}
