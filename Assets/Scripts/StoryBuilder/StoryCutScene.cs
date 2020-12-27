using UnityEngine;
using System;
using System.Collections.Generic;

//associated with Story Object by StoryId
//consists of a list of StoryCutSceneObjects, which are used in CutScene state to display a cut scene
[Serializable]
public class StoryCutScene {

	public int StoryId { get; private set; }
    public List<StoryCutSceneObject> cutSceneList;
    int cutSceneId = 0;

    public StoryCutScene(int storyId)
    {
        this.StoryId = storyId;
        cutSceneList = new List<StoryCutSceneObject>();
    }

    public void AddStoryCutSceneObject()
    {
        StoryCutSceneObject scso = new StoryCutSceneObject(GetNewCutSceneId());
        cutSceneList.Add(scso);
    }

    int GetNewCutSceneId()
    {
        cutSceneId += 1;
        return cutSceneId;
    }

    public void DeleteStoryCutSceneObject(int zCutSceneId)
    {
        foreach( StoryCutSceneObject sc in cutSceneList)
        {
            if (sc.CutSceneId == zCutSceneId)
            {
                cutSceneList.Remove(sc);
                return;
            }
        }
    }

    public StoryCutSceneObject GetCutSceneObject(int zCutSceneId)
    {
        foreach (StoryCutSceneObject sc in cutSceneList)
        {
            if (sc.CutSceneId == zCutSceneId)
            {
                return sc;
            }
        }
        return null;
    }
}

[Serializable]
public class StoryCutSceneObject
{
    public int CutSceneId { get; private set; }
    public int BackgroundImage { get; set; } //not implemented, for now just default image
    public int NextCutSceneId { get; set; } //goes to another cutscene if this value is set, if not returns to StoryMap
    public string BackgroundTitle { get; set; } //just static text, simpleversion for now
    public string BackgroundText { get; set; } //just static text, simple verion for now
    public int DialogueLevel { get; set; } //if this isn't 0 can insert a dialogue conversation

    public StoryCutSceneObject( int zCutSceneId)
    {
        this.CutSceneId = zCutSceneId;
        this.BackgroundImage = 0;
        this.NextCutSceneId = NameAll.NULL_INT;
        this.BackgroundText = "Cut scene text";
        this.BackgroundTitle = "Cut Scene Title";
        this.DialogueLevel = NameAll.NULL_INT;
    }

    public string GetNextCutSceneString()
    {
        if (this.NextCutSceneId == NameAll.NULL_INT)
            return "Return to story";
        else
            return "Play Cut Scene Id: " + this.NextCutSceneId;
    }
}

