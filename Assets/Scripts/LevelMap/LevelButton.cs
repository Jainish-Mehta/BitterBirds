using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("Level Info")]
    public int levelNumber = 1;

    [Header("UI References")]
    public TextMeshProUGUI levelText;
    public GameObject lockIcon;
    public GameObject starContainer;

    public Image starLeft;
    public Image starCenter;
    public Image starRight;

    [Header("Star Colors")]
    public Color earnedStarColor = Color.white;
    public Color unearnedStarColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();

        if (levelText != null) levelText.text = levelNumber.ToString();

        bool isUnlocked = true;

        if (levelNumber > 1)
        {
            string previousLevelName = "Level" + (levelNumber - 1);
            int previousScore = SaveSystem.GetLevelHighScore(previousLevelName);

            if (previousScore <= 0)
                isUnlocked = false;
        }

        if (isUnlocked)
        {
            if (lockIcon != null) lockIcon.SetActive(false);
            if (starContainer != null) starContainer.SetActive(true);
            button.interactable = true;

            string thisLevelName = "Level" + levelNumber;
            int starsEarned = SaveSystem.GetLevelStars(thisLevelName);

            // --- THE FIX: Fill them strictly from left to right based on the number! ---
            if (starLeft != null) starLeft.color = (starsEarned >= 1) ? earnedStarColor : unearnedStarColor;
            if (starCenter != null) starCenter.color = (starsEarned >= 2) ? earnedStarColor : unearnedStarColor;
            if (starRight != null) starRight.color = (starsEarned >= 3) ? earnedStarColor : unearnedStarColor;
        }
        else
        {
            if (lockIcon != null) lockIcon.SetActive(true);
            if (starContainer != null) starContainer.SetActive(false);
            button.interactable = false;
        }
    }

    public void ClickLoadLevel()
    {
        string levelName = "Level" + levelNumber;
        SceneManager.LoadScene(levelName);
    }
}