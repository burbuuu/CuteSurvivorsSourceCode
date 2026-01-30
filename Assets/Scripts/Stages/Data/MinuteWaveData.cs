using UnityEngine;
using System.Collections.Generic;

using Enemies.Data;

namespace Stages.Data
{
    // Class that contains regular spawn data
    [System.Serializable]
    public class RegularSpawnsData
    {
        public EnemyData enemyType;
        public int minSpawnsPerMinute;
        public int maxSpawnsPerMinute;
    }
    
    public enum SpawnPattern
    {
        Circle,
        Rectangle
    }
    
    // Class that contains event spawn data
    [System.Serializable]
    public class EventSpawnsData
    {
        public EnemyData enemyType;
        public SpawnPattern spawnPattern;
        public int spawnCount;
        public int minEventsPerMinute;
        public int maxEventsPerMinute;
    }
    
    // Class that contains the data of each minute of gameplay
    [System.Serializable]
    public class MinuteWaveData
    {
        public int minuteIndex;
        
        [Header("Regular Spawns data.")]
        public List<RegularSpawnsData> regularSpawns;
        
        [Header("Event Spawns")]
        public List<EventSpawnsData> eventSpawns;
    }
}