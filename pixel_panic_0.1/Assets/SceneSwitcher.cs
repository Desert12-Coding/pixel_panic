using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // Needed for List<string>

public class SceneSwitcher : MonoBehaviour
{
    [Tooltip("List of scene names to cycle through")]
    public List<string> scenesToCycle = new List<string>(Lobby,COCKRING SCENE, test scene);
    
    [Tooltip("Time in seconds between scene switches")]
    public float switchInterval = 60f;
    
    private int currentSceneIndex = 0;
    
    private void Start()
    {
        // Start the scene switching coroutine
        StartCoroutine(SwitchScenesPeriodically());
    }
    
    private IEnumerator SwitchScenesPeriodically()
    {
        while (true)
        {
            // Wait for the specified interval
            yield return new WaitForSeconds(switchInterval);
            
            // Move to the next scene in the array
            currentSceneIndex = (currentSceneIndex + 1) % scenesToCycle.Count;
            
            // Load the next scene
            SceneManager.LoadScene(scenesToCycle[currentSceneIndex]);
        }
    }
}