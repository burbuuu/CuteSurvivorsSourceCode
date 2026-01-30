using System.Collections.Generic;
using System.Linq;
using Items.Data;
using LevelUpSystem;
using UnityEngine;

namespace Items
{
    public class ItemDatabase : MonoBehaviour
    {
        [SerializeField] private List<AccessoryData> _accessories;
        [SerializeField] private List<GameObject> _weaponsPrefabs;
        
        [SerializeField] private Inventory _inventory;

        void Awake()
        {
            if (_inventory == null)
            {
                Debug.LogWarning("No inventory found in scene. Fetching from scene hierarchy.");
                _inventory = FindFirstObjectByType<Inventory>();
            }
        }
        
        public List<ILevelUpChoice> GetLevelUpChoices(int count = 3)
        {
            List<ILevelUpChoice> potentialChoices = new List<ILevelUpChoice>();

            // Check existing items for upgrades
            foreach (var weapon in _inventory.EquippedWeapons)
            {
                if (!weapon.WeaponItem.IsMaxLevel())
                {
                    potentialChoices.Add(new UpgradeItemChoice(weapon.WeaponItem));
                }
            }

            foreach (var accessory in _inventory.EquippedAccessories)
            {
                if (!accessory.IsMaxLevel())
                {
                    potentialChoices.Add(new UpgradeItemChoice(accessory));
                }
            }

            // Check for new items if inventory is not full
            if (!_inventory.IsWeaponInventoryFull)
            {
                foreach (var prefab in _weaponsPrefabs)
                {
                    Weapon weaponComp = prefab.GetComponent<Weapon>();
                    if (weaponComp != null)
                    {
                        WeaponData data = weaponComp.WeaponItem.weaponData;
                        // Only suggest if not already in inventory
                        if (_inventory.EquippedWeapons.All(w => w.WeaponItem.weaponData != data))
                        {
                            potentialChoices.Add(new NewWeaponChoice(data, prefab));
                        }
                    }
                }
            }

            if (!_inventory.IsAccessoryInventoryFull)
            {
                foreach (var data in _accessories)
                {
                    // Only suggest if not already in inventory
                    if (_inventory.EquippedAccessories.All(a => a.itemData != data))
                    {
                        potentialChoices.Add(new NewAccessoryChoice(data));
                    }
                }
            }

            // Shuffle and pick N choices from the list
            return potentialChoices.OrderBy(x => Random.value).Take(count).ToList();
        }
    }
}