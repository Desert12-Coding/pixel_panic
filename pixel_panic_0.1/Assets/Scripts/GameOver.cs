    using UnityEngine;
    using UnityEngine.SceneManagement; // Required for scene management

    public class GameOver : MonoBehaviour
    {
        public string nextSceneName; // Assign the name of the next scene in the Inspector

        void OnTriggerEnter(Collider other)
        {
            // Check if the entering object is the player (e.g., by tag)
            if (other.CompareTag("Player")) 
            {
                SceneManager.LoadScene(nextSceneName); 
            }
        }
    }