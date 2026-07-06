    using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    [Header("Data")]
    public BlockData blockData;

    [Header("Physics Tuning")]
    public float fragility = 5f;

    private float currentHealth;
    private SpriteRenderer sr;
    private bool isDamaged = false;
    private bool isDead = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (blockData != null)
        {
            currentHealth = blockData.maxHealth;
            sr.sprite = blockData.fullHealthSprite;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance != null && !GameManager.Instance.hasBirdLaunched) return;
        if (GameManager.Instance != null && GameManager.Instance.isWinScreenVisible) return;
        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed > 0.5f)
        {
            if (AudioManager.Instance != null && blockData != null && blockData.impactSound != null)
            {
                // Calculate volume: speed of 10 gives max volume. Smaller speeds = quieter sounds!
                float dynamicVolume = Mathf.Clamp01(impactSpeed / 10f);
                AudioManager.Instance.PlaySound(blockData.impactSound, dynamicVolume);
            }
        }

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

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (blockData != null && currentHealth <= (blockData.maxHealth * 0.4f) && !isDamaged)
        {
            if (blockData.damagedSprite != null)
            {
                sr.sprite = blockData.damagedSprite;
                isDamaged = true;
            }
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        if (GameManager.Instance != null && blockData != null)
        {
            GameManager.Instance.AddScore(blockData.pointValue);
        }

        if (blockData != null && blockData.explosion != null)
        {
            GameObject effect = Instantiate(blockData.explosion, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // --- THE FIX: Add "0.4f" to play the break/explosion sound at 40% volume ---
        if (AudioManager.Instance != null && blockData != null && blockData.breakSound != null)
        {
            AudioManager.Instance.PlaySound(blockData.breakSound, 0.35f);
        }

        if (blockData != null && blockData.isTNT)
        {
            Explode();
        }

        gameObject.SetActive(false);
    }

    private void Explode()
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, blockData.explosionRadius);

        foreach (Collider2D obj in objectsInRange)
        {
            if (obj.gameObject == this.gameObject) continue; 

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (obj.transform.position - transform.position).normalized;
                float distance = Vector2.Distance(transform.position, obj.transform.position);
                float forceMultiplier = 1f - (distance / blockData.explosionRadius);

                rb.AddForce(direction * blockData.explosionForce * forceMultiplier, ForceMode2D.Impulse);
            }

            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.SendMessage("TakeDamage", blockData.explosionDamage, SendMessageOptions.DontRequireReceiver);
            }

            DestructibleBlock otherBlock = obj.GetComponent<DestructibleBlock>();
            if (otherBlock != null)
            {
                otherBlock.TakeDamage(blockData.explosionDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (blockData != null && blockData.isTNT)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, blockData.explosionRadius);
        }
    }
}