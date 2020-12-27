using UnityEngine;
using System.Collections;

public class AbilityBuilderObject {

	public string Title { get; set; }
    public string Value { get; set; }
    public int Id { get; set; }

    public AbilityBuilderObject(string zTitle, string zValue, int zId = 0)
    {
        this.Title = zTitle;
        this.Value = zValue;
        this.Id = zId;
    }

    public AbilityBuilderObject(string zTitle, int zValue, int zId = 0)
    {
        this.Title = zTitle;
        this.Value = zValue.ToString();
        this.Id = zId;
    }
}
