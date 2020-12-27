using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class BattleMessageController : MonoBehaviour 
{
	[SerializeField] Text label;
	[SerializeField] GameObject canvas;
	[SerializeField] CanvasGroup group;
	EasingControl ec;

	void Awake ()
	{
		ec = gameObject.AddComponent<EasingControl>();
		ec.duration = 0.5f;
		ec.equation = EasingEquations.EaseInOutQuad;
		ec.endBehaviour = EasingControl.EndBehaviour.Constant;
		ec.updateEvent += OnUpdateEvent;
	}

	public void Display (string message, float displayTime = 1.0f)
	{
		group.alpha = 0;
		canvas.SetActive(true);
		label.text = message;
		StartCoroutine(Sequence(displayTime));
	}

	void OnUpdateEvent (object sender, EventArgs e)
	{
		group.alpha = ec.currentValue;
	}

	IEnumerator Sequence (float time = 1.0f)
	{
		ec.Play();

		while (ec.IsPlaying)
			yield return null;

		yield return new WaitForSeconds(time);

		ec.Reverse();

		while (ec.IsPlaying)
			yield return null;

		canvas.SetActive(false);
	}
}
