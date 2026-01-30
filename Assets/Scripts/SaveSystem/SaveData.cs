using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    
[Serializable]
public class SaveData :ISerializationCallbackReceiver
{
    // Unlocks
    public List<string> UnlockedCharacters = new List<string>();
    public List<string> UnlockedWeapons = new List<string>();
    public List<string> UnlockedAccessories = new List<string>();
    
    // Lifetime totals
    public int TotalKills;
    public int TotalDamage;
    public float TotalTimeSurvived;
    
    // High scores (records)
    public int MaxKills;
    public int MaxDamage;
    public float MaxTimeSurvived;
    
    // Weapon specific data
    // Dictionaries are not serializable!
    public Dictionary<string, ItemSaveData> ItemSaveData = new Dictionary<string, ItemSaveData>();
    
    // Serialize helpers
    private List<string> _itemKeys = new List<string>();
    private List<ItemSaveData> _itemValues = new List<ItemSaveData>();

    public void OnBeforeSerialize()
    {
        _itemKeys.Clear();
        _itemValues.Clear();
        foreach (var kvp in ItemSaveData)
        {
            _itemKeys.Add(kvp.Key);
            _itemValues.Add(kvp.Value);
        }
    }
    
    public void OnAfterDeserialize()
    {
        ItemSaveData = new Dictionary<string, ItemSaveData>();
        for (int i = 0; i < _itemKeys.Count; i++)
        {
            ItemSaveData[_itemKeys[i]] = _itemValues[i];
        }
    }
}

[System.Serializable]
public class ItemSaveData
{
    public int ItemLevel;
    public int TotalDamage;
    public int TotalKills;
    public int MaxDamagePerRun;
    public int MaxKillsPerRun;
    public float TotalHealedLife;
    public float MaxHealedLife;
}

[System.Serializable]
public class RunData
{
    public bool Survived = true;
    public int TotalDamage = 0;
    public int Kills;
    public float TimeSurvived;
    public float TotalHeals;

    public string Character;
    public string StageId;

    public Dictionary<string, int> DamagePerItem = new Dictionary<string, int>();
    public Dictionary<string, int> KillsPerItem = new Dictionary<string, int>();
    public Dictionary<string, float> HealsPerItem = new Dictionary<string, float>();
    
    public void RegisterDamage(string weapon, int damage)
    {
        if (!DamagePerItem.ContainsKey(weapon)) DamagePerItem[weapon] = 0;
        
        DamagePerItem[weapon] += damage;
        TotalDamage += damage;
    }

    public void RegisterKill(string weapon)
    {
        if (!KillsPerItem.ContainsKey(weapon)) KillsPerItem[weapon] = 0;
        
        Kills++;
        KillsPerItem[weapon]++;
    }

    public void RegisterHeal(string item, float heal)
    {
        if (!HealsPerItem.ContainsKey(item)) HealsPerItem[item] = 0;
        
        HealsPerItem[item] += heal;
        TotalHeals += heal;
    }

    public void RegisterHealPlayerRegen(float heal)
    {
        TotalHeals += heal;
    }
}

}