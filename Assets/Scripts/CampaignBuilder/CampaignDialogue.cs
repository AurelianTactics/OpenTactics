using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class CampaignDialogue
{
    
    public int CampaignId { get; set; }

    public List<DialogueObject> dialogueList;

    public CampaignDialogue(int id)
    {
        this.CampaignId = id;
        dialogueList = new List<DialogueObject>();
    }
   
    public List<DialogueObject> GetDialogueListByLevel(int level)
    {
        var enumerator = dialogueList.Where(d => d.Level == level);
        return enumerator.ToList();
    }

    public void DeleteDialogue(DialogueObject d)
    {
        int z1 = d.Order;
        for (int i = 0; i < this.dialogueList.Count; i++)
        {
            if (this.dialogueList[i].Level == d.Level)
            {
                if (this.dialogueList[i].Order > d.Order)
                    this.dialogueList[i].Order -= 1;
            }
        }
        dialogueList.Remove(d);
    }

    //creates a new dialogue and returns it for inputs
    public DialogueObject AddNewDialogue(int level)
    {
        var tempList = GetDialogueListByLevel(level);
        int z1 = tempList.Count;
        DialogueObject d = new DialogueObject(level, z1);
        dialogueList.Add(d);
        return d;
    }

    //update the order while moving the items in the list
    public void UpdateOrder(DialogueObject d, int value)
    {
        int oldValue = d.Order;
        if( oldValue == value)
        {
            return;
        }

        for (int i = 0; i < this.dialogueList.Count; i++){
            if (this.dialogueList[i].Level == d.Level)
            {
                if (this.dialogueList[i] == d)
                {
                    this.dialogueList[i].Order = value;
                }
                else if (oldValue < value)
                {
                    if (this.dialogueList[i].Order > oldValue && this.dialogueList[i].Order <= value)
                        this.dialogueList[i].Order -= 1;
                }
                else if( oldValue > value)
                {
                    if (this.dialogueList[i].Order < oldValue && this.dialogueList[i].Order >= value)
                        this.dialogueList[i].Order += 1;
                }
            }
        }
    }

    public void UpdatePhase(DialogueObject d, int value)
    {
        foreach (DialogueObject d2 in this.dialogueList)
        {
            if (d2 == d)
            {
                d2.Phase = value;
                break;
            }
                
        }
    }

    public void UpdateIcon(DialogueObject d, int value)
    {
        foreach (DialogueObject d2 in this.dialogueList)
        {
            if (d2 == d)
            {
                d2.Icon = value;
                break;
            }
                
        }
    }

    public void UpdateDialogue(DialogueObject d, string value)
    {
        foreach (DialogueObject d2 in this.dialogueList)
        {
            if (d2 == d)
            {
                d2.Dialogue = value;
                break;
            }
                
            
        }
    }

    public void UpdateSide(DialogueObject d, int value)
    {
        foreach (DialogueObject d2 in this.dialogueList)
        {
            if (d2 == d)
            {
                d2.Side = value;
                break;
            }
                
            
        }
    }

    //converts DialogueObject to ConversationData (DialogueObject -> SpeakerData -> ConversationData.list
    public ConversationData GetConversationData(int levelId, int dialoguePhase)
    {
        ConversationData cd = new ConversationData();
        cd.list = new List<SpeakerData>();
        var enumerator = dialogueList.Where(d => d.Level == levelId);
        var tempList = enumerator.ToList();
        foreach( DialogueObject d in tempList)
        {
            if( d.Phase == dialoguePhase)
            {
                cd.list.Add(GetDialogueObjectToSpeakerData(d));
            }
        }

        if (cd.list.Count == 0)
            return null;

        return cd;
    }

    SpeakerData GetDialogueObjectToSpeakerData(DialogueObject d)
    {
        SpeakerData sd = new SpeakerData();
        sd.messages = new List<string>();
        sd.messages.Add(d.Dialogue);
        sd.speaker = GetDialogueIconToSprite(d.Icon);
        sd.anchor = GetDialogueSideToTextAnchor(d.Side);
        return sd;
    }

    Sprite GetDialogueIconToSprite(int icon)
    {
        if( icon == NameAll.DIALOGUE_ICON_1)
        {
            return Resources.Load<Sprite>("DialogueIcons/1_icon");
        }
        else
        {
            return Resources.Load<Sprite>("DialogueIcons/0_icon");
        }

    }
    
    TextAnchor GetDialogueSideToTextAnchor(int side)
    {
        if( side == NameAll.DIALOGUE_SIDE_RIGHT)
        {
            return TextAnchor.MiddleRight;
        }
        else
        {
            return TextAnchor.MiddleLeft;
        }
    }

    //public void SaveCampaignDialogue()
    //{
    //    string filePath = Application.dataPath + "/Campaigns/Custom/";
    //    Serializer.Save<CampaignDialogue>((filePath + this.CampaignId + "_dialogue.dat"), this);
    //}
}

[Serializable]
public class DialogueObject
{
    public int Level { get; set; }
    public int Phase { get; set; }
    public int Icon { get; set; }
    public int Order { get; set; }
    public int Side { get; set; }
    public string Dialogue { get; set; }

    public DialogueObject()
    {
        this.Level = 0;
        this.Phase = NameAll.DIALOGUE_PHASE_START;
        this.Icon = NameAll.DIALOGUE_ICON_0;
        this.Order = 0;
        this.Side = NameAll.DIALOGUE_SIDE_LEFT;
        this.Dialogue = "default text";
    }

    public DialogueObject(int level, int order)
    {
        this.Level = level;
        this.Phase = NameAll.DIALOGUE_PHASE_START;
        this.Icon = NameAll.DIALOGUE_ICON_0;
        this.Order = order;
        this.Side = NameAll.DIALOGUE_SIDE_LEFT;
        this.Dialogue = "default text";
    }

}