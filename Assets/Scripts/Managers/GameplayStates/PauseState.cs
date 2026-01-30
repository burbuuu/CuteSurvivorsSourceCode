using Managers.StateMachine;
using UnityEngine;

public class PauseState : MonoBehaviour, IState
{
    [Header("Pause menu")]
    [SerializeField] private CanvasGroup pauseMenu;

    private void OnValidate()
    {
        if (pauseMenu == null)
        {
            Debug.LogWarning($"[PauseManager] {gameObject.name} is missing the PauseMenu reference!",this);
            return;
        }
    }
    

    #region IState implementation
    
    public void Enter()
    {
        // Freeze the game clock and active the menu
        Time.timeScale = 0;

        // Set up the pause cavas
        pauseMenu.gameObject.SetActive(true);
        pauseMenu.alpha = 1f;
        pauseMenu.blocksRaycasts = true;
        pauseMenu.interactable = true;

        // Switch to UI input action map
        GameManager.Instance.SwitchToUIActionMap();
    }

    public void Exit()
    {
        // Resume the clock and close the menu
        Time.timeScale = 1f;
        
        pauseMenu.gameObject.SetActive(false);
    }

    #endregion
  
    
}