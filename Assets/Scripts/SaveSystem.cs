using UnityEngine;

// Notice there is no "MonoBehaviour"! This is a pure static utility class.
public static class SaveSystem
{
    // Call this when the player beats a level
    public static void SaveLevelProgress(string levelName, int score, int stars)
    {
        // 1. Check the previous high score. Only overwrite if the new score is HIGHER!
        int currentHighScore = GetLevelHighScore(levelName);
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt(levelName + "_HighScore", score);
            Debug.Log($"[SaveSystem] New High Score for {levelName}: {score}!");
        }

        // 2. Check the previous stars. Only overwrite if they earned MORE stars!
        int currentStars = GetLevelStars(levelName);
        if (stars > currentStars)
        {
            PlayerPrefs.SetInt(levelName + "_Stars", stars);
            Debug.Log($"[SaveSystem] New Star Record for {levelName}: {stars}!");
        }

        // Force Unity to write the data to the hard drive immediately
        PlayerPrefs.Save();
    }

    // Call this to get the High Score (Returns 0 if they haven't played yet)
    public static int GetLevelHighScore(string levelName)
    {
        return PlayerPrefs.GetInt(levelName + "_HighScore", 0);
    }

    // Call this to get the Stars (Returns 0 if they haven't played yet)
    public static int GetLevelStars(string levelName)
    {
        return PlayerPrefs.GetInt(levelName + "_Stars", 0);
    }
}