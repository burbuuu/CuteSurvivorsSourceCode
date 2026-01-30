using System;
using UnityEngine;

namespace Player.Runtime
{
    public class PlayerStatsDebugView : MonoBehaviour
    {
        [Header("Runtime Stats (Read Only)")]
        [ContextMenuItem("Update Stats", "UpdateStats")]
        [SerializeField] private int level;
        [SerializeField] private float currentExp;
        [SerializeField] private float totalExpToNextLevel;
        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float damage;
        [SerializeField] private float cooldown;
        [SerializeField] private float area;
        [SerializeField] private float duration;
        [SerializeField] private float luck;
        [SerializeField] private float experience;
        [SerializeField] private float healthRegen;
        [SerializeField] private float pickUpRange;

        private PlayerController _player;

        private void OnEnable()
        {
            PlayerStats.OnStatsChanged += UpdateStats;
            PlayerStats.OnHealthChanged += UpdateHealth;
            PlayerStats.OnExpChanged += UpdateEXP;
        }

        private void OnDisable()
        {
            PlayerStats.OnStatsChanged -= UpdateStats;
            PlayerStats.OnHealthChanged -= UpdateHealth;
            PlayerStats.OnExpChanged -= UpdateEXP;
        }

        private void Start()
        {
            _player = PlayerController.Instance;
            
            if (_player != null && _player.Stats != null)
            {
                UpdateStats();
                UpdateHealth(_player.Stats.CurrentHealth, _player.Stats.MaxHealth);
                UpdateEXP(_player.Stats.CurrentExp, _player.Stats.TotalExpToNextLevel);
            }
        }

        private void UpdateStats()
        {
            if (_player == null || _player.Stats == null) return;

            level = _player.Stats.Level;
            maxHealth = _player.Stats.MaxHealth;
            moveSpeed = _player.Stats.MoveSpeed;
            damage = _player.Stats.DamageBonus;
            cooldown = _player.Stats.CooldownBonus;
            area = _player.Stats.AreaBonus;
            duration = _player.Stats.DurationBonus;
            luck = _player.Stats.Luck;
            experience = _player.Stats.ExperienceBonus;
            healthRegen = _player.Stats.HealthRegen;
            pickUpRange = _player.Stats.PickUpRange;
        }

        private void UpdateHealth(float current, float max)
        {
            currentHealth = current;
            maxHealth = max;
        }

        private void UpdateEXP(float current, float total)
        {
            currentExp = current;
            totalExpToNextLevel = total;
        }
    }
}