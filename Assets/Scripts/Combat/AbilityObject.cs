using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


//for now just used to make lists for the character builder, in the future add something like itemobject where ability objects can be generated on the fly and used to get info
public class AbilityObject
{
    //overallId	version	slot	slotId	abilityName	classId	description
    public int OverallId { get; set; }
    public int Version { get; set; }
    public int Slot { get; set; }
    public int SlotId { get; set; }
    public string AbilityName { get; set; }

    public int ClassId { get; set; }
    public string Description { get; set; }
    //public int APCost { get; set; }

    public AbilityObject()
    {

    }

    public AbilityObject(ClassEditObject ce)
    {
        this.OverallId = ce.ClassId;
        this.Version = ce.Version;
        this.Slot = NameAll.ABILITY_SLOT_PRIMARY;
        this.SlotId = ce.ClassId;
        this.AbilityName = ce.ClassName;
        this.ClassId = ce.ClassId;
        this.Description = ce.ClassName;
        //this.APCost = 0;
    }

    public AbilityObject(AbilityData ad)
    {
        this.OverallId = ad.overallId;
        this.Version = ad.version;
        this.Slot = ad.slot;
        this.SlotId = ad.slotId;
        this.AbilityName = ad.abilityName;

        this.ClassId = ad.classId;
        this.Description = ad.description;
        //this.APCost = 0;
    }

    public List<AbilityObject> GetAbilityList(int modVersion) //aurelian
    {
        List<AbilityObject> al = new List<AbilityObject>(); //Debug.Log("getting weapon list");
        AbilityObject a;
        Object[] adArray = Resources.LoadAll("Abilities", typeof(AbilityData)); //(AbilityData[])
        foreach ( Object o in adArray)
        {
            AbilityData ad = (AbilityData)o;
            if( ad.version == modVersion)
            {
                a = new AbilityObject(ad);
                al.Add(a);
            }
        }

        foreach (AbilityData ad in adArray)
        {
            Resources.UnloadAsset(ad);
        }

        return al;
    }

    //used in learning spellnames in storyMode
    public AbilityObject(SpellName sn)
    {
        this.OverallId = sn.SpellId;
        this.Version = sn.Version;
        this.Slot = NameAll.ABILITY_SLOT_SECONDARY;
        this.SlotId = sn.SpellId;
        this.AbilityName = sn.AbilityName;

        this.ClassId = sn.CommandSet;
        this.Description = "";
        //this.APCost = 0;
    }

}
