using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Turns a csv of data placed in the resources file into a directory of objects in the Resources directory
/// The objects can be loaded and used by the game
/// ScriptableObject code is located in Assets/Editor/ScriptableObjectUtility.cs
/// </summary>
public class ConversationData : ScriptableObject 
{
	public List<SpeakerData> list;
}
