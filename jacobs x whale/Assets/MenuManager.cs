using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        // Load the first level (replace "Level1" with your first level scene name)
        SceneManager.LoadScene("Level1");
    }

    public void OpenLevelSelector()
    {
        // Load the level selector scene
        SceneManager.LoadScene("LevelSelector");
    }

    public void QuitGame()
    {
        // Quit the application
        Debug.Log("Quit Game"); // Works only in builds
        Application.Quit();
    }

    public void LoadLevel(string levelName)
    {
        // Load a specific level by name
        SceneManager.LoadScene(levelName);
    }

    public void BackToMainMenu()
    {
        // Return to the main menu
        SceneManager.LoadScene("MainMenu");
    }
}
