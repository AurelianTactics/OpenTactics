using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

//automatically generates Item, Ability, and SpellName data when the csv files with the raw info on them change
public class SettingsAutoConverter : AssetPostprocessor
{
    static Dictionary<string, Action> parsers;

    static SettingsAutoConverter()
    {
        //Debug.Log(" doing settings auto converter");
        parsers = new Dictionary<string, Action>();
        parsers.Add("ItemDataCSV.csv", ParseEnemies);
        parsers.Add("AbilityDataCSV.csv", ParseAbility);
        parsers.Add("SpellNameDataCSV.csv", ParseSpellName);
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        //Debug.Log(" doing OnPostprocess");
        for (int i = 0; i < importedAssets.Length; i++)
        {
            string fileName = Path.GetFileName(importedAssets[i]); //Debug.Log(" filename " + fileName);
            if (parsers.ContainsKey(fileName))
                parsers[fileName]();
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void ParseEnemies()
    {
        Debug.Log(" doing ParseEnemies");
        string filePath = Application.dataPath + "/Settings/ItemDataCSV.csv";
        if (!File.Exists(filePath))
        {
            Debug.LogError("Missing csv file Data: " + filePath);
            return;
        }

        string[] readText = File.ReadAllLines("Assets/Settings/ItemDataCSV.csv");
        filePath = "Assets/Resources/Items/";
        for (int i = 1; i < readText.Length; ++i)
        {
            ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
            itemData.Load(readText[i]);
            string fileName = string.Format("{0}{1}.asset", filePath, "item_" + itemData.item_id);
            AssetDatabase.CreateAsset(itemData, fileName);
        }
    }

    static void ParseAbility()
    {
        Debug.Log(" doing Ability");
        string filePath = Application.dataPath + "/Settings/AbilityDataCSV.csv";
        if (!File.Exists(filePath))
        {
            Debug.LogError("Missing csv file Data: " + filePath);
            return;
        }

        string[] readText = File.ReadAllLines("Assets/Settings/AbilityDataCSV.csv");
        filePath = "Assets/Resources/Abilities/";
        for (int i = 1; i < readText.Length; ++i)
        {
            AbilityData abilityData = ScriptableObject.CreateInstance<AbilityData>();
            abilityData.Load(readText[i]);
            string fileName = string.Format("{0}{1}.asset", filePath, "ability_" + abilityData.slot + "_"+ abilityData.slotId);
            AssetDatabase.CreateAsset(abilityData, fileName);
        }
    }

    static void ParseSpellName()
    {
        Debug.Log(" doing SpellName");
        string filePath = Application.dataPath + "/Settings/SpellNameDataCSV.csv";
        if (!File.Exists(filePath))
        {
            Debug.LogError("Missing csv file Data: " + filePath);
            return;
        }

        string[] readText = File.ReadAllLines("Assets/Settings/SpellNameDataCSV.csv");
        filePath = "Assets/Resources/SpellNames/";
        for (int i = 1; i < readText.Length; ++i)
        {
            SpellNameData snData = ScriptableObject.CreateInstance<SpellNameData>();
            snData.Load(readText[i]);
            string fileName = string.Format("{0}{1}.asset", filePath, "sn_" + snData.Index);
            AssetDatabase.CreateAsset(snData, fileName);
        }
    }
}