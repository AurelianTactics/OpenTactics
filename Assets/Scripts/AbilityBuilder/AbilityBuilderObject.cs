using UnityEngine;
using System.Collections;

/// <summary>
/// Object used for helping create abilities in the AbilityBuilder Scene
/// Displayed and populated by AbilityEditScrollList.cs
/// Can click on these and then edit values to customize an ability
/// </summary>
public class AbilityBuilderObject
{

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
