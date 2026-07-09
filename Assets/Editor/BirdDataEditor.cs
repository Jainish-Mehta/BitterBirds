using UnityEngine;
using UnityEditor;

// This tells Unity: "Use this script to draw the Inspector for BirdData!"
[CustomEditor(typeof(BirdData))]
public class BirdDataEditor : Editor
{
    private SerializedProperty birdName;
    private SerializedProperty description;
    private SerializedProperty scoreMultiplier;
    private SerializedProperty birdSprite;
    private SerializedProperty animatorController;
    private SerializedProperty ability;
    private SerializedProperty abilityPower;
    private SerializedProperty abilityRadius;
    private SerializedProperty shockwavePrefab;
    private SerializedProperty tntPrefab;

    private void OnEnable()
    {
        // Link the properties to the actual variables in BirdData.cs
        birdName = serializedObject.FindProperty("birdName");
        description = serializedObject.FindProperty("description");
        scoreMultiplier = serializedObject.FindProperty("scoreMultiplier");
        birdSprite = serializedObject.FindProperty("birdSprite");
        animatorController = serializedObject.FindProperty("animatorController");
        ability = serializedObject.FindProperty("ability");
        abilityPower = serializedObject.FindProperty("abilityPower");
        abilityRadius = serializedObject.FindProperty("abilityRadius");
        shockwavePrefab = serializedObject.FindProperty("shockwavePrefab");
        tntPrefab = serializedObject.FindProperty("tntPrefab");
    }

    public override void OnInspectorGUI()
    {
        // Always start by updating the serialized object
        serializedObject.Update();

        // --- BIRD INFO ---
        EditorGUILayout.PropertyField(birdName);
        EditorGUILayout.PropertyField(description);
        EditorGUILayout.Space(10);

        // --- SCORING ---
        EditorGUILayout.PropertyField(scoreMultiplier, new GUIContent("Score Multiplier"));

        // --- VISUALS ---
        EditorGUILayout.PropertyField(birdSprite);
        EditorGUILayout.PropertyField(animatorController);
        EditorGUILayout.Space(10);

        // --- ABILITIES ---
        EditorGUILayout.PropertyField(ability);

        // Get the current enum value to decide what to show
        BirdAbility currentAbility = (BirdAbility)ability.enumValueIndex;

        // DYNAMICALLY SHOW/HIDE FIELDS BASED ON THE DROPDOWN!
        switch (currentAbility)
        {
            case BirdAbility.None:
                // Show nothing extra!
                break;

            case BirdAbility.SonicBoom:
                EditorGUILayout.PropertyField(abilityPower, new GUIContent("Shockwave Force"));
                EditorGUILayout.PropertyField(abilityRadius, new GUIContent("Blast Radius"));
                EditorGUILayout.PropertyField(shockwavePrefab, new GUIContent("Shockwave Prefab"));
                break;

            case BirdAbility.StraightDash:
                EditorGUILayout.PropertyField(abilityPower, new GUIContent("Dash Speed"));
                break;

            case BirdAbility.TNTDelivery:
                EditorGUILayout.PropertyField(tntPrefab, new GUIContent("TNT to Drop"));
                // Optional: Show abilityPower if you want to tweak the bird's "recoil" bounce
                EditorGUILayout.PropertyField(abilityPower, new GUIContent("Recoil Bounce Force"));
                break;

            case BirdAbility.Split:
                EditorGUILayout.HelpBox("This bird will dynamically clone itself 4 times into a shotgun spread!", MessageType.Info);
                break;

            case BirdAbility.HeavyFall:
                EditorGUILayout.PropertyField(abilityPower, new GUIContent("Downward Meteor Speed"));
                break;
        }

        // Always end by applying the modified properties
        serializedObject.ApplyModifiedProperties();
    }
}