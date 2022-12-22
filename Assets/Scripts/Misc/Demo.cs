using UnityEngine;
using System.Collections;

/// <summary>
/// Not used anywhere
/// Misc file showing outline of how state behavvior works
/// Not implementing anywhere but useful for understanding things at a high level
/// </summary>

public class Demo : MonoBehaviour 
{
		enum State
		{
			Loading,
			Playing,
			GameOver
		}
		State _state;

	void CheckState ()
	{
		switch (_state)
		{
		case State.Loading:
			// Loading Logic here
			break;
		case State.Playing:
			// Playing Logic here
			break;
		case State.GameOver:
			// GameOver Logic here
			break;
		}
	}
}