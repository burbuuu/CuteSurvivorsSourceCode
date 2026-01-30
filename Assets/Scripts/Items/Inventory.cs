using System.Collections.Generic;
using Items.Data;
using Managers.Scenes;
using Player.Runtime;
using UnityEngine;

namespace Items
{
    // Inventory for a Vampire Survivors-like game:
    // - Weapons are runtime objects
    // - Accessories are passive data
    // - Inventory applies player stat bonuses of the items on pick-up
    public class Inventory : MonoBehaviour
    {
        // Public events
        public event System.Action<Weapon, int> OnWeaponAdded;
        public event System.Action<Accessory, int> OnAccessoryAdded;

        
        [Header("Current equipped items")]
        [SerializeField] private List<Weapon> _equippedWeapons;
        [SerializeField] private List<Accessory> _equippedAccessories;
        [SerializeField] int maxWeapons = 3;
        [SerializeField] int maxAccessories = 3;
        
        #region Public getters
        public List<Weapon> EquippedWeapons => _equippedWeapons;
        public List<Accessory> EquippedAccessories => _equippedAccessories;
        public int MaxWeapons => maxWeapons;
        public int MaxAccessories => maxAccessories;
        public bool IsWeaponInventoryFull => _equippedWeapons.Count >= maxWeapons;
        public bool IsAccessoryInventoryFull => _equippedAccessories.Count >= maxAccessories;
    #endregion
        
        #region Inventory management
        public void AddAccessory(AccessoryData data)
        {
            if (_equippedAccessories.Count >= maxAccessories)
            {
                Debug.LogWarning("Failed to add accessory, inventory full");
                return;
            }
            _equippedAccessories.Add(new Accessory(data));
            
            // Add level 1 bonus
            PlayerController.Instance.Stats.AddBonus(data.GetStatsForLevel(1));
            
            // Notify listeners (UI)
            int slotIndex = _equippedAccessories.Count - 1;
            OnAccessoryAdded?.Invoke(_equippedAccessories[slotIndex], slotIndex);
        }
        public void AddWeapon(GameObject weaponPrefab)
        {
            Weapon weaponComponent = weaponPrefab.GetComponent<Weapon>();
            
            if (weaponComponent == null)
            {
                Debug.LogError($"Prefab {weaponPrefab.name} does not have a Weapon component!");
                return;
            }

            if (_equippedWeapons.Count >= maxWeapons)
            {
                Debug.LogWarning("Failed to add weapon, inventory full");
                return;
            }

            // Instantiate the weapon prefab as a child of the inventory and add it to the list
            GameObject weaponInstance = Instantiate(weaponPrefab, transform);
            Weapon newWeapon = weaponInstance.GetComponent<Weapon>();
            
            _equippedWeapons.Add(newWeapon);
            
            // Notify listeners (UI)
            int slotIndex = _equippedWeapons.Count - 1;
            OnWeaponAdded?.Invoke(newWeapon, slotIndex);
        }
        #endregion
        
        #region Initialization

        void Start()
        {
            var weapon = GameManager.Instance.GetStartingCharacter().weaponPrefab;
            AddWeapon(weapon);
        }
        #endregion
    }
}