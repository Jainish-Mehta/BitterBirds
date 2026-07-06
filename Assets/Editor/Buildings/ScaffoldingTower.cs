#if UNITY_EDITOR
using UnityEngine;

public class ScaffoldingTower : StructureBlueprint
{
    public override string Name => "4_ScaffoldingTower";

    public override void Build(StructureGeneratorWindow tools, GameObject parent)
    {
        // Base Frames (2x2). Center = 1.0.
        tools.Spawn(tools.w_FrameCrate, -2.0f, 1.0f, parent);
        tools.Spawn(tools.w_FrameCrate, 0.0f, 1.0f, parent);
        tools.Spawn(tools.w_FrameCrate, 2.0f, 1.0f, parent);

        // Floor 1 (6x1). Center = 2.5.
        tools.Spawn(tools.w_Plank, 0f, 2.5f, parent);

        // Middle Crates (2x2). Plank top = 3.0 -> Center = 4.0.
        // Pushed to X=1.5 so a 6-unit plank can sit perfectly on top.
        tools.Spawn(tools.w_PlankCrate, -1.5f, 4.0f, parent);
        tools.Spawn(tools.w_PlankCrate, 1.5f, 4.0f, parent);

        // Floor 2 (6x1). Crate top = 5.0 -> Center = 5.5.
        tools.Spawn(tools.w_Plank, 0f, 5.5f, parent);

        // Top Crate (2x2). Plank top = 6.0 -> Center = 7.0.
        tools.Spawn(tools.w_SolidCrate, 0f, 7.0f, parent);

        // Pig. Safely on the solid crate.
        tools.SpawnRandomEnemy(0f, 8.8f, parent);
    }
}
#endif