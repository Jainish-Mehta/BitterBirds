using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalUI : MonoBehaviour
{
    private static GlobalUI instance;
    private Canvas globalCanvas;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void AutoSpawn()
    {
        GameObject prefab = Resources.Load<GameObject>("GlobalUI");
        if (prefab != null)
        {
            Instantiate(prefab);
        }
        else
        {
            Debug.LogWarning("Could not find a prefab named 'GlobalUI' in a Resources folder!");
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            globalCanvas = GetComponent<Canvas>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (globalCanvas != null)
        {
            string currentScene = scene.name;
            // Only show the UI if the scene is an actual gameplay level (e.g. "Level1", "Level2")
            // Make sure it hides on "MainMenu" and "LevelSelect"!

            if (currentScene.StartsWith("Level"))
            {
                globalCanvas.enabled = true; // Show the Pause/Restart buttons!
            }
            else
            {
                globalCanvas.enabled = false; // Hide them!
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}