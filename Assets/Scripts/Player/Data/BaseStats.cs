namespace Player.Data
{
    // This static class represents the base data for the player
    public static class BaseStats
    {
        // Stats by Value
        public const int Level = 1;
        public const float MaxHealth = 100;
        public const float MoveSpeed = 2f;
        public const float HealthRegen = 0; // Per second

        // Stats by multiplier 
        public const float Damage = 1f;
        public const float Cooldown = 1f;
        public const float Area = 1f;
        public const float Duration = 1f;
        public const float Experience = 1f;

        // By value
        public const float PickUpRange = 1f; // Radius in world units
        public const float Luck = 1f;
        public const int ProjectileCount = 0;
        public const float ProjectileSpeed = 1f; // World units per second
    }
}
