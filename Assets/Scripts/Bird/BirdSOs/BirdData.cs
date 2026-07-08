using UnityEngine;

public enum BirdAbility
{
    None,
    SonicBoom,
    StraightDash,
    TNTDelivery,
    Split,
    HeavyFall
}

[CreateAssetMenu(fileName = "New Bird Data", menuName = "BitterBirds/Bird Data")]
public class BirdData : ScriptableObject
{
    [Header("Bird Info")]
    public string birdName = "Red Bird";
    [TextArea(2, 4)] public string description = "The standard bird.";

    [Header("Visuals & Animations")]
    public Sprite birdSprite;
    public RuntimeAnimatorController animatorController;

    [Header("Special Ability")]
    public BirdAbility ability = BirdAbility.None;

    [Tooltip("Used for Sonic Boom force, or Dash Speed, or Fall Speed")]
    public float abilityPower = 20f;

    [Tooltip("Used for Sonic Boom radius")]
    public float abilityRadius = 3f;

    [Tooltip("Drag your Sonic Boom prefab here for the Sonic Boom ability!")]
    public GameObject shockwavePrefab;


    [Tooltip("Drag your TNT prefab here for the TNT Delivery ability!")]
    public GameObject tntPrefab;
}