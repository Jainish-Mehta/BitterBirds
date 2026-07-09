using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Movement))]
public class BirdTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    [Tooltip("Drag your DottedTrajectory Prefab here!")]
    public GameObject dotPrefab;

    [Tooltip("How much distance should the bird travel before dropping another dot?")]
    public float distanceBetweenDots = 0.5f;

    private Movement movementScript;
    private Transform slingshotCenter;
    private Vector2 lastDotPosition;

    private bool wasFlying = false;
    private bool hasPassedCenter = false;

    // STATIC: This list is shared across ALL birds! 
    // It remembers the dots from the previous bird so we can clean them up.
    private static List<GameObject> previousTrailDots = new List<GameObject>();

    private void Awake()
    {
        movementScript = GetComponent<Movement>();

        GameObject centerObj = GameObject.Find("Center");
        if (centerObj != null)
        {
            slingshotCenter = centerObj.transform;
        }
    }

    private void Update()
    {
        // 1. Detect the EXACT frame the bird launches
        if (movementScript.isFlying && !wasFlying)
        {
            wasFlying = true;
            hasPassedCenter = false;

            // Clear the old trail from the previous bird!
            ClearPreviousTrail();
        }

        if (movementScript.isFlying)
        {
            // --- THE FIX: WAIT UNTIL THE BIRD PASSES THE SLINGSHOT CENTER ---
            if (!hasPassedCenter && slingshotCenter != null)
            {
                // If the bird's X position has passed the Slingshot's X position...
                if (transform.position.x >= slingshotCenter.position.x)
                {
                    hasPassedCenter = true; // We crossed the line!
                    lastDotPosition = transform.position;
                    DropDot(); // Drop the very first dot right as it leaves the slingshot!
                }
            }
            else if (hasPassedCenter)
            {
                if (Vector2.Distance(transform.position, lastDotPosition) >= distanceBetweenDots)
                {
                    DropDot();
                }
            }
        }
    }

    private void DropDot()
    {
        if (dotPrefab != null)
        {
            // Spawn the dot at the bird's current position
            GameObject newDot = Instantiate(dotPrefab, transform.position, Quaternion.identity);

            // Push it slightly to the background so it doesn't draw over the bird
            Vector3 fixedPos = newDot.transform.position;
            fixedPos.z = 1f;
            newDot.transform.position = fixedPos;

            // Add it to the shared list so the next bird knows to delete it!
            previousTrailDots.Add(newDot);

            // Record this spot so we know when to drop the next one
            lastDotPosition = transform.position;
        }
    }

    public static void ClearPreviousTrail()
    {
        // Destroy every dot in the list
        foreach (GameObject dot in previousTrailDots)
        {
            if (dot != null)
            {
                Destroy(dot);
            }
        }

        // Empty the list
        previousTrailDots.Clear();
    }
}