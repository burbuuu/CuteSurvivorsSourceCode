using Items;
using UnityEngine;

namespace Player.Data
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Character Data")]
    public class CharacterData : StatsData
    {
        [Header("Character Visuals")]
        [SerializeField] private RuntimeAnimatorController animatorController;
        [SerializeField] private Sprite miniature;
        [SerializeField] private Sprite gameplaySprite;
        [SerializeField] public GameObject weaponPrefab;
        

        public RuntimeAnimatorController AnimatorController => animatorController;
        public Sprite Miniature => miniature;
        public Sprite GameplaySprite => gameplaySprite;
    }
}