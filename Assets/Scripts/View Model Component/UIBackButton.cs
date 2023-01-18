using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// button to indicate cancel/back
/// </summary>
public class UIBackButton : MonoBehaviour
{

	public void Open()
	{
		gameObject.SetActive(true);
	}

	public void Close()
	{
		gameObject.SetActive(false);
	}
}
