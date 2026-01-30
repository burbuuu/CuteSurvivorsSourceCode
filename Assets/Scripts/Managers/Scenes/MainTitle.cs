using UnityEngine;

namespace Managers.Scenes
{
    public class MainTitle : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _titleCanvas;

        public void GoToMainMenu()
        {
            GameManager.Instance.TransitionToScene("MainMenu");
        }
        
    }
}