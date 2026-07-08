using UnityEngine;
using UnityEngine.UI; // <-- REQUIRED for changing Image sprites!
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class Scene_Manager : MonoBehaviour
{
    private string mainMenuName = "BitterBirds";
    private bool isPaused = false;
    private CanvasGroup pauseCanvasGroup;

    [Header("Pause Button UI")]
    public Image pauseButtonImage; // Drag your corner Pause Button's Image component here!
    public Sprite pauseIcon;       // The "||" icon
    public Sprite playIcon;        // The "Play" icon

    private void Start()
    {
        GameObject panelObj = GameObject.Find("PausePanel");
        if (panelObj != null)
        {
            pauseCanvasGroup = panelObj.GetComponent<CanvasGroup>();
            HidePausePanel();
        }

        // Make sure the button starts with the Pause icon
        if (pauseButtonImage != null && pauseIcon != null)
        {
            pauseButtonImage.sprite = pauseIcon;
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            HidePausePanel();

            // Swap icon back to Pause!
            if (pauseButtonImage != null && pauseIcon != null)
                pauseButtonImage.sprite = pauseIcon;
        }
        else
        {
            isPaused = true;
            Time.timeScale = 0f;
            ShowPausePanel();

            // Swap icon to Play!
            if (pauseButtonImage != null && playIcon != null)
                pauseButtonImage.sprite = playIcon;
        }
    }

    private void ShowPausePanel()
    {
        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.alpha = 1f;
            pauseCanvasGroup.interactable = true;
            pauseCanvasGroup.blocksRaycasts = true;
        }
    }

    private void HidePausePanel()
    {
        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.alpha = 0f;
            pauseCanvasGroup.interactable = false;
            pauseCanvasGroup.blocksRaycasts = false;
        }
    }

    // --- SCENE LOADING LOGIC ---
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }

    public void ReloadCurrentLevel()
    {
        Time.timeScale = 1f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        string numberString = Regex.Match(currentSceneName, @"\d+").Value;

        if (!string.IsNullOrEmpty(numberString) && int.TryParse(numberString, out int currentNumber))
        {
            int nextNumber = currentNumber + 1;
            string nextLevelName = "Level" + nextNumber.ToString();

            if (Application.CanStreamedLevelBeLoaded(nextLevelName))
                SceneManager.LoadScene(nextLevelName);
            else
                SceneManager.LoadScene("CommingSoon");
        }
        else
        {
            SceneManager.LoadScene("CommingSoon");
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuName);
    }
}