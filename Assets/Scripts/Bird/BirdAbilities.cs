using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BirdAbilities : MonoBehaviour
{
    private BirdData birdData;
    private Rigidbody2D rb;
    private Collider2D birdCollider;
    private Movement movementScript;

    public bool hasUsedAbility = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        birdCollider = GetComponent<Collider2D>();
        movementScript = GetComponent<Movement>();
    }

    public void Initialize(BirdData data)
    {
        birdData = data;
        hasUsedAbility = false; // Reset just in case
    }

    public void TriggerAbility()
    {
        if (birdData == null || birdData.ability == BirdAbility.None || hasUsedAbility) return;

        hasUsedAbility = true; // Lock it so it only happens once!

        // Play the unique ability sound if assigned!
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBirdSound(AudioManager.Instance.snap);
        }

        switch (birdData.ability)
        {
            case BirdAbility.SonicBoom: TriggerSonicBoom(); break;
            case BirdAbility.StraightDash: TriggerStraightDash(); break;
            case BirdAbility.TNTDelivery: TriggerTNTDelivery(); break;
            case BirdAbility.Split: TriggerSplit(); break;
            case BirdAbility.HeavyFall: TriggerHeavyFall(); break;
        }
    }

    private void TriggerSonicBoom()
    {
        Debug.Log("<color=yellow>[SONIC BOOM]</color> Ability triggered!");

        if (birdData.shockwavePrefab != null)
        {
            GameObject shockwave = Instantiate(birdData.shockwavePrefab, transform.position, transform.rotation);
            Vector3 fixedPos = shockwave.transform.position;
            fixedPos.z = -5f;
            shockwave.transform.position = fixedPos;
        }


        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, birdData.abilityRadius);
        Vector2 birdMovementDir = rb.linearVelocity.normalized;
        int blocksHit = 0;

        foreach (Collider2D obj in objectsInRange)
        {
            if (obj.gameObject == this.gameObject) continue;

            Rigidbody2D objRb = obj.GetComponent<Rigidbody2D>();

            // --- THE FIX: We removed the check for Dynamic bodies because sleeping bodies might fail it! ---
            if (objRb != null)
            {
                // Force the block to wake up before we try to push it!
                objRb.WakeUp();

                Vector2 dirToObject = (obj.transform.position - transform.position).normalized;

                // Only blast things roughly in front of the bird
                if (Vector2.Dot(birdMovementDir, dirToObject) > -0.5f)
                {
                    dirToObject.y += 0.8f;
                    dirToObject = dirToObject.normalized;

                    float distance = Vector2.Distance(transform.position, obj.transform.position);
                    float force = 1f - (distance / birdData.abilityRadius);

                    objRb.AddForce(dirToObject * (birdData.abilityPower * 25f) * force, ForceMode2D.Impulse);
                    blocksHit++;
                }
            }
        }

        Debug.Log($"<color=cyan>[SONIC BOOM]</color> Blasted {blocksHit} objects with massive force!");
    }

    private void TriggerStraightDash()
    {
        rb.gravityScale = 0f;
        Vector2 currentDir = rb.linearVelocity.normalized;
        rb.linearVelocity = currentDir * birdData.abilityPower;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBirdSound(AudioManager.Instance.birdYell);
    }

    private void TriggerTNTDelivery()
    {
        if (birdData.tntPrefab != null)
        {
            Vector3 dropPos = transform.position + new Vector3(0, -0.8f, 0);
            GameObject tnt = Instantiate(birdData.tntPrefab, dropPos, Quaternion.identity);

            Collider2D tntCol = tnt.GetComponent<Collider2D>();
            if (birdCollider != null && tntCol != null)
            {
                Physics2D.IgnoreCollision(birdCollider, tntCol);
            }

            Rigidbody2D tntRb = tnt.GetComponent<Rigidbody2D>();
            if (tntRb != null) tntRb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.8f, 0f);

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 8f);
        }
    }

    private void TriggerSplit()
    {
        rb.linearVelocity *= 1.3f;
        SpawnSplitClone(1);
        SpawnSplitClone(-1);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBirdSound(AudioManager.Instance.birdYell);
    }

    private void SpawnSplitClone(int directionMultiplier)
    {
        Vector3 offset = new Vector3(0, directionMultiplier * 0.8f, 0);
        GameObject clone = Instantiate(this.gameObject, transform.position + offset, transform.rotation);

        // Setup Movement for clone
        Movement cloneScript = clone.GetComponent<Movement>();
        cloneScript.isClone = true;
        cloneScript.isFlying = true;
        cloneScript.enabled = true;

        // Setup Abilities for clone so it can't split again!
        BirdAbilities cloneAbilities = clone.GetComponent<BirdAbilities>();
        if (cloneAbilities != null) cloneAbilities.hasUsedAbility = true;

        Rigidbody2D cloneRb = clone.GetComponent<Rigidbody2D>();
        cloneRb.bodyType = RigidbodyType2D.Dynamic;

        // Grab the default gravity safely from the movement script
        cloneRb.gravityScale = movementScript.DefaultGravity;

        Vector2 newVelocity = rb.linearVelocity;
        newVelocity.y += directionMultiplier * 6f;
        cloneRb.linearVelocity = newVelocity;

        Collider2D cloneCol = clone.GetComponent<Collider2D>();
        if (birdCollider != null && cloneCol != null)
        {
            Physics2D.IgnoreCollision(birdCollider, cloneCol);
        }
    }

    private void TriggerHeavyFall()
    {
        rb.linearVelocity = new Vector2(0f, -birdData.abilityPower);
        rb.gravityScale = movementScript.DefaultGravity * 3f;
    }

    // ==========================================
    // DEBUG TOOLS
    // ==========================================
    private void OnDrawGizmosSelected()
    {
        // Draw a red wire sphere in the Scene View to show the exact size of the Sonic Boom blast!
        if (birdData != null && birdData.ability == BirdAbility.SonicBoom)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, birdData.abilityRadius);
        }
    }
}