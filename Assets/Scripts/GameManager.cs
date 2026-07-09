using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public CanvasGroup winScreenGroup;
    public CanvasGroup loseScreenGroup;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreTag;

    [Header("Star UI System")]
    public Image starLeft;
    public Image starCenter;
    public Image starRight;
    public Color earnedStarColor = Color.white;
    public Color unearnedStarColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    [Header("Spatial Batching")]
    public float scoreBatchRadius = 8f;

    [Header("Bird Bonus")]
    public int pointsPerUnusedBird = 10000;

    public bool hasBirdLaunched = false;
    public bool isGameOver = false;
    public bool isWinScreenVisible = false;

    private int totalPigs = 0;
    private int currentScore = 0;
    private int maxLevelScore = 0;
    public int currentBirdMultiplier = 1;

    private List<FloatingScore> activeScoreTexts = new List<FloatingScore>();

    // --- NEW: SCORE TRACKING FOR THE RECEIPT ---
    // Tracks: "Object Name (Multiplier x)" -> Total Points Earned
    private Dictionary<string, int> scoreReceipt = new Dictionary<string, int>();
    // Tracks: "Object Name (Multiplier x)" -> How many times it was destroyed
    private Dictionary<string, int> destroyCountReceipt = new Dictionary<string, int>();
    private int totalEnemiesDestroyed = 0;
    private int totalObjectsDestroyed = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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

        scoreReceipt.Clear();
        destroyCountReceipt.Clear();
    }

    public void SetCurrentBirdMultiplier(int multiplier)
    {
        currentBirdMultiplier = Mathf.Max(1, multiplier);
        Debug.Log($"[GameManager] Bird Multiplier set to: {currentBirdMultiplier}x");
    }

    // --- UPDATED: RECORD THE SCORE RECEIPT ---
    public void RegisterScoreHit(string objectName, bool isEnemy, int basePoints, Vector3 position, Color textColor)
    {
        int finalPoints = basePoints * currentBirdMultiplier;

        // 1. Log it for the Receipt!
        string receiptKey = $"{objectName} with bird multiplier = {currentBirdMultiplier}";

        if (scoreReceipt.ContainsKey(receiptKey))
        {
            scoreReceipt[receiptKey] += finalPoints;
            destroyCountReceipt[receiptKey]++;
        }
        else
        {
            scoreReceipt[receiptKey] = finalPoints;
            destroyCountReceipt[receiptKey] = 1;
        }

        if (isEnemy) totalEnemiesDestroyed++;
        else totalObjectsDestroyed++;

        // 2. Add to total score
        currentScore += finalPoints;
        UpdateScoreDisplay();

        // 3. Floating Text Logic
        activeScoreTexts.RemoveAll(item => item == null);
        FloatingScore nearbyAbsorber = null;
        foreach (FloatingScore fs in activeScoreTexts)
        {
            if (fs.isAbsorbing && Vector2.Distance(fs.transform.position, position) <= scoreBatchRadius)
            {
                nearbyAbsorber = fs;
                break;
            }
        }

        if (nearbyAbsorber != null) nearbyAbsorber.AddPoints(finalPoints, textColor);
        else SpawnNewFloatingText(finalPoints, position, textColor);
    }

    private void SpawnNewFloatingText(int points, Vector3 position, Color color)
    {
        GameObject scorePrefab = Resources.Load<GameObject>("FloatingScorePrefab");
        if (scorePrefab != null)
        {
            GameObject newTextObj = Instantiate(scorePrefab, position, Quaternion.identity);
            FloatingScore fsScript = newTextObj.GetComponent<FloatingScore>();
            if (fsScript != null)
            {
                fsScript.Setup(points, color);
                activeScoreTexts.Add(fsScript);
            }
        }
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
    }

    public void PigDestroyed(string deadEnemyName)
    {
        totalPigs--;
        if (totalPigs <= 0 && !isGameOver)
        {
            isGameOver = true;
            Invoke(nameof(ShowWinScreen), 6f);
        }
    }

    // --- UPDATED: PRINTS THE FULL RECEIPT INCLUDING BIRDS AND THRESHOLDS ---
    private void PrintScoreReceipt(int birdBonusPoints)
    {
        Debug.Log("======================================");
        Debug.Log("          FINAL SCORE RECEIPT         ");
        Debug.Log("======================================");

        foreach (var item in scoreReceipt)
        {
            string key = item.Key;
            int count = destroyCountReceipt[key];
            int totalPoints = item.Value;

            int multiplier = int.Parse(key.Substring(key.LastIndexOf("=") + 2));
            int baseScore = (totalPoints / count) / multiplier;

            Debug.Log($"- {key.Replace(" with", $" destroyed x {count} : {baseScore * count} with")}");
        }

        Debug.Log("--------------------------------------");
        Debug.Log($"Total {totalEnemiesDestroyed} enemies and {totalObjectsDestroyed} objects destroyed.");

        // --- NEW: Print the Bird Bonus! ---
        if (birdBonusPoints > 0)
        {
            Debug.Log($"- Unused Bird Bonus: +{birdBonusPoints} points");
        }

        Debug.Log("--------------------------------------");
        Debug.Log($"GRAND TOTAL SCORE: {currentScore}");

        // --- NEW: Print the Max Threshold Comparison! ---
        Debug.Log($"Base Level Maximum (1x Multiplier): {maxLevelScore}");
        if (currentScore > maxLevelScore)
        {
            Debug.Log($"<color=green>You beat the base maximum by {currentScore - maxLevelScore} points!</color>");
        }

        Debug.Log("======================================");
    }

    private void ShowWinScreen()
    {
        int birdBonusPoints = 0;

        // 1. Calculate and Add Bird Bonus
        BirdManager bManager = Object.FindAnyObjectByType<BirdManager>();
        if (bManager != null)
        {
            int leftoverBirds = bManager.GetRemainingBirdsCount();

            // Note: If you want to use your unusedBirdMultiplier logic, update the math here!
            // I am using the standard flat points (10k) for this receipt.
            birdBonusPoints = leftoverBirds * pointsPerUnusedBird;

            if (birdBonusPoints > 0)
            {
                currentScore += birdBonusPoints;
                UpdateScoreDisplay();
                SpawnNewFloatingText(birdBonusPoints, Camera.main.transform.position + new Vector3(0, 2, 10), Color.yellow);
            }
        }

        // 2. Print the receipt! (Pass the bird bonus to it so it can display it)
        PrintScoreReceipt(birdBonusPoints);

        // 3. Calculate Stars and Save
        CalculateFinalStars();

        // 4. Show the UI
        isWinScreenVisible = true;

        if (winScreenGroup != null)
        {
            winScreenGroup.gameObject.SetActive(true);
            winScreenGroup.alpha = 1f;
            winScreenGroup.interactable = true;
            winScreenGroup.blocksRaycasts = true;

            if (AudioManager.Instance != null && AudioManager.Instance.levelWon != null)
                AudioManager.Instance.PlaySound(AudioManager.Instance.levelWon);
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
        if (starLeft != null) starLeft.color = (starsEarned >= 1) ? earnedStarColor : unearnedStarColor;
        if (starCenter != null) starCenter.color = (starsEarned >= 2) ? earnedStarColor : unearnedStarColor;
        if (starRight != null) starRight.color = (starsEarned >= 3) ? earnedStarColor : unearnedStarColor;
    }

    public int GetTotalPigs() { return totalPigs; }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null) scoreText.text = currentScore.ToString();
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