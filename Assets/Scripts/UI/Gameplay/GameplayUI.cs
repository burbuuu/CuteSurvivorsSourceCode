using System.Collections.Generic;
using Enemies.Runtime;
using Items;
using Managers.StateMachine;
using Player.Runtime;
using SaveSystem;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        private ActiveState _timer;
        
        // HP and EXP Bars
        [SerializeField] private Image hpBarFill;
        [SerializeField] private Image expBarFill;
        
        // Level text
        [SerializeField] private TMP_Text levelText;
        
        // Kill count text
        [SerializeField] private TMP_Text killCountText;
        
        [Header("Item bar")]
        // Inventory bars
        [SerializeField] private Inventory inventory; // Reference to the inventory
        // Items bars
        [SerializeField] private CanvasGroup weaponCanvas;
        [SerializeField] private CanvasGroup accessoryCanvas;
        [SerializeField] private WeaponView weaponViewPrefab;
        [SerializeField] private AccessoryView accessoryViewPrefab;

        private List<WeaponView> weaponViews = new();
        private List<AccessoryView> accessoryViews = new();
        


        void Awake()
        {
            _timer = FindFirstObjectByType<ActiveState>();
            if (_timer == null)
            {
                enabled = false;
            }
            
            // Delete the Inventory slots 
            ClearContainer(weaponCanvas.transform);
            ClearContainer(accessoryCanvas.transform);
        }
        

        
        #region Event Subscriptions

        private void OnEnable()
        {
            // Player stats
            PlayerStats.OnHealthChanged += OnHealthValueChanged;
            PlayerStats.OnExpChanged += OnEXPValueChanged;
            PlayerStats.OnPlayerLevelUp += OnPlayerLevelChanged;
            
            // Enemy kills
            EnemyController.OnEnemyKilled += HandleEnemyKilled;
            
            // Inventory
            inventory.OnWeaponAdded += HandleWeaponAdded;
            inventory.OnAccessoryAdded += HandleAccessoryAdded;
        }

        private void OnDisable()
        {
            // Player stats
            PlayerStats.OnHealthChanged -= OnHealthValueChanged;
            PlayerStats.OnExpChanged -= OnEXPValueChanged;
            PlayerStats.OnPlayerLevelUp -= OnPlayerLevelChanged;
            
            // Enemy kills
            EnemyController.OnEnemyKilled -= HandleEnemyKilled;
            
            // Item icons
            inventory.OnWeaponAdded -= HandleWeaponAdded;
            inventory.OnAccessoryAdded -= HandleAccessoryAdded;
        }
        
        #endregion

        void Start()
        {
            BuildWeaponSlots();
            BuildAccessorySlots();
            InitializeInventory();
            InitializeUI();
        }
        
        void Update()
        {
            HandleTimer();
        }
        
        private void ClearContainer(Transform container)
        {
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                Destroy(container.GetChild(i).gameObject);
            }
        }

        private void HandleTimer()
        {
            float totalSeconds = _timer.GameplayTimer;

            // Calculate minutes and seconds
            int minutes = Mathf.FloorToInt(totalSeconds / 60);
            int seconds = Mathf.FloorToInt(totalSeconds % 60);

            // Update the text string
            timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        }


        public void InitializeUI()
        {
            var stats = PlayerController.Instance.Stats;
            OnHealthValueChanged(stats.CurrentHealth, stats.MaxHealth);
            OnEXPValueChanged(stats.CurrentExp, stats.TotalExpToNextLevel);
            OnPlayerLevelChanged(stats.Level);
        }
        
        private void OnHealthValueChanged(float currentHealth, float maxHealth)
        {
            // Fill the bar
            var ratio = currentHealth / maxHealth;
            hpBarFill.fillAmount = ratio;
        }

        private void OnEXPValueChanged(float currentExp, float totalExpToNextLevel)
        {
            var ratio = currentExp / totalExpToNextLevel;
            expBarFill.fillAmount = ratio;
        }

        private void OnPlayerLevelChanged(int level)
        {
            if (levelText != null)
            {
                levelText.text = $"LV. {level}";
            }
        }

        private void HandleEnemyKilled(string ignore)
        {
            var kills = SaveManager.Instance.RunData.Kills;
            kills++;
            killCountText.text = $"Kills: {kills}";
        }

        #region Inventory

        private void BuildWeaponSlots()
        {
            if (weaponViewPrefab == null)
            {
                Debug.LogError("[GameplayUI] WeaponViewPrefab is NULL. Assign a prefab in the inspector.", this);
                return;
            }
            
            // Create a weapon slot for every slot on the inventory class
            for (int i = 0; i < inventory.MaxWeapons; i++)
            {
                WeaponView view = Instantiate(weaponViewPrefab, weaponCanvas.transform);
                weaponViews.Add(view);
            }
        }


        private void BuildAccessorySlots()
        {
            // Create a accessory slot for every slot on the inventory class
            for (int i = 0; i < inventory.MaxAccessories; i++)
            {
                AccessoryView view = Instantiate(accessoryViewPrefab, accessoryCanvas.transform);
                accessoryViews.Add(view);
            }
        }
        
        
        private void HandleWeaponAdded(Weapon newWeapon, int slot)
        {
            if (slot < 0 || slot >= weaponViews.Count)
            {
                Debug.LogWarning("[GameplayUI]: Failed to add weapon to slot. Slot out of bounds.");
                return;
            }

            WeaponView view = weaponViews[slot];
            
            view.Bind(newWeapon);
        }

        private void HandleAccessoryAdded(Accessory newAccessory, int slot)
        {
            if (slot < 0 || slot >= accessoryViews.Count)
            {
                Debug.LogWarning("[GameplayUI]: Failed to add accessory to slot. Slot out of bounds.");
                return;
            }

            AccessoryView view = accessoryViews[slot];
            
            view.SetIcon(newAccessory.accessoryData.icon);
            view.SetEquipped(true);
        }

        private void InitializeInventory()
        {
            // Weapons
            var weapons = inventory.EquippedWeapons;
            for (int i = 0; i < weapons.Count; i++)
            {
                HandleWeaponAdded(weapons[i], i);
            }

            // Accessories
            var accessories = inventory.EquippedAccessories;
            for (int i = 0; i < accessories.Count; i++)
            {
                HandleAccessoryAdded(accessories[i], i);
            }
        }
        
        #endregion
    }
}