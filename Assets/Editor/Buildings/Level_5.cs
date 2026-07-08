#if UNITY_EDITOR
using UnityEngine;

public class Level_5 : StructureBlueprint
{
    public override string Name => "Level_5";

    public override void Build(StructureGeneratorWindow tools, GameObject parent)
    {
        // Base Crates (2x2). Center = 1.0.
        tools.Spawn(tools.s_Crate, -5.0f, 1.0f, parent);
        tools.Spawn(tools.s_Crate, -1.0f, 1.0f, parent);
        tools.Spawn(tools.s_Crate, 1.0f, 1.0f, parent);
        tools.Spawn(tools.s_Crate, 5.0f, 1.0f, parent);

        // Lower Planks (6x1). Crate Top = 2.0 -> Center = 2.5.
        // Centered exactly over the 4-unit gap between crates.
        tools.Spawn(tools.s_Plank, -3.0f, 2.5f, parent);
        tools.Spawn(tools.s_Plank, 3.0f, 2.5f, parent);

        // Pillars (1x6). Plank top = 3.0 -> Center = 6.0.
        // Set exactly 5 units apart to leave a massive gap for the pigs.
        tools.Spawn(tools.s_Pillar, -5.5f, 6.0f, parent);
        tools.Spawn(tools.s_Pillar, -0.5f, 6.0f, parent);
        tools.Spawn(tools.s_Pillar, 0.5f, 6.0f, parent);
        tools.Spawn(tools.s_Pillar, 5.5f, 6.0f, parent);

        // Pigs. Safely resting in the 5-unit gap between pillars.
        tools.SpawnRandomEnemy(-3.0f, 3.8f, parent);
        tools.SpawnRandomEnemy(3.0f, 3.8f, parent);

        // Upper Planks (6x1). Pillar top = 9.0 -> Center = 9.5.
        tools.Spawn(tools.w_Plank, -3.0f, 9.5f, parent);
        tools.Spawn(tools.w_Plank, 3.0f, 9.5f, parent);

        // Triangles (1.05x1.05). Plank top = 10.0 -> Center = 10.525.
        tools.Spawn(tools.s_Triangle, -5.5f, 10.525f, parent);
        tools.Spawn(tools.s_Triangle, -0.5f, 10.525f, parent);
        tools.Spawn(tools.s_Triangle, 0.5f, 10.525f, parent);
        tools.Spawn(tools.s_Triangle, 5.5f, 10.525f, parent);
    }
}
#endif