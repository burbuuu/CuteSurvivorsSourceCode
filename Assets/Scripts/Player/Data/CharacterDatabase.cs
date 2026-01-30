using UnityEngine;
using System.Collections.Generic;

namespace Player.Data
{
    // This class is a database that contains the scriptable object data (Character Data) of all the characters.
    public class CharacterDatabase : MonoBehaviour
    {
        [SerializeField] private List<CharacterData> characters;
        [SerializeField] private CharacterData defaultCharacter;

        private Dictionary<string, CharacterData> _byId;

        private void Awake()
        {
            _byId = new Dictionary<string, CharacterData>();
            foreach (var c in characters)
                _byId[c.StatsId] = c;
        }

        // Returns the character data by their id name
        public CharacterData GetCharacterData(string id)
        {
            if (_byId.TryGetValue(id, out var data))
                return data;
    
            Debug.LogError($"Character ID {id} not found in database!");
            return null;
        }

        public CharacterData GetDefaultCharacter()
        {
            return defaultCharacter;
        }
    }
}