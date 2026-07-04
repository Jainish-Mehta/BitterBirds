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
        float impactSpeed = collision.relativeVelocity.magnitude;

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
        if (blockData != null && blockData.explosion != null)
        {
            // Create the explosion at this exact position
            GameObject effect = Instantiate(blockData.explosion, transform.position, Quaternion.identity);

            Destroy(effect, 2f);
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