using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions; // Required for finding numbers in text!

public class Scene_Manager : MonoBehaviour
{
    private string mainMenuName = "BitterBirds";

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void ReloadCurrentLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void LoadNextLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 1. Extract ONLY the numbers from the current scene's name using Regex
        string numberString = Regex.Match(currentSceneName, @"\d+").Value;

        // 2. If it successfully found a number...
        if (!string.IsNullOrEmpty(numberString) && int.TryParse(numberString, out int currentNumber))
        {
            // 3. Do the math
            int nextNumber = currentNumber + 1;

            // 4. Build the exact name of the next scene (Make sure this matches your file names!)
            string nextLevelName = "Level" + nextNumber.ToString();

            // 5. Try to load it
            if (Application.CanStreamedLevelBeLoaded(nextLevelName))
            {
                Debug.Log("Loading next level: " + nextLevelName);
                SceneManager.LoadScene(nextLevelName);
            }
            else
            {
                Debug.Log("No more levels! Returning to Main Menu.");
                SceneManager.LoadScene(mainMenuName);
            }
        }
        else
        {
            // If the code couldn't find a number, just go back to the Main Menu
            Debug.Log("Could not find a number in the scene name. Returning to Main Menu.");
            SceneManager.LoadScene(mainMenuName);
        }
    }

    public void GoToMainMenu()
    {
        Debug.Log("Returning to Main Menu.");
        SceneManager.LoadScene(mainMenuName);
    }
}