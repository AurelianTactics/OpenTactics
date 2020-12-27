using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIConfirmButton : MonoBehaviour {

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
