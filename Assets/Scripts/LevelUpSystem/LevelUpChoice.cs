using Items;
using Items.Data;
using UnityEngine;

namespace LevelUpSystem
{
    // Interface for level up choices
    public interface ILevelUpChoice
    {
        public ItemData ItemData { get; }
        public int PreviewLevel { get; }
        public void Apply(Inventory inventory);
    }
    
    // Abstract class for level up choices
    public abstract class LevelUpChoice : ILevelUpChoice
    {
        public ItemData ItemData { get; protected set; }
        public int PreviewLevel { get; protected set; }
        public abstract void Apply(Inventory inventory);
    }

    public class NewWeaponChoice : LevelUpChoice
    {
        private GameObject _weaponPrefab;

        public NewWeaponChoice(WeaponData data, GameObject prefab)
        {
            ItemData = data;
            PreviewLevel = 1;
            _weaponPrefab = prefab;
        }

        public override void Apply(Inventory inventory)
        {
            inventory.AddWeapon(_weaponPrefab);
        }
    }

    public class NewAccessoryChoice : LevelUpChoice
    {
        private AccessoryData _accessoryData;

        public NewAccessoryChoice(AccessoryData data)
        {
            ItemData = data;
            PreviewLevel = 1;
            _accessoryData = data;
        }

        public override void Apply(Inventory inventory)
        {
            inventory.AddAccessory(_accessoryData);
        }
    }

    public class UpgradeItemChoice : LevelUpChoice
    {
        private Item _item;

        public UpgradeItemChoice(Item item)
        {
            _item = item;
            ItemData = item.itemData;
            PreviewLevel = item.Level + 1;
        }

        public override void Apply(Inventory inventory)
        {
            _item.ItemLevelUp();
        }
    }
    
    
}