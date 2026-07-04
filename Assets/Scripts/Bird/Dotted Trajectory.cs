using UnityEngine;

public class DottedTrajectory : MonoBehaviour
{
    [Header("Trajectory Settings")]
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private int maxTrajectoryDots = 30;
    [SerializeField] private float dotSpacing = 0.05f;
    [SerializeField] private int gapDots = 5; // NEW: How many dots to delete before the object!

    private GameObject[] dotsList;

    private void Start()
    {
        if (dotPrefab == null)
        {
            Debug.LogError("TRAJECTORY: You must assign the Dot Prefab in the Inspector!");
            return;
        }

        dotsList = new GameObject[maxTrajectoryDots];
        for (int i = 0; i < maxTrajectoryDots; i++)
        {
            dotsList[i] = Instantiate(dotPrefab);
            dotsList[i].SetActive(false);
        }
    }

    public void ShowTrajectory(Vector2 startPosition, Vector2 force, float mass, float gravityScale, float stretchRatio)
    {
        if (dotsList == null || dotsList.Length == 0) return;

        int activeDotsCount = Mathf.Max(3, Mathf.RoundToInt(stretchRatio * maxTrajectoryDots));

        Vector2 velocity = force / mass;
        Vector2 gravity = Physics2D.gravity * gravityScale;

        // Arrays to hold our math before we draw anything
        Vector3[] calculatedPoints = new Vector3[activeDotsCount];
        int hitIndex = activeDotsCount; // Default assumes we hit nothing
        Vector2 previousPosition = startPosition;

        // 1. Calculate the path and find exactly which dot hits the box
        for (int i = 0; i < activeDotsCount; i++)
        {
            float time = i * dotSpacing;
            Vector3 dotPosition = startPosition + (velocity * time) + (0.5f * gravity * time * time);
            dotPosition.z = -2f;
            calculatedPoints[i] = dotPosition;

            if (i > 0)
            {
                Vector2 dir = (Vector2)dotPosition - previousPosition;
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, dir.normalized, dir.magnitude);

                if (hit.collider != null && !hit.collider.isTrigger && hit.collider.gameObject.tag != "Player")
                {
                    hitIndex = i; // We found the exact dot that hit the crate!
                    break;
                }
            }
            previousPosition = dotPosition;
        }

        // 2. THE GAP FIX: Subtract our gapDots from the hit index!
        int drawUpToIndex = hitIndex;
        if (hitIndex < activeDotsCount) // If we actually hit a wall...
        {
            drawUpToIndex = Mathf.Max(0, hitIndex - gapDots); // Stop drawing 5 dots early!
        }

        // 3. Draw the dots up to our new cut-off limit
        for (int i = 0; i < maxTrajectoryDots; i++)
        {
            if (i < drawUpToIndex)
            {
                dotsList[i].transform.position = calculatedPoints[i];
                dotsList[i].SetActive(true);
            }
            else
            {
                dotsList[i].SetActive(false);
            }
        }
    }

    public void HideTrajectory()
    {
        if (dotsList == null) return;

        for (int i = 0; i < maxTrajectoryDots; i++)
        {
            dotsList[i].SetActive(false);
        }
    }
}