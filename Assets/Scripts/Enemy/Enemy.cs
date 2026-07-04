using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Data")]
    public EnemyData enemyData;

    [Header("Physics Tuning")]
    public float fragility = 3f;


    [SerializeField] private GameObject poof;

    private float currentHealth;

    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (enemyData != null)
        {
            currentHealth = enemyData.maxHealth;
            sr.sprite = enemyData.defaultSprite;
            gameObject.name = enemyData.enemyName;
        }
        else
        {
            Debug.LogError("Enemy is missing its EnemyData!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed > 1.5f)
        {
            float incomingMass = 1f;
            if(collision.rigidbody != null)
            {
                incomingMass = collision.rigidbody.mass;
            }
            float damageAmount = impactSpeed * fragility * fragility;
            TakeDamage(damageAmount);
        }
    }

    private void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // TODO: Add points to score manager

        Instantiate(poof, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
    }
}