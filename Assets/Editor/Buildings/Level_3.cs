#if UNITY_EDITOR
using UnityEngine;

public class Level_3 : StructureBlueprint
{
    public override string Name => "Level_3";

    public override void Build(StructureGeneratorWindow tools, GameObject parent)
    {
        // ======================================================================
        // OBSTACLE 1: The Hill Boulder
        // Center X = 0.0
        // ======================================================================
        // Boulder (1x1). Assuming you draw a dirt hill up to Y = 3.0.
        // Hill Top = 3.0 -> Boulder Center = 3.5.
        tools.Spawn(tools.w_Boulder, 0.0f, 3.5f, parent);

        // A small enemy hiding on the ground in front of the hill.
        // Floor = 0.0 -> Pig Center = 0.8
        tools.SpawnRandomEnemy(-3.0f, 0.8f, parent);


        // ======================================================================
        // OBSTACLE 2: The "Scarecrow" Tower
        // Center X = 8.0
        // ======================================================================

        // 1. Tall Wooden Pillars (1x6). Floor = 0.0 -> Center = 3.0. (Top = 6.0)
        // The plank is 6 units wide. To support it perfectly flush at the edges, 
        // the 1-unit wide pillars go exactly 2.5 units left and right of the center!
        tools.Spawn(tools.w_Pillar, 5.5f, 3.0f, parent);
        tools.Spawn(tools.w_Pillar, 10.5f, 3.0f, parent);

        // 2. Wood Plank (6x1). Rests on Pillars (Height 6.0) -> Center = 6.5. (Top = 7.0)
        tools.Spawn(tools.w_Plank, 8.0f, 6.5f, parent);

        // 3. The Target Pig. Rests safely on Plank (Height 7.0) -> Center = 7.8.
        // Enemies are NOT load-bearing, so it sits alone at the absolute top.
        tools.SpawnRandomEnemy(8.0f, 7.8f, parent);

        // Another enemy hiding on the ground right before the tower
        tools.SpawnRandomEnemy(4.0f, 0.8f, parent);
    }
}
#endif