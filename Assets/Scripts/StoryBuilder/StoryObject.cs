using UnityEngine;
using System.Collections.Generic;
using System;

//main object for outlining a story
//contains story points that allow user actions during certain points in the story
[Serializable]
public class StoryObject {

    public int StoryId { get; set; }
    public string StoryName { get; set; }
    public int MapId { get; set; }
    public int StoryInt { get; set; } //current progression in the story
    public int CampaignId { get; set; } //associate a campaignId with the story to allow choosing battles and dialogues
    public bool EnableSkipMode { get; set; }
    public List<StoryIntProgression> storyIntProgressionList;
    public int Version { get; set; }
    public List<PlayerUnit> startingPlayerUnitList;

    public StoryObject(int storyId, int mapId, string storyName)
    {
        this.StoryId = storyId;
        this.MapId = mapId;
        this.StoryName = storyName;
        this.StoryInt = 0;
        this.storyIntProgressionList = new List<StoryIntProgression>();
        this.CampaignId = NameAll.NULL_INT;
        this.EnableSkipMode = false;
        this.Version = NameAll.VERSION_AURELIAN;
        this.startingPlayerUnitList = new List<PlayerUnit>();
    }

    //an event is completed. if return true, handler knows to reload the point details
    //public bool ProgressStory(int progressionInt)
    //{
    //    StoryIntProgression sip = new StoryIntProgression(this.StoryInt, progressionInt);
    //    if(this.StoryDict.ContainsKey(sip))
    //    {
    //        this.StoryInt = StoryDict[sip]; //storyInt progresses
    //        return true;
    //    }
    //    return false;
    //}

    //returns new story int after progression in the story
    public int GetNewStoryInt(int storyInt, int progressionInt)
    {
        foreach (StoryIntProgression sip in storyIntProgressionList)
        {
            if (sip.StoryInt == storyInt && sip.ProgressionInt == progressionInt )
            {
                return sip.StoryIntNew;
            }
        }
        return storyInt; //no match found, returns existing storyInt (which is fine)
    }

    public bool IsStoryIntProgressionInt(int storyInt, int progressionInt, int storyIntNew)
    {
        foreach (StoryIntProgression sip in storyIntProgressionList)
        {
            if ( sip.StoryInt == storyInt && sip.ProgressionInt == progressionInt && sip.StoryIntNew == storyIntNew )
            {
                return true;
            }
        }
        return false;
    }

    public void AddStoryIntProgression(int storyInt, int progressionInt, int storyIntNew)
    {
        if (storyInt == NameAll.NULL_INT || progressionInt == NameAll.NULL_INT)
            return;

        if (IsStoryIntProgressionInt(storyInt, progressionInt, storyIntNew))
            return;
        else
            this.storyIntProgressionList.Add(new StoryIntProgression(storyInt, progressionInt, storyIntNew));

    }

    public void DeleteStoryIntProgression(int index)
    {
        try
        {
            storyIntProgressionList.RemoveAt(index);
        }
        catch (Exception)
        {
            Debug.Log("ERROR: deleting storyIntProgression but index not found in list");
        }
    }

    public string GetAssociatedCampaignIdString()
    {
        if (this.CampaignId == NameAll.NULL_INT)
            return "No campaign selected. Unable to choose battles or dialogue";
        else
            return "Selected campaign id: " + this.CampaignId;
    }
}

//key to dictionary in StoryObject
//story progresses (ie storyInt increases) when the current StoryInt and Progression Int are used as a key in the StoryDictionary
//if key doesn't exist, then story doesn't progress
//if key exists, the value found becomes the next StoryInt
[Serializable]
public class StoryIntProgression
{
    public int StoryInt; //StoryInt and ProgressionInt combine to form StoryIntNew
    public int ProgressionInt;
    public int StoryIntNew; //story int upon completion

    public StoryIntProgression(int zStory, int zPro, int zStoryIntNew)
    {
        this.StoryInt = zStory;
        this.ProgressionInt = zPro;
        this.StoryIntNew = zStoryIntNew;
    }
}
