using UnityEngine;
using System.Collections.Generic;
namespace Stages.Data
{
    [CreateAssetMenu(menuName = "Game/Stage Data")]
    public class StageData : ScriptableObject
    {
        public string stageName;
        public int StageDurationMinutes => minuteWaves?.Count ?? 0;
        
        public List<MinuteWaveData> minuteWaves;
    }
}