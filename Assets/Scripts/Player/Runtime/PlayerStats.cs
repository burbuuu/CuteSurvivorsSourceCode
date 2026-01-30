using System;
using Player.Data;
using UnityEngine;

namespace Player.Runtime
{
    [System.Serializable]
    public class PlayerStats
    {
        public static event Action OnStatsChanged; // Meant for pickups. Notify systems that the player stats have changed
        public static event Action<float, float> OnHealthChanged; //  <current health / max health> 
        public static event Action<float, float> OnExpChanged;
        public static event Action<int> OnPlayerLevelUp;
        public static event Action OnPlayerDeath;

        #region Definitions
        private int _level;
        private float _currentExp;
        private float _totalExpToNextLevel;
        
        // Percent bonuses
        private float _healthBonus;
        private float _moveSpeedBonus;
        private float _damageBonus;
        private float _cooldownBonus;
        private float _areaBonus;
        private float _durationBonus;
        private float _luckBonus;
        private float _experienceBonus;
        private float _projectileSpeedBonus;
        private float _pickUpRangeBonus;
        
        // Value bonuses
        private float _healthRegenBonus;
        private int _projectileCountBonus;

        #endregion

        #region Public stats getters
        public int Level { get; private set; }
        public float CurrentExp { get; private set; }
        public float TotalExpToNextLevel { get; private set; }
        public float MaxHealth { get; private set; }
        public float MoveSpeed { get; private set; }
        public float DamageBonus { get; private set; }
        public float CooldownBonus { get; private set; }
        public float AreaBonus { get; private set; }
        public float DurationBonus { get; private set; }
        public float Luck { get; private set; }
        public float ExperienceBonus { get; private set; }
        public float HealthRegen { get; private set; }
        public float ProjectileSpeedBonus { get; private set; }

        public float PickUpRange => BaseStats.PickUpRange * (1f + _pickUpRangeBonus); // Radius in world units
        public int ProjectileCountBonus { get; private set; }
        
        public float CurrentHealth { get; private set; }

        #endregion
        
        #region Constructors
        // Constructor
        public PlayerStats(CharacterData character)
        {
            AddBonus(character.stats);
            Recalculate();
            CurrentHealth = MaxHealth;
        }
        #endregion

        #region Stat creation and actualization
        
        public void AddBonus(PlayerBonusStats bonus)
        {
            _healthBonus += bonus.Heath;
            _moveSpeedBonus +=  bonus.MoveSpeed;
            _damageBonus += bonus.Damage;
            _cooldownBonus += bonus.Cooldown;
            _areaBonus += bonus.Area;
            _durationBonus += bonus.Duration;
            _luckBonus += bonus.Luck;
            _experienceBonus += bonus.Experience;
            _projectileSpeedBonus += bonus.ProjectileSpeed;
            _pickUpRangeBonus += bonus.PickUpRange;
            _healthRegenBonus += bonus.HealthRegen;
            _projectileCountBonus += bonus.ProjectileCount;

            Recalculate();
            OnStatsChanged?.Invoke(); // Notify listeners that the stats changed
        }

        public void AddBonus(
            float health = 0,
            float moveSpeed = 0,
            float damage = 0,
            float cooldown = 0,
            float area = 0,
            float duration = 0,
            float luck = 0,
            float experience = 0,
            float healthRegen = 0,
            float projectileSpeed = 0,
            float pickUpRange = 0,
            int projectileCount = 0)
        {
            _healthBonus += health;
            _moveSpeedBonus += moveSpeed;
            _damageBonus += damage;
            _cooldownBonus += cooldown;
            _areaBonus += area;
            _durationBonus += duration;
            _luckBonus += luck;
            _experienceBonus += experience;
            _healthRegenBonus += healthRegen;
            _projectileSpeedBonus += projectileSpeed;
            _pickUpRangeBonus += pickUpRange;
            _projectileCountBonus += projectileCount;

            Recalculate();
            OnStatsChanged?.Invoke(); // Notify listeners that the stats changed
        }

        private void Recalculate()
        {
            float oldMaxHealth = MaxHealth; // Store old max health

            Level = BaseStats.Level + _level;
            TotalExpToNextLevel = CalculateExpToNextLevel(Level + 1);
            
            MaxHealth = BaseStats.MaxHealth * (1f + _healthBonus);
            MoveSpeed = BaseStats.MoveSpeed * (1f + _moveSpeedBonus);
            DamageBonus = BaseStats.Damage * (1f + _damageBonus);
            CooldownBonus = BaseStats.Cooldown * (1f + _cooldownBonus);
            AreaBonus = BaseStats.Area * (1f + _areaBonus);
            DurationBonus = BaseStats.Duration * (1f + _durationBonus);
            Luck = BaseStats.Luck * (1f + _luckBonus);
            ExperienceBonus = BaseStats.Experience * (1f + _experienceBonus);
            HealthRegen = BaseStats.HealthRegen + _healthRegenBonus;
            ProjectileSpeedBonus = BaseStats.ProjectileSpeed * (1f + _projectileSpeedBonus);
            ProjectileCountBonus = BaseStats.ProjectileCount + _projectileCountBonus;

            // Heal by the increased max health
            if (MaxHealth > oldMaxHealth)
            {
                CurrentHealth += MaxHealth - oldMaxHealth;
            }

            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            // Notify health listeners only if max health changed
            if (!Mathf.Approximately(oldMaxHealth, MaxHealth))
            {
                OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            }
        }
        

        public void AddExperience(float amount)
        {
            if (amount <= 0) return;

            CurrentExp += amount * ExperienceBonus;

            while (CurrentExp >= TotalExpToNextLevel)
            {
                CurrentExp -= TotalExpToNextLevel;
                LevelUp();
            }

            // Notify listeners that the experience changed
            OnExpChanged?.Invoke(CurrentExp, TotalExpToNextLevel);
        }

        private void LevelUp()
        {
            _level++;
            Recalculate();
            OnPlayerLevelUp?.Invoke(Level);
            Debug.Log($"[PlayerStats]: Level Up! New Level: {Level}. Current Stats: DamageBonus={DamageBonus}, HP={MaxHealth}, Speed={MoveSpeed}");
        }
        
        #endregion
        
        #region Health DamageBonus and Health

        public void ApplyDamage(float damage)
        {
            if (damage <= 0) return;
            
            
            CurrentHealth -= damage;
            
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            
            // Play sound
            SoundManager.Instance.Play("Hurt");
            PlayerController.Instance.PlayHurtAnimation();
            
            // Check for death
            if (CurrentHealth <= 0f)
            {
                CurrentHealth = 0f;
                Debug.Log("[PlayerStats]: Player death.");
                OnPlayerDeath?.Invoke();
            }
        }

        public void ApplyHeal(float heal)
        {
            if (heal <= 0) return;
            
            CurrentHealth += heal;
            CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
            
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }
        
        #endregion

        /// <summary>
        /// Calculates the experience required to reach the next level based on this formula from the
        /// Vampire Survivors game:
        /// - Level 1-19: (Level * 10) - 5
        /// - Level 20-39: (Level * 13) - 6
        /// - Level 40+: (Level * 16) - 8
        /// </summary>
        private float CalculateExpToNextLevel(int nextLevel)
        {
            if (nextLevel < 20)
                return (nextLevel * 8f) - 5f;
            
            if (nextLevel < 40)
                return (nextLevel * 10f) - 6f;
            
            return (nextLevel * 12f) - 8f;
        }
    }
}