using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class CreditsUI : MonoBehaviour
    {
        [SerializeField] private Button _toMainMenuButton;

        void Awake()
        {
            if (_toMainMenuButton == null) Debug.LogError("No button found in CreditsUI.");
            _toMainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }

        private void ReturnToMainMenu()
        {
            GameManager.Instance.TransitionToScene("MainMenu");
        }
    }
}