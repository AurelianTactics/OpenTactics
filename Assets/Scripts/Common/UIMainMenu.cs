using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Used in MainMenu Scene
/// Handles various OnClick button functions
/// Launches game modes and builders
/// </summary>
/// <remarks>
/// Some scene object might be missing scripts due to downgrading from paid menu UI to free UI
/// </remarks>
public class UIMainMenu : MonoBehaviour {

    public Button RLButton;

    public Button playCustomGameButton;
    public Button playCampaignButton;
    public Button playOnlineButton;
    public Button playStoryButton;
    public Button playOpenWorldButton;

    public Button createAbilityButton;
    public Button createCharacterButton;
    public Button createMapButton;
    public Button createClassButton;
    public Button createStoryButton;
    public Button createCampaignButton;
    public Button createItemButton;

    //   // Use this for initialization
    //   void Start () {

    //}

    public void OnMPButtonClick() //goes to mp menu, then loads a custom game with the "mp" designation in player preferences
    {
        PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
        PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_TYPE, NameAll.CUSTOM_GAME_ONLINE);
        //PlayerPrefs.SetInt(NameAll.PP_COMBAT_ENTRY, NameAll.SCENE_CUSTOM_GAME); //redundant, done in MPGameController
        SceneManager.LoadScene(NameAll.SCENE_MP_MENU);
    }

    public void OnEditUnitButtonClick()
    {
        PlayerPrefs.SetInt(NameAll.PP_EDIT_UNIT_ENTRY, NameAll.SCENE_MAIN_MENU);
        SceneManager.LoadScene(NameAll.SCENE_EDIT_UNIT);
    }

    public void OnCustomGameButtonClick() //goes straight to custom game with custom game designation
    {
        PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
        PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_TYPE, NameAll.CUSTOM_GAME_OFFLINE);
		PlayerPrefs.SetInt(NameAll.PP_INIT_STATE, NameAll.INIT_STATE_COMBAT);
		SceneManager.LoadScene(NameAll.SCENE_CUSTOM_GAME);
    }

    public void OnLevelBuilderClick()
    {
        PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
        SceneManager.LoadScene(NameAll.SCENE_LEVEL_BUILDER);
    }

    public void OnAbilityBuilderClick()
    {
        //PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
        SceneManager.LoadScene(NameAll.SCENE_ABILITY_BUILDER);
    }

    public void OnCampaignBuilderClick()
    {
        //PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
        SceneManager.LoadScene(NameAll.SCENE_CAMPAIGN_BUILDER);
    }

    public void OnClassBuilderClick()
    {
        //PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
        SceneManager.LoadScene(NameAll.SCENE_CLASS_BUILDER);
    }

    public void OnCampaignsClick()
    {
        //PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
        SceneManager.LoadScene(NameAll.SCENE_CAMPAIGNS);
    }

    public void OnOpenWorldClick()
    {
        //PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
        PlayerPrefs.SetInt(NameAll.PP_INIT_STATE, NameAll.INIT_STATE_WALK_AROUND);
        SceneManager.LoadScene(NameAll.SCENE_WALK_AROUND);
    }

    public void OnStoryModeClick()
    {
        PlayerPrefs.SetInt(NameAll.PP_STORY_MODE_ENTRY, NameAll.SCENE_MAIN_MENU);
        SceneManager.LoadScene(NameAll.SCENE_STORY_MODE);
    }

    public void OnStoryBuilderClick()
    {
        //PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
        SceneManager.LoadScene(NameAll.SCENE_STORY_BUILDER);
    }

    public void OnQuitClick()
    {
        //QUIT WORKS ONLY IN BUILT VERSIONS OF THE GAME Debug.Log("Asdf ");
        Application.Quit();
    }

    public void OnClickRLButtons()
    {
        bool zBool = !RLButton.gameObject.activeSelf;
    }

    public void OnClickOpenPlayButtons()
    {
        bool zBool = !playCustomGameButton.gameObject.activeSelf; //Debug.Log("asdf " + zBool + " " + playCustomGameButton.gameObject.activeSelf);
        playCustomGameButton.gameObject.SetActive(zBool);
        playCampaignButton.gameObject.SetActive(zBool);
        playStoryButton.gameObject.SetActive(zBool);
        //playOnlineButton.gameObject.SetActive(zBool);
        playOpenWorldButton.gameObject.SetActive(zBool);
    }

    public void OnClickOpenCreateButtons()
    {
        bool zBool = !createCampaignButton.gameObject.activeSelf;
        createCampaignButton.gameObject.SetActive(zBool);
        createAbilityButton.gameObject.SetActive(zBool);
        createCharacterButton.gameObject.SetActive(zBool);
        createClassButton.gameObject.SetActive(zBool);
        createItemButton.gameObject.SetActive(zBool);
        createMapButton.gameObject.SetActive(zBool);
        createStoryButton.gameObject.SetActive(zBool);
    }

    public void OnClickItemBuilderClick()
    {
        //PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
        SceneManager.LoadScene(NameAll.SCENE_ITEM_BUILDER);
    }
}
