using System;
using UnityEngine;

//allows for ClassEditObjects for the creating new classes in ClassEditController
//lets user customize classes with the following attributes
[Serializable]
public class CommandSet
{

    public int CommandSetId { get; set; }
    public string CommandSetName { get; set; }
    public int Version { get; set; }

    public CommandSet(int zId, string zName, int zVersion)
    {
        this.CommandSetId = zId;
        this.CommandSetName = zName;
        this.Version = zVersion;
    }

    public void Save()
    {
        string fileName = Application.dataPath + "/Custom/CommandSets/" + this.CommandSetId + "_command_set.dat";
        Serializer.Save<CommandSet>(fileName, this);
    }

    public void RenameAndSave(string zName)
    {
        this.CommandSetName = zName;
        Save();
    }

}