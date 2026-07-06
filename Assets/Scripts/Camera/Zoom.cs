using UnityEngine;
using Unity.Cinemachine;

public class Zoom : MonoBehaviour
{
    [Header("Cinematic Zoom Settings")]
    [SerializeField] private float idleZoom = 7f;
    [SerializeField] private float maxAimingZoom = 12f;
    [SerializeField] private float maxFlyingZoom = 14f;
    [SerializeField] private float zoomSpeed = 4f;

    [Header("Velocity Settings")]
    [SerializeField] private float minSpeedForZoom = 2f;
    [SerializeField] private float maxSpeedForZoom = 15f;

    [Header("Debug Tools")]
    //[SerializeField] private bool showZoomDebugLogs = true;

    private CinemachineCamera cineCam;
    private Rigidbody2D rb;
    private Movement movementScript;
    private float currentZoom;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<Movement>();

        GameObject camObj = GameObject.Find("CinemachineCamera");
        if (camObj != null)
        {
            cineCam = camObj.GetComponent<CinemachineCamera>();
            currentZoom = idleZoom;
        }
    }

    // THE FIX: Changed from LateUpdate to Update so Cinemachine doesn't ignore us!
    void Update()
    {
        if (cineCam == null) return;

        // 1. Calculate Drag Zoom
        float dragZoom = idleZoom;
        float stretch = 0f;
        if (movementScript != null)
        {
            stretch = movementScript.GetStretchRatio();
            dragZoom = Mathf.Lerp(idleZoom, maxAimingZoom, stretch);
        }

        // 2. Calculate Flight Zoom
        float flightZoom = idleZoom;
        float speed = 0f;
        if (rb != null)
        {
            speed = rb.linearVelocity.magnitude;
            float speedPercentage = Mathf.InverseLerp(minSpeedForZoom, maxSpeedForZoom, speed);
            flightZoom = Mathf.Lerp(idleZoom, maxFlyingZoom, speedPercentage);
        }

        // 3. Take the widest zoom requirement
        float targetZoom = Mathf.Max(dragZoom, flightZoom);

        // 4. Smoothly interpolate OUR variable
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomSpeed);

        // ====================================================
        // THE BRUTE FORCE FIX: Apply to EVERYTHING
        // ====================================================

        // A. Tell Cinemachine to update its Lens
        var lens = cineCam.Lens;
        lens.OrthographicSize = currentZoom;
        cineCam.Lens = lens;

        // B. Forcefully tell the Main Camera to update instantly 
        // (This completely bypasses any Cinemachine Brain glitches!)
        if (Camera.main != null)
        {
            Camera.main.orthographicSize = currentZoom;
        }

        //if (showZoomDebugLogs)
        //{
        //    Debug.Log($"ZOOM DETAILS | Speed: {speed:F1} | Stretch: {stretch:F2} | " +
        //              $"Target: {targetZoom:F2} | Actual Camera Size: {currentZoom:F2}");
        //}
    }
}