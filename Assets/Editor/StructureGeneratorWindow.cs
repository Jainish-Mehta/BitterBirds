using UnityEngine;
using UnityEditor;
using System.IO;

public class StructureGeneratorWindow : EditorWindow
{
    // Wood
    public GameObject w_SolidCrate, w_FrameCrate, w_PlankCrate, w_Plank, w_Pillar, w_Triangle, w_Boulder, w_Small;

    // Stone
    public GameObject s_Crate, s_Plank, s_Pillar, s_Triangle, s_Boulder, s_Small;

    // Ice
    public GameObject i_Crate, i_Plank, i_Pillar, i_Triangle, i_Boulder, i_Small;

    // Extras
    public GameObject tnt;
    public GameObject[] enemyPrefabs = new GameObject[0];

    Vector2 scrollPos;

    [MenuItem("BitterBirds/Ultimate Structure Builder")]
    public static void ShowWindow()
    {
        GetWindow<StructureGeneratorWindow>("Structure Builder");
    }

    // --- MAGIC SAVING TRICK: Runs exactly when the window opens ---
    private void OnEnable()
    {
        w_SolidCrate = LoadSavedPrefab("w_SolidCrate");
        w_FrameCrate = LoadSavedPrefab("w_FrameCrate");
        w_PlankCrate = LoadSavedPrefab("w_PlankCrate");
        w_Plank = LoadSavedPrefab("w_Plank");
        w_Pillar = LoadSavedPrefab("w_Pillar");
        w_Triangle = LoadSavedPrefab("w_Triangle");
        w_Boulder = LoadSavedPrefab("w_Boulder");
        w_Small = LoadSavedPrefab("w_Small");

        s_Crate = LoadSavedPrefab("s_Crate");
        s_Plank = LoadSavedPrefab("s_Plank");
        s_Pillar = LoadSavedPrefab("s_Pillar");
        s_Triangle = LoadSavedPrefab("s_Triangle");
        s_Boulder = LoadSavedPrefab("s_Boulder");
        s_Small = LoadSavedPrefab("s_Small");

        i_Crate = LoadSavedPrefab("i_Crate");
        i_Plank = LoadSavedPrefab("i_Plank");
        i_Pillar = LoadSavedPrefab("i_Pillar");
        i_Triangle = LoadSavedPrefab("i_Triangle");
        i_Boulder = LoadSavedPrefab("i_Boulder");
        i_Small = LoadSavedPrefab("i_Small");

        tnt = LoadSavedPrefab("tnt");

        // Load the enemy array
        int enemyCount = EditorPrefs.GetInt("enemyPrefabs_Count", 0);
        enemyPrefabs = new GameObject[enemyCount];
        for (int i = 0; i < enemyCount; i++)
        {
            enemyPrefabs[i] = LoadSavedPrefab("enemyPrefabs_" + i);
        }
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("ASSIGN PREFABS (They save automatically!)", EditorStyles.boldLabel);

        // We use a custom helper method to draw the box AND save it if you change it!
        GUILayout.Label("--- WOOD ---", EditorStyles.boldLabel);
        w_SolidCrate = DrawAndSaveSlot("Solid Crate", w_SolidCrate, "w_SolidCrate");
        w_FrameCrate = DrawAndSaveSlot("Frame Crate", w_FrameCrate, "w_FrameCrate");
        w_PlankCrate = DrawAndSaveSlot("Plank Crate", w_PlankCrate, "w_PlankCrate");
        w_Plank = DrawAndSaveSlot("Plank (3x1)", w_Plank, "w_Plank");
        w_Pillar = DrawAndSaveSlot("Pillar (1x3)", w_Pillar, "w_Pillar");
        w_Triangle = DrawAndSaveSlot("Triangle", w_Triangle, "w_Triangle");
        w_Boulder = DrawAndSaveSlot("Log / Boulder", w_Boulder, "w_Boulder");
        w_Small = DrawAndSaveSlot("Small Block", w_Small, "w_Small");

        GUILayout.Space(5); GUILayout.Label("--- STONE ---", EditorStyles.boldLabel);
        s_Crate = DrawAndSaveSlot("Stone Crate", s_Crate, "s_Crate");
        s_Plank = DrawAndSaveSlot("Stone Plank", s_Plank, "s_Plank");
        s_Pillar = DrawAndSaveSlot("Stone Pillar", s_Pillar, "s_Pillar");
        s_Triangle = DrawAndSaveSlot("Stone Triangle", s_Triangle, "s_Triangle");
        s_Boulder = DrawAndSaveSlot("Stone Boulder", s_Boulder, "s_Boulder");
        s_Small = DrawAndSaveSlot("Small Stone", s_Small, "s_Small");

        GUILayout.Space(5); GUILayout.Label("--- ICE ---", EditorStyles.boldLabel);
        i_Crate = DrawAndSaveSlot("Ice Crate", i_Crate, "i_Crate");
        i_Plank = DrawAndSaveSlot("Ice Plank", i_Plank, "i_Plank");
        i_Pillar = DrawAndSaveSlot("Ice Pillar", i_Pillar, "i_Pillar");
        i_Triangle = DrawAndSaveSlot("Ice Triangle", i_Triangle, "i_Triangle");
        i_Boulder = DrawAndSaveSlot("Ice Boulder", i_Boulder, "i_Boulder");
        i_Small = DrawAndSaveSlot("Small Ice", i_Small, "i_Small");

        GUILayout.Space(5); GUILayout.Label("--- HAZARDS & ENEMIES ---", EditorStyles.boldLabel);
        tnt = DrawAndSaveSlot("TNT Block", tnt, "tnt");

        // Special handling for the Enemy Array
        EditorGUI.BeginChangeCheck();
        SerializedObject so = new SerializedObject(this);
        SerializedProperty enemiesProperty = so.FindProperty("enemyPrefabs");
        EditorGUILayout.PropertyField(enemiesProperty, new GUIContent("Enemy Pig Array"), true);
        if (EditorGUI.EndChangeCheck())
        {
            so.ApplyModifiedProperties();
            // Save the array if it was changed
            EditorPrefs.SetInt("enemyPrefabs_Count", enemyPrefabs.Length);
            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                SavePrefabPath("enemyPrefabs_" + i, enemyPrefabs[i]);
            }
        }

        GUILayout.Space(20);

        if (GUILayout.Button("GENERATE EPIC STRUCTURES", GUILayout.Height(40)))
        {
            GenerateStructures();
        }

        EditorGUILayout.EndScrollView();
    }

    // ==========================================
    // THE SAVING HELPERS
    // ==========================================

    // This draws the Inspector box and instantly saves whatever you drag into it
    private GameObject DrawAndSaveSlot(string label, GameObject currentObj, string saveKey)
    {
        GameObject newObj = (GameObject)EditorGUILayout.ObjectField(label, currentObj, typeof(GameObject), false);

        if (newObj != currentObj) // If you dragged a new item in...
        {
            SavePrefabPath(saveKey, newObj);
        }
        return newObj;
    }

    private void SavePrefabPath(string key, GameObject obj)
    {
        if (obj != null)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            EditorPrefs.SetString(key, path); // Memorizes the hard drive path
        }
        else
        {
            EditorPrefs.DeleteKey(key); // If you delete it, forget the path
        }
    }

    private GameObject LoadSavedPrefab(string key)
    {
        if (EditorPrefs.HasKey(key))
        {
            string path = EditorPrefs.GetString(key);
            return AssetDatabase.LoadAssetAtPath<GameObject>(path); // Fetches it from the hard drive!
        }
        return null;
    }

    // ==========================================
    // STRUCTURE BUILDING LOGIC (Unchanged)
    // ==========================================
    private void GenerateStructures()
    {
        string folder = "Assets/Resources/EpicStructures";
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        SaveStructure(BuildRollingCart(), $"{folder}/1_TheRollingCart.prefab");
        SaveStructure(BuildCathedral(), $"{folder}/2_StoneCathedral.prefab");
        SaveStructure(BuildIcePyramid(), $"{folder}/3_GlassPyramid.prefab");
        SaveStructure(BuildScaffoldingTower(), $"{folder}/4_ScaffoldingTower.prefab");

        AssetDatabase.Refresh();
        Debug.Log("<color=green>SUCCESS:</color> Epic Structures generated!");
    }

    private void SaveStructure(GameObject structure, string path)
    {
        if (structure == null) return;
        PrefabUtility.SaveAsPrefabAsset(structure, path);
        DestroyImmediate(structure);
    }

    // ==========================================
    // DESIGN 1: THE ROLLING CART
    // ==========================================
    private GameObject BuildRollingCart()
    {
        GameObject parent = new GameObject("RollingCart");

        // Wheels widened slightly for stability
        Spawn(s_Boulder, -1.2f, 0.5f, parent);
        Spawn(s_Boulder, 1.2f, 0.5f, parent);

        Spawn(w_Plank, 0f, 1.5f, parent);

        // Small Chocks pushed out to leave room for the TNT in the middle
        Spawn(w_Small, -1.5f, 2.25f, parent);
        Spawn(w_Small, 1.5f, 2.25f, parent);

        Spawn(tnt, 0f, 3.0f, parent);

        // Pig shifted slightly higher so it sits perfectly on the TNT
        SpawnRandomEnemy(0f, 4.6f, parent);

        return parent;
    }

    // ==========================================
    // DESIGN 2: THE STONE CATHEDRAL (Widened for Big Pigs)
    // ==========================================
    // ==========================================
    // DESIGN 2: THE STONE CATHEDRAL (Fixed Foundation!)
    // ==========================================
    private GameObject BuildCathedral()
    {
        GameObject parent = new GameObject("Cathedral");

        // --- THE FOUNDATION ---
        // Thick stone base to hold the massive pillars!
        Spawn(s_Crate, -2.5f, 1.0f, parent);
        Spawn(s_Crate, -0.5f, 1.0f, parent);
        Spawn(s_Crate, 0.5f, 1.0f, parent);
        Spawn(s_Crate, 2.5f, 1.0f, parent);

        // Floor Planks
        Spawn(s_Plank, -1.5f, 2.5f, parent);
        Spawn(s_Plank, 1.5f, 2.5f, parent);

        // --- THE PILLARS ---
        // Sits perfectly on the floor planks (Plank is Y:2.5, Pillar is 3 units tall)
        Spawn(s_Pillar, -2.5f, 5.0f, parent);
        Spawn(s_Pillar, -0.5f, 5.0f, parent);
        Spawn(s_Pillar, 0.5f, 5.0f, parent);
        Spawn(s_Pillar, 2.5f, 5.0f, parent);

        // Pigs sitting securely on the floor planks
        SpawnRandomEnemy(-1.5f, 3.6f, parent);
        SpawnRandomEnemy(1.5f, 3.6f, parent);

        // --- THE ROOF ---
        // Sits perfectly on top of the 3-unit tall pillars
        Spawn(w_Plank, -1.5f, 7.0f, parent);
        Spawn(w_Plank, 1.5f, 7.0f, parent);

        // Triangle Roof Spires
        Spawn(s_Triangle, -2.5f, 8.0f, parent);
        Spawn(s_Triangle, -0.5f, 8.0f, parent);
        Spawn(s_Triangle, 0.5f, 8.0f, parent);
        Spawn(s_Triangle, 2.5f, 8.0f, parent);

        return parent;
    }

    // ==========================================
    // DESIGN 3: THE ICE PYRAMID (Fixed Overlaps)
    // ==========================================
    private GameObject BuildIcePyramid()
    {
        GameObject parent = new GameObject("IcePyramid");

        // Base Layer
        Spawn(i_Crate, -2.2f, 1.0f, parent);
        Spawn(tnt, 0f, 1.0f, parent);
        Spawn(i_Crate, 2.2f, 1.0f, parent);

        // Floor 2
        Spawn(i_Plank, 0f, 2.5f, parent);

        // Pig sitting comfortably
        SpawnRandomEnemy(0f, 3.6f, parent);

        // Small Ice Blocks as brackets (Stacked 2-high to give the pig headroom!)
        Spawn(i_Small, -1.4f, 3.25f, parent);
        Spawn(i_Small, -1.4f, 3.75f, parent);
        Spawn(i_Small, 1.4f, 3.25f, parent);
        Spawn(i_Small, 1.4f, 3.75f, parent);

        // Floor 3 (Now sits perfectly above the 2-high small blocks)
        Spawn(w_Plank, 0f, 4.5f, parent);

        // Top Roof
        Spawn(i_Triangle, 0f, 5.5f, parent);

        return parent;
    }

    // ==========================================
    // DESIGN 4: THE SCAFFOLDING TOWER (Fixed Overlaps)
    // ==========================================
    private GameObject BuildScaffoldingTower()
    {
        GameObject parent = new GameObject("ScaffoldingTower");

        // Bottom frames 
        Spawn(w_FrameCrate, -2.2f, 1.0f, parent);
        Spawn(w_FrameCrate, 0f, 1.0f, parent);
        Spawn(w_FrameCrate, 2.2f, 1.0f, parent);

        // First floor
        Spawn(w_Plank, 0f, 2.5f, parent);

        // Middle layer (Plank crates are 1 unit tall)
        Spawn(w_PlankCrate, -1.2f, 3.5f, parent);
        Spawn(w_PlankCrate, 1.2f, 3.5f, parent);

        // NEW: Small blocks used as structural support brackets in the middle!
        Spawn(w_Small, -2.2f, 3.25f, parent);
        Spawn(w_Small, 2.2f, 3.25f, parent);

        // Second floor (Now sits perfectly on top of the plank crates)
        Spawn(w_Plank, 0f, 4.5f, parent);

        // Solid crate on top
        Spawn(w_SolidCrate, 0f, 6.0f, parent);

        // Pig safely on top
        SpawnRandomEnemy(0f, 7.6f, parent);

        return parent;
    }

    // ==========================================
    // HELPERS
    // ==========================================
    private void Spawn(GameObject prefab, float x, float y, GameObject parent)
    {
        if (prefab == null) return;
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        obj.transform.position = new Vector2(x, y);
        obj.transform.parent = parent.transform;
    }

    private void SpawnRandomEnemy(float x, float y, GameObject parent)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        Spawn(enemyPrefabs[randomIndex], x, y, parent);
    }
}