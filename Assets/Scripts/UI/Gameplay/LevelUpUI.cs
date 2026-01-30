using System.Collections.Generic;
using LevelUpSystem;
using Items;
using UnityEngine;

namespace UI.Gameplay
{
    public class LevelUpUI : MonoBehaviour
    {
        [SerializeField] private GameObject _uiRoot;
        [SerializeField] private Inventory _inventory;
        [SerializeField] private GameObject _choicePrefab;

        [SerializeField] List<GameObject> _choices = new List<GameObject>();
        
        public event System.Action OnChoiceProcessed;

        public void ShowChoices(List<ILevelUpChoice> choices)
        {
            DestroyChoices();
            
            _uiRoot.SetActive(true);
            foreach (var choice in choices)
            {
                // Instantiate choice
                GameObject choiceObject = Instantiate(_choicePrefab, _uiRoot.transform);
                _choices.Add(choiceObject);
                
                // Setup view component
                var view = choiceObject.GetComponent<LevelUpChoiceView>();
                if (view == null)
                {
                    Debug.LogError($"Choice prefab {choiceObject.name} does not have a LevelUpChoiceView component!");
                    continue;
                }
                
                view.Setup(choice,this);
            }
            Debug.Log($"Showing {choices.Count} choices");
        }

        private void DestroyChoices()
        {
            foreach (var choice in _choices)
            {
                Destroy(choice);
            }
            _choices.Clear();
        }

        public void OnChoiceSelected(ILevelUpChoice choice)
        {
            choice.Apply(_inventory);
            DestroyChoices();
            _uiRoot.SetActive(false);
            OnChoiceProcessed?.Invoke();
        }
    }
}