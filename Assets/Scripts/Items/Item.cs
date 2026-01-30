using Items.Data;
using Player.Data;
using Player.Runtime;

namespace Items
{
    // Abstract class, with a leveling system. Used for accessories and weapons.
    [System.Serializable]
    public abstract class Item
    {
        public ItemData itemData;
        public int Level = 1;

        public Item(ItemData data)
        {
            itemData = data;
        }

        public void ItemLevelUp()
        {
            if (IsMaxLevel())
            {
                UnityEngine.Debug.LogWarning($"{itemData.name} is already at max level.");
                return;
            }

            Level++;
            
            // Apply bonifications to player stats
            PlayerBonusStats stats = itemData.GetStatsForLevel(Level);
            if (stats != null)
            {
                PlayerController.Instance.Stats.AddBonus(stats);
            }
            
            HandleItemLevelUp();
            
        }

        // Hook for child classes to react to level changes
        protected virtual void HandleItemLevelUp()
        {
            UnityEngine.Debug.Log($"{itemData.itemName} leveled up to {Level}!");
        }

        public bool IsMaxLevel() => Level >= itemData.MaxLevel;
    }
}