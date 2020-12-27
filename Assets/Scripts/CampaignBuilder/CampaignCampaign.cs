using System;
using UnityEngine;

//Campaigns are saved as 4 files:
//CampaignCampaign: campaign level information
//CampaignLevel: level information and specific level information (how many levels, which maps, etc)
//CampaignDialogue: cut scene conversations before and after the battle
//CampaignSpawn: party and enemy units used in each battle
//CalcCode handles the loading of campaigns and some saving (CampaignEditController handles the rest)
//need to use Map Builder and Character Builder scenes to come up with maps and units for campaigns

[Serializable]
public class CampaignCampaign
{
    public int CampaignId { get; set; }
    public int Version { get; set; }
    public string CampaignName { get; set; }

    public CampaignCampaign(int campaignId)
    {
        this.CampaignId = campaignId;
        this.Version = NameAll.VERSION_AURELIAN;
        this.CampaignName = "Custom Campaign " + campaignId;
    }

}
