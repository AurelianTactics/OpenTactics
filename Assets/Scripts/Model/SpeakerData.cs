using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// In CutScene State in Combat Scene these help display dialogue
/// </summary>
[System.Serializable]
public class SpeakerData 
{
	public List<string> messages;
	public Sprite speaker;
	public TextAnchor anchor;
}
