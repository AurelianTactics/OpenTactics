using UnityEngine;
using System.Collections;

/// <summary>
/// In Combat Scene, game progresses by transitioning from State to State
/// CombatState.cs inherits this class and other states inherit from CombatState.cs
/// </summary>
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