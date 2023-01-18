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
/// To do: unify the menu code to abstract and not rely on scene specific code
/// </remarks>
public class UIMainMenu : MonoBehaviour
{

	// various buttons on the main menu that can be clicked
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

	/// <summary>
	/// Handles click on corresponding button.
	/// In general, changes a value in a shared preferences file and launches a new scene
	/// </summary>
	public void OnMPButtonClick() //goes to mp menu, then loads a custom game with the "mp" designation in player preferences
	{
		PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
		PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_TYPE, NameAll.CUSTOM_GAME_ONLINE);
		//PlayerPrefs.SetInt(NameAll.PP_COMBAT_ENTRY, NameAll.SCENE_CUSTOM_GAME); //redundant, done in MPGameController
		SceneManager.LoadScene(NameAll.SCENE_MP_MENU);
	}

	/// <summary>
	/// Handles click on corresponding button.
	/// In general, changes a value in a shared preferences file and launches a new scene
	/// </summary>
	public void OnEditUnitButtonClick()
	{
		PlayerPrefs.SetInt(NameAll.PP_EDIT_UNIT_ENTRY, NameAll.SCENE_MAIN_MENU);
		SceneManager.LoadScene(NameAll.SCENE_EDIT_UNIT);
	}

	/// <summary>
	/// Handles click on corresponding button.
	/// In general, changes a value in a shared preferences file and launches a new scene
	/// </summary>
	public void OnCustomGameButtonClick() //goes straight to custom game with custom game designation
	{
		PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
		PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_TYPE, NameAll.CUSTOM_GAME_OFFLINE);
		PlayerPrefs.SetInt(NameAll.PP_INIT_STATE, NameAll.INIT_STATE_COMBAT);
		SceneManager.LoadScene(NameAll.SCENE_CUSTOM_GAME);
	}

	/// <summary>
	/// Handles click on corresponding button.
	/// In general, changes a value in a shared preferences file and launches a new scene
	/// </summary>
	public void OnLevelBuilderClick()
	{
		PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
		SceneManager.LoadScene(NameAll.SCENE_LEVEL_BUILDER);
	}

	/// <summary>
	/// Handles click on corresponding button.
	/// In general, changes a value in a shared preferences file and launches a new scene
	/// </summary>
	public void OnAbilityBuilderClick()
	{
		//PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
		SceneManager.LoadScene(NameAll.SCENE_ABILITY_BUILDER);
	}

	/// <summary>
	/// Handles click on corresponding button.
	/// In general, changes a value in a shared preferences file and launches a new scene
	/// </summary>
	public void OnCampaignBuilderClick()
	{
		//PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
		SceneManager.LoadScene(NameAll.SCENE_CAMPAIGN_BUILDER);
	}

	/// <summary>
	/// Handles click on corresponding button.
	/// In general, changes a value in a shared preferences file and launches a new scene
	/// </summary>
	public void OnClassBuilderClick()
	{
		//PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
		SceneManager.LoadScene(NameAll.SCENE_CLASS_BUILDER);
	}

	/// <summary>
	/// Handles click on corresponding button.
	/// In general, changes a value in a shared preferences file and launches a new scene
	/// </summary>
	public void OnCampaignsClick()
	{
		//PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
		SceneManager.LoadScene(NameAll.SCENE_CAMPAIGNS);
	}

	/// <summary>
	/// Handles click on corresponding button.
	/// In general, changes a value in a shared preferences file and launches a new scene
	/// </summary>
	public void OnOpenWorldClick()
	{
		//PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
		PlayerPrefs.SetInt(NameAll.PP_INIT_STATE, NameAll.INIT_STATE_WALK_AROUND);
		SceneManager.LoadScene(NameAll.SCENE_WALK_AROUND);
	}

	/// <summary>
	/// Handles click on corresponding button.
	/// In general, changes a value in a shared preferences file and launches a new scene
	/// </summary>
	public void OnStoryModeClick()
	{
		PlayerPrefs.SetInt(NameAll.PP_STORY_MODE_ENTRY, NameAll.SCENE_MAIN_MENU);
		SceneManager.LoadScene(NameAll.SCENE_STORY_MODE);
	}

	/// <summary>
	/// Handles click on corresponding button.
	/// In general, changes a value in a shared preferences file and launches a new scene
	/// </summary>
	public void OnStoryBuilderClick()
	{
		//PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
		SceneManager.LoadScene(NameAll.SCENE_STORY_BUILDER);
	}

	/// <summary>
	/// Quits app
	/// </summary>
	public void OnQuitClick()
	{
		//QUIT WORKS ONLY IN BUILT VERSIONS OF THE GAME Debug.Log("Asdf ");
		Application.Quit();
	}

	/// <summary>
	/// Expands menu list to show "RL" scene buttons
	/// </summary>
	public void OnClickRLButtons()
	{
		bool zBool = !RLButton.gameObject.activeSelf;
	}

	/// <summary>
	/// Expands menu list to show "play" scene buttons
	/// </summary>
	public void OnClickOpenPlayButtons()
	{
		bool zBool = !playCustomGameButton.gameObject.activeSelf; //Debug.Log("asdf " + zBool + " " + playCustomGameButton.gameObject.activeSelf);
		playCustomGameButton.gameObject.SetActive(zBool);
		playCampaignButton.gameObject.SetActive(zBool);
		playStoryButton.gameObject.SetActive(zBool);
		//playOnlineButton.gameObject.SetActive(zBool);
		playOpenWorldButton.gameObject.SetActive(zBool);
	}

	/// <summary>
	/// Handles click on corresponding button.
	/// Expands menu list to show "create" scene buttons
	/// </summary>
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

	/// <summary>
	/// Handles click on corresponding button.
	/// In general, changes a value in a shared preferences file and launches a new scene
	/// </summary>
	public void OnClickItemBuilderClick()
	{
		//PlayerPrefs.SetInt(NameAll.PP_CUSTOM_GAME_ENTRY, NameAll.SCENE_MAIN_MENU);
		SceneManager.LoadScene(NameAll.SCENE_ITEM_BUILDER);
	}
}
