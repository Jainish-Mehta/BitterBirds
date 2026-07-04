using UnityEngine;

public class DottedTrajectory : MonoBehaviour
{
    [Header("Trajectory Settings")]
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private int maxTrajectoryDots = 30;
    [SerializeField] private float dotSpacing = 0.05f;

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

        for (int i = 0; i < maxTrajectoryDots; i++)
        {
            if (i < activeDotsCount)
            {
                float time = i * dotSpacing;
                Vector3 dotPosition = startPosition + (velocity * time) + (0.5f * gravity * time * time);

                dotPosition.z = -2f;

                dotsList[i].transform.position = dotPosition;
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