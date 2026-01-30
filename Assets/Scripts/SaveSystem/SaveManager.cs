using System;
using UnityEngine;
using System.IO;
using Enemies.Runtime;
using Managers.Scenes;

namespace SaveSystem
{
    
public class SaveManager : MonoBehaviour
{
    #region Definitions
    public static SaveManager Instance { get; private set; } // Singleton
    public SaveData SaveData { get; private set; }
    private string _savePath;
    private const string SaveDataFileName = "save.json";
    
    [SerializeField] private RunData _currentRun;
    public RunData RunData => _currentRun;
    
    #endregion

    public void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("[SaveManager] Duplicate instance detected. Destroying new instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _savePath = Application.persistentDataPath + "/" + SaveDataFileName;
        Load();
    }
    
    private void OnApplicationQuit()
    {
        Save();
    }

    #region Event subscription

    private void OnEnable()
    {
        // Game start
        GameplayManager.RunStart += HandleRunStarts;
        
        // Enemy kills and damage
        EnemyController.OnEnemyKilled += HandleEnemyKilled;
        EnemyController.OnEnemyDamageTaken += HandleEnemyDamageTaken;
    }

    private void OnDisable()
    {
        // Game start
        GameplayManager.RunStart -= HandleRunStarts;
        
        // Enemy kills and damage
        EnemyController.OnEnemyKilled -= HandleEnemyKilled;
        EnemyController.OnEnemyDamageTaken -= HandleEnemyDamageTaken;
    }

    #endregion


    #region Save / Load /Create /Destroy Save
    public void Load()
    {
        // If file doesn't exist create a save file
        if (!File.Exists(_savePath))
        {
            CreateSaveFile();
            Debug.Log("[SaveManager] No save file found. Creating new save.");
            return;
        }

        // Try load save, if it fails (the save is corrupt) 
        try
        {
            string json = File.ReadAllText(_savePath);
            SaveData = JsonUtility.FromJson<SaveData>(json);
            if (SaveData == null) // If it fails throw exception
                throw new System.Exception("Deserialization failed");
            Debug.Log("[SaveManager] Save file loaded successfully.");
        }
        catch
        {
            Debug.LogWarning("[SaveManager] Save file corrupt. Creating a new save file.");
            CreateSaveFile();
        }

    }

    // This is private only the manager can create a file
    private void CreateSaveFile()
    {
        SaveData = new SaveData();
        Save(); // create file on first run
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(SaveData, true);
        File.WriteAllText(_savePath, json);
        Debug.Log("[SaveManager] Save file written to disk.");
    }
    
    public void DeleteSave()
    {
        if (File.Exists(_savePath)) File.Delete(_savePath);
        Debug.Log("[SaveManager] Save file deleted.");
        CreateSaveFile();
    }

    #endregion

    #region Process run data

    public void ProcessRunData(RunData runData)
    {
        if (runData == null)
        {
            Debug.LogWarning("[SaveManager] Tried to process null RunData.");
            return;
        }
        
        // Update lifetime stats
        SaveData.TotalKills += runData.Kills;
        SaveData.TotalDamage += runData.TotalDamage;
        SaveData.TotalTimeSurvived += runData.TimeSurvived;
        
        // Update High Scores
        if (runData.Kills > SaveData.MaxKills) SaveData.MaxKills = runData.Kills;
        if (runData.TotalDamage > SaveData.MaxDamage) SaveData.MaxDamage = runData.TotalDamage;
        if (runData.TimeSurvived > SaveData.MaxTimeSurvived) SaveData.MaxTimeSurvived = runData.TimeSurvived;
        

        // Process damage per item
        foreach (var itemId in runData.DamagePerItem.Keys)
        {
            if (!SaveData.ItemSaveData.ContainsKey(itemId))
                SaveData.ItemSaveData[itemId] = new ItemSaveData();

            var itemData = SaveData.ItemSaveData[itemId];
            var runDamage = runData.DamagePerItem[itemId];

            itemData.TotalDamage += runDamage;

            if (runDamage > itemData.MaxDamagePerRun)
                itemData.MaxDamagePerRun = runDamage;
        }
        
        // Process kills per item
        foreach (var itemId in runData.KillsPerItem.Keys)
        {
            if (!SaveData.ItemSaveData.ContainsKey(itemId))
                SaveData.ItemSaveData[itemId] = new ItemSaveData();

            var itemData = SaveData.ItemSaveData[itemId];
            var runKills = runData.KillsPerItem[itemId];

            itemData.TotalKills += runKills;

            if (runKills > itemData.MaxKillsPerRun)
                itemData.MaxKillsPerRun = runKills;
        }
        
        // Process heals per item 
        foreach (var itemId in runData.HealsPerItem.Keys)
        {
            if (!SaveData.ItemSaveData.ContainsKey(itemId))
                SaveData.ItemSaveData[itemId] = new ItemSaveData();

            var itemData = SaveData.ItemSaveData[itemId];
            var runHeals = runData.HealsPerItem[itemId];

            itemData.TotalHealedLife += runHeals;

            if (runHeals > itemData.MaxHealedLife)
                itemData.MaxHealedLife = runHeals;
        }
        
        // TODO Check for unlocks
        
        // Save the persistent data
        Save();
    }

    // Create run data for the actual run
    private void HandleRunStarts()
    {
        _currentRun = new RunData();
    }
    
    void HandleEnemyKilled(string weapon)
    {
        if (_currentRun == null)
        {
            Debug.LogWarning("[SaveManager] Enemy killed but no active RunData.");
            return;
        }

        _currentRun.RegisterKill(weapon);
    }

    void HandleEnemyDamageTaken(string weapon, float damage)
    {
        if (_currentRun == null)
        {
            Debug.LogWarning("[SaveManager] Enemy damaged but no active RunData.");
            return;
        }

        _currentRun.RegisterDamage(weapon, (int)damage);
    }

    private void HandleHeal(float amount)
    {
        _currentRun.RegisterHealPlayerRegen( amount);
    }
    
    #endregion
    

}


}