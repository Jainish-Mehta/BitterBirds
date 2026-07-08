using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public CanvasGroup winScreenGroup;
    public CanvasGroup loseScreenGroup;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreTag;
    public int pointsPerUnusedBird = 10000;

    [Header("Star UI System")]
    public Image starLeft;
    public Image starCenter;
    public Image starRight;

    [Tooltip("What color should the star be when earned? (Usually Pure White)")]
    public Color earnedStarColor = Color.white;

    [Tooltip("What color should the star be when NOT earned? (Usually dark grey or black)")]
    public Color unearnedStarColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    public bool hasBirdLaunched = false;
    public bool isGameOver = false;
    public bool isWinScreenVisible = false;

    private int totalPigs = 0;
    private int currentScore = 0;
    private int maxLevelScore = 0;

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

        UpdateScoreDisplay();
        CalculateLevelMaxScore();
        UpdateStarUI(0);
    }

    private void CalculateLevelMaxScore()
    {
        maxLevelScore = 0;
        totalPigs = 0;

        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsInactive.Exclude);
        foreach (Enemy e in enemies)
        {
            totalPigs++;
            if (e.enemyData != null) maxLevelScore += e.enemyData.pointValue;
        }

        DestructibleBlock[] blocks = Object.FindObjectsByType<DestructibleBlock>(FindObjectsInactive.Exclude);
        foreach (DestructibleBlock b in blocks)
        {
            if (b.blockData != null) maxLevelScore += b.blockData.pointValue;
        }

        Debug.Log($"[GameManager] Level Started. Total Pigs: {totalPigs} | Max Destruction Score: {maxLevelScore}");
    }

    public void PigDestroyed(string deadEnemyName)
    {
        totalPigs--;

        if (totalPigs <= 0 && !isGameOver)
        {
            isGameOver = true;
            Debug.Log("[GameManager] All enemies defeated! Waiting for physics to settle...");

            // THE FIX: We removed the double-bonus from here! Just start the timer.
            Invoke(nameof(ShowWinScreen), 6f);
        }
    }

    private void CalculateFinalStars()
    {
        int starsEarned = 0;

        if (currentScore >= maxLevelScore * 0.8f) starsEarned = 3;
        else if (currentScore >= maxLevelScore * 0.5f) starsEarned = 2;
        else if (currentScore >= maxLevelScore * 0.3f) starsEarned = 1;

        UpdateStarUI(starsEarned);

        string currentLevelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        int oldHighScore = SaveSystem.GetLevelHighScore(currentLevelName);
        Debug.Log($"[GameManager] Old High Score for {currentLevelName}: {oldHighScore} | Current Score: {currentScore} | Stars Earned: {starsEarned}");

        if (currentScore > oldHighScore)
        {
            if (scoreTag != null)
            {
                scoreTag.text = "NEW HIGH SCORE!";
                scoreTag.color = Color.yellow;
            }
        }
        else
        {
            if (scoreTag != null)
            {
                scoreTag.text = "SCORE";
                scoreTag.color = Color.white;
            }
        }

        SaveSystem.SaveLevelProgress(currentLevelName, currentScore, starsEarned);
    }

    private void UpdateStarUI(int starsEarned)
    {
        // THE FIX: Left star needs 1, Center needs 2, Right needs 3!
        if (starLeft != null)
            starLeft.color = (starsEarned >= 1) ? earnedStarColor : unearnedStarColor;

        if (starCenter != null)
            starCenter.color = (starsEarned >= 2) ? earnedStarColor : unearnedStarColor;

        if (starRight != null)
            starRight.color = (starsEarned >= 3) ? earnedStarColor : unearnedStarColor;
    }

    public int GetTotalPigs() { return totalPigs; }

    public void AddScore(int pointsToAdd)
    {
        currentScore += pointsToAdd;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null) scoreText.text = currentScore.ToString();
    }

    private void ShowWinScreen()
    {
        // 1. ADD BIRD BONUS NOW (After all physics have settled!)
        BirdManager bManager = Object.FindAnyObjectByType<BirdManager>();
        if (bManager != null)
        {
            int leftoverBirds = bManager.GetRemainingBirdsCount();
            int bonusPoints = leftoverBirds * pointsPerUnusedBird;

            if (bonusPoints > 0)
            {
                Debug.Log($"[GameManager] Saved {leftoverBirds} birds! Adding {bonusPoints} bonus points!");
                AddScore(bonusPoints);
            }
        }

        // 2. CALCULATE STARS AND SAVE THE GAME
        CalculateFinalStars();

        // 3. SHOW THE UI
        isWinScreenVisible = true;

        if (winScreenGroup != null)
        {
            winScreenGroup.gameObject.SetActive(true);
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
            loseScreenGroup.gameObject.SetActive(true);
            loseScreenGroup.alpha = 1f;
            loseScreenGroup.interactable = true;
            loseScreenGroup.blocksRaycasts = true;

            if (AudioManager.Instance != null && AudioManager.Instance.levelLost != null)
                AudioManager.Instance.PlaySound(AudioManager.Instance.levelLost);
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