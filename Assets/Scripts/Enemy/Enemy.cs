using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Data")]
    public EnemyData enemyData;

    [Header("Physics Tuning")]
    public float fragility = 3f;


    [SerializeField] private GameObject poof;
    private bool isDead = false;

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
        if (GameManager.Instance != null && GameManager.Instance.isWinScreenVisible) return;

        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed > 1.0f)
        {
            if (AudioManager.Instance != null && enemyData != null && enemyData.gruntSound != null)
            {
                // Play specific pig grunt from EnemyData at 40% volume
                AudioManager.Instance.PlaySound(enemyData.damagedSound, 0.4f);
            }
        }

        // 2. DEAL DAMAGE
        if (impactSpeed > 1.5f)
        {
            float incomingMass = 1f;
            if (collision.rigidbody != null)
            {
                incomingMass = collision.rigidbody.mass;
            }

            float damageAmount = impactSpeed * incomingMass * fragility;
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
        if (isDead) return;
        isDead = true;

        Instantiate(poof, transform.position, Quaternion.identity);

        if (AudioManager.Instance != null && enemyData != null && enemyData.popSound != null)
        {
            AudioManager.Instance.PlaySound(enemyData.popSound, 0.4f);
        }

        if (GameManager.Instance != null && enemyData != null)
        {
            GameManager.Instance.AddScore(enemyData.pointValue);

            // --- NEW: Pass this specific GameObject's name to the GameManager ---
            GameManager.Instance.PigDestroyed(gameObject.name);
        }

        Destroy(gameObject);
    }
}