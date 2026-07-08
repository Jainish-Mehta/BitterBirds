using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalUI : MonoBehaviour
{
    private static GlobalUI instance;
    private Canvas globalCanvas;

    // --- THIS IS THE MAGIC ---
    // This command tells Unity to run this code automatically BEFORE the first scene even loads!
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void AutoSpawn()
    {
        // It looks inside your "Resources" folder for a prefab named "GlobalUI" and spawns it automatically!
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
        // 1. Make sure only ONE of these ever exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // TELL UNITY NEVER TO DESTROY THIS WHEN CHANGING LEVELS!

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
        // Optional: Hide this UI if we are on the Main Menu (Index 0), but show it on all levels!
        if (globalCanvas != null)
        {
            globalCanvas.enabled = (scene.buildIndex != 0);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}