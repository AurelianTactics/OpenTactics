using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
