#if UNITY_EDITOR
using UnityEngine;

public class Level_4 : StructureBlueprint
{
    public override string Name => "Level_4";

    public override void Build(StructureGeneratorWindow tools, GameObject parent)
    {
        // Boulders (1x1). Floor = 0 -> Center = 0.5.
        // Placed at X=1.5 so they are 3 units apart, easily supporting a 6-unit plank.
        tools.Spawn(tools.s_Boulder, -1.5f, 0.5f, parent);
        tools.Spawn(tools.s_Boulder, 1.5f, 0.5f, parent);

        // Plank (6x1). Boulders top = 1.0 -> Center = 1.5.
        tools.Spawn(tools.w_Plank, 0f, 1.5f, parent);

        // Chocks (1x1 small blocks). Plank top = 2.0 -> Center = 2.5.
        // Placed at X=1.5 to leave exactly a 2-unit gap in the middle for the TNT.
        tools.Spawn(tools.w_Small, -1.5f, 2.5f, parent);
        tools.Spawn(tools.w_Small, 1.5f, 2.5f, parent);

        // TNT (2x2). Plank top = 2.0 -> Center = 3.0.
        // Fits perfectly between the chocks.
        tools.Spawn(tools.tnt, 0f, 3.0f, parent);

        // Pig. TNT top = 4.0. Pig center = 4.8.
        tools.SpawnRandomEnemy(0f, 4.8f, parent);
    }
}
#endif