using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Movement : MonoBehaviour
{
    [Header("Bird Data")]
    public BirdData birdData;

    [Header("Gameplay Settings")]
    [SerializeField] private float maxDragDistance = 2f;
    [SerializeField] private float power = 5f;
    [SerializeField] private float birdRadius = 0.4f;
    [SerializeField] private float grabRadius = 1.5f;

    [Header("Reset Settings")]
    [SerializeField] private float maxFlightDuration = 9f;
    [SerializeField] private float resetDelayAfterCollision = 3f;

    [Header("Camera Settings")]
    [SerializeField] private Vector2 slingshotScreenPos = new Vector2(-0.33f, 0.3f);
    [SerializeField] private Vector2 flyingScreenPos = new Vector2(0f, 0.2f);

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
    private Animator anim;
    private DottedTrajectory trajectoryScript;

    private PhysicsMaterial2D slipperyMaterial;
    private PhysicsMaterial2D normalMaterial;

    private Vector3 dragTargetPosition;

    private CinemachineCamera cineCam;
    private CinemachinePositionComposer camComposer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
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

        GameObject camObj = GameObject.Find("CinemachineCamera");
        if (camObj != null)
        {
            cineCam = camObj.GetComponent<CinemachineCamera>();
            if (cineCam != null) camComposer = cineCam.GetComponent<CinemachinePositionComposer>();
        }

        if (leftLine != null) leftLine.enabled = false;
        if (rightLine != null) rightLine.enabled = false;
        if (woodCollider != null) woodCollider.enabled = false;
    }

    // --- THE INJECTION METHOD ---
    public void InitializeBird(BirdData data)
    {
        birdData = data;

        if (birdData != null)
        {
            gameObject.name = birdData.birdName;

            if (birdData.birdSprite != null) sr.sprite = birdData.birdSprite;

            if (anim != null && birdData.animatorController != null)
            {
                anim.runtimeAnimatorController = birdData.animatorController;
            }
        }
    }

    private void Update()
    {
        HandleMouseAndVisuals();

        if (isFlying)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.UpdateFlightWind(rb.linearVelocity.magnitude);
            }

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

    private void LateUpdate()
    {
        HandleCamera();
    }

    private void HandleCamera()
    {
        if (cineCam == null || camComposer == null || slingShotCenter == null) return;

        if (isFlying || hasHitObstacle)
        {
            cineCam.Follow = this.transform;
            camComposer.Composition.ScreenPosition = Vector2.Lerp(
                camComposer.Composition.ScreenPosition,
                flyingScreenPos,
                Time.deltaTime * 4f
            );
        }
        else
        {
            cineCam.Follow = slingShotCenter;
            camComposer.Composition.ScreenPosition = Vector2.Lerp(
                camComposer.Composition.ScreenPosition,
                slingshotScreenPos,
                Time.deltaTime * 4f
            );
        }
    }

    private void HandleMouseAndVisuals()
    {
        if (Pointer.current == null || Camera.main == null) return;

        Vector2 pointerScreenPosition = Pointer.current.position.ReadValue();
        float zDepth = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 pointerWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(pointerScreenPosition.x, pointerScreenPosition.y, zDepth));
        pointerWorldPosition.z = 0f;

        if (Pointer.current.press.wasPressedThisFrame)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

            float distanceToBird = Vector2.Distance(pointerWorldPosition, transform.position);

            if (distanceToBird <= grabRadius)
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlayStretch();

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

        if (Pointer.current.press.wasReleasedThisFrame)
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

                if (GameManager.Instance != null) GameManager.Instance.hasBirdLaunched = true;
                if (trajectoryScript != null) trajectoryScript.HideTrajectory();

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.StopStretch();
                    AudioManager.Instance.PlayBirdSound(AudioManager.Instance.snap);
                    AudioManager.Instance.PlayBirdSound(AudioManager.Instance.birdYell);
                }
            }
        }

        if (isDragging)
        {
            Vector3 pullDirection = (pointerWorldPosition - slingShotCenter.position);

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

            if (trajectoryScript != null)
            {
                Vector2 shootDirection = (Vector2)slingShotCenter.position - (Vector2)transform.position;
                Vector2 shootForce = shootDirection * power;
                Vector2 facePosition = (Vector2)transform.position + (shootDirection.normalized * birdRadius);

                trajectoryScript.ShowTrajectory(facePosition, shootForce, rb.mass, defaultGravity, GetStretchRatio());
            }
        }
    }

    public float GetStretchRatio()
    {
        if (!isDragging || slingShotCenter == null || Pointer.current == null || Camera.main == null) return 0f;

        Vector2 pointerScreenPosition = Pointer.current.position.ReadValue();
        float zDepth = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 pointerWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(pointerScreenPosition.x, pointerScreenPosition.y, zDepth));
        pointerWorldPosition.z = 0f;

        Vector3 pullDirection = pointerWorldPosition - slingShotCenter.position;
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

        isFlying = false;
        hasHitObstacle = false;

        if (trajectoryScript != null) trajectoryScript.HideTrajectory();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateFlightWind(0f);
        }

        BirdManager manager = Object.FindAnyObjectByType<BirdManager>();

        if (manager != null)
        {
            manager.BirdFinishedTurn();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFlying)
        {
            hasHitObstacle = true;
            Invoke(nameof(resetBird), resetDelayAfterCollision);
            isFlying = false;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.UpdateFlightWind(0f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary Limits"))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.UpdateFlightWind(0f);
            }

            resetBird();
        }
    }
}