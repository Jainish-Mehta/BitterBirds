#if UNITY_EDITOR
using UnityEngine;

public class Level_2 : StructureBlueprint
{
    public override string Name => "Level_2";

    public override void Build(StructureGeneratorWindow tools, GameObject parent)
    {
        // ======================================================================
        // LEFT TOWER (The Double-Decker)
        // Center X = 0.0
        // ======================================================================

        // 1. Base tiny pillars (1x1 Small Stones). Floor = 0.0 -> Center = 0.5.
        // Placed evenly to support the 6-unit plank
        tools.Spawn(tools.s_Small, -2.0f, 0.5f, parent);
        tools.Spawn(tools.s_Small, 0.0f, 0.5f, parent);
        tools.Spawn(tools.s_Small, 2.0f, 0.5f, parent);

        // 2. First Floor Plank (6x1). Stone Top = 1.0 -> Center = 1.5.
        tools.Spawn(tools.s_Plank, 0f, 1.5f, parent);

        // 3. Middle Section (Height = 4.0)
        // Left side: Two 2x2 crates stacked
        tools.Spawn(tools.w_SolidCrate, -2.0f, 3.0f, parent); // Bottom
        tools.Spawn(tools.w_SolidCrate, -2.0f, 5.0f, parent); // Top

        // Right side: Two 2x2 crates stacked
        tools.Spawn(tools.w_SolidCrate, 2.0f, 3.0f, parent); // Bottom
        tools.Spawn(tools.w_SolidCrate, 2.0f, 5.0f, parent); // Top

        // Center: Four 1x1 small blocks stacked to act as a 4-tall pillar
        tools.Spawn(tools.w_Small, 0f, 2.5f, parent);
        tools.Spawn(tools.w_Small, 0f, 3.5f, parent);
        tools.Spawn(tools.w_Small, 0f, 4.5f, parent);
        tools.Spawn(tools.w_Small, 0f, 5.5f, parent);

        // 4. Second Floor Plank (6x1). Crates Top = 6.0 -> Center = 6.5.
        tools.Spawn(tools.s_Plank, 0f, 6.5f, parent);

        // 5. Top Chamber
        // Tall Wooden Pillars (1x6). Plank Top = 7.0 -> Center = 10.0.
        // Placed flush at the edges (X = -2.5 and 2.5) leaving a massive 4-unit gap!
        tools.Spawn(tools.w_Pillar, -2.5f, 10.0f, parent);
        tools.Spawn(tools.w_Pillar, 2.5f, 10.0f, parent);

        // Small wood block next to the pig (Center = 7.5)
        tools.Spawn(tools.w_Small, -1.2f, 7.5f, parent);

        // Pig resting safely inside the chamber
        tools.SpawnRandomEnemy(0f, 7.8f, parent);

        // 6. Roof
        // Ice Plank (6x1). Pillars Top = 13.0 -> Center = 13.5.
        tools.Spawn(tools.i_Plank, 0f, 13.5f, parent);

        // Ice Crown (1x1). Plank Top = 14.0 -> Center = 14.5.
        tools.Spawn(tools.i_Small, 0f, 14.5f, parent);


        // ======================================================================
        // RIGHT TOWER (The Elevated Outpost)
        // Center X = 8.0 (Shifted Right)
        // Base Y = 4.0 (Shifted UP to simulate sitting on a dirt hill!)
        // ======================================================================

        // 1. Base tiny pillars (1x1 Small Wood). Hill = 4.0 -> Center = 4.5.
        tools.Spawn(tools.w_Small, 6.0f, 4.5f, parent);
        tools.Spawn(tools.w_Small, 8.0f, 4.5f, parent);
        tools.Spawn(tools.w_Small, 10.0f, 4.5f, parent);

        // 2. Floor Plank (6x1). Small Wood Top = 5.0 -> Center = 5.5.
        tools.Spawn(tools.w_Plank, 8.0f, 5.5f, parent);

        // 3. The Chamber
        // Tall Wooden Pillars (1x6). Plank Top = 6.0 -> Center = 9.0.
        // Placed flush at the edges (X = 5.5 and 10.5)
        tools.Spawn(tools.w_Pillar, 5.5f, 9.0f, parent);
        tools.Spawn(tools.w_Pillar, 10.5f, 9.0f, parent);

        // Pig resting safely inside
        tools.SpawnRandomEnemy(8.0f, 6.8f, parent);

        // 4. Roof
        // Ice Plank (6x1). Pillars Top = 12.0 -> Center = 12.5.
        tools.Spawn(tools.i_Plank, 8.0f, 12.5f, parent);

        // Ice Crown (1x1). Plank Top = 13.0 -> Center = 13.5.
        tools.Spawn(tools.i_Small, 8.0f, 13.5f, parent);
    }
}
#endif