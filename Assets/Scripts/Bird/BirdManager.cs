using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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

    public Transform slingshotCenter;
    private CinemachineCamera cineCam;

    private List<GameObject> activeBirds = new List<GameObject>();
    private int currentBirdIndex = 0;
    private int usedBirdsCount = 0;
    private GameObject currentActiveBird;

    private void Awake()
    {
        if (slingshotCenter == null)
        {
            GameObject centerObj = GameObject.Find("Center");
            if (centerObj != null) slingshotCenter = centerObj.transform;
        }

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

        UpdateWaitingLinePositions();
        LoadNextBird();
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            if (currentActiveBird != null)
            {
                Movement currentMove = currentActiveBird.GetComponent<Movement>();
                if (currentMove != null && (currentMove.isFlying || currentMove.isDragging)) return;
            }

            Vector2 screenPos = Pointer.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject clickedObj = hit.collider.gameObject;

                int clickedIndex = activeBirds.IndexOf(clickedObj);
                int activeIndex = currentBirdIndex - 1;

                if (clickedIndex != -1 && clickedIndex != activeIndex && clickedIndex >= activeIndex)
                {
                    SwapBirds(activeIndex, clickedIndex);
                }
            }
        }
    }

    private void SwapBirds(int activeIndex, int clickedIndex)
    {
        GameObject oldActive = activeBirds[activeIndex];
        GameObject newActive = activeBirds[clickedIndex];

        activeBirds[activeIndex] = newActive;
        activeBirds[clickedIndex] = oldActive;

        newActive.transform.position = slingshotCenter.position;
        UpdateWaitingLinePositions();

        oldActive.GetComponent<Movement>().enabled = false;
        newActive.GetComponent<Movement>().enabled = true;

        currentActiveBird = newActive;

        // --- THE FIX: SEND THE MULTIPLIER ON SWAP! ---
        if (GameManager.Instance != null)
        {
            BirdData data = currentActiveBird.GetComponent<Movement>().birdData;
            if (data != null) GameManager.Instance.SetCurrentBirdMultiplier(data.scoreMultiplier);
        }

        if (cineCam != null) cineCam.Follow = currentActiveBird.transform;

        if (AudioManager.Instance != null) AudioManager.Instance.PlayBirdSound(AudioManager.Instance.snap);
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

        // --- THE FIX: SEND THE MULTIPLIER ON LOAD! ---
        if (GameManager.Instance != null)
        {
            BirdData data = currentActiveBird.GetComponent<Movement>().birdData;
            if (data != null) GameManager.Instance.SetCurrentBirdMultiplier(data.scoreMultiplier);
        }

        if (cineCam != null) cineCam.Follow = currentActiveBird.transform;

        currentBirdIndex++;

        UpdateWaitingLinePositions();
    }

    private void UpdateWaitingLinePositions()
    {
        for (int i = currentBirdIndex; i < activeBirds.Count; i++)
        {
            GameObject waitingBird = activeBirds[i];
            int placeInLine = i - currentBirdIndex;
            int anchorIndex = Mathf.Min(placeInLine, waitPositions.Length - 1);
            Transform currentAnchor = waitPositions[anchorIndex];

            if (currentAnchor != null)
            {
                int offsetMultiplier = Mathf.Max(0, placeInLine - anchorIndex);
                Vector3 newPos = currentAnchor.position + new Vector3(-spacingDistance * offsetMultiplier, 0, 0);
                waitingBird.transform.position = newPos;
            }
        }
    }

    public void BirdFinishedTurn()
    {
        if (currentActiveBird != null) Destroy(currentActiveBird);
        LoadNextBird();
    }

    public void RecordBirdLaunch()
    {
        usedBirdsCount++;
    }

    public int GetRemainingBirdsCount()
    {
        int remaining = activeBirds.Count - usedBirdsCount;
        return Mathf.Max(0, remaining);
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