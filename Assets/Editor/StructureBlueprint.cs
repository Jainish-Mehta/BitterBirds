using UnityEngine;

// This is the "Rulebook" for all buildings!
public abstract class StructureBlueprint
{
    // Every building must have a Name
    public abstract string Name { get; }

    // Every building must have instructions on how to build itself
    public abstract void Build(StructureGeneratorWindow tools, GameObject parent);

    // A helper method that every building can use
    protected void Spawn(GameObject prefab, float x, float y, GameObject parent)
    {
        if (prefab == null) return;

        // This is safe for Editor scripting!
        GameObject obj = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
        obj.transform.position = new Vector2(x, y);
        obj.transform.parent = parent.transform;
    }
}