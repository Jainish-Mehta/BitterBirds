#if UNITY_EDITOR
using UnityEngine;

public class KingsFortress : StructureBlueprint
{
    public override string Name => "5_KingsFortress";

    public override void Build(StructureGeneratorWindow tools, GameObject parent)
    {
        // 1. Massive Floor (Crates are 2x2. Center = 1.0, Top = 2.0)
        tools.Spawn(tools.s_Crate, -6.0f, 1.0f, parent);
        tools.Spawn(tools.s_Crate, -2.0f, 1.0f, parent);
        tools.Spawn(tools.s_Crate, 2.0f, 1.0f, parent);
        tools.Spawn(tools.s_Crate, 6.0f, 1.0f, parent);

        // 2. Lower Planks (Shifted INWARD to fully support the central TNT!)
        // Planks are 6 units wide. Placing at X=-2.5 and X=2.5 leaves a 1-unit gap in the middle.
        // The TNT (2x2) is wider than the gap, so it will sit perfectly on both planks without falling!
        tools.Spawn(tools.s_Plank, -3.5f, 2.5f, parent);
        tools.Spawn(tools.s_Plank, 3.5f, 2.5f, parent);

        // 3. The New, Stable Chamber 
        // Core TNT sits safely in the middle, resting on the edges of both planks.
        tools.Spawn(tools.tnt, 0.0f, 4.0f, parent);

        // THE FIX: Frames moved INWARD (Swapped with enemies)
        tools.Spawn(tools.w_FrameCrate, -2.0f, 4.0f, parent);
        tools.Spawn(tools.w_FrameCrate, 2.0f, 4.0f, parent);

        // PIGS IN THE OUTER GAPS: Sitting safely on the outer edge of the planks
        tools.SpawnRandomEnemy(-4.25f, 3.8f, parent);
        tools.SpawnRandomEnemy(4.25f, 3.8f, parent);

        // 4. Outer Pillars 
        // Moved inward slightly to tightly cap the edges of the planks
        tools.Spawn(tools.s_Pillar, -6f, 6.0f, parent);
        tools.Spawn(tools.s_Pillar, 6f, 6.0f, parent);

        // 5. Core Roof Plank (Rests perfectly on the TNT and the inner frames)
        tools.Spawn(tools.w_Plank, 0f, 5.5f, parent);

        // 6. The King's Chamber
        // Ice brackets tightened to perfectly border the central plank
        tools.Spawn(tools.i_Small, -2.5f, 6.5f, parent); tools.Spawn(tools.i_Small, 2.5f, 6.5f, parent);
        tools.Spawn(tools.i_Small, -2.5f, 7.5f, parent); tools.Spawn(tools.i_Small, 2.5f, 7.5f, parent);
        tools.Spawn(tools.i_Small, -2.5f, 8.5f, parent); tools.Spawn(tools.i_Small, 2.5f, 8.5f, parent);

        // King Pig sitting freely in the middle
        tools.SpawnRandomEnemy(0.0f, 6.8f, parent);

        // 7. Grand Roof (Spans perfectly from the outer pillars to the ice brackets)
        tools.Spawn(tools.s_Plank, -3.5f, 9.5f, parent);
        tools.Spawn(tools.s_Plank, 3.5f, 9.5f, parent);

        // 8. Spires and Top Crates
        tools.Spawn(tools.s_Triangle, -5.0f, 10.5f, parent);

        // Crates tightly closed in the center
        tools.Spawn(tools.w_SolidCrate, -1.0f, 11.0f, parent);
        tools.Spawn(tools.w_SolidCrate, 1.0f, 11.0f, parent);

        tools.Spawn(tools.s_Triangle, 5.0f, 10.5f, parent);

        // Sniper Pig sitting securely on the closed crates
        tools.SpawnRandomEnemy(0.0f, 12.8f, parent);
    }
}
#endif