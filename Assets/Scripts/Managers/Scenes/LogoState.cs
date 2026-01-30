using System.Collections;
using UnityEngine;

namespace Managers.Scenes
{

    public class LogoState : MonoBehaviour
    {

        [SerializeField] private float logoDisplayTimeSeconds = 4f;
        private Coroutine _exitCoroutine;
        
        
        private void Start()
        {
            _exitCoroutine = StartCoroutine(ExitLogo());
        }
        #region IState Implementation
        

        private IEnumerator ExitLogo()
        {
            yield return new WaitForSeconds(logoDisplayTimeSeconds);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.TransitionToScene("MainTitle");
            }

            yield return null;
        }
        #endregion
        
    }
}