using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utility
{
    public class StartMenu : MonoBehaviour
    {
        /// <summary>
        /// Loads the setup scene to start the game.
        /// TODO: change this to instructions screen when done
        /// </summary>
        public void StartGame()
        {
            SceneManager.LoadScene("SetupScene");
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}