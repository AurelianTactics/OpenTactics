using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialogController : MonoBehaviour 
{
	[SerializeField] Text titleLabel;
	[SerializeField] Text messageLabel;
	[SerializeField] Transform content;
	[SerializeField] GameObject blocker;
    [SerializeField] GameObject dialogPanel;
    //Tweener tweener;
    Action onConfirm;
	Action onCancel;

	void Start ()
	{
		blocker.SetActive(false);
        
        //content.localScale = Vector3.zero;
    }

    public void Show(string title, string message, Action confirm, Action cancel)
    {
        dialogPanel.SetActive(true);
        //Debug.Log("opening dialogue box " + confirm + " " + cancel);
        titleLabel.text = title;
        messageLabel.text = message;
        onConfirm = confirm;
        onCancel = cancel;
        blocker.SetActive(true);
        StopAnimation();
        //tweener = content.ScaleTo(Vector3.one, 0.5f, EasingEquations.EaseOutBack);
    }

    
    public void Hide ()
	{
		blocker.SetActive(false);
        dialogPanel.SetActive(false);
        onConfirm = onCancel = null;
		StopAnimation();
		//tweener = content.ScaleTo(Vector3.zero, 0.5f, EasingEquations.EaseInBack);
	}

	void StopAnimation ()
	{
		//if (tweener != null && tweener.easingControl != null && tweener.easingControl.IsPlaying)
		//	tweener.easingControl.Stop();
	}

	public void OnConfirmButton ()
	{
		//Debug.Log("hitting confirm button");
		if (onConfirm != null)
			onConfirm();
		Hide ();
	}

	public void OnCancelButton ()
	{
		//Debug.Log("hitting cancel button");
		if (onCancel != null)
			onCancel();
		Hide ();
	}
}
