using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RandomSceneChanger : MonoBehaviour
{
    [Tooltip("List of scene names to choose from")]
    public List<string> sceneNames = new List<string>(5);
    
    [Tooltip("Time in seconds before changing scene")]
    public float changeInterval = 60f;
    
    private float timer = 0f;

    void Update()
    {
        // Increment the timer
        timer += Time.deltaTime;
        
        // Check if 60 seconds have passed
        if (timer >= changeInterval)
        {
            ChangeToRandomScene();
        }
    }
    
    void ChangeToRandomScene()
    {
        // Check if we have any scenes in the list
        if (sceneNames.Count == 0)
        {
            Debug.LogWarning("No scenes assigned to RandomSceneChanger!");
            return;
        }
        
        // Get a random index
        int randomIndex = Random.Range(0, sceneNames.Count);
        
        // Get the random scene name
        string sceneToLoad = sceneNames[randomIndex];
        
        // Load the scene
        SceneManager.LoadScene(sceneToLoad);
    }
}