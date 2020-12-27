using UnityEngine;
using System.Collections;

public abstract class State : MonoBehaviour 
{
	public virtual void Enter ()
	{
		AddListeners(); //Debug.Log("entering state");
	}
	
	public virtual void Exit ()
	{
		RemoveListeners(); //Debug.Log("exiting state");
	}

	protected virtual void OnDestroy ()
	{
		RemoveListeners();
	}

	protected virtual void AddListeners ()
	{

	}
	
	protected virtual void RemoveListeners ()
	{

	}
}