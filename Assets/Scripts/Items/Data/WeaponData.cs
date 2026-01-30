using System.Collections.Generic;
using UnityEngine;

namespace Items.Data
{
    [System.Serializable]
    public class WeaponBaseStats
    {
        [SerializeField] private float baseDamage;
        [SerializeField] private float baseSize;
        [SerializeField] private float baseCooldown;
        [SerializeField] private float baseProjectileSpeed;
        [SerializeField] private float baseDuration;
        [SerializeField] private int baseProjectileCount;

        public float BaseDamage => baseDamage;
        public float BaseSize => baseSize;
        public float BaseCooldown => baseCooldown;
        public float BaseProjectileSpeed => baseProjectileSpeed;
        public float BaseDuration => baseDuration;
        public int BaseProjectileCount => baseProjectileCount;

        public WeaponBaseStats(float damage, float size, float cooldown, float projectileSpeed, float duration, int projectileCount)
        {
            baseDamage = damage;
            baseSize = size;
            baseCooldown = cooldown;
            baseProjectileSpeed = projectileSpeed;
            baseDuration = duration;
            baseProjectileCount = projectileCount;
        }
        
        // Parameterless constructor for serialization
        public WeaponBaseStats() {}
    }
    
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Items/New Weapon")]
    public class WeaponData : ItemData
    {
        [Header("Weapon settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private ShootPatternType shootPattern;

        [Header("Weapon base stats")]
        [SerializeField] private List<WeaponBaseStats> weaponBaseStats = new List<WeaponBaseStats>();
        
        public GameObject ProjectilePrefab => projectilePrefab;
        public ShootPatternType ShootPattern => shootPattern;

        public WeaponBaseStats GetWeaponBaseStatsForLevel(int level)
        {
            if (weaponBaseStats.Count == 0) return null;
            
            // Level is 1-based, a list is 0-based
            int index = Mathf.Clamp(level - 1, 0, MaxLevel - 1);
            return weaponBaseStats[index];
        }

        public override int MaxLevel => weaponBaseStats.Count;
    }
}
