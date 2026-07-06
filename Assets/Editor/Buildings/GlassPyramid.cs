#if UNITY_EDITOR
using UnityEngine;

public class GlassPyramid : StructureBlueprint
{
    public override string Name => "3_GlassPyramid";

    public override void Build(StructureGeneratorWindow tools, GameObject parent)
    {
        // Base (2x2). Center = 1.0.
        // Pushed out to 2.5 so the 6-unit plank covers them perfectly without overhanging.
        tools.Spawn(tools.i_Crate, -2.5f, 1.0f, parent);
        tools.Spawn(tools.tnt, 0.0f, 1.0f, parent);
        tools.Spawn(tools.i_Crate, 2.5f, 1.0f, parent);

        // Plank (6x1). Center = 2.5.
        tools.Spawn(tools.i_Plank, 0f, 2.5f, parent);

        // Ice Brackets (1x1). Plank top = 3.0 -> Centers = 3.5 & 4.5.
        // Spaced 5 units apart (X = -2.5, 2.5) to give the Pig a huge room.
        tools.Spawn(tools.i_Small, -2.5f, 3.5f, parent); tools.Spawn(tools.i_Small, -2.5f, 4.5f, parent);
        tools.Spawn(tools.i_Small, 2.5f, 3.5f, parent); tools.Spawn(tools.i_Small, 2.5f, 4.5f, parent);

        // Pig. Safely inside the bracket room.
        tools.SpawnRandomEnemy(0f, 3.8f, parent);

        // Top Plank (6x1). Bracket top = 5.0 -> Center = 5.5.
        tools.Spawn(tools.w_Plank, 0f, 5.5f, parent);

        // Top Triangle (1.05x1.05). Plank top = 6.0 -> Center = 6.525.
        tools.Spawn(tools.i_Triangle, 0f, 6.525f, parent);
    }
}
#endif