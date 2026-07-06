using UnityEngine;

[CreateAssetMenu(fileName = "New Bird Data", menuName = "BitterBirds/Bird Data")]
public class BirdData : ScriptableObject
{
    [Header("Bird Info")]
    public string birdName = "Red Bird";

    [TextArea(2, 4)]
    public string description = "The standard bird. No special abilities.";

    [Header("Visuals")]
    public Sprite birdSprite; // The sprite it uses in the game
    public RuntimeAnimatorController animatorController;
}