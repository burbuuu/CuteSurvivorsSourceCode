using System.Collections.Generic;
using Player.Data;
using UnityEngine;

namespace Items.Data
{
    public abstract class ItemData : ScriptableObject
    {
        [Header("Generic data")]
        public Sprite icon;
        public string itemName;
        public List<string> itemDescriptionsPerLevel = new List<string>();
        
        [Header("Item Bonus Stats")]
        [Tooltip("Define the stats for each level here.")]
        public List<PlayerBonusStats> levelStats = new List<PlayerBonusStats>();

        // Getters
        public PlayerBonusStats GetStatsForLevel(int level)
        {
            if (levelStats.Count == 0) return null;
            
            // Level is 1-based, a list is 0-based
            int index = Mathf.Clamp(level - 1, 0, MaxLevel - 1);
            return levelStats[index];
        }

        public string GetDescriptionForLevel(int level)
        {
            if (itemDescriptionsPerLevel == null || itemDescriptionsPerLevel.Count == 0)
                return string.Empty;

            int index = Mathf.Clamp(level - 1, 0, itemDescriptionsPerLevel.Count - 1);
            return itemDescriptionsPerLevel[index];
        }

        public virtual int MaxLevel => levelStats.Count;
        
    }
}