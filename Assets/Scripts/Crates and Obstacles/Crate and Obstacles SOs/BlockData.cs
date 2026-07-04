using UnityEngine;

[CreateAssetMenu(fileName = "New Block Data", menuName = "BitterBirds/Block Data")]
public class BlockData : ScriptableObject
{
    [Header("Block Info")]
    public string blockName = "New Block";
    public string blockDescription = "Description of the block goes here.";

    [Header("Effects")]
    public GameObject explosion; // Drag your explosion prefab here!

    [Header("Block Stats")]
    public float maxHealth = 50f;

    [Header("Visuals")]
    public Sprite fullHealthSprite; 
    public Sprite damagedSprite;

    [Header("TNT Settings")]
    public bool isTNT = false;           
    public float explosionRadius = 3f;   
    public float explosionForce = 50f;   
    public float explosionDamage = 100f;
}