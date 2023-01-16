using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Idk where it is used and why I have so many different UI buttons
/// </summary>
public class UIGenericButton : MonoBehaviour {

    public Text textField;

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SetText(string zString)
    {
        textField.text = zString;
    }
}
