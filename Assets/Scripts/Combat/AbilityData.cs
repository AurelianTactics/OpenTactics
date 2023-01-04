using UnityEngine;
using System;

/// <summary>
/// Turns a csv of data placed in the resources file into a directory of objects in the Resources directory
/// The objects can be loaded and used by the game
/// ScriptableObject code is located in Assets/Editor/ScriptableObjectUtility.cs
/// </summary>
public class AbilityData : ScriptableObject {

    public int overallId;
    public int version;
    public int slot;
    public int slotId;
    public string abilityName;

    public int classId;
    public string description;

    //loads from a .csv placed in the resources file
    public void Load(string line)
    {
        //Debug.Log(" loading an item data " + line);
        string[] elements = line.Split(',');

        this.overallId = Convert.ToInt32(elements[0]);
        this.version = Convert.ToInt32(elements[1]);
        this.slot = Convert.ToInt32(elements[2]);
        this.slotId = Convert.ToInt32(elements[3]);
        this.abilityName = elements[4];

        this.classId = Convert.ToInt32(elements[5]);
        this.description = elements[6];
        
    }
}
