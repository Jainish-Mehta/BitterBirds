using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;

public class BirdManager : MonoBehaviour
{
    [Header("Level Settings")]
    public GameObject masterBirdPrefab;
    public BirdData[] birdsForThisLevel;

    [Header("Waiting Line Setup")]
    [Tooltip("Drop your Wait Anchor(s) here. Birds will automatically line up BEHIND them!")]
    public Transform[] waitPositions;
    [Tooltip("How far apart should the birds stand in the waiting line?")]
    public float spacingDistance = 1.5f;

    private Transform slingshotCenter;
    private CinemachineCamera cineCam;

    private List<GameObject> activeBirds = new List<GameObject>();
    private int currentBirdIndex = 0;
    private GameObject currentActiveBird;

    private void Awake()
    {
        GameObject centerObj = GameObject.Find("Center");
        if (centerObj != null) slingshotCenter = centerObj.transform;

        GameObject camObj = GameObject.Find("CinemachineCamera");
        if (camObj != null) cineCam = camObj.GetComponent<CinemachineCamera>();
    }

    private void Start()
    {
        if (slingshotCenter == null || masterBirdPrefab == null) return;
        if (waitPositions == null || waitPositions.Length == 0)
        {
            Debug.LogError("BirdManager: You must assign at least ONE Wait Position!");
            return;
        }

        // SPAWN THE BIRDS
        for (int i = 0; i < birdsForThisLevel.Length; i++)
        {
            if (birdsForThisLevel[i] != null)
            {
                GameObject newBird = Instantiate(masterBirdPrefab);

                Movement moveScript = newBird.GetComponent<Movement>();
                if (moveScript != null)
                {
                    moveScript.InitializeBird(birdsForThisLevel[i]);
                    moveScript.enabled = false;
                }

                activeBirds.Add(newBird);
            }
        }

        // Put them in their starting positions
        UpdateWaitingLinePositions();

        // Load the first bird!
        LoadNextBird();
    }

    public void LoadNextBird()
    {
        if (currentBirdIndex >= activeBirds.Count)
        {
            StartCoroutine(CheckForGameOver());
            return;
        }

        currentActiveBird = activeBirds[currentBirdIndex];

        currentActiveBird.transform.position = slingshotCenter.position;

        Movement moveScript = currentActiveBird.GetComponent<Movement>();
        if (moveScript != null) moveScript.enabled = true;

        if (cineCam != null) cineCam.Follow = currentActiveBird.transform;

        currentBirdIndex++;

        // Make all remaining birds slide forward!
        UpdateWaitingLinePositions();
    }

    // --- THE NEW SLIDING LOGIC ---
    private void UpdateWaitingLinePositions()
    {
        // Loop through ONLY the birds that are still waiting in line
        for (int i = currentBirdIndex; i < activeBirds.Count; i++)
        {
            GameObject waitingBird = activeBirds[i];

            // This calculates their "Place in Line" (0 is next, 1 is behind that, etc.)
            int placeInLine = i - currentBirdIndex;

            // Find which Anchor they belong to (if you have multiple anchors)
            int anchorIndex = Mathf.Min(placeInLine, waitPositions.Length - 1);
            Transform currentAnchor = waitPositions[anchorIndex];

            if (currentAnchor != null)
            {
                // If multiple birds share the same anchor, push them backwards by the spacing distance!
                // Example: If placeInLine is 3, but they are at the last anchor, they get pushed back 3 * 1.5 = 4.5 units!
                int offsetMultiplier = Mathf.Max(0, placeInLine - anchorIndex);

                Vector3 newPos = currentAnchor.position + new Vector3(-spacingDistance * offsetMultiplier, 0, 0);

                // Move the bird to its new spot!
                waitingBird.transform.position = newPos;
            }
        }
    }

    public void BirdFinishedTurn()
    {
        if (currentActiveBird != null)
        {
            Destroy(currentActiveBird);
        }

        LoadNextBird();
    }

    private IEnumerator CheckForGameOver()
    {
        yield return new WaitForSeconds(4f);

        if (GameManager.Instance != null && GameManager.Instance.GetTotalPigs() > 0)
        {
            if (!GameManager.Instance.isGameOver)
            {
                Debug.Log("OUT OF BIRDS! LEVEL FAILED.");
                GameManager.Instance.ShowLoseScreen();
            }
        }
    }
}