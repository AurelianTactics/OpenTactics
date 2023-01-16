using UnityEngine;
using UnityEngine.UI;

/*
to do:
detail how this is used. I think it's like this is attached to another script or a game object
    then when the scroll list is created, a bunch of these can be instatiated based on the code
which places is this used in?
 */

/// <summary>
/// Script for scroll list buttons that handles intatiating them and clicking on them
/// this script is used on most buttons for most scrolllists in the project.
/// </summary>
public class AbilityEditScrollListButton : MonoBehaviour
{

	//if want more things on button can add more things

	/// <summary>
	/// The button itself that can be clicked on in the list
	/// </summary>
	public Button button;

	/// <summary>
	/// Text title on the button
	/// </summary>
	public Text title;

	/// <summary>
	/// Text box details on the button
	/// </summary>
	public Text details;

}
