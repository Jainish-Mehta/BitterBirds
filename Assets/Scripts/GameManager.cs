using UnityEngine;
using TMPro; // Add this at the top to access TextMeshPro UI elements!

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public CanvasGroup winScreenGroup;
    public CanvasGroup loseScreenGroup;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText; // Drag your on-screen Score Text here in the Inspector!

    public bool hasBirdLaunched = false;
    public bool isGameOver = false;
    public bool isWinScreenVisible = false;

    private int totalPigs = 0;
    private int currentScore = 0; // The internal score counter

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        #if !UNITY_EDITOR
                    Debug.unityLogger.logEnabled = false;
        #endif
    }

    private void Start()
    {
        HideWinScreen();
        hasBirdLaunched = false;
        isGameOver = false;
        isWinScreenVisible = false;

        // Ensure the score text starts at 0
        UpdateScoreDisplay();

        // --- NEW: Find by TAG instead of by Script ---
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        totalPigs = enemies.Length;

        // LOG 1: See exactly how many enemies the game detects on load
        Debug.Log("[GameManager] Level Started. Total Enemies Found by Tag: " + totalPigs);
    }

    // --- NEW: Require the enemy to pass its name so we can log it! ---
    public void PigDestroyed(string deadEnemyName)
    {
        totalPigs--;

        // LOG 2: Watch the math count down exactly when it happens
        Debug.Log($"[GameManager] {deadEnemyName} died! Enemies remaining: {totalPigs}");

        if (totalPigs <= 0 && !isGameOver)
        {
            // LOG 3: Confirm the win condition was hit
            Debug.Log("[GameManager] All enemies defeated! Locking slingshot and starting 6-second win timer.");

            isGameOver = true;
            Invoke(nameof(ShowWinScreen), 6f);
        }
    }

    // --- NEW METHOD TO ADD POINTS ---
    public void AddScore(int pointsToAdd)
    {
        currentScore += pointsToAdd;
        UpdateScoreDisplay();
    }

    public int GetTotalPigs()
    {
        return totalPigs;
    }

    // --- NEW METHOD TO UPDATE UI ---
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    

    private void ShowWinScreen()
    {
        isWinScreenVisible = true; // 2. Tell the physics objects to be quiet!

        if (winScreenGroup != null)
        {
            winScreenGroup.alpha = 1f;
            winScreenGroup.interactable = true;
            winScreenGroup.blocksRaycasts = true;

            if (AudioManager.Instance != null && AudioManager.Instance.levelWon != null)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.levelWon);
            }
        }
    }

    public void ShowLoseScreen()
    {
        isGameOver = true;

        if (loseScreenGroup != null)
        {
            loseScreenGroup.alpha = 1f;
            loseScreenGroup.interactable = true;
            loseScreenGroup.blocksRaycasts = true;

            if (AudioManager.Instance != null && AudioManager.Instance.levelLost != null)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.levelLost);
            }
        }
    }

    private void HideWinScreen()
    {
        if (winScreenGroup != null)
        {
            winScreenGroup.alpha = 0f;
            winScreenGroup.interactable = false;
            winScreenGroup.blocksRaycasts = false;
        }

        if (loseScreenGroup != null)
        {
            loseScreenGroup.alpha = 0f;
            loseScreenGroup.interactable = false;
            loseScreenGroup.blocksRaycasts = false;
        }
    }
}