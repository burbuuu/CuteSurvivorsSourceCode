using UnityEngine;

namespace Player.Data
{
    [System.Serializable]
    public class PlayerBonusStats
    {
        [Header("Bonifications:")] [Tooltip("0.2 = +20%")]
        public float Heath = 0f;
        public float MoveSpeed = 0f;
        public float Damage = 0f;
        public float Cooldown = 0f;
        public float Area = 0f;
        public float Duration = 0f;
        public float Luck = 0f;
        public float Experience = 0f;
        public float ProjectileSpeed = 0f;
        public float PickUpRange = 0f;

        [Header("Flat values")] 
        [Tooltip("HP recovered per second")]
        public float HealthRegen = 0f;
        public int ProjectileCount = 0;
    }

    // This scriptable object represents the stats for characters or items
    public abstract class StatsData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string statsId;
        public string StatsId => statsId;
        
        public PlayerBonusStats stats;
    }
}