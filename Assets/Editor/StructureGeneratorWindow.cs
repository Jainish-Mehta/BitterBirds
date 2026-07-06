using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public class StructureGeneratorWindow : EditorWindow
{
    // ==========================================
    // YOUR ASSET LIBRARY
    // ==========================================
    [Header("Wood")]
    public GameObject w_SolidCrate, w_FrameCrate, w_PlankCrate, w_Plank, w_Pillar, w_Triangle, w_Boulder, w_Small;

    [Header("Stone")]
    public GameObject s_Crate, s_Plank, s_Pillar, s_Triangle, s_Boulder, s_Small;

    [Header("Ice")]
    public GameObject i_Crate, i_Plank, i_Pillar, i_Triangle, i_Boulder, i_Small;

    [Header("Extras")]
    public GameObject tnt;
    public GameObject[] enemyPrefabs = new GameObject[0];

    private Vector2 scrollPos;

    // The lists that hold the scripts and their checkbox states!
    private List<StructureBlueprint> allBlueprints = new List<StructureBlueprint>();
    private List<bool> blueprintSelections = new List<bool>();

    [MenuItem("BitterBirds/Ultimate Structure Builder")]
    public static void ShowWindow()
    {
        GetWindow<StructureGeneratorWindow>("Structure Builder");
    }

    private void OnEnable()
    {
        // 1. Load your saved prefabs
        w_SolidCrate = LoadSavedPrefab("w_SolidCrate"); w_FrameCrate = LoadSavedPrefab("w_FrameCrate");
        w_PlankCrate = LoadSavedPrefab("w_PlankCrate"); w_Plank = LoadSavedPrefab("w_Plank");
        w_Pillar = LoadSavedPrefab("w_Pillar"); w_Triangle = LoadSavedPrefab("w_Triangle");
        w_Boulder = LoadSavedPrefab("w_Boulder"); w_Small = LoadSavedPrefab("w_Small");

        s_Crate = LoadSavedPrefab("s_Crate"); s_Plank = LoadSavedPrefab("s_Plank");
        s_Pillar = LoadSavedPrefab("s_Pillar"); s_Triangle = LoadSavedPrefab("s_Triangle");
        s_Boulder = LoadSavedPrefab("s_Boulder"); s_Small = LoadSavedPrefab("s_Small");

        i_Crate = LoadSavedPrefab("i_Crate"); i_Plank = LoadSavedPrefab("i_Plank");
        i_Pillar = LoadSavedPrefab("i_Pillar"); i_Triangle = LoadSavedPrefab("i_Triangle");
        i_Boulder = LoadSavedPrefab("i_Boulder"); i_Small = LoadSavedPrefab("i_Small");

        tnt = LoadSavedPrefab("tnt");

        int enemyCount = EditorPrefs.GetInt("enemyPrefabs_Count", 0);
        enemyPrefabs = new GameObject[enemyCount];
        for (int i = 0; i < enemyCount; i++) enemyPrefabs[i] = LoadSavedPrefab("enemyPrefabs_" + i);

        // 2. Automatically find all your custom building scripts!
        FindAllBlueprints();
    }

    private void FindAllBlueprints()
    {
        allBlueprints.Clear();
        blueprintSelections.Clear();

        // This searches EVERY folder and subfolder in the project automatically
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(StructureBlueprint)));

        foreach (var type in types)
        {
            StructureBlueprint blueprint = (StructureBlueprint)System.Activator.CreateInstance(type);
            allBlueprints.Add(blueprint);
        }

        // Sort them alphabetically to keep the list organized
        allBlueprints = allBlueprints.OrderBy(b => b.Name).ToList();

        // Add a checkbox state (default to true) for every blueprint we found
        for (int i = 0; i < allBlueprints.Count; i++)
        {
            blueprintSelections.Add(true);
        }
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.Label("ASSIGN PREFABS (Auto-Saves)", EditorStyles.boldLabel);

        // --- WOOD ---
        GUILayout.Label("--- WOOD ---", EditorStyles.boldLabel);
        w_SolidCrate = DrawAndSaveSlot("Wood Crate", w_SolidCrate, "w_SolidCrate");
        w_FrameCrate = DrawAndSaveSlot("Wood Frame", w_FrameCrate, "w_FrameCrate");
        w_PlankCrate = DrawAndSaveSlot("Wood Plank Crate", w_PlankCrate, "w_PlankCrate");
        w_Plank = DrawAndSaveSlot("Wood Plank (6x1)", w_Plank, "w_Plank");
        w_Pillar = DrawAndSaveSlot("Wood Pillar (1x6)", w_Pillar, "w_Pillar");
        w_Triangle = DrawAndSaveSlot("Wood Triangle", w_Triangle, "w_Triangle");
        w_Boulder = DrawAndSaveSlot("Wood Boulder", w_Boulder, "w_Boulder");
        w_Small = DrawAndSaveSlot("Small Wood", w_Small, "w_Small");

        // --- STONE ---
        GUILayout.Space(5); GUILayout.Label("--- STONE ---", EditorStyles.boldLabel);
        s_Crate = DrawAndSaveSlot("Stone Crate", s_Crate, "s_Crate");
        s_Plank = DrawAndSaveSlot("Stone Plank (6x1)", s_Plank, "s_Plank");
        s_Pillar = DrawAndSaveSlot("Stone Pillar (1x6)", s_Pillar, "s_Pillar");
        s_Triangle = DrawAndSaveSlot("Stone Triangle", s_Triangle, "s_Triangle");
        s_Boulder = DrawAndSaveSlot("Stone Boulder", s_Boulder, "s_Boulder");
        s_Small = DrawAndSaveSlot("Small Stone", s_Small, "s_Small");

        // --- ICE ---
        GUILayout.Space(5); GUILayout.Label("--- ICE ---", EditorStyles.boldLabel);
        i_Crate = DrawAndSaveSlot("Ice Crate", i_Crate, "i_Crate");
        i_Plank = DrawAndSaveSlot("Ice Plank (6x1)", i_Plank, "i_Plank");
        i_Pillar = DrawAndSaveSlot("Ice Pillar (1x6)", i_Pillar, "i_Pillar");
        i_Triangle = DrawAndSaveSlot("Ice Triangle", i_Triangle, "i_Triangle");
        i_Boulder = DrawAndSaveSlot("Ice Boulder", i_Boulder, "i_Boulder");
        i_Small = DrawAndSaveSlot("Small Ice", i_Small, "i_Small");

        // --- EXTRAS ---
        GUILayout.Space(5); GUILayout.Label("--- HAZARDS & ENEMIES ---", EditorStyles.boldLabel);
        tnt = DrawAndSaveSlot("TNT Block", tnt, "tnt");

        EditorGUI.BeginChangeCheck();
        SerializedObject so = new SerializedObject(this);
        EditorGUILayout.PropertyField(so.FindProperty("enemyPrefabs"), new GUIContent("Enemy Array"), true);
        if (EditorGUI.EndChangeCheck())
        {
            so.ApplyModifiedProperties();
            EditorPrefs.SetInt("enemyPrefabs_Count", enemyPrefabs.Length);
            for (int i = 0; i < enemyPrefabs.Length; i++) SavePrefabPath("enemyPrefabs_" + i, enemyPrefabs[i]);
        }

        // ===============================================
        // THE CHECKBOX GENERATOR UI
        // ===============================================
        GUILayout.Space(30);
        GUILayout.Label("SELECT STRUCTURES TO BUILD", EditorStyles.boldLabel);

        if (allBlueprints.Count > 0)
        {
            // Quick Select / Deselect All Buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Select All"))
            {
                for (int i = 0; i < blueprintSelections.Count; i++) blueprintSelections[i] = true;
            }
            if (GUILayout.Button("Select None"))
            {
                for (int i = 0; i < blueprintSelections.Count; i++) blueprintSelections[i] = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Draw the Checkboxes inside a nice visual box
            GUILayout.BeginVertical("box");
            for (int i = 0; i < allBlueprints.Count; i++)
            {
                blueprintSelections[i] = EditorGUILayout.ToggleLeft(" " + allBlueprints[i].Name, blueprintSelections[i]);
            }
            GUILayout.EndVertical();

            GUILayout.Space(10);

            // The big Generate Button
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("GENERATE CHECKED STRUCTURES", GUILayout.Height(40)))
            {
                int generatedCount = 0;

                // Loop through the list and only build the ones that are checked!
                for (int i = 0; i < allBlueprints.Count; i++)
                {
                    if (blueprintSelections[i])
                    {
                        GenerateStructure(allBlueprints[i]);
                        generatedCount++;
                    }
                }

                if (generatedCount > 0)
                {
                    AssetDatabase.Refresh();
                    Debug.Log($"<color=green>SUCCESS:</color> Generated {generatedCount} selected structures in Assets/Resources/EpicStructures!");
                }
                else
                {
                    Debug.LogWarning("You didn't check any structures to build!");
                }
            }
            GUI.backgroundColor = Color.white;
        }
        else
        {
            GUILayout.Label("No Structure Blueprints found! Create a script that uses 'StructureBlueprint'.", EditorStyles.helpBox);
        }

        EditorGUILayout.EndScrollView();
    }

    private void GenerateStructure(StructureBlueprint blueprint)
    {
        string folder = "Assets/Resources/EpicStructures";
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        GameObject parent = new GameObject(blueprint.Name);

        // This calls the specific building's math!
        blueprint.Build(this, parent);

        string path = $"{folder}/{blueprint.Name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(parent, path);
        DestroyImmediate(parent);
    }

    // --- SAVING HELPERS ---
    private GameObject DrawAndSaveSlot(string label, GameObject currentObj, string saveKey)
    {
        GameObject newObj = (GameObject)EditorGUILayout.ObjectField(label, currentObj, typeof(GameObject), false);
        if (newObj != currentObj) SavePrefabPath(saveKey, newObj);
        return newObj;
    }

    private void SavePrefabPath(string key, GameObject obj)
    {
        if (obj != null) EditorPrefs.SetString(key, AssetDatabase.GetAssetPath(obj));
        else EditorPrefs.DeleteKey(key);
    }

    private GameObject LoadSavedPrefab(string key)
    {
        if (EditorPrefs.HasKey(key)) return AssetDatabase.LoadAssetAtPath<GameObject>(EditorPrefs.GetString(key));
        return null;
    }

    // =========================================================
    // PUBLIC HELPERS (Your Blueprint Scripts need these!)
    // =========================================================

    public void Spawn(GameObject prefab, float x, float y, GameObject parent)
    {
        if (prefab == null) return;
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        obj.transform.position = new Vector2(x, y);
        obj.transform.parent = parent.transform;
    }

    public void SpawnRandomEnemy(float x, float y, GameObject parent)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        Spawn(enemyPrefabs[randomIndex], x, y, parent);
    }
}