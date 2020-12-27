using UnityEngine;
using System.Collections;

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