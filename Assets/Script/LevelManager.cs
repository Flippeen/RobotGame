using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    public void GoToLevel(int levelNumber)
    {
        string levelName = "Level_" + levelNumber;
        SceneManager.LoadScene(levelName);
    }
    public void LevelCompleted()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
