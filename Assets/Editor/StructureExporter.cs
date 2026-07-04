using UnityEngine;
using UnityEditor;
using System.Text;

public class StructureExporter
{
    // Adds a right-click menu option to GameObjects in the Hierarchy
    [MenuItem("GameObject/BitterBirds/Export Coordinates to Clipboard", false, 0)]
    public static void ExportCoordinates(MenuCommand menuCommand)
    {
        // Get the object you right-clicked on
        GameObject selectedObj = menuCommand.context as GameObject;
        if (selectedObj == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"// === STRUCTURE: {selectedObj.name} ===");

        // Loop through every block inside this structure
        foreach (Transform child in selectedObj.transform)
        {
            // Clean up the name (removes "(Clone)" if it's a prefab)
            string cleanName = child.name.Replace("(Clone)", "").Trim();

            // Round the coordinates so they are clean (e.g., 1.5 instead of 1.499999)
            float x = (float)System.Math.Round(child.localPosition.x, 2);
            float y = (float)System.Math.Round(child.localPosition.y, 2);
            float rot = (float)System.Math.Round(child.localEulerAngles.z, 2);

            if (rot == 0)
                sb.AppendLine($"{cleanName}: X = {x}, Y = {y}");
            else
                sb.AppendLine($"{cleanName}: X = {x}, Y = {y}, Rotation = {rot}");
        }

        // Copy it to the computer's clipboard!
        string result = sb.ToString();
        GUIUtility.systemCopyBuffer = result;

        Debug.Log($"<color=cyan>SUCCESS:</color> Data for {selectedObj.name} copied to clipboard! You can now paste it (Ctrl+V).");
    }
}