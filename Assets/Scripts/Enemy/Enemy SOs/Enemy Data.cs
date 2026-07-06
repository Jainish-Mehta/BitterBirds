using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "BitterBirds/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Info")]
    public string enemyName = "Basic Pig";
    [TextArea(2, 4)]
    public string description = "A low-level enemy that pops easily.";

    [Header("Stats")]
    public float maxHealth = 30f;
    public int pointValue = 500;

    [Header("Visuals")]
    public Sprite defaultSprite; 

    [Header("Audio")]
    public AudioClip gruntSound; 
    public AudioClip popSound;
    public AudioClip damagedSound;
}