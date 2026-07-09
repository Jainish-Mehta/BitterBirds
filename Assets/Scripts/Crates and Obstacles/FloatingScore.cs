using UnityEngine;
using TMPro;

public class FloatingScore : MonoBehaviour
{
    public float floatSpeed = 1.0f;
    public float fadeSpeed = 1.0f;
    public float maxAbsorbTime = 1.0f; // Stops absorbing if no blocks break near it for 1 second

    private TextMeshPro textMesh;
    private Color textColor;

    // Tracking variables for Spatial Batching
    public int currentScoreValue { get; private set; }
    public bool isAbsorbing { get; private set; } = true;
    private float absorbTimer = 0f;

    public void Setup(int scoreValue, Color color)
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            currentScoreValue = scoreValue;
            textMesh.text = currentScoreValue.ToString();
            textMesh.color = color;
            textColor = color;
            textMesh.sortingOrder = 150;
        }
    }

    // --- NEW: Absorbs nearby points and resets its timer! ---
    public void AddPoints(int extraPoints, Color color)
    {
        currentScoreValue += extraPoints;
        if (textMesh != null)
        {
            textMesh.text = currentScoreValue.ToString();

            // If it absorbed a high-value enemy, change the text color to match the enemy!
            if (extraPoints >= 5000)
            {
                textColor = color;
                textMesh.color = color;
            }
        }

        // Reset the timer so it stays alive longer to absorb more!
        absorbTimer = 0f;

        // Add a satisfying "Pop" visual bounce!
        transform.localScale = Vector3.one * 1.5f;
    }

    private void Update()
    {
        // Smoothly shrink back to normal size after a Pop
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 5f);

        if (isAbsorbing)
        {
            absorbTimer += Time.deltaTime;
            // Float up very slowly while absorbing
            transform.position += new Vector3(0, (floatSpeed * 0.5f) * Time.deltaTime, 0);

            if (absorbTimer >= maxAbsorbTime)
            {
                isAbsorbing = false; // Lock it. It's time to fade out!
            }
        }
        else
        {
            // Float up faster and fade away
            transform.position += new Vector3(0, floatSpeed * Time.deltaTime, 0);

            if (textMesh != null)
            {
                textColor.a -= fadeSpeed * Time.deltaTime;
                textMesh.color = textColor;

                if (textColor.a <= 0f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}