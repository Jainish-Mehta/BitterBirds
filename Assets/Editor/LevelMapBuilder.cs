using UnityEngine;
using UnityEditor;
using TMPro;

public class LevelMapBuilder : EditorWindow
{
    public GameObject buttonPrefab;

    // We don't save the Grid Parent to EditorPrefs because it's a Scene object, 
    // not a hard drive file. But we CAN try to auto-find it!
    public Transform gridParent;

    public int startLevel = 1;
    public int endLevel = 10;

    [MenuItem("BitterBirds/Level Map Builder")]
    public static void ShowWindow()
    {
        GetWindow<LevelMapBuilder>("Map Builder");
    }

    private void OnEnable()
    {
        // 1. Load the saved Button Prefab from your hard drive
        string prefabPath = EditorPrefs.GetString("MapBuilder_ButtonPrefab", "");
        if (!string.IsNullOrEmpty(prefabPath))
        {
            buttonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }

        // 2. Load the saved numbers
        startLevel = EditorPrefs.GetInt("MapBuilder_Start", 1);
        endLevel = EditorPrefs.GetInt("MapBuilder_End", 10);

        // 3. Try to auto-find the LevelGrid in the scene!
        GameObject gridObj = GameObject.Find("LevelGrid");
        if (gridObj != null)
        {
            gridParent = gridObj.transform;
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Map Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // --- THE PREFAB SAVING LOGIC ---
        EditorGUI.BeginChangeCheck();
        buttonPrefab = (GameObject)EditorGUILayout.ObjectField("Button Prefab", buttonPrefab, typeof(GameObject), false);
        if (EditorGUI.EndChangeCheck() && buttonPrefab != null)
        {
            // If you drag a new prefab in, save its path permanently!
            string path = AssetDatabase.GetAssetPath(buttonPrefab);
            EditorPrefs.SetString("MapBuilder_ButtonPrefab", path);
        }

        gridParent = (Transform)EditorGUILayout.ObjectField("Grid Parent", gridParent, typeof(Transform), true);

        GUILayout.Space(10);

        // --- THE NUMBER SAVING LOGIC ---
        EditorGUI.BeginChangeCheck();
        startLevel = EditorGUILayout.IntField("Start Level", startLevel);
        endLevel = EditorGUILayout.IntField("End Level", endLevel);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetInt("MapBuilder_Start", startLevel);
            EditorPrefs.SetInt("MapBuilder_End", endLevel);
        }

        GUILayout.Space(20);
        GUI.backgroundColor = Color.green;

        if (GUILayout.Button("GENERATE BUTTONS", GUILayout.Height(40)))
        {
            GenerateButtons();
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("CLEAR ALL BUTTONS", GUILayout.Height(30)))
        {
            ClearButtons();
        }
    }

    private void GenerateButtons()
    {
        if (buttonPrefab == null || gridParent == null)
        {
            Debug.LogError("Map Builder: You must assign the Button Prefab and the Grid Parent!");
            return;
        }

        if (startLevel > endLevel)
        {
            Debug.LogError("Map Builder: Start Level must be less than or equal to End Level!");
            return;
        }

        for (int i = startLevel; i <= endLevel; i++)
        {
            GameObject newBtn = (GameObject)PrefabUtility.InstantiatePrefab(buttonPrefab, gridParent);
            newBtn.name = "Level_" + i + "_Button";

            LevelButton script = newBtn.GetComponent<LevelButton>();
            if (script != null)
            {
                script.levelNumber = i;
            }

            TextMeshProUGUI btnText = newBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = i.ToString();
            }

            EditorUtility.SetDirty(newBtn);
        }

        Debug.Log($"<color=green>SUCCESS:</color> Generated levels {startLevel} to {endLevel}!");
    }

    private void ClearButtons()
    {
        if (gridParent == null) return;

        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(gridParent.GetChild(i).gameObject);
        }
    }
}