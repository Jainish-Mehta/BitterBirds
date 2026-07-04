using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Movement : MonoBehaviour
{
    [Header("Gameplay Settings")]
    [SerializeField] private float maxDragDistance = 2f;
    [SerializeField] private float power = 5f;
    [SerializeField] private float birdRadius = 0.4f;
    [SerializeField] private float grabRadius = 1.5f;

    [Header("Reset Settings")]
    [SerializeField] private float maxFlightDuration = 9f;
    [SerializeField] private float resetDelayAfterCollision = 3f;

    private float currentFlightTime = 0f;
    private bool isDragging = false;
    private bool isFlying = false;
    private bool hasHitObstacle = false;
    private float defaultGravity;

    private Transform leftProng;
    private Transform rightProng;
    private Transform slingShotCenter;

    private LineRenderer leftLine;
    private LineRenderer rightLine;
    private Collider2D woodCollider;
    private Collider2D birdCollider;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private DottedTrajectory trajectoryScript;

    private PhysicsMaterial2D slipperyMaterial;
    private PhysicsMaterial2D normalMaterial;

    private Vector3 dragTargetPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        birdCollider = GetComponent<Collider2D>();
        trajectoryScript = GetComponent<DottedTrajectory>();

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        defaultGravity = rb.gravityScale;
        rb.bodyType = RigidbodyType2D.Kinematic;

        slipperyMaterial = new PhysicsMaterial2D("SlipperyCode");
        slipperyMaterial.friction = 0f;
        slipperyMaterial.bounciness = 0f;

        normalMaterial = new PhysicsMaterial2D("NormalCode");
        normalMaterial.friction = 0.8f;
        normalMaterial.bounciness = 0.0f;
        birdCollider.sharedMaterial = normalMaterial;

        GameObject centerObj = GameObject.Find("Center");
        if (centerObj != null) slingShotCenter = centerObj.transform;

        GameObject leftProngObj = GameObject.Find("Left Prong");
        if (leftProngObj != null) leftProng = leftProngObj.transform;

        GameObject rightProngObj = GameObject.Find("Right Prong");
        if (rightProngObj != null) rightProng = rightProngObj.transform;

        GameObject leftLineObj = GameObject.Find("Left Line");
        if (leftLineObj != null) leftLine = leftLineObj.GetComponent<LineRenderer>();

        GameObject rightLineObj = GameObject.Find("Right Line");
        if (rightLineObj != null) rightLine = rightLineObj.GetComponent<LineRenderer>();

        GameObject woodObj = GameObject.Find("SlingShot Anchor");
        if (woodObj != null) woodCollider = woodObj.GetComponent<Collider2D>();

        if (leftLine != null) leftLine.enabled = false;
        if (rightLine != null) rightLine.enabled = false;
        if (woodCollider != null) woodCollider.enabled = false;
    }

    private void Update()
    {
        HandleMouseAndVisuals();

        if (isFlying)
        {
            if (!hasHitObstacle && rb.linearVelocity.sqrMagnitude > 0.1f)
            {
                float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
                sr.flipY = (rb.linearVelocity.x < 0);
            }

            currentFlightTime += Time.deltaTime;
            if (currentFlightTime >= maxFlightDuration)
            {
                resetBird();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDragging)
        {
            rb.MovePosition(dragTargetPosition);
        }
    }

    private void HandleMouseAndVisuals()
    {
        if (Mouse.current == null || Camera.main == null) return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        float zDepth = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, zDepth));
        mouseWorldPosition.z = 0f;

        // ==========================================
        // MOUSE DOWN
        // ==========================================
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            float distanceToBird = Vector2.Distance(mouseWorldPosition, transform.position);

            if (distanceToBird <= grabRadius)
            {
                isDragging = true;
                hasHitObstacle = false;

                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 0f;

                birdCollider.sharedMaterial = slipperyMaterial;
                if (woodCollider != null)
                {
                    woodCollider.enabled = true;
                    woodCollider.sharedMaterial = slipperyMaterial;
                }

                dragTargetPosition = transform.position;
            }
        }

        // ==========================================
        // MOUSE UP
        // ==========================================
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (isDragging)
            {
                isDragging = false;

                if (leftLine != null) leftLine.enabled = false;
                if (rightLine != null) rightLine.enabled = false;
                if (woodCollider != null) woodCollider.enabled = false;

                birdCollider.sharedMaterial = normalMaterial;
                rb.linearVelocity = Vector2.zero;

                Vector2 shootDirection = slingShotCenter.position - transform.position;
                Vector2 appliedForce = shootDirection * power;

                rb.gravityScale = defaultGravity;
                rb.AddForce(appliedForce, ForceMode2D.Impulse);

                isFlying = true;
                currentFlightTime = 0f;

                if (trajectoryScript != null) trajectoryScript.HideTrajectory();
            }
        }

        // ==========================================
        // DRAGGING
        // ==========================================
        if (isDragging)
        {
            Vector3 pullDirection = (mouseWorldPosition - slingShotCenter.position);

            if (pullDirection.magnitude > maxDragDistance)
            {
                pullDirection = pullDirection.normalized * maxDragDistance;
            }

            dragTargetPosition = slingShotCenter.position + pullDirection;

            Vector2 lookDirection = slingShotCenter.position - transform.position;
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            sr.flipY = (lookDirection.x < 0);

            DrawRubberBands();

            // --- THE TRAJECTORY FIX ---
            if (trajectoryScript != null)
            {
                Vector2 shootDirection = (Vector2)slingShotCenter.position - (Vector2)transform.position;
                Vector2 shootForce = shootDirection * power;

                // Offset the start position of the dots to the very front of the bird's face!
                Vector2 facePosition = (Vector2)transform.position + (shootDirection.normalized * birdRadius);

                trajectoryScript.ShowTrajectory(facePosition, shootForce, rb.mass, defaultGravity, GetStretchRatio());
            }
        }
    }

    public float GetStretchRatio()
    {
        if (!isDragging || slingShotCenter == null || Mouse.current == null || Camera.main == null) return 0f;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        float zDepth = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, zDepth));
        mouseWorldPosition.z = 0f;

        Vector3 pullDirection = mouseWorldPosition - slingShotCenter.position;
        return Mathf.Clamp01(pullDirection.magnitude / maxDragDistance);
    }

    private void DrawRubberBands()
    {
        if (leftLine != null && rightLine != null && leftProng != null && rightProng != null)
        {
            leftLine.enabled = true;
            rightLine.enabled = true;

            Vector3 dirFromCenter = (transform.position - slingShotCenter.position).normalized;
            Vector3 bandAttachPoint = transform.position + (dirFromCenter * birdRadius);

            leftLine.SetPosition(0, leftProng.position);
            leftLine.SetPosition(1, bandAttachPoint);
            rightLine.SetPosition(0, rightProng.position);
            rightLine.SetPosition(1, bandAttachPoint);
        }
    }

    private void resetBird()
    {
        CancelInvoke(nameof(resetBird));

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.rotation = Quaternion.identity;
        sr.flipY = false;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = defaultGravity;
        birdCollider.sharedMaterial = normalMaterial;

        if (woodCollider != null) woodCollider.enabled = false;

        if (slingShotCenter != null)
        {
            transform.position = slingShotCenter.position;
        }

        isFlying = false;
        hasHitObstacle = false;
        currentFlightTime = 0f;

        if (trajectoryScript != null) trajectoryScript.HideTrajectory();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFlying)
        {
            hasHitObstacle = true;
            Invoke(nameof(resetBird), resetDelayAfterCollision);
            isFlying = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary Limits"))
        {
            resetBird();
        }
    }
}