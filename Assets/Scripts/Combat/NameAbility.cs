using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NameAbility {
    //in character select
    //returns a dictionary with the display name and the ability int
    //use the dictionary to create a list that populates the dropdown
    //on dropdown select, an int value is returned, use that int in the list to get the name of the ability
    //use the ability name in the dictionary to get the ability int
    //set the ability int to the player unit's slot
    //Dictionary<KeyType,ValueType> myDictionary = new Dictionary<KeyType,ValueType>();

    //in battle
    //create a dict that only has the abilities present in that battle, can access the dict for display purposes when needed

    public Dictionary<int, string> GetColorDict()
    {
        return CreateColorDict();
    }

    public string GetColorName(int key)
    {
        Dictionary<int, string> myDict = CreateColorDict();
        string value;
        if (myDict.TryGetValue(key, out value))
        {
            return value;
        }
        else
        {
            value = "";
            return value;
        }
        //return myDict[key];
    }

    Dictionary<int, string> CreateColorDict()
    {
        Dictionary<int, string> myDict = new Dictionary<int, string>();
        //red red-purple purple blue-purple
        //blue blue-green green yellow-green
        //yellow yellow-orange orange orange-red
        myDict.Add(0, "Red");
        myDict.Add(1, "Red/Purple");
        myDict.Add(2, "Purple");
        myDict.Add(3, "Purple/Blue");
        myDict.Add(4, "Blue");
        myDict.Add(5, "Blue/Green");
        myDict.Add(6, "Green");
        myDict.Add(7, "Green/Yellow");
        myDict.Add(8, "Yellow");
        myDict.Add(9, "Yellow/Orange");
        myDict.Add(10, "Orange");
        myDict.Add(11, "Orange/Red");
        return myDict;
    }


}
