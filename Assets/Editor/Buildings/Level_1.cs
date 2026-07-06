#if UNITY_EDITOR
using UnityEngine;

public class Level_1 : StructureBlueprint
{
    public override string Name => "Level_1";

    public override void Build(StructureGeneratorWindow tools, GameObject parent)
    {
        // ======================================================================
        // 1. THE FOUNDATION
        // ======================================================================
        // Small Blocks (1x1). Floor = 0 -> Center = 0.5.
        // We use 6 of them side-by-side to create a solid 6-unit wide foundation.
        tools.Spawn(tools.w_Small, -2.5f, 0.5f, parent);
        tools.Spawn(tools.w_Small, -1.5f, 0.5f, parent);
        tools.Spawn(tools.w_Small, -0.5f, 0.5f, parent);
        tools.Spawn(tools.w_Small, 0.5f, 0.5f, parent);
        tools.Spawn(tools.w_Small, 1.5f, 0.5f, parent);
        tools.Spawn(tools.w_Small, 2.5f, 0.5f, parent);

        // ======================================================================
        // 2. LOWER DECK
        // ======================================================================
        // ONE Plank (6x1). Foundation Top = 1.0 -> Center = 1.5.
        // Centered exactly at 0 to cover all 6 small blocks.
        tools.Spawn(tools.w_Plank, 0f, 1.5f, parent);

        // ======================================================================
        // 3. THE GLASS CORE & LOWER PILLARS
        // ======================================================================
        // Pillars (1x6). Plank Top = 2.0 -> Center = 5.0.
        // Outer wood pillars pushed to the absolute edges (-2.5 and 2.5)
        tools.Spawn(tools.w_Pillar, -2.5f, 5.0f, parent);
        tools.Spawn(tools.w_Pillar, 2.5f, 5.0f, parent);

        // Inner glass core centered perfectly
        tools.Spawn(tools.i_Pillar, -0.5f, 5.0f, parent);
        tools.Spawn(tools.i_Pillar, 0.5f, 5.0f, parent);

        // ======================================================================
        // 4. MIDDLE DECK
        // ======================================================================
        // ONE Plank (6x1). Pillars Top = 8.0 -> Center = 8.5.
        tools.Spawn(tools.w_Plank, 0f, 8.5f, parent);

        // ======================================================================
        // 5. UPPER CHAMBER
        // ======================================================================
        // Pillars (1x6). Plank Top = 9.0 -> Center = 12.0.
        // These are on the absolute outer edges, leaving a massive 4-unit gap in the middle!
        tools.Spawn(tools.w_Pillar, -2.5f, 12.0f, parent);
        tools.Spawn(tools.w_Pillar, 2.5f, 12.0f, parent);

        // Pig's Throne (2x2 Crate). Plank Top = 9.0 -> Center = 10.0.
        tools.Spawn(tools.w_SolidCrate, 0f, 10.0f, parent);

        // Target Pig. Crate Top = 11.0 -> Center = ~12.2.
        // Sitting perfectly safe on the crate, miles away from the pillars.
        tools.SpawnRandomEnemy(0f, 12.2f, parent);

        // ======================================================================
        // 6. ROOF & SPIRE
        // ======================================================================
        // Roof Plank (6x1). Pillars Top = 15.0 -> Center = 15.5.
        tools.Spawn(tools.w_Plank, 0f, 15.5f, parent);

        // Spire Pillar (1x6). Roof Top = 16.0 -> Center = 19.0.
        tools.Spawn(tools.w_Pillar, 0f, 19.0f, parent);

        // Spire Triangle (1x1). Pillar Top = 22.0 -> Center = 22.5.
        tools.Spawn(tools.w_Triangle, 0f, 22.5f, parent);
    }
}
#endif