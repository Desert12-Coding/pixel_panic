using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Options : MonoBehaviour
{
    public void OnBackButton()
    {
        SceneManager.LoadScene(0);
    }
}